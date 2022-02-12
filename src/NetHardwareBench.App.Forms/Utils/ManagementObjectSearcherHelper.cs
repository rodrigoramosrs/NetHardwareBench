using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace NetHardwareBench.App.Forms.Utils
{
    public static class ManagementObjectSearcherHelper
    {
        public static ManagementObjectCollection GetManagedInformation(string Query)
        {

            using (ManagementObjectSearcher customObject = new ManagementObjectSearcher(Query))
                return customObject.Get();
        }


        public static StringBuilder GetSimpleManagedInformation(string Query, string PropertyName)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in GetManagedInformation(Query))
            {
                builder.AppendLine(item[PropertyName].ToString());
            }
            return builder;
        }
    }
}
