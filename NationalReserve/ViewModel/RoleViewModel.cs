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
    public class RoleViewModel : ObservableObject, IDataHandler
    {
        #region Команды
        public RelayCommand AddCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand PhysicalDeleteCommand { get; set; }
        public RelayCommand LogicalDeleteCommand { get; set; }
        public RelayCommand LogicalRecoverCommand { get; set; }

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

        public string ValidationErrorMessage()
        {
            if (Role == null) return String.Empty;

            if (String.IsNullOrWhiteSpace(Role.Name)) return "Поле \"Наименование\" не заполнено";

            return String.Empty;
        }

        #endregion
    }
}
