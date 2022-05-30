// <copyright file="BarCodeParseFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// This is the abstract bar code parsing class factory.
    /// </summary>
    public abstract class BarCodeParseFactory
    {
        /// <summary>
        /// Get the parsed bar code data.
        /// </summary>
        /// <returns><see cref="BarCodeParse"/> Parsed bar code data information.</returns>
        public abstract BarCodeParse GetBarCodeParse();
    }
}
