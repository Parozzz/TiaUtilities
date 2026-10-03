using DocumentFormat.OpenXml.Bibliography;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using TiaUtilities.Utility;

namespace TiaUtilities.CustomControls
{
    public class ComboBoxFilterable : ComboBox
    {
        // Proprietà personalizzabili per i colori di selezione
        public Color DropDownHoveredBackColor { get; set; } = Color.LightSeaGreen; // Colore di sfondo della selezione
        public Color DropDownHoveredForeColor { get; set; } = Color.Black;      // Colore del testo della selezione

        public bool AutoWidthFromItems { get; set; } = true;
        public int AutoWidthRightPadding { get; set; } = 20;


        public new IEnumerable<object>? DataSource
        {
            get => _fullDataSource;
            set => this.UpdateFullDataSource(value);
        }

        public Func<object, string, bool>? FilterPredicate { get; set; }


        private readonly ListChangedEventHandler listChangedEvent;

        private IEnumerable<object>? _fullDataSource;
        private string? _lastFilterString;
        private bool _isFiltering;

        public ComboBoxFilterable()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.AutoCompleteMode = AutoCompleteMode.None;
            this.DrawMode = DrawMode.OwnerDrawFixed;

            this.listChangedEvent = (sender, args) =>
            {
                if (sender is IEnumerable<object> enumerable)
                {
                    base.DataSource = enumerable.Cast<object>().ToList();
                    this.CalculateWidthFromItems();
                }
            };
        }

        private void UpdateFullDataSource(object? source)
        {
            if (this._fullDataSource is IBindingList oldBindingList)
            {
                oldBindingList.ListChanged -= this.listChangedEvent;
            }

            if (source is not IEnumerable<object> enumerable)
            {
                _fullDataSource = [];
                base.DataSource = null;
                return;
            }

            if (enumerable is IBindingList newBindingList)
            {//BindingSource implements IBindingList
                newBindingList.ListChanged += this.listChangedEvent;
            }

            this._fullDataSource = enumerable;
            base.DataSource = this._fullDataSource.Cast<object>().ToList();

            this.CalculateWidthFromItems();
        }

        private void CalculateWidthFromItems()
        {
            if (!this.AutoWidthFromItems || this._fullDataSource == null || !this._fullDataSource.Any())
            {
                return;
            }


            var maxWidth = this._fullDataSource
                .Select(this.GetItemText)
                .Max(n => TextRenderer.MeasureText(n, base.Font, Size.Empty, TextFormatFlags.TextBoxControl).Width);

            this.MinimumSize = new(maxWidth + this.AutoWidthRightPadding, this.MinimumSize.Height);
            this.Width = this.MinimumSize.Width;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            Color backColor = isSelected ? DropDownHoveredBackColor : this.BackColor;
            Color foreColor = isSelected ? DropDownHoveredForeColor : this.ForeColor;

            using SolidBrush bgBrush = new(backColor);
            e.Graphics.FillRectangle(bgBrush, e.Bounds);

            var item = this.Items[e.Index];
            var itemText = base.GetItemText(item);

            TextRenderer.DrawText(
                e.Graphics,
                itemText,
                e.Font ?? this.Font,
                e.Bounds,
                foreColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter
            );

            e.DrawFocusRectangle();
        }

        protected override void OnTextUpdate(EventArgs e)
        {
            base.OnTextUpdate(e);

            if (_fullDataSource == null || this.Sorted || this.AutoCompleteMode != AutoCompleteMode.None || !this.IsHandleCreated) //Cannot change DataSource to a Sorted Combobox
            {
                return;
            }

            string searchText = this.Text;
            var selectionStart = this.SelectionStart;
            //Debug.WriteLine($"OnTextUpdate SearchText: {searchText}, LastFilteringString: {_lastFilterString}");

            _isFiltering = true;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                base.DataSource = _fullDataSource.ToList();
                this.DroppedDown = true;

                this.BeginInvoke(() =>
                {
                    this.Text = string.Empty;
                    this.SelectionStart = selectionStart;
                });
            }
            else
            {
                var filteredList = _fullDataSource.Cast<object>()
                    .Where(item => this.FilterPredicate == null ? this.DefaultFilter(item, searchText) : this.FilterPredicate(item, searchText))
                    .ToList();

                var listsEquality = false;
                if (base.DataSource is IList dataSourceList)
                {
                    listsEquality = dataSourceList.Cast<object>().SequenceEqual(filteredList);
                }

                if (!listsEquality || !this.DroppedDown)
                {
                    base.DataSource = filteredList;
                    this.DroppedDown = true;
                    this.BeginInvoke(() =>
                    {
                        this.Text = searchText;
                        this.SelectionStart = selectionStart; //After each input it would move the caret to the first position. }
                    });
                }
            }

            _lastFilterString = searchText;
            _isFiltering = false;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == DllImports.WM_MOUSEWHEEL && !this.DroppedDown)
            {//Avoid having the scroll wheel to change values. Annoying since scrolling can casually change value.
                return;
            }

            try
            {
                base.WndProc(ref m);
            }
            catch (ArgumentOutOfRangeException)
            {
                _isFiltering = true;

                // Paracadute finale per intercettare l'eccezione nativa di WinForms 
                // nel caso in cui get_SelectedItem() venga chiamato internamente.
                base.DataSource = _fullDataSource?.ToList();
                this.SelectedIndex = _fullDataSource == null || !_fullDataSource.Any() ? -1 : 0;
                this.Text = _lastFilterString;

                _isFiltering = false;

                this.BeginInvoke(() =>
                {
                    var text = this.GetItemText(this.SelectedItem);
                    this.Text = text;
                });
            }
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
        }

        protected override void OnSelectedValueChanged(EventArgs e)
        {
            base.OnSelectedValueChanged(e);
        }

        protected override void OnDataSourceChanged(EventArgs e)
        {
            base.OnDataSourceChanged(e);
        }

        protected override void OnDropDown(EventArgs e)
        {
            if (_fullDataSource == null || this.Sorted || this.AutoCompleteMode != AutoCompleteMode.None) //Cannot change DataSource to a Sorted Combobox
            {
                base.OnDropDown(e);
                return;
            }

            // Quando si apre la tendina via click o freccia, ripristina la lista completa
            if (!_isFiltering && base.DataSource != _fullDataSource)
            {
                string currentText = this.Text;
                int selectionStart = this.SelectionStart;

                base.DataSource = _fullDataSource.ToList();

                this.Text = currentText;
                this.SelectionStart = selectionStart;
            }

            base.OnDropDown(e);
        }

        /// <summary>
        /// Algoritmo di filtro di default: cerca tutte le parole digitate all'interno dell'oggetto.
        /// </summary>
        private bool DefaultFilter(object? item, string searchText)
        {
            var itemText = this.GetItemText(item);
            if (itemText == null)
            {
                return false;
            }

            string[] searchWords = searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return searchWords.All(word => itemText.Contains(word, StringComparison.OrdinalIgnoreCase));
        }
    }
}
