using MathNet.Numerics;
using MathNet.Numerics.LinearRegression;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace MKSS.Model
{
    /// <summary>
    ///  存放 T90 等信息
    /// </summary>
    public class SensorStaData {

        /// <summary>
        ///  AD信号波动容差
        /// </summary>
        public static double ADTolerance = 3;
        static Dictionary<PosEnum, SensorStaData> Cache = new Dictionary<PosEnum, SensorStaData>();

        public SensorStaData(PosEnum p) { PosEnum = p; }
        public PosEnum PosEnum { get; set; }
        public SensorDataItem VStart { get; set; }
        public SensorDataItem VEnd { get; set; }
        public SensorDataItem VMax { get; set; }
        public SensorDataItem VMaxStart { get; set; }
        public SensorDataItem V90 { get; set; }
        public SensorDataItem V10 { get; set; }
        public double T90 { get { return V90 == null || VStart == null ? 0 : V90.F_AddTime - VStart.F_AddTime; } }
        public double T10 { get { return V10 == null || VMaxStart == null ? 0 : V10.F_AddTime - VMaxStart.F_AddTime; } }

        public string ToDebugString()
        {
            return string.Format("Start:{0};VMax:{1};VMaxStart:{2};VEnd:{3};V90:{4};V10:{5};"
                , VStart == null ? "N,N" : VStart.ToStringTD()
                , VMax == null ? "N,N" : VMax.ToStringTD()
                , VMaxStart == null ? "N,N" : VMaxStart.ToStringTD()
                , VEnd == null ? "N,N" : VEnd.ToStringTD()
                , V90 == null ? "N,N" : V90.ToStringTD()
                , V10 == null ? "N,N" : V10.ToStringTD());
        }

        public List<SensorDataItem> HistoryDatas { get; set; }
        public double? AD90 { 
            get { 
                if (VStart == null || VMax == null) return null;
                if (VStart.F_DataValueExponential == 0 || VMax.F_DataValueExponential == 0)
                    return VStart.F_DataValue + (VMax.F_DataValue - VStart.F_DataValue) * 0.9;
                return VStart.F_DataValueExponential + (VMax.F_DataValueExponential - VStart.F_DataValueExponential) * 0.9; 
            } 
        }
        public double? AD10 { 
            get { 
                if (VStart == null || VMax == null) return null; 
                if(VStart.F_DataValueExponential==0|| VMax.F_DataValueExponential==0)
                    return VStart.F_DataValue + (VMax.F_DataValue - VStart.F_DataValue ) * 0.1;
                return VStart.F_DataValueExponential + (VMax.F_DataValueExponential - VStart.F_DataValueExponential) * 0.1; 
            } 
        }

        public void CalculateStart()
        {
            VStart = null;
            VEnd = null;
            VMax = null;
            VMaxStart = null;
            V90 = null;
            V10 = null;
            HistoryDatas = new List<SensorDataItem>();
        }




        /// <summary>
        ///  自动计算 T90 T10 
        ///  插盘.....信号稳定....注气（10%）浓度（检测到AD增强信号，检测到AD最小值）......信号稳定...........散气（检测到AD减弱信号，检测到AD最大值）......信号稳定（检测到AD最小值）........
        ///  第一阶段：暂停键，计算 T90
        ///  第二阶段：找到 T10 位置
        /// </summary>
        /// <param name="list"></param>
        public void CalculateAutoV3(List<SensorGroupData> dataList)
        {
            try
            {

                HistoryDatas.AddRange(dataList.Select(w => w.F_DataValue(PosEnum)));

                if (HistoryDatas.Count < 10) return;

                //求最大值
                double min_value_global = HistoryDatas.Min(w => w.F_DataValue);
                var HistoryDatasX1 = HistoryDatas.Where(w => Math.Abs(w.F_DataValue - min_value_global) < 2 * ADTolerance);
                var HistoryDatasX2 =  HistoryDatas.Where(w => Math.Abs(w.F_DataValue - min_value_global) >= 2 * ADTolerance).ToList();
                foreach (var item in HistoryDatasX1)
                {
                    item.F_DataValueExponential = item.F_DataValue;
                }

                //除去最小值之外用曲线拟合
                double[] x = HistoryDatasX2.Select(w => w.F_AddTime).ToArray();
                double[] y = HistoryDatasX2.Select(w => w.F_DataValue).ToArray();
                double[] p = Exponential(x, y); // a=1.017, r=0.687
                double[] yh = Generate.Map(x, k => p[0] * Math.Exp(p[1] * k));
                for (int i = 0; i < HistoryDatasX2.Count; i++)
                {
                    HistoryDatasX2[i].F_DataValueExponential = yh[i];
                }

                //求最大值
                double max_value = HistoryDatas.Max(w => w.F_DataValueExponential);
                //最大值第一次发生时间
                VMax = HistoryDatas.First(w => w.F_DataValueExponential == max_value);
                if (VMax == null) return;

                //求波峰之前最小值
                var list_part1 = HistoryDatas.Where(w => w.F_AddTime < VMax.F_AddTime).Reverse();
                double min_value = list_part1.Min(w => w.F_DataValueExponential);
                //求波峰之前第一次变大的时间
                VStart = list_part1.First(w => (w.F_DataValueExponential - min_value) <= ADTolerance);
                if (VStart == null) return;


                double? ad90 = AD90;
                if (V90 == null)
                {
                    //求 T90
                    if (ad90 != null)
                    {
                        int ii_start = HistoryDatas.IndexOf(VStart);
                        int ii_end = HistoryDatas.IndexOf(VMax);
                        if (ii_start >= 0 && ii_end >= 0)
                        {
                            for (int i = ii_start; i <= ii_end; i++)
                            {
                                SensorDataItem item0 = HistoryDatas[i];
                                if (item0.F_DataValueExponential >= ad90.Value)
                                {
                                    V90 = item0;
                                    if (V90.F_AddTime <= VStart.F_AddTime)
                                    {
                                        V90 = null;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    //有可能峰值 第一拐点和峰值 第二拐点重合
                }

                //求波峰之后第一次变小值的时间
                var list_part2 = HistoryDatas.Where(w => w.F_AddTime > VMax.F_AddTime).ToList();
                var VMaxStartPost = list_part2.First(w => (max_value - w.F_DataValueExponential) >= ADTolerance);
                if (VMaxStartPost == null) return;
                VMaxStart = list_part2[list_part2.IndexOf(VMaxStartPost) - 1];

                //求 VMaxStart 之后第一次接近最小值时间
                var list_part3 = HistoryDatas.Where(w => w.F_AddTime > VMaxStart.F_AddTime);
                double min_value3 = list_part3.Min(w => w.F_DataValueExponential);
                VEnd = list_part3.First(w => (w.F_DataValueExponential - min_value3) <= ADTolerance);

                double? ad10 = AD10;
                if (V10 == null)
                {
                    //求 T10
                    if (ad10 != null && VMaxStart != null)
                    {
                        int ii_start = HistoryDatas.IndexOf(VMaxStart);
                        int ii_end = HistoryDatas.IndexOf(VEnd);
                        if (ii_start >= 0 && ii_end >= 0)
                        {
                            for (int i = ii_start; i <= ii_end; i++)
                            {
                                SensorDataItem item0 = HistoryDatas[i];
                                if (item0.F_DataValueExponential <= ad10.Value)
                                {
                                    V10 = item0;
                                    break;
                                }
                            }
                        }

                    }
                    return;//前置数据没准备好，没必要执行下面计算
                }

            }
            catch (System.Exception w)
            {

                throw;
            }



        }

        /// <summary>
        ///  自动计算 T90 T10 
        ///  插盘.....信号稳定....注气（10%）浓度（检测到AD增强信号，检测到AD最小值）......信号稳定...........散气（检测到AD减弱信号，检测到AD最大值）......信号稳定（检测到AD最小值）........
        ///  第一阶段：暂停键，计算 T90
        ///  第二阶段：找到 T10 位置
        /// </summary>
        /// <param name="list"></param>
        public void CalculateAuto(List<SensorGroupData> dataList)
        {
            try
            {

                HistoryDatas.AddRange(dataList.Select(w => w.F_DataValue(PosEnum)));

                if (HistoryDatas.Count < 10) return;

                //求最大值
                double max_value = HistoryDatas.Max(w => w.F_DataValue);
                //最大值第一次发生时间
                VMax = HistoryDatas.First(w => w.F_DataValue == max_value);
                if (VMax == null) return;

                //求波峰之前最小值
                var list_part1 = HistoryDatas.Where(w => w.F_AddTime < VMax.F_AddTime).Reverse();
                double min_value = list_part1.Min(w => w.F_DataValue);
                //求波峰之前第一次变大的时间
                VStart = list_part1.First(w => (w.F_DataValue - min_value) <= ADTolerance);
                if (VStart == null) return;


                double? ad90 = AD90;
                if (V90 == null)
                {
                    //求 T90
                    if (ad90 != null)
                    {
                        int ii_start = HistoryDatas.IndexOf(VStart);
                        int ii_end = HistoryDatas.IndexOf(VMax);
                        if (ii_start >= 0 && ii_end >= 0) {
                            for (int i = ii_start; i <= ii_end; i++)
                            {
                                SensorDataItem item0 = HistoryDatas[i];
                                if (item0.F_DataValue >= ad90.Value)
                                {
                                    V90 = item0;
                                    if (V90.F_AddTime <= VStart.F_AddTime)
                                    {
                                        V90 = null;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    //有可能峰值 第一拐点和峰值 第二拐点重合
                }

                try
                {
                    //求波峰之后第一次变小值的时间
                    var list_part2 = HistoryDatas.Where(w => w.F_AddTime > VMax.F_AddTime).ToList();
                    VMaxStart = list_part2.FirstOrDefault(w => (max_value - w.F_DataValue) >= ADTolerance);
                    if (VMaxStart == null) return;
                    //VMaxStart = list_part2[list_part2.IndexOf(VMaxStart)];
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                try
                {

                    //求 VMaxStart 之后第一次接近最小值时间
                    var list_part3 = HistoryDatas.Where(w => w.F_AddTime > VMaxStart.F_AddTime);
                    double min_value3 = list_part3.Min(w => w.F_DataValue);
                    VEnd = list_part3.FirstOrDefault(w => (w.F_DataValue - min_value3) <= ADTolerance);
                }
                catch (Exception ex)
                {
                    throw ex;
                }


                double? ad10 = AD10;
                if (V10 == null)
                {
                    //求 T10
                    if (ad10 != null && VMaxStart != null)
                    {
                        int ii_start = HistoryDatas.IndexOf(VMaxStart);
                        int ii_end = HistoryDatas.IndexOf(VEnd);
                        if (ii_start >= 0 && ii_end>=0)
                        {
                            for (int i = ii_start; i <= ii_end; i++)
                            {
                                SensorDataItem item0 = HistoryDatas[i];
                                if (item0.F_DataValue <= ad10.Value)
                                {
                                    V10 = item0;
                                    break;
                                }
                            }
                        }

                    }
                    return;//前置数据没准备好，没必要执行下面计算
                }

            }
            catch (System.Exception w)
            {
                throw w;
            }



        }



        /// <summary>
        ///  自动计算 T90 T10 
        ///  插盘.....信号稳定....注气（10%）浓度（检测到AD增强信号，检测到AD最小值）......信号稳定...........散气（检测到AD减弱信号，检测到AD最大值）......信号稳定（检测到AD最小值）........
        ///  第一阶段：暂停键，计算 T90
        ///  第二阶段：找到 T10 位置
        /// </summary>
        /// <param name="list"></param>
        public void CalculateAutoV1(List<SensorGroupData> dataList)
        {
            try
            {

                HistoryDatas.AddRange(dataList.Select(w=>w.F_DataValue(PosEnum)));

                int bc = 1;
                int hdcs = HistoryDatas.Count;
                if (hdcs < bc *  2 +1) return;

                try
                {
                    for (int h = 0; h < HistoryDatas.Count- (bc * 2 + 1); h++)
                    {

                        var hisData = new List<SensorDataItem>();
                        for (int j = 0; j < bc * 2 + 1; j++)
                        {
                            hisData.Add(HistoryDatas[h + j]);
                        }

                        //拟合数据
                        double[] x = hisData.Select(w => w.F_AddTime).ToArray();
                        double[] y = hisData.Select(w => w.F_DataValue).ToArray();
                        double[] p = Exponential(x, y); // a=1.017, r=0.687
                        double[] yh = Generate.Map(x, k => p[0] * Math.Exp(p[1] * k)); // 2.02, 4.02, 7.98
                        hisData[bc].F_DataValueExponential = Math.Round(yh[bc], 2);

                    }
                   
                    //return;
                    //for (int i = 0; i < yh.Length; i++)
                    //{
                    //    HistoryDatas[HistoryDatas.Count - nxx + (i)].F_DataValueExponential = yh[i];
                    //} 

                }
                catch (Exception ex)
                {
                    throw ex;
                }


                if (HistoryDatas.Count < 12) return;

                for (int i = 9 + 3 -1; i < HistoryDatas.Count - bc; i++)
                {
                    SensorDataItem itemx = HistoryDatas[i];
                    List<SensorDataItem> list = new List<SensorDataItem>();
                    for (int j = i - 11; j <= i; j++)
                    {
                        list.Add(HistoryDatas[j]);
                    }
                    DealInner(list, itemx);
                }

            }
            catch (System.Exception w)
            {

                throw;
            }



        }


        /// <summary>
        ///  自动计算 T90 T10 
        ///  插盘.....信号稳定....注气（10%）浓度（检测到AD增强信号，检测到AD最小值）......信号稳定...........散气（检测到AD减弱信号，检测到AD最大值）......信号稳定（检测到AD最小值）........
        ///  第一阶段：暂停键，计算 T90
        ///  第二阶段：找到 T10 位置
        /// </summary>
        /// <param name="list"></param>
        public void CalculateAutoStepByStep(SensorGroupData data)
        {
            try
            {
                var itemx = data.F_DataValue(PosEnum);
                HistoryDatas.Add(itemx);

                if (HistoryDatas.Count <= 3) HistoryDatas[HistoryDatas.Count - 1].F_DataValueExponential = HistoryDatas[HistoryDatas.Count - 1].F_DataValue;
                int nxx = 7;
                int hdcs = HistoryDatas.Count;
                if (hdcs < nxx) return;

                try
                {
                    //拟合数据
                    double[] x = HistoryDatas.TakeLast(nxx).Select(w => w.F_AddTime).ToArray();
                    double[] y = HistoryDatas.TakeLast(nxx).Select(w => w.F_DataValue).ToArray();
                    double[] p = Exponential(x, y); // a=1.017, r=0.687
                    double[] yh = Generate.Map(x, k => p[0] * Math.Exp(p[1] * k)); // 2.02, 4.02, 7.98
                    HistoryDatas[HistoryDatas.Count - (nxx + 1) / 2].F_DataValueExponential = Math.Round(yh[nxx - (nxx + 1) / 2], 2);
                    //for (int i = 0; i < yh.Length; i++)
                    //{
                    //    HistoryDatas[HistoryDatas.Count - nxx + (i)].F_DataValueExponential = yh[i];
                    //} 
                }
                catch (Exception ex)
                {
                    throw ex;
                }


                if (HistoryDatas.Count < 20) return;
                IEnumerable<SensorDataItem> itemLast = HistoryDatas.TakeLast(9 + 3);
                DealInner(itemLast, itemx);

            }
            catch (System.Exception w)
            {

                throw;
            }



        }

        void DealInner(IEnumerable<SensorDataItem> itemLast,SensorDataItem data)
        {

            if (VStart == null)
            {
                //求 开始值
                IEnumerable<SensorDataItem> item7 = itemLast.Take(9);
                IEnumerable<SensorDataItem> itema = item7.Take(4);
                IEnumerable<SensorDataItem> itemb = item7.TakeLast(4);
                //数据点每个比后三个数据都小，并且后三个依次变大
                if (LargerThan(itemb, itema) && IncreaseOrDecrease(itemb, true))
                {
                    VStart = item7.TakeLast(1).First();//拟合造成变化提前，延后 3 个数据点
                    return;
                }
                return;//前置数据没准备好，没必要执行下面计算
            }


            if (VMax == null)
            {

                //求 峰值第一拐点
                List<SensorDataItem> item7 = itemLast.Take(9).ToList();

                IEnumerable<SensorDataItem> itema = item7.Take(4);
                SensorDataItem b0 = item7[4];
                if (item7[4].F_AddTime > 19.5)
                {
                    string ss = "";
                }
                SensorDataItem b1 = item7[5];
                SensorDataItem b2 = item7[6];
                SensorDataItem b3 = item7[7];
                SensorDataItem b4 = item7[8];
                //数据点比前三个数据都大，比后三个也大， 并且前三个依次变大
                if (LargerThan(b0, itema)
                     && IncreaseOrDecrease(itema, true)
                     &&
                        (LargerEqualThan(b0, b1) && LargerEqualThan(b0, b2) && LargerEqualThan(b0, b3) && LargerEqualThan(b0, b4))
                     )
                {
                    VMax = b0;
                }
                //有可能峰值 第一拐点和峰值 第二拐点重合
            }


            double? ad90 = AD90;
            if (V90 == null)
            {
                //求 T90
                if (ad90 != null)
                {
                    int ii_start = HistoryDatas.IndexOf(VStart);
                    int ii_end = HistoryDatas.IndexOf(VMax);
                    if (ii_start >= 0 && ii_end >= 0)
                    {
                        for (int i = ii_start; i <= ii_end; i++)
                        {
                            SensorDataItem item0 = HistoryDatas[i];
                            if (item0.F_DataValueExponential >= ad90.Value)
                            {
                                V90 = item0;
                                if (V90.F_AddTime <= VStart.F_AddTime)
                                {
                                    V90 = null;
                                }
                                break;
                            }
                        }
                    }
                }
                //有可能峰值 第一拐点和峰值 第二拐点重合
            }


            if (VMaxStart == null)
            {
                //求 峰值第二拐点，第二拐点取最后拐点 
                List<SensorDataItem> item7 = itemLast.Take(9).ToList();
                IEnumerable<SensorDataItem> itema = item7.TakeLast(4);
                if (item7[4].F_AddTime > 20)
                {
                    string ss = "";
                }
                SensorDataItem b0 = item7[4];
                SensorDataItem b1 = item7[0];
                SensorDataItem b2 = item7[1];
                SensorDataItem b3 = item7[2];
                SensorDataItem b4 = item7[3];
                //数据点比后三个数据都大， 比前三个也大， 并且后三个依次变小
                if (LargerThan(b0, itema)
                    && IncreaseOrDecrease(itema, false)
                    &&
                        (LargerEqualThan(b0, b1) && LargerEqualThan(b0, b2) && LargerEqualThan(b0, b3) && LargerEqualThan(b0, b4))
                    )
                {
                    VMaxStart = b0;
                    return;
                }
                return;//前置数据没准备好，没必要执行下面计算
            }




            double? ad10 = AD10;
            if (V10 == null)
            {
                //求 T10
                if (ad10 != null && VMaxStart != null)
                {
                    int ii_start = HistoryDatas.IndexOf(VMaxStart);
                    int ii_end = HistoryDatas.IndexOf(VEnd);
                    if (ii_start >= 0 && ii_end >= 0)
                    {
                        for (int i = ii_start; i <= ii_end; i++)
                        {
                            SensorDataItem item0 = HistoryDatas[i];
                            if (item0.F_DataValueExponential <= ad10.Value)
                            {
                                V10 = item0;
                                break;
                            }
                        }
                    }

                }
                return;//前置数据没准备好，没必要执行下面计算
            }


            //已经设置过 T10
            if (V10 != null
                && data.F_AddTime > V10.F_AddTime)
            {

                //没设置过 TEnd
                if (VEnd == null)
                {
                    //尝试设置 TEnd
                    if (HistoryDatas.Count > 15)
                    {
                        IEnumerable<SensorDataItem> item7 = itemLast.Take(9);
                        IEnumerable<SensorDataItem> itema = item7.Take(4);
                        IEnumerable<SensorDataItem> itemb = item7.TakeLast(4);
                        //数据点每个比后三个数据都大，并且数据点依次减小
                        if (LargerThan(itema, itemb) && IncreaseOrDecrease(itema, false))
                        {
                            VEnd = item7.TakeLast(5).Take(1).First();
                        }
                    }
                }
                //后面不再处理
                return;
            }
        }

         
        /// <summary>
        ///  计算 T90 T10 
        ///  插盘.....信号稳定....注气（10%）浓度......信号稳定.......暂停键......散气+继续键........
        ///  第一阶段：暂停键，计算 T90
        ///  第二阶段：找到 T10 位置
        ///  注气过程中 AD 值会出现波峰
        /// </summary>
        /// <param name="list"></param>
        public void CalculateStep1(List<SensorGroupData> list) {

            VStart = null;
            VEnd = null;
            VMax = null;
            V90 = null;
            V10 = null;
            HistoryDatas = new List<SensorDataItem>();
            if (list == null || list.Count < 10) return;
            HistoryDatas = list.Select(w => w.F_DataValue(PosEnum)).ToList();
            VMax = HistoryDatas[HistoryDatas.Count - 1];
            VMaxStart = HistoryDatas[HistoryDatas.Count - 1];
            for (int i = 0; i < HistoryDatas.Count-7; i++)
            {
                IEnumerable<SensorDataItem> item7 = HistoryDatas.Take(i + 7);
                IEnumerable<SensorDataItem> itema = item7.TakeLast(7).Take(3);
                IEnumerable<SensorDataItem> itemb = item7.TakeLast(3);
                //数据点每个比后三个数据都小
                if (LargerThan(itemb, itema)) {
                    VStart = item7.TakeLast(4).Take(1).First();
                    break;
                } 
            }

            double? v90 = AD90;
            if (v90 == null) return;
            for (int i = 0; i < HistoryDatas.Count; i++)
            {
                SensorDataItem item0 = HistoryDatas[i]; 
                if (item0.F_DataValue >= v90.Value)
                {
                    V90 = item0;
                    break;
                }
            }

        }

        /// <summary>
        ///  计算 T90 T10 
        ///  插盘.....信号稳定....注气（10%）浓度......信号稳定.......暂停键......散气+继续键........
        ///  第一阶段：暂停键，计算 T90
        ///  第二阶段：找到 T10 位置
        /// </summary>
        /// <param name="w"></param>
        public void CalculateStep2(SensorGroupData w)
        {

            double? v90 = AD90;
            if (v90 == null) return;
            if (w == null) return;

            if (VEnd != null) {
                return;//检测已经结束 ，后面不再处理
            }

            SensorDataItem itemx = w.F_DataValue(PosEnum);
            HistoryDatas.Add(itemx);

            if (itemx.F_DataValue <= v90.Value)
            {
                V10 = itemx;
            }

            //已经设置过 T10
            if (V10 != null
                && w.F_AddTime > V10.F_AddTime
                && V10.SensorGroupData.F_BatchId == w.F_BatchId) {
                
                //没设置过 TEnd
                if (VEnd == null)
                {
                    //尝试设置 TEnd
                    if (HistoryDatas.Count > 10)
                    {
                        IEnumerable<SensorDataItem> item7 = HistoryDatas.TakeLast(7);
                        IEnumerable<SensorDataItem> itema = item7.TakeLast(7).Take(3);
                        IEnumerable<SensorDataItem> itemb = item7.TakeLast(3);
                        //数据点每个比后三个数据都大
                        if (LargerThan(itema, itemb))
                        {
                            VEnd = item7.TakeLast(4).Take(1).First();
                        }
                    }
                }
                //后面不再处理
                return;
            }


        }

        /// <summary>
        ///  A 中每一个都比 B 中的大
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        bool LargerThan(IEnumerable<SensorDataItem> A, IEnumerable<SensorDataItem> B) {

            foreach (var a in A)
            {
                foreach (var b in B)
                {
                    if (a.F_DataValueExponential <= b.F_DataValueExponential) return false;
                }
            }
            return true;
        }

        /// <summary>
        ///  A 中数据依次变大
        /// </summary>
        /// <param name="A"></param>
        /// <param name="increase"></param>
        /// <param name="num">变化必须超过</param>
        /// <returns></returns>
        bool IncreaseOrDecrease(IEnumerable<SensorDataItem> A,bool increase)
        {
            double num = 0.2;
            List<SensorDataItem> list = A.ToList();
            if (increase)
            {
                for (int i = 1; i < list.Count; i++)
                {
                    if (list[i].F_DataValueExponential <= list[i - 1].F_DataValueExponential + num) return false;// 
                }
            }
            else {
                for (int i = 1; i < list.Count; i++)
                {
                    if (list[i].F_DataValueExponential >= list[i - 1].F_DataValueExponential + num) return false;//
                }
            } 
            return true;
        }


        /// <summary>
        ///  a 比 B中每一个的大
        /// </summary>
        /// <param name="a"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        bool LargerThan(SensorDataItem a, IEnumerable<SensorDataItem> B)
        {
            foreach (var b in B)
            {
                if (a.F_DataValueExponential <= b.F_DataValueExponential) return false;
            }
            return true;
        }

        /// <summary>
        ///  a 大于等于 b ，b加上容差
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        bool LargerEqualThan(SensorDataItem a, SensorDataItem b)
        {
            if (a.F_DataValueExponential >= b.F_DataValueExponential) return true;
            return false;
        }

        /// <summary>
        ///  使用方法，数据拟合
        /// double[] x = new[] { 1.0, 2.0, 3.0 };
        /// double[] y = new[] { 2.0, 4.1, 7.9 };
        /// double[] p = Exponential(x, y); // a=1.017, r=0.687
        /// double[] yh = Generate.Map(x, k => p[0] * Math.Exp(p[1] * k)) // 2.02, 4.02, 7.98
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        double[] Exponential(double[] x, double[] y, DirectRegressionMethod method = DirectRegressionMethod.QR)
        {
            double[] y_hat = Generate.Map(y, Math.Log);
            double[] p_hat = Fit.LinearCombination(x, y_hat, method, t => 1.0, t => t);
            return new[] { Math.Exp(p_hat[0]), p_hat[1] };
        }

        private static PointF bezier_interpolation_func(float t, PointF[] points, int count)
        { 
            // 一个点都没有
            if (points.Length < 1)  throw new  Exception("一个点都没有");
            PointF[] tmp_points = new PointF[count];
            for (int i = 1; i < count; ++i)
            {
                for (int j = 0; j < count - i; ++j)
                {
                    if (i == 1)
                    {
                        float x1 = (float)(points[j].X * (1 - t) + points[j + 1].X * t);
                        float y1 = (float)(points[j].Y * (1 - t) + points[j + 1].Y * t);
                        tmp_points[j] = new PointF { X = x1, Y = y1 };
                        continue;
                    }
                    float x = (float)(tmp_points[j].X * (1 - t) + tmp_points[j + 1].X * t);
                    float y = (float)(tmp_points[j].Y * (1 - t) + tmp_points[j + 1].Y * t);
                    tmp_points[j] = new PointF { X = x, Y = y };
                }
            }
            return tmp_points[0];

        }

    }
}