// <copyright file="BarCodeParseFacade.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    /// Bar code parsing facade. Create a bar code parse instance.
    /// </summary>
    public static class BarCodeParseFacade
    {
        /// <summary>
        /// Gets or sets the raw data received from the scanner engine.
        /// </summary>
        private static string RawData { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the bar code symbology prefix.
        /// </summary>
        private static string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the raw data with the symbology prefix removed.
        /// </summary>
        private static string Data { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the data length.
        /// </summary>
        private static int Length { get; set; }

        /// <summary>
        /// Gets or sets the symbology identification.
        /// </summary>
        private static BarCodeSymbology Symbology { get; set; } = BarCodeSymbology.UNDEFINED;

        /// <summary>
        /// Parse the bar code scan data.
        /// </summary>
        /// <param name="pData"><see cref="string"/> Raw scan engine read data.</param>
        /// <returns><see cref="BarCodeParse"/> Parsed bar code data information.</returns>
        public static BarCodeParse ParseData(string pData)
        {
            IdentifySymbology(pData);

            BarCodeParseFactory parseFactory = Symbology switch
            {
                BarCodeSymbology.UNDEFINED => new BarCodeParseUnDefinedFactory(),
                BarCodeSymbology.UCC_12_UPC_E => new BarCodeParseUCC_12_UPC_EFactory(),
                BarCodeSymbology.EAN_UCC_14 => new BarCodeParseEAN_UCC_14Factory(),
                BarCodeSymbology.EAN_UCC_13 => new BarCodeParseEAN_UCC_13Factory(),
                BarCodeSymbology.EAN_UCC_8 => new BarCodeParseEAN_UCC_8Factory(),
                BarCodeSymbology.ITF_14_Symbol => new BarCodeParseITF_14_SymbolFactory(),
                BarCodeSymbology.UCC_EAN_128_Symbol =>
                    new BarCodeParseUCC_EAN_128_SymbolFactory(),
                BarCodeSymbology.Code_128 => new BarCodeParseCode_128Factory(),
                BarCodeSymbology.SSCC_18 => new BarCodeParseSSCC_18Factory(),
                BarCodeSymbology.Code_39 => new BarCodeParseCode_39Factory(),
                BarCodeSymbology.EAN_UPC_Symbol => new BarCodeParseEAN_UPC_SymbolFactory(),
                BarCodeSymbology.Code_93 => new BarCodeParseCode_93Factory(),
                BarCodeSymbology.Codabar => new BarCodeParseCodabarFactory(),
                BarCodeSymbology.Code_2_of_5 => new BarCodeParseCode_2_of_5Factory(),
                BarCodeSymbology.MSI => new BarCodeParseMSIFactory(),
                BarCodeSymbology.Plessey => new BarCodeParsePlesseyFactory(),
                BarCodeSymbology.Code_11 => new BarCodeParseCode_11Factory(),
                BarCodeSymbology.Matrix_2_of_5 => new BarCodeParseMatrix_2_of_5Factory(),
                BarCodeSymbology.Telepen => new BarCodeParseTelepenFactory(),
                BarCodeSymbology.PDF417 => new BarCodeParsePDF417Factory(),
                BarCodeSymbology.Micro_PDF417 => new BarCodeParseMicro_PDF417Factory(),
                BarCodeSymbology.RSS_14 => new BarCodeParseRSS_14Factory(),
                BarCodeSymbology.RSS_Limited => new BarCodeParseRSS_LimitedFactory(),
                BarCodeSymbology.RSS_Expanded => new BarCodeParseRSS_ExpandedFactory(),
                BarCodeSymbology.Codablock_A => new BarCodeParseCodablock_AFactory(),
                BarCodeSymbology.Codablock_F => new BarCodeParseCodablock_FFactory(),
                BarCodeSymbology.TLC_39 => new BarCodeParseTLC_39Factory(),
                BarCodeSymbology.Codablock => new BarCodeParseCodablockFactory(),
                BarCodeSymbology.Codablock256 => new BarCodeParseCodablock256Factory(),
                _ => null
            };

            BarCodeParse parse;

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
                throw new NotSupportedException("Unsupported Bar Code Symbology");
            }

            return parse;
        }

        //// public string GetSymbologyPrefixFromSymbologyEnum(BarCodeSymbology eSymbol)
        //// {
        ////  string strSymbologyPrefix = string.Empty;

        //// switch (eSymbol)
        ////  {
        ////    case BarCodeSymbology.UNDEFINED:
        ////      strSymbologyPrefix = "]01";   // or ']O4'
        ////      break;
        ////    case BarCodeSymbology.UCC_12_UPC_E:
        ////      strSymbologyPrefix = "]X0";
        ////      break;
        ////    case BarCodeSymbology.EAN_UCC_14:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////    case BarCodeSymbology.EAN_UCC_13:
        ////      strSymbologyPrefix = "]E0";
        ////      break;
        ////    case BarCodeSymbology.EAN_UCC_8:
        ////      strSymbologyPrefix = "]E4";
        ////      break;
        ////    case BarCodeSymbology.ITF_14_Symbol:
        ////      strSymbologyPrefix = "]I";
        ////      break;
        ////    case BarCodeSymbology.UCC_EAN_128_Symbol:
        ////      strSymbologyPrefix = "]C1";
        ////      break;
        ////    case BarCodeSymbology.Code_128:
        ////      strSymbologyPrefix = "]C";
        ////      break;
        ////    case BarCodeSymbology.SSCC_18:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////    case BarCodeSymbology.Code_39:
        ////      strSymbologyPrefix = "]A";
        ////      break;
        ////    case BarCodeSymbology.EAN_UPC_Symbol:
        ////      strSymbologyPrefix = "]E";
        ////      break;
        ////    case BarCodeSymbology.Code_93:
        ////      strSymbologyPrefix = "]G";
        ////      break;
        ////    case BarCodeSymbology.Codabar:
        ////      strSymbologyPrefix = "]F";
        ////      break;
        ////    case BarCodeSymbology.Code_2_of_5:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////    case BarCodeSymbology.MSI:
        ////      strSymbologyPrefix = "]M";
        ////      break;
        ////    case BarCodeSymbology.Plessey:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////    case BarCodeSymbology.Code_11:
        ////      strSymbologyPrefix = "]H";
        ////      break;
        ////    case BarCodeSymbology.Matrix_2_of_5:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////    case BarCodeSymbology.Telepen:
        ////      strSymbologyPrefix = "]B";
        ////      break;
        ////    case BarCodeSymbology.PDF417:
        ////      strSymbologyPrefix = "]L";
        ////      break;
        ////    case BarCodeSymbology.Micro_PDF417:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////    case BarCodeSymbology.RSS_14:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////    case BarCodeSymbology.RSS_Limited:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////    case BarCodeSymbology.RSS_Expanded:
        ////      strSymbologyPrefix = "]e0";
        ////      break;
        ////    case BarCodeSymbology.Codablock_A:
        ////      strSymbologyPrefix = "]O6";
        ////      break;
        ////    case BarCodeSymbology.Codablock_F:
        ////      strSymbologyPrefix = "]O5";
        ////      break;
        ////    case BarCodeSymbology.TLC_39:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////    case BarCodeSymbology.Codablock:
        ////      strSymbologyPrefix = "]O";
        ////      break;
        ////    case BarCodeSymbology.Codablock256:
        ////      strSymbologyPrefix = "]02";
        ////      break;
        ////    default:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////  }

        //// return strSymbologyPrefix;
        //// }

        /// <summary>
        /// Parses out the Symbology, RawData, BarCodeData and assigns the BarCodeLength.
        /// </summary>
        /// <param name="rawData"><see cref="string"/> Raw data stream from scan engine.</param>
        private static void IdentifySymbology(string rawData)
        {
            RawData = rawData;

            // Make sure we have the Prefixed symbology information
            if (RawData.StartsWith("]"))
            {
                // Parse out the Prefix
                if (!string.IsNullOrEmpty(RawData[..3]))
                {
                    // First 3 Characters are the Prefix
                    Prefix = RawData[..3];

                    if (!string.IsNullOrEmpty(rawData[Prefix.Length..]))
                    {
                        // The Remainder is the actual bar code.
                        Data = rawData[Prefix.Length..];
                        Length = Data.Length;

                        if (Prefix.StartsWith("]O"))
                        {
                            Symbology = BarCodeSymbology.Codablock;

                            if (Prefix == "]O1")
                            {
                                Symbology = BarCodeSymbology.UNDEFINED;
                            }
                            else if (Prefix == "]02")
                            {
                                Symbology = BarCodeSymbology.Codablock256;
                            }
                            else if (Prefix == "]O4")
                            {
                                Symbology = BarCodeSymbology.UNDEFINED;
                            }
                            else if (Prefix == "]O5")
                            {
                                Symbology = BarCodeSymbology.Codablock_F;
                            }
                            else if (Prefix == "]O6")
                            {
                                Symbology = BarCodeSymbology.Codablock_A;
                            }
                        }
                        else if (Prefix.StartsWith("]C"))
                        {
                            Symbology = BarCodeSymbology.Code_128;
                            if (Prefix == "]C1")
                            {
                                // "UCC/EAN-128"
                                Symbology = BarCodeSymbology.UCC_EAN_128_Symbol;
                            }
                        }
                        else if (Prefix.StartsWith("]G"))
                        {
                            Symbology = BarCodeSymbology.Code_93;
                        }
                        else if (Prefix.StartsWith("]A"))
                        {
                            Symbology = BarCodeSymbology.Code_39;
                        }
                        else if (Prefix.StartsWith("]H"))
                        {
                            Symbology = BarCodeSymbology.Code_11;
                        }
                        else if (Prefix.StartsWith("]F"))
                        {
                            Symbology = BarCodeSymbology.Codabar;
                        }
                        else if (Prefix.StartsWith("]E"))
                        {
                            Symbology = BarCodeSymbology.EAN_UPC_Symbol;
                            if (Prefix == "]E0")
                            {
                                // EAN 13
                                Symbology = BarCodeSymbology.EAN_UCC_13;
                            }
                            else if (Prefix == "]E4")
                            {
                                // EAN-8
                                Symbology = BarCodeSymbology.EAN_UCC_8;
                            }
                        }
                        else if (Prefix.StartsWith("]I"))
                        {
                            // "Interleaved 2 of 5" 'SCC-14
                            Symbology = BarCodeSymbology.ITF_14_Symbol;
                        }
                        else if (Prefix.StartsWith("]M"))
                        {
                            Symbology = BarCodeSymbology.MSI;
                        }
                        else if (Prefix.StartsWith("]L"))
                        {
                            Symbology = BarCodeSymbology.PDF417;
                        }
                        else if (Prefix.StartsWith("]e0"))
                        {
                            Symbology = BarCodeSymbology.RSS_Expanded;
                        }
                        else if (Prefix.StartsWith("]B"))
                        {
                            Symbology = BarCodeSymbology.Telepen;
                        }
                        else if (Prefix.StartsWith("]X0"))
                        {
                            // UPC-E
                            Symbology = BarCodeSymbology.UCC_12_UPC_E;
                        }
                        else
                        {
                            Symbology = BarCodeSymbology.UNDEFINED;
                        }
                    }
                }
            }
            else
            {
                Symbology = BarCodeSymbology.UNDEFINED;

                // Otherwise, doesn't have the Symbology info
                if (!string.IsNullOrEmpty(rawData))
                {
                    // The Remainder is the actual bar code.
                    Data = rawData;
                    Length = Data.Length;
                }
                else
                {
                    // still need to override the fields if null or empty
                    Data = string.Empty;
                    Length = 0;
                }
            }
        }
    }
}
