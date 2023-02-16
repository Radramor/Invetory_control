using Accessibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invetory_control
{
    abstract class RowInTable<T>
    {
        protected List<T> _rows = new List<T>();
        public IEnumerable<T> Rows { get { return _rows; } }
        public abstract void CreateRow(DataGridView dataGridView);
        public abstract void save();
        public abstract void load();
        //public abstract void DeleteFromList(int index);

        public T GetByIndex(int index)
        {
            return _rows[index];
        }
        public void DeleteRow(DataGridView dataGridView) 
        {
            int index = dataGridView.CurrentCell.RowIndex;
            _rows.RemoveAt(index);

            if (dataGridView.Rows.Count == 1)
            {
                dataGridView.Rows[index].SetValues("","","","","","","","","","");           
            }
            else
                dataGridView.Rows.RemoveAt(index);
        }        
    }
}
