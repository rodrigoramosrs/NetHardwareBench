using System;
using System.Collections.Generic;
using System.Text;
using NetHardwareBench.Model.Modules.Parameters;
using NetHardwareBench.Model.Modules.Types;

namespace NetHardwareBench.Model.Modules
{
    public abstract class BaseModule : Interfaces.IBenchmarkModule
    {
        public event Events.LifeCycleEvents.StartedDelegate OnStarted = delegate { };
        public event Events.LifeCycleEvents.FinishedDelegate OnFinished = delegate { };
        public event Events.LifeCycleEvents.ExceptionDelegate OnException = delegate { };
        public event Events.LifeCycleEvents.ProgressChangeDelegate OnProgressChanged = delegate { };
        public event Events.LifeCycleEvents.ActivityReportDelegate OnActivityReport = delegate { };
        
        private readonly ConfigurationParameters _ConfigurationParameters;
        public BaseModule(ConfigurationParameters ConfigurationParameters)
        {
            _ConfigurationParameters = ConfigurationParameters;
        }
        public abstract BenchmarkResult DoBenchmark();

        public abstract BenchmarkType GetBenchmarkType();
    }
}
