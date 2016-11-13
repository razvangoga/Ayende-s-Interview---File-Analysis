using FileAnalysis.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysis.v4
{
    public class v4Parser : IParser
    {
        public string Name
        {
            get
            {
                return "Read the file line by line with a StreamReader / internal dictionary for statistics / parsing optimizations";
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
                DateTime start = DateTime.ParseExact(lineItems[0], "yyyy-MM-dd'T'HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal);
                DateTime end = DateTime.ParseExact(lineItems[1], "yyyy-MM-dd'T'HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal);

                long duration = (end - start).Ticks;

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
    }
}
