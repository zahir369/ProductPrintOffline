using MKSS.Util.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Sockets;

namespace MKSS.Service.LaoHua
{

    /// <summary>
    ///  连接池，避免资源未彻底释放等
    /// </summary>
	[LogTagClass(Title = "连接池")]
    public class PooledConnection
    {
        /// <summary>
        ///  池
        /// </summary>
        static PooledConnection pool = null;
        /// <summary>
        ///  池
        /// </summary>
        public static PooledConnection Pool { get { if (pool == null) pool = new PooledConnection(); return pool; } }

        ConcurrentDictionary<string, SerialPort> CacheSerialPort = new ConcurrentDictionary<string, SerialPort>();
        ConcurrentDictionary<IPAddr, TcpClient> CacheTcpClient = new ConcurrentDictionary<IPAddr, TcpClient>();
        /// <summary>
        ///  串口连接
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public SerialPort Of(string portName)
        {
            lock (CacheSerialPort)
            {
                if (!CacheSerialPort.ContainsKey(portName))
                {
                    ULogger.Info(portName + " 初始连接");
                    SerialPort p = new SerialPort() { PortName = portName };
                    p.Disposed += P_Disposed;
                    CacheSerialPort.TryAdd(portName, p);
                }
            }
            SerialPort p1 = CacheSerialPort[portName];
            return p1;
        }

        /// <summary>
        ///  释放时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void P_Disposed(object sender, System.EventArgs e)
        {
            SerialPort p = sender as SerialPort;
            if (p != null)
            {
                ULogger.Info(p.PortName + " 被释放");
                if (CacheSerialPort.ContainsKey(p.PortName))
                {
                    SerialPort r = null;
                    CacheSerialPort.TryRemove(p.PortName,out r);
                }
            }
        }

        /// <summary>
        ///  串口连接
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public bool Open(string portName)
        {
            SerialPort p = Of(portName);
            try
            {
                p.Open();
                bool sucess = p.IsOpen;
                ULogger.Info(portName + " 连接" + (sucess ? "已打开" : "打开失败"));
                return p.IsOpen;
            }
            catch (ObjectDisposedException DisposedException)
            {
                ULogger.Info(portName + " 莫名被释放:" + DisposedException.Message);


                SerialPort r = null;
                CacheSerialPort.TryRemove(portName, out r); 
                try
                {
                    p.Dispose();
                }
                catch (Exception)
                {
                }
                p = null;
                GC.Collect();

                p = Of(portName);
                try
                {
                    p.Open();
                    bool sucess = p.IsOpen;
                    ULogger.Info(portName + " 连接" + (sucess ? "已打开" : "打开失败"));
                    return p.IsOpen;
                }
                catch (System.Exception ex)
                {
                    ULogger.Info(portName + " 连接重试出错:" + ex.Message);
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                ULogger.Info(portName + " 连接出错:" + ex.Message);
                return p.IsOpen;
            }
        }

        /// <summary>
        ///  串口连接
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public bool Close(string portName)
        {
            SerialPort p = Of(portName);
            try
            {
                p.Close();

                bool sucess = p.IsOpen;
                ULogger.Info(portName + " 连接" + (sucess ? "关闭失败" : "已关闭"));
            }
            catch (System.Exception ex)
            {
                ULogger.Info(portName + " 断开出错:" + ex.Message);
            }
            p = Of(portName);
            return !p.IsOpen;
        }

         
        /// <summary>
        ///  串口连接
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public TcpClient Of(IPAddr ip)
        {
            lock (CacheTcpClient)
            {
                if (!CacheTcpClient.ContainsKey(ip))
                {
                    ULogger.Info(ip + " 初始连接");
                    TcpClient p = new TcpClient( );
                    CacheTcpClient.TryAdd(ip, p);
                }
            }
            TcpClient p1 = CacheTcpClient[ip];
            return p1;
        }

        /// <summary>
        ///  网口连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Open(IPAddr ip)
        {
            TcpClient p = Of(ip);
            try
            {
                p.Connect(ip.ip, ip.port);
                bool sucess = p.Connected;
                ULogger.Info(ip + " 连接" + (sucess ? "已打开" : "打开失败"));
                return p.Connected;
            }
            catch (ObjectDisposedException DisposedException)
            {
                ULogger.Info(ip + " 莫名被释放:" + DisposedException.Message);

                TcpClient r = null;
                CacheTcpClient.TryRemove(ip, out r); 
                try
                {
                    p.Dispose();
                }
                catch (Exception)
                {

                }
                p = null;
                GC.Collect();

                p = Of(ip);
                try
                {
                    p.Connect(ip.ip, ip.port);
                    bool sucess = p.Connected;
                    ULogger.Info(ip + " 连接重试" + (sucess ? "已打开" : "打开失败"));
                    return p.Connected;
                }
                catch (System.Exception ex)
                {
                    ULogger.Info(ip + " 连接重试出错:" + ex.Message);
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                ULogger.Info(ip + " 连接出错:" + ex.Message);
                return false;
            }
        }


        /// <summary>
        ///  串口连接
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public bool Close(IPAddr ip)
        {
            TcpClient p = Of(ip);
            try
            {
                p.Close();
                bool sucess = p.Client==null || !p.Connected;
                ULogger.Info(ip + " 关闭" + (sucess ? "成功" : "已关闭"));
                if (sucess)
                {
                    TcpClient r = null;
                    CacheTcpClient.TryRemove(ip, out r);
                    ULogger.Info(ip + " 释放成功");
                }
                return sucess;
            }
            catch (System.Exception ex)
            {
                ULogger.Info(ip + " 断开出错:" + ex.Message);
            }
            return  false;
        }


    }

    public struct IPAddr
    {
        public IPAddr(string i, int o)
        {
            ip = i;
            port = o;
        }
        public string ip { get; set; }
        public int port { get; set; }
        public override string ToString()
        {
            return string.Format("{0}_{1}", ip, port);
        }
    }

}
