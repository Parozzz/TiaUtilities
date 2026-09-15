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

        public Func<object, string, bool> FilterPredicate { get; set; }

        private IList? _fullList;
        private string? _lastFilterString;
        private bool _isFiltering;

        public ComboBoxFilterable()
        {
            this.DoubleBuffered = true;

            this.AutoCompleteMode = AutoCompleteMode.None;
            this.FilterPredicate = DefaultFilter;

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

            _fullList = source.ToList();
            this.DataSource = _fullList;
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

            if (_fullList == null || this.Sorted || this.AutoCompleteMode != AutoCompleteMode.None) //Cannot change DataSource to a Sorted Combobox
            {
                return;
            }

            string searchText = this.Text;
            var selectionStart = this.SelectionStart;
            //Debug.WriteLine($"OnTextUpdate SearchText: {searchText}, LastFilteringString: {_lastFilterString}");

            _isFiltering = true;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                this.DataSource = _fullList;
                this.DroppedDown = true;

                this.BeginInvoke(() =>
                {
                    this.Text = string.Empty;
                    this.SelectionStart = selectionStart;
                });
            }
            else
            {
                var filteredList = _fullList.Cast<object>()
                    .Where(item => FilterPredicate(item, searchText))
                    .ToList();

                var listsEquality = false;
                if(this.DataSource is IList dataSourceList)
                {
                    listsEquality = dataSourceList.Cast<object>().SequenceEqual(filteredList);
                }

                if(!listsEquality)
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
            // Se perde il focus ed è presente una ricerca senza risultati (Items.Count == 0)
            /*
            if (m.Msg == DllImports.WM_KILLFOCUS && _fullList != null && this.Items.Count == 0)
            {
                // Ripristina la lista completa prima che la classe base provi a leggere SelectedItem
                this.DataSource = _fullList;
                this.SelectedIndex = -1;
                this.Text = _lastFilterString;
            }
            */
            try
            {
                base.WndProc(ref m);
            }
            catch (ArgumentOutOfRangeException)
            {
                // Paracadute finale per intercettare l'eccezione nativa di WinForms 
                // nel caso in cui get_SelectedItem() venga chiamato internamente.
                this.DataSource = _fullList;
                this.SelectedIndex = _fullList == null || _fullList.Count == 0 ? -1 : 0;
                this.Text = _lastFilterString;
            }
        }

        protected override void OnValidating(CancelEventArgs e)
        {
            if (_fullList != null && this.Items.Count == 0)
            {
                // Se si esce dal controllo con una ricerca senza risultati, 
                // ripristina la lista originaria e pulisce il testo o deseleziona.
                this.DataSource = _fullList;
                this.SelectedIndex = -1;
                this.Text = string.Empty;
            }

            base.OnValidating(e);
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
            if (_fullList == null || this.Sorted || this.AutoCompleteMode != AutoCompleteMode.None) //Cannot change DataSource to a Sorted Combobox
            {
                base.OnDropDown(e);
                return;
            }

            // Quando si apre la tendina via click o freccia, ripristina la lista completa
            if (!_isFiltering && this.DataSource != _fullList)
            {
                string currentText = this.Text;
                int selectionStart = this.SelectionStart;

                this.DataSource = _fullList;

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

            // Estrae il testo da confrontare
            string itemText = string.IsNullOrEmpty(this.DisplayMember)
                ? item.ToString()
                : item.GetType().GetProperty(this.DisplayMember)?.GetValue(item, null)?.ToString() ?? item.ToString();

            // Controlla che tutte le parole cercate siano presenti
            string[] searchWords = searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return searchWords.All(word => itemText.Contains(word, StringComparison.OrdinalIgnoreCase));
        }
    }
}
