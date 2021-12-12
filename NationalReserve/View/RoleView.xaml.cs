using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NationalReserve.Model;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для RoleView.xaml
    /// </summary>
    public partial class RoleView : UserControl
    {
        private GridViewColumnHeader _sortedColumn;
        private bool isAscending;
        public RoleViewModel ViewModel => DataContext as RoleViewModel;
        public RoleView()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new Role();
        }
        private void ColumnSorting(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;

            string sortBy = column.Tag.ToString();
            if (_sortedColumn == column && !isAscending)
            {
                isAscending = true;
                ViewModel.Roles = new ObservableCollection<Role>(
                    ViewModel.Roles.OrderBy(x => x.GetType().GetProperty(sortBy).GetValue(x, null)));
            }
            else
            {
                _sortedColumn = column;
                isAscending = false;
                ViewModel.Roles = new ObservableCollection<Role>(
                    ViewModel.Roles.OrderByDescending(x => x.GetType().GetProperty(sortBy).GetValue(x, null)));
            }
        }
    }
}
