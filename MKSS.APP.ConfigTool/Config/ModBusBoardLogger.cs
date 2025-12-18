using NModbus;
using System;
using NModbus.Logging;
using System.Text;
using System.Windows.Forms;

namespace MKSS.APP.ConfigTool
{
    public class ModBusBoardLogger : ModbusLogger
	{
		private const int LevelColumnSize = 15;
		private static readonly string BlankHeader = Environment.NewLine + new string(' ', LevelColumnSize);
        TextBox TextBox;
        MainConfig  MainConfig;
        public ModBusBoardLogger(LoggingLevel minimumLoggingLevel  ,TextBox t, MainConfig config)
			: base(minimumLoggingLevel)
		{
            TextBox = t; MainConfig = config;

        }
         
		protected override void LogCore(LoggingLevel level, string message)
		{

            message = message?.Replace(Environment.NewLine, BlankHeader);
            if (MainConfig.ShowDebugComm) {
                TextBox.BeginInvoke(new EventHandler(delegate {
                    TextBox.AppendText(System.Environment.NewLine);
                    TextBox.AppendText(message);
                }));
            }
           
            
             
        }
         

    }
}
