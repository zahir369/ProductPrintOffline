using MKSS.APP.UIBiaoDing.Signal;

namespace MKSS.APP.UIBiaoDing.Signal
{
    public class WX_WriteLedStatus : ISignal
	{

		/// <summary>
		///  √¸¡Ó¥ 
		/// </summary>
		public byte Signal { get { return SignalUtil.Confuse(0XA8); } }
		public ushort[] LedStatus
		{
			get;
			set;
		}

		public ushort[] GetSignalBytes()
		{
			return new ushort[]
			{
				(ushort)(int)Signal,
				this.LedStatus[0],
				this.LedStatus[1],
				this.LedStatus[2],
				this.LedStatus[3]
			};
		}
	}

}
