using NationalReserve.Helpers;
using NationalReserve.Helpers.Interface;
using NationalReserve.Model;
using NationalReserve.View.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace NationalReserve.ViewModel
{
    public class HumanViewModel : ObservableObject, IDataHandler
    {
        #region Команды
        public RelayCommand AddCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand LogicalDeleteCommand { get; set; }
        public RelayCommand LogicalRecoverCommand { get; set; }

        #endregion

        #region Измененные данные

        public ObservableCollection<Human> UpdatedCollection { get; set; }
        public ObservableCollection<Human> AddedCollection { get; set; }

        private ObservableCollection<Human> _deletedCollection;
        public ObservableCollection<Human> DeletedCollection
        {
            get => _deletedCollection;
            set
            {
                _deletedCollection = value;
                OnPropertyChanged();
            }
        }

        private Human _deletedHuman;
        public Human DeletedHuman
        {
            get => _deletedHuman;
            set
            {
                _deletedHuman = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Данные с верстки

        private ObservableCollection<Human> _humans;
        public ObservableCollection<Human> Humans
        {
            get => _humans;
            set
            {
                _humans = value;
                OnPropertyChanged();
            }
        }

        private Human _human;
        public Human Human
        {
            get => _human;
            set
            {
                if (value != null)
                {
                    if (value.Password != null && !SecureData.IsHash(value.Password))
                        _human.Password = SecureData.Hash(value.Password);
                    if (Roles != null && Roles.Any())
                        Role = Roles.FirstOrDefault(x => x.Id == value.IdRole);

                    if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
                        MessageBox.Show(message);
                    else
                        UpdatedCollection.Add(value);
                }
                _human = value;
                OnPropertyChanged();
            }
        }

        private Human _selectedHuman;
        public Human SelectedHuman
        {
            get => _selectedHuman;
            set
            {
                _selectedHuman = value;
                if (value != null)
                {
                    Human = value;
                }
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Role> _roles;
        public ObservableCollection<Role> Roles
        {
            get => _roles;
            set
            {
                _roles = value;
                OnPropertyChanged();
            }
        }

        private Role _role;
        public Role Role
        {
            get => _role;
            set
            {
                _role = value;
                Human.IdRole = value?.Id ?? 2;
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

        public HumanViewModel()
        {
            IsBusy = true;
            InitAsync();
            ReadAsync();
        }

        private async void InitAsync()
        {
            AddCommand = new RelayCommand(o => { AddObject(); });
            SaveCommand = new RelayCommand(o => { SaveAsync(); });
            LogicalDeleteCommand = new RelayCommand(o => { LogicalDelete(); });
            LogicalRecoverCommand = new RelayCommand(o => { LogicalRecover(); });

            Roles = await ApiConnector.GetAll<Role>("Roles");
        }

        #region DataHandler

        public async void ReadAsync()
        {
            AddedCollection = new ObservableCollection<Human>();
            UpdatedCollection = new ObservableCollection<Human>();
            DeletedCollection = new ObservableCollection<Human>();

            Human = new Human();
            Humans = await ApiConnector.GetAll<Human>("Humen");

            IsBusy = false;
        }

        public void AddObject()
        {
            if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message);
                return;
            }

            Human.IdHuman = null;
            if (!SecureData.IsHash(Human.Password))
                Human.Password = SecureData.Hash(Human.Password);
            Humans.Add(Human);
            AddedCollection.Add(Human);

            SelectedHuman = new Human();
        }

        public void LogicalDelete()
        {
            if (SelectedHuman != null)
            {
                DeletedCollection.Add(SelectedHuman);
                Humans.Remove(SelectedHuman);

                SelectedHuman = new Human();
            }
        }

        public void LogicalRecover()
        {
            if (DeletedHuman != null)
            {
                Humans.Add(DeletedHuman);
                DeletedCollection.Remove(DeletedHuman);
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
                foreach (var addedHuman in AddedCollection)
                {
                    if (!SecureData.IsHash(addedHuman.Password))
                        addedHuman.Password = SecureData.Hash(addedHuman.Password);
                    var addMessage = await ApiConnector.AddData<Human>("Humen", addedHuman);
                    allMessageBuilder.Append($"{addedHuman.FirstName} {addedHuman.Name}: {addMessage}\n");
                }
                foreach (var updatedHuman in UpdatedCollection.Where(x => x.IdHuman != null))
                {
                    if (!SecureData.IsHash(updatedHuman.Password))
                        updatedHuman.Password = SecureData.Hash(updatedHuman.Password);
                    var updateMessage = await ApiConnector.UpdateData("Humen", updatedHuman, updatedHuman.IdHuman.Value);
                    allMessageBuilder.Append($"{updatedHuman.FirstName} {updatedHuman.Name}: {updateMessage}\n");
                }
                foreach (var deletedHuman in DeletedCollection)
                {
                    var deleteMessage = await ApiConnector.DeleteData("Humen", deletedHuman.IdHuman.Value);
                    allMessageBuilder.Append($"{deletedHuman.FirstName} {deletedHuman.Name}: {deleteMessage}\n");
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
            if (Human == null) return String.Empty;

            if (string.IsNullOrWhiteSpace(Human.Login)) return "Поле \"Логин\" незаполнено";
            if (string.IsNullOrWhiteSpace(Human.Password)) return "Поле \"Пароль\" незаполнено";
            if (string.IsNullOrWhiteSpace(Human.FirstName)) return "Поле \"Фамилия\" незаполнено";
            if (string.IsNullOrWhiteSpace(Human.Name)) return "Поле \"Имя\" незаполнено";
            if (Human.Gender < 0 || Human.Gender > 1) return "Поле \"Пол\" не выбрано";
            if (!Roles.Select(x => x.Id).Contains(Human.IdRole)) return "Поле \"Роль\" не выбрано";

            return String.Empty;
        }

        #endregion
    }
}
