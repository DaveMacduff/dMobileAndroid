namespace dMobile.Bedrock
{
    using System;
    using System.Net.NetworkInformation;
    using Android.OS;

    public class SysDeviceID
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>LXE device id's are not unique. Therefore we are switching to the first MAC address</remarks>
        public static string GetDeviceID()
        {
            string deviceID = string.Empty;
            NetworkInterface nic = null;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                nic = ni;
                break;
            }

            if (nic != null)
            {
                deviceID = nic.GetPhysicalAddress().ToString();
            }

            // If then MAC address is not known then fall back to device id
            if (string.IsNullOrEmpty(deviceID))
            {
                string id = string.Empty;

                if (!string.IsNullOrEmpty(Build.Device))
                {
                    id += Build.Device;
                }

                // TODO: If we start targeting a higher sdk, this will be changed to Build.getSerial().
                // This will require additional device permissions.
                if (!string.IsNullOrEmpty(Build.Serial))
                {
                    id += Build.Serial;
                }

                deviceID = id;
                //Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            }

            return deviceID;
        }
    }
}
