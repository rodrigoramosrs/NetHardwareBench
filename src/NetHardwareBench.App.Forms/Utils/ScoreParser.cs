using System;
using System.Collections.Generic;
using System.Text;

namespace NetHardwareBench.App.Forms.Utils
{
    public static class ScoreParser
    {
        public static string Calculate(Model.Modules.Types.BenchmarkType types, Double Score)
        {
            switch (types)
            {
                case Model.Modules.Types.BenchmarkType.CPU:
                    return CaclulateCPU(Score);
                case Model.Modules.Types.BenchmarkType.GPU:
                    return CaclulateGPU(Score);
                case Model.Modules.Types.BenchmarkType.INTERNET:
                    return CaclulateInternet(Score);
                case Model.Modules.Types.BenchmarkType.NETWORK:
                    return CalculateNetwork(Score);
                case Model.Modules.Types.BenchmarkType.RAM:
                    return CaclulateRAM(Score);
                case Model.Modules.Types.BenchmarkType.LOCAL_STORAGE:
                    return CaclulateLocalStorage(Score);
                case Model.Modules.Types.BenchmarkType.NETWORK_STORAGE:
                    return CaclulateNetworkStorage(Score);
                case Model.Modules.Types.BenchmarkType.DATABASE:
                    return CaclulateDatabaseStorage(Score);
                default:
                    return Score.ToString();
            }
        }

        private static string CaclulateCPU(double Score)
        {
            if (Score < 1.5) return "100";

            if (Score > 1.5 && Score < 2.5) return "75";

            if (Score > 2.5 && Score < 3.5) return "50";

            if (Score > 3.5 && Score < 6.0) return "25";

            if (Score > 6.0) return "0";

            return "0";
        }
        private static string CaclulateGPU(double Score)
        {
            return "";
        }
        private static string CaclulateInternet(double Score)
        {
            if (Score < 2) return "0";

            if (Score > 2 && Score < 5) return "25";

            if (Score > 5 && Score < 10) return "50";

            if (Score > 10 && Score < 20) return "75";

            if (Score > 20) return "100";
   
            return "100";
        }
        private static string CalculateNetwork(double Score)
        {

            if (Score < 20) return "100";

            if (Score > 20 && Score < 40) return "75";

            if (Score > 40 && Score < 60) return "50";

            if (Score > 60 && Score < 100) return "25";

            if (Score > 100) return "0";

            return "0";
        }
        private static string CaclulateRAM(double Score)
        {

            if (Score < 150) return "0";

            if (Score > 150 && Score < 200) return "25";

            if (Score > 200 && Score < 250) return "50";

            if (Score > 250  && Score < 350) return "75";

            if (Score > 350) return "100";

            return "100";
        }
        private static string CaclulateLocalStorage(double Score)
        {

            if (Score < 1) return "0";

            if (Score > 1 && Score < 3) return "25";

            if (Score > 3 && Score < 6) return "50";

            if (Score > 7 && Score < 50) return "75";

            if (Score > 50) return "100";

            return "100";
        }
        private static string CaclulateNetworkStorage(double Score)
        {
            return "";
        }
        private static string CaclulateDatabaseStorage(double Score)
        {
            return "";
        }

    }
}
