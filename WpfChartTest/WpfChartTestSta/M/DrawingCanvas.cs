using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;


namespace WpfChartTest
{
    public class DrawingCanvas : Canvas
    {
        private List<Visual> visuals = new List<Visual>();

        public const double YZero = 50;
        public const double YMargin = 50;
        private double YLabelLen = 5;
        private double yHeight;
        private double xWidth;
        private Brush labelColor = Brushes.Black;
        private Brush axisColor = Brushes.Black;
        private double thinkness = 0.5;
        private double thinknessAxis = 2;
        
        private double canvasWidth = 0;
        private const int yLinesCount = 12;

        List<LineDatas> allDatas;
        private double canvasHeight;
        public enum MouseMode
        {
            ZOOM,
            VIEW
        }
        public MouseMode mMode = MouseMode.VIEW;
        public DrawingCanvas()
        {
            this.Background = Brushes.Transparent;
            this.Background = Brushes.Yellow;
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Top;
            this.Margin = new Thickness(0, 0, 0, YMargin);
            AllDatas = new List<LineDatas>();
            
        }


        //获取Visual的个数
        protected override int VisualChildrenCount
        {
            get { return visuals.Count; }
        }

        //获取Visual
        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        //添加Visual
        public void AddVisual(Visual visual)
        {
            visuals.Add(visual);

            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
        }

        //删除Visual
        public void RemoveVisual(Visual visual)
        {
            visuals.Remove(visual);

            base.RemoveVisualChild(visual);
            base.RemoveLogicalChild(visual);
        }

