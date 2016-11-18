using FileAnalysis.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysis.v7
{
    public class v7Parser : IParser
    {
        public string Name
        {
            get
            {
                return "v7- Read the file line by line with a StreamReader / internal dictionary for statistics / custom date parsing";
            }
        }

        public void Parse(string inputFileName, string outputFileName)
        {
            StreamReader file = new StreamReader(inputFileName);
            ConcurrentDictionary<string, long> totalDuration = new ConcurrentDictionary<string, long>();

            Parallel.ForEach(File.ReadLines(inputFileName), line =>
            {
                string[] lineItems = line.Split(Constants.Splitter, StringSplitOptions.RemoveEmptyEntries);

                string id = lineItems[2];
                DateTime start = this.ParseDate(lineItems[0]);
                DateTime end = this.ParseDate(lineItems[1]);

                long duration = (end - start).Ticks;

                totalDuration.AddOrUpdate(id, duration, (_, existingDuration) => existingDuration + duration);
            });


            using (var output = File.CreateText(outputFileName))
            {
                foreach (KeyValuePair<string, long> entry in totalDuration)
                {
                    output.WriteLine($"{entry.Key:D10} {TimeSpan.FromTicks(entry.Value):c}");
                }
            }
        }

        private DateTime ParseDate(string value)
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

            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}
