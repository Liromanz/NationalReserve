using System.Windows;
using System.Windows.Input;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для HelperWindow.xaml
    /// </summary>
    public partial class HelperWindow : Window
    {
        public HelperWindow()
        {
            InitializeComponent();
        }

        private void CloseDialog(object sender, MouseButtonEventArgs e) => Close();

        private void DragWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
