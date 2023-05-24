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
            if (Count <= 0)
                throw new ArgumentException("В данной операции может быть только положительное количество продуктов");
            if (UnitPrice <= 0)
                throw new ArgumentException("Цена за единицу может быть только положительным числом");

            count = Count;
            unitPrice = UnitPrice;
            commentary = Commentary;
            date = Date;
            product = Product;
            id = Id;
            sum = Sum;
        }
    }
}
