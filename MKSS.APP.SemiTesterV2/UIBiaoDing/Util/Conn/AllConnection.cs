using MKSS.APP.UIBiaoDing;
using MKSS.APP.UIBiaoDing.Config;
using MKSS.APP.UIBiaoDing.Util;
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

                        APP.UserControls.WaitWindow.ShowWindow("正在读取", "正在读取串号[" + a1 + "/" + totla + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        conn.CommFactory.ReadSerialNo(conn.Address).Wait(); a1++;
                        APP.UserControls.WaitWindow.ShowWindow("正在读取", "正在读取设备时间[" + a2 + "/" + totla + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        conn.CommFactory.ReadDeviceDateTime(conn.Address).Wait(); a2++;
                        APP.UserControls.WaitWindow.ShowWindow("正在读取", "正在读取量程[" + a3 + "/" + totla + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        conn.CommFactory.ReadLiangCheng(conn.Address).Wait(); a3++;
                        APP.UserControls.WaitWindow.ShowWindow("正在读取", "正在读取输出电压[" + a4 + "/" + totla + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        conn.CommFactory.ReadOutPutVolage(conn.Address).Wait(); a4++;
                        APP.UserControls.WaitWindow.ShowWindow("正在读取", "正在读取电压范围[" + a5 + "/" + totla + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        conn.CommFactory.ReadVoltageRange(conn.Address).Wait(); a5++;
                        APP.UserControls.WaitWindow.ShowWindow("正在读取", "正在读取自动校准[" + a6 + "/" + totla + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        conn.CommFactory.ReadAutoAdjustStatus(conn.Address).Wait(); a6++;

                    }
                    catch (Exception)
                    {
                         
                    }
                    finally {
                        t++;
                        if (t >= totla)
                        {
                            APP.UserControls.WaitWindow.CloseWindow(MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        }

                    }


                });
            } 
        }


        public void ReadSerialNo()
        {

            int a1 = 1;
            int t = 0;
            int totla = this.Values.Count;
            foreach (var conn in this.Values.ToList())
            {
                Task.Run(delegate ()
                {
                    try
                    {
                        conn.CommFactory.ReadSerialNoContinue = true;
                        APP.UserControls.WaitWindow.ShowWindow("正在读取", "正在读取串号[" + a1 + "/" + totla + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        conn.CommFactory.ReadSerialNo(conn.Address).Wait(); a1++;
                        APP.UserControls.WaitWindow.ShowWindow("正在读取", "正在读取串号[" + a1 + "/" + totla + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        conn.CommFactory.ReadSerialNoContinue = false;
                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        t++;
                        if (t >= totla)
                        {
                            APP.UserControls.WaitWindow.CloseWindow(MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        }

                    }


                });
            }

        }


        public void ReadDeviceDateTime()
        {

            int a1 = 1;
            int t = 0;
            int totla = this.Values.Count;
            foreach (var conn in this.Values.ToList())
            {
                Task.Run(delegate ()
                {
                    try
                    {
                        conn.CommFactory.ReadDeviceDateTimeContinue = true;
                        APP.UserControls.WaitWindow.ShowWindow("正在读取", "正在读取设备时间[" + a1 + "/" + totla + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        conn.CommFactory.ReadDeviceDateTime(conn.Address).Wait(); ; a1++;
                        APP.UserControls.WaitWindow.ShowWindow("正在读取", "正在读取设备时间[" + a1 + "/" + totla + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        conn.CommFactory.ReadDeviceDateTimeContinue = false;
                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        t++;
                        if (t >= totla)
                        {
                            APP.UserControls.WaitWindow.CloseWindow(MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        }

                    }


                });
            }


        }

        public async Task ReadAll()
        {
             
            //Thread thread = new Thread(new ThreadStart(() =>
            //{
            //    foreach (var conn in this.Values.ToList())
            //    {
            //        conn.CommFactory.ReadAll(conn.Address);
            //    }
            //}));
            //thread.SetApartmentState(ApartmentState.STA);
            //thread.IsBackground = true;
            //thread.Start();

            foreach (var conn in this.Values.ToList())
            {
                ThreadPool.QueueUserWorkItem(state => ReadAllInner(conn));
            }

        }

        public void ReadAllInner(BoardConnection conn )
        {
            conn.CommFactory.ReadAll(conn.Address);
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
                Task task2 = Task.Factory.StartNew(() =>
                {
                    APP.UserControls.WaitWindow.ShowWindow("正在校时", "正在写入" + conn + "出厂时间[" + dateTime + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                    conn.CommFactory.SetManufacturingDate(dateTime, v);
                });

            }
        }

        internal void SetDeviceDateTime(DateTime now, int v)
        {
            foreach (var conn in this.Values.ToList())
            {
                Task task2 = Task.Factory.StartNew(() =>
                {

                    APP.UserControls.WaitWindow.ShowWindow("正在校时", "正在写入" + conn + "设备时间[" + now + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                    conn.CommFactory.SetDeviceDateTime(now, v);
                });
            }
        }


        internal void SetReadDeviceDateTime(DateTime DeviceDateTime, DateTime ManufacturingDate, int v)
        {
            int sucess1 = 0;
            int sucess2 = 0;
            int sucess3 = 0;
            int total = this.Values.Count;
            APP.UserControls.WaitWindow.ShowWindow("正在校时", "正在写入设备时间,出厂日期[" + DeviceDateTime + "][" + sucess1 + "-" + sucess2 + "-" + sucess3 + "/" + total + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
            List<Task> allTask = new List<Task>();
            foreach (var conn in this.Values.ToList())
            {
                Task taskSet = new Task(() =>
                {

                    conn.CommFactory.SetDeviceDateTime(DeviceDateTime, v);
                    sucess1++;
                    APP.UserControls.WaitWindow.ShowWindow("正在校时", "正在写入设备时间,出厂日期[" + DeviceDateTime + "][" + sucess1 + "-" + sucess2 + "-" + sucess3 + "/" + total + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);

                    MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(1000);

                    conn.CommFactory.SetManufacturingDate(ManufacturingDate, v);
                    sucess2++;
                    APP.UserControls.WaitWindow.ShowWindow("正在校时", "正在写入设备时间,出厂日期[" + DeviceDateTime + "][" + sucess1 + "-" + sucess2 + "-" + sucess3 + "/" + total + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);

                    MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(1000);

                    conn.CommFactory.ReadDeviceDateTime(conn.Address).Wait();
                    sucess3++;
                    APP.UserControls.WaitWindow.ShowWindow("正在校时", "正在写入设备时间,出厂日期[" + DeviceDateTime + "][" + sucess1 + "-" + sucess2 + "-" + sucess3 + "/" + total + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);

                });
                allTask.Add(taskSet);
                taskSet.Start();
            }


            Task.WaitAll(allTask.ToArray());

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
                            APP.UserControls.WaitWindow.ShowWindow("正在写入", "正在写入串号【" + add + "-" + position + "," + no + "】......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                            MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(100);
                            conn.CommFactory.SetSerialNo(address, position, no, first_item);
                           MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(500);
                        }
                    }
                }

                
            }
            
        }


        public void SetSpan(int TxtSpan)
        {

            int a1 = 0;
            int t = 0;
            int totla = this.Values.Sum(w => w.Address.Count);
            foreach (var conn in this.Values.ToList())
            {
                Task.Run(delegate ()
                {
                    try
                    {
                        conn.CommFactory.IsWrite = true;
                        APP.UserControls.WaitWindow.ShowWindow("正在设置Span点", "正在设置Span点[" + a1 + "/" + totla + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        foreach (var add in conn.Address)
                        {
                            conn.CommFactory.SetSpan(TxtSpan, add.Vb); a1++;
                            APP.UserControls.WaitWindow.ShowWindow("正在设置Span点", "正在设置Span点[" + a1 + "/" + totla + "]["+ add + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        }
                        APP.UserControls.WaitWindow.ShowWindow("正在设置Span点", "正在设置Span点[" + a1 + "/" + totla + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        conn.CommFactory.IsWrite = false;

                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        t++;
                        if (t >= this.Values.Count)
                        {
                            APP.UserControls.WaitWindow.CloseWindow(MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        }

                    }


                });

            }

        }


        public void SetZreo(int TxtZreo)
        {

            int a1 = 0;
            int t = 0;
            int totla = this.Values.Sum(w => w.Address.Count);
            foreach (var conn in this.Values.ToList())
            {
                Task.Run(delegate ()
                {
                    try
                    {
                        conn.CommFactory.IsWrite = true;
                        APP.UserControls.WaitWindow.ShowWindow("正在设置零点", "正在设置零点[" + a1 + "/" + totla + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        foreach (var add in conn.Address)
                        {
                            conn.CommFactory.SetZero(TxtZreo, add.Vb); a1++;
                            APP.UserControls.WaitWindow.ShowWindow("正在设置零点", "正在设置零点[" + a1 + "/" + totla + "][" + add + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        }
                        APP.UserControls.WaitWindow.ShowWindow("正在设置零点", "正在设置零点[" + a1 + "/" + totla + "]......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        conn.CommFactory.IsWrite = false;

                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        t++;
                        if (t >= this.Values.Count)
                        {
                            APP.UserControls.WaitWindow.CloseWindow(MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                        }

                    }


                });

            }

        }


    }

    public class SerialNoByConnectionEntity {
        public Address Address { get; set; } 
        public int Position { get; set; }
        public ulong SerialNo { get; set; }
        public int F_SlotNO { get { return Address.V % 2 == 0 ? Position + 15 : Position; } }
    }


}
