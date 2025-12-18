using InteractiveDataDisplay.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfAppTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int lngData = 300;
        private int lngLineNum = 64;
        private int thisindex = 0;
        private List<double> lstX = new List<double>();

        public MainWindow()
        {
            InitializeComponent();
            DrawLineData();
        }

        private void DrawLineDataBak()
        {
            for (thisindex = 0; thisindex < lngData; thisindex++)
            {
                lstX.Add(3.1415 * thisindex / (lngData - 1));
            }

            for (int i = 0; i < lngLineNum; i++)
            {
                var lg = new LineGraph();
                lines.Children.Add(lg);
                lg.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, (byte)(i * 10), 0));
                lg.Description = String.Format("数据线 {0}", i + 1);
                lg.StrokeThickness = 1;
                lg.Plot(lstX,
                    lstX.Select(v => Math.Sin(v + i / 10.0)
                    ).ToArray()
                    );
            }
        }

        private void DrawLineData()
        {
            for (thisindex = 0; thisindex < lngData; thisindex++)
            {
                lstX.Add(0.5 * thisindex);
            }

            for (int i = 0; i < lngLineNum; i++)
            {
                var lg = new LineGraph();
                lines.Children.Add(lg);
                lg.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, (byte)(i * 10), 0));
                lg.Description = String.Format("数据线 {0}", i + 1);
                lg.StrokeThickness = 1;
                List<double> lstY = new List<double>();
                lstY = GetRandNums(0, 650, lngData);
                lg.Plot(lstX, lstY.ToArray() );
            }
        }

        public static List<double> GetRandNums(int min, int max, int num)

        {
            List<double> list = new List<double>();
            for (int i = 0; i < num; i++)
            {
                Random rd = new Random();
                ushort temp = (ushort)rd.Next(min, max);
                //while (list.Contains(temp))
                //{
                //	temp = (ushort)rd.Next(min, max);
                //}
                list.Add((double)temp);
            }

            return list;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    DrawLinePointData();

                    Thread.Sleep(1000);
                }
            }
            );
        }

        private void DrawLinePointData()
        {
            thisindex++;

            var lng = lngData - 1;
            if (lng == 1)
                lngData = 2000;

            double x = 1 + 3.1415 * (thisindex + 1) / (lng);

            Dispatcher.BeginInvoke(new Action(delegate
            {
                for (int i = 0; i < lngLineNum; i++)
                {
                    var ci = lines.Children.Count;
                    var ui = lines.Children[i];
                    var uit = ui.GetType().ToString();
                    var lg = (LineGraph)lines.Children[i];
                    lg.Points.RemoveAt(0);
                    lg.Points.Add(
                        new Point(
                            x,
                            Math.Sin(x + i / 10.0)
                            )
                        );
                }
            }));
        }
    }

    public class VisibilityToCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Visibility)value) == Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}