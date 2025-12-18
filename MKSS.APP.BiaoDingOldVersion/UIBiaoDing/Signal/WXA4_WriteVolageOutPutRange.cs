using DeviceDataMonitorWPF.UIBiaoDing.Signal;
using System;

namespace DeviceDataMonitorWPF.UIBiaoDing.Signal
{
	public class WXA4_WriteVolageOutPutRange : ISignal
	{
		public ushort LowVoltage
		{
			get;
			set;
		}

		public ushort HightVoltage
		{
			get;
			set;
		}

		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = 164;
			arr[1] = this.LowVoltage;
			arr[2] = this.HightVoltage;
			return arr;
		}
	}

	public class WriteCommandModel_21 : ISignal
	{
		public byte[] ADDRESS
		{
			get;
			set;
		}
		 
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = 33;
			if (ADDRESS != null) {
                for (int i = 0; i < ADDRESS.Length; i++)
                {
					if(i<5) arr[1+i] = ADDRESS[i];
				}
			} 
			return arr;
		}
	}


}
