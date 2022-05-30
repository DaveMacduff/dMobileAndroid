// <copyright file="BarCodeParseUCC_12_UPC_EFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Defines the <see cref="BarCodeParseUCC_12_UPC_EFactory" />.
    /// </summary>
    public class BarCodeParseUCC_12_UPC_EFactory : BarCodeParseFactory
    {
        /// <summary>
        /// The GetBarCodeParse.
        /// </summary>
        /// <returns>The <see cref="BarCodeParse"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseUCC_12_UPC_E();
        }
    }
}
