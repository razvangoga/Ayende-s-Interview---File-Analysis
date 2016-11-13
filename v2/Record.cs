using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysis.v2
{
    public class Record
    {
        public static readonly char[] Splitter = new char[] { ' ' };

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
        public long Id { get; set; }

        public TimeSpan Duration { get; set; }

        public Record(string line)
        {
            string[] lineItems = line.Split(Splitter, StringSplitOptions.RemoveEmptyEntries);

            this.Start = DateTime.Parse(lineItems[0]);
            this.End = DateTime.Parse(lineItems[1]);
            this.Duration = this.End - this.Start;

            this.Id = long.Parse(lineItems[2]);
        }
    }
}
