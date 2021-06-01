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
            BuildVersionCodes SDK_VERSION = Build.VERSION.SdkInt;

            if (!string.IsNullOrEmpty(Build.Product))
            {
                deviceID += Build.Product;
            }
            else if (!string.IsNullOrEmpty(Build.Device))
            {
                deviceID += Build.Device;
            }

            // TODO: If we start targeting a higher sdk, this will be changed to Build.getSerial().
            // This will require additional device permissions.
            if (SDK_VERSION > BuildVersionCodes.M)
            {
                if (!string.IsNullOrEmpty(Build.GetSerial()))
                {
                    deviceID += Build.GetSerial();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Build.Serial))
                {
                    deviceID += Build.Serial;
                }
            }

            if (string.IsNullOrEmpty(deviceID))
            {
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
            }

            return deviceID;
        }
    }
}
