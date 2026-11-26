#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace TiaUtilities.Generation.GridHandler;

/// <summary>
/// Aggiunge a una DataGridView in VirtualMode un filtro "stile Excel" sulle intestazioni di colonna.
/// <para>
/// La classe NON legge direttamente i dati: riceve due delegati (numero righe e valore di una cella
/// della sorgente) e mantiene una mappa "riga griglia → riga sorgente".
/// </para>
/// <para>Integrazione:</para>
/// <list type="number">
/// <item>Nel CellPainting, alla fine del disegno dell'intestazione: <c>filter.PaintGlyph(e);</c></item>
/// <item>Nel CellValueNeeded / CellValuePushed: <c>int src = filter.ToSourceRow(e.RowIndex);</c></item>
/// <item>Al posto di <c>grid.RowCount = ...</c>, dopo ogni modifica dei dati: <c>filter.Refresh();</c></item>
/// <item>Nel ColumnHeaderMouseClick (es. ordinamento): <c>if (filter.IsOnFilterGlyph(e)) return;</c></item>
/// </list>
/// </summary>
public sealed class DataGridViewHeaderFilter : IDisposable
{
    private readonly DataGridView _grid;
    private readonly Func<int> _getSourceRowCount;
    private readonly Func<int, int, object?> _getSourceValue;

    // Chiave = colonna (non l'indice, così sopravvive a inserimenti/rimozioni di colonne)
    private readonly Dictionary<DataGridViewColumn, HashSet<string>> _filters = new();
    private readonly Dictionary<DataGridViewColumn, Padding> _originalPaddings = new();

    private int[]? _map;              // null = nessun filtro attivo (mappa identità)
    private DataGridViewColumn? _hotColumn;
    private Cursor? _savedCursor;
    private ToolStripDropDown? _dropDown;
    private bool _disposed;

    /// <summary>Sollevato quando cambiano i criteri di filtro (da popup o da codice).</summary>
    public event EventHandler? FilterChanged;

    /// <summary>Testo mostrato nella lista per valori null / DBNull / stringa vuota.</summary>
    public string BlankText { get; set; } = "(Vuote)";
    public Color GlyphColor { get; set; } = Color.DarkSlateBlue;
    public Color ActiveGlyphColor { get; set; } = Color.FromArgb(0, 102, 204);
    public Color HotBackColor { get; set; } = Color.FromArgb(50, 0, 120, 215);

    /// <param name="grid">DataGridView in VirtualMode.</param>
    /// <param name="getSourceRowCount">Numero di righe della sorgente, es. <c>() =&gt; _ds.Count</c>.</param>
    /// <param name="getSourceValue">
    /// Valore della cella (rigaSorgente, indiceColonna), es. <c>(r, c) =&gt; GetCellValue(_ds[r], c)</c>.
    /// Conviene riusare la stessa logica del proprio CellValueNeeded.
    /// </param>
    /// <param name="enableAllColumns">Se true abilita subito il filtro su tutte le colonne esistenti.</param>
    public DataGridViewHeaderFilter(
        DataGridView grid,
        Func<int> getSourceRowCount,
        Func<int, int, object?> getSourceValue,
        bool enableAllColumns = true)
    {
        _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        _getSourceRowCount = getSourceRowCount ?? throw new ArgumentNullException(nameof(getSourceRowCount));
        _getSourceValue = getSourceValue ?? throw new ArgumentNullException(nameof(getSourceValue));

        _grid.CellMouseClick += Grid_CellMouseClick;
        _grid.CellMouseMove += Grid_CellMouseMove;
        _grid.CellMouseLeave += Grid_CellMouseLeave;
        _grid.ColumnRemoved += Grid_ColumnRemoved;

        if (enableAllColumns)
            foreach (DataGridViewColumn c in _grid.Columns)
                EnableFilter(c);
    }

    #region API pubblica

