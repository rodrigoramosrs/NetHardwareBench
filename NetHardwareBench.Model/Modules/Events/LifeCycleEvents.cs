using System;
using System.Collections.Generic;
using System.Text;

namespace NetHardwareBench.Model.Modules.Events
{
    public class LifeCycleEvents
    {
        public delegate void StartedDelegate();
        public delegate void FinishedDelegate();

        public delegate void ProgressChangeDelegate(double progress);

        public delegate void ActivityReportDelegate(string message);

        public delegate void ExceptionDelegate(Exception exception);
        
    }
}
