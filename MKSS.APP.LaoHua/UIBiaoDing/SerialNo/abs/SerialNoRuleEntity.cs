namespace MKSS.Service.UIBiaoDing
{

    /// <summary>
    ///  编号属性
    /// </summary>
    public abstract class SerialNoRuleEntity
    {
        public int OrderID { get; set; }
        public string OrderNumber { get; set; }
        public string OrderNumberPrefix { get; set; }
        public string ProductFullName { get; set; }
        public string ProductCode { get; set; }
        public string ProductType { get; set; }
        public string Department { get; set; }
        public string SalesPersonName { get; set; }
        public int Qty { get; set; }
        public int ToQty { get; set; }
        public string Date { get; set; }
        public string ToDate { get; set; }
        public string Comment { get; set; }
        public string CommentExt { get; set; }
        public SerialNoRule SerialNoRule { get; internal set; }
        public SerialNoNet SerialNoNet { get; internal set; }
    }

}
 
