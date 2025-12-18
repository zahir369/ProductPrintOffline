using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;


namespace WpfChartTest.ChartV0
{

    public class PloyLineChart
    {

        public const float XLeftMargin = 50;
        public const float XRightMargin = 20;
        public const float YBottomMargin = 20;
        public const float YTopMargin = 20;
        private float YLabelLen = 8;
        private const int yLinesCount = 12;
        public bool Continued = true;
        Zooming Zooming { get; set; }
        System.Windows.Media.Imaging.WriteableBitmap CacheBitmap { get;  set; }
        public System.Windows.Controls.Image Img { get;private set; }
        public Brush LabelColor { get; set; } = new SolidBrush(Color.LightGray);
        public Brush AxisColor { get; set; } = new SolidBrush(Color.LightGray);
        public float ThinknessAxis { get; set; } = 2F;
        public float Thinkness { get; set; } = 0.5F;
        public float CanvasWidth { get; set; } = 100;
        public float CanvasHeight { get; set; } = 100;
        public static float X0 { get { return XLeftMargin; } }
        public static float Y0 { get { return YTopMargin; } }
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

        public PloyLineChart(System.Windows.Controls.Image iamge) {
            Img = iamge;
            AllDatas = new List<SeriseDatas>();
        }

        public void Init(Zooming z,int w,int h)
        {
            CacheBitmap = new System.Windows.Media.Imaging.WriteableBitmap(w,h, 96, 96, System.Windows.Media.PixelFormats.Pbgra32, null);
            Img.Source = CacheBitmap; 
            this.CanvasWidth = w;
            this.CanvasHeight = h;
            Zooming = z;
            Zooming.IniDrawingCanvas(this);
        }


