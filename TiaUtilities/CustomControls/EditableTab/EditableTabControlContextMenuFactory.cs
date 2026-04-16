using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Languages;
using TiaUtilities.Resources;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.CustomControls.EditableTab
{
    public static class EditableTabControlContextMenuFactory
    {
        private const string IMAGE_QUICK_EDIT_KEY = "QuickEdit";
        private const string IMAGE_ADD_KEY = "Add";
        private const string IMAGE_EDIT_NAME_KEY = "EditName";
        private const string IMAGE_CLOSE_KEY = "Close";

        public static ContextMenuStrip CreateContextMenu(EditableTabControl tabControl, TabPage tabPage)
        {
            var tabPageIndex = tabControl.TabPages.IndexOf(tabPage);

            ImageList imageList = new();
            imageList.Images.Add(IMAGE_QUICK_EDIT_KEY, ImageResources.EDIT_562275);
            imageList.Images.Add(IMAGE_ADD_KEY, ImageResources.ADD_501366_007435);
            imageList.Images.Add(IMAGE_EDIT_NAME_KEY, ImageResources.A_TO_Z_72773);
            imageList.Images.Add(IMAGE_CLOSE_KEY, ImageResources.CLOSE_193002_FF001C);

            var contextMenu = new ContextMenuStrip()
            {
                MinimumSize = new(300, 0),
                ImageList = imageList,
            };
            contextMenu.KeyDown += (sender, args) => EditableTabControlContextMenuFactory.CloseOnKeyPress(contextMenu, args);

            ToolStripMenuItem quickEditItem = new()
            {
                Text = Locale.EDITABLE_TAB_CONTROL_CONTEXT_MENU_QUICK_EDIT,
                ImageKey = IMAGE_QUICK_EDIT_KEY,
            };
            quickEditItem.Click += (sender, args) =>
            {
                tabControl.SuspendLayout();

                var quickEditForm = new EditableTabControlQuickEditForm(tabControl);
                var result = quickEditForm.ShowDialog(tabControl);
                if (result == DialogResult.OK)
                {
                    var oldSelectedTab = tabControl.SelectedTab;

                    Dictionary<EditableTabControlQuickEditForm.RowData, EditableTabControl.CloseRequest?> rowDataWithCloseDict = [];
                    foreach (var rowData in quickEditForm.RowsData)
                    {
                        tabControl.RenameTab(rowData.TabPage, rowData.NameTextBox.Text);

                        EditableTabControl.CloseRequest? closeRequest = null;
                        if (rowData.Info.NeedsDeletition)
                        {
                            closeRequest = new() { TabPage = rowData.TabPage };
                        }

                        rowDataWithCloseDict.Add(rowData, closeRequest);
                    }

                    var closeRequests = rowDataWithCloseDict.Values.WhereNotNull();
                    tabControl.CloseTabs(closeRequests);

                    var pages = rowDataWithCloseDict.OrderBy(p => p.Key.Info.Index)
                                                    .Where(p => p.Value is null || !p.Value.Closed)
                                                    .Select(p => p.Key.TabPage)
                                                    .ToArray();

                    tabControl.TabPages.Clear();
                    tabControl.TabPages.AddRange(pages);

                    if (oldSelectedTab != null && oldSelectedTab.Parent != null)
                    {
                        tabControl.SelectedTab = oldSelectedTab;
                    }
                }

                tabControl.ResumeLayout(true);
            };

            ToolStripMenuItem addFiveTabs = new()
            {
                Text = Locale.EDITABLE_TAB_CONTROL_CONTEXT_MENU_INSERT_TABS.Replace("{c}", "5"),
                ImageKey = IMAGE_ADD_KEY
            };
            addFiveTabs.Click += (sender, args) => tabControl.InsertTabs(tabPageIndex, count: 5);

            ToolStripMenuItem addOneTab = new()
            {
                Text = Locale.EDITABLE_TAB_CONTROL_CONTEXT_MENU_INSERT_TABS.Replace("{c}", "1"),
                ImageKey = IMAGE_ADD_KEY
            };
            addOneTab.Click += (sender, args) => tabControl.InsertTabs(tabPageIndex, count: 1);

            ToolStripMenuItem closeItem = new()
            {
                Text = Locale.EDITABLE_TAB_CONTROL_CONTEXT_MENU_CLOSE,
                ImageKey = IMAGE_CLOSE_KEY,
            };
            closeItem.Click += (sender, args) => tabControl.CloseTab(tabPage);

            Label editNameLabel = new()
            {
                Text = Locale.EDITABLE_TAB_CONTROL_CONTEXT_MENU_EDIT_NAME,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new(0),
                Margin = new(0, 0, 8, 0),
            };

            TextBox editNameTextBox = new()
            {
                Text = tabPage.Text,
                TextAlign = HorizontalAlignment.Left,
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill,
                Padding = new(0),
                Margin = new(0),
                MinimumSize = new(200, 0)
            };
            editNameTextBox.KeyDown += (sender, args) => EditableTabControlContextMenuFactory.CloseOnKeyPress(contextMenu, args);

            FlowLayoutPanel panel = new()
            {
                AutoSize = true,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Controls = { editNameLabel, editNameTextBox },
                Padding = new(0),
            };

            ToolStripControlHost editNameHost = new(panel)
            {
                BackColor = Color.Transparent,
                ImageKey = IMAGE_EDIT_NAME_KEY, //Seems to not work :(
            };


            contextMenu.Items.Add(editNameHost);
            contextMenu.Items.Add(closeItem);
            contextMenu.Items.Add(new ToolStripSeparator() { Margin = new(0), Padding = new(0) });
            contextMenu.Items.Add(quickEditItem);
            contextMenu.Items.Add(addOneTab);
            contextMenu.Items.Add(addFiveTabs);

            contextMenu.Closed += (sender, args) =>
            {
                tabControl.RenameTab(tabPage, editNameTextBox.Text);
            };

            return contextMenu;
        }

        private static void CloseOnKeyPress(ContextMenuStrip contextMenu, KeyEventArgs args)
        {
            if (args.KeyData == Keys.Escape || args.KeyData == Keys.Enter)
            {
                contextMenu.Close();
            }
        }
    }
}
