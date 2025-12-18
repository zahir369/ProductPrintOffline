namespace MKSS.APP.UIBiaoDing.Util
{
    public class DialogViewModel
    {
        public DialogViewModel(string v1, string v2, bool v3)
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
        }

        public string V1 { get; }
        public string V2 { get; }
        public bool V3 { get; }
    }
}