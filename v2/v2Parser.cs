using FileAnalysis.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysis.v2
{
    public class v2Parser : IParser
    {
        public string Name
        {
            get
            {
                return "Same parser logic - optimized the string usages in the Record class";
            }
        }

        public void Parse(string inputFileName, string outputFileName)
        {
            var summary = from line in File.ReadAllLines(inputFileName)
                          let record = new Record(line)
                          group record by record.Id
                          into g
                          select new
                          {
                              Id = g.Key,
                              Duration = TimeSpan.FromTicks(g.Sum(r => r.Duration.Ticks))
                          };

            using (var output = File.CreateText(outputFileName))
            {
                foreach (var entry in summary)
                {
                    output.WriteLine($"{entry.Id:D10} {entry.Duration:c}");
                }
            }
        }
    }
}
