using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test.Pages
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();
            long l = 00361230999999999;
            byte[] bs = BitConverter.GetBytes(l);
            byte[] bs1 = ByteHelper.Compress(bs);

            

        }

        ushort[] ToUshort(DateTime dt,int sequence)
        {
            string d = DateTime.Now.ToString("yyyyMMddHHmmss");
            byte[] dbs = new byte[] {
                (byte)int.Parse(d.Substring(0,2),NumberStyles.HexNumber),
                (byte)int.Parse(d.Substring(2,2),NumberStyles.HexNumber),
                (byte)int.Parse(d.Substring(4,2),NumberStyles.HexNumber),
                (byte)int.Parse(d.Substring(6,2),NumberStyles.HexNumber),
                (byte)int.Parse(d.Substring(8,2),NumberStyles.HexNumber),
                (byte)int.Parse(d.Substring(10,2),NumberStyles.HexNumber),
                (byte)int.Parse(d.Substring(12,2),NumberStyles.HexNumber)
            };
            return new ushort[]
            {
                171,
                BitConverter.ToUInt16(dbs,0),
                BitConverter.ToUInt16(dbs,2),
                BitConverter.ToUInt16(dbs,4),
                BitConverter.ToUInt16(dbs,6),

            };
            //return BitConverter.ToUInt16(new byte[] { }, 0);
        }

    }

    public class ByteHelper
    {
        public const ushort COMPRESSION_FORMAT_LZNT1 = 2;
        public const ushort COMPRESSION_ENGINE_MAXIMUM = 0x100;

        [DllImport("ntdll.dll")]
        public static extern uint RtlGetCompressionWorkSpaceSize(ushort dCompressionFormat, out uint dNeededBufferSize, out uint dUnknown);

        [DllImport("ntdll.dll")]
        public static extern uint RtlCompressBuffer(ushort dCompressionFormat, byte[] dSourceBuffer, int dSourceBufferLength, byte[] dDestinationBuffer,
        int dDestinationBufferLength, uint dUnknown, out int dDestinationSize, IntPtr dWorkspaceBuffer);

        [DllImport("ntdll.dll")]
        public static extern uint RtlDecompressBuffer(ushort dCompressionFormat, byte[] dDestinationBuffer, int dDestinationBufferLength, byte[] dSourceBuffer, int dSourceBufferLength, out uint dDestinationSize);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr LocalAlloc(int uFlags, IntPtr sizetdwBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalFree(IntPtr hMem);


        public static byte[] Decompress(byte[] buffer)
        {
            var outBuf = new byte[buffer.Length * 6];
            uint dwSize = 0, dwRet = 0;
            uint ret = RtlGetCompressionWorkSpaceSize(COMPRESSION_FORMAT_LZNT1, out dwSize, out dwRet);
            if (ret != 0) return null;

            ret = RtlDecompressBuffer(COMPRESSION_FORMAT_LZNT1, outBuf, outBuf.Length, buffer, buffer.Length, out dwRet);
            if (ret != 0) return null;

            Array.Resize(ref outBuf, (Int32)dwRet);
            return outBuf;
        }


        public static byte[] Compress(byte[] buffer)
        {
            var outBuf = new byte[buffer.Length * 6];
            uint dwSize = 0, dwRet = 0;
            uint ret = RtlGetCompressionWorkSpaceSize(COMPRESSION_FORMAT_LZNT1 | COMPRESSION_ENGINE_MAXIMUM, out dwSize, out dwRet);
            if (ret != 0) return null;

            int dstSize = 0;
            IntPtr hWork = LocalAlloc(0, new IntPtr(dwSize));
            ret = RtlCompressBuffer(COMPRESSION_FORMAT_LZNT1 | COMPRESSION_ENGINE_MAXIMUM, buffer, buffer.Length, outBuf, outBuf.Length, 0, out dstSize, hWork);
            if (ret != 0) return null;

            LocalFree(hWork);

            Array.Resize(ref outBuf, dstSize);
            return outBuf;
        }
    } 

}
