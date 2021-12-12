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
    /// Логика взаимодействия для PaymentTypeView.xaml
    /// </summary>
    public partial class PaymentTypeView : UserControl
    {
        private GridViewColumnHeader _sortedColumn;
        private bool _isAscending;
        public PaymentTypeViewModel ViewModel => DataContext as PaymentTypeViewModel;
        public PaymentTypeView()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new PaymentType();
        }
        private void ColumnSorting(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;

            string sortBy = column.Tag.ToString();
            if (_sortedColumn == column && !_isAscending)
            {
                _isAscending = true;
                ViewModel.PaymentTypes = new ObservableCollection<PaymentType>(
                    ViewModel.PaymentTypes.OrderBy(x => x.GetType().GetProperty(sortBy).GetValue(x, null)));
            }
            else
            {
                _sortedColumn = column;
                _isAscending = false;
                ViewModel.PaymentTypes = new ObservableCollection<PaymentType>(
                    ViewModel.PaymentTypes.OrderByDescending(x => x.GetType().GetProperty(sortBy).GetValue(x, null)));
            }
        }
    }
}
