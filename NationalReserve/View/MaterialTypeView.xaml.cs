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
    /// Логика взаимодействия для MaterialTypeView.xaml
    /// </summary>
    public partial class MaterialTypeView : UserControl
    {
        private GridViewColumnHeader _sortedColumn;
        private bool _isAscending;
        public MaterialTypeViewModel ViewModel => DataContext as MaterialTypeViewModel;
        public MaterialTypeView()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new MaterialType();
        }
        private void ColumnSorting(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;

            string sortBy = column.Tag.ToString();
            if (_sortedColumn == column && !_isAscending)
            {
                _isAscending = true;
                ViewModel.MaterialTypes = new ObservableCollection<MaterialType>(
                    ViewModel.MaterialTypes.OrderBy(x => x.GetType().GetProperty(sortBy).GetValue(x, null)));
            }
            else
            {
                _sortedColumn = column;
                _isAscending = false;
                ViewModel.MaterialTypes = new ObservableCollection<MaterialType>(
                    ViewModel.MaterialTypes.OrderByDescending(x => x.GetType().GetProperty(sortBy).GetValue(x, null)));
            }
        }
    }
}
