using System;
using System.Text.RegularExpressions;

namespace AudioScheduler.Model
{
    public class Time : IComparable
    {
        private readonly string _value;

        private Time(string value)
        {
            _value = Format(value);
            if (_value == null)
                App.InfoMessage("Time Format", "Incorrect time format. Should be HHMM, HMM, HH:MM, or H:MM.");
        }

        private int Int => int.Parse(_value.Remove(2, 1));

        private bool NextDay
        {
            get
            {
                if (_value == null) return true;
                return Int < App.NextDayTimeInt;
            }
        }

        public TimeSpan TimeSpan
        {
            get
            {
                if (_value == null) return new TimeSpan();

                var split = _value.Split(':');
                var hours = int.Parse(split[0]);
                var minutes = int.Parse(split[1]);

                return NextDay ? new TimeSpan(hours + 24, minutes, 0) : new TimeSpan(hours, minutes, 0);
            }
        }

        public int CompareTo(object obj)
        {
            var other = obj as Time;
            if (_value == null) return 1;
            if (other?._value == null) return -1;

            var first = Int;
            var second = other.Int;
            if (NextDay) first += 2400;
            if (other.NextDay) second += 2400;

            return first.CompareTo(second);
        }

        public static implicit operator string(Time time)
        {
            return time._value;
        }

        public static implicit operator Time(string value)
        {
            return value == null ? null : new Time(value);
        }

        private static string Format(string input)
        {
            // Check if null string or too short
            if (input == null || input.Length < 3) return null;

            // Prepend 0 if length of 3
            if (input.Length == 3 || input.Length == 4 && input.Contains(":")) input = input.Insert(0, "0");

            // Add colon if not already there
            if (!input.Contains(":")) input = input.Insert(2, ":");

            // Check if valid time via regex
            var regex = new Regex(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");
            return regex.Match(input).Success ? input : null;
        }
    }
}