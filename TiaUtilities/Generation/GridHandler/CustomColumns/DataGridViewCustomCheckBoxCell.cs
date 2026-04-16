using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Generation.GridHandler.CustomColumns
{
    public class DataGridViewCustomCheckBoxCell : DataGridViewCheckBoxCell
    {

        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            base.OnMouseClick(e);
            /*
            if(this.Value is bool bValue)
            {
                this.Value = !bValue;
            }*/
        }

        protected override void OnContentClick(DataGridViewCellEventArgs e)
        {
            base.OnContentClick(e);
        }
    }
}
