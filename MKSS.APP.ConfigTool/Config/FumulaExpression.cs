using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Reflection;

namespace MKSS.APP.ConfigTool
{
    public class FumulaExpression
    {
        object instance;
        MethodInfo method;
        /// <summary>
        /// 表达试运算
        /// </summary>
        /// <param name="expression">表达试</param>
        public FumulaExpression(string expression)
        {
            if (expression.IndexOf("return") < 0) expression = "return " + expression + ";";
            string className = "Expression";
            string methodName = "Compute";
            CompilerParameters p = new CompilerParameters();
            p.GenerateInMemory = true;
            CompilerResults cr = new CSharpCodeProvider().CompileAssemblyFromSource(p, string.
              Format("using System;sealed class {0}{{public double {1}(double x){{{2}}}}}",
              className, methodName, expression));
            if (cr.Errors.Count > 0)
            {
                string msg = "Expression(\"" + expression + "\"): \n";
                foreach (CompilerError err in cr.Errors)
                { msg += err.ToString() + "\n"; }
                throw new Exception(msg);
            }
            instance = cr.CompiledAssembly.CreateInstance(className);
            method = instance.GetType().GetMethod(methodName);
        }
        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="x"></param>
        /// <returns>返回计算值</returns>
        public double Compute(double x)
        {
            return (double)method.Invoke(instance, new object[] { x });
        }

        static void Test( )
        {
            FumulaExpression e = new FumulaExpression("x+5*9/3.2+6");
            Console.WriteLine(e.Compute(10));
            Console.Read();
        }
    }
     

}
