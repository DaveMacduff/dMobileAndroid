namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;

    public struct SProductSchemeData
    {
        #region Fields

        private string singleTradeItemSerialNumber;
        private string logisticUnitBatchLotNumber;
        private DateTime logisticUnitBestBeforeSellByDate;
        private DateTime logisticUnitExpiryUseByDate;
        private DateTime logisticUnitProductionDate;
        private DateTime logisticUnitDueDate;
        private DateTime logisticUnitPackagingDate;
        private string logisticUnitSkidNumber;
        private string variantCode;
        private string variantDescription;
        private int variantNumber;

        #endregion

        #region Properties

        public string SingleTradeItemSerialNumber
        {
            get
            {
                if (string.IsNullOrEmpty(this.singleTradeItemSerialNumber))
                {
                    this.singleTradeItemSerialNumber = string.Empty;
                }

                return this.singleTradeItemSerialNumber;
            }

            set
            {
                this.singleTradeItemSerialNumber = value;
            }
        }

        public string LogisticUnitBatchLotNumber
        {
            get
            {
                if (string.IsNullOrEmpty(this.logisticUnitBatchLotNumber))
                {
                    this.logisticUnitBatchLotNumber = string.Empty;
                }

                return this.logisticUnitBatchLotNumber;
            }

            set
            {
                this.logisticUnitBatchLotNumber = value;
            }
        }

        public DateTime LogisticUnitBestBeforeSellByDate
        {
            get
            {
                if (SysCommon.IsEmptyDateTime(this.logisticUnitBestBeforeSellByDate))
                {
                    this.logisticUnitBestBeforeSellByDate = new DateTime(1900, 1, 1);
                }

                return this.logisticUnitBestBeforeSellByDate;
            }

            set
            {
                this.logisticUnitBestBeforeSellByDate = value;
            }
        }

        public DateTime LogisticUnitExpiryUseByDate
        {
            get
            {
                if (SysCommon.IsEmptyDateTime(this.logisticUnitExpiryUseByDate))
                {
                    this.logisticUnitExpiryUseByDate = new DateTime(1900, 1, 1);
                }

                return this.logisticUnitExpiryUseByDate;
            }

            set
            {
                this.logisticUnitExpiryUseByDate = value;
            }
        }

        public DateTime LogisticUnitProductionDate
        {
            get
            {
                if (SysCommon.IsEmptyDateTime(this.logisticUnitProductionDate))
                {
                    this.logisticUnitProductionDate = new DateTime(1900, 1, 1);
                }

                return this.logisticUnitProductionDate;
            }

            set
            {
                this.logisticUnitProductionDate = value;
            }
        }

        public DateTime LogisticUnitDueDate
        {
            get
            {
                if (SysCommon.IsEmptyDateTime(this.logisticUnitDueDate))
                {
                    this.logisticUnitDueDate = new DateTime(1900, 1, 1);
                }

                return this.logisticUnitDueDate;
            }

            set
            {
                this.logisticUnitDueDate = value;
            }
        }

        public DateTime LogisticUnitPackagingDate
        {
            get
            {
                if (SysCommon.IsEmptyDateTime(this.logisticUnitPackagingDate))
                {
                    this.logisticUnitPackagingDate = new DateTime(1900, 1, 1);
                }

                return this.logisticUnitPackagingDate;
            }

            set
            {
                this.logisticUnitPackagingDate = value;
            }
        }

        public string LogisticUnitSkidNumber
        {
            get
            {
                if (string.IsNullOrEmpty(this.logisticUnitSkidNumber))
                {
                    this.logisticUnitSkidNumber = string.Empty;
                }

                return this.logisticUnitSkidNumber;
            }

            set
            {
                this.logisticUnitSkidNumber = value;
            }
        }

        public string VariantCode
        {
            get
            {
                if (string.IsNullOrEmpty(this.variantCode))
                {
                    this.variantCode = string.Empty;
                }

                return this.variantCode;
            }

            set
            {
                this.variantCode = value;
            }
        }

        public int VariantNumber
        {
            get
            {
                return this.variantNumber;
            }

            set
            {
                this.variantNumber = value;
            }
        }

        public string VariantDescription
        {
            get
            {
                if (string.IsNullOrEmpty(this.variantDescription))
                {
                    this.variantDescription = string.Empty;
                }

                return this.variantDescription;
            }

            set
            {
                this.variantDescription = value;
            }
        }

        #endregion
    }

    public class SysCommon
    {
        #region Fields

        public const int SystemLoginID = 30;
        public const string SystemLoginPassword = "4274";
        public const int SetupLoginID = 99;
        public const string SetupLoginPassword = "99";

        #region Enumerations

        public enum ShippingLabelQuantitySelectionMode : int
        {
            BothFullRepack = 0,
            FullOnly = 1,
            RepackOnly = 2
        }

        public enum ShowErrorMessageType
        {
            /// <summary>
            /// Error Report Description
            /// </summary>
            ErrorReport = 0,
            MessageBox = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActPickingQtyOverValLevels : int
        {
            NoValidationOrWarning = 0,
            ValidateWarnAndStop = 1,
            ValidateWarnAndContinue = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActPickingQtyUnderValLevels : int
        {
            NoValidationOrWarning = 0,
            ValidateWarnAndStop = 1,
            ValidateWarnAndContinue = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActTransfersQtyOverValLevels : int
        {
            NoValidationOrWarning = 0,
            ValidateWarnAndStop = 1,
            ValidateWarnAndContinue = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActTransfersQtyUnderValLevels : int
        {
            NoValidationOrWarning = 0,
            ValidateWarnAndStop = 1,
            ValidateWarnAndContinue = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActReceivingQtyOverValLevels : int
        {
            NoValidationOrWarning = 0,
            ValidateWarnAndStop = 1,
            ValidateWarnAndContinue = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActReceivingQtyUnderValLevels : int
        {
            NoValidationOrWarning = 0,
            ValidateWarnAndStop = 1,
            ValidateWarnAndContinue = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum AdjTypes : int
        {
            Inventory = 0,
            UnitConversion = 1,
            Rework = 2,
            PartialRework = 3,
            Production = 4,
            PartialProduction = 5,
            Disposal = 6,
            Fresh2Frozen = 7,
            Transfer = 8,
            PackOff = 9,
            LostFound = 10,
            CycleCount = 11,
            MoveToDeli = 12,
            PeriodicCount = 13,
            Manufacture = 14,
            Requisition = 15,
            AutoConvertTo = 16,
            AutoCreateFrom = 17,
            SkidPutaway = 18,
            ConvertProduct = 19,
            Debox = 20,
            ConvertSkid = 21
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum BarCodeSymbologies : int
        {
            UNDEFINED = 0,
            UCC_12_UPC_E = 1,
            EAN_UCC_14 = 2,
            EAN_UCC_13 = 3,
            EAN_UCC_8 = 4,
            ITF_14_Symbol = 5,
            UCC_EAN_128_Symbol = 6,
            Code_128 = 7,
            SSCC_18 = 8,
            Code_39 = 9,
            EAN_UPC_Symbol = 10,
            Code_93 = 11,
            Codabar = 12,
            Code_2_of_5 = 13,
            MSI = 14,
            Plessey = 15,
            Code_11 = 16,
            Matrix_2_of_5 = 17,
            Telepen = 18,
            PDF417 = 19,
            Micro_PDF417 = 20,
            RSS_14 = 21,
            RSS_Limited = 22,
            RSS_Expanded = 23,
            Codablock_A = 24,
            Codablock_F = 25,
            TLC_39 = 26,
            Codablock = 27,
            Codablock256 = 28
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActStatus : int
        {
            NoChange = 0,
            Inserted = 1,
            Updated = 2,
            Deleted = 3
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum SchemeExtraTypes : int
        {
            Disabled = 0,
            Text = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActDetailSchemeVariantXRefFields : int
        {
            ActDetailRefno = 0,
            ProdSchemeVariantRefno = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActDetailFields : int
        {
            Refno = 0,
            SubType = 1,
            Status = 2,
            ActivityItemRefno = 3,
            LocationMapRefno = 4,
            ProductUnitRefno = 5,
            UniqueSchemeID = 6,
            SchemeID = 7,
            Quantity = 8,
            BaseQuantity = 9,
            WeightQuantity = 10,
            SingleTradeItemSerialNumber = 11,
            LogisticUnitBatchLotNumber = 12,
            LogisticUnitBestBeforeSellByDate = 13,
            LogisticUnitExpiryUseByDate = 14,
            LogisticUnitProductionDate = 15,
            LogisticUnitDueDate = 16,
            LogisticUnitPackagingDate = 17,
            LogisticUnitSkidNumber = 18
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActDetailExtraFields : int
        {
            ActDetailRefno = 0,
            ActDetailSubType = 1,
            Text1 = 2,
            Text2 = 3,
            Text3 = 4,
            Text4 = 5,
            DateTime5 = 6
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActItemFields : int
        {
            Refno = 0,
            SubType = 1,
            Status = 2,
            ActRefno = 3
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActFields : int
        {
            Refno = 0,
            Status = 1,
            SubType = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActCountFields : int
        {
            ActRefno = 0,
            ActSubType = 1,
            FKRefno = 2,
            StringFKRefno = 3,
            WhsLocMapRefno = 4,
            Type = 5,
            DateTime = 6,
            TypeDescription = 7,
            Description = 8
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActItemCountFields : int
        {
            ActItemRefno = 0,
            ActItemSubType = 1,
            FKRefno = 2,
            StringFKRefno = 3,
            ProdUnitRefno = 4,
            LocMapRefno = 5,
            QuantityOnHand = 6,
            BaseQuantityOnHand = 7,
            WeightQuantityOnHand = 8,
            Quantity = 9,
            BaseQuantity = 10,
            WeightQuantity = 11
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActDetailCountFields : int
        {
            ActDetailRefno = 0,
            FKRefno = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActReceivingFields : int
        {
            ActRefno = 0,
            ActSubType = 1,
            FKRefno = 2,
            StringFKRefno = 3,
            PONumber = 4,
            WhsLocMapRefno = 5,
            IssuedDateTime = 6,
            ArrivalDateTime = 7,
            VendorContactName = 8,
            VendorSupplierContactName = 9,
            VendorSupplierGTINCompanyPrefix = 10
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActItemReceivingFields : int
        {
            ActItemRefno = 0,
            ActItemSubType = 1,
            FKRefno = 2,
            StringFKRefno = 3,
            ProdUnitRefno = 4,
            WhsLocMapRefno = 5,
            QuantityOrdered = 6,
            BaseQuantityOrdered = 7,
            WeightQuantityOrdered = 8,
            QuantityReceived = 9,
            BaseQuantityReceived = 10,
            WeightQuantityReceived = 11,
            VendorProductUnitCode = 12,
            Description = 13,
            QuantityReceivedToDate = 14,
            BaseQuantityReceivedToDate = 15,
            WeightQuantityReceivedToDate = 16
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActItemReceivingCommentFields : int
        {
            ActItemRefno = 0,
            Comment = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActDetailReceivingFields : int
        {
            ActDetailRefno = 0,
            FKRefno = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActItemWorkSheetFields : int
        {
            ActItemRefno = 0,
            ActItemSubType = 1,
            FKRefno = 2,
            StringFKRefno = 3,
            ProdUnitRefno = 4,
            WhsLocMapRefno = 5,
            QuantityOrdered = 6,
            BaseQuantityOrdered = 7,
            WeightQuantityOrdered = 8
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActPickingRouteFields : int
        {
            PickingActRefno = 0,
            RouteRefno = 1,
            RouteStop = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActPickingFields : int
        {
            ActRefno = 0,
            ActSubType = 1,
            FKRefno = 2,
            StringFKRefno = 3,
            WhsLocMapRefno = 4,
            OrderedDateTime = 5,
            DeliveryDateTime = 6,
            CustomerContactName = 7,
            CustomerShipToInfo = 8,
            IsReviewed = 9
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActReturnsFields : int
        {
            ActRefno = 0,
            ActSubType = 1,
            FKRefno = 2,
            StringFKRefno = 3,
            WhsLocMapRefno = 4,
            OrderedDateTime = 5,
            DeliveryDateTime = 6,
            CustomerContactName = 7,
            CustomerShipToInfo = 8
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActItemPickingFields : int
        {
            ActItemRefno = 0,
            ActItemSubType = 1,
            FKRefno = 2,
            StringFKRefno = 3,
            ProdUnitRefno = 4,
            WhsLocMapRefno = 5,
            QuantityOrdered = 6,
            BaseQuantityOrdered = 7,
            WeightQuantityOrdered = 8,
            QuantityShipped = 9,
            BaseQuantityShipped = 10,
            WeightQuantityShipped = 11,
            CustomerProductUnitCode = 12,
            Description = 13,
            QuantityAvailable = 14,
            BaseQuantityAvailable = 15,
            WeightQuantityAvailable = 16,
            IsReviewed = 17
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActItemReturnsFields : int
        {
            ActItemRefno = 0,
            ActItemSubType = 1,
            FKRefno = 2,
            StringFKRefno = 3,
            ProdUnitRefno = 4,
            WhsLocMapRefno = 5,
            QuantityReturned = 6,
            BaseQuantityReturned = 7,
            WeightQuantityReturned = 8,
            CustomerProdUnitCode = 9,
            Description = 10
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActItemPickingCommentFields : int
        {
            ActItemRefno = 0,
            Comment = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActItemReturnsCommentFields : int
        {
            ActItemRefno = 0,
            Comment = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActDetailPickingFields : int
        {
            ActDetailRefno = 0,
            FKRefno = 1,
            IsConfirmed = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActDetailReturnsFields : int
        {
            ActDetailRefno = 0,
            FKRefno = 1,
            IsConfirmed = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActTransferFields : int
        {
            ActRefno = 0,
            ActSubType = 1,
            FKRefno = 2,
            StringFKRefno = 3,
            TransferToWhsLocMapRefno = 4,
            Description = 5,
            DateTime = 6,
            ArrivalDateTime = 7
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActItemTransferFields : int
        {
            ActItemRefno = 0,
            ActItemSubType = 1,
            FKRefno = 2,
            StringFKRefno = 3,
            ProductUnitRefno = 4,
            TransferFromWhsLocMapRefno = 5,
            RequiredQuantity = 6,
            Quantity = 7,
            BaseQuantity = 8,
            WeightQuantity = 9
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActDetailTransferFields : int
        {
            ActDetailRefno = 0,
            FKRefno = 1,
            IsConfirmed = 2,
            TransModeEnum = 3
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum BarCodeRules : int
        {
            ParseCode_128AsUCC_EAN_128_SymbolCompliant = 1,
            ParseFINICA_Code_128_as_EAN_UCC_128 = 2,
            NoCheckDigitInGlobalLocationNumber = 3,
            NoZeroPaddingOfProductIdentifierForUCC_EAN_Family = 4,
            ParseWOOLWICH_Code_128_RemoveSpaces = 5,
            Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate = 6,
            Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate = 7,
            Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount = 8,
            ProductBarCodeMinimumLength4Digits = 9,
            Parse_UCC_12_UPC_E_as_Deli_PLU = 10,
            Parse_Code128_OvationFielding_Lamb = 11,
            Parse_Code128_China_Seafood = 12,
            Parse_Code_128_AI01_BC_AI30_exclusive = 13,
            ParseRenees_Code_128_RemoveSpaces = 14,
            ParseLanzMeats_Code_128_AI10_as_AI21 = 15,
            HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit = 16,
            ParseNossackBrokerMultiNationalProductFromChili = 17,
            ParseUCC_13asUPC_12_UPC_E = 18,
            Parse_HelaSpiceCanada_Code_128 = 19,
            NebraskaBeef_Parse_Lot_as_Serial_Code_128 = 20,
            ParseNorthernCoop_AustraliaBeefTrim_Product25612 = 21,
            ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct = 22,
            ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs = 23,
            ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 = 24,
            ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents = 25,
            ParseEAN_UCC128_Nossack_GreenleaPremierMeats = 26,
            ParseEAN_UCC128_Dovre_KiiNaturalsCrisps = 27,
            Parse_Code128_CleanSubstituteCharacter = 28,

            /// <summary>
            /// Restrict the amount on deli plu's from 5 to 4.
            /// </summary>
            /// <remarks>12345 becomes 2345.</remarks>
            Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount_Enabled = 29
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Keep in sync with imsck_wms.datacntrl method request_skid_info_cursor() enums.
        /// </remarks>
        public enum ActSubTypes : int
        {
            NotApplicable = 0,
            BarCodeAssociation = 1,
            PhysicalCounts = 2,
            Receiving = 3,
            Picking = 4,
            TransferFrom = 5,
            RefreshData = 6,
            DataStatus = 7,
            Returns = 8,
            SelectProduct = 9,
            SelectLocation = 10,
            CycleCounts = 11,
            ConvertUnit = 12,
            MoveInventory = 13,
            Adjustment = 14,
            PurgeActivityData = 15,
            TicketFinished = 16,
            TicketMaterial = 17,
            TicketAddProduct = 18,
            ReleaseByOrder = 19,
            ReleaseByItemSection = 20,
            PurchaseOrderWorkSheet = 21,
            SkidInfo = 22,
            MoveSkid = 23,
            BreakSkid = 24,
            PickingSingle = 25,
            PutAwaySkid = 26,
            AssignDefaultLocation = 27,
            InventoryInquiry = 28,
            InventoryReplenishment = 29,
            LoginStartup = 30,
            SplashStartup = 31,
            PrintProductLabel = 32,
            PickingSelectCriteria = 33,
            PickingFilter = 34,
            SubmitBatch = 35,
            CommunicationSetup = 36,
            SetupDefaults = 37,
            InventoryInquirySummary = 38,
            ConvertProduct = 39,
            TransferTo = 40
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActCountTypeFields : int
        {
            PhysicalCountByWarehouse = 1,
            CycleCountByWarehouse = 2,
            CycleCountBySection = 3,
            CycleCountBySlot = 4,
            BaseCycleCountByWarehouse = 5
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ContactSubTypes : int
        {
            User = 1,
            Customer = 2,
            CustomerShipTo = 3,
            Vendor = 4,
            VendorSupplier = 5
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum RouteFields : int
        {
            Refno = 0,
            Code = 1,
            Description = 2,
            DayOfWeek = 3
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ContactFields : int
        {
            Refno = 0,
            SubType = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ContactUserFields : int
        {
            ContactRefno = 0,
            ContactSubType = 1,
            Code = 2,
            Name = 3,
            Password = 4,
            IsAllowedAssocProdBarCode = 5,
            IsAllowedPickingOverridePicker = 6,
            IsAllowedPickingModuleOverrideAddManualDetail = 7,
            UserLocalizationLanguage = 8
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ProdLocMapTypeFields : int
        {
            ProdRefno = 0,
            WhsLocMapRefno = 1,
            EnumType = 2,
            LocMapRefno = 3
        }
        
        /// <summary>
        /// 
        /// </summary>
        public enum WhsLocMapProdSchemeFields : int
        {
            WhsLocMapRefno = 0,
            ProdRefno = 1,
            ProdSchemeRefno = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum WhsLocMapTypeFields : int
        {
            WhsLocMapRefno = 0,
            EnumType = 1,
            LocMapRefno = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ProdUnitRelationFields : int
        {
            ProdUnit1Refno = 0,
            ProdUnit2Refno = 1,
            Factor = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ProdUnitBarCodeFields : int
        {
            Refno = 0,
            ProdUnitRefno = 1,
            Code = 2,
            Raw = 3,
            SymbologyEnum = 4,
            Modifier1 = 5,
            Status = 6
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ProdUnitFields : int
        {
            Refno = 0,
            ProdRefno = 1,
            UnitRefno = 2,
            BaseUnitRefno = 3,
            WtUnitRefno = 4,
            IsBC = 5,
            BC = 6,
            Weight = 7,
            SizeDescription = 8,
            IsSellUnit = 9,
            IsDefault = 10,
            BoxCount = 11
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ProductFields : int
        {
            Refno = 0,
            FamilyRefno = 1,
            Code = 2,
            FKRefno = 3,
            Description = 4,
            IsCatchCount = 5,
            IsCatchWeight = 6,
            IsCatchAsk = 7,
            IsCatchAskZero = 8,
            BrandDescription = 9,
            AllowConvertProduct = 10
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ProdSchemeVariantXRefFields : int
        {
            ProdSchemeRefno = 0,
            ProdSchemeVariantRefno = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ProdSchemeVariantFields : int
        {
            Refno = 0,
            Code = 1,
            Description = 2,
            No = 3,
            IsDefault = 4
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ProdSchemeLotNumberXRefFields : int
        {
            ProdSchemeRefno = 0,
            ProdSchemeLotNumberRefno = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ProdSchemeLotNumberFields : int
        {
            Refno = 0,
            Code = 1,
            CodeDefault = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum FamilyFields
        {
            Refno = 0,
            Code = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ProdSchemeFields : int
        {
            Refno = 0,
            Code = 1,
            IsSerialTracked = 2,
            IsProductionDateTracked = 3,
            IsLotTracked = 4,
            IsBestBeforeDateTracked = 5,
            IsVariantTracked = 6,
            IsLotUniqueComponent = 7,
            IsProductionDateUniqueComponent = 8,
            IsBestBeforeDateUniqueComponent = 9,
            IsVariantUniqueComponent = 10,
            Extra1Type = 11,
            Extra2Type = 12,
            Extra3Type = 13,
            Extra4Type = 14,
            Extra5Type = 15,
            Extra1Caption = 16,
            Extra2Caption = 17,
            Extra3Caption = 18,
            Extra4Caption = 19,
            Extra5Caption = 20,
            Extra1IsRequired = 21,
            Extra2IsRequired = 22,
            Extra3IsRequired = 23,
            Extra4IsRequired = 24,
            Extra5IsRequired = 25,
            IsLotTrackedDefault = 26,
            IsProductionDateTrackedDefault = 27,
            IsBestBeforeDateTrackedDefault = 28
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ProdLocMapTypes : int
        {
            Floor = 1,
            Picking = 2,
            Receiving = 3,
            QualityAssurance = 4,
            IssuedTo = 5,
            ByProducts = 6,
            FinishedGoods = 7
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum LocMapTypes : int
        {
            IsTracked = 1,
            IsStaging = 2,
            IsFreezer = 3,
            IsReserved = 4,
            IsToBeOrdered = 5,
            IsHandStack = 6,
            IsGroundLevel = 7,
            IsProcessing = 8
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum LocMapTypeFields : int
        {
            LocMapRefno = 0,
            EnumType = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum LocMapSlotCoordTypes : int
        {
            Coord1 = 1,
            Coord2 = 2,
            Coord3 = 3
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum LocMapSlotCoordFields : int
        {
            Refno = 0,
            EnumType = 1,
            WhsLocMapRefno = 2,
            Code = 3
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum LocMapSubTypes : int
        {
            None = 0,
            Warehouse = 1,
            Section = 2,
            Slot = 3
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum LocMapFields : int
        {
            Refno = 0,
            SubType = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum LocMapSlotFields : int
        {
            LocMapRefno = 0,
            LocMapSubType = 1,
            SectLocMapRefno = 2,
            Code = 3,
            Name = 4,
            SlotCoord1LocMapRefno = 5,
            SlotCoord2LocMapRefno = 6,
            SlotCoord3LocMapRefno = 7,
            FKRefno = 8,
            BarCode = 9,
            WhsLocMapRefno = 10
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum LocMapSectFields : int
        {
            LocMapRefno = 0,
            LocMapSubType = 1,
            WhsLocMapRefno = 2,
            Code = 3,
            Name = 4,
            FKRefno = 5,
            BarCode = 6
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum LocMapWhsFields : int
        {
            LocMapRefno = 0,
            LocMapSubType = 1,
            Name = 2,
            Code = 3,
            FKRefno = 4,
            BarCode = 5,
            IsWMS = 6,
            WhsMask = 7,
            SectionMask = 8,
            SlotMask = 9,
            SlotCoord1Mask = 10,
            SlotCoord2Mask = 11,
            SlotCoord3Mask = 12,
            SlotCoord1Name = 13,
            SlotCoord2Name = 14,
            SlotCoord3Name = 15,
            IsSectionTracked = 16,
            IsSlotTracked = 17,
            IsAdhocEnabled = 18
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum UnitFields : int
        {
            UnitRefno = 0,
            UnitCode = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActAdjFields
        {
            ActRefno = 0,
            ActSubType = 1,
            FKRefno = 2,
            StringFKRefno = 3,
            Type = 4,
            WhsLocMapRefno = 5,
            DateTime = 6,
            Description = 7,
            TypeDescription = 8
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActItemAdjFields : int
        {
            ActItemRefno = 0,
            ActItemSubType = 1,
            FKRefno = 2,
            StringFKRefno = 3,
            ProdUnitRefno = 4,
            WhsLocMapRefno = 5,
            Quantity = 6,
            BaseQuantity = 7,
            WeightQuantity = 8,
            Description = 9
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActDetailAdjFields : int
        {
            ActDetailRefno = 0,
            FKRefno = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActProdFields : int
        {
            ActRefno = 0,
            ActSubType = 1,
            WhsLocMapRefno = 2,
            DateTime = 3,
            Description = 4
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActProdFinishedFields : int
        {
            ActivityRefno = 0,
            FKRefno = 1,
            StringFKRefno = 2,
            DefaultLogisticUnitBatchLotNumber = 3
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActProdMaterialFields : int
        {
            ActRefno = 0,
            FKRefno = 1,
            StringFKRefno = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActProdByProductFields : int
        {
            ActRefno = 0,
            FKRefno = 1,
            StringFKRefno = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActItemProdFields : int
        {
            ActItemRefno = 0,
            ActivityItemSubType = 1,
            ProdUnitRefno = 2,
            LocMapRefno = 3,
            Quantity = 4,
            BaseQuantity = 5,
            WeightQuantity = 6,
            Description = 7,
            QuantityExpected = 8,
            BaseQuantityExpected = 9,
            WeightQuantityExpected = 10
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActItemProdFinishedFields : int
        {
            ActItemRefno = 0,
            FKRefno = 1,
            StringFKRefno = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActItemProdMaterialFields : int
        {
            ActItemRefno = 0,
            FKRefno = 1,
            StringFKRefno = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActItemProdByProductFields : int
        {
            ActItemRefno = 0,
            FKRefno = 1,
            StringFKRefno = 2
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActDetailProdFinishedFields : int
        {
            ActDetailRefno = 0,
            FKRefno = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActDetailProdMaterialFields : int
        {
            ActDetailRefno = 0,
            FKRefno = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public enum ActDetailProdByProductFields : int
        {
            ActDetailRefno = 0,
            FKRefno = 1
        }

        #endregion

        #endregion

        #region Public Members

        /// <summary>
        /// Replace XML reserved characters in a string with the XML entity references.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Well formed xml string</returns>
        public static string AddXMLReservedChars(string data)
        {
            string xmlString = string.Empty;

            xmlString = data.Replace("<", "&lt;");
            xmlString = xmlString.Replace(">", "&gt;");
            xmlString = xmlString.Replace("\"", "&quot;");
            xmlString = xmlString.Replace("'", "&apos;");
            xmlString = xmlString.Replace("&", "&amp;");

            return xmlString;
        }

        /// <summary>
        /// Replace the XML entity references in a string with the reserved characters.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Restored string</returns>
        public static string RemoveXMLReservedChars(string data)
        {
            string xmlString = string.Empty;

            xmlString = data.Replace("&lt;", "<");
            xmlString = xmlString.Replace("&gt;", ">");
            xmlString = xmlString.Replace("&quot;", "\"");
            xmlString = xmlString.Replace("&apos;", "'");
            xmlString = xmlString.Replace("&amp;", "&");

            return xmlString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adjustmentType"></param>
        /// <returns></returns>
        public static bool IsAllowedChangeNegateQuantityFromAdjType(AdjTypes
            adjustmentType)
        {
            bool isAllowedNegateQuantity = false;

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
            }

            return isAllowedNegateQuantity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adjustmentType"></param>
        /// <returns></returns>
        public static bool DefaultNegateQuantityFromAdjType(AdjTypes
            adjustmentType)
        {
            bool isNegateQuantity = false;

            switch (adjustmentType)
            {
                case AdjTypes.CycleCount:
                    break;

                case AdjTypes.Disposal:
                    isNegateQuantity = true;
                    break;

                case AdjTypes.Fresh2Frozen:
                    isNegateQuantity = true;
                    break;

                case AdjTypes.Inventory:
                    break;

                case AdjTypes.LostFound:
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
                    isNegateQuantity = true;
                    break;

                case AdjTypes.PeriodicCount:
                    break;

                case AdjTypes.Production:
                    break;

                case AdjTypes.Rework:
                    isNegateQuantity = true;
                    break;

                case AdjTypes.Transfer:
                    break;

                case AdjTypes.UnitConversion:
                    isNegateQuantity = true;
                    break;

                case AdjTypes.Requisition:
                    isNegateQuantity = true;
                    break;

                case AdjTypes.AutoConvertTo:
                    isNegateQuantity = true;
                    break;

                case AdjTypes.AutoCreateFrom:
                    break;

                case AdjTypes.SkidPutaway:
                    isNegateQuantity = true;
                    break;

                case AdjTypes.ConvertProduct:
                    isNegateQuantity = true;
                    break;

                case AdjTypes.Debox:
                    isNegateQuantity = true;
                    break;

                case AdjTypes.ConvertSkid:
                    isNegateQuantity = true;
                    break;
            }

            return isNegateQuantity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbology"></param>
        /// <returns></returns>
        public static string GetSymbologyPrefixFromSymbologyEnum(BarCodeSymbologies symbology)
        {
            string symbologyPrefix = string.Empty;

            switch (symbology)
            {
                case BarCodeSymbologies.UNDEFINED:

                    // or ']O4'
                    symbologyPrefix = "]01";
                    break;

                case BarCodeSymbologies.UCC_12_UPC_E:
                    symbologyPrefix = "]X0";
                    break;

                case BarCodeSymbologies.EAN_UCC_14:
                    symbologyPrefix = string.Empty;
                    break;

                case BarCodeSymbologies.EAN_UCC_13:
                    symbologyPrefix = "]E0";
                    break;

                case BarCodeSymbologies.EAN_UCC_8:
                    symbologyPrefix = "]E4";
                    break;

                case BarCodeSymbologies.ITF_14_Symbol:
                    symbologyPrefix = "]I";
                    break;

                case BarCodeSymbologies.UCC_EAN_128_Symbol:
                    symbologyPrefix = "]C1";
                    break;

                case BarCodeSymbologies.Code_128:
                    symbologyPrefix = "]C";
                    break;

                case BarCodeSymbologies.SSCC_18:
                    symbologyPrefix = string.Empty;
                    break;

                case BarCodeSymbologies.Code_39:
                    symbologyPrefix = "]A";
                    break;

                case BarCodeSymbologies.EAN_UPC_Symbol:
                    symbologyPrefix = "]E";
                    break;

                case BarCodeSymbologies.Code_93:
                    symbologyPrefix = "]G";
                    break;

                case BarCodeSymbologies.Codabar:
                    symbologyPrefix = "]F";
                    break;

                case BarCodeSymbologies.Code_2_of_5:
                    symbologyPrefix = string.Empty;
                    break;

                case BarCodeSymbologies.MSI:
                    symbologyPrefix = "]M";
                    break;

                case BarCodeSymbologies.Plessey:
                    symbologyPrefix = string.Empty;
                    break;

                case BarCodeSymbologies.Code_11:
                    symbologyPrefix = "]H";
                    break;

                case BarCodeSymbologies.Matrix_2_of_5:
                    symbologyPrefix = string.Empty;
                    break;

                case BarCodeSymbologies.Telepen:
                    symbologyPrefix = "]B";
                    break;

                case BarCodeSymbologies.PDF417:
                    symbologyPrefix = "]L";
                    break;

                case BarCodeSymbologies.Micro_PDF417:
                    symbologyPrefix = string.Empty;
                    break;

                case BarCodeSymbologies.RSS_14:
                    symbologyPrefix = string.Empty;
                    break;

                case BarCodeSymbologies.RSS_Limited:
                    symbologyPrefix = string.Empty;
                    break;

                case BarCodeSymbologies.RSS_Expanded:
                    symbologyPrefix = "]e0";
                    break;

                case BarCodeSymbologies.Codablock_A:
                    symbologyPrefix = "]O6";
                    break;

                case BarCodeSymbologies.Codablock_F:
                    symbologyPrefix = "]O5";
                    break;

                case BarCodeSymbologies.TLC_39:
                    symbologyPrefix = string.Empty;
                    break;

                case BarCodeSymbologies.Codablock:
                    symbologyPrefix = "]O";
                    break;

                case BarCodeSymbologies.Codablock256:
                    symbologyPrefix = "]02";
                    break;

                default:
                    symbologyPrefix = string.Empty;
                    break;
            }

            return symbologyPrefix;
        }

        /// <summary>
        /// device_id, login_id, revision_date, stamp
        /// </summary>
        /// <param name="useRefreshedDataLast"></param>
        /// <returns></returns>
        public static string GetRequestDefaults(bool useRefreshedDataLast)
        {
            SysEnvironment sysEnv = SysEnvironment.GetInstance();
            string current = DateTime.Now.ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
            string revision = new DateTime(1900, 1, 1).ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
            if (useRefreshedDataLast)
            {
                revision = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
            }

            string def = ("<device_id>" + sysEnv.DeviceID + "</device_id>")
                + ("<login_id>" + sysEnv.ContactLog.LoginID + "</login_id>")
                + ("<revision_datetime>" + revision + "</revision_datetime>")
                + ("<stamp>" + current + "</stamp>");

            return def;
        }

        /// <summary>
        /// Check for any activities with change state not equal to ActivityStatus.NoChange
        /// </summary>
        /// <returns>True or false if data unsubmitted data exists</returns>
        /// <remarks></remarks>
        public static bool CheckForUnSubmittedData()
        {
            bool isDataExists = false;

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT tActivity.ActivityRefno ");
            sql.Append("FROM tActivity ");
            sql.AppendFormat("WHERE tActivity.ActivityStatus <> {0} ", Convert.ToInt32(ActStatus.NoChange));
            sql.Append("UNION ");
            sql.Append("SELECT tActivity.ActivityRefno ");
            sql.Append("FROM tActivity ");
            sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
            sql.AppendFormat("WHERE tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(ActStatus.NoChange));
            sql.Append("UNION ");
            sql.Append("SELECT tActivity.ActivityRefno ");
            sql.Append("FROM tActivity ");
            sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
            sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = tActivityItem.ActivityItemRefno ");
            sql.AppendFormat("WHERE tActivityDetail.ActivityDetailStatus <> {0}", Convert.ToInt32(ActStatus.NoChange));

            using (SQLiteCommand cmndDetail = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndDetail.CommandText = sql.ToString();
                cmndDetail.CommandType = CommandType.Text;

                if (cmndDetail.ExecuteNonQuery() != 0)
                {
                    isDataExists = true;
                }
            }

            return isDataExists;
        }

        /// <summary>
        /// Make a nice formatted string of quantity and product units
        /// </summary>
        /// <param name="prodInfo"></param>
        /// <param name="quantity"></param>
        /// <param name="weightQuantity"></param>
        /// <returns>Formatted nice readable string '12.000 CS @ 150.546 KG'</returns>
        /// <remarks></remarks>
        public static string NiceQuantityReadableString(ProductInfo prodInfo, decimal quantity, decimal weightQuantity)
        {
            string unit = Math.Round(quantity, 3).ToString() + " " + prodInfo.UnitCode;
            string wtUnit = Math.Round(weightQuantity, 3).ToString() + " " + prodInfo.WeightUnitCode;

            return unit + " @ " + wtUnit;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodInfo"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static SFillQuantity FillQuantityStructureFromQuantity(ProductInfo prodInfo, decimal quantity)
        {
            SFillQuantity fill = new SFillQuantity();

            if (quantity == 0)
            {
                if (prodInfo.UnitCode == "LB" | prodInfo.UnitCode == "KG")
                {
                    fill.Quantity = prodInfo.UnitWeight;
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

            if (prodInfo.UnitCode == "LB" | prodInfo.UnitCode == "KG")
            {
                if (fill.Quantity != 0)
                {
                    fill.WeightQuantity = fill.Quantity;
                }
                else
                {
                    fill.WeightQuantity = prodInfo.UnitWeight;
                }
            }
            else
            {
                fill.WeightQuantity = prodInfo.UnitWeight;
            }

            if (prodInfo.BaseUnitCode == "LB" | prodInfo.BaseUnitCode == "KG")
            {
                fill.BaseQuantity = fill.WeightQuantity;
            }
            else
            {
                if (prodInfo.Unit2BaseUnitFactor != 0)
                {
                    fill.BaseQuantity = fill.Quantity *
                        prodInfo.Unit2BaseUnitFactor;
                }
                else
                {
                    fill.BaseQuantity = fill.Quantity;
                }
            }

            return fill;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sendData"></param>
        /// <returns></returns>
        public static string SubmitComm(string sendData)
        {
            SysEnvironment sysEnv = SysEnvironment.GetInstance();
            Socket sysSock = null;
            int bytesSent = default(int);
            List<byte> compressedData = null;
            string recvData = string.Empty;

            try
            {
                sysSock = sysEnv.SysSock.Connect();
                if (sysSock != null)
                {
                    bytesSent = sysEnv.SysSock.SendRequest(sysSock, sendData);
                    compressedData = sysEnv.SysSock.ReceiveResponse(sysSock);
                    
                    recvData = Compress.UnZip(compressedData.ToArray());

                    // Only errors from the comm server will contain reserved xml characters that need special handling.
                    // Table data with special characters will be handled when being loaded in the xml.dom object.
                    if (recvData.Contains("<message>ERROR</message>"))
                    {
                        recvData = SysCommon.RemoveXMLReservedChars(recvData);
                    }
                }
                else
                {
                    // TODO: Resource strings
                    throw new Exception("Failed to connect.");
                }
            }
            catch (SocketException ex)
            {
                if (sysSock != null)
                {
                    sysSock.Close();
                }

                // TODO: Resource strings
                throw new Exception("SubmitComm socket error occurred.", ex);
            }
 
            sysSock.Close();
            return recvData;
        }


        /// <summary>
        /// Checks a COM submission reponse string for errors.
        /// Throws exceptions if errors are found.
        /// </summary>
        /// <param name="response">XML response string.</param>
        /// <exception cref="MessageOnlyException"/>
        /// <exception cref="Exception"/>
        public static void CheckCommResponseForErrors(string response)
        {
            if (response == null)
            {
                throw new Exception("COM Response is null and contains no data.");
            }

            if (response.Contains("<message>ERROR</message>"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response);
                string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;
                if (errorDetail.Contains("MSGBOX-"))
                {
                    throw new MessageOnlyException(errorDetail);
                }
                else
                {
                    throw new Exception(errorDetail);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsEmptyDateTime(DateTime dt)
        {
            bool isEmpty = false;
            if (dt.Year <= 1900)
            {
                isEmpty = true;
            }

            return isEmpty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsEmptyDate(System.DateTime dt)
        {
            bool isEmpty = false;
            if (dt.Year <= 1900)
            {
                isEmpty = true;
            }

            return isEmpty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dblLBQuantity"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double ConvertLB2KG(double lbQuantity)
        {
            return (lbQuantity * 0.4535923703);
        }

        /// <summary>
        /// Converts LB to KG.
        /// </summary>
        /// <param name="lbQuantity"></param>
        /// <returns></returns>
        public static decimal ConvertLB2KG(decimal lbQuantity)
        {
            return lbQuantity * 0.4535923703M;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dblKGQuantity"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double ConvertKG2LB(double kgQuantity)
        {
            return (kgQuantity * 2.20462262);
        }

        /// <summary>
        /// Converts KG to LB.
        /// </summary>
        /// <param name="kgQuantity"></param>
        /// <returns></returns>
        public static decimal ConvertKG2LB(decimal kgQuantity)
        {
            return kgQuantity * 2.20462262M;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objList"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static XmlDocument GetXMLDocumentFromObjectArray(Array objList)
        {
            XmlDocument result = new XmlDocument();
            XmlElement root = result.CreateElement("List");

            result.AppendChild(root);

            foreach (object list in objList)
            {
                XmlElement record = result.CreateElement("Record");
                root.AppendChild(record);
                XmlElement tmpElem = null;

                // Interogate properties
                foreach (PropertyInfo prop in list.GetType().GetProperties())
                {
                    if (prop.CanRead)
                    {
                        tmpElem = result.CreateElement(prop.GetGetMethod().Name.Substring(4, prop.GetGetMethod().Name.Length - 4));
                        tmpElem.AppendChild(result.CreateTextNode(prop.GetValue(list, null).ToString()));
                        record.AppendChild(tmpElem);
                    }
                }

                // Interogate fields
                foreach (FieldInfo fld in list.GetType().GetFields())
                {
                    if (fld.IsPublic)
                    {
                        tmpElem = result.CreateElement(fld.Name);
                        tmpElem.AppendChild(result.CreateTextNode(fld.GetValue(list).ToString()));
                        record.AppendChild(tmpElem);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetAssemblyVersion()
        {
            int major = Assembly.GetExecutingAssembly().GetName().Version.Major;
            int minor = Assembly.GetExecutingAssembly().GetName().Version.Minor;
            int build = Assembly.GetExecutingAssembly().GetName().Version.Build;
            int revision = Assembly.GetExecutingAssembly().GetName().Version.Revision;

            return string.Format("{0}.{1}.{2}.{3}", major, minor, build, revision);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFullClassName"></param>
        /// <param name="strReportingMethod"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool ErrorReportAndRequestContinue(string fullClassName, string reportingMethod,
            Exception ex)
        {
            // Determine Class and Assembly
            string[] aClass = fullClassName.Split('.');
            string assembly = aClass[0];

            //Dim strClassName As New System.Text.StringBuilder
            //For Each s As String In aClass
            //  If Not (s = aClass(0)) Then strClassName.Append(s & ".")
            //Next
            //strClassName.Remove(strClassName.Length - 1, 1)

            //Dim strAppDir As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)
            //Dim strFullPathToMyFile As String = Path.Combine(strAppDir, "dMobileResource02.wav")
            //Dim oSnd As SysSound = New SysSound(strFullPathToMyFile)
            //oSnd.Play()

            bool result = false;
//#if MULTILANGUAGE
//      string errorMsg = Translator.GetInstance().Translate(TranslatorVocabulary.AnUnanticipatedErrorOccuredInDmobile) + Constants.vbCrLf +
//#else
//            string errorMsg = "An Unanticipated Error Occured in dMobile: " + Constants.vbCrLf +
//#endif
// "Assembly: " + assembly + Constants.vbCrLf + "Class: " + fullClassName +
//                Constants.vbCrLf + "Method: " + reportingMethod + Constants.vbCrLf + "Type: " +
//                ex.GetType().ToString() + Constants.vbCrLf + "Message: " + ex.Message.ToString() +
//#if MULTILANGUAGE
//          Constants.vbCrLf + Translator.GetInstance().Translate(TranslatorVocabulary.DoYouWishToContinue);
//#else
// Constants.vbCrLf + "DO YOU WISH TO CONTINUE?";
//#endif

//            result = !(Interaction.MsgBox(errorMsg, MsgBoxStyle.YesNo,
//#if MULTILANGUAGE
//          Translator.GetInstance().Translate(TranslatorVocabulary.DmobileErrorOccurredDoYouWishToContinue)) == MsgBoxResult.No);
//#else
// "dMobile Error Occurred Do you wish to continue!") == MsgBoxResult.No);
//#endif

            return result;
        }

        /// <summary>
        /// Removes control characters and other non-UTF-8 characters
        /// </summary>
        /// <param name="inData">The string to process</param>
        /// <returns>A string with no control characters or entities above 0x00FD</returns>
        public static string RemoveTroublesomeCharacters(string inData)
        {
            if (string.IsNullOrEmpty(inData))
            {
                return string.Empty;
            }

            StringBuilder newData = new StringBuilder();
            //char ch = '\0';
            //for (int index = 0; index <= inData.Length - 1; index++)
            //{
            //    ch = inData[index];

            //    // remove any characters outside the valid UTF-8 range as well as all control characters
            //    // except tabs and new lines
            //    ////if ((&HFD > AscW(ch) And AscW(ch) > &H1F) Or ch = "\t" Or ch = "\n" Or ch = "\r")
            //    ////newString.Append(ch)

            //    if (0xfd > Strings.AscW(ch) & Strings.AscW(ch) > 0x1f)
            //    {
            //        newData.Append(ch);
            //    }
            //}

            return newData.ToString();
        }

        /// <summary>
        /// Word wraps the given text to fit within the specified width.
        /// </summary>
        /// <param name="text">Text to be word wrapped</param>
        /// <param name="width">Width, in characters, to which the text
        /// should be word wrapped</param>
        /// <returns>Array list of strings with the modified text</returns>
        public static List<string> WordWrap2List(string text, int width)
        {
            List<string> sb = new List<string>();

            if (width > 0)
            {
                int pos = 0;
                int next = 0;

                // Parse each line of text
                for (pos = 0; pos < text.Length; pos = next)
                {
                    // Find end of line
                    int eol = text.IndexOf(Environment.NewLine, pos);
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
                            int len = eol - pos;
                            if (len > width)
                            {
                                len = BreakLine(text, pos, width);
                            }

                            sb.Add(text.Substring(pos, len));

                            // Trim whitespace following break
                            pos += len;
                            while (pos < eol && Char.IsWhiteSpace(text[pos]))
                            {
                                pos++;
                            }
                        }
                        while (eol > pos);
                    }
                }
            }

            return sb;
        }

        /// <summary>
        /// Word wraps the given text to fit within the specified width.
        /// </summary>
        /// <param name="text">Text to be word wrapped</param>
        /// <param name="width">Width, in characters, to which the text
        /// should be word wrapped</param>
        /// <returns>The modified text</returns>
        public static string WordWrap2String(string text, int width)
        {
            ////if (width > text.Length)
            ////{
            ////    throw new Exception("width > text.Length");
            ////}

            StringBuilder sb = new StringBuilder();

            if (width > 0)
            {
                int pos = 0;
                int next = 0;

                // Parse each line of text
                for (pos = 0; pos < text.Length; pos = next)
                {
                    // Find end of line
                    int eol = text.IndexOf(Environment.NewLine, pos);
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
                            int len = eol - pos;
                            if (len > width)
                            {
                                len = BreakLine(text, pos, width);
                            }

                            sb.Append(text, pos, len);
                            sb.Append(Environment.NewLine);

                            // Trim whitespace following break
                            pos += len;
                            while (pos < eol && Char.IsWhiteSpace(text[pos]))
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

        /// <summary>
        /// Locates position to break the given line so as to avoid breaking words.
        /// </summary>
        /// <param name="text">String that contains line of text</param>
        /// <param name="pos">Index where line of text starts</param>
        /// <param name="max">Maximum line length</param>
        /// <returns>The modified line length</returns>
        private static int BreakLine(string text, int pos, int max)
        {
            // Find last whitespace in line
            int i = max;

            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
            {
                i--;
            }

            // If no whitespace found, break at maximum length
            if (i < 0)
            {
                return max;
            }

            // Find start of whitespace
            while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
            {
                i--;
            }

            // Return length of text before whitespace
            return i + 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static XmlReader GetXmlReader(ref string xml)
        {
            // Parse the Data into Schema and Data
            string endSchema = "</xsd:schema>";
            int eoSchemaEnd = xml.IndexOf(endSchema);
            if (eoSchemaEnd >= 0)
            {
                int length = xml.Length;
                int actualSchemaEnd = eoSchemaEnd + endSchema.Length;
                string schema = xml.Substring(0, actualSchemaEnd);

                int dataLength = xml.Length - (actualSchemaEnd);
                string data = xml.Substring(actualSchemaEnd, dataLength);

                // Create the XmlSchemaSet class.
                XmlSchema xmlschema = new XmlSchema();
                xmlschema = XmlSchema.Read(new StringReader(schema), null);

                XmlSchemaSet sc = new XmlSchemaSet();
                sc.Add(xmlschema);

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.None;
                settings.Schemas = sc;

                StringReader sr = new StringReader(data);
                XmlReader xmlText = XmlReader.Create(sr);

                return xmlText;
            }
            else
            {
                return null;
            }
        }

        #endregion

        /// <summary>
        /// Structure to hold label quantites
        /// </summary>
        /// <remarks>Used in requesting how many shipping labels to print</remarks>
        public struct SLabelQuantity
        {
            #region Fields

            private int fullQuantity;
            private int repackQuantity;
            private int dryQuantity;
            private int frozenQuantity;
            private int liveQuantity;
            private int freshQuantity;
            private int palletQuantity;
            private int bundleQuantity;
            private int repackDryQuantity;
            private int repackFrozenQuantity;
            private int repackLiveQuantity;
            private int repackFreshQuantity;

            /// <summary>
            /// Used as flag to validate if a 'full' repack quantity is required.
            /// </summary>
            private bool isRepack;

            /// <summary>
            /// Used as flag to validate if a 'dry' repack quantity is required.
            /// </summary>
            private bool isRepackDry;

            /// <summary>
            /// Used as flag to validate if a 'frozen' repack quantity is required.
            /// </summary>
            private bool isRepackFrozen;

            /// <summary>
            /// Used as flag to validate if a 'live' repack quantity is required.
            /// </summary>
            private bool isRepackLive;

            /// <summary>
            /// Used as flag to validate if a 'fresh' repack quantity is required.
            /// </summary>
            private bool isRepackFresh;

            private bool isPrinted;

            #endregion

            #region Properties

            public bool IsRepack
            {
                get
                {
                    return this.isRepack;
                }

                set
                {
                    this.isRepack = value;
                }
            }

            public bool IsRepackLive
            {
                get
                {
                    return this.isRepackLive;
                }

                set
                {
                    this.isRepackLive = value;
                }
            }

            public bool IsRepackFresh
            {
                get
                {
                    return this.isRepackFresh;
                }

                set
                {
                    this.isRepackFresh = value;
                }
            }

            public bool IsRepackFrozen
            {
                get
                {
                    return this.isRepackFrozen;
                }

                set
                {
                    this.isRepackFrozen = value;
                }
            }

            public bool IsRepackDry
            {
                get
                {
                    return this.isRepackDry;
                }

                set
                {
                    this.isRepackDry = value;
                }
            }

            public int BundleQuantity
            {
                get
                {
                    return this.bundleQuantity;
                }

                set
                {
                    this.bundleQuantity = value;
                }
            }

            public int RepackFreshQuantity
            {
                get
                {
                    return this.repackFreshQuantity;
                }

                set
                {
                    this.repackFreshQuantity = value;
                }
            }

            public int RepackLiveQuantity
            {
                get
                {
                    return this.repackLiveQuantity;
                }

                set
                {
                    this.repackLiveQuantity = value;
                }
            }

            public int RepackFrozenQuantity
            {
                get
                {
                    return this.repackFrozenQuantity;
                }

                set
                {
                    this.repackFrozenQuantity = value;
                }
            }

            public int RepackDryQuantity
            {
                get
                {
                    return this.repackDryQuantity;
                }

                set
                {
                    this.repackDryQuantity = value;
                }
            }

            public int PalletQuantity
            {
                get
                {
                    return this.palletQuantity;
                }

                set
                {
                    this.palletQuantity = value;
                }
            }

            public int FreshQuantity
            {
                get
                {
                    return this.freshQuantity;
                }

                set
                {
                    this.freshQuantity = value;
                }
            }

            public int LiveQuantity
            {
                get
                {
                    return this.liveQuantity;
                }

                set
                {
                    this.liveQuantity = value;
                }
            }

            public int FrozenQuantity
            {
                get
                {
                    return this.frozenQuantity;
                }

                set
                {
                    this.frozenQuantity = value;
                }
            }

            public int DryQuantity
            {
                get
                {
                    return this.dryQuantity;
                }

                set
                {
                    this.dryQuantity = value;
                }
            }

            public int FullQuantity
            {
                get
                {
                    return this.fullQuantity;
                }

                set
                {
                    this.fullQuantity = value;
                }
            }

            public int RepackQuantity
            {
                get
                {
                    return this.repackQuantity;
                }

                set
                {
                    this.repackQuantity = value;
                }
            }

            public bool IsPrinted
            {
                get
                {
                    return this.isPrinted;
                }

                set
                {
                    this.isPrinted = value;
                }
            }

            #endregion
        }

        /// <summary>
        /// Structure to hold quantity
        /// </summary>
        /// <remarks>Used in FillQuantityStructureFromQuantity</remarks>
        public struct SFillQuantity
        {
            private decimal quantity;
            private decimal baseQuantity;
            private decimal weightQuantity;

            public decimal Quantity
            {
                get { return this.quantity; }
                set { this.quantity = value; }
            }

            public decimal BaseQuantity
            {
                get { return this.baseQuantity; }
                set { this.baseQuantity = value; }
            }

            public decimal WeightQuantity
            {
                get { return this.weightQuantity; }
                set { this.weightQuantity = value; }
            }
        }

        #region "Device Memory"

        public struct MEMORYSTATUS
        {
            public UInt32 dwLength;
            public UInt32 dwMemoryLoad;
            public UInt32 dwTotalPhys;
            public UInt32 dwAvailPhys;
            public UInt32 dwTotalPageFile;
            public UInt32 dwAvailPageFile;
            public UInt32 dwTotalVirtual;
            public UInt32 dwAvailVirtual;
        }

        [DllImport("coredll")]
        public static extern int GlobalMemoryStatus(ref MEMORYSTATUS ms);

        [DllImport("coredll")]
        public static extern int GetSystemMemoryDivision(ref UInt32 lpdwStorePages,
            ref UInt32 ldpwRamPages, ref UInt32 ldpwPageSize);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public static string ShowMemory()
        {
            StringBuilder memoryInfoString = new StringBuilder();

            memoryInfoString.Append("Total Bytes Allocated " + Environment.NewLine + GC.GetTotalMemory(true) + " Bytes" + Environment.NewLine);
            memoryInfoString.Append(Environment.NewLine + "Used Heap Memory " + Environment.NewLine + (Java.Lang.Runtime.GetRuntime().TotalMemory() - Java.Lang.Runtime.GetRuntime().FreeMemory()) + " Bytes" +  Environment.NewLine);
            memoryInfoString.Append(Environment.NewLine + "Free Heap Memory " + Environment.NewLine + Java.Lang.Runtime.GetRuntime().FreeMemory() + " Bytes" + Environment.NewLine);
            memoryInfoString.Append(Environment.NewLine + "Total Heap Memory " + Environment.NewLine + Java.Lang.Runtime.GetRuntime().TotalMemory() + " Bytes" + Environment.NewLine);
            memoryInfoString.Append(Environment.NewLine + "Max Heap Memory " + Environment.NewLine + Java.Lang.Runtime.GetRuntime().MaxMemory() + " Bytes" + Environment.NewLine);

            return memoryInfoString.ToString();
        }

        #endregion

        #region "Reset Device"

        private const int FILE_DEVICE_HAL = 0x101;
        private const int METHOD_BUFFERED = 0;
        private const int FILE_ANY_ACCESS = 0;

        private int CTL_CODE(int DeviceType, int Func, int Method, int Access)
        {
            return (DeviceType << 16) | (Access << 14) | (Func << 2) | Method;
        }

        [DllImport("coredll")]
        public static extern int KernelIoControl(int dwIoControlCode, IntPtr lpInBuf, int nInBufSize,
            IntPtr lpOutBuf, int nOutBufSize, ref int lpBytesReturned);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private int ResetDevice()
        {
            int bytesReturned = 0;
            int IOCTL_HAL_REBOOT = CTL_CODE(FILE_DEVICE_HAL, 15, METHOD_BUFFERED, FILE_ANY_ACCESS);

            return KernelIoControl(IOCTL_HAL_REBOOT, IntPtr.Zero, 0, IntPtr.Zero, 0, ref bytesReturned);
        }

        #endregion

        #region "SelectValueNumericUpDown"

        const int EM_SETSEL = 0xb1;
        const int WM_USER = 0x400;
        const int UDM_GETBUDDY = (WM_USER + 106);
        const int GW_CHILD = 5;
        [DllImport("coredll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("coredll")]
        public static extern IntPtr GetCapture();
        [DllImport("coredll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, int nIndex);
        [DllImport("coredll")]
        public static extern void SetFocus(IntPtr hWnd);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctl"></param>
        /// <remarks></remarks>
        //public static void SetFocusAndSelectValueToNumericUpDown(UpDownBase ctl)
        //{
        //    ctl.Capture = true;
        //    IntPtr hWnd = GetCapture();
        //    ctl.Capture = false;
        //    IntPtr hBuddy = GetWindow(hWnd, GW_CHILD);
        //    SetFocus(hBuddy);
        //    SendMessage(hBuddy, EM_SETSEL, 0, -1);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctl"></param>
        /// <remarks></remarks>
        //public static void SetFocusAndSelectValueToDomainUpDown(UpDownBase ctl)
        //{
        //    ctl.Capture = true;
        //    IntPtr hWnd = GetCapture();
        //    ctl.Capture = false;
        //    IntPtr hBuddy = GetWindow(hWnd, GW_CHILD);
        //    SetFocus(hBuddy);
        //    SendMessage(hBuddy, EM_SETSEL, 0, -1);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctl"></param>
        /// <remarks></remarks>
        //public static void SelectValueInNumericUpDown(UpDownBase ctl)
        //{
        //    ctl.Capture = true;
        //    IntPtr hWnd = GetCapture();
        //    ctl.Capture = false;
        //    IntPtr hBuddy = GetWindow(hWnd, GW_CHILD);
        //    ////SetFocus(hBuddy)
        //    SendMessage(hBuddy, EM_SETSEL, 0, -1);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctl"></param>
        /// <remarks></remarks>
        //public static void SelectValueInDomainUpDown(UpDownBase ctl)
        //{
        //    ctl.Capture = true;
        //    IntPtr hWnd = GetCapture();
        //    ctl.Capture = false;
        //    IntPtr hBuddy = GetWindow(hWnd, GW_CHILD);
        //    ////SetFocus(hBuddy)
        //    SendMessage(hBuddy, EM_SETSEL, 0, -1);
        //}

        #endregion
    }
}
