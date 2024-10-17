// <copyright file="BarCodeParseMSIFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Factory class for creating instances of <see cref="BarCodeParseMSI"/>.
    /// </summary>
    public class BarCodeParseMSIFactory : BarCodeParseFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="BarCodeParseMSI"/>.
        /// </summary>
        /// <returns>An instance of <see cref="BarCodeParse"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseMSI();
        }
    }
}