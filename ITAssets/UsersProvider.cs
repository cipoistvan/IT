using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ITAssets
{
    public class DesignUsersViewModel
    {
        public List<User> Users { get; set; }

        public DesignUsersViewModel()
        {
            Users = new List<User>
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
        public List<User> Users { get; }
        

        public UsersViewModel()
        {
            Users = new DatabaseService(App.connectionString).GetUsers();
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

    }

}
