using System;
using System.Windows;
using System.Windows.Input;
using NationalReserve.View.Core;

namespace NationalReserve.ViewModel
{

    class MainViewModel : ObservableObject
    {
        #region Команды
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand AnimalViewCommand { get; set; }
        #endregion

        #region Верстки
        public HumanViewModel HomeVM { get; set; }
        public AnimalViewModel AnimalVM { get; set; }
        #endregion

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            HomeVM = new HumanViewModel();
            AnimalVM = new AnimalViewModel();
            ChangeCurrentView(HomeVM);

            HomeViewCommand = new RelayCommand(o => { ChangeCurrentView(HomeVM); });
            AnimalViewCommand = new RelayCommand(o => { ChangeCurrentView(AnimalVM); });
        }

        private void ChangeCurrentView(object viewToChange)
        {
            CurrentView = viewToChange;
        }
    }
}