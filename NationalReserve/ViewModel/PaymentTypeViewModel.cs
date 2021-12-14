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
    public class PaymentTypeViewModel : ObservableObject, IDataHandler
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

        public ObservableCollection<PaymentType> UpdatedCollection { get; set; }
        public ObservableCollection<PaymentType> AddedCollection { get; set; }

        private ObservableCollection<PaymentType> _deletedCollection;
        public ObservableCollection<PaymentType> DeletedCollection
        {
            get => _deletedCollection;
            set
            {
                _deletedCollection = value;
                OnPropertyChanged();
            }
        }

        private PaymentType _deleted;
        public PaymentType Deleted
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

        private ObservableCollection<PaymentType> _paymentTypes;
        public ObservableCollection<PaymentType> PaymentTypes
        {
            get => _paymentTypes;
            set
            {
                _paymentTypes = value;
                OnPropertyChanged();
            }
        }

        private PaymentType _paymentType;
        public PaymentType PaymentType
        {
            get => _paymentType;
            set
            {
                if (value != null)
                    UpdatedCollection.Add(value);

                _paymentType = value;
                OnPropertyChanged();
            }
        }

        private PaymentType _selected;
        public PaymentType Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (value != null)
                {
                    PaymentType = value;
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

        public PaymentTypeViewModel()
        {
            IsBusy = true;
            InitAsync();
            ReadAsync();
        }

        private void InitAsync()
        {
            AddCommand = new RelayCommand(o => { AddObject(); });
            SaveCommand = new RelayCommand(o => { SaveAsync(); });
            PhysicalDeleteCommand = new RelayCommand(o => { PhysicalDelete(); });
            LogicalDeleteCommand = new RelayCommand(o => { LogicalDelete(); });
            LogicalRecoverCommand = new RelayCommand(o => { LogicalRecover(); });
            ExportCommand = new RelayCommand(o => { ExportTable(); });
            ImportCommand = new RelayCommand(o => { ImportTable(); });
        }

        #region CRUD

        public async void ReadAsync()
        {
            AddedCollection = new ObservableCollection<PaymentType>();
            UpdatedCollection = new ObservableCollection<PaymentType>();
            DeletedCollection = new ObservableCollection<PaymentType>();

            PaymentType = new PaymentType();
            var fullTableList = await ApiConnector.GetAll<PaymentType>("PaymentTypes");
            PaymentTypes = new ObservableCollection<PaymentType>(fullTableList.Where(x => !x.IsDeleted));
            DeletedCollection = new ObservableCollection<PaymentType>(fullTableList.Where(x => x.IsDeleted));

            IsBusy = false;
        }

        public void AddObject()
        {
            if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message);
                return;
            }

            PaymentType.Id = null;
            PaymentTypes.Add((PaymentType)PaymentType.Clone());
            AddedCollection.Add((PaymentType)PaymentType.Clone());

            Selected = new PaymentType();
        }
        public async void PhysicalDelete()
        {
            if (Deleted != null)
            {
                var deleteMessage = await ApiConnector.DeleteData("PaymentTypes", Deleted.Id.Value);
                MessageBox.Show($"{deleteMessage}\n");
                ReadAsync();
            }
        }

        public void LogicalDelete()
        {
            if (Selected?.Id != null)
            {
                Selected.IsDeleted = true;
                DeletedCollection.Add(Selected);
                PaymentTypes.Remove(Selected);

                Selected = new PaymentType();
            }
        }

        public void LogicalRecover()
        {
            if (Deleted != null)
            {
                Deleted.IsDeleted = false;
                UpdatedCollection.Add(Deleted);
                PaymentTypes.Add(Deleted);
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
                    var addMessage = await ApiConnector.AddData<PaymentType>("PaymentTypes", added);
                    allMessageBuilder.Append($"{added.Name}: {addMessage}\n");
                }
                foreach (var updated in UpdatedCollection.Where(x => x.Id != null))
                {
                    var updateMessage = await ApiConnector.UpdateData("PaymentTypes", updated, updated.Id.Value);
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
            foreach (var item in PaymentTypes)
                exportList.Add($"{item.Id}, {item.Name}, {item.IsDeleted}");
            CsvHelper.WriteCSV(exportList, "PaymentTypes");
        }

        public void ImportTable()
        {
            var imported = CsvHelper.ReadCSV(3);
            try
            {
                foreach (var items in imported)
                {
                    var item = new PaymentType
                    {
                        Id = null,
                        Name = items[1],
                        IsDeleted = Convert.ToBoolean(items[2])
                    };
                    AddedCollection.Add(item);
                    PaymentTypes.Add(item);
                }
            }
            catch (Exception e) { MessageBox.Show(GlobalConstants.ErrorMessage + e.Message); }
        }
        public string ValidationErrorMessage()
        {
            if (PaymentType == null) return String.Empty;

            if (String.IsNullOrWhiteSpace(PaymentType.Name)) return "Поле \"Наименование\" не заполнено";

            return String.Empty;
        }

        #endregion
    }
}
