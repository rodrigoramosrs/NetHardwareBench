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
            Console.WriteLine("== Starting GPU Benchmark ==\r\n");
            result.StepsDetails.Add("Starting GPU Benchmark");

            result.StartedAt = DateTime.Now;
            DoInternalBenchmark();
            result.FinishedAt = DateTime.Now;
            Console.WriteLine("== Finished GPU Benchmark ==\r\n\r\n");
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

        static void BenchmarkDevice(AcceleratorDevice Device)
        {
            Console.Write("\tSingle GFlops = ");
            Console.WriteLine(EasyCL.GetDeviceGFlops_Single(Device).ToString("0.00") + "GFlops");

            Console.Write("\tDouble GFlops = ");
            Console.WriteLine(EasyCL.GetDeviceGFlops_Double(Device).ToString("0.00") + "GFlops");

            Console.Write("\tMemory Bandwidth = ");
            Console.WriteLine(EasyCL.GetDeviceBandwidth_GBps(Device).ToString("0.00") + "GByte/s");
            Console.WriteLine();
        }

        static void DoInternalBenchmark()
        {
            int Devices = AcceleratorDevice.All.Length;

            foreach (var AcceleratorDevice in AcceleratorDevice.All)
            {
                Console.WriteLine(AcceleratorDevice);
                BenchmarkDevice(AcceleratorDevice);
            }

            Console.WriteLine("Please share results @ " + "https://www.codeproject.com/Articles/1116907/How-to-Use-Your-GPU-in-NET");

            Console.WriteLine("");
            Console.WriteLine("Get Prime Number Performance");

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
                    Console.WriteLine("");
                    Console.WriteLine("Async Test");
                    AsyncTest(ArrayE);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static int[] GetArray(int Worksize)
        {
            return Enumerable.Range(2, Worksize).ToArray();
        }

        static void AsyncTest(int[] WorkSet)
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

            Console.WriteLine(i + $" CPU cycles saved ({millis.ToString("0.00")}ms)");
        }

        static void RunCPU(int[] WorkSet)
        {
            Console.WriteLine("\nRun on CPU: " + AcceleratorDevice.CPU.ToString());
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
            Console.WriteLine("\t" + performance.ToString("0.00") + " MegaPrimes/Sec");
        }

        static void RunNative(int[] WorkSet)
        {
            Console.WriteLine("\nRun in C# only");
            Stopwatch time = Stopwatch.StartNew();

            IsPrimeNet(WorkSet);

            time.Stop();
            double performance = WorkSet.Length / (1000000.0 * time.Elapsed.TotalSeconds);
            Console.WriteLine("\t" + performance.ToString("0.00") + " MegaPrimes/Sec");
        }

        static void RunGPU(int[] WorkSet)
        {
            Console.WriteLine("\nRun on GPU: " + AcceleratorDevice.GPU.ToString());

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
            Console.WriteLine("\t" + performance.ToString("0.00") + " MegaPrimes/Sec");
        }

        static void RunAllDevices(int[] WorkSet)
        {
            Console.WriteLine("\nCombined run on " + AcceleratorDevice.All.Length + " Device/s");
            MultiCL cl = new MultiCL();
            cl.SetKernel(IsPrime, "GetIfPrime");
            cl.SetParameter(WorkSet);
            cl.Invoke(0, 2, 2); //Cache

            Stopwatch time = Stopwatch.StartNew();
            cl.Invoke(0, WorkSet.Length, 200);
            time.Stop();
            double performance = WorkSet.Length / (1000000.0 * time.Elapsed.TotalSeconds);
            Console.WriteLine("\t" + performance.ToString("0.00") + " MegaPrimes/Sec");
        }



        private static void Cl_ProgressChangedEvent(object sender, double e)
        {
            Console.WriteLine(e.ToString("0.00%"));
        }
    }
}
