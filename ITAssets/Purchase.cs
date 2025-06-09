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
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int PartId { get; set; }
        public string PartName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime? Date { get; set; }
        public decimal Total => Quantity * UnitPrice;
    }
}
