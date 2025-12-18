using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.IO;
using LeaRun.Loger;

namespace MKSS.Util.Log.License
{

    /// <summary> 
    /// 计算机信息类
    /// </summary> 
    [XmlType("R")]
    public class ComputerInfo
    {

        [XmlElement("M1")]
        public ListString CPU_ID { get; set; }
        [XmlElement("M2")]
        public ListString MAC_ADDRESS { get; set; }
        [XmlElement("M3")]
        public ListString DISK_ID { get; set; }
        [XmlElement("M4")]
        public ListString MAINBOARD_ID { get; set; }
        [XmlElement("M5")]
        public ListString VIDEO_ID { get; set; }
        [XmlElement("M6")]
        public ListString BIOS_ID { get; set; }
        [XmlElement("M7")]
        public ListString PHYSICAL_MEMORY_ID { get; set; }
        [XmlElement("M8")]
        public ListString PRODUCTS { get; set; }
        [XmlElement("M9")]
        public ListString START_TIME { get; set; } 

        static ComputerInfo _instance;

        [XmlIgnore]
        public static ComputerInfo Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ComputerInfo();
                    _instance.InitByLocal();
                }
                return _instance;
            }
        }

        /// <summary>
        ///  入口 
        /// </summary>
        public ComputerInfo(){
            
        }

        /// <summary>
        ///  使用本机信息初始化
        /// </summary>
        void InitByLocal()
        {
            CPU_ID = GetInfo("Win32_Processor",null, "ProcessorId");
            MAC_ADDRESS = GetInfo("Win32_NetworkAdapterConfiguration", "IPEnabled", "MacAddress");
            DISK_ID = GetInfo("Win32_DiskDrive", null, "SerialNumber");
            MAINBOARD_ID = GetInfo("Win32_BaseBoard", null, "SerialNumber");
            BIOS_ID = GetInfo("Win32_BIOS", null, "SerialNumber");
            PHYSICAL_MEMORY_ID = GetInfo("Win32_PhysicalMemory", null, "SerialNumber");
            VIDEO_ID = GetInfo("Win32_VideoController", null, "Caption");
            START_TIME = GetDateNow();
            PRODUCTS = new ListString();
            PRODUCTS.Add(AssemblyTag.AssemblyProduct);
        }

        /// <summary>
        ///  是否匹配
        /// </summary>
        /// <param name="jm">加密算法，建议使用 SHA512 ，不建议使用 MD5 和 SHA1</param>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public bool Match(HashAlgorithm jm, string path,ref string msg )
        {

            ComputerInfo ret = new ComputerInfo();
            if (File.Exists(path))
            {
                XmlSerializer ser = new XmlSerializer(typeof(ComputerInfo));
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    ret = (ComputerInfo)ser.Deserialize(stream);
                }
            }
            else {
                msg = "文件不存在";
                return false;
            }

            int totlal = 0;
            int matched = 0;
            StringBuilder s = new StringBuilder();
            foreach (System.Reflection.PropertyInfo p in this.GetType().GetProperties())
            {
                if (p.CanRead && p.CanWrite && p.PropertyType.FullName != this.GetType().FullName)
                {
                    if (p.PropertyType.FullName == typeof(ListString).FullName)
                    {
                        ListString beforeEnCrypt = p.GetValue(this, null) as ListString;
                        ListString afterEnCrypt = p.GetValue(ret, null) as ListString;
                        bool match = beforeEnCrypt.Match(jm,afterEnCrypt);
                        if (match)
                        {
                            s.Append("找到：" + p.Name + "；");
                            matched++;
                        }
                        else {
                            s.Append("丢失：" + p.Name + "；");
                        }
                        totlal++;
                    }
                }
            }
            s.Append("共：" + matched+"/"+ totlal + "；");
            msg = s.ToString();
            return matched>=4;
        }

        /// <summary>
        ///  加密硬件信息
        ///  建议使用 SHA512 ，不建议使用 MD5 和 SHA1
        /// </summary>
        /// <param name="jm">加密算法，建议使用 SHA512 ，不建议使用 MD5 和 SHA1</param>
        /// <param name="path">文件输出路径</param>
        /// <returns></returns>
        public void SrcTo(string path)
        {
            if (File.Exists(path)) File.Delete(path);
            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                XmlSerializer sz = new XmlSerializer(typeof(ComputerInfo));
                sz.Serialize(stream, this);
            }
        }

        /// <summary>
        ///  加密硬件信息
        ///  建议使用 SHA512 ，不建议使用 MD5 和 SHA1
        /// </summary>
        /// <param name="jm">加密算法，建议使用 SHA512 ，不建议使用 MD5 和 SHA1</param>
        /// <param name="path">文件输出路径</param>
        /// <returns></returns>
        public ComputerInfo EncryptTo(HashAlgorithm jm, string path)
        {
            ComputerInfo ret = new ComputerInfo(); 
            foreach (System.Reflection.PropertyInfo p in this.GetType().GetProperties())
            {
                if (p.CanRead && p.CanWrite && p.PropertyType.FullName != this.GetType().FullName)
                {
                    if (p.PropertyType.FullName == typeof(ListString).FullName) {
                        ListString beforeEnCrypt = p.GetValue(this, null) as ListString;
                        ListString list = beforeEnCrypt.Encrypt(jm);
                        p.SetValue(ret, list, null);
                    }
                }
            }
            if (File.Exists(path)) File.Delete(path);
            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                XmlSerializer sz = new XmlSerializer(typeof(ComputerInfo));
                sz.Serialize(stream, ret);
            }
            return ret;
        }

        /// <summary>
        ///  给客户机器授权 
        /// </summary>
        /// <param name="src_path">客户机器授权申请文件</param>
        /// <param name="path">授权文件</param>
        /// <returns></returns>
        public ComputerInfo LicenseTo(HashAlgorithm jm, string src_path, string path)
        {

            ComputerInfo ret = null;
            if (File.Exists(src_path))
            {
                XmlSerializer ser = new XmlSerializer(typeof(ComputerInfo));
                using (FileStream stream = new FileStream(src_path, FileMode.Open, FileAccess.Read))
                {
                    ret = (ComputerInfo)ser.Deserialize(stream);
                }

                foreach (System.Reflection.PropertyInfo p in this.GetType().GetProperties())
                {
                    if (p.CanRead && p.CanWrite && p.PropertyType.FullName != this.GetType().FullName)
                    {
                        if (p.PropertyType.FullName == typeof(ListString).FullName)
                        {
                            ListString beforeEnCrypt = p.GetValue(this, null) as ListString;
                            ListString list = beforeEnCrypt.Encrypt(jm);
                            p.SetValue(ret, list, null);
                        }
                    }
                }
                if (File.Exists(path)) File.Delete(path);
                using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    XmlSerializer sz = new XmlSerializer(typeof(ComputerInfo));
                    sz.Serialize(stream, ret);
                }

            } 
            return ret;

        }

        /// <summary>
        ///  仅作测试
        /// </summary>
        public string TestInfo {
            get {
                StringBuilder s = new StringBuilder();
                string[] arr = new string[] { 
                    "Win32_VideoController","Win32_VideoSettings" }; 
                try
                {
                    foreach (string type in arr)
                    {
                        ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From " + type);
                        foreach (ManagementObject mo in searcher.Get())
                        {
                            s.Append("============" + type + "开始＝＝＝＝＝＝ ");
                            s.Append(System.Environment.NewLine);
                            foreach (PropertyData pd in mo.Properties)
                            {
                                s.Append(pd.Name + " :: ");
                                if (pd.Value != null)
                                {
                                    s.Append(pd.Value.ToString());
                                }
                                s.Append(System.Environment.NewLine);
                            }
                            s.Append("============" + type + "结束＝＝＝＝＝＝ ");
                            s.Append(System.Environment.NewLine);
                            s.Append(System.Environment.NewLine);
                            s.Append(System.Environment.NewLine); 
                        }
                    }
                }
                catch
                {

                }
                finally
                {
                }

                return s.ToString();
            }
        }

        /// <summary>
        ///  读取 硬件信息
        /// </summary>
        /// <param name="type">硬件类型关键字</param>
        /// <param name="enabled">是否启用关键字</param>
        /// <param name="ids">各种项关键字</param>
        /// <returns>硬件信息列表</returns>
        ListString GetInfo(string type,string enabled, params string[] ids)
        {

            ListString cpuInfo = new ListString();//cpu序列号 
            try
            {

                //获取CPU序列号代码 
                ManagementClass mc = new ManagementClass(type);
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (string id in ids)
                {
                    foreach (ManagementObject mo in moc)
                    {
                        if (!string.IsNullOrEmpty(enabled))
                        {
                            if ((bool)mo[enabled] == false) continue;
                        }
                        cpuInfo.Add(mo.Properties[id].Value.ToString());
                    }
                }
                moc = null;
                mc = null;
                return cpuInfo;

            }
            catch(Exception ex)
            {
                APP.LicenseTools.FormMain.Instance.Log(ex.Message);
                APP.LicenseTools.FormMain.Instance.Log(ex.StackTrace);
                return cpuInfo;
            }
            finally
            {
            }

        }


        /// <summary>
        ///  读取 硬件信息
        /// </summary>
        /// <param name="type">硬件类型关键字</param>
        /// <param name="enabled">是否启用关键字</param>
        /// <param name="ids">各种项关键字</param>
        /// <returns>硬件信息列表</returns>
        ListString GetDateNow()
        {
            ListString cpuInfo = new ListString();//cpu序列号 
            try
            {
                cpuInfo.Add(DateTime.Now.Ticks+"");
                return cpuInfo;
            }
            catch
            {
                return cpuInfo;
            }
            finally
            {
            }

        }


        /// <summary>
        ///  内容
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            foreach (System.Reflection.PropertyInfo p in this.GetType().GetProperties())
            {

                if (p.CanRead && p.CanWrite && p.PropertyType.FullName != this.GetType().FullName)
                {
                    s.Append(p.Name);
                    s.Append(":\t\t\t");
                    s.Append(p.GetValue(this, null));
                    s.Append(System.Environment.NewLine);
                }
            }
            return s.ToString();
        }

    }

    public class ListString : List<string>
    {

        /// <summary>
        ///  加密 List 内容
        /// </summary>
        /// <param name="jm"></param>
        /// <returns></returns>
        public ListString Encrypt(HashAlgorithm jm) {
            ListString ret = new ListString();
            foreach (string s in this)
            {
                byte[] source = Encoding.Default.GetBytes("songguanjun encrypt:" + s);
                byte[] crypto = jm.ComputeHash(source);
                string result = Convert.ToBase64String(crypto);//把加密後的字串從Byte[]轉為字串
                ret.Add(result);
            }
            return ret;
        }

        /// <summary>
        ///  是否匹配加密
        /// </summary>
        /// <param name="jm"></param>
        /// <param name="encrypted">加密后内容</param>
        /// <returns></returns>
        public bool Match(HashAlgorithm jm,ListString encrypted)
        { 
            ListString ret = Encrypt(jm);
            foreach (string s0 in ret) {
                foreach (string s1 in encrypted)
                {
                    if (s0 == s1) return true;
                }
            } 
            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in this) {
                sb.Append(s);
                sb.Append(";");
            }
            return sb.ToString();
        }
    }
    
}