namespace dMobile.Bedrock
{
    using System;
    using System.Data;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Microsoft.VisualBasic;
    using System.Collections.Generic;

    [Serializable]
    public class SystemSettings : ISysSettingsEventPublisher
    {
        #region System Fields

        private Theme theme = Theme.Light;
        private bool isDisconnectedMode = false;
        private DateTime lastRefreshData = DateTime.Now;
        private bool isInvStrict = false;
        private bool isSetCatchWeightZero = false;
        private bool isSetCatchCountZero = false;
        private bool isIntegerCheckOverride = false;
        private bool isDisconnectedModeLoadDetail = false;
        private bool isEnableDefaultDetailValues = false;
        private bool isDetailEditScreenSetFocus2CatchWeight = false;
        private bool isEnableDefaultDetailExtraValues = false;
        private bool isPrintProductLabelsAsBatch = false;
        private int detailDateMinMaxValidYearRange = 0;
        private decimal prodWeightUnitTolerancePercent = 0;
        private bool doesSelectingADifferentProductClearSkid = false;
        private bool isProductLabelCatchAsk = false;
        private string defaultLanguage = "ENU";
        private List<string> languagePaths = new List<string>();
        private bool activityActionsVisible = false;
        private bool activityItemActionsVisible = false;
        private bool activityDetailActionsVisible = false;

        /// <summary>
        /// When true, inventory inquiry will search the whole warehouse instead
        /// of specific section or slot.
        /// </summary>
        private bool inventoryInquirySearchEntireWarehouse = false;

        #endregion

        #region BarCode Fields

        private string gtinCompanyPrefix = string.Empty;
        private bool isProductUnitBarCodeModifier1 = false;
        private string productUnitBarCodeModifier1Codes = string.Empty;

        #endregion Fields

        #region Location Map Fields

        //private string locMapWarehouseMask = string.Empty;
        //private string locMapSectionMask = string.Empty;
        //private string locMapSlotMask = string.Empty;
        //private string locMapSlotCoord1Mask = string.Empty;
        //private string locMapSlotCoord2Mask = string.Empty;
        //private string locMapSlotCoord3Mask = string.Empty;
        //private string locMapSlotCoord1Name = string.Empty;
        //private string locMapSlotCoord2Name = string.Empty;
        //private string locMapSlotCoord3Name = string.Empty;
        private bool isDefaultLocToProductDefaultLoc = false;
        private bool isVerifyProductLocation = false;

        #endregion Fields

        #region Receiving Fields

        private bool isPutAwaySkidNoCatchAskZeroEnabled = false;
        private bool isReceivingNoCatchAskZeroEnabled = false;
        private bool isPOWorkSheetNoCatchAskZeroEnabled = false;
        private bool isReceivingModule = false;
        private bool allowAddProduct2Receiving = false;
        private bool canPrintProductLabelReceiving = false;
        private bool isReceivingExtraDetailEnabled = false;
        private bool isReceivingCatchAskEnabled = false;
        private bool isPurchaseOrderWorkSheetModule = false;
        private bool isPOWorkSheetCatchAskEnabled = false;
        private bool isReceivingCreateNewItemNoAsk = false;
        private bool isPOWorkSheetCreateNewItemNoAsk = false;
        private bool askRecvPrintAllSkidLabelsUponSave = false;

        /// <summary>
        /// Receiving quantity over warning level.
        /// </summary>
        private SysCommon.ActReceivingQtyOverValLevels recvQtyOverWarn;

        /// <summary>
        /// Receiving quantity ounder warning level.
        /// </summary>
        private SysCommon.ActReceivingQtyUnderValLevels recvQtyUnderWarn;

        #endregion

        #region Picking Fields

        private bool isPickingSingleModuleNoCatchAskZeroEnabled = false;
        private bool isPickingNoCatchAskZeroEnabled = false;

        /// <summary>
        /// Is picking module enabled.
        /// </summary>
        private bool isPickMod = false;

        /// <summary>
        /// Is release by order.
        /// </summary>
        private bool isRelByOrder = false;

        /// <summary>
        /// Is release by section.
        /// </summary>
        private bool isRelBySect = false;

        /// <summary>
        /// Allow adding new product to order.
        /// </summary>
        private bool allowAddProd2Pick = false;

        /// <summary>
        /// Is preselection picking criteria.
        /// </summary>
        private bool isPrePickCrit = false;

        /// <summary>
        /// Is picking disallow manual add new product.
        /// </summary>
        private bool isPickDisallowManualAddProd = false;

        /// <summary>
        /// Is picking associate bar code password required.
        /// </summary>
        private bool isPickAssocBarCodePwdReq = false;

        /// <summary>
        /// Picking quantity over warning level.
        /// </summary>
        private SysCommon.ActPickingQtyOverValLevels pickQtyOverWarn;

        /// <summary>
        /// Picking quantity ounder warning level.
        /// </summary>
        private SysCommon.ActPickingQtyUnderValLevels pickQtyUnderWarn;

        /// <summary>
        /// Is picking quantity under show base quantity available.
        /// </summary>
        private bool isPickQtyUnderShowBaseQtyAvail = false;

        /// <summary>
        /// Is picking refresh orders after release.
        /// </summary>
        private bool isPickRefreshOrdAfterRel = false;

        /// <summary>
        /// Is picking show item quantity available.
        /// </summary>
        private bool isPickShowItemQtyAvail = false;

        /// <summary>
        /// Is Picking cofirm detail mode.
        /// </summary>
        private bool isPickConfirmDet = false;

        /// <summary>
        /// Is picking popup comments.
        /// </summary>
        private bool isPickPopupCmnt = false;

        /// <summary>
        /// Is picking must review items before release.
        /// </summary>
        private bool isPickMustReviewItemsBeforeRel = false;

        /// <summary>
        /// Is picking confirm exit enabled.
        /// </summary>
        private bool isPickConfirmExit = false;

        /// <summary>
        /// Is picking override message 1 enabled.
        /// </summary>
        private bool isPickOverrideMsg1 = false;

        /// <summary>
        /// Is picking hide route colums enabled.
        /// </summary>
        private bool isPickHideRouteCol = false;

        /// <summary>
        /// Is picking extra detail enabled.
        /// </summary>
        private bool isPickExtraDet = false;

        /// <summary>
        /// Is picking catch as enabled.
        /// </summary>
        private bool isPickCatchAsk = false;

        /// <summary>
        /// Picking detail grid design layout.
        /// </summary>
        private int pickDetGridDesignLayout = 0;

        /// <summary>
        /// Is picking create new item no ask.
        /// </summary>
        private bool isPickCreateNewItemNoAsk = false;

        /// <summary>
        /// Picking item description option.
        /// </summary>
        private int pickItemDescOpt = 0;

