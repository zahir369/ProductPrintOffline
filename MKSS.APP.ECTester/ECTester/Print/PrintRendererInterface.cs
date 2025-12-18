using System.Windows.Documents;

namespace MKSS.APP.ECTester.Print
{
    public interface PrintRendererInterface {
        FlowDocument LoadDocument(PrintDataList data);
    }
}
