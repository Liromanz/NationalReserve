using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NationalReserve.Model;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для StaffDocumentView.xaml
    /// </summary>
    public partial class StaffDocumentView : UserControl
    {
        private GridViewColumnHeader _sortedColumn;
        private bool _isAscending;
        public StaffDocumentViewModel ViewModel => DataContext as StaffDocumentViewModel;
        public StaffDocumentView()
        {
            InitializeComponent();
        }
        private void ColumnSorting(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;

            string sortBy = column.Tag.ToString();
            if (_sortedColumn == column && !_isAscending)
            {
                _isAscending = true;
                ViewModel.StaffDocuments = new ObservableCollection<StaffDocument>(
                    ViewModel.StaffDocuments.OrderBy(x => x.GetType().GetProperty(sortBy).GetValue(x, null)));
            }
            else
            {
                _sortedColumn = column;
                _isAscending = false;
                ViewModel.StaffDocuments = new ObservableCollection<StaffDocument>(
                    ViewModel.StaffDocuments.OrderByDescending(x => x.GetType().GetProperty(sortBy).GetValue(x, null)));
            }
        }
    }
}
