using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        
        public ICommand ToggleReadOnlyCmd { get; }

        public UsersViewModel()
        {
            Users = new DatabaseService(App.connectionString).GetUsers();
            ToggleReadOnlyCmd = new RelayCommand
                (
                    execute: _ => IsReadOnly1 = false,
                    canExecute: _ => SelectedUser != null
                );

        }
        private User _selectedUser;

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    //OnPropertyChanged(); // INotifyPropertyChanged

                    //OnUserSelected(_selectedUser);

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
        public void OnUserSelected(User selected)
        {
            if (selected != null)
            {
                MessageBox.Show(selected.UserName);
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
        protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        private bool _IsReadOnly1 = true;
        public bool IsReadOnly1
        {
            get => _IsReadOnly1;
            set
            {
                if (_IsReadOnly1 != value)
                {
                    _IsReadOnly1 = value;
                    OnPropertyChanged(nameof(IsReadOnly1));
                }
            }
        }


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

}
