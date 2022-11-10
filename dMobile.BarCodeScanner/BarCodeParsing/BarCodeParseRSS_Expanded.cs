// <copyright file="BarCodeParseRSS_Expanded.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    public class BarCodeParseRSS_Expanded : BarCodeParse
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyProductIdentifier(barCodeData);

            this.Remainder = barCodeData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            this.ProductIdentifier = barCodeData;

            barCodeData = barCodeData.Remove(0, this.ProductIdentifier.Length);

            return barCodeData;
        }
    }
}
