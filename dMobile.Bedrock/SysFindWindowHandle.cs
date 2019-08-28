namespace dMobile.Bedrock.Settings
{
  using System.Runtime.InteropServices;


  /// <summary>
    ///Dim h As Integer = FindWindow("HHTaskBar", "")
    ///ShowWindow(h, 0)
    ///EnableWindow(h, False)
    ///Me.WindowState = FormWindowState.Maximized
    ///Update()
    ///Me.Height = Screen.PrimaryScreen.Bounds.Height
    ///Update()
    /// </summary>
    /// <remarks></remarks>
    public class SysFindWindowHandle
    {
        public const int SW_HIDEWINDOW = 0x0;

        public const int SW_SHOWNORMAL = 0x1;
        [DllImport("coredll.dll", CharSet = CharSet.Auto)]
        public static extern int FindWindow(string lpClassName, string lpWindowName);
        [DllImport("coredll.dll", CharSet = CharSet.Auto)]
        public static extern bool ShowWindow(int hwnd, int nCmdShow);
        [DllImport("coredll.dll", CharSet = CharSet.Auto)]
        public static extern bool EnableWindow(int hwnd, bool enabled);
    }
}
