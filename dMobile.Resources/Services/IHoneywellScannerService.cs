// <copyright file="IHoneywellScannerService.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Services
{
    using System;
    using System.Collections.Generic;

    public interface IHoneywellScannerService
    {
        event EventHandler ProfileDataReady;
        void GetScannerProfile();
        Dictionary<string, object> BarcodeReaderSettings { get; set; }
    }
}
