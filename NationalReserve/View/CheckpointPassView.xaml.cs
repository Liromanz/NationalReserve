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
    /// Логика взаимодействия для CheckpointPassView.xaml
    /// </summary>
    public partial class CheckpointPassView : UserControl
    {
        private GridViewColumnHeader _sortedColumn;
        private bool _isAscending;
        public CheckpointPassViewModel ViewModel => DataContext as CheckpointPassViewModel;
        public CheckpointPassView()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new CheckpointPass { PassType = "Вход"};
        }
        private void ColumnSorting(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;

            string sortBy = column.Tag.ToString();
            if (_sortedColumn == column && !_isAscending)
            {
                _isAscending = true;
                ViewModel.CheckpointPasses = new ObservableCollection<CheckpointPass>(
                    ViewModel.CheckpointPasses.OrderBy(x => x.GetType().GetProperty(sortBy).GetValue(x, null)));
            }
            else
            {
                _sortedColumn = column;
                _isAscending = false;
                ViewModel.CheckpointPasses = new ObservableCollection<CheckpointPass>(
                    ViewModel.CheckpointPasses.OrderByDescending(x => x.GetType().GetProperty(sortBy).GetValue(x, null)));
            }
        }
    }
}
