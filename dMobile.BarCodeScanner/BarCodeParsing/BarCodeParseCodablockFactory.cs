// <copyright file="BarCodeParseCodablockFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Defines a factory for creating instances of <see cref="BarCodeParseCodablock"/>.
    /// </summary>
    public class BarCodeParseCodablockFactory : BarCodeParseFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="BarCodeParseCodablock"/>.
        /// </summary>
        /// <returns>An instance of <see cref="BarCodeParse"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseCodablock();
        }
    }
}