        /// <summary>
        /// Disable creation of a skid during picking.
        /// </summary>
        private bool isPickDisableMakeSkid = false;

        /// <summary>
        /// Require password for adding manual detail quantity.
        /// </summary>
        private bool isPickAddManualDetailPasswordRequired = false;

        /// <summary>
        /// 
        /// </summary>
        private bool isPickingModuleDisallowLoadingAllOrders = false;

        #region Picking Single

        /// <summary>
        /// Is picking single catch ask enabled.
        /// </summary>
        private bool isPickSingleCatchAsk = false;

        /// <summary>
        /// Is picking single product.
        /// </summary>
        private bool isPickSingleProd = false;

        /// <summary>
        /// Is picking single scanned product must match selected product.
        /// </summary>
        private bool isPickSingleScanProdMustMatchSelProd = false;

        private bool isPickSinglePrintShipLabelFullQuantity = false;

        private int pickSingleShipLabelStopThresholdQuantity = 0;

        #endregion

        #region Picking Other Info

        private bool canPrintShippingLabel = false;
        private bool isFullQuantityEnabled = false;
        private bool isRepackQuantityEnabled = false;
        private bool isDryQuantityEnabled = false;
        private bool isDryRepackQuantityEnabled = false;
        private bool isFreshQuantityEnabled = false;
        private bool isFreshRepackQuantityEnabled = false;
        private bool isFrozenQuantityEnabled = false;
        private bool isFrozenRepackQuantityEnabled = false;
        private bool isLiveQuantityEnabled = false;
        private bool isLiveRepackQuantityEnabled = false;
        private bool isPalletQuantityEnabled = false;
        private bool isBundleQuantityEnabled = false;

        #endregion

        #region Returns

        private bool isReturnsNoCatchAskZeroEnabled = false;

        /// <summary>
        /// Is returns module enabled.
        /// </summary>
        private bool isRetMod = false;

        /// <summary>
        /// Allow adding new products to returns.
        /// </summary>
        private bool allowAddProduct2Returns = false;

        /// <summary>
        /// Is returns confirm detail.
        /// </summary>
        private bool isRetConfirmDet = false;

        /// <summary>
        /// Is returns extra detail enabled.
        /// </summary>
        private bool isRetExtraDetEnabled = false;

        /// <summary>
        /// Is returns catch ask enabled.
        /// </summary>
        private bool isRetCatchAskEnabled = false;

        /// <summary>
        /// 
        /// </summary>
        private bool isRetCreateNewItemNoAsk = false;

        #endregion

        private bool isReplenishmentModule = false;

        #endregion

        #region Physical Fields

        private bool isPhysicalCountNoCatchAskZeroEnabled = false;
        private bool isPhysicalCountModule = false;
        private bool allowAddProduct2PhysicalCounts = false;
        private bool isPhysicalExtraDetailEnabled = false;
        private bool isPhysicalCatchAskEnabled = false;
        private bool isPhysicalCreateNewItemNoAsk = false;
        private bool isPhysicalDoNotDefaultProductLocation = false;

        #endregion

        #region Cycle Fields

        private bool isCycleCountNoCatchAskZeroEnabled = false;
        private bool isCycleCountModule = false;
        private bool allowAddProduct2CycleCounts = false;
        private bool isCycleCountExtraDetailEnabled = false;
        private bool isCycleCatchAskEnabled = false;
        private bool isCycleCreateNewItemNoAsk = false;
        private bool isCycleDoNotDefaultProductLocation = false;

        #endregion

        #region Transfers Fields

        private bool isTransferNoCatchAskZeroEnabled = false;
        private bool canPrintProductLabelTransfers = false;

        /// <summary>
        /// Is transfers module enabled.
        /// </summary>
        private bool isTransMod = false;

        /// <summary>
        /// Allowed adding new products to transfers.
        /// </summary>
        private bool allowAddProd2Trans = false;

        /// <summary>
        /// Is transfer extra detail enabled.
        /// </summary>
        private bool isTransExtraDetEnabled = false;

        /// <summary>
        /// Is transfers catch ask enabled.
        /// </summary>
        private bool isTransCatchAskEnabled = false;

        /// <summary>
        /// Is transfers create new item no ask.
        /// </summary>
        private bool isTransCreateNewItemNoAsk = false;

        /// <summary>
        /// Is transfers confirm from detail mode.
        /// </summary>
        private bool isTransConfirmFromDet = false;

        /// <summary>
        /// Is transfers confirm to detail mode.
        /// </summary>
        ////private bool isTransConfirmToDet = false;

        /// <summary>
        /// Transfers quantity over warning level.
        /// </summary>
        private SysCommon.ActTransfersQtyOverValLevels transQtyOverWarn;

        /// <summary>
        /// Transfers quantity ounder warning level.
        /// </summary>
        private SysCommon.ActTransfersQtyUnderValLevels transQtyUnderWarn;

        #endregion

        #region Convert Fields

        private bool isConvertUnitModuleNoCatchAskZeroEnabled = false;
        private bool isConvertUnitsModule = false;
        private bool isConvertUnitOnlyShowSell = false;
        private bool canPrintProductLabelConvertUnits = false;
        private bool canPrintProductLabelConvertProduct = false;
        private bool isConvertUnitCatchAskEnabled = false;

        #endregion

        #region Moves Fields

        private bool isMoveInventoryModuleNoCatchAskZeroEnabled = false;
        private bool isMovesModule = false;
        private bool isMoveInventoryCatchAskEnabled = false;

        #endregion

        #region PutAway Fields

        private bool isPutAwayModule = false;
        private bool isPutAwaySkidCatchAskEnabled = false;

        #endregion

        #region Skid Fields

        private bool isAddToSkidModule = false;
        private bool isRemoveFromSkidModule = false;
        private bool isBreakSkidModule = false;
        private bool isCreateSkidModule = false;
        private bool isSkidTrackingModule = false;
        private bool isMoveSkidModule = false;

        #endregion

        #region Adjustments Fields

        private bool isAdjustmentNoCatchAskZeroEnabled = false;
        private bool isInventoryAdjustmentModule = false;
        private bool allowAddProduct2Adjustments = false;
        private bool isEnabledAdjustmentTypeFresh2Frozen = false;
        private bool isAdjustmentsExtraDetailEnabled = false;
        private bool isAdjustmentModuleCatchAskEnabled = false;
        private bool isAdjustmentCreateNewItemNoAsk = false;

        #endregion

        #region Production Fields

        private bool isFinishedExtraDetailEnabled = false;
        private bool isMaterialExtraDetailEnabled = false;
        private bool isByProductsExtraDetailEnabled = false;

        #region Production Ticket Fields

