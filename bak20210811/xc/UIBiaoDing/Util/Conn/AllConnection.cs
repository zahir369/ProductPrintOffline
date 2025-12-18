using DeviceDataMonitorWPF.UIBiaoDing;
using DeviceDataMonitorWPF.UIBiaoDing.Config;
using DeviceDataMonitorWPF.UIBiaoDing.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MKSS.Service.LaoHua
{

    /// <summary>
    ///  连接池
    /// </summary>
    public class AllConnection : Dictionary<string, BoardConnection> {

        public AllConnection() {
            this.Qualified = new DataQualified();
        }
        public void Push(BoardConnection conn) {
            if (!this.ContainsKey(conn.ID))
            {
                this.Add(conn.ID, conn);
            }
            else {
                this[conn.ID] = conn;
            }
        }

        public DataQualified Qualified
        {
            get;
            set;
        }

        public bool IsOpen
        {
            get {
                return this.Count(w => w.Value.IsOpen) > 0;
            }
        }


        public void ReadParameterAll()
        {
            int a1 = 1;
            int a2 = 1;
            int a3 = 1;
            int a4 = 1;
            int a5 = 1;
            int a6 = 1;
            int t = 0;
            int totla = this.Values.Count;
            foreach (var conn in this.Values.ToList())
            {
                Task.Run(delegate ()
                {
                    try
                    {

                        APP.UserControls.WaitWindow.ShowWindow("正在读取", "正在读取串号[" + a1 + "/" + totla + "]......", DeviceDataMonitorWPF.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        conn.CommFactory.ReadSerialNo(conn.Address).Wait(); a1++;
                        APP.UserControls.WaitWindow.ShowWindow("正在读取", "正在读取设备时间[" + a2 + "/" + totla + "]......", DeviceDataMonitorWPF.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        conn.CommFactory.ReadDeviceDateTime(conn.Address).Wait(); a2++;
                        APP.UserControls.WaitWindow.ShowWindow("正在读取", "正在读取量程[" + a3 + "/" + totla + "]......", DeviceDataMonitorWPF.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        conn.CommFactory.ReadLiangCheng(conn.Address).Wait(); a3++;
                        APP.UserControls.WaitWindow.ShowWindow("正在读取", "正在读取输出电压[" + a4 + "/" + totla + "]......", DeviceDataMonitorWPF.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        conn.CommFactory.ReadOutPutVolage(conn.Address).Wait(); a4++;
                        APP.UserControls.WaitWindow.ShowWindow("正在读取", "正在读取电压范围[" + a5 + "/" + totla + "]......", DeviceDataMonitorWPF.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        conn.CommFactory.ReadVoltageRange(conn.Address).Wait(); a5++;
                        APP.UserControls.WaitWindow.ShowWindow("正在读取", "正在读取自动校准[" + a6 + "/" + totla + "]......", DeviceDataMonitorWPF.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        conn.CommFactory.ReadAutoAdjustStatus(conn.Address).Wait(); a6++;

                    }
                    catch (Exception)
                    {
                         
                    }
                    finally {
                        t++;
                        if (t >= totla)
                        {
                            APP.UserControls.WaitWindow.CloseWindow(DeviceDataMonitorWPF.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        }

                    }


                });
            } 
        }


        public async Task ReadSerialNo()
        {
            List<Task> ts = new List<Task>();
            foreach (var conn in this.Values.ToList())
            {
                Task t_writer = new Task(() => {
                    conn.CommFactory.ReadSerialNo(conn.Address).Wait();
                });
                t_writer.Start();
            }
            Task.WaitAll(ts.ToArray());//等待所有任务完成
        }

        public async Task ReadDeviceDateTime()
        {
            foreach (var conn in this.Values.ToList())
            {
                conn.CommFactory.ReadDeviceDateTime(conn.Address);
            }
            await Task.Delay(5000);//休息几秒保证任务完成
        }

        public async Task ReadAll()
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (var conn in this.Values.ToList())
                {
                    conn.CommFactory.ReadAll(conn.Address);
                }
            }));
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }

        public async Task ReadOutPutVolage()
        {
            foreach (var conn in this.Values.ToList())
            {
                conn.CommFactory.ReadOutPutVolage(conn.Address);
            }
        }

        public async Task ReadVoltageRange()
        {
            foreach (var conn in this.Values.ToList())
            {
                conn.CommFactory.ReadVoltageRange(conn.Address);
            }
        }

        public async Task ReadAutoAdjustStatus()
        {
            foreach (var conn in this.Values.ToList())
            {
                conn.CommFactory.ReadAutoAdjustStatus(conn.Address);
            }
        }

        public async Task ReadLiangCheng()
        {
            foreach (var conn in this.Values.ToList())
            {
                conn.CommFactory.ReadLiangCheng(conn.Address);
            }
        }


        public async Task ReadManufacturingDate()
        {
            foreach (var conn in this.Values.ToList())
            {
                conn.CommFactory.ReadManufacturingDate(conn.Address);
            }
        }

        internal void SetManufacturingDate(DateTime dateTime, int v)
        {
            foreach (var conn in this.Values.ToList())
            {
                APP.UserControls.WaitWindow.ShowWindow("正在校时", "正在写入" + conn + "出厂时间[" + dateTime + "]......", DeviceDataMonitorWPF.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                conn.CommFactory.SetManufacturingDate(  dateTime,v);
            }
        }

        internal void SetDeviceDateTime(DateTime now, int v)
        {
            foreach (var conn in this.Values.ToList())
            {
                APP.UserControls.WaitWindow.ShowWindow("正在校时", "正在写入"+ conn + "设备时间["+ now + "]......", DeviceDataMonitorWPF.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                conn.CommFactory.SetDeviceDateTime(now, v);
            }
        }

        internal void SetSerialNo(Address address, int position, ulong no, bool v)
        {
            foreach (var conn in this.Values.ToList())
            {
                foreach (var add in conn.Address)
                {
                    if (address == add) {
                        
                        conn.CommFactory.SetSerialNo( address,  position,  no,  v);
                    }
                }
            }
        }

        /// <summary>
        ///  按照连接写入串号
        /// </summary>
        /// <param name="address"></param>
        /// <param name="position"></param>
        /// <param name="no"></param>
        /// <param name="v"></param>
        internal void SetSerialNoByConnection( List<SerialNoByConnectionEntity> noList)
        {
            bool first_item = true;
            foreach (SerialNoByConnectionEntity item in noList)
            {
                Address address = item.Address;
                int position = item.Position;
                ulong no = item.SerialNo;
                first_item = false;
                foreach (var conn in this.Values.ToList())
                {
                    foreach (var add in conn.Address)
                    {
                        if (address == add)
                        {
                            APP.UserControls.WaitWindow.ShowWindow("正在写入", "正在写入串号【" + add + "-" + position + "," + no + "】......", DeviceDataMonitorWPF.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                            System.Threading.Thread.Sleep(100);
                            conn.CommFactory.SetSerialNo(address, position, no, first_item);
                            System.Threading.Thread.Sleep(500);
                        }
                    }
                }

                
            }
            
        }

    }

    public class SerialNoByConnectionEntity {
        public Address Address { get; set; } 
        public int Position { get; set; }
        public ulong SerialNo { get; set; }
    }
}
