#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TiaUtilities.Generation.GridHandler;

/// <summary>
/// Contenuto del popup di filtro stile Excel: ricerca, "Seleziona tutto", lista valori con checkbox, OK/Annulla.
/// </summary>
internal sealed class FilterPopupControl : UserControl
{
    private readonly List<FilterItem> _all;
    private List<FilterItem> _displayed;
    // durante una ricerca le spunte sono gestite a parte (come Excel: i risultati partono tutti selezionati)
    private readonly HashSet<string> _searchUnchecked = new(StringComparer.Ordinal);

    private readonly TextBox _search;
    private readonly CheckBox _selectAll;
    private readonly CheckedListBox _list;
    private readonly Button _ok;
    private bool _updating;

    /// <summary>Insieme dei valori ammessi; null = nessun filtro.</summary>
    public event EventHandler<HashSet<string>?>? Applied;
    public event EventHandler? Cancelled;

    public FilterPopupControl(List<FilterItem> items, string columnHeader, bool isFiltered, string blankText, float scale)
    {
        _all = items;
        _displayed = items;

        int S(int v) => (int)Math.Round(v * scale);

        Size = new Size(S(260), S(330));
        Padding = new Padding(S(6));
        BackColor = SystemColors.Window;

        var clear = new Button
        {
            Text = $"Rimuovi filtro da \"{columnHeader}\"",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            TextAlign = ContentAlignment.MiddleLeft,
            Enabled = isFiltered,
            AutoEllipsis = true,
            Height = S(26),
            Margin = new Padding(0, 0, 0, S(4)),
        };
        clear.FlatAppearance.BorderSize = 0;
        clear.Click += (_, _) => Applied?.Invoke(this, null);

        _search = new TextBox
        {
            Dock = DockStyle.Fill,
            PlaceholderText = "Cerca...",
            Margin = new Padding(0, 0, 0, S(4)),
        };
        _search.TextChanged += (_, _) => RebuildList();
        _search.KeyDown += Search_KeyDown;

        _selectAll = new CheckBox
        {
            Text = "(Seleziona tutto)",
            AutoCheck = false,
            ThreeState = true,
            Dock = DockStyle.Fill,
            AutoSize = true,
            Margin = new Padding(S(2), 0, 0, S(2)),
        };
        _selectAll.Click += SelectAll_Click;

        _list = new CheckedListBox
        {
            Dock = DockStyle.Fill,
            CheckOnClick = true,
            IntegralHeight = false,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 0, 0, S(6)),
        };
        _list.ItemCheck += List_ItemCheck;

        _ok = new Button { Text = "OK", Width = S(75), Height = S(26) };
        _ok.Click += (_, _) => Apply();
        var cancel = new Button { Text = "Annulla", Width = S(75), Height = S(26) };
        cancel.Click += (_, _) => Cancelled?.Invoke(this, EventArgs.Empty);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            WrapContents = false,
            Margin = Padding.Empty,
        };
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(_ok);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(clear, 0, 0);
        layout.Controls.Add(_search, 0, 1);
        layout.Controls.Add(_selectAll, 0, 2);
        layout.Controls.Add(_list, 0, 3);
        layout.Controls.Add(buttons, 0, 4);

        Controls.Add(layout);

        RebuildList();
    }

    public void FocusSearch() => _search.Focus();

    private bool IsSearching => _search.TextLength > 0;

    private bool IsChecked(FilterItem item) => IsSearching ? !_searchUnchecked.Contains(item.Key) : item.Checked;

    private void SetChecked(FilterItem item, bool value)
    {
        if (!IsSearching)
        {
            item.Checked = value;
            return;
        }

        if (value)
        {
            _searchUnchecked.Remove(item.Key);
        }
        else
        {
            _searchUnchecked.Add(item.Key);
        }
    }

    private void RebuildList()
    {
        _searchUnchecked.Clear();
        string text = _search.Text.Trim();
        _displayed = text.Length == 0
            ? _all
            : _all.Where(i => i.Display.Contains(text, StringComparison.CurrentCultureIgnoreCase)).ToList();

        _updating = true;
        _list.BeginUpdate();
        try
        {
            _list.Items.Clear();
            _list.Items.AddRange(_displayed.Cast<object>().ToArray());
            for (int i = 0; i < _displayed.Count; i++)
                if (IsChecked(_displayed[i]))
                    _list.SetItemChecked(i, true);
        }
        finally
        {
            _list.EndUpdate();
            _updating = false;
        }
        UpdateState();
    }

    private void List_ItemCheck(object? sender, ItemCheckEventArgs e)
    {
        if (_updating)
        {
            return;
        }
        SetChecked(_displayed[e.Index], e.NewValue == CheckState.Checked);
        UpdateState();
    }

    private void SelectAll_Click(object? sender, EventArgs e)
    {
        bool check = _selectAll.CheckState != CheckState.Checked;

        _updating = true;
        _list.BeginUpdate();
        try
        {
            for (int i = 0; i < _displayed.Count; i++)
            {
                SetChecked(_displayed[i], check);
                _list.SetItemChecked(i, check);
            }
        }
        finally
        {
            _list.EndUpdate();
            _updating = false;
        }
        UpdateState();
    }

    private void UpdateState()
    {
        int checkedCount = _displayed.Count(IsChecked);
        _selectAll.CheckState =
            checkedCount == 0 ? CheckState.Unchecked :
            checkedCount == _displayed.Count ? CheckState.Checked :
            CheckState.Indeterminate;
        _selectAll.Enabled = _displayed.Count > 0;
        _ok.Enabled = checkedCount > 0;
    }

    private void Search_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Down && _list.Items.Count > 0) 
        {
            e.SuppressKeyPress = true; _list.Focus(); 
            _list.SelectedIndex = 0;
        }
    }

    private void Apply()
    {
        // con ricerca attiva valgono solo i risultati spuntati (come Excel)
        var source = IsSearching ? _displayed : _all;
        var allowed = new HashSet<string>(source.Where(IsChecked).Select(i => i.Key), StringComparer.Ordinal);

        // tutti i valori selezionati = nessun filtro
        bool coversAll = allowed.Count == _all.Count && _all.All(i => allowed.Contains(i.Key));
        Applied?.Invoke(this, coversAll ? null : allowed);
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
        if (keyData == Keys.Escape) 
        {
            Cancelled?.Invoke(this, EventArgs.Empty);
            return true; 

        }

        if (keyData == Keys.Enter && _ok.Enabled) 
        {
            Apply();
            return true; 
        }

        return base.ProcessDialogKey(keyData);
    }
}
