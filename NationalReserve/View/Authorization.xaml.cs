using System;
using System.Windows;
using System.Windows.Input;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private void CloseApplication(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
