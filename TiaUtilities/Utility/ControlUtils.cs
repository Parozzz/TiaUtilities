using System.Diagnostics;
using System.Reflection;
using TiaUtilities.CustomControls;
using TiaUtilities.Languages;

namespace TiaUtilities.Utility
{
    public static class ControlUtils
    {
        public static void SetDoubleBuffered(Control c)
        {
            ArgumentNullException.ThrowIfNull(c);
            //Taxes: Remote Desktop Connection and painting
            //http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
            if (SystemInformation.TerminalServerSession)
            {
                return;
            }
                
            PropertyInfo? aProp = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            aProp?.SetValue(c, true, null);
        }

        public static ToolTip CreateStandardToolTip()
        {
            return new()
            {
                InitialDelay = 1500,
                ReshowDelay = 800,
                AutomaticDelay = 1000,
                UseFading = false,
                UseAnimation = false,
            };
        }

        public static ToolTip CreateQuickToolTip(bool fading = true)
        {
            return new()
            {
                InitialDelay = 500,
                ReshowDelay = 100,
                AutomaticDelay = 300,
                UseFading = fading,
                UseAnimation = false,
            };
        }

        public static void CreateComboBoxEnumDataSource(ComboBox comboBox, Type enumType, bool editable = false)
        {
            if (!editable)
            {
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList; //Disable text Editing 
            }

            comboBox.DisplayMember = "Text";
            comboBox.ValueMember = "Value";

            var dataSourceList = new List<object>();
            foreach (Enum enumItem in Enum.GetValues(enumType))
            {
                dataSourceList.Add(new { Text = enumItem.GetTranslation(), Value = enumItem });
            }
            comboBox.DataSource = dataSourceList;
        }

        public static void CreateComboBoxObjectDataSource<T>(ComboBox comboBox, List<T> objectList, bool editable = false)
        {
            if (!editable)
            {
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList; //Disable text Editing 
            }

            comboBox.DisplayMember = "Text";
            comboBox.ValueMember = "Value";

            var dataSourceList = new List<object>();
            foreach (var obj in objectList)
            {
                if (obj == null)
                {
                    continue;
                }

                dataSourceList.Add(new { Text = "" + obj, Value = obj });
            }
            comboBox.DataSource = dataSourceList;
        }
        public static void CreateRJComboBoxEnumDataSource(RJComboBox comboBox, Type enumType, bool editable = false)
        {
            if (!editable)
            {
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList; //Disable text Editing 
            }

            comboBox.DisplayMember = "Text";
            comboBox.ValueMember = "Value";

            var dataSourceList = new List<object>();
            foreach (Enum enumItem in Enum.GetValues(enumType))
            {
                dataSourceList.Add(new { Text = enumItem.GetTranslation(), Value = enumItem });
            }
            comboBox.DataSource = dataSourceList;
        }

        public static void CreateRJComboBoxObjectDataSource<T>(RJComboBox comboBox, List<T> objectList, bool editable = false)
        {
            if (!editable)
            {
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList; //Disable text Editing 
            }

            comboBox.DisplayMember = "Text";
            comboBox.ValueMember = "Value";

            var dataSourceList = new List<object>();
            foreach (var obj in objectList)
            {
                if (obj == null)
                {
                    continue;
                }

                dataSourceList.Add(new { Text = "" + obj, Value = obj });
            }
            comboBox.DataSource = dataSourceList;
        }

        public static void SignedKeyPressEventHandler(object? sender, KeyPressEventArgs args)
        {
            if (args.KeyChar == (char)Keys.Cancel || args.KeyChar == (char)Keys.Enter || args.KeyChar == (char)Keys.Back)
            {
                return;
            }

            var isKeyValid = char.IsNumber(args.KeyChar) || args.KeyChar == '+' || args.KeyChar == '-';
            args.Handled = !isKeyValid;
        }

        public static void UnsignedKeyPressEventHandler(object? sender, KeyPressEventArgs args)
        {
            if (args.KeyChar == (char)Keys.Cancel || args.KeyChar == (char)Keys.Enter || args.KeyChar == (char)Keys.Back)
            {
                return;
            }

            var isKeyValid = char.IsNumber(args.KeyChar);
            args.Handled = !isKeyValid;
        }

        private const UInt32 WPARAM_LEFT_SCROLL = 0xff880000;
        private const UInt32 WPARAM_RIGHT_SCROLL = 0x00780000;
        public static int WncProcHorizontalScrollWheel(Message m)
        {
            if (m.Msg == 0x20E) //WM_MOUSEHWHEEL
            {
                if(m.WParam == WPARAM_LEFT_SCROLL)
                {
                    return -1;
                }
                else if(m.WParam == WPARAM_RIGHT_SCROLL)
                {
                    return +1;
                }
            }

            return 0;
        }

    }
}
