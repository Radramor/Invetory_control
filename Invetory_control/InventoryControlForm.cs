using System;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace Invetory_control
{
    
    public partial class InventoryControlForm : Form
    {
        OperationManager OM = new OperationManager();
        ProductManager PM = new ProductManager();
        WarehouseManager WM = new WarehouseManager();
            
        int selectedRow;
        public InventoryControlForm()
        {
            InitializeComponent();
        }

        private void InventoryControlForm_Load(object sender, EventArgs e)
        {
            PM.load();
            WM.load();
            OM.load();

            CreateWarehouseDataGridView();
            CreateProductDataGridView();
            CreateOperationDataGridView();
            CreateReportDataGridView();
        }

        private void CreateOperationDataGridView()
        {
            int index = 0;
            ÑreateOperationComboBoxes();
            foreach(var row in OM.Rows)
            {               
                switch (row.type) 
                {
                    case OperationType.Buy:
                        OperationDataGridView.Rows.Add(row.date, row.type.GetDescription(), row.product.warehouse.name, row.oldWarehouse, row.product.name, row.product.unit, row.product.warehouse.countProduct, row.unitPrice, row.sum, row.commentary);
                        OperationDataGridView.Rows[index].Cells[3].Style.BackColor = Color.Black;

                        break;
                    case OperationType.Moving:
                        OperationDataGridView.Rows.Add(row.date, row.type.GetDescription(), row.oldWarehouse, row.product.warehouse.name, row.product.name, row.product.unit, row.product.warehouse.countProduct, "", "", row.commentary);
                        OperationDataGridView.Rows[index].Cells[7].Style.BackColor = Color.Black;
                        OperationDataGridView.Rows[index].Cells[8].Style.BackColor = Color.Black;
                        break;
                    case OperationType.WriteOff:
                        OperationDataGridView.Rows.Add(row.date, row.type.GetDescription(), row.product.warehouse.name, row.oldWarehouse, row.product.name, row.product.unit, row.product.warehouse.countProduct, "", "", row.commentary);
                        OperationDataGridView.Rows[index].Cells[3].Style.BackColor = Color.Black;
                        OperationDataGridView.Rows[index].Cells[7].Style.BackColor = Color.Black;
                        OperationDataGridView.Rows[index].Cells[8].Style.BackColor = Color.Black;
                        break;
                    case OperationType.Inventory:
                        OperationDataGridView.Rows.Add(row.date, row.type.GetDescription(), row.product.warehouse.name, row.oldWarehouse, row.product.name, row.product.unit, row.product.warehouse.countProduct, "", "", row.commentary);
                        OperationDataGridView.Rows[index].Cells[3].Style.BackColor = Color.Black;
                        OperationDataGridView.Rows[index].Cells[7].Style.BackColor = Color.Black;
                        OperationDataGridView.Rows[index].Cells[8].Style.BackColor = Color.Black;
                        break;
                }
                index++;
            }
        }
        private void ÑreateOperationComboBoxes()
        {
            FirstWarehouseComboBox.Items.Clear();
            SecondWarehouseComboBox.Items.Clear();
            NameListBox.Items.Clear();
            foreach (var row in WM.Rows)
            {
                FirstWarehouseComboBox.Items.Add(row.name);
                SecondWarehouseComboBox.Items.Add(row.name);
            }   
            foreach (var row in PM.Rows)
            {
                NameListBox.Items.Add(row.name);
            }
        }
        private void CreateWarehouseDataGridView()
        {
            foreach (var row in WM.Rows)
            {
                WarehouseDataGridView.Rows.Add(row.name);
            }
        }
        private void CreateProductDataGridView()
        {
            foreach (var row in PM.Rows)
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

            ReportDataGridView.Columns.Add("NameColumn","Íàçâàíèå");
            ReportDataGridView.Columns.Add("UnitColumn", "Åäèíèöà èçìåðåíèÿ");
            ReportDataGridView.Columns.Add("CountColumn", "Âñåãî");
            ReportDataGridView.Columns[ReportDataGridView.ColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            foreach (var (i, row) in WM.Rows.Ordinate())
            {
                ReportDataGridView.Columns.Add("Column" + i, row.name);
                ReportDataGridView.Columns[i+3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                if( i % 2 == 0 )
                    ReportDataGridView.Columns[i+3].DefaultCellStyle.BackColor = Color.Silver;
            }

            foreach (var (IndexProduct, rowProduct) in PM.Rows.Ordinate())
            {
                ReportDataGridView.Rows.Add(rowProduct.name, rowProduct.unit);

                foreach (var (IndexOperation, rowOperation) in OM.Rows.Ordinate())
                {
                    if (ReportDataGridView.Rows[IndexProduct].Cells[0].Value.ToString() == rowOperation.product.name)
                    {
                        for (int IndexColumn = 3; IndexColumn < ReportDataGridView.Columns.Count; IndexColumn++)
                        {
                            int a = 0;
                            int b = OM.GetByIndex(IndexOperation).product.warehouse.countProduct;
                            SubtractProduct(a, b, IndexProduct, IndexColumn, IndexOperation);
                            AddProduct(a, b, IndexProduct, IndexColumn, IndexOperation);                           
                        }
                    }
                }
                TotalProduct(IndexProduct);
            }
        }

        private void TotalProduct(int rowIndex)
        {
            int sum = 0;
            int selectedCell;
            for (int c = 3; c < ReportDataGridView.Columns.Count; c++)
            {
                if (ReportDataGridView.Rows[rowIndex].Cells[c].Value == null)
                    selectedCell = 0;
                else
                    selectedCell = int.Parse(ReportDataGridView.Rows[rowIndex].Cells[c].Value.ToString());
                sum += selectedCell;
            }
            ReportDataGridView.Rows[rowIndex].Cells[2].Value = sum;
        }

        private void SubtractProduct(int a, int b, int IndexRow, int IndexColumn, int IndexOperation)
        {
            if (ReportDataGridView.Columns[IndexColumn].HeaderText == OM.GetByIndex(IndexOperation).oldWarehouse && OM.GetByIndex(IndexOperation).type == OperationType.Moving)
            {

                if (ReportDataGridView.Rows[IndexRow].Cells[IndexColumn].Value != null)
                    a = int.Parse(ReportDataGridView.Rows[IndexRow].Cells[IndexColumn].Value.ToString());

                ReportDataGridView.Rows[IndexRow].Cells[IndexColumn].Value = a - b;

                if (a - b == 0)
                    ReportDataGridView.Rows[IndexRow].Cells[IndexColumn].Value = null;
                if (a - b < 0)
                    ReportDataGridView.Rows[IndexRow].Cells[IndexColumn].Style.BackColor = Color.Red;
            }
        }
        private void AddProduct(int a, int b, int IndexRow,int IndexColumn, int IndexOperation)
        {
            if (ReportDataGridView.Columns[IndexColumn].HeaderText == OM.GetByIndex(IndexOperation).product.warehouse.name)
            {

                if (ReportDataGridView.Rows[IndexRow].Cells[IndexColumn].Value != null)
                    a = int.Parse(ReportDataGridView.Rows[IndexRow].Cells[IndexColumn].Value.ToString());

                ReportDataGridView.Rows[IndexRow].Cells[IndexColumn].Value = a + b;

                if (a + b == 0)
                    ReportDataGridView.Rows[IndexRow].Cells[IndexColumn].Value = null;
                if (a + b < 0)
                    ReportDataGridView.Rows[IndexRow].Cells[IndexColumn].Style.BackColor = Color.Red;
                else if (IndexColumn % 2 == 0)
                    ReportDataGridView.Rows[IndexRow].Cells[IndexColumn].Style.BackColor = Color.White;
                else
                    ReportDataGridView.Rows[IndexRow].Cells[IndexColumn].Style.BackColor = Color.Silver;
            }
        }


        private void ProductDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (selectedRow >= 0)
            { 
                DataGridViewRow row = ProductDataGridView.Rows[selectedRow];

                IdProductTextBox.Text   = row.Cells[0].Value.ToString();
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

                dateTimePicker.Text          = row.Cells[0].Value.ToString();
                TypeComboBox.Text            = row.Cells[1].Value.ToString();
                FirstWarehouseComboBox.Text  = row.Cells[2].Value.ToString();
                SecondWarehouseComboBox.Text = row.Cells[3].Value.ToString();
                NameListBox.Text             = row.Cells[4].Value.ToString();
                QuantityTextBox.Text         = row.Cells[6].Value.ToString();
                UnitPriceTextBox.Text        = row.Cells[7].Value.ToString();
                CommentTextBox.Text          = row.Cells[9].FormattedValue.ToString();

            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            PM.save();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            PM.DeleteRow(ProductDataGridView);
            ÑreateOperationComboBoxes();
            CreateReportDataGridView();
        }

        private void NewProductButton_Click(object sender, EventArgs e)
        {
            PM.CreateRow(ProductDataGridView);
        }

        private void ChangeButton_Click(object sender, EventArgs e)
        {
            PM.changeProduct(ProductDataGridView, IdProductTextBox, NameProductTextBox, UnitProductTextBox);
            ÑreateOperationComboBoxes();
            CreateReportDataGridView();
        }

        private void NewWarehouseButton_Click(object sender, EventArgs e)
        {
            WM.CreateRow(WarehouseDataGridView);
        }

        private void ChangeWarehouseButton_Click(object sender, EventArgs e)
        {
            WM.changeWarehouse(WarehouseDataGridView, NameWarehouseTextBox);
            ÑreateOperationComboBoxes();
            CreateReportDataGridView();
        }

        private void DeleteWarehouseButton_Click(object sender, EventArgs e)
        {
            WM.DeleteRow(WarehouseDataGridView);
            ÑreateOperationComboBoxes();
            CreateReportDataGridView();
        }
        private void SaveWarehouseButton_Click(object sender, EventArgs e)
        {
            WM.save();
        }

        private void NewOperationButton_Click(object sender, EventArgs e)
        {
            OM.CreateRow(OperationDataGridView);
        }

        private void ChangeOperationButton_Click(object sender, EventArgs e)
        {
            OM.ChangeRow(OperationDataGridView, 
               dateTimePicker, FirstWarehouseComboBox, NameListBox, 
               QuantityTextBox, UnitPriceTextBox, CommentTextBox,
               WM.Rows, PM.Rows, TypeComboBox, SecondWarehouseComboBox);
            CreateReportDataGridView();
        }

        private void DeleteOperationButton_Click(object sender, EventArgs e)
        {
            OM.DeleteRow(OperationDataGridView);
            CreateReportDataGridView();
        }

        private void SaveOperationButton_Click(object sender, EventArgs e)
        {
            OM.save();
        }

        private void TypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (TypeComboBox.SelectedItem.ToString())
            {
                case "Çàêóïêà":
                    SecondWarehouseComboBox.Visible = false;
                    SecondWarehouseLabel.Visible = false;
                    UnitPriceLabel.Visible = true;
                    UnitPriceTextBox.Visible = true;
                    break;
                case "Èíâåíòàðèçàöèÿ":
                    SecondWarehouseComboBox.Visible = false;
                    SecondWarehouseLabel.Visible = false;
                    UnitPriceLabel.Visible = false;
                    UnitPriceTextBox.Visible = false;
                    break;
                case "Ïåðåìåùåíèå":
                    SecondWarehouseComboBox.Visible = true;
                    SecondWarehouseLabel.Visible = true;
                    UnitPriceTextBox.Visible = false;
                    UnitPriceLabel.Visible = false;
                    break;
                case "Ñïèñàíèå":
                    SecondWarehouseComboBox.Visible = false;
                    SecondWarehouseLabel.Visible = false;
                    UnitPriceLabel.Visible = false;
                    UnitPriceTextBox.Visible = false;
                    break;
            }
        }   
    }
    
}