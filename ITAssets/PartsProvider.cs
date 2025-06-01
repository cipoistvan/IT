using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAssets
{
    public class DesignPartsViewModel
    {
        public List<Part> Parts{ get; set; }

        public DesignPartsViewModel()
        {
            Parts = new List<Part>
            {
                new Part { ID = 1, Name = "SSD 1TB", CategoryName = "SSD"},
                new Part { ID = 2, Name = "Intel i5", CategoryName = "CPU"},
                new Part { ID = 3, Name = "DDR4 RAM 16GB", CategoryName = "RAM" }
            };

        }

    }

    public class PartsViewModel
    {
        public List<Part> Parts { get; }

        public PartsViewModel()
        {
            // Purchases = new DesignPurchaseProvider().Purchases;
            Parts = new DatabaseService(App.connectionString).GetParts();
        }
    }

}


