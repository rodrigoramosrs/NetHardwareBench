using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using NetHardwareBench.Model;
using NetHardwareBench.Model.Modules.Parameters;
using NetHardwareBench.Model.Modules.Types;

namespace NetHardwareBench.Module.Network
{
    public class Main : Model.Modules.BaseModule
    {
        public Main(ConfigurationParameters ConfigurationParameters) : base(ConfigurationParameters) { }

        public override BenchmarkResult DoBenchmark()
        {
            BenchmarkResult result = new BenchmarkResult();
            Console.WriteLine("== Starting Network Benchmark ==\r\n");
            result.StepsDetails.Add("Starting Network Benchmark");

            result.StartedAt = DateTime.Now;

            this.DoInternalBenchmark();
            result.FinishedAt = DateTime.Now;
            Console.WriteLine("== Finished Network Benchmark ==\r\n\r\n");
            result.StepsDetails.Add("Finished Network Benchmark");

            return result;
        }

        public override BenchmarkType GetBenchmarkType()
        {
            return BenchmarkType.NETWORK;
        }

        private void DoInternalBenchmark()
        {
            string remoteAddress = "www.google.com.br";
            Console.WriteLine("Start Ping to " + remoteAddress);

            TimeSpan timeSpent = new TimeSpan();
            double TotalTime = 0;
            int NumberOfTests = 15;
            for (int i = 0; i < NumberOfTests; i++)
            {
                DoPing(remoteAddress, out timeSpent);
                TotalTime += timeSpent.Milliseconds;
                Thread.Sleep(100);
            }

            Console.WriteLine("AVG: " + Math.Round(TotalTime / NumberOfTests ) + "ms");
        }

        private bool DoPing(string RemoteIpOrName, out TimeSpan TimeSpent)
        {
            TimeSpent = new TimeSpan();
            bool result = false;
            
            using (Ping pinger = new Ping())
            {
                try
                {
                    
                    Stopwatch cronometro = new Stopwatch();
                    cronometro.Start();
                    var pingResponse = pinger.Send(RemoteIpOrName, 1000, new byte[512], new PingOptions() { DontFragment = true });
                    cronometro.Stop();
                    TimeSpent = cronometro.Elapsed;
                    result = pingResponse.Status == IPStatus.Success;

                    Console.WriteLine(" Ping " + RemoteIpOrName + " [" + pingResponse.Status.ToString() + "]: " + cronometro.Elapsed.Milliseconds.ToString() + "ms");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return result;
        }
    }
}
