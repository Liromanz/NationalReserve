using System.Linq;
using System.Windows;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using NationalReserve.Helpers;
using NationalReserve.Model;
using NationalReserve.View.Core;

namespace NationalReserve.ViewModel
{
    public class DiagrammViewModel : ObservableObject
    {
        private SeriesCollection _values;

        public SeriesCollection Values
        {
            get { return _values; }
            set
            {
                _values = value;
                OnPropertyChanged();
            }
        }

        public DiagrammViewModel()
        {
            Init();
        }

        private async void Init()
        {
            var fullTableList = await ApiConnector.GetAll<CheckpointPass>("CheckpointPasses");
            var AxisY = fullTableList.Select(x => x.PassTime.Day);
            var AxisX = fullTableList.Select(x => x.PassTime.Day).ToHashSet().OrderBy(x => x);

            var values = new ChartValues<Point>();

            foreach (var x in AxisX)
            {
                var point = new Point { X = x, Y = AxisY.Count(y => y == x) };
                values.Add(point);
            }
            Values = new SeriesCollection
            {
                new LineSeries
                {
                    Configuration = new CartesianMapper<Point>()
                        .X(point => point.X)
                        .Y(point => point.Y),
                    Values = values,

                    LineSmoothness = 0,
                    StrokeThickness = 2,
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.White,
                    PointGeometrySize = 8
                }
            };
        }

    }
}
