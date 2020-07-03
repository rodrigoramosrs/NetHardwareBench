using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NetHardwareBench.Model;
using NetHardwareBench.Model.Modules.Parameters;
using NetHardwareBench.Model.Modules.Types;

namespace NetHardwareBench.Module.GPU
{
    public class Main : Model.Modules.BaseModule
    {
        public Main(ConfigurationParameters ConfigurationParameters) : base(ConfigurationParameters) { }

        public override BenchmarkResult DoBenchmark()
        {
            BenchmarkResult result = new BenchmarkResult();
            Console.WriteLine("== Starting GPU Benchmark ==\r\n");
            result.StepsDetails.Add("Starting GPU Benchmark");

            result.StartedAt = DateTime.Now;
            
            result.FinishedAt = DateTime.Now;
            Console.WriteLine("== Finished GPU Benchmark ==\r\n\r\n");
            result.StepsDetails.Add("Finished GPU Benchmark");
            return result;
        }

        public override BenchmarkType GetBenchmarkType()
        {
            return BenchmarkType.GPU;
        }
    }
}
