using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invetory_control
{
    public static class Extensions
    {
        public static string GetDescription(this OperationType? operationType)
        {
            switch (operationType)
            {
                case OperationType.Buy:
                    return "Закупка";
                case OperationType.Moving:
                    return "Перемещение";
                case OperationType.Inventory:
                    return "Инвентаризация";
                case OperationType.WriteOff:
                    return "Списание";
                default:
                    return "";
            }
        }
        public static IEnumerable<(Int32, X)> Ordinate<X>(this IEnumerable<X> lhs)
        {
            return lhs.Ordinate(0);
        }

        public static IEnumerable<(Int32, X)> Ordinate<X>(this IEnumerable<X> lhs, Int32 initial)
        {
            Int32 index = initial - 1;

            return lhs.Select(x => (++index, x));
        }
    }
    public enum OperationType
    {
        Buy = 0,
        Moving,
        WriteOff,
        Inventory,
    }
    internal class Operation 
    {
        public int count;
        public uint unitPrice;
        public uint sum;
        public string? commentary;
        public string? date;
        public string? oldWarehouse;
        public Product product;
        public OperationType? type;

        public Operation()
        {
            count = 0;
            unitPrice = 0;
            sum = 0;
            type = null;
            oldWarehouse = "";
            commentary = "";
            date = "";
            product = new Product();
        }  


    }
}
