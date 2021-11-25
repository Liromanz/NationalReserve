using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using NationalReserve.Helpers;
using NationalReserve.Helpers.Interface;
using NationalReserve.Model;
using NationalReserve.View.Core;

namespace NationalReserve.ViewModel
{
    public class AnimalViewModel : ObservableObject, IDataHandler
    {
        #region Команды
        public RelayCommand AddCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand LogicalDeleteCommand { get; set; }
        public RelayCommand LogicalRecoverCommand { get; set; }

        #endregion

        #region Измененные данные

        public ObservableCollection<Animal> UpdatedCollection { get; set; }
        public ObservableCollection<Animal> AddedCollection { get; set; }

        private ObservableCollection<Animal> _deletedCollection;
        public ObservableCollection<Animal> DeletedCollection
        {
            get => _deletedCollection;
            set
            {
                _deletedCollection = value;
                OnPropertyChanged();
            }
        }

        private Animal _deletedAnimal;
        public Animal DeletedAnimal
        {
            get => _deletedAnimal;
            set
            {
                _deletedAnimal = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Данные с верстки

        private ObservableCollection<Animal> _animals;
        public ObservableCollection<Animal> Animals
        {
            get => _animals;
            set
            {
                _animals = value;
                OnPropertyChanged();
            }
        }

        private Animal _animal;
        public Animal Animal
        {
            get => _animal;
            set
            {
                if (value != null)
                {
                    if (AnimalTypes != null && AnimalTypes.Any())
                        AnimalType = AnimalTypes.FirstOrDefault(x => x.Id == value.IdAnimal);
                    if (Zones != null && Zones.Any())
                        Zone = Zones.FirstOrDefault(x => x.IdZone == value.IdZone);
                    else
                        UpdatedCollection.Add(value);
                }
                _animal = value;
                OnPropertyChanged();
            }
        }

        private Animal _selectedAnimal;
        public Animal SelectedAnimal
        {
            get => _selectedAnimal;
            set
            {
                _selectedAnimal = value;
                if (value != null)
                {
                    Animal = value;
                }
                OnPropertyChanged();
            }
        }

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
                _animalType = value;
                Animal.IdType = value?.Id ?? 1;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Zone> _zones;
        public ObservableCollection<Zone> Zones
        {
            get => _zones;
            set
            {
                _zones = value;
                OnPropertyChanged();
            }
        }

        private Zone _zone;
        public Zone Zone
        {
            get => _zone;
            set
            {
                _zone = value;
                Animal.IdZone = value?.IdZone ?? 1;
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

        public AnimalViewModel()
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

            AnimalTypes = await ApiConnector.GetAll<AnimalType>("AnimalTypes");
            Zones = await ApiConnector.GetAll<Zone>("Zones");
        }

        #region CRUD

        public async void ReadAsync()
        {
            AddedCollection = new ObservableCollection<Animal>();
            UpdatedCollection = new ObservableCollection<Animal>();
            DeletedCollection = new ObservableCollection<Animal>();

            Animal = new Animal();
            Animals = await ApiConnector.GetAll<Animal>("Animals");

            IsBusy = false;
        }

        public void AddObject()
        {
            if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message);
                return;
            }

            Animal.IdAnimal = null;
            Animals.Add(Animal);
            AddedCollection.Add(Animal);

            SelectedAnimal = new Animal();
        }

        public void LogicalDelete()
        {
            if (SelectedAnimal?.IdAnimal != null)
            {
                DeletedCollection.Add(SelectedAnimal);
                Animals.Remove(SelectedAnimal);

                SelectedAnimal = new Animal();
            }
        }

        public void LogicalRecover()
        {
            if (DeletedAnimal != null)
            {
                Animals.Add(DeletedAnimal);
                DeletedCollection.Remove(DeletedAnimal);
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
                    var addMessage = await ApiConnector.AddData<Animal>("Animals", added);
                    allMessageBuilder.Append($"{added.Name}: {addMessage}\n");
                }
                foreach (var updated in UpdatedCollection.Where(x => x.IdAnimal != null))
                {
                    var updateMessage = await ApiConnector.UpdateData("Animals", updated, updated.IdAnimal.Value);
                    allMessageBuilder.Append($"{updated.Name}: {updateMessage}\n");
                }
                foreach (var deleted in DeletedCollection)
                {
                    var deleteMessage = await ApiConnector.DeleteData("Animals", deleted.IdAnimal.Value);
                    allMessageBuilder.Append($"{deleted.Name}: {deleteMessage}\n");
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
            if (Animal == null) return String.Empty;

            if (string.IsNullOrWhiteSpace(Animal.Name)) return "Поле \"Имя\" незаполнено";
            if (!AnimalTypes.Select(x => x.Id).Contains(Animal.IdType)) return "Поле \"Тип\" не выбрано";
            if (Animal.Age < 0) return "Поле \"Пол\" не должно быть отрицательным";
            if (!Zones.Select(x => x.IdZone).Contains(Animal.IdZone)) return "Поле \"Зона\" не выбрано";
            if (Animal.DateRegistration > DateTime.Now) return "Поле \"Дата регистрации\" не может быть в будущем";
            if (Animal.LastCheck > DateTime.Now) return "Поле \"Последняя проверка\" не может быть в будущем";

            return String.Empty;
        }

        #endregion
    }
}
