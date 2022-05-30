// <copyright file="clsCommon.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner
{
    using System.Reflection;

    /// <summary>
    /// Common bar code scanner class methods.
    /// </summary>
    public class clsCommon
    {
        /// <summary>
        /// Get the bar code scanner assembly information.
        /// </summary>
        /// <returns><see cref="string"/> Assembly information.</returns>
        public string GetAssemblyVersion()
        {
            int major = Assembly.GetExecutingAssembly().GetName().Version.Major;
            int minor = Assembly.GetExecutingAssembly().GetName().Version.Minor;
            int build = Assembly.GetExecutingAssembly().GetName().Version.Build;
            int revision = Assembly.GetExecutingAssembly().GetName().Version.Revision;
            return string.Format("{0}.{1}.{2}.{3}", major, minor, build, revision);
        }
    }
}
