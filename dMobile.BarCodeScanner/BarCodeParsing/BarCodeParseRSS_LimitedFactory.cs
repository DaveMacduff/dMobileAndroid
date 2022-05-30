// <copyright file="BarCodeParseRSS_LimitedFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Defines the <see cref="BarCodeParseRSS_LimitedFactory" />.
    /// </summary>
    public class BarCodeParseRSS_LimitedFactory : BarCodeParseFactory
    {
        /// <summary>
        /// The GetBarCodeParse.
        /// </summary>
        /// <returns>The <see cref="BarCodeParse"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseRSS_Limited();
        }
    }
}
