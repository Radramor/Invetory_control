using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invetory_control
{
    internal class OpMoving : Operations
    {
        public string? newWarehouse;
        public OpMoving(int Id, int Count, string Commentary, string Date, Product Product, string OldWarehouse) 
        {
            id = Id;
            count = Count;
            commentary = Commentary;
            date = Date;
            product = Product;
            newWarehouse = OldWarehouse;
        }
    }
}
