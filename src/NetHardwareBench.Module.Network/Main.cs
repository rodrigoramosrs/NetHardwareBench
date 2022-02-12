using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            base.WriteMessage("== Starting Network Benchmark ==\r\n");
            result.StepsDetails.Add("Starting Network Benchmark");

            result.StartedAt = DateTime.Now;

            result.PartialResults = this.DoInternalBenchmark();
            result.Score = Math.Round(result.PartialResults.Sum(x => x.Score) / result.PartialResults.Count, 2);
            result.FinishedAt = DateTime.Now;
            base.WriteMessage("== Finished Network Benchmark ==\r\n\r\n");
            result.StepsDetails.Add("Finished Network Benchmark");

            return result;
        }

        public override BenchmarkType GetBenchmarkType()
        {
            return BenchmarkType.NETWORK;
        }

        private List<BenchmarkPartialResult> DoInternalBenchmark()
        {
            string remoteAddress = "www.google.com.br";
            List<BenchmarkPartialResult> result = new List<BenchmarkPartialResult>();
            base.WriteMessage("Start Ping to " + remoteAddress);

            TimeSpan timeSpent = new TimeSpan();
            double TotalTime = 0;
            int NumberOfTests = 15;
            for (int i = 0; i < NumberOfTests; i++)
            {
                DoPing(remoteAddress, out timeSpent);
                TotalTime += timeSpent.Milliseconds;

                result.Add(new BenchmarkPartialResult()
                {
                    Description = $"NETWORK - DELAY [{i}]",
                    MetricScale = MetricScaleType.MILLISECOND,
                    ResultType = BenchmarkResultType.PING,
                    Score = timeSpent.Milliseconds
                });
                Thread.Sleep(100);

            }

            
            base.WriteMessage("AVG: " + Math.Round(TotalTime / NumberOfTests ) + "ms");
            
            //result.Score = Math.Round(TotalTime / NumberOfTests, 2);
            return result;
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

                    base.WriteMessage(" Ping " + RemoteIpOrName + " [" + pingResponse.Status.ToString() + "]: " + cronometro.Elapsed.Milliseconds.ToString() + "ms");
                }
                catch (Exception ex)
                {
                    base.WriteMessage(ex.ToString());
                }
            }

            return result;
        }
    }
}
