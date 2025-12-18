using NModbus;
using System;
using NModbus.Logging;
using System.Text;
using MKSS.Util.Log;

namespace MKSS.Service.ElectroChemical
{


    [LogTagClass(Title = "底层通讯")]
    public class ModBusBoardLogger : ModbusLogger
	{
		private const int LevelColumnSize = 15;
		private static readonly string BlankHeader = Environment.NewLine + new string(' ', LevelColumnSize);
        public ModBusBoardLogger(LoggingLevel minimumLoggingLevel  )
			: base(minimumLoggingLevel)
		{ 
        }
         
		protected override void LogCore(LoggingLevel level, string message)
		{

            message = message?.Replace(Environment.NewLine, BlankHeader);
            ULogger.Info(message); 
             
        }
         

    }
}
