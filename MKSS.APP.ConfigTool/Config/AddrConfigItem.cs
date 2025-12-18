namespace MKSS.APP.ConfigTool
{
    public class AddrConfigItem
	{ 
		public string Name { get; set; }
		public ushort Value { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

}
