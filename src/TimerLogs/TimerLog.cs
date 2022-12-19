using System;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Utility.Data.Tables;

namespace Utility.Logging.TimerLogs
{
    public class TimerLog
    {
        Stopwatch full = new Stopwatch();
        Stopwatch stage = new Stopwatch();
        LinkedList<(string item, TimeSpan timeTook, bool completed)> log = new LinkedList<(string, TimeSpan, bool)>();

        string currentStage;
        private bool _running;
        public bool running 
        {
            get 
            {
                return _running;
            }
        }

        public TimerLog()
        {
            Init();
        }

        public void Init()
        {
            log.Clear();
            currentStage = null;
            full.Reset();
            stage.Reset();
            _running = true;
            full.Start();
        }

        public void StartStage(string stageName)
        {
            if (!running) { throw new Exception("Timer log not running"); }
            CompleteStage();
            currentStage = stageName;
            stage.Restart();
        }

        public void CompleteStage()
        {
            if (!running) { throw new Exception("Timer log not running"); }
            EndStage(true);
        }

        public void End()
        {
            if (!running) { throw new Exception("Timer log not running"); }
            EndStage(false);
            full.Stop();
            _running = false;
        }

        private void EndStage(bool wasCompleted)
        {
            stage.Stop();
            if(currentStage != null)
            {
                log.AddLast((currentStage, stage.Elapsed, wasCompleted));
            }
            currentStage = null;
        }

        public  Table GetTable()
        {
            if (running) { throw new Exception("Timer log still running"); }
            var table = new Table(new List<string>{"Stage", "Time Took", "Completed?"});

            foreach (var e in log)
            {
                var row = new List<string>();

                row.Add(e.item);
                row.Add(TsFormat(e.timeTook));
                if (e.completed) { row.Add("Completed"); }
                else { row.Add("Not Completed"); }

                table.Add(row);
            }

            return table;
        }

        public string GetHeader()
        {
            if (running) { throw new Exception("Timer log still running"); }
            return $"Total time: {TsFormat(full.Elapsed)}";
        }

        override public string ToString()
        {   
            if (running) { throw new Exception("Timer log still running"); }

            return (GetTable()).Display();
        }

        static string TsFormat(TimeSpan ts)
        {
            return $"{ts.Hours.ToString("D" + 2)}:{ts.Minutes.ToString("D" + 2)}:{ts.Seconds.ToString("D" + 2)}.{ts.Milliseconds.ToString("D" + 3)}";
        }
    }
}
