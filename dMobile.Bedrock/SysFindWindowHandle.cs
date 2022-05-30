// <copyright file="SysFindWindowHandle.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock.Settings
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Dim h As Integer = FindWindow("HHTaskBar", "")
    ///ShowWindow(h, 0)
    ///EnableWindow(h, False)
    ///Me.WindowState = FormWindowState.Maximized
    ///Update()
    ///Me.Height = Screen.PrimaryScreen.Bounds.Height
    ///Update().
    /// </summary>
    public class SysFindWindowHandle
    {
        /// <summary>
        /// Defines the SW_HIDEWINDOW.
        /// </summary>
        public const int SW_HIDEWINDOW = 0x0;

        /// <summary>
        /// Defines the SW_SHOWNORMAL.
        /// </summary>
        public const int SW_SHOWNORMAL = 0x1;

        /// <summary>
        /// The FindWindow.
        /// </summary>
        /// <param name="lpClassName">The lpClassName<see cref="string"/>.</param>
        /// <param name="lpWindowName">The lpWindowName<see cref="string"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        [DllImport("coredll.dll", CharSet = CharSet.Auto)]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// The ShowWindow.
        /// </summary>
        /// <param name="hwnd">The hwnd<see cref="int"/>.</param>
        /// <param name="nCmdShow">The nCmdShow<see cref="int"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        [DllImport("coredll.dll", CharSet = CharSet.Auto)]
        public static extern bool ShowWindow(int hwnd, int nCmdShow);

        /// <summary>
        /// The EnableWindow.
        /// </summary>
        /// <param name="hwnd">The hwnd<see cref="int"/>.</param>
        /// <param name="enabled">The enabled<see cref="bool"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        [DllImport("coredll.dll", CharSet = CharSet.Auto)]
        public static extern bool EnableWindow(int hwnd, bool enabled);
    }
}
