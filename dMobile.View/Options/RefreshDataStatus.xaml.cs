// <copyright file="RefreshDataStatus.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Options
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using SQLite;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RefreshDataStatus : DMobileModal, IAppArea, ISysSettingsEventListener,
        IBarCodeSettingsEventListener, IPrnSettingsListener, IProdSchemeLabelSettingsListener,
        IDataNubListener, IDataAccessEventListener
    {
        private bool isFirstAppearance = true;
        private int messageIndex = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshDataStatus"/> class.
        /// </summary>
        public RefreshDataStatus()
        {
            this.InitializeComponent();
            this.IsIndicatorRunning = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshDataStatus"/> class.
        /// </summary>
        /// <param name="actSubTypes"></param>
        public RefreshDataStatus(SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.IsIndicatorRunning = false;
            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMapWarehouse = new TableLocationMapWarehouse();
            this.LocMapSection = new TableLocationMapSection();
            this.LocMapType = new TableLocationMapType();
            this.LocMapSlotCoordinate = new TableLocationMapSlotCoordinate();
            this.LocMapSlot = new TableLocationMapSlot();
            this.WhsLocMapType = new TableWarehouseLocationMapType();
            this.Unit = new TableUnit();
            this.Route = new TableRoute();
            this.ProdScheme = new TableProductScheme();
            this.ProdSchemeVariant = new TableProductSchemeVariant();
            this.ProdSchemeVariantXRef = new TableProductSchemeVariantXRef();
            this.ProdSchemeLot = new TableProductSchemeLotNumber();
            this.ProdSchemeLotXRef = new TableProductSchemeLotNumberXRef();
            this.Product = new TableProduct();
            this.Family = new TableFamily();
            this.ProdUnit = new TableProductUnit();
            this.ProdUnitRelation = new TableProductUnitRelation();
            this.ProdUnitBarCode = new TableProductUnitBarCode();
            this.ProdLocMapType = new TableProductLocationMapType();
            this.ProdLocMapWhsProdScheme = new TableLocationMapWarehouseProductScheme();
            this.ContactUsers = new TableContactUsers();
            this.PhysicalCount = new TableActivityPhysicalCount();
            this.PhysicalCountItem = new TableActivityPhysicalCountItem();
            this.PhysicalCountDetail = new TableActivityPhysicalCountDetail();
            this.CycleCount = new TableActivityCycleCount();
            this.CycleCountItem = new TableActivityCycleCountItem();
            this.CycleCountDetail = new TableActivityCycleCountDetail();
            this.ActDetailProdSchemeVariantXRef = new TableActivityDetailProductSchemeVariantXRef();
            this.ActItemBarCodeAssocXRef = new TableActivityItemBarCodeAssociationXRef();
            this.Picking = new TableActivityPicking();
            this.PickingItem = new TableActivityPickingItem();
            this.PickingItemComment = new TableActivityPickingItemComment();
            this.PickingDetail = new TableActivityPickingDetail();
            this.PickingRoute = new TableActivityPickingRoute();
            this.Receiving = new TableActivityReceiving();
            this.ReceivingItem = new TableActivityReceivingItem();
            this.ReceivingItemComment = new TableActivityReceivingItemComment();
            this.ReceivingDetail = new TableActivityReceivingDetail();
            this.TransferFrom = new TableActivityTransferFrom();
            this.TransferFromItem = new TableActivityTransferFromItem();
            this.TransferFromDetail = new TableActivityTransferFromDetail();
            this.TransferTo = new TableActivityTransferTo();
            this.TransferToItem = new TableActivityTransferToItem();
            this.TransferToDetail = new TableActivityTransferToDetail();
            this.Returns = new TableActivityReturns();
            this.ReturnsItem = new TableActivityReturnsItem();
            this.ReturnsItemComment = new TableActivityReturnsItemComment();
            this.ReturnsDetail = new TableActivityReturnsDetail();
            this.Adjustment = new TableActivityAdjustment();
            this.AdjustmentItem = new TableActivityAdjustmentItem();
            this.AdjustmentDetail = new TableActivityAdjustmentDetail();
            this.Activity = new TableActivity();
            this.ActivityItem = new TableActivityItem();
            this.TicketFinished = new TableActivityProductionTicketFinished();
            this.TicketMaterial = new TableActivityProductionTicketMaterial();
            this.TicketByProduct = new TableActivityProductionTicketByProduct();
            this.TicketFinishedItem = new TableActivityProductionTicketFinishedItem();
            this.TicketMaterialItem = new TableActivityProductionTicketMaterialItem();
            this.TicketByProductItem = new TableActivityProductionTicketByProductItem();
            this.TicketFinishedDetail = new TableActivityProductionTicketFinishedDetail();
            this.TicketMaterialDetail = new TableActivityProductionTicketMaterialDetail();
            this.TicketByProductDetail = new TableActivityProductionTicketByProductDetail();
            this.PurchaseOrderWorkSheetItem = new TableActivityPurchaseOrderWorkSheetItem();
            this.PrinterSettings = new PrinterSettings();
            this.SchemeLabelSettings = new ProdSchemeLabelSettings();
            this.ExtraInfoItems = new ObservableCollection<Message>();
            this.ActSubType = actSubTypes;
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                this.IsIndicatorRunning = true;
                base.OnAppearing();

                this.lstStatus.ItemsSource = this.ExtraInfoItems;

                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), (keyPressed) =>
                {
                    _ = Enum.TryParse(keyPressed, out Keycode keycode);
                });

                if (this.isFirstAppearance)
                {
                    this.isFirstAppearance = false;
                    await this.RefreshDataStatus_GotFocus(this, EventArgs.Empty);
                }

                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <inheritdoc/>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
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
            });

            return true;
        }

        private SysEnvironment SysEnv { get; set; }

        private bool IsRefreshComplete { get; set; } = false;

        private SysCommon.ActSubTypes ActSubType { get; set; } = SysCommon.ActSubTypes.NotApplicable;

        private string FullClassName { get; set; } = typeof(RefreshDataStatus).FullName;

        private DateTime RefreshTotal { get; set; } = DateTime.Now;

        private TableProductLocationMapType ProdLocMapType { get; set; }

        private TableUnit Unit { get; set; }

        private TableRoute Route { get; set; }

        private TableProductScheme ProdScheme { get; set; }

        private TableProductSchemeVariant ProdSchemeVariant { get; set; }

        private TableProductSchemeVariantXRef ProdSchemeVariantXRef { get; set; }

        private TableProductSchemeLotNumber ProdSchemeLot { get; set; }

        private TableProductSchemeLotNumberXRef ProdSchemeLotXRef { get; set; }

        private TableProduct Product { get; set; }

        private TableFamily Family { get; set; }

        private TableProductUnit ProdUnit { get; set; }

        private TableProductUnitRelation ProdUnitRelation { get; set; }

        private TableProductUnitBarCode ProdUnitBarCode { get; set; }

        private TableLocationMapWarehouse LocMapWarehouse { get; set; }

        private TableLocationMapSection LocMapSection { get; set; }

        private TableLocationMapType LocMapType { get; set; }

        private TableLocationMapSlotCoordinate LocMapSlotCoordinate { get; set; }

        private TableLocationMapSlot LocMapSlot { get; set; }

        private TableWarehouseLocationMapType WhsLocMapType { get; set; }

        private TableContactUsers ContactUsers { get; set; }

        private TableActivityPhysicalCount PhysicalCount { get; set; }

        private TableActivityPhysicalCountItem PhysicalCountItem { get; set; }

        private TableActivityPhysicalCountDetail PhysicalCountDetail { get; set; }

        private TableActivityCycleCount CycleCount { get; set; }

        private TableActivityCycleCountItem CycleCountItem { get; set; }

        private TableActivityCycleCountDetail CycleCountDetail { get; set; }

        private TableActivityDetailProductSchemeVariantXRef ActDetailProdSchemeVariantXRef { get; set; }

        private TableActivityItemBarCodeAssociationXRef ActItemBarCodeAssocXRef { get; set; }

        private TableActivityPicking Picking { get; set; }

        private TableActivityPickingItem PickingItem { get; set; }

        private TableActivityPickingItemComment PickingItemComment { get; set; }

        private TableActivityPickingDetail PickingDetail { get; set; }

        private TableActivityPickingRoute PickingRoute { get; set; }

        private TableActivityReceiving Receiving { get; set; }

        private TableActivityReceivingItem ReceivingItem { get; set; }

        private TableActivityReceivingItemComment ReceivingItemComment { get; set; }

        private TableActivityReceivingDetail ReceivingDetail { get; set; }

        private TableActivityTransferFrom TransferFrom { get; set; }

        private TableActivityTransferFromItem TransferFromItem { get; set; }

        private TableActivityTransferFromDetail TransferFromDetail { get; set; }

        private TableActivityTransferTo TransferTo { get; set; }

        private TableActivityTransferToItem TransferToItem { get; set; }

        private TableActivityTransferToDetail TransferToDetail { get; set; }

        private TableActivityReturns Returns { get; set; }

        private TableActivityReturnsItem ReturnsItem { get; set; }

        private TableActivityReturnsItemComment ReturnsItemComment { get; set; }

        private TableActivityReturnsDetail ReturnsDetail { get; set; }

        private TableActivityAdjustment Adjustment { get; set; }

        private TableActivityAdjustmentItem AdjustmentItem { get; set; }

        private TableActivityAdjustmentDetail AdjustmentDetail { get; set; }

        private TableActivity Activity { get; set; }

        private TableActivityItem ActivityItem { get; set; }

        private TableActivityProductionTicketFinished TicketFinished { get; set; }

        private TableActivityProductionTicketFinishedItem TicketFinishedItem { get; set; }

        private TableActivityProductionTicketFinishedDetail TicketFinishedDetail { get; set; }

        private TableActivityProductionTicketMaterial TicketMaterial { get; set; }

        private TableActivityProductionTicketMaterialItem TicketMaterialItem { get; set; }

        private TableActivityProductionTicketMaterialDetail TicketMaterialDetail { get; set; }

        private TableActivityProductionTicketByProduct TicketByProduct { get; set; }

        private TableActivityProductionTicketByProductItem TicketByProductItem { get; set; }

        private TableActivityProductionTicketByProductDetail TicketByProductDetail { get; set; }

        private TableActivityPurchaseOrderWorkSheetItem PurchaseOrderWorkSheetItem { get; set; }

        private PrinterSettings PrinterSettings { get; set; }

        private ProdSchemeLabelSettings SchemeLabelSettings { get; set; }

        public ObservableCollection<Message> ExtraInfoItems { get; set; }

        private TableLocationMapWarehouseProductScheme ProdLocMapWhsProdScheme { get; set; }

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
        /// <inheritdoc/>
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
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
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
        /// <inheritdoc/>
        public async Task<bool> SaveChanges()
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public async Task<bool> CancelChanges()
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
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
        ///
        /// </summary>
        /// <param name="sysSettingsEventPublisher"></param>
        /// <param name="sysSettingsEventArgs"></param>
        /// <remarks></remarks>
        /// <inheritdoc/>
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
        /// <inheritdoc/>
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
        /// <inheritdoc/>
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
        /// <inheritdoc/>
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
        /// <inheritdoc/>
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
        /// <inheritdoc/>
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
        /// <inheritdoc/>
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
        /// <inheritdoc/>
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
        /// <inheritdoc/>
        public void OnNubStatOut(IDataNubPublisher databaseNubEventPublisher, DataNubEventArgs databaseNubEventArgs)
        {
            this.StatusOut(databaseNubEventArgs.StatusOutMsg);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataAccessEventPublisher"></param>
        /// <param name="dataAccessEventArgs"></param>
        /// <inheritdoc/>
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
        /// <inheritdoc/>
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
            // load refresh criteria
            this.IsRefreshComplete = false;
            this.SysEnv.RefreshOpt = this.SysEnv.RefreshOpt.LoadData();
        }

        /// <summary>
        ///
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

                        var errRpt = new FormErrorReport(this.ActSubType, ex, this.FullClassName, "tmrRefreshData_Tick");
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

                        if (this.SysEnv.RefreshOpt.SystemData && !this.SysEnv.RefreshOpt.RefreshDataChangesOnly && this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
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
                        else if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly && this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
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

                        var errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex, this.FullClassName, "tmrRefreshData_Tick");
                        _ = await Device.InvokeOnMainThreadAsync(async () => await errRpt.ShowDialog());
                    }
                    catch (Exception ex)
                    {
                        this.SysEnv.RefreshOpt.RefreshedDataLast = refreshLast;
                        this.SysEnv.RefreshOpt.IsRefreshDataFailed = true;

                        this.StatusOut(Resource.RefreshDataFailed);

                        var errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex, this.FullClassName, "tmrRefreshData_Tick");
                        _ = await Device.InvokeOnMainThreadAsync(async () => await errRpt.ShowDialog());
                    }
                    finally
                    {
                        this.SysEnv.RefreshOpt.IsRefreshedLastDataChangesOnly = this.SysEnv.RefreshOpt.RefreshDataChangesOnly;
                        var tempRefreshOpt3 = this.SysEnv.RefreshOpt;
                        _ = this.SysEnv.RefreshOpt.SaveData(ref tempRefreshOpt3);
                    }
                }

                // }
            });

            this.IsIndicatorRunning = false;
        }

        /// <summary>
        ///
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
            var settings = this.SysEnv.SysSettings;
            SystemSettingsTasks.RequestSystemSettings(ref settings);
            this.SysEnv.SysSettings = settings;

            var tempSystemSettings = this.SysEnv.SysSettings;
            _ = SystemSettingsTasks.SaveData(ref tempSystemSettings);

            this.SysEnv.SysSettings.UnregisterSysSettingsListenerStatusOut((ISysSettingsEventListener)this);
            this.SysEnv.SysSettings.UnregisterSysSettingsListenerProgressOut((ISysSettingsEventListener)this);
            this.StatusOut(string.Empty);

            this.SysEnv.BarCodeSettings.RegisterBarCodeSettingsListenerStatusOut((IBarCodeSettingsEventListener)this);
            this.SysEnv.BarCodeSettings.RegisterBarCodeSettingsListenerProgressOut((IBarCodeSettingsEventListener)this);
            this.SysEnv.BarCodeSettings.RequestBarCodeSettings();
            var tempBarCodeSettings = this.SysEnv.BarCodeSettings;
            _ = this.SysEnv.BarCodeSettings.SaveData(ref tempBarCodeSettings);
            this.SysEnv.BarCodeSettings.UnregisterBarCodeSettingsListenerStatusOut((IBarCodeSettingsEventListener)this);
            this.SysEnv.BarCodeSettings.UnregisterBarCodeSettingsListenerProgressOut((IBarCodeSettingsEventListener)this);
            this.StatusOut(string.Empty);

            this.PrinterSettings.RegPrnSettingsListenerStatusOut((IPrnSettingsListener)this);
            this.PrinterSettings.RegPrnSettingsListenerProgressOut((IPrnSettingsListener)this);
            this.PrinterSettings.RequestPrinterSettings();
            var tempPrinterSettings = this.PrinterSettings;
            _ = this.PrinterSettings.SaveData(ref tempPrinterSettings);
            this.PrinterSettings = tempPrinterSettings;
            this.PrinterSettings.UnregPrnSettingsListenerStatusOut((IPrnSettingsListener)this);
            this.PrinterSettings.UnregPrnSettingsListenerProgressOut((IPrnSettingsListener)this);
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
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task DataStatus()
        {
            this.SysEnv.RefreshOpt = this.SysEnv.RefreshOpt.LoadData();

            await Device.InvokeOnMainThreadAsync(() =>
            {
                this.ExtraInfoItems.Add(new Message() { MessageIndex = this.messageIndex++, MessageValue = Resource.LastRefresh + " " + this.SysEnv.RefreshOpt.RefreshedDataLast.ToString() });
                this.ExtraInfoItems.Add(new Message() { MessageIndex = this.messageIndex++, MessageValue = Resource.RefreshChangesOnly + " " + this.SysEnv.RefreshOpt.IsRefreshedLastDataChangesOnly.ToString() });
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
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="message"></param>
        private void StatusOut(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.ExtraInfoItems.Add(new Message() { MessageIndex = this.messageIndex++, MessageValue = message });
                this.lstStatus.ScrollTo(this.ExtraInfoItems.LastOrDefault(), ScrollToPosition.MakeVisible, false);
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
            // this.pbarStatus.Maximum = maxValue;
            // this.pbarStatus.Minimum = minValue;
            // this.pbarStatus.Value = value;
        }

        /// <summary>
        ///
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
        ///
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
            this.SchemeLabelSettings.RegProdSchemeLabelSettingsListenerStatusOut((IProdSchemeLabelSettingsListener)this);
            this.SchemeLabelSettings.RegProdSchemeLabelSettingsListenerProgressOut((IProdSchemeLabelSettingsListener)this);
            await this.SchemeLabelSettings.RequestProductSchemeLabelSettings();
            var tempSchemeLblSettings = this.SchemeLabelSettings;
            _ = this.SchemeLabelSettings.SaveData(ref tempSchemeLblSettings);
            this.SchemeLabelSettings = tempSchemeLblSettings;
            this.SchemeLabelSettings.UnregProdSchemeLabelSettingsListenerStatusOut((IProdSchemeLabelSettingsListener)this);
            this.SchemeLabelSettings.UnregProdSchemeLabelSettingsListenerProgressOut((IProdSchemeLabelSettingsListener)this);
            this.StatusOut(string.Empty);

            if (this.SysEnv.SysSettings.IsEnableDefaultDetailValues || this.ActSubType == SysCommon.ActSubTypes.PurgeActivityData)
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
        ///
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
        ///
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
        ///
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
        ///
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
        ///
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
        ///
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
        ///
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
        ///
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
        ///
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
        ///
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
                    await this.Adjustment.RequestHeaderData();
                    this.Adjustment.UnregisterDataAccessListenerStatusOut(this);
                    this.Adjustment.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.AdjustmentItem.RegisterDataAccessListenerProgressOut(this);
                    this.AdjustmentItem.RegisterDataAccessListenerStatusOut(this);
                    await this.AdjustmentItem.RequestItemData(0);
                    this.AdjustmentItem.UnregisterDataAccessListenerStatusOut(this);
                    this.AdjustmentItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                    {
                        this.AdjustmentDetail.RegisterDataAccessListenerProgressOut(this);
                        this.AdjustmentDetail.RegisterDataAccessListenerStatusOut(this);
                        await this.AdjustmentDetail.RequestDetailData(0);
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
                    await this.PhysicalCount.RequestHeaderData();
                    this.PhysicalCount.UnregisterDataAccessListenerStatusOut(this);
                    this.PhysicalCount.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.PhysicalCountItem.RegisterDataAccessListenerProgressOut(this);
                    this.PhysicalCountItem.RegisterDataAccessListenerStatusOut(this);
                    await this.PhysicalCountItem.RequestItemData(0);
                    this.PhysicalCountItem.UnregisterDataAccessListenerStatusOut(this);
                    this.PhysicalCountItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                    {
                        this.PhysicalCountDetail.RegisterDataAccessListenerProgressOut(this);
                        this.PhysicalCountDetail.RegisterDataAccessListenerStatusOut(this);
                        await this.PhysicalCountDetail.RequestDetailData(0);
                        this.PhysicalCountDetail.UnregisterDataAccessListenerStatusOut(this);
                        this.PhysicalCountDetail.UnregisterDataAccessListenerProgressOut(this);
                        this.StatusOut(string.Empty);
                    }
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
                    await this.CycleCount.RequestHeaderData();
                    this.CycleCount.UnregisterDataAccessListenerStatusOut(this);
                    this.CycleCount.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.CycleCountItem.RegisterDataAccessListenerProgressOut(this);
                    this.CycleCountItem.RegisterDataAccessListenerStatusOut(this);
                    await this.CycleCountItem.RequestItemData(0);
                    this.CycleCountItem.UnregisterDataAccessListenerStatusOut(this);
                    this.CycleCountItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                    {
                        this.CycleCountDetail.RegisterDataAccessListenerProgressOut(this);
                        this.CycleCountDetail.RegisterDataAccessListenerStatusOut(this);
                        await this.CycleCountDetail.RequestDetailData(0);
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
                this.TicketFinishedDetail.RegisterDataAccessListenerStatusOut(this);
                await this.TicketFinishedDetail.PurgeData();
                this.TicketFinishedDetail.UnregisterDataAccessListenerStatusOut(this);

                this.TicketMaterialDetail.RegisterDataAccessListenerStatusOut(this);
                await this.TicketMaterialDetail.PurgeData();
                this.TicketMaterialDetail.UnregisterDataAccessListenerStatusOut(this);

                this.TicketByProductDetail.RegisterDataAccessListenerStatusOut(this);
                await this.TicketByProductDetail.PurgeData();
                this.TicketByProductDetail.UnregisterDataAccessListenerStatusOut(this);

                this.TicketFinishedItem.RegisterDataAccessListenerStatusOut(this);
                await this.TicketFinishedItem.PurgeData();
                this.TicketFinishedItem.UnregisterDataAccessListenerStatusOut(this);

                this.TicketMaterialItem.RegisterDataAccessListenerStatusOut(this);
                await this.TicketMaterialItem.PurgeData();
                this.TicketMaterialItem.UnregisterDataAccessListenerStatusOut(this);

                this.TicketByProductItem.RegisterDataAccessListenerStatusOut(this);
                await this.TicketByProductItem.PurgeData();
                this.TicketByProductItem.UnregisterDataAccessListenerStatusOut(this);

                this.TicketFinished.RegisterDataAccessListenerStatusOut(this);
                await this.TicketFinished.PurgeData();
                this.TicketFinished.UnregisterDataAccessListenerStatusOut(this);

                this.TicketMaterial.RegisterDataAccessListenerStatusOut(this);
                await this.TicketMaterial.PurgeData();
                this.TicketMaterial.UnregisterDataAccessListenerStatusOut(this);

                this.TicketByProduct.RegisterDataAccessListenerStatusOut(this);
                await this.TicketByProduct.PurgeData();
                this.TicketByProduct.UnregisterDataAccessListenerStatusOut(this);

                if (this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                {
                    this.TicketFinished.RegisterDataAccessListenerProgressOut(this);
                    this.TicketFinished.RegisterDataAccessListenerStatusOut(this);
                    await this.TicketFinished.RequestHeaderData();
                    this.TicketFinished.UnregisterDataAccessListenerStatusOut(this);
                    this.TicketFinished.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.TicketMaterial.RegisterDataAccessListenerProgressOut(this);
                    this.TicketMaterial.RegisterDataAccessListenerStatusOut(this);
                    await this.TicketMaterial.RequestHeaderData();
                    this.TicketMaterial.UnregisterDataAccessListenerStatusOut(this);
                    this.TicketMaterial.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.TicketByProduct.RegisterDataAccessListenerProgressOut(this);
                    this.TicketByProduct.RegisterDataAccessListenerStatusOut(this);
                    await this.TicketByProduct.RequestHeaderData();
                    this.TicketByProduct.UnregisterDataAccessListenerStatusOut(this);
                    this.TicketByProduct.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.TicketFinishedItem.RegisterDataAccessListenerProgressOut(this);
                    this.TicketFinishedItem.RegisterDataAccessListenerStatusOut(this);
                    await this.TicketFinishedItem.RequestItemData(0);
                    this.TicketFinishedItem.UnregisterDataAccessListenerStatusOut(this);
                    this.TicketFinishedItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.TicketMaterialItem.RegisterDataAccessListenerProgressOut(this);
                    this.TicketMaterialItem.RegisterDataAccessListenerStatusOut(this);
                    await this.TicketMaterialItem.RequestItemData(0);
                    this.TicketMaterialItem.UnregisterDataAccessListenerStatusOut(this);
                    this.TicketMaterialItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.TicketByProductItem.RegisterDataAccessListenerProgressOut(this);
                    this.TicketByProductItem.RegisterDataAccessListenerStatusOut(this);
                    await this.TicketByProductItem.RequestItemData(0);
                    this.TicketByProductItem.UnregisterDataAccessListenerStatusOut(this);
                    this.TicketByProductItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                    {
                        this.TicketFinishedDetail.RegisterDataAccessListenerProgressOut(this);
                        this.TicketFinishedDetail.RegisterDataAccessListenerStatusOut(this);
                        await this.TicketFinishedDetail.RequestDetailData(0);
                        this.TicketFinishedDetail.UnregisterDataAccessListenerStatusOut(this);
                        this.TicketFinishedDetail.UnregisterDataAccessListenerProgressOut(this);
                        this.StatusOut(string.Empty);

                        this.TicketMaterialDetail.RegisterDataAccessListenerProgressOut(this);
                        this.TicketMaterialDetail.RegisterDataAccessListenerStatusOut(this);
                        await this.TicketMaterialDetail.RequestDetailData(0);
                        this.TicketMaterialDetail.UnregisterDataAccessListenerStatusOut(this);
                        this.TicketMaterialDetail.UnregisterDataAccessListenerProgressOut(this);
                        this.StatusOut(string.Empty);

                        this.TicketByProductDetail.RegisterDataAccessListenerProgressOut(this);
                        this.TicketByProductDetail.RegisterDataAccessListenerStatusOut(this);
                        await this.TicketByProductDetail.RequestDetailData(0);
                        this.TicketByProductDetail.UnregisterDataAccessListenerStatusOut(this);
                        this.TicketByProductDetail.UnregisterDataAccessListenerProgressOut(this);
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
                await this.ReceivingDetail.PurgeData();
                this.ReceivingDetail.UnregisterDataAccessListenerStatusOut(this);

                this.ReceivingItemComment.RegisterDataAccessListenerStatusOut(this);
                await this.ReceivingItemComment.PurgeData();
                this.ReceivingItemComment.UnregisterDataAccessListenerStatusOut(this);

                this.ReceivingItem.RegisterDataAccessListenerStatusOut(this);
                await this.ReceivingItem.PurgeData();
                this.ReceivingItem.UnregisterDataAccessListenerStatusOut(this);

                this.Receiving.RegisterDataAccessListenerStatusOut(this);
                await this.Receiving.PurgeData();
                this.Receiving.UnregisterDataAccessListenerStatusOut(this);

                if (this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                {
                    this.Receiving.RegisterDataAccessListenerProgressOut(this);
                    this.Receiving.RegisterDataAccessListenerStatusOut(this);
                    await this.Receiving.RequestHeaderData();
                    this.Receiving.UnregisterDataAccessListenerStatusOut(this);
                    this.Receiving.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.ReceivingItem.RegisterDataAccessListenerProgressOut(this);
                    this.ReceivingItem.RegisterDataAccessListenerStatusOut(this);
                    await this.ReceivingItem.RequestItemData(0);
                    this.ReceivingItem.UnregisterDataAccessListenerStatusOut(this);
                    this.ReceivingItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                    {
                        this.ReceivingDetail.RegisterDataAccessListenerProgressOut(this);
                        this.ReceivingDetail.RegisterDataAccessListenerStatusOut(this);
                        await this.ReceivingDetail.RequestDetailData(0);
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
                await this.PurchaseOrderWorkSheetItem.PurgeData();
                this.PurchaseOrderWorkSheetItem.UnregisterDataAccessListenerStatusOut(this);

                if (this.ActSubType != SysCommon.ActSubTypes.PurgeActivityData)
                {
                    this.PurchaseOrderWorkSheetItem.RegisterDataAccessListenerProgressOut(this);
                    this.PurchaseOrderWorkSheetItem.RegisterDataAccessListenerStatusOut(this);
                    await this.PurchaseOrderWorkSheetItem.RequestItemData(0);
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
                    await this.Picking.RequestHeaderData();
                    this.Picking.UnregisterDataAccessListenerStatusOut(this);
                    this.Picking.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.PickingItem.RegisterDataAccessListenerProgressOut(this);
                    this.PickingItem.RegisterDataAccessListenerStatusOut(this);
                    await this.PickingItem.RequestItemData(0);
                    this.PickingItem.UnregisterDataAccessListenerStatusOut(this);
                    this.PickingItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                    {
                        this.PickingDetail.RegisterDataAccessListenerProgressOut(this);
                        this.PickingDetail.RegisterDataAccessListenerStatusOut(this);
                        await this.PickingDetail.RequestDetailData(0);
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
                    await this.TransferFrom.RequestHeaderData();
                    this.TransferFrom.UnregisterDataAccessListenerStatusOut(this);
                    this.TransferFrom.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.TransferFromItem.RegisterDataAccessListenerProgressOut(this);
                    this.TransferFromItem.RegisterDataAccessListenerStatusOut(this);
                    await this.TransferFromItem.RequestItemData(0);
                    this.TransferFromItem.UnregisterDataAccessListenerStatusOut(this);
                    this.TransferFromItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.TransferTo.RegisterDataAccessListenerProgressOut(this);
                    this.TransferTo.RegisterDataAccessListenerStatusOut(this);
                    await this.TransferTo.RequestHeaderData();
                    this.TransferTo.UnregisterDataAccessListenerStatusOut(this);
                    this.TransferTo.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    this.TransferToItem.RegisterDataAccessListenerProgressOut(this);
                    this.TransferToItem.RegisterDataAccessListenerStatusOut(this);
                    await this.TransferToItem.RequestItemData(0);
                    this.TransferToItem.UnregisterDataAccessListenerStatusOut(this);
                    this.TransferToItem.UnregisterDataAccessListenerProgressOut(this);
                    this.StatusOut(string.Empty);

                    if (this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail)
                    {
                        this.TransferFromDetail.RegisterDataAccessListenerProgressOut(this);
                        this.TransferFromDetail.RegisterDataAccessListenerStatusOut(this);
                        await this.TransferFromDetail.RequestDetailData(0);
                        this.TransferFromDetail.UnregisterDataAccessListenerStatusOut(this);
                        this.TransferFromDetail.UnregisterDataAccessListenerProgressOut(this);
                        this.StatusOut(string.Empty);

                        this.TransferToDetail.RegisterDataAccessListenerProgressOut(this);
                        this.TransferToDetail.RegisterDataAccessListenerStatusOut(this);
                        await this.TransferToDetail.RequestDetailData(0);
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