        public void ReDrawAll()
        {

            //可以在线程中获取数据、生成图像内容，但是 显示图像内容的WriteableBitmap必须和主窗体属于同一线程，否则图像内容不能正常显示
            //采用的方法是：将WriteableBitmap的内存指针传递给线程
            Img.Dispatcher.BeginInvoke(new Action(() => {
                CacheBitmap.Lock();
            }));
             
            Bitmap backBitmap = new Bitmap(CacheBitmap.PixelWidth, CacheBitmap.PixelHeight, CacheBitmap.BackBufferStride, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, CacheBitmap.BackBuffer);
            Graphics dc = Graphics.FromImage(backBitmap);
            dc.Clear(System.Drawing.Color.White);

            Font drawFont = new Font("Microsoft YaHei", 10);
            for (int count = 0; count < AllDatas.Count; count++)
            {
                //计算基础坐标系
                float x0, y0;
                x0 = XLeftMargin;
                y0 = YTopMargin;
                string title = AllDatas[count].Title;

                SeriseDatas datas = AllDatas[count];
                 
                Pen penAxis = new Pen(AxisColor, ThinknessAxis);
                 
                int yLabelMax = (int)AllDatas[count].Max + 1;
                int yLabelMin = (int)AllDatas[count].Min - 1;
                ////画标题
                //FormattedText fttitle = new FormattedText(title, new System.Globalization.CultureInfo("zh-CHS", false), FlowDirection.LeftToRight, new Typeface("Microsoft YaHei"), 15, Brushes.Black);
                //dc.DrawText(fttitle, new PointF(xWidth / 2 + x0 - fttitle.Width / 2, y0 - fttitle.Height));

                if (count == 0)
                {
                    //画Y轴 
                    dc.DrawLine(penAxis, new PointF(x0, y0), new PointF(x0, y0 + YHeight));
                    dc.DrawLine(penAxis, new PointF(x0 - YLabelLen, y0), new PointF(x0, y0));
                    dc.DrawLine(penAxis, new PointF(x0 - YLabelLen, y0 + YHeight / 2), new PointF(x0, y0 + YHeight / 2));
                    dc.DrawLine(penAxis, new PointF(x0 - YLabelLen, y0 + YHeight), new PointF(x0, y0 + YHeight));
                    //y轴文本
                    System.Drawing.SizeF ft1 = dc.MeasureString(yLabelMax.ToString(), drawFont);  // 计算字符串所需要的大小
                    dc.DrawString(yLabelMax.ToString(), drawFont, LabelColor, new PointF(x0 - YLabelLen - ft1.Width, y0 - ft1.Height / 2));
                    System.Drawing.SizeF ft2 = dc.MeasureString(((yLabelMax + yLabelMin) / 2).ToString(), drawFont);  // 计算字符串所需要的大小
                    dc.DrawString(((yLabelMax + yLabelMin) / 2).ToString(), drawFont, LabelColor, new PointF(x0 - YLabelLen - ft2.Width, y0 + YHeight / 2 - ft2.Height / 2));
                    System.Drawing.SizeF ft3 = dc.MeasureString(yLabelMin.ToString(), drawFont);  // 计算字符串所需要的大小
                    dc.DrawString(yLabelMin.ToString(), drawFont, LabelColor, new PointF(x0 - YLabelLen - ft3.Width, y0 + YHeight - ft3.Height / 2));

                    //绘制纵向网格和x轴文本
                    dc.DrawLine(penAxis, new PointF(x0, y0 + YHeight), new PointF(x0 + XWidth, y0 + YHeight));
                    Pen pen3 = new Pen(AxisColor, Thinkness);
                    pen3.DashStyle = DashStyle.Dot;// new System.Drawing.Drawing2D.DashStyle(new float[] { 2.5, 2.5 }, 0);
                    float stepX = XWidth / yLinesCount;
                    int stepData = (datas.EndIndex - datas.StartIndex + 1) / yLinesCount;
                    if (stepData < 1)
                        stepData = 1;
                    for (int i = 1; i < yLinesCount; i++)
                    {
                        //纵向网格
                        PointF p1 = new PointF(x0 + i * stepX, y0 + YHeight);
                        PointF p2 = new PointF(x0 + i * stepX, y0);
                        dc.DrawLine(pen3, p1, p2);
                        if (datas.XData.Count > i * stepData)
                        {
                            //x轴文本
                            System.Drawing.SizeF ftX = dc.MeasureString(datas.XData[i * stepData].ToString(), drawFont);  // 计算字符串所需要的大小
                            PointF p3 = new PointF(x0 + i * stepX - ftX.Width / 2, YHeight + y0);
                            dc.DrawString((datas.XData[datas.StartIndex] + datas.XData[i * stepData]).ToString(), drawFont, LabelColor, p3);
                        }
                        else
                        {
                            break;
                        }

                    }
                }


                //画线
                Pen linePen1 = new Pen(AllDatas[count].Line1Color, Thinkness);
                float ratio = (yLabelMax - yLabelMin) / YHeight;
                float step = XWidth / (datas.EndIndex - datas.StartIndex + 1);
                //Console.WriteLine("=======datastep:" + step);
                for (int i = datas.StartIndex; i < datas.EndIndex; i++)
                {
                    if (datas.RtData.Count > i + 1)
                    {
                        //将数值转换成位置    
                        //实测值
                        PointF p1 = new PointF(x0 + (i - datas.StartIndex) * step, y0 + (yLabelMax - datas.RtData[i]) / ratio);
                        PointF p2 = new PointF(x0 + (i - datas.StartIndex + 1) * step, y0 + (yLabelMax - datas.RtData[i + 1]) / ratio);
                        dc.DrawLine(linePen1, p1, p2);
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



            Img.Dispatcher.BeginInvoke(new Action(() => {
                CacheBitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, (int)CacheBitmap.Width, (int)CacheBitmap.Height));
                CacheBitmap.Unlock();
            }));

        }


        #region demo
        public void StartDrawGraph(System.Windows.Controls.Image GraphPanel)
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

    public class Zooming : System.Windows.Controls.Canvas
    {
        private List<System.Windows.Media.Visual> visuals = new List<System.Windows.Media.Visual>();
        public PloyLineChart drawingCanvas { get; set; }
        private double x0 { get { return PloyLineChart.X0; } }
        private double y0 { get { return PloyLineChart.Y0; } }
        private double XWidth { get { return drawingCanvas.XWidth; } }
        private double YHeight { get { return drawingCanvas.YHeight; } }
        public enum MouseMode
        {
            ZOOM,
            VIEW
        }
        public MouseMode mMode = MouseMode.VIEW;
        private double canvasWidth;

        public Zooming( )
        {
            this.Background = System.Windows.Media.Brushes.Transparent;
            this.SizeChanged += DrawingLine_SizeChanged;
        }
         
        private void DrawingLine_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (drawingCanvas != null)
            {
                IniDrawingCanvas(this.drawingCanvas);
                drawingCanvas.ReDrawAll();
            }
        }

        public void IniDrawingCanvas(PloyLineChart _dc)
        {

            if (_dc == null) return;
            this.SizeChanged -= DrawingLine_SizeChanged;
            this.SizeChanged += DrawingLine_SizeChanged; 
            this.drawingCanvas = _dc;

            this.PreviewMouseLeftButtonDown -= DrawingCanvas_MouseLeftButtonDown;
            this.PreviewMouseLeftButtonUp -= DrawingCanvas_MouseLeftButtonUp;
            this.MouseMove -= DrawingCanvas_MouseMove;
            this.MouseLeave -= DrawingCanvas_MouseLeave;

            this.PreviewMouseLeftButtonDown += DrawingCanvas_MouseLeftButtonDown;
            this.PreviewMouseLeftButtonUp += DrawingCanvas_MouseLeftButtonUp;
            this.MouseMove += DrawingCanvas_MouseMove;
            this.MouseLeave += DrawingCanvas_MouseLeave;
            this.MouseRightButtonUp += DrawingLine_MouseRightButtonUp;

        }

        private void DrawingLine_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (SeriseDatas data in drawingCanvas.AllDatas)
            {
                data.EndIndex = data.XData.Count - 1;
                data.StartIndex = 0;
            }
            drawingCanvas.ReDrawAll();
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
            isMouseDown = false;
            mMode = MouseMode.VIEW;
            endup = e.GetPosition(this).X;
            if (endup < x0)
                endup = x0;
            if (endup > x0 + XWidth)
                endup = x0 + XWidth;
            //重画 选中区域
            int len = drawingCanvas.AllDatas[0].EndIndex - drawingCanvas.AllDatas[0].StartIndex + 1;
            if (drawingCanvas.AllDatas.Count < 1 || len < 1)
                return;
            double step = XWidth / len;
            if (isMouseMoved && Math.Abs(endup - startDown) > step)
            {
                int startIndex = 0;
                int endIndex = 0;
                if (endup > startDown)
                {
                    startIndex = (int)((startDown - x0) / step) + 1;
                    endIndex = (int)((endup - x0) / step);
                }
                else
                {
                    startIndex = (int)((endup - x0) / step) + 1;
                    endIndex = (int)((startDown - x0) / step);
                }

                foreach (SeriseDatas data in drawingCanvas.AllDatas)
                {
                    data.EndIndex = data.StartIndex + endIndex;
                    data.StartIndex = data.StartIndex + startIndex;
                }
                drawingCanvas.ReDrawAll();
                this.RemoveVisual(rectVisual);
                this.InvalidateVisual();
            }
            isMouseMoved = false;
        }
        double preMoved = PloyLineChart.Y0;
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        //long preTime = 0;
        private const long MIN_TIME = 15000;
        private void DrawingCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            double positionX = e.GetPosition(this).X;
            if (mMode == MouseMode.VIEW)
            {
                int len = drawingCanvas.AllDatas[0].EndIndex - drawingCanvas.AllDatas[0].StartIndex + 1;
                if (positionX > x0 && positionX < x0 + XWidth && Math.Abs(positionX - preMoved) >= XWidth / len)
                {
                    //计算速度鼠标移动速度，如果速度过快 ，则不绘制
                    if (stopwatch.IsRunning)
                    {
                        stopwatch.Stop();
                    }
                    long curTime = stopwatch.ElapsedTicks;
                    //Console.WriteLine(curTime);
                    if (curTime > MIN_TIME)
                    {
                        this.PolyText(positionX);
                    }
                    stopwatch.Restart();
                    preMoved = positionX;

                }
            }
            else if (isMouseDown && startDown > x0 && startDown < XWidth + x0)
            {
                isMouseMoved = true;
                this.PolyRect(startDown, positionX);
            }


