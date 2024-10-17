// <copyright file="BarCodeParseEAN_UPC_SymbolFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Factory class for creating instances of <see cref="BarCodeParseEAN_UPC_Symbol"/>.
    /// </summary>
    public class BarCodeParseEAN_UPC_SymbolFactory : BarCodeParseFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="BarCodeParseEAN_UPC_Symbol"/>.
        /// </summary>
        /// <returns>An instance of <see cref="BarCodeParse"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseEAN_UPC_Symbol();
        }
    }
}