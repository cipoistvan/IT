using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAssets
{
    public class ASPart
    {
        public int ID { get; set; }
        public int AssemblyID { get; set; }
        public string AssemblyName { get; set; }
        public int PartID { get; set; }
        public string PartName { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int Quantity { get; set; }

    }
}