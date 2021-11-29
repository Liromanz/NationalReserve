using System.Windows.Controls;
using System.Windows.Input;
using NationalReserve.Model;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для SponsorshipView.xaml
    /// </summary>
    public partial class SponsorshipView : UserControl
    {
        public SponsorshipViewModel ViewModel => DataContext as SponsorshipViewModel;
        public SponsorshipView()
        {
            InitializeComponent();
        }

        private void MouseRightButtonDownCommand(object sender, MouseButtonEventArgs e)
        {
            (sender as ListView).SelectedItems.Clear();
            ViewModel.Selected = new Sponsorship();
        }
    }
}
