using System;
using System.Timers;
using AudioScheduler.Model;

namespace AudioScheduler
{
    public static class Scheduler
    {
        private static Timer _timer;

        public static void Start()
        {
            _timer = new Timer
            {
                AutoReset = false
            };
            _timer.Elapsed += Elapsed;
            Elapsed(null, null);
        }

        private static double GetInterval()
        {
            var now = DateTime.Now;
            return ((now.Second > 30 ? 120 : 60) - now.Second) * 1000 - now.Millisecond;
        }

        private static void Elapsed(object sender, ElapsedEventArgs e)
        {
            Day.PlayCurrent();
            _timer.Interval = GetInterval();
            _timer.Start();
        }
    }
}