// <copyright file="SysPlatformDetector.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    // Add the following declaration and code for the form's Load event.
    // Declare Function KernelIoControl Lib "CoreDll.dll" _
    //    (ByVal dwIoControlCode As Int32, _
    //     ByVal lpInBuf As IntPtr, _
    //     ByVal nInBufSize As Int32, _
    //     ByVal lpOutBuf() As Byte, _
    //     ByVal nOutBufSize As Int32, _
    //     ByRef lpBytesReturned As Int32) As Boolean

    // Private Sub DeviceType_Load(ByVal sender As Object, ByVal e As EventArgs) _
    //    Handles MyBase.Load

    // Try
    //    MessageBox.Show("Platform is " + PlatformDetector.GetPlatform().ToString())
    //  Catch ex As Exception
    //    MessageBox.Show(ex.Message.ToString())
    //  End Try

    // End Sub
    public enum Platform
    {
        /// <summary>
        /// Defines the PocketPC.
        /// </summary>
        PocketPC,

        /// <summary>
        /// Defines the WindowsCE.
        /// </summary>
        WindowsCE,

        /// <summary>
        /// Defines the Smartphone.
        /// </summary>
        Smartphone,

        /// <summary>
        /// Defines the Win32NT.
        /// </summary>
        Win32NT,

        /// <summary>
        /// Defines the Win32S.
        /// </summary>
        Win32S,

        /// <summary>
        /// Defines the Win32Windows.
        /// </summary>
        Win32Windows,

        /// <summary>
        /// Defines the Unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// Defines the WinCE7.
        /// </summary>
        WinCE7,
    }

    /// <summary>
    /// Defines the <see cref="SysPlatformDetector" />.
    /// </summary>
    public class SysPlatformDetector
    {
        /// <summary>
        /// The SystemParametersInfo.
        /// </summary>
        /// <param name="uiAction">The uiAction<see cref="uint"/>.</param>
        /// <param name="uiParam">The uiParam<see cref="uint"/>.</param>
        /// <param name="pvParam">The pvParam<see cref="StringBuilder"/>.</param>
        /// <param name="fWinIni">The fWinIni<see cref="uint"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        [DllImport("Coredll.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SystemParametersInfo(
            uint uiAction,
            uint uiParam,
            StringBuilder pvParam,
            uint fWinIni);

        /// <summary>
        /// Defines the SPI_GETPLATFORMTYPE.
        /// </summary>
        private static uint SPI_GETPLATFORMTYPE = 257;

        /// <summary>
        /// The GetPlatform.
        /// </summary>
        /// <returns>.</returns>
        public static Platform GetPlatform()
        {
            Platform plat = Platform.Unknown;
            switch (System.Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    plat = Platform.Win32NT;

                    break;

                case PlatformID.Win32S:
                    plat = Platform.Win32S;

                    break;

                case PlatformID.Win32Windows:
                    plat = Platform.Win32Windows;

                    break;

                case PlatformID.WinCE:
                    plat = CheckWinCEPlatform();

                    break;
            }

            return plat;
        }

        /// <summary>
        /// The CheckWinCEPlatform.
        /// </summary>
        /// <returns>.</returns>
        public static Platform CheckWinCEPlatform()
        {
            Platform plat = Platform.WindowsCE;
            StringBuilder strbuild = new StringBuilder(200);

            SystemParametersInfo(SPI_GETPLATFORMTYPE, 200, strbuild, 0);

            string str = strbuild.ToString();
            switch (str)
            {
                case "PocketPC":
                    plat = Platform.PocketPC;

                    break;

                case "SmartPhone":
                    // Note that the strbuild parameter from the
                    // PInvoke returns "SmartPhone" with an
                    // upper case P. The correct casing is
                    // "Smartphone" with a lower case p.
                    plat = Platform.Smartphone;

                    break;

                case "Motorola WinCE":
                    plat = Platform.WinCE7;

                    break;
            }

            return plat;
        }
    }
}
