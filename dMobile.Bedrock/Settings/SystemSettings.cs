// <copyright file="SystemSettings.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the <see cref="SystemSettings" />.
    /// </summary>
    [Serializable]
    public class SystemSettings : ISysSettingsEventPublisher
    {
        /// <summary>
        /// Defines the theme.
        /// </summary>
        private Theme theme = Theme.Light;

        /// <summary>
        /// Defines the isDisconnectedMode.
        /// </summary>
        private bool isDisconnectedMode = false;

        /// <summary>
        /// Defines the lastRefreshData.
        /// </summary>
        private DateTime lastRefreshData = DateTime.Now;

        /// <summary>
        /// Defines the isInvStrict.
        /// </summary>
        private bool isInvStrict = false;

        /// <summary>
        /// Defines the isSetCatchWeightZero.
        /// </summary>
        private bool isSetCatchWeightZero = false;

        /// <summary>
        /// Defines the isSetCatchCountZero.
        /// </summary>
        private bool isSetCatchCountZero = false;

        /// <summary>
        /// Defines the isIntegerCheckOverride.
        /// </summary>
        private bool isIntegerCheckOverride = false;

        /// <summary>
        /// Defines the isDisconnectedModeLoadDetail.
        /// </summary>
        private bool isDisconnectedModeLoadDetail = false;

        /// <summary>
        /// Defines the isEnableDefaultDetailValues.
        /// </summary>
        private bool isEnableDefaultDetailValues = false;

        /// <summary>
        /// Defines the isDetailEditScreenSetFocus2CatchWeight.
        /// </summary>
        private bool isDetailEditScreenSetFocus2CatchWeight = false;

        /// <summary>
        /// Defines the isEnableDefaultDetailExtraValues.
        /// </summary>
        private bool isEnableDefaultDetailExtraValues = false;

        /// <summary>
        /// Defines the isPrintProductLabelsAsBatch.
        /// </summary>
        private bool isPrintProductLabelsAsBatch = false;

        /// <summary>
        /// Defines the detailDateMinMaxValidYearRange.
        /// </summary>
        private int detailDateMinMaxValidYearRange = 0;

        /// <summary>
        /// Defines the prodWeightUnitTolerancePercent.
        /// </summary>
        private decimal prodWeightUnitTolerancePercent = 0;

        /// <summary>
        /// Defines the doesSelectingADifferentProductClearSkid.
        /// </summary>
        private bool doesSelectingADifferentProductClearSkid = false;

        /// <summary>
        /// Defines the isProductLabelCatchAsk.
        /// </summary>
        private bool isProductLabelCatchAsk = false;

        /// <summary>
        /// Defines the defaultLanguage.
        /// </summary>
        private string defaultLanguage = "ENU";

        /// <summary>
        /// Defines the languagePaths.
        /// </summary>
        private List<string> languagePaths = new List<string>();

        /// <summary>
        /// When true, inventory inquiry will search the whole warehouse instead
        /// of specific section or slot..
        /// </summary>
        private bool inventoryInquirySearchEntireWarehouse = false;

        /// <summary>
        /// Defines the gtinCompanyPrefix.
        /// </summary>
        private string gtinCompanyPrefix = string.Empty;

        /// <summary>
        /// Defines the isProductUnitBarCodeModifier1.
        /// </summary>
        private bool isProductUnitBarCodeModifier1 = false;

        /// <summary>
        /// Defines the productUnitBarCodeModifier1Codes.
        /// </summary>
        private string productUnitBarCodeModifier1Codes = string.Empty;

        // private string locMapWarehouseMask = string.Empty;
        // private string locMapSectionMask = string.Empty;
        // private string locMapSlotMask = string.Empty;
        // private string locMapSlotCoord1Mask = string.Empty;
        // private string locMapSlotCoord2Mask = string.Empty;
        // private string locMapSlotCoord3Mask = string.Empty;
        // private string locMapSlotCoord1Name = string.Empty;
        // private string locMapSlotCoord2Name = string.Empty;
        // private string locMapSlotCoord3Name = string.Empty;
        /// <summary>
        /// Defines the isDefaultLocToProductDefaultLoc.
        /// </summary>
        private bool isDefaultLocToProductDefaultLoc = false;

        /// <summary>
        /// Defines the isVerifyProductLocation.
        /// </summary>
        private bool isVerifyProductLocation = false;

        /// <summary>
        /// Defines the isPutAwaySkidNoCatchAskZeroEnabled.
        /// </summary>
        private bool isPutAwaySkidNoCatchAskZeroEnabled = false;

        /// <summary>
        /// Defines the isReceivingNoCatchAskZeroEnabled.
        /// </summary>
        private bool isReceivingNoCatchAskZeroEnabled = false;

        /// <summary>
        /// Defines the isPOWorkSheetNoCatchAskZeroEnabled.
        /// </summary>
        private bool isPOWorkSheetNoCatchAskZeroEnabled = false;

        /// <summary>
        /// Defines the isReceivingModule.
        /// </summary>
        private bool isReceivingModule = false;

        /// <summary>
        /// Defines the allowAddProduct2Receiving.
        /// </summary>
        private bool allowAddProduct2Receiving = false;

        /// <summary>
        /// Defines the canPrintProductLabelReceiving.
        /// </summary>
        private bool canPrintProductLabelReceiving = false;

        /// <summary>
        /// Defines the isReceivingExtraDetailEnabled.
        /// </summary>
        private bool isReceivingExtraDetailEnabled = false;

        /// <summary>
        /// Defines the isReceivingCatchAskEnabled.
        /// </summary>
        private bool isReceivingCatchAskEnabled = false;

        /// <summary>
        /// Defines the isPurchaseOrderWorkSheetModule.
        /// </summary>
        private bool isPurchaseOrderWorkSheetModule = false;

        /// <summary>
        /// Defines the isPOWorkSheetCatchAskEnabled.
        /// </summary>
        private bool isPOWorkSheetCatchAskEnabled = false;

        /// <summary>
        /// Defines the isReceivingCreateNewItemNoAsk.
        /// </summary>
        private bool isReceivingCreateNewItemNoAsk = false;

        /// <summary>
        /// Defines the isPOWorkSheetCreateNewItemNoAsk.
        /// </summary>
        private bool isPOWorkSheetCreateNewItemNoAsk = false;

        /// <summary>
        /// Defines the askRecvPrintAllSkidLabelsUponSave.
        /// </summary>
        private bool askRecvPrintAllSkidLabelsUponSave = false;

        /// <summary>
        /// Receiving quantity over warning level..
        /// </summary>
        private SysCommon.ActReceivingQtyOverValLevels recvQtyOverWarn;

        /// <summary>
        /// Receiving quantity ounder warning level..
        /// </summary>
        private SysCommon.ActReceivingQtyUnderValLevels recvQtyUnderWarn;

        /// <summary>
        /// Defines the isPickingSingleModuleNoCatchAskZeroEnabled.
        /// </summary>
        private bool isPickingSingleModuleNoCatchAskZeroEnabled = false;

        /// <summary>
        /// Defines the isPickingNoCatchAskZeroEnabled.
        /// </summary>
        private bool isPickingNoCatchAskZeroEnabled = false;

        /// <summary>
        /// Is picking module enabled..
        /// </summary>
        private bool isPickMod = false;

        /// <summary>
        /// Is release by order..
        /// </summary>
        private bool isRelByOrder = false;

        /// <summary>
        /// Is release by section..
        /// </summary>
        private bool isRelBySect = false;

        /// <summary>
        /// Allow adding new product to order..
        /// </summary>
        private bool allowAddProd2Pick = false;

        /// <summary>
        /// Is preselection picking criteria..
        /// </summary>
        private bool isPrePickCrit = false;

        /// <summary>
        /// Is picking disallow manual add new product..
        /// </summary>
        private bool isPickDisallowManualAddProd = false;

        /// <summary>
        /// Is picking associate bar code password required..
        /// </summary>
        private bool isPickAssocBarCodePwdReq = false;

        /// <summary>
        /// Picking quantity over warning level..
        /// </summary>
        private SysCommon.ActPickingQtyOverValLevels pickQtyOverWarn;

        /// <summary>
        /// Picking quantity ounder warning level..
        /// </summary>
        private SysCommon.ActPickingQtyUnderValLevels pickQtyUnderWarn;

        /// <summary>
        /// Is picking quantity under show base quantity available..
        /// </summary>
        private bool isPickQtyUnderShowBaseQtyAvail = false;

        /// <summary>
        /// Is picking refresh orders after release..
        /// </summary>
        private bool isPickRefreshOrdAfterRel = false;

        /// <summary>
        /// Is picking show item quantity available..
        /// </summary>
        private bool isPickShowItemQtyAvail = false;

        /// <summary>
        /// Is Picking cofirm detail mode..
        /// </summary>
        private bool isPickConfirmDet = false;

        /// <summary>
        /// Is picking popup comments..
        /// </summary>
        private bool isPickPopupCmnt = false;

        /// <summary>
        /// Is picking must review items before release..
        /// </summary>
        private bool isPickMustReviewItemsBeforeRel = false;

        /// <summary>
        /// Is picking confirm exit enabled..
        /// </summary>
        private bool isPickConfirmExit = false;

        /// <summary>
        /// Is picking override message 1 enabled..
        /// </summary>
        private bool isPickOverrideMsg1 = false;

        /// <summary>
        /// Is picking hide route colums enabled..
        /// </summary>
        private bool isPickHideRouteCol = false;

        /// <summary>
        /// Is picking extra detail enabled..
        /// </summary>
        private bool isPickExtraDet = false;

        /// <summary>
        /// Is picking catch as enabled..
        /// </summary>
        private bool isPickCatchAsk = false;

        /// <summary>
        /// Picking detail grid design layout..
        /// </summary>
        private int pickDetGridDesignLayout = 0;

        /// <summary>
        /// Is picking create new item no ask..
        /// </summary>
        private bool isPickCreateNewItemNoAsk = false;

        /// <summary>
        /// Picking item description option..
        /// </summary>
        private int pickItemDescOpt = 0;

        /// <summary>
        /// Disable creation of a skid during picking..
        /// </summary>
        private bool isPickDisableMakeSkid = false;

        /// <summary>
        /// Require password for adding manual detail quantity..
        /// </summary>
        private bool isPickAddManualDetailPasswordRequired = false;

        /// <summary>
        /// Defines the isPickingModuleDisallowLoadingAllOrders.
        /// </summary>
        private bool isPickingModuleDisallowLoadingAllOrders = false;

        /// <summary>
        /// Is picking single catch ask enabled..
        /// </summary>
        private bool isPickSingleCatchAsk = false;

        /// <summary>
        /// Is picking single product..
        /// </summary>
        private bool isPickSingleProd = false;

        /// <summary>
        /// Is picking single scanned product must match selected product..
        /// </summary>
        private bool isPickSingleScanProdMustMatchSelProd = false;

        /// <summary>
        /// Defines the isPickSinglePrintShipLabelFullQuantity.
        /// </summary>
        private bool isPickSinglePrintShipLabelFullQuantity = false;

        /// <summary>
        /// Defines the pickSingleShipLabelStopThresholdQuantity.
        /// </summary>
        private int pickSingleShipLabelStopThresholdQuantity = 0;

        /// <summary>
        /// Defines the canPrintShippingLabel.
        /// </summary>
        private bool canPrintShippingLabel = false;

        /// <summary>
        /// Defines the isFullQuantityEnabled.
        /// </summary>
        private bool isFullQuantityEnabled = false;

        /// <summary>
        /// Defines the isRepackQuantityEnabled.
        /// </summary>
        private bool isRepackQuantityEnabled = false;

        /// <summary>
        /// Defines the isDryQuantityEnabled.
        /// </summary>
        private bool isDryQuantityEnabled = false;

        /// <summary>
        /// Defines the isDryRepackQuantityEnabled.
        /// </summary>
        private bool isDryRepackQuantityEnabled = false;

        /// <summary>
        /// Defines the isFreshQuantityEnabled.
        /// </summary>
        private bool isFreshQuantityEnabled = false;

        /// <summary>
        /// Defines the isFreshRepackQuantityEnabled.
        /// </summary>
        private bool isFreshRepackQuantityEnabled = false;

        /// <summary>
        /// Defines the isFrozenQuantityEnabled.
        /// </summary>
        private bool isFrozenQuantityEnabled = false;

        /// <summary>
        /// Defines the isFrozenRepackQuantityEnabled.
        /// </summary>
        private bool isFrozenRepackQuantityEnabled = false;

        /// <summary>
        /// Defines the isLiveQuantityEnabled.
        /// </summary>
        private bool isLiveQuantityEnabled = false;

        /// <summary>
        /// Defines the isLiveRepackQuantityEnabled.
        /// </summary>
        private bool isLiveRepackQuantityEnabled = false;

        /// <summary>
        /// Defines the isPalletQuantityEnabled.
        /// </summary>
        private bool isPalletQuantityEnabled = false;

        /// <summary>
        /// Defines the isBundleQuantityEnabled.
        /// </summary>
        private bool isBundleQuantityEnabled = false;

        /// <summary>
        /// Defines the isReturnsNoCatchAskZeroEnabled.
        /// </summary>
        private bool isReturnsNoCatchAskZeroEnabled = false;

        /// <summary>
        /// Is returns module enabled..
        /// </summary>
        private bool isRetMod = false;

        /// <summary>
        /// Allow adding new products to returns..
        /// </summary>
        private bool allowAddProduct2Returns = false;

        /// <summary>
        /// Is returns confirm detail..
        /// </summary>
        private bool isRetConfirmDet = false;

        /// <summary>
        /// Is returns extra detail enabled..
        /// </summary>
        private bool isRetExtraDetEnabled = false;

        /// <summary>
        /// Is returns catch ask enabled..
        /// </summary>
        private bool isRetCatchAskEnabled = false;

        /// <summary>
        /// Defines the isRetCreateNewItemNoAsk.
        /// </summary>
        private bool isRetCreateNewItemNoAsk = false;

        /// <summary>
        /// Defines the isReplenishmentModule.
        /// </summary>
        private bool isReplenishmentModule = false;

        /// <summary>
        /// Defines the isPhysicalCountNoCatchAskZeroEnabled.
        /// </summary>
        private bool isPhysicalCountNoCatchAskZeroEnabled = false;

        /// <summary>
        /// Defines the isPhysicalCountModule.
        /// </summary>
        private bool isPhysicalCountModule = false;

        /// <summary>
        /// Defines the allowAddProduct2PhysicalCounts.
        /// </summary>
        private bool allowAddProduct2PhysicalCounts = false;

        /// <summary>
        /// Defines the isPhysicalExtraDetailEnabled.
        /// </summary>
        private bool isPhysicalExtraDetailEnabled = false;

        /// <summary>
        /// Defines the isPhysicalCatchAskEnabled.
        /// </summary>
        private bool isPhysicalCatchAskEnabled = false;

        /// <summary>
        /// Defines the isPhysicalCreateNewItemNoAsk.
        /// </summary>
        private bool isPhysicalCreateNewItemNoAsk = false;

        /// <summary>
        /// Defines the isPhysicalDoNotDefaultProductLocation.
        /// </summary>
        private bool isPhysicalDoNotDefaultProductLocation = false;

        /// <summary>
        /// Defines the isCycleCountNoCatchAskZeroEnabled.
        /// </summary>
        private bool isCycleCountNoCatchAskZeroEnabled = false;

        /// <summary>
        /// Defines the isCycleCountModule.
        /// </summary>
        private bool isCycleCountModule = false;

        /// <summary>
        /// Defines the allowAddProduct2CycleCounts.
        /// </summary>
        private bool allowAddProduct2CycleCounts = false;

        /// <summary>
        /// Defines the isCycleCountExtraDetailEnabled.
        /// </summary>
        private bool isCycleCountExtraDetailEnabled = false;

        /// <summary>
        /// Defines the isCycleCatchAskEnabled.
        /// </summary>
        private bool isCycleCatchAskEnabled = false;

        /// <summary>
        /// Defines the isCycleCreateNewItemNoAsk.
        /// </summary>
        private bool isCycleCreateNewItemNoAsk = false;

        /// <summary>
        /// Defines the isCycleDoNotDefaultProductLocation.
        /// </summary>
        private bool isCycleDoNotDefaultProductLocation = false;

        /// <summary>
        /// Defines the isTransferNoCatchAskZeroEnabled.
        /// </summary>
        private bool isTransferNoCatchAskZeroEnabled = false;

        /// <summary>
        /// Defines the canPrintProductLabelTransfers.
        /// </summary>
        private bool canPrintProductLabelTransfers = false;

        /// <summary>
        /// Is transfers module enabled..
        /// </summary>
        private bool isTransMod = false;

        /// <summary>
        /// Allowed adding new products to transfers..
        /// </summary>
        private bool allowAddProd2Trans = false;

        /// <summary>
        /// Is transfer extra detail enabled..
        /// </summary>
        private bool isTransExtraDetEnabled = false;

        /// <summary>
        /// Is transfers catch ask enabled..
        /// </summary>
        private bool isTransCatchAskEnabled = false;

        /// <summary>
        /// Is transfers create new item no ask..
        /// </summary>
        private bool isTransCreateNewItemNoAsk = false;

        /// <summary>
        /// Is transfers confirm from detail mode.
        /// </summary>
        // private bool isTransConfirmFromDet = false;

        /// <summary>
        /// Is transfers confirm to detail mode.
        /// </summary>
        ////private bool isTransConfirmToDet = false;

        /// <summary>
        /// Transfers quantity over warning level..
        /// </summary>
        private SysCommon.ActTransfersQtyOverValLevels transQtyOverWarn;

        /// <summary>
        /// Transfers quantity ounder warning level..
        /// </summary>
        private SysCommon.ActTransfersQtyUnderValLevels transQtyUnderWarn;

        /// <summary>
        /// Defines the isConvertUnitModuleNoCatchAskZeroEnabled.
        /// </summary>
        private bool isConvertUnitModuleNoCatchAskZeroEnabled = false;

        /// <summary>
        /// Defines the isConvertUnitsModule.
        /// </summary>
        private bool isConvertUnitsModule = false;

        /// <summary>
        /// Defines the isConvertUnitOnlyShowSell.
        /// </summary>
        private bool isConvertUnitOnlyShowSell = false;

        /// <summary>
        /// Defines the canPrintProductLabelConvertUnits.
        /// </summary>
        private bool canPrintProductLabelConvertUnits = false;

        /// <summary>
        /// Defines the canPrintProductLabelConvertProduct.
        /// </summary>
        private bool canPrintProductLabelConvertProduct = false;

        /// <summary>
        /// Defines the isConvertUnitCatchAskEnabled.
        /// </summary>
        private bool isConvertUnitCatchAskEnabled = false;

        /// <summary>
        /// Defines the isMoveInventoryModuleNoCatchAskZeroEnabled.
        /// </summary>
        private bool isMoveInventoryModuleNoCatchAskZeroEnabled = false;

        /// <summary>
        /// Defines the isMovesModule.
        /// </summary>
        private bool isMovesModule = false;

        /// <summary>
        /// Defines the isMoveInventoryCatchAskEnabled.
        /// </summary>
        private bool isMoveInventoryCatchAskEnabled = false;

        /// <summary>
        /// Defines the isPutAwayModule.
        /// </summary>
        private bool isPutAwayModule = false;

        /// <summary>
        /// Defines the isPutAwaySkidCatchAskEnabled.
        /// </summary>
        private bool isPutAwaySkidCatchAskEnabled = false;

        /// <summary>
        /// Defines the isAddToSkidModule.
        /// </summary>
        private bool isAddToSkidModule = false;

        /// <summary>
        /// Defines the isRemoveFromSkidModule.
        /// </summary>
        private bool isRemoveFromSkidModule = false;

        /// <summary>
        /// Defines the isBreakSkidModule.
        /// </summary>
        private bool isBreakSkidModule = false;

        /// <summary>
        /// Defines the isCreateSkidModule.
        /// </summary>
        private bool isCreateSkidModule = false;

        /// <summary>
        /// Defines the isSkidTrackingModule.
        /// </summary>
        private bool isSkidTrackingModule = false;

        /// <summary>
        /// Defines the isMoveSkidModule.
        /// </summary>
        private bool isMoveSkidModule = false;

        /// <summary>
        /// Defines the isAdjustmentNoCatchAskZeroEnabled.
        /// </summary>
        private bool isAdjustmentNoCatchAskZeroEnabled = false;

        /// <summary>
        /// Defines the isInventoryAdjustmentModule.
        /// </summary>
        private bool isInventoryAdjustmentModule = false;

        /// <summary>
        /// Defines the allowAddProduct2Adjustments.
        /// </summary>
        private bool allowAddProduct2Adjustments = false;

        /// <summary>
        /// Defines the isEnabledAdjustmentTypeFresh2Frozen.
        /// </summary>
        private bool isEnabledAdjustmentTypeFresh2Frozen = false;

        /// <summary>
        /// Defines the isAdjustmentsExtraDetailEnabled.
        /// </summary>
        private bool isAdjustmentsExtraDetailEnabled = false;

        /// <summary>
        /// Defines the isAdjustmentModuleCatchAskEnabled.
        /// </summary>
        private bool isAdjustmentModuleCatchAskEnabled = false;

        /// <summary>
        /// Defines the isAdjustmentCreateNewItemNoAsk.
        /// </summary>
        private bool isAdjustmentCreateNewItemNoAsk = false;

        /// <summary>
        /// Defines the isFinishedExtraDetailEnabled.
        /// </summary>
        private bool isFinishedExtraDetailEnabled = false;

        /// <summary>
        /// Defines the isMaterialExtraDetailEnabled.
        /// </summary>
        private bool isMaterialExtraDetailEnabled = false;

        /// <summary>
        /// Defines the isByProductsExtraDetailEnabled.
        /// </summary>
        private bool isByProductsExtraDetailEnabled = false;

        /// <summary>
        /// Defines the isTicketByProductNoCatchAskZeroEnabled.
        /// </summary>
        private bool isTicketByProductNoCatchAskZeroEnabled = false;

        /// <summary>
        /// Defines the isTicketFinishedNoCatchAskZeroEnabled.
        /// </summary>
        private bool isTicketFinishedNoCatchAskZeroEnabled = false;

        /// <summary>
        /// Defines the isTicketMaterialNoCatchAskZeroEnabled.
        /// </summary>
        private bool isTicketMaterialNoCatchAskZeroEnabled = false;

        /// <summary>
        /// Defines the isTicketFinishedGoodModule.
        /// </summary>
        private bool isTicketFinishedGoodModule = false;

        /// <summary>
        /// Defines the isTicketMaterialModule.
        /// </summary>
        private bool isTicketMaterialModule = false;

        /// <summary>
        /// Defines the isTicketByProductModule.
        /// </summary>
        private bool isTicketByProductModule = false;

        /// <summary>
        /// Defines the allowAddProduct2TicketFinishedGood.
        /// </summary>
        private bool allowAddProduct2TicketFinishedGood = false;

        /// <summary>
        /// Defines the allowAddProduct2TicketMaterial.
        /// </summary>
        private bool allowAddProduct2TicketMaterial = false;

        /// <summary>
        /// Defines the allowAddProduct2TicketByProduct.
        /// </summary>
        private bool allowAddProduct2TicketByProduct = false;

        /// <summary>
        /// Defines the isTicketFinishedCatchAskEnabled.
        /// </summary>
        private bool isTicketFinishedCatchAskEnabled = false;

        /// <summary>
        /// Defines the isTicketMaterialCatchAskEnabled.
        /// </summary>
        private bool isTicketMaterialCatchAskEnabled = false;

        /// <summary>
        /// Defines the isTicketByProductCatchAskEnabled.
        /// </summary>
        private bool isTicketByProductCatchAskEnabled = false;

        /// <summary>
        /// Defines the isTicketMaterialCreateNewItemNoAsk.
        /// </summary>
        private bool isTicketMaterialCreateNewItemNoAsk = false;

        /// <summary>
        /// Defines the isTicketByProductCreateNewItemNoAsk.
        /// </summary>
        private bool isTicketByProductCreateNewItemNoAsk = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemSettings"/> class.
        /// </summary>
        public SystemSettings()
        {
            SystemSettingsTasks.Init();
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsInvStrict
        /// Is inventory checking strict..
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

        /// <summary>
        /// Gets or sets the LastRefreshData.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsDetailEditScreenSetFocus2CatchWeight.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPrintProductLabelsAsBatch.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsSetCatchWeightZero.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsSetCatchCountZero.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsIntegerCheckOverride.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsEnableDefaultDetailValues.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsEnableDefaultDetailExtraValues.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the Theme.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsDisconnectedMode.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsDisconnectedModeLoadDetail.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the DetailDateMinMaxValidYearRange.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the ProdWeightUnitTolerancePercent.
        /// </summary>
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
        /// Gets or sets a value indicating whether DoesSelectingADifferentProductClearSkid
        /// In Activity Item, when you have a skid made for a line and go to select a new line:
        /// If this is true, it should clear the skid.
        /// Otherwise, keep the skid as is..
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
        /// Gets or sets a value indicating whether IsProductLabelCatchAsk
        /// Forces the Product Label edit screen to always popup.
        /// It used to only show if scheme was missing. This says OR do it every time..
        /// </summary>
        public bool IsProductLabelCatchAsk
        {
            get
            {
                return this.isProductLabelCatchAsk;
            }
            set
            {
                this.isProductLabelCatchAsk = value;
            }
        }

        /// <summary>
        /// Gets or sets the system default language.
        /// This is the language shown on startup,
        /// when a user is not logged in, or when a user
        /// does not have a preferred language selected..
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
        /// https://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx..
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
        /// Gets or sets a value indicating whether InventoryInquirySearchEntireWarehouse
        /// Gets or sets if the Inventory Inquiry screen should search the entire warehouse (true)
        /// or search specific section and slot (false)..
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

        /// <summary>
        /// Gets or sets the GTINCompanyPrefix.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsProductUnitBarCodeModifier1.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the ProductUnitBarCodeModifier1Codes.
        /// </summary>
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

        // public string LocMapWarehouseMask
        // {
        //    get
        //    {
        //        return this.locMapWarehouseMask;
        //    }

        // set
        //    {
        //        this.locMapWarehouseMask = value;
        //    }
        // }

        // public string LocMapSectionMask
        // {
        //    get
        //    {
        //        return this.locMapSectionMask;
        //    }

        // set
        //    {
        //        this.locMapSectionMask = value;
        //    }
        // }

        // public string LocMapSlotMask
        // {
        //    get
        //    {
        //        return this.locMapSlotMask;
        //    }

        // set
        //    {
        //        this.locMapSlotMask = value;
        //    }
        // }

        // public string LocMapSlotCoord1Mask
        // {
        //    get
        //    {
        //        return this.locMapSlotCoord1Mask;
        //    }

        // set
        //    {
        //        this.locMapSlotCoord1Mask = value;
        //    }
        // }

        // public string LocMapSlotCoord2Mask
        // {
        //    get
        //    {
        //        return this.locMapSlotCoord2Mask;
        //    }

        // set
        //    {
        //        this.locMapSlotCoord2Mask = value;
        //    }
        // }

        // public string LocMapSlotCoord3Mask
        // {
        //    get
        //    {
        //        return this.locMapSlotCoord3Mask;
        //    }

        // set
        //    {
        //        this.locMapSlotCoord3Mask = value;
        //    }
        // }

        // public string LocMapSlotCoord1Name
        // {
        //    get
        //    {
        //        return this.locMapSlotCoord1Name;
        //    }

        // set
        //    {
        //        this.locMapSlotCoord1Name = value;
        //    }
        // }

        // public string LocMapSlotCoord2Name
        // {
        //    get
        //    {
        //        return this.locMapSlotCoord2Name;
        //    }

        // set
        //    {
        //        this.locMapSlotCoord2Name = value;
        //    }
        // }

        // public string LocMapSlotCoord3Name
        // {
        //    get
        //    {
        //        return this.locMapSlotCoord3Name;
        //    }

        // set
        //    {
        //        this.locMapSlotCoord3Name = value;
        //    }
        // }
        /// <summary>
        /// Gets or sets a value indicating whether IsDefaultLocToProductDefaultLoc.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsVerifyProductLocation.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPutAwaySkidNoCatchAskZeroEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPOWorkSheetNoCatchAskZeroEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsReceivingNoCatchAskZeroEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsReceivingModule.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsReceivingCatchAskEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsReceivingExtraDetailEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether CanPrintProductLabelReceiving.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProduct2Receiving.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPOWorkSheetCatchAskEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPOWorkSheetModule.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsReceivingCreateNewItemNoAsk.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPOWorkSheetCreateNewItemNoAsk.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the RecvQtyUnderWarn.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the RecvQtyOverWarn.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether AskRecvPrintAllSkidLabelsUponSave.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickingNoCatchAskZeroEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickingModuleDisallowLoadingAllOrders.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the PickItemDescOpt.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickMod.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickHideRouteCol.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickOverrideMsg1.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickConfirmExit.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickCatchAsk.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickExtraDet.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickMustReviewItemsBeforeRel.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickPopupCmnt.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickRefreshOrdAfterRel.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickQtyUnderShowBaseQtyAvail.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickDisallowManualAddProd.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickAssocBarCodePwdReq.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProd2Pick.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPrePickCrit.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the PickQtyUnderWarn.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the PickQtyOverWarn.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickConfirmDet.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickShowItemQtyAvail.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsRelByOrder.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsRelBySect.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the PickDetGridDesignLayout.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickCreateNewItemNoAsk.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickDisableMakeSkid.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickAddManualDetailPasswordRequired.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickingSingleModuleNoCatchAskZeroEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickSingleProd.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickSingleCatchAsk.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickSingleScanProdMustMatchSelProd.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPickSinglePrintShipLabelFullQuantity.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the PickSingleShipLabelStopThresholdQuantity.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether CanPrintShippingLabel.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsFullQuantityEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsRepackQuantityEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsDryQuantityEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsDryRepackQuantityEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsFreshQuantityEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsFreshRepackQuantityEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsFrozenQuantityEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsFrozenRepackQuantityEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsLiveQuantityEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsLiveRepackQuantityEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPalletQuantityEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsBundleQuantityEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsReturnsNoCatchAskZeroEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsRetMod.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsRetExtraDetEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsRetConfirmDet.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProd2Ret.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsRetCatchAskEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsRetCreateNewItemNoAsk.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPhysicalModule.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPhysicalCatchAskEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPhysicalExtraDetailEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProduct2Physical.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPhysicalCreateNewItemNoAsk.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPhysicalDoNotDefaultProductLocation.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsCycleCountNoCatchAskZeroEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsCycleModule.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsCycleCatchAskEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsCycleExtraDetailEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProduct2Cycle.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsCycleCreateNewItemNoAsk.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsCycleDoNotDefaultProductLocation.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsFinishedExtraDetailEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsIngredientsExtraDetailEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsByProductsExtraDetailEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketMaterialNoCatchAskZeroEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketByProductNoCatchAskZeroEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketFinishedNoCatchAskZeroEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPhysicalCountNoCatchAskZeroEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketFinishedGoodModule.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketMaterialModule.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketByProductModule.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketFinishedCatchAskEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketMaterialCatchAskEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketByProductCatchAskEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProduct2TicketFinished.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProduct2TicketMaterial.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProduct2TicketByProduct.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketMaterialCreateNewItemNoAsk.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketByProductCreateNewItemNoAsk.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsAdjustmentNoCatchAskZeroEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsInventoryAdjustmentModule.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsAdjustmentCatchAskEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsAdjustmentsExtraDetailEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsEnabledAdjustmentTypeFresh2Frozen.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProduct2Adjustments.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsAdjustmentCreateNewItemNoAsk.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsTransferNoCatchAskZeroEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether CanPrintProductLabelTransfers.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsTransMod.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsTransCatchAskEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsTransExtraDetEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProd2Trans.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsTransCreateNewItemNoAsk.
        /// </summary>
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

        // public bool IsTransConfirmFromDet
        // {
        //    get
        //    {
        //        return this.isTransConfirmFromDet;
        //    }

        // set
        //    {
        //        this.isTransConfirmFromDet = value;
        //    }
        // }

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
        /// <summary>
        /// Gets or sets the TransQtyUnderWarn.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the TransQtyOverWarn.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsConvertUnitModuleNoCatchAskZeroEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsConvertUnitsModule.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsConvertUnitCatchAskEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsConvertUnitOnlyShowSell.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether CanPrintProductLabelConvertUnits.
        /// </summary>
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
        /// Gets or sets a value indicating whether CanPrintProductLabelConvertProduct
        /// <para>Causes a label to print from ActivityDetail screen:</para>
        /// <para>- After scanning with complete scheme data.</para>
        /// <para>- After save of scheme data in ActivtyDetailEdit screen.</para>.
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

        /// <summary>
        /// Gets or sets a value indicating whether IsMoveInventoryModuleNoCatchAskZeroEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsMovesModule.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsMoveInventoryCatchAskEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsSkidTrackingModule.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsAddToSkidModule.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsRemoveFromSkidModule.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsBreakSkidModule.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsCreateSkidModule.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsMoveSkidModule.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPutAwayModule.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsPutAwaySkidCatchAskEnabled.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsReplenishmentModule.
        /// </summary>
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

        /// <summary>
        /// The RegisterSysSettingsListenerStatusOut.
        /// </summary>
        /// <param name="sysSettingsListener">The sysSettingsListener<see cref="ISysSettingsEventListener"/>.</param>
        public void RegisterSysSettingsListenerStatusOut(ISysSettingsEventListener sysSettingsListener)
        {
            SystemSettingsTasks.OnSysSettingsStatusOut += new SystemSettingsTasks.OnSysSettingsStatusOutEventHandler(sysSettingsListener.OnSysSettingsStatusOut);
        }

        /// <summary>
        /// The UnregisterSysSettingsListenerStatusOut.
        /// </summary>
        /// <param name="sysSettingsListener">The sysSettingsListener<see cref="ISysSettingsEventListener"/>.</param>
        public void UnregisterSysSettingsListenerStatusOut(ISysSettingsEventListener sysSettingsListener)
        {
            SystemSettingsTasks.OnSysSettingsStatusOut -= new SystemSettingsTasks.OnSysSettingsStatusOutEventHandler(sysSettingsListener.OnSysSettingsStatusOut);
        }

        /// <summary>
        /// The RegisterSysSettingsListenerProgressOut.
        /// </summary>
        /// <param name="sysSettingsListener">The sysSettingsListener<see cref="ISysSettingsEventListener"/>.</param>
        public void RegisterSysSettingsListenerProgressOut(ISysSettingsEventListener sysSettingsListener)
        {
            SystemSettingsTasks.OnSysSettingsProgressOut += new SystemSettingsTasks.OnSysSettingsProgressOutEventHandler(
                sysSettingsListener.OnSysSettingsProgressOut);
        }

        /// <summary>
        /// The UnregisterSysSettingsListenerProgressOut.
        /// </summary>
        /// <param name="sysSettingsListener">The sysSettingsListener<see cref="ISysSettingsEventListener"/>.</param>
        public void UnregisterSysSettingsListenerProgressOut(ISysSettingsEventListener sysSettingsListener)
        {
            SystemSettingsTasks.OnSysSettingsProgressOut -= new SystemSettingsTasks.OnSysSettingsProgressOutEventHandler(
                sysSettingsListener.OnSysSettingsProgressOut);
        }
    }
}
