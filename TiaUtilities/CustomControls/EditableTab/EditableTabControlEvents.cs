using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.CustomControls.EditableTab
{
    public static class EditableTabControlEvents
    {
        public delegate void TabRemovedEventHandler(object? sender, TabRemovedEventArgs args);
        public class TabRemovedEventArgs(TabPage tabPage, int tabIndex) : EventArgs
        {
            public TabPage TabPage { get; init; } = tabPage;
            public int TabIndex { get; init; } = tabIndex;
            public bool Cancel { get; set; }
        }

        public delegate void TabAddedEventHandler(object? sender, TabAddedEventArgs args);
        public class TabAddedEventArgs(TabPage tabPage) : EventArgs
        {
            public TabPage TabPage { get; init; } = tabPage;
            public bool Cancel { get; set; }
        }

        public delegate void TabRenamedEventHandler(object? sender, TabRenamedEventArgs args);
        public class TabRenamedEventArgs(TabPage tabPage, string newName, string oldName) : EventArgs
        {
            public TabPage TabPage { get; init; } = tabPage;
            public string NewName { get; set; } = newName;
            public string OldName { get; init; } = oldName;
            public bool Handled { get; set; } = false;
        }
    }
}
