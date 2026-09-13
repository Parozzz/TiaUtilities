using System.Collections;
using System.ComponentModel;

namespace TiaUtilities.SettingsStep.CustomControls
{
    public class ComboBoxFilterable : ComboBox
    {
        // Proprietà personalizzabili per i colori di selezione
        public Color DropDownHoveredBackColor { get; set; } = Color.LightSeaGreen; // Colore di sfondo della selezione
        public Color DropDownHoveredForeColor { get; set; } = Color.Black;      // Colore del testo della selezione

        public Func<object, string, bool> FilterPredicate { get; set; }

        private IList? _fullList;
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
            int selectionStart = this.SelectionStart;

            _isFiltering = true;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Ripristina lista completa se l'input è vuoto
                this.DataSource = _fullList;
                this.Text = string.Empty;
                this.DroppedDown = true;
            }
            else
            {
                // Filtra la lista usando il predicato custom
                var filteredList = _fullList.Cast<object>()
                    .Where(item => FilterPredicate(item, searchText))
                    .ToList();

                if (filteredList.Count <= 0)
                {
                    this.DataSource = null;
                }
                else
                {
                    this.DataSource = filteredList;
                    this.Text = searchText;
                    this.SelectionStart = selectionStart;

                    if (filteredList.Count > 0)
                    {
                        this.DroppedDown = true;
                        Cursor.Current = Cursors.Default;
                    }
                    else
                    {
                        this.DroppedDown = false;
                    }
                }



            }

            _isFiltering = false;
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
