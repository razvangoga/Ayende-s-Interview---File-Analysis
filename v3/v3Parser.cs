using FileAnalysis.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysis.v3
{
    public class v3Parser : IParser
    {
        public string Name
        {
            get
            {
                return "Read the file line by line with a StreamReader / internal dictionary for statistics / default .net parsing for values";
            }
        }

        public void Parse(string inputFileName, string outputFileName)
        {
            StreamReader file = new StreamReader(inputFileName);
            string line;
            Dictionary<long, long> totalDuration = new Dictionary<long, long>();

            while ((line = file.ReadLine()) != null)
            {
                string[] lineItems = line.Split(Constants.Splitter, StringSplitOptions.RemoveEmptyEntries);

                long id = long.Parse(lineItems[2]);
                DateTime start = DateTime.Parse(lineItems[0]);
                DateTime end = DateTime.Parse(lineItems[1]);

                long duration = (end - start).Ticks;

                if (totalDuration.ContainsKey(id))
                    totalDuration[id] += duration;
                else
                    totalDuration.Add(id, duration);
            }

            file.Close();


            using (var output = File.CreateText(outputFileName))
            {
                foreach (KeyValuePair<long, long> entry in totalDuration)
                {
                    output.WriteLine($"{entry.Key:D10} {entry.Value:c}");
                }
            }
        }
    }
}
