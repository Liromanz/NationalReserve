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
    }
}
