using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAssets
{
    public class DesignMainViewModel
    {

        public DesignPurchasesViewModel PurchaseVM { get; set; }
        public DesignPartsViewModel PartVM { get; set; }
        public DesignITAssembyViewModel ITAssemblyVM { get; set; }
        public DesignASPartsViewModel ASPartVM { get; set; }

        public DesignMainViewModel()
        {
            PurchaseVM = new DesignPurchasesViewModel();
            PartVM = new DesignPartsViewModel();
            ITAssemblyVM = new DesignITAssembyViewModel();
            ASPartVM = new DesignASPartsViewModel();
        }


    }
}
