﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mime;
using System.Windows;
using NationalReserve.Helpers;
using NationalReserve.Model;
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

        private ObservableCollection<Human> _humans;

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
            InitAsync();
        }

        private async void InitAsync()
        {
            Authorization = new Authorization();
            LoginCommand = new RelayCommand(o => { CheckLogin(); });
            _humans =  await ApiConnector.GetAll<Human>("Humen");
        }

        private void CheckLogin()
        {
            if (_humans.FirstOrDefault(x => x.Login == Authorization.Login && x.Password == SecureData.Hash(Authorization.Password)) !=
                null)
                OpenMainWindow();
            else
                MessageBox.Show(GlobalConstants.LoginPasswordError);
        }

        private void OpenMainWindow()
        {
            var currentWindow = Application.Current.MainWindow;
            Application.Current.MainWindow = new MainWindow();
            Application.Current.MainWindow.Show();
            currentWindow.Close();
        }
    }
}
