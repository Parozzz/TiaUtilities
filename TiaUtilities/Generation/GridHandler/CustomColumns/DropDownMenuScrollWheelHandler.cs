using System.Reflection;
using TiaUtilities.Utility;

namespace TiaXmlReader.Generation.GridHandler.CustomColumns
{
    public class DropDownMenuScrollWheelHandler : IMessageFilter
    {
        private static DropDownMenuScrollWheelHandler? Instance;
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
        private ToolStripDropDown? activeMenu;

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == DllImports.WM_MOUSEMOVE && activeHwnd != m.HWnd)
            {
                activeHwnd = m.HWnd;
                this.activeMenu = Control.FromHandle(m.HWnd) as ToolStripDropDown;
            }
            else if (m.Msg == DllImports.WM_MOUSEWHEEL && this.activeMenu != null)
            {
                int delta = (short)(ushort)(((uint)(ulong)m.WParam) >> 16);
                HandleDelta(this.activeMenu, delta);
                return true;
            }

            return false;
        }

        public static readonly Action<ToolStrip, int> ScrollInternal
            = (Action<ToolStrip, int>)Delegate.CreateDelegate(typeof(Action<ToolStrip, int>), typeof(ToolStrip).GetMethod("ScrollInternal", BindingFlags.NonPublic | BindingFlags.Instance));

        private const int MARGIN_TOP_BOTTOM = 20;

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
            if (delta < 0 && firstItem.Bounds.Top - delta > MARGIN_TOP_BOTTOM)
            {
                delta = firstItem.Bounds.Top - MARGIN_TOP_BOTTOM;
            }
            else if (delta > 0 && delta > lastItem.Bounds.Bottom - dropDown.Height + MARGIN_TOP_BOTTOM)
            {
                delta = lastItem.Bounds.Bottom - dropDown.Height + MARGIN_TOP_BOTTOM;
            }

            if (delta != 0)
            {
                ScrollInternal(dropDown, delta);
            }
        }
    }
}
