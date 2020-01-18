namespace dMobile.BarCodeScanner
{
    using System;

    /// <summary>
    /// Codabar parsing
    /// </summary>
    /// <remarks>
    /// <para>
    /// Parsing rules reviewed
    /// </para><para>
    /// A Code 11 Barcode has the following structure:
    /// 1. One of four possible start characters (A, B, C, or D), encoded from the table below.
    /// 2. A narrow, inter-character space.
    /// 3. The data of the message, encoded from the table below, with a narrow inter-character space 
    /// between each character.
    /// 4. One of four possible stop characters (A, B, C, or D), encoded from the table below. 
    /// </para><para>
    /// This table indicates how to encode each digit of a Codabar barcode. Note that the "Width Encoding" 
    /// column is expressed as "0" (narrow bar or space) or "1" (wide bar or space) while the "Barcode 
    /// Encoding" column represents how the barcode will actually be encoded as described above in 
    /// "Encoding the Symbol."
    /// For example, the character 0 is defined as "0000011" by Codabar. This means a "Narrow bar, narrow 
    /// space, narrow bar, narrow space, narrow bar, wide space, wide bar". We convert this to 101010011 
    /// in the "Barcode Encoding" column which is consistent with the method we've used to express 
    /// barcode formats in other documents on this site.
    /// </para><para>
    /// ASCII
    /// CHARACTER	    WIDTH
    /// ENCODING	    BARCODE
    /// ASCII         WIDTH     BARCODE
    /// CHARACTER     ENCODING  ENCODING
    /// 0	            0000011	  101010011
    /// 1	            0000110	  101011001
    /// 2	            0001001	  101001011
    /// 3	            1100000	  110010101
    /// 4	            0010010	  101101001
    /// 5	            1000010	  110101001
    /// 6	            0100001	  100101011
    /// 7	            0100100	  100101101
    /// 8             0110000	  100110101
    /// 9	            1001000   110100101
    /// - (Dash)	    0001100   101001101
    /// $	            0011000   101100101
    /// : (Colon)     1000101   1101011011
    /// (Slash)       1010001   1101101011
    /// . (Point)     1010100   1101101101
    /// + (Plus)      0011111   101100110011
    /// Start/Stop A  0011010   1011001001
    /// Start/Stop B  0001011   1010010011
    /// Start/Stop C  0101001   1001001011
    /// Start/Stop D  0001110   1010011001
    /// </para><para>
    /// We will now code the example barcode, A40156B. Of course, the "A" refers to the 'A' Start/Stop 
    /// character and the 'B' refers to the 'B' Start/Stop character.
    /// </para><para>
    ///  1. The 'A' START/STOP character: 1011001001.
    ///  2. An intercharacter space encoded as 0.
    ///  3. The digit "4": enocded as 101101001.
    ///  4. An intercharacter space encoded as 0.
    ///  5. The digit "0": enocded as 101010011.
    ///  6. An intercharacter space encoded as 0.
    ///  7. The digit "1": enocded as 101011001.
    ///  8. An intercharacter space encoded as 0.
    ///  9. The digit "5": enocded as 101101001.
    /// 10. An intercharacter space encoded as 0.
    /// 11. The digit "6": enocded as 100101011.
    /// 12. An intercharacter space encoded as 0.
    /// 13. The 'B' START/STOP: enocded as 1010010011. 
    /// </para></remarks>
    public class BarCodeParseCodabar : BarCodeParse
    {
        #region Public Members

        /// <summary>
        /// Main call to parse the bar code data.
        /// </summary>
        /// <param name="barCodeData">Bar code data to parse</param>
        /// <returns>Remainder of the unparsable bar code data</returns>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyProductIdentifier(barCodeData);

            this.Remainder = barCodeData;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            this.ProductIdentifier = barCodeData;

            // remove the product identifier
            barCodeData = barCodeData.Remove(0, this.ProductIdentifier.Length);

            return barCodeData;
        }

        #endregion
    }
}
