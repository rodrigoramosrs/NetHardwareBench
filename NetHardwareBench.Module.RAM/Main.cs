using System;
using System.Diagnostics;
using NetHardwareBench.Model;
using NetHardwareBench.Model.Modules.Parameters;
using NetHardwareBench.Model.Modules.Types;

namespace NetHardwareBench.Module.RAM
{
    public class Main : Model.Modules.BaseModule
    {
        public Main(ConfigurationParameters ConfigurationParameters) : base(ConfigurationParameters) { }

        public override BenchmarkResult DoBenchmark()
        {
            BenchmarkResult result = new BenchmarkResult();
            Console.WriteLine("== Starting RAM Benchmark ==\r\n");
            result.StepsDetails.Add("Starting RAM Benchmark");

            result.StartedAt = DateTime.Now;

            DoInternalBenchmark();
            result.FinishedAt = DateTime.Now;
            Console.WriteLine("== Finished RAM Benchmark ==\r\n\r\n");
            result.StepsDetails.Add("Finished RAM Benchmark");

            return result;
        }

        public override BenchmarkType GetBenchmarkType()
        {
            return BenchmarkType.RAM;
        }

        private void DoInternalBenchmark()
        {
            runTest();
        }






        public Int64 RAMTotal;
        public Int64 RAMFree;

        public int LastWrite = 0;

        public bool testRunning = false;
        public bool abortTest = false;

        private Int64[] getRAMinfo()
        {
            Int64[] ret = new Int64[2];
            //Int64 totRam = 0;
            //Int64 freeRam = 0;


            var output = "";

            var info = new ProcessStartInfo();
            info.FileName = "wmic";
            info.Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value";
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            using (var process = Process.Start(info))
            {
                output = process.StandardOutput.ReadToEnd();
            }

            var lines = output.Trim().Split('\n');
            var freeMemoryParts = lines[0].Split('=', (char)StringSplitOptions.RemoveEmptyEntries);
            var totalMemoryParts = lines[1].Split('=', (char)StringSplitOptions.RemoveEmptyEntries);

            RAMTotal = long.Parse(totalMemoryParts[1]) * 1024; // Math.Round(/ 1024, 2);
            RAMFree = long.Parse(freeMemoryParts[1]) * 1024;// Math.Round( / 1024, 2);
            ret[0] = RAMTotal;
            ret[1] = RAMFree;
            return ret;


        }

        public string bytesToSize(Int64 inbytes)
        {
            if (inbytes < 1024)
            {
                return inbytes.ToString() + "B";
            }
            else if ((inbytes >= 1024) && (inbytes < 1048576))
            {
                double kbs = inbytes / 1024;
                return Convert.ToString(Math.Round(kbs, 2) + "KB");
            }
            else if (inbytes >= 1048576)
            {
                double mbs = inbytes / 1048576;
                return Convert.ToString(Math.Round(mbs, 2) + "MB");
            }
            else
            {
                return "N/A";
            }
        }




        public void runTest()
        {
            testRunning = true;
            abortTest = false;
            long MegabytesToTest = 1500;

            Int64[] ramInfo = getRAMinfo();
            Int64 autoRam = Convert.ToInt64(Math.Floor((RAMFree * 0.75)));

            System.IO.MemoryStream mem = new System.IO.MemoryStream();


            const int BUFFER_SIZE = 30000;

            //MegabytesToTest = Convert.ToInt32((RAMFree / 1024 ) /1024 );
            long MaximumMemoryToTest = Convert.ToInt64(MegabytesToTest * 1048576);
            long NumberOfIterations = Convert.ToInt64(Math.Round(Convert.ToDouble(MaximumMemoryToTest / BUFFER_SIZE), 0));
            long BytesWritten = 0;
            long BytesLeft = MaximumMemoryToTest;

            //progressBar1.Maximum = runs;
            //progressBar1.Value = 0;
            //progressBar1.Step = 1;

            // speed array for averages
            long[] SpeedArray = new long[NumberOfIterations];//[1000];
            long CurrentSpeedIterationIndex = 0;

            //timer2.Enabled = true;
            DateTime sTime = DateTime.Now;
            DateTime startTime = DateTime.Now;

            for (long i = 0; i <= NumberOfIterations; i++)
            {
                if (abortTest == true)
                {
                    break;
                }
                System.Text.UTF8Encoding str = new System.Text.UTF8Encoding();
                string buf = "";
                buf = buf.PadLeft(BUFFER_SIZE, Convert.ToChar("0"));
                BytesWritten = BytesWritten + BUFFER_SIZE;

                if (BytesLeft > BUFFER_SIZE)
                    BytesLeft = BytesLeft - BUFFER_SIZE;
                else
                    BytesLeft = 0;

                LastWrite = LastWrite + BUFFER_SIZE;
                mem.Write(str.GetBytes(buf), 0, buf.Length);

                //progressBar1.PerformStep();
                
                
                //double perc = Math.Round((Convert.ToDouble(progressBar1.Value) / Convert.ToDouble(progressBar1.Maximum)) * 100, 2);
                //label7.Text = perc.ToString() + "%";
                
                

                DateTime stopTime = DateTime.Now;
                TimeSpan duration = stopTime - startTime;
                if (duration.Milliseconds >= 250)
                {
                    startTime = DateTime.Now;
                    int BytesPerSecond = LastWrite * 4;
                    LastWrite = 0;
                    SpeedArray[CurrentSpeedIterationIndex] = BytesPerSecond;
                    CurrentSpeedIterationIndex++;

                    Console.WriteLine(bytesToSize(BytesWritten) + " written" + " | " + bytesToSize(BytesLeft) + " left | " + bytesToSize(BytesPerSecond) + "/s");
                    //Console.WriteLine(bytesToSize();
                }
            }

            //timer2.Enabled = false;

            mem.Close();

            // get avg
            long avg = 0;
            long z;
            for (z = 0; z < CurrentSpeedIterationIndex; z++)
            {
                avg += SpeedArray[z];
            }
            avg = avg / CurrentSpeedIterationIndex;
            DateTime stTime = DateTime.Now;
            TimeSpan dur = stTime - sTime;

            Console.WriteLine("Test completed in " + dur.Minutes.ToString() + " mins " + dur.Seconds.ToString() + " secs" + Environment.NewLine + Environment.NewLine + "Average RAM speed is " + bytesToSize(avg) + " per second.");

            testRunning = false;
        }

   

        private void timer2_Tick(object sender, EventArgs e)
        {
            int bps = LastWrite * 4;
            LastWrite = 0;
            Console.WriteLine(bytesToSize(bps) + "/s");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            abortTest = true;
        }























    }



























        


















}
