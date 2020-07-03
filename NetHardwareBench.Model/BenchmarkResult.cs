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
        }
        Modules.Types.BenchmarkType BenchmarkType { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }

        public TimeSpan TimeSpend
        {
            get { return FinishedAt - StartedAt; }
        }

        public int Score { get; set; }

        public List<string> StepsDetails { get; set; }
    }

}
