using System;
using System.Collections.Generic;
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
        public RelayCommand PhysicalDeleteCommand { get; set; }
        public RelayCommand LogicalDeleteCommand { get; set; }
        public RelayCommand LogicalRecoverCommand { get; set; }
        public RelayCommand ExportCommand { get; set; }
        public RelayCommand ImportCommand { get; set; }

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
                Animal.IdType = value?.Id ?? Animal.IdType;
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
                Animal.IdZone = value?.IdZone ?? Animal.IdZone;
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
            PhysicalDeleteCommand = new RelayCommand(o => { PhysicalDelete(); });
            LogicalDeleteCommand = new RelayCommand(o => { LogicalDelete(); });
            LogicalRecoverCommand = new RelayCommand(o => { LogicalRecover(); });
            ExportCommand = new RelayCommand(o => { ExportTable(); });
            ImportCommand = new RelayCommand(o => { ImportTable(); });


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
            var fullTableList = await ApiConnector.GetAll<Animal>("Animals");
            Animals = new ObservableCollection<Animal>(fullTableList.Where(x => !x.IsDeleted));
            DeletedCollection = new ObservableCollection<Animal>(fullTableList.Where(x => x.IsDeleted));

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
            Animals.Add((Animal)Animal.Clone());
            AddedCollection.Add((Animal)Animal.Clone());

            SelectedAnimal = new Animal();
        }
        public async void PhysicalDelete()
        {
            if (DeletedAnimal != null)
            {
                var deleteMessage = await ApiConnector.DeleteData("Animals", DeletedAnimal.IdAnimal.Value);
                MessageBox.Show($"{deleteMessage}\n");
                ReadAsync();
            }
        }
        public void LogicalDelete()
        {
            if (SelectedAnimal?.IdAnimal != null)
            {
                SelectedAnimal.IsDeleted = true;
                DeletedCollection.Add(SelectedAnimal);
                Animals.Remove(SelectedAnimal);

                SelectedAnimal = new Animal();
            }
        }

        public void LogicalRecover()
        {
            if (DeletedAnimal != null)
            {
                DeletedAnimal.IsDeleted = true;
                Animals.Add(DeletedAnimal);
                UpdatedCollection.Add(DeletedAnimal);
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
                MessageBox.Show(allMessageBuilder.ToString());
                ReadAsync();
            }
            catch (Exception e)
            {
                IsBusy = false;
                MessageBox.Show(GlobalConstants.ErrorMessage + e.Message);
            }
        }

        public void ExportTable()
        {
            List<string> exportList = new List<string>();
            foreach (var item in Animals)
                exportList.Add($"{item.IdAnimal}, {item.Name}, {item.IdType}, {item.Age}, " +
                               $"{item.HasFamily}, {item.IsSick}, {item.DateRegistration}, " +
                               $"{item.IdZone}, {item.LastCheck}, {item.IsDeleted}");
            CsvHelper.WriteCSV(exportList, "Animals");
        }

        public void ImportTable()
        {
            var imported = CsvHelper.ReadCSV(9);
            try
            {
                foreach (var items in imported)
                {
                    var item = new Animal
                    {
                        IdAnimal = null,
                        Name = items[0],
                        IdType = Convert.ToInt32(items[1]),
                        Age = Convert.ToInt32(items[2]),
                        HasFamily = Convert.ToInt32(items[3]),
                        IsSick = Convert.ToInt32(items[4]),
                        DateRegistration = Convert.ToDateTime(items[5]),
                        IdZone = Convert.ToInt32(items[6]),
                        LastCheck = Convert.ToDateTime(items[7]),
                        IsDeleted = Convert.ToBoolean(items[8])
                    };
                    AddedCollection.Add(item);
                    Animals.Add(item);
                }
            }
            catch (Exception e) { MessageBox.Show(GlobalConstants.ErrorMessage + e.Message); }
        }

        public string ValidationErrorMessage()
        {
            if (Animal == null) return String.Empty;

            if (string.IsNullOrWhiteSpace(Animal.Name)) return "Поле \"Имя\" незаполнено";
            if (!AnimalTypes.Select(x => x.Id).Contains(Animal.IdType)) return "Поле \"Тип\" не выбрано";
            if (Animal.Age < 0) return "Поле \"Возраст\" не должно быть отрицательным";
            if (!Zones.Select(x => x.IdZone).Contains(Animal.IdZone)) return "Поле \"Зона\" не выбрано";
            if (Animal.DateRegistration > DateTime.Now) return "Поле \"Дата регистрации\" не может быть в будущем";
            if (Animal.LastCheck > DateTime.Now) return "Поле \"Последняя проверка\" не может быть в будущем";
            if (Animal.DateRegistration.Year < 2010) return "Минимальное значение поля \"Дата регистрации\" - 2010 год";
            if (Animal.LastCheck.Year < 2010) return "Минимальное значение поля \"Последняя проверка\" - 2010 год";

            return String.Empty;
        }

        #endregion
    }
}
