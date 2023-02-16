using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invetory_control
{
    internal class WarehouseManager : RowInTable<Warehouse>
    {
        //public List<Warehouse> Rows = new List<Warehouse>();
        public override void save()
        {
            File.WriteAllText("saveWarehouse.json", JsonConvert.SerializeObject(_rows));
        }
        public override void load()
        {
            _rows = File.Exists("saveWarehouse.json") ? JsonConvert.DeserializeObject<List<Warehouse>>(File.ReadAllText("saveWarehouse.json")) : new List<Warehouse>();       
        }
        public override void CreateRow(DataGridView WarehouseDataGridView)
        {
            _rows.Add(new Warehouse());
            WarehouseDataGridView.Rows.Add(_rows[_rows.Count - 1].name);
        }

        internal void changeWarehouse(DataGridView productDataGridView, TextBox textBoxName)
        {
            int index = productDataGridView.CurrentCell.RowIndex;
            bool copy = false;

            for (int i = 0; i < _rows.Count; i++)
            {
                if (i == index) continue;
                if (_rows[i].name == textBoxName.Text)
                    copy = true;
            }
            if (copy == false)
            {
                _rows[index].name = textBoxName.Text;
                productDataGridView.Rows[index].SetValues(_rows[index].name);
            }

        }

        /*public override void DeleteFromList(int index)
        {
            _rows.RemoveAt(index);
        }*/
    }
}
