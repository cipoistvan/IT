using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ITAssets
{
    public class DesignUsersViewModel
    {
        public ObservableCollection<User> Users { get; set; }

        public DesignUsersViewModel()
        {
            Users = new ObservableCollection<User>
            {
                new User { ID = 1, UserName="admin", Password = "********", Email = "a@b.hu" },
                new User { ID = 2, UserName="admin2", Password = "********", Email = "b@b.hu" },
                new User { ID = 3, UserName="admin3", Password = "********", Email = "c@b.hu" }
            };

        }

        public User SelectedUser { get; set; }
        public User EditUser { 
            get
                {
                    return new User { UserName = "test" };
                } 
            
            }

    }

    public class UsersViewModel:INotifyPropertyChanged
    {
        public ObservableCollection<User> Users { get; }
        
        public ICommand AddUserCmd { get; }
        public ICommand ModifyUserCmd { get; }
        public ICommand DeleteUserCmd { get; }
        public ICommand SaveUserCmd { get; }


        public UsersViewModel()
        {
            Users = new DatabaseService(App.connectionString).GetUsers();


            AddUserCmd = new RelayCommand
                (
                    execute: _ => { },
                    canExecute: _ => !IsEnabled
                );

            ModifyUserCmd = new RelayCommand
                (
                    execute: _ => IsEnabled = true,
                    canExecute: _ => SelectedUser != null && !IsEnabled
                );

            DeleteUserCmd = new RelayCommand
                (
                    execute: _ => { },
                    canExecute: _ => SelectedUser != null && !IsEnabled
                );

            SaveUserCmd = new RelayCommand(ExecuteSave, CanExecuteSave);
                //(
                //    execute: _ => IsEnabled = false,
                //    canExecute: _ => IsEnabled == true
                //);

        }

        private void ExecuteSave (object parameter)
        {

            if (parameter is object[] values && values.Length == 2)
            {
                string val1 = values[0] as string ?? "";
                string val2 = values[1] as string ?? "";
                MessageBox.Show($"Beérkező: {val1} és {val2}");
            }

            IsEnabled = false;

        }

        private bool CanExecuteSave(object parameter) => IsEnabled;

        private User _selectedUser;

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;

                    if (value is null)
                        EditUser = null;
                    else
                    {

                        EditUser = new User
                        {
                            ID = _selectedUser.ID,
                            UserName = _selectedUser.UserName,
                            Password = _selectedUser.Password,
                            Email = _selectedUser.Email
                        };
                    }

                }
            }
        }

        private User _editUser;

        public User EditUser
        {
            get => _editUser;
            set
            {
                if (_editUser != value)
                {
                    _editUser = value;
                    OnPropertyChanged(nameof(EditUser));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        private bool _IsEnabled = false;
        public bool IsEnabled
        {
            get => _IsEnabled;
            set
            {
                if (_IsEnabled != value)
                {
                    _IsEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                    OnPropertyChanged(nameof(IsGridEnabled));
                }
            }
        }

        public bool IsGridEnabled => !IsEnabled;

        public class RelayCommand : ICommand
        {
            private readonly Action<object?> _execute;
            private readonly Predicate<object?>? _canExecute;
            public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object? parameter)
            {
                return _canExecute == null || _canExecute(parameter);
            }

            public void Execute(object? parameter)
            {
                _execute(parameter);
            }

            public event EventHandler? CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public void RaiseCanExecuteChanged()
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public class MultiParamConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
