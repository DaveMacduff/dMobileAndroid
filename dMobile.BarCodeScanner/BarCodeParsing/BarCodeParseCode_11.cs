// <copyright file="BarCodeParseCode_11.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Parsing rules reviewed
    ///
    /// A Code 11 Barcode has the following structure:
    ///
    /// 1. A start/stop character, encoded from the table below.
    /// 2. Any number of characters encoded from the table below.
    /// 3. An optional (but highly recommended) 'C' check digit calculated as described above and encoded from the table below.
    /// 4. An optional (but recommended for messages with a length greater than 10) 'K' check digit calculated as described above and encoded from the table below.
    /// 5. A start/stop character, encoded from the table below..
    ///
    /// CODE 11 ENCODING TABLE
    /// This table indicates how to encode each digit of a Code 11 bar code. Note that the "Width Encoding" column is expressed as "0"
    /// (narrow bar or space) or "1" (wide bar or space) while the "Barcode Encoding" column represents how the bar code will actually be
    /// encoded as described above in "Encoding the Symbol."
    ///
    /// For example, the character 0 is defined as "00001" by Code 11. This means a "Narrow bar, narrow space, narrow bar, narrow space, wide bar".
    /// We convert this to "101011" in the "Barcode Encoding" column which is consistent with the method we've used to express bar code formats in
    /// other documents on this site.
    ///
    ///    ASCII        WIDTH       BARCODE
    ///    CHARACTER    ENCODING    ENCODING
    ///    0            00001       101011
    ///    1            10001       1101011
    ///    2            01001       1001011
    ///    3            11000       1100101
    ///    4            00101       1011011
    ///    5            10100       1101101
    ///    6            01100       1001101
    ///    7            00011       1010011
    ///    8            10010       1101001
    ///    9            10000       110101
    ///    - (Dash)     00100       101101
    ///    EnableScanning/DisableScanning   00110       1011001.
    ///
    /// </summary>
    public class BarCodeParseCode_11 : BarCodeParse
    {
        /// <summary>
        /// Main call to parse the bar code data.
        /// </summary>
        /// <param name="barCodeData">Bar code data to parse.</param>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyProductIdentifier(barCodeData);

            this.Remainder = barCodeData;
        }

        /// <summary>
        /// Identify the product identifier.
        /// </summary>
        /// <param name="barCodeData"><see cref="string"/> Bar code data.</param>
        /// <returns>Remainder of the unparsable bar code data.</returns>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            this.ProductIdentifier = barCodeData;

            barCodeData = barCodeData.Remove(0, this.ProductIdentifier.Length);

            return barCodeData;
        }
    }
}
