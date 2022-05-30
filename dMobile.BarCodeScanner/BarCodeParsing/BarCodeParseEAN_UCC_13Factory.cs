// <copyright file="BarCodeParseEAN_UCC_13Factory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Defines the <see cref="BarCodeParseEAN_UCC_13Factory" />.
    /// </summary>
    public class BarCodeParseEAN_UCC_13Factory : BarCodeParseFactory
    {
        /// <summary>
        /// The GetBarCodeParse.
        /// </summary>
        /// <returns>The <see cref="BarCodeParse"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseEAN_UCC_13();
        }
    }
}
