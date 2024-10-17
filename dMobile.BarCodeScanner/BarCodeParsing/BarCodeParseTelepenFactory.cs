// <copyright file="BarCodeParseTelepenFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Factory class for creating instances of <see cref="BarCodeParseTelepen"/>.
    /// </summary>
    public class BarCodeParseTelepenFactory : BarCodeParseFactory
    {
        /// <summary>
        /// Creates and returns a new instance of <see cref="BarCodeParseTelepen"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="BarCodeParseTelepen"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseTelepen();
        }
    }
}