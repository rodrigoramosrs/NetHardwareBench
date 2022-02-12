using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            base.WriteMessage("== Starting CPU Benchmark ==\r\n");
            result.StepsDetails.Add("Starting CPU Benchmark");

            result.StartedAt = DateTime.Now;
            //DoInternalBenchmark();
            var partialBigBenchmark = StartBigBenchmark();
            var partialInternalBenchmark = DoInternalBenchmark();
            result.FinishedAt = DateTime.Now;

            result.PartialResults.AddRange(partialBigBenchmark);
            result.PartialResults.AddRange(partialInternalBenchmark);
            result.Score = Math.Round(result.PartialResults.Sum(x => x.Score) / result.PartialResults.Count, 2);
            
            base.WriteMessage("== Finished CPU Benchmark ==\r\n\r\n");
            result.StepsDetails.Add("Finished CPU Benchmark");
            return result;
        }

        List<BenchmarkPartialResult> DoInternalBenchmark()
        {
            // FindPrimeNumber(99999);
            List<BenchmarkPartialResult> result = new List<BenchmarkPartialResult>();
            Stopwatch Cronometro = new Stopwatch();
            Cronometro.Start();
            base.WriteMessage("\r\nCalculating prime number in CPU");
            base.WriteMessage("==================================");
            TimeSpan TempoTotalGasto;
            int NumeroDeIteracoes = 5;
            for (int i = 1; i < NumeroDeIteracoes + 1; i++)
            {
                FindPrimeNumber(299999);
                var TempoAtual = Cronometro.Elapsed;
                base.WriteMessage("Attempt " + i + ": " + TempoAtual.ToString());
                TempoTotalGasto += TempoAtual;

                result.Add(new BenchmarkPartialResult()
                {
                    Description = $"CPU - CALCULATE PRIME_NUMBER [{i}]",
                    Score = TempoAtual.TotalSeconds,
                    MetricScale = MetricScaleType.SECOND,
                    ResultType = BenchmarkResultType.SPEED_TIME
                });

                Cronometro.Restart();
            }
            var avgTime = new TimeSpan((TempoTotalGasto.Ticks / NumeroDeIteracoes));
            base.WriteMessage("Prime Number AVG: " + avgTime.ToString());
            Cronometro.Stop();
            return result;
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

         double MulBigDouble(int loop, out TimeSpan timeSpent)
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
            timeSpent = ts;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            base.WriteMessage("MulBigDouble RunTime:" + elapsedTime);

            return result;
        }
         Int64 MulBigInt(int loop, out TimeSpan timeSpent)
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
            timeSpent = ts;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            base.WriteMessage("MulBigInt RunTime:" + elapsedTime);

            return result;
        }
         double DivBigDouble(int loop, out TimeSpan timeSpent)
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
            timeSpent = ts;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            base.WriteMessage("DivBigDouble RunTime:" + elapsedTime);

            return result;
        }
         Int64 DivBigInt(int loop, out TimeSpan timeSpent)
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
            timeSpent = ts;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            base.WriteMessage("DivBigInt RunTime:" + elapsedTime);

            return result;
        }
         double MulSmallDouble(int loop, out TimeSpan timeSpent)
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
            timeSpent = ts;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            base.WriteMessage("MulSmallDouble RunTime:" + elapsedTime);

            return result;
        }
         Int64 MulSmallInt(int loop, out TimeSpan timeSpent)
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
            timeSpent = ts;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            base.WriteMessage("MulSmallInt RunTime:" + elapsedTime);

            return result;
        }
         double DivSmallDouble(int loop, out TimeSpan timeSpent)
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
            timeSpent = ts;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            base.WriteMessage("DivSmallDouble RunTime:" + elapsedTime);

            return result;
        }
         Int64 DivSmallInt(int loop, out TimeSpan timeSpent)
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
            timeSpent = ts;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            base.WriteMessage("DivSmallInt RunTime:" + elapsedTime);

            return result;
        }
         double AddBigDouble(int loop, out TimeSpan timeSpent)
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
            timeSpent = ts;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            base.WriteMessage("AddBigDouble RunTime:" + elapsedTime);

            return result;
        }
         Int64 AddBigInt(int loop, out TimeSpan timeSpent)
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
            timeSpent = ts;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            base.WriteMessage("AddBigInt RunTime:" + elapsedTime);

            return result;
        }
         double SubBigDouble(int loop, out TimeSpan timeSpent)
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
            timeSpent = ts;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            base.WriteMessage("SubBigDouble RunTime:" + elapsedTime);

            return result;
        }
         Int64 SubBigInt(int loop, out TimeSpan timeSpent)
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
            timeSpent = ts;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            base.WriteMessage("SubBigInt RunTime:" + elapsedTime);

            return result;
        }
         double AddSmallDouble(int loop, out TimeSpan timeSpent)
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
            timeSpent = ts;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            base.WriteMessage("AddSmallDouble RunTime:" + elapsedTime);

            return result;
        }
         Int64 AddSmallInt(int loop, out TimeSpan timeSpent)
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
            timeSpent = ts;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            base.WriteMessage("AddSmallInt RunTime:" + elapsedTime);

            return result;
        }
         double SubSmallDouble(int loop, out TimeSpan timeSpent)
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
            timeSpent = ts;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            base.WriteMessage("SubSmallDouble RunTime:" + elapsedTime);

            return result;
        }
         Int64 SubSmallInt(int loop, out TimeSpan timeSpent)
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
            timeSpent = ts;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            base.WriteMessage("SubSmallInt RunTime:" + elapsedTime);

            return result;
        }

        List<BenchmarkPartialResult> StartBigBenchmark()
        {
            List<BenchmarkPartialResult> result = new List<BenchmarkPartialResult>();
            TimeSpan TimeSpent = new TimeSpan();
            Init();
            int loop = 1000000;
            base.WriteMessage("Multiplication and Division Benchmark");
            base.WriteMessage("=====================================");
            MulBigDouble(loop, out TimeSpent);
            result.Add(new BenchmarkPartialResult() { MetricScale = MetricScaleType.SECOND, Description = "CPU - MulBigDouble" , ResultType = BenchmarkResultType.SPEED_TIME, Score = TimeSpent.TotalSeconds});
            MulBigInt(loop, out TimeSpent);
            result.Add(new BenchmarkPartialResult() { MetricScale = MetricScaleType.SECOND, Description = "CPU - MulBigInt", ResultType = BenchmarkResultType.SPEED_TIME, Score = TimeSpent.TotalSeconds });
            DivBigDouble(loop, out TimeSpent);
            result.Add(new BenchmarkPartialResult() { MetricScale = MetricScaleType.SECOND, Description = "CPU - DivBigDouble", ResultType = BenchmarkResultType.SPEED_TIME, Score = TimeSpent.TotalSeconds });
            DivBigInt(loop, out TimeSpent);
            result.Add(new BenchmarkPartialResult() { MetricScale = MetricScaleType.SECOND, Description = "CPU - DivBigInt", ResultType = BenchmarkResultType.SPEED_TIME, Score = TimeSpent.TotalSeconds });
            MulSmallDouble(loop, out TimeSpent);
            result.Add(new BenchmarkPartialResult() { MetricScale = MetricScaleType.SECOND, Description = "CPU - MulSmallDouble", ResultType = BenchmarkResultType.SPEED_TIME, Score = TimeSpent.TotalSeconds });
            MulSmallInt(loop, out TimeSpent);
            result.Add(new BenchmarkPartialResult() { MetricScale = MetricScaleType.SECOND, Description = "CPU - MulSmallInt", ResultType = BenchmarkResultType.SPEED_TIME, Score = TimeSpent.TotalSeconds });
            DivSmallDouble(loop, out TimeSpent);
            result.Add(new BenchmarkPartialResult() { MetricScale = MetricScaleType.SECOND, Description = "CPU - DivSmallDouble", ResultType = BenchmarkResultType.SPEED_TIME, Score = TimeSpent.TotalSeconds });
            DivSmallInt(loop, out TimeSpent);
            result.Add(new BenchmarkPartialResult() { MetricScale = MetricScaleType.SECOND, Description = "CPU - DivSmallInt", ResultType = BenchmarkResultType.SPEED_TIME, Score = TimeSpent.TotalSeconds });
            base.WriteMessage("\nAddition and Subtraction Benchmark");
            base.WriteMessage("==================================");
            AddBigDouble(loop, out TimeSpent);
            result.Add(new BenchmarkPartialResult() { MetricScale = MetricScaleType.SECOND, Description = "CPU - AddBigDouble", ResultType = BenchmarkResultType.SPEED_TIME, Score = TimeSpent.TotalSeconds });
            AddBigInt(loop, out TimeSpent);
            result.Add(new BenchmarkPartialResult() { MetricScale = MetricScaleType.SECOND, Description = "CPU - AddBigInt", ResultType = BenchmarkResultType.SPEED_TIME, Score = TimeSpent.TotalSeconds });
            SubBigDouble(loop, out TimeSpent);
            result.Add(new BenchmarkPartialResult() { MetricScale = MetricScaleType.SECOND, Description = "CPU - SubBigDouble", ResultType = BenchmarkResultType.SPEED_TIME, Score = TimeSpent.TotalSeconds });
            SubBigInt(loop, out TimeSpent);
            result.Add(new BenchmarkPartialResult() { MetricScale = MetricScaleType.SECOND, Description = "CPU - SubBigInt", ResultType = BenchmarkResultType.SPEED_TIME, Score = TimeSpent.TotalSeconds });
            AddSmallDouble(loop, out TimeSpent);
            result.Add(new BenchmarkPartialResult() { MetricScale = MetricScaleType.SECOND, Description = "CPU - AddSmallDouble", ResultType = BenchmarkResultType.SPEED_TIME, Score = TimeSpent.TotalSeconds });
            AddSmallInt(loop, out TimeSpent);
            result.Add(new BenchmarkPartialResult() { MetricScale = MetricScaleType.SECOND, Description = "CPU - AddSmallInt", ResultType = BenchmarkResultType.SPEED_TIME, Score = TimeSpent.TotalSeconds });
            SubSmallDouble(loop, out TimeSpent);
            result.Add(new BenchmarkPartialResult() { MetricScale = MetricScaleType.SECOND, Description = "CPU - SubSmallDouble", ResultType = BenchmarkResultType.SPEED_TIME, Score = TimeSpent.TotalSeconds });
            SubSmallInt(loop, out TimeSpent);
            result.Add(new BenchmarkPartialResult() { MetricScale = MetricScaleType.SECOND, Description = "CPU - SubSmallInt", ResultType = BenchmarkResultType.SPEED_TIME, Score = TimeSpent.TotalSeconds });
            return result;
        }
























































    }
}