            //stopwatch.Start();
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
                foreach (SeriseDatas data in drawingCanvas.AllDatas)
                {
                    data.StartIndex = 0;
                    data.EndIndex = data.RtData.Count - 1;
                }

                drawingCanvas.ReDrawAll(); 
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
        public void PolyText(double xPosition)
        {
            if (drawingCanvas.AllDatas.Count < 1)
                return;
            if (textVisual != null)
            {
                this.RemoveVisual(textVisual);
            }
            textVisual = new System.Windows.Media.DrawingVisual();

            System.Windows.Media.DrawingContext dc = textVisual.RenderOpen();
            int len = drawingCanvas.AllDatas[0].EndIndex - drawingCanvas.AllDatas[0].StartIndex + 1;
            double step = XWidth / len;
            // Console.WriteLine("*****linestep:" + step);
            int index = (int)((xPosition - x0) / step);
            double ax = x0 + index * step;
            //竖线
            System.Windows.Media.Pen pen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Transparent, 3);
            pen.Freeze();
            System.Windows.Media.FormattedText[] ftXs = new System.Windows.Media.FormattedText[drawingCanvas.AllDatas.Count];

            for (int i = 0; i < drawingCanvas.AllDatas.Count; i++)
            {
                int mIndex = index + drawingCanvas.AllDatas[i].StartIndex;
                if (drawingCanvas.AllDatas[i].XData.Count <= mIndex) break;
                ftXs[i] = new System.Windows.Media.FormattedText("X:" + drawingCanvas.AllDatas[i].XData[mIndex] + " 实测:" + drawingCanvas.AllDatas[i].RtData[mIndex] + " ", new System.Globalization.CultureInfo("zh-CHS", false), FlowDirection.LeftToRight, new System.Windows.Media.Typeface("Microsoft YaHei"), 15, System.Windows.Media.Brushes.White);
                //计算是否超出范围
                if (ax + ftXs[i].Width < x0 + XWidth)
                {
                    dc.DrawRectangle(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 0, 150, 179)), pen, new System.Windows.Rect(new System.Windows.Point(ax, (i + 1) * y0 + i * drawingCanvas.YHeight), new System.Windows.Point(ax + ftXs[i].Width, (i + 1) * y0 + i * drawingCanvas.YHeight + ftXs[i].Height)));
                    dc.DrawText(ftXs[i], new System.Windows.Point(ax, (i + 1) * y0 + i * drawingCanvas.YHeight));
                }
                else
                {
                    dc.DrawRectangle(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 0, 150, 179)), pen, new Rect(new System.Windows.Point(ax - ftXs[i].Width, (i + 1) * y0 + i * drawingCanvas.YHeight), new System.Windows.Point(ax, (i + 1) * y0 + i * drawingCanvas.YHeight + ftXs[i].Height)));
                    dc.DrawText(ftXs[i], new System.Windows.Point(ax - ftXs[i].Width, (i + 1) * y0 + i * drawingCanvas.YHeight));
                }
            }
            System.Windows.Media.Pen penLine = new System.Windows.Media.Pen(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 0, 150, 179)), 3);
            penLine.DashStyle = new System.Windows.Media.DashStyle(new double[] { 2.5, 2.5 }, 0);
            penLine.Freeze();
            dc.DrawLine(penLine, new System.Windows.Point(ax, y0), new System.Windows.Point(ax, y0 + YHeight));
            dc.Close();
            this.AddVisual(textVisual);
            this.InvalidateVisual();
        }

    }

    public class SeriseDatas
    {
        string title;
        float min;
        float max;
        List<float> xData;
        List<float> rtData;
        int startIndex;
        int endIndex;
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
        public float Min
        {
            get
            {
                return min;
            }

            set
            {
                min = value;
            }
        }

        public float Max
        {
            get
            {
                return max;
            }

            set
            {
                max = value;
            }
        }



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

        public int StartIndex
        {
            get
            {
                return startIndex;
            }

            set
            {
                startIndex = value;
            }
        }

        public int EndIndex
        {
            get
            {
                return endIndex;
            }

            set
            {
                endIndex = value;
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
            this.Max = 0;
            this.Min = 0;
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
            this.Min = min;
            this.Max = max;
            this.XData = _xData;
            this.RtData = rtdata;
            this.StartIndex = 0;
            this.EndIndex = rtData.Count - 1;
        }
        public void clear()
        {
            this.XData.Clear();
            this.RtData.Clear();
        }

        public SeriseDatas(string _title, List<float> xx, List<float> yy)
        {
            this.title = _title;
            this.Min = yy.Min();
            this.Max = yy.Max();
            this.XData = new List<float>(xx);
            this.RtData = new List<float>(yy);
            this.StartIndex = 0;
            this.EndIndex = RtData.Count - 1;
        }
    }


}