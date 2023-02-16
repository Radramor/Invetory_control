using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invetory_control 
{
    internal class OperationManager : RowInTable<Operation>
    {
        
        public override void save()
        {
            File.WriteAllText("saveOperation.json", JsonConvert.SerializeObject(_rows));
        }

        public override void load()
        {
            _rows = JsonConvert.DeserializeObject<List<Operation>>(File.ReadAllText("saveOperation.json"));  
        }
   
        public void ChangeRow(DataGridView operationDataGridView, 
              DateTimePicker dateTimePicker, ComboBox warehouseComboBox,
              ListBox nameListBox, TextBox countTextBox, TextBox unitPriceTextbox, 
              TextBox commentTextBox, IEnumerable<Warehouse> warehouses, IEnumerable<Product> products,
              ComboBox typeComboBox, ComboBox secondWarehouseComboBox)
        {
            if (warehouseComboBox.SelectedIndex > -1 && nameListBox.SelectedIndex > -1 && typeComboBox.SelectedIndex > -1 && countTextBox.Text != "0")
            {
                
                int index = operationDataGridView.CurrentCell.RowIndex;
                Operation Op = _rows[index];

                Op.date = dateTimePicker.Text;
                Op.commentary = commentTextBox.Text;
                Op.product.name = nameListBox.SelectedItem.ToString();
                foreach (var rowProduct in products)
                {
                    if (Op.product.name == rowProduct.name)
                    {
                        Op.product.unit = rowProduct.unit;
                    }
                }

                Op.product.warehouse.name = warehouseComboBox.SelectedItem.ToString();

                if (int.TryParse(countTextBox.Text, out Op.product.warehouse.countProduct))
                {
                    switch (typeComboBox.SelectedItem.ToString())
                    {
                        case "Закупка":

                            if (uint.TryParse(unitPriceTextbox.Text, out Op.unitPrice) && int.Parse(countTextBox.Text) > 0)
                            {
                                operationDataGridView.Rows[index].Cells[3].Style.BackColor = Color.Black;
                                operationDataGridView.Rows[index].Cells[7].Style.BackColor = Color.White;
                                operationDataGridView.Rows[index].Cells[8].Style.BackColor = Color.Silver;

                                Op.type = OperationType.Buy;
                                Op.oldWarehouse = "";
                                Op.sum = (uint)(Op.unitPrice * Op.product.warehouse.countProduct);

                                operationDataGridView.Rows[index].SetValues(Op.date, Op.type.GetDescription(), Op.product.warehouse.name, "", Op.product.name, Op.product.unit, Op.product.warehouse.countProduct, Op.unitPrice, Op.sum, Op.commentary);
                            }
                            break;

                        case "Перемещение":
                            if (secondWarehouseComboBox.SelectedIndex > -1 && int.Parse(countTextBox.Text) > 0 && warehouseComboBox.Text != secondWarehouseComboBox.Text)
                            {
                                operationDataGridView.Rows[index].Cells[3].Style.BackColor = Color.White;
                                operationDataGridView.Rows[index].Cells[7].Style.BackColor = Color.Black;
                                operationDataGridView.Rows[index].Cells[8].Style.BackColor = Color.Black;

                                Op.type = OperationType.Moving;
                                Op.unitPrice = 0;
                                Op.sum = 0;
                                Op.product.warehouse.name = secondWarehouseComboBox.SelectedItem.ToString();
                                Op.oldWarehouse = warehouseComboBox.SelectedItem.ToString();

                                
                                operationDataGridView.Rows[index].SetValues(Op.date, Op.type.GetDescription(), Op.oldWarehouse, Op.product.warehouse.name, Op.product.name, Op.product.unit, Op.product.warehouse.countProduct, "", "", Op.commentary);
                            }
                            break;

                        case "Списание":
                            operationDataGridView.Rows[index].Cells[3].Style.BackColor = Color.Black;
                            operationDataGridView.Rows[index].Cells[7].Style.BackColor = Color.Black;
                            operationDataGridView.Rows[index].Cells[8].Style.BackColor = Color.Black;

                            Op.type = OperationType.WriteOff;
                            Op.product.warehouse.countProduct = -Math.Abs(Op.product.warehouse.countProduct);
                            Op.oldWarehouse = "";
                            Op.unitPrice = 0;
                            Op.sum = 0;

                            operationDataGridView.Rows[index].SetValues(Op.date, Op.type.GetDescription(), Op.product.warehouse.name, Op.oldWarehouse, Op.product.name, Op.product.unit, Op.product.warehouse.countProduct, Op.unitPrice, Op.sum, Op.commentary);
                            break;

                        case "Инвентаризация":

                            operationDataGridView.Rows[index].Cells[3].Style.BackColor = Color.Black;
                            operationDataGridView.Rows[index].Cells[7].Style.BackColor = Color.Black;
                            operationDataGridView.Rows[index].Cells[8].Style.BackColor = Color.Black;

                            Op.type = OperationType.Inventory;
                            Op.oldWarehouse = "";
                            Op.unitPrice = 0;
                            Op.sum = 0;

                            operationDataGridView.Rows[index].SetValues(Op.date, Op.type.GetDescription(), Op.product.warehouse.name, Op.oldWarehouse, Op.product.name, Op.product.unit, Op.product.warehouse.countProduct, Op.unitPrice, Op.sum, Op.commentary);
                            break;
                    } 
                }
            }
        }

        public override void CreateRow(DataGridView operationDataGridView)
        {
            _rows.Add(new Operation());
            operationDataGridView.Rows.Add
                (
                _rows.Last().date,
                _rows.Last().product.warehouse.name,
                _rows.Last().product.name,
                _rows.Last().product.unit,
                _rows.Last().product.warehouse.countProduct,
                _rows.Last().unitPrice,
                _rows.Last().sum,
                _rows.Last().commentary
                );
        }

        /*public override void DeleteFromList(int index)
        {
            _rows.RemoveAt(index);
        }*/
    }
}
