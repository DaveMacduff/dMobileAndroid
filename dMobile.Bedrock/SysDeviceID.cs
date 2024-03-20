// <copyright file="SysDeviceId.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="SysDeviceId" />.
    /// </summary>
    public class SysDeviceId
    {
        /// <summary>
        /// Get the Google Wide Vine SN.
        /// </summary>
        /// <returns><see cref="string"/> Wide vine serial number.</returns>
        public static string GetWidevineSn()
        {
            var sRet = string.Empty;
            var wideVineUuid = new UUID(-0x121074568629b532L, -0x5c37d8232ae2de13L);
            MediaDrm mediaDrm = null;

            try
            {
                mediaDrm = new MediaDrm(wideVineUuid);
                var widevineId = mediaDrm.GetPropertyByteArray(MediaDrm.PropertyDeviceUniqueId);

                var md = MessageDigest.GetInstance("SHA-256");
                if (md != null)
                {
                    md.Update(widevineId);
                    sRet = BitConverter.ToString(md.Digest() ?? Array.Empty<byte>());
                }

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
        /// The GetDeviceId.
        /// </summary>
        /// <returns>.</returns>
        public static string GetDeviceId()
        {
            return GetWidevineSn();
        }
    }
}
