// <copyright file="BarCodeSymbology.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner
{
    /// <summary>
    /// Represents the different barcode symbology.
    /// </summary>
    public enum BarCodeSymbology
    {
        /// <summary>
        /// Undefined barcode symbology.
        /// </summary>
        UNDEFINED = 0,

        /// <summary>
        /// UCC 12/UPC E barcode symbology.
        /// </summary>
        UCC_12_UPC_E = 1,

        /// <summary>
        /// EAN UCC 14 barcode symbology.
        /// </summary>
        EAN_UCC_14 = 2,

        /// <summary>
        /// EAN UCC 13 barcode symbology.
        /// </summary>
        EAN_UCC_13 = 3,

        /// <summary>
        /// EAN UCC 8 barcode symbology.
        /// </summary>
        EAN_UCC_8 = 4,

        /// <summary>
        /// ITF 14 barcode symbology.
        /// </summary>
        ITF_14_Symbol = 5,

        /// <summary>
        /// UCC EAN 128 barcode symbology.
        /// </summary>
        UCC_EAN_128_Symbol = 6,

        /// <summary>
        /// Code 128 barcode symbology.
        /// </summary>
        Code_128 = 7,

        /// <summary>
        /// SSCC 18 barcode symbology.
        /// </summary>
        SSCC_18 = 8,

        /// <summary>
        /// Code 39 barcode symbology.
        /// </summary>
        Code_39 = 9,

        /// <summary>
        /// EAN UPC barcode symbology.
        /// </summary>
        EAN_UPC_Symbol = 10,

        /// <summary>
        /// Code 93 barcode symbology.
        /// </summary>
        Code_93 = 11,

        /// <summary>
        /// Codabar barcode symbology.
        /// </summary>
        Codabar = 12,

        /// <summary>
        /// Code 2 of 5 barcode symbology.
        /// </summary>
        Code_2_of_5 = 13,

        /// <summary>
        /// MSI barcode symbology.
        /// </summary>
        MSI = 14,

        /// <summary>
        /// Plessey barcode symbology.
        /// </summary>
        Plessey = 15,

        /// <summary>
        /// Code 11 barcode symbology.
        /// </summary>
        Code_11 = 16,

        /// <summary>
        /// Matrix 2 of 5 barcode symbology.
        /// </summary>
        Matrix_2_of_5 = 17,

        /// <summary>
        /// Telepen barcode symbology.
        /// </summary>
        Telepen = 18,

        /// <summary>
        /// PDF417 barcode symbology.
        /// </summary>
        PDF417 = 19,

        /// <summary>
        /// Micro PDF417 barcode symbology.
        /// </summary>
        Micro_PDF417 = 20,

        /// <summary>
        /// RSS 14 barcode symbology.
        /// </summary>
        RSS_14 = 21,

        /// <summary>
        /// RSS Limited barcode symbology.
        /// </summary>
        RSS_Limited = 22,

        /// <summary>
        /// RSS Expanded barcode symbology.
        /// </summary>
        RSS_Expanded = 23,

        /// <summary>
        /// Codablock A barcode symbology.
        /// </summary>
        Codablock_A = 24,

        /// <summary>
        /// Codablock F barcode symbology.
        /// </summary>
        Codablock_F = 25,

        /// <summary>
        /// TLC 39 barcode symbology.
        /// </summary>
        TLC_39 = 26,

        /// <summary>
        /// Codablock barcode symbology.
        /// </summary>
        Codablock = 27,

        /// <summary>
        /// Codablock 256 barcode symbology.
        /// </summary>
        Codablock256 = 28,
    }
}