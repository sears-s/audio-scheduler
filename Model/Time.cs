using System;
using System.Text.RegularExpressions;

namespace AudioScheduler.Model
{
    public class Time : IComparable
    {
        private readonly string _value;

        // Format Time when created
        private Time(string value)
        {
            _value = Format(value);
            if (_value == null)
                App.InfoMessage("Time Format", "Incorrect time format. Should be HHMM, HMM, HH:MM, or H:MM.");
        }

        // Parse Time as an integer
        private int Int => int.Parse(_value.Remove(2, 1));

        // True if Time should be at next day
        public bool NextDay
        {
            get
            {
                if (_value == null) return true;
                return Int < App.NextDayStart.Int;
            }
        }

        // Sort Times correctly
        public int CompareTo(object obj)
        {
            // Check if null
            var other = obj as Time;
            if (_value == null) return 1;
            if (other?._value == null) return -1;

            // Add 24 hours if next day
            var first = Int;
            var second = other.Int;
            if (NextDay) first += 2400;
            if (other.NextDay) second += 2400;

            // Compare as strings
            return first.CompareTo(second);
        }

        public override string ToString()
        {
            return _value;
        }

        public static implicit operator string(Time time)
        {
            return time._value;
        }

        public static implicit operator Time(string value)
        {
            return value == null ? null : new Time(value);
        }

        public static bool operator ==(Time x, Time y)
        {
            return x?._value == y?._value;
        }

        public static bool operator !=(Time x, Time y)
        {
            return x?._value != y?._value;
        }

        // Format Time as HH:MM, returns null if bad time
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