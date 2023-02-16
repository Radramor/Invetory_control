using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invetory_control
{
    internal class Product
    {
        public uint id;
        public string? name;      
        public string? unit;
        public Warehouse warehouse;
        public Product() 
        {
            id = 0;
            name = "";
            unit = "";
            warehouse = new Warehouse();
        }
    }
}
