using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAssets
{
    public class DesignPurchasesViewModel
    {
        public List<Purchase> Purchases { get; set; }

        public DesignPurchasesViewModel()
        {
            Purchases = new List<Purchase>
            {
                new Purchase { ID = 1, Date = DateTime.Today, User = "admin", ItemName = "SSD 1TB", Type = "Storage", Quantity = 2, UnitPrice = 25000 },
                new Purchase { ID = 2, Date = DateTime.Today, User = "admin", ItemName = "Intel i5", Type = "CPU", Quantity = 1, UnitPrice = 60000 },
                new Purchase { ID = 3, Date = DateTime.Today, User = "admin", ItemName = "DDR4 RAM 16GB", Type = "Memory", Quantity = 4, UnitPrice = 15000 }
            };

        }

    }

    public class PurchasesViewModel
    {
        public List<Purchase> Purchases { get; }

        public PurchasesViewModel()
        {
            //Purchases = new DesignPurchasesViewModel().Purchases;
            Purchases = new DatabaseService(App.connectionString).GetPurchases();
        }
    }

}

