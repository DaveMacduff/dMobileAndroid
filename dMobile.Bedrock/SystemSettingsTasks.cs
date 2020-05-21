using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Data;

namespace dMobile.Bedrock
{
    public static class SystemSettingsTasks
    {
        private static string fileName = "dMobileSystemSettings.xml";
        private static SysSocket socket = new SysSocket();
        private static CommunicationSetup commSetup = new CommunicationSetup();

        public delegate void OnSysSettingsStatusOutEventHandler(ISysSettingsEventPublisher sysSettingsEventPublisher, SysSettingsEventArgs sysSettingsEventArgs);
        public delegate void OnSysSettingsProgressOutEventHandler(ISysSettingsEventPublisher sysSettingsEventPublisher, SysSettingsEventArgs sysSettingsEventArgs);
        public static event OnSysSettingsStatusOutEventHandler OnSysSettingsStatusOut;
        public static event OnSysSettingsProgressOutEventHandler OnSysSettingsProgressOut;

        public static CommunicationSetup CommSetup
        {
            get
            {
                return commSetup;
            }

            set
            {
                commSetup = value;
            }
        }

        public static SysSocket Socket
        {
            get
            {
                return socket;
            }

            set
            {
                socket = value;
            }
        }

        public static string FileName
        {
            get
            {
                return fileName;
            }

            set
            {
                fileName = value;
            }
        }

        public static void Init()
        {
            CommSetup.LoadData();
            Socket.BasePort = CommSetup.iBaseSocketPort;
            Socket.DefSocketPort = CommSetup.iDefaultSocketPort;
            Socket.MaxPorts = CommSetup.iMaxSocketPorts;
            Socket.ConnTotalRetries = CommSetup.iConnectionAttempts;
            Socket.SendTotalRetries = CommSetup.iSendAttempts;
            Socket.RecvTotalRetries = CommSetup.iReceiveAttempts;
            Socket.SendTimeOutMilliseconds = CommSetup.iSendTimeOutSeconds * 1000;
            Socket.ReceiveTimeOutMilliseconds = CommSetup.iReceiveTimeOutSeconds * 1000;
            Socket.RecvBuffSize = CommSetup.iiReceiveBufferSize;
            Socket.ServerIP = CommSetup.iHostIPAddressComponent1.ToString() + "."
                + CommSetup.iHostIPAddressComponent2.ToString() + "."
                + CommSetup.iHostIPAddressComponent3.ToString() + "."
                + CommSetup.iHostIPAddressComponent4.ToString();
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static void LoadData(ref SystemSettings settings)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SystemSettings));

            string appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string fullPathToMyFile = Path.Combine(appDir, SystemSettingsTasks.FileName);

            if (!File.Exists(fullPathToMyFile))
            {
                SaveData(ref settings);
            }

