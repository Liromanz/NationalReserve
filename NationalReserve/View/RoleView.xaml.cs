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
    }
}
