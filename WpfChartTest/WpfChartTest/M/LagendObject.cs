using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InteractiveDataDisplay.WPF;

namespace WpfChartTest
{
    public class LagendObject : INotifyPropertyChanged
    {
        Grid lines1;
        public LineGraph LineGraph;
        public LagendObject(LineGraph l, PosEnum p, Grid grid)
        {
            LineGraph = l; lines1 = grid;
            PosEnum = p;
             
            LineGraph.Visibility = Visibility.Visible;
        }
        public PosEnum PosEnum { get; set; }
        int PosIndex { get { PosEnum p = PosEnum; return (((int)p) / 100 - 1) * 16 + ((int)p) % 100 - 1; } }
         
        public bool Checked
        {
            get
            {
                return LineGraph.Visibility == Visibility.Visible;
            }
            set
            {
                LineGraph.Visibility = value ? Visibility.Visible : Visibility.Hidden;
                if (value)
                {
                    if (!lines1.Children.Contains(LineGraph))
                    {
                        lines1.Children.Add(LineGraph);
                    }
                }
                else
                {
                    if (lines1.Children.Contains(LineGraph))
                    {
                        lines1.Children.Remove(LineGraph);
                    }
                }
                OnPropertyChanged("Checked"); 
            }
        }
        public Brush Stroke { get { return LineGraph.Stroke; } }
        public string Description { get { return LineGraph.Description; } }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string strPropertyInfo)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyInfo));
            }
        }
    }

}
