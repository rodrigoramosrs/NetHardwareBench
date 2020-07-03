using System;
using System.Collections.Generic;
using System.Diagnostics;
using NetHardwareBench.Model;
using NetHardwareBench.Model.Modules.Parameters;
using NetHardwareBench.Model.Modules.Types;

namespace NetHardwareBench.Module.CPU
{
    public class Main : Model.Modules.BaseModule
    {
        public Main(ConfigurationParameters ConfigurationParameters) : base(ConfigurationParameters) { }

        public override BenchmarkType GetBenchmarkType()
        {
            return BenchmarkType.CPU;
        }

        public override BenchmarkResult DoBenchmark()
        {
            BenchmarkResult result = new BenchmarkResult();
            Console.WriteLine("== Starting CPU Benchmark ==\r\n");
            result.StepsDetails.Add("Starting CPU Benchmark");

            result.StartedAt = DateTime.Now;
            //DoInternalBenchmark();
            StartBigBenchmark();
            DoInternalBenchmark();
            result.FinishedAt = DateTime.Now;
            Console.WriteLine("== Finished CPU Benchmark ==\r\n\r\n");
            result.StepsDetails.Add("Finished CPU Benchmark");
            return result;
        }

        private void DoInternalBenchmark()
        {
            // FindPrimeNumber(99999);
            Stopwatch Cronometro = new Stopwatch();
            Cronometro.Start();
            Console.WriteLine("\r\nCalculating prime number in CPU");
            Console.WriteLine("==================================");
            TimeSpan TempoTotalGasto;
            int NumeroDeIteracoes = 3;
            for (int i = 1; i < NumeroDeIteracoes + 1; i++)
            {
                FindPrimeNumber(299999);
                var TempoAtual = Cronometro.Elapsed;
                Console.WriteLine("Attempt " + i + ": " + TempoAtual.ToString());
                TempoTotalGasto += TempoAtual;
                Cronometro.Restart();
            }
            Console.WriteLine("Prime Number AVG: " + new TimeSpan((TempoTotalGasto.Ticks/ NumeroDeIteracoes)).ToString());
            Cronometro.Stop();
        }

        public long FindPrimeNumber(int n)
        {
            int count = 0;
            long a = 2;
            while (count < n)
            {
                long b = 2;
                int prime = 1;// to check if found a prime
                while (b * b <= a)
                {
                    if (a % b == 0)
                    {
                        prime = 0;
                        break;
                    }
                    b++;
                }
                if (prime > 0)
                {
                    count++;
                }
                a++;
            }
            return (--a);
        }
















        private List<Int64> smallIntList = new List<Int64>();
        private List<Int64> bigIntList = new List<Int64>();
        private List<Double> smallDoubleList = new List<Double>();
        private List<Double> bigDoubleList = new List<Double>();

        private void Init()
        {
            smallIntList.Add(158);
            smallIntList.Add(21);
            smallIntList.Add(7813);
            smallIntList.Add(632);
            smallIntList.Add(87);
            smallIntList.Add(14);
            smallIntList.Add(751);
            smallIntList.Add(201);
            smallIntList.Add(79);
            smallIntList.Add(26);

            smallDoubleList.Add(158.0);
            smallDoubleList.Add(21.0);
            smallDoubleList.Add(7813.0);
            smallDoubleList.Add(632.0);
            smallDoubleList.Add(87.0);
            smallDoubleList.Add(14.0);
            smallDoubleList.Add(751.0);
            smallDoubleList.Add(201.0);
            smallDoubleList.Add(79.0);
            smallDoubleList.Add(26.0);

            bigIntList.Add(158862);
            bigIntList.Add(78213);
            bigIntList.Add(425763);
            bigIntList.Add(412489);
            bigIntList.Add(852362);
            bigIntList.Add(23546);
            bigIntList.Add(145823);
            bigIntList.Add(352689);
            bigIntList.Add(558721);

            bigDoubleList.Add(158862.0);
            bigDoubleList.Add(78213.0);
            bigDoubleList.Add(425763.0);
            bigDoubleList.Add(412489.0);
            bigDoubleList.Add(852362.0);
            bigDoubleList.Add(23546.0);
            bigDoubleList.Add(145823.0);
            bigDoubleList.Add(352689.0);
            bigDoubleList.Add(558721.0);
        }

