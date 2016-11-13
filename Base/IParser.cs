using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysis.Base
{
    public interface IParser
    {
        string Name { get; }
        void Parse(string inputFileName, string outputFileName);
    }
}
