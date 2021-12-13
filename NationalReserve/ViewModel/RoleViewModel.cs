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
    public class RoleViewModel : ObservableObject, IDataHandler
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

        public ObservableCollection<Role> UpdatedCollection { get; set; }
        public ObservableCollection<Role> AddedCollection { get; set; }

        private ObservableCollection<Role> _deletedCollection;
        public ObservableCollection<Role> DeletedCollection
        {
            get => _deletedCollection;
            set
            {
                _deletedCollection = value;
                OnPropertyChanged();
            }
        }

        private Role _deleted;
        public Role Deleted
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
                if (value != null)
                    UpdatedCollection.Add(value);

                _role = value;
                OnPropertyChanged();
            }
        }

        private Role _selected;
        public Role Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (value != null)
                {
                    Role = value;
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

        public RoleViewModel()
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
            AddedCollection = new ObservableCollection<Role>();
            UpdatedCollection = new ObservableCollection<Role>();

            Role = new Role();
            var fullTableList = await ApiConnector.GetAll<Role>("Roles");
            Roles = new ObservableCollection<Role>(fullTableList.Where(x => !x.IsDeleted));
            DeletedCollection = new ObservableCollection<Role>(fullTableList.Where(x => x.IsDeleted));

            IsBusy = false;
        }

        public void AddObject()
        {
            if (ValidationErrorMessage() is string message && !string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message);
                return;
            }

            Role.Id = null;
            Roles.Add((Role)Role.Clone());
            AddedCollection.Add((Role)Role.Clone());

            Selected = new Role();
        }
        public async void PhysicalDelete()
        {
            if (Deleted != null)
            {
                var deleteMessage = await ApiConnector.DeleteData("Roles", Deleted.Id.Value);
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
                Roles.Remove(Selected);

                Selected = new Role();
            }
        }

        public void LogicalRecover()
        {
            if (Deleted != null)
            {
                Deleted.IsDeleted = false;
                UpdatedCollection.Add(Deleted);
                Roles.Add(Deleted);
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
                    var addMessage = await ApiConnector.AddData<Role>("Roles", added);
                    allMessageBuilder.Append($"{added.Name}: {addMessage}\n");
                }
                foreach (var updated in UpdatedCollection.Where(x => x.Id != null))
                {
                    var updateMessage = await ApiConnector.UpdateData("Roles", updated, updated.Id.Value);
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
            foreach (var item in Roles)
                exportList.Add($"{item.Id}, {item.Name}, {item.IsAnimalFeedVisible}, {item.IsAnimalVisible}, " +
                               $"{item.IsAnimalTypeVisible}, {item.IsCheckpointVisible}, {item.IsCheckpointPassVisible}, " +
                               $"{item.IsHumanVisible}, {item.IsMaterialVisible}, {item.IsMaterialTypeVisible}, " +
                               $"{item.IsPaymentTypeVisible}, {item.IsPlantListVisible}, {item.IsRoleVisible}, " +
                               $"{item.IsSecurityListVisible}, {item.IsSponsorshipVisible}, {item.IsStaffDocumentsVisible}, " +
                               $"{item.IsSupplierVisible}, {item.IsSupplyVisible}, {item.IsZoneVisible}, {item.IsDeleted}");
            CsvHelper.WriteCSV(exportList, "Roles");
        }

        public void ImportTable()
        {
            var imported = CsvHelper.ReadCSV(20);
            try
            {
                foreach (var items in imported)
                {
                    var item = new Role
                    {
                        Id = null,
                        Name = items[1],
                        IsAnimalFeedVisible = Convert.ToBoolean(items[2]),
                        IsAnimalVisible = Convert.ToBoolean(items[3]),
                        IsAnimalTypeVisible = Convert.ToBoolean(items[4]),
                        IsCheckpointVisible = Convert.ToBoolean(items[5]),
                        IsCheckpointPassVisible = Convert.ToBoolean(items[6]),
                        IsHumanVisible = Convert.ToBoolean(items[7]),
                        IsMaterialVisible = Convert.ToBoolean(items[8]),
                        IsMaterialTypeVisible = Convert.ToBoolean(items[9]),
                        IsPaymentTypeVisible = Convert.ToBoolean(items[10]),
                        IsPlantListVisible = Convert.ToBoolean(items[11]),
                        IsRoleVisible = Convert.ToBoolean(items[12]),
                        IsSecurityListVisible = Convert.ToBoolean(items[13]),
                        IsSponsorshipVisible = Convert.ToBoolean(items[14]),
                        IsStaffDocumentsVisible = Convert.ToBoolean(items[15]),
                        IsSupplierVisible = Convert.ToBoolean(items[16]),
                        IsSupplyVisible = Convert.ToBoolean(items[17]),
                        IsZoneVisible = Convert.ToBoolean(items[18]),
                        IsDeleted = Convert.ToBoolean(items[19])
                    };
                    AddedCollection.Add(item);
                    Roles.Add(item);
                }
            }
            catch (Exception e) { MessageBox.Show(GlobalConstants.ErrorMessage + e.Message); }
        }

        public string ValidationErrorMessage()
        {
            if (Role == null) return String.Empty;

            if (String.IsNullOrWhiteSpace(Role.Name)) return "Поле \"Наименование\" не заполнено";

            return String.Empty;
        }

        #endregion
    }
}
