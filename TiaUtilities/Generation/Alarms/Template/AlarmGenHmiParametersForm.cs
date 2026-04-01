using Newtonsoft.Json;
using TiaUtilities.Constants;
using TiaUtilities.Generation.Alarms.Xml;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Languages;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.Generation.Alarms.Module.Template
{
    public partial class AlarmGenHmiParametersForm : Form
    {
        public class ParameterJsonObject
        {
            [JsonProperty] public Dictionary<uint, AlarmXmlHmiParameter> Dict { get; set; } = [];
        }

        private const int WM_NCACTIVATE = 0x86;

        public bool CloseOnOutsideClick { get; set; } = true;

        private readonly List<RadioButton> selectParametersRadioButtons;
        private uint selectedParameter;

        private readonly AlarmGenHmiParametersForm.ParameterJsonObject parameterJsonObject;

        private bool loadingControls = false;

        public AlarmGenHmiParametersForm(string jsonString)
        {
            InitializeComponent();

            this.selectParametersRadioButtons = [];
            if (!string.IsNullOrEmpty(jsonString))
            {
                try
                {
                    var deserializedItemDictionary = JsonConvert.DeserializeObject<AlarmGenHmiParametersForm.ParameterJsonObject>(jsonString);
                    if (deserializedItemDictionary != null)
                    {
                        this.parameterJsonObject = deserializedItemDictionary;
                    }
                }
                catch { }
            }

            this.parameterJsonObject ??= new();

            Init();
        }

        private void Init()
        {
            Utils.CreateComboBoxEnumDataSource(this.displayTypeComboBox, typeof(AlarmXmlHmiParameter.DisplayTypeEnum), editable: false);
            this.displayTypeComboBox.SelectedValueChanged += (sender, args) =>
            {
                UpdateControlEnabledFromDisplayType();

                if (!loadingControls && this.displayTypeComboBox.SelectedValue is AlarmXmlHmiParameter.DisplayTypeEnum displayType)
                {
                    this.GetSelectedOrCreateParameter().DisplayType = displayType;
                }
            };

            Utils.CreateComboBoxEnumDataSource(this.alignmentComboBox, typeof(AlarmXmlHmiParameter.AlignmentEnum), editable: false);
            this.alignmentComboBox.SelectedValueChanged += (sender, args) =>
            {
                if (!loadingControls && this.alignmentComboBox.SelectedValue is AlarmXmlHmiParameter.AlignmentEnum alignment)
                {
                    this.GetSelectedOrCreateParameter().Alignment = alignment;
                }
            };

            this.tagTextBox.TextChanged += (sender, args) =>
            {
                if(!loadingControls)
                {
                    this.GetSelectedOrCreateParameter().Tag = this.tagTextBox.Text;
                }
            };

            this.textListTextBox.TextChanged += (sender, args) =>
            {
                if (!loadingControls)
                {
                    this.GetSelectedOrCreateParameter().TextList = this.textListTextBox.Text;
                }
            };

            this.lengthTextBox.KeyPress += Utils.UnsignedKeyPressEventHandler;
            this.lengthTextBox.TextChanged += (sender, args) =>
            {
                if (!loadingControls && int.TryParse(this.lengthTextBox.Text, out var length))
                {
                    this.GetSelectedOrCreateParameter().Length = length;
                }
            };

            this.precisionTextBox.KeyPress += Utils.UnsignedKeyPressEventHandler;
            this.precisionTextBox.TextChanged += (sender, args) =>
            {
                if (!loadingControls && int.TryParse(this.precisionTextBox.Text, out var precision))
                {
                    this.GetSelectedOrCreateParameter().Precision = precision;
                }
            };

            this.zeroPaddingsCheckBox.CheckedChanged += (sender, args) =>
            {
                if (!loadingControls)
                {
                    this.GetSelectedOrCreateParameter().ZeroPadding = this.zeroPaddingsCheckBox.Checked;
                }
            };

            this.selectParametersRadioButtons.AddRange([
                selectParam1, selectParam2, selectParam3, selectParam4, selectParam5,
                selectParam6, selectParam7, selectParam8, selectParam9, selectParam10
            ]);

            foreach (var radioButton in this.selectParametersRadioButtons)
            {
                if (uint.TryParse(radioButton.Text, out var number))
                {
                    var toolTip = $"{GenPlaceholders.Alarms.HMI_PARAMETER.Replace("x", number.ToString())}";
                    Utils.CreateStandardToolTip().SetToolTip(radioButton, toolTip);
                }

                radioButton.CheckedChanged += (sender, args) =>
                {
                    if (uint.TryParse(radioButton.Text, out var number) && this.selectedParameter != number)
                    {
                        this.selectParametersRadioButtons
                            .Where(rb => rb != radioButton)
                            .ForEach(rb => rb.Checked = false);

                        this.selectedParameter = number;
                        LoadSelectedParameter();
                    }
                };
            }

            this.selectedParameter = 1;
            this.selectParam1.Checked = true;
            this.UpdateAll();

            this.Shown += (sender, args) => formReadyToClose = true;

            this.Translate();
        }

        private void Translate()
        {
            this.tagLabel.Text = Locale.ALARM_TEMPLATE_HMI_PARAM_TAG;
            this.displayTypeLabel.Text = Locale.ALARM_TEMPLATE_HMI_PARAM_DISPLAY_TYPE;
            this.textListLabel.Text = Locale.ALARM_TEMPLATE_HMI_PARAM_TEXT_LIST;
            this.lengthLabel.Text = Locale.ALARM_TEMPLATE_HMI_PARAM_LENGTH;
            this.precisionLabel.Text = Locale.ALARM_TEMPLATE_HMI_PARAM_PRECISION;
            this.alignmentLabel.Text = Locale.ALARM_TEMPLATE_HMI_PARAM_ALIGNMENT;
            this.zeroPaddingsLabel.Text = Locale.ALARM_TEMPLATE_HMI_PARAM_ZERO_PADDING;
        }

        public string GetJsonSerializedItems()
        {
            try
            {
                return JsonConvert.SerializeObject(this.parameterJsonObject, Formatting.None);
            }
            catch
            {
                return "";
            }
        }

        private void LoadSelectedParameter()
        {
            loadingControls = true;

            var hmiParameter = this.GetSelectedParameter() ?? new();  //So it loads default values
            this.tagTextBox.Text = hmiParameter.Tag;
            this.displayTypeComboBox.SelectedValue = hmiParameter.DisplayType;
            this.textListTextBox.Text = hmiParameter.TextList;
            this.lengthTextBox.Text = "" + hmiParameter.Length;
            this.precisionTextBox.Text = "" + hmiParameter.Precision;
            this.alignmentComboBox.SelectedValue = hmiParameter.Alignment;
            this.zeroPaddingsCheckBox.Checked = hmiParameter.ZeroPadding;

            loadingControls = false;
        }

        private void UpdateControlEnabledFromDisplayType()
        {
            if (this.displayTypeComboBox.SelectedValue is AlarmXmlHmiParameter.DisplayTypeEnum displayType)
            {
                if (displayType == AlarmXmlHmiParameter.DisplayTypeEnum.TEXT_LIST)
                {
                    this.textListLabel.Enabled = this.textListTextBox.Enabled = true;

                    this.lengthLabel.Enabled = this.lengthTextBox.Enabled = false;
                    this.precisionLabel.Enabled = this.precisionTextBox.Enabled = false;
                    this.zeroPaddingsLabel.Enabled = this.zeroPaddingsCheckBox.Enabled = false;
                }
                else
                {
                    this.textListLabel.Enabled = this.textListTextBox.Enabled = false;

                    this.lengthLabel.Enabled = this.lengthTextBox.Enabled = true;
                    this.precisionLabel.Enabled = this.precisionTextBox.Enabled = true;
                    this.zeroPaddingsLabel.Enabled = this.zeroPaddingsCheckBox.Enabled = true;
                }
            }
        }

        private void UpdateRadioButtonsColors()
        {
            foreach(var radioButton in this.selectParametersRadioButtons)
            {
                radioButton.ForeColor = SystemColors.ControlText;
            }

            foreach(var entry in this.parameterJsonObject.Dict)
            {
                var radioButton = this.selectParametersRadioButtons[(int) entry.Key - 1];
                radioButton.ForeColor = SystemColors.HotTrack;
            }
        }

        private void UpdateAll()
        {
            this.LoadSelectedParameter();
            this.UpdateControlEnabledFromDisplayType();
            this.UpdateRadioButtonsColors();
        }

        private AlarmXmlHmiParameter? GetSelectedParameter()
        {
            return this.parameterJsonObject.Dict.TryGetValue(this.selectedParameter, out var parameterValue) ? parameterValue : null;
        }

        private AlarmXmlHmiParameter GetSelectedOrCreateParameter()
        {
            var hmiParameter = GetSelectedParameter();
            if (hmiParameter == null)
            {
                hmiParameter = new();
                this.parameterJsonObject.Dict.Add(this.selectedParameter, hmiParameter);

                this.UpdateRadioButtonsColors();
            }

            return hmiParameter;
        }

        private void SetSelectedParameter(AlarmXmlHmiParameter parameter)
        {
            if(!parameterJsonObject.Dict.TryAdd(this.selectedParameter, parameter))
            {
                this.parameterJsonObject.Dict[this.selectedParameter] = parameter;
            }
        }

        private bool formReadyToClose = false;
        protected override void WndProc(ref Message m)
        {
            try
            {
                // if click outside dialog -> Close Dlg
                //CanFocus is false if there is a modal window open to avoid closing for cliking it (Like color dialog or file dialog)
                if (CloseOnOutsideClick && formReadyToClose && m.Msg == WM_NCACTIVATE && this.CanFocus && !this.RectangleToScreen(this.DisplayRectangle).Contains(Cursor.Position))
                {
                    this.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            base.WndProc(ref m);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                switch (keyData)
                {
                    case Keys.V | Keys.Control:
                        if(this.ActiveControl is not TextBox)
                        {
                            if (Clipboard.GetData(ClipboardConstants.ALARM_XLM_HMI_PARAMETER_FORMAT) is string stringDataObject)
                            {
                                var deserializedObject = JsonConvert.DeserializeObject<AlarmXmlHmiParameter>(stringDataObject);
                                if (deserializedObject != null)
                                {
                                    this.SetSelectedParameter(deserializedObject);
                                    this.UpdateAll();
                                }
                            }
                            return true;
                        }
                        break;

                    case Keys.C | Keys.Control:
                        if(this.ActiveControl is not TextBox)
                        {
                            Clipboard.Clear();

                            var selected = this.GetSelectedParameter();
                            if (selected != null)
                            {
                                var serializedSelected = JsonConvert.SerializeObject(selected, Formatting.None);
                                Clipboard.SetData(ClipboardConstants.ALARM_XLM_HMI_PARAMETER_FORMAT, serializedSelected);
                            }
                            return true;
                        }
                        break;

                    case Keys.Enter:
                    case Keys.Escape:
                        this.Close();
                        return true;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
