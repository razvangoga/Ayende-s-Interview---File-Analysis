using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysis.v1
{
    public class Record
    {
        public DateTime Start => DateTime.Parse(_line.Split(' ')[0]);

        public DateTime End => DateTime.Parse(_line.Split(' ')[1]);
        public long Id => long.Parse(_line.Split(' ')[2]);

        public TimeSpan Duration => End - Start;

        private readonly string _line;

        public Record(string line)
        {
            _line = line;
        }
    }
}
