// <copyright file="BarCodeParseUCC_EAN_128_SymbolFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Factory class for creating instances of <see cref="BarCodeParseUCC_EAN_128_Symbol"/>.
    /// </summary>
    public class BarCodeParseUCC_EAN_128_SymbolFactory : BarCodeParseFactory
    {
        /// <summary>
        /// Creates and returns a new instance of <see cref="BarCodeParseUCC_EAN_128_Symbol"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="BarCodeParse"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseUCC_EAN_128_Symbol();
        }
    }
}