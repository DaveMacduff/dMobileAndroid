namespace dMobile.Bedrock
{
    using System.Net.NetworkInformation;

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

            // Return OpenNETCF.WindowsCE.DeviceManagement.GetDeviceID()
            //// Return OpenNETCF.WindowsCE.DeviceManagement.GetDeviceGuid().ToString()

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
                //TODO: Get Device ID
                //deviceID = OpenNETCF.WindowsCE.DeviceManagement.GetDeviceID();
            }

            return deviceID;
        }
    }
}
