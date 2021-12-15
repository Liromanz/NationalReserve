using System;
using System.Windows;
using System.Windows.Input;
using NationalReserve.ViewModel;

namespace NationalReserve.View
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        public AuthorizationVIewModel ViewModel => DataContext as AuthorizationVIewModel;
        public Authorization()
        {
            InitializeComponent();
        }

        private void CloseApplication(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }

        private void DragWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void LoginThroughEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ViewModel.CheckLogin();
        }
    }
}
