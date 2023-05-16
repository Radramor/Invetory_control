using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invetory_control
{
    internal class Product
    {
        public int count { get; private set; }
        public uint id { get; private set; }
        public string? name { get; private set; }
        public string? unit { get; private set; }

        public Product(uint Id, string Name, string Unit)
        {
            count = 0;
            id = Id;
            name = Name;
            unit = Unit;
        }

        [JsonConstructor]
        public Product(int Count,uint Id, string Name, string Unit)
        {
            count = Count;
            id = Id;
            name = Name;
            unit = Unit;
        }

        internal void SetValues(string Name, string Unit, uint Id)
        {
            name = Name;
            unit = Unit;
            id = Id;
        }

        internal void AddCount(int Count)
        {
            count += Count;
        }
    }
}
