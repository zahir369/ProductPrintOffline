using System;

namespace DeviceDataMonitorWPF.UIBiaoDing.Signal
{
	public interface ISignal
	{
		ushort[] GetSignalBytes();
	}
}
