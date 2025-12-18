using MKSS.Model;
using System.Collections.Generic;
using System.Threading;

namespace MKSS.Service.LaoHua
{


	public class TaskInfo: TaskInfoBase
	{
		public long DataBusId { get; set; }
		public DataBus DataBus { get; set; }
		public Batch Batch { get; set; }
		public Board Board { get; set; }
		public BoardCase BoardCase { get; set; }
		public Dictionary<string,Sensor> SensorDictionary { get; set; }
        public override string ToString()
        {
            return string.Format("{0}{1}", BoardCase==null?"#": BoardCase.F_BoardCaseAddress+"#", Board==null?"": Board.F_FloorNO);
        }
    }

	public class TaskResult : TaskInfoBase
	{
		public long DataBusId { get; set; }
		public List<dynamic> Data { get; set; }
	}

	public class TaskAddress
	{
		public byte Address { get; set; }
		public TaskInfo TaskInfo { get; set; }
	}

	public class TaskInfoBase {
		public long BatchId { get; set; }
		public long BoardId { get; set; }
		public TaskInfoEnum TaskType { get; set; }
		public string[] Taskparams { get; set; }
	}


	public class SerialNoByConnectionEntity
	{
		public TaskAddress Address { get; set; }
		public int Position { get; set; }
		public ulong SerialNo { get; set; }
	}

	public enum TaskInfoEnum
	{
		ReadAll = 100, ReadAllNoThread = 201, ReadAutoAdjustStatus = 202,
		ReadLiangCheng = 203, ReadOutPutVolage = 204, ReadSerialNo = 205, ReadDeviceDateTime = 2051, ReadManufacturingDate = 2051,
		ReadVoltageLoop = 206, ReadVoltageOnce = 207, ReadVoltageRange = 208,
		SetAutoAdjust = 209, SetLedStatus = 210, SetLiangCheng = 211,
		SetModeQuery = 212, SetSpan = 213, SetTemperature = 214,
		SetVolageOutPutRange = 215, SetZero = 216, SetSerialNo = 217, SetDeviceDateTime = 218, SetManufacturingDate = 219,
	}

	 
}
