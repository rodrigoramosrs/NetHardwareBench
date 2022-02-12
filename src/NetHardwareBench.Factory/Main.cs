using System;

namespace NetHardwareBench.Factory
{
    public static class Main
    {
        public static Model.Modules.BaseModule GetModuleByType(Model.Modules.Types.BenchmarkType BenchmarkType, Model.Modules.Parameters.ConfigurationParameters ConfigurationParameters)
        {
            switch (BenchmarkType)
            {
                case Model.Modules.Types.BenchmarkType.CPU:
                    return new Module.CPU.Main(ConfigurationParameters);
                case Model.Modules.Types.BenchmarkType.GPU:
                    return new Module.GPU.Windows.Main(ConfigurationParameters);
                case Model.Modules.Types.BenchmarkType.INTERNET:
                    return new Module.Internet.Main(ConfigurationParameters);
                case Model.Modules.Types.BenchmarkType.NETWORK:
                    return new Module.Network.Main(ConfigurationParameters);
                case Model.Modules.Types.BenchmarkType.RAM:
                    return new Module.RAM.Main(ConfigurationParameters);
                case Model.Modules.Types.BenchmarkType.LOCAL_STORAGE:
                    return new Module.LocalStorage.Main(ConfigurationParameters);
                case Model.Modules.Types.BenchmarkType.NETWORK_STORAGE:
                    return new Module.RemoteStorage.Main(ConfigurationParameters);
                case Model.Modules.Types.BenchmarkType.DATABASE:
                    return new Module.Database.Main(ConfigurationParameters);

                default:
                    throw new Exception("BenchmarkType Not found for " + BenchmarkType.ToString());
                    
            }
        }
    }
}
