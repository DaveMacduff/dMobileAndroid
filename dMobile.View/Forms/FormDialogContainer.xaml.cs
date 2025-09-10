// <copyright file="FormDialogContainer.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Forms
{
    using System;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Activity;
    using Bedrock;
    using Controller.Activity;
    using Controller.Activity_Detail;
    using Controller.Activity_Item;
    using CustomPages.Modal;
    using Options;
    using Views;
    using Xamarin.Forms.Xaml;

    /// <summary>
    ///     Defines the <see cref="FormDialogContainer" /> main form dialog container.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormDialogContainer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FormDialogContainer" /> class.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes" /> Inner model.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes" /> Activity subtype.</param>
        /// <param name="activityType">The activityType <see cref="SysShared.ActivityType" />.</param>
        public FormDialogContainer(
            EDialogTypes modelType,
            SysCommon.ActSubTypes actSubType,
            SysShared.ActivityType activityType = SysShared.ActivityType.Header)
        {
            this.InitializeComponent();
            var model = modelType switch
            {
                EDialogTypes.RefreshDataStatus => new RefreshDatabaseStatus(actSubType),
                EDialogTypes.ActivityInventoryInquiry => new ActivityInventoryInquiry(),
                EDialogTypes.ActivityDeboxLabelCount => new ActivityDeboxLabelCount(),
                EDialogTypes.ActivityConvertUnitEditCatch => throw new NotImplementedException(),
                EDialogTypes.ActivityConvertUnitDetail => throw new NotImplementedException(),
                _ => this.GetModel(modelType, actSubType, activityType)
            };

            model ??= this.GetModel(modelType);
            this.DisplayControl = model;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FormDialogContainer" /> class.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes" /> Inner model.</param>
        /// <param name="labelQtyEdit"><see cref="SLabelQuantity" /> Label quantity.</param>
        /// <param name="activity"><see cref="Activity" /> Activity header.</param>
        /// <param name="selectMode">The selectMode<see cref="ShippingLabelQuantitySelectionMode" />.</param>
        public FormDialogContainer(
            EDialogTypes modelType,
            SLabelQuantity labelQtyEdit,
            Activity activity,
            ShippingLabelQuantitySelectionMode selectMode)
        {
            this.InitializeComponent();

            var msg = $"Model object for {SysCommon.SourceEnum2String(modelType)}.";
            this.DisplayControl = modelType switch
            {
                EDialogTypes.ActivityShippingLabelEdit => new ActivityShippingLabelEdit(
                    SysCommon.ActSubTypes.Picking, // Override the subtype to Picking
                    labelQtyEdit,
                    activity,
                    selectMode),
                _ => throw new NotSupportedException(msg)
            };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FormDialogContainer" /> class.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes" /> Inner model.</param>
        /// <param name="activity"><see cref="Activity" /> Activity header.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes" /> Activity subtype.</param>
        /// <param name="activityType"><see cref="SysShared.ActivityType"/> Activity type.</param>
        public FormDialogContainer(
            EDialogTypes modelType,
            Activity activity,
            SysCommon.ActSubTypes actSubType,
            SysShared.ActivityType activityType = SysShared.ActivityType.Header)
        {
            this.InitializeComponent();
            this.DisplayControl = modelType switch
            {
                EDialogTypes.RefreshDataStatus => new RefreshDatabaseStatus(actSubType),
                EDialogTypes.ActivityInventoryInquiry => new ActivityInventoryInquiry(),
                _ => this.GetModel(modelType, activity, actSubType, activityType)
            };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FormDialogContainer" /> class.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes" /> Inner model.</param>
        /// <param name="actItem"><see cref="ActItem" /> Activity item object.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes" /> Activity subtype.</param>
        /// <param name="callingActSubType">The callingActSubType<see cref="SysCommon.ActSubTypes" />.</param>
        /// <param name="activityType"><see cref="SysShared.ActivityType"/> Activity type.</param>
        public FormDialogContainer(
            EDialogTypes modelType,
            ActItem actItem,
            SysCommon.ActSubTypes actSubType,
            SysCommon.ActSubTypes callingActSubType = SysCommon.ActSubTypes.SkidInfo,
            SysShared.ActivityType activityType = SysShared.ActivityType.Header)
        {
            this.InitializeComponent();
            this.DisplayControl = modelType switch
            {
                EDialogTypes.RefreshDataStatus => new RefreshDatabaseStatus(actSubType),
                EDialogTypes.ActivityInventoryInquiry => new ActivityInventoryInquiry(actItem.ProdInfo),
                EDialogTypes.ActivityInventoryInquirySummary => new ActivityItemInventoryInquirySummary(actItem),
                _ => this.GetModel(modelType, actItem, actSubType, callingActSubType, activityType)
            };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FormDialogContainer" /> class.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes" /> Inner model.</param>
        /// <param name="actDetail"><see cref="ActDetail" /> Activity detail object.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes" /> Activity subtype.</param>
        /// <param name="activityType"><see cref="SysShared.ActivityType"/> Activity type.</param>
        public FormDialogContainer(
            EDialogTypes modelType,
            ActDetail actDetail,
            SysCommon.ActSubTypes actSubType,
            SysShared.ActivityType activityType = SysShared.ActivityType.Header)
        {
            this.InitializeComponent();
            this.DisplayControl = modelType switch
            {
                EDialogTypes.RefreshDataStatus => new RefreshDatabaseStatus(actSubType),
                EDialogTypes.ActivityInventoryInquiry => new ActivityInventoryInquiry(actDetail.ActItem.ProdInfo),
                _ => this.GetModel(modelType, actDetail, actSubType, activityType)
            };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FormDialogContainer" /> class.
        /// </summary>
        /// <param name="modelType">The modelType<see cref="EDialogTypes" />.</param>
        /// <param name="defaultWhsLocMapRefno">The defaultWhsLocMapRefno<see cref="int" />.</param>
        /// <param name="lockWhsSelection">The lockWhsSelection<see cref="bool" />.</param>
        public FormDialogContainer(EDialogTypes modelType, int defaultWhsLocMapRefno, bool lockWhsSelection)
        {
            this.InitializeComponent();

            var msg = $"Model object for {SysCommon.SourceEnum2String(modelType)}.";
            this.DisplayControl = modelType switch
            {
                EDialogTypes.SelectLocationMap => new SetLocationMap(defaultWhsLocMapRefno, lockWhsSelection),
                _ => throw new NotSupportedException(msg)
            };
        }

        /// <summary>
        ///     Gets or sets the display control.
        /// </summary>
        public IDMobileModal DisplayControl { get; set; }

        /// <summary>
        ///     Get the display control XML return data.
        /// </summary>
        /// <returns><see cref="XDocument" /> XML data.</returns>
        public async Task<XDocument> GetXml()
        {
            return await ((IAppArea)this.DisplayControl).GetXml();
        }

        /// <summary>
        ///     Get the model base on the dialog type.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes" /> Inner model.</param>
        /// <param name="activity"><see cref="Activity" /> Activity header.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes" /> Activity subtype.</param>
        /// <param name="activityType"><see cref="activityType"/> Activity type.</param>
        /// <returns><see cref="IDMobileModal" /> Model.</returns>
        private IDMobileModal GetModel(
            EDialogTypes modelType,
            Activity activity,
            SysCommon.ActSubTypes actSubType,
            SysShared.ActivityType activityType = SysShared.ActivityType.Header)
        {
            return modelType switch
            {
                EDialogTypes.ActivityItem => new ActivityItem(activity, actSubType),
                EDialogTypes.SelectActivityItem => new SelectActivityItem(activity, actSubType),
                EDialogTypes.ActivityItemSingle => new ActivityItemSingle(activity, actSubType),
                EDialogTypes.SkidInfo => new ActivityHdrSkidInfo(actSubType, activity, activityType),
                _ => null
            };
        }

        /// <summary>
        ///     Get the model base on the dialog type.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes" /> Inner model.</param>
        /// <param name="actDetail">.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes" /> Activity subtype.</param>
        /// <param name="activityType"><see cref="SysShared.ActivityType"/> Activity type.</param>
        /// <returns><see cref="IDMobileModal" /> Model.</returns>
        private IDMobileModal GetModel(
            EDialogTypes modelType,
            ActDetail actDetail,
            SysCommon.ActSubTypes actSubType,
            SysShared.ActivityType activityType)
        {
            return modelType switch
            {
                EDialogTypes.ActivityProductLabelQuantity => new ActivityProductLabelQuantity(actDetail.Quantity),
                EDialogTypes.ActivityDetailEdit => new ActivityDetailEdit(actDetail, actSubType),
                EDialogTypes.ActivityProductLabel => new ActivityProductLabel(actDetail, actSubType),
                EDialogTypes.ActivityPurchaseOrderWorkSheetEdit => new ActivityItemPurchaseOrderWorkSheetEdit(
                    actDetail,
                    SysCommon.ActSubTypes.PurchaseOrderWorkSheet), // Override the subtype to PurchaseOrderWorkSheet
                EDialogTypes.ActivityConvertUnitEdit => new ActivityConvertUnitEdit(actDetail, SysCommon.ActSubTypes.ConvertUnit), // Override the subtype to ConvertUnit
                EDialogTypes.ActivityConvertUnitEditCatch => new ActivityConvertUnitEditCatch(actDetail, SysCommon.ActSubTypes.ConvertUnit), // Override the subtype to ConvertUnit
                EDialogTypes.ActivityInventoryInquiryDetail => new ActivityDetailInventoryInquiry(actDetail, actSubType),
                EDialogTypes.ActivityInventoryInquirySingleItem => new ActivityItemSingleInventoryInquiry(actDetail, actSubType),
                EDialogTypes.ActivityConvertProductDetail => new ActivityConvertProductDetail(actDetail, actSubType),
                EDialogTypes.ActivityRemoveFromSkid => new ActivityRemoveFromSkid(actDetail, actSubType),
                _ => null
            };
        }

        /// <summary>
        ///     Get the model base on the dialog type.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes" /> Inner model.</param>
        /// <param name="actItem">.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes" /> Activity sub type.</param>
        /// <param name="callingActSubType">The callingActSubType<see cref="SysCommon.ActSubTypes" />.</param>
        /// <param name="activityType"><see cref="SysShared.ActivityType"/> Activity type.</param>
        /// <returns><see cref="IDMobileModal" /> Model.</returns>
        private IDMobileModal GetModel(
            EDialogTypes modelType,
            ActItem actItem,
            SysCommon.ActSubTypes actSubType,
            SysCommon.ActSubTypes callingActSubType = SysCommon.ActSubTypes.SkidInfo,
            SysShared.ActivityType activityType = SysShared.ActivityType.Header)
        {
            return modelType switch
            {
                EDialogTypes.ActivityDetail => new ActivityDetail(actItem, actSubType),
                EDialogTypes.SelectProductUnitFilterToProduct => new SelectProductUnit(
                    actItem.ProdInfo.Refno,
                    actItem.ProdInfo.UnitRefno,
                    actSubType),
                EDialogTypes.SkidInfo => new ActivityHdrSkidInfo(actSubType, actItem, activityType),
                EDialogTypes.ActivityDetailSkid => new ActivityDetailSkid(actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityInventoryInquiryItem => new ActivityItemInventoryInquiry(actItem, actSubType),
                EDialogTypes.ActivityConvertUnitDetail => new ActivityConvertUnitDetail(actItem, actSubType),
                _ => null
            };
        }

        /// <summary>
        ///     Get the model base on the dialog type.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes" /> Inner model.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes" /> Activity subtype.</param>
        /// <param name="activityType"><see cref="SysShared.ActivityType"/> Activity type.</param>
        /// <returns><see cref="IDMobileModal" /> Model.</returns>
        private IDMobileModal GetModel(
            EDialogTypes modelType,
            SysCommon.ActSubTypes actSubType,
            SysShared.ActivityType activityType = SysShared.ActivityType.Header)
        {
            return modelType switch
            {
                EDialogTypes.SelectProductUnit => new SelectProductUnit(actSubType),
                _ => null
            };
        }

        /// <summary>
        ///     Get the model base on the dialog type.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes" /> Inner model.</param>
        /// <param name="activityType"><see cref="SysShared.ActivityType"/> Activity type.</param>
        /// <returns><see cref="IDMobileModal" /> Model.</returns>
        private IDMobileModal GetModel(
            EDialogTypes modelType,
            SysShared.ActivityType activityType = SysShared.ActivityType.Header)
        {
            return modelType switch
            {
                EDialogTypes.SelectProduct => new SelectProduct(),
                EDialogTypes.SelectLocationMap => new SetLocationMap(),
                EDialogTypes.ActivityHdrPickingFilter => new ucActivityHdrPickingFilter(),
                EDialogTypes.ActivityHdrReceivingFilter => new ActivityHdrReceivingFilter(),
                EDialogTypes.ActivityHdrPhysicalCountFilter => new ActivityHdrPhysicalCountFilter(),
                EDialogTypes.ActivityHdrCycleCountFilter => new ucActivityHdrCycleCountFilter(),
                EDialogTypes.ActivityHdrReturnsFilter => new ActivityHdrReturnsFilter(),
                EDialogTypes.ActivityHdrTransferFilter => new ActivityHdrTransferFilter(),
                EDialogTypes.SubmitBatchFilter => new SubmitBatchFilter(),
                EDialogTypes.ActivityAdjustmentFilter => new ucActivityHdrAdjustmentFilter(),
                EDialogTypes.ActivityProductionTicketFinishedFilter => new ActivityHdrProductionTicketFilter(),
                EDialogTypes.ActivityProductionTicketMaterialFilter => new ActivityHdrProductionTicketFilter(),
                EDialogTypes.ActivityProductionTicketByProductFilter => new ActivityHdrProductionTicketFilter(),
                EDialogTypes.ActivityPickingSelectCriteria => new ActivityHdrPickingSelectCriteria(),
                EDialogTypes.SkidInfo => new ActivityHdrSkidInfo(activityType),
                EDialogTypes.ActivityCutSheetFinishedFilter => new ActivityHdrCutSheetFilter(),
                EDialogTypes.ActivityCutSheetMaterialFilter => new ActivityHdrCutSheetFilter(),
                EDialogTypes.ActivityYieldSheetFinishedFilter => new ActivityHdrYieldSheetFilter(),
                EDialogTypes.ActivityYieldSheetMaterialFilter => new ActivityHdrYieldSheetFilter(),
                _ => null
            };
        }
    }
}