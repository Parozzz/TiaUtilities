using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.Utility
{
    public class ScriptTimer
    {
        private class TimeInfo
        {
            public double TotalMillisecond { get; set; }
        }

        private readonly List<TimeInfo> scriptTimeInfoList;
        private readonly Stopwatch stopwatch;
        private int executions = 0;
        public ScriptTimer()
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

            var message = $"JS Script executed for {classInfo}\n";
            message += $"Average[ms]: {scriptCalcInfo.Average}, Deviation[ms]: {scriptCalcInfo.Deviation}, Min[ms]: {scriptCalcInfo.Min}, Max[ms]: {scriptCalcInfo.Max}, Executions: {executions}\n";
            message += $"Script:\n";
            message += $"{script}";

            LogHandler.INSTANCE.AddMessage(message, "scripts");
        }

        private class CalcInfo
        {
            public double Average {  get; set; }
            public double Deviation { get; set; }
            public double Min { get; set; }
            public double Max { get; set; }

            public CalcInfo Calculate(List<TimeInfo> timeInfoList)
            {
                if(timeInfoList.Count == 0)
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
