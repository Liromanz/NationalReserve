using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NationalReserve.Helpers;
using NationalReserve.Helpers.Interface;
using NationalReserve.Model;
using NationalReserve.View.Core;

namespace NationalReserve.ViewModel
{
    public class AnimalTypeViewModel : ObservableObject, IDataHandler
    {
        #region Команды
        public RelayCommand AddCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand LogicalDeleteCommand { get; set; }
        public RelayCommand LogicalRecoverCommand { get; set; }

        #endregion

        #region Измененные данные

        public ObservableCollection<AnimalType> UpdatedCollection { get; set; }
        public ObservableCollection<AnimalType> AddedCollection { get; set; }

        private ObservableCollection<AnimalType> _deletedCollection;
        public ObservableCollection<AnimalType> DeletedCollection
        {
            get => _deletedCollection;
            set
            {
                _deletedCollection = value;
                OnPropertyChanged();
            }
        }

        private AnimalType _deleted;
        public AnimalType Deleted
        {
            get => _deleted;
            set
            {
                _deleted = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Данные с верстки

        private ObservableCollection<AnimalType> _animalTypes;
        public ObservableCollection<AnimalType> AnimalTypes
        {
            get => _animalTypes;
            set
            {
                _animalTypes = value;
                OnPropertyChanged();
            }
        }

        private AnimalType _animalType;
        public AnimalType AnimalType
        {
            get => _animalType;
            set
            {
                if (value != null)
                    UpdatedCollection.Add(value);

                _animalType = value;
                OnPropertyChanged();
            }
        }

        private AnimalType _selected;
        public AnimalType Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (value != null)
                {
                    AnimalType = value;
                }
                OnPropertyChanged();
            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public AnimalTypeViewModel()
        {
            IsBusy = true;
            InitAsync();
            ReadAsync();
        }

        private void InitAsync()
        {
            AddCommand = new RelayCommand(o => { AddObject(); });
            SaveCommand = new RelayCommand(o => { SaveAsync(); });
            LogicalDeleteCommand = new RelayCommand(o => { LogicalDelete(); });
            LogicalRecoverCommand = new RelayCommand(o => { LogicalRecover(); });
        }

        #region CRUD

        public async void ReadAsync()
        {
            AddedCollection = new ObservableCollection<AnimalType>();
            UpdatedCollection = new ObservableCollection<AnimalType>();
            DeletedCollection = new ObservableCollection<AnimalType>();

            AnimalType = new AnimalType();
            AnimalTypes = await ApiConnector.GetAll<AnimalType>("AnimalTypes");

            IsBusy = false;
        }

        public void AddObject()
        {
            if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message);
                return;
            }

            AnimalType.Id = null;
            AnimalTypes.Add(AnimalType);
            AddedCollection.Add(AnimalType);

            Selected = new AnimalType();
        }

        public void LogicalDelete()
        {
            if (Selected?.Id != null)
            {
                DeletedCollection.Add(Selected);
                AnimalTypes.Remove(Selected);

                Selected = new AnimalType();
            }
        }

        public void LogicalRecover()
        {
            if (Deleted != null)
            {
                AnimalTypes.Add(Deleted);
                DeletedCollection.Remove(Deleted);
            }
        }

        public async void SaveAsync()
        {
            IsBusy = true;
            if (!AddedCollection.Any() && !UpdatedCollection.Any() && !DeletedCollection.Any())
            {
                IsBusy = false;
                return;
            }
            try
            {
                StringBuilder allMessageBuilder = new StringBuilder();
                foreach (var added in AddedCollection)
                {
                    var addMessage = await ApiConnector.AddData<AnimalType>("AnimalTypes", added);
                    allMessageBuilder.Append($"{addMessage}\n");
                }
                foreach (var updated in UpdatedCollection.Where(x => x.Id != null))
                {
                    var updateMessage = await ApiConnector.UpdateData("AnimalTypes", updated, updated.Id.Value);
                    allMessageBuilder.Append($"{updateMessage}\n");
                }
                foreach (var deleted in DeletedCollection)
                {
                    var deleteMessage = await ApiConnector.DeleteData("AnimalTypes", deleted.Id.Value);
                    allMessageBuilder.Append($"{deleteMessage}\n");
                }
                MessageBox.Show(allMessageBuilder.ToString());
                ReadAsync();
            }
            catch (Exception e)
            {
                IsBusy = false;
                MessageBox.Show(GlobalConstants.ErrorMessage + e.Message);
            }
        }

        public string ValidationErrorMessage()
        {
            if (AnimalType == null) return String.Empty;

            if (String.IsNullOrWhiteSpace(AnimalType.Name)) return "Поле \"Наименование\" не заполнено";

            return String.Empty;
        }

        #endregion
    }
}
