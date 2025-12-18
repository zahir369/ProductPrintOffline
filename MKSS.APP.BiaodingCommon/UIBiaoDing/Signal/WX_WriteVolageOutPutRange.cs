using MKSS.APP.UIBiaoDing.Signal;
using System;

namespace MKSS.APP.UIBiaoDing.Signal
{
	public class WX_WriteVolageOutPutRange : ISignal
	{

		/// <summary>
		///  √¸¡Ó¥ 
		/// </summary>
		public byte Signal { get { return SignalUtil.Confuse(0XA4); } }
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
			arr[0] = (ushort)(int)Signal;
			arr[1] = this.LowVoltage;
			arr[2] = this.HightVoltage;
			return arr;
		}
	}

	public class WX21_XX : ISignal
	{
		/// <summary>
		///  √¸¡Ó¥ 
		/// </summary>
		public byte Signal { get { return 0X21; } }
		public byte[] ADDRESS
		{
			get;
			set;
		}
		 
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = (ushort)(int)Signal;
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
