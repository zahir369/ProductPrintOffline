using System.Collections.Generic;

namespace MKSS.Model
{
    public class ErrorEntity: MessageEntity
    {  
        public string Message { get; set; } 
    }
    public class MessageEntity
    {
        public int Floor { get; set; }
        public string RegionName { get; set; } 
    }
    public class StatusEntity
    {
        public long F_BatchId { get; set; }
        public int FullQueryTimes { get; set; }
        public LaHuaTaskStatus TaskStatus { get;   set; }
        public LaoHuaTaskOperate TaskOperate { get;   set; }
        public string Message { get;   set; }
        public Dictionary<int, bool> FloorsVisible { get; set; }
        public override string ToString()
        {
            return string.Format("Status={0},Operate={1}：{2}", TaskStatus, TaskOperate, Message);
        }
    }
    public enum MqttTaskCommand
    {
        QueryRealtimeData, StartTask, StopTask, PauseTask, ResumeTask, StatusTask, SetVisible, SetHidden
    }


    public enum LaHuaTaskStatus
    {
        Starting, Running, Paused, Stoped
    }
    public enum LaoHuaTaskOperate
    {
        Start, Stop, Pause, Resume
    }

}