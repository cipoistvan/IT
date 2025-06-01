using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAssets
{
    public class DesignASPartsViewModel
    {
        public List<ASPart> ASParts { get; set; }

        public DesignASPartsViewModel()
        {
            ASParts = new List<ASPart>
            {
                new ASPart { ID = 1, AssemblyName="PC1", PartName = "SSD 1TB", CategoryName = "SSD", Quantity = 1 },
                new ASPart { ID = 2, AssemblyName="PC1", PartName = "Intel i5", CategoryName = "CPU", Quantity = 1},
                new ASPart { ID = 3, AssemblyName="PC1", PartName = "DDR4 RAM 16GB", CategoryName = "RAM", Quantity = 1 }
            };

        }

    }

    public class ASPartsViewModel
    {
        public List<ASPart> ASParts { get; }

        public ASPartsViewModel()
        {
            // Purchases = new DesignPurchaseProvider().Purchases;
            ASParts = new DatabaseService(App.connectionString).GetASParts();
        }
    }

}


