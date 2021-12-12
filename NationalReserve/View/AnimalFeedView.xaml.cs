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
    /// Логика взаимодействия для AnimalFeedView.xaml
    /// </summary>
    public partial class AnimalFeedView : UserControl
    {
        private GridViewColumnHeader _sortedColumn;
        private bool _isAscending;
        public AnimalFeedViewModel ViewModel => DataContext as AnimalFeedViewModel;
        public AnimalFeedView()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new AnimalFeed();
        }
        private void ColumnSorting(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;

            string sortBy = column.Tag.ToString();
            if (_sortedColumn == column && !_isAscending)
            {
                _isAscending = true;
                ViewModel.AnimalFeeds = new ObservableCollection<AnimalFeed>(
                    ViewModel.AnimalFeeds.OrderBy(x => x.GetType().GetProperty(sortBy).GetValue(x, null)));
            }
            else
            {
                _sortedColumn = column;
                _isAscending = false;
                ViewModel.AnimalFeeds = new ObservableCollection<AnimalFeed>(
                    ViewModel.AnimalFeeds.OrderByDescending(x => x.GetType().GetProperty(sortBy).GetValue(x, null)));
            }
        }
    }
}
