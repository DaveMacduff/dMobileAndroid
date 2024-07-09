// <copyright file="BarCodeParseFacade.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    /// Bar code parsing facade. Create an bar code parse instance.
    /// </summary>
    public static class BarCodeParseFacade
    {
        /// <summary>
        /// Gets or sets the raw data received from the scanner engine.
        /// </summary>
        private static string RawData { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the the bar code symbology prefix.
        /// </summary>
        private static string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the raw data with the symbology prefix removed.
        /// </summary>
        private static string Data { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the data length.
        /// </summary>
        private static int Length { get; set; } = 0;

        /// <summary>
        /// Gets or sets the symbology identification.
        /// </summary>
        private static BarCodeSymbologies Symbology { get; set; } = BarCodeSymbologies.UNDEFINED;

        /// <summary>
        /// Parse the bar code scan data.
        /// </summary>
        /// <param name="pData"><see cref="string"/> Raw scan engine read data.</param>
        /// <returns><see cref="BarCodeParse"/> Parsed bar code data information.</returns>
        public static BarCodeParse ParseData(string pData)
        {
            BarCodeParseFactory parseFactory = null;
            IdentifySymbology(pData);

            switch (Symbology)
            {
                case BarCodeSymbologies.UNDEFINED:
                    parseFactory = new BarCodeParseUnDefinedFactory();
                    break;

                case BarCodeSymbologies.UCC_12_UPC_E:
                    parseFactory = new BarCodeParseUCC_12_UPC_EFactory();
                    break;

                case BarCodeSymbologies.EAN_UCC_14:
                    parseFactory = new BarCodeParseEAN_UCC_14Factory();
                    break;

                case BarCodeSymbologies.EAN_UCC_13:
                    parseFactory = new BarCodeParseEAN_UCC_13Factory();
                    break;

                case BarCodeSymbologies.EAN_UCC_8:
                    parseFactory = new BarCodeParseEAN_UCC_8Factory();
                    break;

                case BarCodeSymbologies.ITF_14_Symbol:
                    parseFactory = new BarCodeParseITF_14_SymbolFactory();
                    break;

                case BarCodeSymbologies.UCC_EAN_128_Symbol:
                    parseFactory = new BarCodeParseUCC_EAN_128_SymbolFactory();
                    break;

                case BarCodeSymbologies.Code_128:
                    parseFactory = new BarCodeParseCode_128Factory();
                    break;

                case BarCodeSymbologies.SSCC_18:
                    parseFactory = new BarCodeParseSSCC_18Factory();
                    break;

                case BarCodeSymbologies.Code_39:
                    parseFactory = new BarCodeParseCode_39Factory();
                    break;

                case BarCodeSymbologies.EAN_UPC_Symbol:
                    parseFactory = new BarCodeParseEAN_UPC_SymbolFactory();
                    break;

                case BarCodeSymbologies.Code_93:
                    parseFactory = new BarCodeParseCode_93Factory();
                    break;

                case BarCodeSymbologies.Codabar:
                    parseFactory = new BarCodeParseCodabarFactory();
                    break;

                case BarCodeSymbologies.Code_2_of_5:
                    parseFactory = new BarCodeParseCode_2_of_5Factory();
                    break;

                case BarCodeSymbologies.MSI:
                    parseFactory = new BarCodeParseMSIFactory();
                    break;

                case BarCodeSymbologies.Plessey:
                    parseFactory = new BarCodeParsePlesseyFactory();
                    break;

                case BarCodeSymbologies.Code_11:
                    parseFactory = new BarCodeParseCode_11Factory();
                    break;

                case BarCodeSymbologies.Matrix_2_of_5:
                    parseFactory = new BarCodeParseMatrix_2_of_5Factory();
                    break;

                case BarCodeSymbologies.Telepen:
                    parseFactory = new BarCodeParseTelepenFactory();
                    break;

                case BarCodeSymbologies.PDF417:
                    parseFactory = new BarCodeParsePDF417Factory();
                    break;

                case BarCodeSymbologies.Micro_PDF417:
                    parseFactory = new BarCodeParseMicro_PDF417Factory();
                    break;

                case BarCodeSymbologies.RSS_14:
                    parseFactory = new BarCodeParseRSS_14Factory();
                    break;

                case BarCodeSymbologies.RSS_Limited:
                    parseFactory = new BarCodeParseRSS_LimitedFactory();
                    break;

                case BarCodeSymbologies.RSS_Expanded:
                    parseFactory = new BarCodeParseRSS_ExpandedFactory();
                    break;

                case BarCodeSymbologies.Codablock_A:
                    parseFactory = new BarCodeParseCodablock_AFactory();
                    break;

                case BarCodeSymbologies.Codablock_F:
                    parseFactory = new BarCodeParseCodablock_FFactory();
                    break;

                case BarCodeSymbologies.TLC_39:
                    parseFactory = new BarCodeParseTLC_39Factory();
                    break;

                case BarCodeSymbologies.Codablock:
                    parseFactory = new BarCodeParseCodablockFactory();
                    break;

                case BarCodeSymbologies.Codablock256:
                    parseFactory = new BarCodeParseCodablock256Factory();
                    break;
            }

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

        //// public string GetSymbologyPrefixFromSymbologyEnum(BarCodeSymbologies eSymbol)
        //// {
        ////  string strSymbologyPrefix = string.Empty;

        //// switch (eSymbol)
        ////  {
        ////    case BarCodeSymbologies.UNDEFINED:
        ////      strSymbologyPrefix = "]01";   // or ']O4'
        ////      break;
        ////    case BarCodeSymbologies.UCC_12_UPC_E:
        ////      strSymbologyPrefix = "]X0";
        ////      break;
        ////    case BarCodeSymbologies.EAN_UCC_14:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////    case BarCodeSymbologies.EAN_UCC_13:
        ////      strSymbologyPrefix = "]E0";
        ////      break;
        ////    case BarCodeSymbologies.EAN_UCC_8:
        ////      strSymbologyPrefix = "]E4";
        ////      break;
        ////    case BarCodeSymbologies.ITF_14_Symbol:
        ////      strSymbologyPrefix = "]I";
        ////      break;
        ////    case BarCodeSymbologies.UCC_EAN_128_Symbol:
        ////      strSymbologyPrefix = "]C1";
        ////      break;
        ////    case BarCodeSymbologies.Code_128:
        ////      strSymbologyPrefix = "]C";
        ////      break;
        ////    case BarCodeSymbologies.SSCC_18:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////    case BarCodeSymbologies.Code_39:
        ////      strSymbologyPrefix = "]A";
        ////      break;
        ////    case BarCodeSymbologies.EAN_UPC_Symbol:
        ////      strSymbologyPrefix = "]E";
        ////      break;
        ////    case BarCodeSymbologies.Code_93:
        ////      strSymbologyPrefix = "]G";
        ////      break;
        ////    case BarCodeSymbologies.Codabar:
        ////      strSymbologyPrefix = "]F";
        ////      break;
        ////    case BarCodeSymbologies.Code_2_of_5:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////    case BarCodeSymbologies.MSI:
        ////      strSymbologyPrefix = "]M";
        ////      break;
        ////    case BarCodeSymbologies.Plessey:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////    case BarCodeSymbologies.Code_11:
        ////      strSymbologyPrefix = "]H";
        ////      break;
        ////    case BarCodeSymbologies.Matrix_2_of_5:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////    case BarCodeSymbologies.Telepen:
        ////      strSymbologyPrefix = "]B";
        ////      break;
        ////    case BarCodeSymbologies.PDF417:
        ////      strSymbologyPrefix = "]L";
        ////      break;
        ////    case BarCodeSymbologies.Micro_PDF417:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////    case BarCodeSymbologies.RSS_14:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////    case BarCodeSymbologies.RSS_Limited:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////    case BarCodeSymbologies.RSS_Expanded:
        ////      strSymbologyPrefix = "]e0";
        ////      break;
        ////    case BarCodeSymbologies.Codablock_A:
        ////      strSymbologyPrefix = "]O6";
        ////      break;
        ////    case BarCodeSymbologies.Codablock_F:
        ////      strSymbologyPrefix = "]O5";
        ////      break;
        ////    case BarCodeSymbologies.TLC_39:
        ////      strSymbologyPrefix = string.Empty;
        ////      break;
        ////    case BarCodeSymbologies.Codablock:
        ////      strSymbologyPrefix = "]O";
        ////      break;
        ////    case BarCodeSymbologies.Codablock256:
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
                            Symbology = BarCodeSymbologies.Codablock;

                            if (Prefix == "]O1")
                            {
                                Symbology = BarCodeSymbologies.UNDEFINED;
                            }
                            else if (Prefix == "]02")
                            {
                                Symbology = BarCodeSymbologies.Codablock256;
                            }
                            else if (Prefix == "]O4")
                            {
                                Symbology = BarCodeSymbologies.UNDEFINED;
                            }
                            else if (Prefix == "]O5")
                            {
                                Symbology = BarCodeSymbologies.Codablock_F;
                            }
                            else if (Prefix == "]O6")
                            {
                                Symbology = BarCodeSymbologies.Codablock_A;
                            }
                        }
                        else if (Prefix.StartsWith("]C"))
                        {
                            Symbology = BarCodeSymbologies.Code_128;
                            if (Prefix == "]C1")
                            {
                                // "UCC/EAN-128"
                                Symbology = BarCodeSymbologies.UCC_EAN_128_Symbol;
                            }
                        }
                        else if (Prefix.StartsWith("]G"))
                        {
                            Symbology = BarCodeSymbologies.Code_93;
                        }
                        else if (Prefix.StartsWith("]A"))
                        {
                            Symbology = BarCodeSymbologies.Code_39;
                        }
                        else if (Prefix.StartsWith("]H"))
                        {
                            Symbology = BarCodeSymbologies.Code_11;
                        }
                        else if (Prefix.StartsWith("]F"))
                        {
                            Symbology = BarCodeSymbologies.Codabar;
                        }
                        else if (Prefix.StartsWith("]E"))
                        {
                            Symbology = BarCodeSymbologies.EAN_UPC_Symbol;
                            if (Prefix == "]E0")
                            {
                                // EAN 13
                                Symbology = BarCodeSymbologies.EAN_UCC_13;
                            }
                            else if (Prefix == "]E4")
                            {
                                // EAN-8
                                Symbology = BarCodeSymbologies.EAN_UCC_8;
                            }
                        }
                        else if (Prefix.StartsWith("]I"))
                        {
                            // "Interleaved 2 of 5" 'SCC-14
                            Symbology = BarCodeSymbologies.ITF_14_Symbol;
                        }
                        else if (Prefix.StartsWith("]M"))
                        {
                            Symbology = BarCodeSymbologies.MSI;
                        }
                        else if (Prefix.StartsWith("]L"))
                        {
                            Symbology = BarCodeSymbologies.PDF417;
                        }
                        else if (Prefix.StartsWith("]e0"))
                        {
                            Symbology = BarCodeSymbologies.RSS_Expanded;
                        }
                        else if (Prefix.StartsWith("]B"))
                        {
                            Symbology = BarCodeSymbologies.Telepen;
                        }
                        else if (Prefix.StartsWith("]X0"))
                        {
                            // UPC-E
                            Symbology = BarCodeSymbologies.UCC_12_UPC_E;
                        }
                        else
                        {
                            Symbology = BarCodeSymbologies.UNDEFINED;
                        }
                    }
                }
            }
            else
            {
                Symbology = BarCodeSymbologies.UNDEFINED;

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
                    rawData = string.Empty;
                    Data = string.Empty;
                    Length = 0;
                }
            }
        }
    }
}
