// <copyright file="SystemSettings.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using dMobile.Bedrock;

    /// <summary>
    /// Defines the <see cref="SystemSettings" />.
    /// </summary>
    [Serializable]
    public class SystemSettings : ISysSettingsEventPublisher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemSettings"/> class.
        /// </summary>
        public SystemSettings()
        {
            SystemSettingsTasks.Init();
        }

        /// <summary>
        /// Gets or sets a value indicating whether strict inventory quantity checking is enabled.
        /// </summary>
        [Obsolete("Quantity strictness will be enforced only on the communication server." +
            " With the exception of serial number tracking.")]
        public bool IsInvStrict { get; set; }

        /// <summary>
        /// Gets or sets the LastRefreshData.
        /// </summary>
        public DateTime LastRefreshData { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets a value indicating whether the detail edit screen should set focus to the catch weight field.
        /// </summary>
        public bool IsDetailEditScreenSetFocus2CatchWeight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to print product labels as a batch instead of individually.
        /// </summary>
        public bool IsPrintProductLabelsAsBatch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to set the catch weight to zero when the detail is entered.
        /// </summary>
        public bool IsSetCatchWeightZero { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to set the catch count to zero when the detail is entered.
        /// </summary>
        public bool IsSetCatchCountZero { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether non integer quantities are allowed.
        /// </summary>
        public bool IsIntegerCheckOverride { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether default detail values are enabled.
        /// </summary>
        public bool IsEnableDefaultDetailValues { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether default detail extra values are enabled.
        /// </summary>
        public bool IsEnableDefaultDetailExtraValues { get; set; }

        /// <summary>
        /// Gets or sets the Theme.
        /// </summary>
        public Theme Theme { get; set; } = Theme.Light;

        /// <summary>
        /// Gets or sets a value indicating whether dmobile is in disconnected mode.
        /// </summary>
        public bool IsDisconnectedMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether dmobile should load detail records when in disconnected mode.
        /// </summary>
        public bool IsDisconnectedModeLoadDetail { get; set; }

        /// <summary>
        /// Gets or sets the detail date min max valid year range.
        /// </summary>
        public int DetailDateMinMaxValidYearRange { get; set; }

        /// <summary>
        /// Gets or sets the product weight unit tolerance percent.
        /// </summary>
        public decimal ProdWeightUnitTolerancePercent { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether to clear skid when selecting a different product.
        /// </summary>
        /// <remarks>
        /// In Activity Item, when you have a skid made for a line and go to select a new line:
        /// If this is true, it should clear the skid.
        /// Otherwise, keep the skid as is..
        /// </remarks>
        public bool DoesSelectingADifferentProductClearSkid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsProductLabelCatchAsk
        /// Forces the Product Label edit screen to always popup.
        /// It used to only show if scheme was missing. This says OR do it every time..
        /// </summary>
        public bool IsProductLabelCatchAsk { get; set; }

        /// <summary>
        /// Gets or sets the system default language.
        /// This is the language shown on startup,
        /// when a user is not logged in, or when a user
        /// does not have a preferred language selected..
        /// </summary>
        public string DefaultLanguage { get; set; } = "ENU";

        /// <summary>
        /// Gets or sets the paths to the language files.
        /// Each file should have a naming convention of dMobileTranslation.CODE.txt
        /// where code is 3 letters from this site.
        /// https://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx..
        /// </summary>
        [XmlArrayItem("Path")]
        public List<string> LanguagePaths { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether InventoryInquirySearchEntireWarehouse
        /// Gets or sets if the Inventory Inquiry screen should search the entire warehouse (true)
        /// or search specific section and slot (false)..
        /// </summary>
        public bool InventoryInquirySearchEntireWarehouse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disable the move all slot quantity option.
        /// </summary>
        public bool IsDisableMoveAllSlotQuantityOption { get; set; } = false;

        /// <summary>
        /// Gets or sets the GTINCompanyPrefix.
        /// </summary>
        public string GTINCompanyPrefix { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsProductUnitBarCodeModifier1.
        /// </summary>
        public bool IsProductUnitBarCodeModifier1 { get; set; }

        /// <summary>
        /// Gets or sets the ProductUnitBarCodeModifier1Codes.
        /// </summary>
        public string ProductUnitBarCodeModifier1Codes { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsDefaultLocToProductDefaultLoc.
        /// </summary>
        public bool IsDefaultLocToProductDefaultLoc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsVerifyProductLocation.
        /// </summary>
        public bool IsVerifyProductLocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPutAwaySkidNoCatchAskZeroEnabled.
        /// </summary>
        public bool IsPutAwaySkidNoCatchAskZeroEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPOWorkSheetNoCatchAskZeroEnabled.
        /// </summary>
        public bool IsPOWorkSheetNoCatchAskZeroEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsReceivingNoCatchAskZeroEnabled.
        /// </summary>
        public bool IsReceivingNoCatchAskZeroEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsReceivingModule.
        /// </summary>
        public bool IsReceivingModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsReceivingCatchAskEnabled.
        /// </summary>
        public bool IsReceivingCatchAskEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsReceivingExtraDetailEnabled.
        /// </summary>
        public bool IsReceivingExtraDetailEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CanPrintProductLabelReceiving.
        /// </summary>
        public bool CanPrintProductLabelReceiving { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProduct2Receiving.
        /// </summary>
        public bool AllowAddProduct2Receiving { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPOWorkSheetCatchAskEnabled.
        /// </summary>
        public bool IsPOWorkSheetCatchAskEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPOWorkSheetModule.
        /// </summary>
        public bool IsPOWorkSheetModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsReceivingCreateNewItemNoAsk.
        /// </summary>
        public bool IsReceivingCreateNewItemNoAsk { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPOWorkSheetCreateNewItemNoAsk.
        /// </summary>
        public bool IsPOWorkSheetCreateNewItemNoAsk { get; set; }

        /// <summary>
        /// Gets or sets the RecvQtyUnderWarn.
        /// </summary>
        public SysCommon.ActReceivingQtyUnderValLevels RecvQtyUnderWarn { get; set; }

        /// <summary>
        /// Gets or sets the RecvQtyOverWarn.
        /// </summary>
        public SysCommon.ActReceivingQtyOverValLevels RecvQtyOverWarn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether AskRecvPrintAllSkidLabelsUponSave.
        /// </summary>
        public bool AskRecvPrintAllSkidLabelsUponSave { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickingNoCatchAskZeroEnabled.
        /// </summary>
        public bool IsPickingNoCatchAskZeroEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickingModuleDisallowLoadingAllOrders.
        /// </summary>
        public bool IsPickingModuleDisallowLoadingAllOrders { get; set; }

        /// <summary>
        /// Gets or sets the PickItemDescOpt.
        /// </summary>
        public int PickItemDescOpt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickMod.
        /// </summary>
        public bool IsPickMod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickHideRouteCol.
        /// </summary>
        public bool IsPickHideRouteCol { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickOverrideMsg1.
        /// </summary>
        public bool IsPickOverrideMsg1 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickConfirmExit.
        /// </summary>
        public bool IsPickConfirmExit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickCatchAsk.
        /// </summary>
        public bool IsPickCatchAsk { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickExtraDet.
        /// </summary>
        public bool IsPickExtraDet { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsPickMustReviewItemsBeforeRel.
        /// </summary>
        public bool IsPickMustReviewItemsBeforeRel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickPopupCmnt.
        /// </summary>
        public bool IsPickPopupCmnt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickRefreshOrdAfterRel.
        /// </summary>
        public bool IsPickRefreshOrdAfterRel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickQtyUnderShowBaseQtyAvail.
        /// </summary>
        public bool IsPickQtyUnderShowBaseQtyAvail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickDisallowManualAddProd.
        /// </summary>
        public bool IsPickDisallowManualAddProd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickAssocBarCodePwdReq.
        /// </summary>
        public bool IsPickAssocBarCodePwdReq { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProd2Pick.
        /// </summary>
        public bool AllowAddProd2Pick { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPrePickCrit.
        /// </summary>
        public bool IsPrePickCrit { get; set; }

        /// <summary>
        /// Gets or sets the PickQtyUnderWarn.
        /// </summary>
        public SysCommon.ActPickingQtyUnderValLevels PickQtyUnderWarn { get; set; }

        /// <summary>
        /// Gets or sets the PickQtyOverWarn.
        /// </summary>
        public SysCommon.ActPickingQtyOverValLevels PickQtyOverWarn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickConfirmDet.
        /// </summary>
        public bool IsPickConfirmDet { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickShowItemQtyAvail.
        /// </summary>
        public bool IsPickShowItemQtyAvail { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsRelByOrder.
        /// </summary>
        public bool IsRelByOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsRelBySect.
        /// </summary>
        public bool IsRelBySect { get; set; } = false;

        /// <summary>
        /// Gets or sets the PickDetGridDesignLayout.
        /// </summary>
        public int PickDetGridDesignLayout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickCreateNewItemNoAsk.
        /// </summary>
        public bool IsPickCreateNewItemNoAsk { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickDisableMakeSkid.
        /// </summary>
        public bool IsPickDisableMakeSkid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickAddManualDetailPasswordRequired.
        /// </summary>
        public bool IsPickAddManualDetailPasswordRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickingSingleModuleNoCatchAskZeroEnabled.
        /// </summary>
        public bool IsPickingSingleModuleNoCatchAskZeroEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsSingleActivityItem.
        /// </summary>
        public bool IsSingleActivityItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickSingleCatchAsk.
        /// </summary>
        public bool IsPickSingleCatchAsk { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickSingleScanProdMustMatchSelProd.
        /// </summary>
        public bool IsPickSingleScanProdMustMatchSelProd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPickSinglePrintShipLabelFullQuantity.
        /// </summary>
        public bool IsPickSinglePrintShipLabelFullQuantity { get; set; }

        /// <summary>
        /// Gets or sets the PickSingleShipLabelStopThresholdQuantity.
        /// </summary>
        public int PickSingleShipLabelStopThresholdQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CanPrintShippingLabel.
        /// </summary>
        public bool CanPrintShippingLabel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsFullQuantityEnabled.
        /// </summary>
        public bool IsFullQuantityEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsRepackQuantityEnabled.
        /// </summary>
        public bool IsRepackQuantityEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDryQuantityEnabled.
        /// </summary>
        public bool IsDryQuantityEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDryRepackQuantityEnabled.
        /// </summary>
        public bool IsDryRepackQuantityEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsFreshQuantityEnabled.
        /// </summary>
        public bool IsFreshQuantityEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsFreshRepackQuantityEnabled.
        /// </summary>
        public bool IsFreshRepackQuantityEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsFrozenQuantityEnabled.
        /// </summary>
        public bool IsFrozenQuantityEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsFrozenRepackQuantityEnabled.
        /// </summary>
        public bool IsFrozenRepackQuantityEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsLiveQuantityEnabled.
        /// </summary>
        public bool IsLiveQuantityEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsLiveRepackQuantityEnabled.
        /// </summary>
        public bool IsLiveRepackQuantityEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPalletQuantityEnabled.
        /// </summary>
        public bool IsPalletQuantityEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsBundleQuantityEnabled.
        /// </summary>
        public bool IsBundleQuantityEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsReturnsNoCatchAskZeroEnabled.
        /// </summary>
        public bool IsReturnsNoCatchAskZeroEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsRetMod.
        /// </summary>
        public bool IsRetMod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsRetExtraDetEnabled.
        /// </summary>
        public bool IsRetExtraDetEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsRetConfirmDet.
        /// </summary>
        public bool IsRetConfirmDet { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProd2Ret.
        /// </summary>
        public bool AllowAddProd2Ret { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsRetCatchAskEnabled.
        /// </summary>
        public bool IsRetCatchAskEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsRetCreateNewItemNoAsk.
        /// </summary>
        public bool IsRetCreateNewItemNoAsk { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPhysicalModule.
        /// </summary>
        public bool IsPhysicalModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPhysicalCatchAskEnabled.
        /// </summary>
        public bool IsPhysicalCatchAskEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPhysicalExtraDetailEnabled.
        /// </summary>
        public bool IsPhysicalExtraDetailEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProduct2Physical.
        /// </summary>
        public bool AllowAddProduct2Physical { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPhysicalCreateNewItemNoAsk.
        /// </summary>
        public bool IsPhysicalCreateNewItemNoAsk { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPhysicalDoNotDefaultProductLocation.
        /// </summary>
        public bool IsPhysicalDoNotDefaultProductLocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsCycleCountNoCatchAskZeroEnabled.
        /// </summary>
        public bool IsCycleCountNoCatchAskZeroEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsCycleModule.
        /// </summary>
        public bool IsCycleModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsCycleCatchAskEnabled.
        /// </summary>
        public bool IsCycleCatchAskEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsCycleExtraDetailEnabled.
        /// </summary>
        public bool IsCycleExtraDetailEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProduct2Cycle.
        /// </summary>
        public bool AllowAddProduct2Cycle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsCycleCreateNewItemNoAsk.
        /// </summary>
        public bool IsCycleCreateNewItemNoAsk { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsCycleDoNotDefaultProductLocation.
        /// </summary>
        public bool IsCycleDoNotDefaultProductLocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsFinishedExtraDetailEnabled.
        /// </summary>
        public bool IsFinishedExtraDetailEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsIngredientsExtraDetailEnabled.
        /// </summary>
        public bool IsIngredientsExtraDetailEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsByProductsExtraDetailEnabled.
        /// </summary>
        public bool IsByProductsExtraDetailEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketMaterialNoCatchAskZeroEnabled.
        /// </summary>
        public bool IsTicketMaterialNoCatchAskZeroEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketByProductNoCatchAskZeroEnabled.
        /// </summary>
        public bool IsTicketByProductNoCatchAskZeroEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketFinishedNoCatchAskZeroEnabled.
        /// </summary>
        public bool IsTicketFinishedNoCatchAskZeroEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPhysicalCountNoCatchAskZeroEnabled.
        /// </summary>
        public bool IsPhysicalCountNoCatchAskZeroEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketFinishedGoodModule.
        /// </summary>
        public bool IsTicketFinishedGoodModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketMaterialModule.
        /// </summary>
        public bool IsTicketMaterialModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketByProductModule.
        /// </summary>
        public bool IsTicketByProductModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketFinishedCatchAskEnabled.
        /// </summary>
        public bool IsTicketFinishedCatchAskEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketMaterialCatchAskEnabled.
        /// </summary>
        public bool IsTicketMaterialCatchAskEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketByProductCatchAskEnabled.
        /// </summary>
        public bool IsTicketByProductCatchAskEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProduct2TicketFinished.
        /// </summary>
        public bool AllowAddProduct2TicketFinished { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProduct2TicketMaterial.
        /// </summary>
        public bool AllowAddProduct2TicketMaterial { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProduct2TicketByProduct.
        /// </summary>
        public bool AllowAddProduct2TicketByProduct { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketMaterialCreateNewItemNoAsk.
        /// </summary>
        public bool IsTicketMaterialCreateNewItemNoAsk { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsTicketByProductCreateNewItemNoAsk.
        /// </summary>
        public bool IsTicketByProductCreateNewItemNoAsk { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsAdjustmentNoCatchAskZeroEnabled.
        /// </summary>
        public bool IsAdjustmentNoCatchAskZeroEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsInventoryAdjustmentModule.
        /// </summary>
        public bool IsInventoryAdjustmentModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsAdjustmentCatchAskEnabled.
        /// </summary>
        public bool IsAdjustmentCatchAskEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsAdjustmentsExtraDetailEnabled.
        /// </summary>
        public bool IsAdjustmentsExtraDetailEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsEnabledAdjustmentTypeFresh2Frozen.
        /// </summary>
        public bool IsEnabledAdjustmentTypeFresh2Frozen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProduct2Adjustments.
        /// </summary>
        public bool AllowAddProduct2Adjustments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsAdjustmentCreateNewItemNoAsk.
        /// </summary>
        public bool IsAdjustmentCreateNewItemNoAsk { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsTransferNoCatchAskZeroEnabled.
        /// </summary>
        public bool IsTransferNoCatchAskZeroEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CanPrintProductLabelTransfers.
        /// </summary>
        public bool CanPrintProductLabelTransfers { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether warehouse transfers is enabled.
        /// </summary>
        public bool IsTransMod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsTransCatchAskEnabled.
        /// </summary>
        public bool IsTransCatchAskEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsTransExtraDetEnabled.
        /// </summary>
        public bool IsTransExtraDetEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether AllowAddProd2Trans.
        /// </summary>
        public bool AllowAddProd2Trans { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsTransCreateNewItemNoAsk.
        /// </summary>
        public bool IsTransCreateNewItemNoAsk { get; set; }

        /// <summary>
        /// Gets or sets the TransQtyUnderWarn.
        /// </summary>
        public SysCommon.ActTransfersQtyUnderValLevels TransQtyUnderWarn { get; set; }

        /// <summary>
        /// Gets or sets the TransQtyOverWarn.
        /// </summary>
        public SysCommon.ActTransfersQtyOverValLevels TransQtyOverWarn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsConvertUnitModuleNoCatchAskZeroEnabled.
        /// </summary>
        public bool IsConvertUnitModuleNoCatchAskZeroEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsConvertUnitsModule.
        /// </summary>
        public bool IsConvertUnitsModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsConvertUnitCatchAskEnabled.
        /// </summary>
        public bool IsConvertUnitCatchAskEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsConvertUnitOnlyShowSell.
        /// </summary>
        public bool IsConvertUnitOnlyShowSell { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CanPrintProductLabelConvertUnits.
        /// </summary>
        public bool CanPrintProductLabelConvertUnits { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CanPrintProductLabelConvertProduct.
        /// <para>Causes a label to print from ActivityDetail screen:</para>
        /// <para>- After scanning with complete scheme data.</para>
        /// <para>- After save of scheme data in ActivtyDetailEdit screen.</para>.
        /// </summary>
        public bool CanPrintProductLabelConvertProduct { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsMoveInventoryModuleNoCatchAskZeroEnabled.
        /// </summary>
        public bool IsMoveInventoryModuleNoCatchAskZeroEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsMovesModule.
        /// </summary>
        public bool IsMovesModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsMoveInventoryCatchAskEnabled.
        /// </summary>
        public bool IsMoveInventoryCatchAskEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsSkidTrackingModule.
        /// </summary>
        public bool IsSkidTrackingModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsAddToSkidModule.
        /// </summary>
        public bool IsAddToSkidModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsRemoveFromSkidModule.
        /// </summary>
        public bool IsRemoveFromSkidModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsBreakSkidModule.
        /// </summary>
        public bool IsBreakSkidModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsCreateSkidModule.
        /// </summary>
        public bool IsCreateSkidModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsMoveSkidModule.
        /// </summary>
        public bool IsMoveSkidModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPutAwayModule.
        /// </summary>
        public bool IsPutAwayModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPutAwaySkidCatchAskEnabled.
        /// </summary>
        public bool IsPutAwaySkidCatchAskEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsReplenishmentModule.
        /// </summary>
        public bool IsReplenishmentModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a filter selection screen should be shown once in an activity header.
        /// </summary>
        public bool IsShowActivityFilterSelection { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the count activity item data should be refreshed more than once.
        /// </summary>
        public bool IsCountActivityItemRefreshData { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the activity has new details added.
        /// </summary>
        public bool IsNewActivityDetailsAdded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Cut Sheet Finished Good Module is enabled.
        /// </summary>
        public bool IsCutSheetFinishedGoodModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Cut Sheet Material Module is enabled.
        /// </summary>
        public bool IsCutSheetRawMaterialModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Yield Sheet Finished Good Module is enabled.
        /// </summary>
        public bool IsYieldSheetFinishedGoodModule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Yield Sheet Material Module is enabled.
        /// </summary>
        public bool IsYieldSheetRawMaterialModule { get; set; }

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
