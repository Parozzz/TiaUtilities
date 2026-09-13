using System.ComponentModel;
using TiaUtilities.Utility;

namespace TiaUtilities.CustomControls
{
    [ProvideProperty("FlowBreak", typeof(Control))]
    [DefaultProperty(nameof(FlowDirection))]
    [Docking(DockingBehavior.Ask)]
    public class FlowLayoutPanelNoScrollbars : FlowLayoutPanel, IMessageFilter
    {
        public FlowLayoutPanelNoScrollbars()
        {
            SetStyle(ControlStyles.UserMouse | ControlStyles.Selectable, true);
            this.DoubleBuffered = true;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Application.AddMessageFilter(this);

            VerticalScroll.LargeChange = 60;
            VerticalScroll.SmallChange = 20;
            HorizontalScroll.LargeChange = 60;
            HorizontalScroll.SmallChange = 20;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Application.RemoveMessageFilter(this);
            base.OnHandleDestroyed(e);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);/*
            switch (m.Msg)
            {
                case DllImports.WM_PAINT:
                case DllImports.WM_ERASEBKGND:
                case DllImports.WM_NCCALCSIZE:
                    if (DesignMode || !AutoScroll)
                    {
                        break;
                    }

                    DllImports.ShowScrollBar(this.Handle, DllImports.SB_SHOW_BOTH, false);
                    break;
                case DllImports.WM_MOUSEWHEEL:
                    // Handle Mouse Wheel for other specific cases
                    int delta = (int)(m.WParam.ToInt64() >> 16);
                    int direction = Math.Sign(delta);
                    DllImports.ShowScrollBar(this.Handle, DllImports.SB_SHOW_BOTH, false);
                    break;
            }*/
        }

        public bool PreFilterMessage(ref Message m)
        {/*
            var horScroll = ControlUtils.WncProcHorizontalScrollWheel(m);
            if (horScroll != 0)
            {
                DllImports.SendMessage(this.Handle, DllImports.WM_MOUSEWHEEL, m.WParam, m.LParam);
                return true;
            }

            var mousePosition = Control.MousePosition;
            switch (m.Msg)
            {
                case DllImports.WM_MOUSEWHEEL:
                case DllImports.WM_MOUSEHWHEEL:
                    if (base.DesignMode || !base.AutoScroll)
                    {
                        return false;
                    }

                    if (base.VerticalScroll.Maximum <= ClientSize.Height)
                    {
                        return false;
                    }

                    // Should also check whether the ForegroundWindow matches the parent Form.
                    if (base.RectangleToScreen(ClientRectangle).Contains(mousePosition))
                    {
                        DllImports.SendMessage(this.Handle, DllImports.WM_MOUSEWHEEL, m.WParam, m.LParam);
                        return true;
                    }
                    break;
                case DllImports.WM_LBUTTONDOWN:
                    // Pre-handle Left Mouse clicks for all child Controls
                    if (RectangleToScreen(ClientRectangle).Contains(mousePosition))
                    {
                        // Inside our bounds but it's not our window
                        if (base.TopLevelControl == null || DllImports.GetForegroundWindow() != base.TopLevelControl.Handle)
                        {
                            return false;
                        }

                        // The hosted Control that contains the mouse pointer 
                        var ctrl = FromHandle(DllImports.ChildWindowFromPoint(this.Handle, PointToClient(mousePosition)));
                        // A child Control of the hosted Control that will be clicked 
                        // If no child Controls at that position the Parent's handle
                        var child = FromHandle(DllImports.WindowFromPoint(mousePosition));
                    }
                    return false;
                    // Eventually, if you don't want the message to reach the child Control
                    // return true; 
            }*/
            return false;
        }

    }
}
