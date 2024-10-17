// <copyright file="BarCodeParseUnDefinedFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Factory class for creating instances of <see cref="BarCodeParseUnDefined"/>.
    /// </summary>
    public class BarCodeParseUnDefinedFactory : BarCodeParseFactory
    {
        /// <summary>
        /// Creates and returns a new instance of <see cref="BarCodeParseUnDefined"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="BarCodeParse"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseUnDefined();
        }
    }
}