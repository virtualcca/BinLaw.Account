using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.System.Profile;
using Windows.UI.Xaml;

namespace BinLaw.Account.FE.Foundation.Helper
{
    public static class DeviceHelper
    {
        /// <summary>
        /// Get current package version
        /// </summary>
        /// <Author>
        /// BinLaw
        /// </Author>
        /// <returns>Current package version</returns>
        public static string GetCurrentAppVersion()
        {
            var version = Package.Current.Id.Version;
            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        public static bool IsNetworkAvailable
        {
            get
            {
                return NetworkInterface.GetIsNetworkAvailable();
            }
        }
        public static HardwareToken GetHardwareToken()
        {
            return HardwareIdentification.GetPackageSpecificToken(null);
        }

    }
}
