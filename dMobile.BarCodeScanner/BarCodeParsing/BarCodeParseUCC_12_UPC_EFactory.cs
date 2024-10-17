// <copyright file="BarCodeParseUCC_12_UPC_EFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Factory class for creating instances of <see cref="BarCodeParseUCC_12_UPC_E"/>.
    /// </summary>
    public class BarCodeParseUCC_12_UPC_EFactory : BarCodeParseFactory
    {
        /// <summary>
        /// Creates and returns a new instance of <see cref="BarCodeParseUCC_12_UPC_E"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="BarCodeParseUCC_12_UPC_E"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseUCC_12_UPC_E();
        }
    }
}