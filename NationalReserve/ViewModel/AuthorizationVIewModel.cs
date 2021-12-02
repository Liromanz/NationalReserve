using System.Net.Mime;
using System.Windows;
using NationalReserve.View;
using NationalReserve.View.Core;
using Authorization = NationalReserve.Model.Authorization;

namespace NationalReserve.ViewModel
{
    public class AuthorizationVIewModel : ObservableObject
    {
        #region Команды
        public RelayCommand LoginCommand { get; set; }

        #endregion

        #region Параметры
        private Authorization _authorization;

        public Authorization Authorization    
        {
            get => _authorization;
            set
            {
                _authorization = value; 
                OnPropertyChanged();
            }
        }

        #endregion

        public AuthorizationVIewModel()
        {
            OpenMainWindow();
            Authorization = new Authorization();
        }

        private void OpenMainWindow()
        {
            Application.Current.MainWindow = new MainWindow();
            Application.Current.MainWindow.Show();
            var a = Application.Current.Windows;
        }
    }
}
