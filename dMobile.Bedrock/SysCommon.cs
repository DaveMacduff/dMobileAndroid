// <copyright file="SysCommon.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Resources.Models.Headers;

    /// <summary>
    /// Defines the <see cref="SysCommon" />.
    /// </summary>
    public class SysCommon
    {
        /// <summary>
        ///     Defines the SystemLoginId.
        /// </summary>
        public const int SystemLoginId = 30;

        /// <summary>
        ///     Defines the SystemLoginPassword.
        /// </summary>
        public const string SystemLoginPassword = "4274";

        /// <summary>
        ///     Defines the SetupLoginId.
        /// </summary>
        public const int SetupLoginId = 99;

        /// <summary>
        ///     Defines the SetupLoginPassword.
        /// </summary>
        public const string SetupLoginPassword = "99";

        /// <summary>
        ///     Defines the EmptyDateTime.
        /// </summary>
        public static readonly DateTime EmptyDateTime = new DateTime(1900, 1, 1);

        /// <summary>
        ///     Defines the show error message type.
        /// </summary>
        public enum ShowErrorMessageType
        {
            /// <summary>
            /// Error Report Description
            /// </summary>
            ErrorReport = 0,

            /// <summary>
            /// Defines the MessageBox.
            /// </summary>
            MessageBox = 1,
        }

        /// <summary>
        ///     Activity picking quantity over validation levels.
        /// </summary>
        public enum ActPickingQtyOverValLevels
        {
            /// <summary>
            /// Defines the NoValidationOrWarning.
            /// </summary>
            NoValidationOrWarning = 0,

            /// <summary>
            /// Defines the ValidateWarnAndStop.
            /// </summary>
            ValidateWarnAndStop = 1,

            /// <summary>
            /// Defines the ValidateWarnAndContinue.
            /// </summary>
            ValidateWarnAndContinue = 2,
        }

        /// <summary>
        ///     Activity picking quantity under validation levels.
        /// </summary>
        public enum ActPickingQtyUnderValLevels
        {
            /// <summary>
            /// Defines the NoValidationOrWarning.
            /// </summary>
            NoValidationOrWarning = 0,

            /// <summary>
            /// Defines the ValidateWarnAndStop.
            /// </summary>
            ValidateWarnAndStop = 1,

            /// <summary>
            /// Defines the ValidateWarnAndContinue.
            /// </summary>
            ValidateWarnAndContinue = 2,
        }

        /// <summary>
        ///     Activity transfers quantity over validation levels.
        /// </summary>
        public enum ActTransfersQtyOverValLevels
        {
            /// <summary>
            /// Defines the NoValidationOrWarning.
            /// </summary>
            NoValidationOrWarning = 0,

            /// <summary>
            /// Defines the ValidateWarnAndStop.
            /// </summary>
            ValidateWarnAndStop = 1,

            /// <summary>
            /// Defines the ValidateWarnAndContinue.
            /// </summary>
            ValidateWarnAndContinue = 2,
        }

        /// <summary>
        ///     Activity transfers quantity under validation levels.
        /// </summary>
        public enum ActTransfersQtyUnderValLevels
        {
            /// <summary>
            /// Defines the NoValidationOrWarning.
            /// </summary>
            NoValidationOrWarning = 0,

            /// <summary>
            /// Defines the ValidateWarnAndStop.
            /// </summary>
            ValidateWarnAndStop = 1,

            /// <summary>
            /// Defines the ValidateWarnAndContinue.
            /// </summary>
            ValidateWarnAndContinue = 2,
        }

        /// <summary>
        ///     Activity receiving quantity over validation levels.
        /// </summary>
        public enum ActReceivingQtyOverValLevels
        {
            /// <summary>
            /// Defines the NoValidationOrWarning.
            /// </summary>
            NoValidationOrWarning = 0,

            /// <summary>
            /// Defines the ValidateWarnAndStop.
            /// </summary>
            ValidateWarnAndStop = 1,

            /// <summary>
            /// Defines the ValidateWarnAndContinue.
            /// </summary>
            ValidateWarnAndContinue = 2,
        }

        /// <summary>
        ///     Activity receiving quantity under validation levels.
        /// </summary>
        public enum ActReceivingQtyUnderValLevels
        {
            /// <summary>
            /// Defines the NoValidationOrWarning.
            /// </summary>
            NoValidationOrWarning = 0,

            /// <summary>
            /// Defines the ValidateWarnAndStop.
            /// </summary>
            ValidateWarnAndStop = 1,

            /// <summary>
            /// Defines the ValidateWarnAndContinue.
            /// </summary>
            ValidateWarnAndContinue = 2,
        }

        /// <summary>
        ///     Replenishment move type codes.
        /// </summary>
        public enum ReplenishmentTypes
        {
            /// <summary>
            /// Manual replenishment move type.
            /// </summary>
            Manual = 0,

            /// <summary>
            /// PullDown replenishment move type.
            /// </summary>
            PullDown = 1,

            /// <summary>
            /// Restock replenishment move type.
            /// </summary>
            Restock = 2,
        }

        /// <summary>
        ///     Adjustments types.
        /// </summary>
        public enum AdjTypes
        {
            /// <summary>
            /// Defines the Inventory.
            /// </summary>
            Inventory = 0,

            /// <summary>
            /// Defines the UnitConversion.
            /// </summary>
            UnitConversion = 1,

            /// <summary>
            /// Defines the Rework.
            /// </summary>
            Rework = 2,

            /// <summary>
            /// Defines the PartialRework.
            /// </summary>
            PartialRework = 3,

            /// <summary>
            /// Defines the Production.
            /// </summary>
            Production = 4,

            /// <summary>
            /// Defines the PartialProduction.
            /// </summary>
            PartialProduction = 5,

            /// <summary>
            /// Defines the Disposal.
            /// </summary>
            Disposal = 6,

            /// <summary>
            /// Defines the Fresh2Frozen.
            /// </summary>
            Fresh2Frozen = 7,

            /// <summary>
            /// Defines the Transfer.
            /// </summary>
            Transfer = 8,

            /// <summary>
            /// Defines the PackOff.
            /// </summary>
            PackOff = 9,

            /// <summary>
            /// Defines the LostFound.
            /// </summary>
            LostFound = 10,

            /// <summary>
            /// Defines the CycleCount.
            /// </summary>
            CycleCount = 11,

            /// <summary>
            /// Defines the MoveToDeli.
            /// </summary>
            MoveToDeli = 12,

            /// <summary>
            /// Defines the PeriodicCount.
            /// </summary>
            PeriodicCount = 13,

            /// <summary>
            /// Defines the Manufacture.
            /// </summary>
            Manufacture = 14,

            /// <summary>
            /// Defines the Requisition.
            /// </summary>
            Requisition = 15,

            /// <summary>
            /// Defines the AutoConvertTo.
            /// </summary>
            AutoConvertTo = 16,

            /// <summary>
            /// Defines the AutoCreateFrom.
            /// </summary>
            AutoCreateFrom = 17,

            /// <summary>
            /// Defines the SkidPutaway.
            /// </summary>
            SkidPutaway = 18,

            /// <summary>
            /// Defines the ConvertProduct.
            /// </summary>
            ConvertProduct = 19,

            /// <summary>
            /// Defines the Debox.
            /// </summary>
            Debox = 20,

            /// <summary>
            /// Defines the ConvertSkid.
            /// </summary>
            ConvertSkid = 21,
        }

        /// <summary>
        ///     Activity status types.
        /// </summary>
        public enum ActStatus
        {
            /// <summary>
            /// Defines the NoChange.
            /// </summary>
            NoChange = 0,

            /// <summary>
            /// Defines the Inserted.
            /// </summary>
            Inserted = 1,

            /// <summary>
            /// Defines the Updated.
            /// </summary>
            Updated = 2,

            /// <summary>
            /// Defines the Deleted.
            /// </summary>
            Deleted = 3,
        }

        /// <summary>
        ///     Scheme extra types.
        /// </summary>
        public enum SchemeExtraTypes
        {
            /// <summary>
            /// Defines the Disabled.
            /// </summary>
            Disabled = 0,

            /// <summary>
            /// Defines the Text.
            /// </summary>
            Text = 1,
        }

        /// <summary>
        ///     Bar code rules.
        /// </summary>
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode", Justification = "Not sure how to fix the warnings.")]
        public enum BarCodeRules
        {
            /// <summary>
            /// Defines the ParseCode_128AsUCC_EAN_128_SymbolCompliant.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            ParseCode_128AsUCC_EAN_128_SymbolCompliant = 1,

            /// <summary>
            /// Defines the ParseFINICA_Code_128_as_EAN_UCC_128.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            ParseFINICA_Code_128_as_EAN_UCC_128 = 2,

            /// <summary>
            /// Defines the NoCheckDigitInGlobalLocationNumber.
            /// </summary>
            NoCheckDigitInGlobalLocationNumber = 3,

            /// <summary>
            /// Defines the NoZeroPaddingOfProductIdentifierForUCC_EAN_Family.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            NoZeroPaddingOfProductIdentifierForUCC_EAN_Family = 4,

            /// <summary>
            /// Defines the ParseWOOLWICH_Code_128_RemoveSpaces.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            ParseWOOLWICH_Code_128_RemoveSpaces = 5,

            /// <summary>
            /// Defines the Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate = 6,

            /// <summary>
            /// Defines the Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate = 7,

            /// <summary>
            /// Defines the Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount = 8,

            /// <summary>
            /// Defines the ProductBarCodeMinimumLength4Digits.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            ProductBarCodeMinimumLength4Digits = 9,

            /// <summary>
            /// Defines the Parse_UCC_12_UPC_E_as_Deli_PLU.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            Parse_UCC_12_UPC_E_as_Deli_PLU = 10,

            /// <summary>
            /// Defines the Parse_Code128_OvationFielding_Lamb.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            Parse_Code128_OvationFielding_Lamb = 11,

            /// <summary>
            /// Defines the Parse_Code128_China_Seafood.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            Parse_Code128_China_Seafood = 12,

            /// <summary>
            /// Defines the Parse_Code_128_AI01_BC_AI30_exclusive.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            Parse_Code_128_AI01_BC_AI30_exclusive = 13,

            /// <summary>
            /// Defines the ParseRenees_Code_128_RemoveSpaces.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            ParseRenees_Code_128_RemoveSpaces = 14,

            /// <summary>
            /// Defines the ParseLanzMeats_Code_128_AI10_as_AI21.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            ParseLanzMeats_Code_128_AI10_as_AI21 = 15,

            /// <summary>
            /// Defines the HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit = 16,

            /// <summary>
            /// Defines the ParseNossackBrokerMultiNationalProductFromChili.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            ParseNossackBrokerMultiNationalProductFromChili = 17,

            /// <summary>
            /// Defines the ParseUCC_13asUPC_12_UPC_E.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            ParseUCC_13asUPC_12_UPC_E = 18,

            /// <summary>
            /// Defines the Parse_HelaSpiceCanada_Code_128.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            Parse_HelaSpiceCanada_Code_128 = 19,

            /// <summary>
            /// Defines the NebraskaBeef_Parse_Lot_as_Serial_Code_128.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            NebraskaBeef_Parse_Lot_as_Serial_Code_128 = 20,

            /// <summary>
            /// Defines the ParseNorthernCoop_AustraliaBeefTrim_Product25612.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            ParseNorthernCoop_AustraliaBeefTrim_Product25612 = 21,

            /// <summary>
            /// Defines the ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct = 22,

            /// <summary>
            /// Defines the ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs = 23,

            /// <summary>
            /// Defines the ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 = 24,

            /// <summary>
            /// Defines the ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents = 25,

            /// <summary>
            /// Defines the ParseEAN_UCC128_Nossack_GreenleaPremierMeats.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            ParseEAN_UCC128_Nossack_GreenleaPremierMeats = 26,

            /// <summary>
            /// Defines the ParseEAN_UCC128_Dovre_KiiNaturalsCrisps.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            ParseEAN_UCC128_Dovre_KiiNaturalsCrisps = 27,

            /// <summary>
            /// Defines the Parse_Code128_CleanSubstituteCharacter.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            Parse_Code128_CleanSubstituteCharacter = 28,

            /// <summary>
            /// Restrict the amount on deli PLU's from 5 to 4.
            /// </summary>
            /// <remarks>12345 becomes 2345.</remarks>
            // ReSharper disable once InconsistentNaming
            Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount_Enabled = 29,

            /// <summary>
            /// Define the parse EAN / UCC Code 128 Bourvy Exports production date from the AI(21) serial number.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            ParseEAN_UCC128_BourvyExports_ProductionDate = 30,
        }

        /// <summary>
        ///     Activity subtypes.
        /// </summary>
        /// <remarks>
        /// Keep in sync with imsck_wms.datacntrl method request_skid_info_cursor() enums.
        /// </remarks>
        public enum ActSubTypes
        {
            /// <summary>
            /// Defines the NotApplicable.
            /// </summary>
            NotApplicable = 0,

            /// <summary>
            /// Defines the BarCodeAssociation.
            /// </summary>
            BarCodeAssociation = 1,

            /// <summary>
            /// Defines the PhysicalCounts.
            /// </summary>
            PhysicalCounts = 2,

            /// <summary>
            /// Defines the Receiving.
            /// </summary>
            Receiving = 3,

            /// <summary>
            /// Defines the Picking.
            /// </summary>
            Picking = 4,

            /// <summary>
            /// Defines the TransferFrom.
            /// </summary>
            TransferFrom = 5,

            /// <summary>
            /// Defines the RefreshData.
            /// </summary>
            RefreshData = 6,

            /// <summary>
            /// Defines the DataStatus.
            /// </summary>
            DataStatus = 7,

            /// <summary>
            /// Defines the Returns.
            /// </summary>
            Returns = 8,

            /// <summary>
            /// Defines the SelectProduct.
            /// </summary>
            SelectProduct = 9,

            /// <summary>
            /// Defines the SelectLocation.
            /// </summary>
            SelectLocation = 10,

            /// <summary>
            /// Defines the CycleCounts.
            /// </summary>
            CycleCounts = 11,

            /// <summary>
            /// Defines the ConvertUnit.
            /// </summary>
            ConvertUnit = 12,

            /// <summary>
            /// Defines the MoveInventory.
            /// </summary>
            MoveInventory = 13,

            /// <summary>
            /// Defines the Adjustment.
            /// </summary>
            Adjustment = 14,

            /// <summary>
            /// Defines the PurgeActivityData.
            /// </summary>
            PurgeActivityData = 15,

            /// <summary>
            /// Defines the TicketFinished.
            /// </summary>
            TicketFinished = 16,

            /// <summary>
            /// Defines the TicketMaterial.
            /// </summary>
            TicketMaterial = 17,

            /// <summary>
            /// Defines the TicketAddProduct.
            /// </summary>
            TicketAddProduct = 18,

            /// <summary>
            /// Defines the ReleaseByOrder.
            /// </summary>
            ReleaseByOrder = 19,

            /// <summary>
            /// Defines the ReleaseByItemSection.
            /// </summary>
            ReleaseByItemSection = 20,

            /// <summary>
            /// Defines the PurchaseOrderWorkSheet.
            /// </summary>
            PurchaseOrderWorkSheet = 21,

            /// <summary>
            /// Defines the SkidInfo.
            /// </summary>
            SkidInfo = 22,

            /// <summary>
            /// Defines the MoveSkid.
            /// </summary>
            MoveSkid = 23,

            /// <summary>
            /// Defines the BreakSkid.
            /// </summary>
            BreakSkid = 24,

            /// <summary>
            /// Defines the PickingSingle.
            /// </summary>
            PickingSingle = 25,

            /// <summary>
            /// Defines the PutAwaySkid.
            /// </summary>
            PutAwaySkid = 26,

            /// <summary>
            /// Defines the AssignDefaultLocation.
            /// </summary>
            AssignDefaultLocation = 27,

            /// <summary>
            /// Defines the InventoryInquiry.
            /// </summary>
            InventoryInquiry = 28,

            /// <summary>
            /// Defines the InventoryReplenishment.
            /// </summary>
            InventoryReplenishment = 29,

            /// <summary>
            /// Defines the LoginStartup.
            /// </summary>
            LoginStartup = 30,

            /// <summary>
            /// Defines the SplashStartup.
            /// </summary>
            SplashStartup = 31,

            /// <summary>
            /// Defines the PrintProductLabel.
            /// </summary>
            PrintProductLabel = 32,

            /// <summary>
            /// Defines the PickingSelectCriteria.
            /// </summary>
            PickingSelectCriteria = 33,

            /// <summary>
            /// Defines the PickingFilter.
            /// </summary>
            PickingFilter = 34,

            /// <summary>
            /// Defines the SubmitBatch.
            /// </summary>
            SubmitBatch = 35,

            /// <summary>
            /// Defines the CommunicationSetup.
            /// </summary>
            CommunicationSetup = 36,

            /// <summary>
            /// Defines the SetupDefaults.
            /// </summary>
            SetupDefaults = 37,

            /// <summary>
            /// Defines the InventoryInquirySummary.
            /// </summary>
            InventoryInquirySummary = 38,

            /// <summary>
            /// Defines the ConvertProduct.
            /// </summary>
            ConvertProduct = 39,

            /// <summary>
            /// Defines the TransferTo.
            /// </summary>
            TransferTo = 40,
        }

        /// <summary>
        ///     Activity count type fields.
        /// </summary>
        public enum ActCountTypeFields
        {
            /// <summary>
            /// Defines the PhysicalCountByWarehouse.
            /// </summary>
            PhysicalCountByWarehouse = 1,

            /// <summary>
            /// Defines the CycleCountByWarehouse.
            /// </summary>
            CycleCountByWarehouse = 2,

            /// <summary>
            /// Defines the CycleCountBySection.
            /// </summary>
            CycleCountBySection = 3,

            /// <summary>
            /// Defines the CycleCountBySlot.
            /// </summary>
            CycleCountBySlot = 4,

            /// <summary>
            /// Defines the BaseCycleCountByWarehouse.
            /// </summary>
            BaseCycleCountByWarehouse = 5,
        }

        /// <summary>
        ///     Contact subtypes.
        /// </summary>
        public enum ContactSubTypes
        {
            /// <summary>
            /// Defines the User.
            /// </summary>
            User = 1,

            /// <summary>
            /// Defines the Customer.
            /// </summary>
            Customer = 2,

            /// <summary>
            /// Defines the CustomerShipTo.
            /// </summary>
            CustomerShipTo = 3,

            /// <summary>
            /// Defines the Vendor.
            /// </summary>
            Vendor = 4,

            /// <summary>
            /// Defines the VendorSupplier.
            /// </summary>
            VendorSupplier = 5,
        }

        /// <summary>
        ///     Product location map types.
        /// </summary>
        public enum ProdLocMapTypes
        {
            /// <summary>
            /// Defines the Floor.
            /// </summary>
            Floor = 1,

            /// <summary>
            /// Defines the Picking.
            /// </summary>
            Picking = 2,

            /// <summary>
            /// Defines the Receiving.
            /// </summary>
            Receiving = 3,

            /// <summary>
            /// Defines the QualityAssurance.
            /// </summary>
            QualityAssurance = 4,

            /// <summary>
            /// Defines the IssuedTo.
            /// </summary>
            IssuedTo = 5,

            /// <summary>
            /// Defines the ByProducts.
            /// </summary>
            ByProducts = 6,

            /// <summary>
            /// Defines the FinishedGoods.
            /// </summary>
            FinishedGoods = 7,
        }

        /// <summary>
        ///     Location map types.
        /// </summary>
        public enum LocMapTypes
        {
            /// <summary>
            /// Defines the IsTracked.
            /// </summary>
            IsTracked = 1,

            /// <summary>
            /// Defines the IsStaging.
            /// </summary>
            IsStaging = 2,

            /// <summary>
            /// Defines the IsFreezer.
            /// </summary>
            IsFreezer = 3,

            /// <summary>
            /// Defines the IsReserved.
            /// </summary>
            IsReserved = 4,

            /// <summary>
            /// Defines the IsToBeOrdered.
            /// </summary>
            IsToBeOrdered = 5,

            /// <summary>
            /// Defines the IsHandStack.
            /// </summary>
            IsHandStack = 6,

            /// <summary>
            /// Defines the IsGroundLevel.
            /// </summary>
            IsGroundLevel = 7,

            /// <summary>
            /// Defines the IsProcessing.
            /// </summary>
            IsProcessing = 8,
        }

        /// <summary>
        ///     Location map slot coordinate types.
        /// </summary>
        public enum LocMapSlotCoordTypes
        {
            /// <summary>
            /// Defines the Coord1.
            /// </summary>
            Coord1 = 1,

            /// <summary>
            /// Defines the Coord2.
            /// </summary>
            Coord2 = 2,

            /// <summary>
            /// Defines the Coord3.
            /// </summary>
            Coord3 = 3,
        }

        /// <summary>
        ///     Location map subtypes.
        /// </summary>
        public enum LocMapSubTypes
        {
            /// <summary>
            /// Defines the None.
            /// </summary>
            None = 0,

            /// <summary>
            /// Defines the Warehouse.
            /// </summary>
            Warehouse = 1,

            /// <summary>
            /// Defines the Section.
            /// </summary>
            Section = 2,

            /// <summary>
            /// Defines the Slot.
            /// </summary>
            Slot = 3,
        }

        /// <summary>
        ///     Replace XML reserved characters in a string with the XML entity references.
        /// </summary>
        /// <param name="data">Data to format.</param>
        /// <returns>Well formed xml string.</returns>
        public static string AddXmlReservedChars(string data)
        {
            var xmlString = data.Replace("<", "&lt;");
            xmlString = xmlString.Replace(">", "&gt;");
            xmlString = xmlString.Replace("\"", "&quot;");
            xmlString = xmlString.Replace("'", "&apos;");
            xmlString = xmlString.Replace("&", "&amp;");

            return xmlString;
        }

        /// <summary>
        ///     Replace the XML entity references in a string with the reserved characters.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>Restored string.</returns>
        public static string RemoveXmlReservedChars(string data)
        {
            var xmlString = data.Replace("&lt;", "<");
            xmlString = xmlString.Replace("&gt;", ">");
            xmlString = xmlString.Replace("&quot;", "\"");
            xmlString = xmlString.Replace("&apos;", "'");
            xmlString = xmlString.Replace("&amp;", "&");

            return xmlString;
        }

        /// <summary>
        /// The IsAllowedChangeNegateQuantityFromAdjType.
        /// </summary>
        /// <param name="adjustmentType">.</param>
        /// <returns>True if allowed to change negate quantity, else false.</returns>
        public static bool IsAllowedChangeNegateQuantityFromAdjType(AdjTypes adjustmentType)
        {
            var isAllowedNegateQuantity = false;

            switch (adjustmentType)
            {
                case AdjTypes.CycleCount:
                    break;

                case AdjTypes.Disposal:
                    isAllowedNegateQuantity = true;
                    break;

                case AdjTypes.Fresh2Frozen:
                    break;

                case AdjTypes.Inventory:
                    isAllowedNegateQuantity = true;
                    break;

                case AdjTypes.LostFound:
                    isAllowedNegateQuantity = true;
                    break;

                case AdjTypes.Manufacture:
                    break;

                case AdjTypes.MoveToDeli:
                    break;

                case AdjTypes.PackOff:
                    break;

                case AdjTypes.PartialProduction:
                    break;

                case AdjTypes.PartialRework:
                    isAllowedNegateQuantity = true;
                    break;

                case AdjTypes.PeriodicCount:
                    break;

                case AdjTypes.Production:
                    break;

                case AdjTypes.Rework:
                    isAllowedNegateQuantity = true;
                    break;

                case AdjTypes.Transfer:
                    break;

                case AdjTypes.UnitConversion:
                    isAllowedNegateQuantity = true;
                    break;

                case AdjTypes.Requisition:
                    break;

                case AdjTypes.AutoConvertTo:
                    break;

                case AdjTypes.AutoCreateFrom:
                    break;

                case AdjTypes.SkidPutaway:
                    isAllowedNegateQuantity = true;
                    break;

                case AdjTypes.ConvertProduct:
                    isAllowedNegateQuantity = true;
                    break;

                case AdjTypes.Debox:
                    break;

                case AdjTypes.ConvertSkid:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(adjustmentType), adjustmentType, null);
            }

            return isAllowedNegateQuantity;
        }

        /// <summary>
        /// The DefaultNegateQuantityFromAdjType.
        /// </summary>
        /// <param name="adjustmentType">.</param>
        /// <returns>True if to default negate quantity, else false.</returns>
        public static bool DefaultNegateQuantityFromAdjType(AdjTypes adjustmentType)
        {
            var isNegateQuantity = false;

            switch (adjustmentType)
            {
                case AdjTypes.Disposal:
                case AdjTypes.Fresh2Frozen:
                case AdjTypes.PartialRework:
                case AdjTypes.Rework:
                case AdjTypes.UnitConversion:
                case AdjTypes.Requisition:
                case AdjTypes.AutoConvertTo:
                case AdjTypes.SkidPutaway:
                case AdjTypes.ConvertProduct:
                case AdjTypes.Debox:
                case AdjTypes.ConvertSkid:
                    isNegateQuantity = true;
                    break;

                case AdjTypes.CycleCount:
                case AdjTypes.Inventory:
                case AdjTypes.LostFound:
                case AdjTypes.Manufacture:
                case AdjTypes.MoveToDeli:
                case AdjTypes.PackOff:
                case AdjTypes.PartialProduction:
                case AdjTypes.PeriodicCount:
                case AdjTypes.Production:
                case AdjTypes.Transfer:
                case AdjTypes.AutoCreateFrom:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(adjustmentType), adjustmentType, null);
            }

            return isNegateQuantity;
        }

        /// <summary>
        ///     device_id, login_id, revision_date, stamp.
        /// </summary>
        /// <param name="useRefreshedDataLast">Use refreshed data list.</param>
        /// <returns>Requested default string.</returns>
        public static async Task<string> GetRequestDefaults(bool useRefreshedDataLast)
        {
            var sysEnv = SysEnvironment.GetInstance();
            var current = DateTime.Now.ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
            var revision = new DateTime(1900, 1, 1).ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
            if (useRefreshedDataLast)
            {
                revision = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
            }

            // Added the login warehouse fk refno, for request filtering.
            // Easier to add it here than review every activity request, for filtering.
            var loginWhs = LocationMap.GetInstance().LoginWhsLocMapRefno;
            var loginLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(loginWhs);
            var def = $@"
                <device_id>{sysEnv.DeviceId}</device_id>
                <platform>Android</platform>
                <login_id>{sysEnv.ContactLog.LoginID}</login_id>
                <login_whs_id>{loginLocMap.WhsFkRefno}</login_whs_id>
                <revision_datetime>{revision}</revision_datetime>
                <stamp>{current}</stamp>";
            return def;
        }

        /// <summary>
        ///     Check for any activities with change state not equal to ActivityStatus.NoChange.
        /// </summary>
        /// <returns>True or false if data that has not been submitted exists.</returns>
        public static async Task<bool> CheckForUnSubmittedData()
        {
            var isDataExists = false;
            var noChangeStatus = Convert.ToInt32(ActStatus.NoChange);

            var sql = $@"
                SELECT tActivity.ActivityRefno
                FROM tActivity
                WHERE tActivity.ActivityStatus <> {noChangeStatus}
                UNION
                SELECT tActivity.ActivityRefno
                FROM tActivity
                INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno
                WHERE tActivityItem.ActivityItemStatus <> {noChangeStatus}
                UNION
                SELECT tActivity.ActivityRefno
                FROM tActivity
                INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno
                INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = tActivityItem.ActivityItemRefno
                WHERE tActivityDetail.ActivityDetailStatus <> {noChangeStatus}";

            var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ActivityModel>(Convert.ToString(sql));
            if (results?.Count > 0)
            {
                isDataExists = true;
            }

            return isDataExists;
        }

        /// <summary>
        ///     Make a nice formatted string of quantity and product units.
        /// </summary>
        /// <param name="prodInfo">Product information.</param>
        /// <param name="quantity">Quantity.</param>
        /// <param name="weightQuantity">Weight quantity.</param>
        /// <returns>Formatted nice readable string '12.000 CS @ 150.546 KG'.</returns>
        public static string NiceQuantityReadableString(ProductInfo prodInfo, decimal quantity, decimal weightQuantity)
        {
            var unit = Math.Round(quantity, 3).ToString(CultureInfo.CurrentCulture) + " " + prodInfo.UnitCode;
            var wtUnit = Math.Round(weightQuantity, 3).ToString(CultureInfo.CurrentCulture) + " " + prodInfo.WeightUnitCode;
            return unit + " @ " + wtUnit;
        }

        /// <summary>
        ///     Fill quantity structure from quantity.
        /// </summary>
        /// <param name="prodInfo"><see cref="ProductInfo"/> Product information.</param>
        /// <param name="quantity"><see cref="decimal"/> Quantity.</param>
        /// <returns>Filled quantity structure.</returns>
        public static SFillQuantity FillQuantityStructureFromQuantity(ProductInfo prodInfo, decimal quantity)
        {
            var fill = default(SFillQuantity);
            if (quantity == 0)
            {
                if ((prodInfo.UnitCode == "LB") | (prodInfo.UnitCode == "KG"))
                {
                    fill.Quantity = prodInfo.UnitGrossWeight;
                }
                else
                {
                    fill.Quantity = 1;
                }
            }
            else
            {
                fill.Quantity = quantity;
            }

            decimal baseQty;
            if (prodInfo.UnitCode == prodInfo.BaseUnitCode && prodInfo.Unit2BaseUnitFactor <= 1)
            {
                baseQty = fill.Quantity;
            }
            else if (prodInfo.BaseUnitCode == "LB" || prodInfo.BaseUnitCode == "KG")
            {
                baseQty = fill.Quantity * prodInfo.UnitGrossWeight;
            }
            else
            {
                baseQty = fill.Quantity * prodInfo.Unit2BaseUnitFactor;
            }

            fill.BaseQuantity = Math.Round(baseQty, 3);

            decimal weightQty;
            if (prodInfo.UnitCode == prodInfo.WeightUnitCode)
            {
                weightQty = fill.Quantity;
            }
            else if (prodInfo.BaseUnitCode == prodInfo.WeightUnitCode)
            {
                weightQty = baseQty;
            }
            else
            {
                weightQty = fill.Quantity * prodInfo.UnitGrossWeight;
            }

            fill.WeightQuantity = Math.Round(weightQty, 3);

            return fill;
        }

        /// <summary>
        ///     Submit communication request.
        /// </summary>
        /// <param name="sendData"><see cref="string"/> Communication data to submit.</param>
        /// <returns><see cref="string"/> Communication response.</returns>
        public static string SubmitComm(string sendData)
        {
            var receivedData = string.Empty;
            var sysEnv = SysEnvironment.GetInstance();
            Socket sysSock = null;
            var port = sysEnv.SysSock.DefSocketPort;
            const int numberOfPasses = 2;
            var success = false;
            var maxAttempts = sysEnv.SysSock.MaxPorts * numberOfPasses;
            maxAttempts = maxAttempts > 1 ? maxAttempts - 1 : 1;
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var tryNewPort = false;
                    sysEnv.SysSock.Connect(ref sysSock, port, ref tryNewPort);
                    if (tryNewPort)
                    {
                        port = GetNextPort();
                    }
                    else
                    {
                        sysEnv.SysSock.SendRequest(sysSock, sendData);
                        var compressedData = sysEnv.SysSock.ReceiveResponse(sysSock);
                        receivedData = Compress.UnZip(compressedData.ToArray());
                        CheckCommResponseForErrors(receivedData);
                        success = true;
                    }
                }
                finally
                {
                    sysEnv.SysSock.Disconnect(sysSock);
                }

                if (success)
                {
                    break;
                }
            }

            return receivedData;

            ///////////////////////////////////////////////////////////
            int GetNextPort()
            {
                // New port will be compared to the current port to determine if we have
                // tried all ports. Or if we have tried all ports, we will start over at the
                // base port. Depending on the error code, we will assign the new port to the
                // current port.
                var newPort = port + 1;
                if (newPort > sysEnv.SysSock.BasePort + (sysEnv.SysSock.MaxPorts - 1))
                {
                    newPort = sysEnv.SysSock.BasePort;
                }

                return newPort;
            }
        }

        /// <summary>
        ///     Checks a COM submission response string for errors.
        ///     Throws exceptions if errors are found.
        /// </summary>
        /// <param name="response">XML response string.</param>
        public static void CheckCommResponseForErrors(string response)
        {
            var msg = "Communication response is empty or contains no data.";
            response = response.IsNullOrEmpty() ? throw new Exception(msg) : response;

            // Only errors from the comm server will contain reserved xml characters that need
            // special handling.
            // Table data with special characters will be handled when being loaded in the
            // xml.dom object.
            if (response.Contains("<message>ERROR</message>"))
            {
                response = SysCommon.RemoveXmlReservedChars(response);
                var receivedError = new XmlDocument();
                receivedError.LoadXml(response);
                if (receivedError.DocumentElement == null)
                {
                    ////throw new CommunicationException("receivedError.DocumentElement root element == null.");
                    throw new CommunicationException("Received data was not complete.");
                }

                if (receivedError.DocumentElement["detail"] == null)
                {
                    ////throw new CommunicationException(@"receivedError.DocumentElement['detail'] element == null.");
                    throw new CommunicationException("Received detail message was not complete.");
                }

                msg = receivedError.DocumentElement["detail"] !.InnerText;
                if (msg.Contains("MSGBOX-"))
                {
                    // Throw a message only exception with the 'MSGBOX-' text removed.
                    throw new MessageOnlyException(msg[7..]);
                }
                else
                {
                    throw new CommunicationException(msg);
                }
            }
        }

        /// <summary>
        ///     Check if the date time is empty.
        /// </summary>
        /// <param name="dt">Date time to check.</param>
        /// <returns>True if the date time is empty, else false.</returns>
        public static bool IsEmptyDateTime(DateTime dt)
        {
            return dt.Year <= 1900;
        }

        /// <summary>
        ///     Check if the date is empty.
        /// </summary>
        /// <param name="dt">Date to check.</param>
        /// <returns>True if the date is empty, else false.</returns>
        public static bool IsEmptyDate(DateTime dt)
        {
            return dt.Year <= 1900;
        }

        /// <summary>
        /// Convert the LB to KG.
        /// </summary>
        /// <param name="lbQuantity">The quantity in pounds.<see cref="double"/>.</param>
        /// <returns>Quantity in kilograms.</returns>
        public static double ConvertLb2Kg(double lbQuantity)
        {
            return lbQuantity * 0.4535923703;
        }

        /// <summary>
        /// Converts the LB to KG.
        /// </summary>
        /// <param name="lbQuantity">The quantity in pounds.<see cref="double"/>.</param>
        /// <returns>Quantity in kilograms.</returns>
        public static decimal ConvertLb2Kg(decimal lbQuantity)
        {
            return lbQuantity * 0.4535923703M;
        }

        /// <summary>
        /// Convert the KG to LB.
        /// </summary>
        /// <param name="kgQuantity">The quantity in kilograms.<see cref="double"/>.</param>
        /// <returns>Quantity in pounds.</returns>
        public static double ConvertKg2Lb(double kgQuantity)
        {
            return kgQuantity * 2.20462262;
        }

        /// <summary>
        /// Converts KG to LB.
        /// </summary>
        /// <param name="kgQuantity">.</param>
        /// <returns>Quantity in pounds.</returns>
        public static decimal ConvertKg2Lb(decimal kgQuantity)
        {
            return kgQuantity * 2.20462262M;
        }

        /// <summary>
        ///     Error report and request continue.
        /// </summary>
        /// <param name="fullClassName">The fullClassName<see cref="string"/>.</param>
        /// <param name="reportingMethod">The reportingMethod<see cref="string"/>.</param>
        /// <param name="ex">.</param>
        /// <returns>True if request to continue else false.</returns>
        public static bool ErrorReportAndRequestContinue(
            string fullClassName,
            string reportingMethod,
            Exception ex)
        {
            // Determine Class and Assembly
            // TODO: Rework / reimplement this method.
            var aClass = fullClassName.Split('.');
            //var assembly = aClass[0];

            // Dim strClassName As New System.Text.StringBuilder
            // For Each s As String In aClass
            //  If Not (s = aClass(0)) Then strClassName.Append(s & ".")
            // Next
            // strClassName.Remove(strClassName.Length - 1, 1)

            // Dim strAppDir As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)
            // Dim strFullPathToMyFile As String = Path.Combine(strAppDir, "dMobileResource02.wav")
            // Dim oSnd As SysSound = New SysSound(strFullPathToMyFile)
            // oSnd.Play()
            var result = false;

            // #if MULTILANGUAGE
            //      string errorMsg = Translator.GetInstance().Translate(TranslatorVocabulary.AnUnanticipatedErrorOccuredInDmobile) + Constants.vbCrLf +
            // #else
            //            string errorMsg = "An Unanticipated Error Occurred in dMobile: " + Constants.vbCrLf +
            // #endif
            // "Assembly: " + assembly + Constants.vbCrLf + "Class: " + fullClassName +
            //                Constants.vbCrLf + "Method: " + reportingMethod + Constants.vbCrLf + "Type: " +
            //                ex.GetType().ToString() + Constants.vbCrLf + "Message: " + ex.Message.ToString() +
            // #if MULTILANGUAGE
            //          Constants.vbCrLf + Translator.GetInstance().Translate(TranslatorVocabulary.DoYouWishToContinue);
            // #else
            // Constants.vbCrLf + "DO YOU WISH TO CONTINUE?";
            // #endif

            // result = !(Interaction.MsgBox(errorMsg, MsgBoxStyle.YesNo,
            // #if MULTILANGUAGE
            //          Translator.GetInstance().Translate(TranslatorVocabulary.DmobileErrorOccurredDoYouWishToContinue)) == MsgBoxResult.No);
            // #else
            // "dMobile Error Occurred Do you wish to continue!") == MsgBoxResult.No);
            // #endif
            return result;
        }

        /// <summary>
        /// Word wraps the given text to fit within the specified width.
        /// </summary>
        /// <param name="text">Text to be word wrapped.</param>
        /// <param name="width">The width<see cref="int"/>.</param>
        /// <returns>Array list of strings with the modified text.</returns>
        public static List<string> WordWrap2List(string text, int width)
        {
            var sb = new List<string>();

            if (width > 0)
            {
                int pos;
                int next;

                // Parse each line of text
                for (pos = 0; pos < text.Length; pos = next)
                {
                    // Find end of line
                    var eol = text.IndexOf(Environment.NewLine, pos, StringComparison.Ordinal);
                    if (eol == -1)
                    {
                        next = eol = text.Length;
                    }
                    else
                    {
                        next = eol + Environment.NewLine.Length;
                    }

                    // Copy this line of text, breaking into smaller lines as needed
                    while (eol > pos)
                    {
                        var len = eol - pos;
                        if (len > width)
                        {
                            len = BreakLine(text, pos, width);
                        }

                        sb.Add(text.Substring(pos, len));

                        // Trim whitespace following break
                        pos += len;
                        while (pos < eol && char.IsWhiteSpace(text[pos]))
                        {
                            pos++;
                        }
                    }
                }
            }

            return sb;
        }

        /// <summary>
        /// Word wraps the given text to fit within the specified width.
        /// </summary>
        /// <param name="text">Text to be word wrapped.</param>
        /// <param name="width">The width<see cref="int"/>.</param>
        /// <returns>The modified text.</returns>
        public static string WordWrap2String(string text, int width)
        {
            ////if (width > text.Length)
            ////{
            ////    throw new Exception("width > text.Length");
            ////}

            var sb = new StringBuilder();

            if (width > 0)
            {
                int pos;
                int next;

                // Parse each line of text
                for (pos = 0; pos < text.Length; pos = next)
                {
                    // Find end of line
                    var eol = text.IndexOf(Environment.NewLine, pos, StringComparison.Ordinal);
                    if (eol == -1)
                    {
                        next = eol = text.Length;
                    }
                    else
                    {
                        next = eol + Environment.NewLine.Length;
                    }

                    // Copy this line of text, breaking into smaller lines as needed
                    if (eol > pos)
                    {
                        do
                        {
                            var len = eol - pos;
                            if (len > width)
                            {
                                len = BreakLine(text, pos, width);
                            }

                            sb.Append(text, pos, len);
                            sb.Append(Environment.NewLine);

                            // Trim whitespace following break
                            pos += len;
                            while (pos < eol && char.IsWhiteSpace(text[pos]))
                            {
                                pos++;
                            }
                        }
                        while (eol > pos);
                    }
                    else
                    {
                        sb.Append(Environment.NewLine); // Empty line
                    }
                }
            }

            return sb.ToString();
        }

        public static XmlReader GetXmlReader(ref string xml)
        {
            const string endSchema = "</xsd:schema>";
            var eoSchemaEnd = xml.IndexOf(endSchema, StringComparison.Ordinal);

            if (eoSchemaEnd >= 0)
            {
                var actualSchemaEnd = eoSchemaEnd + endSchema.Length;
                var dataSpan = xml.AsSpan(actualSchemaEnd);
                var sr = new StringReader(dataSpan.ToString());
                return XmlReader.Create(sr);
            }

            return null;
        }

        /// <summary>
        /// The GetXmlReader.
        /// </summary>
        /// <param name="xml">.</param>
        /// <returns>.</returns>
        ////public static XmlReader GetXmlReader(ref string xml)
        ////{
        ////    // Parse the Data into Schema and Data
        ////    const string endSchema = "</xsd:schema>";
        ////    var eoSchemaEnd = xml.IndexOf(endSchema, StringComparison.Ordinal);
        ////    if (eoSchemaEnd >= 0)
        ////    {
        ////        var actualSchemaEnd = eoSchemaEnd + endSchema.Length;
        ////        var dataLength = xml.Length - actualSchemaEnd;
        ////        var data = xml.Substring(actualSchemaEnd, dataLength);

        ////        ////var schema = xml[..actualSchemaEnd];
        ////        ////var xmlschema = XmlSchema.Read(new StringReader(schema), null);
        ////        ////var sc = new XmlSchemaSet();
        ////        ////sc.Add(xmlschema);
        ////        ////var settings = new XmlReaderSettings();
        ////        ////settings.ValidationType = ValidationType.None;
        ////        ////settings.Schemas = sc;

        ////        var sr = new StringReader(data);
        ////        var xmlText = XmlReader.Create(sr);

        ////        return xmlText;
        ////    }
        ////    else
        ////    {
        ////        return null;
        ////    }
        ////}

        /// <summary>
        /// The ShowMemory.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public static string ShowMemory()
        {
            var memoryInfoString = new StringBuilder();
            var runTime = Java.Lang.Runtime.GetRuntime();
            memoryInfoString.Append(
                "Total Bytes Allocated " + Environment.NewLine + GC.GetTotalMemory(true) +
                " Bytes" + Environment.NewLine);
            memoryInfoString.Append(
                Environment.NewLine + "Used Heap Memory " + Environment.NewLine +
                (runTime !.TotalMemory() - runTime !.FreeMemory()) +
                " Bytes" + Environment.NewLine);
            memoryInfoString.Append(
                Environment.NewLine + "Free Heap Memory " + Environment.NewLine +
                runTime!.FreeMemory() + " Bytes" + Environment.NewLine);
            memoryInfoString.Append(
                Environment.NewLine + "Total Heap Memory " + Environment.NewLine +
                runTime!.TotalMemory() + " Bytes" + Environment.NewLine);
            memoryInfoString.Append(
                Environment.NewLine + "Max Heap Memory " + Environment.NewLine +
                runTime!.MaxMemory() + " Bytes" + Environment.NewLine);

            return memoryInfoString.ToString();
        }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="original">Object to clone.</param>
        /// <returns>New clone object.</returns>
        public static T Clone<T>(T original)
        {
            var tempMyClass = (T)Activator.CreateInstance(original.GetType());
            var fis = original.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var fi in fis)
            {
                var fieldValue = fi.GetValue(original);
                if (fi.FieldType.Namespace != original.GetType().Namespace)
                {
                    fi.SetValue(tempMyClass, fieldValue);
                }
                else
                {
                    fi.SetValue(tempMyClass, Clone(fieldValue));
                }
            }

            return tempMyClass;
        }

        /// <summary>
        /// Convert enum dialog type value to a dialog type name string.
        /// </summary>
        /// <param name="dialogType"><see cref="EDialogTypes"/> Dialog type enum.</param>
        /// <returns><see cref="string"/> Dialog type name string.</returns>
        public static string SourceEnum2String(EDialogTypes dialogType)
        {
            return Enum.GetName(typeof(EDialogTypes), dialogType);
        }

        /// <summary>
        /// Get message string from message only exception.
        /// </summary>
        /// <param name="msgEx"><see cref="MessageOnlyException"/> Message only exception.</param>
        /// <returns><see cref="string"/> Create message string.</returns>
        public static string GetMessageFromMessageException(MessageOnlyException msgEx)
        {
            var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1)
                ? msgEx.Message
                : msgEx.Message["MSGBOX-".Length..];
            return msg;
        }

        /// <summary>
        /// The GetAssemblyVersion.
        /// </summary>
        /// <returns>.</returns>
        public string GetAssemblyVersion()
        {
            var major = Assembly.GetExecutingAssembly().GetName().Version.Major;
            var minor = Assembly.GetExecutingAssembly().GetName().Version.Minor;
            var build = Assembly.GetExecutingAssembly().GetName().Version.Build;
            var revision = Assembly.GetExecutingAssembly().GetName().Version.Revision;

            return $"{major}.{minor}.{build}.{revision}";
        }

        /// <summary>
        /// Locates position to break the given line to avoid breaking words.
        /// </summary>
        /// <param name="text">String that contains line of text.</param>
        /// <param name="pos">Index where line of text starts.</param>
        /// <param name="max">Maximum line length.</param>
        /// <returns>The modified line length.</returns>
        private static int BreakLine(string text, int pos, int max)
        {
            // Find last whitespace in line
            var i = max;

            while (i >= 0 && !char.IsWhiteSpace(text[pos + i]))
            {
                i--;
            }

            // If no whitespace found, break at maximum length
            if (i < 0)
            {
                return max;
            }

            // Find start of whitespace
            while (i >= 0 && char.IsWhiteSpace(text[pos + i]))
            {
                i--;
            }

            // Return length of text before whitespace
            return i + 1;
        }
    }
}
