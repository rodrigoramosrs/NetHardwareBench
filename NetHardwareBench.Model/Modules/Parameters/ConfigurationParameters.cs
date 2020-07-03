using System;
using System.Collections.Generic;
using System.Text;

namespace NetHardwareBench.Model.Modules.Parameters
{
    public class ConfigurationParameters
    {
        public string WorkDirectory { get; set; }
        
        public string RemoteIP { get; set; }

        public string RemoteEndpoint { get; set; }

        public string NetworkPath { get; set; }
    }
}
