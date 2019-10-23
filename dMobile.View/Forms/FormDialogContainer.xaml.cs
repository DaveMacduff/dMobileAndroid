
//namespace dMobile.View.Forms
//{
//    using dMobile.Bedrock;
//    using dMobile.Controller;
//    using dMobile.View.Activity;
//    using dMobile.View.CustomPages.Modal;
//    using dMobile.View.Options;
//    using System;
//    using System.ComponentModel;
//    using System.Drawing;
//    using System.Xml.Linq;
//    using Xamarin.Forms;
//    using Xamarin.Forms.Xaml;
//    using dMobile.Resources.Enums;
//    using Resource = dMobile.Resources.Properties.Resources;

//    [XamlCompilation(XamlCompilationOptions.Compile)]
//    public partial class FormDialogContainer : DialogModal, IAppListener
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="FormDialogContainer"/> class.
//        /// </summary>
//        public FormDialogContainer()
//        {
//            InitializeComponent();
//        }

//        //private UserControl displayControl;
//        /// <summary>
//        /// Defines the isControlClosing
//        /// </summary>
//        private bool isControlClosing = false;

//        /// <summary>
//        /// Defines the displayControlData
//        /// </summary>
//        private XDocument displayControlData;

//        private ContentView displayControl;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="FormDialogContainer"/> class.
//        /// </summary>
//        /// <param name="typeInnerControl"></param>
//        /// <param name="actSubType"></param>
//        public FormDialogContainer(
//        EDialogTypes typeInnerControl,
//        SysCommon.ActSubTypes actSubType
//        //UserControl callingControl
//        )
//        {
//            ContentView control = null;

//            this.InitializeComponent();

//            //this.menuItemCancel.Enabled = false;
//            //this.menuItemSave.Enabled = false;

//            switch (typeInnerControl)
//            {
//                case EDialogTypes.ucRefreshDataStatus:
//                    control = new ucRefreshDataStatus(actSubType);
//                    break;

//                case EDialogTypes.ucActivityInventoryInquiry:
//                    control = new ucActivityInventoryInquiry();
//                    break;

//                default:
//                    control = this.GetControlObj(typeInnerControl, /*callingControl, */actSubType);

//                    if (control == null)
//                    {
//                        control = this.GetControlObj(typeInnerControl/*, callingControl*/);
//                    }
//                    break;
//            }

//            this.ShowActivityObj(control);
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="FormDialogContainer"/> class.
//        /// </summary>
//        /// <param name="typeInnerControl"></param>
//        /// <param name="callingControl"></param>
//        /// <param name="actSubType"></param>
//        /// <param name="labelQtyEdit"></param>
//        /// <param name="activity"></param>
//        /// <param name="selectMode">The selectMode<see cref="SysCommon.ShippingLabelQuantitySelectionMode"/></param>
//        public FormDialogContainer(
//        EDialogTypes typeInnerControl,
//        //UserControl callingControl,
//        SysCommon.ActSubTypes actSubType,
//        SysCommon.SLabelQuantity labelQtyEdit,
//        Activity activity,
//        SysCommon.ShippingLabelQuantitySelectionMode selectMode)
//        {
//            ContentView control = null;

//            this.InitializeComponent();

//            //this.menuItemCancel.Enabled = false;
//            //this.menuItemSave.Enabled = false;

//            switch (typeInnerControl)
//            {
//                case EDialogTypes.ucActivityShippingLabelEdit:
//                    // Override the subtype to Picking
//                    control = new ucActivityShippingLabelEdit(
//                        SysCommon.ActSubTypes.Picking,
//                        //callingControl,
//                        labelQtyEdit,
//                        activity,
//                        selectMode);
//                    break;
//            }

//            this.ShowActivityObj(control);
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="FormDialogContainer"/> class.
//        /// </summary>
//        /// <param name="typeInnerControl"></param>
//        /// <param name="activity"></param>
//        /// <param name="actSubType"></param>
//        /// <param name="callingControl"></param>
//        public FormDialogContainer(
//        EDialogTypes typeInnerControl,
//        Activity activity,
//        SysCommon.ActSubTypes actSubType
//        //UserControl callingControl
//        )
//        {
//            ContentView control = null;

