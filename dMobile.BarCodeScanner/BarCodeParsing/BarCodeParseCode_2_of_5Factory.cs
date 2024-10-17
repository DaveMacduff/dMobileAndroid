// <copyright file="BarCodeParseCode_2_of_5Factory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Factory class for creating instances of <see cref="BarCodeParseCode_2_of_5"/>.
    /// </summary>
    public class BarCodeParseCode_2_of_5Factory : BarCodeParseFactory
    {
        /// <summary>
        /// Creates and returns a new instance of <see cref="BarCodeParseCode_2_of_5"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="BarCodeParseCode_2_of_5"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseCode_2_of_5();
        }
    }
}