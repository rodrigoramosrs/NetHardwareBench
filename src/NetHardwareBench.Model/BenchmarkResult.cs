using System;
using System.Collections.Generic;
using System.Text;

namespace NetHardwareBench.Model
{
    public class BenchmarkResult
    {
        public BenchmarkResult()
        {
            this.StepsDetails = new List<string>();
            this.PartialResults = new List<BenchmarkPartialResult>();
        }

        public List<BenchmarkPartialResult> PartialResults { get; set; }
        public Modules.Types.BenchmarkType BenchmarkType { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }

        public TimeSpan TimeSpend
        {
            get { return FinishedAt - StartedAt; }
        }

        public double Score { get; set; }

        public List<string> StepsDetails { get; set; }
    }

}