//            this.InitializeComponent();

//            this.menuItemCancel.Enabled = false;
//            this.menuItemSave.Enabled = false;

//            switch (typeInnerControl)
//            {
//                case EDialogTypes.ucRefreshDataStatus:
//                    control = new ucRefreshDataStatus(actSubType);
//                    break;

//                case EDialogTypes.ucActivityInventoryInquiry:
//                    control = new ucActivityInventoryInquiry();
//                    break;

//                default:
//                    control = this.GetControlObj(typeInnerControl, activity, actSubType/*, callingControl*/);
//                    break;
//            }

//            this.ShowActivityObj(control);
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="FormDialogContainer"/> class.
//        /// </summary>
//        /// <param name="typeInnerControl"></param>
//        /// <param name="actItem"></param>
//        /// <param name="actSubType"></param>
//        /// <param name="callingControl"></param>
//        public FormDialogContainer(
//        EDialogTypes typeInnerControl,
//        ActItem actItem,
//        SysCommon.ActSubTypes actSubType
//        //UserControl callingControl
//        )
//        {
//            ContentView control = null;

//            this.InitializeComponent();

//            //this.menuItemCancel.Enabled = false;
//            //this.menuItemSave.Enabled = false;

//            switch (typeInnerControl)
//            {
//                case EDialogTypes.ucRefreshDataStatus:
//                    control = new ucRefreshDataStatus(actSubType);
//                    break;

//                case EDialogTypes.ucActivityInventoryInquiry:
//                    control = new ucActivityInventoryInquiry(actItem.ProdInfo);
//                    break;

//                default:
//                    control = this.GetControlObj(typeInnerControl, actItem, actSubType/*, callingControl*/);
//                    break;
//            }

//            this.ShowActivityObj(control);
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="FormDialogContainer"/> class.
//        /// </summary>
//        /// <param name="typeInnerControl"></param>
//        /// <param name="actDetail"></param>
//        /// <param name="actSubType"></param>
//        /// <param name="callingControl"></param>
//        public FormDialogContainer(
//        EDialogTypes typeInnerControl,
//        ActDetail actDetail,
//        SysCommon.ActSubTypes actSubType
//        //UserControl callingControl
//        )
//        {
//            ContentView control = null;

//            this.InitializeComponent();

//            //this.menuItemCancel.Enabled = false;
//            //this.menuItemSave.Enabled = false;

//            switch (typeInnerControl)
//            {
//                case EDialogTypes.ucRefreshDataStatus:
//                    control = new ucRefreshDataStatus(actSubType);
//                    break;

//                case EDialogTypes.ucActivityInventoryInquiry:
//                    control = new ucActivityInventoryInquiry(actDetail.ActItem.ProdInfo);
//                    break;

//                default:
//                    control = this.GetControlObj(typeInnerControl, actDetail, actSubType, callingControl);
//                    break;
//            }

//            this.ShowActivityObj(control);
//        }

//        public enum EDialogTypes
//        {
//            /// <summary>
//            /// Defines the ucRefreshDataStatus
//            /// </summary>
//            ucRefreshDataStatus,

//            /// <summary>
//            /// Defines the ucSelectProductUnit
//            /// </summary>
//            ucSelectProductUnit,

//            /// <summary>
//            /// Defines the ucSelectProductUnitFilterToProduct
//            /// </summary>
//            ucSelectProductUnitFilterToProduct,

//            /// <summary>
//            /// Defines the ucSelectLocationMap
//            /// </summary>
//            ucSelectLocationMap,

//            /// <summary>
//            /// Defines the ucActivityItem
//            /// </summary>
//            ucActivityItem,

//            /// <summary>
//            /// Defines the ucActivityDetail
//            /// </summary>
//            ucActivityDetail,

//            /// <summary>
//            /// Defines the ucActivityDetailEdit
//            /// </summary>
//            ucActivityDetailEdit,

//            /// <summary>
//            /// Defines the ucActivityHdrPickingFilter
//            /// </summary>
//            ucActivityHdrPickingFilter,

