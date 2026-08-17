using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Generation.GridHandler.CustomColumns.ButtonColumn
{
    public class DataGridViewEventableButtonCell : DataGridViewButtonCell
    {

        protected override void OnContentClick(DataGridViewCellEventArgs e)
        {
            base.OnContentClick(e); 
            
            if (this.OwningColumn is DataGridViewEventableButtonColumn eventableButtonColumn)
            {
                eventableButtonColumn.ButtonPressedEvent(this);
            }
        }

        protected override void OnContentDoubleClick(DataGridViewCellEventArgs e)
        {
            base.OnContentDoubleClick(e); 
            
            if (this.OwningColumn is DataGridViewEventableButtonColumn eventableButtonColumn)
            {
                eventableButtonColumn.ButtonDoublePressedEvent(this);
            }
        }
    }
}