        //命中测试
        public DrawingVisual GetVisual(Point point)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, point);
            return hitResult.VisualHit as DrawingVisual;
        }

        //使用DrawVisual画Polyline
        //DrawingVisual lineVisual;
        public void addData(string title, double min, double max, DoubleCollection xData, DoubleCollection mData, DoubleCollection sData)
        {
            AllDatas.Add(new LineDatas(title, min, max, xData, mData, sData));
        }
        public void removeFirst()
        {
            for (int i = 0; i < AllDatas.Count; i++)
            {
                AllDatas[i].RtData.RemoveAt(0);
            }
        }
        public void removeLast()
        {
            for (int i = 0; i < AllDatas.Count; i++)
            {
                AllDatas[i].RtData.RemoveAt(AllDatas[i].RtData.Count - 1);
            }
        }
        /// <summary>
        /// 添加数据点
        /// </summary>
        /// <param name="xdatas">x坐标</param>
        /// <param name="rtdatas">实测数据</param>
        /// <param name="thdatas">理论数据</param>
        public void addPoint(double[] xdatas, double[] rtdatas, double[] thdatas)
        {
            if (rtdatas.Length != allDatas.Count)
                throw new Exception() { Source = "数据个数不匹配" };
            for (int i = 0; i < allDatas.Count; i++)
            {
                AllDatas[i].XData.Insert(0, xdatas[i]);
                AllDatas[i].RtData.Insert(0, rtdatas[i]); 
                if (allDatas[i].Min > rtdatas[i])
                    allDatas[i].Min = rtdatas[i];
                if (allDatas[i].Max < rtdatas[i])
                    allDatas[i].Max = rtdatas[i];
            }
        }
        //清空所有数据
        public void clearData()
        {
            AllDatas.Clear();

        }
        //将数据清零
        public void cleanData()
        {
            foreach (LineDatas item in allDatas)
            {
                for (int i = 0; i < item.RtData.Count; i++)
                {
                    item.RtData[i] = 0; 
                }
                item.Min = -1;
                item.Max = 1;
            }
        }

        public void Polyline()
        {
            //如果虚画布没有数据
            if (this.visuals.Count == 0)
            {
                for (int i = 0; i < AllDatas.Count; i++)
                {
                    this.visuals.Add(new DrawingVisual());
                }
            }
            for (int count = 0; count < AllDatas.Count; count++)
            {
                //计算基础坐标系
                double x0, y0;
                x0 = YZero;
                y0 = (0 + 1) * YMargin + 0 * yHeight;
                string title = AllDatas[count].Title;

                LineDatas datas = allDatas[count];
                DrawingVisual lineVisual = (DrawingVisual)this.visuals[count];
                DrawingContext dc = lineVisual.RenderOpen();
                Pen penAxis = new Pen(AxisColor, ThinknessAxis);
                penAxis.Freeze();

                int yLabelMax = (int)AllDatas[count].Max + 1;
                int yLabelMin = (int)AllDatas[count].Min - 1;
                ////画标题
                //FormattedText fttitle = new FormattedText(title, new System.Globalization.CultureInfo("zh-CHS", false), FlowDirection.LeftToRight, new Typeface("Microsoft YaHei"), 15, Brushes.Black);
                //dc.DrawText(fttitle, new Point(xWidth / 2 + x0 - fttitle.Width / 2, y0 - fttitle.Height));

                if (count == 0) {
                    //画Y轴
                    dc.DrawLine(penAxis, new Point(x0, y0), new Point(x0, y0 + yHeight));
                    dc.DrawLine(penAxis, new Point(x0 - YLabelLen, y0), new Point(x0, y0));
                    dc.DrawLine(penAxis, new Point(x0 - YLabelLen, y0 + yHeight / 2), new Point(x0, y0 + yHeight / 2));
                    dc.DrawLine(penAxis, new Point(x0 - YLabelLen, y0 + yHeight), new Point(x0, y0 + yHeight));
                    //y轴文本
                    FormattedText ft1 = new FormattedText(yLabelMax.ToString(), new System.Globalization.CultureInfo("zh-CHS", false), FlowDirection.LeftToRight, new Typeface("Microsoft YaHei"), 10, LabelColor);
                    dc.DrawText(ft1, new Point(x0 - YLabelLen - ft1.Width, y0 - ft1.Height / 2));
                    FormattedText ft2 = new FormattedText(((yLabelMax + yLabelMin) / 2).ToString(), new System.Globalization.CultureInfo("zh-CHS", false), FlowDirection.LeftToRight, new Typeface("Microsoft YaHei"), 10, LabelColor);
                    dc.DrawText(ft2, new Point(x0 - YLabelLen - ft2.Width, y0 + YHeight / 2 - ft2.Height / 2));
                    FormattedText ft3 = new FormattedText(yLabelMin.ToString(), new System.Globalization.CultureInfo("zh-CHS", false), FlowDirection.LeftToRight, new Typeface("Microsoft YaHei"), 10, LabelColor);
                    dc.DrawText(ft3, new Point(x0 - YLabelLen - ft3.Width, y0 + yHeight - ft3.Height / 2));

                    //绘制纵向网格和x轴文本
                    dc.DrawLine(penAxis, new Point(x0, y0 + yHeight), new Point(x0 + xWidth, y0 + yHeight));
                    Pen pen3 = new Pen(axisColor, Thinkness);
                    pen3.DashStyle = new DashStyle(new double[] { 2.5, 2.5 }, 0);
                    pen3.Freeze();
                    double stepX = xWidth / yLinesCount;
                    int stepData = (datas.EndIndex - datas.StartIndex + 1) / yLinesCount;
                    if (stepData < 1)
                        stepData = 1;
                    for (int i = 1; i < yLinesCount; i++)
                    {
                        //纵向网格
                        Point p1 = new Point(x0 + i * stepX, y0 + yHeight);
                        Point p2 = new Point(x0 + i * stepX, y0);
                        dc.DrawLine(pen3, p1, p2);
                        if (datas.XData.Count > i * stepData) {
                            //x轴文本
                            FormattedText ftX = new FormattedText(datas.XData[i * stepData].ToString(), new System.Globalization.CultureInfo("zh-CHS", false), FlowDirection.LeftToRight, new Typeface("Microsoft YaHei"), 10, LabelColor);
                            Point p3 = new Point(x0 + i * stepX - ftX.Width / 2, yHeight + y0);
                            dc.DrawText(ftX, p3);
                        }
                        else
                        {
                            break;
                        }

                    }
                }
                

                //画线
                Pen linePen1 = new Pen(AllDatas[count].Line1Color, Thinkness);
                linePen1.Freeze();
                double ratio = (yLabelMax - yLabelMin) / yHeight;
                double step = xWidth / (datas.EndIndex - datas.StartIndex + 1);
                //Console.WriteLine("=======datastep:" + step);
                for (int i = datas.StartIndex; i < datas.EndIndex; i++)
                {
                    if (datas.RtData.Count > i + 1)
                    {
                        //将数值转换成位置    
                        //实测值
                        Point p1 = new Point(x0 + (i - datas.StartIndex) * step, y0 + (yLabelMax - datas.RtData[i]) / ratio);
                        Point p2 = new Point(x0 + (i - datas.StartIndex + 1) * step, y0 + (yLabelMax - datas.RtData[i + 1]) / ratio);
                        dc.DrawLine(linePen1, p1, p2);
                    }
                    else {
                        break;
                    }
                }


                dc.Close();
            }
            this.InvalidateVisual();
        }

        public double YHeight
        {
            get
            {
                return yHeight;
            }

            set
            {
                yHeight = value;
            }
        }



        public Brush LabelColor
        {
            get
            {
                return labelColor;
            }

            set
            {
                labelColor = value;
            }
        }

        public Brush AxisColor
        {
            get
            {
                return axisColor;
            }

            set
            {
                axisColor = value;
            }
        }

        public double ThinknessAxis
        {
            get
            {
                return thinknessAxis;
            }

            set
            {
                thinknessAxis = value;
            }
        }


        public double Thinkness
        {
            get
            {
                return thinkness;
            }

            set
            {
                thinkness = value;
            }
        }

        public static double YZero1
        {
            get
            {
                return YZero;
            }
        }

        public double CanvasWidth
        {
            get
            {
                return canvasWidth;
            }

            set
            {
                canvasWidth = value;
                this.Width = value;
                xWidth = value - YMargin - YZero;
            }
        }
         


        public double CanvasHeight
        {
            get
            {
                return canvasHeight;
            }

            set
            {
                canvasHeight = value; 
                YHeight = canvasHeight - YMargin ;
            }
        }


        public int DataCount
        {
            get
            {
                return this.allDatas.Count;
            }

        }

        public int RtCount
        {
            get
            {
                return allDatas.Max(w => w.RtData.Count);
            }
        }

        internal List<LineDatas> AllDatas
        {
            get
            {
                return allDatas;
            }

            set
            {
                allDatas = value;
            }
        }

    }
    public class DrawingLine : Canvas
    {
        private List<Visual> visuals = new List<Visual>();
        public DrawingCanvas drawingCanvas { get; set; }
        private double x0;
        private double y0;
        private double mWidth;
        private double mHeight;
        public enum MouseMode
        {
            ZOOM,
            VIEW
        }
        public MouseMode mMode = MouseMode.VIEW;
        private double canvasWidth;
        public DrawingLine()
        {
            this.Background = Brushes.Red;
            this.SizeChanged += DrawingLine_SizeChanged;
        }

        private void DrawingLine_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (drawingCanvas != null) {
                IniDrawingCanvas(this.drawingCanvas);
                drawingCanvas.Polyline();
            }
        }

        public void IniDrawingCanvas(DrawingCanvas _dc)
        {
            if (_dc == null) return;
            _dc.CanvasHeight = _dc.ActualHeight;
            _dc.CanvasWidth = _dc.ActualWidth;
            this.drawingCanvas = _dc;
            this.Background = Brushes.Transparent;
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Top;
            this.Height = drawingCanvas.CanvasHeight;
            this.Width = drawingCanvas.CanvasWidth;
            this.MWidth = drawingCanvas.CanvasWidth;
            this.Height = drawingCanvas.CanvasHeight;
            this.CanvasWidth = drawingCanvas.CanvasWidth;

            x0 = DrawingCanvas.YZero;
            y0 = DrawingCanvas.YMargin;

            mHeight = (_dc.YHeight);

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
            foreach (LineDatas data in drawingCanvas.AllDatas)
            {
                data.EndIndex = data.XData.Count -1;
                data.StartIndex = 0;
            }
            drawingCanvas.Polyline();
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
            if (endup > x0 + mWidth)
                endup = x0 + mWidth;
            //重画 选中区域
            int len = drawingCanvas.AllDatas[0].EndIndex - drawingCanvas.AllDatas[0].StartIndex + 1;
            if (drawingCanvas.AllDatas.Count < 1 || len < 1)
                return;
            double step = mWidth / len;
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

                foreach (LineDatas data in drawingCanvas.AllDatas)
                {
                    data.EndIndex = data.StartIndex + endIndex;
                    data.StartIndex = data.StartIndex + startIndex;
                }
                drawingCanvas.Polyline();
                this.RemoveVisual(rectVisual);
                this.InvalidateVisual();
            }
            isMouseMoved = false;
        }
        double preMoved = DrawingCanvas.YZero;
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        //long preTime = 0;
        private const long MIN_TIME = 15000;
        private void DrawingCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            double positionX = e.GetPosition(this).X;
            if (mMode == MouseMode.VIEW)
            {
                int len = drawingCanvas.AllDatas[0].EndIndex - drawingCanvas.AllDatas[0].StartIndex + 1;
                if (positionX > x0 && positionX < x0 + mWidth && Math.Abs(positionX - preMoved) >= mWidth / len)
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
            else if (isMouseDown && startDown > x0 && startDown < mWidth + x0)
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
                foreach (LineDatas data in drawingCanvas.AllDatas)
                {
                    data.StartIndex = 0;
                    data.EndIndex = data.RtData.Count - 1;
                }

                drawingCanvas.Polyline();
                drawingCanvas.InvalidateVisual();
                //this.InvalidateVisual();              
            }
        }


        //获取Visual的个数
        protected override int VisualChildrenCount
        {
            get { return visuals.Count; }
        }

        public double MWidth
        {
            get
            {
                return mWidth;
            }

            set
            {
                mWidth = value;
            }
        }

        public double MHeight
        {
            get
            {
                return mHeight;
            }

            set
            {
                mHeight = value;
            }
        }

        public double CanvasWidth
        {
            get
            {
                return canvasWidth;
            }

            set
            {
                canvasWidth = value;
                this.Width = value;
                mWidth = value - x0 - y0;
            }
        }



        //获取Visual
        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        //添加Visual
        public void AddVisual(Visual visual)
        {
            visuals.Add(visual);

            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
        }

        //删除Visual
        public void RemoveVisual(Visual visual)
        {
            visuals.Remove(visual);

            base.RemoveVisualChild(visual);
            base.RemoveLogicalChild(visual);
            //visual = null;
        }

        //命中测试
        public DrawingVisual GetVisual(Point point)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, point);
            return hitResult.VisualHit as DrawingVisual;
        }


        //绘制鼠标选择放大的矩形
        DrawingVisual rectVisual = new DrawingVisual();
        public void PolyRect(double startx, double endx)
        {
            if (rectVisual != null)
            {
                this.RemoveVisual(rectVisual);
            }
            rectVisual = new DrawingVisual();
            DrawingContext dc = rectVisual.RenderOpen();

            Pen pen = new Pen(new SolidColorBrush(Color.FromArgb(100, 255, 200, 200)), 1);
            dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(100, 255, 200, 200)), pen, new Rect(new Point(startx, y0), new Point(endx, y0 + mHeight)));
            dc.Close();
            this.AddVisual(rectVisual);
            this.InvalidateVisual();
        }
        //绘制显示选中的数值
        DrawingVisual textVisual = new DrawingVisual();
        public void PolyText(double xPosition)
        {
            if (drawingCanvas.AllDatas.Count < 1)
                return;
            if (textVisual != null)
            {
                this.RemoveVisual(textVisual);
            }
            textVisual = new DrawingVisual();

            DrawingContext dc = textVisual.RenderOpen();
            int len = drawingCanvas.AllDatas[0].EndIndex - drawingCanvas.AllDatas[0].StartIndex + 1;
            double step = mWidth / len;
            // Console.WriteLine("*****linestep:" + step);
            int index = (int)((xPosition - x0) / step);
            double ax = x0 + index * step;
            //竖线
            Pen pen = new Pen(Brushes.Transparent, 3);
            pen.Freeze();
            FormattedText[] ftXs = new FormattedText[drawingCanvas.AllDatas.Count];

            for (int i = 0; i < drawingCanvas.AllDatas.Count; i++)
            {
                int mIndex = index + drawingCanvas.AllDatas[i].StartIndex;
                if (drawingCanvas.AllDatas[i].XData.Count <= mIndex) break;
                ftXs[i] = new FormattedText("X:" + drawingCanvas.AllDatas[i].XData[mIndex] + " 实测:" + drawingCanvas.AllDatas[i].RtData[mIndex] + " " , new System.Globalization.CultureInfo("zh-CHS", false), FlowDirection.LeftToRight, new Typeface("Microsoft YaHei"), 15, Brushes.White);
                //计算是否超出范围
                if (ax + ftXs[i].Width < x0 + mWidth)
                {
                    dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(200, 0, 150, 179)), pen, new Rect(new Point(ax, (i + 1) * y0 + i * drawingCanvas.YHeight), new Point(ax + ftXs[i].Width, (i + 1) * y0 + i * drawingCanvas.YHeight + ftXs[i].Height)));
                    dc.DrawText(ftXs[i], new Point(ax, (i + 1) * y0 + i * drawingCanvas.YHeight));
                }
                else
                {
                    dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(200, 0, 150, 179)), pen, new Rect(new Point(ax - ftXs[i].Width, (i + 1) * y0 + i * drawingCanvas.YHeight), new Point(ax, (i + 1) * y0 + i * drawingCanvas.YHeight + ftXs[i].Height)));
                    dc.DrawText(ftXs[i], new Point(ax - ftXs[i].Width, (i + 1) * y0 + i * drawingCanvas.YHeight));
                }
            }
            Pen penLine = new Pen(new SolidColorBrush(Color.FromArgb(200, 0, 150, 179)), 3);
            penLine.DashStyle = new DashStyle(new double[] { 2.5, 2.5 }, 0);
            penLine.Freeze();
            dc.DrawLine(penLine, new Point(ax, y0), new Point(ax, y0 + mHeight));
            dc.Close();
            this.AddVisual(textVisual);
            this.InvalidateVisual();
        }

    }

    public class LineDatas
    {
        string title;
        double min;
        double max;
        DoubleCollection xData;
        DoubleCollection rtData;
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
        public double Min
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

        public double Max
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

        public DoubleCollection RtData
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

        public DoubleCollection XData
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

        public LineDatas()
        {
            this.title = "";
            this.Max = 0;
            this.Min = 0;
            this.XData = new DoubleCollection();
            this.RtData = new DoubleCollection(); 

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
        public LineDatas(string _title, double min, double max, DoubleCollection _xData, DoubleCollection rtdata, DoubleCollection thdata)
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

        public LineDatas(string _title, IEnumerable<double> xx, IEnumerable<double> yy)
        {
            this.title = _title;
            this.Min = yy.Min();
            this.Max = yy.Max();
            this.XData = new DoubleCollection(xx);
            this.RtData = new DoubleCollection(yy);
            this.StartIndex = 0;
            this.EndIndex = RtData.Count - 1;
        }
    }

}