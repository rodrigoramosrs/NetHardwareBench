using System;
using NetHardwareBench.Model.Modules.Types;

namespace NetHardwareBench.App.Commons
{
    public static class EntryPoint
    {
        public static void Main(string[] args)
        {
            NetHardwareBench.Benchmark.Main BenchmarkCore = new Benchmark.Main(new Model.Modules.Parameters.ConfigurationParameters()
            {
                NetworkPath = "",
                RemoteEndpoint = "",
                RemoteIP = "",
                WorkDirectory = ""
            });


            while (true)
            {
                var benchmarkResult = BenchmarkCore
                    .DoBenchmark(
                    BenchmarkType.NETWORK,
                    BenchmarkType.INTERNET,
                    BenchmarkType.GPU,
                    BenchmarkType.LOCAL_STORAGE,
                    BenchmarkType.RAM,
                    BenchmarkType.CPU);
                Console.ReadKey();
            }

        }
    }
}