    /// <summary>True se almeno una colonna ha un filtro attivo.</summary>
    public bool IsAnyFilterActive => _filters.Count > 0;

    /// <summary>Numero di righe visibili (escluse eventuali righe "nuova riga").</summary>
    public int VisibleRowCount => _map?.Length ?? _getSourceRowCount();

    /// <summary>Abilita/disabilita l'imbuto su una colonna.</summary>
    public void EnableFilter(DataGridViewColumn column, bool enabled = true)
    {
        ArgumentNullException.ThrowIfNull(column);

        if (enabled)
        {
            if (_originalPaddings.ContainsKey(column)) return;
            var p = column.HeaderCell.Style.Padding;
            _originalPaddings[column] = p;
            // riserva spazio a destra perché il testo dell'header non finisca sotto l'imbuto
            column.HeaderCell.Style.Padding = new Padding(p.Left, p.Top, p.Right + GlyphReservedWidth, p.Bottom);
        }
        else
        {
            if (!_originalPaddings.Remove(column, out var p)) return;
            column.HeaderCell.Style.Padding = p;
            if (_filters.Remove(column))
            {
                Refresh();
                OnFilterChanged();
            }
        }
        _grid.InvalidateCell(column.HeaderCell);
    }

    public bool IsFilterEnabled(DataGridViewColumn column) => _originalPaddings.ContainsKey(column);

    public bool IsFiltered(DataGridViewColumn column) => _filters.ContainsKey(column);

    /// <summary>Converte l'indice di riga della griglia nell'indice della sorgente (-1 se fuori range / nuova riga).</summary>
    public int ToSourceRow(int viewRowIndex)
    {
        if (viewRowIndex < 0) return -1;
        if (_map is null) return viewRowIndex < _getSourceRowCount() ? viewRowIndex : -1;
        return viewRowIndex < _map.Length ? _map[viewRowIndex] : -1;
    }

    /// <summary>Converte l'indice della sorgente nell'indice di riga della griglia (-1 se la riga è filtrata via).</summary>
    public int ToViewRow(int sourceRowIndex)
    {
        if (sourceRowIndex < 0) return -1;
        if (_map is null) return sourceRowIndex;
        int i = Array.BinarySearch(_map, sourceRowIndex);
        return i >= 0 ? i : -1;
    }

    /// <summary>
    /// Ricalcola le righe visibili e aggiorna <see cref="DataGridView.RowCount"/>.
    /// Da chiamare dopo ogni modifica della sorgente dati (al posto di impostare RowCount a mano).
    /// </summary>
    public void Refresh()
    {
        ThrowIfDisposed();

        // ricorda la riga sorgente corrente per riposizionarsi dopo il ricalcolo
        int currentSource = _grid.CurrentCell is { } cc ? ToSourceRow(cc.RowIndex) : -1;
        int currentCol = _grid.CurrentCell?.ColumnIndex ?? -1;

        if (_filters.Count == 0)
        {
            _map = null;
        }
        else
        {
            int n = _getSourceRowCount();
            var active = _filters.ToArray();
            var rows = new List<int>(n);
            for (int r = 0; r < n; r++)
                if (PassesFilters(r, active, except: null))
                    rows.Add(r);
            _map = rows.ToArray();
        }

        if (_grid.IsCurrentCellInEditMode) _grid.EndEdit();

        int count = VisibleRowCount + (_grid.AllowUserToAddRows ? 1 : 0);
        if (_grid.RowCount != count) _grid.RowCount = count;
        _grid.Invalidate();

        if (currentSource >= 0 && currentCol >= 0)
        {
            int v = ToViewRow(currentSource);
            if (v >= 0 && v < _grid.RowCount)
            {
                try { _grid.CurrentCell = _grid[currentCol, v]; }
                catch (InvalidOperationException) { /* validazione in corso: ignora */ }
            }
        }
    }

