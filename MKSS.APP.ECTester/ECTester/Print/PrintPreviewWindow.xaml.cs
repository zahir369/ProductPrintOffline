using System;
using System.IO;
using System.IO.Packaging;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace MKSS.APP.ECTester.Print
{
    public partial class PrintPreviewWindow : Window
    {
        private delegate void LoadXpsMethod();
        private readonly Object m_data;
        private readonly FlowDocument m_doc;
        public static FlowDocument LoadDocumentAndRender(PrintDataList data, PrintRendererInterface renderer )
        {
            FlowDocument doc = renderer.LoadDocument(data); 
            return doc;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.docViewer.Zoom = 100;
        }
        public PrintPreviewWindow( PrintDataList data, PrintRendererInterface renderer )
        {
            InitializeComponent();
            m_data = data;
            m_doc = renderer.LoadDocument(data);
            Dispatcher.BeginInvoke(new LoadXpsMethod(LoadXps), DispatcherPriority.ApplicationIdle);
        }

        public void LoadXps()
        {
            //构造一个基于内存的xps document
            MemoryStream ms = new MemoryStream();
            Package package = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite);
            Uri DocumentUri = new Uri("pack://InMemoryDocument.xps");
            PackageStore.RemovePackage(DocumentUri);
            PackageStore.AddPackage(DocumentUri, package);
            XpsDocument xpsDocument = new XpsDocument(package, CompressionOption.Fast, DocumentUri.AbsoluteUri);

            //将flow document写入基于内存的xps document中去
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
            writer.Write(((IDocumentPaginatorSource)m_doc).DocumentPaginator);


            //获取这个基于内存的xps document的fixed document
            docViewer.Document = xpsDocument.GetFixedDocumentSequence();  
            docViewer.Document.DocumentPaginator.PageSize = new Size(4.00 / 25.4 * 96, 1.50 / 25.4 * 96);
             

            //关闭基于内存的xps document
            xpsDocument.Close();

            //https://docs.microsoft.com/zh-cn/dotnet/api/system.windows.controls.documentviewer?view=net-5.0
            this.docViewer.FitToMaxPagesAcross(1);

        }

        public void LoadXps11()
        {
            //构造一个基于内存的xps document
            MemoryStream ms = new MemoryStream();
            Package package = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite);
            Uri DocumentUri = new Uri("pack://InMemoryDocument.xps");
            PackageStore.RemovePackage(DocumentUri);
            PackageStore.AddPackage(DocumentUri, package);
            XpsDocument xpsDocument = new XpsDocument(package, CompressionOption.Fast, DocumentUri.AbsoluteUri);


            //将flow document写入基于内存的xps document中去
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

            PrintTicket ticket = new PrintTicket();
            ticket.PageMediaSize = new PageMediaSize(PageMediaSizeName.Unknown, 4.00 / 25.4 * 96, 1.50 / 25.4 * 96);
            writer.WritingPrintTicketRequired += (s, e) =>
            {
                e.CurrentPrintTicket = ticket;
            };
            writer.Write(((IDocumentPaginatorSource)m_doc).DocumentPaginator);

            //获取这个基于内存的xps document的fixed document
            docViewer.Document = xpsDocument.GetFixedDocumentSequence();

            //关闭基于内存的xps document
            xpsDocument.Close();
            //docViewer.Document.DocumentPaginator.PageSize = new Size(4.00 / 25.4 * 96, 1.50 / 25.4 * 96);
            //https://docs.microsoft.com/zh-cn/dotnet/api/system.windows.controls.documentviewer?view=net-5.0
            this.docViewer.FitToMaxPagesAcross(2);

            // Create a PrintDialog 
            PrintDialog printDlg = new PrintDialog();
            printDlg.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;
            printDlg.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.Unknown, 4.00 / 25.4 * 96, 1.50 / 25.4 * 96);
            printDlg.PrintDocument(((IDocumentPaginatorSource)m_doc).DocumentPaginator, "sum");

            //PrintDialog printDlg = new PrintDialog();
            //LocalPrintServer ps = new LocalPrintServer();
            //PrintQueue pq = ps.DefaultPrintQueue;

            //PrintTicket pt = pq.UserPrintTicket;

            //pt.PageOrientation = PageOrientation.Landscape;

            //FlowDocument doc = CreateFlowDocumentSum();


            //doc.PageHeight = 768;
            //doc.PageWidth = 1104;

            //PageMediaSize pageMediaSize = new PageMediaSize(doc.PageWidth, doc.PageHeight);
            //pt.PageMediaSize = pageMediaSize;
            //IDocumentPaginatorSource source = doc as IDocumentPaginatorSource;


            //printDlg.PrintDocument(source.DocumentPaginator, "sum");


        }

    }
}
