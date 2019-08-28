namespace dMobile.BarCodeScanner
{
  using System;
  using dMobile.Bedrock;

  public class BarCodeParseFacade
  {
    // vars are assigned in IdentifySymbology
    private static string RawData = string.Empty;
    private static string Prefix = string.Empty;
    private static string Data = string.Empty;
    private static int Length = 0;
    private static SysCommon.BarCodeSymbologies Symbology = SysCommon.BarCodeSymbologies.UNDEFINED;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="strPData"></param>
    /// <returns></returns>
    public static BarCodeParse ParseData(string pData)
    {
      BarCodeParseFactory parseFactory = null;
      BarCodeParse parse = null;

      IdentifySymbology(pData);

      switch (Symbology)
      {
        case SysCommon.BarCodeSymbologies.UNDEFINED:
          parseFactory = new BarCodeParseUnDefinedFactory();
          break;

        case SysCommon.BarCodeSymbologies.UCC_12_UPC_E:
          parseFactory = new BarCodeParseUCC_12_UPC_EFactory();
          break;

        case SysCommon.BarCodeSymbologies.EAN_UCC_14:
          parseFactory = new BarCodeParseEAN_UCC_14Factory();
          break;

        case SysCommon.BarCodeSymbologies.EAN_UCC_13:
          parseFactory = new BarCodeParseEAN_UCC_13Factory();
          break;

        case SysCommon.BarCodeSymbologies.EAN_UCC_8:
          parseFactory = new BarCodeParseEAN_UCC_8Factory();
          break;

        case SysCommon.BarCodeSymbologies.ITF_14_Symbol:
          parseFactory = new BarCodeParseITF_14_SymbolFactory();
          break;

        case SysCommon.BarCodeSymbologies.UCC_EAN_128_Symbol:
          parseFactory = new BarCodeParseUCC_EAN_128_SymbolFactory();
          break;

        case SysCommon.BarCodeSymbologies.Code_128:
          parseFactory = new BarCodeParseCode_128Factory();
          break;

        case SysCommon.BarCodeSymbologies.SSCC_18:
          parseFactory = new BarCodeParseSSCC_18Factory();
          break;

        case SysCommon.BarCodeSymbologies.Code_39:
          parseFactory = new BarCodeParseCode_39Factory();
          break;

        case SysCommon.BarCodeSymbologies.EAN_UPC_Symbol:
          parseFactory = new BarCodeParseEAN_UPC_SymbolFactory();
          break;

        case SysCommon.BarCodeSymbologies.Code_93:
          parseFactory = new BarCodeParseCode_93Factory();
          break;

        case SysCommon.BarCodeSymbologies.Codabar:
          parseFactory = new BarCodeParseCodabarFactory();
          break;

        case SysCommon.BarCodeSymbologies.Code_2_of_5:
          parseFactory = new BarCodeParseCode_2_of_5Factory();
          break;

        case SysCommon.BarCodeSymbologies.MSI:
          parseFactory = new BarCodeParseMSIFactory();
          break;

        case SysCommon.BarCodeSymbologies.Plessey:
          parseFactory = new BarCodeParsePlesseyFactory();
          break;

        case SysCommon.BarCodeSymbologies.Code_11:
          parseFactory = new BarCodeParseCode_11Factory();
          break;

        case SysCommon.BarCodeSymbologies.Matrix_2_of_5:
          parseFactory = new BarCodeParseMatrix_2_of_5Factory();
          break;

        case SysCommon.BarCodeSymbologies.Telepen:
          parseFactory = new BarCodeParseTelepenFactory();
          break;

        case SysCommon.BarCodeSymbologies.PDF417:
          parseFactory = new BarCodeParsePDF417Factory();
          break;

        case SysCommon.BarCodeSymbologies.Micro_PDF417:
          parseFactory = new BarCodeParseMicro_PDF417Factory();
          break;

        case SysCommon.BarCodeSymbologies.RSS_14:
          parseFactory = new BarCodeParseRSS_14Factory();
          break;

        case SysCommon.BarCodeSymbologies.RSS_Limited:
          parseFactory = new BarCodeParseRSS_LimitedFactory();
          break;

        case SysCommon.BarCodeSymbologies.RSS_Expanded:
          parseFactory = new BarCodeParseRSS_ExpandedFactory();
          break;

        case SysCommon.BarCodeSymbologies.Codablock_A:
          parseFactory = new BarCodeParseCodablock_AFactory();
          break;

        case SysCommon.BarCodeSymbologies.Codablock_F:
          parseFactory = new BarCodeParseCodablock_FFactory();
          break;

        case SysCommon.BarCodeSymbologies.TLC_39:
          parseFactory = new BarCodeParseTLC_39Factory();
          break;

        case SysCommon.BarCodeSymbologies.Codablock:
          parseFactory = new BarCodeParseCodablockFactory();
          break;

        case SysCommon.BarCodeSymbologies.Codablock256:
          parseFactory = new BarCodeParseCodablock256Factory();
          break;
      }

      // Create generic bar code parse object
      if (parseFactory != null)
      {
        parse = parseFactory.GetBarCodeParse();
        parse.ClearResetBarCodeProperties(true);
        parse.Settings = parse.Settings.LoadData();
        parse.Raw = RawData;
        parse.SymbologyPrefix = Prefix;
        parse.Data = Data;
        parse.Length = Length;
        parse.Symbology = Symbology;
        parse.ParseBarCodeData(Data);
      }
      else
      {
#if MULTILANGUAGE
        throw new NotSupportedException(Translator.GetInstance().Translate(TranslatorVocabulary.UnsupportedBarCodeSymbology));
#else
        throw new NotSupportedException("Unsupported Bar Code Symbology");
#endif
      }

      return parse;
    }

    // public string GetSymbologyPrefixFromSymbologyEnum(BarCodeSymbologies eSymbol)
    // {
    //  string strSymbologyPrefix = string.Empty;

    //  switch (eSymbol)
    //  {
    //    case BarCodeSymbologies.UNDEFINED:
    //      strSymbologyPrefix = "]01";   // or ']O4'
    //      break;
    //    case BarCodeSymbologies.UCC_12_UPC_E:
    //      strSymbologyPrefix = "]X0";
    //      break;
    //    case BarCodeSymbologies.EAN_UCC_14:
    //      strSymbologyPrefix = string.Empty;
    //      break;
    //    case BarCodeSymbologies.EAN_UCC_13:
    //      strSymbologyPrefix = "]E0";
    //      break;
    //    case BarCodeSymbologies.EAN_UCC_8:
    //      strSymbologyPrefix = "]E4";
    //      break;
    //    case BarCodeSymbologies.ITF_14_Symbol:
    //      strSymbologyPrefix = "]I";
    //      break;
    //    case BarCodeSymbologies.UCC_EAN_128_Symbol:
    //      strSymbologyPrefix = "]C1";
    //      break;
    //    case BarCodeSymbologies.Code_128:
    //      strSymbologyPrefix = "]C";
    //      break;
    //    case BarCodeSymbologies.SSCC_18:
    //      strSymbologyPrefix = string.Empty;
    //      break;
    //    case BarCodeSymbologies.Code_39:
    //      strSymbologyPrefix = "]A";
    //      break;
    //    case BarCodeSymbologies.EAN_UPC_Symbol:
    //      strSymbologyPrefix = "]E";
    //      break;
    //    case BarCodeSymbologies.Code_93:
    //      strSymbologyPrefix = "]G";
    //      break;
    //    case BarCodeSymbologies.Codabar:
    //      strSymbologyPrefix = "]F";
    //      break;
    //    case BarCodeSymbologies.Code_2_of_5:
    //      strSymbologyPrefix = string.Empty;
    //      break;
    //    case BarCodeSymbologies.MSI:
    //      strSymbologyPrefix = "]M";
    //      break;
    //    case BarCodeSymbologies.Plessey:
    //      strSymbologyPrefix = string.Empty;
    //      break;
    //    case BarCodeSymbologies.Code_11:
    //      strSymbologyPrefix = "]H";
    //      break;
    //    case BarCodeSymbologies.Matrix_2_of_5:
    //      strSymbologyPrefix = string.Empty;
    //      break;
    //    case BarCodeSymbologies.Telepen:
    //      strSymbologyPrefix = "]B";
    //      break;
    //    case BarCodeSymbologies.PDF417:
    //      strSymbologyPrefix = "]L";
    //      break;
    //    case BarCodeSymbologies.Micro_PDF417:
    //      strSymbologyPrefix = string.Empty;
    //      break;
    //    case BarCodeSymbologies.RSS_14:
    //      strSymbologyPrefix = string.Empty;
    //      break;
    //    case BarCodeSymbologies.RSS_Limited:
    //      strSymbologyPrefix = string.Empty;
    //      break;
    //    case BarCodeSymbologies.RSS_Expanded:
    //      strSymbologyPrefix = "]e0";
    //      break;
    //    case BarCodeSymbologies.Codablock_A:
    //      strSymbologyPrefix = "]O6";
    //      break;
    //    case BarCodeSymbologies.Codablock_F:
    //      strSymbologyPrefix = "]O5";
    //      break;
    //    case BarCodeSymbologies.TLC_39:
    //      strSymbologyPrefix = string.Empty;
    //      break;
    //    case BarCodeSymbologies.Codablock:
    //      strSymbologyPrefix = "]O";
    //      break;
    //    case BarCodeSymbologies.Codablock256:
    //      strSymbologyPrefix = "]02";
    //      break;
    //    default:
    //      strSymbologyPrefix = string.Empty;
    //      break;
    //  }

    //  return strSymbologyPrefix;
    // }

    /// <summary>
    /// Parses out the Symbology, RawData, BarCodeData and assigns the BarCodeLength
    /// </summary>
    /// <param name="rawData"></param>
    private static void IdentifySymbology(string rawData)
    {
      RawData = rawData;

      // Make sure we have the Prefixed symbology information
      if (RawData.StartsWith("]"))
      {
        // Parse out the Prefix
        if (!string.IsNullOrEmpty(RawData.Substring(0, 3)))
        {
          // First 3 Characters are the Prefix
          Prefix = RawData.Substring(0, 3);

          if (!string.IsNullOrEmpty(rawData.Substring(Prefix.Length, rawData.Length - Prefix.Length)))
          {
            // The Remainder is the actual barcode
            Data = rawData.Substring(Prefix.Length, rawData.Length - Prefix.Length);
            Length = Data.Length;

            if (Prefix.StartsWith("]O"))
            {
              Symbology = SysCommon.BarCodeSymbologies.Codablock;

              if (Prefix == "]O1")
              {
                Symbology = SysCommon.BarCodeSymbologies.UNDEFINED;
              }
              else if (Prefix == "]02")
              {
                Symbology = SysCommon.BarCodeSymbologies.Codablock256;
              }
              else if (Prefix == "]O4")
              {
                Symbology = SysCommon.BarCodeSymbologies.UNDEFINED;
              }
              else if (Prefix == "]O5")
              {
                Symbology = SysCommon.BarCodeSymbologies.Codablock_F;
              }
              else if (Prefix == "]O6")
              {
                Symbology = SysCommon.BarCodeSymbologies.Codablock_A;
              }
            }
            else if (Prefix.StartsWith("]C"))
            {
              Symbology = SysCommon.BarCodeSymbologies.Code_128;
              if (Prefix == "]C1")
              {
                // "UCC/EAN-128"
                Symbology = SysCommon.BarCodeSymbologies.UCC_EAN_128_Symbol;
              }
            }
            else if (Prefix.StartsWith("]G"))
            {
              Symbology = SysCommon.BarCodeSymbologies.Code_93;
            }
            else if (Prefix.StartsWith("]A"))
            {
              Symbology = SysCommon.BarCodeSymbologies.Code_39;
            }
            else if (Prefix.StartsWith("]H"))
            {
              Symbology = SysCommon.BarCodeSymbologies.Code_11;
            }
            else if (Prefix.StartsWith("]F"))
            {
              Symbology = SysCommon.BarCodeSymbologies.Codabar;
            }
            else if (Prefix.StartsWith("]E"))
            {
              Symbology = SysCommon.BarCodeSymbologies.EAN_UPC_Symbol;
              if (Prefix == "]E0")
              {
                // EAN 13
                Symbology = SysCommon.BarCodeSymbologies.EAN_UCC_13;
              }
              else if (Prefix == "]E4")
              {
                // EAN-8                
                Symbology = SysCommon.BarCodeSymbologies.EAN_UCC_8;
              }
            }
            else if (Prefix.StartsWith("]I"))
            {
              // "Interleaved 2 of 5" 'SCC-14
              Symbology = SysCommon.BarCodeSymbologies.ITF_14_Symbol;
            }
            else if (Prefix.StartsWith("]M"))
            {
              Symbology = SysCommon.BarCodeSymbologies.MSI;
            }
            else if (Prefix.StartsWith("]L"))
            {
              Symbology = SysCommon.BarCodeSymbologies.PDF417;
            }
            else if (Prefix.StartsWith("]e0"))
            {
              Symbology = SysCommon.BarCodeSymbologies.RSS_Expanded;
            }
            else if (Prefix.StartsWith("]B"))
            {
              Symbology = SysCommon.BarCodeSymbologies.Telepen;
            }
            else if (Prefix.StartsWith("]X0"))
            {
              // UPC-E
              Symbology = SysCommon.BarCodeSymbologies.UCC_12_UPC_E;
            }
            else
            {
              Symbology = SysCommon.BarCodeSymbologies.UNDEFINED;
            }
          }
        }
      }
      else
      {
          Symbology = SysCommon.BarCodeSymbologies.UNDEFINED;

          // Otherwise, doesn't have the Symbology info
          if (!string.IsNullOrEmpty(rawData))
          {
              // The Remainder is the actual barcode
              Data = rawData;
              Length = Data.Length;
          }
          else
          {
              // still need to override the fields if null or empty
              rawData = string.Empty;
              Data = string.Empty;
              Length = 0;
          }
      }
    }
  }
}