    /// <summary>Imposta da codice i valori ammessi per una colonna (null o vuoto = rimuove il filtro).</summary>
    public void SetFilter(DataGridViewColumn column, IEnumerable<object?>? allowedValues)
    {
        ArgumentNullException.ThrowIfNull(column);
        if (allowedValues is null)
        {
            _filters.Remove(column);
        }
        else
        {
            var set = new HashSet<string>(allowedValues.Select(v => ToKey(v, column)), StringComparer.Ordinal);
            if (set.Count == 0) _filters.Remove(column); else _filters[column] = set;
        }
        Refresh();
        _grid.InvalidateCell(column.HeaderCell);
        OnFilterChanged();
    }

    public void ClearFilter(DataGridViewColumn column)
    {
        if (!_filters.Remove(column)) return;
        Refresh();
        _grid.InvalidateCell(column.HeaderCell);
        OnFilterChanged();
    }

    public void ClearAllFilters()
    {
        if (_filters.Count == 0) return;
        _filters.Clear();
        Refresh();
        _grid.Invalidate();
        OnFilterChanged();
    }

    /// <summary>
    /// Da chiamare nel proprio handler CellPainting, DOPO aver disegnato l'intestazione.
    /// Non tocca e.Handled.
    /// </summary>
    public void PaintGlyph(DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex != -1 || e.ColumnIndex < 0 || e.Graphics is null) 
        { 
            return;
        }

        var column = _grid.Columns[e.ColumnIndex];
        if (!IsFilterEnabled(column))
        {
            return;
        }

