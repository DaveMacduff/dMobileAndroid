namespace dMobile.Bedrock
{
    using System;
    using System.Net.NetworkInformation;
    using Android;
    using Android.OS;
    using Android.Util;
    using Xamarin.Forms;

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

            try
            {
                if (!string.IsNullOrEmpty(Build.Product))
                {
                    deviceID += Build.Product;
                }
                else if (!string.IsNullOrEmpty(Build.Device))
                {
                    deviceID += Build.Device;
                }

                if (SDK_VERSION >= BuildVersionCodes.P)
                {
                    deviceID += Build.GetSerial();
                }
                else if (!string.IsNullOrEmpty(Build.Serial))
                {
                    deviceID += Build.Serial;
                }

                if (string.IsNullOrEmpty(deviceID) || deviceID.ToLower().Contains("unknown"))
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
            }
            catch (Exception ex)
            {
                Log.Error("GetDeviceID", "Error retrieving device ID. " + ex.Message);
            }

            return deviceID;
        }
    }
}
