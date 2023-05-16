using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invetory_control
{
    internal class OpBuy : Operations
    {
        public uint unitPrice { get; private set; }
        public uint sum { get; private set; }

        public OpBuy(int Id, int Count, string Commentary, string Date, Product Product, uint UnitPrice, uint Sum)
        {
            id = Id;
            count = Count;
            commentary = Commentary;
            date = Date;
            product = Product;
            unitPrice = UnitPrice;
            sum = Sum;
        }
    }
}
