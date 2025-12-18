using MKSS.APP.UIBiaoDing.Signal;
using System;

namespace MKSS.APP.UIBiaoDing.Signal
{

	/**
		0x94-写入串号
		举例：串号，990036990666 =》HEX E6 82 CD 9A CA
		99 表示年份，00369表示任务单流水号，90666表示单内流水号
		步骤1：←◆00 10 00 00 00 05 0A 00 FF 00 00 00 00 00 00 00 00 15 9C 	（固定格式直接发送）
		步骤2：←◆00 10 00 00 00 05 0A 00 94 00 0C 00 E6 82 CD 9A CA BB 73 
		//************************************************************************************
		校准传感器写入串号解析
		地址                      00		//00代表广播命令，所有地址都执行
		功能码 						10		//
		寄存器地址开始高低位 	00	00
		寄存器长度高低位		00	05		//
		数据长度					0A
		数据1                 00	94		//工作模式	94
		数据2				  00	0C      // 0C对应位置12的传感器
		数据3                 00	E6      //一共六个字节组成串号 990036990666 
		数据4                 82 	CD      //一共六个字节组成串号 990036990666 
		数据5                 9A    CA       //一共六个字节组成串号 990036990666 
		CRC校验 BB  73
		步骤3：←◆ 00 10 00 00 00 05 0A 00 01 00 00 00 00 00 00 00 00 5D F8 	（固定格式直接发送）
		数据2、数据3 是零点值，由人工在上位机输入，将步骤2数据打包完成，通过串口发送，将步骤1、2、3数次发送给工装从机，串口每发送一次，下位机将进行一次标定操作
	**/
	public class WX_WriteSerialNo : ISignal
	{
		/// <summary>
		///  命令词
		/// </summary>
		public byte Signal { get { return SignalUtil.Confuse(0X94); } }

		/// <summary>
		///  位置号， 1 - 15
		/// </summary>
		public int Postion 
		{
			get;
			set;
		}

		/// <summary>
		///  串号
		///  举例：串号，990036990666 =》HEX E6 82 CD 9A CA
		///  99 表示年份，00369表示任务单流水号，90666表示单内流水号
		/// </summary>
		public ulong SerialNo
		{
			get;
			set;
		}

		//0x94-写入串号
		public ushort[] GetSignalBytes()
		{
			if (SerialNo > 999999999999 || SerialNo<= 100000000000) {
				throw new Exception("串号【"+ SerialNo + "】必须小于999999999999，大于 100000000000");
			}
			if (Postion > 15 || Postion < 1)
			{
				throw new Exception("所在位置【" + Postion + "】必须小于16，大于0");
			}
			byte[] bn = BitConverter.GetBytes(SerialNo);
			ushort[] arr = new ushort[5];
			arr[0] = BitConverter.ToUInt16(new byte[] { (byte)Signal, (byte)0X00 }, 0);
			arr[1] = (ushort)Postion;
			arr[2] = BitConverter.ToUInt16(new byte[] { bn[4], bn[5] }, 0);
			arr[3] = BitConverter.ToUInt16(new byte[] { bn[2], bn[3] }, 0);
			arr[4] = BitConverter.ToUInt16(new byte[] { bn[0], bn[1] }, 0);
			return arr;
		}

	}

}
