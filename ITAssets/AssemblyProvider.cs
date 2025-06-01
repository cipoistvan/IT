using ITAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAssets
{
    public class DesignITAssembyViewModel
    {
        public List<ITAssembly> ITAssemblies { get; set; }

        public DesignITAssembyViewModel()
        {
            ITAssemblies = new List<ITAssembly>
            {
                new ITAssembly { ID = 1, Name = "PC1", UserName = "admin", Date = DateTime.Today},
                new ITAssembly { ID = 2, Name = "PC2", UserName = "admin", Date = DateTime.Today},
                new ITAssembly { ID = 3, Name = "PC3", UserName = "admin", Date = DateTime.Today}
            };

        }
    }



    public class ITAssemblyViewModel
    {
        public List<ITAssembly> ITAssemblies { get; }

        public ITAssemblyViewModel()
        {
            ITAssemblies = new DatabaseService(App.connectionString).GetITAssemblies();
        }
    }
}