//            /// <summary>
//            /// Defines the ucActivityHdrReceivingFilter
//            /// </summary>
//            ucActivityHdrReceivingFilter,

//            /// <summary>
//            /// Defines the ucActivityHdrTransferFilter
//            /// </summary>
//            ucActivityHdrTransferFilter,

//            /// <summary>
//            /// Defines the ucActivityHdrPhysicalCountFilter
//            /// </summary>
//            ucActivityHdrPhysicalCountFilter,

//            /// <summary>
//            /// Defines the ucActivityHdrReturnsFilter
//            /// </summary>
//            ucActivityHdrReturnsFilter,

//            /// <summary>
//            /// Defines the ucActivityHdrCycleCountFilter
//            /// </summary>
//            ucActivityHdrCycleCountFilter,

//            /// <summary>
//            /// Defines the ucSubmitBatchFilter
//            /// </summary>
//            ucSubmitBatchFilter,

//            /// <summary>
//            /// Defines the ucActivityAdjustmentFilter
//            /// </summary>
//            ucActivityAdjustmentFilter,

//            /// <summary>
//            /// Defines the ucActivityProductionTicketFinishedFilter
//            /// </summary>
//            ucActivityProductionTicketFinishedFilter,

//            /// <summary>
//            /// Defines the ucActivityProductionTicketMaterialFilter
//            /// </summary>
//            ucActivityProductionTicketMaterialFilter,

//            /// <summary>
//            /// Defines the ucActivityProductionTicketByProductFilter
//            /// </summary>
//            ucActivityProductionTicketByProductFilter,

//            /// <summary>
//            /// Defines the ucActivityPurchaseOrderWorkSheetEdit
//            /// </summary>
//            ucActivityPurchaseOrderWorkSheetEdit,

//            /// <summary>
//            /// Defines the ucActivityPickingSelectCriteria
//            /// </summary>
//            ucActivityPickingSelectCriteria,

//            /// <summary>
//            /// Defines the ucActivityShippingLabelEdit
//            /// </summary>
//            ucActivityShippingLabelEdit,

//            /// <summary>
//            /// Defines the ucActivityConvertUnitEdit
//            /// </summary>
//            ucActivityConvertUnitEdit,

//            /// <summary>
//            /// Defines the ucSkidInfo
//            /// </summary>
//            ucSkidInfo,

//            /// <summary>
//            /// Defines the ucSkidInfoEdit
//            /// </summary>
//            ucSkidInfoEdit,

//            /// <summary>
//            /// Defines the ucActivityDetailSkid
//            /// </summary>
//            ucActivityDetailSkid,

//            /// <summary>
//            /// Defines the ucActivityItemSingle
//            /// </summary>
//            ucActivityItemSingle,

//            /// <summary>
//            /// Defines the ucActivityInventoryInquiry
//            /// </summary>
//            ucActivityInventoryInquiry,

//            /// <summary>
//            /// Defines the ucActivityInventoryInquiryItem
//            /// </summary>
//            ucActivityInventoryInquiryItem,

//            /// <summary>
//            /// Defines the ucActivityInventoryInquiryDetail
//            /// </summary>
//            ucActivityInventoryInquiryDetail,

//            /// <summary>
//            /// Defines the ucSelectProduct
//            /// </summary>
//            ucSelectProduct,

//            /// <summary>
//            /// Defines the ucActivityProductLabel
//            /// </summary>
//            ucActivityProductLabel
//        }

//        /// <summary>
//        /// Gets or sets the DisplayControlData
//        /// </summary>
//        public XDocument DisplayControlData
//        {
//            get
//            {
//                return this.displayControlData;
//            }

//            set
//            {
//                this.displayControlData = value;
//            }
//        }

//        /// <summary>
//        /// Gets or sets the DisplayControl
//        /// </summary>
//        public ContentView DisplayControl
//        {
//            get
//            {
//                return this.displayControl;
//            }

//            set
//            {
//                this.displayControl = value;
//            }
//        }

//        /// <summary>
//        /// Gets or sets a value indicating whether IsControlClosing
//        /// </summary>
//        public bool IsControlClosing
//        {
//            get
//            {
//                return this.isControlClosing;
//            }

