using Timer = System.Windows.Forms.Timer;

namespace TiaXmlReader.AutoSave
{
    public class TimedSaveHandler(ProgramSettings settings)
    {
        private readonly ProgramSettings programSettings = settings;
        private readonly Timer timer = new();

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
            SetIntervalAndStart(programSettings.TimedSaveTime);
        }

        public void SetIntervalAndStart(int interval)
        {
            timer.Stop();
            if (interval > 0)
            {
                timer.Interval = interval;
                timer.Start();
            }
        }

    }
}
