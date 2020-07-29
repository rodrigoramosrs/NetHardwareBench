using Saplin.StorageSpeedMeter;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetHardwareBench.Model;
using NetHardwareBench.Model.Modules.Parameters;
using NetHardwareBench.Model.Modules.Types;
using System.Linq;

namespace NetHardwareBench.Module.LocalStorage
{
    public class Main : Model.Modules.BaseModule
    {
        public Main(ConfigurationParameters ConfigurationParameters) : base(ConfigurationParameters) { }
        public override BenchmarkType GetBenchmarkType()
        {
            return BenchmarkType.LOCAL_STORAGE;
        }
        public override BenchmarkResult DoBenchmark()
        {
            BenchmarkResult result = new BenchmarkResult();
            base.WriteMessage("== Starting Local Storage Benchmark ==\r\n");
            result.StepsDetails.Add("Starting Local Storage Benchmark");

            result.StartedAt = DateTime.Now;

            result.PartialResults = this.DoInternalBenchmark();
            result.Score = Math.Round(result.PartialResults.Sum( x => x.Score) / result.PartialResults.Count,2);
            //result.StepsDetails.AddRange(testResult.StepsDetails);

            result.FinishedAt = DateTime.Now;
            base.WriteMessage("== Finished Local Storage Benchmark ==\r\n\r\n");
            result.StepsDetails.Add("Finished Local Storage Benchmark");

            return result;

            
        }

        

































        private string PickDrive(long freeSpace)
        {
            var i = 0;
            var k = 0;

            var drives = RamDiskUtil.GetEligibleDrives();
            var driveIndexes = new int[drives.Length];

            foreach (var d in drives)
            {
                try
                {
                    var flag = false;

                    if (d.TotalFreeSpace < freeSpace) flag = true;
                    else
                    {
                        driveIndexes[i] = k;
                        i++;
                    }

                    base.WriteMessage(string.Format(
                        "[{0}] {1} {2:0.00} Gb free {3}",
                            flag ? " " : i.ToString(),
                            d.Name,
                            (double)d.TotalFreeSpace / 1024 / 1024 / 1024,
                            flag ? "- insufficient free space" : ""
                            ));

                    k++;
                }
                catch (Exception ex)
                {
                    //base.WriteMessage(ex);

                }

            }

            Console.Write("- please pick drive to test: ");

            int index;
            do
            {

                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape) return null;

                var input = Console.ReadLine();

                if (!Int32.TryParse(input, out index)) index = -1;
            } while ((index < 1) || (index > i));

            return drives[driveIndexes[--index]].Name;
        }

        public const string unit = "MB/s";

        List<BenchmarkPartialResult> DoInternalBenchmark()
        {
            List<BenchmarkPartialResult> result = new List<BenchmarkPartialResult>();

            base.WriteMessage("STORAGE SPEED TEST\n");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            //base.WriteMessage("Total RAM: {0:0.00}Gb, Available RAM: {1:0.00}Gb\n", (double)RamDiskUtil.TotalRam / 1024 / 1024 / 1024, (double)RamDiskUtil.FreeRam / 1024 / 1024 / 1024);
            WriteLineWordWrap("The test uses standrd OS's file API (WinAPI on Windows and POSIX on Mac/Linux) to measure the speed of transfers between storage device and system memory.\nWrite buffering and mem cahce are disabled\n");
            Console.ResetColor();

            const long fileSize = 1024 * 1024 * 1024;

            //var drivePath = PickDrive(fileSize);

            //if (drivePath == null) return result;

            var drivers = RamDiskUtil.GetEligibleDrives();

            foreach (var driver in drivers)
            {
                try
                {
                    if (driver.TotalFreeSpace < fileSize)
                    {
                        base.WriteMessage($"CANNOT BENCHMARK LOCAL STORAGE SPEED FOR DRIVER [{driver.Name}] - Not enough space");
                    }
                    else
                    {
                        var testSuite = new BigTest(driver.Name, fileSize, false);
                        result.AddRange(DoDiskBenchmark(testSuite, driver.Name));
                    }
                   
                }
                catch (Exception ex)
                {
                    string message = $"Error while test disk [{driver.Name}]";
                    base.WriteMessage(message + ex.ToString());
                }

                
            }

            return result;
        }

