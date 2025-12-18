namespace MKSS.Service.UIBiaoDing
{

    /// <summary>
    ///  编号解析规则
    /// </summary>
    public abstract class SerialNoParser {
		public string StrValue { get; set; }
		public virtual string Prefix { get; }
		public virtual ulong GetNext(ulong seed)
		{
			return seed;
		}
		public virtual ulong GetMin()
		{
			return ulong.MinValue;
		}
		public virtual ulong GetMax()
		{
			return ulong.MaxValue;
		}
	}


}
 
