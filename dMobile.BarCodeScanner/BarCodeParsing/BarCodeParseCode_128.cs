namespace dMobile.BarCodeScanner
{
  using System;
  using dMobile.Bedrock;

  public class BarCodeParseCode_128 : BarCodeParse
  {
    #region Public Members

    /// <summary>
    /// Parsing rules reviewed
    /// </summary>
    /// <param name="barCodeData"></param>
    /// <returns></returns>
    public override void ParseBarCodeData(string barCodeData)
    {
      this.Remainder = barCodeData;

      // WO 14686
      if (SysEnvironment.GetInstance().BarCodeSettings.IsRuleEnabled_Parse_Code128_CleanSubstituteCharacter &&
          !string.IsNullOrEmpty(barCodeData) &&
          barCodeData[0] == 0x001a)
      {
          barCodeData = barCodeData.Substring(1);
          this.Remainder = barCodeData;
      }


      if (!string.IsNullOrEmpty(Remainder))
      {
        if (this.Remainder.Length < 4)
        {
          if (this.Settings.IsRuleEnabled_ProductBarCodeMinimumLengthRequirement4Digits)
          {
#if MULTILANGUAGE
            throw new RuleViolationException(Translator.GetInstance().Translate(TranslatorVocabulary.BarCodeMinimumLengthRuleViolatedMustHaveALength) +
                Translator.GetInstance().Translate(TranslatorVocabulary.GreaterThan));
#else
            throw new RuleViolationException("Bar code minimum length rule violated, must have a length" +
                " greater than 4.");
#endif
          }
        }

        if (this.Settings.IsRuleEnabled_ParseNorthernCoop_AustraliaBeefTrim_Product25612
            && this.Remainder.Length >= 6
            && barCodeData.Substring(0, 6) == "256912")
        {
          this.ClearResetBarCodeProperties(false);
          BarCodeParseCode_128_NorthCoopMeat_Product256912 barCodeParse 
                = new BarCodeParseCode_128_NorthCoopMeat_Product256912();
          barCodeParse.ParseBarCodeData(barCodeData);
        }
        else if (this.Settings.IsRuleEnabled_ParseFINICA_Code_128_as_EAN_UCC_128
            && this.Remainder.Length >= 7
            && barCodeData.Substring(0, 7) == "9062852")
        {
          this.ClearResetBarCodeProperties(false);
          BarCodeParseCode_128_FINICA_as_EAN_UCC_128 barCodeParse = new BarCodeParseCode_128_FINICA_as_EAN_UCC_128();
          barCodeParse.ParseBarCodeData(barCodeData);
        }
        // 8767 is the Ovation Fielding pack site identifier WO# 11127
        else if (this.Settings.IsRuleEnabled_Parse_Code128_OvationFielding_Lamb
            && this.Remainder.Length == 32
            && barCodeData.Substring(5, 2) == "67"
            && barCodeData.Substring(30, 2) == "87")
        {
          this.ClearResetBarCodeProperties(false);
          BarCodeParseCode_128_OvationFielding_Lamb barCodeParse = new BarCodeParseCode_128_OvationFielding_Lamb();
          barCodeParse.ParseBarCodeData(barCodeData);
        }
        else if (this.Settings.IsRuleEnabled_ParseNossackBrokerMultiNationalProductFromChili
            && IsChiliBarcode(barCodeData))
        {
          this.ClearResetBarCodeProperties(false);
          BarCodeParseCode_128_BrokerMultiNationalFromChili barCodeParse 
              = new BarCodeParseCode_128_BrokerMultiNationalFromChili();
          barCodeParse.ParseBarCodeData(barCodeData);
        }
        else if (this.Settings.IsRuleEnabled_Parse_HelaSpiceCanada_Code_128
            && this.Remainder.Length >= 8
            && barCodeData.Substring(0, 8) == "10066973")
        {
          this.ClearResetBarCodeProperties(false);
          BarCodeParseUCC_EAN_128_Symbol barCodeParse = new BarCodeParseUCC_EAN_128_Symbol();

          // Added missing GTIN AI 01 and left filled the GTIN data with and extra 0 to make the length 14 digits.
          // Called the GS1 standard parsing routine.
          barCodeParse.ParseBarCodeData("010" + barCodeData);
        }
        else if (this.Settings.IsRuleEnabled_ParseCode_128AsUCC_EAN_128_SymbolCompliant)
        {
          this.ClearResetBarCodeProperties(false);
          BarCodeParseUCC_EAN_128_Symbol barCodeParse = new BarCodeParseUCC_EAN_128_Symbol();
          barCodeParse.ParseBarCodeData(barCodeData);
        }
        else
        {
          // false do not reset symbology data, just reset the barcode parsed data
          this.ClearResetBarCodeProperties(false);

          barCodeData = this.IdentifyProductIdentifier(barCodeData);

          this.Remainder = barCodeData;
        }
      }
    }

    public bool IsChiliBarcode(string barcode)
    {
        return barcode.Length >= 8
            && barcode.Substring(2, 6) == "896775";
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
      int len = 0;

      if (barCodeData.Length < 14)
      {
        this.ProductIdentifier = barCodeData.PadLeft(14, '0');
        len = barCodeData.Length;
      }
      else if (barCodeData.Length > 14)
      {
        this.ProductIdentifier = barCodeData.Substring(0, 14);
        len = 14;
      }
      else
      {
        this.ProductIdentifier = barCodeData;
        len = barCodeData.Length;
      }

      barCodeData = barCodeData.Remove(0, len);

      return barCodeData;
    }

    #endregion
  }
}
