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
    public class MaterialTypeViewModel : ObservableObject, IDataHandler
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

        public ObservableCollection<MaterialType> UpdatedCollection { get; set; }
        public ObservableCollection<MaterialType> AddedCollection { get; set; }

        private ObservableCollection<MaterialType> _deletedCollection;
        public ObservableCollection<MaterialType> DeletedCollection
        {
            get => _deletedCollection;
            set
            {
                _deletedCollection = value;
                OnPropertyChanged();
            }
        }

        private MaterialType _deleted;
        public MaterialType Deleted
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

        private ObservableCollection<MaterialType> _materialTypes;
        public ObservableCollection<MaterialType> MaterialTypes
        {
            get => _materialTypes;
            set
            {
                _materialTypes = value;
                OnPropertyChanged();
            }
        }

        private MaterialType _materialType;
        public MaterialType MaterialType
        {
            get => _materialType;
            set
            {
                if (value != null)
                    UpdatedCollection.Add(value);

                _materialType = value;
                OnPropertyChanged();
            }
        }

        private MaterialType _selected;
        public MaterialType Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (value != null)
                {
                    MaterialType = value;
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

        public MaterialTypeViewModel()
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
            AddedCollection = new ObservableCollection<MaterialType>();
            UpdatedCollection = new ObservableCollection<MaterialType>();
            DeletedCollection = new ObservableCollection<MaterialType>();

            MaterialType = new MaterialType();
            var fullTableList = await ApiConnector.GetAll<MaterialType>("MaterialTypes");
            MaterialTypes = new ObservableCollection<MaterialType>(fullTableList.Where(x=> !x.IsDeleted));
            DeletedCollection = new ObservableCollection<MaterialType>(fullTableList.Where(x=> x.IsDeleted));

            IsBusy = false;
        }

        public void AddObject()
        {
            if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message);
                return;
            }

            MaterialType.Id = null;
            MaterialTypes.Add((MaterialType)MaterialType.Clone());
            AddedCollection.Add((MaterialType)MaterialType.Clone());

            Selected = new MaterialType();
        }

        public async void PhysicalDelete()
        {
            if (Deleted != null)
            {
                var deleteMessage = await ApiConnector.DeleteData("MaterialTypes", Deleted.Id.Value);
                MessageBox.Show($"{Deleted.Name}: {deleteMessage}\n");
                ReadAsync();
            }
        }

        public void LogicalDelete()
        {
            if (Selected?.Id != null)
            {
                Selected.IsDeleted = true;
                DeletedCollection.Add(Selected);
                MaterialTypes.Remove(Selected);

                Selected = new MaterialType();
            }
        }

        public void LogicalRecover()
        {
            if (Deleted != null)
            {
                Deleted.IsDeleted = false;
                UpdatedCollection.Add(Deleted);
                MaterialTypes.Add(Deleted);
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
                    var addMessage = await ApiConnector.AddData<MaterialType>("MaterialTypes", added);
                    allMessageBuilder.Append($"{added.Name}: {addMessage}\n");
                }
                foreach (var updated in UpdatedCollection.Where(x => x.Id != null))
                {
                    var updateMessage = await ApiConnector.UpdateData("MaterialTypes", updated, updated.Id.Value);
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
            foreach (var item in MaterialTypes)
                exportList.Add($"{item.Id}, {item.Name}, {item.IsDeleted}");
            CsvHelper.WriteCSV(exportList, "MaterialTypes");
        }

        public void ImportTable()
        {
            var imported = CsvHelper.ReadCSV(3);
            try
            {
                foreach (var items in imported)
                {
                    var item = new MaterialType
                    {
                        Id = null,
                        Name = items[1],
                        IsDeleted = Convert.ToBoolean(items[2])
                    };
                    AddedCollection.Add(item);
                    MaterialTypes.Add(item);
                }
            }
            catch (Exception e) { MessageBox.Show(GlobalConstants.ErrorMessage + e.Message); }
        }

        public string ValidationErrorMessage()
        {
            if (MaterialType == null) return String.Empty;

            if (String.IsNullOrWhiteSpace(MaterialType.Name)) return "Поле \"Наименование\" не заполнено";

            return String.Empty;
        }
        #endregion
    }
}
