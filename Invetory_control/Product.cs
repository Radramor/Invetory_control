using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invetory_control
{
    internal class Product
    {
        public int count;
        public uint id;
        public string? name;      
        public string? unit;

        public Product(uint Id, string Name, string Unit)
        {
            count = 0;
            id = Id;
            name = Name;
            unit = Unit;
        }
    }
}
