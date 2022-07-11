// <copyright file="SysDeviceID.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using Android.Media;
    using Android.OS;
    using Java.Security;
    using Java.Util;

    /// <summary>
    /// Defines the <see cref="SysDeviceID" />.
    /// </summary>
    public class SysDeviceID
    {
        /// <summary>
        /// Get the Google Wide Vine SN.
        /// </summary>
        /// <returns><see cref="string"/> Wide vine serial number.</returns>
        public static string GetWidevineSN()
        {
            string sRet = string.Empty;
            UUID wideVineUUID = new UUID(-0x121074568629b532L, -0x5c37d8232ae2de13L);
            MediaDrm mediaDrm = null;

            try
            {
                mediaDrm = new MediaDrm(wideVineUUID);
                byte[] widevineId = mediaDrm.GetPropertyByteArray(MediaDrm.PropertyDeviceUniqueId);

                MessageDigest md = MessageDigest.GetInstance("SHA-256");
                md.Update(widevineId);
                sRet = BitConverter.ToString(md.Digest());
                sRet = sRet.Replace("-", string.Empty);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                {
                    if (mediaDrm != null)
                    {
                        mediaDrm.Close();
                    }
                }
                else
                {
                    if (mediaDrm != null)
                    {
                        ////mediaDrm.Release();
                        mediaDrm.Dispose();
                    }
                }
            }

            return sRet;
        }

        /// <summary>
        /// The GetDeviceID.
        /// </summary>
        /// <returns>.</returns>
        public static string GetDeviceID()
        {
            return GetWidevineSN();
        }
    }
}
