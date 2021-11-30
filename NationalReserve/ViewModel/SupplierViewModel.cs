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
    public class SupplierViewModel : ObservableObject, IDataHandler
    {
        #region Команды
        public RelayCommand AddCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand LogicalDeleteCommand { get; set; }
        public RelayCommand LogicalRecoverCommand { get; set; }

        #endregion

        #region Измененные данные

        public ObservableCollection<Supplier> UpdatedCollection { get; set; }
        public ObservableCollection<Supplier> AddedCollection { get; set; }

        private ObservableCollection<Supplier> _deletedCollection;
        public ObservableCollection<Supplier> DeletedCollection
        {
            get => _deletedCollection;
            set
            {
                _deletedCollection = value;
                OnPropertyChanged();
            }
        }

        private Supplier _deleted;
        public Supplier Deleted
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
                if (value != null)
                {
                    UpdatedCollection.Add(value);
                }
                _supplier = value;
                OnPropertyChanged();
            }
        }

        private Supplier _selected;
        public Supplier Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (value != null)
                {
                    Supplier = value;
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

        public SupplierViewModel()
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
            AddedCollection = new ObservableCollection<Supplier>();
            UpdatedCollection = new ObservableCollection<Supplier>();
            DeletedCollection = new ObservableCollection<Supplier>();

            Supplier = new Supplier();
            Suppliers = await ApiConnector.GetAll<Supplier>("Suppliers");

            IsBusy = false;
        }

        public void AddObject()
        {
            if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message);
                return;
            }

            Supplier.IdSupplier = null;
            Suppliers.Add((Supplier)Supplier.Clone());
            AddedCollection.Add((Supplier)Supplier.Clone());

            Selected = new Supplier();
        }

        public void LogicalDelete()
        {
            if (Selected?.IdSupplier != null)
            {
                DeletedCollection.Add(Selected);
                Suppliers.Remove(Selected);

                Selected = new Supplier();
            }
        }

        public void LogicalRecover()
        {
            if (Deleted != null)
            {
                Suppliers.Add(Deleted);
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
                    var addMessage = await ApiConnector.AddData<Supplier>("Suppliers", added);
                    allMessageBuilder.Append($"{added.Name}: {addMessage}\n");
                }
                foreach (var updated in UpdatedCollection.Where(x => x.IdSupplier != null))
                {
                    var updateMessage = await ApiConnector.UpdateData("Suppliers", updated, updated.IdSupplier.Value);
                    allMessageBuilder.Append($"{updated.Name}: {updateMessage}\n");
                }
                foreach (var deleted in DeletedCollection)
                {
                    var deleteMessage = await ApiConnector.DeleteData("Suppliers", deleted.IdSupplier.Value);
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
            if (Supplier == null) return String.Empty;

            if (string.IsNullOrWhiteSpace(Supplier.Name)) return "Поле \"Наименование\" не заполнено";
            if (string.IsNullOrWhiteSpace(Supplier.Phone)) return "Поле \"Телефон\" не заполнено";
            if (Supplier.Phone.Contains('_')) return "Поле \"Телефон\" не полностью заполнено";
            if (string.IsNullOrWhiteSpace(Supplier.City)) return "Поле \"Город\" не заполнено";

            return String.Empty;
        }

        #endregion
    }
}
