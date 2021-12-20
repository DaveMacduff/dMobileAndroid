using Android.Views;
using dMobile.Bedrock;
using dMobile.GroundWork;
using dMobile.Resources.Enums;
using dMobile.Resources.Types;
using dMobile.View.CustomPages.Modal;
using dMobile.View.Forms;
using SQLite;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Resource = dMobile.Resources.Properties.Resources;

namespace dMobile.View.Options
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucRefreshDataStatus : DMobileModal, IAppArea, ISysSettingsEventListener,
        IBarCodeSettingsEventListener, IPrnSettingsListener, IProdSchemeLabelSettingsListener,
        IDataNubListener, IDataAccessEventListener
    {
        private string fullClassName = typeof(ucRefreshDataStatus).FullName;
        private DateTime refreshTotal = DateTime.Now;
        private SysCommon.ActSubTypes actSubTypes = SysCommon.ActSubTypes.NotApplicable;
        private bool isRefreshComplete = false;
        private SysEnvironment sysEnv;
        private TableLocationMapWarehouse locMapWarehouse;
        private TableLocationMapSection locMapSection;
        private TableLocationMapType locMapType;
        private TableLocationMapSlotCoordinate locMapSlotCoordinate;
        private TableLocationMapSlot locMapSlot;
        private TableWarehouseLocationMapType whsLocMapType;
        private TableUnit unit;
        private TableRoute route;
        private TableProductScheme prodScheme;
        private TableProductSchemeVariant prodSchemeVariant;
        private TableProductSchemeVariantXRef prodSchemeVariantXRef;
        private TableProductSchemeLotNumber prodSchemeLot;
        private TableProductSchemeLotNumberXRef prodSchemeLotXRef;
        private TableProduct product;
        private TableFamily family;
        private TableProductUnit prodUnit;
        private TableProductUnitRelation prodUnitRelation;
        private TableProductUnitBarCode prodUnitBarCode;
        private TableProductLocationMapType prodLocMapType;
        private TableLocationMapWarehouseProductScheme prodLocMapWhsProdScheme;
        private TableContactUsers contactUsers;
        private TableActivityPhysicalCount physicalCount;
        private TableActivityPhysicalCountItem physicalCountItem;
        private TableActivityPhysicalCountDetail physicalCountDetail;
        private TableActivityCycleCount cycleCount;
        private TableActivityCycleCountItem cycleCountItem;
        private TableActivityCycleCountDetail cycleCountDetail;
        private TableActivityDetailProductSchemeVariantXRef actDetailProdSchemeVariantXRef;
        private TableActivityItemBarCodeAssociationXRef actItemBarCodeAssocXRef;
        private TableActivityPicking picking;
        private TableActivityPickingItem pickingItem;
        private TableActivityPickingItemComment pickingItemComment;
        private TableActivityPickingDetail pickingDetail;
        private TableActivityPickingRoute pickingRoute;
        private TableActivityReceiving receiving;
        private TableActivityRecevingItem receivingItem;
        private TableActivityReceivingItemComment receivingItemComment;
        private TableActivityReceivingDetail receivingDetail;
        private TableActivityTransferFrom transferFrom;
        private TableActivityTransferFromItem transferFromItem;
        private TableActivityTransferFromDetail transferFromDetail;
        private TableActivityTransferTo transferTo;
        private TableActivityTransferToItem transferToItem;
        private TableActivityTransferToDetail transferToDetail;
        private TableActivityReturns returns;
        private TableActivityReturnsItem returnsItem;
        private TableActivityReturnsItemComment returnsItemComment;
        private TableActivityReturnsDetail returnsDetail;
        private TableActivityAdjustment adjustment;
        private TableActivityAdjustmentItem adjustmentItem;
        private TableActivityAdjustmentDetail adjustmentDetail;
        private TableActivity activity;
        private TableActivityItem activityItem;
        private TableActivityProductionTicketFinished ticketFinished;
        private TableActivityProductionTicketMaterial ticketMaterial;
        private TableActivityProductionTicketByProduct ticketByProduct;
        private TableActivityProductionTicketFinishedItem ticketFinishedItem;
        private TableActivityProductionTicketMaterialItem ticketMaterialItem;
        private TableActivityProductionTicketByProductItem ticketByProductItem;
        private TableActivityProductionTicketFinishedDetail ticketDetailFinished;
        private TableActivityProductionTicketMaterialDetail ticketDetailMaterial;
        private TableActivityProductionTicketByProductDetail ticketDetailByProduct;
        private TableActivityPurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem;
        private PrinterSettings printerSettings;
        private ProdSchemeLabelSettings schemeLabelSettings;
        private ObservableCollection<Message> itemsList;
        private bool isFirstAppearance = true;
        private int messageIndex = 0;

        public ucRefreshDataStatus()
        {
            InitializeComponent();
        }

        public ucRefreshDataStatus(SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.Title = GetControlDescription();

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.locMapWarehouse = new TableLocationMapWarehouse();
                this.locMapSection = new TableLocationMapSection();
                this.locMapType = new TableLocationMapType();
                this.locMapSlotCoordinate = new TableLocationMapSlotCoordinate();
                this.locMapSlot = new TableLocationMapSlot();
                this.whsLocMapType = new TableWarehouseLocationMapType();
                this.unit = new TableUnit();
                this.route = new TableRoute();
                this.prodScheme = new TableProductScheme();
                this.prodSchemeVariant = new TableProductSchemeVariant();
                this.prodSchemeVariantXRef = new TableProductSchemeVariantXRef();
                this.prodSchemeLot = new TableProductSchemeLotNumber();
                this.prodSchemeLotXRef = new TableProductSchemeLotNumberXRef();
                this.product = new TableProduct();
                this.family = new TableFamily();
                this.prodUnit = new TableProductUnit();
                this.prodUnitRelation = new TableProductUnitRelation();
                this.prodUnitBarCode = new TableProductUnitBarCode();
                this.prodLocMapType = new TableProductLocationMapType();
                this.prodLocMapWhsProdScheme = new TableLocationMapWarehouseProductScheme();
                this.contactUsers = new TableContactUsers();
                this.physicalCount = new TableActivityPhysicalCount();
                this.physicalCountItem = new TableActivityPhysicalCountItem();
                this.physicalCountDetail = new TableActivityPhysicalCountDetail();
                this.cycleCount = new TableActivityCycleCount();
                this.cycleCountItem = new TableActivityCycleCountItem();
                this.cycleCountDetail = new TableActivityCycleCountDetail();
                this.actDetailProdSchemeVariantXRef = new TableActivityDetailProductSchemeVariantXRef();
                this.actItemBarCodeAssocXRef = new TableActivityItemBarCodeAssociationXRef();
                this.picking = new TableActivityPicking();
                this.pickingItem = new TableActivityPickingItem();
                this.pickingItemComment = new TableActivityPickingItemComment();
                this.pickingDetail = new TableActivityPickingDetail();
                this.pickingRoute = new TableActivityPickingRoute();
                this.receiving = new TableActivityReceiving();
                this.receivingItem = new TableActivityRecevingItem();
                this.receivingItemComment = new TableActivityReceivingItemComment();
                this.receivingDetail = new TableActivityReceivingDetail();
                this.transferFrom = new TableActivityTransferFrom();
                this.transferFromItem = new TableActivityTransferFromItem();
                this.transferFromDetail = new TableActivityTransferFromDetail();
                this.transferTo = new TableActivityTransferTo();
                this.transferToItem = new TableActivityTransferToItem();
                this.transferToDetail = new TableActivityTransferToDetail();
                this.returns = new TableActivityReturns();
                this.returnsItem = new TableActivityReturnsItem();
                this.returnsItemComment = new TableActivityReturnsItemComment();
                this.returnsDetail = new TableActivityReturnsDetail();
                this.adjustment = new TableActivityAdjustment();
                this.adjustmentItem = new TableActivityAdjustmentItem();
                this.adjustmentDetail = new TableActivityAdjustmentDetail();
                this.activity = new TableActivity();
                this.activityItem = new TableActivityItem();
                this.ticketFinished = new TableActivityProductionTicketFinished();
                this.ticketMaterial = new TableActivityProductionTicketMaterial();
                this.ticketByProduct = new TableActivityProductionTicketByProduct();
                this.ticketFinishedItem = new TableActivityProductionTicketFinishedItem();
                this.ticketMaterialItem = new TableActivityProductionTicketMaterialItem();
                this.ticketByProductItem = new TableActivityProductionTicketByProductItem();
                this.ticketDetailFinished = new TableActivityProductionTicketFinishedDetail();
                this.ticketDetailMaterial = new TableActivityProductionTicketMaterialDetail();
                this.ticketDetailByProduct = new TableActivityProductionTicketByProductDetail();
                this.purchaseOrderWorkSheetItem = new TableActivityPurchaseOrderWorkSheetItem();
                this.printerSettings = new PrinterSettings();
                this.schemeLabelSettings = new ProdSchemeLabelSettings();
                this.itemsList = new ObservableCollection<Message>();
            }

            this.ActSubType = actSubTypes;
        }

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.lstStatus.ItemsSource = this.ItemsList;

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);
            });

            if (this.isFirstAppearance)
            {
                this.isFirstAppearance = false;
                await this.RefreshDataStatus_GotFocus(this, new EventArgs());
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                base.OnBackButtonPressed();

                if (await this.SaveChanges())
                {
                    this.DialogResult = DialogResult.OK;
                    await this.Close();
                }
                else if (await this.CancelChanges())
                {
                    this.DialogResult = DialogResult.None;
                    await this.Close();
                }
            });

            return true;
        }

        private SysEnvironment SysEnv
        {
            get
            {
                return this.sysEnv;
            }

            set
            {
                this.sysEnv = value;
            }
        }

        private bool IsRefreshComplete
        {
            get
            {
                return this.isRefreshComplete;
            }

            set
            {
                this.isRefreshComplete = value;
            }
        }

        private SysCommon.ActSubTypes ActSubType
        {
            get
            {
                return this.actSubTypes;
            }

            set
            {
                this.actSubTypes = value;
            }
        }

        private string FullClassName
        {
            get
            {
                return this.fullClassName;
            }

            set
            {
                this.fullClassName = value;
            }
        }

        private DateTime RefreshTotal
        {
            get
            {
                return this.refreshTotal;
            }

            set
            {
                this.refreshTotal = value;
            }
        }

        private TableProductLocationMapType ProdLocMapType
        {
            get
            {
                return this.prodLocMapType;
            }

            set
            {
                this.prodLocMapType = value;
            }
        }

        private TableUnit Unit
        {
            get
            {
                return this.unit;
            }

            set
            {
                this.unit = value;
            }
        }

        private TableRoute Route
        {
            get
            {
                return this.route;
            }

            set
            {
                this.route = value;
            }
        }

        private TableProductScheme ProdScheme
        {
            get
            {
                return this.prodScheme;
            }

            set
            {
                this.prodScheme = value;
            }
        }

        private TableProductSchemeVariant ProdSchemeVariant
        {
            get
            {
                return this.prodSchemeVariant;
            }

            set
            {
                this.prodSchemeVariant = value;
            }
        }

        private TableProductSchemeVariantXRef ProdSchemeVariantXRef
        {
            get
            {
                return this.prodSchemeVariantXRef;
            }

            set
            {
                this.prodSchemeVariantXRef = value;
            }
        }

        private TableProductSchemeLotNumber ProdSchemeLot
        {
            get
            {
                return this.prodSchemeLot;
            }

            set
            {
                this.prodSchemeLot = value;
            }
        }

        private TableProductSchemeLotNumberXRef ProdSchemeLotXRef
        {
            get
            {
                return this.prodSchemeLotXRef;
            }

            set
            {
                this.prodSchemeLotXRef = value;
            }
        }

        private TableProduct Product
        {
            get
            {
                return this.product;
            }

            set
            {
                this.product = value;
            }
        }

        private TableFamily Family
        {
            get
            {
                return this.family;
            }

            set
            {
                this.family = value;
            }
        }

        private TableProductUnit ProdUnit
        {
            get
            {
                return this.prodUnit;
            }

            set
            {
                this.prodUnit = value;
            }
        }

        private TableProductUnitRelation ProdUnitRelation
        {
            get
            {
                return this.prodUnitRelation;
            }

            set
            {
                this.prodUnitRelation = value;
            }
        }

        private TableProductUnitBarCode ProdUnitBarCode
        {
            get
            {
                return this.prodUnitBarCode;
            }

            set
            {
                this.prodUnitBarCode = value;
            }
        }

        private TableLocationMapWarehouse LocMapWarehouse
        {
            get
            {
                return this.locMapWarehouse;
            }

            set
            {
                this.locMapWarehouse = value;
            }
        }

        private TableLocationMapSection LocMapSection
        {
            get
            {
                return this.locMapSection;
            }

            set
            {
                this.locMapSection = value;
            }
        }

        private TableLocationMapType LocMapType
        {
            get
            {
                return this.locMapType;
            }

            set
            {
                this.locMapType = value;
            }
        }

        private TableLocationMapSlotCoordinate LocMapSlotCoordinate
        {
            get
            {
                return this.locMapSlotCoordinate;
            }

            set
            {
                this.locMapSlotCoordinate = value;
            }
        }

        private TableLocationMapSlot LocMapSlot
        {
            get
            {
                return this.locMapSlot;
            }

            set
            {
                this.locMapSlot = value;
            }
        }

        private TableWarehouseLocationMapType WhsLocMapType
        {
            get
            {
                return this.whsLocMapType;
            }

            set
            {
                this.whsLocMapType = value;
            }
        }

        private TableContactUsers ContactUsers
        {
            get
            {
                return this.contactUsers;
            }

            set
            {
                this.contactUsers = value;
            }
        }

        private TableActivityPhysicalCount PhysicalCount
        {
            get
            {
                return this.physicalCount;
            }

            set
            {
                this.physicalCount = value;
            }
        }

        private TableActivityPhysicalCountItem PhysicalCountItem
        {
            get
            {
                return this.physicalCountItem;
            }

            set
            {
                this.physicalCountItem = value;
            }
        }

        private TableActivityPhysicalCountDetail PhysicalCountDetail
        {
            get
            {
                return this.physicalCountDetail;
            }

            set
            {
                this.physicalCountDetail = value;
            }
        }

        private TableActivityCycleCount CycleCount
        {
            get
            {
                return this.cycleCount;
            }

            set
            {
                this.cycleCount = value;
            }
        }

        private TableActivityCycleCountItem CycleCountItem
        {
            get
            {
                return this.cycleCountItem;
            }

            set
            {
                this.cycleCountItem = value;
            }
        }

        private TableActivityCycleCountDetail CycleCountDetail
        {
            get
            {
                return this.cycleCountDetail;
            }

            set
            {
                this.cycleCountDetail = value;
            }
        }

        private TableActivityDetailProductSchemeVariantXRef ActDetailProdSchemeVariantXRef
        {
            get
            {
                return this.actDetailProdSchemeVariantXRef;
            }

            set
            {
                this.actDetailProdSchemeVariantXRef = value;
            }
        }

        private TableActivityItemBarCodeAssociationXRef ActItemBarCodeAssocXRef
        {
            get
            {
                return this.actItemBarCodeAssocXRef;
            }

            set
            {
                this.actItemBarCodeAssocXRef = value;
            }
        }

        private TableActivityPicking Picking
        {
            get
            {
                return this.picking;
            }

            set
            {
                this.picking = value;
            }
        }

        private TableActivityPickingItem PickingItem
        {
            get
            {
                return this.pickingItem;
            }

            set
            {
                this.pickingItem = value;
            }
        }

        private TableActivityPickingItemComment PickingItemComment
        {
            get
            {
                return this.pickingItemComment;
            }

            set
            {
                this.pickingItemComment = value;
            }
        }

        private TableActivityPickingDetail PickingDetail
        {
            get
            {
                return this.pickingDetail;
            }

            set
            {
                this.pickingDetail = value;
            }
        }

        private TableActivityPickingRoute PickingRoute
        {
            get
            {
                return this.pickingRoute;
            }

            set
            {
                this.pickingRoute = value;
            }
        }

        private TableActivityReceiving Receiving
        {
            get
            {
                return this.receiving;
            }

            set
            {
                this.receiving = value;
            }
        }

        private TableActivityRecevingItem ReceivingItem
        {
            get
            {
                return this.receivingItem;
            }

            set
            {
                this.receivingItem = value;
            }
        }

        private TableActivityReceivingItemComment ReceivingItemComment
        {
            get
            {
                return this.receivingItemComment;
            }

            set
            {
                this.receivingItemComment = value;
            }
        }

        private TableActivityReceivingDetail ReceivingDetail
        {
            get
            {
                return this.receivingDetail;
            }

            set
            {
                this.receivingDetail = value;
            }
        }

        private TableActivityTransferFrom TransferFrom
        {
            get
            {
                return this.transferFrom;
            }

            set
            {
                this.transferFrom = value;
            }
        }

        private TableActivityTransferFromItem TransferFromItem
        {
            get
            {
                return this.transferFromItem;
            }

            set
            {
                this.transferFromItem = value;
            }
        }

        private TableActivityTransferFromDetail TransferFromDetail
        {
            get
            {
                return this.transferFromDetail;
            }

            set
            {
                this.transferFromDetail = value;
            }
        }

        private TableActivityTransferTo TransferTo
        {
            get
            {
                return this.transferTo;
            }

            set
            {
                this.transferTo = value;
            }
        }

        private TableActivityTransferToItem TransferToItem
        {
            get
            {
                return this.transferToItem;
            }

            set
            {
                this.transferToItem = value;
            }
        }

        private TableActivityTransferToDetail TransferToDetail
        {
            get
            {
                return this.transferToDetail;
            }

            set
            {
                this.transferToDetail = value;
            }
        }

        private TableActivityReturns Returns
        {
            get
            {
                return this.returns;
            }

            set
            {
                this.returns = value;
            }
        }

        private TableActivityReturnsItem ReturnsItem
        {
            get
            {
                return this.returnsItem;
            }

            set
            {
                this.returnsItem = value;
            }
        }

        private TableActivityReturnsItemComment ReturnsItemComment
        {
            get
            {
                return this.returnsItemComment;
            }

            set
            {
                this.returnsItemComment = value;
            }
        }

        private TableActivityReturnsDetail ReturnsDetail
        {
            get
            {
                return this.returnsDetail;
            }

            set
            {
                this.returnsDetail = value;
            }
        }

        private TableActivityAdjustment Adjustment
        {
            get
            {
                return this.adjustment;
            }

            set
            {
                this.adjustment = value;
            }
        }

        private TableActivityAdjustmentItem AdjustmentItem
        {
            get
            {
                return this.adjustmentItem;
            }

            set
            {
                this.adjustmentItem = value;
            }
        }

        private TableActivityAdjustmentDetail AdjustmentDetail
        {
            get
            {
                return this.adjustmentDetail;
            }

            set
            {
                this.adjustmentDetail = value;
            }
        }

        private TableActivity Activity
        {
            get
            {
                return this.activity;
            }

            set
            {
                this.activity = value;
            }
        }

        private TableActivityItem ActivityItem
        {
            get
            {
                return this.activityItem;
            }

            set
            {
                this.activityItem = value;
            }
        }

        private TableActivityProductionTicketFinished TicketFinished
        {
            get
            {
                return this.ticketFinished;
            }

            set
            {
                this.ticketFinished = value;
            }
        }

        private TableActivityProductionTicketMaterial TicketMaterial
        {
            get
            {
                return this.ticketMaterial;
            }

            set
            {
                this.ticketMaterial = value;
            }
        }

        private TableActivityProductionTicketByProduct TicketByProduct
        {
            get
            {
                return this.ticketByProduct;
            }

            set
            {
                this.ticketByProduct = value;
            }
        }

        private TableActivityProductionTicketFinishedItem TicketFinishedItem
        {
            get
            {
                return this.ticketFinishedItem;
            }

            set
            {
                this.ticketFinishedItem = value;
            }
        }

        private TableActivityProductionTicketMaterialItem TicketMaterialItem
        {
            get
            {
                return this.ticketMaterialItem;
            }

            set
            {
                this.ticketMaterialItem = value;
            }
        }

        private TableActivityProductionTicketByProductItem TicketByProductItem
        {
            get
            {
                return this.ticketByProductItem;
            }

            set
            {
                this.ticketByProductItem = value;
            }
        }

        private TableActivityProductionTicketFinishedDetail TicketDetailFinished
        {
            get
            {
                return this.ticketDetailFinished;
            }

            set
            {
                this.ticketDetailFinished = value;
            }
        }

        private TableActivityProductionTicketMaterialDetail TicketDetailMaterial
        {
            get
            {
                return this.ticketDetailMaterial;
            }

            set
            {
                this.ticketDetailMaterial = value;
            }
        }

        private TableActivityProductionTicketByProductDetail TicketDetailByProduct
        {
            get
            {
                return this.ticketDetailByProduct;
            }

            set
            {
                this.ticketDetailByProduct = value;
            }
        }

        private TableActivityPurchaseOrderWorkSheetItem PurchaseOrderWorkSheetItem
        {
            get
            {
                return this.purchaseOrderWorkSheetItem;
            }

            set
            {
                this.purchaseOrderWorkSheetItem = value;
            }
        }

        private PrinterSettings PrinterSettings
        {
            get
            {
                return this.printerSettings;
            }

            set
            {
                this.printerSettings = value;
            }
        }

        private ProdSchemeLabelSettings SchemeLabelSettings
        {
            get
            {
                return this.schemeLabelSettings;
            }

            set
            {
                this.schemeLabelSettings = value;
            }
        }


        public ObservableCollection<Message> ItemsList
        {
            get
            {
                return this.itemsList;
            }

            set
            {
                this.itemsList = value;
            }
        }

        private TableLocationMapWarehouseProductScheme ProdLocMapWhsProdScheme
        {
            get
            {
                return this.prodLocMapWhsProdScheme;
            }

            set
            {
                this.prodLocMapWhsProdScheme = value;
            }
        }

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerDisable()
        {
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerEnable()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = true;

            try
            {
            }
            catch (Exception ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.FullClassName, Resource.IsReadyToSave, ex) == false)
                {
                    //Application.Exit();
                }

                isReadyToSave = false;
            }

            return isReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToClose()
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            if (this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                return Resource.DMobilePurgeActivityData;
            }
            else
            {
                return "Refresh Data Status";
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> CancelChanges()
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<XDocument> GetXML()
        {
            XDocument xmldocParams = System.Xml.Linq.XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + ("<IsRefreshComplete>" + this.IsRefreshComplete + "</IsRefreshComplete>")
                + "</params>");

            return xmldocParams;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sysSettingsEventPublisher"></param>
        /// <param name="sysSettingsEventArgs"></param>
        /// <remarks></remarks>
        public void OnSysSettingsProgressOut(
            ISysSettingsEventPublisher sysSettingsEventPublisher,
            SysSettingsEventArgs sysSettingsEventArgs)
        {
            this.ProgressOut(
                this,
                null,
                sysSettingsEventArgs.CurrentValue,
                sysSettingsEventArgs.MinValue,
                sysSettingsEventArgs.MaxValue);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sysSettingsEventPublisher"></param>
        /// <param name="sysSettingsEventArgs"></param>
        /// <remarks></remarks>
        public void OnSysSettingsStatusOut(
            ISysSettingsEventPublisher sysSettingsEventPublisher,
            SysSettingsEventArgs sysSettingsEventArgs)
        {
            this.StatusOut(sysSettingsEventArgs.StatusOutMsg);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="BarCodeSettingsEventPublisher"></param>
        /// <param name="barCodeSettingsEventArgs"></param>
        /// <remarks></remarks>
        public void OnBarCodeSettingsProgressOut(
            IBarCodeSettingsEventPublisher barCodeSettingsEventPublisher,
            BarCodeSettingsEventArgs barCodeSettingsEventArgs)
        {
            this.ProgressOut(
                this,
                null,
                barCodeSettingsEventArgs.CurrentValue,
                barCodeSettingsEventArgs.MinValue,
                barCodeSettingsEventArgs.MaxValue);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="BarCodeSettingsEventPublisher"></param>
        /// <param name="barCodeSettingsEventArgs"></param>
        /// <remarks></remarks>
        public void OnBarCodeSettingsStatusOut(
            IBarCodeSettingsEventPublisher barCodeSettingsEventPublisher,
            BarCodeSettingsEventArgs barCodeSettingsEventArgs)
        {
            this.StatusOut(barCodeSettingsEventArgs.StatusOutMsg);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="PrinterSettingsEventPublisher"></param>
        /// <param name="PrinterSettingsEventArgs"></param>
        /// <remarks></remarks>
        public void OnPrnSettingsProgressOut(
            IPrnSettingsPublisher printerSettingsEventPublisher,
            PrnSettingsArgs printerSettingsEventArgs)
        {
            this.ProgressOut(
                this,
                null,
                printerSettingsEventArgs.CurrentValue,
                printerSettingsEventArgs.MinValue,
                printerSettingsEventArgs.MaxValue);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="PrinterSettingsEventPublisher"></param>
        /// <param name="PrinterSettingsEventArgs"></param>
        /// <remarks></remarks>
        public void OnPrnSettingsStatusOut(
            IPrnSettingsPublisher printerSettingsEventPublisher,
            PrnSettingsArgs printerSettingsEventArgs)
        {
            this.StatusOut(printerSettingsEventArgs.StatusOutMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodSchemeLabelSettingsPublisher"></param>
        /// <param name="prodSchemeLabelSettingsArgs"></param>
        public void OnProdSchemeLabelSettingsStatusOut(
          IProdSchemeLabelSettingsPublisher prodSchemeLabelSettingsPublisher,
          ProdSchemeLabelSettingsEventArgs prodSchemeLabelSettingsArgs)
        {
            this.StatusOut(prodSchemeLabelSettingsArgs.StatusOutMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodSchemeLabelSettingsPublisher"></param>
        /// <param name="prodSchemeLabelSettingsArgs"></param>
        public void OnProdSchemeLabelSettingsProgressOut(
          IProdSchemeLabelSettingsPublisher prodSchemeLabelSettingsPublisher,
          ProdSchemeLabelSettingsEventArgs prodSchemeLabelSettingsArgs)
        {
            this.ProgressOut(
                this,
                null,
                prodSchemeLabelSettingsArgs.CurrentValue,
                prodSchemeLabelSettingsArgs.MinValue,
                prodSchemeLabelSettingsArgs.MaxValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseNubEventPublisher"></param>
        /// <param name="databaseNubEventArgs"></param>
        public void OnNubStatOut(IDataNubPublisher databaseNubEventPublisher, DataNubEventArgs databaseNubEventArgs)
        {
            this.StatusOut(databaseNubEventArgs.StatusOutMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataAccessEventPublisher"></param>
        /// <param name="dataAccessEventArgs"></param>
        public void OnDataAccessStatusOut(
            IDataAccessEventPublisher dataAccessEventPublisher,
            DataAccessEventArgs dataAccessEventArgs)
        {
            this.StatusOut(dataAccessEventArgs.StatusOutMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataAccessEventPublisher"></param>
        /// <param name="dataAccessEventArgs"></param>
        public void OnDataAccessProgressOut(
            IDataAccessEventPublisher dataAccessEventPublisher,
            DataAccessEventArgs dataAccessEventArgs)
        {
            this.ProgressOut(
                this,
                null,
                dataAccessEventArgs.CurrentValue,
                dataAccessEventArgs.MinValue,
                dataAccessEventArgs.MaxValue);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucRefreshDataStatus_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                // load refresh criteria
                this.IsRefreshComplete = false;
                this.SysEnv.RefreshOpt = this.SysEnv.RefreshOpt.LoadData();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task RefreshDataStatus_GotFocus(object sender, EventArgs e)
        {
            DateTime refreshLast = new DateTime(1900, 1, 1);

            this.Loading = true;

            await Task.Run(async () =>
            {
                if (this.ActSubType == SysCommon.ActSubTypes.DataStatus)
                {
                    try
                    {
                        await this.DataStatus();
                    }
                    catch (Exception ex)
                    {
                        this.StatusOut(Resource.DataStatusFailed);

                        FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex, this.FullClassName, "tmrRefreshData_Tick");
                        await Device.InvokeOnMainThreadAsync(async () => await errRpt.ShowDialog());
                    }
                }
                else
                {
                    try
                    {
                        this.IsRefreshComplete = true;
                        this.SysEnv.RefreshOpt.IsRefreshDataFailed = false;
                        this.SysEnv.RefreshOpt = this.SysEnv.RefreshOpt.LoadData();

                        // if there is no data or we are purging reset the last refresh date
                        if (!await Common.IsDataBaseDataExist())
                        {
                            this.SysEnv.RefreshOpt.RefreshedDataLast = new DateTime(1900, 1, 1);
                            RefreshDataOptions tempRefreshOpt1 = this.SysEnv.RefreshOpt;
                            this.SysEnv.RefreshOpt.SaveData(ref tempRefreshOpt1);
                        }

                        // Store the last refresh date
                        refreshLast = this.SysEnv.RefreshOpt.RefreshedDataLast;

                        if (this.SysEnv.RefreshOpt.SystemData && !this.SysEnv.RefreshOpt.RefreshDataChangesOnly && this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                        {
                            await this.CreateDatabase();
                            this.SysEnv.RefreshOpt.IsDataCreatedInStartUp = false;
                            this.SysEnv.RefreshOpt.RefreshedDataLast = new DateTime(1900, 1, 1);
                            RefreshDataOptions tempRefreshOpt2 = this.SysEnv.RefreshOpt;
                            this.SysEnv.RefreshOpt.SaveData(ref tempRefreshOpt2);

                            // Assign the last refresh date to DateSerial(1900, 1, 1) because after the database is recreated
                            // we want everything
                            refreshLast = this.SysEnv.RefreshOpt.RefreshedDataLast;
                            await this.RefreshSystemData();
                        }
                        else if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly && this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                        {
                            DatabaseConnection.OpenDataBaseConnection();
                            await this.RefreshSystemData();
                        }

                        await this.RefreshData();

                        if (this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
                        {
                            this.StatusOut(Resource.PurgeDataCompleted);
                        }
                        else
                        {
                            this.StatusOut(Resource.RefreshDataCompleted);
                            TimeSpan span = DateTime.Now - this.RefreshTotal;
                            this.StatusOut(string.Format(Resource.RefreshMinutes0, span.TotalMinutes));
                        }

                        this.IsRefreshComplete = true;
                        this.SysEnv.RefreshOpt.RefreshedDataLast = DateTime.Now;
                        this.SysEnv.RefreshOpt.IsRefreshDataFailed = false;
                    }
                    catch (SQLiteException ex)
                    {
                        this.SysEnv.RefreshOpt.RefreshedDataLast = refreshLast;
                        this.SysEnv.RefreshOpt.IsRefreshDataFailed = true;

                        this.StatusOut(Resource.RefreshDataFailed);

                        FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex, this.FullClassName, "tmrRefreshData_Tick");
                        await Device.InvokeOnMainThreadAsync(async () => await errRpt.ShowDialog());
                    }
                    catch (Exception ex)
                    {
                        this.SysEnv.RefreshOpt.RefreshedDataLast = refreshLast;
                        this.SysEnv.RefreshOpt.IsRefreshDataFailed = true;

                        this.StatusOut(Resource.RefreshDataFailed);

                        FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex, this.FullClassName, "tmrRefreshData_Tick");
                        await Device.InvokeOnMainThreadAsync(async () => await errRpt.ShowDialog());
                    }
                    finally
                    {
                        this.SysEnv.RefreshOpt.IsRefreshedLastDataChangesOnly = this.SysEnv.RefreshOpt.RefreshDataChangesOnly;
                        RefreshDataOptions tempRefreshOpt3 = this.SysEnv.RefreshOpt;
                        this.SysEnv.RefreshOpt.SaveData(ref tempRefreshOpt3);
                    }
                }
                //}
            });

            this.Loading = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateDatabase()
        {
            DatabaseNub db = new DatabaseNub();

            try
            {
                bool isCreateDatabase = true;

                db.RegNubListenerStatOut(this);

                this.StatusOut(Resource.LocatingDatabase);

                if (File.Exists(DatabaseConnection.GetInstance().DBASCIIZ))
                {
                    this.StatusOut(Resource.Found + DatabaseConnection.GetInstance().DBASCIIZ);

                    if (this.SysEnv.RefreshOpt.IsDataCreatedInStartUp)
                    {
                        isCreateDatabase = false;
                    }
                }
                else
                {
                    this.StatusOut(Resource.DatabaseMissing);
                }

                if (isCreateDatabase)
                {
                    this.StatusOut(Resource.DatabaseMissing);

                    await db.CreateNewDatabase();
                    await db.CreateTables();

                    this.StatusOut(Resource.SeedingSystemUsers);

                    TableContactUsers users = new TableContactUsers();
                    await users.SeedContactUsersWithSystemUsers();

                    this.StatusOut(Resource.DatabaseCreationCompleted);
                    this.StatusOut(string.Empty);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                db.UnregNubListenerStatOut(this);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task RefreshData()
        {
            await this.RefreshDataBarCodeAssociation();
            await this.RefreshDataAdjustments();
            await this.RefreshDataCounts();
            await this.RefreshDataProduction();
            await this.RefreshDataReceiving();
            await this.RefreshDataPurchaseOrderWorkSheet();
            await this.RefreshDataPicking();
            await this.RefreshDataTransfers();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task RefreshSystemData()
        {
            this.SysEnv.SysSettings.RegisterSysSettingsListenerStatusOut((ISysSettingsEventListener)this);
            this.SysEnv.SysSettings.RegisterSysSettingsListenerProgressOut((ISysSettingsEventListener)this);
            SystemSettings settings = this.sysEnv.SysSettings;
            SystemSettingsTasks.RequestSystemSettings(ref settings);
            this.SysEnv.SysSettings = settings;

            SystemSettings tempSystemSettings = this.SysEnv.SysSettings;
            SystemSettingsTasks.SaveData(ref tempSystemSettings);

            this.SysEnv.SysSettings.UnregisterSysSettingsListenerStatusOut((ISysSettingsEventListener)this);
            this.SysEnv.SysSettings.UnregisterSysSettingsListenerProgressOut((ISysSettingsEventListener)this);
            this.StatusOut(string.Empty);

            this.SysEnv.BarCodeSettings.RegisterBarCodeSettingsListenerStatusOut((IBarCodeSettingsEventListener)this);
            this.SysEnv.BarCodeSettings.RegisterBarCodeSettingsListenerProgressOut((IBarCodeSettingsEventListener)this);
            this.SysEnv.BarCodeSettings.RequestBarCodeSettings();
            SysBarCodeSettings tempBarCodeSettings = this.SysEnv.BarCodeSettings;
            this.SysEnv.BarCodeSettings.SaveData(ref tempBarCodeSettings);
            this.SysEnv.BarCodeSettings.UnregisterBarCodeSettingsListenerStatusOut((IBarCodeSettingsEventListener)this);
            this.SysEnv.BarCodeSettings.UnregisterBarCodeSettingsListenerProgressOut((IBarCodeSettingsEventListener)this);
            this.StatusOut(string.Empty);

            this.PrinterSettings.RegPrnSettingsListenerStatusOut((IPrnSettingsListener)this);
            this.PrinterSettings.RegPrnSettingsListenerProgressOut((IPrnSettingsListener)this);
            this.PrinterSettings.RequestPrinterSettings();
            PrinterSettings tempPrinterSettings = this.PrinterSettings;
            this.PrinterSettings.SaveData(ref tempPrinterSettings);
            this.PrinterSettings = tempPrinterSettings;
            this.PrinterSettings.UnregPrnSettingsListenerStatusOut((IPrnSettingsListener)this);
            this.PrinterSettings.UnregPrnSettingsListenerProgressOut((IPrnSettingsListener)this);
            this.StatusOut(string.Empty);

            this.ContactUsers.RegisterDataAccessListenerProgressOut(this);
            this.ContactUsers.RegisterDataAccessListenerStatusOut(this);
            await this.ContactUsers.PurgeTContact();
            await this.ContactUsers.Purge();
            await this.ContactUsers.Request();
            this.ContactUsers.UnregisterDataAccessListenerStatusOut(this);
            this.ContactUsers.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            await this.RefreshSystemDataLocationMap();

            this.Route.RegisterDataAccessListenerProgressOut(this);
            this.Route.RegisterDataAccessListenerStatusOut(this);
            await this.Route.Request();
            this.Route.UnregisterDataAccessListenerStatusOut(this);
            this.Route.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.Family.RegisterDataAccessListenerProgressOut(this);
            this.Family.RegisterDataAccessListenerStatusOut(this);
            await this.Family.Request();
            this.Family.UnregisterDataAccessListenerStatusOut(this);
            this.Family.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            await this.RefreshSystemDataProducts();

            if (!await Common.IsDataBaseDataExist())
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), Resource.SomeExpectedDataWasNotRefreshedRecommendRefreshingDataAgain, Resource.OK));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task DataStatus()
        {
            this.SysEnv.RefreshOpt = this.SysEnv.RefreshOpt.LoadData();

            await Device.InvokeOnMainThreadAsync(() =>
            {
                ItemsList.Add(new Message() { MessageIndex = this.messageIndex++, MessageValue = Resource.LastRefresh + " " + this.SysEnv.RefreshOpt.RefreshedDataLast.ToString() });
                ItemsList.Add(new Message() { MessageIndex = this.messageIndex++, MessageValue = Resource.RefreshChangesOnly + " " + this.SysEnv.RefreshOpt.IsRefreshedLastDataChangesOnly.ToString() });
            });

            this.ContactUsers.RegisterDataAccessListenerProgressOut(this);
            this.ContactUsers.RegisterDataAccessListenerStatusOut(this);
            await this.ContactUsers.DataStatus();
            this.ContactUsers.UnregisterDataAccessListenerStatusOut(this);
            this.ContactUsers.UnregisterDataAccessListenerProgressOut(this);

            await this.DataStatusLocationMap();
            await this.DataStatusProducts();

            this.Route.RegisterDataAccessListenerProgressOut(this);
            this.Route.RegisterDataAccessListenerStatusOut(this);
            await this.Route.DataStatus();
            this.Route.UnregisterDataAccessListenerStatusOut(this);
            this.Route.UnregisterDataAccessListenerProgressOut(this);

            this.Family.RegisterDataAccessListenerProgressOut(this);
            this.Family.RegisterDataAccessListenerStatusOut(this);
            await this.Family.DataStatus();
            this.Family.UnregisterDataAccessListenerStatusOut(this);
            this.Family.UnregisterDataAccessListenerProgressOut(this);

            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await this.DataStatusReceiving();
                await this.DataStatusPicking();
                await this.DataStatusReturns();
                await this.DataStatusTransfers();
                await this.DataStatusCounts();
                await this.DataStatusTickets();

                this.PurchaseOrderWorkSheetItem.RegisterDataAccessListenerProgressOut(this);
                this.PurchaseOrderWorkSheetItem.RegisterDataAccessListenerStatusOut(this);
                await this.PurchaseOrderWorkSheetItem.DataStatus();
                this.PurchaseOrderWorkSheetItem.UnregisterDataAccessListenerStatusOut(this);
                this.PurchaseOrderWorkSheetItem.UnregisterDataAccessListenerProgressOut(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="message"></param>
        private void StatusOut(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.ItemsList.Add(new Message() { MessageIndex = this.messageIndex++, MessageValue = message });
                this.lstStatus.ScrollTo(this.ItemsList.LastOrDefault(), ScrollToPosition.MakeVisible, false);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="value"></param>  
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        private void ProgressOut(object sender, EventArgs e, int value, int minValue, int maxValue)
        {
            //this.pbarStatus.Maximum = maxValue;
            //this.pbarStatus.Minimum = minValue;
            //this.pbarStatus.Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task RefreshSystemDataLocationMap()
        {
            this.LocMapWarehouse.RegisterDataAccessListenerProgressOut(this);
            this.LocMapWarehouse.RegisterDataAccessListenerStatusOut(this);
            await this.LocMapWarehouse.Request();
            this.LocMapWarehouse.UnregisterDataAccessListenerStatusOut(this);
            this.LocMapWarehouse.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.LocMapSection.RegisterDataAccessListenerProgressOut(this);
            this.LocMapSection.RegisterDataAccessListenerStatusOut(this);
            await this.LocMapSection.Request(0);
            this.LocMapSection.UnregisterDataAccessListenerStatusOut(this);
            this.LocMapSection.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.LocMapSlotCoordinate.RegisterDataAccessListenerProgressOut(this);
            this.LocMapSlotCoordinate.RegisterDataAccessListenerStatusOut(this);
            await this.LocMapSlotCoordinate.Request();
            this.LocMapSlotCoordinate.UnregisterDataAccessListenerStatusOut(this);
            this.LocMapSlotCoordinate.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.LocMapSlot.RegisterDataAccessListenerProgressOut(this);
            this.LocMapSlot.RegisterDataAccessListenerStatusOut(this);
            await this.LocMapSlot.Request(0);
            this.LocMapSlot.UnregisterDataAccessListenerStatusOut(this);
            this.LocMapSlot.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.LocMapType.RegisterDataAccessListenerProgressOut(this);
            this.LocMapType.RegisterDataAccessListenerStatusOut(this);
            await this.LocMapType.Request();
            this.LocMapType.UnregisterDataAccessListenerStatusOut(this);
            this.LocMapType.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.WhsLocMapType.RegisterDataAccessListenerProgressOut(this);
            this.WhsLocMapType.RegisterDataAccessListenerStatusOut(this);
            await this.WhsLocMapType.Request();
            this.WhsLocMapType.UnregisterDataAccessListenerStatusOut(this);
            this.WhsLocMapType.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task RefreshSystemDataProducts()
        {
            this.Unit.RegisterDataAccessListenerProgressOut(this);
            this.Unit.RegisterDataAccessListenerStatusOut(this);
            await this.Unit.Request();
            this.Unit.UnregisterDataAccessListenerStatusOut(this);
            this.Unit.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.ProdScheme.RegisterDataAccessListenerProgressOut(this);
            this.ProdScheme.RegisterDataAccessListenerStatusOut(this);
            await this.ProdScheme.Request();
            this.ProdScheme.UnregisterDataAccessListenerStatusOut(this);
            this.ProdScheme.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.ProdSchemeVariant.RegisterDataAccessListenerProgressOut(this);
            this.ProdSchemeVariant.RegisterDataAccessListenerStatusOut(this);
            await this.ProdSchemeVariant.Request();
            this.ProdSchemeVariant.UnregisterDataAccessListenerStatusOut(this);
            this.ProdSchemeVariant.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.ProdSchemeVariantXRef.RegisterDataAccessListenerProgressOut(this);
            this.ProdSchemeVariantXRef.RegisterDataAccessListenerStatusOut(this);
            await this.ProdSchemeVariantXRef.Request();
            this.ProdSchemeVariantXRef.UnregisterDataAccessListenerStatusOut(this);
            this.ProdSchemeVariantXRef.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            // This xml settings file need to be called hear not called from the refresh system data method,
            // because scheme codes must exist in there table. This routine seeks the scheme code.
            this.SchemeLabelSettings.RegProdSchemeLabelSettingsListenerStatusOut((IProdSchemeLabelSettingsListener)this);
            this.SchemeLabelSettings.RegProdSchemeLabelSettingsListenerProgressOut((IProdSchemeLabelSettingsListener)this);
            await this.SchemeLabelSettings.RequestProductSchemeLabelSettings();
            ProdSchemeLabelSettings tempSchemeLblSettings = this.SchemeLabelSettings;
            this.SchemeLabelSettings.SaveData(ref tempSchemeLblSettings);
            this.SchemeLabelSettings = tempSchemeLblSettings;
            this.SchemeLabelSettings.UnregProdSchemeLabelSettingsListenerStatusOut((IProdSchemeLabelSettingsListener)this);
            this.SchemeLabelSettings.UnregProdSchemeLabelSettingsListenerProgressOut((IProdSchemeLabelSettingsListener)this);
            this.StatusOut(string.Empty);

            if (this.SysEnv.SysSettings.IsEnableDefaultDetailValues || this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                this.ProdSchemeLot.RegisterDataAccessListenerProgressOut(this);
                this.ProdSchemeLot.RegisterDataAccessListenerStatusOut(this);
                await this.ProdSchemeLot.Request();
                this.ProdSchemeLot.UnregisterDataAccessListenerStatusOut(this);
                this.ProdSchemeLot.UnregisterDataAccessListenerProgressOut(this);
                this.StatusOut(string.Empty);

                this.ProdSchemeLotXRef.RegisterDataAccessListenerProgressOut(this);
                this.ProdSchemeLotXRef.RegisterDataAccessListenerStatusOut(this);
                await this.ProdSchemeLotXRef.Request();
                this.ProdSchemeLotXRef.UnregisterDataAccessListenerStatusOut(this);
                this.ProdSchemeLotXRef.UnregisterDataAccessListenerProgressOut(this);
                this.StatusOut(string.Empty);
            }

            this.Product.RegisterDataAccessListenerProgressOut(this);
            this.Product.RegisterDataAccessListenerStatusOut(this);
            await this.Product.Request();
            this.Product.UnregisterDataAccessListenerStatusOut(this);
            this.Product.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.ProdUnit.RegisterDataAccessListenerProgressOut(this);
            this.ProdUnit.RegisterDataAccessListenerStatusOut(this);
            await this.ProdUnit.Request();
            this.ProdUnit.UnregisterDataAccessListenerStatusOut(this);
            this.ProdUnit.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.ProdUnitRelation.RegisterDataAccessListenerProgressOut(this);
            this.ProdUnitRelation.RegisterDataAccessListenerStatusOut(this);
            await this.ProdUnitRelation.Request();
            this.ProdUnitRelation.UnregisterDataAccessListenerStatusOut(this);
            this.ProdUnitRelation.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.ProdUnitBarCode.RegisterDataAccessListenerProgressOut(this);
            this.ProdUnitBarCode.RegisterDataAccessListenerStatusOut(this);
            await this.ProdUnitBarCode.Request();
            this.ProdUnitBarCode.UnregisterDataAccessListenerStatusOut(this);
            this.ProdUnitBarCode.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.ProdLocMapWhsProdScheme.RegisterDataAccessListenerProgressOut(this);
            this.ProdLocMapWhsProdScheme.RegisterDataAccessListenerStatusOut(this);
            await this.ProdLocMapWhsProdScheme.Request();
            this.ProdLocMapWhsProdScheme.UnregisterDataAccessListenerStatusOut(this);
            this.ProdLocMapWhsProdScheme.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.ProdLocMapType.RegisterDataAccessListenerProgressOut(this);
            this.ProdLocMapType.RegisterDataAccessListenerStatusOut(this);
            await this.ProdLocMapType.Request();
            this.ProdLocMapType.UnregisterDataAccessListenerStatusOut(this);
            this.ProdLocMapType.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task DataStatusLocationMap()
        {
            this.LocMapWarehouse.RegisterDataAccessListenerProgressOut(this);
            this.LocMapWarehouse.RegisterDataAccessListenerStatusOut(this);
            await this.LocMapWarehouse.DataStatus();
            this.LocMapWarehouse.UnregisterDataAccessListenerStatusOut(this);
            this.LocMapWarehouse.UnregisterDataAccessListenerProgressOut(this);

            this.LocMapSection.RegisterDataAccessListenerProgressOut(this);
            this.LocMapSection.RegisterDataAccessListenerStatusOut(this);
            await this.LocMapSection.DataStatus();
            this.LocMapSection.UnregisterDataAccessListenerStatusOut(this);
            this.LocMapSection.UnregisterDataAccessListenerProgressOut(this);

            this.LocMapSlotCoordinate.RegisterDataAccessListenerProgressOut(this);
            this.LocMapSlotCoordinate.RegisterDataAccessListenerStatusOut(this);
            await this.LocMapSlotCoordinate.DataStatus();
            this.LocMapSlotCoordinate.UnregisterDataAccessListenerStatusOut(this);
            this.LocMapSlotCoordinate.UnregisterDataAccessListenerProgressOut(this);

            this.LocMapSlot.RegisterDataAccessListenerProgressOut(this);
            this.LocMapSlot.RegisterDataAccessListenerStatusOut(this);
            await this.LocMapSlot.DataStatus();
            this.LocMapSlot.UnregisterDataAccessListenerStatusOut(this);
            this.LocMapSlot.UnregisterDataAccessListenerProgressOut(this);

            this.WhsLocMapType.RegisterDataAccessListenerProgressOut(this);
            this.WhsLocMapType.RegisterDataAccessListenerStatusOut(this);
            await this.WhsLocMapType.DataStatus();
            this.WhsLocMapType.UnregisterDataAccessListenerStatusOut(this);
            this.WhsLocMapType.UnregisterDataAccessListenerProgressOut(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task DataStatusProducts()
        {
            this.Unit.RegisterDataAccessListenerProgressOut(this);
            this.Unit.RegisterDataAccessListenerStatusOut(this);
            await this.Unit.DataStatus();
            this.Unit.UnregisterDataAccessListenerStatusOut(this);
            this.Unit.UnregisterDataAccessListenerProgressOut(this);

            this.Product.RegisterDataAccessListenerProgressOut(this);
            this.Product.RegisterDataAccessListenerStatusOut(this);
            await this.Product.DataStatus();
            this.Product.UnregisterDataAccessListenerStatusOut(this);
            this.Product.UnregisterDataAccessListenerProgressOut(this);

            this.ProdUnit.RegisterDataAccessListenerProgressOut(this);
            this.ProdUnit.RegisterDataAccessListenerStatusOut(this);
            await this.ProdUnit.DataStatus();
            this.ProdUnit.UnregisterDataAccessListenerStatusOut(this);
            this.ProdUnit.UnregisterDataAccessListenerProgressOut(this);

            this.ProdUnitBarCode.RegisterDataAccessListenerProgressOut(this);
            this.ProdUnitBarCode.RegisterDataAccessListenerStatusOut(this);
            await this.ProdUnitBarCode.DataStatus();
            this.ProdUnitBarCode.UnregisterDataAccessListenerStatusOut(this);
            this.ProdUnitBarCode.UnregisterDataAccessListenerProgressOut(this);

            this.ProdScheme.RegisterDataAccessListenerProgressOut(this);
            this.ProdScheme.RegisterDataAccessListenerStatusOut(this);
            await this.ProdScheme.DataStatus();
            this.ProdScheme.UnregisterDataAccessListenerStatusOut(this);
            this.ProdScheme.UnregisterDataAccessListenerProgressOut(this);

            this.ProdLocMapType.RegisterDataAccessListenerProgressOut(this);
            this.ProdLocMapType.RegisterDataAccessListenerStatusOut(this);
            await this.ProdLocMapType.DataStatus();
            this.ProdLocMapType.UnregisterDataAccessListenerStatusOut(this);
            this.ProdLocMapType.UnregisterDataAccessListenerProgressOut(this);

            this.ProdSchemeVariant.RegisterDataAccessListenerProgressOut(this);
            this.ProdSchemeVariant.RegisterDataAccessListenerStatusOut(this);
            await this.ProdSchemeVariant.DataStatus();
            this.ProdSchemeVariant.UnregisterDataAccessListenerStatusOut(this);
            this.ProdSchemeVariant.UnregisterDataAccessListenerProgressOut(this);

            this.ProdSchemeLot.RegisterDataAccessListenerProgressOut(this);
            this.ProdSchemeLot.RegisterDataAccessListenerStatusOut(this);
            await this.ProdSchemeLot.DataStatus();
            this.ProdSchemeLot.UnregisterDataAccessListenerStatusOut(this);
            this.ProdSchemeLot.UnregisterDataAccessListenerProgressOut(this);

            this.ProdLocMapWhsProdScheme.RegisterDataAccessListenerProgressOut(this);
            this.ProdLocMapWhsProdScheme.RegisterDataAccessListenerStatusOut(this);
            await this.ProdLocMapWhsProdScheme.DataStatus();
            this.ProdLocMapWhsProdScheme.UnregisterDataAccessListenerStatusOut(this);
            this.ProdLocMapWhsProdScheme.UnregisterDataAccessListenerProgressOut(this);

            this.ProdLocMapType.RegisterDataAccessListenerProgressOut(this);
            this.ProdLocMapType.RegisterDataAccessListenerStatusOut(this);
            await this.ProdLocMapType.DataStatus();
            this.ProdLocMapType.UnregisterDataAccessListenerStatusOut(this);
            this.ProdLocMapType.UnregisterDataAccessListenerProgressOut(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task DataStatusReceiving()
        {
            this.Receiving.RegisterDataAccessListenerProgressOut(this);
            this.Receiving.RegisterDataAccessListenerStatusOut(this);
            await this.Receiving.DataStatus();
            this.Receiving.UnregisterDataAccessListenerStatusOut(this);
            this.Receiving.UnregisterDataAccessListenerProgressOut(this);

            this.ReceivingItem.RegisterDataAccessListenerProgressOut(this);
            this.ReceivingItem.RegisterDataAccessListenerStatusOut(this);
            await this.ReceivingItem.DataStatus();
            this.ReceivingItem.UnregisterDataAccessListenerStatusOut(this);
            this.ReceivingItem.UnregisterDataAccessListenerProgressOut(this);

            this.ReceivingDetail.RegisterDataAccessListenerProgressOut(this);
            this.ReceivingDetail.RegisterDataAccessListenerStatusOut(this);
            await this.ReceivingDetail.DataStatus();
            this.ReceivingDetail.UnregisterDataAccessListenerStatusOut(this);
            this.ReceivingDetail.UnregisterDataAccessListenerProgressOut(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task DataStatusPicking()
        {
            this.Picking.RegisterDataAccessListenerProgressOut(this);
            this.Picking.RegisterDataAccessListenerStatusOut(this);
            await this.Picking.DataStatus();
            this.Picking.UnregisterDataAccessListenerStatusOut(this);
            this.Picking.UnregisterDataAccessListenerProgressOut(this);

            this.PickingItem.RegisterDataAccessListenerProgressOut(this);
            this.PickingItem.RegisterDataAccessListenerStatusOut(this);
            await this.PickingItem.DataStatus();
            this.PickingItem.UnregisterDataAccessListenerStatusOut(this);
            this.PickingItem.UnregisterDataAccessListenerProgressOut(this);

            this.PickingDetail.RegisterDataAccessListenerProgressOut(this);
            this.PickingDetail.RegisterDataAccessListenerStatusOut(this);
            await this.PickingDetail.DataStatus();
            this.PickingDetail.UnregisterDataAccessListenerStatusOut(this);
            this.PickingDetail.UnregisterDataAccessListenerProgressOut(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task DataStatusReturns()
        {
            this.Returns.RegisterDataAccessListenerProgressOut(this);
            this.Returns.RegisterDataAccessListenerStatusOut(this);
            await this.Returns.DataStatus();
            this.Returns.UnregisterDataAccessListenerStatusOut(this);
            this.Returns.UnregisterDataAccessListenerProgressOut(this);

            this.ReturnsItem.RegisterDataAccessListenerProgressOut(this);
            this.ReturnsItem.RegisterDataAccessListenerStatusOut(this);
            await this.ReturnsItem.DataStatus();
            this.ReturnsItem.UnregisterDataAccessListenerStatusOut(this);
            this.ReturnsItem.UnregisterDataAccessListenerProgressOut(this);

            this.ReturnsDetail.RegisterDataAccessListenerProgressOut(this);
            this.ReturnsDetail.RegisterDataAccessListenerStatusOut(this);
            await this.ReturnsDetail.DataStatus();
            this.ReturnsDetail.UnregisterDataAccessListenerStatusOut(this);
            this.ReturnsDetail.UnregisterDataAccessListenerProgressOut(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task DataStatusTransfers()
        {
            this.TransferFrom.RegisterDataAccessListenerProgressOut(this);
            this.TransferFrom.RegisterDataAccessListenerStatusOut(this);
            await this.TransferFrom.DataStatus();
            this.TransferFrom.UnregisterDataAccessListenerStatusOut(this);
            this.TransferFrom.UnregisterDataAccessListenerProgressOut(this);

            this.TransferFromItem.RegisterDataAccessListenerProgressOut(this);
            this.TransferFromItem.RegisterDataAccessListenerStatusOut(this);
            await this.TransferFromItem.DataStatus();
            this.TransferFromItem.UnregisterDataAccessListenerStatusOut(this);
            this.TransferFromItem.UnregisterDataAccessListenerProgressOut(this);

            this.TransferFromDetail.RegisterDataAccessListenerProgressOut(this);
            this.TransferFromDetail.RegisterDataAccessListenerStatusOut(this);
            await this.TransferFromDetail.DataStatus();
            this.TransferFromDetail.UnregisterDataAccessListenerStatusOut(this);
            this.TransferFromDetail.UnregisterDataAccessListenerProgressOut(this);

            this.TransferTo.RegisterDataAccessListenerProgressOut(this);
            this.TransferTo.RegisterDataAccessListenerStatusOut(this);
            await this.TransferTo.DataStatus();
            this.TransferTo.UnregisterDataAccessListenerStatusOut(this);
            this.TransferTo.UnregisterDataAccessListenerProgressOut(this);

            this.TransferToItem.RegisterDataAccessListenerProgressOut(this);
            this.TransferToItem.RegisterDataAccessListenerStatusOut(this);
            await this.TransferToItem.DataStatus();
            this.TransferToItem.UnregisterDataAccessListenerStatusOut(this);
            this.TransferToItem.UnregisterDataAccessListenerProgressOut(this);

            this.TransferToDetail.RegisterDataAccessListenerProgressOut(this);
            this.TransferToDetail.RegisterDataAccessListenerStatusOut(this);
            await this.TransferToDetail.DataStatus();
            this.TransferToDetail.UnregisterDataAccessListenerStatusOut(this);
            this.TransferToDetail.UnregisterDataAccessListenerProgressOut(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task DataStatusCounts()
        {
            this.CycleCount.RegisterDataAccessListenerProgressOut(this);
            this.CycleCount.RegisterDataAccessListenerStatusOut(this);
            await this.CycleCount.DataStatus();
            this.CycleCount.UnregisterDataAccessListenerStatusOut(this);
            this.CycleCount.UnregisterDataAccessListenerProgressOut(this);

            this.CycleCountItem.RegisterDataAccessListenerProgressOut(this);
            this.CycleCountItem.RegisterDataAccessListenerStatusOut(this);
            await this.CycleCountItem.DataStatus();
            this.CycleCountItem.UnregisterDataAccessListenerStatusOut(this);
            this.CycleCountItem.UnregisterDataAccessListenerProgressOut(this);

            this.CycleCountDetail.RegisterDataAccessListenerProgressOut(this);
            this.CycleCountDetail.RegisterDataAccessListenerStatusOut(this);
            await this.CycleCountDetail.DataStatus();
            this.CycleCountDetail.UnregisterDataAccessListenerStatusOut(this);
            this.CycleCountDetail.UnregisterDataAccessListenerProgressOut(this);

            this.PhysicalCount.RegisterDataAccessListenerProgressOut(this);
            this.PhysicalCount.RegisterDataAccessListenerStatusOut(this);
            await this.PhysicalCount.DataStatus();
            this.PhysicalCount.UnregisterDataAccessListenerStatusOut(this);
            this.PhysicalCount.UnregisterDataAccessListenerProgressOut(this);

            this.PhysicalCountItem.RegisterDataAccessListenerProgressOut(this);
            this.PhysicalCountItem.RegisterDataAccessListenerStatusOut(this);
            await this.PhysicalCountItem.DataStatus();
            this.PhysicalCountItem.UnregisterDataAccessListenerStatusOut(this);
            this.PhysicalCountItem.UnregisterDataAccessListenerProgressOut(this);

            this.PhysicalCountDetail.RegisterDataAccessListenerProgressOut(this);
            this.PhysicalCountDetail.RegisterDataAccessListenerStatusOut(this);
            await this.PhysicalCountDetail.DataStatus();
            this.PhysicalCountDetail.UnregisterDataAccessListenerStatusOut(this);
            this.PhysicalCountDetail.UnregisterDataAccessListenerProgressOut(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task DataStatusTickets()
        {
            this.TicketFinished.RegisterDataAccessListenerProgressOut(this);
            this.TicketFinished.RegisterDataAccessListenerStatusOut(this);
            await this.TicketFinished.DataStatus();
            this.TicketFinished.UnregisterDataAccessListenerStatusOut(this);
            this.TicketFinished.UnregisterDataAccessListenerProgressOut(this);

            this.TicketMaterial.RegisterDataAccessListenerProgressOut(this);
            this.TicketMaterial.RegisterDataAccessListenerStatusOut(this);
            await this.TicketMaterial.DataStatus();
            this.TicketMaterial.UnregisterDataAccessListenerStatusOut(this);
            this.TicketMaterial.UnregisterDataAccessListenerProgressOut(this);

            this.TicketByProduct.RegisterDataAccessListenerProgressOut(this);
            this.TicketByProduct.RegisterDataAccessListenerStatusOut(this);
            await this.TicketByProduct.DataStatus();
            this.TicketByProduct.UnregisterDataAccessListenerStatusOut(this);
            this.TicketByProduct.UnregisterDataAccessListenerProgressOut(this);

            this.TicketFinishedItem.RegisterDataAccessListenerProgressOut(this);
            this.TicketFinishedItem.RegisterDataAccessListenerStatusOut(this);
            await this.TicketFinishedItem.DataStatus();
            this.TicketFinishedItem.UnregisterDataAccessListenerStatusOut(this);
            this.TicketFinishedItem.UnregisterDataAccessListenerProgressOut(this);

            this.TicketMaterialItem.RegisterDataAccessListenerProgressOut(this);
            this.TicketMaterialItem.RegisterDataAccessListenerStatusOut(this);
            await this.TicketMaterialItem.DataStatus();
            this.TicketMaterialItem.UnregisterDataAccessListenerStatusOut(this);
            this.TicketMaterialItem.UnregisterDataAccessListenerProgressOut(this);

            this.TicketByProductItem.RegisterDataAccessListenerProgressOut(this);
            this.TicketByProductItem.RegisterDataAccessListenerStatusOut(this);
            await this.TicketByProductItem.DataStatus();
            this.TicketByProductItem.UnregisterDataAccessListenerStatusOut(this);
            this.TicketByProductItem.UnregisterDataAccessListenerProgressOut(this);

            this.TicketDetailFinished.RegisterDataAccessListenerProgressOut(this);
            this.TicketDetailFinished.RegisterDataAccessListenerStatusOut(this);
            await this.TicketDetailFinished.DataStatus();
            this.TicketDetailFinished.UnregisterDataAccessListenerStatusOut(this);
            this.TicketDetailFinished.UnregisterDataAccessListenerProgressOut(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task RefreshDataBarCodeAssociation()
        {
            // Bar Code Association
            if (this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                this.ActItemBarCodeAssocXRef.RegisterDataAccessListenerStatusOut(this);
                await this.ActItemBarCodeAssocXRef.Purge();
                this.ActItemBarCodeAssocXRef.UnregisterDataAccessListenerStatusOut(this);

                this.ActivityItem.RegisterDataAccessListenerStatusOut(this);
                await this.ActivityItem.Purge(SysCommon.ActSubTypes.BarCodeAssociation);
                this.ActivityItem.UnregisterDataAccessListenerStatusOut(this);

                this.Activity.RegisterDataAccessListenerStatusOut(this);
                await this.Activity.Purge(SysCommon.ActSubTypes.BarCodeAssociation);
                this.Activity.UnregisterDataAccessListenerStatusOut(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task RefreshDataAdjustments()
        {
            if (this.SysEnv.RefreshOpt.Adjustments || this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                this.AdjustmentDetail.RegisterDataAccessListenerStatusOut(this);
                await this.AdjustmentDetail.Purge();
                this.AdjustmentDetail.UnregisterDataAccessListenerStatusOut(this);

                this.AdjustmentItem.RegisterDataAccessListenerStatusOut(this);
                await this.AdjustmentItem.Purge();
                this.AdjustmentItem.UnregisterDataAccessListenerStatusOut(this);

                this.Adjustment.RegisterDataAccessListenerStatusOut(this);
                await this.Adjustment.Purge();
                this.Adjustment.UnregisterDataAccessListenerStatusOut(this);

                if (this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                {
                    this.Adjustment.RegisterDataAccessListenerProgressOut(this);
                    this.Adjustment.RegisterDataAccessListenerStatusOut(this);
                    await this.Adjustment.Request();
                    this.Adjustment.UnregisterDataAccessListenerStatusOut(this);
                    this.Adjustment.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.AdjustmentItem.RegisterDataAccessListenerProgressOut(this);
                    this.AdjustmentItem.RegisterDataAccessListenerStatusOut(this);
                    await this.AdjustmentItem.Request(0);
                    this.AdjustmentItem.UnregisterDataAccessListenerStatusOut(this);
                    this.AdjustmentItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                    {
                        this.AdjustmentDetail.RegisterDataAccessListenerProgressOut(this);
                        this.AdjustmentDetail.RegisterDataAccessListenerStatusOut(this);
                        await this.AdjustmentDetail.Request(0);
                        this.AdjustmentDetail.UnregisterDataAccessListenerStatusOut(this);
                        this.AdjustmentDetail.UnregisterDataAccessListenerProgressOut(this);
                        this.StatusOut(string.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task RefreshDataCounts()
        {
            if (this.SysEnv.RefreshOpt.Counts || this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                this.PhysicalCountDetail.RegisterDataAccessListenerStatusOut(this);
                await this.PhysicalCountDetail.Purge();
                this.PhysicalCountDetail.UnregisterDataAccessListenerStatusOut(this);

                this.PhysicalCountItem.RegisterDataAccessListenerStatusOut(this);
                await this.PhysicalCountItem.Purge();
                this.PhysicalCountItem.UnregisterDataAccessListenerStatusOut(this);

                this.PhysicalCount.RegisterDataAccessListenerStatusOut(this);
                await this.PhysicalCount.Purge();
                this.PhysicalCount.UnregisterDataAccessListenerStatusOut(this);

                if (this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                {
                    this.PhysicalCount.RegisterDataAccessListenerProgressOut(this);
                    this.PhysicalCount.RegisterDataAccessListenerStatusOut(this);
                    await this.PhysicalCount.Request();
                    this.PhysicalCount.UnregisterDataAccessListenerStatusOut(this);
                    this.PhysicalCount.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.PhysicalCountItem.RegisterDataAccessListenerProgressOut(this);
                    this.PhysicalCountItem.RegisterDataAccessListenerStatusOut(this);
                    await this.PhysicalCountItem.Request(0);
                    this.PhysicalCountItem.UnregisterDataAccessListenerStatusOut(this);
                    this.PhysicalCountItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                    {
                        this.PhysicalCountDetail.RegisterDataAccessListenerProgressOut(this);
                        this.PhysicalCountDetail.RegisterDataAccessListenerStatusOut(this);
                        await this.PhysicalCountDetail.Request(0);
                        this.PhysicalCountDetail.UnregisterDataAccessListenerStatusOut(this);
                        this.PhysicalCountDetail.UnregisterDataAccessListenerProgressOut(this);
                        this.StatusOut(string.Empty);
                    }
                }

                this.CycleCountDetail.RegisterDataAccessListenerStatusOut(this);
                await this.CycleCountDetail.Purge();
                this.CycleCountDetail.UnregisterDataAccessListenerStatusOut(this);

                this.CycleCountItem.RegisterDataAccessListenerStatusOut(this);
                await this.CycleCountItem.Purge();
                this.CycleCountItem.UnregisterDataAccessListenerStatusOut(this);

                this.CycleCount.RegisterDataAccessListenerStatusOut(this);
                await this.CycleCount.Purge();
                this.CycleCount.UnregisterDataAccessListenerStatusOut(this);

                if (this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                {
                    this.CycleCount.RegisterDataAccessListenerProgressOut(this);
                    this.CycleCount.RegisterDataAccessListenerStatusOut(this);
                    await this.CycleCount.Request();
                    this.CycleCount.UnregisterDataAccessListenerStatusOut(this);
                    this.CycleCount.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.CycleCountItem.RegisterDataAccessListenerProgressOut(this);
                    this.CycleCountItem.RegisterDataAccessListenerStatusOut(this);
                    await this.CycleCountItem.Request(0);
                    this.CycleCountItem.UnregisterDataAccessListenerStatusOut(this);
                    this.CycleCountItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                    {
                        this.CycleCountDetail.RegisterDataAccessListenerProgressOut(this);
                        this.CycleCountDetail.RegisterDataAccessListenerStatusOut(this);
                        await this.CycleCountDetail.Request(0);
                        this.CycleCountDetail.UnregisterDataAccessListenerStatusOut(this);
                        this.CycleCountDetail.UnregisterDataAccessListenerProgressOut(this);
                        this.StatusOut(string.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task RefreshDataProduction()
        {
            if (this.SysEnv.RefreshOpt.ProductionTickets || this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                this.TicketDetailFinished.RegisterDataAccessListenerStatusOut(this);
                await this.TicketDetailFinished.Purge();
                this.TicketDetailFinished.UnregisterDataAccessListenerStatusOut(this);

                this.TicketDetailMaterial.RegisterDataAccessListenerStatusOut(this);
                await this.TicketDetailMaterial.Purge();
                this.TicketDetailMaterial.UnregisterDataAccessListenerStatusOut(this);

                this.TicketDetailByProduct.RegisterDataAccessListenerStatusOut(this);
                await this.TicketDetailByProduct.Purge();
                this.TicketDetailByProduct.UnregisterDataAccessListenerStatusOut(this);

                this.TicketFinishedItem.RegisterDataAccessListenerStatusOut(this);
                await this.TicketFinishedItem.Purge();
                this.TicketFinishedItem.UnregisterDataAccessListenerStatusOut(this);

                this.TicketMaterialItem.RegisterDataAccessListenerStatusOut(this);
                await this.TicketMaterialItem.Purge();
                this.TicketMaterialItem.UnregisterDataAccessListenerStatusOut(this);

                this.TicketByProductItem.RegisterDataAccessListenerStatusOut(this);
                await this.TicketByProductItem.Purge();
                this.TicketByProductItem.UnregisterDataAccessListenerStatusOut(this);

                this.TicketFinished.RegisterDataAccessListenerStatusOut(this);
                await this.TicketFinished.Purge();
                this.TicketFinished.UnregisterDataAccessListenerStatusOut(this);

                this.TicketMaterial.RegisterDataAccessListenerStatusOut(this);
                await this.TicketMaterial.Purge();
                this.TicketMaterial.UnregisterDataAccessListenerStatusOut(this);

                this.TicketByProduct.RegisterDataAccessListenerStatusOut(this);
                await this.TicketByProduct.Purge();
                this.TicketByProduct.UnregisterDataAccessListenerStatusOut(this);

                if (this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                {
                    this.TicketFinished.RegisterDataAccessListenerProgressOut(this);
                    this.TicketFinished.RegisterDataAccessListenerStatusOut(this);
                    await this.TicketFinished.Request();
                    this.TicketFinished.UnregisterDataAccessListenerStatusOut(this);
                    this.TicketFinished.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.TicketMaterial.RegisterDataAccessListenerProgressOut(this);
                    this.TicketMaterial.RegisterDataAccessListenerStatusOut(this);
                    await this.TicketMaterial.Request();
                    this.TicketMaterial.UnregisterDataAccessListenerStatusOut(this);
                    this.TicketMaterial.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.TicketByProduct.RegisterDataAccessListenerProgressOut(this);
                    this.TicketByProduct.RegisterDataAccessListenerStatusOut(this);
                    await this.TicketByProduct.Request();
                    this.TicketByProduct.UnregisterDataAccessListenerStatusOut(this);
                    this.TicketByProduct.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.TicketFinishedItem.RegisterDataAccessListenerProgressOut(this);
                    this.TicketFinishedItem.RegisterDataAccessListenerStatusOut(this);
                    await this.TicketFinishedItem.Request(0);
                    this.TicketFinishedItem.UnregisterDataAccessListenerStatusOut(this);
                    this.TicketFinishedItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.TicketMaterialItem.RegisterDataAccessListenerProgressOut(this);
                    this.TicketMaterialItem.RegisterDataAccessListenerStatusOut(this);
                    await this.TicketMaterialItem.Request(0);
                    this.TicketMaterialItem.UnregisterDataAccessListenerStatusOut(this);
                    this.TicketMaterialItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.TicketByProductItem.RegisterDataAccessListenerProgressOut(this);
                    this.TicketByProductItem.RegisterDataAccessListenerStatusOut(this);
                    await this.TicketByProductItem.Request(0);
                    this.TicketByProductItem.UnregisterDataAccessListenerStatusOut(this);
                    this.TicketByProductItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                    {
                        this.TicketDetailFinished.RegisterDataAccessListenerProgressOut(this);
                        this.TicketDetailFinished.RegisterDataAccessListenerStatusOut(this);
                        await this.TicketDetailFinished.Request(0);
                        this.TicketDetailFinished.UnregisterDataAccessListenerStatusOut(this);
                        this.TicketDetailFinished.UnregisterDataAccessListenerProgressOut(this);
                        this.StatusOut(string.Empty);

                        this.TicketDetailMaterial.RegisterDataAccessListenerProgressOut(this);
                        this.TicketDetailMaterial.RegisterDataAccessListenerStatusOut(this);
                        await this.TicketDetailMaterial.Request(0);
                        this.TicketDetailMaterial.UnregisterDataAccessListenerStatusOut(this);
                        this.TicketDetailMaterial.UnregisterDataAccessListenerProgressOut(this);
                        this.StatusOut(string.Empty);

                        this.TicketDetailByProduct.RegisterDataAccessListenerProgressOut(this);
                        this.TicketDetailByProduct.RegisterDataAccessListenerStatusOut(this);
                        await this.TicketDetailByProduct.Request(0);
                        this.TicketDetailByProduct.UnregisterDataAccessListenerStatusOut(this);
                        this.TicketDetailByProduct.UnregisterDataAccessListenerProgressOut(this);
                        this.StatusOut(string.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task RefreshDataReceiving()
        {
            if (this.SysEnv.RefreshOpt.PurchaseOrders || this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                this.ReceivingDetail.RegisterDataAccessListenerStatusOut(this);
                await this.ReceivingDetail.Purge();
                this.ReceivingDetail.UnregisterDataAccessListenerStatusOut(this);

                this.ReceivingItemComment.RegisterDataAccessListenerStatusOut(this);
                await this.ReceivingItemComment.Purge();
                this.ReceivingItemComment.UnregisterDataAccessListenerStatusOut(this);

                this.ReceivingItem.RegisterDataAccessListenerStatusOut(this);
                await this.ReceivingItem.Purge();
                this.ReceivingItem.UnregisterDataAccessListenerStatusOut(this);

                this.Receiving.RegisterDataAccessListenerStatusOut(this);
                await this.Receiving.Purge();
                this.Receiving.UnregisterDataAccessListenerStatusOut(this);

                if (this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                {
                    this.Receiving.RegisterDataAccessListenerProgressOut(this);
                    this.Receiving.RegisterDataAccessListenerStatusOut(this);
                    await this.Receiving.Request();
                    this.Receiving.UnregisterDataAccessListenerStatusOut(this);
                    this.Receiving.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.ReceivingItem.RegisterDataAccessListenerProgressOut(this);
                    this.ReceivingItem.RegisterDataAccessListenerStatusOut(this);
                    await this.ReceivingItem.Request(0);
                    this.ReceivingItem.UnregisterDataAccessListenerStatusOut(this);
                    this.ReceivingItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                    {
                        this.ReceivingDetail.RegisterDataAccessListenerProgressOut(this);
                        this.ReceivingDetail.RegisterDataAccessListenerStatusOut(this);
                        await this.ReceivingDetail.Request(0);
                        this.ReceivingDetail.UnregisterDataAccessListenerStatusOut(this);
                        this.ReceivingDetail.UnregisterDataAccessListenerProgressOut(this);
                        this.StatusOut(string.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task RefreshDataPurchaseOrderWorkSheet()
        {
            if (this.SysEnv.RefreshOpt.PurchaseOrderWorkSheet || this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                this.PurchaseOrderWorkSheetItem.RegisterDataAccessListenerStatusOut(this);
                await this.PurchaseOrderWorkSheetItem.Purge();
                this.PurchaseOrderWorkSheetItem.UnregisterDataAccessListenerStatusOut(this);

                if (this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                {
                    this.PurchaseOrderWorkSheetItem.RegisterDataAccessListenerProgressOut(this);
                    this.PurchaseOrderWorkSheetItem.RegisterDataAccessListenerStatusOut(this);
                    await this.PurchaseOrderWorkSheetItem.Request(0);
                    this.PurchaseOrderWorkSheetItem.UnregisterDataAccessListenerStatusOut(this);
                    this.PurchaseOrderWorkSheetItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task RefreshDataPicking()
        {
            if (this.SysEnv.RefreshOpt.SalesOrders || this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                this.PickingDetail.RegisterDataAccessListenerStatusOut(this);
                await this.PickingDetail.Purge();
                this.PickingDetail.UnregisterDataAccessListenerStatusOut(this);

                this.PickingItemComment.RegisterDataAccessListenerStatusOut(this);
                await this.PickingItemComment.Purge();
                this.PickingItemComment.UnregisterDataAccessListenerStatusOut(this);

                this.PickingItem.RegisterDataAccessListenerStatusOut(this);
                await this.PickingItem.Purge();
                this.PickingItem.UnregisterDataAccessListenerStatusOut(this);

                this.PickingRoute.RegisterDataAccessListenerStatusOut(this);
                await this.PickingRoute.Purge();
                this.PickingRoute.UnregisterDataAccessListenerStatusOut(this);

                this.Picking.RegisterDataAccessListenerStatusOut(this);
                await this.Picking.Purge();
                this.Picking.UnregisterDataAccessListenerStatusOut(this);

                if (this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                {
                    this.Picking.RegisterDataAccessListenerProgressOut(this);
                    this.Picking.RegisterDataAccessListenerStatusOut(this);
                    await this.Picking.RequestData(null);
                    this.Picking.UnregisterDataAccessListenerStatusOut(this);
                    this.Picking.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.PickingItem.RegisterDataAccessListenerProgressOut(this);
                    this.PickingItem.RegisterDataAccessListenerStatusOut(this);
                    await this.PickingItem.RequestData(0, null);
                    this.PickingItem.UnregisterDataAccessListenerStatusOut(this);
                    this.PickingItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                    {
                        this.PickingDetail.RegisterDataAccessListenerProgressOut(this);
                        this.PickingDetail.RegisterDataAccessListenerStatusOut(this);
                        await this.PickingDetail.Request(0);
                        this.PickingDetail.UnregisterDataAccessListenerStatusOut(this);
                        this.PickingDetail.UnregisterDataAccessListenerProgressOut(this);
                        this.StatusOut(string.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task RefreshDataTransfers()
        {
            if (this.SysEnv.RefreshOpt.Transfers || this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                this.TransferFromDetail.RegisterDataAccessListenerStatusOut(this);
                await this.TransferFromDetail.Purge();
                this.TransferFromDetail.UnregisterDataAccessListenerStatusOut(this);

                this.TransferFromItem.RegisterDataAccessListenerStatusOut(this);
                await this.TransferFromItem.Purge();
                this.TransferFromItem.UnregisterDataAccessListenerStatusOut(this);

                this.TransferFrom.RegisterDataAccessListenerStatusOut(this);
                await this.TransferFrom.Purge();
                this.TransferFrom.UnregisterDataAccessListenerStatusOut(this);

                this.TransferToDetail.RegisterDataAccessListenerStatusOut(this);
                await this.TransferToDetail.Purge();
                this.TransferToDetail.UnregisterDataAccessListenerStatusOut(this);

                this.TransferToItem.RegisterDataAccessListenerStatusOut(this);
                await this.TransferToItem.Purge();
                this.TransferToItem.UnregisterDataAccessListenerStatusOut(this);

                this.TransferTo.RegisterDataAccessListenerStatusOut(this);
                await this.TransferTo.Purge();
                this.TransferTo.UnregisterDataAccessListenerStatusOut(this);

                if (this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                {
                    this.TransferFrom.RegisterDataAccessListenerProgressOut(this);
                    this.TransferFrom.RegisterDataAccessListenerStatusOut(this);
                    await this.TransferFrom.Request();
                    this.TransferFrom.UnregisterDataAccessListenerStatusOut(this);
                    this.TransferFrom.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.TransferFromItem.RegisterDataAccessListenerProgressOut(this);
                    this.TransferFromItem.RegisterDataAccessListenerStatusOut(this);
                    await this.TransferFromItem.Request(0);
                    this.TransferFromItem.UnregisterDataAccessListenerStatusOut(this);
                    this.TransferFromItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.TransferTo.RegisterDataAccessListenerProgressOut(this);
                    this.TransferTo.RegisterDataAccessListenerStatusOut(this);
                    await this.TransferTo.Request();
                    this.TransferTo.UnregisterDataAccessListenerStatusOut(this);
                    this.TransferTo.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.TransferToItem.RegisterDataAccessListenerProgressOut(this);
                    this.TransferToItem.RegisterDataAccessListenerStatusOut(this);
                    await this.TransferToItem.Request(0);
                    this.TransferToItem.UnregisterDataAccessListenerStatusOut(this);
                    this.TransferToItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                    {
                        this.TransferFromDetail.RegisterDataAccessListenerProgressOut(this);
                        this.TransferFromDetail.RegisterDataAccessListenerStatusOut(this);
                        await this.TransferFromDetail.Request(0);
                        this.TransferFromDetail.UnregisterDataAccessListenerStatusOut(this);
                        this.TransferFromDetail.UnregisterDataAccessListenerProgressOut(this);
                        this.StatusOut(string.Empty);

                        this.TransferToDetail.RegisterDataAccessListenerProgressOut(this);
                        this.TransferToDetail.RegisterDataAccessListenerStatusOut(this);
                        await this.TransferToDetail.Request(0);
                        this.TransferToDetail.UnregisterDataAccessListenerStatusOut(this);
                        this.TransferToDetail.UnregisterDataAccessListenerProgressOut(this);
                        this.StatusOut(string.Empty);

                    }
                }
            }
        }

        private async void BtnClose_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            await this.Close();
        }
    }
}