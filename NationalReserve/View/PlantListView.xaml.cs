using System.Windows.Controls;
using System.Windows.Input;
using NationalReserve.Model;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для PlantListView.xaml
    /// </summary>
    public partial class PlantListView : UserControl
    {
        public PlantListViewModel ViewModel => DataContext as PlantListViewModel;
        public PlantListView()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new PlantList();
        }
    }
}