        private List<BenchmarkPartialResult> DoDiskBenchmark(BigTest testSuite, string driverName)
        {
            List<BenchmarkPartialResult> result = new List<BenchmarkPartialResult>();
            try
            {
                using (testSuite)
                {

                    base.WriteMessage(string.Format("Test file: {0}, Size: {1:0.00}Gb\n\n Press ESC to break", testSuite.FilePath, (double)testSuite.FileSize / 1024 / 1024 / 1024));

                    string currentTest = null;
                    const int curCursor = 40;
                    var breakTest = false;

                    testSuite.StatusUpdate += (sender, e) =>
                    {
                        if (breakTest) return;
                        if (e.Status == TestStatus.NotStarted) return;

                        if ((sender as Test).DisplayName != currentTest)
                        {
                            currentTest = (sender as Test).DisplayName;
                            Console.Write("\n{0}/{1} {2}", testSuite.CompletedTests + 1, testSuite.TotalTests, (sender as Test).DisplayName);
                        }

                        ClearLine(curCursor);

                        if (e.Status != TestStatus.Completed)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            switch (e.Status)
                            {
                                case TestStatus.Started:
                                    Console.Write("Started");
                                    break;
                                case TestStatus.InitMemBuffer:
                                    Console.Write("Initializing test data in RAM...");
                                    break;
                                case TestStatus.PurgingMemCache:
                                    Console.Write("Purging file cache in RAM...");
                                    break;
                                case TestStatus.WarmigUp:
                                    Console.Write("Warming up...");
                                    break;
                                case TestStatus.Interrupted:
                                    Console.Write("Test interrupted");
                                    break;
                                case TestStatus.Running:
                                    Console.Write("{0}% {2} {1:0.00} MB/s", e.ProgressPercent, e.RecentResult, GetNextAnimation());
                                    break;
                            }
                            Console.ResetColor();
                        }
                        else if ((e.Status == TestStatus.Completed) && (e.Results != null))
                        {
                            Console.Write(
                                string.Format("Avg: {1:0.00}{0}\t",
                                unit,
                                e.Results.AvgThroughput)
                            );

                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write(
                                string.Format(" Min÷Max: {1:0.00} ÷ {2:0.00}, Time: {3}m{4:00}s",
                                unit,
                                e.Results.Min,
                                e.Results.Max,
                                e.ElapsedMs / 1000 / 60,
                                e.ElapsedMs / 1000 % 60)
                            );
                            Console.ResetColor();
                        }

                        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                        {
                            base.WriteMessage("  Stopping...");
                            breakTest = true;
                            testSuite.Break();
                        }

                        ShowCounters(testSuite);
                    };

                    var results = testSuite.Execute();

                    if (!breakTest)
                    {
                        base.WriteMessage(string.Format("\n\nWrite Score*:\t {0:0.00} MB/s", testSuite.WriteScore));
                        base.WriteMessage(string.Format("Read Score*:\t {0:0.00} MB/s", testSuite.ReadScore));
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        base.WriteMessage("*Calculation: average throughput with 80% read/written seqentialy and 20% randomly");
                        Console.ResetColor();
                        base.WriteMessage("\nTest file deleted.");

                        result.Add(new BenchmarkPartialResult()
                        {
                            Description = $"LOCAL STORAGE SPEED - WRITE [{driverName}]",
                            MetricScale = MetricScaleType.MEGABYTE,
                            ResultType = BenchmarkResultType.UPLOAD_SPEED,
                            Score = Math.Round(testSuite.WriteScore, 2)
                        });

                        result.Add(new BenchmarkPartialResult()
                        {
                            Description = $"LOCAL STORAGE SPEED - READ [{driverName}]",
                            MetricScale = MetricScaleType.MEGABYTE,
                            ResultType = BenchmarkResultType.UPLOAD_SPEED,
                            Score = Math.Round(testSuite.ReadScore, 2)
                        });


                        //result.Score = Math.Round((testSuite.WriteScore + testSuite.ReadScore) / 2,2);
                        //result.StepsDetails.Add(string.Format("Write Score*:\t {0:0.00} MB/s", testSuite.WriteScore) + " | " + string.Format("Read Score*:\t {0:0.00} MB/s", testSuite.ReadScore));
                        //base.WriteMessage("\nTest file deleted.  Saving results to CSV files in folder: " + testSuite.ResultsFolderPath);
                        //testSuite.ExportToCsv(testSuite.ResultsFolderPath, true);
                    }
                }
            }
            catch (Exception ex)
            {
                base.WriteMessage("\nProgram interupted due to unexpected error:");
                base.WriteMessage("\t" + ex.GetType() + " " + ex.Message);
                base.WriteMessage(ex.StackTrace);
            }

            /*if (!RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
            {
                base.WriteMessage("\nPress any key to quit");
                Console.ReadKey();
            }*/

            return result;
        }

        private void ClearLine(int cursorLeft)
        {
            Console.CursorLeft = cursorLeft;
            Console.Write(new string(' ', Console.WindowWidth - cursorLeft - 1));
            Console.CursorLeft = cursorLeft;
        }

        char[] anim = new char[] { '/', '|', '\\', '-', '/', '|', '\\', '-' };
        int animCounter = 0;

        private char GetNextAnimation()
        {
            animCounter++;
            return anim[animCounter % anim.Length];
        }

        long prevElapsedSecs = 0;

        private void ShowCounters(TestSuite ts)
        {
            var left = Console.CursorLeft;
            var top = Console.CursorTop;
            var elapsedSecs = ts.ElapsedMs / 1000;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            if (prevElapsedSecs != elapsedSecs)
            {
                var elapsed = string.Format("                          Elapsed: {0:00}m {1:00}s", elapsedSecs / 60, elapsedSecs % 60);
                Console.CursorLeft = Console.WindowWidth - elapsed.Length - 1;
                Console.CursorTop = 0;
                Console.Write(elapsed);

                var remaing = string.Format("                          Remaining: {0:00}m {1:00}s", ts.RemainingMs / 1000 / 60, ts.RemainingMs / 1000 % 60);
                Console.CursorLeft = Console.WindowWidth - remaing.Length - 1;
                Console.CursorTop = 1;
                Console.Write(remaing);

                prevElapsedSecs = elapsedSecs;
            }

            Console.CursorLeft = left;
            Console.CursorTop = top;
            Console.ResetColor();
        }

        public void WriteLineWordWrap(string text, int tabSize = 8)
        {
            string[] lines = text
                .Replace("\t", new String(' ', tabSize))
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                string process = lines[i];
                List<String> wrapped = new List<string>();

                while (process.Length > Console.WindowWidth)
                {
                    int wrapAt = process.LastIndexOf(' ', Math.Min(Console.WindowWidth - 1, process.Length));
                    if (wrapAt <= 0) break;

                    wrapped.Add(process.Substring(0, wrapAt));
                    process = process.Remove(0, wrapAt + 1);
                }

                foreach (string wrap in wrapped)
                {
                    base.WriteMessage(wrap);
                }

                base.WriteMessage(process);
            }
        }




















    }
}
