namespace NModbus.Extensions
{
    using System;
    using System.Text;
    using Logging;

    internal static class ModbusFactoryExtensions
    {
        private const int MinRequestFrameLength = 3;

        public static IModbusMessage CreateModbusRequest(this IModbusFactory factory, byte[] frame)
        {
            if (frame.Length < MinRequestFrameLength)
            {
                string msg = $"Argument 'frame' must have a length of at least {MinRequestFrameLength} bytes.";
                throw new FormatException(msg);
            }

            byte functionCode = frame[1];

            var functionService = factory.GetFunctionService(functionCode);

            return functionService.CreateRequest(frame);
        }

        public static IModbusFunctionService GetFunctionServiceOrThrow(this IModbusFactory factory, byte functionCode,byte[] frame)
        {
            IModbusFunctionService service = factory.GetFunctionService(functionCode);

            if (service == null)
            {

                StringBuilder strBuider = new StringBuilder();
                for (int index = 0; index < frame.Length; index++)
                {
                    strBuider.Append(((int)frame[index]).ToString("X2"));
                } 
                string msg = $"Function code {functionCode} not supported：RX: {strBuider}";
                factory.Logger.Warning(msg);
                throw new NotImplementedException(msg);
            }

            return service;
        }
    }
}