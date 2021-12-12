using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NationalReserve.Model;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для AnimalTypeView.xaml
    /// </summary>
    public partial class AnimalTypeView : UserControl
    {
        private GridViewColumnHeader _sortedColumn;
        private bool _isAscending;
        public AnimalTypeViewModel ViewModel => DataContext as AnimalTypeViewModel;
        public AnimalTypeView()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new AnimalType();
        }

        private void ColumnSorting(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;

            string sortBy = column.Tag.ToString();
            if (_sortedColumn == column && !_isAscending)
            {
                _isAscending = true;
                ViewModel.AnimalTypes = new ObservableCollection<AnimalType>(
                    ViewModel.AnimalTypes.OrderBy(x => x.GetType().GetProperty(sortBy).GetValue(x, null)));
            }
            else
            {
                _sortedColumn = column;
                _isAscending = false;
                ViewModel.AnimalTypes = new ObservableCollection<AnimalType>(
                    ViewModel.AnimalTypes.OrderByDescending(x => x.GetType().GetProperty(sortBy).GetValue(x, null)));
            }
        }
    }
}
