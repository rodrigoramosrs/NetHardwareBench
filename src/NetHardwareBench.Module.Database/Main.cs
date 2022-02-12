using System;
using NetHardwareBench.Model;
using NetHardwareBench.Model.Modules.Parameters;
using NetHardwareBench.Model.Modules.Types;

namespace NetHardwareBench.Module.Database
{
    public class Main : Model.Modules.BaseModule
    {
        public Main(ConfigurationParameters ConfigurationParameters) : base(ConfigurationParameters) { }

        public override BenchmarkResult DoBenchmark()
        {
            throw new NotImplementedException();
        }

        public override BenchmarkType GetBenchmarkType()
        {
            return BenchmarkType.GPU;
        }
    }
}
