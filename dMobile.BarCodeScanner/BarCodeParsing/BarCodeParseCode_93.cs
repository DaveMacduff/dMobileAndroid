// <copyright file="BarCodeParseCode_93.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    /// Parsing rules reviewed.
    /// </summary>
    public class BarCodeParseCode_93 : BarCodeParse
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyProductIdentifier(barCodeData);
            this.Remainder = barCodeData;
        }
    }
}
