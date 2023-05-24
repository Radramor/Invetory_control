using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invetory_control
{
    internal class OpMoving : Operations
    {
        public string? newWarehouse { get; private set; }
        public OpMoving(int Id, int Count, string Commentary, string Date, Product Product, string NewWarehouse) 
        {
            if (Count == 0)
                throw new ArgumentException("Количество продуктов не может быть равен нулю");

            count = Count;
            id = Id;
            commentary = Commentary;
            date = Date;
            product = Product;
            newWarehouse = NewWarehouse;
        }
    }
}
