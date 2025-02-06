// <copyright file="SystemSettingsTasks.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// <see cref="SystemSettingsTasks" /> System settings tasks class.
    /// </summary>
    public static class SystemSettingsTasks
    {
        /// <summary>
        /// On system setting status out event handler.
        /// </summary>
        /// <param name="publisher"><see cref="ISysSettingsEventPublisher"/> System settings event publisher.</param>
        /// <param name="eventArgs"><see cref="SysSettingsEventArgs"/> System settings status out event arguments.</param>
        public delegate void OnSysSettingsStatusOutEventHandler(
            ISysSettingsEventPublisher publisher,
            SysSettingsEventArgs eventArgs);

        /// <summary>
        /// On system settings progress out event handler.
        /// </summary>
        /// <param name="publisher"><see cref="ISysSettingsEventPublisher"/> System settings event publisher.</param>
        /// <param name="eventArgs"><see cref="SysSettingsEventArgs"/> System settings progress out event arguments.</param>
        public delegate void OnSysSettingsProgressOutEventHandler(
            ISysSettingsEventPublisher publisher,
            SysSettingsEventArgs eventArgs);

        /// <summary>
        /// On system settings status out event.
        /// </summary>
        public static event OnSysSettingsStatusOutEventHandler OnSysSettingsStatusOut;

        /// <summary>
        /// On system settings progress out event.
        /// </summary>
        public static event OnSysSettingsProgressOutEventHandler OnSysSettingsProgressOut;

        /// <summary>
        /// Gets or sets the communication setup.
        /// </summary>
        public static CommunicationSetup CommSetup { get; set; } = new CommunicationSetup();

        /// <summary>
        /// Gets or sets the system socket.
        /// </summary>
        public static SysSocket Socket { get; set; } = new SysSocket();

        /// <summary>
        /// Gets or sets the settings file name.
        /// </summary>
        public static string FileName { get; set; } = "dMobileSystemSettings.xml";

        /// <summary>
        /// Initialize the system settings.
        /// </summary>
        public static void Init()
        {
            CommSetup = CommSetup.LoadData();
            Socket.BasePort = CommSetup.BaseSocketPort;
            Socket.DefSocketPort = CommSetup.DefaultSocketPort;
            Socket.MaxPorts = CommSetup.MaxSocketPorts;
            Socket.ConnTotalRetries = CommSetup.ConnectionAttempts;
            Socket.SendTotalRetries = CommSetup.SendAttempts;
            Socket.ReceiveTotalRetries = CommSetup.ReceiveAttempts;
            Socket.SendTimeOutMilliseconds = CommSetup.SendTimeOutSeconds * 1000;
            Socket.ReceiveTimeOutMilliseconds = CommSetup.ReceiveTimeOutSeconds * 1000;
            Socket.ReceiveBuffSize = CommSetup.ReceiveBufferSize;
            Socket.ServerIp = CommSetup.HostIpAddressComponent1.ToString() + "."
                + CommSetup.HostIpAddressComponent2.ToString() + "."
                + CommSetup.HostIpAddressComponent3.ToString() + "."
                + CommSetup.HostIpAddressComponent4.ToString();
        }

        /// <summary>
        /// The LoadData.
        /// </summary>
        /// <param name="settings">The settings<see cref="SystemSettings"/>.</param>
        public static void LoadData(ref SystemSettings settings)
        {
            var serializer = new XmlSerializer(typeof(SystemSettings));
            var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var fullPathToMyFile = Path.Combine(appDir, SystemSettingsTasks.FileName);
            if (!File.Exists(fullPathToMyFile))
            {
                _ = SaveData(ref settings);
            }

            // Create a FileStream to read with.
            var reader = new FileStream(fullPathToMyFile, FileMode.Open);

            // Deserialize the stream to the object, and close the TextReader
            settings = (SystemSettings)serializer.Deserialize(reader);
            reader.Close();
        }

        /// <summary>
        /// The CreateFirstRequest.
        /// </summary>
        /// <returns>.</returns>
        public static string CreateFirstRequest()
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);
            var deviceID = SysDeviceId.GetDeviceId();
            var revision = new DateTime(1900, 1, 1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var login = new ContactLogin().LoadData();
            var loginID = login.LoginID.ToString();

            var request = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"), string.Empty, new XElement(
                    "imclient",
                    new XElement(
                        "command",
                        new XElement("action", "REQUEST SYSTEM SETTINGS"),
                        new XElement("device_id", deviceID),
                        new XElement("login_id", loginID),
                        new XElement("revision_datetime", revision))));

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// Gets the second set of system settings.
        /// We exceeded the limit of columns in VFP.
        /// We decided the simplest and easiest way for now was to make
        /// a second request. June 24, 2016.
        /// </summary>
        /// <returns>.</returns>
        public static string CreateSecondRequest()
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);
            var deviceID = SysDeviceId.GetDeviceId();
            var revision = new DateTime(1900, 1, 1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var login = new ContactLogin().LoadData();
            var loginID = login.LoginID.ToString();

            var request = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"), string.Empty, new XElement(
                    "imclient",
                    new XElement(
                        "command",
                        new XElement("action", "REQUEST SYSTEM SETTINGS 2"),
                        new XElement("device_id", deviceID),
                        new XElement("login_id", loginID),
                        new XElement("revision_datetime", revision))));

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// The AssignPropertiesFromDataRowPicking.
        /// </summary>
        /// <param name="drow">.</param>
        /// <param name="settings">The settings<see cref="SystemSettings"/>.</param>
        public static void AssignPropertiesFromDataRowPicking(DataRow drow, ref SystemSettings settings)
        {
            settings.IsReturnsNoCatchAskZeroEnabled = Convert.ToBoolean(drow["IsReturnsNoCatchAskZeroEnabled"]);
            settings.IsPickingNoCatchAskZeroEnabled = Convert.ToBoolean(drow["IsPickingNoCatchAskZeroEnabled"]);
            settings.IsPickingSingleModuleNoCatchAskZeroEnabled = Convert.ToBoolean(
                drow["IsPickingSingleModuleNoCatchAskZeroEnabled"]);

            settings.IsPickMod = Convert.ToBoolean(drow["IsPickingModule"]);
            settings.AllowAddProd2Pick = Convert.ToBoolean(drow["AllowAddProduct2Picking"]);
            settings.CanPrintShippingLabel = Convert.ToBoolean(drow["CanPrintPickingModuleShippingLabel"]);
            settings.IsRelByOrder = Convert.ToBoolean(drow["IsReleaseByOrder"]);
            settings.IsRelBySect = Convert.ToBoolean(drow["IsReleaseBySection"]);
            settings.IsPrePickCrit = Convert.ToBoolean(drow["IsPreselectPickingCriteria"]);
            settings.PickDetGridDesignLayout = Convert.ToInt32(drow["PickingDetailGridDesignLayout"]);
            settings.IsPickShowItemQtyAvail = Convert.ToBoolean(drow["IsPickingModuleShowItemQuantityAvailable"]);
            settings.IsPickConfirmDet = Convert.ToBoolean(drow["IsPickingModuleConfirmDetailMode"]);
            settings.PickQtyOverWarn = (SysCommon.ActPickingQtyOverValLevels)Convert.ToInt32(
                drow["IsPickingModuleQuantityOverWarning"]);
            settings.PickQtyUnderWarn = (SysCommon.ActPickingQtyUnderValLevels)Convert.ToInt32(
                drow["IsPickingModuleQuantityUnderWarning"]);
            settings.IsSingleActivityItem = Convert.ToBoolean(drow["IsPickingModuleSingleProduct"]);
            settings.IsPickDisallowManualAddProd = Convert.ToBoolean(drow["IsPickingModuleDisallowManualAddProduct"]);
            settings.IsPickAssocBarCodePwdReq = Convert.ToBoolean(drow["IsPickingModuleAssociateProductBarCodePasswordRequired"]);
            settings.IsPickSingleScanProdMustMatchSelProd = Convert.ToBoolean(
                drow["IsPickingModuleSingleScannedProductMustMatchSelectedProduct"]);
            settings.IsPickQtyUnderShowBaseQtyAvail = Convert.ToBoolean(
                drow["IsPickingModuleQuantityUnderShowBaseQuantityAvailable"]);
            settings.IsPickRefreshOrdAfterRel = Convert.ToBoolean(drow["IsPickingModuleRefreshOrdersAfterRelease"]);
            settings.IsPickPopupCmnt = Convert.ToBoolean(drow["IsPickingModulePopupComments"]);
            settings.IsPickMustReviewItemsBeforeRel = Convert.ToBoolean(drow["IsPickingModuleMustReviewItemsBeforeRelease"]);
            settings.IsPickSingleCatchAsk = Convert.ToBoolean(drow["IsPickingSingleModuleCatchAskEnabled"]);
            settings.IsPickConfirmExit = Convert.ToBoolean(drow["IsPickingModuleConfirmExitEnabled"]);
            settings.IsPickOverrideMsg1 = Convert.ToBoolean(drow["IsPickingModuleOverrideMessage1Enabled"]);
            settings.IsPickHideRouteCol = Convert.ToBoolean(drow["IsPickingModuleHideRouteColumnsEnabled"]);
            settings.IsPickExtraDet = Convert.ToBoolean(drow["IsPickingModuleExtraDetailEnabled"]);
            settings.IsPickCatchAsk = Convert.ToBoolean(drow["IsPickingModuleCatchAskEnabled"]);
            settings.IsFullQuantityEnabled = Convert.ToBoolean(drow["IsPickingModuleFullCaseQuantityEnabled"]);
            settings.IsRepackQuantityEnabled = Convert.ToBoolean(drow["IsPickingModuleRepackQuantityEnabled"]);
            settings.IsLiveQuantityEnabled = Convert.ToBoolean(drow["IsPickingModuleLiveQuantityEnabled"]);
            settings.IsFreshQuantityEnabled = Convert.ToBoolean(drow["IsPickingModuleFreshQuantityEnabled"]);
            settings.IsDryQuantityEnabled = Convert.ToBoolean(drow["IsPickingModuleDryQuantityEnabled"]);
            settings.IsFrozenQuantityEnabled = Convert.ToBoolean(drow["IsPickingModuleFrozenQuantityEnabled"]);
            settings.IsLiveRepackQuantityEnabled = Convert.ToBoolean(drow["IsPickingModuleRepackLiveQuantityEnabled"]);
            settings.IsFreshRepackQuantityEnabled = Convert.ToBoolean(drow["IsPickingModuleRepackFreshQuantityEnabled"]);
            settings.IsDryRepackQuantityEnabled = Convert.ToBoolean(drow["IsPickingModuleRepackDryQuantityEnabled"]);
            settings.IsFrozenRepackQuantityEnabled = Convert.ToBoolean(drow["IsPickingModuleRepackFrozenQuantityEnabled"]);
            settings.IsPalletQuantityEnabled = Convert.ToBoolean(drow["IsPickingModulePalletQuantityEnabled"]);
            settings.IsBundleQuantityEnabled = Convert.ToBoolean(drow["IsPickingModuleBundleQuantityEnabled"]);
            settings.IsPickCreateNewItemNoAsk = Convert.ToBoolean(drow["IsPickingCreateNewItemNoAsk"]);
            settings.PickItemDescOpt = Convert.ToInt32(drow["PickingItemDescriptionOption"]);
            settings.IsPickDisableMakeSkid = Convert.ToBoolean(drow["IsPickingDisableMakeSkid"]);
            settings.IsPickSinglePrintShipLabelFullQuantity = Convert.ToBoolean(drow["IsPickSinglePrintShipLabelFullQuantity"]);
            settings.PickSingleShipLabelStopThresholdQuantity = Convert.ToInt32(drow["PickSingleShipLabelStopThresholdQuantity"]);
            settings.IsPickAddManualDetailPasswordRequired = Convert.ToBoolean(
                drow["IsPickingModuleAddManualDetailPasswordRequired"]);
            settings.IsPickingModuleDisallowLoadingAllOrders = Convert.ToBoolean(drow["IsPickingModuleDisallowLoadingAllOrders"]);
        }

        /// <summary>
        /// The AssignPropertiesFromDataRowAdjustments.
        /// </summary>
        /// <param name="drow">.</param>
        /// <param name="settings">The settings<see cref="SystemSettings"/>.</param>
        public static void AssignPropertiesFromDataRowAdjustments(DataRow drow, ref SystemSettings settings)
        {
            settings.IsAdjustmentNoCatchAskZeroEnabled = Convert.ToBoolean(drow["IsAdjustmentNoCatchAskZeroEnabled"]);
            settings.AllowAddProduct2Adjustments = Convert.ToBoolean(drow["AllowAddProduct2Adjustments"]);
            settings.IsInventoryAdjustmentModule = Convert.ToBoolean(drow["IsInventoryAdjustmentModule"]);
            settings.IsEnabledAdjustmentTypeFresh2Frozen = Convert.ToBoolean(drow["IsEnabledAdjustmentTypeFresh2Frozen"]);
            settings.IsAdjustmentCatchAskEnabled = Convert.ToBoolean(drow["IsAdjustmentModuleCatchAskEnabled"]);
            settings.IsAdjustmentsExtraDetailEnabled = Convert.ToBoolean(drow["IsAdjustmentsModuleExtraDetailEnabled"]);
            settings.IsAdjustmentCreateNewItemNoAsk = Convert.ToBoolean(drow["IsAdjustmentCreateNewItemNoAsk"]);
        }

        /// <summary>
        /// The AssignPropertiesFromDataRowProduction.
        /// </summary>
        /// <param name="drow">.</param>
        /// <param name="settings">The settings<see cref="SystemSettings"/>.</param>
        public static void AssignPropertiesFromDataRowProduction(DataRow drow, ref SystemSettings settings)
        {
            settings.IsTicketMaterialNoCatchAskZeroEnabled = Convert.ToBoolean(drow["IsTicketMaterialNoCatchAskZeroEnabled"]);
            settings.IsTicketByProductNoCatchAskZeroEnabled = Convert.ToBoolean(drow["IsTicketByProductNoCatchAskZeroEnabled"]);
            settings.IsTicketFinishedNoCatchAskZeroEnabled = Convert.ToBoolean(drow["IsTicketFinishedNoCatchAskZeroEnabled"]);
            settings.IsTicketMaterialModule = Convert.ToBoolean(drow["IsProductionTicketIngredientMaterialModule"]);
            settings.IsTicketFinishedGoodModule = Convert.ToBoolean(drow["IsProductionTicketFinishedGoodModule"]);
            settings.IsTicketByProductModule = Convert.ToBoolean(drow["IsProductionTicketByProductModule"]);
            settings.IsFinishedExtraDetailEnabled = Convert.ToBoolean(drow["IsFinishedModuleExtraDetailEnabled"]);
            settings.IsIngredientsExtraDetailEnabled = Convert.ToBoolean(drow["IsIngredientsModuleExtraDetailEnabled"]);
            settings.AllowAddProduct2TicketByProduct = Convert.ToBoolean(drow["AllowAddProduct2ProductionTicketByProduct"]);
            settings.AllowAddProduct2TicketFinished = Convert.ToBoolean(drow["AllowAddProduct2ProductionTicketFinishedGood"]);
            settings.AllowAddProduct2TicketMaterial = Convert.ToBoolean(
                drow["AllowAddProduct2ProductionTicketIngredientMaterial"]);
            settings.IsByProductsExtraDetailEnabled = Convert.ToBoolean(drow["IsByProductsModuleExtraDetailEnabled"]);
            settings.IsTicketFinishedCatchAskEnabled = Convert.ToBoolean(drow["IsProductionTicketFinishedModuleCatchAskEnabled"]);
            settings.IsTicketMaterialCatchAskEnabled = Convert.ToBoolean(drow["IsProductionTicketMaterialModuleCatchAskEnabled"]);
            settings.IsTicketByProductCatchAskEnabled = Convert.ToBoolean(
                drow["IsProductionTicketByProductModuleCatchAskEnabled"]);
            settings.IsTicketMaterialCreateNewItemNoAsk = Convert.ToBoolean(drow["IsTicketMaterialCreateNewItemNoAsk"]);
            settings.IsTicketByProductCreateNewItemNoAsk = Convert.ToBoolean(drow["IsTicketByProductCreateNewItemNoAsk"]);
        }

        /// <summary>
        /// The AssignPropertiesFromDataRowReceiving.
        /// </summary>
        /// <param name="drow">.</param>
        /// <param name="settings">The settings<see cref="SystemSettings"/>.</param>
        public static void AssignPropertiesFromDataRowReceiving(DataRow drow, ref SystemSettings settings)
        {
            settings.IsPutAwaySkidNoCatchAskZeroEnabled = Convert.ToBoolean(drow["IsPutAwaySkidModuleNoCatchAskZeroEnabled"]);
            settings.IsPOWorkSheetNoCatchAskZeroEnabled = Convert.ToBoolean(drow["IsPOWorkSheetNoCatchAskZeroEnabled"]);
            settings.IsReceivingNoCatchAskZeroEnabled = Convert.ToBoolean(drow["IsReceivingNoCatchAskZeroEnabled"]);
            settings.IsReceivingModule = Convert.ToBoolean(drow["IsReceivingModule"]);
            settings.AllowAddProduct2Receiving = Convert.ToBoolean(drow["AllowAddProduct2Receiving"]);
            settings.CanPrintProductLabelReceiving = Convert.ToBoolean(drow["CanPrintProductLabelReceiving"]);
            settings.IsReceivingExtraDetailEnabled = Convert.ToBoolean(drow["IsReceivingModuleExtraDetailEnabled"]);
            settings.IsReceivingCatchAskEnabled = Convert.ToBoolean(drow["IsReceivingModuleCatchAskEnabled"]);
            settings.IsReceivingCreateNewItemNoAsk = Convert.ToBoolean(drow["IsReceivingCreateNewItemNoAsk"]);
            settings.RecvQtyOverWarn = (SysCommon.ActReceivingQtyOverValLevels)Convert.ToInt32(
                drow["IsReceivingModuleQuantityOverWarning"]);
            settings.RecvQtyUnderWarn = (SysCommon.ActReceivingQtyUnderValLevels)Convert.ToInt32(
                drow["IsReceivingModuleQuantityUnderWarning"]);
            settings.IsPOWorkSheetModule = Convert.ToBoolean(drow["IsPurchaseOrderWorkSheetModule"]);
            settings.IsPOWorkSheetCatchAskEnabled = Convert.ToBoolean(drow["IsPurchaseOrderWorkSheetModuleCatchAskEnabled"]);
            settings.IsPOWorkSheetCreateNewItemNoAsk = Convert.ToBoolean(drow["IsPOWorkSheetCreateNewItemNoAsk"]);
            settings.AskRecvPrintAllSkidLabelsUponSave = Convert.ToBoolean(drow["IsReceivingAskPrintAllSkidLabelsUponSave"]);
        }

        /// <summary>
        /// The AssignPropertiesFromDataRowSkids.
        /// </summary>
        /// <param name="drow">.</param>
        /// <param name="settings">The settings<see cref="SystemSettings"/>.</param>
        public static void AssignPropertiesFromDataRowSkids(DataRow drow, ref SystemSettings settings)
        {
            settings.IsSkidTrackingModule = Convert.ToBoolean(drow["IsSkidTrackingModule"]);
            settings.IsAddToSkidModule = Convert.ToBoolean(drow["IsAddToSkidModule"]);
            settings.IsBreakSkidModule = Convert.ToBoolean(drow["IsBreakSkidModule"]);
            settings.IsCreateSkidModule = Convert.ToBoolean(drow["IsCreateSkidModule"]);
            settings.IsRemoveFromSkidModule = Convert.ToBoolean(drow["IsRemoveFromSkidModule"]);
            settings.IsPutAwaySkidCatchAskEnabled = Convert.ToBoolean(drow["IsPutAwaySkidModuleCatchAskEnabled"]);
        }

        /// <summary>
        /// The AssignPropertiesFromDataRowLocationMap.
        /// </summary>
        /// <param name="drow">.</param>
        /// <param name="settings">The settings<see cref="SystemSettings"/>.</param>
        public static void AssignPropertiesFromDataRowLocationMap(DataRow drow, ref SystemSettings settings)
        {
            ////settings.LocMapWarehouseMask = Convert.ToString(drow["LocationMapWarehouseMask"]);
            ////settings.LocMapSectionMask = Convert.ToString(drow["LocationMapSectionMask"]);
            ////settings.LocMapSlotMask = Convert.ToString(drow["LocationMapSlotMask"]);
            ////settings.LocMapSlotCoord1Mask = Convert.ToString(drow["LocationMapSlotCoordinate1Mask"]);
            ////settings.LocMapSlotCoord2Mask = Convert.ToString(drow["LocationMapSlotCoordinate2Mask"]);
            ////settings.LocMapSlotCoord3Mask = Convert.ToString(drow["LocationMapSlotCoordinate3Mask"]);
            ////settings.LocMapSlotCoord1Name = Convert.ToString(drow["LocationMapSlotCoordinate1Name"]);
            ////settings.LocMapSlotCoord2Name = Convert.ToString(drow["LocationMapSlotCoordinate2Name"]);
            ////settings.LocMapSlotCoord3Name = Convert.ToString(drow["LocationMapSlotCoordinate3Name"]);
            settings.IsDefaultLocToProductDefaultLoc = Convert.ToBoolean(drow["IsDefaultLocationToProductDefaultLocation"]);
        }

        /// <summary>
        /// The AssignPropertiesFromDataRowCounts.
        /// </summary>
        /// <param name="drow">.</param>
        /// <param name="settings">The settings<see cref="SystemSettings"/>.</param>
        public static void AssignPropertiesFromDataRowCounts(DataRow drow, ref SystemSettings settings)
        {
            settings.IsPhysicalCountNoCatchAskZeroEnabled = Convert.ToBoolean(drow["IsPhysicalCountNoCatchAskZeroEnabled"]);
            settings.IsCycleCountNoCatchAskZeroEnabled = Convert.ToBoolean(drow["IsCycleCountNoCatchAskZeroEnabled"]);
            settings.IsCycleModule = Convert.ToBoolean(drow["IsCycleModule"]);
            settings.AllowAddProduct2Cycle = Convert.ToBoolean(drow["AllowAddProduct2CycleCounts"]);
            settings.IsCycleExtraDetailEnabled = Convert.ToBoolean(drow["IsCycleCountModuleExtraDetailEnabled"]);
            settings.IsCycleCatchAskEnabled = Convert.ToBoolean(drow["IsCycleCountsModuleCatchAskEnabled"]);
            settings.IsCycleCreateNewItemNoAsk = Convert.ToBoolean(drow["IsCycleCreateNewItemNoAsk"]);
            settings.IsCycleDoNotDefaultProductLocation = Convert.ToBoolean(drow["IsCycleDoNotDefaultProductLocation"]);

            settings.IsPhysicalModule = Convert.ToBoolean(drow["IsPhysicalModule"]);
            settings.AllowAddProduct2Physical = Convert.ToBoolean(drow["AllowAddProduct2PhysicalCounts"]);
            settings.IsPhysicalExtraDetailEnabled = Convert.ToBoolean(drow["IsPhysicalCountModuleExtraDetailEnabled"]);
            settings.IsPhysicalCatchAskEnabled = Convert.ToBoolean(drow["IsPhysicalCountModuleCatchAskEnabled"]);
            settings.IsPhysicalCreateNewItemNoAsk = Convert.ToBoolean(drow["IsPhysicalCreateNewItemNoAsk"]);
            settings.IsPhysicalDoNotDefaultProductLocation = Convert.ToBoolean(drow["IsPhysicalDoNotDefaultProductLocation"]);
        }

        /// <summary>
        /// The AssignPropertiesFromDataRowTransfers.
        /// </summary>
        /// <param name="drow">.</param>
        /// <param name="settings">The settings<see cref="SystemSettings"/>.</param>
        public static void AssignPropertiesFromDataRowTransfers(DataRow drow, ref SystemSettings settings)
        {
            settings.IsTransferNoCatchAskZeroEnabled = Convert.ToBoolean(drow["IsTransferNoCatchAskZeroEnabled"]);
            settings.AllowAddProd2Trans = Convert.ToBoolean(drow["AllowAddProduct2Transfers"]);
            settings.IsTransMod = Convert.ToBoolean(drow["IsWarehouseTransfersModule"]);
            settings.IsTransExtraDetEnabled = Convert.ToBoolean(drow["IsTransfersModuleExtraDetailEnabled"]);
            settings.IsTransCatchAskEnabled = Convert.ToBoolean(drow["IsTransferModuleCatchAskEnabled"]);
            settings.IsTransCreateNewItemNoAsk = Convert.ToBoolean(drow["IsTransferCreateNewItemNoAsk"]);
            ////settings.IsTransConfirmFromDet = Convert.ToBoolean(drow["IsTransferModuleConfirmFromDetailMode"]);
            ////settings.IsTransConfirmToDet = Convert.ToBoolean(drow["IsTransferModuleConfirmToDetailMode"]);
            settings.TransQtyOverWarn = (SysCommon.ActTransfersQtyOverValLevels)Convert.ToInt32(drow["IsTransferModuleQuantityOverWarning"]);
            settings.TransQtyUnderWarn = (SysCommon.ActTransfersQtyUnderValLevels)Convert.ToInt32(drow["IsTransferModuleQuantityUnderWarning"]);
        }

        /// <summary>
        /// The AssignPropertiesFromDataRowReturns.
        /// </summary>
        /// <param name="drow">.</param>
        /// <param name="settings">The settings<see cref="SystemSettings"/>.</param>
        public static void AssignPropertiesFromDataRowReturns(DataRow drow, ref SystemSettings settings)
        {
            settings.IsRetMod = Convert.ToBoolean(drow["IsReturnsModule"]);
            settings.AllowAddProd2Ret = Convert.ToBoolean(drow["AllowAddProduct2Returns"]);
            settings.IsRetConfirmDet = Convert.ToBoolean(drow["IsReturnsModuleConfirmDetailMode"]);
            settings.IsRetExtraDetEnabled = Convert.ToBoolean(drow["IsReturnsModuleExtraDetailEnabled"]);
            settings.IsRetCatchAskEnabled = Convert.ToBoolean(drow["IsReturnsModuleCatchAskEnabled"]);
            settings.IsRetCreateNewItemNoAsk = Convert.ToBoolean(drow["IsReturnsCreateNewItemNoAsk"]);
        }

        /// <summary>
        /// The AssignPropertiesFromDataRowConvertUnits.
        /// </summary>
        /// <param name="drow">.</param>
        /// <param name="settings">The settings<see cref="SystemSettings"/>.</param>
        public static void AssignPropertiesFromDataRowConvertUnits(DataRow drow, ref SystemSettings settings)
        {
            settings.IsConvertUnitModuleNoCatchAskZeroEnabled = Convert.ToBoolean(drow["IsConvertUnitModuleNoCatchAskZeroEnabled"]);
            settings.IsConvertUnitsModule = Convert.ToBoolean(drow["IsConvertUnitsModule"]);
            settings.CanPrintProductLabelConvertUnits = Convert.ToBoolean(drow["CanPrintProductLabelConvertUnits"]);
            settings.IsConvertUnitCatchAskEnabled = Convert.ToBoolean(drow["IsConvertUnitModuleCatchAskEnabled"]);
        }

        /// <summary>
        /// The AssignPropertiesFromDataRowMoves.
        /// </summary>
        /// <param name="drow">.</param>
        /// <param name="settings">The settings<see cref="SystemSettings"/>.</param>
        public static void AssignPropertiesFromDataRowMoves(DataRow drow, ref SystemSettings settings)
        {
            settings.IsMoveInventoryModuleNoCatchAskZeroEnabled = Convert.ToBoolean(drow["IsMoveInventoryModuleNoCatchAskZeroEnabled"]);
            settings.IsMovesModule = Convert.ToBoolean(drow["IsMovesModule"]);
            settings.IsMoveInventoryCatchAskEnabled = Convert.ToBoolean(drow["IsMoveInventoryModuleCatchAskEnabled"]);
            settings.IsPutAwayModule = Convert.ToBoolean(drow["IsPutAwayModule"]);
            settings.IsReplenishmentModule = Convert.ToBoolean(drow["IsReplenishmentModule"]);
        }

        /// <summary>
        /// The AssignPropertiesFromDataRow.
        /// </summary>
        /// <param name="drow">.</param>
        /// <param name="settings">The settings<see cref="SystemSettings"/>.</param>
        public static void AssignPropertiesFromDataRow(DataRow drow, ref SystemSettings settings)
        {
            settings.IsInvStrict = Convert.ToBoolean(drow["IsInventoryStrict"]);
            settings.IsSetCatchWeightZero = Convert.ToBoolean(drow["IsSetCatchWeightZero"]);
            settings.IsSetCatchCountZero = Convert.ToBoolean(drow["IsSetCatchCountZero"]);
            settings.IsIntegerCheckOverride = Convert.ToBoolean(drow["IsIntegerCheckOverride"]);

            settings.IsDisconnectedModeLoadDetail = Convert.ToBoolean(drow["IsDisconnectedModeLoadDetail"]);
            settings.IsDetailEditScreenSetFocus2CatchWeight = Convert.ToBoolean(
                drow["IsDetailEditScreenSetFocus2CatchWeight"]);
            settings.IsEnableDefaultDetailValues = Convert.ToBoolean(drow["EnableDefaultDetailValues"]);
            settings.IsEnableDefaultDetailExtraValues = Convert.ToBoolean(drow["EnableDefaultDetailExtraValues"]);

            settings.GTINCompanyPrefix = Convert.ToString(drow["GTINCompanyPrefix"]);
            settings.ProductUnitBarCodeModifier1Codes = Convert.ToString(drow["ProductUnitBarCodeModifier1Codes"]);
            settings.IsProductUnitBarCodeModifier1 = Convert.ToBoolean(drow["IsProductUnitBarCodeModifier1"]);

            settings.ProdWeightUnitTolerancePercent = Convert.ToDecimal(drow["ProdWeightUnitTolerancePercent"]);
            settings.DetailDateMinMaxValidYearRange = Convert.ToInt32(drow["DetailDateMinMaxValidYearRange"]);

            AssignPropertiesFromDataRowLocationMap(drow, ref settings);
            AssignPropertiesFromDataRowPicking(drow, ref settings);
            AssignPropertiesFromDataRowAdjustments(drow, ref settings);
            AssignPropertiesFromDataRowProduction(drow, ref settings);
            AssignPropertiesFromDataRowReceiving(drow, ref settings);
            AssignPropertiesFromDataRowSkids(drow, ref settings);
            AssignPropertiesFromDataRowCounts(drow, ref settings);
            AssignPropertiesFromDataRowTransfers(drow, ref settings);
            AssignPropertiesFromDataRowReturns(drow, ref settings);
            AssignPropertiesFromDataRowConvertUnits(drow, ref settings);
            AssignPropertiesFromDataRowMoves(drow, ref settings);
        }

        /// <summary>
        /// The AssignPropertiesFromDataRow2.
        /// </summary>
        /// <param name="drow">The drow<see cref="DataRow"/>.</param>
        /// <param name="settings">The settings<see cref="SystemSettings"/>.</param>
        public static void AssignPropertiesFromDataRow2(DataRow drow, ref SystemSettings settings)
        {
            settings.DoesSelectingADifferentProductClearSkid = Convert.ToBoolean(drow["IsClearSkidUponChangeLineItemProductEnabled"]);
            settings.CanPrintProductLabelTransfers = Convert.ToBoolean(drow["CanPrintProductLabelTransfers"]);
            settings.IsProductLabelCatchAsk = Convert.ToBoolean(drow["IsProductLabelCatchAsk"]);
            settings.EnableAndroidSingleActivityItemMode = Convert.ToBoolean(drow["EnableAndroidSingleActivityItemMode"]);

            // WO 15547 conflicting with old WO 13770. Huer wants section slot filter, Sysco doesn't
            if (drow.Table.Columns.Contains("InventoryInquirySearchEntireWarehouse"))
            {
                settings.InventoryInquirySearchEntireWarehouse =
                    Convert.ToBoolean(drow["InventoryInquirySearchEntireWarehouse"]);
            }
            else
            {
                settings.InventoryInquirySearchEntireWarehouse = false;
            }

            if (drow.Table.Columns.Contains("IsDisableMoveAllSlotQuantityOption"))
            {
                settings.IsDisableMoveAllSlotQuantityOption = Convert.ToBoolean(drow["IsDisableMoveAllSlotQuantityOption"]);
            }
            else
            {
                settings.IsDisableMoveAllSlotQuantityOption = false;
            }
        }

        /// <summary>
        /// The SaveData.
        /// </summary>
        /// <param name="sysSettings">The sysSettings<see cref="SystemSettings"/>.</param>
        /// <returns>.</returns>
        public static bool SaveData(ref SystemSettings sysSettings)
        {
            var serializer = new XmlSerializer(typeof(SystemSettings));
            bool saveComplete;
            try
            {
                // use reflection to get the current assembly path for the database
                var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var fullPathToMyFile = Path.Combine(appDir, SystemSettingsTasks.FileName);

                // Create a FileStream to write with.
                using (var writer = new FileStream(fullPathToMyFile, FileMode.Create))
                {
                    // Serialize the object, and close the TextWriter
                    serializer.Serialize(writer, sysSettings);
                    writer.Close();
                }

                saveComplete = true;
            }
            catch
            {
                saveComplete = false;
            }

            return saveComplete;
        }

        /// <summary>
        /// The RequestSystemSettings.
        /// </summary>
        /// <param name="settings">The settings<see cref="SystemSettings"/>.</param>
        public static void RequestSystemSettings(ref SystemSettings settings)
        {
            var msgEventArg = new SysSettingsEventArgs();

            try
            {
                var responseData = string.Empty;

                if (OnSysSettingsStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Seeding system settings.";
                    OnSysSettingsStatusOut(settings, msgEventArg);
                }

                // First request
                responseData = SendFirstRequest();
                if (responseData.Contains("<message>ERROR</message>"))
                {
                    var docError = new XmlDocument();
                    docError.LoadXml(responseData);
                    throw new Exception(docError.DocumentElement!["detail"] !.InnerText);
                }

                Save(responseData, 1, ref settings);

                // Second request
                responseData = SendSecondRequest();
                if (responseData.Contains("<message>ERROR</message>"))
                {
                    var docError = new XmlDocument();
                    docError.LoadXml(responseData);
                    throw new Exception(docError.DocumentElement!["detail"] !.InnerText);
                }

                Save(responseData, 2, ref settings);

                if (OnSysSettingsStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Seeding system settings complete.";
                    OnSysSettingsStatusOut(settings, msgEventArg);
                }
            }
            catch (Exception ex)
            {
                if (OnSysSettingsStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Seeding system settings failed.";
                    OnSysSettingsStatusOut(settings, msgEventArg);
                }

                throw new Exception("Seeding system settings failed.", ex);
            }
        }

        /// <summary>
        /// The SendFirstRequest.
        /// </summary>
        /// <returns>.</returns>
        private static string SendFirstRequest()
        {
            return SysCommon.SubmitComm(SystemSettingsTasks.CreateFirstRequest());
        }

        /// <summary>
        /// The SendSecondRequest.
        /// </summary>
        /// <returns>.</returns>
        private static string SendSecondRequest()
        {
            return SysCommon.SubmitComm(SystemSettingsTasks.CreateSecondRequest());
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData">.</param>
        /// <param name="sysSettingsNumber">The sysSettingsNumber<see cref="int"/>.</param>
        /// <param name="settings">The settings<see cref="SystemSettings"/>.</param>
        private static void Save(string responseData, int sysSettingsNumber, ref SystemSettings settings)
        {
            var msgEventArg = new SysSettingsEventArgs();
            using var ds = new DataSet();
            _ = ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (ds.Tables.Count != 0)
            {
                foreach (DataRow drow in ds.Tables["tSystemSettings"].Rows)
                {
                    if (sysSettingsNumber == 1)
                    {
                        SystemSettingsTasks.AssignPropertiesFromDataRow(drow, ref settings);

                        // Set aggregate flags that are dependent upon a set of other flags
                        settings.IsMoveSkidModule = settings.IsAddToSkidModule | settings.IsBreakSkidModule |
                            settings.IsCreateSkidModule | settings.IsRemoveFromSkidModule;
                    }
                    else if (sysSettingsNumber == 2)
                    {
                        AssignPropertiesFromDataRow2(drow, ref settings);
                    }
                }
            }

            if (OnSysSettingsStatusOut != null)
            {
                msgEventArg.StatusOutMsg = $"Saving system settings {sysSettingsNumber} complete.";
                OnSysSettingsStatusOut(settings, msgEventArg);
            }
        }
    }
}
