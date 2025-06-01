using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAssets
{
    public class MainViewModel
    {
        public PurchasesViewModel PurchaseVM { get; set; }
        public PartsViewModel PartVM { get; set; }
        public ITAssemblyViewModel ITAssemblyVM { get; set; }
        public ASPartsViewModel ASPartVM { get; set; }
        public MainViewModel()
        {
            PurchaseVM = new PurchasesViewModel();
            PartVM = new PartsViewModel();
            ITAssemblyVM = new ITAssemblyViewModel();
            ASPartVM = new ASPartsViewModel();
        }


    }
}
