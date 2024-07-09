// <copyright file="BarCodeParseTelepen.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    ///     Bar code parsing for Telepen symbology.
    /// </summary>
    public class BarCodeParseTelepen : BarCodeParse
    {
        /// <summary>
        ///     Parse bar code data.
        /// </summary>
        /// <param name="barCodeData"> Bar code parse data.</param>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyProductIdentifier(barCodeData);

            this.Remainder = barCodeData;
        }
    }
}