        private bool isTicketByProductNoCatchAskZeroEnabled = false;
        private bool isTicketFinishedNoCatchAskZeroEnabled = false;
        private bool isTicketMaterialNoCatchAskZeroEnabled = false;
        private bool isTicketFinishedGoodModule = false;
        private bool isTicketMaterialModule = false;
        private bool isTicketByProductModule = false;
        private bool allowAddProduct2TicketFinishedGood = false;
        private bool allowAddProduct2TicketMaterial = false;
        private bool allowAddProduct2TicketByProduct = false;
        private bool isTicketFinishedCatchAskEnabled = false;
        private bool isTicketMaterialCatchAskEnabled = false;
        private bool isTicketByProductCatchAskEnabled = false;
        private bool isTicketMaterialCreateNewItemNoAsk = false;
        private bool isTicketByProductCreateNewItemNoAsk = false;

        #endregion

        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public SystemSettings()
        {
            SystemSettingsTasks.Init();
        }

        #region System Properties

        /// <summary>
        /// Is inventory checking strict.
        /// </summary>
        public bool IsInvStrict
        {
            get
            {
                return this.isInvStrict;
            }

            set
            {
                this.isInvStrict = value;
            }
        }

        public DateTime LastRefreshData
        {
            get
            {
                return this.lastRefreshData;
            }

            set
            {
                this.lastRefreshData = value;
            }
        }

        public bool IsDetailEditScreenSetFocus2CatchWeight
        {
            get
            {
                return this.isDetailEditScreenSetFocus2CatchWeight;
            }

            set
            {
                this.isDetailEditScreenSetFocus2CatchWeight = value;
            }
        }

        public bool IsPrintProductLabelsAsBatch
        {
            get
            {
                return this.isPrintProductLabelsAsBatch;
            }

            set
            {
                this.isPrintProductLabelsAsBatch = value;
            }
        }


        public bool IsSetCatchWeightZero
        {
            get
            {
                return this.isSetCatchWeightZero;
            }

            set
            {
                this.isSetCatchWeightZero = value;
            }
        }

        public bool IsSetCatchCountZero
        {
            get
            {
                return this.isSetCatchCountZero;
            }

            set
            {
                this.isSetCatchCountZero = value;
            }
        }

        public bool IsIntegerCheckOverride
        {
            get
            {
                return this.isIntegerCheckOverride;
            }

            set
            {
                this.isIntegerCheckOverride = value;
            }
        }

        public bool IsEnableDefaultDetailValues
        {
            get
            {
                return this.isEnableDefaultDetailValues;
            }

            set
            {
                this.isEnableDefaultDetailValues = value;
            }
        }

        public bool IsEnableDefaultDetailExtraValues
        {
            get
            {
                return this.isEnableDefaultDetailExtraValues;
            }

            set
            {
                this.isEnableDefaultDetailExtraValues = value;
            }
        }

        public Theme Theme
        {
            get
            {
                return this.theme;
            }

            set
            {
                this.theme = value;
            }
        }

        public bool IsDisconnectedMode
        {
            get
            {
                return this.isDisconnectedMode;
            }

            set
            {
                this.isDisconnectedMode = value;
            }
        }

        public bool IsDisconnectedModeLoadDetail
        {
            get
            {
                return this.isDisconnectedModeLoadDetail;
            }

            set
            {
                this.isDisconnectedModeLoadDetail = value;
            }
        }

        public int DetailDateMinMaxValidYearRange
        {
            get
            {
                return this.detailDateMinMaxValidYearRange;
            }

            set
            {
                this.detailDateMinMaxValidYearRange = value;
            }
        }

        public decimal ProdWeightUnitTolerancePercent
        {
            get
            {
                return this.prodWeightUnitTolerancePercent;
            }

            set
            {
                this.prodWeightUnitTolerancePercent = value;
            }
        }

        /// <summary>
        /// In Activity Item, when you have a skid made for a line and go to select a new line:
        /// If this is true, it should clear the skid.
        /// Otherwise, keep the skid as is.
        /// </summary>
        public bool DoesSelectingADifferentProductClearSkid
        {
            get
            {
                return this.doesSelectingADifferentProductClearSkid;
            }
            set
            {
                this.doesSelectingADifferentProductClearSkid = value;
            }
        }

        /// <summary>
        /// Forces the Product Label edit screen to always popup.
        /// It used to only show if scheme was missing. This says OR do it every time.
        /// </summary>
        public bool IsProductLabelCatchAsk
        {
            get
            {
                return isProductLabelCatchAsk;
            }
            set
            {
                isProductLabelCatchAsk = value;
            }
        }

        /// <summary>
        /// Gets or sets the system default language.
        /// This is the language shown on startup,
        /// when a user is not logged in, or when a user
        /// does not have a preferred language selected.
        /// </summary>
        public string DefaultLanguage
        {
            get
            {
                return this.defaultLanguage;
            }
            set
            {
                this.defaultLanguage = value;
            }
        }

        /// <summary>
        /// Gets or sets the paths to the language files.
        /// Each file should have a naming convention of dMobileTranslation.CODE.txt
        /// where code is 3 letters from this site.
        /// https://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx
        /// </summary>
        [XmlArrayItem("Path")]
        public List<string> LanguagePaths
        {
            get
            {
                return this.languagePaths;
            }
            set
            {
                this.languagePaths = value;
            }
        }


        /// <summary>
        /// Gets or sets if the Inventory Inquiry screen should search the entire warehouse (true)
        /// or search specific section and slot (false).
        /// </summary>
        public bool InventoryInquirySearchEntireWarehouse
        {
            get
            {
                return this.inventoryInquirySearchEntireWarehouse;
            }

            set
            {
                this.inventoryInquirySearchEntireWarehouse = value;
            }
        }

        #endregion

        #region BarCode Properties

        public string GTINCompanyPrefix
        {
            get
            {
                return this.gtinCompanyPrefix;
            }

            set
            {
                this.gtinCompanyPrefix = value;
            }
        }

        public bool IsProductUnitBarCodeModifier1
        {
            get
            {
                return this.isProductUnitBarCodeModifier1;
            }

            set
            {
                this.isProductUnitBarCodeModifier1 = value;
            }
        }

        public string ProductUnitBarCodeModifier1Codes
        {
            get
            {
                return this.productUnitBarCodeModifier1Codes;
            }

            set
            {
                this.productUnitBarCodeModifier1Codes = value;
            }
        }

        #endregion

        #region Location Map Properties

        //public string LocMapWarehouseMask
        //{
        //    get
        //    {
        //        return this.locMapWarehouseMask;
        //    }

        //    set
        //    {
        //        this.locMapWarehouseMask = value;
        //    }
        //}

        //public string LocMapSectionMask
        //{
        //    get
        //    {
        //        return this.locMapSectionMask;
        //    }

