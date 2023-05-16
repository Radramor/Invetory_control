using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invetory_control
{
    abstract class Operations
    {
        public int count { get; private protected set; }
        public string? commentary { get; private protected set; }
        public string? date { get; private protected set; }
        public Product? product { get; private protected set; }
        public int id { get; private protected set; }

    }
}
