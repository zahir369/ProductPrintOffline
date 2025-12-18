using MKSS.Model;
using System.Collections.Generic;
using System.Threading;

namespace MKSS.Service.LaoHua
{


	//public class TaskInfo: TaskInfoBase
	//{
	//	public long BatchId { get; set; }
	//	public long BoardId { get; set; }
	//	public long DataBusId { get; set; }
	//	public DataBus DataBus { get; set; }
	//	public Batch Batch { get; set; }
	//	public Board Board { get; set; }
	//}

	//public class TaskResult : TaskInfoBase
	//{
	//	public long DataBusId { get; set; }
	//	public List<dynamic> Data { get; set; }
	//}

	public class TaskAddress
	{
		public byte Address { get; set; }
		public AddressInfoBase TaskInfo { get; set; }
	}

	public class AddressInfoBase {
		public TaskInfoEnum TaskType { get; set; }
		public string[] Taskparams { get; set; }
	}


	public enum TaskInfoEnum
	{
		ReadAll = 100, ReadAllNoThread = 201, ReadAutoAdjustStatus = 202,
		ReadLiangCheng = 203, ReadOutPutVolage = 204, ReadSerialNo = 205,
		ReadVoltageLoop = 206, ReadVoltageOnce = 207, ReadVoltageRange = 208,
		SetAutoAdjust = 209, SetLedStatus = 210, SetLiangCheng = 211,
		SetModeQuery = 212, SetSpan = 213, SetTemperature = 214,
		SetVolageOutPutRange = 215, SetZero = 216
	}

	 
}
