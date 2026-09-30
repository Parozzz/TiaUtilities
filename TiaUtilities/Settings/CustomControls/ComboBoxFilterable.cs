using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using TiaUtilities.Utility;

namespace TiaUtilities.SettingsStep.CustomControls
{
    public class ComboBoxFilterable : ComboBox
    {
        // Proprietà personalizzabili per i colori di selezione
        public Color DropDownHoveredBackColor { get; set; } = Color.LightSeaGreen; // Colore di sfondo della selezione
        public Color DropDownHoveredForeColor { get; set; } = Color.Black;      // Colore del testo della selezione

        public bool AutoWidthFromItems { get; set; } = true;
        public int AutoWidthRightPadding { get; set; } = 20;


        public Func<object, string, bool>? FilterPredicate { get; set; }

        private List<object>? _unfilteredItems;
        private string? _lastFilterString;
        private bool _isFiltering;

        public ComboBoxFilterable()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.AutoCompleteMode = AutoCompleteMode.None;
            this.DrawMode = DrawMode.OwnerDrawFixed;
        }

        public void SetFilterableSource<T>(IEnumerable<T> source, string displayMember = "", string valueMember = "")
        {
            if (!string.IsNullOrEmpty(displayMember))
            {
                this.DisplayMember = displayMember;
            }

            if (!string.IsNullOrEmpty(valueMember))
            {
                this.ValueMember = valueMember;
            }

            if(source is IBindingList bindingList)
            {//BindingSource implements IBindingList
                bindingList.ListChanged += (sender, args) =>
                {
                    this._unfilteredItems = [.. bindingList.Cast<object>()];
                    this.DataSource = this._unfilteredItems;

                    this.CalculateWidthFromItems();
                };
            }

            this._unfilteredItems = [.. source.Cast<object>()];
            this.DataSource = this._unfilteredItems;

            this.CalculateWidthFromItems();
        }

        private void CalculateWidthFromItems()
        {
            if(!this.AutoWidthFromItems || this._unfilteredItems == null || this._unfilteredItems.Count <= 0)
            {
                return;
            }

            var maxWidth = this._unfilteredItems.Select(this.GetDisplayMember).Max(n => TextRenderer.MeasureText(n, base.Font, Size.Empty, TextFormatFlags.TextBoxControl).Width);
            this.Width = maxWidth + this.AutoWidthRightPadding;
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

            if (_unfilteredItems == null || this.Sorted || this.AutoCompleteMode != AutoCompleteMode.None) //Cannot change DataSource to a Sorted Combobox
            {
                return;
            }

            string searchText = this.Text;
            var selectionStart = this.SelectionStart;
            //Debug.WriteLine($"OnTextUpdate SearchText: {searchText}, LastFilteringString: {_lastFilterString}");

            _isFiltering = true;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                this.DataSource = _unfilteredItems;
                this.DroppedDown = true;

                this.BeginInvoke(() =>
                {
                    this.Text = string.Empty;
                    this.SelectionStart = selectionStart;
                });
            }
            else
            {
                var filteredList = _unfilteredItems.Cast<object>()
                    .Where(item => this.FilterPredicate == null ? this.DefaultFilter(item, searchText) : this.FilterPredicate(item, searchText))
                    .ToList();

                var listsEquality = false;
                if(this.DataSource is IList dataSourceList)
                {
                    listsEquality = dataSourceList.Cast<object>().SequenceEqual(filteredList);
                }

                if(!listsEquality || !this.DroppedDown)
                {
                    this.DataSource = filteredList;
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
            if(m.Msg == DllImports.WM_MOUSEWHEEL && !this.DroppedDown)
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
                this.DataSource = _unfilteredItems;
                this.SelectedIndex = _unfilteredItems == null || _unfilteredItems.Count == 0 ? -1 : 0;
                this.Text = _lastFilterString;

                _isFiltering = false;

                this.BeginInvoke(() =>
                {
                    var text = this.GetDisplayMember(this.SelectedItem);
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
            if (_unfilteredItems == null || this.Sorted || this.AutoCompleteMode != AutoCompleteMode.None) //Cannot change DataSource to a Sorted Combobox
            {
                base.OnDropDown(e);
                return;
            }

            // Quando si apre la tendina via click o freccia, ripristina la lista completa
            if (!_isFiltering && this.DataSource != _unfilteredItems)
            {
                string currentText = this.Text;
                int selectionStart = this.SelectionStart;

                this.DataSource = _unfilteredItems;

                this.Text = currentText;
                this.SelectionStart = selectionStart;
            }

            base.OnDropDown(e);
        }

        /// <summary>
        /// Algoritmo di filtro di default: cerca tutte le parole digitate all'interno dell'oggetto.
        /// </summary>
        private bool DefaultFilter(object item, string searchText)
        {
            if (item == null)
            {
                return false;
            }

            string itemText = this.GetDisplayMember(item);

            string[] searchWords = searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return searchWords.All(word => itemText.Contains(word, StringComparison.OrdinalIgnoreCase));
        }

        private string GetDisplayMember(object? item)
        {
            if(item == null)
            {
                return "NULL";
            }

            if(string.IsNullOrEmpty(this.DisplayMember))
            {
                return $"{item}";
            }

            var itemProperty = item.GetType().GetProperty(this.DisplayMember);
            if(itemProperty == null)
            {
                return $"{item}";
            }

            var itemValue = itemProperty.GetValue(item, null);
            return itemValue == null ? $"{item}" : $"{itemValue}";
        }
    }
}
