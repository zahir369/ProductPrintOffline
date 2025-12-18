using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq; 
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;


namespace MKSS.APP.ECTester
{

    public class PloyLineChart
    {

        public static float XLeftMargin = 50;
        public static float XRightMargin = 20;
        public static float YBottomMargin = 20;
        public static float YTopMargin = 35;
        private static float YLabelLen = 8;
        private static int YAxisCount = 5;
        private static int XAxisCount = 12;

        public static float XLabelMinDefault { get; set; } = 0F;
        public static float XLabelMaxDefault { get; set; } = 320F;
        public static float YLabelMinDefault { get; set; } = 0F;
        public static float YLabelMaxDefault { get; set; } = 1000F;
        public static string AxisXLabeleFormate { get; set; } = "f1";
        public static string AxisYLabeleFormate { get; set; } = "f0";

        public bool Continued = true;
        public Zooming Zooming { get; set; }
        public bool ShowLinePoint { get; set; } = true;
        System.Windows.Media.Imaging.WriteableBitmap CacheBitmap { get; set; }
        public System.Windows.Controls.Image Img { get; private set; }
        public Brush LabelColor { get; set; } = new SolidBrush(Color.LightGray);
        public Brush AxisColor { get; set; } = new SolidBrush(Color.LightGray);
        public float ThinknessAxis { get; set; } = 2F;
        public float Thinkness { get; set; } = 1F;
        public float CanvasWidth { get; set; } = 100;
        public float CanvasHeight { get; set; } = 100;
        public static float X0 { get { return XLeftMargin; } }
        public static float Y0 { get { return YTopMargin; } }

        public float XLabelMin { get; set; } = XLabelMinDefault;
        public float XLabelMax { get; set; } = XLabelMaxDefault;
        public float YLabelMin { get; set; } = YLabelMinDefault;
        public float YLabelMax { get; set; } = YLabelMaxDefault;
        public float RatioX { get { return XWidth / (XLabelMax - XLabelMin); } }
        public float RatioY { get { return YHeight / (YLabelMax - YLabelMin); } }

        public Dictionary<float, Color> StdLineX { get; set; } = new Dictionary<float, Color>();
        internal List<SeriseDatas> AllDatas { get; set; }
        public float XWidth
        {
            get
            {
                return CanvasWidth - XRightMargin - XLeftMargin;
            }
        }
        public float YHeight
        {
            get
            {
                return CanvasHeight - YTopMargin - YBottomMargin;
            }
        }
        public int DataCount
        {
            get
            {
                return this.AllDatas.Count;
            }

        }
        public int RtCount
        {
            get
            {
                return AllDatas.Max(w => w.RtData.Count);
            }
        }
        public System.Windows.Controls.Panel Parent { get; set; }
        public PloyLineChart(System.Windows.Controls.Grid parent)
        {
            Parent = parent;
            ShowLinePoint = true;
            Img = new System.Windows.Controls.Image();
            Img.VerticalAlignment = VerticalAlignment.Stretch;
            Img.HorizontalAlignment = HorizontalAlignment.Stretch;
            Img.Cursor = System.Windows.Input.Cursors.Cross;
            Zooming = new Zooming();
            Zooming.VerticalAlignment = VerticalAlignment.Stretch;
            Zooming.HorizontalAlignment = HorizontalAlignment.Stretch;
            Zooming.Cursor = System.Windows.Input.Cursors.Cross;
            Parent.Children.Insert(0, Img);
            Parent.Children.Insert(1, Zooming);
            AllDatas = new List<SeriseDatas>();
        }

        /// <summary>
        ///  重置坐标范围
        /// </summary>
        public void SetDefaultAxis()
        {
            XLabelMin = XLabelMinDefault;
            XLabelMax = XLabelMaxDefault;
            YLabelMin = YLabelMinDefault;
            YLabelMax = YLabelMaxDefault;
        }

        public void Init(int w, int h)
        {
            if (CacheBitmap != null)
            {
                Img.Source = null;
                CacheBitmap = null;
                GC.Collect();
            }
            CacheBitmap = new System.Windows.Media.Imaging.WriteableBitmap(w, h, 96, 96, System.Windows.Media.PixelFormats.Pbgra32, null);
            Img.Source = CacheBitmap;
            this.CanvasWidth = w;
            this.CanvasHeight = h;
            Zooming.IniDrawingCanvas(this);

            this.XLabelMin = this.XLabelMin;
            this.XLabelMax = this.XLabelMax;
            foreach (var item in AllDatas)
            {
                item.clear();
            }

        }