        //    set
        //    {
        //        this.locMapSectionMask = value;
        //    }
        //}

        //public string LocMapSlotMask
        //{
        //    get
        //    {
        //        return this.locMapSlotMask;
        //    }

        //    set
        //    {
        //        this.locMapSlotMask = value;
        //    }
        //}

        //public string LocMapSlotCoord1Mask
        //{
        //    get
        //    {
        //        return this.locMapSlotCoord1Mask;
        //    }

        //    set
        //    {
        //        this.locMapSlotCoord1Mask = value;
        //    }
        //}

        //public string LocMapSlotCoord2Mask
        //{
        //    get
        //    {
        //        return this.locMapSlotCoord2Mask;
        //    }

        //    set
        //    {
        //        this.locMapSlotCoord2Mask = value;
        //    }
        //}

        //public string LocMapSlotCoord3Mask
        //{
        //    get
        //    {
        //        return this.locMapSlotCoord3Mask;
        //    }

        //    set
        //    {
        //        this.locMapSlotCoord3Mask = value;
        //    }
        //}

        //public string LocMapSlotCoord1Name
        //{
        //    get
        //    {
        //        return this.locMapSlotCoord1Name;
        //    }

        //    set
        //    {
        //        this.locMapSlotCoord1Name = value;
        //    }
        //}

        //public string LocMapSlotCoord2Name
        //{
        //    get
        //    {
        //        return this.locMapSlotCoord2Name;
        //    }

        //    set
        //    {
        //        this.locMapSlotCoord2Name = value;
        //    }
        //}

        //public string LocMapSlotCoord3Name
        //{
        //    get
        //    {
        //        return this.locMapSlotCoord3Name;
        //    }

        //    set
        //    {
        //        this.locMapSlotCoord3Name = value;
        //    }
        //}

        public bool IsDefaultLocToProductDefaultLoc
        {
            get
            {
                return this.isDefaultLocToProductDefaultLoc;
            }

            set
            {
                this.isDefaultLocToProductDefaultLoc = value;
            }
        }

        public bool IsVerifyProductLocation
        {
            get
            {
                return this.isVerifyProductLocation;
            }

            set
            {
                this.isVerifyProductLocation = value;
            }
        }

        #endregion

        #region Receiving Properties

        public bool IsPutAwaySkidNoCatchAskZeroEnabled
        {
            get
            {
                return this.isPutAwaySkidNoCatchAskZeroEnabled;
            }

            set
            {
                this.isPutAwaySkidNoCatchAskZeroEnabled = value;
            }
        }

        public bool IsPOWorkSheetNoCatchAskZeroEnabled
        {
            get
            {
                return this.isPOWorkSheetNoCatchAskZeroEnabled;
            }

            set
            {
                this.isPOWorkSheetNoCatchAskZeroEnabled = value;
            }
        }

        public bool IsReceivingNoCatchAskZeroEnabled
        {
            get
            {
                return this.isReceivingNoCatchAskZeroEnabled;
            }

            set
            {
                this.isReceivingNoCatchAskZeroEnabled = value;
            }
        }

        public bool IsReceivingModule
        {
            get
            {
                return this.isReceivingModule;
            }

            set
            {
                this.isReceivingModule = value;
            }
        }

        public bool IsReceivingCatchAskEnabled
        {
            get
            {
                return this.isReceivingCatchAskEnabled;
            }

            set
            {
                this.isReceivingCatchAskEnabled = value;
            }
        }

        public bool IsReceivingExtraDetailEnabled
        {
            get
            {
                return this.isReceivingExtraDetailEnabled;
            }

            set
            {
                this.isReceivingExtraDetailEnabled = value;
            }
        }

        public bool CanPrintProductLabelReceiving
        {
            get
            {
                return this.canPrintProductLabelReceiving;
            }

            set
            {
                this.canPrintProductLabelReceiving = value;
            }
        }

        public bool AllowAddProduct2Receiving
        {
            get
            {
                return this.allowAddProduct2Receiving;
            }

            set
            {
                this.allowAddProduct2Receiving = value;
            }
        }

        public bool IsPOWorkSheetCatchAskEnabled
        {
            get
            {
                return this.isPOWorkSheetCatchAskEnabled;
            }

            set
            {
                this.isPOWorkSheetCatchAskEnabled = value;
            }
        }

        public bool IsPOWorkSheetModule
        {
            get
            {
                return this.isPurchaseOrderWorkSheetModule;
            }

            set
            {
                this.isPurchaseOrderWorkSheetModule = value;
            }
        }

        public bool IsReceivingCreateNewItemNoAsk
        {
            get
            {
                return this.isReceivingCreateNewItemNoAsk;
            }

            set
            {
                this.isReceivingCreateNewItemNoAsk = value;
            }
        }

        public bool IsPOWorkSheetCreateNewItemNoAsk
        {
            get
            {
                return this.isPOWorkSheetCreateNewItemNoAsk;
            }

            set
            {
                this.isPOWorkSheetCreateNewItemNoAsk = value;
            }
        }

        public SysCommon.ActReceivingQtyUnderValLevels RecvQtyUnderWarn
        {
            get
            {
                return this.recvQtyUnderWarn;
            }

            set
            {
                this.recvQtyUnderWarn = value;
            }
        }

        public SysCommon.ActReceivingQtyOverValLevels RecvQtyOverWarn
        {
            get
            {
                return this.recvQtyOverWarn;
            }

            set
            {
                this.recvQtyOverWarn = value;
            }
        }

        public bool AskRecvPrintAllSkidLabelsUponSave
        {
            get
            {
                return this.askRecvPrintAllSkidLabelsUponSave;
            }

            set
            {
                this.askRecvPrintAllSkidLabelsUponSave = value;
            }
        }

        #endregion

        #region Picking Properties

        public bool IsPickingNoCatchAskZeroEnabled
        {
            get
            {
                return this.isPickingNoCatchAskZeroEnabled;
            }

            set
            {
                this.isPickingNoCatchAskZeroEnabled = value;
            }
        }

        public bool IsPickingModuleDisallowLoadingAllOrders
        {
            get
            {
                return this.isPickingModuleDisallowLoadingAllOrders;
            }

            set
            {
                this.isPickingModuleDisallowLoadingAllOrders = value;
            }
        }

        public int PickItemDescOpt
        {
            get
            {
                return this.pickItemDescOpt;
            }

            set
            {
                this.pickItemDescOpt = value;
            }
        }

        public bool IsPickMod
        {
            get
            {
                return this.isPickMod;
            }

            set
            {
                this.isPickMod = value;
            }
        }

        public bool IsPickHideRouteCol
        {
            get
            {
                return this.isPickHideRouteCol;
            }

            set
            {
                this.isPickHideRouteCol = value;
            }
        }

