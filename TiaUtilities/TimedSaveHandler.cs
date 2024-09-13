using Timer = System.Windows.Forms.Timer;

namespace TiaUtilities
{
    public class TimedSaveHandler()
    {
        private readonly Timer timer = new();

        public void AddTickEventHandler(EventHandler eventHandler)
        {
            timer.Tick += eventHandler;
        }

        public void RemoveTickEventHandler(EventHandler eventHandler)
        {
            timer.Tick -= eventHandler;
        }

        public void Start(int interval)
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
