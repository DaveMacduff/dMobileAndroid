// <copyright file="BarCodeParseCode_93Factory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Factory class for creating instances of <see cref="BarCodeParseCode_93"/>.
    /// </summary>
    public class BarCodeParseCode_93Factory : BarCodeParseFactory
    {
        /// <summary>
        /// Creates and returns a new instance of <see cref="BarCodeParseCode_93"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="BarCodeParseCode_93"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseCode_93();
        }
    }
}