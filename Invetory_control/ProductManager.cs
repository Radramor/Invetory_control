using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Invetory_control
{
    internal class ProductManager : RowInTable<Product>
    {
        public override void save()
        {
            File.WriteAllText("saveProduct.json", JsonConvert.SerializeObject(_rows));
        }

        public override void load()
        {
            _rows = JsonConvert.DeserializeObject<List<Product>>(File.ReadAllText("saveProduct.json")); 
        }

        public override void CreateRow(DataGridView productDataGridView)
        {
            _rows.Add(new Product());
            productDataGridView.Rows.Add(_rows.Last().id, _rows.Last().name, _rows.Last().unit);
        }   

        internal void changeProduct (DataGridView productDataGridView, TextBox textBoxID, TextBox textBoxName, TextBox textBoxUnit)
        {
            int index = productDataGridView.CurrentCell.RowIndex;
            bool copy = false;
            if (uint.TryParse(textBoxID.Text, out _rows[index].id))
            {
                for (int i = 0; i < _rows.Count; i++)
                {
                    if (i == index) continue;
                    if (_rows[i].name == textBoxName.Text || _rows[i].id == int.Parse(textBoxID.Text))
                        copy = true;
                }
                if (copy == false)
                {
                    _rows[index].name = textBoxName.Text;
                    _rows[index].unit = textBoxUnit.Text;
                    productDataGridView.Rows[index].SetValues(_rows[index].id, _rows[index].name, _rows[index].unit);
                }
            }
        }

        /*public override void DeleteFromList(int index)
        {
            _rows.RemoveAt(index);
        }*/
    }
}
