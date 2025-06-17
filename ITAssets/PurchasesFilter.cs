using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ITAssets
{
    public class PurchasesFilter
    {

        private DateTime? fromDate = DateTime.Now;
        public DateTime? FromDate
        {
            get => fromDate;
            set
            {
                if (fromDate != value)
                {
                    fromDate = value;
                    OnPropertyChanged(nameof(FromDate));
                }
            }
        }

        private DateTime? toDate;
        public DateTime? ToDate
        {
            get => toDate;
            set
            {
                if (toDate != value)
                {
                    toDate = value;
                    OnPropertyChanged(nameof(ToDate));
                }
            }
        }

        private string partName;
        public string PartName
        {
            get => partName;
            set
            {
                if (partName != value)
                {
                    partName = value;
                    OnPropertyChanged(nameof(PartName));
                }
            }
        }

        private string partType;
        public string PartType
        {
            get => partType;
            set
            {
                if (partType != value)
                {
                    partType = value;
                    OnPropertyChanged(nameof(PartType));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}