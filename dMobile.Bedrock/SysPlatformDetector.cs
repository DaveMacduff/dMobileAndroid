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

    //  Try
    //    MessageBox.Show("Platform is " + PlatformDetector.GetPlatform().ToString())
    //  Catch ex As Exception
    //    MessageBox.Show(ex.Message.ToString())
    //  End Try

    // End Sub

    public enum Platform
    {
        PocketPC,
        WindowsCE,
        Smartphone,
        Win32NT,
        Win32S,
        Win32Windows,
        Unknown,
        WinCE7
    }

    public class SysPlatformDetector
    {
        [DllImport("Coredll.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SystemParametersInfo(
            uint uiAction,
            uint uiParam,
            StringBuilder pvParam,
            uint fWinIni);

        private static uint SPI_GETPLATFORMTYPE = 257;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
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
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
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
