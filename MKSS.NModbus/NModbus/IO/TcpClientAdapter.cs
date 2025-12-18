using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using NModbus.Unme.Common;

namespace NModbus.IO
{
    /// <summary>
    ///     Concrete Implementor - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    public class TcpClientAdapterxx : IStreamResource
    {
        private TcpClient _tcpClientx;
        private NetworkStream _stream;
        byte[] _readBuffer = new byte[1024];
        List<byte> _readBufferList = new List<byte>();
        public TcpClientAdapterxx(TcpClient tcpClient)
        {
            Debug.Assert(tcpClient != null, "Argument tcpClient cannot be null.");
            _tcpClientx = tcpClient;
            _stream = _tcpClientx.GetStream();
            try
            {
                _stream.BeginRead(_readBuffer, 0, _readBuffer.Length, new AsyncCallback(ReadCallback), _stream); //异步接受服务器回报的字符串
            }
            catch(Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        ///   Asynchronous read callback operation.
        /// </summary>
        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                int numBytesRead = this._stream.EndRead(ar);
                if (numBytesRead > 0)
                {
                    // Create byte array to hold newly received bytes.
                    byte[] rcvdBytes = new byte[numBytesRead];
                    Buffer.BlockCopy(this._readBuffer, 0, rcvdBytes, 0, numBytesRead);
                    _readBufferList.AddRange(rcvdBytes);
                }
                else
                {
                    // Do stuff.
                }
            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }

        public int InfiniteTimeout => Timeout.Infinite;
         
        public int ReadTimeout
        {
            get => _stream.ReadTimeout;
            set => _stream.ReadTimeout = value;
        }

        public int WriteTimeout
        {
            get => _stream.WriteTimeout;
            set => _stream.WriteTimeout = value;
        }

        public void Write(byte[] buffer, int offset, int size)
        {
            _stream.Write(buffer, offset, size);
        }

        public int Read(byte[] buffer, int offset, int size)
        {
            //byte[] buffer1 = _readBufferList.ToArray();
            //Buffer.BlockCopy(buffer1, 0, buffer, offset, size);
            return size;
        }

        public int Read(Span<byte> buffer) {
            return 0;
        }
        public void DiscardInBuffer()
        {
            _readBufferList.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposableUtility.Dispose(ref _tcpClientx);
            }
        }
    }


    /// <summary>
    ///     Concrete Implementor - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    public class TcpClientAdapter : IStreamResource
    {
        private TcpClient _tcpClient;
        public TcpClientAdapter(TcpClient tcpClient)
        {
            Debug.Assert(tcpClient != null, "Argument tcpClient cannot be null.");
            _tcpClient = tcpClient;
        }

        public int InfiniteTimeout => Timeout.Infinite;

        public int ReadTimeout
        {
            get => _tcpClient.GetStream().ReadTimeout;
            set => _tcpClient.GetStream().ReadTimeout = value;
        }

        public int WriteTimeout
        {
            get => _tcpClient.GetStream().WriteTimeout;
            set => _tcpClient.GetStream().WriteTimeout = value;
        }

        public void Write(byte[] buffer, int offset, int size)
        {
            _tcpClient.GetStream().Write(buffer, offset, size);
        }

        public int Read(byte[] buffer, int offset, int size)
        {
            return _tcpClient.GetStream().Read(buffer, offset, size);
        }

        public int Read(Span<byte> buffer)
        {
            return _tcpClient.GetStream().Read(buffer);
        }

        byte[] temp = new byte[1024];
        public void DiscardInBuffer()
        {
            try
            {
                
                NetworkStream s = _tcpClient.GetStream();
                while (s.DataAvailable) {
                    s.Read(temp,0, temp.Length);
                }
            }
            catch (Exception)
            {

            }

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposableUtility.Dispose(ref _tcpClient);
            }
        }
    }



}
