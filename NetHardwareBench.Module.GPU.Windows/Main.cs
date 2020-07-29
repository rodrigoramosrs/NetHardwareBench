using OpenCL;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NetHardwareBench.Model;
using NetHardwareBench.Model.Modules.Parameters;
using NetHardwareBench.Model.Modules.Types;

namespace NetHardwareBench.Module.GPU.Windows
{
    public class Main : Model.Modules.BaseModule
    {
        public Main(ConfigurationParameters ConfigurationParameters) : base(ConfigurationParameters) { }

        public override BenchmarkResult DoBenchmark()
        {
            BenchmarkResult result = new BenchmarkResult();
            base.WriteMessage("== Starting GPU Benchmark ==\r\n");
            result.StepsDetails.Add("Starting GPU Benchmark");

            result.StartedAt = DateTime.Now;
            DoInternalBenchmark();
            result.FinishedAt = DateTime.Now;
            base.WriteMessage("== Finished GPU Benchmark ==\r\n\r\n");
            result.StepsDetails.Add("Finished GPU Benchmark");
            return result;
        }

        public override BenchmarkType GetBenchmarkType()
        {
            return BenchmarkType.GPU;
        }



















        static string IsPrime
        {
            get
            {
                return @"
                kernel void GetIfPrime(global int* message) 
                {
                    int index = get_global_id(0);

                    int upperl=(int)sqrt((float)message[index]);
                    for(int i=2;i<=upperl;i++)
                    {
                        if(message[index]%i==0)
                        {
                            //printf("" %d / %d\n"",index,i );
                            message[index]=0;
                            return;
                        }
                    }
                    //printf("" % d"",index);
                }";
            }
        }

        static void IsPrimeNet(int[] message)
        {
            Parallel.ForEach(message, (number, state, index) =>
            {
                int upperlimit = (int)Math.Sqrt(number);
                for (int i = 2; i <= upperlimit; i++)
                {
                    if (message[index] % i == 0)  //no lock needed. every index is independent
                    {
                        message[index] = 0;
                        break;
                    }
                }
            });
        }

        void BenchmarkDevice(AcceleratorDevice Device)
        {
            Console.Write("\tSingle GFlops = ");
            base.WriteMessage(EasyCL.GetDeviceGFlops_Single(Device).ToString("0.00") + "GFlops");

            Console.Write("\tDouble GFlops = ");
            base.WriteMessage(EasyCL.GetDeviceGFlops_Double(Device).ToString("0.00") + "GFlops");

            Console.Write("\tMemory Bandwidth = ");
            base.WriteMessage(EasyCL.GetDeviceBandwidth_GBps(Device).ToString("0.00") + "GByte/s");
            base.WriteMessage("");
        }

        void DoInternalBenchmark()
        {
            int Devices = AcceleratorDevice.All.Length;

            foreach (var AcceleratorDevice in AcceleratorDevice.All)
            {
                base.WriteMessage(AcceleratorDevice.ToString());
                BenchmarkDevice(AcceleratorDevice);
            }

            base.WriteMessage("Please share results @ " + "https://www.codeproject.com/Articles/1116907/How-to-Use-Your-GPU-in-NET");

            base.WriteMessage("");
            base.WriteMessage("Get Prime Number Performance");

            int WorkSize = 5000000;
            int[] ArrayA = GetArray(WorkSize);
            int[] ArrayB = new int[WorkSize];
            int[] ArrayC = new int[WorkSize];
            int[] ArrayD = new int[WorkSize];
            int[] ArrayE = new int[WorkSize];

            Array.Copy(ArrayA, ArrayB, ArrayA.Length);
            Array.Copy(ArrayA, ArrayC, ArrayA.Length);
            Array.Copy(ArrayA, ArrayD, ArrayA.Length);
            Array.Copy(ArrayA, ArrayE, ArrayA.Length);

            try
            {
                if (AcceleratorDevice.HasCPU)
                {
                    RunCPU(ArrayA);
                }
                if (AcceleratorDevice.HasGPU)
                {
                    RunGPU(ArrayB);
                }
                RunAllDevices(ArrayC);
                RunNative(ArrayD);

                if (AcceleratorDevice.HasGPU)
                {
                    base.WriteMessage("");
                    base.WriteMessage("Async Test");
                    AsyncTest(ArrayE);
                }
            }
            catch (Exception ex)
            {
                base.WriteMessage(ex.Message);
            }
        }

