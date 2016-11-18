using FileAnalysis.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysis.v6
{
    public class v6Parser : IParser
    {
        public string Name
        {
            get
            {
                return "v6- Read the file line by line with a StreamReader / internal dictionary for statistics / custom date parsing with custom ticks calculation";
            }
        }

        public void Parse(string inputFileName, string outputFileName)
        {
            StreamReader file = new StreamReader(inputFileName);
            string line;
            Dictionary<string, long> totalDuration = new Dictionary<string, long>();

            while ((line = file.ReadLine()) != null)
            {
                string[] lineItems = line.Split(Constants.Splitter, StringSplitOptions.RemoveEmptyEntries);

                string id = lineItems[2];
                long start = this.ParseDate(lineItems[0]);
                long end = this.ParseDate(lineItems[1]);

                long duration = end - start;

                if (totalDuration.ContainsKey(id))
                    totalDuration[id] += duration;
                else
                    totalDuration.Add(id, duration);
            }

            file.Close();


            using (var output = File.CreateText(outputFileName))
            {
                foreach (KeyValuePair<string, long> entry in totalDuration)
                {
                    output.WriteLine($"{entry.Key:D10} {TimeSpan.FromTicks(entry.Value):c}");
                }
            }
        }

        private long ParseDate(string value)
        {
            //format is yyyy-MM-dd'T'HH:mm:ss

            int year = (int)char.GetNumericValue(value[0]) * 1000 +
                        (int)char.GetNumericValue(value[1]) * 100 +
                        (int)char.GetNumericValue(value[2]) * 10 +
                        (int)char.GetNumericValue(value[3]);

            int month = (int)char.GetNumericValue(value[5]) * 10 +
                        (int)char.GetNumericValue(value[6]);

            int day = (int)char.GetNumericValue(value[8]) * 10 +
                        (int)char.GetNumericValue(value[9]);

            int hour = (int)char.GetNumericValue(value[11]) * 10 +
                        (int)char.GetNumericValue(value[12]);

            int minute = (int)char.GetNumericValue(value[14]) * 10 +
                        (int)char.GetNumericValue(value[15]);

            int second = (int)char.GetNumericValue(value[17]) * 10 +
                        (int)char.GetNumericValue(value[18]);

            return this.DateToTicks(year, month, day) + this.TimeToTicks(hour, minute, second);
        }

        //taken from the System.DateTime class
        private long DateToTicks(int year, int month, int day)
        {
            if (year >= 1 && year <= 9999 && month >= 1 && month <= 12)
            {
                int[] days = DateTime.IsLeapYear(year) ? DaysToMonth366 : DaysToMonth365;
                if (day >= 1 && day <= days[month] - days[month - 1])
                {
                    int y = year - 1;
                    int n = y * 365 + y / 4 - y / 100 + y / 400 + days[month - 1] + day - 1;
                    return n * TicksPerDay;
                }
            }

            throw new ArgumentOutOfRangeException(null, "ArgumentOutOfRange_BadYearMonthDay");
        }

        private long TimeToTicks(int hour, int minute, int second)
        {
            // totalSeconds is bounded by 2^31 * 2^12 + 2^31 * 2^8 + 2^31,
            // which is less than 2^44, meaning we won't overflow totalSeconds.
            long totalSeconds = (long)hour * 3600 + (long)minute * 60 + (long)second;
            if (totalSeconds > MaxSeconds || totalSeconds < MinSeconds)
                throw new ArgumentOutOfRangeException(null, "Overflow_TimeSpanTooLong");
            return totalSeconds * TicksPerSecond;
        }

        private static readonly int[] DaysToMonth365 = {
            0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365};
        private static readonly int[] DaysToMonth366 = {
            0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366};

        private const long TicksPerMillisecond = 10000;
        private const long TicksPerSecond = TicksPerMillisecond * 1000;
        private const long TicksPerMinute = TicksPerSecond * 60;
        private const long TicksPerHour = TicksPerMinute * 60;
        private const long TicksPerDay = TicksPerHour * 24;

        internal const long MaxSeconds = Int64.MaxValue / TicksPerSecond;
        internal const long MinSeconds = Int64.MinValue / TicksPerSecond;
    }
}
