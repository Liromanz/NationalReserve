using System.Windows.Controls;
using System.Windows.Input;
using NationalReserve.Model;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для SecurityListView.xaml
    /// </summary>
    public partial class SecurityListView : UserControl
    {
        public SecurityListViewModel ViewModel => DataContext as SecurityListViewModel;
        public SecurityListView()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new SecurityList();
        }
    }
}
