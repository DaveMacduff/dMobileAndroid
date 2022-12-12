// <copyright file="FormDialogContainer.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Forms
{
    using System;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.Controller.Activity;
    using dMobile.Controller.Activity_Detail;
    using dMobile.Controller.Activity_Item;
    using dMobile.View.Activity;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Options;
    using dMobile.View.Views;
    using Xamarin.Forms.Xaml;

    /// <summary>
    /// Defines the <see cref="FormDialogContainer" /> main form dialog container.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormDialogContainer : DMobileModal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormDialogContainer"/> class.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes"/> Inner model.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        public FormDialogContainer(EDialogTypes modelType, SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();
            var model = modelType switch
            {
                EDialogTypes.RefreshDataStatus => new RefreshDataStatus(actSubType),
                EDialogTypes.ActivityInventoryInquiry => new ActivityInventoryInquiry(),
                EDialogTypes.ActivityDeboxLabelCount => new ActivityDeboxLabelCount(),
                EDialogTypes.SelectProductUnit => this.GetModel(modelType, actSubType),
                EDialogTypes.SelectProductUnitFilterToProduct => this.GetModel(modelType, actSubType),
                EDialogTypes.SelectLocationMap => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityItem => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityDetail => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityDetailEdit => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityHdrPickingFilter => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityHdrReceivingFilter => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityHdrTransferFilter => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityHdrPhysicalCountFilter => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityHdrReturnsFilter => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityHdrCycleCountFilter => this.GetModel(modelType, actSubType),
                EDialogTypes.SubmitBatchFilter => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityAdjustmentFilter => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityProductionTicketFinishedFilter => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityProductionTicketMaterialFilter => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityProductionTicketByProductFilter => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityPurchaseOrderWorkSheetEdit => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityPickingSelectCriteria => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityShippingLabelEdit => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityConvertUnitEdit => this.GetModel(modelType, actSubType),
                EDialogTypes.SkidInfo => this.GetModel(modelType, actSubType),
                EDialogTypes.SkidInfoEdit => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityDetailSkid => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityItemSingle => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityInventoryInquiryItem => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityInventoryInquiryDetail => this.GetModel(modelType, actSubType),
                EDialogTypes.SelectProduct => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityProductLabel => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityConvertProductDetail => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityInventoryInquirySummary => this.GetModel(modelType, actSubType),
                EDialogTypes.ActivityRemoveFromSkid => this.GetModel(modelType, actSubType),
                _ => this.GetModel(modelType, actSubType),
            };

            model ??= this.GetModel(modelType);
            this.DisplayControl = model;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormDialogContainer"/> class.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes"/> Inner model.</param>
        /// <param name="labelQtyEdit"><see cref="SLabelQuantity"/> Label quantity.</param>
        /// <param name="activity"><see cref="Activity"/> Activity header.</param>
        /// <param name="selectMode">The selectMode<see cref="SysCommon.ShippingLabelQuantitySelectionMode"/>.</param>
        public FormDialogContainer(
            EDialogTypes modelType,
            SLabelQuantity labelQtyEdit,
            Controller.Activity.Activity activity,
            ShippingLabelQuantitySelectionMode selectMode)
        {
            this.InitializeComponent();

            var msg = $"Model object for {SysCommon.SourceEnum2String(modelType)}.";
            IDMobileModal model = modelType switch
            {
                EDialogTypes.ActivityShippingLabelEdit => new ActivityShippingLabelEdit(
                    SysCommon.ActSubTypes.Picking,  // Override the subtype to Picking
                    labelQtyEdit,
                    activity,
                    selectMode),
                EDialogTypes.RefreshDataStatus => throw new NotSupportedException(msg),
                EDialogTypes.SelectProductUnit => throw new NotSupportedException(msg),
                EDialogTypes.SelectProductUnitFilterToProduct => throw new NotSupportedException(msg),
                EDialogTypes.SelectLocationMap => throw new NotSupportedException(msg),
                EDialogTypes.ActivityItem => throw new NotSupportedException(msg),
                EDialogTypes.ActivityDetail => throw new NotSupportedException(msg),
                EDialogTypes.ActivityDetailEdit => throw new NotSupportedException(msg),
                EDialogTypes.ActivityHdrPickingFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityHdrReceivingFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityHdrTransferFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityHdrPhysicalCountFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityHdrReturnsFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityHdrCycleCountFilter => throw new NotSupportedException(msg),
                EDialogTypes.SubmitBatchFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityAdjustmentFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityProductionTicketFinishedFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityProductionTicketMaterialFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityProductionTicketByProductFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityPurchaseOrderWorkSheetEdit => throw new NotSupportedException(msg),
                EDialogTypes.ActivityPickingSelectCriteria => throw new NotSupportedException(msg),
                EDialogTypes.ActivityConvertUnitEdit => throw new NotSupportedException(msg),
                EDialogTypes.SkidInfo => throw new NotSupportedException(msg),
                EDialogTypes.SkidInfoEdit => throw new NotSupportedException(msg),
                EDialogTypes.ActivityDetailSkid => throw new NotSupportedException(msg),
                EDialogTypes.ActivityItemSingle => throw new NotSupportedException(msg),
                EDialogTypes.ActivityInventoryInquiry => throw new NotSupportedException(msg),
                EDialogTypes.ActivityInventoryInquiryItem => throw new NotSupportedException(msg),
                EDialogTypes.ActivityInventoryInquiryDetail => throw new NotSupportedException(msg),
                EDialogTypes.SelectProduct => throw new NotSupportedException(msg),
                EDialogTypes.ActivityProductLabel => throw new NotSupportedException(msg),
                EDialogTypes.ActivityConvertProductDetail => throw new NotSupportedException(msg),
                EDialogTypes.ActivityInventoryInquirySummary => throw new NotSupportedException(msg),
                EDialogTypes.ActivityRemoveFromSkid => throw new NotSupportedException(msg),
                EDialogTypes.ActivityDeboxLabelCount => throw new NotSupportedException(msg),
                _ => throw new NotSupportedException(msg),
            };

            this.DisplayControl = model;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormDialogContainer"/> class.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes"/> Inner model.</param>
        /// <param name="activity"><see cref="Activity"/> Activity header.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        public FormDialogContainer(
            EDialogTypes modelType,
            Controller.Activity.Activity activity,
            SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();
            var model = modelType switch
            {
                EDialogTypes.RefreshDataStatus => new RefreshDataStatus(actSubType),
                EDialogTypes.ActivityInventoryInquiry => new ActivityInventoryInquiry(),
                EDialogTypes.SelectProductUnit => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.SelectProductUnitFilterToProduct => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.SelectLocationMap => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityItem => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityDetail => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityDetailEdit => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityHdrPickingFilter => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityHdrReceivingFilter => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityHdrTransferFilter => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityHdrPhysicalCountFilter => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityHdrReturnsFilter => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityHdrCycleCountFilter => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.SubmitBatchFilter => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityAdjustmentFilter => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityProductionTicketFinishedFilter => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityProductionTicketMaterialFilter => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityProductionTicketByProductFilter => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityPurchaseOrderWorkSheetEdit => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityPickingSelectCriteria => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityShippingLabelEdit => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityConvertUnitEdit => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.SkidInfo => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.SkidInfoEdit => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityDetailSkid => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityItemSingle => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityInventoryInquiryItem => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityInventoryInquiryDetail => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.SelectProduct => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityProductLabel => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityConvertProductDetail => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityInventoryInquirySummary => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityRemoveFromSkid => this.GetModel(modelType, activity, actSubType),
                EDialogTypes.ActivityDeboxLabelCount => this.GetModel(modelType, activity, actSubType),
                _ => this.GetModel(modelType, activity, actSubType),
            };

            this.DisplayControl = model;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormDialogContainer"/> class.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes"/> Inner model.</param>
        /// <param name="actItem"><see cref="ActItem"/> Activity item object.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        /// <param name="callingActSubType">The callingActSubType<see cref="SysCommon.ActSubTypes"/>.</param>
        public FormDialogContainer(
            EDialogTypes modelType,
            ActItem actItem,
            SysCommon.ActSubTypes actSubType,
            SysCommon.ActSubTypes callingActSubType = SysCommon.ActSubTypes.SkidInfo)
        {
            this.InitializeComponent();
            var model = modelType switch
            {
                EDialogTypes.RefreshDataStatus => new RefreshDataStatus(actSubType),
                EDialogTypes.ActivityInventoryInquiry => new ActivityInventoryInquiry(actItem.ProdInfo),
                EDialogTypes.ActivityInventoryInquirySummary => new ActivityItemInventoryInquirySummary(actItem),
                EDialogTypes.SelectProductUnit => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.SelectProductUnitFilterToProduct => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.SelectLocationMap => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityItem => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityDetail => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityDetailEdit => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityHdrPickingFilter => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityHdrReceivingFilter => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityHdrTransferFilter => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityHdrPhysicalCountFilter => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityHdrReturnsFilter => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityHdrCycleCountFilter => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.SubmitBatchFilter => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityAdjustmentFilter => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityProductionTicketFinishedFilter => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityProductionTicketMaterialFilter => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityProductionTicketByProductFilter => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityPurchaseOrderWorkSheetEdit => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityPickingSelectCriteria => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityShippingLabelEdit => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityConvertUnitEdit => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.SkidInfo => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.SkidInfoEdit => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityDetailSkid => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityItemSingle => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityInventoryInquiryItem => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityInventoryInquiryDetail => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.SelectProduct => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityProductLabel => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityConvertProductDetail => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityRemoveFromSkid => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityDeboxLabelCount => this.GetModel(modelType, actItem, actSubType, callingActSubType),
                _ => this.GetModel(modelType, actItem, actSubType, callingActSubType),
            };

            this.DisplayControl = model;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormDialogContainer"/> class.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes"/> Inner model.</param>
        /// <param name="actDetail"><see cref="ActDetail"/> Activity detail object.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        public FormDialogContainer(EDialogTypes modelType, ActDetail actDetail, SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();
            var model = modelType switch
            {
                EDialogTypes.RefreshDataStatus => new RefreshDataStatus(actSubType),
                EDialogTypes.ActivityInventoryInquiry => new ActivityInventoryInquiry(actDetail.ActItem.ProdInfo),
                EDialogTypes.SelectProductUnit => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.SelectProductUnitFilterToProduct => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.SelectLocationMap => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityItem => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityDetail => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityDetailEdit => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityHdrPickingFilter => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityHdrReceivingFilter => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityHdrTransferFilter => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityHdrPhysicalCountFilter => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityHdrReturnsFilter => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityHdrCycleCountFilter => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.SubmitBatchFilter => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityAdjustmentFilter => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityProductionTicketFinishedFilter => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityProductionTicketMaterialFilter => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityProductionTicketByProductFilter => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityPurchaseOrderWorkSheetEdit => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityPickingSelectCriteria => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityShippingLabelEdit => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityConvertUnitEdit => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityConvertUnitEditCatch => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.SkidInfo => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.SkidInfoEdit => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityDetailSkid => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityItemSingle => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityInventoryInquiryItem => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityInventoryInquiryDetail => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.SelectProduct => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityProductLabel => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityConvertProductDetail => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityInventoryInquirySummary => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityRemoveFromSkid => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityDeboxLabelCount => this.GetModel(modelType, actDetail, actSubType),
                EDialogTypes.ActivityConvertUnitDetail => this.GetModel(modelType, actDetail, actSubType),
                _ => this.GetModel(modelType, actDetail, actSubType),
            };

            this.DisplayControl = model;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormDialogContainer"/> class.
        /// </summary>
        /// <param name="modelType">The modelType<see cref="EDialogTypes"/>.</param>
        /// <param name="defaultWhsLocMapRefno">The defaultWhsLocMapRefno<see cref="int"/>.</param>
        /// <param name="lockWhsSelection">The lockWhsSelection<see cref="bool"/>.</param>
        public FormDialogContainer(EDialogTypes modelType, int defaultWhsLocMapRefno, bool lockWhsSelection)
        {
            this.InitializeComponent();

            var msg = $"Model object for {SysCommon.SourceEnum2String(modelType)}.";
            IDMobileModal model = modelType switch
            {
                EDialogTypes.SelectLocationMap => new SetLocationMap(defaultWhsLocMapRefno, lockWhsSelection),
                EDialogTypes.RefreshDataStatus => throw new NotSupportedException(msg),
                EDialogTypes.SelectProductUnit => throw new NotSupportedException(msg),
                EDialogTypes.SelectProductUnitFilterToProduct => throw new NotSupportedException(msg),

                EDialogTypes.ActivityHdrPickingFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityHdrReceivingFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityHdrTransferFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityHdrPhysicalCountFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityHdrReturnsFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityHdrCycleCountFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityAdjustmentFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityProductionTicketFinishedFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityProductionTicketMaterialFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityProductionTicketByProductFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityItem => throw new NotSupportedException(msg),
                EDialogTypes.ActivityDetail => throw new NotSupportedException(msg),
                EDialogTypes.ActivityDetailEdit => throw new NotSupportedException(msg),
                EDialogTypes.SubmitBatchFilter => throw new NotSupportedException(msg),
                EDialogTypes.ActivityPurchaseOrderWorkSheetEdit => throw new NotSupportedException(msg),
                EDialogTypes.ActivityPickingSelectCriteria => throw new NotSupportedException(msg),
                EDialogTypes.ActivityShippingLabelEdit => throw new NotSupportedException(msg),
                EDialogTypes.ActivityConvertUnitEdit => throw new NotSupportedException(msg),
                EDialogTypes.SkidInfo => throw new NotSupportedException(msg),
                EDialogTypes.SkidInfoEdit => throw new NotSupportedException(msg),
                EDialogTypes.ActivityDetailSkid => throw new NotSupportedException(msg),
                EDialogTypes.ActivityItemSingle => throw new NotSupportedException(msg),
                EDialogTypes.ActivityInventoryInquiry => throw new NotSupportedException(msg),
                EDialogTypes.ActivityInventoryInquiryItem => throw new NotSupportedException(msg),
                EDialogTypes.ActivityInventoryInquiryDetail => throw new NotSupportedException(msg),
                EDialogTypes.SelectProduct => throw new NotSupportedException(msg),
                EDialogTypes.ActivityProductLabel => throw new NotSupportedException(msg),
                EDialogTypes.ActivityConvertProductDetail => throw new NotSupportedException(msg),
                EDialogTypes.ActivityInventoryInquirySummary => throw new NotSupportedException(msg),
                EDialogTypes.ActivityRemoveFromSkid => throw new NotSupportedException(msg),
                EDialogTypes.ActivityDeboxLabelCount => throw new NotSupportedException(msg),
                EDialogTypes.ActivityConvertUnitDetail => throw new NotSupportedException(msg),
                _ => throw new NotSupportedException(msg),
            };

            this.DisplayControl = model;
        }

        /// <summary>
        /// Gets or sets the display control.
        /// </summary>
        public IDMobileModal DisplayControl { get; set; }

        /// <summary>
        /// Get the display control XML return data.
        /// </summary>
        /// <returns><see cref="Task(XDocument)"/> XML data.</returns>
        public async Task<XDocument> GetXML()
        {
            return await ((IAppArea)this.DisplayControl).GetXML();
        }

        /// <summary>
        /// Get the model base on the dialog type.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes"/> Inner model.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        /// <returns><see cref="IDMobileModal"/> Model.</returns>
        private IDMobileModal GetModel(
            EDialogTypes modelType,
            SysCommon.ActSubTypes actSubType,
            SLabelQuantity labelQtyEdit,
            Controller.Activity.Activity activity,
            ShippingLabelQuantitySelectionMode selectMode)
        {
            IDMobileModal model = modelType switch
            {
                EDialogTypes.ActivityItem => new ActivityItem(activity, actSubType),
                EDialogTypes.ActivityItemSingle => new ActivityItemSingle(activity, actSubType),
                EDialogTypes.SkidInfo => new ActivityHdrSkidInfo(actSubType, activity),
                EDialogTypes.RefreshDataStatus => null,
                EDialogTypes.SelectProductUnit => null,
                EDialogTypes.SelectProductUnitFilterToProduct => null,
                EDialogTypes.SelectLocationMap => null,
                EDialogTypes.ActivityDetail => null,
                EDialogTypes.ActivityDetailEdit => null,
                EDialogTypes.ActivityHdrPickingFilter => null,
                EDialogTypes.ActivityHdrReceivingFilter => null,
                EDialogTypes.ActivityHdrTransferFilter => null,
                EDialogTypes.ActivityHdrPhysicalCountFilter => null,
                EDialogTypes.ActivityHdrReturnsFilter => null,
                EDialogTypes.ActivityHdrCycleCountFilter => null,
                EDialogTypes.SubmitBatchFilter => null,
                EDialogTypes.ActivityAdjustmentFilter => null,
                EDialogTypes.ActivityProductionTicketFinishedFilter => null,
                EDialogTypes.ActivityProductionTicketMaterialFilter => null,
                EDialogTypes.ActivityProductionTicketByProductFilter => null,
                EDialogTypes.ActivityPurchaseOrderWorkSheetEdit => null,
                EDialogTypes.ActivityPickingSelectCriteria => null,
                EDialogTypes.ActivityShippingLabelEdit => new ActivityShippingLabelEdit(actSubType, labelQtyEdit, activity, selectMode),
                EDialogTypes.ActivityConvertUnitEdit => null,
                EDialogTypes.SkidInfoEdit => null,
                EDialogTypes.ActivityDetailSkid => null,
                EDialogTypes.ActivityInventoryInquiry => null,
                EDialogTypes.ActivityInventoryInquiryItem => null,
                EDialogTypes.ActivityInventoryInquiryDetail => null,
                EDialogTypes.SelectProduct => null,
                EDialogTypes.ActivityProductLabel => null,
                EDialogTypes.ActivityConvertProductDetail => null,
                EDialogTypes.ActivityInventoryInquirySummary => null,
                EDialogTypes.ActivityRemoveFromSkid => null,
                EDialogTypes.ActivityDeboxLabelCount => null,
                EDialogTypes.ActivityConvertUnitDetail => null,
                _ => null,
            };

            return model;
        }

        /// <summary>
        /// Get the model base on the dialog type.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes"/> Inner model.</param>
        /// <param name="activity"><see cref="Activity"/> Activity header.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        /// <returns><see cref="IDMobileModal"/> Model.</returns>
        private IDMobileModal GetModel(
            EDialogTypes modelType,
            Controller.Activity.Activity activity,
            SysCommon.ActSubTypes actSubType)
        {
            IDMobileModal model = modelType switch
            {
                EDialogTypes.ActivityItem => new ActivityItem(activity, actSubType),
                EDialogTypes.ActivityItemSingle => new ActivityItemSingle(activity, actSubType),
                EDialogTypes.SkidInfo => new ActivityHdrSkidInfo(actSubType, activity),
                EDialogTypes.RefreshDataStatus => null,
                EDialogTypes.SelectProductUnit => null,
                EDialogTypes.SelectProductUnitFilterToProduct => null,
                EDialogTypes.SelectLocationMap => null,
                EDialogTypes.ActivityDetail => null,
                EDialogTypes.ActivityDetailEdit => null,
                EDialogTypes.ActivityHdrPickingFilter => null,
                EDialogTypes.ActivityHdrReceivingFilter => null,
                EDialogTypes.ActivityHdrTransferFilter => null,
                EDialogTypes.ActivityHdrPhysicalCountFilter => null,
                EDialogTypes.ActivityHdrReturnsFilter => null,
                EDialogTypes.ActivityHdrCycleCountFilter => null,
                EDialogTypes.SubmitBatchFilter => null,
                EDialogTypes.ActivityAdjustmentFilter => null,
                EDialogTypes.ActivityProductionTicketFinishedFilter => null,
                EDialogTypes.ActivityProductionTicketMaterialFilter => null,
                EDialogTypes.ActivityProductionTicketByProductFilter => null,
                EDialogTypes.ActivityPurchaseOrderWorkSheetEdit => null,
                EDialogTypes.ActivityPickingSelectCriteria => null,
                EDialogTypes.ActivityShippingLabelEdit => null,
                EDialogTypes.ActivityConvertUnitEdit => null,
                EDialogTypes.SkidInfoEdit => null,
                EDialogTypes.ActivityDetailSkid => null,
                EDialogTypes.ActivityInventoryInquiry => null,
                EDialogTypes.ActivityInventoryInquiryItem => null,
                EDialogTypes.ActivityInventoryInquiryDetail => null,
                EDialogTypes.SelectProduct => null,
                EDialogTypes.ActivityProductLabel => null,
                EDialogTypes.ActivityConvertProductDetail => null,
                EDialogTypes.ActivityInventoryInquirySummary => null,
                EDialogTypes.ActivityRemoveFromSkid => null,
                EDialogTypes.ActivityDeboxLabelCount => null,
                EDialogTypes.ActivityConvertUnitDetail => null,
                _ => null,
            };

            return model;
        }

        /// <summary>
        /// Get the model base on the dialog type.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes"/> Inner model.</param>
        /// <param name="actDetail">.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        /// <returns><see cref="IDMobileModal"/> Model.</returns>
        private IDMobileModal GetModel(
            EDialogTypes modelType,
            ActDetail actDetail,
            SysCommon.ActSubTypes actSubType)
        {
            IDMobileModal model = modelType switch
            {
                EDialogTypes.ActivityDetailEdit => new ActivityDetailEdit(actDetail, actSubType),
                EDialogTypes.ActivityProductLabel => new ActivityProductLabel(actDetail, actSubType),
                EDialogTypes.ActivityPurchaseOrderWorkSheetEdit => new ActivityItemPurchaseOrderWorkSheetEdit(
                    actDetail,
                    SysCommon.ActSubTypes.PurchaseOrderWorkSheet), // Override the subtype to PurchaseOrderWorkSheet
                EDialogTypes.ActivityConvertUnitEdit => new ActivityConvertUnitEdit(
                    actDetail,
                    SysCommon.ActSubTypes.ConvertUnit), // Override the subtype to ConvertUnit
                EDialogTypes.ActivityConvertUnitEditCatch => new ActivityConvertUnitEditCatch(
                    actDetail,
                    SysCommon.ActSubTypes.ConvertUnit), // Override the subtype to ConvertUnit
                EDialogTypes.ActivityInventoryInquiryDetail => new ActivityDetailInventoryInquiry(actDetail, actSubType),
                EDialogTypes.ActivityConvertProductDetail => new ActivityConvertProductDetail(actDetail, actSubType),
                EDialogTypes.ActivityRemoveFromSkid => new ActivityRemoveFromSkid(actDetail, actSubType),
                EDialogTypes.RefreshDataStatus => null,
                EDialogTypes.SelectProductUnit => null,
                EDialogTypes.SelectProductUnitFilterToProduct => null,
                EDialogTypes.SelectLocationMap => null,
                EDialogTypes.ActivityItem => null,
                EDialogTypes.ActivityDetail => null,
                EDialogTypes.ActivityHdrPickingFilter => null,
                EDialogTypes.ActivityHdrReceivingFilter => null,
                EDialogTypes.ActivityHdrTransferFilter => null,
                EDialogTypes.ActivityHdrPhysicalCountFilter => null,
                EDialogTypes.ActivityHdrReturnsFilter => null,
                EDialogTypes.ActivityHdrCycleCountFilter => null,
                EDialogTypes.SubmitBatchFilter => null,
                EDialogTypes.ActivityAdjustmentFilter => null,
                EDialogTypes.ActivityProductionTicketFinishedFilter => null,
                EDialogTypes.ActivityProductionTicketMaterialFilter => null,
                EDialogTypes.ActivityProductionTicketByProductFilter => null,
                EDialogTypes.ActivityPickingSelectCriteria => null,
                EDialogTypes.ActivityShippingLabelEdit => null,
                EDialogTypes.SkidInfo => null,
                EDialogTypes.SkidInfoEdit => null,
                EDialogTypes.ActivityDetailSkid => null,
                EDialogTypes.ActivityItemSingle => null,
                EDialogTypes.ActivityInventoryInquiry => null,
                EDialogTypes.ActivityInventoryInquiryItem => null,
                EDialogTypes.SelectProduct => null,
                EDialogTypes.ActivityInventoryInquirySummary => null,
                EDialogTypes.ActivityDeboxLabelCount => null,
                EDialogTypes.ActivityConvertUnitDetail => null,
                _ => null,
            };

            return model;
        }

        /// <summary>
        /// Get the model base on the dialog type.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes"/> Inner model.</param>
        /// <param name="actItem">.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        /// <param name="callingActSubType">The callingActSubType<see cref="SysCommon.ActSubTypes"/>.</param>
        /// <returns><see cref="IDMobileModal"/> Model.</returns>
        private IDMobileModal GetModel(
            EDialogTypes modelType,
            ActItem actItem,
            SysCommon.ActSubTypes actSubType,
            SysCommon.ActSubTypes callingActSubType = SysCommon.ActSubTypes.SkidInfo)
        {
            IDMobileModal model = modelType switch
            {
                EDialogTypes.ActivityDetail => new ActivityDetail(actItem, actSubType),
                EDialogTypes.SelectProductUnitFilterToProduct => new SelectProductUnit(
                    actItem.ProdInfo.Refno,
                    actItem.ProdInfo.UnitRefno,
                    actSubType),
                EDialogTypes.SkidInfo => new ActivityHdrSkidInfo(actSubType, actItem),
                EDialogTypes.ActivityDetailSkid => new ActivityDetailSkid(actItem, actSubType, callingActSubType),
                EDialogTypes.ActivityInventoryInquiryItem => new ActivityItemInventoryInquiry(actItem, actSubType),
                EDialogTypes.RefreshDataStatus => null,
                EDialogTypes.SelectProductUnit => null,
                EDialogTypes.SelectLocationMap => null,
                EDialogTypes.ActivityItem => null,
                EDialogTypes.ActivityDetailEdit => null,
                EDialogTypes.ActivityHdrPickingFilter => null,
                EDialogTypes.ActivityHdrReceivingFilter => null,
                EDialogTypes.ActivityHdrTransferFilter => null,
                EDialogTypes.ActivityHdrPhysicalCountFilter => null,
                EDialogTypes.ActivityHdrReturnsFilter => null,
                EDialogTypes.ActivityHdrCycleCountFilter => null,
                EDialogTypes.SubmitBatchFilter => null,
                EDialogTypes.ActivityAdjustmentFilter => null,
                EDialogTypes.ActivityProductionTicketFinishedFilter => null,
                EDialogTypes.ActivityProductionTicketMaterialFilter => null,
                EDialogTypes.ActivityProductionTicketByProductFilter => null,
                EDialogTypes.ActivityPurchaseOrderWorkSheetEdit => null,
                EDialogTypes.ActivityPickingSelectCriteria => null,
                EDialogTypes.ActivityShippingLabelEdit => null,
                EDialogTypes.ActivityConvertUnitEdit => null,
                EDialogTypes.SkidInfoEdit => null,
                EDialogTypes.ActivityItemSingle => null,
                EDialogTypes.ActivityInventoryInquiry => null,
                EDialogTypes.ActivityInventoryInquiryDetail => null,
                EDialogTypes.SelectProduct => null,
                EDialogTypes.ActivityProductLabel => null,
                EDialogTypes.ActivityConvertProductDetail => null,
                EDialogTypes.ActivityInventoryInquirySummary => null,
                EDialogTypes.ActivityRemoveFromSkid => null,
                EDialogTypes.ActivityDeboxLabelCount => null,
                EDialogTypes.ActivityConvertUnitDetail => new ActivityConvertUnitDetail(actItem, actSubType),
                _ => null,
            };

            return model;
        }

        /// <summary>
        /// Get the model base on the dialog type.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes"/> Inner model.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        /// <returns><see cref="IDMobileModal"/> Model.</returns>
        private IDMobileModal GetModel(EDialogTypes modelType, SysCommon.ActSubTypes actSubType)
        {
            IDMobileModal model = modelType switch
            {
                EDialogTypes.SelectProductUnit => new SelectProductUnit(actSubType),
                EDialogTypes.RefreshDataStatus => null,
                EDialogTypes.SelectProductUnitFilterToProduct => null,
                EDialogTypes.SelectLocationMap => null,
                EDialogTypes.ActivityItem => null,
                EDialogTypes.ActivityDetail => null,
                EDialogTypes.ActivityDetailEdit => null,
                EDialogTypes.ActivityHdrPickingFilter => null,
                EDialogTypes.ActivityHdrReceivingFilter => null,
                EDialogTypes.ActivityHdrTransferFilter => null,
                EDialogTypes.ActivityHdrPhysicalCountFilter => null,
                EDialogTypes.ActivityHdrReturnsFilter => null,
                EDialogTypes.ActivityHdrCycleCountFilter => null,
                EDialogTypes.SubmitBatchFilter => null,
                EDialogTypes.ActivityAdjustmentFilter => null,
                EDialogTypes.ActivityProductionTicketFinishedFilter => null,
                EDialogTypes.ActivityProductionTicketMaterialFilter => null,
                EDialogTypes.ActivityProductionTicketByProductFilter => null,
                EDialogTypes.ActivityPurchaseOrderWorkSheetEdit => null,
                EDialogTypes.ActivityPickingSelectCriteria => null,
                EDialogTypes.ActivityShippingLabelEdit => null,
                EDialogTypes.ActivityConvertUnitEdit => null,
                EDialogTypes.SkidInfo => null,
                EDialogTypes.SkidInfoEdit => null,
                EDialogTypes.ActivityDetailSkid => null,
                EDialogTypes.ActivityItemSingle => null,
                EDialogTypes.ActivityInventoryInquiry => null,
                EDialogTypes.ActivityInventoryInquiryItem => null,
                EDialogTypes.ActivityInventoryInquiryDetail => null,
                EDialogTypes.SelectProduct => null,
                EDialogTypes.ActivityProductLabel => null,
                EDialogTypes.ActivityConvertProductDetail => null,
                EDialogTypes.ActivityInventoryInquirySummary => null,
                EDialogTypes.ActivityRemoveFromSkid => null,
                EDialogTypes.ActivityDeboxLabelCount => null,
                EDialogTypes.ActivityConvertUnitDetail => null,
                _ => null,
            };

            return model;
        }

        /// <summary>
        /// Get the model base on the dialog type.
        /// </summary>
        /// <param name="modelType"><see cref="EDialogTypes"/> Inner model.</param>
        /// <returns><see cref="IDMobileModal"/> Model.</returns>
        private IDMobileModal GetModel(EDialogTypes modelType)
        {
            IDMobileModal model = modelType switch
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
                EDialogTypes.SkidInfo => new ActivityHdrSkidInfo(),
                EDialogTypes.RefreshDataStatus => null,
                EDialogTypes.SelectProductUnit => null,
                EDialogTypes.SelectProductUnitFilterToProduct => null,
                EDialogTypes.ActivityItem => null,
                EDialogTypes.ActivityDetail => null,
                EDialogTypes.ActivityDetailEdit => null,
                EDialogTypes.ActivityPurchaseOrderWorkSheetEdit => null,
                EDialogTypes.ActivityShippingLabelEdit => null,
                EDialogTypes.ActivityConvertUnitEdit => null,
                EDialogTypes.SkidInfoEdit => null,
                EDialogTypes.ActivityDetailSkid => null,
                EDialogTypes.ActivityItemSingle => null,
                EDialogTypes.ActivityInventoryInquiry => null,
                EDialogTypes.ActivityInventoryInquiryItem => null,
                EDialogTypes.ActivityInventoryInquiryDetail => null,
                EDialogTypes.ActivityProductLabel => null,
                EDialogTypes.ActivityConvertProductDetail => null,
                EDialogTypes.ActivityInventoryInquirySummary => null,
                EDialogTypes.ActivityRemoveFromSkid => null,
                EDialogTypes.ActivityDeboxLabelCount => null,
                EDialogTypes.ActivityConvertUnitDetail => null,
                _ => null,
            };

            return model;
        }
    }
}