//            set
//            {
//                this.isControlClosing = value;
//            }
//        }

//        /// <summary>
//        /// The XML.
//        /// </summary>
//        /// <returns></returns>
//        public XDocument GetXML()
//        {
//            if (this.DisplayControl == null)
//            {
//                return null;
//            }
//            else
//            {
//                return ((IAppArea)this.DisplayControl).GetXML();
//            }
//        }

//        /// <summary>
//        /// The SaveChanges
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        public void SaveChanges(object sender, EventArgs e)
//        {
//            //this.menuItemSave.Enabled = false;

//            if (this.DisplayControl == null)
//            {
//                this.ReqCloseDialogContainer(this.DisplayControl, EventArgs.Empty);
//            }
//            else
//            {
//                if (((IAppArea)this.DisplayControl).SaveChanges())
//                {
//                    this.ReqCloseDialogContainer(this.DisplayControl, EventArgs.Empty);
//                    if (this.IsControlClosing)
//                    {
//                        this.DialogResult = DialogResult.OK;
//                    }
//                    else
//                    {
//                        this.DialogResult = DialogResult.Cancel;
//                    }
//                }
//            }

//            //this.menuItemSave.Enabled = true;
//        }

//        /// <summary>
//        /// The menuItemCancel_Click
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        public void menuItemCancel_Click(object sender, EventArgs e)
//        {
//            //this.menuItemCancel.Enabled = false;

//            if (this.DisplayControl == null)
//            {
//                this.ReqCloseDialogContainer(this.DisplayControl, EventArgs.Empty);
//            }
//            else
//            {
//                if (((IAppArea)this.DisplayControl).CancelChanges())
//                {
//                    this.ReqCloseDialogContainer(this.DisplayControl, EventArgs.Empty);
//                }
//            }

//            //this.menuItemCancel.Enabled = true;
//        }

//        /// <summary>
//        /// The FormDialogContainer_Closing
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        public void FormDialogContainer_Closing(object sender, CancelEventArgs e)
//        {
//            if (this.IsControlClosing)
//            {
//                e.Cancel = false;
//            }
//            else
//            {
//                e.Cancel = true;
//            }
//        }

//        /// <summary>
//        /// The FormDialogContainer_GotFocus
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        public void FormDialogContainer_GotFocus(object sender, EventArgs e)
//        {
//            //this.panelContents.Focus();
//        }

//        /// <summary>
//        /// The FormDialogContainer_Load
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        public void FormDialogContainer_Load(object sender, EventArgs e)
//        {
//            try
//            {
//                //int platform = (int)SysPlatformDetector.GetPlatform();

//                ////adjust the form size and pnlMain location depending on the OS
//                ////Windows CE menus appear at the top versus at the bottom for PPC
//                //if (platform == Convert.ToInt32(Platform.WindowsCE))
//                //{
//                //    this.Size = new Size(321, 241);
//                //    panelContents.Location = new Point(0, 27);

//                //    this.mainMenu.MenuItems[0].Text = "      Save        ";
//                //    this.mainMenu.MenuItems[1].Text = "       Cancel     ";

//                //}
//                //else
//                //{
//                //    if (platform == Convert.ToInt32(Platform.PocketPC))
//                //    {
//                //        this.WindowState = FormWindowState.Normal;
//                //    }

//                //    this.Size = new Size(321, 241);
//                //    this.panelContents.Location = new Point(0, 27);

//                //    this.mainMenu.MenuItems[0].Text = "    Save    ";
//                //    this.mainMenu.MenuItems[1].Text = "   Cancel   ";
//                //}
//            }
//            catch (Exception ex)
//            {
//                //MessageBox.Show(
//                //  ex.Message,
//                //  "dMobile",
//                //  MessageBoxButtons.OK,
//                //  MessageBoxIcon.Exclamation,
//                //  MessageBoxDefaultButton.Button1);

//                DisplayAlert("dMobile", ex.Message, Resource.OK);
//            }
//        }

//        /// <summary>
//        /// The ReqClose
//        /// </summary>
//        /// <param name="appEventPub"></param>
//        /// <param name="appEventArgs"></param>
//        public void ReqClose(IAppPublisher appEventPub, AppEventArgs appEventArgs)
//        {
//            this.ReqCloseDialogContainer(appEventPub, appEventArgs);
//        }

