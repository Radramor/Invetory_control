using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Invetory_control
{
    internal class Warehouse
    {
        public string? name { get; private set; }
        public List<Product> products { get; private set; }
        public List<OpBuy> opBuys { get; private set; }
        public List<OpMoving> opMovings { get; private set; }
        public List<OpWriteOff> opWriteOffs { get; private set; }
        public List<OpInventory> opInventorys { get; private set; }
        
        public Warehouse(string Name, List<Product> ProductList)
        {
            products = ProductList;
            name = Name;
            opBuys = new List<OpBuy>();
            opMovings = new List<OpMoving>();
            opWriteOffs = new List<OpWriteOff>();
            opInventorys = new List<OpInventory>();
        }

        [JsonConstructor]
        public Warehouse(string Name, List<Product> Products, List<OpBuy> OpBuys, List<OpMoving> OpMovings, List<OpWriteOff> OpWriteOffs, List<OpInventory> OpInventorys)
        {
            products = Products;
            name = Name;
            opBuys = OpBuys;
            opMovings = OpMovings;
            opWriteOffs = OpWriteOffs;
            opInventorys = OpInventorys;
        }

        public void AddOperations(DataGridView OperationDataGridView,
            string Date, string Type, string Warehouse, string NewWarehouse,
            string NameProduct, int Count, uint UnitPrice, string Comment, int Id, bool isWriteInDataGridView)
        {
            OperationDataGridView.Rows.Add();
            int indexRow = OperationDataGridView.Rows.Count - 1;
            int indexProduct = SearchIndexProduct(NameProduct);

            switch (Type)
            {
                case "Закупка":
                    uint Sum = (uint)(Count * UnitPrice);
                    opBuys.Add(new OpBuy(Id, Count, Comment, Date, products[indexProduct], UnitPrice, Sum));
                    OpBuy ob = opBuys.Last();
                    OperationDataGridView.Rows[indexRow].SetValues(ob.id, ob.date, Type, Warehouse, NewWarehouse, ob.product.name,
                        ob.product.unit, ob.count, ob.unitPrice, ob.sum, ob.commentary);

                    OperationDataGridView.Rows[indexRow].Cells[4].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[indexRow].Cells[8].Style.BackColor = Color.White;
                    OperationDataGridView.Rows[indexRow].Cells[9].Style.BackColor = Color.Silver;

                    break;

                case "Перемещение":
                    opMovings.Add(new OpMoving(Id, Count, Comment, Date, products[indexProduct], NewWarehouse));
                    OpMoving om = opMovings.Last();
                    if (isWriteInDataGridView)
                    {
                        OperationDataGridView.Rows[indexRow].SetValues(om.id, om.date, Type, Warehouse, NewWarehouse, om.product.name,
                        om.product.unit, om.count, "", "", om.commentary);

                        OperationDataGridView.Rows[indexRow].Cells[8].Style.BackColor = Color.Black;
                        OperationDataGridView.Rows[indexRow].Cells[9].Style.BackColor = Color.Black;
                    }
                    else OperationDataGridView.Rows.RemoveAt(indexRow);
                    break;

                case "Списание":
                    Count = -Math.Abs(Count);
                    opWriteOffs.Add(new OpWriteOff(Id, Count, Comment, Date, products[indexProduct]));
                    OpWriteOff ow = opWriteOffs.Last();

                    OperationDataGridView.Rows[indexRow].SetValues(ow.id, ow.date, Type, Warehouse, NewWarehouse, ow.product.name,
                        ow.product.unit, ow.count, "", "", ow.commentary);

                    OperationDataGridView.Rows[indexRow].Cells[4].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[indexRow].Cells[8].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[indexRow].Cells[9].Style.BackColor = Color.Black;
                    break;

                case "Инвентаризация":
                    opInventorys.Add(new OpInventory(Id, Count, Comment, Date, products[indexProduct]));
                    OpInventory oi = opInventorys.Last();

                    OperationDataGridView.Rows[indexRow].SetValues(oi.id, oi.date, Type, Warehouse, NewWarehouse, oi.product.name,
                        oi.product.unit, oi.count, "", "", oi.commentary);

                    OperationDataGridView.Rows[indexRow].Cells[4].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[indexRow].Cells[8].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[indexRow].Cells[9].Style.BackColor = Color.Black;
                    break;
            }
            ProductCountUpdate();            
        }

        public void ChangeOperations(DataGridView OperationDataGridView,
            string Date, string Type, string Warehouse, string NewWarehouse,
            string NameProduct, int Count, uint UnitPrice, string Comment, int Id)
        {
            int indexRow = OperationDataGridView.CurrentRow.Index;
            int indexProduct = SearchIndexProduct(NameProduct);

            switch (Type)
            {
                case "Закупка":
                    uint Sum = (uint)(Count * UnitPrice);
                    opBuys.Add(new OpBuy(Id, Count, Comment, Date, products[indexProduct], UnitPrice, Sum));
                    OpBuy ob = opBuys.Last();

                    OperationDataGridView.Rows[indexRow].SetValues(ob.id, ob.date, Type, Warehouse, NewWarehouse, ob.product.name,
                        ob.product.unit, ob.count, ob.unitPrice, ob.sum, ob.commentary);
                    OperationDataGridView.Rows[indexRow].Cells[4].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[indexRow].Cells[8].Style.BackColor = Color.White;
                    OperationDataGridView.Rows[indexRow].Cells[9].Style.BackColor = Color.Silver;
                    break;

                case "Перемещение":
                    opMovings.Add(new OpMoving(Id, Count, Comment, Date, products[indexProduct], NewWarehouse));
                    OpMoving om = opMovings.Last();

                    OperationDataGridView.Rows[indexRow].SetValues(om.id, om.date, Type, Warehouse, NewWarehouse, om.product.name,
                        om.product.unit, om.count, "", "", om.commentary);

                    OperationDataGridView.Rows[indexRow].Cells[4].Style.BackColor = Color.White;
                    OperationDataGridView.Rows[indexRow].Cells[8].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[indexRow].Cells[9].Style.BackColor = Color.Black;
                    break;

                case "Списание":
                    Count = -Math.Abs(Count);
                    opWriteOffs.Add(new OpWriteOff(Id, Count, Comment, Date, products[indexProduct]));
                    OpWriteOff ow = opWriteOffs.Last();

                    OperationDataGridView.Rows[indexRow].SetValues(ow.id, ow.date, Type, Warehouse, NewWarehouse, ow.product.name,
                        ow.product.unit, ow.count, "", "", ow.commentary);

                    OperationDataGridView.Rows[indexRow].Cells[4].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[indexRow].Cells[8].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[indexRow].Cells[9].Style.BackColor = Color.Black;
                    break;

                case "Инвентаризация":
                    opInventorys.Add(new OpInventory(Id, Count, Comment, Date, products[indexProduct]));
                    OpInventory oi = opInventorys.Last();

                    OperationDataGridView.Rows[indexRow].SetValues(oi.id, oi.date, Type, Warehouse, NewWarehouse, oi.product.name,
                        oi.product.unit, oi.count, "", "", oi.commentary);

                    OperationDataGridView.Rows[indexRow].Cells[4].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[indexRow].Cells[8].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[indexRow].Cells[9].Style.BackColor = Color.Black;
                    break;
            }
            ProductCountUpdate();
        }

        private void ProductCountUpdate()
        {
            foreach (Product product in products)
            {
                product.AddCount(-product.count);

                for (int i = 0; i < opBuys.Count; i++)
                {
                    if (opBuys[i].product.name == product.name)
                        product.AddCount(opBuys[i].count);
                }

                for (int i = 0; i < opInventorys.Count; i++)
                {
                    if (opInventorys[i].product.name == product.name)
                        product.AddCount(opInventorys[i].count);
                }

                for (int i = 0; i < opMovings.Count; i++)
                {
                    if (opMovings[i].product.name == product.name)
                        product.AddCount(opMovings[i].count);
                }

                for (int i = 0; i < opWriteOffs.Count; i++)
                {
                    if (opWriteOffs[i].product.name == product.name)
                        product.AddCount(opWriteOffs[i].count);
                }
            }
        }
        public int SearchIndexProduct(string NameProduct)
        {
            for(int i = 0; i < products.Count; i++)
            {
                if (products[i].name == NameProduct) return i;
            }
            return -1;
        }

        //по id в таблице находит операцию и удаляет его
        public void DeleteOperationInList (int Id, string NameProduct)
        {
            int indexProduct = SearchIndexProduct(NameProduct); 

            for (int i = 0; i < opBuys.Count; i++)
            {
                if (Id == opBuys[i].id)
                {
                    products[indexProduct].AddCount(-opBuys[i].count);
                    opBuys.RemoveAt(i);
                }
            }

            for (int i = 0; i < opInventorys.Count; i++)
            {
                if (Id == opInventorys[i].id)
                {
                    products[indexProduct].AddCount(-opInventorys[i].count);
                    opInventorys.RemoveAt(i);
                }
            }

            for (int i = 0; i < opMovings.Count; i++)
            {
                if (Id == opMovings[i].id)
                {
                    products[indexProduct].AddCount(-opMovings[i].count);
                    opMovings.RemoveAt(i);
                }
            }

            for (int i = 0; i < opWriteOffs.Count; i++)
            {
                if (Id == opWriteOffs[i].id)
                {
                    products[indexProduct].AddCount(-opWriteOffs[i].count);
                    opWriteOffs.RemoveAt(i);
                }
            }
        }

        //проверяет совпадение id c имеющимися операциями 
        public bool CheckId(int Id)
        {
            for (int i = 0; i < opBuys.Count; i++)
            {
                if (Id == opBuys[i].id)
                    return true;                
            }

            for (int i = 0; i < opInventorys.Count; i++)
            {
                if (Id == opInventorys[i].id)
                    return true;
            }

            for (int i = 0; i < opMovings.Count; i++)
            {
                if (Id == opMovings[i].id)
                    return true;
            }

            for (int i = 0; i < opWriteOffs.Count; i++)
            {
                if (Id == opWriteOffs[i].id)
                    return true;
            }

            return false;
        }

        internal int SearchCountProductInOperation(int Id)
        {
            for (int i = 0; i < opBuys.Count; i++)
            {
                if (Id == opBuys[i].id)
                    return opBuys[i].count;
            }

            for (int i = 0; i < opInventorys.Count; i++)
            {
                if (Id == opInventorys[i].id)
                    return opInventorys[i].count;
            }

            for (int i = 0; i < opMovings.Count; i++)
            {
                if (Id == opMovings[i].id)
                    return opMovings[i].count;
            }

            for (int i = 0; i < opWriteOffs.Count; i++)
            {
                if (Id == opWriteOffs[i].id)
                    return opWriteOffs[i].count;
            }
            return -1;
        }
        public void SetName (string Name)
        {
            name = Name;
        }
    }
}