            // Create a FileStream to read with.
            using (FileStream reader = new FileStream(fullPathToMyFile, FileMode.Open))
            {
                // Deserialize the stream to the object, and close the TextReader
                settings = (SystemSettings)serializer.Deserialize(reader);
                reader.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string CreateFirstRequest()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            string deviceID = SysDeviceID.GetDeviceID();
            string revision = new DateTime(1900, 1, 1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            ContactLogin login = new ContactLogin().LoadData();
            string loginID = login.LoginID.ToString();

            XDocument request = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"), string.Empty,
                  new XElement("imclient",
                    new XElement("command",
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
        /// We exceeded the limit of columns in Vfp.
        /// We decided the simplest and easiest way for now was to make
        /// a second request. June 24, 2016
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string CreateSecondRequest()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            string deviceID = SysDeviceID.GetDeviceID();
            string revision = new DateTime(1900, 1, 1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            ContactLogin login = new ContactLogin().LoadData();
            string loginID = login.LoginID.ToString();

            XDocument request = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"), string.Empty,
                  new XElement("imclient",
                    new XElement("command",
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
        /// 
        /// </summary>
        /// <param name="drow"></param>
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
            settings.IsPickSingleProd = Convert.ToBoolean(drow["IsPickingModuleSingleProduct"]);
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
        /// 
        /// </summary>
        /// <param name="drow"></param>
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
        /// 
        /// </summary>
        /// <param name="drow"></param>
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
        /// 
        /// </summary>
        /// <param name="drow"></param>
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
        /// 
        /// </summary>
        /// <param name="drow"></param>
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
        /// 
        /// </summary>
        /// <param name="drow"></param>
        public static void AssignPropertiesFromDataRowLocationMap(DataRow drow, ref SystemSettings settings)
        {
            //settings.LocMapWarehouseMask = Convert.ToString(drow["LocationMapWarehouseMask"]);
            //settings.LocMapSectionMask = Convert.ToString(drow["LocationMapSectionMask"]);
            //settings.LocMapSlotMask = Convert.ToString(drow["LocationMapSlotMask"]);
            //settings.LocMapSlotCoord1Mask = Convert.ToString(drow["LocationMapSlotCoordinate1Mask"]);
            //settings.LocMapSlotCoord2Mask = Convert.ToString(drow["LocationMapSlotCoordinate2Mask"]);
            //settings.LocMapSlotCoord3Mask = Convert.ToString(drow["LocationMapSlotCoordinate3Mask"]);
            //settings.LocMapSlotCoord1Name = Convert.ToString(drow["LocationMapSlotCoordinate1Name"]);
            //settings.LocMapSlotCoord2Name = Convert.ToString(drow["LocationMapSlotCoordinate2Name"]);
            //settings.LocMapSlotCoord3Name = Convert.ToString(drow["LocationMapSlotCoordinate3Name"]);
            settings.IsDefaultLocToProductDefaultLoc = Convert.ToBoolean(drow["IsDefaultLocationToProductDefaultLocation"]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drow"></param>
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
        /// 
        /// </summary>
        /// <param name="drow"></param>
        public static void AssignPropertiesFromDataRowTransfers(DataRow drow, ref SystemSettings settings)
        {
            settings.IsTransferNoCatchAskZeroEnabled = Convert.ToBoolean(drow["IsTransferNoCatchAskZeroEnabled"]);
            settings.AllowAddProd2Trans = Convert.ToBoolean(drow["AllowAddProduct2Transfers"]);
            settings.IsTransMod = Convert.ToBoolean(drow["IsWarehouseTransfersModule"]);
            settings.IsTransExtraDetEnabled = Convert.ToBoolean(drow["IsTransfersModuleExtraDetailEnabled"]);
            settings.IsTransCatchAskEnabled = Convert.ToBoolean(drow["IsTransferModuleCatchAskEnabled"]);
            settings.IsTransCreateNewItemNoAsk = Convert.ToBoolean(drow["IsTransferCreateNewItemNoAsk"]);
            //settings.IsTransConfirmFromDet = Convert.ToBoolean(drow["IsTransferModuleConfirmFromDetailMode"]);
            ////settings.IsTransConfirmToDet = Convert.ToBoolean(drow["IsTransferModuleConfirmToDetailMode"]);
            settings.TransQtyOverWarn = (SysCommon.ActTransfersQtyOverValLevels)Convert.ToInt32(drow["IsTransferModuleQuantityOverWarning"]);
            settings.TransQtyUnderWarn = (SysCommon.ActTransfersQtyUnderValLevels)Convert.ToInt32(drow["IsTransferModuleQuantityUnderWarning"]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drow"></param>
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
        /// 
        /// </summary>
        /// <param name="drow"></param>
        public static void AssignPropertiesFromDataRowConvertUnits(DataRow drow, ref SystemSettings settings)
        {
            settings.IsConvertUnitModuleNoCatchAskZeroEnabled = Convert.ToBoolean(drow["IsConvertUnitModuleNoCatchAskZeroEnabled"]);
            settings.IsConvertUnitsModule = Convert.ToBoolean(drow["IsConvertUnitsModule"]);
            settings.CanPrintProductLabelConvertUnits = Convert.ToBoolean(drow["CanPrintProductLabelConvertUnits"]);
            settings.IsConvertUnitCatchAskEnabled = Convert.ToBoolean(drow["IsConvertUnitModuleCatchAskEnabled"]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drow"></param>
        public static void AssignPropertiesFromDataRowMoves(DataRow drow, ref SystemSettings settings)
        {
            settings.IsMoveInventoryModuleNoCatchAskZeroEnabled = Convert.ToBoolean(drow["IsMoveInventoryModuleNoCatchAskZeroEnabled"]);
            settings.IsMovesModule = Convert.ToBoolean(drow["IsMovesModule"]);
            settings.IsMoveInventoryCatchAskEnabled = Convert.ToBoolean(drow["IsMoveInventoryModuleCatchAskEnabled"]);
            settings.IsPutAwayModule = Convert.ToBoolean(drow["IsPutAwayModule"]);
            settings.IsReplenishmentModule = Convert.ToBoolean(drow["IsReplenishmentModule"]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drow"></param>
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

        public static void AssignPropertiesFromDataRow2(DataRow drow, ref SystemSettings settings)
        {
            settings.DoesSelectingADifferentProductClearSkid = Convert.ToBoolean(drow["IsClearSkidUponChangeLineItemProductEnabled"]);
            settings.CanPrintProductLabelTransfers = Convert.ToBoolean(drow["CanPrintProductLabelTransfers"]);
            settings.IsProductLabelCatchAsk = Convert.ToBoolean(drow["IsProductLabelCatchAsk"]);

            // WO 15547 conflicting with old WO 13770. Huer wants section slot filter, Sysco doesn't
            if (drow.Table.Columns.Contains("InventoryInquirySearchEntireWarehouse"))
                settings.InventoryInquirySearchEntireWarehouse = Convert.ToBoolean(drow["InventoryInquirySearchEntireWarehouse"]);
            else
                settings.InventoryInquirySearchEntireWarehouse = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oSystemSettings"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool SaveData(ref SystemSettings sysSettings)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SystemSettings));
            bool saveComplete = false;

            try
            {
                // use relection to get the current assembly path for the database
                string appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string fullPathToMyFile = Path.Combine(appDir, SystemSettingsTasks.FileName);

                // Create a FileStream to write with.
                using (FileStream writer = new FileStream(fullPathToMyFile, FileMode.Create))
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
        /// 
        /// </summary>
        /// <remarks></remarks>
        public static void RequestSystemSettings(ref SystemSettings settings)
        {
            SysSettingsEventArgs msgEventArg = new SysSettingsEventArgs();

            try
            {
                string reqData = string.Empty;

                if (OnSysSettingsStatusOut != null)
                {

                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Seeding system settings.";
                    OnSysSettingsStatusOut(settings, msgEventArg);
                }

                // First request
                reqData = SendFirstRequest();
                
                if (reqData.Length > 0)
                {
                    // check if result is an error message
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument docError = new XmlDocument();
                        docError.LoadXml(reqData);
                        throw new Exception(docError.DocumentElement["detail"].InnerText);
                    }

                    Save(reqData, 1, ref settings);
                }

                // Second request
                reqData = SendSecondRequest();

                if (reqData.Length > 0)
                {
                    // check if result is an error message
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument docError = new XmlDocument();
                        docError.LoadXml(reqData);
                        throw new Exception(docError.DocumentElement["detail"].InnerText);
                    }

                    Save(reqData, 2, ref settings);
                }

                if (OnSysSettingsStatusOut != null)
                {
                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Seeding system settings complete.";
                    OnSysSettingsStatusOut(settings, msgEventArg);
                }
            }
            catch (Exception ex)
            {
                if (OnSysSettingsStatusOut != null)
                {
                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Seeding system settings failed.";
                    OnSysSettingsStatusOut(settings, msgEventArg);
                }

                // TODO: Resource strings
                throw new Exception("Seeding system settings failed.", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private static string SendFirstRequest()
        {
            return SysCommon.SubmitComm(SystemSettingsTasks.CreateFirstRequest());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private static string SendSecondRequest()
        {
            return SysCommon.SubmitComm(SystemSettingsTasks.CreateSecondRequest());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqData"></param>
        private static void Save(string reqData, int sysSettingsNumber, ref SystemSettings settings)
        {
            SysSettingsEventArgs msgEventArg = new SysSettingsEventArgs();

            try
            {
                using (DataSet ds = new DataSet())
                {
                    if (OnSysSettingsStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = "Reading received data.";
                        OnSysSettingsStatusOut(settings, msgEventArg);
                    }

                    ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                    if (OnSysSettingsStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = string.Format("Saving system settings {0}.", sysSettingsNumber);
                        OnSysSettingsStatusOut(settings, msgEventArg);
                    }

                    if (ds.Tables.Count != 0)
                    {
                        int progMax = ds.Tables["tSystemSettings"].Rows.Count;

                        if (OnSysSettingsStatusOut != null)
                        {
                            // TODO: Resource strings
                            msgEventArg.StatusOutMsg = string.Format("Records {0}.", progMax);
                            OnSysSettingsStatusOut(settings, msgEventArg);
                        }

                        int progValue = 0;

                        if (OnSysSettingsProgressOut != null)
                        {
                            msgEventArg.CurrentValue = 0;
                            msgEventArg.MinValue = 0;
                            msgEventArg.MaxValue = progMax;
                            OnSysSettingsProgressOut(settings, msgEventArg);
                        }

                        foreach (DataRow drow in ds.Tables["tSystemSettings"].Rows)
                        {
                            if (sysSettingsNumber == 1)
                            {
                                SystemSettingsTasks.AssignPropertiesFromDataRow(drow, ref settings);

                                // Set agregate flags that are dependant upon a set of other flags
                                settings.IsMoveSkidModule = settings.IsAddToSkidModule | settings.IsBreakSkidModule | settings.IsCreateSkidModule | settings.IsRemoveFromSkidModule;
                            }
                            else if (sysSettingsNumber == 2)
                            {
                                AssignPropertiesFromDataRow2(drow, ref settings);
                            }
                            
                            progValue += 1;

                            if (OnSysSettingsProgressOut != null)
                            {
                                msgEventArg.CurrentValue = progValue;
                                msgEventArg.MinValue = 0;
                                msgEventArg.MaxValue = progMax;
                                OnSysSettingsProgressOut(settings, msgEventArg);
                            }
                        }

                        if (OnSysSettingsProgressOut != null)
                        {
                            msgEventArg.CurrentValue = progMax;
                            msgEventArg.MinValue = 0;
                            msgEventArg.MaxValue = progMax;
                            OnSysSettingsProgressOut(settings, msgEventArg);
                        }
                    }

                    if (OnSysSettingsStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = string.Format("Saving system settings {0} complete.", sysSettingsNumber);
                        OnSysSettingsStatusOut(settings, msgEventArg);
                    }
                }
            }
            catch (Exception ex)
            {
                if (OnSysSettingsStatusOut != null)
                {
                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Saving system settings failed.";
                    OnSysSettingsStatusOut(settings, msgEventArg);
                }

                // TODO: Resource strings
                throw new Exception("Saving system settings failed.", ex);
            }
        }
    }
}
