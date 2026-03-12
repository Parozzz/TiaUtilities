using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.SettingsNew.FormHelpers
{
    public class SettingsFormSectionListView : ListView
    {
        public class ItemTag() { }

        public class ItemSectionTag(SettingsFormSection section) : ItemTag
        {
            public bool IsSectionVisible { get; set; } = false;
            public SettingsFormSection Section { get; init; } = section; //Section or MacroSection
        }

        public class ItemMacroSectionTag(SettingsFormMacroSection macroSection) : ItemTag
        {
            public SettingsFormMacroSection MacroSection { get; init; } = macroSection;
        }


        private const int SETCURSOR = 0x0020;
        private const int LBUTTONDOWN = 0x0201;
        private const int MBUTTONDBLCLK = 0x0209;

        public SettingsFormSectionListView()
        {
            this.DoubleBuffered = true; 
            this.Font = SettingsConstants.LIST_LEFT_FONT;
            this.View = View.Tile; //This view shows group 
            this.Dock = DockStyle.Fill;
            this.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.Activation = ItemActivation.OneClick;
            this.BorderStyle = BorderStyle.None;
            this.LabelEdit = false;
            this.AllowColumnReorder = false;
            this.CheckBoxes = false;
            this.FullRowSelect = true;
            this.GridLines = false;
            this.Sorting = SortOrder.None;
            this.Scrollable = false;
            this.MaximumSize = new Size(SettingsConstants.SECTIONS_LIST_VIEW_WIDTH, 0);
            this.MinimumSize = new Size(30, 80); 
            
            var textSize = TextRenderer.MeasureText("AaGg", SettingsConstants.LIST_LEFT_FONT);
            this.TileSize = new Size(SettingsConstants.SECTIONS_LIST_VIEW_WIDTH, textSize.Height + 4);

            this.OwnerDraw = true;
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            var item = e.Item;
            if (item == null || e.State == 0)
            {
                return;
            }

            if (item.Tag is SettingsFormSectionListView.ItemTag tag)
            {
                var rectsLeftPadding = 0;//(SECTIONS_LEFT_PADDING * 3) - 2;

                var foreColor = SettingsConstants.SECTIONS_ITEM_FORE_COLOR;
                var backColor = tag is SettingsFormSectionListView.ItemSectionTag sectionTag && sectionTag.IsSectionVisible ? SettingsConstants.SECTIONS_SELECTED_ITEM_BACK_COLOR : Color.Transparent;

                //BACKGROUND
                var backBounds = e.Bounds;
                backBounds = Rectangle.Inflate(backBounds, -rectsLeftPadding / 2, 0);
                backBounds.Offset(rectsLeftPadding / 2, 0);
                using (var backBrush = new SolidBrush(backColor))
                {
                    e.Graphics.FillRectangle(backBrush, backBounds);
                }

                //DRAW TEXT
                var textBounds = e.Bounds;
                textBounds = Rectangle.Inflate(textBounds, tag is SettingsFormSectionListView.ItemMacroSectionTag ? 0 : -SettingsConstants.SECTIONS_LEFT_PADDING * 3, 0);
                TextRenderer.DrawText(e.Graphics, item.Text, this.Font, textBounds, foreColor, TextFormatFlags.Left);
            }
        }

        protected override void WndProc(ref Message m)
        {
            //https://www.pinvoke.net/default.aspx/enums/windowsmessages.html
            // Suppress mouse messages that are OUTSIDE of the items area
            /*
                LBUTTONDOWN = 0x0201,
                LBUTTONUP = 0x0202,
                LBUTTONDBLCLK = 0x0203,
                RBUTTONDOWN = 0x0204,
                RBUTTONUP = 0x0205,
                RBUTTONDBLCLK = 0x0206,
                MBUTTONDOWN = 0x0207,
                MBUTTONUP = 0x0208,
                MBUTTONDBLCLK = 0x0209, 
            */
            if (m.Msg >= LBUTTONDOWN && m.Msg <= MBUTTONDBLCLK)
            {
                Point pos = new(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);

                var hit = this.HitTest(pos);
                switch (hit.Location)
                {
                    case ListViewHitTestLocations.AboveClientArea:
                    case ListViewHitTestLocations.BelowClientArea:
                    case ListViewHitTestLocations.LeftOfClientArea:
                    case ListViewHitTestLocations.RightOfClientArea:
                    case ListViewHitTestLocations.None:
                        return;

                }
            }

            base.WndProc(ref m);
        }

        public bool IsVerticalScrollbarVisible()
        {
            var delta = (this.Width - this.ClientSize.Width);
            return this.BorderStyle switch
            {
                BorderStyle.None => (delta > 0),
                BorderStyle.FixedSingle => (delta > 2),
                BorderStyle.Fixed3D => (delta > 4),
                _ => throw new NotImplementedException(),
            };
        }

    }
}
