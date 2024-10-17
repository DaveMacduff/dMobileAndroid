// <copyright file="BarCodeParseCode_128Factory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Factory class for creating instances of <see cref="BarCodeParseCode_128"/>.
    /// </summary>
    public class BarCodeParseCode_128Factory : BarCodeParseFactory
    {
        /// <summary>
        /// Creates and returns a new instance of <see cref="BarCodeParseCode_128"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="BarCodeParseCode_128"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseCode_128();
        }
    }
}