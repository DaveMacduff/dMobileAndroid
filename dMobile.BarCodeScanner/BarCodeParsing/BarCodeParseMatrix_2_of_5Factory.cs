// <copyright file="BarCodeParseMatrix_2_of_5Factory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Factory class for creating instances of <see cref="BarCodeParseMatrix_2_of_5"/>.
    /// </summary>
    public class BarCodeParseMatrix_2_of_5Factory : BarCodeParseFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="BarCodeParseMatrix_2_of_5"/>.
        /// </summary>
        /// <returns>An instance of <see cref="BarCodeParse"/>.</returns>
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseMatrix_2_of_5();
        }
    }
}