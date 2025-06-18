using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ITAssets
{
    public class PurchasesFilter:INotifyPropertyChanged
    {

        private DateTime? fromDate;
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


    public interface IFilterStrategy<T>
    {
        bool Matches(T item);
    }

    public class PartNameFilter : IFilterStrategy<Purchase>
    {
        private readonly string keyword;
        public PartNameFilter(string keyword)
        {
            this.keyword = keyword?.ToLower();
        }

        public bool Matches(Purchase p)
        {
            return !string.IsNullOrWhiteSpace(keyword) &&
                   p.PartName?.ToLower().Contains(keyword) == true;
        }
    }

    public class PartTypeFilter : IFilterStrategy<Purchase>
    {
        private readonly string keyword;
        public PartTypeFilter(string keyword)
        {
            this.keyword = keyword?.ToLower();
        }

        public bool Matches(Purchase p)
        {
            return !string.IsNullOrWhiteSpace(keyword) &&
                   p.CategoryName?.ToLower().Contains(keyword) == true;
        }
    }




    public class DateRangeFilter : IFilterStrategy<Purchase>
    {
        private readonly DateTime? from;
        private readonly DateTime? to;

        public DateRangeFilter(DateTime? from, DateTime? to)
        {
            this.from = from;
            this.to = to;
        }

        public bool Matches(Purchase p)
        {
            if (from.HasValue && p.Date < from.Value) return false;
            if (to.HasValue && p.Date > to.Value) return false;
            return true;
        }
    }


    public class AndFilter<T> : IFilterStrategy<T>
    {
        private readonly List<IFilterStrategy<T>> filters;

        public AndFilter(IEnumerable<IFilterStrategy<T>> filters)
        {
            this.filters = filters.ToList();
        }

        public bool Matches(T item)
        {
            return filters.All(f => f.Matches(item));
        }
    }
}