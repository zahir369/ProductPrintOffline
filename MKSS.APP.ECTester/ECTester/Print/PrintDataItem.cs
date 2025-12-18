using MKSS.Service.ECTester;

namespace MKSS.APP.ECTester.Print
{
    public class PrintDataItem
    {
        public int REGION_ROW { 
            get {

                //根据模式切换布局
                if (ECTesterService.IndustryMode)
                {
                    //工业传感器 8 * 8， 4 * *
                    switch (REGION)
                    {
                        case "A":
                            return ROW + 0;
                        case "B":
                            return ROW + 0;
                        case "C":
                            return ROW + 4;
                        case "D":
                            return ROW + 4;
                        default:
                            break;
                    }
                }
                else
                {
                    //水性传感器 16 * 4， 8 * 2
                    int r_base = (REGION_INDEX - 1)/8 +1;
                    switch (REGION)
                    {
                        case "A":
                            return r_base + 0;
                        case "B":
                            return r_base + 2;
                        case "C":
                            return r_base + 4;
                        case "D":
                            return r_base + 6;
                        default:
                            break;
                    }
                }

                return ROW;
            } 
        }
        public int REGION_COL
        {
            get
            {
                //根据模式切换布局
                if (ECTesterService.IndustryMode)
                {
                    //工业传感器 8 * 8， 4 * *
                    switch (REGION)
                    {
                        case "A":
                            return COL + 0;
                        case "B":
                            return COL + 4;
                        case "C":
                            return COL + 0;
                        case "D":
                            return COL + 4;
                        default:
                            break;
                    }
                }
                else
                {
                    //水性传感器 16 * 4， 8 * 2
                    return REGION_INDEX >8 ? REGION_INDEX-8: REGION_INDEX;
                }
                return ROW;
            }
        }
        public string REGION { get; set; }
        public int REGION_INDEX { get; set; }
        public int ROW { get { return (REGION_INDEX-1) / 4 + 1; } }
        public int COL { get { return REGION_INDEX % 4==0?4: REGION_INDEX % 4; } }
        public string CONTENT { get; set; }
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", ROW, COL, REGION, REGION_INDEX);
        }
    }
}