//        /// <summary>
//        /// The ReqSaveEnable
//        /// </summary>
//        /// <param name="appEventPub"></param>
//        /// <param name="appEventArgs"></param>
//        public void ReqSaveEnable(IAppPublisher appEventPub, AppEventArgs appEventArgs)
//        {
//            //this.menuItemSave.Enabled = true;
//        }

//        /// <summary>
//        /// The ReqSaveDisable
//        /// </summary>
//        /// <param name="appEventPub"></param>
//        /// <param name="appEventArgs"></param>
//        public void ReqSaveDisable(IAppPublisher appEventPub, AppEventArgs appEventArgs)
//        {
//            //this.menuItemSave.Enabled = false;
//        }

//        /// <summary>
//        /// The ReqCancelEnable
//        /// </summary>
//        /// <param name="appEventPub"></param>
//        /// <param name="appEventArgs"></param>
//        public void ReqCancelEnable(IAppPublisher appEventPub, AppEventArgs appEventArgs)
//        {
//            //this.menuItemCancel.Enabled = true;
//        }

//        /// <summary>
//        /// The ReqCancelDisable
//        /// </summary>
//        /// <param name="appEventPub"></param>
//        /// <param name="appEventArgs"></param>
//        public void ReqCancelDisable(IAppPublisher appEventPub, AppEventArgs appEventArgs)
//        {
//            //this.menuItemCancel.Enabled = false;
//        }

//        /// <summary>
//        /// The ShowActivityObj
//        /// </summary>
//        /// <param name="control"></param>
//        private void ShowActivityObj(ContentView control)
//        {
//            if (control == null)
//            {
//                throw new Exception("Activity control could not be created. Check if the EDialogType was defined.");
//            }
//            else
//            {
//                //panelContents.Controls.Add(control);
//                //control.Dock = DockStyle.Fill;

//                ((IAppPublisher)control).RegisterReqClose((IAppListener)this);
//                ((IAppPublisher)control).RegisterReqSaveEnable((IAppListener)this);
//                ((IAppPublisher)control).RegisterReqSaveDisable((IAppListener)this);
//                ((IAppPublisher)control).RegisterReqCancelEnable((IAppListener)this);
//                ((IAppPublisher)control).RegisterReqCancelDisable((IAppListener)this);

//                //this.Text = ((IAppArea)control).GetControlDescription();
//                //control.BringToFront();
//                this.DisplayControl = control;
//                control.Focus();
//            }
//        }

//        /// <summary>
//        /// The GetControlObj
//        /// </summary>
//        /// <param name="typeInnerControl"></param>
//        /// <param name="actSubType"></param>
//        /// <param name="callingControl"></param>
//        /// <param name="labelQtyEdit"></param>
//        /// <param name="activity"></param>
//        /// <param name="selectMode">The selectMode<see cref="SysCommon.ShippingLabelQuantitySelectionMode"/></param>
//        /// <returns></returns>
//        private ContentView GetControlObj(
//        EDialogTypes typeInnerControl,
//        SysCommon.ActSubTypes actSubType,
//        //UserControl callingControl,
//        SysCommon.SLabelQuantity labelQtyEdit,
//        Activity activity,
//        SysCommon.ShippingLabelQuantitySelectionMode selectMode)
//        {
//            ContentView control = null;

//            switch (typeInnerControl)
//            {
//                case EDialogTypes.ucActivityShippingLabelEdit:
//                    control = new ucActivityShippingLabelEdit(actSubType, /*callingControl,*/ labelQtyEdit, activity, selectMode);
//                    break;
//            }

//            return control;
//        }

//        /// <summary>
//        /// The GetControlObj
//        /// </summary>
//        /// <param name="typeInnerControl"></param>
//        /// <param name="activity"></param>
//        /// <param name="actSubType"></param>
//        /// <param name="callingControl"></param>
//        /// <returns></returns>
//        private ContentView GetControlObj(
//        EDialogTypes typeInnerControl,
//        Activity activity,
//        SysCommon.ActSubTypes actSubType
//        //UserControl callingControl
//        )
//        {
//            ContentView control = null;

