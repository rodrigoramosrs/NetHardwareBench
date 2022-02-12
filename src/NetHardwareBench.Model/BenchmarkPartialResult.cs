using NetHardwareBench.Model.Modules.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetHardwareBench.Model
{
    public class BenchmarkPartialResult
    {
        public BenchmarkPartialResult() { }

        public string Description { get; set; }
        public BenchmarkResultType ResultType { get; set; }
        
        public double Score { get; set; }
        public MetricScaleType MetricScale { get; set; }
    }

}
