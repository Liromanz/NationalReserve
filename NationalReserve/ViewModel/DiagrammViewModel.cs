using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
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
            get => _values;
            set
            {
                _values = value;
                OnPropertyChanged();
            }
        }

        private string _diagrammName;
        public string DiagrammName
        {
            get => _diagrammName;
            set
            {
                _diagrammName = value;
                OnPropertyChanged();
            }
        }

        public string AxisXName { get; set; }
        public string AxisYName { get; set; }

        public void MakeDiagramm(IEnumerable<int> valuesX, IEnumerable<int> valuesY)
        {
            var values = new ChartValues<Point>();

            foreach (var x in valuesX)
            {
                var point = new Point { X = x, Y = valuesY.Count(y => y == x) };
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