//            switch (typeInnerControl)
//            {
//                case EDialogTypes.ucActivityItem:
//                    control = new ucActivityItem(activity, actSubType);
//                    break;

//                case EDialogTypes.ucActivityItemSingle:
//                    control = new ucActivityItemSingle(activity, actSubType);
//                    break;

//                case EDialogTypes.ucSkidInfo:
//                    control = new ucActivityHdrSkidInfo(callingControl, actSubType, activity);
//                    break;
//            }

//            return control;
//        }

//        /// <summary>
//        /// The GetControlObj
//        /// </summary>
//        /// <param name="typeInnerControl"></param>
//        /// <param name="actDetail"></param>
//        /// <param name="actSubType"></param>
//        /// <param name="callingControl"></param>
//        /// <returns></returns>
//        private ContentView GetControlObj(
//        EDialogTypes typeInnerControl,
//        ActDetail actDetail,
//        SysCommon.ActSubTypes actSubType
//        //UserControl callingControl
//        )
//        {
//            ContentView control = null;

//            switch (typeInnerControl)
//            {
//                case EDialogTypes.ucActivityDetailEdit:
//                    control = new ucActivityDetailEdit(actDetail, actSubType);
//                    break;

//                case EDialogTypes.ucActivityProductLabel:
//                    control = new ucActivityProductLabel(actDetail, actSubType);
//                    break;

//                case EDialogTypes.ucActivityPurchaseOrderWorkSheetEdit:
//                    // Override the subtype to PurchaseOrderWorkSheet
//                    control = new ucActivityItemPurchaseOrderWorkSheetEdit(
//                        actDetail,
//                        SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
//                    break;

//                case EDialogTypes.ucActivityConvertUnitEdit:
//                    // Override the subtype to ConvertUnit
//                    control = new ucActivityConvertUnitEdit(actDetail, SysCommon.ActSubTypes.ConvertUnit);
//                    break;

//                case EDialogTypes.ucActivityInventoryInquiryDetail:
//                    control = new ucActivityDetailInventoryInquiry(actDetail, actSubType);
//                    break;
//            }

//            return control;
//        }

//        /// <summary>
//        /// The GetControlObj
//        /// </summary>
//        /// <param name="typeInnerControl"></param>
//        /// <param name="actItem"></param>
//        /// <param name="actSubType"></param>
//        /// <param name="callingControl"></param>
//        /// <returns></returns>
//        private ContentView GetControlObj(
//        EDialogTypes typeInnerControl,
//        ActItem actItem,
//        SysCommon.ActSubTypes actSubType
//        //UserControl callingControl
//        )
//        {
//            ContentView control = null;

//            switch (typeInnerControl)
//            {
//                case EDialogTypes.ucActivityDetail:
//                    control = new ucActivityDetail(actItem, actSubType);
//                    break;

//                case EDialogTypes.ucSelectProductUnitFilterToProduct:
//                    control = new ucSelectProductUnit(
//                        //callingControl,
//                        actItem.ProdInfo.Refno,
//                        actItem.ProdInfo.UnitRefno,
//                        actSubType);
//                    break;

//                case EDialogTypes.ucSkidInfo:
//                    control = new ucActivityHdrSkidInfo(/*callingControl,*/ actSubType, actItem);
//                    break;

//                case EDialogTypes.ucActivityDetailSkid:
//                    control = new ucActivityDetailSkid(actItem, actSubType);
//                    break;

//                case EDialogTypes.ucActivityInventoryInquiryItem:
//                    control = new ucActItemInventoryInquiry(actItem, actSubType);
//                    break;
//            }

//            return control;
//        }

//        /// <summary>
//        /// The GetControlObj
//        /// </summary>
//        /// <param name="typeInnerControl"></param>
//        /// <param name="callingControl"></param>
//        /// <param name="actSubType"></param>
//        /// <returns></returns>
//        private ContentView GetControlObj(
//        EDialogTypes typeInnerControl,
//        //UserControl callingControl,
//        SysCommon.ActSubTypes actSubType)
//        {
//            ContentView control = null;

