using FileAnalysis.Base;
using FileAnalysis.v1;
using FileAnalysis.v2;
using FileAnalysis.v3;
using FileAnalysis.v4;
using FileAnalysis.v5;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Specify the parser version : 1, 2, 3, 4, 5");
                return;
            }

            try
            {
                string version = args[0];

                string inputFileName = "input.txt";
                string outputFileName = $"output_{version}.txt";
                if (!File.Exists(inputFileName))
                {
                    //File is here https://1drv.ms/t/s!AoI1E0VC665zg8pnPYn0a9QOj6An5g in case the download link stops working
                    string sampleDateUrl = "https://onedrive.live.com/download?cid=73AEEB4245133582&resid=73AEEB4245133582%2158727&authkey=ALYWFO0dqnkKdBU";

                    Console.WriteLine($"Downloading sample data from {sampleDateUrl} (276MB). Please wait....");
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.DownloadFile(sampleDateUrl, inputFileName);
                    }
                    Console.WriteLine("Download complete. Program will exit. Please restart to execute the parsing.");
                    return;
                }

                if (File.Exists(outputFileName))
                    File.Delete(outputFileName);

                IParser parser = GetParser(version);

                AppDomain.MonitoringIsEnabled = true;
                Stopwatch sp = Stopwatch.StartNew();

                Console.WriteLine();

                Console.WriteLine(parser.Name);
                parser.Parse(inputFileName, outputFileName);

                Console.WriteLine($"Took: {sp.ElapsedMilliseconds:#,#} ms and allocated " +
                      $"{AppDomain.CurrentDomain.MonitoringTotalAllocatedMemorySize / 1024:#,#} kb " +
                      $"with peak working set of {Process.GetCurrentProcess().PeakWorkingSet64 / 1024:#,#} kb");

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Parsing failed");
                Console.WriteLine(ex.ToString());
            }
        }

        private static IParser GetParser(string version)
        {
            switch (version)
            {
                case "1":
                    return new v1Parser();
                case "2":
                    return new v2Parser();
                case "3":
                    return new v3Parser();
                case "4":
                    return new v4Parser();
                case "5":
                    return new v5Parser();
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), $"Unknown parser version {version}");
            }
        }
    }
}
