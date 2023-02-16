using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invetory_control
{
    internal class Warehouse
    {
        public string? name;
        public int countProduct; 

        public Warehouse()
        {
            countProduct = 0;
            name = "";
        }
    }
}
