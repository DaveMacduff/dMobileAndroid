using System;
using System.Collections.Generic;

namespace dMobile.Resources.Services
{
    public interface IHoneywellScannerService
    {
        event EventHandler ProfileDataReady;
        void GetScannerProfile();
        Dictionary<string, object> BarcodeReaderSettings { get; set; }
    }
}