        public void AppendAxises()
        {

            //可以在线程中获取数据、生成图像内容，但是 显示图像内容的WriteableBitmap必须和主窗体属于同一线程，否则图像内容不能正常显示
            //采用的方法是：将WriteableBitmap的内存指针传递给线程
            Img.Dispatcher.BeginInvoke(new Action(() => {
                CacheBitmap.Lock();
            }));

            Bitmap backBitmap = new Bitmap(CacheBitmap.PixelWidth, CacheBitmap.PixelHeight, CacheBitmap.BackBufferStride, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, CacheBitmap.BackBuffer);
            Graphics dc = Graphics.FromImage(backBitmap);
            dc.Clear(System.Drawing.Color.Transparent);
            Font drawFont = new Font("Microsoft YaHei", 10);
            {

                int count = 0;
                //计算基础坐标系
                float x0, y0;
                x0 = XLeftMargin;
                y0 = YTopMargin;

                Pen penAxis = new Pen(AxisColor, ThinknessAxis);

                float yLabelMax = this.YLabelMax;
                float yLabelMin = this.YLabelMin;
                ////画标题
                //string title = AllDatas[count].Title;
                //FormattedText fttitle = new FormattedText(title, new System.Globalization.CultureInfo("zh-CHS", false), FlowDirection.LeftToRight, new Typeface("Microsoft YaHei"), 15, Brushes.Black);
                //dc.DrawText(fttitle, new PointF(xWidth / 2 + x0 - fttitle.Width / 2, y0 - fttitle.Height));

                Pen pen3 = new Pen(AxisColor, Thinkness);
                pen3.DashStyle = DashStyle.Dot;// new System.Drawing.Drawing2D.DashStyle(new float[] { 2.5, 2.5 }, 0);

                //画Y轴 
                dc.DrawLine(penAxis, new PointF(x0, y0), new PointF(x0, y0 + YHeight));
                dc.DrawLine(penAxis, new PointF(x0 + XWidth, y0), new PointF(x0 + XWidth, y0 + YHeight));
                //y轴文本 
                for (int i = 0; i <= YAxisCount; i++)
                {

                    string yLabel = ((yLabelMax - yLabelMin) / YAxisCount * i + yLabelMin).ToString(AxisYLabeleFormate);
                    System.Drawing.SizeF ftx = dc.MeasureString(yLabel, drawFont);  // 计算字符串所需要的大小
                    float yPos = y0 + YHeight - (YHeight / YAxisCount * i);
                    dc.DrawString(yLabel, drawFont, LabelColor, new PointF(x0 - YLabelLen - ftx.Width, yPos - ftx.Height / 2));
                    dc.DrawLine(penAxis, new PointF(x0 - YLabelLen, yPos), new PointF(x0, yPos));

                    //横向网格
                    PointF p1 = new PointF(x0, yPos);
                    PointF p2 = new PointF(x0 + XWidth, yPos);
                    dc.DrawLine(pen3, p1, p2);

                }

                //绘制纵向网格和x轴文本
                dc.DrawLine(penAxis, new PointF(x0, y0), new PointF(x0 + XWidth, y0));
                dc.DrawLine(penAxis, new PointF(x0, y0 + YHeight), new PointF(x0 + XWidth, y0 + YHeight));
                float stepX1 = XWidth / XAxisCount;//X 步进画布宽度
                float stepData = ((this.XLabelMax - this.XLabelMin) / (float)XAxisCount);//X 步进坐标数值

                for (int i = 1; i <= XAxisCount; i++)
                {
                    if (i * stepX1 > XWidth) continue;
                    //纵向网格
                    PointF p1 = new PointF(x0 + i * stepX1, y0 + YHeight);
                    PointF p2 = new PointF(x0 + i * stepX1, y0);
                    dc.DrawLine(pen3, p1, p2);
                    //x轴文本
                    string strAxisLabel = (this.XLabelMin + (i) * stepData).ToString(AxisXLabeleFormate);
                    System.Drawing.SizeF ftX = dc.MeasureString(strAxisLabel, drawFont);  // 计算字符串所需要的大小
                    PointF p3 = new PointF(x0 + i * stepX1 - ftX.Width / 2, YHeight + y0);
                    dc.DrawString(strAxisLabel, drawFont, LabelColor, p3);

                }

            }

            dc.Flush();
            dc.Dispose();
            dc = null;
            backBitmap.Dispose();
            backBitmap = null;



            Img.Dispatcher.BeginInvoke(new Action(() => {
                CacheBitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, (int)CacheBitmap.Width, (int)CacheBitmap.Height));
                CacheBitmap.Unlock();
            }));

        }

        public void AppendData(float x, List<float> yList)
        {


            //可以在线程中获取数据、生成图像内容，但是 显示图像内容的WriteableBitmap必须和主窗体属于同一线程，否则图像内容不能正常显示
            //采用的方法是：将WriteableBitmap的内存指针传递给线程
            Img.Dispatcher.BeginInvoke(new Action(() => {
                CacheBitmap.Lock();
            }));

            Bitmap backBitmap = new Bitmap(CacheBitmap.PixelWidth, CacheBitmap.PixelHeight, CacheBitmap.BackBufferStride, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, CacheBitmap.BackBuffer);
            Graphics dc = Graphics.FromImage(backBitmap);
            dc.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Font drawFont = new Font("Microsoft YaHei", 10);
            for (int count = 0; count < AllDatas.Count; count++)
            {
                try
                {
                    if (yList.Count > count) {
                        AllDatas[count].XData.Add(x);
                        AllDatas[count].RtData.Add(yList[count]);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }


            for (int count = 0; count < AllDatas.Count; count++)
            {

                //计算基础坐标系
                float x0, y0;
                x0 = XLeftMargin;
                y0 = YTopMargin;
                SeriseDatas datas = AllDatas[count];
                if (!datas.IsVisible) continue;
                if (datas.XData.Count < 2) continue;

                int i = datas.XData.Count - 1;

                //将数值转换成位置     
                float x_start = x0 + (datas.XData[i - 1] - XLabelMin) * RatioX;
                float y_start = y0 + YHeight - (datas.RtData[i - 1] - YLabelMin) * RatioY;
                float x_end = x0 + (datas.XData[i] - XLabelMin) * RatioX;
                float y_end = y0 + YHeight - (datas.RtData[i] - YLabelMin) * RatioY;

                PointF p1 = new PointF(x_start, y_start);
                PointF p2 = new PointF(x_end, y_end);
                //画线
                Pen linePen1 = new Pen(AllDatas[count].Line1Color, AllDatas[count].Thinkness);
                AppendLineSegment(p1, p2, dc, count, count == 0);

            }


            dc.Flush();
            dc.Dispose();
            dc = null;
            backBitmap.Dispose();
            backBitmap = null;

            Img.Dispatcher.BeginInvoke(new Action(() => {
                CacheBitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, (int)CacheBitmap.Width, (int)CacheBitmap.Height));
                CacheBitmap.Unlock();
            }));

        }


        /// <summary>
        ///  批量添加
        /// </summary>
        /// <param name="xs">横轴</param>
        /// <param name="yListList">纵轴</param>
        public void AppendDataList(List<float> xs, List<List<float>> yListList)
        {

            if (AllDatas.Count == 0 || xs.Count == 0) return;

            int indexFrom = AllDatas[0].XData.Count - 1;
            for (int s = 0; s < xs.Count; s++)
            {
                for (int count = 0; count < AllDatas.Count; count++)
                {
                    AllDatas[count].XData.Add(xs[s]);
                    AllDatas[count].RtData.Add(yListList[s][count]);
                }
            }
            int indexTo = AllDatas[0].XData.Count - 1;

            //可以在线程中获取数据、生成图像内容，但是 显示图像内容的WriteableBitmap必须和主窗体属于同一线程，否则图像内容不能正常显示
            //采用的方法是：将WriteableBitmap的内存指针传递给线程
            Img.Dispatcher.BeginInvoke(new Action(() => {
                CacheBitmap.Lock();
            }));

            Bitmap backBitmap = new Bitmap(CacheBitmap.PixelWidth, CacheBitmap.PixelHeight, CacheBitmap.BackBufferStride, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, CacheBitmap.BackBuffer);
            Graphics dc = Graphics.FromImage(backBitmap);

            Font drawFont = new Font("Microsoft YaHei", 10);
            for (int s = 0; s < xs.Count; s++)
            {
                for (int count = indexFrom; count <= indexTo; count++)
                {
                    AllDatas[count].XData.Add(xs[s]);
                    AllDatas[count].RtData.Add(yListList[s][count]);

                    //计算基础坐标系
                    float x0, y0;
                    x0 = XLeftMargin;
                    y0 = YTopMargin;
                    SeriseDatas datas = AllDatas[count];
                    if (!datas.IsVisible) continue;
                    if (datas.XData.Count < 2) continue;

                    //画线
                    Pen linePen1 = new Pen(AllDatas[count].Line1Color, AllDatas[count].Thinkness);
                    int i = datas.XData.Count - 1;

                    //将数值转换成位置     
                    float x_start = x0 + (datas.XData[i - 1] - XLabelMin) * RatioX;
                    float y_start = y0 + YHeight - (datas.RtData[i - 1] - YLabelMin) * RatioY;
                    float x_end = x0 + (datas.XData[i] - XLabelMin) * RatioX;
                    float y_end = y0 + YHeight - (datas.RtData[i] - YLabelMin) * RatioY;

                    PointF p1 = new PointF(x_start, y_start);
                    PointF p2 = new PointF(x_end, y_end);
                    AppendLineSegment(p1, p2, dc, count, s == 0);

                }

            }


            dc.Flush();
            dc.Dispose();
            dc = null;
            backBitmap.Dispose();
            backBitmap = null;

            Img.Dispatcher.BeginInvoke(new Action(() => {
                CacheBitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, (int)CacheBitmap.Width, (int)CacheBitmap.Height));
                CacheBitmap.Unlock();
            }));


        }

        public void AddStdLineX(Color color, double x)
        {
            if (!StdLineX.ContainsKey((float)x)) StdLineX.Add((float)x, color);
            Img.Dispatcher.BeginInvoke(new Action(() => {
                CacheBitmap.Lock();
            }));

            Bitmap backBitmap = new Bitmap(CacheBitmap.PixelWidth, CacheBitmap.PixelHeight, CacheBitmap.BackBufferStride, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, CacheBitmap.BackBuffer);
            Graphics dc = Graphics.FromImage(backBitmap);

            Pen penLine = new Pen(new SolidBrush(color), 1);
            penLine.DashStyle = DashStyle.DashDot;
            x = x * XWidth / (XLabelMax - XLabelMin) + XLabelMin + X0;
            dc.DrawLine(penLine, new PointF((float)x, Y0), new PointF((float)x, Y0 + YHeight));

            dc.Flush();
            dc.Dispose();
            dc = null;
            backBitmap.Dispose();
            backBitmap = null;

            Img.Dispatcher.BeginInvoke(new Action(() => {
                CacheBitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, (int)CacheBitmap.Width, (int)CacheBitmap.Height));
                CacheBitmap.Unlock();
            }));

        }

        public void AppendAll()
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();
            AppendAxises();
            var sw1 = sw.Elapsed.TotalSeconds.ToString("f2");

            foreach (var item in this.StdLineX.Keys)
            {
                this.AddStdLineX(StdLineX[item], item);
            }
            var sw2 = sw.Elapsed.TotalSeconds.ToString("f2");

            //可以在线程中获取数据、生成图像内容，但是 显示图像内容的WriteableBitmap必须和主窗体属于同一线程，否则图像内容不能正常显示
            //采用的方法是：将WriteableBitmap的内存指针传递给线程
            Img.Dispatcher.BeginInvoke(new Action(() => {
                CacheBitmap.Lock();
            }));

            Bitmap backBitmap = new Bitmap(CacheBitmap.PixelWidth, CacheBitmap.PixelHeight, CacheBitmap.BackBufferStride, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, CacheBitmap.BackBuffer);
            Graphics dc = Graphics.FromImage(backBitmap);

            var sw3 = sw.Elapsed.TotalSeconds.ToString("f2");
            Font drawFont = new Font("Microsoft YaHei", 10);
            for (int count = 0; count < AllDatas.Count; count++)
            {

                //计算基础坐标系
                float x0, y0;
                x0 = XLeftMargin;
                y0 = YTopMargin;
                SeriseDatas datas = AllDatas[count];
                if (!datas.IsVisible) continue;
                if (datas.XData.Count < 2) continue;

                //画线
                Pen linePen1 = new Pen(AllDatas[count].Line1Color, AllDatas[count].Thinkness);
                int StartIndex = datas.XData.FindIndex(w => w >= this.XLabelMin);
                int EndIndex = datas.XData.FindIndex(w => w >= this.XLabelMax);
                if (StartIndex < 1) StartIndex = 1;
                if (EndIndex < 0) EndIndex = datas.XData.Count - 1;

                for (int i = StartIndex; i <= EndIndex; i++)
                {
                    if (datas.RtData.Count > i)
                    {
                        //将数值转换成位置     
                        float x_start = x0 + (datas.XData[i - 1] - XLabelMin) * RatioX;
                        float y_start = y0 + YHeight - (datas.RtData[i - 1] - YLabelMin) * RatioY;
                        float x_end = x0 + (datas.XData[i] - XLabelMin) * RatioX;
                        float y_end = y0 + YHeight - (datas.RtData[i] - YLabelMin) * RatioY;

                        PointF p1 = new PointF(x_start, y_start);
                        PointF p2 = new PointF(x_end, y_end);
                        AppendLineSegment(p1, p2, dc, count, i == StartIndex);

                    }
                    else
                    {
                        break;
                    }
                }


            }

            dc.Flush();
            dc.Dispose();
            dc = null;
            backBitmap.Dispose();
            backBitmap = null;
            var sw4 = sw.Elapsed.TotalSeconds.ToString("f2");

            Img.Dispatcher.BeginInvoke(new Action(() => {
                CacheBitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, (int)CacheBitmap.Width, (int)CacheBitmap.Height));
                CacheBitmap.Unlock();
            }));
            var sw5 = sw.Elapsed.TotalSeconds.ToString("f2");

        }


        void AppendLineSegment(PointF p1, PointF p2, Graphics dc, int count, bool firstPoi)
        {

            RectangleF rect = new RectangleF(X0, Y0, XWidth, YHeight);
            bool StartWithIn = rect.Contains(p1);
            bool EndWithIn = rect.Contains(p2);
            if (!StartWithIn && !EndWithIn)
            {
                return;// 起点,终点不在画布范围
            }

            bool tempP1 = false;
            bool tempP2 = false;
            //如果 p1 p2 越界，需要计算跨界坐标
            if ((!StartWithIn && EndWithIn)
                ||
                (StartWithIn && !EndWithIn)
                )
            {
                PointF[] ps = GetIntersectionPoint(p1, p2, rect, StartWithIn, EndWithIn,ref tempP1,ref tempP2);
                if (ps == null) return;
                p1 = ps[0];
                p2 = ps[1];
            }
            float size = 2.3F;
            Pen linePen1 = new Pen(AllDatas[count].Line1Color, AllDatas[count].Thinkness);
            dc.DrawLine(linePen1, p1, p2);
            if (firstPoi && !tempP1 && ShowLinePoint) dc.FillEllipse(AllDatas[count].Line1Color, p1.X - size, p1.Y - size, size * 2, size * 2);//画填充椭圆的方法，x坐标、y坐标、宽、高，如果是100，则半径为50
            if (!tempP2 && ShowLinePoint) dc.FillEllipse(AllDatas[count].Line1Color, p2.X - size, p2.Y - size, size * 2, size * 2);//画填充椭圆的方法，x坐标、y坐标、宽、高，如果是100，则半径为50


        }


        /// <summary>
        /// 计算线段与画布交点 
        /// </summary>
        /// <param name="poiStart"> 点1坐标</param>
        /// <param name="poiEnd"> 点2坐标</param>
        /// <param name="lineSecondStar">L2的点1坐标</param>
        /// <param name="lineSecondEnd">L2的点2坐标</param>
        /// <returns></returns>
        public static PointF[] GetIntersectionPoint(PointF poiStart, PointF poiEnd, RectangleF rect, bool StartWithIn, bool EndWithIn,ref bool tempP1,ref bool tempP2)
        {
            float x0 = rect.X;
            float x1 = rect.X + rect.Width;
            float y0 = rect.Y;
            float y1 = rect.Y + rect.Height;

            if (poiStart.Y == poiEnd.Y)
            {
                //都平行X轴
                if (!StartWithIn && EndWithIn)
                {
                    poiStart.X = x0;
                }
                if (StartWithIn && !EndWithIn)
                {
                    poiEnd.X = x1;
                }
                return new PointF[] { poiStart, poiEnd };
            }

            if (poiStart.X == poiEnd.X)
            {
                //都平行Y轴，错误
                return new PointF[] { poiStart, poiEnd };
            }

            float a = 0;
            if (poiStart.X != poiEnd.X)
            {
                a = (poiEnd.Y - poiStart.Y) / (poiEnd.X - poiStart.X);
            }

            //起点不在画布范围,终点在
            if (!StartWithIn && EndWithIn)
            {
                float x = x0;
                float y = (poiStart.X - x) * (-a) + poiStart.Y;//左线交点
                if (!FloatWithIn(y, y0, y1))//左线交点越界，
                {
                    float x_bottom = poiStart.X + (y1 - poiStart.Y) / a;//下线交点
                    float x_top = poiStart.X + (y0 - poiStart.Y) / a;//上线交点
                    if (FloatWithIn(x_bottom, poiStart.X, poiEnd.X))
                    {
                        y = y1;
                        x = x_bottom;
                    }
                    if (FloatWithIn(x_top, poiStart.X, poiEnd.X))
                    {
                        y = y0;
                        x = x_top;
                    }
                }
                poiStart.X = x;
                poiStart.Y = y;
                tempP1 = true;
                return new PointF[] { poiStart, poiEnd };
            }

            //起点在,终点不在画布范围
            if (StartWithIn && !EndWithIn)
            {
                float x = x1;
                float y = (poiEnd.X - x) * (-a) + poiEnd.Y;//右线交点
                if (!FloatWithIn(y, y0, y1))//右线交点越界，
                {
                    float x_bottom = poiEnd.X + (y1 - poiEnd.Y) / a;//下线交点
                    float x_top = poiEnd.X + (y0 - poiEnd.Y) / a;//上线交点
                    if (FloatWithIn(x_bottom, poiStart.X, poiEnd.X))
                    {
                        y = y1;
                        x = x_bottom;
                    }
                    if (FloatWithIn(x_top, poiStart.X, poiEnd.X))
                    {
                        y = y0;
                        x = x_top;
                    }
                }
                poiEnd.X = x;
                poiEnd.Y = y;
                tempP2 = true;
                return new PointF[] { poiStart, poiEnd };
            }


            return new PointF[] { poiStart, poiEnd };
        }

        /// <summary>
        /// 计算两条直线的交点(标准版)
        /// </summary>
        /// <param name="lineFirstStar">L1的点1坐标</param>
        /// <param name="lineFirstEnd">L1的点2坐标</param>
        /// <param name="lineSecondStar">L2的点1坐标</param>
        /// <param name="lineSecondEnd">L2的点2坐标</param>
        /// <returns></returns>
        public static PointF GetIntersection(PointF lineFirstStar, PointF lineFirstEnd, PointF lineSecondStar, PointF lineSecondEnd)
        {
            /*
             * L1，L2都存在斜率的情况：
             * 直线方程L1: ( y - y1 ) / ( y2 - y1 ) = ( x - x1 ) / ( x2 - x1 ) 
             * => y = [ ( y2 - y1 ) / ( x2 - x1 ) ]( x - x1 ) + y1
             * 令 a = ( y2 - y1 ) / ( x2 - x1 )
             * 有 y = a * x - a * x1 + y1   .........1
             * 直线方程L2: ( y - y3 ) / ( y4 - y3 ) = ( x - x3 ) / ( x4 - x3 )
             * 令 b = ( y4 - y3 ) / ( x4 - x3 )
             * 有 y = b * x - b * x3 + y3 ..........2
             * 
             * 如果 a = b，则两直线平等，否则， 联解方程 1,2，得:
             * x = ( a * x1 - b * x3 - y1 + y3 ) / ( a - b )
             * y = a * x - a * x1 + y1
             * 
             * L1存在斜率, L2平行Y轴的情况：
             * x = x3
             * y = a * x3 - a * x1 + y1
             * 
             * L1 平行Y轴，L2存在斜率的情况：
             * x = x1
             * y = b * x - b * x3 + y3
             * 
             * L1与L2都平行Y轴的情况：
             * 如果 x1 = x3，那么L1与L2重合，否则平等
             * 
            */
            float a = 0, b = 0;
            int state = 0;
            if (lineFirstStar.X != lineFirstEnd.X)
            {
                a = (lineFirstEnd.Y - lineFirstStar.Y) / (lineFirstEnd.X - lineFirstStar.X);
                state |= 1;
            }
            if (lineSecondStar.X != lineSecondEnd.X)
            {
                b = (lineSecondEnd.Y - lineSecondStar.Y) / (lineSecondEnd.X - lineSecondStar.X);
                state |= 2;
            }
            switch (state)
            {
                case 0: //L1与L2都平行Y轴
                    {
                        if (lineFirstStar.X == lineSecondStar.X)
                        {
                            //throw new Exception("两条直线互相重合，且平行于Y轴，无法计算交点。");
                            return new PointF(0, 0);
                        }
                        else
                        {
                            //throw new Exception("两条直线互相平行，且平行于Y轴，无法计算交点。");
                            return new PointF(0, 0);
                        }
                    }
                case 1: //L1存在斜率, L2平行Y轴
                    {
                        float x = lineSecondStar.X;
                        float y = (lineFirstStar.X - x) * (-a) + lineFirstStar.Y;
                        return new PointF(x, y);
                    }
                case 2: //L1 平行Y轴，L2存在斜率
                    {
                        float x = lineFirstStar.X;
                        //网上有相似代码的，这一处是错误的。你可以对比case 1 的逻辑 进行分析
                        //源code:lineSecondStar * x + lineSecondStar * lineSecondStar.X + p3.Y;
                        float y = (lineSecondStar.X - x) * (-b) + lineSecondStar.Y;
                        return new PointF(x, y);
                    }
                case 3: //L1，L2都存在斜率
                    {
                        if (a == b)
                        {
                            // throw new Exception("两条直线平行或重合，无法计算交点。");
                            return new PointF(0, 0);
                        }
                        float x = (a * lineFirstStar.X - b * lineSecondStar.X - lineFirstStar.Y + lineSecondStar.Y) / (a - b);
                        float y = a * x - a * lineFirstStar.X + lineFirstStar.Y;
                        return new PointF(x, y);
                    }
            }
            // throw new Exception("不可能发生的情况");
            return new PointF(0, 0);
        }

        static bool FloatWithIn(float x, float a, float b)
        {
            if (x >= a && x <= b) return true;
            if (x >= b && x <= a) return true;
            return false;
        }
        /// <summary>
        ///  数据装画布坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public float[] ToPoint(float x, float y)
        {
            float x0, y0;
            x0 = XLeftMargin;
            y0 = YTopMargin;
            //将数值转换成位置     
            float x_start = XLeftMargin + (x - XLabelMin) * RatioX;
            float y_start = YTopMargin + YHeight - (y - YLabelMin) * RatioY;
            return new float[] { x_start, y_start };
        }
        /// <summary>
        ///  画布坐标转数据
        /// </summary>
        /// <param name="poi"></param>
        /// <returns></returns>
        public PointF ToData(float x, float y)
        {
            float xPoi = (this.XLabelMax - this.XLabelMin) / XWidth * (x - X0) + this.XLabelMin;
            float yPoi = this.YLabelMax - (this.YLabelMax - this.YLabelMin) / YHeight * (y - Y0);
            return new PointF(xPoi, yPoi);
        }
        /// <summary>
        /// 画布坐标转数据
        /// </summary>
        /// <param name="xCenter"></param>
        /// <param name="yCenter"></param>
        /// <param name="pixels"></param>
        /// <returns></returns>
        public RectangleF ToDataRectangle(float x, float y, float padding)
        {
            float xPoi1 = (this.XLabelMax - this.XLabelMin) / XWidth * (x - padding - X0) + this.XLabelMin;
            float yPoi1 = this.YLabelMax - (this.YLabelMax - this.YLabelMin) / YHeight * (y - padding - Y0);
            float xPoi2 = (this.XLabelMax - this.XLabelMin) / XWidth * (x + padding - X0) + this.XLabelMin;
            float yPoi2 = this.YLabelMax - (this.YLabelMax - this.YLabelMin) / YHeight * (y + padding - Y0);
            return new RectangleF(xPoi1, yPoi2, (xPoi2 - xPoi1), (yPoi1 - yPoi2));
        }

        #region demo
        public void StartDrawGraph()
        {
            Thread drawThread = new Thread(new ParameterizedThreadStart(DoDraw));

            //wBitmap.Lock();
            drawThread.Start(new object[] { CacheBitmap.BackBuffer, CacheBitmap.BackBufferStride, CacheBitmap.PixelWidth, CacheBitmap.PixelHeight });
        }

        public void DoDraw(object Parameter)
        {

            object[] domain = (object[])Parameter;
            while (Continued)
            {
                //可以在线程中获取数据、生成图像内容，但是 显示图像内容的WriteableBitmap必须和主窗体属于同一线程，否则图像内容不能正常显示
                //采用的方法是：将WriteableBitmap的内存指针传递给线程
                Img.Dispatcher.BeginInvoke(new Action(() => {
                    CacheBitmap.Lock();
                }));

                FillGraphToBitmap((IntPtr)domain[0], (int)domain[1], (int)domain[2], (int)domain[3]);

                Img.Dispatcher.BeginInvoke(new Action(() => {
                    CacheBitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, (int)CacheBitmap.Width, (int)CacheBitmap.Height));
                    CacheBitmap.Unlock();
                }));
                Thread.Sleep(100);
            }
        }

        public void FillGraphToBitmap(IntPtr WBmpBackBuffer, int WBmpBackBufferStride, int x, int y)
        {
            Bitmap backBitmap = new Bitmap(x, y, WBmpBackBufferStride, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, WBmpBackBuffer);
            Graphics graphics = Graphics.FromImage(backBitmap);
            //graphics.Clear(System.Drawing.Color.White);
            GraphicsPath path = new GraphicsPath();
            path.FillMode = FillMode.Winding;
            AddPolyLines(path, x, y);
            graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Color.Green, 1f), path);
            graphics.Flush();
            path.Dispose();
            path = null;
            graphics.Dispose();
            graphics = null;
            backBitmap.Dispose();
            backBitmap = null;
        }

        public void AddPolyLines(GraphicsPath gPath, int X, int Y)
        {
            Random rx = new Random();
            for (int i = 0; i < 30; i++)
            {
                PointF p1 = new PointF(rx.Next(X), rx.Next(Y));
                PointF p2 = new PointF(rx.Next(X), rx.Next(Y));
                gPath.AddLine(p1, p2);
            }
        }

        #endregion


    }

    public class HitTester
    {

        Zooming Control { get; set; }
        System.Timers.Timer timer = null;//悬停计时器
        DateTime ElapsedStart = DateTime.Now;
        TimeSpan Elapsed { get { return DateTime.Now - ElapsedStart; } }
        public bool Enabled
        {
            get { return timer.Enabled; }
            set
            {
                if (value)
                {
                    timer.Enabled = true;
                    ElapsedStart = DateTime.Now;
                }
                else
                {
                    timer.Enabled = false;
                }
            }
        }
        public HitTester(Zooming control)
        {
            Control = control;
        }

        public void Init()
        {

            if (timer != null)
            {
                timer.Enabled = false;
                timer.Elapsed -= HitTest;
                timer.Stop();
                timer = null;
            }
            timer = new System.Timers.Timer();
            timer.Elapsed -= HitTest;
            timer.Elapsed += HitTest;
            timer.Interval = 20;
            timer.Enabled = false;
            timer.Start();

            Control.PreviewMouseMove -= Control_PreviewMouseMove;
            Control.PreviewMouseMove += Control_PreviewMouseMove;

        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }
        float positionX; float positionY;
        private void Control_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            positionX = (float)e.GetPosition(Control).X;
            positionY = (float)e.GetPosition(Control).Y;
            if (positionX > this.Control.x0 && positionX < this.Control.x0 + this.Control.XWidth
                &&
                positionY > this.Control.y0 && positionY < this.Control.y0 + this.Control.YHeight)
            {
                //启动检测
                this.Enabled = true;
            }
            else
            {
                //越界停止
                this.Enabled = false;
            }


        }

        bool HitTestIng = false;
        void HitTest(object sender, System.Timers.ElapsedEventArgs e)
        {

            try
            {

                if (HitTestIng) return;//上次检测没完成，这vi不启动
                HitTestIng = true;


                if (!this.Control.IsMouseOver)
                {
                    //越界停止
                    this.Enabled = false;
                    return;
                }
                if (Control.mMode == Zooming.MouseMode.VIEW)
                {
                    //计算速度鼠标移动速度，如果速度过快 ，则不绘制 
                    double sec = this.Elapsed.TotalSeconds;
                    //Console.WriteLine(curTime);
                    if (sec > 0.2)
                    {

                        this.Enabled = false;//开始检测，不做重复检测
                        List<SeriseDatas> datas = this.Control.Chart.AllDatas;
                        if (datas.Count < 1) return;
                        PointF p2 = this.Control.Chart.ToData(positionX, positionY);
                        RectangleF rect = this.Control.Chart.ToDataRectangle(positionX, positionY, 3F);
                        int left = datas[0].XData.FindIndex(w => w >= rect.Left);
                        int right = datas[0].XData.FindLastIndex(w => w <= rect.Right);
                        if (left == -1 || right == -1) return;
                        if (left > right) return;
                        List<HitTestResult> res = new List<HitTestResult>();
                        for (int i = 0; i < datas.Count; i++)
                        {
                            SeriseDatas data = datas[i];
                            if (!data.IsVisible) continue;
                            for (int p = left; p <= right; p++)
                            {
                                PointF pTst = new PointF(data.XData[p], data.RtData[p]);
                                if (rect.Contains(pTst))
                                {
                                    HitTestResult r = new HitTestResult()
                                    {
                                        SeriseDatas = data,
                                        Index = p,
                                        Distance = (float)Math.Sqrt(Math.Abs(pTst.X - p2.X) * Math.Abs(pTst.X - p2.X) + Math.Abs(pTst.Y - p2.Y) * Math.Abs(pTst.Y - p2.Y))
                                    };
                                    res.Add(r);
                                }
                            }
                        }
                        if (res.Count > 0)
                        {
                            HitTestResult result = res.OrderBy(w => w.Distance).FirstOrDefault();
                            Control.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                this.Control.PolyTestResult(result);
                            }));
                        }

                    }
                    else
                    {
                        //未到检测时间,继续
                    }
                }
                else
                {
                    this.Enabled = false;//缩放功能，停止检测
                }

            }
            catch (Exception exe)
            {
                throw exe;
            }
            finally
            {
                HitTestIng = false;
            }




        }

        public class HitTestResult
        {
            public SeriseDatas SeriseDatas { get; set; }
            public int Index { get; set; }
            public float Distance { get; set; }
            public string Desc { get; set; }
        }
    }
    public class Zooming : System.Windows.Controls.Canvas
    {
        HitTester HitTester;
        private List<System.Windows.Media.Visual> visuals = new List<System.Windows.Media.Visual>();
        public PloyLineChart Chart { get; set; }
        public double x0 { get { return PloyLineChart.X0; } }
        public double y0 { get { return PloyLineChart.Y0; } }
        public double XWidth { get { return Chart.XWidth; } }
        public double YHeight { get { return Chart.YHeight; } }
        public enum MouseMode
        {
            ZOOM,
            VIEW
        }
        public MouseMode mMode = MouseMode.VIEW;
        private double canvasWidth;

        public Zooming()
        {
            this.Background = System.Windows.Media.Brushes.Transparent;
            this.SizeChanged += DrawingLine_SizeChanged;
        }

        private void DrawingLine_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Chart != null)
            {
                IniDrawingCanvas(this.Chart);
                Chart.AppendAll();
            }
        }

        public void IniDrawingCanvas(PloyLineChart _dc)
        {



            if (_dc == null) return;

            this.SizeChanged -= DrawingLine_SizeChanged;
            this.SizeChanged += DrawingLine_SizeChanged;
            this.Chart = _dc;

            this.PreviewMouseLeftButtonDown -= DrawingCanvas_MouseLeftButtonDown;
            this.PreviewMouseLeftButtonUp -= DrawingCanvas_MouseLeftButtonUp;
            this.MouseMove -= DrawingCanvas_MouseMove;
            this.MouseLeave -= DrawingCanvas_MouseLeave;

            this.PreviewMouseLeftButtonDown += DrawingCanvas_MouseLeftButtonDown;
            this.PreviewMouseLeftButtonUp += DrawingCanvas_MouseLeftButtonUp;
            this.MouseMove += DrawingCanvas_MouseMove;
            this.MouseLeave += DrawingCanvas_MouseLeave;
            this.MouseRightButtonUp += DrawingLine_MouseRightButtonUp;

            this.PreviewMouseWheel += Zooming_PreviewMouseWheel;

            HitTester = new HitTester(this);
            HitTester.Init();

        }

        private void Zooming_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {

        }

        private void DrawingLine_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (SeriseDatas data in Chart.AllDatas)
            {
                Chart.XLabelMin = PloyLineChart.XLabelMinDefault;// data.XData[data.XData.Count - 1];
                Chart.XLabelMax = PloyLineChart.XLabelMaxDefault;//.XData[0]; 
            }
            Chart.AppendAll();
            this.RemoveVisual(rectVisual);
            this.InvalidateVisual();
        }

        //鼠标离开画布
        private void DrawingCanvas_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (mMode == MouseMode.VIEW)
            {
                this.RemoveVisual(textVisual);
                this.InvalidateVisual();
            }
        }
        //选中放大区域完成，显示放大区域
        private void DrawingCanvas_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            try
            {
                isMouseDown = false;
                mMode = MouseMode.VIEW;
                endup = e.GetPosition(this).X;
                if (endup < x0)
                    endup = x0;
                if (endup > x0 + XWidth)
                    endup = x0 + XWidth;
                //重画 选中区域
                float len = Chart.XLabelMax - Chart.XLabelMin + 1;
                if (Chart.AllDatas.Count < 1 || len < 1)
                {
                    Chart.AppendAll();
                    this.RemoveVisual(rectVisual);
                    this.InvalidateVisual();
                    return;
                }

                double step = XWidth / len;
                if (isMouseMoved && Math.Abs(endup - startDown) > step)
                {
                    float startIndex = 0;
                    float endIndex = 0;
                    if (endup > startDown)
                    {
                        startIndex = (float)((startDown - x0) / step) + Chart.XLabelMin;
                        endIndex = (float)((endup - x0) / step) + Chart.XLabelMin;
                    }
                    else
                    {
                        startIndex = (float)((endup - x0) / step) + Chart.XLabelMin;
                        endIndex = (float)((startDown - x0) / step) + Chart.XLabelMin;
                    }

                    Chart.XLabelMax = endIndex;
                    Chart.XLabelMin = startIndex;

                    Chart.AppendAll();
                    this.RemoveVisual(rectVisual);
                    this.InvalidateVisual();
                }
                isMouseMoved = false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {

            }

        }
        private void DrawingCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            //清除焦点
            if (rectTestResult != null)
            {
                this.RemoveVisual(rectTestResult);
                rectTestResult = null;
            }

            double ax = e.GetPosition(this).X;
            double ay = e.GetPosition(this).Y;
            if (ax < x0 || ax > x0 + XWidth || ay < y0 || ay > y0 + YHeight) return;

            if (mMode == MouseMode.VIEW)
            {

                if (textVisual != null)
                {
                    this.RemoveVisual(textVisual);
                }
                textVisual = new System.Windows.Media.DrawingVisual();


                //尝试找到热点，找不到直接跳过
                PointF pd = this.Chart.ToData((float)ax, (float)ay);
                System.Windows.Media.DrawingContext dc = textVisual.RenderOpen();
                System.Windows.Media.Pen penLine = new System.Windows.Media.Pen(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red), 1);
                penLine.DashStyle = new System.Windows.Media.DashStyle(new double[] { 2.5, 2.5 }, 0);
                penLine.Freeze();
                System.Windows.Media.Pen pen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Transparent, 3);
                pen.Freeze();
                if (ax >= x0 && ax <= x0 + XWidth)
                {
                    dc.DrawLine(penLine, new System.Windows.Point(ax, y0), new System.Windows.Point(ax, y0 + YHeight));
                    System.Windows.Media.FormattedText ft =
                        new System.Windows.Media.FormattedText("X:" + pd.X.ToString(PloyLineChart.AxisXLabeleFormate) + "",
                            new System.Globalization.CultureInfo("zh-CHS", false), FlowDirection.LeftToRight, new System.Windows.Media.Typeface("Microsoft YaHei"), 15, System.Windows.Media.Brushes.White);
                    dc.DrawRectangle(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 0, 150, 179)), pen,
                        new System.Windows.Rect(new System.Windows.Point(ax - ft.Width / 2, y0 + YHeight), new System.Windows.Point(ax + ft.Width / 2, y0 + YHeight + ft.Height)));
                    dc.DrawText(ft, new System.Windows.Point(ax - ft.Width / 2, y0 + YHeight));
                }
                if (ay >= y0 && ay <= y0 + YHeight)
                {
                    dc.DrawLine(penLine, new System.Windows.Point(x0, ay), new System.Windows.Point(x0 + XWidth, ay));
                    System.Windows.Media.FormattedText ft =
                        new System.Windows.Media.FormattedText("Y:" + pd.Y.ToString(PloyLineChart.AxisYLabeleFormate) + "",
                            new System.Globalization.CultureInfo("zh-CHS", false), FlowDirection.LeftToRight, new System.Windows.Media.Typeface("Microsoft YaHei"), 15, System.Windows.Media.Brushes.White);
                    dc.DrawRectangle(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 0, 150, 179)), pen,
                        new System.Windows.Rect(new System.Windows.Point(x0 - ft.Width, ay - ft.Height / 2), new System.Windows.Point(x0, ay + ft.Height / 2)));
                    dc.DrawText(ft, new System.Windows.Point(x0 - ft.Width, ay - ft.Height / 2));
                }


                dc.Close();
                this.AddVisual(textVisual);
                this.InvalidateVisual();


            }
            else if (isMouseDown && startDown > x0 && startDown < XWidth + x0)
            {
                isMouseMoved = true;
                this.PolyRect(startDown, ax);
            }

        }

        private bool isMouseDown = false;
        private bool isMouseMoved = false;
        private double startDown;
        private double endup;
        private int clickTimes = 0;
        private void DrawingCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            isMouseDown = true;
            mMode = MouseMode.ZOOM;
            startDown = e.GetPosition(this).X;
            this.RemoveVisual(textVisual);
            this.InvalidateVisual();
            //双击
            clickTimes++;
            DispatcherTimer timer = new DispatcherTimer();

            timer.Interval = new TimeSpan(0, 0, 0, 0, 300);

            timer.Tick += (s, e1) => { timer.IsEnabled = false; clickTimes = 0; };

            timer.IsEnabled = true;

            if (clickTimes % 2 == 0)
            {
                //Console.WriteLine("double");
                timer.IsEnabled = false;

                clickTimes = 0;
                foreach (SeriseDatas data in Chart.AllDatas)
                {
                    if (data.XData.Count < 1) break;
                    Chart.XLabelMin = data.XData[0];
                    Chart.XLabelMax = data.XData[data.XData.Count - 1];
                }

                Chart.AppendAll();
                this.InvalidateVisual();

            }
        }


        //获取Visual的个数
        protected override int VisualChildrenCount
        {
            get { return visuals.Count; }
        }



        //获取Visual
        protected override System.Windows.Media.Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        //添加Visual
        public void AddVisual(System.Windows.Media.Visual visual)
        {
            visuals.Add(visual);

            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
        }

        //删除Visual
        public void RemoveVisual(System.Windows.Media.Visual visual)
        {
            visuals.Remove(visual);

            base.RemoveVisualChild(visual);
            base.RemoveLogicalChild(visual);
            //visual = null;
        }


        //绘制鼠标选择放大的矩形
        System.Windows.Media.DrawingVisual rectVisual = new System.Windows.Media.DrawingVisual();
        public void PolyRect(double startx, double endx)
        {
            if (rectVisual != null)
            {
                this.RemoveVisual(rectVisual);
            }
            rectVisual = new System.Windows.Media.DrawingVisual();
            System.Windows.Media.DrawingContext dc = rectVisual.RenderOpen();

            System.Windows.Media.Pen pen = new System.Windows.Media.Pen(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(100, 255, 200, 200)), 1);
            dc.DrawRectangle(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(100, 255, 200, 200)), pen, new System.Windows.Rect(new System.Windows.Point(startx, y0), new System.Windows.Point(endx, y0 + YHeight)));
            dc.Close();
            this.AddVisual(rectVisual);
            this.InvalidateVisual();
        }
        //绘制显示选中的数值
        System.Windows.Media.DrawingVisual textVisual = new System.Windows.Media.DrawingVisual();
        public void PolyText(double ax, double ay)
        {
            if (Chart.AllDatas.Count < 1)
                return;
            if (textVisual != null)
            {
                this.RemoveVisual(textVisual);
            }
            textVisual = new System.Windows.Media.DrawingVisual();

            PointF pd = this.Chart.ToData((float)ax, (float)ay);
            System.Windows.Media.DrawingContext dc = textVisual.RenderOpen();
            //float len = Chart.XLabelMax - Chart.XLabelMin;
            //double step = XWidth / len;
            //// Console.WriteLine("*****linestep:" + step);
            //int index = (int)((xPosition - x0) / step);
            //double ax = x0 + index * step + step;
            //竖线
            System.Windows.Media.Pen pen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Transparent, 3);
            pen.Freeze();
            System.Windows.Media.FormattedText[] ftXs = new System.Windows.Media.FormattedText[Chart.AllDatas.Count];

            if (Chart.AllDatas.Count > 0)
            {
                //int StartIndex = Chart.AllDatas[0].XData.FindIndex(w => w >= Chart.XLabelMin);
                //int EndIndex = Chart.AllDatas[0].XData.FindIndex(w => w >= Chart.XLabelMax);
                //if (StartIndex < 1) StartIndex = 1;
                //if (EndIndex < 0) EndIndex = Chart.AllDatas[0].XData.Count - 1;
                //for (int i = 0; i < Chart.AllDatas.Count; i++)
                //{
                //    int mIndex = index + StartIndex;
                //    if (Chart.AllDatas[i].XData.Count <= mIndex) break;
                //    ftXs[i] = new System.Windows.Media.FormattedText("X:" + Chart.AllDatas[i].XData[mIndex] + " 实测:" + Chart.AllDatas[i].RtData[mIndex] + " ", new System.Globalization.CultureInfo("zh-CHS", false), FlowDirection.LeftToRight, new System.Windows.Media.Typeface("Microsoft YaHei"), 15, System.Windows.Media.Brushes.White);
                //    //计算是否超出范围
                //    if (ax + ftXs[i].Width < x0 + XWidth)
                //    {
                //        dc.DrawRectangle(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 0, 150, 179)), pen, new System.Windows.Rect(new System.Windows.Point(ax, (i + 1) * y0 + i * Chart.YHeight), new System.Windows.Point(ax + ftXs[i].Width, (i + 1) * y0 + i * Chart.YHeight + ftXs[i].Height)));
                //        dc.DrawText(ftXs[i], new System.Windows.Point(ax, (i + 1) * y0 + i * Chart.YHeight));
                //    }
                //    else
                //    {
                //        dc.DrawRectangle(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 0, 150, 179)), pen, new Rect(new System.Windows.Point(ax - ftXs[i].Width, (i + 1) * y0 + i * Chart.YHeight), new System.Windows.Point(ax, (i + 1) * y0 + i * Chart.YHeight + ftXs[i].Height)));
                //        dc.DrawText(ftXs[i], new System.Windows.Point(ax - ftXs[i].Width, (i + 1) * y0 + i * Chart.YHeight));
                //    }
                //}
            }



            System.Windows.Media.Pen penLine = new System.Windows.Media.Pen(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.OrangeRed), 1);
            penLine.DashStyle = new System.Windows.Media.DashStyle(new double[] { 2.5, 2.5 }, 0);
            penLine.Freeze();
            if (ax >= x0 && ax <= x0 + XWidth)
            {
                dc.DrawLine(penLine, new System.Windows.Point(ax, y0), new System.Windows.Point(ax, y0 + YHeight));
                System.Windows.Media.FormattedText ft =
                    new System.Windows.Media.FormattedText("X:" + pd.X.ToString(PloyLineChart.AxisXLabeleFormate) + "",
                        new System.Globalization.CultureInfo("zh-CHS", false), FlowDirection.LeftToRight, new System.Windows.Media.Typeface("Microsoft YaHei"), 15, System.Windows.Media.Brushes.White);
                dc.DrawRectangle(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 0, 150, 179)), pen,
                    new System.Windows.Rect(new System.Windows.Point(ax, y0 - ft.Height), new System.Windows.Point(ax + ft.Width, y0)));
                dc.DrawText(ft, new System.Windows.Point(ax, y0 - ft.Height));
            }
            if (ay >= y0 && ay <= y0 + YHeight)
            {
                dc.DrawLine(penLine, new System.Windows.Point(x0, ay), new System.Windows.Point(x0 + XWidth, ay));
                System.Windows.Media.FormattedText ft =
                    new System.Windows.Media.FormattedText("Y:" + pd.Y.ToString(PloyLineChart.AxisYLabeleFormate) + "",
                        new System.Globalization.CultureInfo("zh-CHS", false), FlowDirection.LeftToRight, new System.Windows.Media.Typeface("Microsoft YaHei"), 15, System.Windows.Media.Brushes.White);
                dc.DrawRectangle(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 0, 150, 179)), pen,
                    new System.Windows.Rect(new System.Windows.Point(x0 - ft.Width, ay), new System.Windows.Point(x0, ay)));
                dc.DrawText(ft, new System.Windows.Point(x0 - ft.Width, ay));
            }
            dc.Close();
            this.AddVisual(textVisual);
            this.InvalidateVisual();
        }

        //绘制鼠标选择放大的矩形
        public event OnHitTestResult OnHitTestResult;
        System.Windows.Media.DrawingVisual rectTestResult = new System.Windows.Media.DrawingVisual();
        public void PolyTestResult(HitTester.HitTestResult result)
        {

            if (this.OnHitTestResult != null) this.OnHitTestResult(result);
            PointF pd = new PointF(result.SeriseDatas.XData[result.Index], result.SeriseDatas.RtData[result.Index]);
            float[] fs = this.Chart.ToPoint(pd.X, pd.Y);
            System.Windows.Rect rect = new System.Windows.Rect(new System.Windows.Point(fs[0] - 3, fs[1] - 3), new System.Windows.Size(6, 6));

            if (rectTestResult != null)
            {
                this.RemoveVisual(rectTestResult);
            }
            rectTestResult = new System.Windows.Media.DrawingVisual();
            System.Windows.Media.DrawingContext dc = rectTestResult.RenderOpen();

            //焦点方框
            System.Windows.Media.Pen pen = new System.Windows.Media.Pen(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.YellowGreen), 1.5);
            dc.DrawRectangle(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent), pen, rect);

            //焦点文字
            System.Windows.Media.Pen penLine = new System.Windows.Media.Pen(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red), 1);
            penLine.DashStyle = new System.Windows.Media.DashStyle(new double[] { 2.5, 2.5 }, 0);
            penLine.Freeze();
            pen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Transparent, 3);
            pen.Freeze();

            if (string.IsNullOrEmpty(result.Desc))
            {
                result.Desc = result.SeriseDatas.Title + ":X=" + pd.X.ToString(PloyLineChart.AxisXLabeleFormate) + "," + "Y=" + pd.Y.ToString(PloyLineChart.AxisYLabeleFormate);
            }

            float ax = fs[0];
            float ay = fs[1];
            System.Windows.Media.FormattedText ft =
                   new System.Windows.Media.FormattedText(result.Desc,
                       new System.Globalization.CultureInfo("zh-CHS", false), FlowDirection.LeftToRight, new System.Windows.Media.Typeface("Microsoft YaHei"), 15, System.Windows.Media.Brushes.White);
            dc.DrawRectangle(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 0, 150, 179)), pen,
                new System.Windows.Rect(new System.Windows.Point(ax + 3, ay + 3), new System.Windows.Point(ax + ft.Width + 3, ay + ft.Height + 3)));
            dc.DrawText(ft, new System.Windows.Point(ax + 3, ay + 3));

            dc.Close();
            this.AddVisual(rectTestResult);
            this.InvalidateVisual();
        }

    }
    public delegate void OnHitTestResult(HitTester.HitTestResult result);
    public class SeriseDatas
    {
        string title;
        float min;
        float max;
        List<float> xData;
        List<float> rtData;
        public bool IsVisible { get; set; } = true;

        private Brush line1Color = Brushes.Black;

        public Brush Line1Color
        {
            get
            {
                return line1Color;
            }

            set
            {
                line1Color = value;
            }
        }

        public float Thinkness { get; set; } = 1F;



        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }

        public List<float> RtData
        {
            get
            {
                return rtData;
            }

            set
            {
                rtData = value;
                if (value.Count == 0)
                {
                    min = 0;
                    max = 10;
                }
                else
                {
                    min = value.Min();
                    max = value.Max();
                }
            }
        }

        public List<float> XData
        {
            get
            {
                return xData;
            }

            set
            {
                xData = value;
            }
        }

        public SeriseDatas()
        {
            this.title = "";
            this.XData = new List<float>();
            this.RtData = new List<float>();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_title">数据标题</param>
        /// <param name="min">数据最小值</param>
        /// <param name="max">数据最大值</param>
        /// <param name="_xData">x坐标</param>
        /// <param name="rtdata">实测数据</param>
        /// <param name="thdata">理论数据</param>
        public SeriseDatas(string _title, float min, float max, List<float> _xData, List<float> rtdata, List<float> thdata)
        {
            this.title = _title;
            this.XData = _xData;
            this.RtData = rtdata;
        }
        public void clear()
        {
            this.XData.Clear();
            this.RtData.Clear();
        }

        public SeriseDatas(string _title, List<float> xx, List<float> yy)
        {
            this.title = _title;
            this.XData = new List<float>(xx);
            this.RtData = new List<float>(yy);
        }
    }


}