        public bool IsPickOverrideMsg1
        {
            get
            {
                return this.isPickOverrideMsg1;
            }

            set
            {
                this.isPickOverrideMsg1 = value;
            }
        }

        public bool IsPickConfirmExit
        {
            get
            {
                return this.isPickConfirmExit;
            }

            set
            {
                this.isPickConfirmExit = value;
            }
        }

        public bool IsPickCatchAsk
        {
            get
            {
                return this.isPickCatchAsk;
            }

            set
            {
                this.isPickCatchAsk = value;
            }
        }

        public bool IsPickExtraDet
        {
            get
            {
                return this.isPickExtraDet;
            }

            set
            {
                this.isPickExtraDet = value;
            }
        }

        public bool IsPickMustReviewItemsBeforeRel
        {
            get
            {
                return this.isPickMustReviewItemsBeforeRel;
            }

            set
            {
                this.isPickMustReviewItemsBeforeRel = value;
            }
        }

        public bool IsPickPopupCmnt
        {
            get
            {
                return this.isPickPopupCmnt;
            }

            set
            {
                this.isPickPopupCmnt = value;
            }
        }

        public bool IsPickRefreshOrdAfterRel
        {
            get
            {
                return this.isPickRefreshOrdAfterRel;
            }

            set
            {
                this.isPickRefreshOrdAfterRel = value;
            }
        }

        public bool IsPickQtyUnderShowBaseQtyAvail
        {
            get
            {
                return this.isPickQtyUnderShowBaseQtyAvail;
            }

            set
            {
                this.isPickQtyUnderShowBaseQtyAvail = value;
            }
        }

        public bool IsPickDisallowManualAddProd
        {
            get
            {
                return this.isPickDisallowManualAddProd;
            }

            set
            {
                this.isPickDisallowManualAddProd = value;
            }
        }

        public bool IsPickAssocBarCodePwdReq
        {
            get
            {
                return this.isPickAssocBarCodePwdReq;
            }

            set
            {
                this.isPickAssocBarCodePwdReq = value;
            }
        }

        public bool AllowAddProd2Pick
        {
            get
            {
                return this.allowAddProd2Pick;
            }

            set
            {
                this.allowAddProd2Pick = value;
            }
        }

        public bool IsPrePickCrit
        {
            get
            {
                return this.isPrePickCrit;
            }

            set
            {
                this.isPrePickCrit = value;
            }
        }

        public SysCommon.ActPickingQtyUnderValLevels PickQtyUnderWarn
        {
            get
            {
                return this.pickQtyUnderWarn;
            }

            set
            {
                this.pickQtyUnderWarn = value;
            }
        }

        public SysCommon.ActPickingQtyOverValLevels PickQtyOverWarn
        {
            get
            {
                return this.pickQtyOverWarn;
            }

            set
            {
                this.pickQtyOverWarn = value;
            }
        }

        public bool IsPickConfirmDet
        {
            get
            {
                return this.isPickConfirmDet;
            }

            set
            {
                this.isPickConfirmDet = value;
            }
        }

        public bool IsPickShowItemQtyAvail
        {
            get
            {
                return this.isPickShowItemQtyAvail;
            }

            set
            {
                this.isPickShowItemQtyAvail = value;
            }
        }

        public bool IsRelByOrder
        {
            get
            {
                return this.isRelByOrder;
            }

            set
            {
                this.isRelByOrder = value;
            }
        }

        public bool IsRelBySect
        {
            get
            {
                return this.isRelBySect;
            }

            set
            {
                this.isRelBySect = value;
            }
        }

        public int PickDetGridDesignLayout
        {
            get
            {
                return this.pickDetGridDesignLayout;
            }

            set
            {
                this.pickDetGridDesignLayout = value;
            }
        }

        public bool IsPickCreateNewItemNoAsk
        {
            get
            {
                return this.isPickCreateNewItemNoAsk;
            }

            set
            {
                this.isPickCreateNewItemNoAsk = value;
            }
        }

        public bool IsPickDisableMakeSkid
        {
            get
            {
                return this.isPickDisableMakeSkid;
            }

            set
            {
                this.isPickDisableMakeSkid = value;
            }
        }

        public bool IsPickAddManualDetailPasswordRequired
        {
            get
            {
                return this.isPickAddManualDetailPasswordRequired;
            }

            set
            {
                this.isPickAddManualDetailPasswordRequired = value;
            }
        }

        #region Picking Single

        public bool IsPickingSingleModuleNoCatchAskZeroEnabled
        {
            get
            {
                return this.isPickingSingleModuleNoCatchAskZeroEnabled;
            }

            set
            {
                this.isPickingSingleModuleNoCatchAskZeroEnabled = value;
            }
        }

        public bool IsPickSingleProd
        {
            get
            {
                return this.isPickSingleProd;
            }

            set
            {
                this.isPickSingleProd = value;
            }
        }

        public bool IsPickSingleCatchAsk
        {
            get
            {
                return this.isPickSingleCatchAsk;
            }

            set
            {
                this.isPickSingleCatchAsk = value;
            }
        }

        public bool IsPickSingleScanProdMustMatchSelProd
        {
            get
            {
                return this.isPickSingleScanProdMustMatchSelProd;
            }

            set
            {
                this.isPickSingleScanProdMustMatchSelProd = value;
            }
        }

        public bool IsPickSinglePrintShipLabelFullQuantity
        {
            get
            {
                return this.isPickSinglePrintShipLabelFullQuantity;
            }

            set
            {
                this.isPickSinglePrintShipLabelFullQuantity = value;
            }
        }

        public int PickSingleShipLabelStopThresholdQuantity
        {
            get
            {
                return this.pickSingleShipLabelStopThresholdQuantity;
            }

            set
            {
                this.pickSingleShipLabelStopThresholdQuantity = value;
            }
        }

        #endregion Properties

        #region Picking Other Info

        public bool CanPrintShippingLabel
        {
            get
            {
                return this.canPrintShippingLabel;
            }

            set
            {
                this.canPrintShippingLabel = value;
            }
        }

        public bool IsFullQuantityEnabled
        {
            get
            {
                return this.isFullQuantityEnabled;
            }

            set
            {
                this.isFullQuantityEnabled = value;
            }
        }

        public bool IsRepackQuantityEnabled
        {
            get
            {
                return this.isRepackQuantityEnabled;
            }

            set
            {
                this.isRepackQuantityEnabled = value;
            }
        }

        public bool IsDryQuantityEnabled
        {
            get
            {
                return this.isDryQuantityEnabled;
            }

            set
            {
                this.isDryQuantityEnabled = value;
            }
        }

        public bool IsDryRepackQuantityEnabled
        {
            get
            {
                return this.isDryRepackQuantityEnabled;
            }

            set
            {
                this.isDryRepackQuantityEnabled = value;
            }
        }

