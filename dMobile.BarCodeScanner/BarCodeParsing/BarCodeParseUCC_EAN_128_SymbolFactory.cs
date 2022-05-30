// <copyright file="BarCodeParseUCC_EAN_128_SymbolFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Defines the <see cref="BarCodeParseUCC_EAN_128_SymbolFactory" />.
    /// </summary>
    public class BarCodeParseUCC_EAN_128_SymbolFactory : BarCodeParseFactory
    {
        /// <summary>
        /// The GetBarCodeParse.
        /// </summary>
        /// <returns>The <see cref="BarCodeParse"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseUCC_EAN_128_Symbol();
        }
    }
}
