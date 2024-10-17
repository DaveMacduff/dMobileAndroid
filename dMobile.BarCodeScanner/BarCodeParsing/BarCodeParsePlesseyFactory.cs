// <copyright file="BarCodeParsePlesseyFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Factory class for creating instances of <see cref="BarCodeParsePlessey"/>.
    /// </summary>
    public class BarCodeParsePlesseyFactory : BarCodeParseFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="BarCodeParsePlessey"/>.
        /// </summary>
        /// <returns>An instance of <see cref="BarCodeParse"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParsePlessey();
        }
    }
}