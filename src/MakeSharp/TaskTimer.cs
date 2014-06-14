using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MakeSharp
{
    public class TaskTimer
    {
        Stopwatch _timer = new Stopwatch();


        private string _current = "";
        public void Start(string task)
        {
            _current = task;
            _timer.Restart();
        }

        public void End()
        {
            _current.MustNotBeEmpty();
            _timer.Stop();
            _durations[_current] = _timer.Elapsed;
        }

        Dictionary<string,TimeSpan> _durations=new Dictionary<string, TimeSpan>();

        public IEnumerable<KeyValuePair<string, TimeSpan>> Durations
        {
            get { return _durations; }
        }

        public TimeSpan TotalDuration
        {
            get { return _durations.Values.Aggregate((t1, t2) => t1.Add(t2)); }
        }

        public void WriteReportToConsole()
        {
if (_durations.Count==0) return;
@"
---------------------------------------------
Build Time Report
---------------------------------------------".ToConsole();
            var longest = Durations.OrderByDescending(t => t.Key.Length).First().Key.Length;
            longest = Math.Max("Name".Length, longest);
            Console.WriteLine("{0,-" + longest + "}  {1}", "Name", "Duration");
            Console.WriteLine("{0,-" + longest + "}  {1}", "----", "--------");
            foreach (var time in Durations)
            {
                Console.WriteLine("{0,-" + longest + "}  {1}", time.Key, time.Value.ToString());
            }
            var old = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            "---------------------------------------------".ToConsole();
            Console.WriteLine("{0}  {1}", "Total:".PadRight(longest), TotalDuration);
            Console.ForegroundColor = old;
            Console.WriteLine(); 
        }       
    }
}