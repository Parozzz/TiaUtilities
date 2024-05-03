using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiaXmlReader.Generation.GridHandler.CustomColumns
{
    public class DropDownMenuScrollWheelHandler : IMessageFilter
    {
        private static DropDownMenuScrollWheelHandler Instance;
        public static void Enable(bool enabled)
        {
            if (enabled)
            {
                if (Instance == null)
                {
                    Instance = new DropDownMenuScrollWheelHandler();
                    Application.AddMessageFilter(Instance);
                }
            }
            else
            {
                if (Instance != null)
                {
                    Application.RemoveMessageFilter(Instance);
                    Instance = null;
                }
            }
        }
        private IntPtr activeHwnd;
        private ToolStripDropDown activeMenu;

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x200 && activeHwnd != m.HWnd) // WM_MOUSEMOVE
            {
                activeHwnd = m.HWnd;
                this.activeMenu = Control.FromHandle(m.HWnd) as ToolStripDropDown;
            }
            else if (m.Msg == 0x20A && this.activeMenu != null) // WM_MOUSEWHEEL
            {
                int delta = (short)(ushort)(((uint)(ulong)m.WParam) >> 16);
                HandleDelta(this.activeMenu, delta);
                return true;
            }
            return false;
        }

        public static readonly Action<ToolStrip, int> ScrollInternal
            = (Action<ToolStrip, int>) Delegate.CreateDelegate(typeof(Action<ToolStrip, int>), typeof(ToolStrip).GetMethod("ScrollInternal", BindingFlags.NonPublic | BindingFlags.Instance));

        public static void HandleDelta(ToolStripDropDown dropDown, int delta)
        {
            if (dropDown.Items.Count == 0)
            {
                return;
            }
                
            var firstItem = dropDown.Items[0];
            var lastItem = dropDown.Items[dropDown.Items.Count - 1];
            if (lastItem.Bounds.Bottom < dropDown.Height && firstItem.Bounds.Top > 0)
            {
                return;
            }
                
            delta /= -4;
            if (delta < 0 && firstItem.Bounds.Top - delta > 9)
            {
                delta = firstItem.Bounds.Top - 9;
            }
            else if (delta > 0 && delta > lastItem.Bounds.Bottom - dropDown.Height + 9)
            {
                delta = lastItem.Bounds.Bottom - dropDown.Height + 9;
            }

            if (delta != 0)
            {
                ScrollInternal(dropDown, delta);
            }
        }
    }
}
