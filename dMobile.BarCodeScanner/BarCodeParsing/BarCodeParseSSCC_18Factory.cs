// <copyright file="BarCodeParseSSCC_18Factory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Factory class for creating instances of <see cref="BarCodeParseSSCC_18"/>.
    /// </summary>
    public class BarCodeParseSSCC_18Factory : BarCodeParseFactory
    {
        /// <summary>
        /// Creates and returns a new instance of <see cref="BarCodeParseSSCC_18"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="BarCodeParseSSCC_18"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseSSCC_18();
        }
    }
}