using MKSS.Model.BusModel;
using MKSS.Util;
using System;
using System.IO.Ports;
using System.Linq;

namespace MKSS.SerialPortUtil
{
    public class SerialPortHelper
    {
        public static SerialPortModel InitPort()
        {
            var sp = Appsettings.App<SerialPortModel>("ZhanHuiComs");
            return sp.FirstOrDefault();
        }



    }
}