        public bool IsFreshQuantityEnabled
        {
            get
            {
                return this.isFreshQuantityEnabled;
            }

            set
            {
                this.isFreshQuantityEnabled = value;
            }
        }

        public bool IsFreshRepackQuantityEnabled
        {
            get
            {
                return this.isFreshRepackQuantityEnabled;
            }

            set
            {
                this.isFreshRepackQuantityEnabled = value;
            }
        }

        public bool IsFrozenQuantityEnabled
        {
            get
            {
                return this.isFrozenQuantityEnabled;
            }

            set
            {
                this.isFrozenQuantityEnabled = value;
            }
        }

        public bool IsFrozenRepackQuantityEnabled
        {
            get
            {
                return this.isFrozenRepackQuantityEnabled;
            }

            set
            {
                this.isFrozenRepackQuantityEnabled = value;
            }
        }

        public bool IsLiveQuantityEnabled
        {
            get
            {
                return this.isLiveQuantityEnabled;
            }

            set
            {
                this.isLiveQuantityEnabled = value;
            }
        }

        public bool IsLiveRepackQuantityEnabled
        {
            get
            {
                return this.isLiveRepackQuantityEnabled;
            }

            set
            {
                this.isLiveRepackQuantityEnabled = value;
            }
        }

        public bool IsPalletQuantityEnabled
        {
            get
            {
                return this.isPalletQuantityEnabled;
            }

            set
            {
                this.isPalletQuantityEnabled = value;
            }
        }

        public bool IsBundleQuantityEnabled
        {
            get
            {
                return this.isBundleQuantityEnabled;
            }

            set
            {
                this.isBundleQuantityEnabled = value;
            }
        }

        #endregion Properties

        #endregion

        #region Returns Properties

        public bool IsReturnsNoCatchAskZeroEnabled
        {
            get
            {
                return this.isReturnsNoCatchAskZeroEnabled;
            }

            set
            {
                this.isReturnsNoCatchAskZeroEnabled = value;
            }
        }

        public bool IsRetMod
        {
            get
            {
                return this.isRetMod;
            }

            set
            {
                this.isRetMod = value;
            }
        }

        public bool IsRetExtraDetEnabled
        {
            get
            {
                return this.isRetExtraDetEnabled;
            }

            set
            {
                this.isRetExtraDetEnabled = value;
            }
        }

        public bool IsRetConfirmDet
        {
            get
            {
                return this.isRetConfirmDet;
            }

            set
            {
                this.isRetConfirmDet = value;
            }
        }

        public bool AllowAddProd2Ret
        {
            get
            {
                return this.allowAddProduct2Returns;
            }

            set
            {
                this.allowAddProduct2Returns = value;
            }
        }

        public bool IsRetCatchAskEnabled
        {
            get
            {
                return this.isRetCatchAskEnabled;
            }

            set
            {
                this.isRetCatchAskEnabled = value;
            }
        }

        public bool IsRetCreateNewItemNoAsk
        {
            get
            {
                return this.isRetCreateNewItemNoAsk;
            }

            set
            {
                this.isRetCreateNewItemNoAsk = value;
            }
        }

        #endregion Properties

        #region Physical Properties

        public bool IsPhysicalModule
        {
            get
            {
                return this.isPhysicalCountModule;
            }

            set
            {
                this.isPhysicalCountModule = value;
            }
        }

        public bool IsPhysicalCatchAskEnabled
        {
            get
            {
                return this.isPhysicalCatchAskEnabled;
            }

            set
            {
                this.isPhysicalCatchAskEnabled = value;
            }
        }

        public bool IsPhysicalExtraDetailEnabled
        {
            get
            {
                return this.isPhysicalExtraDetailEnabled;
            }

            set
            {
                this.isPhysicalExtraDetailEnabled = value;
            }
        }

        public bool AllowAddProduct2Physical
        {
            get
            {
                return this.allowAddProduct2PhysicalCounts;
            }

            set
            {
                this.allowAddProduct2PhysicalCounts = value;
            }
        }

        public bool IsPhysicalCreateNewItemNoAsk
        {
            get
            {
                return this.isPhysicalCreateNewItemNoAsk;
            }

            set
            {
                this.isPhysicalCreateNewItemNoAsk = value;
            }
        }

        public bool IsPhysicalDoNotDefaultProductLocation
        {
            get
            {
                return this.isPhysicalDoNotDefaultProductLocation;
            }

            set
            {
                this.isPhysicalDoNotDefaultProductLocation = value;
            }
        }

        #endregion

        #region Cycle Properties

        public bool IsCycleCountNoCatchAskZeroEnabled
        {
            get
            {
                return this.isCycleCountNoCatchAskZeroEnabled;
            }

            set
            {
                this.isCycleCountNoCatchAskZeroEnabled = value;
            }
        }

        public bool IsCycleModule
        {
            get
            {
                return this.isCycleCountModule;
            }

            set
            {
                this.isCycleCountModule = value;
            }
        }

        public bool IsCycleCatchAskEnabled
        {
            get
            {
                return this.isCycleCatchAskEnabled;
            }

            set
            {
                this.isCycleCatchAskEnabled = value;
            }
        }

        public bool IsCycleExtraDetailEnabled
        {
            get
            {
                return this.isCycleCountExtraDetailEnabled;
            }

            set
            {
                this.isCycleCountExtraDetailEnabled = value;
            }
        }

        public bool AllowAddProduct2Cycle
        {
            get
            {
                return this.allowAddProduct2CycleCounts;
            }

            set
            {
                this.allowAddProduct2CycleCounts = value;
            }
        }

        public bool IsCycleCreateNewItemNoAsk
        {
            get
            {
                return this.isCycleCreateNewItemNoAsk;
            }

            set
            {
                this.isCycleCreateNewItemNoAsk = value;
            }
        }

        public bool IsCycleDoNotDefaultProductLocation
        {
            get
            {
                return this.isCycleDoNotDefaultProductLocation;
            }

            set
            {
                this.isCycleDoNotDefaultProductLocation = value;
            }
        }

        #endregion

        #region Production Properties

        public bool IsFinishedExtraDetailEnabled
        {
            get
            {
                return this.isFinishedExtraDetailEnabled;
            }

            set
            {
                this.isFinishedExtraDetailEnabled = value;
            }
        }

        public bool IsIngredientsExtraDetailEnabled
        {
            get
            {
                return this.isMaterialExtraDetailEnabled;
            }

            set
            {
                this.isMaterialExtraDetailEnabled = value;
            }
        }

        public bool IsByProductsExtraDetailEnabled
        {
            get
            {
                return this.isByProductsExtraDetailEnabled;
            }

            set
            {
                this.isByProductsExtraDetailEnabled = value;
            }
        }

