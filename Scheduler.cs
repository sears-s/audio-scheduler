using System;
using System.Runtime.InteropServices;
using System.Timers;
using AudioScheduler.Model;

namespace AudioScheduler
{
    public static class Scheduler
    {
        private const uint EsContinuous = 0x80000000;
        private const uint EsSystemRequired = 0x00000001;
        private const uint EsDisplayRequired = 0x00000002;
        private const uint EsAwayModeRequired = 0x00000040;

        private static Timer _timer;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint SetThreadExecutionState(uint esFlags);

        // Called to start the Scheduler
        public static void Start()
        {
            _timer = new Timer
            {
                AutoReset = false
            };
            _timer.Elapsed += Elapsed;
            Elapsed(null, null);
        }

        // Get interval to next minute
        private static double GetInterval()
        {
            var now = DateTime.Now;
            return ((now.Second > 30 ? 120 : 60) - now.Second) * 1000 - now.Millisecond;
        }

        // Called on each minute
        private static void Elapsed(object sender, ElapsedEventArgs e)
        {
            // Play sound for current minute
            Day.PlayCurrent();

            // Reset compute idle timer
            if (SetThreadExecutionState(EsContinuous | EsSystemRequired | EsDisplayRequired | EsAwayModeRequired) == 0)
                SetThreadExecutionState(EsContinuous | EsSystemRequired | EsDisplayRequired);

            // Wait for next call
            _timer.Interval = GetInterval();
            _timer.Start();
        }
    }
}