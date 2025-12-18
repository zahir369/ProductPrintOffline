using System;
using System.Xml.Serialization;

namespace MKSS.Model
{
    public class BatchStaRange
    {
        [XmlIgnore()]
        public Batch Batch { get; set; }
        [XmlIgnore()]
        public Sensor Sensor { get; set; }
        [XmlIgnore()]
        public DateTime TypeFrom { get { return Batch.F_AgingLastUpdateTime - Type; } }
        [XmlIgnore()]
        public DateTime TypeTo { get { return Batch.F_AgingLastUpdateTime; } }

        public TimeSpan Type { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }

        public BatchSensorData Value { get; set; }

        public BatchStaRange()
        {
            Value = new BatchSensorData();
        }
         
        public BatchStaRange(Batch p, Sensor s)
        {
            Batch = p; Sensor = s;
            Value = new BatchSensorData();
        }


        public override string ToString()
        {
            return ToBdString();
        }

        public string ToBdString()
        {
            int val = (Min + Max) / 2;
            int valbd = (Max - Min) / 2;
            return string.Format("{0}±{1}", val, Math.Abs(valbd));
        }

    }

}