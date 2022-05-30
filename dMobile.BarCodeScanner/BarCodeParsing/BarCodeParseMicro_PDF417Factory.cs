// <copyright file="BarCodeParseMicro_PDF417Factory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Defines the <see cref="BarCodeParseMicro_PDF417Factory" />.
    /// </summary>
    public class BarCodeParseMicro_PDF417Factory : BarCodeParseFactory
    {
        /// <summary>
        /// The GetBarCodeParse.
        /// </summary>
        /// <returns>The <see cref="BarCodeParse"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseMicro_PDF417();
        }
    }
}
