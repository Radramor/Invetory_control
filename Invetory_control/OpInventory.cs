using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invetory_control
{
    internal class OpInventory : Operations
    {
        public OpInventory(int Id, int Count, string Commentary, string Date, Product Product)
        {
            if (Count == 0)
                throw new ArgumentException("Количество продуктов не может быть равен нулю");

            count = Count;
            id = Id;
            commentary = Commentary;
            date = Date;
            product = Product;
        }
    }
}
