using MKSS.APP.UIBiaoDing.Signal;
using System;

namespace MKSS.APP.UIBiaoDing.Signal
{
	public class WX_WriteAdjustZeroAppend : ISignal
	{
		/// <summary>
		///  √¸¡Ó¥ 
		/// </summary>
		public byte Signal { get { return SignalUtil.Confuse(0X87); } }
		public int Zero
		{
			get;
			set;
		}

		public int DifferenceValue
		{
			get;
			set;
		}

		public int TemperaturePoint
		{
			get;
			set;
		}

		public ushort[] GetSignalBytes()
		{
			return new ushort[]
			{
				(ushort)(int)Signal,
				(ushort)(this.Zero / 256),
				(ushort)(this.Zero % 256),
				170,
				(ushort)this.TemperaturePoint,
				(ushort)this.DifferenceValue
			};
		}
	}
}
