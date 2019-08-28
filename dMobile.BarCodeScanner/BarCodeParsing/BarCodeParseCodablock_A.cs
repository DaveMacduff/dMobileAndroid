namespace dMobile.BarCodeScanner
{
  using System;

  /// <summary>
  /// Parsing rules reviewed
  /// 
  /// Codablock A is a stacked version of ICS (IdentCode-System). Invented by Heinrich Oehlmann, 
  /// it was originally a stacked version of Code 39.
  /// Each Codablock symbol contains between 1 and 22 rows. The number of characters per row is set according to the 
  /// standardized size of the smallest bar. Each row contains a variable number of characters.
  ///Each symbol has a group of bars for the start character and the end character, which extends the height of the
  ///symbol. Each row has two characters indicating the row number, and the last row has an optional check character.
  ///The code is a continuous series of rows of variable size, encoded in Code 39, and is no more dense than a given
  ///density of Code 39.
  /// </summary>
  public class BarCodeParseCodablock_A : BarCodeParse
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="barCodeData"></param>
    /// <returns></returns>
    public override void ParseBarCodeData(string barCodeData)
    {
      barCodeData = this.IdentifyProductIdentifier(barCodeData);

      this.Remainder = barCodeData;
    }
  }
}