        #region Production Ticket Properties

        public bool IsTicketMaterialNoCatchAskZeroEnabled
        {
            get
            {
                return this.isTicketMaterialNoCatchAskZeroEnabled;
            }

            set
            {
                this.isTicketMaterialNoCatchAskZeroEnabled = value;
            }
        }

        public bool IsTicketByProductNoCatchAskZeroEnabled
        {
            get
            {
                return this.isTicketByProductNoCatchAskZeroEnabled;
            }

            set
            {
                this.isTicketByProductNoCatchAskZeroEnabled = value;
            }
        }

        public bool IsTicketFinishedNoCatchAskZeroEnabled
        {
            get
            {
                return this.isTicketFinishedNoCatchAskZeroEnabled;
            }

            set
            {
                this.isTicketFinishedNoCatchAskZeroEnabled = value;
            }
        }

        public bool IsPhysicalCountNoCatchAskZeroEnabled
        {
            get
            {
                return this.isPhysicalCountNoCatchAskZeroEnabled;
            }

            set
            {
                this.isPhysicalCountNoCatchAskZeroEnabled = value;
            }
        }

        public bool IsTicketFinishedGoodModule
        {
            get
            {
                return this.isTicketFinishedGoodModule;
            }

            set
            {
                this.isTicketFinishedGoodModule = value;
            }
        }

        public bool IsTicketMaterialModule
        {
            get
            {
                return this.isTicketMaterialModule;
            }

            set
            {
                this.isTicketMaterialModule = value;
            }
        }

        public bool IsTicketByProductModule
        {
            get
            {
                return this.isTicketByProductModule;
            }

            set
            {
                this.isTicketByProductModule = value;
            }
        }

        public bool IsTicketFinishedCatchAskEnabled
        {
            get
            {
                return this.isTicketFinishedCatchAskEnabled;
            }

            set
            {
                this.isTicketFinishedCatchAskEnabled = value;
            }
        }

        public bool IsTicketMaterialCatchAskEnabled
        {
            get
            {
                return this.isTicketMaterialCatchAskEnabled;
            }

            set
            {
                this.isTicketMaterialCatchAskEnabled = value;
            }
        }

        public bool IsTicketByProductCatchAskEnabled
        {
            get
            {
                return this.isTicketByProductCatchAskEnabled;
            }

            set
            {
                this.isTicketByProductCatchAskEnabled = value;
            }
        }

        public bool AllowAddProduct2TicketFinished
        {
            get
            {
                return this.allowAddProduct2TicketFinishedGood;
            }

            set
            {
                this.allowAddProduct2TicketFinishedGood = value;
            }
        }

        public bool AllowAddProduct2TicketMaterial
        {
            get
            {
                return this.allowAddProduct2TicketMaterial;
            }

            set
            {
                this.allowAddProduct2TicketMaterial = value;
            }
        }

        public bool AllowAddProduct2TicketByProduct
        {
            get
            {
                return this.allowAddProduct2TicketByProduct;
            }

            set
            {
                this.allowAddProduct2TicketByProduct = value;
            }
        }

        public bool IsTicketMaterialCreateNewItemNoAsk
        {
            get
            {
                return this.isTicketMaterialCreateNewItemNoAsk;
            }

            set
            {
                this.isTicketMaterialCreateNewItemNoAsk = value;
            }
        }

        public bool IsTicketByProductCreateNewItemNoAsk
        {
            get
            {
                return this.isTicketByProductCreateNewItemNoAsk;
            }

            set
            {
                this.isTicketByProductCreateNewItemNoAsk = value;
            }
        }

        #endregion

        #endregion

        #region Adjustments Properties

        public bool IsAdjustmentNoCatchAskZeroEnabled
        {
            get
            {
                return this.isAdjustmentNoCatchAskZeroEnabled;
            }

            set
            {
                this.isAdjustmentNoCatchAskZeroEnabled = value;
            }
        }

        public bool IsInventoryAdjustmentModule
        {
            get
            {
                return this.isInventoryAdjustmentModule;
            }

            set
            {
                this.isInventoryAdjustmentModule = value;
            }
        }

        public bool IsAdjustmentCatchAskEnabled
        {
            get
            {
                return this.isAdjustmentModuleCatchAskEnabled;
            }

            set
            {
                this.isAdjustmentModuleCatchAskEnabled = value;
            }
        }

        public bool IsAdjustmentsExtraDetailEnabled
        {
            get
            {
                return this.isAdjustmentsExtraDetailEnabled;
            }

            set
            {
                this.isAdjustmentsExtraDetailEnabled = value;
            }
        }

        public bool IsEnabledAdjustmentTypeFresh2Frozen
        {
            get
            {
                return this.isEnabledAdjustmentTypeFresh2Frozen;
            }

            set
            {
                this.isEnabledAdjustmentTypeFresh2Frozen = value;
            }
        }

        public bool AllowAddProduct2Adjustments
        {
            get
            {
                return this.allowAddProduct2Adjustments;
            }

            set
            {
                this.allowAddProduct2Adjustments = value;
            }
        }

        public bool IsAdjustmentCreateNewItemNoAsk
        {
            get
            {
                return this.isAdjustmentCreateNewItemNoAsk;
            }

            set
            {
                this.isAdjustmentCreateNewItemNoAsk = value;
            }
        }

        #endregion

        #region Transfers Properties

        public bool IsTransferNoCatchAskZeroEnabled
        {
            get
            {
                return this.isTransferNoCatchAskZeroEnabled;
            }

            set
            {
                this.isTransferNoCatchAskZeroEnabled = value;
            }
        }

        public bool CanPrintProductLabelTransfers
        {
            get
            {
                return this.canPrintProductLabelTransfers;
            }
            set
            {
                this.canPrintProductLabelTransfers = value;
            }
        }

        public bool IsTransMod
        {
            get
            {
                return this.isTransMod;
            }

            set
            {
                this.isTransMod = value;
            }
        }

        public bool IsTransCatchAskEnabled
        {
            get
            {
                return this.isTransCatchAskEnabled;
            }

            set
            {
                this.isTransCatchAskEnabled = value;
            }
        }

        public bool IsTransExtraDetEnabled
        {
            get
            {
                return this.isTransExtraDetEnabled;
            }

            set
            {
                this.isTransExtraDetEnabled = value;
            }
        }

        public bool AllowAddProd2Trans
        {
            get
            {
                return this.allowAddProd2Trans;
            }

            set
            {
                this.allowAddProd2Trans = value;
            }
        }

        public bool IsTransCreateNewItemNoAsk
        {
            get
            {
                return this.isTransCreateNewItemNoAsk;
            }

            set
            {
                this.isTransCreateNewItemNoAsk = value;
            }
        }

