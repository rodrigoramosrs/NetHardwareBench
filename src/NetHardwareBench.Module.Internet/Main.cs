using SpeedTest;
using SpeedTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using NetHardwareBench.Model;
using NetHardwareBench.Model.Modules.Parameters;
using NetHardwareBench.Model.Modules.Types;

namespace NetHardwareBench.Module.Internet
{
    //Referencia: https://github.com/JoyMoe/SpeedTest.Net
    public class Main : Model.Modules.BaseModule
    {
        public override BenchmarkType GetBenchmarkType()
        {
            return BenchmarkType.INTERNET;
        }
        public Main(ConfigurationParameters ConfigurationParameters) : base(ConfigurationParameters) { }

        public override BenchmarkResult DoBenchmark()
        {
            BenchmarkResult result = new BenchmarkResult();
            base.WriteMessage("== Starting Internet Benchmark ==\r\n");
            result.StepsDetails.Add("Starting Internet Benchmark");
            base.WriteMessage("");
            result.StartedAt = DateTime.Now;
            result.PartialResults = DoInternalBenchmark();
            result.FinishedAt = DateTime.Now;

            result.Score = result.PartialResults.Sum(x => x.Score) / result.PartialResults.Count;

            base.WriteMessage("== Finished Internet Benchmark ==\r\n\r\n");
            result.StepsDetails.Add("Finished Internet Benchmark");
            return result;
        }



        private SpeedTestClient client;
        private Settings settings;

        List<BenchmarkPartialResult> DoInternalBenchmark()
        {
            var result = new List<BenchmarkPartialResult>();
            base.WriteMessage("Getting speedtest.net settings and server list...");
            client = new SpeedTestClient();
            settings = client.GetSettings();

            var servers = SelectServers();
            var bestServer = SelectBestServer(servers);

            base.WriteMessage("Testing speed...");
            var downloadSpeed = client.TestDownloadSpeed(bestServer, settings.Download.ThreadsPerUrl);
            PrintSpeed("Download", downloadSpeed);
            var uploadSpeed = client.TestUploadSpeed(bestServer, settings.Upload.ThreadsPerUrl);
            PrintSpeed("Upload", uploadSpeed);

            result.Add(new BenchmarkPartialResult() 
            {
                Description = "UPLOAD",
                ResultType = BenchmarkResultType.UPLOAD_SPEED,
                Score = Math.Round(uploadSpeed / 1024,2),
                MetricScale = MetricScaleType.MEGABYTE
            } );
            result.Add(new BenchmarkPartialResult()
            {
                Description = "DOWNLOAD",
                ResultType = BenchmarkResultType.DOWNLOAD_SPEED,
                Score = Math.Round(downloadSpeed / 1024,2),
                MetricScale = MetricScaleType.MEGABYTE
            });
            //result.Score = Math.Round((downloadSpeed + uploadSpeed) / 1024, 2);
            return result;
        }

        private Server SelectBestServer(IEnumerable<Server> servers)
        {
            base.WriteMessage();
            base.WriteMessage("Best server by latency:");
            var bestServer = servers.OrderBy(x => x.Latency).First();
            PrintServerDetails(bestServer);
            base.WriteMessage();
            return bestServer;
        }

        private IEnumerable<Server> SelectServers()
        {
            base.WriteMessage();
            base.WriteMessage("Selecting best server by distance...");
            var servers = settings.Servers.Take(10).ToList();

            foreach (var server in servers)
            {
                server.Latency = client.TestServerLatency(server);
                PrintServerDetails(server);
            }
            return servers;
        }

        private void PrintServerDetails(Server server)
        {
            base.WriteMessage(string.Format("Hosted by {0} ({1}/{2}), distance: {3}km, latency: {4}ms", server.Sponsor, server.Name,
                server.Country, (int)server.Distance / 1000, server.Latency));
        }

        private void PrintSpeed(string type, double speed)
        {
            if (speed > 1024)
            {
                base.WriteMessage(string.Format("{0} speed: {1} Mbps", type, Math.Round(speed / 1024, 2)));
            }
            else
            {
                base.WriteMessage(string.Format("{0} speed: {1} Kbps", type, Math.Round(speed, 2)));
            }
        }

    }
}
