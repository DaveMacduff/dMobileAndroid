// <copyright file="BarCodeParseEAN_UPC_SymbolFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Defines the <see cref="BarCodeParseEAN_UPC_SymbolFactory" />.
    /// </summary>
    public class BarCodeParseEAN_UPC_SymbolFactory : BarCodeParseFactory
    {
        /// <summary>
        /// The GetBarCodeParse.
        /// </summary>
        /// <returns>The <see cref="BarCodeParse"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseEAN_UPC_Symbol();
        }
    }
}
