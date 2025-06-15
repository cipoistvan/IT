using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ITAssets
{
    
    public class DesignLoginViewModel
    {
        public User LoginUser { get; set; }

        public DesignLoginViewModel()
        {
            LoginUser = new User { Email = "a@a.hu", Password = "" };
        }

    }

    public class LoginViewModel:INotifyPropertyChanged
    {
        public DatabaseService DBConnection;
        private User _loginUser;
        public User LoginUser {
            get { return _loginUser; }
            set { 
                _loginUser = value;
                OnPropertyChanged(nameof(LoginUser));
            }
        
        }
        public ICommand LoginStart { get; }
        public ICommand Login { get; }
        public ICommand LoginCancel { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private MainViewModel mainviewmodel;

        

        public LoginViewModel(MainViewModel mainViewModel)
        {
            mainviewmodel = mainViewModel;
            DBConnection = new DatabaseService(App.connectionString);

            LoginUser = new User { Email = "", Password = "" };

            LoginStart = new RelayCommand(
                execute: _ =>
                {
                    IsLoginMode = true;
                },


                canExecute: _ => !IsLoginMode
            );

            Login = new RelayCommand(ExecuteLogin, CanExecuteLogin);


            LoginCancel = new RelayCommand(
                execute: parameter =>
                {
                    IsLoginMode = false;
                },
                canExecute: _ => IsLoginMode
                );


        }

        private void ExecuteLogin(object parameter)
        {
            LoginUser.Password = ((PasswordBox)parameter).Password;
            var FoundUser = DBConnection.GetUser(LoginUser);

            bool ValidCredentials = true;

            if (FoundUser is null)
            {
                ValidCredentials = false;
            }
            else
            {
                if (UsersViewModel.InvalidPassword(FoundUser.Password))
                    ValidCredentials = false;
                else if (!BCrypt.Net.BCrypt.Verify(LoginUser.Password, FoundUser.Password))
                    ValidCredentials = false;
                
            }

            if (ValidCredentials)
            {
                LoginUser = FoundUser;
                IsLoginMode = false;
                mainviewmodel.IsLoggedIn = true;
                mainviewmodel.SelectedTab = 4;
                App.logger.LogInformation($"Sikeres bejelentkezés (Email: {LoginUser.Email})");
            }
            else
            {
                MessageBox.Show("E-mail cím, vagy jelszó hibás !");
                App.logger.LogWarning($"Érvénytelen bejelentkezési kísérlet (Email: {LoginUser.Email})");
            }


        }

        private bool CanExecuteLogin(object parameter) => IsLoginMode;

        private bool _IsLoginMode = false;
        public bool IsLoginMode
        {
            get => _IsLoginMode;
            set
            {
                if (_IsLoginMode != value)
                {
                    _IsLoginMode = value;
                    OnPropertyChanged(nameof(IsLoginMode));
                }
            }
        }


    }

}
