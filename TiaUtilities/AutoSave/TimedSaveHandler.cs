using Timer = System.Windows.Forms.Timer;

namespace TiaXmlReader.AutoSave
{
    public class TimedSaveHandler
    {
        public enum TimeEnum
        {
            OFF = 0,
            SEC_30 = 30,
            MIN_1 = 60,
            MIN_2 = 120,
            MIN_5 = 300,
            MIN_10 = 600
        }

        private readonly ProgramSettings programSettings;
        private readonly ComboBox comboBox;
        private readonly Timer timer;

        public TimedSaveHandler(ProgramSettings settings, ComboBox comboBox)
        {
            this.programSettings = settings;
            this.comboBox = comboBox;
            this.timer = new Timer();
        }

        public void AddTickEventHandler(EventHandler eventHandler)
        {
            this.timer.Tick += eventHandler;
        }

        public void RemoveTickEventHandler(EventHandler eventHandler) 
        {
            this.timer.Tick -= eventHandler;
        }

        public void Start()
        {
            this.comboBox.Items.Clear();

            var timeEnumType = typeof(TimeEnum);
            foreach (TimeEnum autoSaveEnum in Enum.GetValues(timeEnumType))
            {
                var enumName = Enum.GetName(timeEnumType, autoSaveEnum);
                this.comboBox.Items.Add(enumName);
            }
            this.comboBox.Text = Enum.GetName(timeEnumType, programSettings.TimedSaveTime);

            SetIntervalAndStart(programSettings.TimedSaveTime);
            this.comboBox.SelectedValueChanged += (sender, args) =>
            {
                timer.Stop();
                if (Enum.TryParse(this.comboBox.Text, out TimeEnum autoSave))
                {
                    programSettings.TimedSaveTime = autoSave;
                    SetIntervalAndStart(autoSave);
                }
            };
        }

        private void SetIntervalAndStart(TimeEnum timeEnum)
        {
            var interval = ((int)timeEnum) * 1000;
            if (interval > 0)
            {
                timer.Interval = interval;
                timer.Start();
            }
        }

    }
}
