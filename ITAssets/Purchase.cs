using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAssets
{
    public class Purchase
    {

        public int ID{ get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; }
        public string ItemName { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total => Quantity * UnitPrice;
    }
}
