namespace TiaUtilities.CustomControls
{
    public class NoScrollbarListBox : ListBox
    {
        public NoScrollbarListBox()
        {
            this.DoubleBuffered = true;
        }

        private bool mShowScroll;
        public bool ShowScrollbar
        {
            get => mShowScroll;
            set
            {
                if (value != mShowScroll)
                {
                    mShowScroll = value;
                    if (IsHandleCreated)
                    {
                        RecreateHandle();
                    }
                }
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (!mShowScroll)
                {
                    cp.Style = cp.Style & ~0x200000;
                }
                return cp;
            }
        }

    }
}