         double MulBigDouble(int loop)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            double result = 0.0;
            for (int k = 0; k < loop; ++k)
            {
                for (int i = 0; i < bigDoubleList.Count; ++i)
                {
                    for (int j = 0; j < bigDoubleList.Count; ++j)
                    {
                        result = bigDoubleList[i] * bigDoubleList[j];
                    }
                }
            }

            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine("MulBigDouble RunTime:" + elapsedTime);

            return result;
        }
         Int64 MulBigInt(int loop)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Int64 result = 0;
            for (int k = 0; k < loop; ++k)
            {
                for (int i = 0; i < bigIntList.Count; ++i)
                {
                    for (int j = 0; j < bigIntList.Count; ++j)
                    {
                        result = bigIntList[i] * bigIntList[j];
                    }
                }
            }
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine("MulBigInt RunTime:" + elapsedTime);

            return result;
        }
         double DivBigDouble(int loop)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            double result = 0.0;
            for (int k = 0; k < loop; ++k)
            {
                for (int i = 0; i < bigDoubleList.Count; ++i)
                {
                    for (int j = 0; j < bigDoubleList.Count; ++j)
                    {
                        result = bigDoubleList[i] / bigDoubleList[j];
                    }
                }
            }
            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine("DivBigDouble RunTime:" + elapsedTime);

            return result;
        }
         Int64 DivBigInt(int loop)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Int64 result = 0;
            for (int k = 0; k < loop; ++k)
            {
                for (int i = 0; i < bigIntList.Count; ++i)
                {
                    for (int j = 0; j < bigIntList.Count; ++j)
                    {
                        result = bigIntList[i] / bigIntList[j];
                    }
                }
            }
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine("DivBigInt RunTime:" + elapsedTime);

            return result;
        }
         double MulSmallDouble(int loop)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            double result = 0.0;
            for (int k = 0; k < loop; ++k)
            {
                for (int i = 0; i < smallDoubleList.Count; ++i)
                {
                    for (int j = 0; j < smallDoubleList.Count; ++j)
                    {
                        result = smallDoubleList[i] * smallDoubleList[j];
                    }
                }
            }
            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine("MulSmallDouble RunTime:" + elapsedTime);

            return result;
        }
         Int64 MulSmallInt(int loop)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Int64 result = 0;
            for (int k = 0; k < loop; ++k)
            {
                for (int i = 0; i < smallIntList.Count; ++i)
                {
                    for (int j = 0; j < smallIntList.Count; ++j)
                    {
                        result = smallIntList[i] * smallIntList[j];
                    }
                }
            }
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine("MulSmallInt RunTime:" + elapsedTime);

            return result;
        }
         double DivSmallDouble(int loop)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            double result = 0.0;
            for (int k = 0; k < loop; ++k)
            {
                for (int i = 0; i < smallDoubleList.Count; ++i)
                {
                    for (int j = 0; j < smallDoubleList.Count; ++j)
                    {
                        result = smallDoubleList[i] / smallDoubleList[j];
                    }
                }
            }
            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine("DivSmallDouble RunTime:" + elapsedTime);

            return result;
        }
         Int64 DivSmallInt(int loop)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Int64 result = 0;
            for (int k = 0; k < loop; ++k)
            {
                for (int i = 0; i < smallIntList.Count; ++i)
                {
                    for (int j = 0; j < smallIntList.Count; ++j)
                    {
                        result = smallIntList[i] / smallIntList[j];
                    }
                }
            }
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine("DivSmallInt RunTime:" + elapsedTime);

            return result;
        }
         double AddBigDouble(int loop)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            double result = 0.0;
            for (int k = 0; k < loop; ++k)
            {
                for (int i = 0; i < bigDoubleList.Count; ++i)
                {
                    for (int j = 0; j < bigDoubleList.Count; ++j)
                    {
                        result = bigDoubleList[i] + bigDoubleList[j];
                    }
                }
            }
            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine("AddBigDouble RunTime:" + elapsedTime);

            return result;
        }
         Int64 AddBigInt(int loop)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Int64 result = 0;
            for (int k = 0; k < loop; ++k)
            {
                for (int i = 0; i < bigIntList.Count; ++i)
                {
                    for (int j = 0; j < bigIntList.Count; ++j)
                    {
                        result = bigIntList[i] + bigIntList[j];
                    }
                }
            }
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine("AddBigInt RunTime:" + elapsedTime);

            return result;
        }
         double SubBigDouble(int loop)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            double result = 0.0;
            for (int k = 0; k < loop; ++k)
            {
                for (int i = 0; i < bigDoubleList.Count; ++i)
                {
                    for (int j = 0; j < bigDoubleList.Count; ++j)
                    {
                        result = bigDoubleList[i] - bigDoubleList[j];
                    }
                }
            }
            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine("SubBigDouble RunTime:" + elapsedTime);

            return result;
        }
         Int64 SubBigInt(int loop)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Int64 result = 0;
            for (int k = 0; k < loop; ++k)
            {
                for (int i = 0; i < bigIntList.Count; ++i)
                {
                    for (int j = 0; j < bigIntList.Count; ++j)
                    {
                        result = bigIntList[i] - bigIntList[j];
                    }
                }
            }
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine("SubBigInt RunTime:" + elapsedTime);

            return result;
        }
         double AddSmallDouble(int loop)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            double result = 0.0;
            for (int k = 0; k < loop; ++k)
            {
                for (int i = 0; i < smallDoubleList.Count; ++i)
                {
                    for (int j = 0; j < smallDoubleList.Count; ++j)
                    {
                        result = smallDoubleList[i] + smallDoubleList[j];
                    }
                }
            }
            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine("AddSmallDouble RunTime:" + elapsedTime);

            return result;
        }
         Int64 AddSmallInt(int loop)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Int64 result = 0;
            for (int k = 0; k < loop; ++k)
            {
                for (int i = 0; i < smallIntList.Count; ++i)
                {
                    for (int j = 0; j < smallIntList.Count; ++j)
                    {
                        result = smallIntList[i] + smallIntList[j];
                    }
                }
            }
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine("AddSmallInt RunTime:" + elapsedTime);

            return result;
        }
         double SubSmallDouble(int loop)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            double result = 0.0;
            for (int k = 0; k < loop; ++k)
            {
                for (int i = 0; i < smallDoubleList.Count; ++i)
                {
                    for (int j = 0; j < smallDoubleList.Count; ++j)
                    {
                        result = smallDoubleList[i] - smallDoubleList[j];
                    }
                }
            }
            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine("SubSmallDouble RunTime:" + elapsedTime);

            return result;
        }
         Int64 SubSmallInt(int loop)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Int64 result = 0;
            for (int k = 0; k < loop; ++k)
            {
                for (int i = 0; i < smallIntList.Count; ++i)
                {
                    for (int j = 0; j < smallIntList.Count; ++j)
                    {
                        result = smallIntList[i] - smallIntList[j];
                    }
                }
            }
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine("SubSmallInt RunTime:" + elapsedTime);

            return result;
        }

         void StartBigBenchmark()
        {
            Init();
            int loop = 1000000;
            Console.WriteLine("Multiplication and Division Benchmark");
            Console.WriteLine("=====================================");
            MulBigDouble(loop);
            MulBigInt(loop);
            DivBigDouble(loop);
            DivBigInt(loop);
            MulSmallDouble(loop);
            MulSmallInt(loop);
            DivSmallDouble(loop);
            DivSmallInt(loop);
            Console.WriteLine("\nAddition and Subtraction Benchmark");
            Console.WriteLine("==================================");
            AddBigDouble(loop);
            AddBigInt(loop);
            SubBigDouble(loop);
            SubBigInt(loop);
            AddSmallDouble(loop);
            AddSmallInt(loop);
            SubSmallDouble(loop);
            SubSmallInt(loop);
        }
























































    }
}
