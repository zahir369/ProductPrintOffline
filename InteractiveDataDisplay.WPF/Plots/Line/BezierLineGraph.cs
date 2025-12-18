// Copyright (c) Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace InteractiveDataDisplay.WPF
{
    /// <summary>
    /// A plot to draw simple line.
    /// </summary>
    [Description("Plots BezierLine graph")]
    public class BezierLineGraph : Plot
    {
        private Path path;
        private PathFigure pathFigure;
        /// <summary>
        /// Gets or sets line graph points.
        /// </summary>
        [Category("InteractiveDataDisplay")]
        [Description("Line graph points")]
        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        private static void PointsPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BezierLineGraph linePlot = (BezierLineGraph)d;
            if (linePlot != null)
            {
                //InteractiveDataDisplay.WPF.Plot.SetPoints(linePlot.polyline, (PointCollection)e.NewValue);
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="BezierLineGraph"/> class.
        /// </summary>
        public BezierLineGraph()
        {
             
            path = new Path { StrokeThickness = 1, Stroke = new SolidColorBrush(Colors.Black) };
            Points = new PointCollection();
            BindingOperations.SetBinding(path, Polyline.StrokeThicknessProperty, new Binding("StrokeThickness") { Source = this });
            BindingOperations.SetBinding(this, PlotBase.PaddingProperty, new Binding("StrokeThickness") { Source = this, Converter = new BezierLineGraphThicknessConverter() });

            Children.Add(path);
        }
        static BezierLineGraph()
        {
            PointsProperty.OverrideMetadata(typeof(BezierLineGraph), new PropertyMetadata(new PointCollection(), PointsPropertyChangedHandler));
        }

        /// <summary>
        /// Updates data in <see cref="Points"/> and causes a redrawing of line graph.
        /// </summary>
        /// <param name="x">A set of x coordinates of new points.</param>
        /// <param name="y">A set of y coordinates of new points.</param>
        public void Plot(PointCollection li)
        {

            pathFigure = new PathFigure { StartPoint = li[0] };
            for (var i = 0; i < li.Count - 1; i++)
            {
                int current = i, last = i - 1, next = i + 1, next2 = i + 2;
                if (last == -1)
                {
                    last = 0;
                }
                if (next == li.Count)
                {
                    next = li.Count - 1;
                }
                if (next2 == li.Count)
                {
                    next2 = li.Count - 1;
                }
                var bzs = GetBezierSegment(li[current], li[last], li[next], li[next2]);
                pathFigure.Segments.Add(bzs);
                //画点的圆圈
                var lipt = li[i];
                Children.Add(new Ellipse { Width = 4, Height = 4, Margin = new Thickness(lipt.X - 2, lipt.Y - 2, 0, 0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Fill = Brushes.Red, ToolTip = string.Format("x:{0},y:{1} ", lipt.X, lipt.Y) });
            }

            //添加曲线到图上
            var pfc = new PathFigureCollection { pathFigure };
            var pg = new PathGeometry(pfc);
            var path = new Path { StrokeThickness = 1, Stroke = Brushes.Green, Data = pg };
            Children.Add(path);
            Points = li;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Plot(double x, double y)
        {

            Point lipt = new Point(
                Convert.ToDouble(x, CultureInfo.InvariantCulture), 
                Convert.ToDouble(y, CultureInfo.InvariantCulture));
            Points.Add(lipt);


            //画点的圆圈
            Children.Add(new Ellipse { Width = 4, Height = 4, Margin = new Thickness(lipt.X - 2, lipt.Y - 2, 0, 0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Fill = Brushes.Red, ToolTip = string.Format("x:{0},y:{1} ", lipt.X, lipt.Y) });

            var li = Points;
            if (Points.Count == 1) {
                pathFigure = new PathFigure { StartPoint = li[0] };
                //添加曲线到图上
                var pfc = new PathFigureCollection { pathFigure };
                var pg = new PathGeometry(pfc);
                var path = new Path { StrokeThickness = 1, Stroke = Brushes.Green, Data = pg };
                Children.Add(path);
            } 
            
            if (li.Count > 4) {
                int i = li.Count - 2 - 1;
                int current = i, last = i - 1, next = i + 1, next2 = i + 2;
                if (last == -1)
                {
                    last = 0;
                }
                if (next == li.Count)
                {
                    next = li.Count - 1;
                }
                if (next2 == li.Count)
                {
                    next2 = li.Count - 1;
                }
                var bzs = GetBezierSegment(li[current], li[last], li[next], li[next2]);
                pathFigure.Segments.Add(bzs);
            }

           

        }

        #region 
        /// <summary>
        /// 获得贝塞尔曲线
        /// </summary>
        /// <param name="currentPt">当前点</param>
        /// <param name="lastPt">上一个点</param>
        /// <param name="nextPt1">下一个点1</param>
        /// <param name="nextPt2">下一个点2</param>
        /// <returns></returns>
        private BezierSegment GetBezierSegment(Point currentPt, Point lastPt, Point nextPt1, Point nextPt2)
        {
            //计算中点
            var lastC = GetCenterPoint(lastPt, currentPt);
            var nextC1 = GetCenterPoint(currentPt, nextPt1); //贝塞尔控制点
            var nextC2 = GetCenterPoint(nextPt1, nextPt2);

            //计算相邻中点连线跟目的点的垂足
            //效果并不算太好，因为可能点在两个线上或者线的延长线上，计算会有误差
            //所以就直接使用中点平移方法。
            //var C1 = GetFootPoint(lastC, nextC1, currentPt);
            //var C2 = GetFootPoint(nextC1, nextC2, nextPt1);


            //计算“相邻中点”的中点
            var c1 = GetCenterPoint(lastC, nextC1);
            var c2 = GetCenterPoint(nextC1, nextC2);


            //计算【"中点"的中点】需要的点位移
            var controlPtOffset1 = currentPt - c1;
            var controlPtOffset2 = nextPt1 - c2;

            //移动控制点
            var controlPt1 = nextC1 + controlPtOffset1;
            var controlPt2 = nextC1 + controlPtOffset2;

            //如果觉得曲线幅度太大，可以将控制点向当前点靠近一定的系数。
            controlPt1 = controlPt1 + 0 * (currentPt - controlPt1);
            controlPt2 = controlPt2 + 0 * (nextPt1 - controlPt2);

            var bzs = new BezierSegment(controlPt1, controlPt2, nextPt1, true);
            return bzs;
        }

        /// <summary>
        ///     过c点做A和B连线的垂足
        /// </summary>
        /// <param name="aPoint"></param>
        /// <param name="bPoint"></param>
        /// <param name="cPoint"></param>
        /// <returns></returns>
        private Point GetFootPoint(Point aPoint, Point bPoint, Point cPoint)
        {
            //设三点坐标是A，B，C，AB构成直线，C是线外的点
            //三点对边距离是a,b,c,垂足为D，
            //根据距离推导公式得：AD距离是（b平方-a平方+c平方）/2c
            //本人数学不好，可能没考虑点c在线ab上的情况
            var offsetADist = (Math.Pow(cPoint.X - aPoint.X, 2) + Math.Pow(cPoint.Y - aPoint.Y, 2) - Math.Pow(bPoint.X - cPoint.X, 2) - Math.Pow(bPoint.Y - cPoint.Y, 2) + Math.Pow(aPoint.X - bPoint.X, 2) + Math.Pow(aPoint.Y - bPoint.Y, 2)) / (2 * GetDistance(aPoint, bPoint));

            var v = bPoint - aPoint;
            var distab = GetDistance(aPoint, bPoint);
            var offsetVector = v * offsetADist / distab;
            return aPoint + offsetVector;
        }

        private Point GetCenterPoint(Point pt1, Point pt2)
        {
            return new Point((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);
        }

        private double GetDistance(Point pt1, Point pt2)
        {
            return Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + Math.Pow(pt1.Y - pt2.Y, 2));
        }
        #endregion


        #region Description
        /// <summary>
        /// Identifies the <see cref="Description"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
           DependencyProperty.Register("Description",
           typeof(string),
           typeof(BezierLineGraph),
           new PropertyMetadata(null,
               (s, a) =>
               {
                   var lg = (BezierLineGraph)s;
                   ToolTipService.SetToolTip(lg, a.NewValue);
               }));

        /// <summary>
        /// Gets or sets description text for line graph. Description text appears in default
        /// legend and tooltip.
        /// </summary>
        [Category("InteractiveDataDisplay")]
        public string Description
        {
            get
            {
                return (string)GetValue(DescriptionProperty);
            }
            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

        #endregion

        #region Thickness
        /// <summary>
        /// Identifies the <see cref="Thickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
           DependencyProperty.Register("StrokeThickness",
           typeof(double),
           typeof(BezierLineGraph),
           new PropertyMetadata(1.0));

        /// <summary>
        /// Gets or sets the line thickness.
        /// </summary>
        /// <remarks>
        /// The default stroke thickness is 1.0
        /// </remarks>
        [Category("Appearance")]
        public double StrokeThickness
        {
            get
            {
                return (double)GetValue(StrokeThicknessProperty);
            }
            set
            {
                SetValue(StrokeThicknessProperty, value);
            }
        }
        #endregion

        #region Stroke

        /// <summary>
        /// Identifies the <see cref="Stroke"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
           DependencyProperty.Register("Stroke",
           typeof(Brush),
           typeof(BezierLineGraph),
           new PropertyMetadata(new SolidColorBrush(Colors.Black), OnStrokeChanged));

        private static void OnStrokeChanged(object target, DependencyPropertyChangedEventArgs e)
        {
            BezierLineGraph BezierLineGraph = (BezierLineGraph)target;
            BezierLineGraph.path.Stroke = e.NewValue as Brush;
        }

        /// <summary>
        /// Gets or sets the brush to draw the line.
        /// </summary>
        /// <remarks>
        /// The default color of stroke is black
        /// </remarks>
        [Category("Appearance")]
        public Brush Stroke
        {
            get
            {
                return (Brush)GetValue(StrokeProperty);
            }
            set
            {
                SetValue(StrokeProperty, value);
            }
        }
        #endregion

        #region StrokeDashArray

        private static DoubleCollection EmptyDoubleCollection
        {
            get
            {
                var result = new DoubleCollection(0);
                result.Freeze();
                return result;
            }
        }

        /// <summary>
        /// Identifies the <see cref="StrokeDashArray"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register("StrokeDashArray",
                typeof(DoubleCollection),
                typeof(BezierLineGraph),
                new PropertyMetadata(EmptyDoubleCollection, OnStrokeDashArrayChanged));

        private static void OnStrokeDashArrayChanged(object target, DependencyPropertyChangedEventArgs e)
        {
            BezierLineGraph BezierLineGraph = (BezierLineGraph)target;
            BezierLineGraph.path.StrokeDashArray = e.NewValue as DoubleCollection;
        }

        /// <summary>
        /// Gets or sets a collection of <see cref="Double"/> values that indicate the pattern of dashes and gaps that is used to draw the line.
        /// </summary>
        [Category("Appearance")]
        public DoubleCollection StrokeDashArray
        {
            get
            {
                return (DoubleCollection)GetValue(StrokeDashArrayProperty);
            }
            set
            {
                SetValue(StrokeDashArrayProperty, value);
            }
        }
        #endregion


    }

    internal class BezierLineGraphThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double thickness = (double)value;
            return new Thickness(thickness / 2.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

