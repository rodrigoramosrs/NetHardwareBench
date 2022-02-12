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

            BenchmarkCore.OnProgressChanged += BenchmarkCore_OnProgressChanged;

            while (true)
            {
                var benchmarkResult = BenchmarkCore
                    .DoBenchmark(
                    BenchmarkType.NETWORK,
                    BenchmarkType.INTERNET,
                   // BenchmarkType.GPU,
                    BenchmarkType.LOCAL_STORAGE,
                    BenchmarkType.RAM,
                    BenchmarkType.CPU
                    );

                Console.WriteLine("Benchmark Results");
                foreach (var result in benchmarkResult)
                {
                    Console.WriteLine($"[{result.BenchmarkType.ToString()}]\r\n|- AVG: " + result.Score + " | Score: " +
                               ScoreParser.Calculate(result.BenchmarkType, result.Score) + " %");

                    switch (result.BenchmarkType)
                    {
                      
                        case BenchmarkType.INTERNET:
                        case BenchmarkType.LOCAL_STORAGE:

                            foreach (var partialresult in result.PartialResults)
                            {
                                Console.WriteLine($" |- {partialresult.Description}: " + partialresult.Score + " | Score: " +
                                ScoreParser.Calculate(result.BenchmarkType, partialresult.Score) + " %");
                            }
                            break;
                        case BenchmarkType.NETWORK_STORAGE:
                        case BenchmarkType.DATABASE:
                        case BenchmarkType.RAM:
                        case BenchmarkType.NETWORK:
                        case BenchmarkType.CPU:
                        case BenchmarkType.GPU:
                        default:
                            break;
                    }
                    
                }
                Console.ReadKey();
            }

        }

        private static void BenchmarkCore_OnProgressChanged(double progress)
        {
            Console.WriteLine("BenchmarkCore_OnProgressChanged: " + progress);
        }
    }
}