        var glyph = GetGlyphBounds(e.CellBounds);
        DrawFunnel(e.Graphics, glyph, IsFiltered(column), ReferenceEquals(column, _hotColumn));
    }

    /// <summary>True se il click è avvenuto sull'imbuto (usalo per saltare l'ordinamento nel ColumnHeaderMouseClick).</summary>
    public bool IsOnFilterGlyph(DataGridViewCellMouseEventArgs e)
    {
        if (e.RowIndex != -1 || e.ColumnIndex < 0) return false;
        var column = _grid.Columns[e.ColumnIndex];
        if (!IsFilterEnabled(column)) return false;
        // e.Location è relativa all'origine della cella
        var local = GetGlyphBounds(new Rectangle(0, 0, column.Width, _grid.ColumnHeadersHeight));
        return local.Contains(e.Location);
    }

    /// <summary>Apre il popup di filtro per la colonna indicata.</summary>
    public void ShowFilterPopup(DataGridViewColumn column)
    {
        ThrowIfDisposed();
        if (!IsFilterEnabled(column)) return;
        _dropDown?.Close();

        if (_grid.IsCurrentCellInEditMode) _grid.EndEdit();

        var items = BuildItems(column);
        float scale = Scale;
        var popup = new FilterPopupControl(items, column.HeaderText, IsFiltered(column), BlankText, scale);

        var host = new ToolStripControlHost(popup)
        {
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            AutoSize = false,
            Size = popup.Size
        };
        var dd = new ToolStripDropDown { Padding = Padding.Empty, AutoClose = true };
        dd.Items.Add(host);

        popup.Applied += (_, allowed) =>
        {
            dd.Close();
            ApplyFromPopup(column, allowed);
        };
        popup.Cancelled += (_, _) => dd.Close();
        dd.Opened += (_, _) => popup.FocusSearch();
        dd.Closed += (_, _) =>
        {
            if (ReferenceEquals(_dropDown, dd)) _dropDown = null;
            _grid.BeginInvoke(new Action(dd.Dispose));
        };

        _dropDown = dd;

        var cellRect = _grid.GetCellDisplayRectangle(column.Index, -1, false);
        var pt = new Point(Math.Max(0, cellRect.Right - popup.Width), cellRect.Bottom);
        dd.Show(_grid, pt);
    }

    #endregion

    #region Logica filtro

    private void ApplyFromPopup(DataGridViewColumn column, HashSet<string>? allowed)
    {
        if (allowed is null) _filters.Remove(column);
        else _filters[column] = allowed;

        Refresh();
        _grid.InvalidateCell(column.HeaderCell);
        OnFilterChanged();
    }

    private bool PassesFilters(int sourceRow, KeyValuePair<DataGridViewColumn, HashSet<string>>[] active, DataGridViewColumn? except)
    {
        foreach (var (column, allowed) in active)
        {
            if (ReferenceEquals(column, except)) continue;
            if (!allowed.Contains(ToKey(_getSourceValue(sourceRow, column.Index), column)))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Valori distinti della colonna calcolati sulle righe che passano i filtri delle ALTRE colonne
    /// (stesso comportamento di Excel).
    /// </summary>
    private List<FilterItem> BuildItems(DataGridViewColumn column)
    {
        int n = _getSourceRowCount();
        var active = _filters.ToArray();
        var distinct = new Dictionary<string, object?>(StringComparer.Ordinal);

        for (int r = 0; r < n; r++)
        {
            if (!PassesFilters(r, active, except: column)) continue;
            object? raw = _getSourceValue(r, column.Index);
            string key = ToKey(raw, column);
            distinct.TryAdd(key, raw);
        }

        _filters.TryGetValue(column, out var current);

        var list = distinct
            .Select(kv => new FilterItem(kv.Key, kv.Key.Length == 0 ? BlankText : kv.Key, kv.Value,
                                         current is null || current.Contains(kv.Key)))
            .ToList();

        list.Sort(static (a, b) =>
        {
            // le "(Vuote)" sempre in fondo
            if (a.Key.Length == 0) return b.Key.Length == 0 ? 0 : 1;
            if (b.Key.Length == 0) return -1;
            return CompareRaw(a.Raw, b.Raw, a.Display, b.Display);
        });
        return list;
    }

    private static int CompareRaw(object? a, object? b, string da, string db)
    {
        if (a is not null && b is not null && a.GetType() == b.GetType() && a is IComparable ca)
        {
            try { return ca.CompareTo(b); } catch (ArgumentException) { }
        }
        return string.Compare(da, db, StringComparison.CurrentCultureIgnoreCase);
    }

    /// <summary>Chiave testuale del valore, formattata come nella colonna (Format/FormatProvider).</summary>
    private static string ToKey(object? value, DataGridViewColumn column)
    {
        if (value is null || value is DBNull) return string.Empty;

        var style = column.DefaultCellStyle;
        IFormatProvider provider = style.FormatProvider ?? CultureInfo.CurrentCulture;

        string s = value is IFormattable f && !string.IsNullOrEmpty(style.Format)
            ? f.ToString(style.Format, provider)
            : Convert.ToString(value, provider) ?? string.Empty;

        return s.Trim();
    }

    #endregion

    #region Disegno e mouse

    private float Scale => _grid.DeviceDpi / 96f;
    private int GlyphSize => (int)Math.Round(16 * Scale);
    // margine dal bordo destro: evita la zona di ridimensionamento colonna
    private int GlyphMargin => (int)Math.Round(6 * Scale);
    private int GlyphReservedWidth => GlyphSize + GlyphMargin + (int)Math.Round(2 * Scale);

    private Rectangle GetGlyphBounds(Rectangle cellBounds)
    {
        int size = GlyphSize;
        int x = cellBounds.Right - size - GlyphMargin;
        int y = cellBounds.Top + (cellBounds.Height - size) / 2;
        return new Rectangle(x, y, size, size);
    }

    private void DrawFunnel(Graphics g, Rectangle r, bool active, bool hot)
    {
        var state = g.Save();
        try
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            if (hot || active)
            {
                using var back = new SolidBrush(hot ? HotBackColor : Color.FromArgb(35, ActiveGlyphColor));
                using var path = RoundedRect(r, Math.Max(2, (int)(3 * Scale)));
                g.FillPath(back, path);
            }

            float pad = r.Width * 0.18f;
            var f = new RectangleF(r.X + pad, r.Y + pad, r.Width - 2 * pad, r.Height - 2 * pad);
            float w = f.Width, h = f.Height;

            PointF[] pts =
            [
                new(f.Left, f.Top),
                new(f.Right, f.Top),
                new(f.Left + w * 0.62f, f.Top + h * 0.50f),
                new(f.Left + w * 0.62f, f.Top + h * 0.85f),
                new(f.Left + w * 0.38f, f.Bottom),
                new(f.Left + w * 0.38f, f.Top + h * 0.50f),
            ];

            var color = active ? ActiveGlyphColor : GlyphColor;
            if (active)
            {
                using var fill = new SolidBrush(color);
                g.FillPolygon(fill, pts);
            }
            using var pen = new Pen(color, Math.Max(1f, 1.2f * Scale)) { LineJoin = LineJoin.Round };
            g.DrawPolygon(pen, pts);
        }
        catch(Exception e)
        {
            Debug.WriteLine(e.Message);
        }
        finally
        {
            g.Restore(state);
        }
    }

    private static GraphicsPath RoundedRect(Rectangle r, int radius)
    {
        int d = radius * 2;
        var p = new GraphicsPath();
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }

    private void Grid_CellMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left || !IsOnFilterGlyph(e)) return;
        ShowFilterPopup(_grid.Columns[e.ColumnIndex]);
    }

    private void Grid_CellMouseMove(object? sender, DataGridViewCellMouseEventArgs e)
    {
        var hot = IsOnFilterGlyph(e) ? _grid.Columns[e.ColumnIndex] : null;
        SetHotColumn(hot);
    }

    private void Grid_CellMouseLeave(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex == -1) SetHotColumn(null);
    }

    private void SetHotColumn(DataGridViewColumn? column)
    {
        if (ReferenceEquals(column, _hotColumn)) return;

        var old = _hotColumn;
        _hotColumn = column;

        if (old is null && column is not null)
        {
            _savedCursor = _grid.Cursor;
            _grid.Cursor = Cursors.Hand;
        }
        else if (old is not null && column is null)
        {
            _grid.Cursor = _savedCursor ?? Cursors.Default;
            _savedCursor = null;
        }

        if (old is not null && old.DataGridView == _grid) _grid.InvalidateCell(old.HeaderCell);
        if (column is not null) _grid.InvalidateCell(column.HeaderCell);
    }

    private void Grid_ColumnRemoved(object? sender, DataGridViewColumnEventArgs e)
    {
        _originalPaddings.Remove(e.Column);
        if (ReferenceEquals(_hotColumn, e.Column)) SetHotColumn(null);
        if (_filters.Remove(e.Column))
        {
            Refresh();
            OnFilterChanged();
        }
    }

    #endregion

    private void OnFilterChanged() => FilterChanged?.Invoke(this, EventArgs.Empty);

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _grid.CellMouseClick -= Grid_CellMouseClick;
        _grid.CellMouseMove -= Grid_CellMouseMove;
        _grid.CellMouseLeave -= Grid_CellMouseLeave;
        _grid.ColumnRemoved -= Grid_ColumnRemoved;

        _dropDown?.Close();
        _dropDown = null;

        foreach (var (column, padding) in _originalPaddings)
            if (column.DataGridView == _grid) column.HeaderCell.Style.Padding = padding;
        _originalPaddings.Clear();
        _filters.Clear();
        _map = null;
    }
}

/// <summary>Voce della lista valori nel popup.</summary>
internal sealed class FilterItem(string key, string display, object? raw, bool isChecked)
{
    public string Key { get; } = key;
    public string Display { get; } = display;
    public object? Raw { get; } = raw;
    public bool Checked { get; set; } = isChecked;
    public override string ToString() => Display;
}
