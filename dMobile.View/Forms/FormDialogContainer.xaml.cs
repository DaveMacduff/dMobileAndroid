
namespace dMobile.View.Forms
{
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.View.Activity;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Options;
    using System;
    using System.ComponentModel;
    using System.Xml.Linq;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using dMobile.Resources.Enums;
    using Resource = dMobile.Resources.Properties.Resources;
    using System.Threading.Tasks;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormDialogContainer : DMobileModal, IAppListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormDialogContainer"/> class.
        /// </summary>
        public FormDialogContainer()
        {
            InitializeComponent();
        }

        private bool isControlClosing = false;
        private XDocument displayControlData;
        private IDMobileModal displayControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormDialogContainer"/> class.
        /// </summary>
        /// <param name="typeInnerControl"></param>
        /// <param name="actSubType"></param>
        public FormDialogContainer(EDialogTypes typeInnerControl, SysCommon.ActSubTypes actSubType)
        {
            IDMobileModal control = null;

            this.InitializeComponent();

            //this.menuItemCancel.Enabled = false;
            //this.menuItemSave.Enabled = false;

            switch (typeInnerControl)
            {
                case EDialogTypes.ucRefreshDataStatus:
                    control = new ucRefreshDataStatus(actSubType);
                    break;

                case EDialogTypes.ucActivityInventoryInquiry:
                    control = new ucActivityInventoryInquiry();
                    break;

                case EDialogTypes.ucActivityDeboxLabelCount:
                    control = new ucActivityDeboxLabelCount();
                    break;

                default:
                    control = this.GetControlObj(typeInnerControl, actSubType);

                    if (control == null)
                    {
                        control = this.GetControlObj(typeInnerControl);
                    }
                    break;
            }

            this.ShowActivityObj(control);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormDialogContainer"/> class.
        /// </summary>
        /// <param name="typeInnerControl"></param>
        /// <param name="callingControl"></param>
        /// <param name="actSubType"></param>
        /// <param name="labelQtyEdit"></param>
        /// <param name="activity"></param>
        /// <param name="selectMode">The selectMode<see cref="SysCommon.ShippingLabelQuantitySelectionMode"/></param>
        public FormDialogContainer(EDialogTypes typeInnerControl, SysCommon.ActSubTypes actSubType, SysCommon.SLabelQuantity labelQtyEdit, Activity activity, SysCommon.ShippingLabelQuantitySelectionMode selectMode)
        {
            IDMobileModal control = null;

            this.InitializeComponent();

            //this.menuItemCancel.Enabled = false;
            //this.menuItemSave.Enabled = false;

            switch (typeInnerControl)
            {
                case EDialogTypes.ucActivityShippingLabelEdit:
                    // Override the subtype to Picking
                    control = new ucActivityShippingLabelEdit(SysCommon.ActSubTypes.Picking, labelQtyEdit, activity, selectMode);
                    break;
            }

            this.ShowActivityObj(control);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormDialogContainer"/> class.
        /// </summary>
        /// <param name="typeInnerControl"></param>
        /// <param name="activity"></param>
        /// <param name="actSubType"></param>
        /// <param name="callingControl"></param>
        public FormDialogContainer(EDialogTypes typeInnerControl, Activity activity, SysCommon.ActSubTypes actSubType)
        {
            IDMobileModal control = null;

            this.InitializeComponent();

            //this.menuItemCancel.Enabled = false;
            //this.menuItemSave.Enabled = false;

            switch (typeInnerControl)
            {
                case EDialogTypes.ucRefreshDataStatus:
                    control = new ucRefreshDataStatus(actSubType);
                    break;

                case EDialogTypes.ucActivityInventoryInquiry:
                    control = new ucActivityInventoryInquiry();
                    break;

                default:
                    control = this.GetControlObj(typeInnerControl, activity, actSubType/*, callingControl*/);
                    break;
            }

            this.ShowActivityObj(control);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormDialogContainer"/> class.
        /// </summary>
        /// <param name="typeInnerControl"></param>
        /// <param name="actItem"></param>
        /// <param name="actSubType"></param>
        /// <param name="callingControl"></param>
        public FormDialogContainer(EDialogTypes typeInnerControl, ActItem actItem, SysCommon.ActSubTypes actSubType, SysCommon.ActSubTypes callingActSubType = SysCommon.ActSubTypes.SkidInfo)
        {
            IDMobileModal control = null;

            this.InitializeComponent();

            //this.menuItemCancel.Enabled = false;
            //this.menuItemSave.Enabled = false;

            switch (typeInnerControl)
            {
                case EDialogTypes.ucRefreshDataStatus:
                    control = new ucRefreshDataStatus(actSubType);
                    break;

                case EDialogTypes.ucActivityInventoryInquiry:
                    control = new ucActivityInventoryInquiry(actItem.ProdInfo);
                    break;

                case EDialogTypes.ucActivityInventoryInquirySummary:
                    control = new ucActivityItemInventoryInquirySummary(actItem);
                    break;

                default:
                    control = this.GetControlObj(typeInnerControl, actItem, actSubType, callingActSubType);
                    break;
            }

            this.ShowActivityObj(control);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormDialogContainer"/> class.
        /// </summary>
        /// <param name="typeInnerControl"></param>
        /// <param name="actDetail"></param>
        /// <param name="actSubType"></param>
        /// <param name="callingControl"></param>
        public FormDialogContainer(EDialogTypes typeInnerControl, ActDetail actDetail, SysCommon.ActSubTypes actSubType)
        {
            IDMobileModal control = null;

            this.InitializeComponent();

            //this.menuItemCancel.Enabled = false;
            //this.menuItemSave.Enabled = false;

            switch (typeInnerControl)
            {
                case EDialogTypes.ucRefreshDataStatus:
                    control = new ucRefreshDataStatus(actSubType);
                    break;

                case EDialogTypes.ucActivityInventoryInquiry:
                    control = new ucActivityInventoryInquiry(actDetail.ActItem.ProdInfo);
                    break;

                default:
                    control = this.GetControlObj(typeInnerControl, actDetail, actSubType);
                    break;
            }

            this.ShowActivityObj(control);
        }

        public FormDialogContainer(EDialogTypes typeInnerControl, int defaultWhsLocMapRefno, bool lockWhsSelection)
        {
            IDMobileModal control = null;
            this.InitializeComponent();

            switch (typeInnerControl)
            {
                case EDialogTypes.ucSelectLocationMap:
                    control = new ucSetLocationMap(defaultWhsLocMapRefno, lockWhsSelection);
                    break;

                default:
                    throw new NotSupportedException("FormDialogContainer constructor is specific to EDialogTypes = EDialogTypes.ucSelectLocationMap");
            }

            this.ShowActivityObj(control);
        }

        public enum EDialogTypes
        {
            ucRefreshDataStatus,
            ucSelectProductUnit,
            ucSelectProductUnitFilterToProduct,
            ucSelectLocationMap,
            ucActivityItem,
            ucActivityDetail,
            ucActivityDetailEdit,
            ucActivityHdrPickingFilter,
            ucActivityHdrReceivingFilter,
            ucActivityHdrTransferFilter,
            ucActivityHdrPhysicalCountFilter,
            ucActivityHdrReturnsFilter,
            ucActivityHdrCycleCountFilter,
            ucSubmitBatchFilter,
            ucActivityAdjustmentFilter,
            ucActivityProductionTicketFinishedFilter,
            ucActivityProductionTicketMaterialFilter,
            ucActivityProductionTicketByProductFilter,
            ucActivityPurchaseOrderWorkSheetEdit,
            ucActivityPickingSelectCriteria,
            ucActivityShippingLabelEdit,
            ucActivityConvertUnitEdit,
            ucSkidInfo,
            ucSkidInfoEdit,
            ucActivityDetailSkid,
            ucActivityItemSingle,
            ucActivityInventoryInquiry,
            ucActivityInventoryInquiryItem,
            ucActivityInventoryInquiryDetail,
            ucSelectProduct,
            ucActivityProductLabel,
            ucActivityConvertProductDetail,
            ucActivityInventoryInquirySummary,
            ucActivityRemoveFromSkid,
            ucActivityDeboxLabelCount
        }

        /// <summary>
        /// Gets or sets the DisplayControlData
        /// </summary>
        public XDocument DisplayControlData
        {
            get
            {
                return this.displayControlData;
            }

            set
            {
                this.displayControlData = value;
            }
        }

        /// <summary>
        /// Gets or sets the DisplayControl
        /// </summary>
        public IDMobileModal DisplayControl
        {
            get
            {
                return this.displayControl;
            }

            set
            {
                this.displayControl = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsControlClosing
        /// </summary>
        public bool IsControlClosing
        {
            get
            {
                return this.isControlClosing;
            }

            set
            {
                this.isControlClosing = value;
            }
        }

        /// <summary>
        /// The XML.
        /// </summary>
        /// <returns></returns>
        public async Task<XDocument> GetXML()
        {
            if (this.DisplayControl == null)
            {
                return null;
            }
            else
            {
                return await ((IAppArea)this.DisplayControl).GetXML();
            }
        }

        /// <summary>
        /// The SaveChanges
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async Task SaveChanges(object sender, EventArgs e)
        {
            if (this.DisplayControl == null)
            {
                await this.ReqCloseDialogContainer(this.DisplayControl, EventArgs.Empty);
            }
            else
            {
                if (await ((IAppArea)this.DisplayControl).SaveChanges())
                {
                    await this.ReqCloseDialogContainer(this.DisplayControl, EventArgs.Empty);

                    if (this.IsControlClosing)
                    {
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        this.DialogResult = DialogResult.Cancel;
                    }
                }
            }
        }

        /// <summary>
        /// The menuItemCancel_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void menuItemCancel_Click(object sender, EventArgs e)
        {
            //this.menuItemCancel.Enabled = false;

            if (this.DisplayControl == null)
            {
                this.ReqCloseDialogContainer(this.DisplayControl, EventArgs.Empty);
            }
            else
            {
                if (await ((IAppArea)this.DisplayControl).CancelChanges())
                {
                    this.ReqCloseDialogContainer(this.DisplayControl, EventArgs.Empty);
                }
            }

            //this.menuItemCancel.Enabled = true;
        }

        /// <summary>
        /// The FormDialogContainer_Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FormDialogContainer_Closing(object sender, CancelEventArgs e)
        {
            if (this.IsControlClosing)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// The FormDialogContainer_GotFocus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FormDialogContainer_GotFocus(object sender, EventArgs e)
        {
            //this.panelContents.Focus();
        }

        /// <summary>
        /// The FormDialogContainer_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FormDialogContainer_Load(object sender, EventArgs e)
        {
            //try
            //{
                //int platform = (int)SysPlatformDetector.GetPlatform();

                ////adjust the form size and pnlMain location depending on the OS
                ////Windows CE menus appear at the top versus at the bottom for PPC
                //if (platform == Convert.ToInt32(Platform.WindowsCE))
                //{
                //    this.Size = new Size(321, 241);
                //    panelContents.Location = new Point(0, 27);

                //    this.mainMenu.MenuItems[0].Text = "      Save        ";
                //    this.mainMenu.MenuItems[1].Text = "       Cancel     ";

                //}
                //else
                //{
                //    if (platform == Convert.ToInt32(Platform.PocketPC))
                //    {
                //        this.WindowState = FormWindowState.Normal;
                //    }

                //    this.Size = new Size(321, 241);
                //    this.panelContents.Location = new Point(0, 27);

                //    this.mainMenu.MenuItems[0].Text = "    Save    ";
                //    this.mainMenu.MenuItems[1].Text = "   Cancel   ";
                //}
            //}
            //catch (Exception ex)
            //{
                //MessageBox.Show(
                //  ex.Message,
                //  "dMobile",
                //  MessageBoxButtons.OK,
                //  MessageBoxIcon.Exclamation,
                //  MessageBoxDefaultButton.Button1);

                //DisplayAlert("dMobile", ex.Message, Resource.OK);
            //}
        }

        /// <summary>
        /// The ReqClose
        /// </summary>
        /// <param name="appEventPub"></param>
        /// <param name="appEventArgs"></param>
        public void ReqClose(IAppPublisher appEventPub, AppEventArgs appEventArgs)
        {
            this.ReqCloseDialogContainer(appEventPub, appEventArgs);
        }

        /// <summary>
        /// The ReqSaveEnable
        /// </summary>
        /// <param name="appEventPub"></param>
        /// <param name="appEventArgs"></param>
        public void ReqSaveEnable(IAppPublisher appEventPub, AppEventArgs appEventArgs)
        {
            //this.menuItemSave.Enabled = true;
        }

        /// <summary>
        /// The ReqSaveDisable
        /// </summary>
        /// <param name="appEventPub"></param>
        /// <param name="appEventArgs"></param>
        public void ReqSaveDisable(IAppPublisher appEventPub, AppEventArgs appEventArgs)
        {
            //this.menuItemSave.Enabled = false;
        }

        /// <summary>
        /// The ReqCancelEnable
        /// </summary>
        /// <param name="appEventPub"></param>
        /// <param name="appEventArgs"></param>
        public void ReqCancelEnable(IAppPublisher appEventPub, AppEventArgs appEventArgs)
        {
            //this.menuItemCancel.Enabled = true;
        }

        /// <summary>
        /// The ReqCancelDisable
        /// </summary>
        /// <param name="appEventPub"></param>
        /// <param name="appEventArgs"></param>
        public void ReqCancelDisable(IAppPublisher appEventPub, AppEventArgs appEventArgs)
        {
            //this.menuItemCancel.Enabled = false;
        }

        /// <summary>
        /// The ShowActivityObj
        /// </summary>
        /// <param name="control"></param>
        private void ShowActivityObj(IDMobileModal control)
        {
            if (control == null)
            {
                throw new Exception("Activity control could not be created. Check if the EDialogType was defined.");
            }
            else
            {
                ((IAppPublisher)control).RegisterReqClose((IAppListener)this);
                ((IAppPublisher)control).RegisterReqSaveEnable((IAppListener)this);
                ((IAppPublisher)control).RegisterReqSaveDisable((IAppListener)this);
                ((IAppPublisher)control).RegisterReqCancelEnable((IAppListener)this);
                ((IAppPublisher)control).RegisterReqCancelDisable((IAppListener)this);

                this.DisplayControl = control;

            }
        }

        /// <summary>
        /// The GetControlObj
        /// </summary>
        /// <param name="typeInnerControl"></param>
        /// <param name="actSubType"></param>
        /// <param name="callingControl"></param>
        /// <param name="labelQtyEdit"></param>
        /// <param name="activity"></param>
        /// <param name="selectMode">The selectMode<see cref="SysCommon.ShippingLabelQuantitySelectionMode"/></param>
        /// <returns></returns>
        private IDMobileModal GetControlObj(EDialogTypes typeInnerControl, SysCommon.ActSubTypes actSubType, SysCommon.SLabelQuantity labelQtyEdit, Activity activity, SysCommon.ShippingLabelQuantitySelectionMode selectMode)
        {
            IDMobileModal control = null;

            switch (typeInnerControl)
            {
                case EDialogTypes.ucActivityShippingLabelEdit:
                    control = new ucActivityShippingLabelEdit(actSubType, labelQtyEdit, activity, selectMode);
                    break;
            }

            return control;
        }

        /// <summary>
        /// The GetControlObj
        /// </summary>
        /// <param name="typeInnerControl"></param>
        /// <param name="activity"></param>
        /// <param name="actSubType"></param>
        /// <param name="callingControl"></param>
        /// <returns></returns>
        private IDMobileModal GetControlObj(EDialogTypes typeInnerControl, Activity activity, SysCommon.ActSubTypes actSubType)
        {
            IDMobileModal control = null;

            switch (typeInnerControl)
            {
                case EDialogTypes.ucActivityItem:
                    control = new ucActivityItem(activity, actSubType);
                    break;

                case EDialogTypes.ucActivityItemSingle:
                    control = new ucActivityItemSingle(activity, actSubType);
                    break;

                case EDialogTypes.ucSkidInfo:
                    control = new ucActivityHdrSkidInfo(actSubType, activity);
                    break;
            }

            return control;
        }

        /// <summary>
        /// The GetControlObj
        /// </summary>
        /// <param name="typeInnerControl"></param>
        /// <param name="actDetail"></param>
        /// <param name="actSubType"></param>
        /// <param name="callingControl"></param>
        /// <returns></returns>
        private IDMobileModal GetControlObj(EDialogTypes typeInnerControl, ActDetail actDetail, SysCommon.ActSubTypes actSubType)
        {
            IDMobileModal control = null;

            switch (typeInnerControl)
            {
                case EDialogTypes.ucActivityDetailEdit:
                    control = new ucActivityDetailEdit(actDetail, actSubType);
                    break;

                case EDialogTypes.ucActivityProductLabel:
                    control = new ucActivityProductLabel(actDetail, actSubType);
                    break;

                case EDialogTypes.ucActivityPurchaseOrderWorkSheetEdit:
                    // Override the subtype to PurchaseOrderWorkSheet
                    control = new ucActivityItemPurchaseOrderWorkSheetEdit(actDetail, SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                    break;

                case EDialogTypes.ucActivityConvertUnitEdit:
                    // Override the subtype to ConvertUnit
                    control = new ucActivityConvertUnitEdit(actDetail, SysCommon.ActSubTypes.ConvertUnit);
                    break;

                case EDialogTypes.ucActivityInventoryInquiryDetail:
                    control = new ucActivityDetailInventoryInquiry(actDetail, actSubType);
                    break;

                case EDialogTypes.ucActivityConvertProductDetail:
                    control = new ucActivityConvertProductDetail(actDetail, actSubType);
                    break;

                case EDialogTypes.ucActivityRemoveFromSkid:
                    control = new ucActivityRemoveFromSkid(actDetail, actSubType);
                    break;
            }

            return control;
        }

        /// <summary>
        /// The GetControlObj
        /// </summary>
        /// <param name="typeInnerControl"></param>
        /// <param name="actItem"></param>
        /// <param name="actSubType"></param>
        /// <param name="callingControl"></param>
        /// <returns></returns>
        private IDMobileModal GetControlObj(EDialogTypes typeInnerControl, ActItem actItem, SysCommon.ActSubTypes actSubType, SysCommon.ActSubTypes callingActSubType = SysCommon.ActSubTypes.SkidInfo)
        {
            IDMobileModal control = null;

            switch (typeInnerControl)
            {
                case EDialogTypes.ucActivityDetail:
                    control = new ucActivityDetail(actItem, actSubType);
                    break;

                case EDialogTypes.ucSelectProductUnitFilterToProduct:
                    control = new ucSelectProductUnit(actItem.ProdInfo.Refno, actItem.ProdInfo.UnitRefno, actSubType);
                    break;

                case EDialogTypes.ucSkidInfo:
                    control = new ucActivityHdrSkidInfo(actSubType, actItem);
                    break;

                case EDialogTypes.ucActivityDetailSkid:
                    control = new ucActivityDetailSkid(actItem, actSubType, callingActSubType);
                    break;

                case EDialogTypes.ucActivityInventoryInquiryItem:
                    control = new ucActivityItemInventoryInquiry(actItem, actSubType);
                    break;
            }

            return control;
        }

        /// <summary>
        /// The GetControlObj
        /// </summary>
        /// <param name="typeInnerControl"></param>
        /// <param name="callingControl"></param>
        /// <param name="actSubType"></param>
        /// <returns></returns>
        private IDMobileModal GetControlObj(EDialogTypes typeInnerControl, SysCommon.ActSubTypes actSubType)
        {
            IDMobileModal control = null;

            switch (typeInnerControl)
            {
                case EDialogTypes.ucSelectProductUnit:
                    control = new ucSelectProductUnit(actSubType);
                    break;
            }

            return control;
        }

        /// <summary>
        /// The GetControlObj
        /// </summary>
        /// <param name="typeInnerControl"></param>
        /// <param name="callingControl"></param>
        /// <returns></returns>
        private IDMobileModal GetControlObj(EDialogTypes typeInnerControl)
        {
            IDMobileModal control = null;

            switch (typeInnerControl)
            {
                case EDialogTypes.ucSelectProduct:
                    control = new ucSelectProduct();
                    break;

                case EDialogTypes.ucSelectLocationMap:
                    control = new ucSetLocationMap();
                    break;

                case EDialogTypes.ucActivityHdrPickingFilter:
                    control = new ucActivityHdrPickingFilter();
                    break;

                case EDialogTypes.ucActivityHdrReceivingFilter:
                    control = new ucActivityHdrReceivingFilter();
                    break;

                case EDialogTypes.ucActivityHdrPhysicalCountFilter:
                    control = new ucActivityHdrPhysicalCountFilter();
                    break;

                case EDialogTypes.ucActivityHdrCycleCountFilter:
                    control = new ucActivityHdrCycleCountFilter();
                    break;

                case EDialogTypes.ucActivityHdrReturnsFilter:
                    control = new ucActivityHdrReturnsFilter();
                    break;

                case EDialogTypes.ucActivityHdrTransferFilter:
                    control = new ucActivityHdrTransferFilter();
                    break;

                case EDialogTypes.ucSubmitBatchFilter:
                    control = new ucSubmitBatchFilter();
                    break;

                case EDialogTypes.ucActivityAdjustmentFilter:
                    control = new ucActivityHdrAdjustmentFilter();
                    break;

                case EDialogTypes.ucActivityProductionTicketFinishedFilter:
                    control = new ucActivityHdrProductionTicketFilter();
                    break;

                case EDialogTypes.ucActivityProductionTicketMaterialFilter:
                    control = new ucActivityHdrProductionTicketFilter();
                    break;

                case EDialogTypes.ucActivityProductionTicketByProductFilter:
                    control = new ucActivityHdrProductionTicketFilter();
                    break;

                case EDialogTypes.ucActivityPickingSelectCriteria:
                    control = new ucActivityHdrPickingSelectCriteria();
                    break;

                case EDialogTypes.ucSkidInfo:
                    control = new ucActivityHdrSkidInfo();
                    break;
            }

            return control;
        }

        /// <summary>
        /// The ReqCloseDialogContainer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task ReqCloseDialogContainer(object sender, EventArgs e)
        {
            this.IsControlClosing = true;

            if (this.DisplayControl != null)
            {
                await ((IAppArea)sender).ScannerDisable();
                this.DisplayControlData = await ((IAppArea)sender).GetXML();

                ((IAppPublisher)sender).UnregisterReqClose((IAppListener)this);
                ((IAppPublisher)sender).UnregisterReqSaveEnable((IAppListener)this);
                ((IAppPublisher)sender).UnregisterReqSaveDisable((IAppListener)this);
                ((IAppPublisher)sender).UnregisterReqCancelEnable((IAppListener)this);
                ((IAppPublisher)sender).UnregisterReqCancelDisable((IAppListener)this);
            }
        }
    }
}
