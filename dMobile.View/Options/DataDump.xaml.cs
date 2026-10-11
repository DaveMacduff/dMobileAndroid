// <copyright file="DataDump.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Options
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using Bedrock;
    using dMobile.Bedrock.Settings;
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
    public partial class DataDump : IAppArea
    {
        /// <summary>
        /// 
        /// </summary>
        private bool isFirstAppearance = true;

        /// <summary>
        /// 
        /// </summary>
        private int messageIndex;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataDump" /> class.
        /// </summary>
        /// <param name="actSubTypes"><see cref="SysCommon.ActSubTypes" /> Activity subtypes.</param>
        public DataDump(SysCommon.ActSubTypes actSubTypes)
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
        ///     Gets or sets a value indicating whether the data dump is complete.
        /// </summary>
        private bool IsDataDumpComplete { get; set; }

        /// <summary>
        ///     Gets the activity subtype.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; }

        /// <summary>
        ///     Gets the full class name.
        /// </summary>
        private string FullClassName { get; } = typeof(RefreshDatabaseStatus).FullName;

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
        public Task ScannerDisable()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns></returns>
        public Task ScannerEnable()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsReadyToSave()
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

            return Task.FromResult(isReadyToSave);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsReadyToClose()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Task<bool> SaveChanges()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Task<bool> CancelChanges()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Task<XDocument> GetXml()
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<IsDataDumpComplete>" + this.IsDataDumpComplete + "</IsDataDumpComplete>"
                + "</params>");

            return Task.FromResult(xmldocParams);
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
                    await RefreshDataStatus_GotFocus(this, EventArgs.Empty);
                }

                this.IsIndicatorRunning = false;
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            async Task RefreshDataStatus_GotFocus(object sender, EventArgs e)
            {
                this.IsIndicatorRunning = true;
                await Task.Run(async () =>
                {
                    try
                    {
                        this.IsDataDumpComplete = true;
                        ////await this.RefreshData();

                        this.StatusOut(Resource.RefreshDataCompleted);
                        this.IsDataDumpComplete = true;
                    }
                    catch (SQLiteException ex)
                    {
                        this.StatusOut(Resource.RefreshDataFailed);
                        var errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex, this.FullClassName,
                            "OnAppearing");
                        _ = await Device.InvokeOnMainThreadAsync(async () => await errRpt.ShowDialog());
                    }
                    catch (Exception ex)
                    {
                        this.StatusOut(Resource.RefreshDataFailed);
                        var errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex, this.FullClassName,
                            "tmrRefreshData_Tick");
                        _ = await Device.InvokeOnMainThreadAsync(async () => await errRpt.ShowDialog());
                    }
                });

                this.IsIndicatorRunning = false;
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
        ///     The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            SysCommon.ShutdownConnection();
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
            return "Data Dump Status";
        }

        /// <summary>
        ///     Button dump it click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated this event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event argument data.</param>
        private async void BtnDumpIt_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            await this.Close();
        }
    }
}