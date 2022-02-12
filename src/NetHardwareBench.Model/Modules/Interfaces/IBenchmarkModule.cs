using System;
using System.Collections.Generic;
using System.Text;

namespace NetHardwareBench.Model.Modules.Interfaces

{
    interface IBenchmarkModule
    {
        Types.BenchmarkType GetBenchmarkType();

        BenchmarkResult DoBenchmark();
    }
}