//            switch (typeInnerControl)
//            {
//                case EDialogTypes.ucSelectProductUnit:
//                    control = new ucSelectProductUnit(/*callingControl, */actSubType);
//                    break;
//            }

//            return control;
//        }

//        /// <summary>
//        /// The GetControlObj
//        /// </summary>
//        /// <param name="typeInnerControl"></param>
//        /// <param name="callingControl"></param>
//        /// <returns></returns>
//        private ContentView GetControlObj(EDialogTypes typeInnerControl/*, UserControl callingControl*/)
//        {
//            ContentView control = null;

//            switch (typeInnerControl)
//            {
//                case EDialogTypes.ucSelectProduct:
//                    control = new ucSelectProduct(/*callingControl*/);
//                    break;

//                case EDialogTypes.ucSelectLocationMap:
//                    control = new ucSetLocationMap(/*callingControl*/);
//                    break;

//                case EDialogTypes.ucActivityHdrPickingFilter:
//                    control = new ucActivityHdrPickingFilter(/*callingControl*/);
//                    break;

//                case EDialogTypes.ucActivityHdrReceivingFilter:
//                    control = new ucActivityHdrReceivingFilter(/*callingControl*/);
//                    break;

//                case EDialogTypes.ucActivityHdrPhysicalCountFilter:
//                    control = new ucActivityHdrPhysicalCountFilter(/*callingControl*/);
//                    break;

//                case EDialogTypes.ucActivityHdrCycleCountFilter:
//                    control = new ucActivityHdrCycleCountFilter(/*callingControl*/);
//                    break;

//                case EDialogTypes.ucActivityHdrReturnsFilter:
//                    control = new ucActivityHdrReturnsFilter(/*callingControl*/);
//                    break;

//                case EDialogTypes.ucActivityHdrTransferFilter:
//                    control = new ucActivityHdrTransferFilter(/*callingControl*/);
//                    break;

//                case EDialogTypes.ucSubmitBatchFilter:
//                    control = new ucSubmitBatchFilter(/*callingControl*/);
//                    break;

//                case EDialogTypes.ucActivityAdjustmentFilter:
//                    control = new ucActivityHdrAdjustmentFilter(/*callingControl*/);
//                    break;

//                case EDialogTypes.ucActivityProductionTicketFinishedFilter:
//                    control = new ucActivityHdrProductionTicketFilter(/*callingControl*/);
//                    break;

//                case EDialogTypes.ucActivityProductionTicketMaterialFilter:
//                    control = new ucActivityHdrProductionTicketFilter(/*callingControl*/);
//                    break;

//                case EDialogTypes.ucActivityProductionTicketByProductFilter:
//                    control = new ucActivityHdrProductionTicketFilter(/*callingControl*/);
//                    break;

//                case EDialogTypes.ucActivityPickingSelectCriteria:
//                    control = new ucActivityHdrPickingSelectCriteria(/*callingControl*/);
//                    break;

//                case EDialogTypes.ucSkidInfo:
//                    control = new ucActivityHdrSkidInfo(/*callingControl*/);
//                    break;
//            }

//            return control;
//        }

//        /// <summary>
//        /// The ReqCloseDialogContainer
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void ReqCloseDialogContainer(object sender, EventArgs e)
//        {
//            this.IsControlClosing = true;

//            if (this.DisplayControl != null)
//            {
//                ((IAppArea)sender).ScannerDisable();
//                this.DisplayControlData = ((IAppArea)sender).GetXML();

//                ((IAppPublisher)sender).UnregisterReqClose((IAppListener)this);
//                ((IAppPublisher)sender).UnregisterReqSaveEnable((IAppListener)this);
//                ((IAppPublisher)sender).UnregisterReqSaveDisable((IAppListener)this);
//                ((IAppPublisher)sender).UnregisterReqCancelEnable((IAppListener)this);
//                ((IAppPublisher)sender).UnregisterReqCancelDisable((IAppListener)this);

//                //((UserControl)sender).Dispose();
//            }

//            this.Close();

//            GC.Collect();
//            GC.WaitForPendingFinalizers();
//        }
//    }
//}
