using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invetory_control
{
    internal class OpWriteOff : Operations
    {
        public OpWriteOff(int Id, int Count, string Commentary, string Date, Product Product)
        {
            if (Count >= 0)
                throw new ArgumentException("В данной операции может быть только отрицательное количество продуктов");

            count = Count;
            id = Id;
            commentary = Commentary;
            date = Date;
            product = Product;
        }
    }
}
