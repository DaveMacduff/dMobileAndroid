// <copyright file="BarCodeParseEAN_UCC_8Factory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Factory class for creating instances of <see cref="BarCodeParseEAN_UCC_8"/>.
    /// </summary>
    public class BarCodeParseEAN_UCC_8Factory : BarCodeParseFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="BarCodeParseEAN_UCC_8"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="BarCodeParseEAN_UCC_8"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseEAN_UCC_8();
        }
    }
}