        public bool IsTransConfirmFromDet
        {
            get
            {
                return this.isTransConfirmFromDet;
            }

            set
            {
                this.isTransConfirmFromDet = value;
            }
        }

        ////public bool IsTransConfirmToDet
        ////{
        ////  get
        ////  {
        ////    return this.isTransConfirmToDet;
        ////  }

        ////  set
        ////  {
        ////    this.IsTransConfirmToDet = value;
        ////  }
        ////}

        public SysCommon.ActTransfersQtyUnderValLevels TransQtyUnderWarn
        {
            get
            {
                return this.transQtyUnderWarn;
            }

            set
            {
                this.transQtyUnderWarn = value;
            }
        }

        public SysCommon.ActTransfersQtyOverValLevels TransQtyOverWarn
        {
            get
            {
                return this.transQtyOverWarn;
            }

            set
            {
                this.transQtyOverWarn = value;
            }
        }

        #endregion

        #region Convert Properties

        public bool IsConvertUnitModuleNoCatchAskZeroEnabled
        {
            get
            {
                return this.isConvertUnitModuleNoCatchAskZeroEnabled;
            }

            set
            {
                this.isConvertUnitModuleNoCatchAskZeroEnabled = value;
            }
        }

        public bool IsConvertUnitsModule
        {
            get
            {
                return this.isConvertUnitsModule;
            }

            set
            {
                this.isConvertUnitsModule = value;
            }
        }

        public bool IsConvertUnitCatchAskEnabled
        {
            get
            {
                return this.isConvertUnitCatchAskEnabled;
            }

            set
            {
                this.isConvertUnitCatchAskEnabled = value;
            }
        }

        public bool IsConvertUnitOnlyShowSell
        {
            get
            {
                return this.isConvertUnitOnlyShowSell;
            }

            set
            {
                this.isConvertUnitOnlyShowSell = value;
            }
        }

        public bool CanPrintProductLabelConvertUnits
        {
            get
            {
                return this.canPrintProductLabelConvertUnits;
            }

            set
            {
                this.canPrintProductLabelConvertUnits = value;
            }
        }


        /// <summary>
        /// <para>Causes a label to print from ActivityDetail screen:</para>
        /// <para>- After scanning with complete scheme data</para>
        /// <para>- After save of scheme data in ActivtyDetailEdit screen</para>
        /// </summary>
        public bool CanPrintProductLabelConvertProduct
        {
            get
            {
                return this.canPrintProductLabelConvertProduct;
            }
            set
            {
                this.canPrintProductLabelConvertProduct = value;
            }
        }

        #endregion

        #region Moves Properties

        public bool IsMoveInventoryModuleNoCatchAskZeroEnabled
        {
            get
            {
                return this.isMoveInventoryModuleNoCatchAskZeroEnabled;
            }

            set
            {
                this.isMoveInventoryModuleNoCatchAskZeroEnabled = value;
            }
        }

        public bool IsMovesModule
        {
            get
            {
                return this.isMovesModule;
            }

            set
            {
                this.isMovesModule = value;
            }
        }

        public bool IsMoveInventoryCatchAskEnabled
        {
            get
            {
                return this.isMoveInventoryCatchAskEnabled;
            }

            set
            {
                this.isMoveInventoryCatchAskEnabled = value;
            }
        }

        #endregion

        #region Skid Properties

        public bool IsSkidTrackingModule
        {
            get
            {
                return this.isSkidTrackingModule;
            }

            set
            {
                this.isSkidTrackingModule = value;
            }
        }

        public bool IsAddToSkidModule
        {
            get
            {
                return this.isAddToSkidModule;
            }

            set
            {
                this.isAddToSkidModule = value;
            }
        }

        public bool IsRemoveFromSkidModule
        {
            get
            {
                return this.isRemoveFromSkidModule;
            }

            set
            {
                this.isRemoveFromSkidModule = value;
            }
        }

        public bool IsBreakSkidModule
        {
            get
            {
                return this.isBreakSkidModule;
            }

            set
            {
                this.isBreakSkidModule = value;
            }
        }

        public bool IsCreateSkidModule
        {
            get
            {
                return this.isCreateSkidModule;
            }

            set
            {
                this.isCreateSkidModule = value;
            }
        }

        public bool IsMoveSkidModule
        {
            get
            {
                return this.isMoveSkidModule;
            }

            set
            {
                this.isMoveSkidModule = value;
            }
        }

        #endregion

        #region PutAway Properties

        public bool IsPutAwayModule
        {
            get
            {
                return this.isPutAwayModule;
            }

            set
            {
                this.isPutAwayModule = value;
            }
        }

        public bool IsPutAwaySkidCatchAskEnabled
        {
            get
            {
                return this.isPutAwaySkidCatchAskEnabled;
            }

            set
            {
                this.isPutAwaySkidCatchAskEnabled = value;
            }
        }

        #endregion

        public bool IsReplenishmentModule
        {
            get
            {
                return this.isReplenishmentModule;
            }

            set
            {
                this.isReplenishmentModule = value;
            }
        }

        public bool ActivityActionsVisible { get => activityActionsVisible; set => activityActionsVisible = value; }
        public bool ActivityItemActionsVisible { get => activityItemActionsVisible; set => activityItemActionsVisible = value; }
        public bool ActivityDetailActionsVisible { get => activityDetailActionsVisible; set => activityDetailActionsVisible = value; }

        #region ISysSettingsEventPublisher Members

        public void RegisterSysSettingsListenerStatusOut(ISysSettingsEventListener sysSettingsListener)
        {
            SystemSettingsTasks.OnSysSettingsStatusOut += new SystemSettingsTasks.OnSysSettingsStatusOutEventHandler(sysSettingsListener.OnSysSettingsStatusOut);
        }

        public void UnregisterSysSettingsListenerStatusOut(ISysSettingsEventListener sysSettingsListener)
        {
            SystemSettingsTasks.OnSysSettingsStatusOut -= new SystemSettingsTasks.OnSysSettingsStatusOutEventHandler(sysSettingsListener.OnSysSettingsStatusOut);
        }

        public void RegisterSysSettingsListenerProgressOut(ISysSettingsEventListener sysSettingsListener)
        {
            SystemSettingsTasks.OnSysSettingsProgressOut += new SystemSettingsTasks.OnSysSettingsProgressOutEventHandler(
                sysSettingsListener.OnSysSettingsProgressOut);
        }

        public void UnregisterSysSettingsListenerProgressOut(ISysSettingsEventListener sysSettingsListener)
        {
            SystemSettingsTasks.OnSysSettingsProgressOut -= new SystemSettingsTasks.OnSysSettingsProgressOutEventHandler(
                sysSettingsListener.OnSysSettingsProgressOut);
        }
        
        #endregion
    }
}
