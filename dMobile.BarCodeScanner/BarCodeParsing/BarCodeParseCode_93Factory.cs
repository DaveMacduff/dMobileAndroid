// <copyright file="BarCodeParseCode_93Factory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Defines the <see cref="BarCodeParseCode_93Factory" />.
    /// </summary>
    public class BarCodeParseCode_93Factory : BarCodeParseFactory
    {
        /// <summary>
        /// The GetBarCodeParse.
        /// </summary>
        /// <returns>The <see cref="BarCodeParse"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseCode_93();
        }
    }
}
