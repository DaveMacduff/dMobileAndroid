// <copyright file="IHoneywellScannerService.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Services
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the interface for Honeywell scanner services.
    /// </summary>
    public interface IHoneywellScannerService
    {
        /// <summary>
        /// Event triggered when the profile data is ready.
        /// </summary>
        event EventHandler ProfileDataReady;

        /// <summary>
        /// Gets or sets the barcode reader settings.
        /// </summary>
        Dictionary<string, object> BarcodeReaderSettings { get; set; }

        /// <summary>
        /// Retrieves the scanner profile.
        /// </summary>
        void GetScannerProfile();
    }
}