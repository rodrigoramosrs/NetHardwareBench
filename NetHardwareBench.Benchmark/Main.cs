    using System;
using System.Collections.Generic;
using NetHardwareBench.Model;
using NetHardwareBench.Model.Modules.Parameters;

namespace NetHardwareBench.Benchmark
{
    public  class Main
    {
        private readonly ConfigurationParameters _ConfigurationParameters;
        public Main(Model.Modules.Parameters.ConfigurationParameters ConfigurationParameters)
        {
            _ConfigurationParameters = ConfigurationParameters;
        }

        public List<BenchmarkResult> DoBenchmark(params Model.Modules.Types.BenchmarkType[] BenchmarkTypes)
        {
            if (BenchmarkTypes == null) throw new Exception("BenchmarkType could not be empty");

            List<BenchmarkResult> BenchmarkResultList = new List<BenchmarkResult>();
            foreach (var benchmarkType in BenchmarkTypes)
            {

                var benchmarkModule = Factory.Main.GetModuleByType(benchmarkType, this._ConfigurationParameters);
                var benchmarkResultPartial = benchmarkModule.DoBenchmark();

                BenchmarkResultList.Add(benchmarkResultPartial);
            }

            return BenchmarkResultList;
        }
    }
}
