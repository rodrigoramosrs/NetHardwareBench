using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NetHardwareBench.App.Forms.Hardware
{
    public static class HardwareInfo
    {
        public static string GetGPUDescription()
        {
            return Utils.ManagementObjectSearcherHelper
                .GetSimpleManagedInformation("select * from Win32_VideoController", "Name")
                .ToString();

        }

        public static string GetCPUDescription()
        {
            return Utils.ManagementObjectSearcherHelper
                .GetSimpleManagedInformation("select * from Win32_Processor", "Name")
                .ToString();

        }

        public static string GetHDDDescription()
        {
            double TotalMemory = 0;
            string Description = "";
            foreach (var item in Utils.ManagementObjectSearcherHelper
                .GetManagedInformation("SELECT * FROM Win32_DiskDrive"))
            {

                if (item.Properties["InterfaceType"].Value.ToString() == "USB") continue;

                TotalMemory += Convert.ToInt64(item.Properties["Size"].Value.ToString());
                Description = item["Model"].ToString();
                Description += " - " + item["InterfaceType"].ToString();

                break;
            }

            return Description + " [" + Convert.ToInt32(((TotalMemory / 1024) / 1024) /1024).ToString() + " GB" + "]" ;
        }


        public static string GetNetworDescription()
        {
            string Description = "";


            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var adapter in adapters)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback
                     || adapter.NetworkInterfaceType == NetworkInterfaceType.Tunnel
                     || adapter.OperationalStatus == OperationalStatus.Up
                     || adapter.Name.StartsWith("vEthernet") == true
                     || adapter.Description.Contains("Hyper-v") == true  
                     || adapter.Description.Contains("Virtual") == true
                     || adapter.Description.Contains("Microsoft") == true)
                        continue;

                Description += adapter.Description + " [" + adapter.Name + "]" + "\r\n";
            }



            return Description;
        }



        public static string GetRamDescription()
        {
            double TotalMemory = 0;
            foreach (var item in Utils.ManagementObjectSearcherHelper
                .GetManagedInformation("Select * From Win32_PhysicalMemory"))
            {
                TotalMemory += double.Parse(item["Capacity"].ToString());
            }

            return Convert.ToInt32((TotalMemory / 1024) / 1024).ToString() + " MB";

        }

        public static StringBuilder GetHardwareInfo()
        {
            StringBuilder retorno = new StringBuilder();
            ManagementObjectSearcher myVideoObject = new ManagementObjectSearcher("select * from Win32_VideoController");

            foreach (ManagementObject obj in myVideoObject.Get())
            {
                retorno.AppendLine(string.Format("Name  -  " + obj["Name"]));
                retorno.AppendLine(string.Format("Status  -  " + obj["Status"]));
                retorno.AppendLine(string.Format("Caption  -  " + obj["Caption"]));
                retorno.AppendLine(string.Format("DeviceID  -  " + obj["DeviceID"]));
                retorno.AppendLine(string.Format("AdapterRAM  -  " + obj["AdapterRAM"]));
                retorno.AppendLine(string.Format("AdapterDACType  -  " + obj["AdapterDACType"]));
                retorno.AppendLine(string.Format("Monochrome  -  " + obj["Monochrome"]));
                retorno.AppendLine(string.Format("InstalledDisplayDrivers  -  " + obj["InstalledDisplayDrivers"]));
                retorno.AppendLine(string.Format("DriverVersion  -  " + obj["DriverVersion"]));
                retorno.AppendLine(string.Format("VideoProcessor  -  " + obj["VideoProcessor"]));
                retorno.AppendLine(string.Format("VideoArchitecture  -  " + obj["VideoArchitecture"]));
                retorno.AppendLine(string.Format("VideoMemoryType  -  " + obj["VideoMemoryType"]));
            }


            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                retorno.AppendLine(string.Format("Drive {0}", d.Name));
                retorno.AppendLine(string.Format("  Drive type: {0}", d.DriveType));
                if (d.IsReady == true)
                {
                    retorno.AppendLine(string.Format("  Volume label: {0}", d.VolumeLabel));
                    retorno.AppendLine(string.Format("  File system: {0}", d.DriveFormat));
                    retorno.AppendLine(string.Format("  Available space to current user:{0, 15}", SizeSuffix(d.AvailableFreeSpace)));
                    retorno.AppendLine(string.Format("  Total available space:          {0, 15}", SizeSuffix(d.TotalFreeSpace)));
                    retorno.AppendLine(string.Format("  Total size of drive:            {0, 15} ", SizeSuffix(d.TotalSize)));
                    retorno.AppendLine(string.Format("  Root directory:            {0, 12}", d.RootDirectory));
                }
            }



            ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher("select * from Win32_Processor");

            foreach (ManagementObject obj in myProcessorObject.Get())
            {
                retorno.AppendLine(string.Format("Name  -  " + obj["Name"]));
                retorno.AppendLine(string.Format("DeviceID  -  " + obj["DeviceID"]));
                retorno.AppendLine(string.Format("Manufacturer  -  " + obj["Manufacturer"]));
                retorno.AppendLine(string.Format("CurrentClockSpeed  -  " + obj["CurrentClockSpeed"]));
                retorno.AppendLine(string.Format("Caption  -  " + obj["Caption"]));
                retorno.AppendLine(string.Format("NumberOfCores  -  " + obj["NumberOfCores"]));
                retorno.AppendLine(string.Format("NumberOfEnabledCore  -  " + obj["NumberOfEnabledCore"]));
                retorno.AppendLine(string.Format("NumberOfLogicalProcessors  -  " + obj["NumberOfLogicalProcessors"]));
                retorno.AppendLine(string.Format("Architecture  -  " + obj["Architecture"]));
                retorno.AppendLine(string.Format("Family  -  " + obj["Family"]));
                retorno.AppendLine(string.Format("ProcessorType  -  " + obj["ProcessorType"]));
                retorno.AppendLine(string.Format("Characteristics  -  " + obj["Characteristics"]));
                retorno.AppendLine(string.Format("AddressWidth  -  " + obj["AddressWidth"]));
            }


            ManagementObjectSearcher myOperativeSystemObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

            foreach (ManagementObject obj in myOperativeSystemObject.Get())
            {
                retorno.AppendLine(string.Format("Caption  -  " + obj["Caption"]));
                retorno.AppendLine(string.Format("WindowsDirectory  -  " + obj["WindowsDirectory"]));
                retorno.AppendLine(string.Format("ProductType  -  " + obj["ProductType"]));
                retorno.AppendLine(string.Format("SerialNumber  -  " + obj["SerialNumber"]));
                retorno.AppendLine(string.Format("SystemDirectory  -  " + obj["SystemDirectory"]));
                retorno.AppendLine(string.Format("CountryCode  -  " + obj["CountryCode"]));
                retorno.AppendLine(string.Format("CurrentTimeZone  -  " + obj["CurrentTimeZone"]));
                retorno.AppendLine(string.Format("EncryptionLevel  -  " + obj["EncryptionLevel"]));
                retorno.AppendLine(string.Format("OSType  -  " + obj["OSType"]));
                retorno.AppendLine(string.Format("Version  -  " + obj["Version"]));
            }


            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            if (nics == null || nics.Length < 1)
            {
                retorno.AppendLine(string.Format("  No network interfaces found."));
            }
            else
            {
                foreach (NetworkInterface adapter in nics)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    
                    retorno.AppendLine(string.Format(adapter.Description));
                    retorno.AppendLine(string.Format(String.Empty.PadLeft(adapter.Description.Length, '=')));
                    retorno.AppendLine(string.Format("  Interface type .......................... : {0}", adapter.NetworkInterfaceType));
                    retorno.AppendLine(string.Format("  Physical Address ........................ : {0}", adapter.GetPhysicalAddress().ToString()));
                    retorno.AppendLine(string.Format("  Operational status ...................... : {0}", adapter.OperationalStatus));
                }
            }


            ManagementObjectSearcher myAudioObject = new ManagementObjectSearcher("select * from Win32_SoundDevice");

            foreach (ManagementObject obj in myAudioObject.Get())
            {
                retorno.AppendLine(string.Format("Name  -  " + obj["Name"]));
                retorno.AppendLine(string.Format("ProductName  -  " + obj["ProductName"]));
                retorno.AppendLine(string.Format("Availability  -  " + obj["Availability"]));
                retorno.AppendLine(string.Format("DeviceID  -  " + obj["DeviceID"]));
                retorno.AppendLine(string.Format("PowerManagementSupported  -  " + obj["PowerManagementSupported"]));
                retorno.AppendLine(string.Format("Status  -  " + obj["Status"]));
                retorno.AppendLine(string.Format("StatusInfo  -  " + obj["StatusInfo"]));
                retorno.AppendLine(string.Format(String.Empty.PadLeft(obj["ProductName"].ToString().Length, '=')));
            }


            ManagementObjectSearcher myPrinterObject = new ManagementObjectSearcher("select * from Win32_Printer");

            foreach (ManagementObject obj in myPrinterObject.Get())
            {
                retorno.AppendLine(string.Format("Name  -  " + obj["Name"]));
                retorno.AppendLine(string.Format("Network  -  " + obj["Network"]));
                retorno.AppendLine(string.Format("Availability  -  " + obj["Availability"]));
                retorno.AppendLine(string.Format("Is default printer  -  " + obj["Default"]));
                retorno.AppendLine(string.Format("DeviceID  -  " + obj["DeviceID"]));
                retorno.AppendLine(string.Format("Status  -  " + obj["Status"]));

                retorno.AppendLine(string.Format(String.Empty.PadLeft(obj["Name"].ToString().Length, '=')));
            }

            retorno.Append(ShowNetworkInterfaces().ToString());
            return retorno;
        }

        public static StringBuilder ShowNetworkInterfaces()
        {
            StringBuilder retorno = new StringBuilder();
            
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            retorno.AppendLine(string.Format("Interface information for {0}.{1}     ",
                    computerProperties.HostName, computerProperties.DomainName));
            if (nics == null || nics.Length < 1)
            {
                retorno.AppendLine(string.Format("  No network interfaces found."));
                return retorno;
            }

            retorno.AppendLine(string.Format("  Number of interfaces .................... : {0}", nics.Length));

            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                
                retorno.AppendLine(string.Format(adapter.Description));
                retorno.AppendLine(string.Format(String.Empty.PadLeft(adapter.Description.Length, '=')));
                retorno.AppendLine(string.Format("  Interface type .......................... : {0}", adapter.NetworkInterfaceType));
                retorno.AppendLine(string.Format("  Physical Address ........................ : {0}",
                            adapter.GetPhysicalAddress().ToString()));
                retorno.AppendLine(string.Format("  Operational status ...................... : {0}",
                    adapter.OperationalStatus));
                string versions = "";

                // Create a display string for the supported IP versions.
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    versions = "IPv4";
                }
                if (adapter.Supports(NetworkInterfaceComponent.IPv6))
                {
                    if (versions.Length > 0)
                    {
                        versions += " ";
                    }
                    versions += "IPv6";
                }
                retorno.AppendLine(string.Format("  IP version .............................. : {0}", versions));
                //ShowIPAddresses(properties));

                // The following information is not useful for loopback adapters.
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }
                retorno.AppendLine(string.Format("  DNS suffix .............................. : {0}",
                    properties.DnsSuffix));

                string label;
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    IPv4InterfaceProperties ipv4 = properties.GetIPv4Properties();
                    retorno.AppendLine(string.Format("  MTU...................................... : {0}", ipv4.Mtu));
                    if (ipv4.UsesWins)
                    {

                        IPAddressCollection winsServers = properties.WinsServersAddresses;
                        if (winsServers.Count > 0)
                        {
                            label = "  WINS Servers ............................ :";
                            //ShowIPAddresses(label, winsServers));
                        }
                    }
                }

                retorno.AppendLine(string.Format("  DNS enabled ............................. : {0}",
                    properties.IsDnsEnabled));
                retorno.AppendLine(string.Format("  Dynamically configured DNS .............. : {0}",
                    properties.IsDynamicDnsEnabled));
                retorno.AppendLine(string.Format("  Receive Only ............................ : {0}",
                    adapter.IsReceiveOnly));
                retorno.AppendLine(string.Format("  Multicast ............................... : {0}",
                    adapter.SupportsMulticast));
            }

            return retorno;
        }

        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        static string SizeSuffix(Int64 value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }
    }
}
