using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAssets
{
    public class MainViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public PurchasesViewModel PurchaseVM { get; set; }
        public PartsViewModel PartVM { get; set; }
        public ITAssemblyViewModel ITAssemblyVM { get; set; }
        public ASPartsViewModel ASPartVM { get; set; }
        public UsersViewModel UserVM { get; set; }
        public LoginViewModel LoginVM { get; set; }
        public MainViewModel()
        {
            PurchaseVM = new PurchasesViewModel(this);
            PartVM = new PartsViewModel();
            ITAssemblyVM = new ITAssemblyViewModel(this);
            ASPartVM = new ASPartsViewModel();
            UserVM = new UsersViewModel();
            LoginVM = new LoginViewModel(this);
        }


        private bool _isLoggedIn = false;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                if (_isLoggedIn != value)
                {
                    _isLoggedIn = value;
                    OnPropertyChanged(nameof(IsLoggedIn));
                }
            }
        }

        private int _selectedTab;
        public int SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (_selectedTab != value)
                {
                    _selectedTab = value;
                    OnPropertyChanged(nameof(SelectedTab));
                }
            }
        }


    }
}
