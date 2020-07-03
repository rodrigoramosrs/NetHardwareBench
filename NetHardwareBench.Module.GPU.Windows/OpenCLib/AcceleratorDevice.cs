using Cloo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCL
{
    public class AcceleratorDevice
    {
        public static AcceleratorDevice[] All = ComputePlatform.Platforms.Select(x => x.Devices).SelectMany(i => i).Select(x => new AcceleratorDevice(x)).ToArray();

        public static bool HasCPU
        {
            get
            {
                return All.Any(x => x.Type == ComputeDeviceTypes.Cpu);
            }
        }

        public static bool HasGPU
        {
            get
            {
                return All.Any(x => x.Type == ComputeDeviceTypes.Gpu);
            }
        }

        //http://www.nvidia.com/Download/index.aspx?lang=en-us
        //http://support.amd.com/en-us/download
        //https://software.intel.com/en-us/articles/opencl-drivers#latest_CPU_runtime

        public static AcceleratorDevice CPU
        {
            get
            {
                AcceleratorDevice cpu = All.FirstOrDefault(x => x.Type == ComputeDeviceTypes.Cpu);
                if (cpu == null)
                {
                    throw new InvalidOperationException("No OpenCL compatible CPU found on this computer. Maybe drivers are missing: " + "https://software.intel.com/en-us/articles/opencl-drivers#latest_CPU_runtime");
                }
                return cpu;
            }
        }

        public static AcceleratorDevice GPU
        {
            get
            {
                AcceleratorDevice gpu = All.FirstOrDefault(x => x.Type == ComputeDeviceTypes.Gpu);
                if (gpu == null)
                {
                    throw new InvalidOperationException("No OpenCL compatible GPU found on this computer. Drivers at: " + "http://www.nvidia.com/Download/index.aspx?lang=en-us" + " or " + "http://support.amd.com/en-us/download");
                }
                return gpu;
            }
        }


        public string Name { get; private set; }
        public string Vendor { get; private set; }
        public ComputeDevice Device { get; private set; }
        public ComputeDeviceTypes Type { get; private set; }

        public AcceleratorDevice(ComputeDevice Device)
        {
            this.Device = Device;
            this.Name = Device.Name;
            this.Vendor = Device.Vendor;
            this.Type = Device.Type;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
