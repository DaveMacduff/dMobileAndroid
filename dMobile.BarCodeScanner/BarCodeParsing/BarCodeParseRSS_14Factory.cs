// <copyright file="BarCodeParseRSS_14Factory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    class BarCodeParseRSS_14Factory : BarCodeParseFactory
    {
        /// <inheritdoc/>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseRSS_14();
        }
    }
}