        int[] GetArray(int Worksize)
        {
            return Enumerable.Range(2, Worksize).ToArray();
        }

        void AsyncTest(int[] WorkSet)
        {
            EasyCL cl = new EasyCL()
            {
                Accelerator = AcceleratorDevice.GPU
            };
            cl.LoadKernel(IsPrime);
            Task primes = cl.InvokeAsync("GetIfPrime", WorkSet.Length, WorkSet);   //OpenCL uses a Cache. Real speed after that

            Stopwatch time = Stopwatch.StartNew();
            long i = 0;
            while (primes.IsCompleted == false)
            {
                i++;
            }
            double millis = time.Elapsed.TotalMilliseconds;

            base.WriteMessage(i + $" CPU cycles saved ({millis.ToString("0.00")}ms)");
        }

        void RunCPU(int[] WorkSet)
        {
            base.WriteMessage("\nRun on CPU: " + AcceleratorDevice.CPU.ToString());
            EasyCL cl = new EasyCL()
            {
                Accelerator = AcceleratorDevice.CPU
            };
            cl.LoadKernel(IsPrime);
            cl.Invoke("GetIfPrime", 0, 1, WorkSet);    //OpenCL uses a Cache. Real speed after that
            Stopwatch time = Stopwatch.StartNew();

            cl.Invoke("GetIfPrime", 0, WorkSet.Length, WorkSet);

            time.Stop();
            double performance = WorkSet.Length / (1000000.0 * time.Elapsed.TotalSeconds);
            base.WriteMessage("\t" + performance.ToString("0.00") + " MegaPrimes/Sec");
        }

        void RunNative(int[] WorkSet)
        {
            base.WriteMessage("\nRun in C# only");
            Stopwatch time = Stopwatch.StartNew();

            IsPrimeNet(WorkSet);

            time.Stop();
            double performance = WorkSet.Length / (1000000.0 * time.Elapsed.TotalSeconds);
            base.WriteMessage("\t" + performance.ToString("0.00") + " MegaPrimes/Sec");
        }

        void RunGPU(int[] WorkSet)
        {
            base.WriteMessage("\nRun on GPU: " + AcceleratorDevice.GPU.ToString());

            EasyCL cl = new EasyCL()
            {
                Accelerator = AcceleratorDevice.GPU
            };
            cl.LoadKernel(IsPrime);
            cl.Invoke("GetIfPrime", 0, 1, WorkSet);    //OpenCL uses a Cache. Real speed after that
            Stopwatch time = Stopwatch.StartNew();

            cl.Invoke("GetIfPrime", 0, WorkSet.Length, WorkSet);

            time.Stop();
            double performance = WorkSet.Length / (1000000.0 * time.Elapsed.TotalSeconds);
            base.WriteMessage("\t" + performance.ToString("0.00") + " MegaPrimes/Sec");
        }

        void RunAllDevices(int[] WorkSet)
        {
            base.WriteMessage("\nCombined run on " + AcceleratorDevice.All.Length + " Device/s");
            MultiCL cl = new MultiCL();
            cl.SetKernel(IsPrime, "GetIfPrime");
            cl.SetParameter(WorkSet);
            cl.Invoke(0, 2, 2); //Cache

            Stopwatch time = Stopwatch.StartNew();
            cl.Invoke(0, WorkSet.Length, 200);
            time.Stop();
            double performance = WorkSet.Length / (1000000.0 * time.Elapsed.TotalSeconds);
            base.WriteMessage("\t" + performance.ToString("0.00") + " MegaPrimes/Sec");
        }



        private void Cl_ProgressChangedEvent(object sender, double e)
        {
            base.WriteMessage(e.ToString("0.00%"));
        }
    }
}
