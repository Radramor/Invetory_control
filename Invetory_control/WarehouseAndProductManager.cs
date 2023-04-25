using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Invetory_control
{
    internal class WarehouseAndProductManager
    {
        public List<Warehouse> WarehouseList = new List<Warehouse>();
        public List<Product> TotalProductList = new List<Product>();
        public void save()
        {
            File.WriteAllText("saveProduct.json", JsonConvert.SerializeObject(TotalProductList));
            File.WriteAllText("saveWarehouse.json", JsonConvert.SerializeObject(WarehouseList));
        }
        public void load()
        {
            WarehouseList = File.Exists("saveWarehouse.json") ? JsonConvert.DeserializeObject<List<Warehouse>>(File.ReadAllText("saveWarehouse.json")) : new List<Warehouse>();
            TotalProductList = JsonConvert.DeserializeObject<List<Product>>(File.ReadAllText("saveProduct.json"));
        }
        public void CreateRowWarehouse(DataGridView WarehouseDataGridView, TextBox textBox)
        {
            bool copy = false;
            for (int i = 0; i < WarehouseList.Count; i++)
            {
                if (WarehouseList[i].name == textBox.Text)
                    copy = true;
            }
            if (copy)
                WarehouseList.Add(new Warehouse("", TotalProductList));
            else
                WarehouseList.Add(new Warehouse(textBox.Text, TotalProductList));
            WarehouseDataGridView.Rows.Add(WarehouseList[WarehouseList.Count - 1].name);
        }

        internal void changeRowWarehouse(DataGridView productDataGridView, TextBox textBox)
        {
            int index = productDataGridView.CurrentCell.RowIndex;
            bool copy = false;

            for (int i = 0; i < WarehouseList.Count; i++)
            {
                if (i == index) continue;
                if (WarehouseList[i].name == textBox.Text)
                    copy = true;
            }
            if (!copy)
            {
                WarehouseList[index].name = textBox.Text;
                WarehouseList[index].products = TotalProductList;
                productDataGridView.Rows[index].SetValues(WarehouseList[index].name);
            }

        }
        public void DeleteRowProductOrWarehouse(DataGridView dataGridView, bool isProduct)
        {
            int index = dataGridView.CurrentCell.RowIndex;
            if (isProduct == true)
            {
                TotalProductList.RemoveAt(index);
                foreach (var warehouse in WarehouseList)
                {
                    warehouse.products.RemoveAt(index);
                }
            }
            else
                WarehouseList.RemoveAt(index);

            if (dataGridView.Rows.Count == 1)
            {
                dataGridView.Rows[index].SetValues("", "", "", "", "", "", "", "", "", "");
            }
            else
                dataGridView.Rows.RemoveAt(index);

            
        }

        public void CreateRowProduct(DataGridView ProductDataGridView, TextBox IdTextBox, TextBox NameTextBox, TextBox UnitTextBox)
        {
            if (uint.TryParse(IdTextBox.Text, out uint Id) || IdTextBox.Text == "")
            {
                string Name = NameTextBox.Text;
                string Unit = UnitTextBox.Text;
                bool isCopy = false;
                for (int i = 0; i < TotalProductList.Count; i++)
                {
                    if (TotalProductList[i].id == Id || TotalProductList[i].name == Name)
                        isCopy = true;
                }
                if (isCopy)
                {
                    Id = CreateIdProduct();
                    TotalProductList.Add(new Product(Id, "", ""));
                }
                else
                    TotalProductList.Add(new Product(Id, Name, Unit));

                foreach (var warehouse in WarehouseList)
                {
                    warehouse.products.Add(TotalProductList.Last());
                }
                ProductDataGridView.Rows.Add(TotalProductList.Last().id, TotalProductList.Last().name, TotalProductList.Last().unit);
            }
        }

        public void ChangeRowProduct (DataGridView ProductDataGridView, TextBox IdTextBox, TextBox NameTextBox, TextBox UnitTextBox)
        {
            int index = ProductDataGridView.CurrentCell.RowIndex;
            bool copy = false;
            if (uint.TryParse(IdTextBox.Text, out TotalProductList[index].id))
            {
                for (int i = 0; i < TotalProductList.Count; i++)
                {
                    if (i == index) continue;
                    if (TotalProductList[i].name == NameTextBox.Text || TotalProductList[i].id == int.Parse(IdTextBox.Text))
                        copy = true;
                }
                if (copy == false)
                {
                    TotalProductList[index].name = NameTextBox.Text;
                    TotalProductList[index].unit = UnitTextBox.Text;
                    ProductDataGridView.Rows[index].SetValues(TotalProductList[index].id, TotalProductList[index].name, TotalProductList[index].unit);
                }
                foreach (var warehouse in WarehouseList)
                {
                    warehouse.products[index].name = TotalProductList[index].name;
                    warehouse.products[index].unit = TotalProductList[index].unit;
                }
            }
        }

        //Ищет наименьший доступный id продукта
        private uint CreateIdProduct()
        {
            uint id = 0;
            bool isCopy = false;
            while (true)
            {
                foreach (var item in TotalProductList)
                {
                    if (item.id == id)
                        isCopy = true;

                }
                if (isCopy)
                {
                    isCopy = false;
                    id++;
                }
                else return id;
            }
        }

        public void TryAddOperation(DataGridView OperationDataGridView,
            DateTimePicker DateTimePicker, ComboBox TypeComboBox, ComboBox WarehouseComboBox, 
            ComboBox NewWarehouseComboBox, ListBox NameProductListBox,
            NumericUpDown CountNumericUpDown, NumericUpDown UnitPriceNumericUpDown,
            TextBox CommentTextBox)
        {
            //проверка на наличие необходимых данных
            if (TypeComboBox.SelectedIndex > -1 && WarehouseComboBox.SelectedIndex > -1 &&
                CountNumericUpDown.Value != 0 && NameProductListBox.SelectedIndex > -1) 
            {
                string Date = DateTimePicker.Text;
                string Type = TypeComboBox.Text;
                string Warehouse = WarehouseComboBox.Text;
                string NewWarehouse = NewWarehouseComboBox.Text;
                string NameProduct = NameProductListBox.Text;
                string Comment = CommentTextBox.Text;
                uint UnitPrice = uint.Parse(UnitPriceNumericUpDown.Value.ToString());
                int Count = int.Parse(CountNumericUpDown.Value.ToString());
                int indexWarehouse = SearchIndexWarehouse(Warehouse);
                int indexProduct = WarehouseList[indexWarehouse].SearchIndexProduct(NameProduct);

                if (Type == "Закупка" && Count < 0) return;
                //проверка на то, что операция не отнимет товаров больше чем в самом складе 
                if (WarehouseList[indexWarehouse].products[indexProduct].count - Math.Abs(Count) >= 0 ||
                   (Type == "Инвентаризация" && WarehouseList[indexWarehouse].products[indexProduct].count + Count >= 0))
                {

                    int Id = CreateIdOperation();
                    bool isWriteInDataGridView = true;

                    if (Type == "Перемещение")
                    {
                        //вычитаем кол-во продуктов из старого склада 
                        Count = -Math.Abs(Count);
                        WarehouseList[indexWarehouse].AddOperations(OperationDataGridView, Date, Type, Warehouse,
                        NewWarehouse, NameProduct, Count, UnitPrice, Comment, Id, isWriteInDataGridView);

                        //прибавляем в новом складе
                        isWriteInDataGridView = false;
                        Count = -Count;
                        indexWarehouse = SearchIndexWarehouse(NewWarehouse);
                        WarehouseList[indexWarehouse].AddOperations(OperationDataGridView, Date, Type, Warehouse,
                        NewWarehouse, NameProduct, Count, UnitPrice, Comment, Id, isWriteInDataGridView);
                    }
                    else
                    {
                        WarehouseList[indexWarehouse].AddOperations(OperationDataGridView, Date, Type, Warehouse,
                            NewWarehouse, NameProduct, Count, UnitPrice, Comment, Id, isWriteInDataGridView);
                    }
                }
            }
        }

        public void TryChangeOperation(DataGridView OperationDataGridView,
            DateTimePicker DateTimePicker, ComboBox TypeComboBox, ComboBox WarehouseComboBox,
            ComboBox NewWarehouseComboBox, ListBox NameProductListBox,
            NumericUpDown CountNumericUpDown, NumericUpDown UnitPriceNumericUpDown,
            TextBox CommentTextBox)
        {
            //проверка на наличие необходимых данных
            if (TypeComboBox.SelectedIndex > -1 && WarehouseComboBox.SelectedIndex > -1 &&
                CountNumericUpDown.Value != 0 && NameProductListBox.SelectedIndex > -1)
            {
                string Date = DateTimePicker.Text;
                string Type = TypeComboBox.Text;
                string Warehouse = WarehouseComboBox.Text;
                string NewWarehouse = NewWarehouseComboBox.Text;
                string NameProduct = NameProductListBox.Text;
                string Comment = CommentTextBox.Text;
                uint UnitPrice = uint.Parse(UnitPriceNumericUpDown.Value.ToString());
                int Count = int.Parse(CountNumericUpDown.Value.ToString());
                int indexWarehouse = SearchIndexWarehouse(Warehouse);
                int indexProduct = WarehouseList[indexWarehouse].SearchIndexProduct(NameProduct);

                if (Type == "Закупка" && Count < 0) return;

                if (!int.TryParse(OperationDataGridView.CurrentRow.Cells[0].Value.ToString(), out int Id))
                    Id = CreateIdOperation();
                int CountProductInOldOperation = WarehouseList[indexWarehouse].SearchCountProductInOldOperation(Id);
                //проверка на то, что операция не отнимет товаров больше чем в самом складе 
                if ((Type == "Закупка" && Count > 0) ||
                    WarehouseList[indexWarehouse].products[indexProduct].count - CountProductInOldOperation - Math.Abs(Count) >= 0 ||
                   (Type == "Инвентаризация" && WarehouseList[indexWarehouse].products[indexProduct].count - CountProductInOldOperation + Count >= 0))
                {


                    //удаляем старые операции
                    foreach (var warehouse in WarehouseList)
                    {
                        warehouse.DeleteOperationInList(Id, NameProduct);
                    }

                    if (Type == "Перемещение")
                    {
                        //вычитаем кол-во продуктов из старого склада 
                        Count = -Math.Abs(Count);
                        WarehouseList[indexWarehouse].ChangeOperations(OperationDataGridView, Date, Type, Warehouse,
                        NewWarehouse, NameProduct, Count, UnitPrice, Comment, Id);

                        //прибавляем в новом складе
                        Count = -Count;
                        indexWarehouse = SearchIndexWarehouse(NewWarehouse);
                        WarehouseList[indexWarehouse].ChangeOperations(OperationDataGridView, Date, Type, Warehouse,
                        NewWarehouse, NameProduct, Count, UnitPrice, Comment, Id);
                    }
                    else
                    {
                        WarehouseList[indexWarehouse].ChangeOperations(OperationDataGridView, Date, Type, Warehouse,
                            NewWarehouse, NameProduct, Count, UnitPrice, Comment, Id);
                    }
                }
            }
        }

        private int CreateIdOperation()
        {
            int Id = 0;
            bool isCopy = false;
            while (true)
            {
                foreach (var warehouse in WarehouseList)
                {
                    isCopy = warehouse.CheckId(Id);
                    if (isCopy) break;
                }
                if (isCopy)
                {
                    isCopy = false;
                    Id++;
                }
                else return Id;
            }
        }

        private int SearchIndexWarehouse(string WarehouseName)
        {
            for (int i = 0; i < WarehouseList.Count; i++)
            {
                if (WarehouseList[i].name == WarehouseName) return i;
            }
            return -1;
        }

        internal void TryDeleteOperation(DataGridView operationDataGridView)
        {
            int Rowindex = operationDataGridView.CurrentCell.RowIndex;
            if (int.TryParse(operationDataGridView.Rows[Rowindex].Cells[0].Value.ToString(), out int Id)) {

                string NameProduct = operationDataGridView.Rows[Rowindex].Cells[5].Value.ToString();

                foreach (var warehouse in WarehouseList)
                {
                    warehouse.DeleteOperationInList(Id, NameProduct);
                }

                if (operationDataGridView.Rows.Count == 1)
                {
                    operationDataGridView.Rows[Rowindex].SetValues("", "", "", "", "", "", "", "", "", "");
                }
                else
                    operationDataGridView.Rows.RemoveAt(Rowindex);
            }
        }
    }
}
