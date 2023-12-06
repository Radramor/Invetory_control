using System;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace Invetory_control
{
    
    public partial class InventoryControlForm : Form
    {
        WarehouseAndProductManager WM = new WarehouseAndProductManager();
            
        int selectedRow;
        public InventoryControlForm()
        {
            InitializeComponent();
        }

        private void InventoryControlForm_Load(object sender, EventArgs e)
        {
            WM.load();

            CreateWarehouseDataGridView();
            CreateProductDataGridView();
            CreateOperationDataGridView();
            CreateReportDataGridView();
        }

        private void CreateOperationDataGridView()
        {
            int LastRow = 0;
            СreateOperationComboBoxes();
            foreach(var warehouse in WM.WarehouseList)
            {
                foreach (var opBuy in warehouse.opBuys)
                {
                    OperationDataGridView.Rows.Add(opBuy.id, opBuy.date, "Закупка", warehouse.name, "", opBuy.product.name, opBuy.product.unit, opBuy.count, opBuy.unitPrice, opBuy.sum, opBuy.commentary);
                    OperationDataGridView.Rows[LastRow].Cells[4].Style.BackColor = Color.Black;
                    LastRow++;
                }

                foreach (var opMoving in warehouse.opMovings)
                {
                    if (opMoving.count > 0) continue; //не выписывает добавление продуктов в новый склад (вторая операция перемещения)
                    OperationDataGridView.Rows.Add(opMoving.id, opMoving.date, "Перемещение", warehouse.name, opMoving.newWarehouse, opMoving.product.name, opMoving.product.unit, opMoving.count, "", "", opMoving.commentary);
                    OperationDataGridView.Rows[LastRow].Cells[8].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[LastRow].Cells[9].Style.BackColor = Color.Black;
                    LastRow++;
                }

                foreach (var opWriteOff in warehouse.opWriteOffs)
                {
                    OperationDataGridView.Rows.Add(opWriteOff.id, opWriteOff.date, "Списание", warehouse.name, "", opWriteOff.product.name, opWriteOff.product.unit, opWriteOff.count, "", "", opWriteOff.commentary);
                    OperationDataGridView.Rows[LastRow].Cells[4].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[LastRow].Cells[8].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[LastRow].Cells[9].Style.BackColor = Color.Black;
                    LastRow++;
                }

                foreach (var opInventory in warehouse.opInventorys)
                {
                    OperationDataGridView.Rows.Add(opInventory.id, opInventory.date, "Инвентаризация", warehouse.name, "", opInventory.product.name, opInventory.product.unit, opInventory.count, "", "", opInventory.commentary);
                    OperationDataGridView.Rows[LastRow].Cells[4].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[LastRow].Cells[8].Style.BackColor = Color.Black;
                    OperationDataGridView.Rows[LastRow].Cells[9].Style.BackColor = Color.Black;
                    LastRow++;
                }
            }
        }
        private void СreateOperationComboBoxes()
        {
            FirstWarehouseComboBox.Items.Clear();
            SecondWarehouseComboBox.Items.Clear();
            NameProductListBox.Items.Clear();
            foreach (var row in WM.WarehouseList)
            {
                FirstWarehouseComboBox.Items.Add(row.name);
                SecondWarehouseComboBox.Items.Add(row.name);
            }   
            foreach (var row in WM.TotalProductList)
            {
                NameProductListBox.Items.Add(row.name);
            }
        }
        private void CreateWarehouseDataGridView()
        {
            foreach (var row in WM.WarehouseList)
            {
                WarehouseDataGridView.Rows.Add(row.name);
            }
        }
        private void CreateProductDataGridView()
        {
            foreach (var row in WM.TotalProductList)
            {
                ProductDataGridView.Rows.Add(row.id, row.name, row.unit);
            }
        }

        private void CreateReportDataGridView()
        {
            ReportDataGridView.AllowUserToAddRows = false;
            ReportDataGridView.AllowUserToDeleteRows = false;
            ReportDataGridView.ReadOnly = true;

            ReportDataGridView.Columns.Clear();

            ReportDataGridView.Columns.Add("NameColumn","Название");
            ReportDataGridView.Columns.Add("UnitColumn", "Единица измерения");
            ReportDataGridView.Columns.Add("CountColumn", "Всего");
            ReportDataGridView.Columns[ReportDataGridView.ColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            //создание столбцов из складов
            for(int i = 0; i < WM.WarehouseList.Count; i++)
            {
                ReportDataGridView.Columns.Add("Column" + i, WM.WarehouseList[i].name);
                ReportDataGridView.Columns[i+3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                if( i % 2 == 0 )
                    ReportDataGridView.Columns[i+3].DefaultCellStyle.BackColor = Color.Silver;
            }

            for (int IndexProduct = 0; IndexProduct < WM.TotalProductList.Count; IndexProduct++)
            {              
                ReportDataGridView.Rows.Add(WM.TotalProductList[IndexProduct].name, WM.TotalProductList[IndexProduct].unit);
                for (int indexWarehouse = 0; indexWarehouse < WM.WarehouseList.Count; indexWarehouse++)
                {
                    ReportDataGridView.Rows[IndexProduct].Cells[indexWarehouse + 3].Value = WM.WarehouseList[indexWarehouse].products[IndexProduct].count;
                }
                TotalProductInRow(IndexProduct);
            }
        }

        private void TotalProductInRow(int rowIndex)
        {
            int sum = 0;
            for (int c = 3; c < ReportDataGridView.Columns.Count; c++)
            {
                if (ReportDataGridView.Rows[rowIndex].Cells[c].Value != null)
                    sum += int.Parse(ReportDataGridView.Rows[rowIndex].Cells[c].Value.ToString());
            }
            ReportDataGridView.Rows[rowIndex].Cells[2].Value = sum;
        }
        private void ProductDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (selectedRow >= 0)
            { 
                DataGridViewRow row = ProductDataGridView.Rows[selectedRow];

                IdProductNumericUpDown.Text   = row.Cells[0].Value.ToString();
                NameProductTextBox.Text = row.Cells[1].Value.ToString();
                UnitProductTextBox.Text = row.Cells[2].Value.ToString();

            }
        }

        private void WarehouseDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (selectedRow >= 0)
            {
                DataGridViewRow row = WarehouseDataGridView.Rows[selectedRow];

                NameWarehouseTextBox.Text = row.Cells[0].Value.ToString();

            }
        }

        private void OperationDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;            
            if (selectedRow >= 0)
            {
                DataGridViewRow row = OperationDataGridView.Rows[selectedRow];

                dateTimePicker.Text          = row.Cells[1].Value.ToString();
                TypeComboBox.Text            = row.Cells[2].Value.ToString();
                FirstWarehouseComboBox.Text  = row.Cells[3].Value.ToString();
                SecondWarehouseComboBox.Text = row.Cells[4].Value.ToString();
                NameProductListBox.Text      = row.Cells[5].Value.ToString();
                CountNumericUpDown.Text      = row.Cells[7].Value.ToString();
                UnitPriceNumericUpDown.Text  = row.Cells[8].Value.ToString();
                CommentTextBox.Text          = row.Cells[10].FormattedValue.ToString();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            WM.save();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                WM.DeleteRowProduct(ProductDataGridView);
                СreateOperationComboBoxes();
                CreateReportDataGridView();
                ErrorLabel.Text = "";
            }
            catch (Exception ex)
            {
                ErrorLabel.Text = ex.Message;
            }
        }

        private void NewProductButton_Click(object sender, EventArgs e)
        {
            WM.CreateRowProduct(ProductDataGridView, IdProductNumericUpDown, NameProductTextBox, UnitProductTextBox, WarehouseDataGridView);
            СreateOperationComboBoxes();
            CreateReportDataGridView();
        }

        private void ChangeButton_Click(object sender, EventArgs e)
        {
            try
            {
                WM.ChangeRowProduct(ProductDataGridView, OperationDataGridView, IdProductNumericUpDown, NameProductTextBox, UnitProductTextBox);
                СreateOperationComboBoxes();
                CreateReportDataGridView();
                ErrorLabel.Text = "";
            }
            catch (Exception ex) 
            { 
                ErrorLabel.Text = ex.Message;
            }
        }

        private void NewWarehouseButton_Click(object sender, EventArgs e)
        {
            WM.CreateRowWarehouse(WarehouseDataGridView, NameWarehouseTextBox);
            СreateOperationComboBoxes();
            CreateReportDataGridView();
        }

        private void ChangeWarehouseButton_Click(object sender, EventArgs e)
        {
            try
            {
                WM.ChangeRowWarehouse(WarehouseDataGridView, NameWarehouseTextBox);
                СreateOperationComboBoxes();
                CreateReportDataGridView();
                ErrorLabel.Text = "";
            }
            catch (Exception ex)
            {
                ErrorLabel.Text = ex.Message;
            }
        }

        private void DeleteWarehouseButton_Click(object sender, EventArgs e)
        {
            try
            {
                WM.DeleteRowWarehouse(WarehouseDataGridView);
                СreateOperationComboBoxes();
                CreateReportDataGridView();
                ErrorLabel.Text = "";
            }
            catch (Exception ex)
            {
                ErrorLabel.Text = ex.Message;
            }
        }


        private void NewOperationButton_Click(object sender, EventArgs e)
        {
            try
            {
                WM.TryAddOperation(OperationDataGridView, dateTimePicker, TypeComboBox, FirstWarehouseComboBox,
                    SecondWarehouseComboBox, NameProductListBox, CountNumericUpDown, UnitPriceNumericUpDown,
                    CommentTextBox);
                CreateReportDataGridView();
                ErrorOperationLabel.Text = "";
            }
            catch (Exception ex)
            {
                ErrorOperationLabel.Text = ex.Message;
            }
        }

        private void ChangeOperationButton_Click(object sender, EventArgs e)
        {
            try
            {
                WM.TryChangeOperation(OperationDataGridView, dateTimePicker, TypeComboBox, FirstWarehouseComboBox,
                    SecondWarehouseComboBox, NameProductListBox, CountNumericUpDown, UnitPriceNumericUpDown,
                    CommentTextBox);
                CreateReportDataGridView();
                ErrorOperationLabel.Text = "";
            }
            catch (Exception ex)
            {
                ErrorOperationLabel.Text = ex.Message;
            }
        }

        private void DeleteOperationButton_Click(object sender, EventArgs e)
        {
            try
            {
                WM.TryDeleteOperation(OperationDataGridView);
                CreateReportDataGridView();
                ErrorOperationLabel.Text = "";
            }
            catch (Exception ex)
            {
                ErrorOperationLabel.Text = ex.Message;
            }
        }

        private void TypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (TypeComboBox.SelectedItem.ToString())
            {
                case "Закупка":
                    SecondWarehouseComboBox.Visible = false;
                    SecondWarehouseLabel.Visible = false;
                    UnitPriceLabel.Visible = true;
                    UnitPriceNumericUpDown.Visible = true;
                    break;
                case "Инвентаризация":
                    SecondWarehouseComboBox.Visible = false;
                    SecondWarehouseLabel.Visible = false;
                    UnitPriceLabel.Visible = false;
                    UnitPriceNumericUpDown.Visible = false;
                    break;
                case "Перемещение":
                    SecondWarehouseComboBox.Visible = true;
                    SecondWarehouseLabel.Visible = true;
                    UnitPriceNumericUpDown.Visible = false;
                    UnitPriceLabel.Visible = false;
                    break;
                case "Списание":
                    SecondWarehouseComboBox.Visible = false;
                    SecondWarehouseLabel.Visible = false;
                    UnitPriceLabel.Visible = false;
                    UnitPriceNumericUpDown.Visible = false;
                    break;
            }
        }

        private void FirstWarehouseComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecondWarehouseComboBox.Items.Clear();
            foreach (var row in WM.WarehouseList)
            {
                if (FirstWarehouseComboBox.SelectedItem.ToString() == row.name) continue;
                SecondWarehouseComboBox.Items.Add(row.name);
            }
        }
    } 
}