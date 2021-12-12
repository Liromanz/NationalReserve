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
    /// Логика взаимодействия для CheckpointVIew.xaml
    /// </summary>
    public partial class CheckpointVIew : UserControl
    {
        private GridViewColumnHeader _sortedColumn;
        private bool _isAscending;
        public CheckpointViewModel ViewModel => DataContext as CheckpointViewModel;
        public CheckpointVIew()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new Checkpoint();
        }
        private void ColumnSorting(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;

            string sortBy = column.Tag.ToString();
            if (_sortedColumn == column && !_isAscending)
            {
                _isAscending = true;
                ViewModel.Checkpoints = new ObservableCollection<Checkpoint>(
                    ViewModel.Checkpoints.OrderBy(x => x.GetType().GetProperty(sortBy).GetValue(x, null)));
            }
            else
            {
                _sortedColumn = column;
                _isAscending = false;
                ViewModel.Checkpoints = new ObservableCollection<Checkpoint>(
                    ViewModel.Checkpoints.OrderByDescending(x => x.GetType().GetProperty(sortBy).GetValue(x, null)));
            }
        }
    }
}
