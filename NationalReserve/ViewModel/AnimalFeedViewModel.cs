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
    public class AnimalFeedViewModel : ObservableObject, IDataHandler
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

        public ObservableCollection<AnimalFeed> UpdatedCollection { get; set; }
        public ObservableCollection<AnimalFeed> AddedCollection { get; set; }

        private ObservableCollection<AnimalFeed> _deletedCollection;
        public ObservableCollection<AnimalFeed> DeletedCollection
        {
            get => _deletedCollection;
            set
            {
                _deletedCollection = value;
                OnPropertyChanged();
            }
        }

        private AnimalFeed _deleted;
        public AnimalFeed Deleted
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

        private ObservableCollection<AnimalFeed> _animalFeeds;
        public ObservableCollection<AnimalFeed> AnimalFeeds
        {
            get => _animalFeeds;
            set
            {
                _animalFeeds = value;
                OnPropertyChanged();
            }
        }

        private AnimalFeed _animalFeed;
        public AnimalFeed AnimalFeed
        {
            get => _animalFeed;
            set
            {
                if (value != null)
                {
                    if (Animals != null && Animals.Any())
                        Animal = Animals.FirstOrDefault(x => x.IdAnimal == value.IdAnimal);
                    if (Supplies != null && Supplies.Any())
                        Supply = Supplies.FirstOrDefault(x => x.IdSupply == value.IdSupply);
                    UpdatedCollection.Add(value);
                }
                _animalFeed = value;
                OnPropertyChanged();
            }
        }

        private AnimalFeed _selected;
        public AnimalFeed Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (value != null)
                {
                    AnimalFeed = value;
                }
                OnPropertyChanged();
            }
        }

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
                _animal = value;
                Animal.IdType = value?.IdAnimal ?? Animal.IdType;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Supply> _supplies;
        public ObservableCollection<Supply> Supplies
        {
            get => _supplies;
            set
            {
                _supplies = value;
                OnPropertyChanged();
            }
        }

        private Supply _supply;
        public Supply Supply
        {
            get => _supply;
            set
            {
                _supply = value;
                Animal.IdZone = value?.IdSupply ?? Animal.IdZone;
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

        public AnimalFeedViewModel()
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

            Animals = await ApiConnector.GetAll<Animal>("Animals");
            Supplies = await ApiConnector.GetAll<Supply>("Supplies");
        }

        #region CRUD

        public async void ReadAsync()
        {
            AddedCollection = new ObservableCollection<AnimalFeed>();
            UpdatedCollection = new ObservableCollection<AnimalFeed>();
            DeletedCollection = new ObservableCollection<AnimalFeed>();

            AnimalFeed = new AnimalFeed();
            var fullTableList = await ApiConnector.GetAll<AnimalFeed>("AnimalFeeds");
            AnimalFeeds = new ObservableCollection<AnimalFeed>(fullTableList.Where(x => !x.IsDeleted));
            DeletedCollection = new ObservableCollection<AnimalFeed>(fullTableList.Where(x => x.IsDeleted));

            IsBusy = false;
        }

        public void AddObject()
        {
            if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message);
                return;
            }

            AnimalFeed.IdFeed = null;
            AnimalFeeds.Add((AnimalFeed)AnimalFeed.Clone());
            AddedCollection.Add((AnimalFeed)AnimalFeed.Clone());

            Selected = new AnimalFeed();
        }

        public async void PhysicalDelete()
        {
            if (Deleted != null)
            {
                var deleteMessage = await ApiConnector.DeleteData("AnimalFeed", Deleted.IdFeed.Value);
                MessageBox.Show($"{deleteMessage}\n");
            }
        }

        public void LogicalDelete()
        {
            if (Selected?.IdFeed != null)
            {
                Selected.IsDeleted = true;
                DeletedCollection.Add(Selected);
                AnimalFeeds.Remove(Selected);

                Selected = new AnimalFeed();
            }
        }

        public void LogicalRecover()
        {
            if (Deleted != null)
            {
                Deleted.IsDeleted = true;
                AnimalFeeds.Add(Deleted);
                UpdatedCollection.Add(Deleted);
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
                    var addMessage = await ApiConnector.AddData<AnimalFeed>("AnimalFeeds", added);
                    allMessageBuilder.Append($"{addMessage}\n");
                }
                foreach (var updated in UpdatedCollection.Where(x => x.IdFeed != null))
                {
                    var updateMessage = await ApiConnector.UpdateData("AnimalFeeds", updated, updated.IdFeed.Value);
                    allMessageBuilder.Append($"{updateMessage}\n");
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
            foreach (var item in AnimalFeeds)
                exportList.Add($"{item.IdFeed}, {item.IdSupply}, {item.IdAnimal}, {item.Amount}, {item.IsDeleted}");
            CsvHelper.WriteCSV(exportList, "AnimalFeeds");
        }

        public void ImportTable()
        {
            var imported = CsvHelper.ReadCSV(5);
            try
            {
                foreach (var items in imported)
                {
                    var item = new AnimalFeed
                    {
                        IdFeed = null,
                        IdSupply = Convert.ToInt32(items[1]),
                        IdAnimal = Convert.ToInt32(items[2]),
                        Amount = Convert.ToInt32(items[3]),
                        IsDeleted = Convert.ToBoolean(items[4])
                    };
                    AddedCollection.Add(item);
                    AnimalFeeds.Add(item);
                }
            }
            catch (Exception e) { MessageBox.Show(GlobalConstants.ErrorMessage + e.Message); }
        }
        public string ValidationErrorMessage()
        {
            if (AnimalFeed == null) return String.Empty;

            if (!Animals.Select(x => x.IdAnimal).Contains(AnimalFeed.IdAnimal)) return "Поле \"Животное\" не выбрано";
            if (!Supplies.Select(x => x.IdSupply).Contains(AnimalFeed.IdSupply)) return "Поле \"Поставка\" не выбрано";
            if (AnimalFeed.Amount <= 0) return "Поле \"Количество\" не может быть отрицательным или равным нулю";

            return String.Empty;
        }

        #endregion
    }
}
