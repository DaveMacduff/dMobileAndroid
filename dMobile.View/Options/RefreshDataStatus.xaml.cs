// <copyright file="RefreshDataStatus.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Options
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using Bedrock;
    using Controller.Activity_Detail;
    using dMobile.Controller.Activity;
    using dMobile.Controller.Activity_Item;
    using Forms;
    using GroundWork;
    using Resources.Enums;
    using Resources.Types;
    using SQLite;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    ///     Refresh database status.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RefreshDataStatus : IAppArea, ISysSettingsEventListener,
        IBarCodeSettingsEventListener, IPrnSettingsListener, IProdSchemeLabelSettingsListener,
        IDataNubListener, IDataAccessEventListener
    {
        private bool isFirstAppearance = true;
        private int messageIndex;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RefreshDataStatus" /> class.
        /// </summary>
        /// <param name="actSubTypes"><see cref="SysCommon.ActSubTypes" /> Activity subtypes.</param>
        public RefreshDataStatus(SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.ActSubType = actSubTypes;
            this.IsIndicatorRunning = false;
            this.Title = this.GetControlDescription();
        }

        /// <summary>
        ///     Gets the <see cref="SysEnvironment" />.
        /// </summary>
        private SysEnvironment SysEnv { get; } = SysEnvironment.GetInstance();

        /// <summary>
        ///     Gets or sets a value indicating whether the data refresh is complete.
        /// </summary>
        private bool IsRefreshComplete { get; set; }

        /// <summary>
        ///     Gets the activity subtype.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; }

        /// <summary>
        ///     Gets the full class name.
        /// </summary>
        private string FullClassName { get; } = typeof(RefreshDataStatus).FullName;

        /// <summary>
        ///     Gets the total refresh data time.
        /// </summary>
        private DateTime RefreshTotal { get; } = DateTime.Now;

        /// <summary>
        ///     Gets the product location map type table.
        /// </summary>
        private TableProductLocationMapType ProdLocMapType { get; } = new TableProductLocationMapType();

        /// <summary>
        ///     Gets the unit of measure table.
        /// </summary>
        private TableUnit Unit { get; } = new TableUnit();

        /// <summary>
        ///     Gets the route table.
        /// </summary>
        private TableRoute Route { get; } = new TableRoute();

        /// <summary>
        ///     Gets the product scheme table.
        /// </summary>
        private TableProductScheme ProdScheme { get; } = new TableProductScheme();

        /// <summary>
        ///     Gets the product scheme variant table.
        /// </summary>
        private TableProductSchemeVariant ProdSchemeVariant { get; } = new TableProductSchemeVariant();

        /// <summary>
        ///     Gets the product scheme variant xref table.
        /// </summary>
        private TableProductSchemeVariantXRef ProdSchemeVariantXRef { get; } = new TableProductSchemeVariantXRef();

        /// <summary>
        ///     Gets the product scheme lot number table.
        /// </summary>
        private TableProductSchemeLotNumber ProdSchemeLot { get; } = new TableProductSchemeLotNumber();

        /// <summary>
        ///     Gets the product scheme lot number xref table.
        /// </summary>
        private TableProductSchemeLotNumberXRef ProdSchemeLotXRef { get; } = new TableProductSchemeLotNumberXRef();

        /// <summary>
        ///     Gets the products table.
        /// </summary>
        private TableProduct Product { get; } = new TableProduct();

        /// <summary>
        ///     Gets the product families table.
        /// </summary>
        private TableFamily Family { get; } = new TableFamily();

        /// <summary>
        ///     Gets the product units table.
        /// </summary>
        private TableProductUnit ProdUnit { get; } = new TableProductUnit();

        /// <summary>
        ///     Gets the product unit relations table.
        /// </summary>
        private TableProductUnitRelation ProdUnitRelation { get; } = new TableProductUnitRelation();

        /// <summary>
        ///     Gets the product unit bar codes table.
        /// </summary>
        private TableProductUnitBarCode ProdUnitBarCode { get; } = new TableProductUnitBarCode();

        /// <summary>
        ///     Gets the location mpa warehouses table.
        /// </summary>
        private TableLocationMapWarehouse LocMapWarehouse { get; } = new TableLocationMapWarehouse();

        /// <summary>
        ///     Gets the location map sections table.
        /// </summary>
        private TableLocationMapSection LocMapSection { get; } = new TableLocationMapSection();

        /// <summary>
        ///     Gets the location map types table.
        /// </summary>
        private TableLocationMapType LocMapType { get; } = new TableLocationMapType();

        /// <summary>
        ///     Gets the location map slot coordinates table.
        /// </summary>
        private TableLocationMapSlotCoordinate LocMapSlotCoordinate { get; } = new TableLocationMapSlotCoordinate();

        /// <summary>
        ///     Gets the location map slots table.
        /// </summary>
        private TableLocationMapSlot LocMapSlot { get; } = new TableLocationMapSlot();

        /// <summary>
        ///     Gets the warehouse location map types table.
        /// </summary>
        private TableWarehouseLocationMapType WhsLocMapType { get; } = new TableWarehouseLocationMapType();

        /// <summary>
        ///     Gets the contact users table.
        /// </summary>
        private TableContactUsers ContactUsers { get; } = new TableContactUsers();

        /// <summary>
        ///     Gets the table activity physical count.
        /// </summary>
        private TableActivityPhysicalCount PhysicalCount { get; } = new TableActivityPhysicalCount();

        /// <summary>
        ///     Gets the table activity physical count item.
        /// </summary>
        private TableActivityPhysicalCountItem PhysicalCountItem { get; } = new TableActivityPhysicalCountItem();

        /// <summary>
        ///     Gets the table activity physical count detail.
        /// </summary>
        private TableActivityPhysicalCountDetail PhysicalCountDetail { get; } = new TableActivityPhysicalCountDetail();

        /// <summary>
        ///     Gets the table activity cycle count.
        /// </summary>
        private TableActivityCycleCount CycleCount { get; } = new TableActivityCycleCount();

        /// <summary>
        ///     Gets the table activity cycle count item.
        /// </summary>
        private TableActivityCycleCountItem CycleCountItem { get; } = new TableActivityCycleCountItem();

        /// <summary>
        ///     Gets the table activity cycle count detail.
        /// </summary>
        private TableActivityCycleCountDetail CycleCountDetail { get; } = new TableActivityCycleCountDetail();

        /// <summary>
        ///     Gets or sets the table activity detail product scheme variant xref.
        /// </summary>
        private TableActivityDetailProductSchemeVariantXRef ActDetailProdSchemeVariantXRef { get; set; } =
            new TableActivityDetailProductSchemeVariantXRef();

        /// <summary>
        ///     Gets the table activity item bar code association xref.
        /// </summary>
        private TableActivityItemBarCodeAssociationXRef ActItemBarCodeAssocXRef { get; } =
            new TableActivityItemBarCodeAssociationXRef();

        /// <summary>
        ///     Gets the table activity picking.
        /// </summary>
        private TableActivityPicking Picking { get; } = new TableActivityPicking();

        /// <summary>
        ///     Gets the table activity picking item.
        /// </summary>
        private TableActivityPickingItem PickingItem { get; } = new TableActivityPickingItem();

        /// <summary>
        ///     Gets the table activity picking item comment.
        /// </summary>
        private TableActivityPickingItemComment PickingItemComment { get; } = new TableActivityPickingItemComment();

        /// <summary>
        ///     Gets the table activity picking detail.
        /// </summary>
        private TableActivityPickingDetail PickingDetail { get; } = new TableActivityPickingDetail();

        /// <summary>
        ///     Gets the table activity picking route.
        /// </summary>
        private TableActivityPickingRoute PickingRoute { get; } = new TableActivityPickingRoute();

        /// <summary>
        ///     Gets the table activity receiving.
        /// </summary>
        private TableActivityReceiving Receiving { get; } = new TableActivityReceiving();

        /// <summary>
        ///     Gets the table activity receiving item.
        /// </summary>
        private TableActivityReceivingItem ReceivingItem { get; } = new TableActivityReceivingItem();

        /// <summary>
        ///     Gets the table activity receiving item comment.
        /// </summary>
        private TableActivityReceivingItemComment ReceivingItemComment { get; } =
            new TableActivityReceivingItemComment();

        /// <summary>
        ///     Gets the table activity receiving detail.
        /// </summary>
        private TableActivityReceivingDetail ReceivingDetail { get; } = new TableActivityReceivingDetail();

        /// <summary>
        ///     Gets the table activity transfer from.
        /// </summary>
        private TableActivityTransferFrom TransferFrom { get; } = new TableActivityTransferFrom();

        /// <summary>
        ///     Gets the table activity transfer from item.
        /// </summary>
        private TableActivityTransferFromItem TransferFromItem { get; } = new TableActivityTransferFromItem();

        /// <summary>
        ///     Gets the table activity transfer from detail.
        /// </summary>
        private TableActivityTransferFromDetail TransferFromDetail { get; } = new TableActivityTransferFromDetail();

        /// <summary>
        ///     Gets the table activity transfer to.
        /// </summary>
        private TableActivityTransferTo TransferTo { get; } = new TableActivityTransferTo();

        /// <summary>
        ///     Gets the table activity transfer to item.
        /// </summary>
        private TableActivityTransferToItem TransferToItem { get; } = new TableActivityTransferToItem();

        /// <summary>
        ///     Gets the table activity transfer to detail.
        /// </summary>
        private TableActivityTransferToDetail TransferToDetail { get; } = new TableActivityTransferToDetail();

        /// <summary>
        ///     Gets the table activity returns.
        /// </summary>
        private TableActivityReturns Returns { get; } = new TableActivityReturns();

        /// <summary>
        ///     Gets the table activity returns item.
        /// </summary>
        private TableActivityReturnsItem ReturnsItem { get; } = new TableActivityReturnsItem();

        /// <summary>
        ///     Gets or sets the table activity returns item comment.
        /// </summary>
        private TableActivityReturnsItemComment ReturnsItemComment { get; set; } =
            new TableActivityReturnsItemComment();

        /// <summary>
        ///     Gets the table activity returns detail.
        /// </summary>
        private TableActivityReturnsDetail ReturnsDetail { get; } = new TableActivityReturnsDetail();

        /// <summary>
        ///     Gets the table activity adjustments.
        /// </summary>
        private TableActivityAdjustment Adjustment { get; } = new TableActivityAdjustment();

        /// <summary>
        ///     Gets the table activity adjustment item.
        /// </summary>
        private TableActivityAdjustmentItem AdjustmentItem { get; } = new TableActivityAdjustmentItem();

        /// <summary>
        ///     Gets the table activity adjustment detail.
        /// </summary>
        private TableActivityAdjustmentDetail AdjustmentDetail { get; } = new TableActivityAdjustmentDetail();

        /// <summary>
        ///     Gets the table activity.
        /// </summary>
        private TableActivity Activity { get; } = new TableActivity();

        /// <summary>
        ///     Gets the table activity item.
        /// </summary>
        private TableActivityItem ActivityItem { get; } = new TableActivityItem();

        /// <summary>
        ///     Gets the table activity production finished tickets.
        /// </summary>
        private TableActivityProductionTicketFinished TicketFinished { get; } =
            new TableActivityProductionTicketFinished();

        /// <summary>
        ///     Gets the table activity production finished item tickets.
        /// </summary>
        private TableActivityProductionTicketFinishedItem TicketFinishedItem { get; } =
            new TableActivityProductionTicketFinishedItem();

        /// <summary>
        ///     Gets the table activity production finished detail tickets.
        /// </summary>
        private TableActivityProductionTicketFinishedDetail TicketFinishedDetail { get; } =
            new TableActivityProductionTicketFinishedDetail();

        /// <summary>
        ///     Gets the table activity production material tickets.
        /// </summary>
        private TableActivityProductionTicketMaterial TicketMaterial { get; } =
            new TableActivityProductionTicketMaterial();

        /// <summary>
        ///     Gets the table activity production material item tickets.
        /// </summary>
        private TableActivityProductionTicketMaterialItem TicketMaterialItem { get; } =
            new TableActivityProductionTicketMaterialItem();

        /// <summary>
        ///     Gets the table activity production material detail tickets.
        /// </summary>
        private TableActivityProductionTicketMaterialDetail TicketMaterialDetail { get; } =
            new TableActivityProductionTicketMaterialDetail();

        /// <summary>
        ///     Gets the table activity production by product tickets.
        /// </summary>
        private TableActivityProductionTicketByProduct TicketByProduct { get; } =
            new TableActivityProductionTicketByProduct();

        /// <summary>
        ///     Gets the table activity production by product item tickets.
        /// </summary>
        private TableActivityProductionTicketByProductItem TicketByProductItem { get; } =
            new TableActivityProductionTicketByProductItem();

        /// <summary>
        ///     Gets the table activity production by product detail tickets.
        /// </summary>
        private TableActivityProductionTicketByProductDetail TicketByProductDetail { get; } =
            new TableActivityProductionTicketByProductDetail();

        /// <summary>
        ///     Gets the table activity purchase order work sheet items.
        /// </summary>
        private TableActivityPurchaseOrderWorkSheetItem PurchaseOrderWorkSheetItem { get; } =
            new TableActivityPurchaseOrderWorkSheetItem();

        /// <summary>
        ///     Gets or sets the printer settings.
        /// </summary>
        private PrinterSettings PrinterSettings { get; set; } = new PrinterSettings();

        /// <summary>
        ///     Gets or sets the product scheme label settings.
        /// </summary>
        private ProdSchemeLabelSettings SchemeLabelSettings { get; set; } = new ProdSchemeLabelSettings();

        /// <summary>
        ///     Gets the location map warehouse product scheme.
        /// </summary>
        private TableLocationMapWarehouseProductScheme ProdLocMapWhsProdScheme { get; } =
            new TableLocationMapWarehouseProductScheme();

        /// <summary>
        ///     Gets or sets the extra information items.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Message> ExtraInfoItems { get; set; } = new ObservableCollection<Message>();

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns></returns>
        public async Task ScannerDisable()
        {
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns></returns>
        public async Task ScannerEnable()
        {
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = true;

            try
            {
            }
            catch (Exception ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.FullClassName, Resource.IsReadyToSave, ex) == false)
                {
                    // Application.Exit();
                }

                isReadyToSave = false;
            }

            return isReadyToSave;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsReadyToClose()
        {
            return true;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveChanges()
        {
            return true;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CancelChanges()
        {
            return true;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public async Task<XDocument> GetXml()
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<IsRefreshComplete>" + this.IsRefreshComplete + "</IsRefreshComplete>"
                + "</params>");

            return xmldocParams;
        }

        /// <summary>
        /// </summary>
        /// <param name="BarCodeSettingsEventPublisher"></param>
        /// <param name="barCodeSettingsEventArgs"></param>
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
        /// </summary>
        /// <param name="BarCodeSettingsEventPublisher"></param>
        /// <param name="barCodeSettingsEventArgs"></param>
        /// <remarks></remarks>
        /// <inheritdoc />
        public void OnBarCodeSettingsStatusOut(
            IBarCodeSettingsEventPublisher barCodeSettingsEventPublisher,
            BarCodeSettingsEventArgs barCodeSettingsEventArgs)
        {
            this.StatusOut(barCodeSettingsEventArgs.StatusOutMsg);
        }

        /// <summary>
        /// </summary>
        /// <param name="dataAccessEventPublisher"></param>
        /// <param name="dataAccessEventArgs"></param>
        /// <inheritdoc />
        public void OnDataAccessStatusOut(
            IDataAccessEventPublisher dataAccessEventPublisher,
            DataAccessEventArgs dataAccessEventArgs)
        {
            this.StatusOut(dataAccessEventArgs.StatusOutMsg);
        }

        /// <summary>
        /// </summary>
        /// <param name="dataAccessEventPublisher"></param>
        /// <param name="dataAccessEventArgs"></param>
        /// <inheritdoc />
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
        /// </summary>
        /// <param name="databaseNubEventPublisher"></param>
        /// <param name="databaseNubEventArgs"></param>
        /// <inheritdoc />
        public void OnNubStatOut(IDataNubPublisher databaseNubEventPublisher, DataNubEventArgs databaseNubEventArgs)
        {
            this.StatusOut(databaseNubEventArgs.StatusOutMsg);
        }

        /// <summary>
        /// </summary>
        /// <param name="PrinterSettingsEventPublisher"></param>
        /// <param name="PrinterSettingsEventArgs"></param>
        /// <remarks></remarks>
        /// <inheritdoc />
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
        /// </summary>
        /// <param name="PrinterSettingsEventPublisher"></param>
        /// <param name="PrinterSettingsEventArgs"></param>
        /// <remarks></remarks>
        /// <inheritdoc />
        public void OnPrnSettingsStatusOut(
            IPrnSettingsPublisher printerSettingsEventPublisher,
            PrnSettingsArgs printerSettingsEventArgs)
        {
            this.StatusOut(printerSettingsEventArgs.StatusOutMsg);
        }

        /// <summary>
        /// </summary>
        /// <param name="prodSchemeLabelSettingsPublisher"></param>
        /// <param name="prodSchemeLabelSettingsArgs"></param>
        /// <inheritdoc />
        public void OnProdSchemeLabelSettingsStatusOut(
            IProdSchemeLabelSettingsPublisher prodSchemeLabelSettingsPublisher,
            ProdSchemeLabelSettingsEventArgs prodSchemeLabelSettingsArgs)
        {
            this.StatusOut(prodSchemeLabelSettingsArgs.StatusOutMsg);
        }

        /// <summary>
        /// </summary>
        /// <param name="prodSchemeLabelSettingsPublisher"></param>
        /// <param name="prodSchemeLabelSettingsArgs"></param>
        /// <inheritdoc />
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
        /// </summary>
        /// <param name="sysSettingsEventPublisher"></param>
        /// <param name="sysSettingsEventArgs"></param>
        /// <remarks></remarks>
        /// <inheritdoc />
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
        /// </summary>
        /// <param name="sysSettingsEventPublisher"></param>
        /// <param name="sysSettingsEventArgs"></param>
        /// <remarks></remarks>
        /// <inheritdoc />
        public void OnSysSettingsStatusOut(
            ISysSettingsEventPublisher sysSettingsEventPublisher,
            SysSettingsEventArgs sysSettingsEventArgs)
        {
            this.StatusOut(sysSettingsEventArgs.StatusOutMsg);
        }

        /// <summary>
        ///     The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                this.IsIndicatorRunning = true;
                base.OnAppearing();

                this.lstStatus.ItemsSource = this.ExtraInfoItems;

                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(),
                    keyPressed => { _ = Enum.TryParse(keyPressed, out Keycode keycode); });

                if (this.isFirstAppearance)
                {
                    this.isFirstAppearance = false;
                    await this.RefreshDataStatus_GotFocus(this, EventArgs.Empty);
                }

                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        ///     The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <inheritdoc />
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(BackButtonCallback);
            return true;

            //////////////////////////////////////////////////////////////////////////
            async void BackButtonCallback()
            {
                _ = base.OnBackButtonPressed();

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
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            if (this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                return Resource.DMobilePurgeActivityData;
            }

            return "Refresh Data Status";
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucRefreshDataStatus_GotFocus(object sender, EventArgs e)
        {
            // load refresh criteria
            this.IsRefreshComplete = false;
            this.SysEnv.RefreshOpt = this.SysEnv.RefreshOpt.LoadData();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task RefreshDataStatus_GotFocus(object sender, EventArgs e)
        {
            var refreshLast = new DateTime(1900, 1, 1);

            this.IsIndicatorRunning = true;

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

                        var errRpt = new FormErrorReport(this.ActSubType, ex, this.FullClassName,
                            "tmrRefreshData_Tick");
                        _ = await Device.InvokeOnMainThreadAsync(async () => await errRpt.ShowDialog());
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
                            var tempRefreshOpt1 = this.SysEnv.RefreshOpt;
                            _ = this.SysEnv.RefreshOpt.SaveData(ref tempRefreshOpt1);
                        }

                        // Store the last refresh date
                        refreshLast = this.SysEnv.RefreshOpt.RefreshedDataLast;

                        if (this.SysEnv.RefreshOpt.SystemData && !this.SysEnv.RefreshOpt.RefreshDataChangesOnly &&
                            this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                        {
                            await this.CreateDatabase();
                            this.SysEnv.RefreshOpt.IsDataCreatedInStartUp = false;
                            this.SysEnv.RefreshOpt.RefreshedDataLast = new DateTime(1900, 1, 1);
                            var tempRefreshOpt2 = this.SysEnv.RefreshOpt;
                            _ = this.SysEnv.RefreshOpt.SaveData(ref tempRefreshOpt2);

                            // Assign the last refresh date to DateSerial(1900, 1, 1) because after the database is recreated
                            // we want everything
                            refreshLast = this.SysEnv.RefreshOpt.RefreshedDataLast;
                            await this.RefreshSystemData();
                        }
                        else if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly &&
                                 this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                        {
                            DatabaseConnection.Open();
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
                            var span = DateTime.Now - this.RefreshTotal;
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

                        var errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex, this.FullClassName,
                            "tmrRefreshData_Tick");
                        _ = await Device.InvokeOnMainThreadAsync(async () => await errRpt.ShowDialog());
                    }
                    catch (Exception ex)
                    {
                        this.SysEnv.RefreshOpt.RefreshedDataLast = refreshLast;
                        this.SysEnv.RefreshOpt.IsRefreshDataFailed = true;

                        this.StatusOut(Resource.RefreshDataFailed);

                        var errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex, this.FullClassName,
                            "tmrRefreshData_Tick");
                        _ = await Device.InvokeOnMainThreadAsync(async () => await errRpt.ShowDialog());
                    }
                    finally
                    {
                        this.SysEnv.RefreshOpt.IsRefreshedLastDataChangesOnly =
                            this.SysEnv.RefreshOpt.RefreshDataChangesOnly;
                        var tempRefreshOpt3 = this.SysEnv.RefreshOpt;
                        _ = this.SysEnv.RefreshOpt.SaveData(ref tempRefreshOpt3);
                    }
                }

                // }
            });

            this.IsIndicatorRunning = false;
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateDatabase()
        {
            var db = new DatabaseNub();

            try
            {
                var isCreateDatabase = true;

                db.RegNubListenerStatOut(this);

                this.StatusOut(Resource.LocatingDatabase);

                if (File.Exists(DatabaseConnection.GetInstance().DatabaseFullPath))
                {
                    this.StatusOut(Resource.Found + DatabaseConnection.GetInstance().DatabaseFullPath);

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

                    var users = new TableContactUsers();
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
        /// </summary>
        /// <remarks></remarks>
        private async Task RefreshSystemData()
        {
            this.SysEnv.SysSettings.RegisterSysSettingsListenerStatusOut(this);
            this.SysEnv.SysSettings.RegisterSysSettingsListenerProgressOut(this);
            var settings = this.SysEnv.SysSettings;
            SystemSettingsTasks.RequestSystemSettings(ref settings);
            this.SysEnv.SysSettings = settings;

            var tempSystemSettings = this.SysEnv.SysSettings;
            _ = SystemSettingsTasks.SaveData(ref tempSystemSettings);

            this.SysEnv.SysSettings.UnregisterSysSettingsListenerStatusOut(this);
            this.SysEnv.SysSettings.UnregisterSysSettingsListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.SysEnv.BarCodeSettings.RegisterBarCodeSettingsListenerStatusOut(this);
            this.SysEnv.BarCodeSettings.RegisterBarCodeSettingsListenerProgressOut(this);
            this.SysEnv.BarCodeSettings.RequestBarCodeSettings();
            var tempBarCodeSettings = this.SysEnv.BarCodeSettings;
            _ = this.SysEnv.BarCodeSettings.SaveData(ref tempBarCodeSettings);
            this.SysEnv.BarCodeSettings.UnregisterBarCodeSettingsListenerStatusOut(this);
            this.SysEnv.BarCodeSettings.UnregisterBarCodeSettingsListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.PrinterSettings.RegPrnSettingsListenerStatusOut(this);
            this.PrinterSettings.RegPrnSettingsListenerProgressOut(this);
            this.PrinterSettings.RequestPrinterSettings();
            var tempPrinterSettings = this.PrinterSettings;
            _ = this.PrinterSettings.SaveData(ref tempPrinterSettings);
            this.PrinterSettings = tempPrinterSettings;
            this.PrinterSettings.UnregPrnSettingsListenerStatusOut(this);
            this.PrinterSettings.UnregPrnSettingsListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.ContactUsers.RegisterDataAccessListenerProgressOut(this);
            this.ContactUsers.RegisterDataAccessListenerStatusOut(this);
            await this.ContactUsers.PurgeTContact();
            await this.ContactUsers.PurgeData();
            await this.ContactUsers.RequestData();
            this.ContactUsers.UnregisterDataAccessListenerStatusOut(this);
            this.ContactUsers.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            await this.RefreshSystemDataLocationMap();

            this.Route.RegisterDataAccessListenerProgressOut(this);
            this.Route.RegisterDataAccessListenerStatusOut(this);
            await this.Route.RequestData();
            this.Route.UnregisterDataAccessListenerStatusOut(this);
            this.Route.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.Family.RegisterDataAccessListenerProgressOut(this);
            this.Family.RegisterDataAccessListenerStatusOut(this);
            await this.Family.RequestData();
            this.Family.UnregisterDataAccessListenerStatusOut(this);
            this.Family.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            await this.RefreshSystemDataProducts();

            if (!await Common.IsDataBaseDataExist())
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    this.GetControlDescription(),
                    Resource.SomeExpectedDataWasNotRefreshedRecommendRefreshingDataAgain,
                    Resource.OK));
            }
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private async Task DataStatus()
        {
            this.SysEnv.RefreshOpt = this.SysEnv.RefreshOpt.LoadData();

            await Device.InvokeOnMainThreadAsync(() =>
            {
                this.ExtraInfoItems.Add(new Message
                {
                    MessageIndex = this.messageIndex++,
                    MessageValue = Resource.LastRefresh + " " + this.SysEnv.RefreshOpt.RefreshedDataLast
                });
                this.ExtraInfoItems.Add(new Message
                {
                    MessageIndex = this.messageIndex++,
                    MessageValue = Resource.RefreshChangesOnly + " " +
                                   this.SysEnv.RefreshOpt.IsRefreshedLastDataChangesOnly
                });
            });

            this.ContactUsers.RegisterDataAccessListenerProgressOut(this);
            this.ContactUsers.RegisterDataAccessListenerStatusOut(this);
            _ = await this.ContactUsers.DataStatus();
            this.ContactUsers.UnregisterDataAccessListenerStatusOut(this);
            this.ContactUsers.UnregisterDataAccessListenerProgressOut(this);

            await this.DataStatusLocationMap();
            await this.DataStatusProducts();

            this.Route.RegisterDataAccessListenerProgressOut(this);
            this.Route.RegisterDataAccessListenerStatusOut(this);
            _ = await this.Route.DataStatus();
            this.Route.UnregisterDataAccessListenerStatusOut(this);
            this.Route.UnregisterDataAccessListenerProgressOut(this);

            this.Family.RegisterDataAccessListenerProgressOut(this);
            this.Family.RegisterDataAccessListenerStatusOut(this);
            _ = await this.Family.DataStatus();
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
                _ = await this.PurchaseOrderWorkSheetItem.DataItemStatus();
                this.PurchaseOrderWorkSheetItem.UnregisterDataAccessListenerStatusOut(this);
                this.PurchaseOrderWorkSheetItem.UnregisterDataAccessListenerProgressOut(this);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="message"></param>
        private void StatusOut(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.ExtraInfoItems.Add(new Message { MessageIndex = this.messageIndex++, MessageValue = message });
                this.lstStatus.ScrollTo(this.ExtraInfoItems.LastOrDefault(), ScrollToPosition.MakeVisible, false);
            });
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="value"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        private void ProgressOut(object sender, EventArgs e, int value, int minValue, int maxValue)
        {
            // this.pbarStatus.Maximum = maxValue;
            // this.pbarStatus.Minimum = minValue;
            // this.pbarStatus.Value = value;
        }

        /// <summary>
        /// </summary>
        private async Task RefreshSystemDataLocationMap()
        {
            this.LocMapWarehouse.RegisterDataAccessListenerProgressOut(this);
            this.LocMapWarehouse.RegisterDataAccessListenerStatusOut(this);
            await this.LocMapWarehouse.RequestData();
            this.LocMapWarehouse.UnregisterDataAccessListenerStatusOut(this);
            this.LocMapWarehouse.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.LocMapSection.RegisterDataAccessListenerProgressOut(this);
            this.LocMapSection.RegisterDataAccessListenerStatusOut(this);
            await this.LocMapSection.RequestData(0);
            this.LocMapSection.UnregisterDataAccessListenerStatusOut(this);
            this.LocMapSection.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.LocMapSlotCoordinate.RegisterDataAccessListenerProgressOut(this);
            this.LocMapSlotCoordinate.RegisterDataAccessListenerStatusOut(this);
            await this.LocMapSlotCoordinate.RequestData();
            this.LocMapSlotCoordinate.UnregisterDataAccessListenerStatusOut(this);
            this.LocMapSlotCoordinate.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.LocMapSlot.RegisterDataAccessListenerProgressOut(this);
            this.LocMapSlot.RegisterDataAccessListenerStatusOut(this);
            await this.LocMapSlot.RequestData(0);
            this.LocMapSlot.UnregisterDataAccessListenerStatusOut(this);
            this.LocMapSlot.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.LocMapType.RegisterDataAccessListenerProgressOut(this);
            this.LocMapType.RegisterDataAccessListenerStatusOut(this);
            await this.LocMapType.RequestData();
            this.LocMapType.UnregisterDataAccessListenerStatusOut(this);
            this.LocMapType.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.WhsLocMapType.RegisterDataAccessListenerProgressOut(this);
            this.WhsLocMapType.RegisterDataAccessListenerStatusOut(this);
            await this.WhsLocMapType.RequestData();
            this.WhsLocMapType.UnregisterDataAccessListenerStatusOut(this);
            this.WhsLocMapType.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);
        }

        /// <summary>
        /// </summary>
        private async Task RefreshSystemDataProducts()
        {
            this.Unit.RegisterDataAccessListenerProgressOut(this);
            this.Unit.RegisterDataAccessListenerStatusOut(this);
            await this.Unit.RequestData();
            this.Unit.UnregisterDataAccessListenerStatusOut(this);
            this.Unit.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.ProdScheme.RegisterDataAccessListenerProgressOut(this);
            this.ProdScheme.RegisterDataAccessListenerStatusOut(this);
            await this.ProdScheme.RequestData();
            this.ProdScheme.UnregisterDataAccessListenerStatusOut(this);
            this.ProdScheme.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.ProdSchemeVariant.RegisterDataAccessListenerProgressOut(this);
            this.ProdSchemeVariant.RegisterDataAccessListenerStatusOut(this);
            await this.ProdSchemeVariant.RequestData();
            this.ProdSchemeVariant.UnregisterDataAccessListenerStatusOut(this);
            this.ProdSchemeVariant.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.ProdSchemeVariantXRef.RegisterDataAccessListenerProgressOut(this);
            this.ProdSchemeVariantXRef.RegisterDataAccessListenerStatusOut(this);
            await this.ProdSchemeVariantXRef.RequestData();
            this.ProdSchemeVariantXRef.UnregisterDataAccessListenerStatusOut(this);
            this.ProdSchemeVariantXRef.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            // This xml settings file need to be called hear not called from the refresh system data method,
            // because scheme codes must exist in there table. This routine seeks the scheme code.
            this.SchemeLabelSettings.RegProdSchemeLabelSettingsListenerStatusOut(this);
            this.SchemeLabelSettings.RegProdSchemeLabelSettingsListenerProgressOut(this);
            await this.SchemeLabelSettings.RequestProductSchemeLabelSettings();
            var tempSchemeLblSettings = this.SchemeLabelSettings;
            _ = this.SchemeLabelSettings.SaveData(ref tempSchemeLblSettings);
            this.SchemeLabelSettings = tempSchemeLblSettings;
            this.SchemeLabelSettings.UnregProdSchemeLabelSettingsListenerStatusOut(this);
            this.SchemeLabelSettings.UnregProdSchemeLabelSettingsListenerProgressOut(this);
            this.StatusOut(string.Empty);

            if (this.SysEnv.SysSettings.IsEnableDefaultDetailValues ||
                this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                this.ProdSchemeLot.RegisterDataAccessListenerProgressOut(this);
                this.ProdSchemeLot.RegisterDataAccessListenerStatusOut(this);
                await this.ProdSchemeLot.RequestData();
                this.ProdSchemeLot.UnregisterDataAccessListenerStatusOut(this);
                this.ProdSchemeLot.UnregisterDataAccessListenerProgressOut(this);
                this.StatusOut(string.Empty);

                this.ProdSchemeLotXRef.RegisterDataAccessListenerProgressOut(this);
                this.ProdSchemeLotXRef.RegisterDataAccessListenerStatusOut(this);
                await this.ProdSchemeLotXRef.RequestData();
                this.ProdSchemeLotXRef.UnregisterDataAccessListenerStatusOut(this);
                this.ProdSchemeLotXRef.UnregisterDataAccessListenerProgressOut(this);
                this.StatusOut(string.Empty);
            }

            this.Product.RegisterDataAccessListenerProgressOut(this);
            this.Product.RegisterDataAccessListenerStatusOut(this);
            await this.Product.RequestData();
            this.Product.UnregisterDataAccessListenerStatusOut(this);
            this.Product.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.ProdUnit.RegisterDataAccessListenerProgressOut(this);
            this.ProdUnit.RegisterDataAccessListenerStatusOut(this);
            await this.ProdUnit.RequestData();
            this.ProdUnit.UnregisterDataAccessListenerStatusOut(this);
            this.ProdUnit.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.ProdUnitRelation.RegisterDataAccessListenerProgressOut(this);
            this.ProdUnitRelation.RegisterDataAccessListenerStatusOut(this);
            await this.ProdUnitRelation.RequestData();
            this.ProdUnitRelation.UnregisterDataAccessListenerStatusOut(this);
            this.ProdUnitRelation.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.ProdUnitBarCode.RegisterDataAccessListenerProgressOut(this);
            this.ProdUnitBarCode.RegisterDataAccessListenerStatusOut(this);
            await this.ProdUnitBarCode.RequestData();
            this.ProdUnitBarCode.UnregisterDataAccessListenerStatusOut(this);
            this.ProdUnitBarCode.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.ProdLocMapWhsProdScheme.RegisterDataAccessListenerProgressOut(this);
            this.ProdLocMapWhsProdScheme.RegisterDataAccessListenerStatusOut(this);
            await this.ProdLocMapWhsProdScheme.RequestData();
            this.ProdLocMapWhsProdScheme.UnregisterDataAccessListenerStatusOut(this);
            this.ProdLocMapWhsProdScheme.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);

            this.ProdLocMapType.RegisterDataAccessListenerProgressOut(this);
            this.ProdLocMapType.RegisterDataAccessListenerStatusOut(this);
            await this.ProdLocMapType.RequestData();
            this.ProdLocMapType.UnregisterDataAccessListenerStatusOut(this);
            this.ProdLocMapType.UnregisterDataAccessListenerProgressOut(this);
            this.StatusOut(string.Empty);
        }

        /// <summary>
        /// </summary>
        private async Task DataStatusLocationMap()
        {
            this.LocMapWarehouse.RegisterDataAccessListenerProgressOut(this);
            this.LocMapWarehouse.RegisterDataAccessListenerStatusOut(this);
            _ = await this.LocMapWarehouse.DataStatus();
            this.LocMapWarehouse.UnregisterDataAccessListenerStatusOut(this);
            this.LocMapWarehouse.UnregisterDataAccessListenerProgressOut(this);

            this.LocMapSection.RegisterDataAccessListenerProgressOut(this);
            this.LocMapSection.RegisterDataAccessListenerStatusOut(this);
            _ = await this.LocMapSection.DataStatus();
            this.LocMapSection.UnregisterDataAccessListenerStatusOut(this);
            this.LocMapSection.UnregisterDataAccessListenerProgressOut(this);

            this.LocMapSlotCoordinate.RegisterDataAccessListenerProgressOut(this);
            this.LocMapSlotCoordinate.RegisterDataAccessListenerStatusOut(this);
            _ = await this.LocMapSlotCoordinate.DataStatus();
            this.LocMapSlotCoordinate.UnregisterDataAccessListenerStatusOut(this);
            this.LocMapSlotCoordinate.UnregisterDataAccessListenerProgressOut(this);

            this.LocMapSlot.RegisterDataAccessListenerProgressOut(this);
            this.LocMapSlot.RegisterDataAccessListenerStatusOut(this);
            _ = await this.LocMapSlot.DataStatus();
            this.LocMapSlot.UnregisterDataAccessListenerStatusOut(this);
            this.LocMapSlot.UnregisterDataAccessListenerProgressOut(this);

            this.WhsLocMapType.RegisterDataAccessListenerProgressOut(this);
            this.WhsLocMapType.RegisterDataAccessListenerStatusOut(this);
            _ = await this.WhsLocMapType.DataStatus();
            this.WhsLocMapType.UnregisterDataAccessListenerStatusOut(this);
            this.WhsLocMapType.UnregisterDataAccessListenerProgressOut(this);
        }

        /// <summary>
        /// </summary>
        private async Task DataStatusProducts()
        {
            this.Unit.RegisterDataAccessListenerProgressOut(this);
            this.Unit.RegisterDataAccessListenerStatusOut(this);
            _ = await this.Unit.DataStatus();
            this.Unit.UnregisterDataAccessListenerStatusOut(this);
            this.Unit.UnregisterDataAccessListenerProgressOut(this);

            this.Product.RegisterDataAccessListenerProgressOut(this);
            this.Product.RegisterDataAccessListenerStatusOut(this);
            _ = await this.Product.DataStatus();
            this.Product.UnregisterDataAccessListenerStatusOut(this);
            this.Product.UnregisterDataAccessListenerProgressOut(this);

            this.ProdUnit.RegisterDataAccessListenerProgressOut(this);
            this.ProdUnit.RegisterDataAccessListenerStatusOut(this);
            _ = await this.ProdUnit.DataStatus();
            this.ProdUnit.UnregisterDataAccessListenerStatusOut(this);
            this.ProdUnit.UnregisterDataAccessListenerProgressOut(this);

            this.ProdUnitBarCode.RegisterDataAccessListenerProgressOut(this);
            this.ProdUnitBarCode.RegisterDataAccessListenerStatusOut(this);
            _ = await this.ProdUnitBarCode.DataStatus();
            this.ProdUnitBarCode.UnregisterDataAccessListenerStatusOut(this);
            this.ProdUnitBarCode.UnregisterDataAccessListenerProgressOut(this);

            this.ProdScheme.RegisterDataAccessListenerProgressOut(this);
            this.ProdScheme.RegisterDataAccessListenerStatusOut(this);
            _ = await this.ProdScheme.DataStatus();
            this.ProdScheme.UnregisterDataAccessListenerStatusOut(this);
            this.ProdScheme.UnregisterDataAccessListenerProgressOut(this);

            this.ProdLocMapType.RegisterDataAccessListenerProgressOut(this);
            this.ProdLocMapType.RegisterDataAccessListenerStatusOut(this);
            _ = await this.ProdLocMapType.DataStatus();
            this.ProdLocMapType.UnregisterDataAccessListenerStatusOut(this);
            this.ProdLocMapType.UnregisterDataAccessListenerProgressOut(this);

            this.ProdSchemeVariant.RegisterDataAccessListenerProgressOut(this);
            this.ProdSchemeVariant.RegisterDataAccessListenerStatusOut(this);
            _ = await this.ProdSchemeVariant.DataStatus();
            this.ProdSchemeVariant.UnregisterDataAccessListenerStatusOut(this);
            this.ProdSchemeVariant.UnregisterDataAccessListenerProgressOut(this);

            this.ProdSchemeLot.RegisterDataAccessListenerProgressOut(this);
            this.ProdSchemeLot.RegisterDataAccessListenerStatusOut(this);
            _ = await this.ProdSchemeLot.DataStatus();
            this.ProdSchemeLot.UnregisterDataAccessListenerStatusOut(this);
            this.ProdSchemeLot.UnregisterDataAccessListenerProgressOut(this);

            this.ProdLocMapWhsProdScheme.RegisterDataAccessListenerProgressOut(this);
            this.ProdLocMapWhsProdScheme.RegisterDataAccessListenerStatusOut(this);
            _ = await this.ProdLocMapWhsProdScheme.DataStatus();
            this.ProdLocMapWhsProdScheme.UnregisterDataAccessListenerStatusOut(this);
            this.ProdLocMapWhsProdScheme.UnregisterDataAccessListenerProgressOut(this);

            this.ProdLocMapType.RegisterDataAccessListenerProgressOut(this);
            this.ProdLocMapType.RegisterDataAccessListenerStatusOut(this);
            _ = await this.ProdLocMapType.DataStatus();
            this.ProdLocMapType.UnregisterDataAccessListenerStatusOut(this);
            this.ProdLocMapType.UnregisterDataAccessListenerProgressOut(this);
        }

        /// <summary>
        /// </summary>
        private async Task DataStatusReceiving()
        {
            this.Receiving.RegisterDataAccessListenerProgressOut(this);
            this.Receiving.RegisterDataAccessListenerStatusOut(this);
            _ = await this.Receiving.DataHeaderStatus();
            this.Receiving.UnregisterDataAccessListenerStatusOut(this);
            this.Receiving.UnregisterDataAccessListenerProgressOut(this);

            this.ReceivingItem.RegisterDataAccessListenerProgressOut(this);
            this.ReceivingItem.RegisterDataAccessListenerStatusOut(this);
            _ = await this.ReceivingItem.DataItemStatus();
            this.ReceivingItem.UnregisterDataAccessListenerStatusOut(this);
            this.ReceivingItem.UnregisterDataAccessListenerProgressOut(this);

            this.ReceivingDetail.RegisterDataAccessListenerProgressOut(this);
            this.ReceivingDetail.RegisterDataAccessListenerStatusOut(this);
            _ = await this.ReceivingDetail.DataDetailStatus();
            this.ReceivingDetail.UnregisterDataAccessListenerStatusOut(this);
            this.ReceivingDetail.UnregisterDataAccessListenerProgressOut(this);
        }

        /// <summary>
        /// </summary>
        private async Task DataStatusPicking()
        {
            this.Picking.RegisterDataAccessListenerProgressOut(this);
            this.Picking.RegisterDataAccessListenerStatusOut(this);
            _ = await this.Picking.DataHeaderStatus();
            this.Picking.UnregisterDataAccessListenerStatusOut(this);
            this.Picking.UnregisterDataAccessListenerProgressOut(this);

            this.PickingItem.RegisterDataAccessListenerProgressOut(this);
            this.PickingItem.RegisterDataAccessListenerStatusOut(this);
            _ = await this.PickingItem.DataItemStatus();
            this.PickingItem.UnregisterDataAccessListenerStatusOut(this);
            this.PickingItem.UnregisterDataAccessListenerProgressOut(this);

            this.PickingDetail.RegisterDataAccessListenerProgressOut(this);
            this.PickingDetail.RegisterDataAccessListenerStatusOut(this);
            _ = await this.PickingDetail.DataDetailStatus();
            this.PickingDetail.UnregisterDataAccessListenerStatusOut(this);
            this.PickingDetail.UnregisterDataAccessListenerProgressOut(this);
        }

        /// <summary>
        /// </summary>
        private async Task DataStatusReturns()
        {
            this.Returns.RegisterDataAccessListenerProgressOut(this);
            this.Returns.RegisterDataAccessListenerStatusOut(this);
            _ = await this.Returns.DataHeaderStatus();
            this.Returns.UnregisterDataAccessListenerStatusOut(this);
            this.Returns.UnregisterDataAccessListenerProgressOut(this);

            this.ReturnsItem.RegisterDataAccessListenerProgressOut(this);
            this.ReturnsItem.RegisterDataAccessListenerStatusOut(this);
            _ = await this.ReturnsItem.DataItemStatus();
            this.ReturnsItem.UnregisterDataAccessListenerStatusOut(this);
            this.ReturnsItem.UnregisterDataAccessListenerProgressOut(this);

            this.ReturnsDetail.RegisterDataAccessListenerProgressOut(this);
            this.ReturnsDetail.RegisterDataAccessListenerStatusOut(this);
            _ = await this.ReturnsDetail.DataDetailStatus();
            this.ReturnsDetail.UnregisterDataAccessListenerStatusOut(this);
            this.ReturnsDetail.UnregisterDataAccessListenerProgressOut(this);
        }

        /// <summary>
        /// </summary>
        private async Task DataStatusTransfers()
        {
            this.TransferFrom.RegisterDataAccessListenerProgressOut(this);
            this.TransferFrom.RegisterDataAccessListenerStatusOut(this);
            _ = await this.TransferFrom.DataHeaderStatus();
            this.TransferFrom.UnregisterDataAccessListenerStatusOut(this);
            this.TransferFrom.UnregisterDataAccessListenerProgressOut(this);

            this.TransferFromItem.RegisterDataAccessListenerProgressOut(this);
            this.TransferFromItem.RegisterDataAccessListenerStatusOut(this);
            _ = await this.TransferFromItem.DataItemStatus();
            this.TransferFromItem.UnregisterDataAccessListenerStatusOut(this);
            this.TransferFromItem.UnregisterDataAccessListenerProgressOut(this);

            this.TransferFromDetail.RegisterDataAccessListenerProgressOut(this);
            this.TransferFromDetail.RegisterDataAccessListenerStatusOut(this);
            _ = await this.TransferFromDetail.DataDetailStatus();
            this.TransferFromDetail.UnregisterDataAccessListenerStatusOut(this);
            this.TransferFromDetail.UnregisterDataAccessListenerProgressOut(this);

            this.TransferTo.RegisterDataAccessListenerProgressOut(this);
            this.TransferTo.RegisterDataAccessListenerStatusOut(this);
            _ = await this.TransferTo.DataHeaderStatus();
            this.TransferTo.UnregisterDataAccessListenerStatusOut(this);
            this.TransferTo.UnregisterDataAccessListenerProgressOut(this);

            this.TransferToItem.RegisterDataAccessListenerProgressOut(this);
            this.TransferToItem.RegisterDataAccessListenerStatusOut(this);
            _ = await this.TransferToItem.DataItemStatus();
            this.TransferToItem.UnregisterDataAccessListenerStatusOut(this);
            this.TransferToItem.UnregisterDataAccessListenerProgressOut(this);

            this.TransferToDetail.RegisterDataAccessListenerProgressOut(this);
            this.TransferToDetail.RegisterDataAccessListenerStatusOut(this);
            _ = await this.TransferToDetail.DataDetailStatus();
            this.TransferToDetail.UnregisterDataAccessListenerStatusOut(this);
            this.TransferToDetail.UnregisterDataAccessListenerProgressOut(this);
        }

        /// <summary>
        /// </summary>
        private async Task DataStatusCounts()
        {
            this.CycleCount.RegisterDataAccessListenerProgressOut(this);
            this.CycleCount.RegisterDataAccessListenerStatusOut(this);
            _ = await this.CycleCount.DataHeaderStatus();
            this.CycleCount.UnregisterDataAccessListenerStatusOut(this);
            this.CycleCount.UnregisterDataAccessListenerProgressOut(this);

            this.CycleCountItem.RegisterDataAccessListenerProgressOut(this);
            this.CycleCountItem.RegisterDataAccessListenerStatusOut(this);
            _ = await this.CycleCountItem.DataItemStatus();
            this.CycleCountItem.UnregisterDataAccessListenerStatusOut(this);
            this.CycleCountItem.UnregisterDataAccessListenerProgressOut(this);

            this.CycleCountDetail.RegisterDataAccessListenerProgressOut(this);
            this.CycleCountDetail.RegisterDataAccessListenerStatusOut(this);
            _ = await this.CycleCountDetail.DataDetailStatus();
            this.CycleCountDetail.UnregisterDataAccessListenerStatusOut(this);
            this.CycleCountDetail.UnregisterDataAccessListenerProgressOut(this);

            this.PhysicalCount.RegisterDataAccessListenerProgressOut(this);
            this.PhysicalCount.RegisterDataAccessListenerStatusOut(this);
            _ = await this.PhysicalCount.DataHeaderStatus();
            this.PhysicalCount.UnregisterDataAccessListenerStatusOut(this);
            this.PhysicalCount.UnregisterDataAccessListenerProgressOut(this);

            this.PhysicalCountItem.RegisterDataAccessListenerProgressOut(this);
            this.PhysicalCountItem.RegisterDataAccessListenerStatusOut(this);
            _ = await this.PhysicalCountItem.DataItemStatus();
            this.PhysicalCountItem.UnregisterDataAccessListenerStatusOut(this);
            this.PhysicalCountItem.UnregisterDataAccessListenerProgressOut(this);

            this.PhysicalCountDetail.RegisterDataAccessListenerProgressOut(this);
            this.PhysicalCountDetail.RegisterDataAccessListenerStatusOut(this);
            _ = await this.PhysicalCountDetail.DataDetailStatus();
            this.PhysicalCountDetail.UnregisterDataAccessListenerStatusOut(this);
            this.PhysicalCountDetail.UnregisterDataAccessListenerProgressOut(this);
        }

        /// <summary>
        /// </summary>
        private async Task DataStatusTickets()
        {
            this.TicketFinished.RegisterDataAccessListenerProgressOut(this);
            this.TicketFinished.RegisterDataAccessListenerStatusOut(this);
            _ = await this.TicketFinished.DataHeaderStatus();
            this.TicketFinished.UnregisterDataAccessListenerStatusOut(this);
            this.TicketFinished.UnregisterDataAccessListenerProgressOut(this);

            this.TicketFinishedItem.RegisterDataAccessListenerProgressOut(this);
            this.TicketFinishedItem.RegisterDataAccessListenerStatusOut(this);
            _ = await this.TicketFinishedItem.DataItemStatus();
            this.TicketFinishedItem.UnregisterDataAccessListenerStatusOut(this);
            this.TicketFinishedItem.UnregisterDataAccessListenerProgressOut(this);

            this.TicketFinishedDetail.RegisterDataAccessListenerProgressOut(this);
            this.TicketFinishedDetail.RegisterDataAccessListenerStatusOut(this);
            _ = await this.TicketFinishedDetail.DataDetailStatus();
            this.TicketFinishedDetail.UnregisterDataAccessListenerStatusOut(this);
            this.TicketFinishedDetail.UnregisterDataAccessListenerProgressOut(this);

            this.TicketMaterial.RegisterDataAccessListenerProgressOut(this);
            this.TicketMaterial.RegisterDataAccessListenerStatusOut(this);
            _ = await this.TicketMaterial.DataHeaderStatus();
            this.TicketMaterial.UnregisterDataAccessListenerStatusOut(this);
            this.TicketMaterial.UnregisterDataAccessListenerProgressOut(this);

            this.TicketMaterialItem.RegisterDataAccessListenerProgressOut(this);
            this.TicketMaterialItem.RegisterDataAccessListenerStatusOut(this);
            _ = await this.TicketMaterialItem.DataItemStatus();
            this.TicketMaterialItem.UnregisterDataAccessListenerStatusOut(this);
            this.TicketMaterialItem.UnregisterDataAccessListenerProgressOut(this);

            this.TicketMaterialDetail.RegisterDataAccessListenerProgressOut(this);
            this.TicketMaterialDetail.RegisterDataAccessListenerStatusOut(this);
            _ = await this.TicketMaterialDetail.DataDetailStatus();
            this.TicketMaterialDetail.UnregisterDataAccessListenerStatusOut(this);
            this.TicketMaterialDetail.UnregisterDataAccessListenerProgressOut(this);

            this.TicketByProduct.RegisterDataAccessListenerProgressOut(this);
            this.TicketByProduct.RegisterDataAccessListenerStatusOut(this);
            _ = await this.TicketByProduct.DataHeaderStatus();
            this.TicketByProduct.UnregisterDataAccessListenerStatusOut(this);
            this.TicketByProduct.UnregisterDataAccessListenerProgressOut(this);

            this.TicketByProductItem.RegisterDataAccessListenerProgressOut(this);
            this.TicketByProductItem.RegisterDataAccessListenerStatusOut(this);
            _ = await this.TicketByProductItem.DataItemStatus();
            this.TicketByProductItem.UnregisterDataAccessListenerStatusOut(this);
            this.TicketByProductItem.UnregisterDataAccessListenerProgressOut(this);

            this.TicketByProductDetail.RegisterDataAccessListenerProgressOut(this);
            this.TicketByProductDetail.RegisterDataAccessListenerStatusOut(this);
            _ = await this.TicketByProductDetail.DataDetailStatus();
            this.TicketByProductDetail.UnregisterDataAccessListenerStatusOut(this);
            this.TicketByProductDetail.UnregisterDataAccessListenerProgressOut(this);
        }

        /// <summary>
        /// </summary>
        private async Task RefreshDataBarCodeAssociation()
        {
            // Bar Code Association
            if (this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                this.ActItemBarCodeAssocXRef.RegisterDataAccessListenerStatusOut(this);
                await this.ActItemBarCodeAssocXRef.PurgeData();
                this.ActItemBarCodeAssocXRef.UnregisterDataAccessListenerStatusOut(this);

                this.ActivityItem.RegisterDataAccessListenerStatusOut(this);
                await this.ActivityItem.PurgeData(SysCommon.ActSubTypes.BarCodeAssociation);
                this.ActivityItem.UnregisterDataAccessListenerStatusOut(this);

                this.Activity.RegisterDataAccessListenerStatusOut(this);
                await this.Activity.PurgeData(SysCommon.ActSubTypes.BarCodeAssociation);
                this.Activity.UnregisterDataAccessListenerStatusOut(this);
            }
        }

        /// <summary>
        /// </summary>
        private async Task RefreshDataAdjustments()
        {
            if (this.SysEnv.RefreshOpt.Adjustments || this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                this.AdjustmentDetail.RegisterDataAccessListenerStatusOut(this);
                await this.AdjustmentDetail.PurgeData();
                this.AdjustmentDetail.UnregisterDataAccessListenerStatusOut(this);

                this.AdjustmentItem.RegisterDataAccessListenerStatusOut(this);
                await this.AdjustmentItem.PurgeData();
                this.AdjustmentItem.UnregisterDataAccessListenerStatusOut(this);

                this.Adjustment.RegisterDataAccessListenerStatusOut(this);
                await this.Adjustment.PurgeData();
                this.Adjustment.UnregisterDataAccessListenerStatusOut(this);

                if (this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                {
                    this.Adjustment.RegisterDataAccessListenerProgressOut(this);
                    this.Adjustment.RegisterDataAccessListenerStatusOut(this);
                    this.AdjustmentItem.RegisterDataAccessListenerProgressOut(this);
                    this.AdjustmentItem.RegisterDataAccessListenerStatusOut(this);
                    this.AdjustmentDetail.RegisterDataAccessListenerProgressOut(this);
                    this.AdjustmentDetail.RegisterDataAccessListenerStatusOut(this);

                    var activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.Adjustment);
                    var activityItem = ActItemFacade.GetActItem(activity, SysCommon.ActSubTypes.Adjustment);
                    activityItem.ActHdr = activity;
                    var activityDetail = ActDetailFacade.GetActDetail(activityItem, SysCommon.ActSubTypes.Adjustment);
                    activityDetail.ActItem = activityItem;

                    await this.Adjustment.RequestHeaderData();
                    var headers = await activity.GetActivityList(isSubmitBatch: false);
                    foreach (var header in headers)
                    {
                        if (header is Count countHeader)
                        {
                            await this.AdjustmentItem.RequestItemData(
                                countHeader.ActivityFKRefno,
                                countHeader.ActivityRefno);
                            if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                            {
                                activityItem.ActHdr.HdrRefno = countHeader.ActivityRefno;
                                var items = await activityItem.GetActivityItemList(isSubmitBatch: false);
                                foreach (var item in items)
                                {
                                    if (item is CountItem countItem)
                                    {
                                        activityDetail.ActItem.ItemRefno = countItem.ActivityItemRefno;
                                        await this.AdjustmentDetail.RequestDetailData(
                                            countItem.ActivityItemFKRefno,
                                            countItem.ActivityItemRefno);
                                    }
                                }
                            }
                        }
                    }

                    this.AdjustmentDetail.UnregisterDataAccessListenerProgressOut(this);
                    this.AdjustmentDetail.UnregisterDataAccessListenerStatusOut(this);
                    this.AdjustmentItem.UnregisterDataAccessListenerProgressOut(this);
                    this.AdjustmentItem.UnregisterDataAccessListenerStatusOut(this);
                    this.Adjustment.UnregisterDataAccessListenerProgressOut(this);
                    this.Adjustment.UnregisterDataAccessListenerStatusOut(this);
                    this.StatusOut(string.Empty);
                }
            }
        }

        /// <summary>
        /// </summary>
        private async Task RefreshDataCounts()
        {
            if (this.SysEnv.RefreshOpt.Counts || this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                this.PhysicalCountDetail.RegisterDataAccessListenerStatusOut(this);
                await this.PhysicalCountDetail.PurgeData();
                this.PhysicalCountDetail.UnregisterDataAccessListenerStatusOut(this);

                this.PhysicalCountItem.RegisterDataAccessListenerStatusOut(this);
                await this.PhysicalCountItem.PurgeData();
                this.PhysicalCountItem.UnregisterDataAccessListenerStatusOut(this);

                this.PhysicalCount.RegisterDataAccessListenerStatusOut(this);
                await this.PhysicalCount.PurgeData();
                this.PhysicalCount.UnregisterDataAccessListenerStatusOut(this);

                if (this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                {
                    this.PhysicalCount.RegisterDataAccessListenerProgressOut(this);
                    this.PhysicalCount.RegisterDataAccessListenerStatusOut(this);
                    this.PhysicalCountItem.RegisterDataAccessListenerProgressOut(this);
                    this.PhysicalCountItem.RegisterDataAccessListenerStatusOut(this);
                    this.PhysicalCountDetail.RegisterDataAccessListenerProgressOut(this);
                    this.PhysicalCountDetail.RegisterDataAccessListenerStatusOut(this);

                    var activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.PhysicalCounts);
                    var activityItem = ActItemFacade.GetActItem(activity, SysCommon.ActSubTypes.PhysicalCounts);
                    activityItem.ActHdr = activity;
                    var activityDetail = ActDetailFacade.GetActDetail(activityItem, SysCommon.ActSubTypes.PhysicalCounts);
                    activityDetail.ActItem = activityItem;

                    await this.PhysicalCount.RequestHeaderData();
                    var headers = await activity.GetActivityList(isSubmitBatch: false);
                    foreach (var header in headers)
                    {
                        if (header is Count countHeader)
                        {
                            await this.PhysicalCountItem.RequestItemData(
                                countHeader.ActivityFKRefno,
                                countHeader.ActivityRefno);
                            if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                            {
                                activityItem.ActHdr.HdrRefno = countHeader.ActivityRefno;
                                var items = await activityItem.GetActivityItemList(isSubmitBatch: false);
                                foreach (var item in items)
                                {
                                    if (item is CountItem countItem)
                                    {
                                        activityDetail.ActItem.ItemRefno = countItem.ActivityItemRefno;
                                        await this.PhysicalCountDetail.RequestDetailData(
                                            countItem.ActivityItemFKRefno,
                                            countItem.ActivityItemRefno);
                                    }
                                }
                            }
                        }
                    }

                    this.PhysicalCountDetail.UnregisterDataAccessListenerProgressOut(this);
                    this.PhysicalCountDetail.UnregisterDataAccessListenerStatusOut(this);
                    this.PhysicalCountItem.UnregisterDataAccessListenerProgressOut(this);
                    this.PhysicalCountItem.UnregisterDataAccessListenerStatusOut(this);
                    this.PhysicalCount.UnregisterDataAccessListenerProgressOut(this);
                    this.PhysicalCount.UnregisterDataAccessListenerStatusOut(this);
                    this.StatusOut(string.Empty);
                }

                this.CycleCountDetail.RegisterDataAccessListenerStatusOut(this);
                await this.CycleCountDetail.PurgeData();
                this.CycleCountDetail.UnregisterDataAccessListenerStatusOut(this);

                this.CycleCountItem.RegisterDataAccessListenerStatusOut(this);
                await this.CycleCountItem.PurgeData();
                this.CycleCountItem.UnregisterDataAccessListenerStatusOut(this);

                this.CycleCount.RegisterDataAccessListenerStatusOut(this);
                await this.CycleCount.PurgeData();
                this.CycleCount.UnregisterDataAccessListenerStatusOut(this);

                if (this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                {
                    this.CycleCount.RegisterDataAccessListenerProgressOut(this);
                    this.CycleCount.RegisterDataAccessListenerStatusOut(this);
                    this.CycleCountItem.RegisterDataAccessListenerProgressOut(this);
                    this.CycleCountItem.RegisterDataAccessListenerStatusOut(this);
                    this.CycleCountDetail.RegisterDataAccessListenerProgressOut(this);
                    this.CycleCountDetail.RegisterDataAccessListenerStatusOut(this);

                    var activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.CycleCounts);
                    var activityItem = ActItemFacade.GetActItem(activity, SysCommon.ActSubTypes.CycleCounts);
                    activityItem.ActHdr = activity;
                    var activityDetail = ActDetailFacade.GetActDetail(activityItem, SysCommon.ActSubTypes.CycleCounts);
                    activityDetail.ActItem = activityItem;

                    await this.CycleCount.RequestHeaderData();
                    var headers = await activity.GetActivityList(isSubmitBatch: false);
                    foreach (var header in headers)
                    {
                        if (header is Count countHeader)
                        {
                            await this.CycleCountItem.RequestItemData(
                                countHeader.ActivityFKRefno,
                                countHeader.ActivityRefno);
                            if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                            {
                                activityItem.ActHdr.HdrRefno = countHeader.ActivityRefno;
                                var items = await activityItem.GetActivityItemList(isSubmitBatch: false);
                                foreach (var item in items)
                                {
                                    if (item is CountItem countItem)
                                    {
                                        activityDetail.ActItem.ItemRefno = countItem.ActivityItemRefno;
                                        await this.CycleCountDetail.RequestDetailData(
                                            countItem.ActivityItemFKRefno,
                                            countItem.ActivityItemRefno);
                                    }
                                }
                            }
                        }
                    }

                    this.CycleCountDetail.UnregisterDataAccessListenerProgressOut(this);
                    this.CycleCountDetail.UnregisterDataAccessListenerStatusOut(this);
                    this.CycleCountItem.UnregisterDataAccessListenerProgressOut(this);
                    this.CycleCountItem.UnregisterDataAccessListenerStatusOut(this);
                    this.CycleCount.UnregisterDataAccessListenerProgressOut(this);
                    this.CycleCount.UnregisterDataAccessListenerStatusOut(this);
                    this.StatusOut(string.Empty);
                }
            }
        }

        /// <summary>
        /// </summary>
        private async Task RefreshDataProduction()
        {
            if (this.SysEnv.RefreshOpt.ProductionTickets || this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                this.TicketFinished.RegisterDataAccessListenerProgressOut(this);
                this.TicketFinished.RegisterDataAccessListenerStatusOut(this);
                this.TicketFinishedItem.RegisterDataAccessListenerProgressOut(this);
                this.TicketFinishedItem.RegisterDataAccessListenerStatusOut(this);
                this.TicketFinishedDetail.RegisterDataAccessListenerProgressOut(this);
                this.TicketFinishedDetail.RegisterDataAccessListenerStatusOut(this);

                var activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.TicketFinished);
                var activityItem = ActItemFacade.GetActItem(activity, SysCommon.ActSubTypes.TicketFinished);
                activityItem.ActHdr = activity;
                var activityDetail = ActDetailFacade.GetActDetail(activityItem, SysCommon.ActSubTypes.TicketFinished);
                activityDetail.ActItem = activityItem;

                await this.TicketFinished.RequestHeaderData();
                var headers = await activity.GetActivityList(isSubmitBatch: false);
                foreach (var header in headers)
                {
                    if (header is Count countHeader)
                    {
                        await this.TicketFinishedItem.RequestItemData(
                            countHeader.ActivityFKRefno,
                            countHeader.ActivityRefno);
                        if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                        {
                            activityItem.ActHdr.HdrRefno = countHeader.ActivityRefno;
                            var items = await activityItem.GetActivityItemList(isSubmitBatch: false);
                            foreach (var item in items)
                            {
                                if (item is CountItem countItem)
                                {
                                    activityDetail.ActItem.ItemRefno = countItem.ActivityItemRefno;
                                    await this.TicketFinishedDetail.RequestDetailData(
                                        countItem.ActivityItemFKRefno,
                                        countItem.ActivityItemRefno);
                                }
                            }
                        }
                    }
                }

                this.TicketFinishedDetail.UnregisterDataAccessListenerProgressOut(this);
                this.TicketFinishedDetail.UnregisterDataAccessListenerStatusOut(this);
                this.TicketFinishedItem.UnregisterDataAccessListenerProgressOut(this);
                this.TicketFinishedItem.UnregisterDataAccessListenerStatusOut(this);
                this.TicketFinished.UnregisterDataAccessListenerProgressOut(this);
                this.TicketFinished.UnregisterDataAccessListenerStatusOut(this);
                this.StatusOut(string.Empty);

                this.TicketMaterial.RegisterDataAccessListenerProgressOut(this);
                this.TicketMaterial.RegisterDataAccessListenerStatusOut(this);
                this.TicketMaterialItem.RegisterDataAccessListenerProgressOut(this);
                this.TicketMaterialItem.RegisterDataAccessListenerStatusOut(this);
                this.TicketMaterialDetail.RegisterDataAccessListenerProgressOut(this);
                this.TicketMaterialDetail.RegisterDataAccessListenerStatusOut(this);

                activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.TicketMaterial);
                activityItem = ActItemFacade.GetActItem(activity, SysCommon.ActSubTypes.TicketMaterial);
                activityItem.ActHdr = activity;
                activityDetail = ActDetailFacade.GetActDetail(activityItem, SysCommon.ActSubTypes.TicketMaterial);
                activityDetail.ActItem = activityItem;

                await this.TicketMaterial.RequestHeaderData();
                headers = await activity.GetActivityList(isSubmitBatch: false);
                foreach (var header in headers)
                {
                    if (header is Count countHeader)
                    {
                        await this.TicketMaterialItem.RequestItemData(
                            countHeader.ActivityFKRefno,
                            countHeader.ActivityRefno);
                        if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                        {
                            activityItem.ActHdr.HdrRefno = countHeader.ActivityRefno;
                            var items = await activityItem.GetActivityItemList(isSubmitBatch: false);
                            foreach (var item in items)
                            {
                                if (item is CountItem countItem)
                                {
                                    activityDetail.ActItem.ItemRefno = countItem.ActivityItemRefno;
                                    await this.TicketMaterialDetail.RequestDetailData(
                                        countItem.ActivityItemFKRefno,
                                        countItem.ActivityItemRefno);
                                }
                            }
                        }
                    }
                }

                this.TicketMaterialDetail.UnregisterDataAccessListenerProgressOut(this);
                this.TicketMaterialDetail.UnregisterDataAccessListenerStatusOut(this);
                this.TicketMaterialItem.UnregisterDataAccessListenerProgressOut(this);
                this.TicketMaterialItem.UnregisterDataAccessListenerStatusOut(this);
                this.TicketMaterial.UnregisterDataAccessListenerProgressOut(this);
                this.TicketMaterial.UnregisterDataAccessListenerStatusOut(this);
                this.StatusOut(string.Empty);

                this.TicketByProduct.RegisterDataAccessListenerProgressOut(this);
                this.TicketByProduct.RegisterDataAccessListenerStatusOut(this);
                this.TicketByProductItem.RegisterDataAccessListenerProgressOut(this);
                this.TicketByProductItem.RegisterDataAccessListenerStatusOut(this);
                this.TicketByProductDetail.RegisterDataAccessListenerProgressOut(this);
                this.TicketByProductDetail.RegisterDataAccessListenerStatusOut(this);

                activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.TicketAddProduct);
                activityItem = ActItemFacade.GetActItem(activity, SysCommon.ActSubTypes.TicketAddProduct);
                activityItem.ActHdr = activity;
                activityDetail = ActDetailFacade.GetActDetail(activityItem, SysCommon.ActSubTypes.TicketAddProduct);
                activityDetail.ActItem = activityItem;

                await this.TicketByProduct.RequestHeaderData();
                headers = await activity.GetActivityList(isSubmitBatch: false);
                foreach (var header in headers)
                {
                    if (header is Count countHeader)
                    {
                        await this.TicketByProductItem.RequestItemData(
                            countHeader.ActivityFKRefno,
                            countHeader.ActivityRefno);
                        if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                        {
                            activityItem.ActHdr.HdrRefno = countHeader.ActivityRefno;
                            var items = await activityItem.GetActivityItemList(isSubmitBatch: false);
                            foreach (var item in items)
                            {
                                if (item is CountItem countItem)
                                {
                                    activityDetail.ActItem.ItemRefno = countItem.ActivityItemRefno;
                                    await this.TicketByProductDetail.RequestDetailData(
                                        countItem.ActivityItemFKRefno,
                                        countItem.ActivityItemRefno);
                                }
                            }
                        }
                    }
                }

                this.TicketByProductDetail.UnregisterDataAccessListenerProgressOut(this);
                this.TicketByProductDetail.UnregisterDataAccessListenerStatusOut(this);
                this.TicketByProductItem.UnregisterDataAccessListenerProgressOut(this);
                this.TicketByProductItem.UnregisterDataAccessListenerStatusOut(this);
                this.TicketByProduct.UnregisterDataAccessListenerProgressOut(this);
                this.TicketByProduct.UnregisterDataAccessListenerStatusOut(this);
                this.StatusOut(string.Empty);
            }
        }

        /// <summary>
        /// </summary>
        private async Task RefreshDataReceiving()
        {
            if (this.SysEnv.RefreshOpt.PurchaseOrders || this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                this.Receiving.RegisterDataAccessListenerProgressOut(this);
                this.Receiving.RegisterDataAccessListenerStatusOut(this);
                this.ReceivingItem.RegisterDataAccessListenerProgressOut(this);
                this.ReceivingItem.RegisterDataAccessListenerStatusOut(this);
                this.ReceivingDetail.RegisterDataAccessListenerProgressOut(this);
                this.ReceivingDetail.RegisterDataAccessListenerStatusOut(this);

                var activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.Receiving);
                var activityItem = ActItemFacade.GetActItem(activity, SysCommon.ActSubTypes.Receiving);
                activityItem.ActHdr = activity;
                var activityDetail = ActDetailFacade.GetActDetail(activityItem, SysCommon.ActSubTypes.Receiving);
                activityDetail.ActItem = activityItem;

                await this.Receiving.RequestHeaderData();
                var headers = await activity.GetActivityList(isSubmitBatch: false);
                foreach (var header in headers)
                {
                    if (header is Count countHeader)
                    {
                        await this.ReceivingItem.RequestItemData(
                            countHeader.ActivityFKRefno,
                            countHeader.ActivityRefno);
                        if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                        {
                            activityItem.ActHdr.HdrRefno = countHeader.ActivityRefno;
                            var items = await activityItem.GetActivityItemList(isSubmitBatch: false);
                            foreach (var item in items)
                            {
                                if (item is CountItem countItem)
                                {
                                    activityDetail.ActItem.ItemRefno = countItem.ActivityItemRefno;
                                    await this.ReceivingDetail.RequestDetailData(
                                        countItem.ActivityItemFKRefno,
                                        countItem.ActivityItemRefno);
                                }
                            }
                        }
                    }
                }

                this.ReceivingDetail.UnregisterDataAccessListenerProgressOut(this);
                this.ReceivingDetail.UnregisterDataAccessListenerStatusOut(this);
                this.ReceivingItem.UnregisterDataAccessListenerProgressOut(this);
                this.ReceivingItem.UnregisterDataAccessListenerStatusOut(this);
                this.Receiving.UnregisterDataAccessListenerProgressOut(this);
                this.Receiving.UnregisterDataAccessListenerStatusOut(this);
                this.StatusOut(string.Empty);
            }
        }

        /// <summary>
        /// </summary>
        private async Task RefreshDataPurchaseOrderWorkSheet()
        {
            if (this.SysEnv.RefreshOpt.PurchaseOrderWorkSheet ||
                this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                this.PurchaseOrderWorkSheetItem.RegisterDataAccessListenerStatusOut(this);
                await this.PurchaseOrderWorkSheetItem.PurgeData();
                this.PurchaseOrderWorkSheetItem.UnregisterDataAccessListenerStatusOut(this);

                if (this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                {
                    ////this.PurchaseOrderWorkSheetItem.RegisterDataAccessListenerProgressOut(this);
                    ////this.PurchaseOrderWorkSheetItem.RegisterDataAccessListenerStatusOut(this);
                    ////await this.PurchaseOrderWorkSheetItem.RequestItemData(0);
                    ////this.PurchaseOrderWorkSheetItem.UnregisterDataAccessListenerStatusOut(this);
                    ////this.PurchaseOrderWorkSheetItem.UnregisterDataAccessListenerProgressOut(this);
                    ////this.StatusOut(string.Empty);
                }
            }
        }

        /// <summary>
        /// </summary>
        private async Task RefreshDataPicking()
        {
            if (this.SysEnv.RefreshOpt.SalesOrders ||
                this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                this.PickingDetail.RegisterDataAccessListenerStatusOut(this);
                await this.PickingDetail.PurgeData();
                this.PickingDetail.UnregisterDataAccessListenerStatusOut(this);

                this.PickingItemComment.RegisterDataAccessListenerStatusOut(this);
                await this.PickingItemComment.PurgeData();
                this.PickingItemComment.UnregisterDataAccessListenerStatusOut(this);

                this.PickingItem.RegisterDataAccessListenerStatusOut(this);
                await this.PickingItem.PurgeData();
                this.PickingItem.UnregisterDataAccessListenerStatusOut(this);

                this.PickingRoute.RegisterDataAccessListenerStatusOut(this);
                await this.PickingRoute.PurgeData();
                this.PickingRoute.UnregisterDataAccessListenerStatusOut(this);

                this.Picking.RegisterDataAccessListenerStatusOut(this);
                await this.Picking.PurgeData();
                this.Picking.UnregisterDataAccessListenerStatusOut(this);

                if (this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                {
                    this.Picking.RegisterDataAccessListenerProgressOut(this);
                    this.Picking.RegisterDataAccessListenerStatusOut(this);
                    this.PickingItem.RegisterDataAccessListenerProgressOut(this);
                    this.PickingItem.RegisterDataAccessListenerStatusOut(this);
                    this.PickingDetail.RegisterDataAccessListenerProgressOut(this);
                    this.PickingDetail.RegisterDataAccessListenerStatusOut(this);

                    var activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.Picking);
                    await this.Picking.RequestHeaderData();
                    var headers = await activity.GetActivityList(isSubmitBatch: false);
                    foreach (var header in headers)
                    {
                        await Task.Run(async () =>
                        {
                            if (header is Picking pickingHeader)
                            {
                                await this.PickingItem.RequestItemData(
                                    pickingHeader.ActivityFKRefno,
                                    pickingHeader.ActivityRefno);

                                if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                                {
                                    var activityItem = ActItemFacade.GetActItem(
                                        activity,
                                        SysCommon.ActSubTypes.Picking);
                                    activityItem.ActHdr.HdrRefno = pickingHeader.ActivityRefno;
                                    var items = await activityItem.GetActivityItemList(isSubmitBatch: false);
                                    foreach (var item in items)
                                    {
                                        if (item is PickingItem pickingItem)
                                        {
                                            var activityDetail = ActDetailFacade.GetActDetail(
                                                activityItem,
                                                SysCommon.ActSubTypes.Picking);
                                            activityDetail.ActItem.ItemRefno = pickingItem.ActivityItemRefno;
                                            await this.PickingDetail.RequestDetailData(
                                                pickingItem.ActivityItemFKRefno,
                                                pickingItem.ActivityItemRefno);
                                        }
                                    }
                                }
                            }
                        });

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }

                    this.PickingDetail.UnregisterDataAccessListenerProgressOut(this);
                    this.PickingDetail.UnregisterDataAccessListenerStatusOut(this);
                    this.PickingItem.UnregisterDataAccessListenerProgressOut(this);
                    this.PickingItem.UnregisterDataAccessListenerStatusOut(this);
                    this.Picking.UnregisterDataAccessListenerProgressOut(this);
                    this.Picking.UnregisterDataAccessListenerStatusOut(this);
                    this.StatusOut(string.Empty);
                }
            }
        }

        /// <summary>
        /// </summary>
        private async Task RefreshDataTransfers()
        {
            if (this.SysEnv.RefreshOpt.Transfers || this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
            {
                this.TransferFromDetail.RegisterDataAccessListenerStatusOut(this);
                await this.TransferFromDetail.PurgeData();
                this.TransferFromDetail.UnregisterDataAccessListenerStatusOut(this);

                this.TransferFromItem.RegisterDataAccessListenerStatusOut(this);
                await this.TransferFromItem.PurgeData();
                this.TransferFromItem.UnregisterDataAccessListenerStatusOut(this);

                this.TransferFrom.RegisterDataAccessListenerStatusOut(this);
                await this.TransferFrom.PurgeData();
                this.TransferFrom.UnregisterDataAccessListenerStatusOut(this);

                this.TransferToDetail.RegisterDataAccessListenerStatusOut(this);
                await this.TransferToDetail.PurgeData();
                this.TransferToDetail.UnregisterDataAccessListenerStatusOut(this);

                this.TransferToItem.RegisterDataAccessListenerStatusOut(this);
                await this.TransferToItem.PurgeData();
                this.TransferToItem.UnregisterDataAccessListenerStatusOut(this);

                this.TransferTo.RegisterDataAccessListenerStatusOut(this);
                await this.TransferTo.PurgeData();
                this.TransferTo.UnregisterDataAccessListenerStatusOut(this);

                if (this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                {
                    this.TransferFrom.RegisterDataAccessListenerProgressOut(this);
                    this.TransferFrom.RegisterDataAccessListenerStatusOut(this);
                    this.TransferFromItem.RegisterDataAccessListenerProgressOut(this);
                    this.TransferFromItem.RegisterDataAccessListenerStatusOut(this);
                    this.TransferFromDetail.RegisterDataAccessListenerProgressOut(this);
                    this.TransferFromDetail.RegisterDataAccessListenerStatusOut(this);

                    var activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.TransferFrom);
                    var activityItem = ActItemFacade.GetActItem(activity, SysCommon.ActSubTypes.TransferFrom);
                    activityItem.ActHdr = activity;
                    var activityDetail = ActDetailFacade.GetActDetail(activityItem, SysCommon.ActSubTypes.TransferFrom);
                    activityDetail.ActItem = activityItem;

                    await this.TransferFrom.RequestHeaderData();
                    var headers = await activity.GetActivityList(isSubmitBatch: false);
                    foreach (var header in headers)
                    {
                        if (header is Count countHeader)
                        {
                            await this.TransferFromItem.RequestItemData(
                                countHeader.ActivityFKRefno,
                                countHeader.ActivityRefno);
                            if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                            {
                                activityItem.ActHdr.HdrRefno = countHeader.ActivityRefno;
                                var items = await activityItem.GetActivityItemList(isSubmitBatch: false);
                                foreach (var item in items)
                                {
                                    if (item is CountItem countItem)
                                    {
                                        activityDetail.ActItem.ItemRefno = countItem.ActivityItemRefno;
                                        await this.TransferFromDetail.RequestDetailData(
                                            countItem.ActivityItemFKRefno,
                                            countItem.ActivityItemRefno);
                                    }
                                }
                            }
                        }
                    }

                    this.TransferToDetail.UnregisterDataAccessListenerProgressOut(this);
                    this.TransferToDetail.UnregisterDataAccessListenerStatusOut(this);
                    this.TransferToItem.UnregisterDataAccessListenerProgressOut(this);
                    this.TransferToItem.UnregisterDataAccessListenerStatusOut(this);
                    this.TransferTo.UnregisterDataAccessListenerProgressOut(this);
                    this.TransferTo.UnregisterDataAccessListenerStatusOut(this);
                    this.StatusOut(string.Empty);

                    this.TransferTo.RegisterDataAccessListenerProgressOut(this);
                    this.TransferTo.RegisterDataAccessListenerStatusOut(this);
                    this.TransferToItem.RegisterDataAccessListenerProgressOut(this);
                    this.TransferToItem.RegisterDataAccessListenerStatusOut(this);
                    this.TransferToDetail.RegisterDataAccessListenerProgressOut(this);
                    this.TransferToDetail.RegisterDataAccessListenerStatusOut(this);

                    activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.TransferTo);
                    activityItem = ActItemFacade.GetActItem(activity, SysCommon.ActSubTypes.TransferTo);
                    activityItem.ActHdr = activity;
                    activityDetail = ActDetailFacade.GetActDetail(activityItem, SysCommon.ActSubTypes.TransferTo);
                    activityDetail.ActItem = activityItem;

                    await this.TransferTo.RequestHeaderData();
                    headers = await activity.GetActivityList(isSubmitBatch: false);
                    foreach (var header in headers)
                    {
                        if (header is Count countHeader)
                        {
                            await this.TransferToItem.RequestItemData(
                                countHeader.ActivityFKRefno,
                                countHeader.ActivityRefno);
                            if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                            {
                                activityItem.ActHdr.HdrRefno = countHeader.ActivityRefno;
                                var items = await activityItem.GetActivityItemList(isSubmitBatch: false);
                                foreach (var item in items)
                                {
                                    if (item is CountItem countItem)
                                    {
                                        activityDetail.ActItem.ItemRefno = countItem.ActivityItemRefno;
                                        await this.TransferToDetail.RequestDetailData(
                                            countItem.ActivityItemFKRefno,
                                            countItem.ActivityItemRefno);
                                    }
                                }
                            }
                        }
                    }

                    this.TransferToDetail.UnregisterDataAccessListenerProgressOut(this);
                    this.TransferToDetail.UnregisterDataAccessListenerStatusOut(this);
                    this.TransferToItem.UnregisterDataAccessListenerProgressOut(this);
                    this.TransferToItem.UnregisterDataAccessListenerStatusOut(this);
                    this.TransferTo.UnregisterDataAccessListenerProgressOut(this);
                    this.TransferTo.UnregisterDataAccessListenerStatusOut(this);
                    this.StatusOut(string.Empty);
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