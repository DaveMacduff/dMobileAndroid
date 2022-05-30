// <copyright file="BarCodeScannerEventArgs.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    using System;
    using dMobile.BarCodeScanner.BarCodeParsing;

    /// <summary>
    /// This is the event arguments for the BarCode Scanner class event BarCodeScan.
    /// </summary>
    public class BarCodeScannerEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarCodeScannerEventArgs"/> class.
        /// </summary>
        /// <param name="strPData"><see cref="string"/> Scanner data stream to parse.</param>
        public BarCodeScannerEventArgs(string strPData)
        {
            this.ParseInfo = BarCodeParseFacade.ParseData(strPData);
        }

        /// <summary>
        /// Gets or sets the bar code parsing object.
        /// </summary>
        public BarCodeParse ParseInfo { get; set; }
    }
}
