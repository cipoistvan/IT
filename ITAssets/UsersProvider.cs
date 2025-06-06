using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        public User EditUser
        {
            get
            {
                return new User { UserName = "test" };
            }

        }

    }

    public class UsersViewModel : INotifyPropertyChanged
    {
        public DatabaseService DBConnection;
        public bool _IsAddMode = false;
        public ObservableCollection<User> Users { get; }

        public ICommand AddUserCmd { get; }
        public ICommand ModifyUserCmd { get; }
        public ICommand CancelUserCmd { get; }
        public ICommand DeleteUserCmd { get; }
        public ICommand SaveUserCmd { get; }


        public UsersViewModel()
        {

            DBConnection = new DatabaseService(App.connectionString);
            Users = DBConnection.GetUsers();


            AddUserCmd = new RelayCommand
                (
                    execute: _ =>
                    {
                        IsEditMode = true;
                        _IsAddMode = true;
                        EditUser = null;
                    },
                    canExecute: _ => !IsEditMode
                );

            ModifyUserCmd = new RelayCommand
                (
                    execute: _ => IsEditMode = true,
                    canExecute: _ => SelectedUser != null && !IsEditMode
                );

            DeleteUserCmd = new RelayCommand
                (
                    execute: _ =>
                    {
                        var result = DBConnection.DeleteUser(SelectedUser);

                        switch (result)
                        {
                            case DeleteResult.Success:
                                SelectedUser = null;
                                Users.Clear();
                                foreach (var user in DBConnection.GetUsers())
                                {
                                    Users.Add(user);
                                }
                                break;

                            case DeleteResult.ForeignKeyConstraint:
                                MessageBox.Show("Nem törölhető a felhasználó, mert más rekord hivatkozik rá.");
                                break;

                            case DeleteResult.Error:
                                MessageBox.Show("Ismeretlen adatbázis hiba történt.");
                                break;

                        }

                    },
                    canExecute: _ => SelectedUser != null && !IsEditMode

                );

            SaveUserCmd = new RelayCommand(ExecuteSave, CanExecuteSave);

            CancelUserCmd = new RelayCommand
                (
                    execute: parameter =>
                    {
                        IsEditMode = false;
                        _IsAddMode = false;
                        EditUser = SelectedUser;

                        if (parameter is object[] values && values.Length == 2)
                        {
                            ((PasswordBox)values[0]).Password = "";
                            ((PasswordBox)values[1]).Password = "";
                        }
                    },
                    canExecute: _ => IsEditMode == true

                );
        }
        private void ExecuteSave(object parameter)
        {

            if (parameter is object[] values && values.Length == 2)
            {
                string val1 = ((PasswordBox)values[0]).Password;
                string val2 = ((PasswordBox)values[1]).Password;

                bool EmptyStoredPassword = string.IsNullOrEmpty(EditUser.Password);
                bool InvalidStoredPassword = EmptyStoredPassword || InvalidPassword(EditUser.Password);
                bool EmptyPasswords = string.IsNullOrEmpty(val1) && string.IsNullOrEmpty(val2);
                bool EqualPasswords = val1 == val2;

                if (EmptyStoredPassword && EmptyPasswords)
                {
                    MessageBox.Show("Nincs jelszó megadva");
                }
                else if (!EqualPasswords)
                {
                    MessageBox.Show("A két jelszó nem egyezik");
                }

                else if (EmptyPasswords && InvalidStoredPassword)
                {
                    MessageBox.Show("A jelenlegi jelszó nem biztonságosan tárolt, adjon meg újat!");
                }
                else
                {
                    ((PasswordBox)values[0]).Password = "";
                    ((PasswordBox)values[1]).Password = "";

                    if (InvalidStoredPassword || !BCrypt.Net.BCrypt.Verify(val1, EditUser.Password))
                        EditUser.Password = BCrypt.Net.BCrypt.HashPassword(val1);


                    UpdateResult result;

                    if (_IsAddMode)
                    {
                        result = DBConnection.AddUser(EditUser);
                    }
                    else
                    {
                        result = DBConnection.ModifyUser(EditUser);
                    }



                    switch (result)
                    {
                        case UpdateResult.Success:
                            SelectedUser = null;
                            Users.Clear();
                            foreach (var user in DBConnection.GetUsers())
                            {
                                Users.Add(user);
                            }

                            EditUser.Password = "";
                            _IsAddMode = false;
                            IsEditMode = false;

                            break;

                        case UpdateResult.Duplicate:
                            MessageBox.Show("Ilyen email címmel már létezik felhasználó, adj meg másikat !");
                            break;

                        case UpdateResult.Error:
                            MessageBox.Show("Ismeretlen adatbázis hiba történt.");
                            break;
                    }
                }
            }
        }


        public static bool InvalidPassword(string hash)
        {

            try
            {
                BCrypt.Net.BCrypt.Verify("", hash);
                return false;
            }
            catch (BCrypt.Net.SaltParseException)

            {
                return true;
            }

        }

        private bool CanExecuteSave(object parameter) => IsEditMode;

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

                        EditUser = SelectedUser;
                    }

                }
            }
        }

        private User _editUser;
        private User _buffer1 = new User();
        private User _buffer2 = new User();
        private User target;

        public User EditUser
        {
            get => _editUser;
            set
            {

                if (value == null)
                {
                    _buffer1.ID = 0;
                    _buffer1.UserName = "";
                    _buffer1.Password = "";
                    _buffer1.Email = "";
                    target = _buffer1;
                }
                else
                {
                    target = ReferenceEquals(EditUser, _buffer1) ? _buffer2 : _buffer1;

                    target.ID = value.ID;
                    target.UserName = value.UserName;
                    target.Password = value.Password;
                    target.Email = value.Email;
                }

                _editUser = target;
                OnPropertyChanged(nameof(EditUser));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        private bool _IsEditMode = false;
        public bool IsEditMode
        {
            get => _IsEditMode;
            set
            {
                if (_IsEditMode != value)
                {
                    _IsEditMode = value;
                    OnPropertyChanged(nameof(IsEditMode));
                    OnPropertyChanged(nameof(IsGridEnabled));
                }
            }
        }

        public bool IsGridEnabled => !IsEditMode;

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
            if (values.Length == 2 && values[0] is PasswordBox box1 && values[1] is PasswordBox box2)
                return new[] { box1, box2 };
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}