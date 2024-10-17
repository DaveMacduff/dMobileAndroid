// <copyright file="BarCodeParseCodabar.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Codabar parsing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Parsing rules reviewed.
    /// </para><para>
    /// A Code 11 Barcode has the following structure:
    /// 1. One of four possible start characters (A, B, C, or D), encoded from the table below.
    /// 2. A narrow, inter-character space.
    /// 3. The data of the message, encoded from the table below, with a narrow inter-character space
    /// between each character.
    /// 4. One of four possible stop characters (A, B, C, or D), encoded from the table below.
    /// </para><para>
    /// This table indicates how to encode each digit of a Codabar bar code. Note that the "Width Encoding"
    /// column is expressed as "0" (narrow bar or space) or "1" (wide bar or space) while the "Barcode
    /// Encoding" column represents how the bar code will actually be encoded as described above in
    /// "Encoding the Symbol."
    /// For example, the character 0 is defined as "0000011" by Codabar. This means a "Narrow bar, narrow
    /// space, narrow bar, narrow space, narrow bar, wide space, wide bar". We convert this to 101010011
    /// in the "Barcode Encoding" column which is consistent with the method we've used to express
    /// bar code formats in other documents on this site.
    /// </para><para>
    /// ASCII
    /// CHARACTER     WIDTH
    /// ENCODING      BARCODE
    /// ASCII         WIDTH     BARCODE
    /// CHARACTER     ENCODING
    /// 0             0000011   101010011
    /// 1             0000110   101011001
    /// 2             0001001   101001011
    /// 3             1100000   110010101
    /// 4             0010010   101101001
    /// 5             1000010   110101001
    /// 6             0100001   100101011
    /// 7             0100100   100101101
    /// 8             0110000   100110101
    /// 9             1001000   110100101
    /// - (Dash)      0001100   101001101
    /// $             0011000   101100101
    /// : (Colon)     1000101   1101011011
    /// (Slash)       1010001   1101101011
    /// . (Point)     1010100   1101101101
    /// + (Plus)      0011111   101100110011
    /// EnableScanning/DisableScanning A  0011010   1011001001
    /// EnableScanning/DisableScanning B  0001011   1010010011
    /// EnableScanning/DisableScanning C  0101001   1001001011
    /// EnableScanning/DisableScanning D  0001110   1010011001.
    /// </para><para>
    /// We will now code the example bar code, A40156B. Of course, the "A" refers to the 'A' EnableScanning/DisableScanning
    /// character and the 'B' refers to the 'B' EnableScanning/DisableScanning character.
    /// </para><para>
    ///  1. The 'A' START/STOP character: 1011001001.
    ///  2. An intercharacter space encoded as 0.
    ///  3. The digit "4": encoded as 101101001.
    ///  4. An intercharacter space encoded as 0.
    ///  5. The digit "0": encoded as 101010011.
    ///  6. An intercharacter space encoded as 0.
    ///  7. The digit "1": encoded as 101011001.
    ///  8. An intercharacter space encoded as 0.
    ///  9. The digit "5": encoded as 101101001.
    /// 10. An intercharacter space encoded as 0.
    /// 11. The digit "6": encoded as 100101011.
    /// 12. An intercharacter space encoded as 0.
    /// 13. The 'B' START/STOP: encoded as 1010010011.
    /// </para></remarks>
    public class BarCodeParseCodabar : BarCodeParse
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

            // remove the product identifier
            barCodeData = barCodeData.Remove(0, this.ProductIdentifier.Length);

            return barCodeData;
        }
    }
}
