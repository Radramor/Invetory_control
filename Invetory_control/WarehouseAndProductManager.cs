﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;

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
            TotalProductList = File.Exists("saveProduct.json") ? JsonConvert.DeserializeObject<List<Product>>(File.ReadAllText("saveProduct.json")) : new List<Product>();
            WarehouseList = File.Exists("saveWarehouse.json") ? JsonConvert.DeserializeObject<List<Warehouse>>(File.ReadAllText("saveWarehouse.json")) : new List<Warehouse>();
        }
        public void CreateRowWarehouse(DataGridView WarehouseDataGridView, TextBox textBox)
        {
            bool isCopy = false;
            for (int i = 0; i < WarehouseList.Count; i++)
            {
                if (WarehouseList[i].name == textBox.Text)
                    isCopy = true;
            }
            if (isCopy)
                WarehouseList.Add(new Warehouse("", TotalProductList));
            else
                WarehouseList.Add(new Warehouse(textBox.Text, TotalProductList));
            WarehouseDataGridView.Rows.Add(WarehouseList.Last().name);
        }

        internal void ChangeRowWarehouse(DataGridView DataGridView, TextBox textBox)
        {
            try
            {
                int index = DataGridView.CurrentCell.RowIndex;
                bool isCopy = false;

                for (int i = 0; i < WarehouseList.Count; i++)
                {
                    if (i == index) continue;
                    if (WarehouseList[i].name == textBox.Text)
                        isCopy = true;
                }

                if (isCopy) throw new ArgumentException("Данный склад уже существует");

                WarehouseList[index].SetName(textBox.Text);
                DataGridView.Rows[index].SetValues(WarehouseList[index].name);
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException("Невозможно изменить несуществующий склад");
            }

        }
        public void DeleteRowWarehouse(DataGridView dataGridView)
        {
            try
            {
                int index = dataGridView.CurrentCell.RowIndex;
                int indexDeletedWarehouseInList = SearchIndexWarehouse(dataGridView.Rows[index].Cells[0].Value.ToString());
                WarehouseList.RemoveAt(indexDeletedWarehouseInList);
                dataGridView.Rows.RemoveAt(index);
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException("Все склады уже удалены");
            }
        }
        public void CreateRowProduct(DataGridView ProductDataGridView, NumericUpDown IdNumericUpDown,
            TextBox NameTextBox, TextBox UnitTextBox, DataGridView WarehouseDataGridView)
        {
            //if (WarehouseDataGridView.RowCount == 0)
            //throw new ArgumentException("Сначала создайте склад");
            uint Id = (uint)IdNumericUpDown.Value;
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

            /*foreach (var warehouse in WarehouseList)
            {
                warehouse.products.Add(TotalProductList.Last());
            }*/
            ProductDataGridView.Rows.Add(TotalProductList.Last().id, TotalProductList.Last().name, TotalProductList.Last().unit);

        }
        public void ChangeRowProduct(DataGridView ProductDataGridView, DataGridView OperationDataGridView, NumericUpDown IdNumericUpDown, TextBox NameTextBox, TextBox UnitTextBox)
        {
            try
            {
                int IndexCurrentRow = ProductDataGridView.CurrentCell.RowIndex;
                bool isCopy = false;
                string NameInRow = "";
                uint IdInRow = 0;

                if (!uint.TryParse(IdNumericUpDown.Text, out uint Id))
                    throw new ArgumentException("неверный формат id продукта");

                for (int i = 0; i < TotalProductList.Count; i++)
                {
                    NameInRow = ProductDataGridView.Rows[i].Cells[1].Value.ToString();
                    IdInRow = uint.Parse(ProductDataGridView.Rows[i].Cells[0].Value.ToString());
                    if (i == IndexCurrentRow) continue;
                    if (NameInRow == NameTextBox.Text || IdInRow == Id) isCopy = true;
                }

                if (isCopy) throw new ArgumentException("Данный продукт или id продукта уже существует");

                string OldName = ProductDataGridView.Rows[IndexCurrentRow].Cells[1].Value.ToString();
                int indexChangedProductInList = SearchProductInTotalList(OldName);
                Product ChangedProduct = TotalProductList[indexChangedProductInList];
                ChangedProduct.SetValues(NameTextBox.Text, UnitTextBox.Text, Id);
                ProductDataGridView.Rows[IndexCurrentRow].SetValues(ChangedProduct.id, ChangedProduct.name, ChangedProduct.unit);

                foreach (var warehouse in WarehouseList)
                {
                    warehouse.ChangeProductInOperations(OldName, ChangedProduct.name, ChangedProduct.unit, ChangedProduct.id);
                    warehouse.products[indexChangedProductInList].SetValues(ChangedProduct.name, ChangedProduct.unit, ChangedProduct.id);
                }

                for (int i = 0; i < OperationDataGridView.Rows.Count; i++)
                {
                    if (OperationDataGridView.Rows[i].Cells[5].Value.ToString() == OldName)
                        OperationDataGridView.Rows[i].Cells[5].Value = ChangedProduct.name;
                }
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException("Невозможно изменить несуществующий продукт");
            }
        }

        public void DeleteRowProduct(DataGridView dataGridView)
        {
            try
            {
                int index = dataGridView.CurrentCell.RowIndex;
                int indexDeletedProductInList = SearchProductInTotalList(dataGridView.Rows[index].Cells[1].Value.ToString());
                TotalProductList.RemoveAt(indexDeletedProductInList);
                /*foreach (var warehouse in WarehouseList)
                {
                    warehouse.products.RemoveAt(indexDeletedProductInList);
                }*/
                dataGridView.Rows.RemoveAt(index);
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException("Все продукты уже удалены");
            }
        }
        
        private int SearchProductInTotalList(string Name)
        {
            for (int i = 0; i < TotalProductList.Count; i++)
            {
                if (TotalProductList[i].name == Name)
                    return i;
            }
            throw new ArgumentException("Не удалось найти продукт");
        }

        //Создает наименьший доступный id продукта
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
            if (TypeComboBox.SelectedIndex < 0) 
                throw new ArgumentException("Не выбран тип операции");  
            
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

            //проверка на то, что операция не отнимет товаров больше чем в самом складе 
            if ((Type == "Закупка" && Count > 0) ||
            WarehouseList[indexWarehouse].products[indexProduct].count - Math.Abs(Count) >= 0 ||
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
            else throw new ArgumentException("Добавление данной операции приведёт к отрицательному количеству продуктов");      
        }

        public void TryChangeOperation(DataGridView OperationDataGridView,
            DateTimePicker DateTimePicker, ComboBox TypeComboBox, ComboBox WarehouseComboBox,
            ComboBox NewWarehouseComboBox, ListBox NameProductListBox,
            NumericUpDown CountNumericUpDown, NumericUpDown UnitPriceNumericUpDown,
            TextBox CommentTextBox)
        {
            
            try
            {
                if (TypeComboBox.SelectedIndex < 0)
                    throw new ArgumentException("Не выбран тип операции");
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
                int Id = int.Parse(OperationDataGridView.CurrentRow.Cells[0].Value.ToString());

                if (Type == "Закупка" && Count < 0)
                    throw new ArgumentException("В данной операции может быть только положительное количество продуктов");

                //добавляем id в пустую строку
                //if (!int.TryParse(OperationDataGridView.CurrentRow.Cells[0].Value.ToString(), out int Id))
                    //Id = CreateIdOperation();

                int CountProductInOldOperation = WarehouseList[indexWarehouse].SearchCountProductInOperation(Id);
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
                else throw new ArgumentException("Изменение данной операции приведёт к отрицательному количеству продуктов");
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException("Невозможно изменить несуществующую операцию");
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
            throw new ArgumentException("Не удалось найти склад");
        }

        internal void TryDeleteOperation(DataGridView operationDataGridView)
        {
            try
            {
                int Rowindex = operationDataGridView.CurrentCell.RowIndex;
                int Id = int.Parse(operationDataGridView.Rows[Rowindex].Cells[0].Value.ToString());

                string NameProduct = operationDataGridView.Rows[Rowindex].Cells[5].Value.ToString();

                foreach (var warehouse in WarehouseList)
                {
                    warehouse.DeleteOperationInList(Id, NameProduct);
                }

                operationDataGridView.Rows.RemoveAt(Rowindex);
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException("Все операции уже удалены");
            }
            
        }
    }
}
