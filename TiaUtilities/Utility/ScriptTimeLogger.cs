using System.Diagnostics;

namespace TiaXmlReader.Utility
{
    public class ScriptTimeLogger
    {
        private class TimeInfo
        {
            public double TotalMillisecond { get; set; }
        }

        private readonly List<TimeInfo> scriptTimeInfoList;
        private readonly Stopwatch stopwatch;
        private int executions = 0;
        public ScriptTimeLogger()
        {
            this.scriptTimeInfoList = new List<TimeInfo>();
            this.stopwatch = new Stopwatch();
        }

        public void Restart()
        {
            stopwatch.Restart();
        }

        public void StopAndSave()
        {
            stopwatch.Stop();

            executions++;
            scriptTimeInfoList.Add(new TimeInfo()
            {
                TotalMillisecond = stopwatch.Elapsed.TotalMilliseconds,
            });
        }

        public void Log(string script, string classInfo)
        {
            var scriptCalcInfo = new CalcInfo().Calculate(this.scriptTimeInfoList);

            var message = $"JS executed for {classInfo}. Avg[ms]:{scriptCalcInfo.Average:F3}, Dev[ms]:{scriptCalcInfo.Deviation:F3}, Min[ms]:{scriptCalcInfo.Min:F3}, Max[ms]:{scriptCalcInfo.Max:F3}, Count:{executions}\n";
            message += $"{script}";

            LogHandler.INSTANCE.AddMessage(message, "scripts");
        }

        private class CalcInfo
        {
            public double Average { get; set; }
            public double Deviation { get; set; }
            public double Min { get; set; }
            public double Max { get; set; }

            public CalcInfo Calculate(List<TimeInfo> timeInfoList)
            {
                if (timeInfoList.Count == 0)
                {
                    return this;
                }

                Average = timeInfoList.Average(i => i.TotalMillisecond);
                Deviation = Math.Sqrt(timeInfoList.Average(i => Math.Pow(i.TotalMillisecond - Average, 2)));
                Min = timeInfoList.Select(i => i.TotalMillisecond).Min();
                Max = timeInfoList.Select(i => i.TotalMillisecond).Max();

                return this;
            }
        }
    }
}
