    using System;
using System.Collections.Generic;
using NetHardwareBench.Model;
using NetHardwareBench.Model.Modules.Parameters;
using static NetHardwareBench.Model.Modules.Events.LifeCycleEvents;

namespace NetHardwareBench.Benchmark
{
    public  class Main
    {
        public event StartedDelegate OnStarted = delegate { };
        public event FinishedDelegate OnFinished = delegate { };
        public event ExceptionDelegate OnException = delegate { };
        public event ProgressChangeDelegate OnProgressChanged = delegate { };
        public event ActivityReportDelegate OnActivityReport = delegate { };


        private readonly ConfigurationParameters _ConfigurationParameters;
        public Main(Model.Modules.Parameters.ConfigurationParameters ConfigurationParameters)
        {
            _ConfigurationParameters = ConfigurationParameters;
        }

        public List<BenchmarkResult> DoBenchmark(params Model.Modules.Types.BenchmarkType[] BenchmarkTypes)
        {
            if (BenchmarkTypes == null) throw new Exception("BenchmarkType could not be empty");

            List<BenchmarkResult> BenchmarkResultList = new List<BenchmarkResult>();
            int percentage = 0;
            for (int i = 0; i < BenchmarkTypes.Length; i++)
            {
                var benchmarkModule = Factory.Main.GetModuleByType(BenchmarkTypes[i], this._ConfigurationParameters);

                AttachEvents(ref benchmarkModule, true);
                var benchmarkResultPartial = benchmarkModule.DoBenchmark();
                benchmarkResultPartial.BenchmarkType = BenchmarkTypes[i];
                BenchmarkResultList.Add(benchmarkResultPartial);
                AttachEvents(ref benchmarkModule, false);

                OnProgressChanged.DynamicInvoke( Math.Round(((double)(i + 1) / (double)BenchmarkTypes.Length ) *100,2));
            }
            

            return BenchmarkResultList;
        }

        private void AttachEvents(ref Model.Modules.BaseModule baseModule, bool Attach)
        {
            if (Attach)
            {
                baseModule.OnStarted += BaseModule_OnStarted;
                baseModule.OnFinished += BaseModule_OnFinished;
                baseModule.OnProgressChanged += BaseModule_OnProgressChanged;
                baseModule.OnActivityReport += BaseModule_OnActivityReport;
                baseModule.OnException += BaseModule_OnException;
            }
            else
            {
                baseModule.OnStarted -= BaseModule_OnStarted;
                baseModule.OnFinished -= BaseModule_OnFinished;
                baseModule.OnProgressChanged -= BaseModule_OnProgressChanged;
                baseModule.OnActivityReport -= BaseModule_OnActivityReport;
                baseModule.OnException -= BaseModule_OnException;
            }


        }

        private void BaseModule_OnStarted()
        {
            throw new NotImplementedException();
        }
        private void BaseModule_OnFinished()
        {
            throw new NotImplementedException();
        }
        private void BaseModule_OnProgressChanged(double progress)
        {
            throw new NotImplementedException();
        }

        private void BaseModule_OnActivityReport(string message)
        {
            throw new NotImplementedException();
        }

        private void BaseModule_OnException(Exception exception)
        {
            throw new NotImplementedException();
        }

        
    }
}
