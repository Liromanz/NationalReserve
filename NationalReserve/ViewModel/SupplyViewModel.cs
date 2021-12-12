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
    public class SupplyViewModel : ObservableObject, IDataHandler
    {
        #region Команды
        public RelayCommand AddCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand PhysicalDeleteCommand { get; set; }
        public RelayCommand LogicalDeleteCommand { get; set; }
        public RelayCommand LogicalRecoverCommand { get; set; }

        #endregion

        #region Измененные данные

        public ObservableCollection<Supply> UpdatedCollection { get; set; }
        public ObservableCollection<Supply> AddedCollection { get; set; }

        private ObservableCollection<Supply> _deletedCollection;
        public ObservableCollection<Supply> DeletedCollection
        {
            get => _deletedCollection;
            set
            {
                _deletedCollection = value;
                OnPropertyChanged();
            }
        }

        private Supply _deleted;
        public Supply Deleted
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
                if (value != null)
                {
                    if (Suppliers != null && Suppliers.Any())
                        Supplier = Suppliers.FirstOrDefault(x => x.IdSupplier == value.IdSupplier);
                    if (Materials != null && Materials.Any())
                        Material = Materials.FirstOrDefault(x => x.IdMaterial == value.IdMaterial);
                    UpdatedCollection.Add(value);
                }
                _supply = value;
                OnPropertyChanged();
            }
        }

        private Supply _selected;
        public Supply Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (value != null)
                {
                    Supply = value;
                }
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Supplier> _suppliers;
        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set
            {
                _suppliers = value;
                OnPropertyChanged();
            }
        }

        private Supplier _supplier;
        public Supplier Supplier
        {
            get => _supplier;
            set
            {
                _supplier = value;
                Supply.IdSupplier = value?.IdSupplier ?? Supply.IdSupplier;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Material> _materials;
        public ObservableCollection<Material> Materials
        {
            get => _materials;
            set
            {
                _materials = value;
                OnPropertyChanged();
            }
        }

        private Material _material;
        public Material Material
        {
            get => _material;
            set
            {
                _material = value;
                Supply.IdMaterial = value?.IdMaterial ?? Supply.IdMaterial;
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

        public SupplyViewModel()
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

            Suppliers = await ApiConnector.GetAll<Supplier>("Suppliers");
            Materials = await ApiConnector.GetAll<Material>("Materials");
        }

        #region CRUD

        public async void ReadAsync()
        {
            AddedCollection = new ObservableCollection<Supply>();
            UpdatedCollection = new ObservableCollection<Supply>();
            DeletedCollection = new ObservableCollection<Supply>();

            Supply = new Supply();
            var fullTableList = await ApiConnector.GetAll<Supply>("Supplies");
            Supplies = new ObservableCollection<Supply>(fullTableList.Where(x => !x.IsDeleted));
            DeletedCollection = new ObservableCollection<Supply>(fullTableList.Where(x => x.IsDeleted));

            IsBusy = false;
        }

        public void AddObject()
        {
            if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message);
                return;
            }

            Supply.IdSupply = null;
            Supplies.Add((Supply)Supply.Clone());
            AddedCollection.Add((Supply)Supply.Clone());

            Selected = new Supply();
        }
        public async void PhysicalDelete()
        {
            if (Deleted != null)
            {
                var deleteMessage = await ApiConnector.DeleteData("Supplies", Deleted.IdSupply.Value);
                MessageBox.Show($"{deleteMessage}\n");
                ReadAsync();
            }
        }
        public void LogicalDelete()
        {
            if (Selected?.IdSupply != null)
            {
                Selected.IsDeleted = true;
                DeletedCollection.Add(Selected);
                Supplies.Remove(Selected);

                Selected = new Supply();
            }
        }

        public void LogicalRecover()
        {
            if (Deleted != null)
            {
                Deleted.IsDeleted = false;
                UpdatedCollection.Add(Deleted);
                Supplies.Add(Deleted);
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
                    var addMessage = await ApiConnector.AddData<Supply>("Supplies", added);
                    allMessageBuilder.Append($"Поставка №:{added.IdSupply}: {addMessage}\n");
                }
                foreach (var updated in UpdatedCollection.Where(x => x.IdSupply != null))
                {
                    var updateMessage = await ApiConnector.UpdateData("Supplies", updated, updated.IdSupply.Value);
                    allMessageBuilder.Append($"Поставка №:{updated.IdSupply}: {updateMessage}\n");
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
            if (Supply == null) return String.Empty;

            if (!Suppliers.Select(x => x.IdSupplier).Contains(Supply.IdSupplier)) return "Поле \"Поставщик\" не выбрано";
            if (!Materials.Select(x => x.IdMaterial).Contains(Supply.IdMaterial)) return "Поле \"Материал\" не выбрано";
            if (Supply.Amount <= 0) return "Поле \"Количество\" не может быть отрицательным или равным нулю";
            if (Supply.Date > DateTime.Now) return "Поле \"Дата поставки\" не может быть в будущем";
            if (Supply.Date.Year < 2010) return "Минимальное значение поля \"Дата поставки\" - 2010 год";


            return String.Empty;
        }

        #endregion
    }
}
