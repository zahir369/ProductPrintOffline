using System;
using Stylet;

namespace Test.Pages
{
    public class ShellViewModel : Screen
    {
        public string Name { get; set; } = "waku";  // C#6的语法, 声明自动属性并赋值
        public void SayHello() => Name = "Hello " + Name;    // C#6的语法, 表达式方法
    }
}
