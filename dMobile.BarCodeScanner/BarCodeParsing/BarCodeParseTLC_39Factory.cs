// <copyright file="BarCodeParseTLC_39Factory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Factory class for creating instances of <see cref="BarCodeParseTLC_39"/>.
    /// </summary>
    public class BarCodeParseTLC_39Factory : BarCodeParseFactory
    {
        /// <summary>
        /// Creates and returns a new instance of <see cref="BarCodeParseTLC_39"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="BarCodeParseTLC_39"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseTLC_39();
        }
    }
}