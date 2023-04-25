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
        public string? name;
        public List<Product> products;
        public List<OpBuy> opBuys;
        public List<OpMoving> opMovings;
        public List<OpWriteOff> opWriteOffs;
        public List<OpInventory> opInventorys;

        public Warehouse(string Name, List<Product> ProductList)
        {
            products = ProductList;
            name = Name;
        }
        public void AddOperations(DataGridView OperationDataGridView,
            string Date, string Type, string Warehouse, string NewWarehouse,
            string NameProduct, int Count, uint UnitPrice, string Comment, int Id, bool isWriteInDataGridView)
        {
            int indexRow;
            int indexProduct = SearchIndexProduct(NameProduct);

            switch (Type)
            {
                case "Закупка":

                    if (opBuys == null)
                        opBuys = new List<OpBuy>();
                    uint Sum = (uint)(Count * UnitPrice);
                    opBuys.Add(new OpBuy(Id, Count, Comment, Date, products[indexProduct], UnitPrice, Sum));
                    OpBuy ob = opBuys.Last();
                    OperationDataGridView.Rows.Add(ob.id, ob.date, Type, Warehouse, NewWarehouse, ob.product.name,
                        ob.product.unit, ob.count, ob.unitPrice, ob.sum, ob.commentary);

                    indexRow = OperationDataGridView.Rows.Count - 1;
                    OperationDataGridView.Rows[indexRow].Cells[4].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[indexRow].Cells[8].Style.BackColor = Color.White;
                    OperationDataGridView.Rows[indexRow].Cells[9].Style.BackColor = Color.Silver;

                    break;

                case "Перемещение":
                    if (opMovings == null)
                        opMovings = new List<OpMoving>();
                    opMovings.Add(new OpMoving(Id, Count, Comment, Date, products[indexProduct], NewWarehouse));
                    OpMoving om = opMovings.Last();
                    if (isWriteInDataGridView)
                    {
                        OperationDataGridView.Rows.Add(om.id, om.date, Type, Warehouse, NewWarehouse, om.product.name,
                        om.product.unit, om.count, "", "", om.commentary);

                        indexRow = OperationDataGridView.Rows.Count - 1;
                        OperationDataGridView.Rows[indexRow].Cells[8].Style.BackColor = Color.Black;
                        OperationDataGridView.Rows[indexRow].Cells[9].Style.BackColor = Color.Black;
                    }
                    break;

                case "Списание":
                    Count = -Math.Abs(Count);
                    if (opWriteOffs == null)
                        opWriteOffs = new List<OpWriteOff>();
                    opWriteOffs.Add(new OpWriteOff(Id, Count, Comment, Date, products[indexProduct]));
                    OpWriteOff ow = opWriteOffs.Last();

                    OperationDataGridView.Rows.Add(ow.id, ow.date, Type, Warehouse, NewWarehouse, ow.product.name,
                        ow.product.unit, ow.count, "", "", ow.commentary);

                    indexRow = OperationDataGridView.Rows.Count - 1;
                    OperationDataGridView.Rows[indexRow].Cells[4].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[indexRow].Cells[8].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[indexRow].Cells[9].Style.BackColor = Color.Black;

                    break;

                case "Инвентаризация":

                    if (opInventorys == null)
                        opInventorys = new List<OpInventory>();
                    opInventorys.Add(new OpInventory(Id, Count, Comment, Date, products[indexProduct]));
                    OpInventory oi = opInventorys.Last();

                    OperationDataGridView.Rows.Add(oi.id, oi.date, Type, Warehouse, NewWarehouse, oi.product.name,
                        oi.product.unit, oi.count, "", "", oi.commentary);

                    indexRow = OperationDataGridView.Rows.Count - 1;
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
            //DeleteOperationInList(Id, NameProduct);
            int indexRow;
            int indexProduct = SearchIndexProduct(NameProduct);

            switch (Type)
            {
                case "Закупка":

                    if (opBuys == null)
                        opBuys = new List<OpBuy>();
                    uint Sum = (uint)(Count * UnitPrice);
                    opBuys.Add(new OpBuy(Id, Count, Comment, Date, products[indexProduct], UnitPrice, Sum));
                    OpBuy ob = opBuys.Last();

                    indexRow = OperationDataGridView.CurrentRow.Index;

                    OperationDataGridView.Rows[indexRow].SetValues(ob.id, ob.date, Type, Warehouse, NewWarehouse, ob.product.name,
                        ob.product.unit, ob.count, ob.unitPrice, ob.sum, ob.commentary);
                    OperationDataGridView.Rows[indexRow].Cells[4].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[indexRow].Cells[8].Style.BackColor = Color.White;
                    OperationDataGridView.Rows[indexRow].Cells[9].Style.BackColor = Color.Silver;

                    break;

                case "Перемещение":
                    if (opMovings == null)
                        opMovings = new List<OpMoving>();
                    opMovings.Add(new OpMoving(Id, Count, Comment, Date, products[indexProduct], NewWarehouse));
                    OpMoving om = opMovings.Last();

                    indexRow = OperationDataGridView.CurrentRow.Index;
                    OperationDataGridView.Rows[indexRow].SetValues(om.id, om.date, Type, Warehouse, NewWarehouse, om.product.name,
                        om.product.unit, om.count, "", "", om.commentary);

                    OperationDataGridView.Rows[indexRow].Cells[4].Style.BackColor = Color.White;
                    OperationDataGridView.Rows[indexRow].Cells[8].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[indexRow].Cells[9].Style.BackColor = Color.Black;
                    
                    break;

                case "Списание":
                    Count = -Math.Abs(Count);
                    if (opWriteOffs == null)
                        opWriteOffs = new List<OpWriteOff>();
                    opWriteOffs.Add(new OpWriteOff(Id, Count, Comment, Date, products[indexProduct]));
                    OpWriteOff ow = opWriteOffs.Last();

                    indexRow = OperationDataGridView.CurrentRow.Index;
                    OperationDataGridView.Rows[indexRow].SetValues(ow.id, ow.date, Type, Warehouse, NewWarehouse, ow.product.name,
                        ow.product.unit, ow.count, "", "", ow.commentary);

                    OperationDataGridView.Rows[indexRow].Cells[4].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[indexRow].Cells[8].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[indexRow].Cells[9].Style.BackColor = Color.Black;

                    break;

                case "Инвентаризация":

                    if (opInventorys == null)
                        opInventorys = new List<OpInventory>();
                    opInventorys.Add(new OpInventory(Id, Count, Comment, Date, products[indexProduct]));
                    OpInventory oi = opInventorys.Last();

                    indexRow = OperationDataGridView.CurrentRow.Index;
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
                product.count = 0;
                if (opBuys != null)
                    for (int i = 0; i < opBuys.Count; i++)
                    {
                        if (opBuys[i].product.name == product.name)
                            product.count += opBuys[i].count;
                    }

                if (opInventorys != null)
                    for (int i = 0; i < opInventorys.Count; i++)
                    {
                        if (opInventorys[i].product.name == product.name)
                            product.count += opInventorys[i].count;
                    }

                if (opMovings != null)
                    for (int i = 0; i < opMovings.Count; i++)
                    {
                        if (opMovings[i].product.name == product.name)
                            product.count += opMovings[i].count;
                    }

                if (opWriteOffs != null)
                    for (int i = 0; i < opWriteOffs.Count; i++)
                    {
                        if (opWriteOffs[i].product.name == product.name)
                            product.count += opWriteOffs[i].count;
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

            if (opBuys != null)
                for (int i = 0; i < opBuys.Count; i++)
                {
                    if (Id == opBuys[i].id)
                    {
                        products[indexProduct].count -= opBuys[i].count;
                        opBuys.RemoveAt(i);
                    }
                }

            if (opInventorys != null)
                for (int i = 0; i < opInventorys.Count; i++)
                {
                    if (Id == opInventorys[i].id)
                    {
                        products[indexProduct].count -= opInventorys[i].count;
                        opInventorys.RemoveAt(i);
                    }
                }

            if (opMovings != null)
                for (int i = 0; i < opMovings.Count; i++)
                {
                    if (Id == opMovings[i].id)
                    {
                        products[indexProduct].count -= opMovings[i].count;
                        opMovings.RemoveAt(i);
                    }
                }

            if (opWriteOffs != null)
                for (int i = 0; i < opWriteOffs.Count; i++)
                {
                    if (Id == opWriteOffs[i].id)
                    {
                        products[indexProduct].count -= opWriteOffs[i].count;
                        opWriteOffs.RemoveAt(i);
                    }
                }
        }

        //проверяет совпадение id c имеющимися операциями 
        public bool CheckId(int Id)
        {
            if (opBuys != null)
                for (int i = 0; i < opBuys.Count; i++)
                {
                    if (Id == opBuys[i].id)
                        return true;                
                }

            if (opInventorys != null)
                for (int i = 0; i < opInventorys.Count; i++)
                {
                    if (Id == opInventorys[i].id)
                        return true;
                }

            if (opMovings != null)
                for (int i = 0; i < opMovings.Count; i++)
                {
                    if (Id == opMovings[i].id)
                        return true;
                }

            if (opWriteOffs != null)
                for (int i = 0; i < opWriteOffs.Count; i++)
                {
                    if (Id == opWriteOffs[i].id)
                        return true;
                }

            return false;
        }

        internal int SearchCountProductInOldOperation(int Id)
        {
            if (opBuys != null)
                for (int i = 0; i < opBuys.Count; i++)
                {
                    if (Id == opBuys[i].id)
                        return opBuys[i].count;
                }

            if (opInventorys != null)
                for (int i = 0; i < opInventorys.Count; i++)
                {
                    if (Id == opInventorys[i].id)
                        return opInventorys[i].count;
                }

            if (opMovings != null)
                for (int i = 0; i < opMovings.Count; i++)
                {
                    if (Id == opMovings[i].id)
                        return opMovings[i].count;
                }

            if (opWriteOffs != null)
                for (int i = 0; i < opWriteOffs.Count; i++)
                {
                    if (Id == opWriteOffs[i].id)
                        return opWriteOffs[i].count;
                }
            return -1;
        }
    }
}
