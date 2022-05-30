// <copyright file="TranslatorVocabulary.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

#if MULTILANGUAGE
//+------------------------------------------------------------------------+
//| This file has been automatically generated.                            |
//| Any changes to this file will be lost upon build.                      |
//| This enum was generated in a pre-build event for this project.         |
//| Date: 2018-03-21 03:50:36 PM                                           |
//| Source: "Language\dMobileTranslation.ENU.txt"                          |
//+------------------------------------------------------------------------+
namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Words and messages that have alternate translations.
    /// </summary>
    public enum TranslatorVocabulary
    {

        /// <summary>
        /// Login ID
        /// </summary>
        LoginID = 0,

        /// <summary>
        /// Password
        /// </summary>
        Password = 1,

        /// <summary>
        /// Login
        /// </summary>
        Login = 2,

        /// <summary>
        /// Warehouse
        /// </summary>
        Warehouse = 3,

        /// <summary>
        /// Save
        /// </summary>
        Save = 4,

        /// <summary>
        /// Cancel
        /// </summary>
        Cancel = 5,

        /// <summary>
        /// KG
        /// </summary>
        KG = 6,

        /// <summary>
        /// Weight
        /// </summary>
        Weight = 7,

        /// <summary>
        /// Quantity
        /// </summary>
        Quantity = 8,

        /// <summary>
        /// Equals
        /// </summary>
        Equals = 9,

        /// <summary>
        /// Startup
        /// </summary>
        Startup = 10,

        /// <summary>
        /// Loading location map.
        /// </summary>
        LoadLocationMap = 11,

        /// <summary>
        /// Loading location map completed.
        /// </summary>
        LoadLocationMapCompleted = 12,

        /// <summary>
        /// Attempting to sync date & time.
        /// </summary>
        AttemptSyncDateAndTime = 13,

        /// <summary>
        /// Connection failed.
        /// </summary>
        ConnectionFailed = 14,

        /// <summary>
        /// Requesting date & time.
        /// </summary>
        RequestingDateAndTime = 15,

        /// <summary>
        /// Request date & time error.
        /// </summary>
        RequestingDateAndTimeError = 16,

        /// <summary>
        /// Date & time synchronized.
        /// </summary>
        DateAndTimeSyncronized = 17,

        /// <summary>
        /// Socket exception occurred.
        /// </summary>
        SocketExceptionOccurred = 18,

        /// <summary>
        /// Sync date & time failed.
        /// </summary>
        SyncDateAndTimeFailed = 19,

        /// <summary>
        /// Locating database.
        /// </summary>
        LocatingDB = 20,

        /// <summary>
        /// Database found.
        /// </summary>
        DBFound = 21,

        /// <summary>
        /// Database missing.
        /// </summary>
        DBMissing = 22,

        /// <summary>
        /// Creating database.
        /// </summary>
        CreatingDB = 23,

        /// <summary>
        /// Seeding system users.
        /// </summary>
        SeedingSystemUsers = 24,

        /// <summary>
        /// Database creation failed.
        /// </summary>
        DBCreationFailed = 25,

        /// <summary>
        /// Database creation completed.
        /// </summary>
        DBCreationCompleted = 26,

        /// <summary>
        /// Identifying bar code scanner.
        /// </summary>
        IdentifyingBarcodeScanner = 27,

        /// <summary>
        /// Unable to identify bar code scanner.
        /// </summary>
        UnableToIDBarcodeScanner = 28,

        /// <summary>
        /// Loading products.
        /// </summary>
        LoadingProducts = 29,

        /// <summary>
        /// Loading products completed.
        /// </summary>
        LoadingProductsCompleted = 30,

        /// <summary>
        /// Initializing environment.
        /// </summary>
        InitializingEnvironment = 31,

        /// <summary>
        /// Attempting communication test.
        /// </summary>
        AttemptingCommTest = 32,

        /// <summary>
        /// Base socket port: {0}
        /// </summary>
        BaseSocketPort = 33,

        /// <summary>
        /// Default socket port: {0}
        /// </summary>
        DefaultSocketPort = 34,

        /// <summary>
        /// Max socket port: {0}
        /// </summary>
        MaxSocketPort = 35,

        /// <summary>
        /// Connection retries: {0}
        /// </summary>
        ConnectionRetries = 36,

        /// <summary>
        /// Socket send retries: {0}
        /// </summary>
        SocketSendRetries = 37,

        /// <summary>
        /// Socket receive retries: {0}
        /// </summary>
        SocketReceiveRetries = 38,

        /// <summary>
        /// Send time out: {0} seconds.
        /// </summary>
        SendTimeOut = 39,

        /// <summary>
        /// Receive time out: {0} seconds.
        /// </summary>
        ReceiveTimeOut = 40,

        /// <summary>
        /// Receive buffer size: {0}
        /// </summary>
        ReceiveBufferSize = 41,

        /// <summary>
        /// Server host IP: {0}
        /// </summary>
        ServerHostIP = 42,

        /// <summary>
        /// Creating connection request.
        /// </summary>
        CreatingConnectionRequest = 43,

        /// <summary>
        /// Sending device ID.
        /// </summary>
        SendingDeviceID = 44,

        /// <summary>
        /// Bytes sent: {0}
        /// </summary>
        BytesSent = 45,

        /// <summary>
        /// Waiting for response.
        /// </summary>
        WaitingForResponse = 46,

        /// <summary>
        /// Socket closed.
        /// </summary>
        SocketClosed = 47,

        /// <summary>
        /// Submit device id error.
        /// </summary>
        SubmitDeviceIDError = 48,

        /// <summary>
        /// Connection succeeded.
        /// </summary>
        ConnectionSucceeded = 49,

        /// <summary>
        /// Communication test failed.
        /// </summary>
        CommunicationTestFailed = 50,

        /// <summary>
        /// Login Startup
        /// </summary>
        LoginStartup = 51,

        /// <summary>
        /// Entered data failed validation.
        /// </summary>
        EnteredDataFailedValidation = 52,

        /// <summary>
        /// Invalid login id {0}
        /// </summary>
        InvalidLoginID = 53,

        /// <summary>
        /// Invalid login password.
        /// </summary>
        InvalidLoginPassword = 54,

        /// <summary>
        /// Selected warehouse required.
        /// </summary>
        SelectedWarehouseRequired = 55,

        /// <summary>
        /// All modules enabled. Bypassing device licenses.
        /// </summary>
        AllModulesEnabledBypassDeviceLicense = 56,

        /// <summary>
        /// Set Location
        /// </summary>
        SetLocation = 57,

        /// <summary>
        /// Select To Unit
        /// </summary>
        SelectToUnit = 58,

        /// <summary>
        /// Select From Unit
        /// </summary>
        SelectFromUnit = 59,

        /// <summary>
        /// Converted Qty 0 UOM @ 0 KG/LB
        /// </summary>
        ConvertedQtyUOMKGLB = 60,

        /// <summary>
        /// Qty 0 UOM @ 0 KG/LB
        /// </summary>
        QtyUOMKGLB = 61,

        /// <summary>
        /// Qty 0 UOM @ KG/LB
        /// </summary>
        QtyUOMKGLBz = 62,

        /// <summary>
        /// Weight 
        /// </summary>
        Weightz = 63,

        /// <summary>
        /// Split To Singles
        /// </summary>
        SplitToSingles = 64,

        /// <summary>
        /// From Unit
        /// </summary>
        FromUnit = 65,

        /// <summary>
        /// Select Skid
        /// </summary>
        SelectSkid = 66,

        /// <summary>
        /// Add Skid
        /// </summary>
        AddSkid = 67,

        /// <summary>
        /// Add Detail
        /// </summary>
        AddDetail = 68,

        /// <summary>
        /// Variant
        /// </summary>
        Variant = 69,

        /// <summary>
        /// Best Before
        /// </summary>
        BestBefore = 70,

        /// <summary>
        /// Lot No
        /// </summary>
        LotNo = 71,

        /// <summary>
        /// Production
        /// </summary>
        Production = 72,

        /// <summary>
        /// Serial No
        /// </summary>
        SerialNo = 73,

        /// <summary>
        /// Extra 5
        /// </summary>
        Extra = 74,

        /// <summary>
        /// Extra 4
        /// </summary>
        Extraz = 75,

        /// <summary>
        /// Extra 3
        /// </summary>
        Extrazz = 76,

        /// <summary>
        /// Extra 2
        /// </summary>
        Extrazzz = 77,

        /// <summary>
        /// Extra 1
        /// </summary>
        Extrazzzz = 78,

        /// <summary>
        /// Add
        /// </summary>
        Add = 79,

        /// <summary>
        /// Filter
        /// </summary>
        Filter = 80,

        /// <summary>
        /// Sort By
        /// </summary>
        SortBy = 81,

        /// <summary>
        /// Apply Adjustment Type Filter
        /// </summary>
        ApplyAdjustmentTypeFilter = 82,

        /// <summary>
        /// Apply Adjustment Date Filter
        /// </summary>
        ApplyAdjustmentDateFilter = 83,

        /// <summary>
        /// Date
        /// </summary>
        Date = 84,

        /// <summary>
        /// Type
        /// </summary>
        Type = 85,

        /// <summary>
        /// Negate Scan Quantity
        /// </summary>
        NegateScanQuantity = 86,

        /// <summary>
        /// Apply Date Filter
        /// </summary>
        ApplyDateFilter = 87,

        /// <summary>
        /// Count Date
        /// </summary>
        CountDate = 88,

        /// <summary>
        /// Apply Physical Count Date Filter
        /// </summary>
        ApplyPhysicalCountDateFilter = 89,

        /// <summary>
        /// Apply Route Code Filter
        /// </summary>
        ApplyRouteCodeFilter = 90,

        /// <summary>
        /// Apply Delivery Date Filter
        /// </summary>
        ApplyDeliveryDateFilter = 91,

        /// <summary>
        /// Delivery
        /// </summary>
        Delivery = 92,

        /// <summary>
        /// Apply Ordered Date Filter
        /// </summary>
        ApplyOrderedDateFilter = 93,

        /// <summary>
        /// Ordered
        /// </summary>
        Ordered = 94,

        /// <summary>
        /// Route
        /// </summary>
        Route = 95,

        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 96,

        /// <summary>
        /// Search For
        /// </summary>
        SearchFor = 97,

        /// <summary>
        /// Whs
        /// </summary>
        Whs = 98,

        /// <summary>
        /// Section
        /// </summary>
        Section = 99,

        /// <summary>
        /// 1. Warehouse
        /// </summary>
        Warehousez = 100,

        /// <summary>
        /// 2. Section
        /// </summary>
        Sectionz = 101,

        /// <summary>
        /// Apply Production Date Filter
        /// </summary>
        ApplyProductionDateFilter = 102,

        /// <summary>
        /// Issued
        /// </summary>
        Issued = 103,

        /// <summary>
        /// Apply Issued Date Filter
        /// </summary>
        ApplyIssuedDateFilter = 104,

        /// <summary>
        /// Apply Arrival Date Filter
        /// </summary>
        ApplyArrivalDateFilter = 105,

        /// <summary>
        /// Arrival
        /// </summary>
        Arrival = 106,

        /// <summary>
        /// Apply Returned Date Filter
        /// </summary>
        ApplyReturnedDateFilter = 107,

        /// <summary>
        /// Returned
        /// </summary>
        Returned = 108,

        /// <summary>
        /// Apply Entered Date Filter
        /// </summary>
        ApplyEnteredDateFilter = 109,

        /// <summary>
        /// Entered
        /// </summary>
        Entered = 110,

        /// <summary>
        /// Complete
        /// </summary>
        Complete = 111,

        /// <summary>
        /// Detail
        /// </summary>
        Detail = 112,

        /// <summary>
        /// Select Product
        /// </summary>
        SelectProduct = 113,

        /// <summary>
        /// UOM
        /// </summary>
        UOM = 114,

        /// <summary>
        /// Cube
        /// </summary>
        Cube = 115,

        /// <summary>
        /// Hi
        /// </summary>
        Hi = 116,

        /// <summary>
        /// Ti
        /// </summary>
        Ti = 117,

        /// <summary>
        /// New
        /// </summary>
        New = 118,

        /// <summary>
        /// Is Put Away
        /// </summary>
        IsPutAway = 119,

        /// <summary>
        /// Height
        /// </summary>
        Height = 120,

        /// <summary>
        /// Skid #
        /// </summary>
        Skid = 121,

        /// <summary>
        /// Transfer Date
        /// </summary>
        TransferDate = 122,

        /// <summary>
        /// Apply Transfer Date Filter
        /// </summary>
        ApplyTransferDateFilter = 123,

        /// <summary>
        /// Display Defaults
        /// </summary>
        DisplayDefaults = 124,

        /// <summary>
        /// Add Prod
        /// </summary>
        AddProd = 125,

        /// <summary>
        /// Details
        /// </summary>
        Details = 126,

        /// <summary>
        /// Next
        /// </summary>
        Next = 127,

        /// <summary>
        /// Ordered:
        /// </summary>
        Orderedz = 128,

        /// <summary>
        /// Shipped:
        /// </summary>
        Shipped = 129,

        /// <summary>
        /// Product:
        /// </summary>
        Product = 130,

        /// <summary>
        /// Prev
        /// </summary>
        Prev = 131,

        /// <summary>
        /// C
        /// </summary>
        C = 132,

        /// <summary>
        /// Set From Location
        /// </summary>
        SetFromLocation = 133,

        /// <summary>
        /// Set To Location
        /// </summary>
        SetToLocation = 134,

        /// <summary>
        /// Scan To Loc.
        /// </summary>
        ScanToLoc = 135,

        /// <summary>
        /// Product
        /// </summary>
        Productz = 136,

        /// <summary>
        /// Assign Scan To Product
        /// </summary>
        AssignScanToProduct = 137,

        /// <summary>
        /// Unidentified
        /// </summary>
        Unidentified = 138,

        /// <summary>
        /// Full Cases
        /// </summary>
        FullCases = 139,

        /// <summary>
        /// Repacks
        /// </summary>
        Repacks = 140,

        /// <summary>
        /// Order #
        /// </summary>
        Order = 141,

        /// <summary>
        /// Fresh
        /// </summary>
        Fresh = 142,

        /// <summary>
        /// Frozen
        /// </summary>
        Frozen = 143,

        /// <summary>
        /// Dry
        /// </summary>
        Dry = 144,

        /// <summary>
        /// Live
        /// </summary>
        Live = 145,

        /// <summary>
        /// Pallets
        /// </summary>
        Pallets = 146,

        /// <summary>
        /// Total
        /// </summary>
        Total = 147,

        /// <summary>
        /// 3. Rework
        /// </summary>
        Rework = 148,

        /// <summary>
        /// 2. Production
        /// </summary>
        Productionz = 149,

        /// <summary>
        /// 1. Inventory
        /// </summary>
        Inventory = 150,

        /// <summary>
        /// 4. Material Requisition
        /// </summary>
        MaterialRequisition = 151,

        /// <summary>
        /// 5. Disposal
        /// </summary>
        Disposal = 152,

        /// <summary>
        /// 2. Cycle Count
        /// </summary>
        CycleCount = 153,

        /// <summary>
        /// 1. Physical Count
        /// </summary>
        PhysicalCount = 154,

        /// <summary>
        /// 4. Skids...
        /// </summary>
        Skids = 155,

        /// <summary>
        /// 3. Counts...
        /// </summary>
        Counts = 156,

        /// <summary>
        /// 2. Convert Units (Break Case)
        /// </summary>
        ConvertUnitsBreakCase = 157,

        /// <summary>
        /// 1. Movement...
        /// </summary>
        Movement = 158,

        /// <summary>
        /// 5. Inquiry
        /// </summary>
        Inquiry = 159,

        /// <summary>
        /// 6. Adjustments...
        /// </summary>
        Adjustments = 160,

        /// <summary>
        /// 1. Move Product
        /// </summary>
        MoveProduct = 161,

        /// <summary>
        /// 2. Transfers
        /// </summary>
        Transfers = 162,

        /// <summary>
        /// 3. Replenishment
        /// </summary>
        Replenishment = 163,

        /// <summary>
        /// 4. Move Skid
        /// </summary>
        MoveSkid = 164,

        /// <summary>
        /// 5. Options...
        /// </summary>
        Options = 165,

        /// <summary>
        /// 4. Production...
        /// </summary>
        Productionzz = 166,

        /// <summary>
        /// 3. Inventory...
        /// </summary>
        Inventoryz = 167,

        /// <summary>
        /// 2. Sales Orders...
        /// </summary>
        SalesOrders = 168,

        /// <summary>
        /// 1. Receiving && Put Away...
        /// </summary>
        ReceivingPutAway = 169,

        /// <summary>
        /// 6. About
        /// </summary>
        About = 170,

        /// <summary>
        /// 4. Submit Batch Detail
        /// </summary>
        SubmitBatchDetail = 171,

        /// <summary>
        /// 3. Refresh Data
        /// </summary>
        RefreshData = 172,

        /// <summary>
        /// 2. Communication Setup
        /// </summary>
        CommunicationSetup = 173,

        /// <summary>
        /// 1. Setup Defaults
        /// </summary>
        SetupDefaults = 174,

        /// <summary>
        /// 5. Utilities...
        /// </summary>
        Utilities = 175,

        /// <summary>
        /// 1. Finished Goods
        /// </summary>
        FinishedGoods = 176,

        /// <summary>
        /// 2. Ingredient Materials
        /// </summary>
        IngredientMaterials = 177,

        /// <summary>
        /// 3. Additional Products
        /// </summary>
        AdditionalProducts = 178,

        /// <summary>
        /// 2. Put Away
        /// </summary>
        PutAway = 179,

        /// <summary>
        /// 1. Receiving
        /// </summary>
        Receiving = 180,

        /// <summary>
        /// 3. Work Sheet
        /// </summary>
        WorkSheet = 181,

        /// <summary>
        /// 2. Returns
        /// </summary>
        Returns = 182,

        /// <summary>
        /// 1. Picking
        /// </summary>
        Picking = 183,

        /// <summary>
        /// 3. Release By Order #
        /// </summary>
        ReleaseByOrder = 184,

        /// <summary>
        /// 4. Release By Section
        /// </summary>
        ReleaseBySection = 185,

        /// <summary>
        /// 1. Scan Bar Codes
        /// </summary>
        ScanBarCodes = 186,

        /// <summary>
        /// 2. Product Labels
        /// </summary>
        ProductLabels = 187,

        /// <summary>
        /// 3. Assign Default Location
        /// </summary>
        AssignDefaultLocation = 188,

        /// <summary>
        /// Device ID / Name / IP
        /// </summary>
        DeviceIDNameIP = 189,

        /// <summary>
        /// MAC Address:
        /// </summary>
        MACAddress = 190,

        /// <summary>
        /// Status:
        /// </summary>
        Status = 191,

        /// <summary>
        /// <Unknown>
        /// </summary>
        Unknownz = 192,

        /// <summary>
        /// Speed:
        /// </summary>
        Speed = 193,

        /// <summary>
        /// Show Memory
        /// </summary>
        ShowMemory = 194,

        /// <summary>
        /// Version Info
        /// </summary>
        VersionInfo = 195,

        /// <summary>
        /// Mobile Server IP Address
        /// </summary>
        MobileServerIPAddress = 196,

        /// <summary>
        /// Default Port
        /// </summary>
        DefaultPort = 197,

        /// <summary>
        /// Max Ports
        /// </summary>
        MaxPorts = 198,

        /// <summary>
        /// Base Port
        /// </summary>
        BasePort = 199,

        /// <summary>
        /// Connect
        /// </summary>
        Connect = 200,

        /// <summary>
        /// Advanced
        /// </summary>
        Advanced = 201,

        /// <summary>
        /// Send TimeOut Seconds
        /// </summary>
        SendTimeOutSeconds = 202,

        /// <summary>
        /// Receive TimeOut Seconds
        /// </summary>
        ReceiveTimeOutSeconds = 203,

        /// <summary>
        /// Receive Buffer Size Bytes
        /// </summary>
        ReceiveBufferSizeBytes = 204,

        /// <summary>
        /// Connection Attempts
        /// </summary>
        ConnectionAttempts = 205,

        /// <summary>
        /// Send Attempts
        /// </summary>
        SendAttempts = 206,

        /// <summary>
        /// Receive Attempts
        /// </summary>
        ReceiveAttempts = 207,

        /// <summary>
        /// Counts
        /// </summary>
        Countsz = 208,

        /// <summary>
        /// Refresh Data
        /// </summary>
        RefreshDataz = 209,

        /// <summary>
        /// Replenishment
        /// </summary>
        Replenishmentz = 210,

        /// <summary>
        /// Production Tickets
        /// </summary>
        ProductionTickets = 211,

        /// <summary>
        /// Select/Deselect All
        /// </summary>
        SelectDeselectAll = 212,

        /// <summary>
        /// Sales Orders
        /// </summary>
        SalesOrdersz = 213,

        /// <summary>
        /// Purchase Orders
        /// </summary>
        PurchaseOrders = 214,

        /// <summary>
        /// Adjustments
        /// </summary>
        Adjustmentsz = 215,

        /// <summary>
        /// Purchase Order Work Sheet
        /// </summary>
        PurchaseOrderWorkSheet = 216,

        /// <summary>
        /// Transfers
        /// </summary>
        Transfersz = 217,

        /// <summary>
        /// System Data
        /// </summary>
        SystemData = 218,

        /// <summary>
        /// Data Status
        /// </summary>
        DataStatus = 219,

        /// <summary>
        /// Refresh Data Changes Only
        /// </summary>
        RefreshDataChangesOnly = 220,

        /// <summary>
        /// Setup
        /// </summary>
        Setup = 221,

        /// <summary>
        /// Print Product Labels as a Batch
        /// </summary>
        PrintProductLabelsasaBatch = 222,

        /// <summary>
        /// Verify Location
        /// </summary>
        VerifyLocation = 223,

        /// <summary>
        /// Disconnected Mode Load Detail
        /// </summary>
        DisconnectedModeLoadDetail = 224,

        /// <summary>
        /// Disconnected Mode
        /// </summary>
        DisconnectedMode = 225,

        /// <summary>
        /// Portable Printers
        /// </summary>
        PortablePrinters = 226,

        /// <summary>
        /// Product Label Printer
        /// </summary>
        ProductLabelPrinter = 227,

        /// <summary>
        /// Shelf Label Printer
        /// </summary>
        ShelfLabelPrinter = 228,

        /// <summary>
        /// Shipping Label Printer
        /// </summary>
        ShippingLabelPrinter = 229,

        /// <summary>
        /// Submit Selected
        /// </summary>
        SubmitSelected = 230,

        /// <summary>
        /// Status
        /// </summary>
        Statusz = 231,

        /// <summary>
        /// Activity
        /// </summary>
        Activity = 232,

        /// <summary>
        /// Current Default Location(s)
        /// </summary>
        CurrentDefaultLocations = 233,

        /// <summary>
        /// Not Set
        /// </summary>
        NotSet = 234,

        /// <summary>
        /// Receiving
        /// </summary>
        Receivingz = 235,

        /// <summary>
        /// Picking
        /// </summary>
        Pickingz = 236,

        /// <summary>
        /// Convert Unit
        /// </summary>
        ConvertUnit = 237,

        /// <summary>
        /// Convert Units Edit
        /// </summary>
        ConvertUnitsEdit = 238,

        /// <summary>
        /// Adjustment # {0}
        /// </summary>
        Adjustment = 239,

        /// <summary>
        /// Count # {0}
        /// </summary>
        Count = 240,

        /// <summary>
        /// Pick # {0}
        /// </summary>
        PickingOrder = 241,

        /// <summary>
        /// Receive # {0}
        /// </summary>
        Receive = 242,

        /// <summary>
        /// Return # {0}
        /// </summary>
        Return = 243,

        /// <summary>
        /// Ticket Add-Product # {0}
        /// </summary>
        TicketAddProduct = 244,

        /// <summary>
        /// Ticket Finished # {0}
        /// </summary>
        TicketFinished = 245,

        /// <summary>
        /// Ticket Material # {0}
        /// </summary>
        TicketMaterial = 246,

        /// <summary>
        /// Transfer # {0}
        /// </summary>
        Transfer = 247,

        /// <summary>
        /// Adjustments Menu
        /// </summary>
        AdjustmentsMenu = 248,

        /// <summary>
        /// Inventory Counts Menu
        /// </summary>
        InventoryCountsMenu = 249,

        /// <summary>
        /// Inventory Menu
        /// </summary>
        InventoryMenu = 250,

        /// <summary>
        /// Inventory Movement Menu
        /// </summary>
        InventoryMovementMenu = 251,

        /// <summary>
        /// Main Menu
        /// </summary>
        MainMenu = 252,

        /// <summary>
        /// Options Menu
        /// </summary>
        OptionsMenu = 253,

        /// <summary>
        /// Production Ticket Menu
        /// </summary>
        ProductionTicketMenu = 254,

        /// <summary>
        /// Purchase Orders Menu
        /// </summary>
        PurchaseOrdersMenu = 255,

        /// <summary>
        /// Sales Orders Menu
        /// </summary>
        SalesOrdersMenu = 256,

        /// <summary>
        /// Utilities Menu
        /// </summary>
        UtilitiesMenu = 257,

        /// <summary>
        /// About dMobile
        /// </summary>
        AboutdMobile = 258,

        /// <summary>
        /// Unit conversion complete.
        /// </summary>
        UnitConversionComplete = 259,

        /// <summary>
        /// Do you wish to print product labels?
        /// </summary>
        DoYouWishToPrintProductLabels = 260,

        /// <summary>
        /// Unit Conversion
        /// </summary>
        UnitConversion = 261,

        /// <summary>
        /// Do you wish to convert any more unit(s)?
        /// </summary>
        DoYouWishToConvertAnyMoreUnits = 262,

        /// <summary>
        /// Convert Unit(s)
        /// </summary>
        ConvertUnits = 263,

        /// <summary>
        /// Location must be set.
        /// </summary>
        LocationMustBeSet = 264,

        /// <summary>
        /// Convert from unit must be set.
        /// </summary>
        ConvertFromUnitMustBeSet = 265,

        /// <summary>
        /// Convert to unit(s) must be set.
        /// </summary>
        ConvertToUnitsMustBeSet = 266,

        /// <summary>
        /// Qty {0} {1} @ {2} {3}
        /// </summary>
        Qty = 267,

        /// <summary>
        /// A new skid will be created upon saving.
        /// </summary>
        ANewSkidWillBeCreatedUponSaving = 268,

        /// <summary>
        /// Skid Info
        /// </summary>
        SkidInfo = 269,

        /// <summary>
        /// Do you wish to exit?
        /// </summary>
        DoYouWishToExit = 270,

        /// <summary>
        /// Print skid label?
        /// </summary>
        PrintSkidLabel = 271,

        /// <summary>
        /// Quantity {0} {1} @ {2} {3}
        /// </summary>
        Quantityz = 272,

        /// <summary>
        /// Skid completed.
        /// </summary>
        SkidCompleted = 273,

        /// <summary>
        /// Cube value must be greater than or equal to zero.
        /// </summary>
        CubeValueMustBeGreaterThanOrEqualToZero = 274,

        /// <summary>
        /// Do you wish to break this skid?
        /// </summary>
        DoYouWishToBreakThisSkid = 275,

        /// <summary>
        /// This skid is now broken.
        /// </summary>
        ThisSkidIsNowBroken = 276,

        /// <summary>
        /// A new Skid will be created upon saving.
        /// </summary>
        ANewSkidWillBeCreatedUponSavingz = 277,

        /// <summary>
        /// Continue?
        /// </summary>
        Continue = 278,

        /// <summary>
        /// Quantity inquiry is not supported in disconnected mode.
        /// </summary>
        QuantityInquiryIsNotSupportedInDisconnectedMode = 279,

        /// <summary>
        /// Inventory Inquiry
        /// </summary>
        InventoryInquiry = 280,

        /// <summary>
        /// Quantity Validation
        /// </summary>
        QuantityValidation = 281,

        /// <summary>
        /// Product ID
        /// </summary>
        ProductId = 282,

        /// <summary>
        /// SIZE: 
        /// </summary>
        Size = 283,

        /// <summary>
        /// Default
        /// </summary>
        Default = 284,

        /// <summary>
        /// Activity Items
        /// </summary>
        ActivityItems = 285,

        /// <summary>
        /// Description
        /// </summary>
        Description = 286,

        /// <summary>
        /// dMobile Activity Transfer Filter
        /// </summary>
        DmobileActivityTransferFilter = 287,

        /// <summary>
        /// Data Failed Validation. Check Your Data.
        /// </summary>
        DataFailedValidationCheckYourData = 288,

        /// <summary>
        /// Do you wish to print all skid label(s)?
        /// </summary>
        DoYouWishToPrintAllSkidLabels = 289,

        /// <summary>
        /// Brand
        /// </summary>
        Brand = 290,

        /// <summary>
        /// Skid Labels
        /// </summary>
        SkidLabels = 291,

        /// <summary>
        /// Some line items exists where the shipped quantity is less that the quantity ordered.
        /// </summary>
        SomeLineItemsExistsWhereTheShippedQuantityIsLessThatTheQuantityOrdered = 292,

        /// <summary>
        /// Inventory Inquiry Summary
        /// </summary>
        InventoryInquirySummary = 293,

        /// <summary>
        /// Slot
        /// </summary>
        Slot = 294,

        /// <summary>
        /// Delete Skid
        /// </summary>
        DeleteSkid = 295,

        /// <summary>
        /// Item Comment
        /// </summary>
        ItemComment = 296,

        /// <summary>
        /// A line item does not exist for this warehouse {0} and product \'{1}.\'
        /// </summary>
        ALineItemDoesNotExistForThisWarehouseAndProduct = 297,

        /// <summary>
        /// Do you wish to create one?
        /// </summary>
        DoYouWishToCreateOne = 298,

        /// <summary>
        /// Add the ENTIRE skid to this activity?
        /// </summary>
        AddTheEntireSkidToThisActivity = 299,

        /// <summary>
        /// Do you wish to delete this skid \'{0}\'?
        /// </summary>
        DoYouWishToDeleteThisSkid = 300,

        /// <summary>
        /// Scanning a shipping container bar code is not supported in disconnected mode.
        /// </summary>
        ScanningAShippingContainerBarCodeIsNotSupportedInDisconnectedMode = 301,

        /// <summary>
        /// Loading items. Please wait...
        /// </summary>
        LoadingItemsPleaseWait = 302,

        /// <summary>
        /// Invalid Quantity!
        /// </summary>
        InvalidQuantity = 303,

        /// <summary>
        /// Scanning an line item detail bar code is not supported here! In Disconnected Mode
        /// </summary>
        ScanningAnLineItemDetailBarCodeIsNotSupportedHereInDisconnectedMode = 304,

        /// <summary>
        /// Skid# 
        /// </summary>
        Skidz = 305,

        /// <summary>
        /// dMobile Activity Detail Edit
        /// </summary>
        DmobileActivityDetailEdit = 306,

        /// <summary>
        /// PRODUCT: {0}
        /// </summary>
        Productzz = 307,

        /// <summary>
        /// LOCATION: {0}
        /// </summary>
        Location = 308,

        /// <summary>
        /// LOCATION: 
        /// </summary>
        Locationz = 309,

        /// <summary>
        /// dMobile Activity Move Inventory
        /// </summary>
        DmobileActivityMoveInventory = 310,

        /// <summary>
        /// Scanned product code \'{0}\' does not match the currently selected one \'{1}\'
        /// </summary>
        ScannedProductCodeDoesNotMatchTheCurrentlySelectedOne = 311,

        /// <summary>
        /// To 
        /// </summary>
        To = 312,

        /// <summary>
        /// Do you wish to delete this record?
        /// </summary>
        DoYouWishToDeleteThisRecord = 313,

        /// <summary>
        /// Scanning an line item detail bar code is not supported here in disconnected mode.
        /// </summary>
        ScanningAnLineItemDetailBarCodeIsNotSupportedHereInDisconnectedModez = 314,

        /// <summary>
        /// Item not found for 
        /// </summary>
        ItemNotFoundFor = 315,

        /// <summary>
        /// Identified line item product does not match the selected skid product.
        /// </summary>
        IdentifiedLineItemProductDoesNotMatchTheSelectedSkidProduct = 316,

        /// <summary>
        /// Product: 
        /// </summary>
        Productzzz = 317,

        /// <summary>
        /// Ordered: 
        /// </summary>
        Orderedzz = 318,

        /// <summary>
        /// Shipped: 
        /// </summary>
        Shippedz = 319,

        /// <summary>
        /// Comment
        /// </summary>
        Comment = 320,

        /// <summary>
        /// DESC: 
        /// </summary>
        Desc = 321,

        /// <summary>
        /// From 
        /// </summary>
        From = 322,

        /// <summary>
        /// Skid
        /// </summary>
        Skidzz = 323,

        /// <summary>
        /// Last Item
        /// </summary>
        LastItem = 324,

        /// <summary>
        /// dMobile Activity Skid Info
        /// </summary>
        DmobileActivitySkidInfo = 325,

        /// <summary>
        /// Do you wish to move any more skid(s)?
        /// </summary>
        DoYouWishToMoveAnyMoreSkids = 326,

        /// <summary>
        /// Product: {0} {1} {2}
        /// </summary>
        Productzzzz = 327,

        /// <summary>
        /// Bar Code: {0}
        /// </summary>
        BarCode = 328,

        /// <summary>
        /// Qty: {0} {1}
        /// </summary>
        Qtyz = 329,

        /// <summary>
        /// Weight: {0} {1}
        /// </summary>
        Weightzz = 330,

        /// <summary>
        /// Ti: {0} Hi: {1}
        /// </summary>
        TiHi = 331,

        /// <summary>
        /// Height: {0} {1}
        /// </summary>
        Heightz = 332,

        /// <summary>
        /// Cube: {0} {1}
        /// </summary>
        Cubez = 333,

        /// <summary>
        /// CHEP: {0}
        /// </summary>
        Chep = 334,

        /// <summary>
        /// Hold: {0}
        /// </summary>
        Hold = 335,

        /// <summary>
        /// Skid does not exist.
        /// </summary>
        SkidDoesNotExist = 336,

        /// <summary>
        /// Product Label Edit
        /// </summary>
        ProductLabelEdit = 337,

        /// <summary>
        /// Print product labels?
        /// </summary>
        PrintProductLabels = 338,

        /// <summary>
        /// Invalid weight quantity.
        /// </summary>
        InvalidWeightQuantity = 339,

        /// <summary>
        /// Serial number required.
        /// </summary>
        SerialNumberRequired = 340,

        /// <summary>
        /// Lot number required.
        /// </summary>
        LotNumberRequired = 341,

        /// <summary>
        /// Variant code required.
        /// </summary>
        VariantCodeRequired = 342,

        /// <summary>
        /// Scanning an Activity Item Reference # Is Not Supported Here
        /// </summary>
        ScanningAnActivityItemReferenceIsNotSupportedHere = 343,

        /// <summary>
        /// Scanning an Activity Detail Reference # Is Not Supported Here
        /// </summary>
        ScanningAnActivityDetailReferenceIsNotSupportedHere = 344,

        /// <summary>
        /// Scanning a shipping container bar code is not supported here!
        /// </summary>
        ScanningAShippingContainerBarCodeIsNotSupportedHere = 345,

        /// <summary>
        /// Scanning a Location Identifier Bar Code Is Not Supported Here
        /// </summary>
        ScanningALocationIdentifierBarCodeIsNotSupportedHere = 346,

        /// <summary>
        /// Bar Code Association
        /// </summary>
        BarCodeAssociation = 347,

        /// <summary>
        /// Loading Work Sheet. Please Wait...
        /// </summary>
        LoadingWorkSheetPleaseWait = 348,

        /// <summary>
        /// You Have To Set Location!
        /// </summary>
        YouHaveToSetLocation = 349,

        /// <summary>
        /// Product associated to bar code
        /// </summary>
        ProductAssociatedToBarCode = 350,

        /// <summary>
        /// Not associated to a product
        /// </summary>
        NotAssociatedToAProduct = 351,

        /// <summary>
        /// Product Associated To Bar Code
        /// </summary>
        ProductAssociatedToBarCodez = 352,

        /// <summary>
        /// Product Deli Scale Amount {0}
        /// </summary>
        ProductDeliScaleAmount = 353,

        /// <summary>
        /// Product Deli Scale Weight {0} {1}
        /// </summary>
        ProductDeliScaleWeight = 354,

        /// <summary>
        /// Product unidentified
        /// </summary>
        ProductUnidentified = 355,

        /// <summary>
        /// SSCC-18: 
        /// </summary>
        Sscc = 356,

        /// <summary>
        /// Code: {0}
        /// </summary>
        Code = 357,

        /// <summary>
        /// Refno: {0}
        /// </summary>
        Refno = 358,

        /// <summary>
        /// Number: {0}
        /// </summary>
        Number = 359,

        /// <summary>
        /// Location: {0}
        /// </summary>
        Locationzz = 360,

        /// <summary>
        /// Product: {0}
        /// </summary>
        Productzzzzz = 361,

        /// <summary>
        /// Qty: {0} {1} @ {2} {3}
        /// </summary>
        Qtyzz = 362,

        /// <summary>
        /// Put Away: {0}
        /// </summary>
        PutAwayz = 363,

        /// <summary>
        /// On Hold: {0}
        /// </summary>
        OnHold = 364,

        /// <summary>
        /// Ti/Hi: {0}/{1}
        /// </summary>
        Tihi = 365,

        /// <summary>
        /// Completed: {0}
        /// </summary>
        Completed = 366,

        /// <summary>
        /// Consumed: {0}
        /// </summary>
        Consumed = 367,

        /// <summary>
        /// Skid unidentified.
        /// </summary>
        SkidUnidentified = 368,

        /// <summary>
        /// Location identifier: 
        /// </summary>
        LocationIdentifier = 369,

        /// <summary>
        /// Warehouse code: {0}
        /// </summary>
        WarehouseCode = 370,

        /// <summary>
        /// Warehouse refno: {0}
        /// </summary>
        WarehouseRefno = 371,

        /// <summary>
        /// Warehouse name: {0}
        /// </summary>
        WarehouseName = 372,

        /// <summary>
        /// Section code: {0}
        /// </summary>
        SectionCode = 373,

        /// <summary>
        /// Section refno: {0}
        /// </summary>
        SectionRefno = 374,

        /// <summary>
        /// Section name: {0}
        /// </summary>
        SectionName = 375,

        /// <summary>
        /// Slot code: {0}
        /// </summary>
        SlotCode = 376,

        /// <summary>
        /// Slot refno: {0}
        /// </summary>
        SlotRefno = 377,

        /// <summary>
        /// Slot name: {0}
        /// </summary>
        SlotName = 378,

        /// <summary>
        /// Not associated to a location.
        /// </summary>
        NotAssociatedToALocation = 379,

        /// <summary>
        /// GTIN: 
        /// </summary>
        Gtin = 380,

        /// <summary>
        /// GTIN in a Logistic Unit: 
        /// </summary>
        GtinInALogisticUnit = 381,

        /// <summary>
        /// Packaging Type: 
        /// </summary>
        PackagingType = 382,

        /// <summary>
        /// Number System Character: 
        /// </summary>
        NumberSystemCharacter = 383,

        /// <summary>
        /// Product Identifier: 
        /// </summary>
        ProductIdentifier = 384,

        /// <summary>
        /// Manufacturer Serial Number: 
        /// </summary>
        ManufacturerSerialNumber = 385,

        /// <summary>
        /// Item Reference: 
        /// </summary>
        ItemReference = 386,

        /// <summary>
        /// (Tracked) Serial #:
        /// </summary>
        TrackedSerial = 387,

        /// <summary>
        /// Serial #:
        /// </summary>
        Serial = 388,

        /// <summary>
        /// (Tracked) Serial # Missing
        /// </summary>
        TrackedSerialMissing = 389,

        /// <summary>
        /// (Tracked) Batch / Lot Number: 
        /// </summary>
        TrackedBatchLotNumber = 390,

        /// <summary>
        /// Batch / Lot Number: 
        /// </summary>
        BatchLotNumber = 391,

        /// <summary>
        /// (Tracked) Batch / Lot Number Missing.
        /// </summary>
        TrackedBatchLotNumberMissing = 392,

        /// <summary>
        /// (Tracked) Production Date: 
        /// </summary>
        TrackedProductionDate = 393,

        /// <summary>
        /// Production Date: 
        /// </summary>
        ProductionDate = 394,

        /// <summary>
        /// (Tracked) Production Date Missing
        /// </summary>
        TrackedProductionDateMissing = 395,

        /// <summary>
        /// (Tracked) Best Before Date: 
        /// </summary>
        TrackedBestBeforeDate = 396,

        /// <summary>
        /// Best Before Date: 
        /// </summary>
        BestBeforeDate = 397,

        /// <summary>
        /// (Tracked) Best Before Date Missing
        /// </summary>
        TrackedBestBeforeDateMissing = 398,

        /// <summary>
        /// Expiry Date: 
        /// </summary>
        ExpiryDate = 399,

        /// <summary>
        /// Due Date: 
        /// </summary>
        DueDate = 400,

        /// <summary>
        /// Packaging Date: 
        /// </summary>
        PackagingDate = 401,

        /// <summary>
        /// (Tracked) Product Variant: 
        /// </summary>
        TrackedProductVariant = 402,

        /// <summary>
        /// Product Variant: 
        /// </summary>
        ProductVariant = 403,

        /// <summary>
        /// (Tracked) Product variant missing.
        /// </summary>
        TrackedProductVariantMissing = 404,

        /// <summary>
        /// Net weight metric KG: 
        /// </summary>
        NetWeightMetricKg = 405,

        /// <summary>
        /// Net weight non-metric LB: 
        /// </summary>
        NetWeightNonmetricLb = 406,

        /// <summary>
        /// Gross weight metric KG: 
        /// </summary>
        GrossWeightMetricKg = 407,

        /// <summary>
        /// Gross weight non-metric LB: 
        /// </summary>
        GrossWeightNonmetricLb = 408,

        /// <summary>
        /// Count of trade items: 
        /// </summary>
        CountOfTradeItems = 409,

        /// <summary>
        /// Variable Count: 
        /// </summary>
        VariableCount = 410,

        /// <summary>
        /// Length Meters: 
        /// </summary>
        LengthMeters = 411,

        /// <summary>
        /// Activity Item FK Ref#: 
        /// </summary>
        ActivityItemFkRef = 412,

        /// <summary>
        /// Activity FK Ref#: 
        /// </summary>
        ActivityFkRef = 413,

        /// <summary>
        /// Activity Detail FK Ref#: 
        /// </summary>
        ActivityDetailFkRef = 414,

        /// <summary>
        /// Unidentifiable Data
        /// </summary>
        UnidentifiableData = 415,

        /// <summary>
        /// Symbology: 
        /// </summary>
        Symbology = 416,

        /// <summary>
        /// Symbology Prefix: 
        /// </summary>
        SymbologyPrefix = 417,

        /// <summary>
        /// Length: {0} prefix excluded
        /// </summary>
        LengthPrefixExcluded = 418,

        /// <summary>
        /// Bar Code Raw
        /// </summary>
        BarCodeRaw = 419,

        /// <summary>
        /// Product associated to bar code.
        /// </summary>
        ProductAssociatedToBarCodezz = 420,

        /// <summary>
        /// Order # 
        /// </summary>
        Orderz = 421,

        /// <summary>
        /// Shipping Label Edit
        /// </summary>
        ShippingLabelEdit = 422,

        /// <summary>
        /// Invalid quantity entered. Repack fresh quantity must be greater than zero.
        /// </summary>
        InvalidQuantityEnteredRepackFreshQuantityMustBeGreaterThanZero = 423,

        /// <summary>
        /// Invalid quantity entered. Repack frozen quantity must be greater than zero.
        /// </summary>
        InvalidQuantityEnteredRepackFrozenQuantityMustBeGreaterThanZero = 424,

        /// <summary>
        /// Invalid quantity entered. Repack live quantity must be greater than zero.
        /// </summary>
        InvalidQuantityEnteredRepackLiveQuantityMustBeGreaterThanZero = 425,

        /// <summary>
        /// Skid Product: 
        /// </summary>
        SkidProduct = 426,

        /// <summary>
        /// Skid Location: 
        /// </summary>
        SkidLocation = 427,

        /// <summary>
        /// Select a different unit of measure?
        /// </summary>
        SelectADifferentUnitOfMeasure = 428,

        /// <summary>
        /// Select UOM
        /// </summary>
        SelectUom = 429,

        /// <summary>
        /// Do you wish to put away another skid?
        /// </summary>
        DoYouWishToPutAwayAnotherSkid = 430,

        /// <summary>
        /// Skid Put Away
        /// </summary>
        SkidPutAway = 431,

        /// <summary>
        /// Total quantity of {0} {2} to be put away does not match the current skid 
        /// </summary>
        TotalQuantityOfToBePutAwayDoesNotMatchTheCurrentSkid = 432,

        /// <summary>
        /// quantity of {1} {2}. Do you wish to continue?
        /// </summary>
        QuantityOfDoYouWishToContinue = 433,

        /// <summary>
        /// Skid put away complete!
        /// </summary>
        SkidPutAwayComplete = 434,

        /// <summary>
        /// Do you wish to put away any more skids?
        /// </summary>
        DoYouWishToPutAwayAnyMoreSkids = 435,

        /// <summary>
        /// Do you wish yo delete this record?
        /// </summary>
        DoYouWishYoDeleteThisRecord = 436,

        /// <summary>
        /// Put Away
        /// </summary>
        PutAwayzz = 437,

        /// <summary>
        /// Communication Setup
        /// </summary>
        CommunicationSetupz = 438,

        /// <summary>
        /// Base port must be between {0} and {1}
        /// </summary>
        BasePortMustBeBetweenAnd = 439,

        /// <summary>
        /// Maximum port must be between {0} and {1}
        /// </summary>
        MaximumPortMustBeBetweenAnd = 440,

        /// <summary>
        /// IP address component must be between {0} and {1}
        /// </summary>
        IpAddressComponentMustBeBetweenAnd = 441,

        /// <summary>
        /// Default port must be between {0} and {1}
        /// </summary>
        DefaultPortMustBeBetweenAnd = 442,

        /// <summary>
        /// Receive buffer size: {0} 
        /// </summary>
        ReceiveBufferSizez = 443,

        /// <summary>
        /// Server host ip: {0}
        /// </summary>
        ServerHostIp = 444,

        /// <summary>
        /// Sending device id.
        /// </summary>
        SendingDeviceId = 445,

        /// <summary>
        /// Code {0}, {1}
        /// </summary>
        Codez = 446,

        /// <summary>
        /// Communication Setup Advanced
        /// </summary>
        CommunicationSetupAdvanced = 447,

        /// <summary>
        /// Data failed validation. Check your data.
        /// </summary>
        DataFailedValidationCheckYourDataz = 448,

        /// <summary>
        /// Connection attempts must be between {0} and {1}
        /// </summary>
        ConnectionAttemptsMustBeBetweenAnd = 449,

        /// <summary>
        /// Receive attempts must be between {0} and {1}
        /// </summary>
        ReceiveAttemptsMustBeBetweenAnd = 450,

        /// <summary>
        /// Receive buffer size must be between {0} and {1}
        /// </summary>
        ReceiveBufferSizeMustBeBetweenAnd = 451,

        /// <summary>
        /// Send attempts must be between {0} and {1}
        /// </summary>
        SendAttemptsMustBeBetweenAnd = 452,

        /// <summary>
        /// Send time out seconds must be between {0} and {1}
        /// </summary>
        SendTimeOutSecondsMustBeBetweenAnd = 453,

        /// <summary>
        /// Please log back in for changes to be made in effect.
        /// </summary>
        PleaseLogBackInForChangesToBeMadeInEffect = 454,

        /// <summary>
        /// Refresh Data Options
        /// </summary>
        RefreshDataOptions = 455,

        /// <summary>
        /// Warning! Unsubmitted data exists and will be lost. Continue with refresh data?
        /// </summary>
        WarningUnsubmittedDataExistsAndWillBeLostContinueWithRefreshData = 456,

        /// <summary>
        /// Confirm refresh data?
        /// </summary>
        ConfirmRefreshData = 457,

        /// <summary>
        /// dMobile Purge Activity Data
        /// </summary>
        DmobilePurgeActivityData = 458,

        /// <summary>
        /// dMobile Refresh Data Status
        /// </summary>
        DmobileRefreshDataStatus = 459,

        /// <summary>
        /// Data status failed.
        /// </summary>
        DataStatusFailed = 460,

        /// <summary>
        /// Setup Defaults
        /// </summary>
        SetupDefaultsz = 461,

        /// <summary>
        /// Purge Data Completed
        /// </summary>
        PurgeDataCompleted = 462,

        /// <summary>
        /// Refresh Data Completed
        /// </summary>
        RefreshDataCompleted = 463,

        /// <summary>
        /// Refresh Minutes {0}
        /// </summary>
        RefreshMinutes = 464,

        /// <summary>
        /// Refresh data failed.
        /// </summary>
        RefreshDataFailed = 465,

        /// <summary>
        /// Found 
        /// </summary>
        Found = 466,

        /// <summary>
        /// Seeding system users
        /// </summary>
        SeedingSystemUsersz = 467,

        /// <summary>
        /// Some expected data was not refreshed. Recommend refreshing data again.
        /// </summary>
        SomeExpectedDataWasNotRefreshedRecommendRefreshingDataAgain = 468,

        /// <summary>
        /// Last refresh 
        /// </summary>
        LastRefresh = 469,

        /// <summary>
        /// Refresh changes only 
        /// </summary>
        RefreshChangesOnly = 470,

        /// <summary>
        /// dMobile Submit Batch
        /// </summary>
        DmobileSubmitBatch = 471,

        /// <summary>
        /// Error 
        /// </summary>
        Error = 472,

        /// <summary>
        /// Submit Batch
        /// </summary>
        SubmitBatch = 473,

        /// <summary>
        /// Submission Complete.
        /// </summary>
        SubmissionComplete = 474,

        /// <summary>
        /// All
        /// </summary>
        All = 475,

        /// <summary>
        /// Inserted
        /// </summary>
        Inserted = 476,

        /// <summary>
        /// Updated
        /// </summary>
        Updated = 477,

        /// <summary>
        /// Deleted
        /// </summary>
        Deleted = 478,

        /// <summary>
        /// Returns
        /// </summary>
        Returnsz = 479,

        /// <summary>
        /// Cycle Counts
        /// </summary>
        CycleCounts = 480,

        /// <summary>
        /// Physical Counts
        /// </summary>
        PhysicalCounts = 481,

        /// <summary>
        /// Ticket Finished
        /// </summary>
        TicketFinishedz = 482,

        /// <summary>
        /// Ticket Material
        /// </summary>
        TicketMaterialz = 483,

        /// <summary>
        /// Ticket Add-Product
        /// </summary>
        TicketAddproduct = 484,

        /// <summary>
        /// Co-Product Add-Product
        /// </summary>
        CoproductAddproduct = 485,

        /// <summary>
        /// PO Work Sheet
        /// </summary>
        PoWorkSheet = 486,

        /// <summary>
        /// Submit Batch Filter
        /// </summary>
        SubmitBatchFilter = 487,

        /// <summary>
        /// The current and the new default picking locations are the same.
        /// </summary>
        TheCurrentAndTheNewDefaultPickingLocationsAreTheSame = 488,

        /// <summary>
        /// Are you sure that you want to update the location anyway?
        /// </summary>
        AreYouSureThatYouWantToUpdateTheLocationAnyway = 489,

        /// <summary>
        /// Ready To Save
        /// </summary>
        ReadyToSave = 490,

        /// <summary>
        /// The current and the new default receiving locations are the same.
        /// </summary>
        TheCurrentAndTheNewDefaultReceivingLocationsAreTheSame = 491,

        /// <summary>
        /// Assign Default Location
        /// </summary>
        AssignDefaultLocationz = 492,

        /// <summary>
        /// Product default location(s) saved.
        /// </summary>
        ProductDefaultLocationsSaved = 493,

        /// <summary>
        /// Your location must be set.
        /// </summary>
        YourLocationMustBeSet = 494,

        /// <summary>
        /// Please set the from product unit first.
        /// </summary>
        PleaseSetTheFromProductUnitFirst = 495,

        /// <summary>
        /// {0} {1}\'S ({2} per {3})
        /// </summary>
        SPer = 496,

        /// <summary>
        /// Invalid quantity!
        /// </summary>
        InvalidQuantityz = 497,

        /// <summary>
        /// Invalid weight quantity!
        /// </summary>
        InvalidWeightQuantityz = 498,

        /// <summary>
        /// PROD: 
        /// </summary>
        Prod = 499,

        /// <summary>
        /// Unexpected print product label(s) error.
        /// </summary>
        UnexpectedPrintProductLabelsError = 500,

        /// <summary>
        /// Printing Label(s)
        /// </summary>
        PrintingLabels = 501,

        /// <summary>
        /// Print product label(s) failed. 
        /// </summary>
        PrintProductLabelsFailed = 502,

        /// <summary>
        /// Print Label(s)
        /// </summary>
        PrintLabels = 503,

        /// <summary>
        /// Location must be set first.
        /// </summary>
        LocationMustBeSetFirst = 504,

        /// <summary>
        /// Add the ENTIRE skid?
        /// </summary>
        AddTheEntireSkid = 505,

        /// <summary>
        /// Do you wish to delete this entire skid \'{0}\'?
        /// </summary>
        DoYouWishToDeleteThisEntireSkid = 506,

        /// <summary>
        /// Scanning a header reference # is not supported here.
        /// </summary>
        ScanningAHeaderReferenceIsNotSupportedHere = 507,

        /// <summary>
        /// Scanning an item reference # is not supported here.
        /// </summary>
        ScanningAnItemReferenceIsNotSupportedHere = 508,

        /// <summary>
        /// Scanning a detail reference # is not supported here.
        /// </summary>
        ScanningADetailReferenceIsNotSupportedHere = 509,

        /// <summary>
        /// Scanning a shipping container bar code is not enabled.
        /// </summary>
        ScanningAShippingContainerBarCodeIsNotEnabled = 510,

        /// <summary>
        /// Skid # 
        /// </summary>
        Skidzzz = 511,

        /// <summary>
        /// Scanned bar code type is not supported here.
        /// </summary>
        ScannedBarCodeTypeIsNotSupportedHere = 512,

        /// <summary>
        /// Loading item detail(s). Please wait...
        /// </summary>
        LoadingItemDetailsPleaseWait = 513,

        /// <summary>
        /// Total: 
        /// </summary>
        Totalz = 514,

        /// <summary>
        /// Detail Edit
        /// </summary>
        DetailEdit = 515,

        /// <summary>
        ///  is required.
        /// </summary>
        IsRequired = 516,

        /// <summary>
        /// Inventory Inquiry Detail
        /// </summary>
        InventoryInquiryDetail = 517,

        /// <summary>
        /// Loading details. Please wait...
        /// </summary>
        LoadingDetailsPleaseWait = 518,

        /// <summary>
        /// Skid Detail
        /// </summary>
        SkidDetail = 519,

        /// <summary>
        /// Loading Item Detail(s). Please Wait...
        /// </summary>
        LoadingItemDetailsPleaseWaitz = 520,

        /// <summary>
        /// Do You Wish To Remove This Record From This Skid?
        /// </summary>
        DoYouWishToRemoveThisRecordFromThisSkid = 521,

        /// <summary>
        /// This skid is now consumed.
        /// </summary>
        ThisSkidIsNowConsumed = 522,

        /// <summary>
        /// Confirm exit?
        /// </summary>
        ConfirmExit = 523,

        /// <summary>
        /// Loading header(s). Please wait...
        /// </summary>
        LoadingHeadersPleaseWait = 524,

        /// <summary>
        /// dMobile Activity Adjustment Filter
        /// </summary>
        DmobileActivityAdjustmentFilter = 525,

        /// <summary>
        /// Cycle Count Filter
        /// </summary>
        CycleCountFilter = 526,

        /// <summary>
        /// dMobile Activity Physical Count Filter
        /// </summary>
        DmobileActivityPhysicalCountFilter = 527,

        /// <summary>
        /// Picking Filter
        /// </summary>
        PickingFilter = 528,

        /// <summary>
        /// Picking Selection Criteria
        /// </summary>
        PickingSelectionCriteria = 529,

        /// <summary>
        /// All orders for all sections will be loaded. Continue?
        /// </summary>
        AllOrdersForAllSectionsWillBeLoadedContinue = 530,

        /// <summary>
        /// Select the warehouse first!
        /// </summary>
        SelectTheWarehouseFirst = 531,

        /// <summary>
        /// Code
        /// </summary>
        Codezz = 532,

        /// <summary>
        /// Name
        /// </summary>
        Name = 533,

        /// <summary>
        /// LocationMapRefno
        /// </summary>
        Locationmaprefno = 534,

        /// <summary>
        /// dMobile Activity Production Filter
        /// </summary>
        DmobileActivityProductionFilter = 535,

        /// <summary>
        /// dMobile Activity Receiving Filter
        /// </summary>
        DmobileActivityReceivingFilter = 536,

        /// <summary>
        /// Change Status
        /// </summary>
        ChangeStatus = 537,

        /// <summary>
        /// Unable To Find Reference# {0}
        /// </summary>
        UnableToFindReference = 538,

        /// <summary>
        /// Unable To Parse Reference# {0}
        /// </summary>
        UnableToParseReference = 539,

        /// <summary>
        /// Scanning an activity item reference # is not supported here.
        /// </summary>
        ScanningAnActivityItemReferenceIsNotSupportedHerez = 540,

        /// <summary>
        /// Scanning a product bar code is not dupported here.
        /// </summary>
        ScanningAProductBarCodeIsNotDupportedHere = 541,

        /// <summary>
        /// Scanning a location identifier bar code is not supported here.
        /// </summary>
        ScanningALocationIdentifierBarCodeIsNotSupportedHerez = 542,

        /// <summary>
        /// Returns Filter
        /// </summary>
        ReturnsFilter = 543,

        /// <summary>
        /// Permission denied.
        /// </summary>
        PermissionDenied = 544,

        /// <summary>
        /// Add Manual Detail Override
        /// </summary>
        AddManualDetailOverride = 545,

        /// <summary>
        /// Bar Code
        /// </summary>
        BarCodez = 546,

        /// <summary>
        /// Select Location
        /// </summary>
        SelectLocation = 547,

        /// <summary>
        /// Unable to detect the OS platform or minimize the taskbar.
        /// </summary>
        UnableToDetectTheOsPlatformOrMinimizeTheTaskbar = 548,

        /// <summary>
        /// An instance of dMobile is already running.
        /// </summary>
        AnInstanceOfDmobileIsAlreadyRunning = 549,

        /// <summary>
        /// Crd1: 
        /// </summary>
        Crd = 550,

        /// <summary>
        /// The last data refresh did not complete. You must refresh the data again.
        /// </summary>
        TheLastDataRefreshDidNotCompleteYouMustRefreshTheDataAgain = 551,

        /// <summary>
        /// Loading Skid(s). Please Wait...
        /// </summary>
        LoadingSkidsPleaseWait = 552,

        /// <summary>
        /// Select the section first!
        /// </summary>
        SelectTheSectionFirst = 553,

        /// <summary>
        /// Crd2: 
        /// </summary>
        Crdz = 554,

        /// <summary>
        /// Crd3: 
        /// </summary>
        Crdzz = 555,

        /// <summary>
        /// CoordinateRefno
        /// </summary>
        Coordinaterefno = 556,

        /// <summary>
        /// Scanning an header reference # is not supported here!
        /// </summary>
        ScanningAnHeaderReferenceIsNotSupportedHere = 557,

        /// <summary>
        /// Scanning an item reference # is not supported here!
        /// </summary>
        ScanningAnItemReferenceIsNotSupportedHerez = 558,

        /// <summary>
        /// Scanning a product bar code is not supported here!
        /// </summary>
        ScanningAProductBarCodeIsNotSupportedHere = 559,

        /// <summary>
        /// Scanned bar code type is not supported here!
        /// </summary>
        ScannedBarCodeTypeIsNotSupportedHerez = 560,

        /// <summary>
        /// Convert from product must be set.
        /// </summary>
        ConvertFromProductMustBeSet = 561,

        /// <summary>
        /// No convert to items have be defined.
        /// </summary>
        NoConvertToItemsHaveBeDefined = 562,

        /// <summary>
        /// No convert to item details have be defined.
        /// </summary>
        NoConvertToItemDetailsHaveBeDefined = 563,

        /// <summary>
        /// Scanning an line item detail bar code is not supported here!
        /// </summary>
        ScanningAnLineItemDetailBarCodeIsNotSupportedHere = 564,

        /// <summary>
        /// Location Must Be Set First
        /// </summary>
        LocationMustBeSetFirstz = 565,

        /// <summary>
        /// Unexpected confirm detail error.
        /// </summary>
        UnexpectedConfirmDetailError = 566,

        /// <summary>
        /// Scanned product does not match the selected product.
        /// </summary>
        ScannedProductDoesNotMatchTheSelectedProduct = 567,

        /// <summary>
        /// Identified line item product does not match the selected product.
        /// </summary>
        IdentifiedLineItemProductDoesNotMatchTheSelectedProduct = 568,

        /// <summary>
        /// Skid warehouse does not match the selected warehouse.
        /// </summary>
        SkidWarehouseDoesNotMatchTheSelectedWarehouse = 569,

        /// <summary>
        /// Skid product does not match the line item product.
        /// </summary>
        SkidProductDoesNotMatchTheLineItemProduct = 570,

        /// <summary>
        /// PRODUCT: 
        /// </summary>
        Productzzzzzz = 571,

        /// <summary>
        /// OK
        /// </summary>
        Ok = 572,

        /// <summary>
        /// Continue
        /// </summary>
        Continuez = 573,

        /// <summary>
        /// Send Report
        /// </summary>
        SendReport = 574,

        /// <summary>
        /// Support Detail
        /// </summary>
        SupportDetail = 575,

        /// <summary>
        /// 3. Slot
        /// </summary>
        Slotz = 576,

        /// <summary>
        /// Coord 3
        /// </summary>
        Coord = 577,

        /// <summary>
        /// Coord 2
        /// </summary>
        Coordz = 578,

        /// <summary>
        /// Coord 1
        /// </summary>
        Coordzz = 579,

        /// <summary>
        /// Print Label
        /// </summary>
        PrintLabel = 580,

        /// <summary>
        /// Skid product must match the selected line item product.
        /// </summary>
        SkidProductMustMatchTheSelectedLineItemProduct = 581,

        /// <summary>
        /// Invalid quantity.
        /// </summary>
        InvalidQuantityzz = 582,

        /// <summary>
        /// The sign of quantities must be the same.
        /// </summary>
        TheSignOfQuantitiesMustBeTheSame = 583,

        /// <summary>
        /// Both quantity and weight quantity cannot be zero.
        /// </summary>
        BothQuantityAndWeightQuantityCannotBeZero = 584,

        /// <summary>
        /// Best before date required.
        /// </summary>
        BestBeforeDateRequired = 585,

        /// <summary>
        /// The entered best before date is outside an acceptable range.
        /// </summary>
        TheEnteredBestBeforeDateIsOutsideAnAcceptableRange = 586,

        /// <summary>
        /// Production date required.
        /// </summary>
        ProductionDateRequired = 587,

        /// <summary>
        /// The entered production date is outside an acceptable range.
        /// </summary>
        TheEnteredProductionDateIsOutsideAnAcceptableRange = 588,

        /// <summary>
        /// Scanning an activity reference # is not supported here.
        /// </summary>
        ScanningAnActivityReferenceIsNotSupportedHere = 589,

        /// <summary>
        /// Scanning an product identifier is not supported here.
        /// </summary>
        ScanningAnProductIdentifierIsNotSupportedHere = 590,

        /// <summary>
        /// Scanning a shipping container bar code is not supported here.
        /// </summary>
        ScanningAShippingContainerBarCodeIsNotSupportedHerez = 591,

        /// <summary>
        /// Scanning a global location identifier is not supported here.
        /// </summary>
        ScanningAGlobalLocationIdentifierIsNotSupportedHere = 592,

        /// <summary>
        /// Adding product to a skid can only be done in skid maintenance.
        /// </summary>
        AddingProductToASkidCanOnlyBeDoneInSkidMaintenance = 593,

        /// <summary>
        /// Scanning a location code is not supported here.
        /// </summary>
        ScanningALocationCodeIsNotSupportedHere = 594,

        /// <summary>
        /// Scanning an activity header reference number is not supported here.
        /// </summary>
        ScanningAnActivityHeaderReferenceNumberIsNotSupportedHere = 595,

        /// <summary>
        /// Scanning an activity item reference number is not supported here.
        /// </summary>
        ScanningAnActivityItemReferenceNumberIsNotSupportedHere = 596,

        /// <summary>
        /// Scanning an shipping container code is not supported here.
        /// </summary>
        ScanningAnShippingContainerCodeIsNotSupportedHere = 597,

        /// <summary>
        /// Scanned product does not match selected skid product.
        /// </summary>
        ScannedProductDoesNotMatchSelectedSkidProduct = 598,

        /// <summary>
        /// Unable to find reference # {0}
        /// </summary>
        UnableToFindReferencez = 599,

        /// <summary>
        /// Unable to parse reference # {0}
        /// </summary>
        UnableToParseReferencez = 600,

        /// <summary>
        /// Scanning an activity item reference # is not supported here!
        /// </summary>
        ScanningAnActivityItemReferenceIsNotSupportedHerezz = 601,

        /// <summary>
        /// Scanning a location identifier bar code is not supported here!
        /// </summary>
        ScanningALocationIdentifierBarCodeIsNotSupportedHerezz = 602,

        /// <summary>
        /// WHS:  
        /// </summary>
        Whsz = 603,

        /// <summary>
        /// CUSTOMER: 
        /// </summary>
        Customer = 604,

        /// <summary>
        /// SHIPTO:   
        /// </summary>
        Shipto = 605,

        /// <summary>
        /// VENDOR:  
        /// </summary>
        Vendor = 606,

        /// <summary>
        /// SUPPLIER:  
        /// </summary>
        Supplier = 607,

        /// <summary>
        /// TO WHS:  
        /// </summary>
        ToWhs = 608,

        /// <summary>
        /// Please select a record to release.
        /// </summary>
        PleaseSelectARecordToRelease = 609,

        /// <summary>
        /// LocationMapWarehouseName
        /// </summary>
        Locationmapwarehousename = 610,

        /// <summary>
        /// ActivityPickingCustomerContactName
        /// </summary>
        Activitypickingcustomercontactname = 611,

        /// <summary>
        /// ActivityPickingCustomerShipToInfo
        /// </summary>
        Activitypickingcustomershiptoinfo = 612,

        /// <summary>
        /// ActivityReceivingVendorContactName
        /// </summary>
        Activityreceivingvendorcontactname = 613,

        /// <summary>
        /// ActivityReceivingVendorSupplierContactName
        /// </summary>
        Activityreceivingvendorsuppliercontactname = 614,

        /// <summary>
        /// ActivityReturnsCustomerContactName
        /// </summary>
        Activityreturnscustomercontactname = 615,

        /// <summary>
        /// ActivityReturnsCustomerShipToInfo
        /// </summary>
        Activityreturnscustomershiptoinfo = 616,

        /// <summary>
        /// Do you wish to clear the current picker?
        /// </summary>
        DoYouWishToClearTheCurrentPicker = 617,

        /// <summary>
        /// Picker Override
        /// </summary>
        PickerOverride = 618,

        /// <summary>
        /// Picker cleared.
        /// </summary>
        PickerCleared = 619,

        /// <summary>
        /// Unable to clear the current picker.
        /// </summary>
        UnableToClearTheCurrentPicker = 620,

        /// <summary>
        /// Scanning an activity header reference # is not supported here.
        /// </summary>
        ScanningAnActivityHeaderReferenceIsNotSupportedHere = 621,

        /// <summary>
        /// Scanning a product bar code is not supported here.
        /// </summary>
        ScanningAProductBarCodeIsNotSupportedHerez = 622,

        /// <summary>
        /// The skid location must be set.
        /// </summary>
        TheSkidLocationMustBeSet = 623,

        /// <summary>
        /// A product must be assigned to a skid.
        /// </summary>
        AProductMustBeAssignedToASkid = 624,

        /// <summary>
        /// Making a new skid has been disabled.
        /// </summary>
        MakingANewSkidHasBeenDisabled = 625,

        /// <summary>
        /// Scanning a location identifier can only be done with a new skid.
        /// </summary>
        ScanningALocationIdentifierCanOnlyBeDoneWithANewSkid = 626,

        /// <summary>
        /// Scanning a product identifier can only be done with a new skid.
        /// </summary>
        ScanningAProductIdentifierCanOnlyBeDoneWithANewSkid = 627,

        /// <summary>
        /// The set location must have a warehouse selected.
        /// </summary>
        TheSetLocationMustHaveAWarehouseSelected = 628,

        /// <summary>
        /// The set location must have a section selected.
        /// </summary>
        TheSetLocationMustHaveASectionSelected = 629,

        /// <summary>
        /// Print all skid label(s) error.
        /// </summary>
        PrintAllSkidLabelsError = 630,

        /// <summary>
        /// Check skids to print error.
        /// </summary>
        CheckSkidsToPrintError = 631,

        /// <summary>
        /// Items exist that have not been reviewed.
        /// </summary>
        ItemsExistThatHaveNotBeenReviewed = 632,

        /// <summary>
        /// The quantity shipped is less than the quantity ordered.
        /// </summary>
        TheQuantityShippedIsLessThanTheQuantityOrdered = 633,

        /// <summary>
        /// On Hand {0} {1}
        /// </summary>
        OnHand = 634,

        /// <summary>
        /// Product rule says that it must exist on this document already.
        /// </summary>
        ProductRuleSaysThatItMustExistOnThisDocumentAlready = 635,

        /// <summary>
        /// The current location does not match the product home location.
        /// </summary>
        TheCurrentLocationDoesNotMatchTheProductHomeLocation = 636,

        /// <summary>
        /// Selected line item product must match the selected skid product.
        /// </summary>
        SelectedLineItemProductMustMatchTheSelectedSkidProduct = 637,

        /// <summary>
        /// Please select and line item to show the comments for.
        /// </summary>
        PleaseSelectAndLineItemToShowTheCommentsFor = 638,

        /// <summary>
        /// Location must be set first!
        /// </summary>
        LocationMustBeSetFirstzz = 639,

        /// <summary>
        /// Line item not found.
        /// </summary>
        LineItemNotFound = 640,

        /// <summary>
        /// Clear Picker
        /// </summary>
        ClearPicker = 641,

        /// <summary>
        /// Line item was not found for 
        /// </summary>
        LineItemWasNotFoundFor = 642,

        /// <summary>
        /// Location {0} 
        /// </summary>
        Locationzzz = 643,

        /// <summary>
        /// Product {1} BC {2} UOM {3}
        /// </summary>
        ProductBcUom = 644,

        /// <summary>
        /// Scanning an line item bar code is not supported here.
        /// </summary>
        ScanningAnLineItemBarCodeIsNotSupportedHere = 645,

        /// <summary>
        /// Scanning an line item detail bar code is not supported here.
        /// </summary>
        ScanningAnLineItemDetailBarCodeIsNotSupportedHerez = 646,

        /// <summary>
        /// Product {1} BC {2} Unit {3}
        /// </summary>
        ProductBcUnit = 647,

        /// <summary>
        /// Line item was not found.
        /// </summary>
        LineItemWasNotFound = 648,

        /// <summary>
        /// The location must be set.
        /// </summary>
        TheLocationMustBeSet = 649,

        /// <summary>
        /// Invalid Move \'From\' Location
        /// </summary>
        InvalidMoveFromLocation = 650,

        /// <summary>
        /// Send Moves
        /// </summary>
        SendMoves = 651,

        /// <summary>
        /// Invalid Move \'To\' Location
        /// </summary>
        InvalidMoveToLocation = 652,

        /// <summary>
        /// The \'From\' And \'To\' Warehouse(s) Must Be The Same
        /// </summary>
        TheFromAndToWarehousesMustBeTheSame = 653,

        /// <summary>
        /// Submit Move Failed
        /// </summary>
        SubmitMoveFailed = 654,

        /// <summary>
        /// Move Submission
        /// </summary>
        MoveSubmission = 655,

        /// <summary>
        /// Scanning an header reference # is not supported here.
        /// </summary>
        ScanningAnHeaderReferenceIsNotSupportedHerez = 656,

        /// <summary>
        /// Scanning a shipping container bar code # is not supported here.
        /// </summary>
        ScanningAShippingContainerBarCodeIsNotSupportedHerezz = 657,

        /// <summary>
        /// Scanning bar code type is not supported here.
        /// </summary>
        ScanningBarCodeTypeIsNotSupportedHere = 658,

        /// <summary>
        /// Scanning a line item bar code is not supported here!
        /// </summary>
        ScanningALineItemBarCodeIsNotSupportedHere = 659,

        /// <summary>
        /// Item Not Found For 
        /// </summary>
        ItemNotFoundForz = 660,

        /// <summary>
        /// Item Not Found.
        /// </summary>
        ItemNotFound = 661,

        /// <summary>
        /// Unable To Indentify Item
        /// </summary>
        UnableToIndentifyItem = 662,

        /// <summary>
        /// Scanned Bar Code Type Is Not Supported Here
        /// </summary>
        ScannedBarCodeTypeIsNotSupportedHerezz = 663,

        /// <summary>
        /// Activity Item
        /// </summary>
        ActivityItem = 664,

        /// <summary>
        /// Scanned Product Bar Code Is Not Associated To a Product.
        /// </summary>
        ScannedProductBarCodeIsNotAssociatedToAProduct = 665,

        /// <summary>
        /// Do You Wish To Associate It Now? 
        /// </summary>
        DoYouWishToAssociateItNow = 666,

        /// <summary>
        /// Refresh
        /// </summary>
        Refresh = 667,

        /// <summary>
        /// Unable to Identify Activity Items
        /// </summary>
        UnableToIdentifyActivityItems = 668,

        /// <summary>
        /// Activity Work Sheet
        /// </summary>
        ActivityWorkSheet = 669,

        /// <summary>
        /// Unable to locate the product scheme for product \'{0}\'
        /// </summary>
        UnableToLocateTheProductSchemeForProduct = 670,

        /// <summary>
        /// Not associated to a skid.
        /// </summary>
        NotAssociatedToASkid = 671,

        /// <summary>
        /// Invalid full case quantity entered!
        /// </summary>
        InvalidFullCaseQuantityEntered = 672,

        /// <summary>
        /// Invalid quantity entered. Full case quantity must be greater than zero.
        /// </summary>
        InvalidQuantityEnteredFullCaseQuantityMustBeGreaterThanZero = 673,

        /// <summary>
        /// Invalid repack quantity entered!
        /// </summary>
        InvalidRepackQuantityEntered = 674,

        /// <summary>
        /// Invalid quantity entered. Repack quantity must be greater than zero.
        /// </summary>
        InvalidQuantityEnteredRepackQuantityMustBeGreaterThanZero = 675,

        /// <summary>
        /// Invalid dry quantity entered!
        /// </summary>
        InvalidDryQuantityEntered = 676,

        /// <summary>
        /// Invalid quantity entered. Dry quantity must be greater than zero.
        /// </summary>
        InvalidQuantityEnteredDryQuantityMustBeGreaterThanZero = 677,

        /// <summary>
        /// Invalid fresh quantity entered!
        /// </summary>
        InvalidFreshQuantityEntered = 678,

        /// <summary>
        /// Invalid quantity entered. Fresh quantity must be greater than zero.
        /// </summary>
        InvalidQuantityEnteredFreshQuantityMustBeGreaterThanZero = 679,

        /// <summary>
        /// Invalid frozen quantity entered!
        /// </summary>
        InvalidFrozenQuantityEntered = 680,

        /// <summary>
        /// Invalid quantity entered. Frozen quantity must be greater than zero.
        /// </summary>
        InvalidQuantityEnteredFrozenQuantityMustBeGreaterThanZero = 681,

        /// <summary>
        /// Invalid live quantity entered!
        /// </summary>
        InvalidLiveQuantityEntered = 682,

        /// <summary>
        /// Invalid quantity entered. Live quantity must be greater than zero.
        /// </summary>
        InvalidQuantityEnteredLiveQuantityMustBeGreaterThanZero = 683,

        /// <summary>
        /// Invalid repack dry quantity entered!
        /// </summary>
        InvalidRepackDryQuantityEntered = 684,

        /// <summary>
        /// Invalid quantity entered. Repack dry quantity must be greater than zero.
        /// </summary>
        InvalidQuantityEnteredRepackDryQuantityMustBeGreaterThanZero = 685,

        /// <summary>
        /// Invalid repack fresh quantity entered!
        /// </summary>
        InvalidRepackFreshQuantityEntered = 686,

        /// <summary>
        /// Invalid repack frozen quantity entered!
        /// </summary>
        InvalidRepackFrozenQuantityEntered = 687,

        /// <summary>
        /// Invalid repack live quantity entered!
        /// </summary>
        InvalidRepackLiveQuantityEntered = 688,

        /// <summary>
        /// Invalid pallet quantity entered!
        /// </summary>
        InvalidPalletQuantityEntered = 689,

        /// <summary>
        /// Invalid quantity entered. Pallet quantity must be greater than zero.
        /// </summary>
        InvalidQuantityEnteredPalletQuantityMustBeGreaterThanZero = 690,

        /// <summary>
        /// A skid must be selected first before scanning any product detail.
        /// </summary>
        ASkidMustBeSelectedFirstBeforeScanningAnyProductDetail = 691,

        /// <summary>
        /// Identified scanned product does not match the selected skid product.
        /// </summary>
        IdentifiedScannedProductDoesNotMatchTheSelectedSkidProduct = 692,

        /// <summary>
        /// Confirm disconnected mode changes?
        /// </summary>
        ConfirmDisconnectedModeChanges = 693,

        /// <summary>
        /// This will purge any unsubmitted data.
        /// </summary>
        ThisWillPurgeAnyUnsubmittedData = 694,

        /// <summary>
        /// Warehouse is required.
        /// </summary>
        WarehouseIsRequired = 695,

        /// <summary>
        /// Section is required.
        /// </summary>
        SectionIsRequired = 696,

        /// <summary>
        /// Slot is required.
        /// </summary>
        SlotIsRequired = 697,

        /// <summary>
        /// A selected product is required.
        /// </summary>
        ASelectedProductIsRequired = 698,

        /// <summary>
        /// A selected default picking or receiving option is required.
        /// </summary>
        ASelectedDefaultPickingOrReceivingOptionIsRequired = 699,

        /// <summary>
        /// Assign Default Product Location failed.
        /// </summary>
        AssignDefaultProductLocationFailed = 700,

        /// <summary>
        /// Scanning a detail bar code is not supported here!
        /// </summary>
        ScanningADetailBarCodeIsNotSupportedHere = 701,

        /// <summary>
        /// Scanning an shipping container bar code is not supported here!
        /// </summary>
        ScanningAnShippingContainerBarCodeIsNotSupportedHere = 702,

        /// <summary>
        /// Scanning a global location identifier is not supported here!
        /// </summary>
        ScanningAGlobalLocationIdentifierIsNotSupportedHerez = 703,

        /// <summary>
        /// Scanning an activity bar code is not supported here!
        /// </summary>
        ScanningAnActivityBarCodeIsNotSupportedHere = 704,

        /// <summary>
        /// Scanning a line item detail bar code is not supported here!
        /// </summary>
        ScanningALineItemDetailBarCodeIsNotSupportedHere = 705,

        /// <summary>
        /// Scanning an product code is not supported here.
        /// </summary>
        ScanningAnProductCodeIsNotSupportedHere = 706,

        /// <summary>
        /// Finished Moves?
        /// </summary>
        FinishedMoves = 707,

        /// <summary>
        ///       Save        
        /// </summary>
        Savez = 708,

        /// <summary>
        ///        Cancel     
        /// </summary>
        Cancelz = 709,

        /// <summary>
        ///     Save    
        /// </summary>
        Savezz = 710,

        /// <summary>
        ///    Cancel   
        /// </summary>
        Cancelzz = 711,

        /// <summary>
        /// Identify Product
        /// </summary>
        IdentifyProduct = 712,

        /// <summary>
        /// Scanned product bar code is not associated to a product.
        /// </summary>
        ScannedProductBarCodeIsNotAssociatedToAProductz = 713,

        /// <summary>
        /// Do you wish to associate it now? 
        /// </summary>
        DoYouWishToAssociateItNowz = 714,

        /// <summary>
        /// Product not identified.
        /// </summary>
        ProductNotIdentified = 715,

        /// <summary>
        /// Cancelled from detail edit.
        /// </summary>
        CancelledFromDetailEdit = 716,

        /// <summary>
        /// Unable to identify product scanned.
        /// </summary>
        UnableToIdentifyProductScanned = 717,

        /// <summary>
        /// Seeding bar code settings.
        /// </summary>
        SeedingBarCodeSettings = 718,

        /// <summary>
        /// Seeding bar code settings complete.
        /// </summary>
        SeedingBarCodeSettingsComplete = 719,

        /// <summary>
        /// Seeding bar code settings failed.
        /// </summary>
        SeedingBarCodeSettingsFailed = 720,

        /// <summary>
        /// Reading received data.
        /// </summary>
        ReadingReceivedData = 721,

        /// <summary>
        /// Saving bar code settings.
        /// </summary>
        SavingBarCodeSettings = 722,

        /// <summary>
        /// Records {0}.
        /// </summary>
        Records = 723,

        /// <summary>
        /// Saving bar code settings complete.
        /// </summary>
        SavingBarCodeSettingsComplete = 724,

        /// <summary>
        /// Saving bar code settings failed.
        /// </summary>
        SavingBarCodeSettingsFailed = 725,

        /// <summary>
        /// Seeding printer settings.
        /// </summary>
        SeedingPrinterSettings = 726,

        /// <summary>
        /// Seeding printer settings complete.
        /// </summary>
        SeedingPrinterSettingsComplete = 727,

        /// <summary>
        /// Seeding printer settings failed.
        /// </summary>
        SeedingPrinterSettingsFailed = 728,

        /// <summary>
        /// Saving printer settings.
        /// </summary>
        SavingPrinterSettings = 729,

        /// <summary>
        /// Records {0}
        /// </summary>
        Recordsz = 730,

        /// <summary>
        /// Saving printer settings complete.
        /// </summary>
        SavingPrinterSettingsComplete = 731,

        /// <summary>
        /// Seeding product scheme label settings.
        /// </summary>
        SeedingProductSchemeLabelSettings = 732,

        /// <summary>
        /// Seeding product scheme label settings complete.
        /// </summary>
        SeedingProductSchemeLabelSettingsComplete = 733,

        /// <summary>
        /// Seeding product scheme label settings failed.
        /// </summary>
        SeedingProductSchemeLabelSettingsFailed = 734,

        /// <summary>
        /// Saving product scheme label settings.
        /// </summary>
        SavingProductSchemeLabelSettings = 735,

        /// <summary>
        /// Saving product scheme label settings complete.
        /// </summary>
        SavingProductSchemeLabelSettingsComplete = 736,

        /// <summary>
        /// Database exception occurred.
        /// </summary>
        DatabaseExceptionOccurred = 737,

        /// <summary>
        /// Saving product scheme label settings failed.
        /// </summary>
        SavingProductSchemeLabelSettingsFailed = 738,

        /// <summary>
        /// Database exception occurred. Saving product scheme label settings failed.
        /// </summary>
        DatabaseExceptionOccurredSavingProductSchemeLabelSettingsFailed = 739,

        /// <summary>
        /// Seeding system settings.
        /// </summary>
        SeedingSystemSettings = 740,

        /// <summary>
        /// Seeding system settings complete.
        /// </summary>
        SeedingSystemSettingsComplete = 741,

        /// <summary>
        /// Seeding system settings failed.
        /// </summary>
        SeedingSystemSettingsFailed = 742,

        /// <summary>
        /// Saving system settings.
        /// </summary>
        SavingSystemSettings = 743,

        /// <summary>
        /// Saving system settings complete.
        /// </summary>
        SavingSystemSettingsComplete = 744,

        /// <summary>
        /// Saving system settings failed.
        /// </summary>
        SavingSystemSettingsFailed = 745,

        /// <summary>
        /// Creating request for contact log in
        /// </summary>
        CreatingRequestForContactLogIn = 746,

        /// <summary>
        /// Attempting to connect
        /// </summary>
        AttemptingToConnect = 747,

        /// <summary>
        /// Sending request
        /// </summary>
        SendingRequest = 748,

        /// <summary>
        /// Waiting for response
        /// </summary>
        WaitingForResponsez = 749,

        /// <summary>
        /// Response received
        /// </summary>
        ResponseReceived = 750,

        /// <summary>
        /// Decompressing
        /// </summary>
        Decompressing = 751,

        /// <summary>
        /// Failed to connect
        /// </summary>
        FailedToConnect = 752,

        /// <summary>
        /// Socket exception occurred
        /// </summary>
        SocketExceptionOccurredz = 753,

        /// <summary>
        /// Request for contact log in failed
        /// </summary>
        RequestForContactLogInFailed = 754,

        /// <summary>
        /// Creating request for contact log out
        /// </summary>
        CreatingRequestForContactLogOut = 755,

        /// <summary>
        /// Request for contact log out failed
        /// </summary>
        RequestForContactLogOutFailed = 756,

        /// <summary>
        /// Parsing received data
        /// </summary>
        ParsingReceivedData = 757,

        /// <summary>
        /// Submit contact log in error
        /// </summary>
        SubmitContactLogInError = 758,

        /// <summary>
        /// Received nothing to process
        /// </summary>
        ReceivedNothingToProcess = 759,

        /// <summary>
        /// Database exception occurred
        /// </summary>
        DatabaseExceptionOccurredz = 760,

        /// <summary>
        /// Submit contact log in failed.
        /// </summary>
        SubmitContactLogInFailed = 761,

        /// <summary>
        /// Submit contact log out error
        /// </summary>
        SubmitContactLogOutError = 762,

        /// <summary>
        /// Submit contact log out failed.
        /// </summary>
        SubmitContactLogOutFailed = 763,

        /// <summary>
        /// Unable To Locate Global Location Identifier '{0}' In {1}
        /// </summary>
        UnableToLocateGlobalLocationIdentifierIn = 764,

        /// <summary>
        /// Location map tracking map settings are not set.
        /// </summary>
        LocationMapTrackingMapSettingsAreNotSet = 765,

        /// <summary>
        /// Unable to find product '{0}' unit relation factor between {1} and {2}
        /// </summary>
        UnableToFindProductUnitRelationFactorBetweenAnd = 766,

        /// <summary>
        /// Failed to connect.
        /// </summary>
        FailedToConnectz = 767,

        /// <summary>
        /// SubmitComm socket error occurred.
        /// </summary>
        SubmitcommSocketErrorOccurred = 768,

        /// <summary>
        /// An Unanticipated Error Occured in dMobile: 
        /// </summary>
        AnUnanticipatedErrorOccuredInDmobile = 769,

        /// <summary>
        /// DO YOU WISH TO CONTINUE?
        /// </summary>
        DoYouWishToContinue = 770,

        /// <summary>
        /// dMobile Error Occurred Do you wish to continue!
        /// </summary>
        DmobileErrorOccurredDoYouWishToContinue = 771,

        /// <summary>
        /// Memory Load: 
        /// </summary>
        MemoryLoad = 772,

        /// <summary>
        /// Total Physical: 
        /// </summary>
        TotalPhysical = 773,

        /// <summary>
        /// Avail Physical: 
        /// </summary>
        AvailPhysical = 774,

        /// <summary>
        /// Total Page File: 
        /// </summary>
        TotalPageFile = 775,

        /// <summary>
        /// Avail Page File: 
        /// </summary>
        AvailPageFile = 776,

        /// <summary>
        /// Total Virtual: 
        /// </summary>
        TotalVirtual = 777,

        /// <summary>
        /// Avail Virtual: 
        /// </summary>
        AvailVirtual = 778,

        /// <summary>
        /// Memory Usage
        /// </summary>
        MemoryUsage = 779,

        /// <summary>
        /// IP address {0}
        /// </summary>
        IpAddress = 780,

        /// <summary>
        /// Connection attempt {0} port {1}.
        /// </summary>
        ConnectionAttemptPort = 781,

        /// <summary>
        /// Connection successful.
        /// </summary>
        ConnectionSuccessful = 782,

        /// <summary>
        /// Connection attempt failed on port {0}.
        /// </summary>
        ConnectionAttemptFailedOnPort = 783,

        /// <summary>
        /// Connection attempt timed out.
        /// </summary>
        ConnectionAttemptTimedOut = 784,

        /// <summary>
        /// Connection attempt refused.
        /// </summary>
        ConnectionAttemptRefused = 785,

        /// <summary>
        /// Code {0}.
        /// </summary>
        Codezzz = 786,

        /// <summary>
        /// Msg 
        /// </summary>
        Msg = 787,

        /// <summary>
        /// Getting IP host entry
        /// </summary>
        GettingIpHostEntry = 788,

        /// <summary>
        /// IP address {0} 
        /// </summary>
        IpAddressz = 789,

        /// <summary>
        /// Sending request.
        /// </summary>
        SendingRequestz = 790,

        /// <summary>
        /// Bytes to send {0} 
        /// </summary>
        BytesToSend = 791,

        /// <summary>
        /// Send attempt {0} of {1} 
        /// </summary>
        SendAttemptOf = 792,

        /// <summary>
        /// Sending bytes {0} 
        /// </summary>
        SendingBytes = 793,

        /// <summary>
        /// Bytes sent {0} 
        /// </summary>
        BytesSentz = 794,

        /// <summary>
        /// Bytes remaining {0} 
        /// </summary>
        BytesRemaining = 795,

        /// <summary>
        /// Request sent.
        /// </summary>
        RequestSent = 796,

        /// <summary>
        /// Code {0}
        /// </summary>
        Codezzzz = 797,

        /// <summary>
        /// Total send attempts exceeded.
        /// </summary>
        TotalSendAttemptsExceeded = 798,

        /// <summary>
        /// Receive attempt {0} of {1} 
        /// </summary>
        ReceiveAttemptOf = 799,

        /// <summary>
        /// Received {0} bytes
        /// </summary>
        ReceivedBytes = 800,

        /// <summary>
        /// Total receive attempts exceeded.
        /// </summary>
        TotalReceiveAttemptsExceeded = 801,

        /// <summary>
        /// EOM received.
        /// </summary>
        EomReceived = 802,

        /// <summary>
        /// Bar code minimum length rule violated, must have a length
        /// </summary>
        BarCodeMinimumLengthRuleViolatedMustHaveALength = 803,

        /// <summary>
        ///  greater than 4.
        /// </summary>
        GreaterThan = 804,

        /// <summary>
        /// Unsupported Bar Code Symbology
        /// </summary>
        UnsupportedBarCodeSymbology = 805,

        /// <summary>
        /// Unidentified bar code scanner type
        /// </summary>
        UnidentifiedBarCodeScannerType = 806,

        /// <summary>
        /// Bar code rule violation.
        /// </summary>
        BarCodeRuleViolation = 807,

        /// <summary>
        /// Scan Engine.
        /// </summary>
        ScanEngine = 808,

        /// <summary>
        /// Engine last error \'
        /// </summary>
        EngineLastError = 809,

        /// <summary>
        /// Database exception occurred. Create activity failed.
        /// </summary>
        DatabaseExceptionOccurredCreateActivityFailed = 810,

        /// <summary>
        /// Could not create activity object.
        /// </summary>
        CouldNotCreateActivityObject = 811,

        /// <summary>
        /// Adj #
        /// </summary>
        Adj = 812,

        /// <summary>
        /// Database exception occurred. Purging bar code association failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingBarCodeAssociationFailed = 813,

        /// <summary>
        /// Convert Units
        /// </summary>
        ConvertUnitsz = 814,

        /// <summary>
        /// Count#
        /// </summary>
        Countz = 815,

        /// <summary>
        /// Count Type
        /// </summary>
        CountType = 816,

        /// <summary>
        /// Move Inventory
        /// </summary>
        MoveInventory = 817,

        /// <summary>
        /// Move Skid
        /// </summary>
        MoveSkidz = 818,

        /// <summary>
        /// Order# {0} print shipping label(s) failed.
        /// </summary>
        OrderPrintShippingLabelsFailed = 819,

        /// <summary>
        /// Order# {0} Print shipping label(s) failed.
        /// </summary>
        OrderPrintShippingLabelsFailedz = 820,

        /// <summary>
        /// Order# {0} request shipping label(s) quantity failed.
        /// </summary>
        OrderRequestShippingLabelsQuantityFailed = 821,

        /// <summary>
        /// Get full unit label quantity failed.
        /// </summary>
        GetFullUnitLabelQuantityFailed = 822,

        /// <summary>
        /// Release order# {0} ?
        /// </summary>
        ReleaseOrder = 823,

        /// <summary>
        /// Release order# {0} failed.
        /// </summary>
        ReleaseOrderFailed = 824,

        /// <summary>
        /// Order# {0} quantity under check failed.
        /// </summary>
        OrderQuantityUnderCheckFailed = 825,

        /// <summary>
        /// {1} Items exist where the quantity shipped is less than the quantity ordered.
        /// </summary>
        ItemsExistWhereTheQuantityShippedIsLessThanTheQuantityOrdered = 826,

        /// <summary>
        /// Items that have not been reviewed exist for this section.
        /// </summary>
        ItemsThatHaveNotBeenReviewedExistForThisSection = 827,

        /// <summary>
        /// Items that have not been reviewed exist for this order.
        /// </summary>
        ItemsThatHaveNotBeenReviewedExistForThisOrder = 828,

        /// <summary>
        /// dMobile Picking
        /// </summary>
        DmobilePicking = 829,

        /// <summary>
        /// Del Date
        /// </summary>
        DelDate = 830,

        /// <summary>
        /// Stop
        /// </summary>
        Stop = 831,

        /// <summary>
        /// Ship To
        /// </summary>
        ShipTo = 832,

        /// <summary>
        /// Release order# {0} for warehouse {1}, section {2}?
        /// </summary>
        ReleaseOrderForWarehouseSection = 833,

        /// <summary>
        /// Release order# {0} by order item section {1} failed.
        /// </summary>
        ReleaseOrderByOrderItemSectionFailed = 834,

        /// <summary>
        /// Ticket Additional Products
        /// </summary>
        TicketAdditionalProducts = 835,

        /// <summary>
        /// Ticket #
        /// </summary>
        Ticket = 836,

        /// <summary>
        /// Tickets Finished
        /// </summary>
        TicketsFinished = 837,

        /// <summary>
        /// Ticket Materials
        /// </summary>
        TicketMaterials = 838,

        /// <summary>
        /// Work Sheet
        /// </summary>
        WorkSheetz = 839,

        /// <summary>
        /// PutAway Skid
        /// </summary>
        PutawaySkid = 840,

        /// <summary>
        /// PO #
        /// </summary>
        Po = 841,

        /// <summary>
        /// Arrived
        /// </summary>
        Arrived = 842,

        /// <summary>
        /// Release Order# {0} To Invoice Status?
        /// </summary>
        ReleaseOrderToInvoiceStatus = 843,

        /// <summary>
        /// Release Order
        /// </summary>
        ReleaseOrderz = 844,

        /// <summary>
        /// Release order # {0} to invoice status failed.
        /// </summary>
        ReleaseOrderToInvoiceStatusFailed = 845,

        /// <summary>
        /// Order Date
        /// </summary>
        OrderDate = 846,

        /// <summary>
        /// Return #
        /// </summary>
        Returnz = 847,

        /// <summary>
        /// Qty
        /// </summary>
        Qtyzzz = 848,

        /// <summary>
        /// Transfer #
        /// </summary>
        Transferz = 849,

        /// <summary>
        /// Duplicate scheme ID!
        /// </summary>
        DuplicateSchemeId = 850,

        /// <summary>
        /// Database exception occurred. Inserting detail in disconnected mode failed.
        /// </summary>
        DatabaseExceptionOccurredInsertingDetailInDisconnectedModeFailed = 851,

        /// <summary>
        /// Scheme ID
        /// </summary>
        SchemeId = 852,

        /// <summary>
        /// BC
        /// </summary>
        Bc = 853,

        /// <summary>
        /// Unit#
        /// </summary>
        Unit = 854,

        /// <summary>
        /// Refresh inventory inquiry details failed.
        /// </summary>
        RefreshInventoryInquiryDetailsFailed = 855,

        /// <summary>
        /// Request delete detail error.
        /// </summary>
        RequestDeleteDetailError = 856,

        /// <summary>
        /// Request add detail error.
        /// </summary>
        RequestAddDetailError = 857,

        /// <summary>
        /// Could not create activity detail object.
        /// </summary>
        CouldNotCreateActivityDetailObject = 858,

        /// <summary>
        /// Nothing received. Data was expected.
        /// </summary>
        NothingReceivedDataWasExpected = 859,

        /// <summary>
        /// Duplicate scheme id!
        /// </summary>
        DuplicateSchemeIdz = 860,

        /// <summary>
        /// Could not create activity item object.
        /// </summary>
        CouldNotCreateActivityItemObject = 861,

        /// <summary>
        /// Unable to parse adjustment item # {0}
        /// </summary>
        UnableToParseAdjustmentItem = 862,

        /// <summary>
        /// Unable to find adjustment item # {0}
        /// </summary>
        UnableToFindAdjustmentItem = 863,

        /// <summary>
        /// A line item already exists for this location and product.
        /// </summary>
        ALineItemAlreadyExistsForThisLocationAndProduct = 864,

        /// <summary>
        /// New Item
        /// </summary>
        NewItem = 865,

        /// <summary>
        /// Scanned bar code is already associated to a different product {0} {1} {2}.
        /// </summary>
        ScannedBarCodeIsAlreadyAssociatedToADifferentProduct = 866,

        /// <summary>
        /// Unable To Parse Count Item# {0}
        /// </summary>
        UnableToParseCountItem = 867,

        /// <summary>
        /// Unable To Find Count Item# {0}
        /// </summary>
        UnableToFindCountItem = 868,

        /// <summary>
        /// On Hand
        /// </summary>
        OnHandz = 869,

        /// <summary>
        /// Counted
        /// </summary>
        Counted = 870,

        /// <summary>
        /// Refresh inventory inquiry items failed.
        /// </summary>
        RefreshInventoryInquiryItemsFailed = 871,

        /// <summary>
        /// Unable to parse count item# {0}
        /// </summary>
        UnableToParseCountItemz = 872,

        /// <summary>
        /// Unable to find count item# {0}
        /// </summary>
        UnableToFindCountItemz = 873,

        /// <summary>
        /// Wt On Hand
        /// </summary>
        WtOnHand = 874,

        /// <summary>
        /// Product scannned, code \'{0}\' bc \'{1}\' unit \'{2}\', is not found on this order for warehouse {3}.
        /// </summary>
        ProductScannnedCodeBcUnitIsNotFoundOnThisOrderForWarehouse = 875,

        /// <summary>
        /// Item is outside of expected weight.
        /// </summary>
        ItemIsOutsideOfExpectedWeight = 876,

        /// <summary>
        /// Quantity Over
        /// </summary>
        QuantityOver = 877,

        /// <summary>
        /// Request picking item refno from detail refno error.
        /// </summary>
        RequestPickingItemRefnoFromDetailRefnoError = 878,

        /// <summary>
        /// Unable to parse sales order item # {0}.
        /// </summary>
        UnableToParseSalesOrderItem = 879,

        /// <summary>
        /// Unable to find sales order item # {0}.
        /// </summary>
        UnableToFindSalesOrderItem = 880,

        /// <summary>
        /// Shipped
        /// </summary>
        Shippedzz = 881,

        /// <summary>
        /// Available
        /// </summary>
        Available = 882,

        /// <summary>
        /// Only skid deletion is supported.
        /// </summary>
        OnlySkidDeletionIsSupported = 883,

        /// <summary>
        /// Total ordered:\r{0} @ {1}{2}\r\rTotal shipped:\r{3} @ {4}{5}
        /// </summary>
        TotalOrderedrRrtotalShippedr = 884,

        /// <summary>
        /// The resulting quantity shipped {0} {1} will be greater than the quantity ordered {2} {3}.
        /// </summary>
        TheResultingQuantityShippedWillBeGreaterThanTheQuantityOrdered = 885,

        /// <summary>
        /// Order # {0} item request shipping label(s) quantity failed.
        /// </summary>
        OrderItemRequestShippingLabelsQuantityFailed = 886,

        /// <summary>
        /// Weight Range Check
        /// </summary>
        WeightRangeCheck = 887,

        /// <summary>
        /// Unable to parse sales order item # {0}
        /// </summary>
        UnableToParseSalesOrderItemz = 888,

        /// <summary>
        /// Unable to find sales order item # {0}
        /// </summary>
        UnableToFindSalesOrderItemz = 889,

        /// <summary>
        /// Unable to parse production item# {0}
        /// </summary>
        UnableToParseProductionItem = 890,

        /// <summary>
        /// Unable to find production item# {0}
        /// </summary>
        UnableToFindProductionItem = 891,

        /// <summary>
        /// ExpQty
        /// </summary>
        Expqty = 892,

        /// <summary>
        /// Unable to parse purchase order work sheet item# {0}
        /// </summary>
        UnableToParsePurchaseOrderWorkSheetItem = 893,

        /// <summary>
        /// Unable to find purchase order work sheet item# {0}
        /// </summary>
        UnableToFindPurchaseOrderWorkSheetItem = 894,

        /// <summary>
        /// Unable to parse purchase order item# {0}
        /// </summary>
        UnableToParsePurchaseOrderItem = 895,

        /// <summary>
        /// Unable to find purchase order item# {0}
        /// </summary>
        UnableToFindPurchaseOrderItem = 896,

        /// <summary>
        /// Received
        /// </summary>
        Received = 897,

        /// <summary>
        /// Total received:
        /// </summary>
        TotalReceived = 898,

        /// <summary>
        /// The resulting quantity received {0} {1} will be greater than the quantity ordered {2} {3}.
        /// </summary>
        TheResultingQuantityReceivedWillBeGreaterThanTheQuantityOrdered = 899,

        /// <summary>
        /// Unable to parse returns item # {0}
        /// </summary>
        UnableToParseReturnsItem = 900,

        /// <summary>
        /// Unable to find returns item # {0}
        /// </summary>
        UnableToFindReturnsItem = 901,

        /// <summary>
        /// Add new return items is not supported.
        /// </summary>
        AddNewReturnItemsIsNotSupported = 902,

        /// <summary>
        /// Unable to parse the transfer item # {0}
        /// </summary>
        UnableToParseTheTransferItem = 903,

        /// <summary>
        /// Unable to find the transfer item # {0}
        /// </summary>
        UnableToFindTheTransferItem = 904,

        /// <summary>
        /// Required
        /// </summary>
        Required = 905,

        /// <summary>
        /// The resulting quantity transferred {0} {1} will be greater than the quantity required {2} {3}.
        /// </summary>
        TheResultingQuantityTransferredWillBeGreaterThanTheQuantityRequired = 906,

        /// <summary>
        /// Purging activity.
        /// </summary>
        PurgingActivity = 907,

        /// <summary>
        /// Purging activity complete.
        /// </summary>
        PurgingActivityComplete = 908,

        /// <summary>
        /// Purging activity failed.
        /// </summary>
        PurgingActivityFailed = 909,

        /// <summary>
        /// Database exception occurred. Purging activity failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingActivityFailed = 910,

        /// <summary>
        /// Seeding adjustments.
        /// </summary>
        SeedingAdjustments = 911,

        /// <summary>
        /// Seeding adjustments complete.
        /// </summary>
        SeedingAdjustmentsComplete = 912,

        /// <summary>
        /// Seeding adjustments failed.
        /// </summary>
        SeedingAdjustmentsFailed = 913,

        /// <summary>
        /// Purging adjustments.
        /// </summary>
        PurgingAdjustments = 914,

        /// <summary>
        /// Purging adjustments complete.
        /// </summary>
        PurgingAdjustmentsComplete = 915,

        /// <summary>
        /// Purging adjustments failed.
        /// </summary>
        PurgingAdjustmentsFailed = 916,

        /// <summary>
        /// Database exception occurred. Purge adjustments failed.
        /// </summary>
        DatabaseExceptionOccurredPurgeAdjustmentsFailed = 917,

        /// <summary>
        /// {0} adjustment(s)
        /// </summary>
        Adjustmentszz = 918,

        /// <summary>
        /// Database exception occurred. Adjustments status failed.
        /// </summary>
        DatabaseExceptionOccurredAdjustmentsStatusFailed = 919,

        /// <summary>
        /// Saving adjustments.
        /// </summary>
        SavingAdjustments = 920,

        /// <summary>
        /// Arguments:
        /// </summary>
        Arguments = 921,

        /// <summary>
        /// Adj type \'
        /// </summary>
        AdjType = 922,

        /// <summary>
        /// Saving adjustments complete.
        /// </summary>
        SavingAdjustmentsComplete = 923,

        /// <summary>
        /// Saving adjustments failed.
        /// </summary>
        SavingAdjustmentsFailed = 924,

        /// <summary>
        /// Uniqueness violation.
        /// </summary>
        UniquenessViolation = 925,

        /// <summary>
        /// Database exception occurred. Saving adjustments failed.
        /// </summary>
        DatabaseExceptionOccurredSavingAdjustmentsFailed = 926,

        /// <summary>
        /// Purging convert units.
        /// </summary>
        PurgingConvertUnits = 927,

        /// <summary>
        /// Purging convert units complete.
        /// </summary>
        PurgingConvertUnitsComplete = 928,

        /// <summary>
        /// Purging convert units failed.
        /// </summary>
        PurgingConvertUnitsFailed = 929,

        /// <summary>
        /// Database exception occurred. Purging convert units failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingConvertUnitsFailed = 930,

        /// <summary>
        /// Seeding cycle counts.
        /// </summary>
        SeedingCycleCounts = 931,

        /// <summary>
        /// Seeding cycle counts complete.
        /// </summary>
        SeedingCycleCountsComplete = 932,

        /// <summary>
        /// Seeding cycle counts failed.
        /// </summary>
        SeedingCycleCountsFailed = 933,

        /// <summary>
        /// {0} cycle count(s)
        /// </summary>
        CycleCountsz = 934,

        /// <summary>
        /// Database exception occurred. Cycle counts status failed.
        /// </summary>
        DatabaseExceptionOccurredCycleCountsStatusFailed = 935,

        /// <summary>
        /// Purging cycle counts.
        /// </summary>
        PurgingCycleCounts = 936,

        /// <summary>
        /// Purging cycle counts complete.
        /// </summary>
        PurgingCycleCountsComplete = 937,

        /// <summary>
        /// Purging cycle counts failed.
        /// </summary>
        PurgingCycleCountsFailed = 938,

        /// <summary>
        /// Database exception occurred. Purging cycle counts failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingCycleCountsFailed = 939,

        /// <summary>
        /// Saving cycle counts.
        /// </summary>
        SavingCycleCounts = 940,

        /// <summary>
        /// Type \'
        /// </summary>
        Typez = 941,

        /// <summary>
        /// Saving cycle counts complete.
        /// </summary>
        SavingCycleCountsComplete = 942,

        /// <summary>
        /// Saving cycle counts failed.
        /// </summary>
        SavingCycleCountsFailed = 943,

        /// <summary>
        /// Database exception occurred. Saving cycle counts failed.
        /// </summary>
        DatabaseExceptionOccurredSavingCycleCountsFailed = 944,

        /// <summary>
        /// Seeding cycle count details.
        /// </summary>
        SeedingCycleCountDetails = 945,

        /// <summary>
        /// Seeding cycle count details complete.
        /// </summary>
        SeedingCycleCountDetailsComplete = 946,

        /// <summary>
        /// Request cycle count details failed.
        /// </summary>
        RequestCycleCountDetailsFailed = 947,

        /// <summary>
        /// Database exception occurred. Request cycle count details failed.
        /// </summary>
        DatabaseExceptionOccurredRequestCycleCountDetailsFailed = 948,

        /// <summary>
        /// {0} cycle count detail(s)
        /// </summary>
        CycleCountDetails = 949,

        /// <summary>
        /// Database exception occurred. Cycle count details status failed.
        /// </summary>
        DatabaseExceptionOccurredCycleCountDetailsStatusFailed = 950,

        /// <summary>
        /// Purging cycle count details.
        /// </summary>
        PurgingCycleCountDetails = 951,

        /// <summary>
        /// Purging cycle count details complete.
        /// </summary>
        PurgingCycleCountDetailsComplete = 952,

        /// <summary>
        /// Purging cycle count details failed.
        /// </summary>
        PurgingCycleCountDetailsFailed = 953,

        /// <summary>
        /// Database exception occurred. Purging cycle count details failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingCycleCountDetailsFailed = 954,

        /// <summary>
        /// drItemCount.Read() failed.
        /// </summary>
        DritemcountreadFailed = 955,

        /// <summary>
        /// Unable to locate the item cycle count refno {0} in tActivityItemCount.
        /// </summary>
        UnableToLocateTheItemCycleCountRefnoInTactivityitemcount = 956,

        /// <summary>
        /// Saving cycle count details.
        /// </summary>
        SavingCycleCountDetails = 957,

        /// <summary>
        /// Unable to locate the activity item refno {0} in tActivityItemCount
        /// </summary>
        UnableToLocateTheActivityItemRefnoInTactivityitemcount = 958,

        /// <summary>
        /// Detail fk refno
        /// </summary>
        DetailFkRefno = 959,

        /// <summary>
        /// Saving cycle count details complete.
        /// </summary>
        SavingCycleCountDetailsComplete = 960,

        /// <summary>
        /// Saving cycle count details failed.
        /// </summary>
        SavingCycleCountDetailsFailed = 961,

        /// <summary>
        /// Database exception occurred. Saving cycle count details failed.
        /// </summary>
        DatabaseExceptionOccurredSavingCycleCountDetailsFailed = 962,

        /// <summary>
        /// Seeding cycle count items.
        /// </summary>
        SeedingCycleCountItems = 963,

        /// <summary>
        /// Seeding cycle count items complete.
        /// </summary>
        SeedingCycleCountItemsComplete = 964,

        /// <summary>
        /// Seeding cycle count items failed.
        /// </summary>
        SeedingCycleCountItemsFailed = 965,

        /// <summary>
        /// Database exception occurred. Seeding cycle count items failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingCycleCountItemsFailed = 966,

        /// <summary>
        /// {0} cycle count item(s)
        /// </summary>
        CycleCountItems = 967,

        /// <summary>
        /// Database exception occurred. Cycle count items status failed.
        /// </summary>
        DatabaseExceptionOccurredCycleCountItemsStatusFailed = 968,

        /// <summary>
        /// Purging cycle count items.
        /// </summary>
        PurgingCycleCountItems = 969,

        /// <summary>
        /// Purging cycle count items complete.
        /// </summary>
        PurgingCycleCountItemsComplete = 970,

        /// <summary>
        /// Purging cycle count items failed.
        /// </summary>
        PurgingCycleCountItemsFailed = 971,

        /// <summary>
        /// Database exception occurred.Purging cycle count items failed.
        /// </summary>
        DatabaseExceptionOccurredpurgingCycleCountItemsFailed = 972,

        /// <summary>
        /// Unable to locate the cycle count refno {0} in tActivityCount
        /// </summary>
        UnableToLocateTheCycleCountRefnoInTactivitycount = 973,

        /// <summary>
        /// Saving cycle count items.
        /// </summary>
        SavingCycleCountItems = 974,

        /// <summary>
        /// Unable to indentify the location map reference number.{0}
        /// </summary>
        UnableToIndentifyTheLocationMapRefno = 975,

        /// <summary>
        /// Saving cycle count items complete.
        /// </summary>
        SavingCycleCountItemsComplete = 976,

        /// <summary>
        /// Saving cycle count items failed.
        /// </summary>
        SavingCycleCountItemsFailed = 977,

        /// <summary>
        /// Database exception occurred. Saving cycle count items failed.
        /// </summary>
        DatabaseExceptionOccurredSavingCycleCountItemsFailed = 978,

        /// <summary>
        /// Seeding adjustment details.
        /// </summary>
        SeedingAdjustmentDetails = 979,

        /// <summary>
        /// Seeding adjustment details complete.
        /// </summary>
        SeedingAdjustmentDetailsComplete = 980,

        /// <summary>
        /// {0} adjustment details.
        /// </summary>
        AdjustmentDetails = 981,

        /// <summary>
        /// Database exception occurred. Adjustment details status failed.
        /// </summary>
        DatabaseExceptionOccurredAdjustmentDetailsStatusFailed = 982,

        /// <summary>
        /// Purging adjustment details.
        /// </summary>
        PurgingAdjustmentDetails = 983,

        /// <summary>
        /// Purging adjustment details complete.
        /// </summary>
        PurgingAdjustmentDetailsComplete = 984,

        /// <summary>
        /// Purging adjustment details failed.
        /// </summary>
        PurgingAdjustmentDetailsFailed = 985,

        /// <summary>
        /// Database exception occurred. Purging adjustment details failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingAdjustmentDetailsFailed = 986,

        /// <summary>
        /// Unable to locate item adjustment {0} in tActivityItemAdjustment
        /// </summary>
        UnableToLocateItemAdjustmentInTactivityitemadjustment = 987,

        /// <summary>
        /// Saving adjustment details.
        /// </summary>
        SavingAdjustmentDetails = 988,

        /// <summary>
        /// Unable to locate the item adjustment refno {0} in tActivityItemAdjustment
        /// </summary>
        UnableToLocateTheItemAdjustmentRefnoInTactivityitemadjustment = 989,

        /// <summary>
        /// Saving adjustment details complete.
        /// </summary>
        SavingAdjustmentDetailsComplete = 990,

        /// <summary>
        /// Saving adjustment details failed.
        /// </summary>
        SavingAdjustmentDetailsFailed = 991,

        /// <summary>
        /// Database exception occurred. Saving adjustment details failed.
        /// </summary>
        DatabaseExceptionOccurredSavingAdjustmentDetailsFailed = 992,

        /// <summary>
        /// Purging convert unit details.
        /// </summary>
        PurgingConvertUnitDetails = 993,

        /// <summary>
        /// Purging convert unit details complete.
        /// </summary>
        PurgingConvertUnitDetailsComplete = 994,

        /// <summary>
        /// Purging convert unit details failed.
        /// </summary>
        PurgingConvertUnitDetailsFailed = 995,

        /// <summary>
        /// Database exception occurred. Purging convert unit details failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingConvertUnitDetailsFailed = 996,

        /// <summary>
        /// Purging move inventory details.
        /// </summary>
        PurgingMoveInventoryDetails = 997,

        /// <summary>
        /// Purging move inventory details complete.
        /// </summary>
        PurgingMoveInventoryDetailsComplete = 998,

        /// <summary>
        /// Purging move inventory details failed.
        /// </summary>
        PurgingMoveInventoryDetailsFailed = 999,

        /// <summary>
        /// Database exception occurred. Purging move inventory details failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingMoveInventoryDetailsFailed = 1000,

        /// <summary>
        /// Purging activity items.
        /// </summary>
        PurgingActivityItems = 1001,

        /// <summary>
        /// Purging activity items complete.
        /// </summary>
        PurgingActivityItemsComplete = 1002,

        /// <summary>
        /// Purging activity items failed.
        /// </summary>
        PurgingActivityItemsFailed = 1003,

        /// <summary>
        /// Database exception occurred. Purging activity items failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingActivityItemsFailed = 1004,

        /// <summary>
        /// Seeding adjustment items.
        /// </summary>
        SeedingAdjustmentItems = 1005,

        /// <summary>
        /// Seeding adjustment items complete.
        /// </summary>
        SeedingAdjustmentItemsComplete = 1006,

        /// <summary>
        /// Seeding adjustment items failed.
        /// </summary>
        SeedingAdjustmentItemsFailed = 1007,

        /// <summary>
        /// Database exception occurred. Seeding adjustment items failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingAdjustmentItemsFailed = 1008,

        /// <summary>
        /// {0} adjustment items.
        /// </summary>
        AdjustmentItems = 1009,

        /// <summary>
        /// Database exception occurred. Adjustment items status failed.
        /// </summary>
        DatabaseExceptionOccurredAdjustmentItemsStatusFailed = 1010,

        /// <summary>
        /// Purging existing adjustment items.
        /// </summary>
        PurgingExistingAdjustmentItems = 1011,

        /// <summary>
        /// Purging existing adjustment items complete.
        /// </summary>
        PurgingExistingAdjustmentItemsComplete = 1012,

        /// <summary>
        /// Purging adjustment items failed.
        /// </summary>
        PurgingAdjustmentItemsFailed = 1013,

        /// <summary>
        /// Database exception occurred. Purging adjustment items failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingAdjustmentItemsFailed = 1014,

        /// <summary>
        /// drActAdj.Read() failed.
        /// </summary>
        DractadjreadFailed = 1015,

        /// <summary>
        /// Unable to locate the adjustment refno {0} in tActivityAdjustment
        /// </summary>
        UnableToLocateTheAdjustmentRefnoInTactivityadjustment = 1016,

        /// <summary>
        /// Saving adjustment items.
        /// </summary>
        SavingAdjustmentItems = 1017,

        /// <summary>
        /// Item fk refno 
        /// </summary>
        ItemFkRefno = 1018,

        /// <summary>
        /// Saving adjustment items complete.
        /// </summary>
        SavingAdjustmentItemsComplete = 1019,

        /// <summary>
        /// saving adjustment items failed.
        /// </summary>
        SavingAdjustmentItemsFailed = 1020,

        /// <summary>
        /// Database exception occurred. Saving adjustment items failed.
        /// </summary>
        DatabaseExceptionOccurredSavingAdjustmentItemsFailed = 1021,

        /// <summary>
        /// Purging item bar code association xrefs.
        /// </summary>
        PurgingItemBarCodeAssociationXrefs = 1022,

        /// <summary>
        /// Purging item bar code association xrefs complete.
        /// </summary>
        PurgingItemBarCodeAssociationXrefsComplete = 1023,

        /// <summary>
        /// Purging item bar code association xrefs failed.
        /// </summary>
        PurgingItemBarCodeAssociationXrefsFailed = 1024,

        /// <summary>
        /// Database exception occurred. Purging item bar code association xrefs failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingItemBarCodeAssociationXrefsFailed = 1025,

        /// <summary>
        /// Purging convert unit items.
        /// </summary>
        PurgingConvertUnitItems = 1026,

        /// <summary>
        /// Purging convert unit items complete.
        /// </summary>
        PurgingConvertUnitItemsComplete = 1027,

        /// <summary>
        /// Purging convert unit items failed.
        /// </summary>
        PurgingConvertUnitItemsFailed = 1028,

        /// <summary>
        /// Database exception occurred. Purging convert unit items failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingConvertUnitItemsFailed = 1029,

        /// <summary>
        /// Purging move inventory items.
        /// </summary>
        PurgingMoveInventoryItems = 1030,

        /// <summary>
        /// Purging move inventory items complete.
        /// </summary>
        PurgingMoveInventoryItemsComplete = 1031,

        /// <summary>
        /// Purging move inventory items failed.
        /// </summary>
        PurgingMoveInventoryItemsFailed = 1032,

        /// <summary>
        /// Database exception occurred. Purging move inventory items failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingMoveInventoryItemsFailed = 1033,

        /// <summary>
        /// Purging move inventory.
        /// </summary>
        PurgingMoveInventory = 1034,

        /// <summary>
        /// Purging move inventory complete.
        /// </summary>
        PurgingMoveInventoryComplete = 1035,

        /// <summary>
        /// Purging move inventory failed.
        /// </summary>
        PurgingMoveInventoryFailed = 1036,

        /// <summary>
        /// Database exception occurred. Purging move inventory failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingMoveInventoryFailed = 1037,

        /// <summary>
        /// Seeding physical counts.
        /// </summary>
        SeedingPhysicalCounts = 1038,

        /// <summary>
        /// physical counts
        /// </summary>
        PhysicalCountsz = 1039,

        /// <summary>
        /// Seeding physical counts complete.
        /// </summary>
        SeedingPhysicalCountsComplete = 1040,

        /// <summary>
        /// Seeding physical counts failed.
        /// </summary>
        SeedingPhysicalCountsFailed = 1041,

        /// <summary>
        /// {0} physical counts.
        /// </summary>
        PhysicalCountszz = 1042,

        /// <summary>
        /// Database exception occurred. Physical counts status failed.
        /// </summary>
        DatabaseExceptionOccurredPhysicalCountsStatusFailed = 1043,

        /// <summary>
        /// Purging physical counts.
        /// </summary>
        PurgingPhysicalCounts = 1044,

        /// <summary>
        /// Purging physical counts complete.
        /// </summary>
        PurgingPhysicalCountsComplete = 1045,

        /// <summary>
        /// Purging physical counts failed.
        /// </summary>
        PurgingPhysicalCountsFailed = 1046,

        /// <summary>
        /// Database exception occurred. Purging physical counts failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingPhysicalCountsFailed = 1047,

        /// <summary>
        /// Saving physical counts.
        /// </summary>
        SavingPhysicalCounts = 1048,

        /// <summary>
        /// Standard Physical Count By Warehouse
        /// </summary>
        StandardPhysicalCountByWarehouse = 1049,

        /// <summary>
        /// Cycle Count By Warehouse
        /// </summary>
        CycleCountByWarehouse = 1050,

        /// <summary>
        /// Cycle Count By Section
        /// </summary>
        CycleCountBySection = 1051,

        /// <summary>
        /// Cycle Count By Slot
        /// </summary>
        CycleCountBySlot = 1052,

        /// <summary>
        /// Base Cycle Count By Warehouse
        /// </summary>
        BaseCycleCountByWarehouse = 1053,

        /// <summary>
        /// Saving physical counts complete.
        /// </summary>
        SavingPhysicalCountsComplete = 1054,

        /// <summary>
        /// Saving physical counts failed.
        /// </summary>
        SavingPhysicalCountsFailed = 1055,

        /// <summary>
        /// Database exception occurred. Saving physical counts failed.
        /// </summary>
        DatabaseExceptionOccurredSavingPhysicalCountsFailed = 1056,

        /// <summary>
        /// Seeding physical count details.
        /// </summary>
        SeedingPhysicalCountDetails = 1057,

        /// <summary>
        /// Seeding physical count details complete.
        /// </summary>
        SeedingPhysicalCountDetailsComplete = 1058,

        /// <summary>
        /// Seeding physical count details failed.
        /// </summary>
        SeedingPhysicalCountDetailsFailed = 1059,

        /// <summary>
        /// Database exception occurred. Seeding physical count details failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingPhysicalCountDetailsFailed = 1060,

        /// <summary>
        /// Purging physical count details.
        /// </summary>
        PurgingPhysicalCountDetails = 1061,

        /// <summary>
        /// Purging physical count details complete.
        /// </summary>
        PurgingPhysicalCountDetailsComplete = 1062,

        /// <summary>
        /// Purging physical count details failed.
        /// </summary>
        PurgingPhysicalCountDetailsFailed = 1063,

        /// <summary>
        /// Database exception occurred. Purging physical count details failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingPhysicalCountDetailsFailed = 1064,

        /// <summary>
        /// {0} Physical count details.
        /// </summary>
        PhysicalCountDetails = 1065,

        /// <summary>
        /// Database exception occurred. Physical count details status failed.
        /// </summary>
        DatabaseExceptionOccurredPhysicalCountDetailsStatusFailed = 1066,

        /// <summary>
        /// Unable to locate the item physical count refno {0} in tActivityItemCount.
        /// </summary>
        UnableToLocateTheItemPhysicalCountRefnoInTactivityitemcount = 1067,

        /// <summary>
        /// Saving physical count details.
        /// </summary>
        SavingPhysicalCountDetails = 1068,

        /// <summary>
        /// Saving physical count details complete.
        /// </summary>
        SavingPhysicalCountDetailsComplete = 1069,

        /// <summary>
        /// Saving physical count details failed.
        /// </summary>
        SavingPhysicalCountDetailsFailed = 1070,

        /// <summary>
        /// Database exception occurred. Saving physical count details failed.
        /// </summary>
        DatabaseExceptionOccurredSavingPhysicalCountDetailsFailed = 1071,

        /// <summary>
        /// Seeding physical count items.
        /// </summary>
        SeedingPhysicalCountItems = 1072,

        /// <summary>
        /// Seeding physical count items complete.
        /// </summary>
        SeedingPhysicalCountItemsComplete = 1073,

        /// <summary>
        /// Seeding physical count items failed.
        /// </summary>
        SeedingPhysicalCountItemsFailed = 1074,

        /// <summary>
        /// Database exception occurred. Seeding physical count items failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingPhysicalCountItemsFailed = 1075,

        /// <summary>
        /// {0} physical count items.
        /// </summary>
        PhysicalCountItems = 1076,

        /// <summary>
        /// Database exception occurred. Physical count items status failed.
        /// </summary>
        DatabaseExceptionOccurredPhysicalCountItemsStatusFailed = 1077,

        /// <summary>
        /// Purging physical count items.
        /// </summary>
        PurgingPhysicalCountItems = 1078,

        /// <summary>
        /// Purging physical count items complete.
        /// </summary>
        PurgingPhysicalCountItemsComplete = 1079,

        /// <summary>
        /// Purging physical count items failed.
        /// </summary>
        PurgingPhysicalCountItemsFailed = 1080,

        /// <summary>
        /// Database exception occurred. Purging physical count items failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingPhysicalCountItemsFailed = 1081,

        /// <summary>
        /// drCount.Read() failed.
        /// </summary>
        DrcountreadFailed = 1082,

        /// <summary>
        /// Unable to locate the physical count refno {0} in tActivityCount
        /// </summary>
        UnableToLocateThePhysicalCountRefnoInTactivitycount = 1083,

        /// <summary>
        /// Saving physical count items.
        /// </summary>
        SavingPhysicalCountItems = 1084,

        /// <summary>
        /// Request physical count items complete.
        /// </summary>
        RequestPhysicalCountItemsComplete = 1085,

        /// <summary>
        /// Request physical count items failed.
        /// </summary>
        RequestPhysicalCountItemsFailed = 1086,

        /// <summary>
        /// Database exception occurred. Saving physical count items failed.
        /// </summary>
        DatabaseExceptionOccurredSavingPhysicalCountItemsFailed = 1087,

        /// <summary>
        /// Seeding sales orders.
        /// </summary>
        SeedingSalesOrders = 1088,

        /// <summary>
        /// Seeding sales orders complete.
        /// </summary>
        SeedingSalesOrdersComplete = 1089,

        /// <summary>
        /// Seeding sales orders failed.
        /// </summary>
        SeedingSalesOrdersFailed = 1090,

        /// <summary>
        /// {0} sales orders.
        /// </summary>
        SalesOrderszz = 1091,

        /// <summary>
        /// Database exception occurred. Sales orders status failed.
        /// </summary>
        DatabaseExceptionOccurredSalesOrdersStatusFailed = 1092,

        /// <summary>
        /// Purging sales orders.
        /// </summary>
        PurgingSalesOrders = 1093,

        /// <summary>
        /// Purging sales orders complete.
        /// </summary>
        PurgingSalesOrdersComplete = 1094,

        /// <summary>
        /// Purging sales orders failed.
        /// </summary>
        PurgingSalesOrdersFailed = 1095,

        /// <summary>
        /// Database exception occurred. Purging sales orders failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingSalesOrdersFailed = 1096,

        /// <summary>
        /// Saving sales orders.
        /// </summary>
        SavingSalesOrders = 1097,

        /// <summary>
        /// Unable to locate the route code in tRoute.
        /// </summary>
        UnableToLocateTheRouteCodeInTroute = 1098,

        /// <summary>
        /// Saving sales orders complete.
        /// </summary>
        SavingSalesOrdersComplete = 1099,

        /// <summary>
        /// Saving sales orders failed.
        /// </summary>
        SavingSalesOrdersFailed = 1100,

        /// <summary>
        /// Database exception occurred. Saving sales orders failed.
        /// </summary>
        DatabaseExceptionOccurredSavingSalesOrdersFailed = 1101,

        /// <summary>
        /// Seeding sales order details.
        /// </summary>
        SeedingSalesOrderDetails = 1102,

        /// <summary>
        /// Seeding sales order details complete.
        /// </summary>
        SeedingSalesOrderDetailsComplete = 1103,

        /// <summary>
        /// Seeding picking details failed.
        /// </summary>
        SeedingPickingDetailsFailed = 1104,

        /// <summary>
        /// Database exception occurred. Seeding picking details failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingPickingDetailsFailed = 1105,

        /// <summary>
        /// Seeding sales order details failed.
        /// </summary>
        SeedingSalesOrderDetailsFailed = 1106,

        /// <summary>
        /// Purging sales order details.
        /// </summary>
        PurgingSalesOrderDetails = 1107,

        /// <summary>
        /// Purging sales order details complete.
        /// </summary>
        PurgingSalesOrderDetailsComplete = 1108,

        /// <summary>
        /// Purging sales order details failed.
        /// </summary>
        PurgingSalesOrderDetailsFailed = 1109,

        /// <summary>
        /// Database exception occurred. Purging sales order details failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingSalesOrderDetailsFailed = 1110,

        /// <summary>
        /// {0} Sales order details.
        /// </summary>
        SalesOrderDetails = 1111,

        /// <summary>
        /// Database exception occurred. Sales order details status failed.
        /// </summary>
        DatabaseExceptionOccurredSalesOrderDetailsStatusFailed = 1112,

        /// <summary>
        /// drItemPick.Read() failed.
        /// </summary>
        DritempickreadFailed = 1113,

        /// <summary>
        /// Unable to locate the item picking refno {0} in tActivityItemPicking.
        /// </summary>
        UnableToLocateTheItemPickingRefnoInTactivityitempicking = 1114,

        /// <summary>
        /// Saving sales order details.
        /// </summary>
        SavingSalesOrderDetails = 1115,

        /// <summary>
        /// Unable to locate the activity item refno {0} in 
        /// </summary>
        UnableToLocateTheActivityItemRefnoIn = 1116,

        /// <summary>
        /// tActivityItemPicking
        /// </summary>
        Tactivityitempicking = 1117,

        /// <summary>
        /// Saving sales order details completed.
        /// </summary>
        SavingSalesOrderDetailsCompleted = 1118,

        /// <summary>
        /// Saving sales order details failed.
        /// </summary>
        SavingSalesOrderDetailsFailed = 1119,

        /// <summary>
        /// Database exception occurred. Saving sales order details failed.
        /// </summary>
        DatabaseExceptionOccurredSavingSalesOrderDetailsFailed = 1120,

        /// <summary>
        /// Seeding sales order items.
        /// </summary>
        SeedingSalesOrderItems = 1121,

        /// <summary>
        /// Seeding sales order items complete.
        /// </summary>
        SeedingSalesOrderItemsComplete = 1122,

        /// <summary>
        /// Seeding sales order items failed.
        /// </summary>
        SeedingSalesOrderItemsFailed = 1123,

        /// <summary>
        /// Database exception occurred. Seeding sales order items failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingSalesOrderItemsFailed = 1124,

        /// <summary>
        /// Purging sales order items.
        /// </summary>
        PurgingSalesOrderItems = 1125,

        /// <summary>
        /// Purging sales order items completed.
        /// </summary>
        PurgingSalesOrderItemsCompleted = 1126,

        /// <summary>
        /// Purging sales order items failed.
        /// </summary>
        PurgingSalesOrderItemsFailed = 1127,

        /// <summary>
        /// Database exception occurred. Purging sales order items failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingSalesOrderItemsFailed = 1128,

        /// <summary>
        /// {0} Sales order items.
        /// </summary>
        SalesOrderItems = 1129,

        /// <summary>
        /// Database exception occurred. Sales order items status failed.
        /// </summary>
        DatabaseExceptionOccurredSalesOrderItemsStatusFailed = 1130,

        /// <summary>
        /// drPicking.Read() failed.
        /// </summary>
        DrpickingreadFailed = 1131,

        /// <summary>
        /// Unable to locate the sales order refno {0} In tActivityPicking
        /// </summary>
        UnableToLocateTheSalesOrderRefnoInTactivitypicking = 1132,

        /// <summary>
        /// Saving sales order items.
        /// </summary>
        SavingSalesOrderItems = 1133,

        /// <summary>
        /// Saving sales order items complete.
        /// </summary>
        SavingSalesOrderItemsComplete = 1134,

        /// <summary>
        /// Saving sales order items failed.
        /// </summary>
        SavingSalesOrderItemsFailed = 1135,

        /// <summary>
        /// Database exception occurred. Saving sales order items failed.
        /// </summary>
        DatabaseExceptionOccurredSavingSalesOrderItemsFailed = 1136,

        /// <summary>
        /// Purging sales order item comments.
        /// </summary>
        PurgingSalesOrderItemComments = 1137,

        /// <summary>
        /// Purging sales order item comments complete.
        /// </summary>
        PurgingSalesOrderItemCommentsComplete = 1138,

        /// <summary>
        /// Purge sales order item comments failed.
        /// </summary>
        PurgeSalesOrderItemCommentsFailed = 1139,

        /// <summary>
        /// Database exception occurred. Purging sales order item comments failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingSalesOrderItemCommentsFailed = 1140,

        /// <summary>
        /// Purging sales order routes.
        /// </summary>
        PurgingSalesOrderRoutes = 1141,

        /// <summary>
        /// Purging sales order routes complete.
        /// </summary>
        PurgingSalesOrderRoutesComplete = 1142,

        /// <summary>
        /// Purging sales order routes failed.
        /// </summary>
        PurgingSalesOrderRoutesFailed = 1143,

        /// <summary>
        /// Database exception occurred. Purging sales order routes failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingSalesOrderRoutesFailed = 1144,

        /// <summary>
        /// Seeding ticket add-products.
        /// </summary>
        SeedingTicketAddproducts = 1145,

        /// <summary>
        /// Seeding ticket add-products complete.
        /// </summary>
        SeedingTicketAddproductsComplete = 1146,

        /// <summary>
        /// Seeding ticket add-products failed.
        /// </summary>
        SeedingTicketAddproductsFailed = 1147,

        /// <summary>
        /// Database exception occurred. Seeding ticket add-products failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingTicketAddproductsFailed = 1148,

        /// <summary>
        /// {0} ticket add-products.
        /// </summary>
        TicketAddproducts = 1149,

        /// <summary>
        /// Database exception occurred. Ticket add-product status failed.
        /// </summary>
        DatabaseExceptionOccurredTicketAddproductStatusFailed = 1150,

        /// <summary>
        /// Purging ticket add-products.
        /// </summary>
        PurgingTicketAddproducts = 1151,

        /// <summary>
        /// Purging ticket add-products complete.
        /// </summary>
        PurgingTicketAddproductsComplete = 1152,

        /// <summary>
        /// Database exception occurred. Purging ticket add-products complete.
        /// </summary>
        DatabaseExceptionOccurredPurgingTicketAddproductsComplete = 1153,

        /// <summary>
        /// Saving ticket add-products.
        /// </summary>
        SavingTicketAddproducts = 1154,

        /// <summary>
        /// Saving ticket add-products complete.
        /// </summary>
        SavingTicketAddproductsComplete = 1155,

        /// <summary>
        /// Saving ticket add-products failed.
        /// </summary>
        SavingTicketAddproductsFailed = 1156,

        /// <summary>
        /// Database exception occurred. Saving ticket add-products failed.
        /// </summary>
        DatabaseExceptionOccurredSavingTicketAddproductsFailed = 1157,

        /// <summary>
        /// Seeding ticket add-product details.
        /// </summary>
        SeedingTicketAddproductDetails = 1158,

        /// <summary>
        /// Seeding ticket add-product details complete.
        /// </summary>
        SeedingTicketAddproductDetailsComplete = 1159,

        /// <summary>
        /// Seeding ticket add-product details failed.
        /// </summary>
        SeedingTicketAddproductDetailsFailed = 1160,

        /// <summary>
        /// Database exception occurred. Seeding ticket add-product details failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingTicketAddproductDetailsFailed = 1161,

        /// <summary>
        /// {0} ticket add-product details.
        /// </summary>
        TicketAddproductDetails = 1162,

        /// <summary>
        /// Database exception occurred. Ticket add-product details status failed.
        /// </summary>
        DatabaseExceptionOccurredTicketAddproductDetailsStatusFailed = 1163,

        /// <summary>
        /// Purging ticket add-product details.
        /// </summary>
        PurgingTicketAddproductDetails = 1164,

        /// <summary>
        /// Purging ticket add-product details complete.
        /// </summary>
        PurgingTicketAddproductDetailsComplete = 1165,

        /// <summary>
        /// Purging ticket add-product details failed.
        /// </summary>
        PurgingTicketAddproductDetailsFailed = 1166,

        /// <summary>
        /// Database exception occurred. Purging ticket add-product details failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingTicketAddproductDetailsFailed = 1167,

        /// <summary>
        /// drItemFinished.Read() failed.
        /// </summary>
        DritemfinishedreadFailed = 1168,

        /// <summary>
        /// Unable to locate the activity item ticket finished {0} in tActivityItemProductionFinished.
        /// </summary>
        UnableToLocateTheActivityItemTicketFinishedInTactivityitemproductionfinished = 1169,

        /// <summary>
        /// Saving ticket add-product details.
        /// </summary>
        SavingTicketAddproductDetails = 1170,

        /// <summary>
        /// drItemProduction.Read() failed.
        /// </summary>
        DritemproductionreadFailed = 1171,

        /// <summary>
        /// Unable to Locate the activity item refno in 
        /// </summary>
        UnableToLocateTheActivityItemRefnoInz = 1172,

        /// <summary>
        /// Saving ticket add-product details complete.
        /// </summary>
        SavingTicketAddproductDetailsComplete = 1173,

        /// <summary>
        /// Saving ticket add-product details failed.
        /// </summary>
        SavingTicketAddproductDetailsFailed = 1174,

        /// <summary>
        /// Database exception occurred. Saving ticket add-product details failed.
        /// </summary>
        DatabaseExceptionOccurredSavingTicketAddproductDetailsFailed = 1175,

        /// <summary>
        /// Seeding ticket add-product items.
        /// </summary>
        SeedingTicketAddproductItems = 1176,

        /// <summary>
        /// Seeding ticket add-product items complete.
        /// </summary>
        SeedingTicketAddproductItemsComplete = 1177,

        /// <summary>
        /// Seeding ticket add-product items failed.
        /// </summary>
        SeedingTicketAddproductItemsFailed = 1178,

        /// <summary>
        /// Database exception occurred. Seeding ticket add-product items failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingTicketAddproductItemsFailed = 1179,

        /// <summary>
        /// Purging ticket add-product items.
        /// </summary>
        PurgingTicketAddproductItems = 1180,

        /// <summary>
        /// Purging ticket add-product items complete.
        /// </summary>
        PurgingTicketAddproductItemsComplete = 1181,

        /// <summary>
        /// Purging ticket add-product items failed.
        /// </summary>
        PurgingTicketAddproductItemsFailed = 1182,

        /// <summary>
        /// Database exception occurred. Purging ticket add-product items failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingTicketAddproductItemsFailed = 1183,

        /// <summary>
        /// {0} ticket add-product items.
        /// </summary>
        TicketAddproductItems = 1184,

        /// <summary>
        /// Database exception occurred. Ticket add-product items status failed.
        /// </summary>
        DatabaseExceptionOccurredTicketAddproductItemsStatusFailed = 1185,

        /// <summary>
        /// drByProduct.Read() failed.
        /// </summary>
        DrbyproductreadFailed = 1186,

        /// <summary>
        /// Unable to locate the ticket add-product refno {0} in tActivityProductionByProduct.
        /// </summary>
        UnableToLocateTheTicketAddproductRefnoInTactivityproductionbyproduct = 1187,

        /// <summary>
        /// Saving ticket add-product items.
        /// </summary>
        SavingTicketAddproductItems = 1188,

        /// <summary>
        /// Saving ticket add-product items complete.
        /// </summary>
        SavingTicketAddproductItemsComplete = 1189,

        /// <summary>
        /// Saving ticket add-product items failed.
        /// </summary>
        SavingTicketAddproductItemsFailed = 1190,

        /// <summary>
        /// Database exception occurred. Saving ticket add-product items failed.
        /// </summary>
        DatabaseExceptionOccurredSavingTicketAddproductItemsFailed = 1191,

        /// <summary>
        /// Seeding finished tickets.
        /// </summary>
        SeedingFinishedTickets = 1192,

        /// <summary>
        /// Seeding finished tickets complete.
        /// </summary>
        SeedingFinishedTicketsComplete = 1193,

        /// <summary>
        /// Seeding finished tickets failed.
        /// </summary>
        SeedingFinishedTicketsFailed = 1194,

        /// <summary>
        /// Database exception occurred. Seeding finished tickets failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingFinishedTicketsFailed = 1195,

        /// <summary>
        /// {0} finished tickets.
        /// </summary>
        FinishedTickets = 1196,

        /// <summary>
        /// Database exception occurred. Finished tickets status failed.
        /// </summary>
        DatabaseExceptionOccurredFinishedTicketsStatusFailed = 1197,

        /// <summary>
        /// Purging finished ticket.
        /// </summary>
        PurgingFinishedTicket = 1198,

        /// <summary>
        /// Purging finished ticket complete.
        /// </summary>
        PurgingFinishedTicketComplete = 1199,

        /// <summary>
        /// Database exception occurred. Purging finished ticket complete.
        /// </summary>
        DatabaseExceptionOccurredPurgingFinishedTicketComplete = 1200,

        /// <summary>
        /// Saving finished tickets.
        /// </summary>
        SavingFinishedTickets = 1201,

        /// <summary>
        /// Saving finished tickets complete.
        /// </summary>
        SavingFinishedTicketsComplete = 1202,

        /// <summary>
        /// Saving finished tickets failed.
        /// </summary>
        SavingFinishedTicketsFailed = 1203,

        /// <summary>
        /// Database exception occurred. Saving finished tickets failed.
        /// </summary>
        DatabaseExceptionOccurredSavingFinishedTicketsFailed = 1204,

        /// <summary>
        /// Seeding finished ticket details.
        /// </summary>
        SeedingFinishedTicketDetails = 1205,

        /// <summary>
        /// Seeding finished ticket details complete.
        /// </summary>
        SeedingFinishedTicketDetailsComplete = 1206,

        /// <summary>
        /// Seeding finished ticket details failed.
        /// </summary>
        SeedingFinishedTicketDetailsFailed = 1207,

        /// <summary>
        /// Database exception occurred. Seeding finished ticket details failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingFinishedTicketDetailsFailed = 1208,

        /// <summary>
        /// Purging finished ticket details.
        /// </summary>
        PurgingFinishedTicketDetails = 1209,

        /// <summary>
        /// Purging finished ticket details failed.
        /// </summary>
        PurgingFinishedTicketDetailsFailed = 1210,

        /// <summary>
        /// Database exception occurred. Purging finished ticket details failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingFinishedTicketDetailsFailed = 1211,

        /// <summary>
        /// {0} finished ticket details.
        /// </summary>
        FinishedTicketDetails = 1212,

        /// <summary>
        /// Database exception occurred. Finished ticket details status failed.
        /// </summary>
        DatabaseExceptionOccurredFinishedTicketDetailsStatusFailed = 1213,

        /// <summary>
        /// Saving finished ticket details.
        /// </summary>
        SavingFinishedTicketDetails = 1214,

        /// <summary>
        /// Saving finished ticket details complete.
        /// </summary>
        SavingFinishedTicketDetailsComplete = 1215,

        /// <summary>
        /// Saving finished ticket details failed.
        /// </summary>
        SavingFinishedTicketDetailsFailed = 1216,

        /// <summary>
        /// Database exception occurred. Saving finished ticket details failed.
        /// </summary>
        DatabaseExceptionOccurredSavingFinishedTicketDetailsFailed = 1217,

        /// <summary>
        /// Seeding finished ticket items.
        /// </summary>
        SeedingFinishedTicketItems = 1218,

        /// <summary>
        /// Seeding finished ticket items complete.
        /// </summary>
        SeedingFinishedTicketItemsComplete = 1219,

        /// <summary>
        /// Seeding finished ticket items failed.
        /// </summary>
        SeedingFinishedTicketItemsFailed = 1220,

        /// <summary>
        /// Database exception occurred. Seeding finished ticket items failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingFinishedTicketItemsFailed = 1221,

        /// <summary>
        /// Purging finished ticket items.
        /// </summary>
        PurgingFinishedTicketItems = 1222,

        /// <summary>
        /// Purging finished ticket items complete.
        /// </summary>
        PurgingFinishedTicketItemsComplete = 1223,

        /// <summary>
        /// Purging finished ticket items failed.
        /// </summary>
        PurgingFinishedTicketItemsFailed = 1224,

        /// <summary>
        /// Database exception occurred. Purging finished ticket items failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingFinishedTicketItemsFailed = 1225,

        /// <summary>
        /// {0} finished ticket items.
        /// </summary>
        FinishedTicketItems = 1226,

        /// <summary>
        /// Database exception occurred. Finished ticket items status failed.
        /// </summary>
        DatabaseExceptionOccurredFinishedTicketItemsStatusFailed = 1227,

        /// <summary>
        /// drFinished.Read() failed.
        /// </summary>
        DrfinishedreadFailed = 1228,

        /// <summary>
        /// Unable to locate the activity production ticket finished refno {0} in 
        /// </summary>
        UnableToLocateTheActivityProductionTicketFinishedRefnoIn = 1229,

        /// <summary>
        /// Saving finished ticket items.
        /// </summary>
        SavingFinishedTicketItems = 1230,

        /// <summary>
        /// Saving finished ticket items complete.
        /// </summary>
        SavingFinishedTicketItemsComplete = 1231,

        /// <summary>
        /// Saving finished ticket items failed.
        /// </summary>
        SavingFinishedTicketItemsFailed = 1232,

        /// <summary>
        /// Database exception occurred. Saving finished ticket items failed.
        /// </summary>
        DatabaseExceptionOccurredSavingFinishedTicketItemsFailed = 1233,

        /// <summary>
        /// Seeding ticket materials.
        /// </summary>
        SeedingTicketMaterials = 1234,

        /// <summary>
        /// Seeding ticket materials complete.
        /// </summary>
        SeedingTicketMaterialsComplete = 1235,

        /// <summary>
        /// Seeding ticket materials failed.
        /// </summary>
        SeedingTicketMaterialsFailed = 1236,

        /// <summary>
        /// {0} ticket materials.
        /// </summary>
        TicketMaterialsz = 1237,

        /// <summary>
        /// Database exception occurred. Material tickets status failed.
        /// </summary>
        DatabaseExceptionOccurredMaterialTicketsStatusFailed = 1238,

        /// <summary>
        /// Purging ticket materials.
        /// </summary>
        PurgingTicketMaterials = 1239,

        /// <summary>
        /// Purging ticket materials complete.
        /// </summary>
        PurgingTicketMaterialsComplete = 1240,

        /// <summary>
        /// Purging ticket materials failed.
        /// </summary>
        PurgingTicketMaterialsFailed = 1241,

        /// <summary>
        /// Database exception occurred. Purging ticket materials failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingTicketMaterialsFailed = 1242,

        /// <summary>
        /// Saving ticket materials.
        /// </summary>
        SavingTicketMaterials = 1243,

        /// <summary>
        /// Saving ticket materials complete.
        /// </summary>
        SavingTicketMaterialsComplete = 1244,

        /// <summary>
        /// Saving ticket materials failed.
        /// </summary>
        SavingTicketMaterialsFailed = 1245,

        /// <summary>
        /// Database exception occurred. Saving ticket materials failed.
        /// </summary>
        DatabaseExceptionOccurredSavingTicketMaterialsFailed = 1246,

        /// <summary>
        /// Seeding ticket material details.
        /// </summary>
        SeedingTicketMaterialDetails = 1247,

        /// <summary>
        /// Seeding ticket material details complete.
        /// </summary>
        SeedingTicketMaterialDetailsComplete = 1248,

        /// <summary>
        /// Seeding ticket materials details failed.
        /// </summary>
        SeedingTicketMaterialsDetailsFailed = 1249,

        /// <summary>
        /// Database exception occurred. Seeding ticket materials details failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingTicketMaterialsDetailsFailed = 1250,

        /// <summary>
        /// {0} ticket material details.
        /// </summary>
        TicketMaterialDetails = 1251,

        /// <summary>
        /// Database exception occurred. Material ticket details status failed.
        /// </summary>
        DatabaseExceptionOccurredMaterialTicketDetailsStatusFailed = 1252,

        /// <summary>
        /// Purging ticket material details.
        /// </summary>
        PurgingTicketMaterialDetails = 1253,

        /// <summary>
        /// Purging ticket material details complete.
        /// </summary>
        PurgingTicketMaterialDetailsComplete = 1254,

        /// <summary>
        /// Purging ticket material details failed.
        /// </summary>
        PurgingTicketMaterialDetailsFailed = 1255,

        /// <summary>
        /// Database exception occurred. Purging ticket material details failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingTicketMaterialDetailsFailed = 1256,

        /// <summary>
        /// drActItemMaterial.Read() failed.
        /// </summary>
        DractitemmaterialreadFailed = 1257,

        /// <summary>
        /// Unable to locate the activity item production ticket material refno {0} in 
        /// </summary>
        UnableToLocateTheActivityItemProductionTicketMaterialRefnoIn = 1258,

        /// <summary>
        /// Saving ticket material details.
        /// </summary>
        SavingTicketMaterialDetails = 1259,

        /// <summary>
        /// Saving ticket material details complete.
        /// </summary>
        SavingTicketMaterialDetailsComplete = 1260,

        /// <summary>
        /// Saving ticket material details failed.
        /// </summary>
        SavingTicketMaterialDetailsFailed = 1261,

        /// <summary>
        /// Database exception occurred. Saving ticket material details failed.
        /// </summary>
        DatabaseExceptionOccurredSavingTicketMaterialDetailsFailed = 1262,

        /// <summary>
        /// Seeding ticket material items.
        /// </summary>
        SeedingTicketMaterialItems = 1263,

        /// <summary>
        /// Seeding ticket material items complete.
        /// </summary>
        SeedingTicketMaterialItemsComplete = 1264,

        /// <summary>
        /// Seeding ticket material items failed.
        /// </summary>
        SeedingTicketMaterialItemsFailed = 1265,

        /// <summary>
        /// Database exception occurred. Seeding ticket material items failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingTicketMaterialItemsFailed = 1266,

        /// <summary>
        /// {0} ticket material items.
        /// </summary>
        TicketMaterialItems = 1267,

        /// <summary>
        /// Database exception occurred. Material ticket items status failed.
        /// </summary>
        DatabaseExceptionOccurredMaterialTicketItemsStatusFailed = 1268,

        /// <summary>
        /// Purging ticket material items.
        /// </summary>
        PurgingTicketMaterialItems = 1269,

        /// <summary>
        /// Purging ticket material items complete.
        /// </summary>
        PurgingTicketMaterialItemsComplete = 1270,

        /// <summary>
        /// Purging ticket material items failed.
        /// </summary>
        PurgingTicketMaterialItemsFailed = 1271,

        /// <summary>
        /// Database exception occurred. Purging ticket material items failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingTicketMaterialItemsFailed = 1272,

        /// <summary>
        /// drActMaterial.Read() failed.
        /// </summary>
        DractmaterialreadFailed = 1273,

        /// <summary>
        /// Unable to locate the activity production ticket material refno {0} in 
        /// </summary>
        UnableToLocateTheActivityProductionTicketMaterialRefnoIn = 1274,

        /// <summary>
        /// Saving ticket material items.
        /// </summary>
        SavingTicketMaterialItems = 1275,

        /// <summary>
        /// Saving ticket material items complete.
        /// </summary>
        SavingTicketMaterialItemsComplete = 1276,

        /// <summary>
        /// Saving ticket material items failed.
        /// </summary>
        SavingTicketMaterialItemsFailed = 1277,

        /// <summary>
        /// Database exception occurred. Saving ticket material items failed.
        /// </summary>
        DatabaseExceptionOccurredSavingTicketMaterialItemsFailed = 1278,

        /// <summary>
        /// Seeding work sheet items.
        /// </summary>
        SeedingWorkSheetItems = 1279,

        /// <summary>
        /// Seeding work sheet items complete.
        /// </summary>
        SeedingWorkSheetItemsComplete = 1280,

        /// <summary>
        /// Seeding work sheet items failed.
        /// </summary>
        SeedingWorkSheetItemsFailed = 1281,

        /// <summary>
        /// {0} work sheet items.
        /// </summary>
        WorkSheetItems = 1282,

        /// <summary>
        /// Database exception occurred. Work sheet status failed.
        /// </summary>
        DatabaseExceptionOccurredWorkSheetStatusFailed = 1283,

        /// <summary>
        /// Purging work sheet items.
        /// </summary>
        PurgingWorkSheetItems = 1284,

        /// <summary>
        /// Purging work sheet items complete.
        /// </summary>
        PurgingWorkSheetItemsComplete = 1285,

        /// <summary>
        /// Purging work sheet items failed.
        /// </summary>
        PurgingWorkSheetItemsFailed = 1286,

        /// <summary>
        /// Database exception occurred. Purging work sheet items failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingWorkSheetItemsFailed = 1287,

        /// <summary>
        /// Saving work sheet items.
        /// </summary>
        SavingWorkSheetItems = 1288,

        /// <summary>
        /// Saving work sheet items complete.
        /// </summary>
        SavingWorkSheetItemsComplete = 1289,

        /// <summary>
        /// Saving work sheet items failed.
        /// </summary>
        SavingWorkSheetItemsFailed = 1290,

        /// <summary>
        /// Database exception occurred. Saving work sheet items failed.
        /// </summary>
        DatabaseExceptionOccurredSavingWorkSheetItemsFailed = 1291,

        /// <summary>
        /// Seeding receivings.
        /// </summary>
        SeedingReceivings = 1292,

        /// <summary>
        /// Seeding receivings complete.
        /// </summary>
        SeedingReceivingsComplete = 1293,

        /// <summary>
        /// Seeding receivings failed.
        /// </summary>
        SeedingReceivingsFailed = 1294,

        /// <summary>
        /// {0} receivings.
        /// </summary>
        Receivings = 1295,

        /// <summary>
        /// Database exception occurred. Receivings status failed.
        /// </summary>
        DatabaseExceptionOccurredReceivingsStatusFailed = 1296,

        /// <summary>
        /// Purging receivings.
        /// </summary>
        PurgingReceivings = 1297,

        /// <summary>
        /// Purging receivings completed.
        /// </summary>
        PurgingReceivingsCompleted = 1298,

        /// <summary>
        /// Purging receivings failed.
        /// </summary>
        PurgingReceivingsFailed = 1299,

        /// <summary>
        /// Database exception occurred. Purging receivings failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingReceivingsFailed = 1300,

        /// <summary>
        /// Saving receivings.
        /// </summary>
        SavingReceivings = 1301,

        /// <summary>
        /// Saving receivings complete.
        /// </summary>
        SavingReceivingsComplete = 1302,

        /// <summary>
        /// Saving receivings failed.
        /// </summary>
        SavingReceivingsFailed = 1303,

        /// <summary>
        /// Database exception occurred. Saving receivings failed.
        /// </summary>
        DatabaseExceptionOccurredSavingReceivingsFailed = 1304,

        /// <summary>
        /// Seeding receiving details.
        /// </summary>
        SeedingReceivingDetails = 1305,

        /// <summary>
        /// Seeding receiving details complete.
        /// </summary>
        SeedingReceivingDetailsComplete = 1306,

        /// <summary>
        /// Seeding receiving details failed.
        /// </summary>
        SeedingReceivingDetailsFailed = 1307,

        /// <summary>
        /// Database exception occurred. Seeding receiving details failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingReceivingDetailsFailed = 1308,

        /// <summary>
        /// Purging receiving details.
        /// </summary>
        PurgingReceivingDetails = 1309,

        /// <summary>
        /// Purging receiving details complete.
        /// </summary>
        PurgingReceivingDetailsComplete = 1310,

        /// <summary>
        /// Purging receiving details failed.
        /// </summary>
        PurgingReceivingDetailsFailed = 1311,

        /// <summary>
        /// Database exception occurred. Purging receiving details failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingReceivingDetailsFailed = 1312,

        /// <summary>
        /// {0} receiving details.
        /// </summary>
        ReceivingDetails = 1313,

        /// <summary>
        /// Database exception occurred. Receiving details status failed.
        /// </summary>
        DatabaseExceptionOccurredReceivingDetailsStatusFailed = 1314,

        /// <summary>
        /// drItemRecv.Read() failed.
        /// </summary>
        DritemrecvreadFailed = 1315,

        /// <summary>
        /// Unable to locate the item receiving refno {0} in tActivityItemReceiving
        /// </summary>
        UnableToLocateTheItemReceivingRefnoInTactivityitemreceiving = 1316,

        /// <summary>
        /// Saving receiving details.
        /// </summary>
        SavingReceivingDetails = 1317,

        /// <summary>
        /// !drItemRecv.Read() failed.
        /// </summary>
        DritemrecvreadFailedz = 1318,

        /// <summary>
        /// Unable to locate the item refno {0} in tActivityItemReceiving
        /// </summary>
        UnableToLocateTheItemRefnoInTactivityitemreceiving = 1319,

        /// <summary>
        /// Saving receiving details complete.
        /// </summary>
        SavingReceivingDetailsComplete = 1320,

        /// <summary>
        /// Saving receiving details failed.
        /// </summary>
        SavingReceivingDetailsFailed = 1321,

        /// <summary>
        /// Database exception occurred. Saving receiving details failed.
        /// </summary>
        DatabaseExceptionOccurredSavingReceivingDetailsFailed = 1322,

        /// <summary>
        /// Seeding receiving items.
        /// </summary>
        SeedingReceivingItems = 1323,

        /// <summary>
        /// Seeding receiving items complete.
        /// </summary>
        SeedingReceivingItemsComplete = 1324,

        /// <summary>
        /// Seeding receiving items failed.
        /// </summary>
        SeedingReceivingItemsFailed = 1325,

        /// <summary>
        /// Database exception occurred. Seeding receiving items failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingReceivingItemsFailed = 1326,

        /// <summary>
        /// {0} receiving items.
        /// </summary>
        ReceivingItems = 1327,

        /// <summary>
        /// Database exception occurred. Receiving items status failed.
        /// </summary>
        DatabaseExceptionOccurredReceivingItemsStatusFailed = 1328,

        /// <summary>
        /// Purging receiving items.
        /// </summary>
        PurgingReceivingItems = 1329,

        /// <summary>
        /// Purging receiving items complete.
        /// </summary>
        PurgingReceivingItemsComplete = 1330,

        /// <summary>
        /// Purging receiving items failed.
        /// </summary>
        PurgingReceivingItemsFailed = 1331,

        /// <summary>
        /// Database exception occurred. Purging receiving items failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingReceivingItemsFailed = 1332,

        /// <summary>
        /// drRecv.Read() failed.
        /// </summary>
        DrrecvreadFailed = 1333,

        /// <summary>
        /// Unable to locate the receiving activity refno {0} in tActivityReceiving
        /// </summary>
        UnableToLocateTheReceivingActivityRefnoInTactivityreceiving = 1334,

        /// <summary>
        /// Saving receiving items.
        /// </summary>
        SavingReceivingItems = 1335,

        /// <summary>
        /// Saving receiving items complete.
        /// </summary>
        SavingReceivingItemsComplete = 1336,

        /// <summary>
        /// Saving receiving items failed.
        /// </summary>
        SavingReceivingItemsFailed = 1337,

        /// <summary>
        /// Database exception occurred. Saving receiving items failed.
        /// </summary>
        DatabaseExceptionOccurredSavingReceivingItemsFailed = 1338,

        /// <summary>
        /// drProduct.Read() failed.
        /// </summary>
        DrproductreadFailed = 1339,

        /// <summary>
        /// Unable to locate the product refno from the product fk refno in tProduct.
        /// </summary>
        UnableToLocateTheProductRefnoFromTheProductFkRefnoInTproduct = 1340,

        /// <summary>
        /// drUnit.Read() failed.
        /// </summary>
        DrunitreadFailed = 1341,

        /// <summary>
        /// Unable to locate the unit refno from the xref unit code in  tUnit.
        /// </summary>
        UnableToLocateTheUnitRefnoFromTheXrefUnitCodeInTunit = 1342,

        /// <summary>
        /// drProdUnit.Read() failed.
        /// </summary>
        DrprodunitreadFailed = 1343,

        /// <summary>
        /// Unable to locate the product unit refno from the product fk refno, bc and xref unit code in tProductUnit.
        /// </summary>
        UnableToLocateTheProductUnitRefnoFromTheProductFkRefnoBcAndXrefUnitCodeInTproductunit = 1344,

        /// <summary>
        /// Purging receiving item comments.
        /// </summary>
        PurgingReceivingItemComments = 1345,

        /// <summary>
        /// Purging receiving item comments complete.
        /// </summary>
        PurgingReceivingItemCommentsComplete = 1346,

        /// <summary>
        /// Purging receiving item comments failed.
        /// </summary>
        PurgingReceivingItemCommentsFailed = 1347,

        /// <summary>
        /// Database exception occurred. Purging receiving item comments failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingReceivingItemCommentsFailed = 1348,

        /// <summary>
        /// Seeding sales returns.
        /// </summary>
        SeedingSalesReturns = 1349,

        /// <summary>
        /// Seeding sales returns complete.
        /// </summary>
        SeedingSalesReturnsComplete = 1350,

        /// <summary>
        /// Seeding sales returns failed.
        /// </summary>
        SeedingSalesReturnsFailed = 1351,

        /// <summary>
        /// Purging sales returns.
        /// </summary>
        PurgingSalesReturns = 1352,

        /// <summary>
        /// Purging sales returns failed.
        /// </summary>
        PurgingSalesReturnsFailed = 1353,

        /// <summary>
        /// Database exception occurred. Purging sales returns failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingSalesReturnsFailed = 1354,

        /// <summary>
        /// {0} sales returns.
        /// </summary>
        SalesReturns = 1355,

        /// <summary>
        /// Database exception occurred. Sales returns status failed.
        /// </summary>
        DatabaseExceptionOccurredSalesReturnsStatusFailed = 1356,

        /// <summary>
        /// Saving sales returns.
        /// </summary>
        SavingSalesReturns = 1357,

        /// <summary>
        /// Saving sales returns complete.
        /// </summary>
        SavingSalesReturnsComplete = 1358,

        /// <summary>
        /// Saving sales returns failed.
        /// </summary>
        SavingSalesReturnsFailed = 1359,

        /// <summary>
        /// Database exception occurred. Saving sales returns failed.
        /// </summary>
        DatabaseExceptionOccurredSavingSalesReturnsFailed = 1360,

        /// <summary>
        /// Seeding sales return details.
        /// </summary>
        SeedingSalesReturnDetails = 1361,

        /// <summary>
        /// Seeding sales return details complete.
        /// </summary>
        SeedingSalesReturnDetailsComplete = 1362,

        /// <summary>
        /// Seeding sales return details failed.
        /// </summary>
        SeedingSalesReturnDetailsFailed = 1363,

        /// <summary>
        /// Database exception occurred. Seeding sales return details failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingSalesReturnDetailsFailed = 1364,

        /// <summary>
        /// Purging sales return details.
        /// </summary>
        PurgingSalesReturnDetails = 1365,

        /// <summary>
        /// Purging sales return details complete.
        /// </summary>
        PurgingSalesReturnDetailsComplete = 1366,

        /// <summary>
        /// Purging sales return details failed.
        /// </summary>
        PurgingSalesReturnDetailsFailed = 1367,

        /// <summary>
        /// Database exception occurred. Purging sales return details failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingSalesReturnDetailsFailed = 1368,

        /// <summary>
        /// {0} sales return details.
        /// </summary>
        SalesReturnDetails = 1369,

        /// <summary>
        /// Database exception occurred. Sales return details status failed.
        /// </summary>
        DatabaseExceptionOccurredSalesReturnDetailsStatusFailed = 1370,

        /// <summary>
        /// drItemReturns.Read() failed.
        /// </summary>
        DritemreturnsreadFailed = 1371,

        /// <summary>
        /// Unable to locate the item returns refno {0} in tActivityItemReturns.
        /// </summary>
        UnableToLocateTheItemReturnsRefnoInTactivityitemreturns = 1372,

        /// <summary>
        /// Saving sales return details.
        /// </summary>
        SavingSalesReturnDetails = 1373,

        /// <summary>
        /// drItemRet.Read() failed.
        /// </summary>
        DritemretreadFailed = 1374,

        /// <summary>
        /// Unable to locate the returns item refno {0} in tActivityItemReturns
        /// </summary>
        UnableToLocateTheReturnsItemRefnoInTactivityitemreturns = 1375,

        /// <summary>
        /// Saving sales return details complete.
        /// </summary>
        SavingSalesReturnDetailsComplete = 1376,

        /// <summary>
        /// Saving sales return details failed.
        /// </summary>
        SavingSalesReturnDetailsFailed = 1377,

        /// <summary>
        /// Database exception occurred. Saving sales return details failed.
        /// </summary>
        DatabaseExceptionOccurredSavingSalesReturnDetailsFailed = 1378,

        /// <summary>
        /// Seeding sales return items.
        /// </summary>
        SeedingSalesReturnItems = 1379,

        /// <summary>
        /// Seeding sales return items complete.
        /// </summary>
        SeedingSalesReturnItemsComplete = 1380,

        /// <summary>
        /// Seeding sales return items failed.
        /// </summary>
        SeedingSalesReturnItemsFailed = 1381,

        /// <summary>
        /// Database exception occurred. Seeding sales return items failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingSalesReturnItemsFailed = 1382,

        /// <summary>
        /// Purging sales return items.
        /// </summary>
        PurgingSalesReturnItems = 1383,

        /// <summary>
        /// Purging sales return items complete.
        /// </summary>
        PurgingSalesReturnItemsComplete = 1384,

        /// <summary>
        /// Purging sales return items failed.
        /// </summary>
        PurgingSalesReturnItemsFailed = 1385,

        /// <summary>
        /// Database exception occurred. Purging sales return items failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingSalesReturnItemsFailed = 1386,

        /// <summary>
        /// {0} Sales return items.
        /// </summary>
        SalesReturnItems = 1387,

        /// <summary>
        /// Database exception occurred. Sales return items status failed.
        /// </summary>
        DatabaseExceptionOccurredSalesReturnItemsStatusFailed = 1388,

        /// <summary>
        /// drReturns.Read() failed.
        /// </summary>
        DrreturnsreadFailed = 1389,

        /// <summary>
        /// Unable to locate the sales return refno {0} in tActivityReturns
        /// </summary>
        UnableToLocateTheSalesReturnRefnoInTactivityreturns = 1390,

        /// <summary>
        /// Saving sales return items.
        /// </summary>
        SavingSalesReturnItems = 1391,

        /// <summary>
        /// Saving sales return items complete.
        /// </summary>
        SavingSalesReturnItemsComplete = 1392,

        /// <summary>
        /// Saving sales return items failed.
        /// </summary>
        SavingSalesReturnItemsFailed = 1393,

        /// <summary>
        /// Database exception occurred. Saving sales return items failed.
        /// </summary>
        DatabaseExceptionOccurredSavingSalesReturnItemsFailed = 1394,

        /// <summary>
        /// Purging sales return item comments.
        /// </summary>
        PurgingSalesReturnItemComments = 1395,

        /// <summary>
        /// Purging sales return item comments complete.
        /// </summary>
        PurgingSalesReturnItemCommentsComplete = 1396,

        /// <summary>
        /// Purging sales return item comments failed.
        /// </summary>
        PurgingSalesReturnItemCommentsFailed = 1397,

        /// <summary>
        /// Database exception occurred. Purging sales return item comments failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingSalesReturnItemCommentsFailed = 1398,

        /// <summary>
        /// Seeding transfers.
        /// </summary>
        SeedingTransfers = 1399,

        /// <summary>
        /// Seeding transfers complete.
        /// </summary>
        SeedingTransfersComplete = 1400,

        /// <summary>
        /// Seeding transfers failed.
        /// </summary>
        SeedingTransfersFailed = 1401,

        /// <summary>
        /// {0} transfers.
        /// </summary>
        Transferszz = 1402,

        /// <summary>
        /// Database exception occurred. Transfers status failed.
        /// </summary>
        DatabaseExceptionOccurredTransfersStatusFailed = 1403,

        /// <summary>
        /// Purging transfers.
        /// </summary>
        PurgingTransfers = 1404,

        /// <summary>
        /// Purging transfers complete.
        /// </summary>
        PurgingTransfersComplete = 1405,

        /// <summary>
        /// Purging transfers failed.
        /// </summary>
        PurgingTransfersFailed = 1406,

        /// <summary>
        /// Database exception occurred. Purging transfers failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingTransfersFailed = 1407,

        /// <summary>
        /// Saving transfers.
        /// </summary>
        SavingTransfers = 1408,

        /// <summary>
        /// drWhs.Read() failed.
        /// </summary>
        DrwhsreadFailed = 1409,

        /// <summary>
        /// Unable to locate the warehouse fk refno in tLocationMapWarehouse.
        /// </summary>
        UnableToLocateTheWarehouseFkRefnoInTlocationmapwarehouse = 1410,

        /// <summary>
        /// Saving transfers complete.
        /// </summary>
        SavingTransfersComplete = 1411,

        /// <summary>
        /// Saving transfers failed.
        /// </summary>
        SavingTransfersFailed = 1412,

        /// <summary>
        /// Uniqueness Violation.
        /// </summary>
        UniquenessViolationz = 1413,

        /// <summary>
        /// Database exception occurred. Saving transfers failed.
        /// </summary>
        DatabaseExceptionOccurredSavingTransfersFailed = 1414,

        /// <summary>
        /// Seeding transfer details.
        /// </summary>
        SeedingTransferDetails = 1415,

        /// <summary>
        /// Seeding transfer details complete.
        /// </summary>
        SeedingTransferDetailsComplete = 1416,

        /// <summary>
        /// Seeding co-product materials details failed.
        /// </summary>
        SeedingCoproductMaterialsDetailsFailed = 1417,

        /// <summary>
        /// Database exception occurred. Seeding transfer details failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingTransferDetailsFailed = 1418,

        /// <summary>
        /// Seeding transfer details failed.
        /// </summary>
        SeedingTransferDetailsFailed = 1419,

        /// <summary>
        /// {0} transfer details.
        /// </summary>
        TransferDetails = 1420,

        /// <summary>
        /// Database exception occurred. Transfer details status failed.
        /// </summary>
        DatabaseExceptionOccurredTransferDetailsStatusFailed = 1421,

        /// <summary>
        /// Purging transfer details.
        /// </summary>
        PurgingTransferDetails = 1422,

        /// <summary>
        /// Purging transfer details complete.
        /// </summary>
        PurgingTransferDetailsComplete = 1423,

        /// <summary>
        /// Purging transfer details failed.
        /// </summary>
        PurgingTransferDetailsFailed = 1424,

        /// <summary>
        /// Database exception occurred. Purging transfer details failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingTransferDetailsFailed = 1425,

        /// <summary>
        /// drItem.Read() failed.
        /// </summary>
        DritemreadFailed = 1426,

        /// <summary>
        /// Unable to locate to activity item transfer refno {0} in tActivityItemTransfer.
        /// </summary>
        UnableToLocateToActivityItemTransferRefnoInTactivityitemtransfer = 1427,

        /// <summary>
        /// Saving transfer details.
        /// </summary>
        SavingTransferDetails = 1428,

        /// <summary>
        /// drItemTransfer.Read() failed.
        /// </summary>
        DritemtransferreadFailed = 1429,

        /// <summary>
        /// Unable to locate the activity item transfer refno {0} 
        /// </summary>
        UnableToLocateTheActivityItemTransferRefno = 1430,

        /// <summary>
        /// in tActivityItemTransfer.
        /// </summary>
        InTactivityitemtransfer = 1431,

        /// <summary>
        /// Saving transfer details failed.
        /// </summary>
        SavingTransferDetailsFailed = 1432,

        /// <summary>
        /// Database exception occurred. Saving transfer details failed.
        /// </summary>
        DatabaseExceptionOccurredSavingTransferDetailsFailed = 1433,

        /// <summary>
        /// Seeding transfer items.
        /// </summary>
        SeedingTransferItems = 1434,

        /// <summary>
        /// Seeding transfer items complete.
        /// </summary>
        SeedingTransferItemsComplete = 1435,

        /// <summary>
        /// Seeding transfer items failed.
        /// </summary>
        SeedingTransferItemsFailed = 1436,

        /// <summary>
        /// Database exception occurred. Seeding transfer items failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingTransferItemsFailed = 1437,

        /// <summary>
        /// Purging transfer items.
        /// </summary>
        PurgingTransferItems = 1438,

        /// <summary>
        /// Purging transfer items complete.
        /// </summary>
        PurgingTransferItemsComplete = 1439,

        /// <summary>
        /// Purging transfer items failed.
        /// </summary>
        PurgingTransferItemsFailed = 1440,

        /// <summary>
        /// Database exception occurred. Purging transfer items failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingTransferItemsFailed = 1441,

        /// <summary>
        /// {0} transfer items.
        /// </summary>
        TransferItems = 1442,

        /// <summary>
        /// Database exception occurred. Transfer items status failed.
        /// </summary>
        DatabaseExceptionOccurredTransferItemsStatusFailed = 1443,

        /// <summary>
        /// drTrans.Read() failed.
        /// </summary>
        DrtransreadFailed = 1444,

        /// <summary>
        /// Unable to locate the transfer refno {0} in tActivityTransfer.
        /// </summary>
        UnableToLocateTheTransferRefnoInTactivitytransfer = 1445,

        /// <summary>
        /// Saving transfer items.
        /// </summary>
        SavingTransferItems = 1446,

        /// <summary>
        /// Unable to indentify the location map reference number.
        /// </summary>
        UnableToIndentifyTheLocationMapRefnoz = 1447,

        /// <summary>
        /// Saving transfer items complete.
        /// </summary>
        SavingTransferItemsComplete = 1448,

        /// <summary>
        /// Saving transfer items failed.
        /// </summary>
        SavingTransferItemsFailed = 1449,

        /// <summary>
        /// Database exception occurred. Saving transfer items failed.
        /// </summary>
        DatabaseExceptionOccurredSavingTransferItemsFailed = 1450,

        /// <summary>
        /// Seeding users.
        /// </summary>
        SeedingUsers = 1451,

        /// <summary>
        /// Seeding users complete.
        /// </summary>
        SeedingUsersComplete = 1452,

        /// <summary>
        /// Seeding users failed.
        /// </summary>
        SeedingUsersFailed = 1453,

        /// <summary>
        /// {0} users.
        /// </summary>
        Users = 1454,

        /// <summary>
        /// Database exception occurred. Users status failed.
        /// </summary>
        DatabaseExceptionOccurredUsersStatusFailed = 1455,

        /// <summary>
        /// Purging users.
        /// </summary>
        PurgingUsers = 1456,

        /// <summary>
        /// Purging users complete.
        /// </summary>
        PurgingUsersComplete = 1457,

        /// <summary>
        /// Purging users failed.
        /// </summary>
        PurgingUsersFailed = 1458,

        /// <summary>
        /// Database exception occurred. Purging users failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingUsersFailed = 1459,

        /// <summary>
        /// Database exception occurred. Seeding system users failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingSystemUsersFailed = 1460,

        /// <summary>
        /// Saving users.
        /// </summary>
        SavingUsers = 1461,

        /// <summary>
        /// Saving users complete.
        /// </summary>
        SavingUsersComplete = 1462,

        /// <summary>
        /// Saving users failed.
        /// </summary>
        SavingUsersFailed = 1463,

        /// <summary>
        /// Database exception occurred. Saving users failed.
        /// </summary>
        DatabaseExceptionOccurredSavingUsersFailed = 1464,

        /// <summary>
        /// Seeding families.
        /// </summary>
        SeedingFamilies = 1465,

        /// <summary>
        /// Seeding families complete.
        /// </summary>
        SeedingFamiliesComplete = 1466,

        /// <summary>
        /// Seeding families failed.
        /// </summary>
        SeedingFamiliesFailed = 1467,

        /// <summary>
        /// {0} families.
        /// </summary>
        Families = 1468,

        /// <summary>
        /// Families status failed.
        /// </summary>
        FamiliesStatusFailed = 1469,

        /// <summary>
        /// Purging families.
        /// </summary>
        PurgingFamilies = 1470,

        /// <summary>
        /// Purging families complete.
        /// </summary>
        PurgingFamiliesComplete = 1471,

        /// <summary>
        /// Purging families failed.
        /// </summary>
        PurgingFamiliesFailed = 1472,

        /// <summary>
        /// Database exception occurred. Purging families failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingFamiliesFailed = 1473,

        /// <summary>
        /// drFamily.Read() failed.
        /// </summary>
        DrfamilyreadFailed = 1474,

        /// <summary>
        /// Unable to locate the family refno from family code {0} in tFamily.
        /// </summary>
        UnableToLocateTheFamilyRefnoFromFamilyCodeInTfamily = 1475,

        /// <summary>
        /// Database exception occurred. GetiFamilyRefnoFromFamilyCode failed.
        /// </summary>
        DatabaseExceptionOccurredGetifamilyrefnofromfamilycodeFailed = 1476,

        /// <summary>
        /// Saving families.
        /// </summary>
        SavingFamilies = 1477,

        /// <summary>
        /// Saving families complete.
        /// </summary>
        SavingFamiliesComplete = 1478,

        /// <summary>
        /// Saving families failed.
        /// </summary>
        SavingFamiliesFailed = 1479,

        /// <summary>
        /// Database exception occurred. Saving families failed.
        /// </summary>
        DatabaseExceptionOccurredSavingFamiliesFailed = 1480,

        /// <summary>
        /// Seeding sections.
        /// </summary>
        SeedingSections = 1481,

        /// <summary>
        /// For whs \'{0}\'.
        /// </summary>
        ForWhs = 1482,

        /// <summary>
        /// Unable to locate the warehouse name using the warehouse refno {0} in tLocationMapWarehouse.
        /// </summary>
        UnableToLocateTheWarehouseNameUsingTheWarehouseRefnoInTlocationmapwarehouse = 1483,

        /// <summary>
        /// Seeding sections complete.
        /// </summary>
        SeedingSectionsComplete = 1484,

        /// <summary>
        /// Seeding sections failed.
        /// </summary>
        SeedingSectionsFailed = 1485,

        /// <summary>
        /// Database exception occurred. Seeding sections failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingSectionsFailed = 1486,

        /// <summary>
        /// Purging sections.
        /// </summary>
        PurgingSections = 1487,

        /// <summary>
        /// Purging sections complete.
        /// </summary>
        PurgingSectionsComplete = 1488,

        /// <summary>
        /// Purging sections failed.
        /// </summary>
        PurgingSectionsFailed = 1489,

        /// <summary>
        /// Database exception occurred. Purging sections failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingSectionsFailed = 1490,

        /// <summary>
        /// {0} sections.
        /// </summary>
        Sections = 1491,

        /// <summary>
        /// Database exception occurred. Sections status failed.
        /// </summary>
        DatabaseExceptionOccurredSectionsStatusFailed = 1492,

        /// <summary>
        /// Saving sections.
        /// </summary>
        SavingSections = 1493,

        /// <summary>
        /// Saving sections complete.
        /// </summary>
        SavingSectionsComplete = 1494,

        /// <summary>
        /// Saving sections failed.
        /// </summary>
        SavingSectionsFailed = 1495,

        /// <summary>
        /// Database exception occurred. Saving sections failed.
        /// </summary>
        DatabaseExceptionOccurredSavingSectionsFailed = 1496,

        /// <summary>
        /// Seeding slots.
        /// </summary>
        SeedingSlots = 1497,

        /// <summary>
        /// Seeding slots complete.
        /// </summary>
        SeedingSlotsComplete = 1498,

        /// <summary>
        /// Seeding slots failed.
        /// </summary>
        SeedingSlotsFailed = 1499,

        /// <summary>
        /// Database exception occurred. Seeding slots failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingSlotsFailed = 1500,

        /// <summary>
        /// Purging slots.
        /// </summary>
        PurgingSlots = 1501,

        /// <summary>
        /// Purging slots complete.
        /// </summary>
        PurgingSlotsComplete = 1502,

        /// <summary>
        /// Purging slots failed.
        /// </summary>
        PurgingSlotsFailed = 1503,

        /// <summary>
        /// Database exception occurred. Purging slots failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingSlotsFailed = 1504,

        /// <summary>
        /// {0} slots.
        /// </summary>
        Slots = 1505,

        /// <summary>
        /// Database exception occurred. Slots status failed.
        /// </summary>
        DatabaseExceptionOccurredSlotsStatusFailed = 1506,

        /// <summary>
        /// Saving slots.
        /// </summary>
        SavingSlots = 1507,

        /// <summary>
        /// drSect.Read() failed.
        /// </summary>
        DrsectreadFailed = 1508,

        /// <summary>
        /// Unable to locate the section fk refno in tLocationMapSection.
        /// </summary>
        UnableToLocateTheSectionFkRefnoInTlocationmapsection = 1509,

        /// <summary>
        /// Unable to locate the slot coordinate 1 code in tLocationMapSlotCoordinate.
        /// </summary>
        UnableToLocateTheSlotCoordinateCodeInTlocationmapslotcoordinate = 1510,

        /// <summary>
        /// Unable to locate the slot coordinate 2 code in tLocationMapSlotCoordinate.
        /// </summary>
        UnableToLocateTheSlotCoordinateCodeInTlocationmapslotcoordinatez = 1511,

        /// <summary>
        /// Unable to locate the slot coordinate 3 code in tLocationMapSlotCoordinate.
        /// </summary>
        UnableToLocateTheSlotCoordinateCodeInTlocationmapslotcoordinatezz = 1512,

        /// <summary>
        /// Saving slots complete.
        /// </summary>
        SavingSlotsComplete = 1513,

        /// <summary>
        /// Saving slots failed.
        /// </summary>
        SavingSlotsFailed = 1514,

        /// <summary>
        /// Database exception occurred. Saving slots failed.
        /// </summary>
        DatabaseExceptionOccurredSavingSlotsFailed = 1515,

        /// <summary>
        /// Seeding slot coordinates.
        /// </summary>
        SeedingSlotCoordinates = 1516,

        /// <summary>
        /// Seeding slot coordinates complete.
        /// </summary>
        SeedingSlotCoordinatesComplete = 1517,

        /// <summary>
        /// Seeding slot coordinates failed.
        /// </summary>
        SeedingSlotCoordinatesFailed = 1518,

        /// <summary>
        /// Purging slot coordinates.
        /// </summary>
        PurgingSlotCoordinates = 1519,

        /// <summary>
        /// Purging slot coordinates complete.
        /// </summary>
        PurgingSlotCoordinatesComplete = 1520,

        /// <summary>
        /// Purging slot coordinates failed.
        /// </summary>
        PurgingSlotCoordinatesFailed = 1521,

        /// <summary>
        /// Database exception occurred. Purging slot coordinates failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingSlotCoordinatesFailed = 1522,

        /// <summary>
        /// {0} slot coordinates.
        /// </summary>
        SlotCoordinates = 1523,

        /// <summary>
        /// Database exception occurred. Slot coordinates status failed.
        /// </summary>
        DatabaseExceptionOccurredSlotCoordinatesStatusFailed = 1524,

        /// <summary>
        /// Saving slot coordinates.
        /// </summary>
        SavingSlotCoordinates = 1525,

        /// <summary>
        /// Unidentified slot coordinate enum type 
        /// </summary>
        UnidentifiedSlotCoordinateEnumType = 1526,

        /// <summary>
        /// Saving slot coordinates complete.
        /// </summary>
        SavingSlotCoordinatesComplete = 1527,

        /// <summary>
        /// Saving slot coordinates failed.
        /// </summary>
        SavingSlotCoordinatesFailed = 1528,

        /// <summary>
        /// Database exception occurred. Saving slot coordinates failed.
        /// </summary>
        DatabaseExceptionOccurredSavingSlotCoordinatesFailed = 1529,

        /// <summary>
        /// Seeding location map types.
        /// </summary>
        SeedingLocationMapTypes = 1530,

        /// <summary>
        /// Seeding location map types complete.
        /// </summary>
        SeedingLocationMapTypesComplete = 1531,

        /// <summary>
        /// Seeding location map types failed.
        /// </summary>
        SeedingLocationMapTypesFailed = 1532,

        /// <summary>
        /// Purging location map types.
        /// </summary>
        PurgingLocationMapTypes = 1533,

        /// <summary>
        /// Purging location map types complete.
        /// </summary>
        PurgingLocationMapTypesComplete = 1534,

        /// <summary>
        /// Purging location map types failed.
        /// </summary>
        PurgingLocationMapTypesFailed = 1535,

        /// <summary>
        /// Database exception occurred. Purging location map types failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingLocationMapTypesFailed = 1536,

        /// <summary>
        /// {0} location map types.
        /// </summary>
        LocationMapTypes = 1537,

        /// <summary>
        /// Database exception occurred. Location map types status failed.
        /// </summary>
        DatabaseExceptionOccurredLocationMapTypesStatusFailed = 1538,

        /// <summary>
        /// Saving location map types.
        /// </summary>
        SavingLocationMapTypes = 1539,

        /// <summary>
        /// Undefined location map type {0}
        /// </summary>
        UndefinedLocationMapType = 1540,

        /// <summary>
        /// Unable ot locate the section fk refno in tLocationMapSection.
        /// </summary>
        UnableOtLocateTheSectionFkRefnoInTlocationmapsection = 1541,

        /// <summary>
        /// drSlot.Read() failed.
        /// </summary>
        DrslotreadFailed = 1542,

        /// <summary>
        /// Unable to locate the slot fk refno in tLocationMapSlot.
        /// </summary>
        UnableToLocateTheSlotFkRefnoInTlocationmapslot = 1543,

        /// <summary>
        /// Undefined Location Map SubType {0}
        /// </summary>
        UndefinedLocationMapSubtype = 1544,

        /// <summary>
        /// Saving location map types complete.
        /// </summary>
        SavingLocationMapTypesComplete = 1545,

        /// <summary>
        /// Saving location map types failed.
        /// </summary>
        SavingLocationMapTypesFailed = 1546,

        /// <summary>
        /// Database exception occurred. Saving location map types failed.
        /// </summary>
        DatabaseExceptionOccurredSavingLocationMapTypesFailed = 1547,

        /// <summary>
        /// Seeding warehouses.
        /// </summary>
        SeedingWarehouses = 1548,

        /// <summary>
        /// Seeding warehouses complete.
        /// </summary>
        SeedingWarehousesComplete = 1549,

        /// <summary>
        /// Seeding warehouses failed.
        /// </summary>
        SeedingWarehousesFailed = 1550,

        /// <summary>
        /// Purging warehouses.
        /// </summary>
        PurgingWarehouses = 1551,

        /// <summary>
        /// Purging warehouses complete.
        /// </summary>
        PurgingWarehousesComplete = 1552,

        /// <summary>
        /// Purging warehouses failed.
        /// </summary>
        PurgingWarehousesFailed = 1553,

        /// <summary>
        /// Database exception occurred. Purging warehouses failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingWarehousesFailed = 1554,

        /// <summary>
        /// Unable to locate warehouse refno using the warehouse fk refno {0} in tLocationMapWarehouse.
        /// </summary>
        UnableToLocateWarehouseRefnoUsingTheWarehouseFkRefnoInTlocationmapwarehouse = 1555,

        /// <summary>
        /// Database exception occurred. GetLocMapWhsRefnoFromLocWhsFKRefno failed.
        /// </summary>
        DatabaseExceptionOccurredGetlocmapwhsrefnofromlocwhsfkrefnoFailed = 1556,

        /// <summary>
        /// Unable to locate warehouse fk refno using the warehouse refno {0} in tLocationMapWarehouse.
        /// </summary>
        UnableToLocateWarehouseFkRefnoUsingTheWarehouseRefnoInTlocationmapwarehouse = 1557,

        /// <summary>
        /// Database exception occurred. GetLocMapWhsFKRefnoFromLocMapWhsRefno failed.
        /// </summary>
        DatabaseExceptionOccurredGetlocmapwhsfkrefnofromlocmapwhsrefnoFailed = 1558,

        /// <summary>
        /// {0} warehouses.
        /// </summary>
        Warehouses = 1559,

        /// <summary>
        /// Database exception occurred. Warehouses status failed.
        /// </summary>
        DatabaseExceptionOccurredWarehousesStatusFailed = 1560,

        /// <summary>
        /// Saving warehouses.
        /// </summary>
        SavingWarehouses = 1561,

        /// <summary>
        /// Saving warehouses complete.
        /// </summary>
        SavingWarehousesComplete = 1562,

        /// <summary>
        /// Saving warehouses failed.
        /// </summary>
        SavingWarehousesFailed = 1563,

        /// <summary>
        /// Database exception occurred. Saving warehouses failed.
        /// </summary>
        DatabaseExceptionOccurredSavingWarehousesFailed = 1564,

        /// <summary>
        /// Seeding products.
        /// </summary>
        SeedingProducts = 1565,

        /// <summary>
        /// For family {0}
        /// </summary>
        ForFamily = 1566,

        /// <summary>
        /// Seeding products complete.
        /// </summary>
        SeedingProductsComplete = 1567,

        /// <summary>
        /// Seeding products failed.
        /// </summary>
        SeedingProductsFailed = 1568,

        /// <summary>
        /// Database exception occurred. Seeding products failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingProductsFailed = 1569,

        /// <summary>
        /// Purging products.
        /// </summary>
        PurgingProducts = 1570,

        /// <summary>
        /// Purging products complete.
        /// </summary>
        PurgingProductsComplete = 1571,

        /// <summary>
        /// Purging products failed.
        /// </summary>
        PurgingProductsFailed = 1572,

        /// <summary>
        /// Database exception occurred. Purging products failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingProductsFailed = 1573,

        /// <summary>
        /// Unable to locate product Refno from product FK Refno {0} in tProduct
        /// </summary>
        UnableToLocateProductRefnoFromProductFkRefnoInTproduct = 1574,

        /// <summary>
        /// Database exception occurred. GetProdRefnoFromProdFKRefno failed.
        /// </summary>
        DatabaseExceptionOccurredGetprodrefnofromprodfkrefnoFailed = 1575,

        /// <summary>
        /// {0} products.
        /// </summary>
        Products = 1576,

        /// <summary>
        /// Database exception occurred. Products status failed.
        /// </summary>
        DatabaseExceptionOccurredProductsStatusFailed = 1577,

        /// <summary>
        /// Saving products.
        /// </summary>
        SavingProducts = 1578,

        /// <summary>
        /// Unable to locate the product scheme code in tProductScheme.
        /// </summary>
        UnableToLocateTheProductSchemeCodeInTproductscheme = 1579,

        /// <summary>
        /// Unable to locate the family code in tFamily.
        /// </summary>
        UnableToLocateTheFamilyCodeInTfamily = 1580,

        /// <summary>
        /// Saving products complete.
        /// </summary>
        SavingProductsComplete = 1581,

        /// <summary>
        /// Saving products failed.
        /// </summary>
        SavingProductsFailed = 1582,

        /// <summary>
        /// Database exception occurred. Saving products failed.
        /// </summary>
        DatabaseExceptionOccurredSavingProductsFailed = 1583,

        /// <summary>
        /// Seeding product locations.
        /// </summary>
        SeedingProductLocations = 1584,

        /// <summary>
        /// Default picking locations.
        /// </summary>
        DefaultPickingLocations = 1585,

        /// <summary>
        /// Default receiving locations.
        /// </summary>
        DefaultReceivingLocations = 1586,

        /// <summary>
        /// Seeding product locations complete.
        /// </summary>
        SeedingProductLocationsComplete = 1587,

        /// <summary>
        /// Seeding product locations failed.
        /// </summary>
        SeedingProductLocationsFailed = 1588,

        /// <summary>
        /// Database exception occurred. Seeding product locations failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingProductLocationsFailed = 1589,

        /// <summary>
        /// {0} product locations.
        /// </summary>
        ProductLocations = 1590,

        /// <summary>
        /// product locations status failed.
        /// </summary>
        ProductLocationsStatusFailed = 1591,

        /// <summary>
        /// Database exception occurred. Product location status failed.
        /// </summary>
        DatabaseExceptionOccurredProductLocationStatusFailed = 1592,

        /// <summary>
        /// Purging product locations.
        /// </summary>
        PurgingProductLocations = 1593,

        /// <summary>
        /// Database exception occurred. Purging product locations.
        /// </summary>
        DatabaseExceptionOccurredPurgingProductLocations = 1594,

        /// <summary>
        /// Saving product locations.
        /// </summary>
        SavingProductLocations = 1595,

        /// <summary>
        /// drProd.Read() failed.
        /// </summary>
        DrprodreadFailed = 1596,

        /// <summary>
        /// Undefined location map subtype {0}
        /// </summary>
        UndefinedLocationMapSubtypez = 1597,

        /// <summary>
        /// Unable to locate section fk refno in tLocationMapSection.
        /// </summary>
        UnableToLocateSectionFkRefnoInTlocationmapsection = 1598,

        /// <summary>
        /// Unable to locate slot fk refno in tLocationMapSlot.
        /// </summary>
        UnableToLocateSlotFkRefnoInTlocationmapslot = 1599,

        /// <summary>
        /// drLocMapType.Read() failed.
        /// </summary>
        DrlocmaptypereadFailed = 1600,

        /// <summary>
        /// Saving product locations complete.
        /// </summary>
        SavingProductLocationsComplete = 1601,

        /// <summary>
        /// Saving product locations failed.
        /// </summary>
        SavingProductLocationsFailed = 1602,

        /// <summary>
        /// Database exception occurred. Saving product locations failed.
        /// </summary>
        DatabaseExceptionOccurredSavingProductLocationsFailed = 1603,

        /// <summary>
        /// Seeding product schemes.
        /// </summary>
        SeedingProductSchemes = 1604,

        /// <summary>
        /// Seeding product schemes complete.
        /// </summary>
        SeedingProductSchemesComplete = 1605,

        /// <summary>
        /// Seeding product schemes failed.
        /// </summary>
        SeedingProductSchemesFailed = 1606,

        /// <summary>
        /// drScheme.Read() failed.
        /// </summary>
        DrschemereadFailed = 1607,

        /// <summary>
        /// Scheme code \'
        /// </summary>
        SchemeCode = 1608,

        /// <summary>
        /// Database exception occurred. GetProdSchemeRefnoFromProdSchemeCode failed.
        /// </summary>
        DatabaseExceptionOccurredGetprodschemerefnofromprodschemecodeFailed = 1609,

        /// <summary>
        /// {0} product schemes.
        /// </summary>
        ProductSchemes = 1610,

        /// <summary>
        /// Database exception occurred. Product schemes status failed.
        /// </summary>
        DatabaseExceptionOccurredProductSchemesStatusFailed = 1611,

        /// <summary>
        /// Purging product schemes.
        /// </summary>
        PurgingProductSchemes = 1612,

        /// <summary>
        /// Purging product schemes failed.
        /// </summary>
        PurgingProductSchemesFailed = 1613,

        /// <summary>
        /// Database exception occurred. Purge product schemes failed.
        /// </summary>
        DatabaseExceptionOccurredPurgeProductSchemesFailed = 1614,

        /// <summary>
        /// Saving product schemes.
        /// </summary>
        SavingProductSchemes = 1615,

        /// <summary>
        /// Saving product schemes complete.
        /// </summary>
        SavingProductSchemesComplete = 1616,

        /// <summary>
        /// Saving product schemes failed.
        /// </summary>
        SavingProductSchemesFailed = 1617,

        /// <summary>
        /// Database exception occurred. Saving product schemes failed.
        /// </summary>
        DatabaseExceptionOccurredSavingProductSchemesFailed = 1618,

        /// <summary>
        /// Seeding product scheme variants.
        /// </summary>
        SeedingProductSchemeVariants = 1619,

        /// <summary>
        /// Seeding product scheme variants complete.
        /// </summary>
        SeedingProductSchemeVariantsComplete = 1620,

        /// <summary>
        /// Seeding product scheme variants failed.
        /// </summary>
        SeedingProductSchemeVariantsFailed = 1621,

        /// <summary>
        /// Purging product scheme variants.
        /// </summary>
        PurgingProductSchemeVariants = 1622,

        /// <summary>
        /// Purging product scheme variants complete.
        /// </summary>
        PurgingProductSchemeVariantsComplete = 1623,

        /// <summary>
        /// Purging product scheme variants failed.
        /// </summary>
        PurgingProductSchemeVariantsFailed = 1624,

        /// <summary>
        /// Database exception occurred. Purging product scheme variants failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingProductSchemeVariantsFailed = 1625,

        /// <summary>
        /// drSchemeVar.Read() failed.
        /// </summary>
        DrschemevarreadFailed = 1626,

        /// <summary>
        /// Unable To Locate Product Scheme Variant Number {0} In tProductSchemeVariant
        /// </summary>
        UnableToLocateProductSchemeVariantNumberInTproductschemevariant = 1627,

        /// <summary>
        /// Database exception occurred. GetProdSchemeVarRefnoFromProdSchemeVarNo failed.
        /// </summary>
        DatabaseExceptionOccurredGetprodschemevarrefnofromprodschemevarnoFailed = 1628,

        /// <summary>
        /// {0} product scheme variants.
        /// </summary>
        ProductSchemeVariants = 1629,

        /// <summary>
        /// Database exception occurred. Product scheme variants status failed.
        /// </summary>
        DatabaseExceptionOccurredProductSchemeVariantsStatusFailed = 1630,

        /// <summary>
        /// Saving product scheme variants.
        /// </summary>
        SavingProductSchemeVariants = 1631,

        /// <summary>
        /// Saving product scheme variants complete.
        /// </summary>
        SavingProductSchemeVariantsComplete = 1632,

        /// <summary>
        /// Database exception occurred. Saving product scheme variants failed.
        /// </summary>
        DatabaseExceptionOccurredSavingProductSchemeVariantsFailed = 1633,

        /// <summary>
        /// Seeding product scheme variant xref.
        /// </summary>
        SeedingProductSchemeVariantXref = 1634,

        /// <summary>
        /// Seeding product scheme variant xref complete.
        /// </summary>
        SeedingProductSchemeVariantXrefComplete = 1635,

        /// <summary>
        /// Seeding product scheme variant xref failed.
        /// </summary>
        SeedingProductSchemeVariantXrefFailed = 1636,

        /// <summary>
        /// Purging product scheme variant xref.
        /// </summary>
        PurgingProductSchemeVariantXref = 1637,

        /// <summary>
        /// Purging product scheme variant xref failed.
        /// </summary>
        PurgingProductSchemeVariantXrefFailed = 1638,

        /// <summary>
        /// Database exception occurred. Purging product scheme variant xref failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingProductSchemeVariantXrefFailed = 1639,

        /// <summary>
        /// {0} product scheme variant xrefs.
        /// </summary>
        ProductSchemeVariantXrefs = 1640,

        /// <summary>
        /// Database exception occurred. Product scheme variant xref status failed.
        /// </summary>
        DatabaseExceptionOccurredProductSchemeVariantXrefStatusFailed = 1641,

        /// <summary>
        /// Saving product scheme variant xrefs.
        /// </summary>
        SavingProductSchemeVariantXrefs = 1642,

        /// <summary>
        /// Unable to locate the product scheme variant number in tProductSchemeVariant.
        /// </summary>
        UnableToLocateTheProductSchemeVariantNumberInTproductschemevariant = 1643,

        /// <summary>
        /// Saving product scheme variant xref complete.
        /// </summary>
        SavingProductSchemeVariantXrefComplete = 1644,

        /// <summary>
        /// Saving product scheme variant xref failed.
        /// </summary>
        SavingProductSchemeVariantXrefFailed = 1645,

        /// <summary>
        /// Database exception occurred. Saving product scheme variant xref failed.
        /// </summary>
        DatabaseExceptionOccurredSavingProductSchemeVariantXrefFailed = 1646,

        /// <summary>
        /// Seeding product uom.
        /// </summary>
        SeedingProductUom = 1647,

        /// <summary>
        /// Seeding product uom complete.
        /// </summary>
        SeedingProductUomComplete = 1648,

        /// <summary>
        /// Seeding product uom failed.
        /// </summary>
        SeedingProductUomFailed = 1649,

        /// <summary>
        /// Database exception occurred. Seeding product uom failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingProductUomFailed = 1650,

        /// <summary>
        /// Purging product uom.
        /// </summary>
        PurgingProductUom = 1651,

        /// <summary>
        /// Purging product uom complete.
        /// </summary>
        PurgingProductUomComplete = 1652,

        /// <summary>
        /// Purging product uom failed.
        /// </summary>
        PurgingProductUomFailed = 1653,

        /// <summary>
        /// Database exception occurred. Purging product uom failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingProductUomFailed = 1654,

        /// <summary>
        /// drProductUnit.Read() failed.
        /// </summary>
        DrproductunitreadFailed = 1655,

        /// <summary>
        /// Unable to locate product unit refno from product refno and unit refno and is BC {0}, {1}, {2} 
        /// </summary>
        UnableToLocateProductUnitRefnoFromProductRefnoAndUnitRefnoAndIsBc = 1656,

        /// <summary>
        /// in tProductUnit
        /// </summary>
        InTproductunit = 1657,

        /// <summary>
        /// Database exception occurred. GetProdUnitRefnoFromProdRefnoAndUnitRefnoAndProdUnitIsBC failed.
        /// </summary>
        DatabaseExceptionOccurredGetprodunitrefnofromprodrefnoandunitrefnoandprodunitisbcFailed = 1658,

        /// <summary>
        /// {0} product units.
        /// </summary>
        ProductUnits = 1659,

        /// <summary>
        /// Database exception occurred. Product uom status failed.
        /// </summary>
        DatabaseExceptionOccurredProductUomStatusFailed = 1660,

        /// <summary>
        /// Saving product uom.
        /// </summary>
        SavingProductUom = 1661,

        /// <summary>
        /// Unable to locate the product refno using the product FK refno 
        /// </summary>
        UnableToLocateTheProductRefnoUsingTheProductFkRefno = 1662,

        /// <summary>
        /// in tProduct.
        /// </summary>
        InTproduct = 1663,

        /// <summary>
        /// Unable to locate the unit refno using the unit code in tUnit.
        /// </summary>
        UnableToLocateTheUnitRefnoUsingTheUnitCodeInTunit = 1664,

        /// <summary>
        /// Unable to locate the unit refno using the base unit code in tUnit.
        /// </summary>
        UnableToLocateTheUnitRefnoUsingTheBaseUnitCodeInTunit = 1665,

        /// <summary>
        /// Unable to locate the unit refno using the weight unit code in tUnit.
        /// </summary>
        UnableToLocateTheUnitRefnoUsingTheWeightUnitCodeInTunit = 1666,

        /// <summary>
        /// Saving product uom complete.
        /// </summary>
        SavingProductUomComplete = 1667,

        /// <summary>
        /// Saving product uom failed.
        /// </summary>
        SavingProductUomFailed = 1668,

        /// <summary>
        /// Database exception occurred. Saving product uom failed.
        /// </summary>
        DatabaseExceptionOccurredSavingProductUomFailed = 1669,

        /// <summary>
        /// Seeding product uom bar codes.
        /// </summary>
        SeedingProductUomBarCodes = 1670,

        /// <summary>
        /// Seeding product uom bar codes complete.
        /// </summary>
        SeedingProductUomBarCodesComplete = 1671,

        /// <summary>
        /// Seeding product uom bar codes failed.
        /// </summary>
        SeedingProductUomBarCodesFailed = 1672,

        /// <summary>
        /// Database exception occurred. Seeding product uom bar codes failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingProductUomBarCodesFailed = 1673,

        /// <summary>
        /// Purging product uom bar codes.
        /// </summary>
        PurgingProductUomBarCodes = 1674,

        /// <summary>
        /// Purging product uom bar codes complete.
        /// </summary>
        PurgingProductUomBarCodesComplete = 1675,

        /// <summary>
        /// Purging product uom bar codes failed.
        /// </summary>
        PurgingProductUomBarCodesFailed = 1676,

        /// <summary>
        /// Database exception occurred. Purging product uom bar codes failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingProductUomBarCodesFailed = 1677,

        /// <summary>
        /// {0} product uom bar codes.
        /// </summary>
        ProductUomBarCodes = 1678,

        /// <summary>
        /// Database exception occurred. Product uom bar codes status failed.
        /// </summary>
        DatabaseExceptionOccurredProductUomBarCodesStatusFailed = 1679,

        /// <summary>
        /// Saving product uom bar codes.
        /// </summary>
        SavingProductUomBarCodes = 1680,

        /// <summary>
        /// Unable to locate the unit refno from the unit code in tUnit
        /// </summary>
        UnableToLocateTheUnitRefnoFromTheUnitCodeInTunit = 1681,

        /// <summary>
        /// Unable to locate the product unit refno from the product refno, unit refno 
        /// </summary>
        UnableToLocateTheProductUnitRefnoFromTheProductRefnoUnitRefno = 1682,

        /// <summary>
        /// and is bc in tProductUnit
        /// </summary>
        AndIsBcInTproductunit = 1683,

        /// <summary>
        /// Saving product uom bar codes complete.
        /// </summary>
        SavingProductUomBarCodesComplete = 1684,

        /// <summary>
        /// Saving product uom bar codes failed.
        /// </summary>
        SavingProductUomBarCodesFailed = 1685,

        /// <summary>
        /// Database exception occurred. Saving product uom bar codes failed.
        /// </summary>
        DatabaseExceptionOccurredSavingProductUomBarCodesFailed = 1686,

        /// <summary>
        /// Seeding product uom relations.
        /// </summary>
        SeedingProductUomRelations = 1687,

        /// <summary>
        /// No product uom relations where received.
        /// </summary>
        NoProductUomRelationsWhereReceived = 1688,

        /// <summary>
        /// Seeding product uom relations complete.
        /// </summary>
        SeedingProductUomRelationsComplete = 1689,

        /// <summary>
        /// Seeding product uom relations failed.
        /// </summary>
        SeedingProductUomRelationsFailed = 1690,

        /// <summary>
        /// Database exception occurred. Seeding product uom relations failed.
        /// </summary>
        DatabaseExceptionOccurredSeedingProductUomRelationsFailed = 1691,

        /// <summary>
        /// Purging product uom relations.
        /// </summary>
        PurgingProductUomRelations = 1692,

        /// <summary>
        /// Purging product uom relations complete.
        /// </summary>
        PurgingProductUomRelationsComplete = 1693,

        /// <summary>
        /// Purging product uom relations failed.
        /// </summary>
        PurgingProductUomRelationsFailed = 1694,

        /// <summary>
        /// Database exception occurred. Purging product uom relations failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingProductUomRelationsFailed = 1695,

        /// <summary>
        /// {0} product unit relations.
        /// </summary>
        ProductUnitRelations = 1696,

        /// <summary>
        /// Database exception occurred. Product unit relations status failed.
        /// </summary>
        DatabaseExceptionOccurredProductUnitRelationsStatusFailed = 1697,

        /// <summary>
        /// Saving product uom relations.
        /// </summary>
        SavingProductUomRelations = 1698,

        /// <summary>
        /// Unable to locate the unit refno from the unit code 1 in tUnit.
        /// </summary>
        UnableToLocateTheUnitRefnoFromTheUnitCodeInTunitz = 1699,

        /// <summary>
        /// Unable to locate the product unit refno from the product refno, 
        /// </summary>
        UnableToLocateTheProductUnitRefnoFromTheProductRefno = 1700,

        /// <summary>
        /// unit code 1 refno and is bc in tProductUnit.
        /// </summary>
        UnitCodeRefnoAndIsBcInTproductunit = 1701,

        /// <summary>
        /// Unable to locate the unit refno from the unit code 2 in tUnit.
        /// </summary>
        UnableToLocateTheUnitRefnoFromTheUnitCodeInTunitzz = 1702,

        /// <summary>
        /// unit code 2 refno and is bc in tProductUnit.
        /// </summary>
        UnitCodeRefnoAndIsBcInTproductunitz = 1703,

        /// <summary>
        /// Saving product uom relations complete.
        /// </summary>
        SavingProductUomRelationsComplete = 1704,

        /// <summary>
        /// Saving product uom relations failed.
        /// </summary>
        SavingProductUomRelationsFailed = 1705,

        /// <summary>
        /// Database exception occurred. Saving product uom relations failed.
        /// </summary>
        DatabaseExceptionOccurredSavingProductUomRelationsFailed = 1706,

        /// <summary>
        /// Seeding routes.
        /// </summary>
        SeedingRoutes = 1707,

        /// <summary>
        /// Seeding routes complete.
        /// </summary>
        SeedingRoutesComplete = 1708,

        /// <summary>
        /// Seeding routes failed.
        /// </summary>
        SeedingRoutesFailed = 1709,

        /// <summary>
        /// drRoute.Read() failed.
        /// </summary>
        DrroutereadFailed = 1710,

        /// <summary>
        /// Unable To Locate Route Refno From Route Code {0} In tRoute
        /// </summary>
        UnableToLocateRouteRefnoFromRouteCodeInTroute = 1711,

        /// <summary>
        /// Database exception occurred. GetiRouteRefnoFromRouteCode failed.
        /// </summary>
        DatabaseExceptionOccurredGetirouterefnofromroutecodeFailed = 1712,

        /// <summary>
        /// {0} routes.
        /// </summary>
        Routes = 1713,

        /// <summary>
        /// Database exception occurred. Routes status failed.
        /// </summary>
        DatabaseExceptionOccurredRoutesStatusFailed = 1714,

        /// <summary>
        /// Purging routes.
        /// </summary>
        PurgingRoutes = 1715,

        /// <summary>
        /// Purging routes complete.
        /// </summary>
        PurgingRoutesComplete = 1716,

        /// <summary>
        /// Purging routes failed.
        /// </summary>
        PurgingRoutesFailed = 1717,

        /// <summary>
        /// Database exception occurred. Purging routes failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingRoutesFailed = 1718,

        /// <summary>
        /// Saving routes.
        /// </summary>
        SavingRoutes = 1719,

        /// <summary>
        /// Saving routes complete.
        /// </summary>
        SavingRoutesComplete = 1720,

        /// <summary>
        /// Saving routes failed.
        /// </summary>
        SavingRoutesFailed = 1721,

        /// <summary>
        /// Database exception occurred. Saving routes failed.
        /// </summary>
        DatabaseExceptionOccurredSavingRoutesFailed = 1722,

        /// <summary>
        /// Seeding Translations.
        /// </summary>
        SeedingTranslations = 1723,

        /// <summary>
        /// Database exception occurred. GetTranslation \
        /// </summary>
        DatabaseExceptionOccurredGettranslation = 1724,

        /// <summary>
        /// Seeding units of measure.
        /// </summary>
        SeedingUnitsOfMeasure = 1725,

        /// <summary>
        /// No units of measure where received.
        /// </summary>
        NoUnitsOfMeasureWhereReceived = 1726,

        /// <summary>
        /// Seeding units of measure complete.
        /// </summary>
        SeedingUnitsOfMeasureComplete = 1727,

        /// <summary>
        /// Seeding units of measure failed.
        /// </summary>
        SeedingUnitsOfMeasureFailed = 1728,

        /// <summary>
        /// Unable to locate the unit refno from unit code {0} in tUnit
        /// </summary>
        UnableToLocateTheUnitRefnoFromUnitCodeInTunit = 1729,

        /// <summary>
        /// Database exception occurred. GetiUnitRefnoFromUnitCode failed.
        /// </summary>
        DatabaseExceptionOccurredGetiunitrefnofromunitcodeFailed = 1730,

        /// <summary>
        /// {0} units of measure.
        /// </summary>
        UnitsOfMeasure = 1731,

        /// <summary>
        /// Database exception occurred. Units of measure status failed.
        /// </summary>
        DatabaseExceptionOccurredUnitsOfMeasureStatusFailed = 1732,

        /// <summary>
        /// Purging units of measure.
        /// </summary>
        PurgingUnitsOfMeasure = 1733,

        /// <summary>
        /// Purging units of measure complete.
        /// </summary>
        PurgingUnitsOfMeasureComplete = 1734,

        /// <summary>
        /// Purging units of measure failed.
        /// </summary>
        PurgingUnitsOfMeasureFailed = 1735,

        /// <summary>
        /// Database exception occurred. Purging units of measure failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingUnitsOfMeasureFailed = 1736,

        /// <summary>
        /// Saving units of measure.
        /// </summary>
        SavingUnitsOfMeasure = 1737,

        /// <summary>
        /// Saving units of measure complete.
        /// </summary>
        SavingUnitsOfMeasureComplete = 1738,

        /// <summary>
        /// Saving units of measure failed.
        /// </summary>
        SavingUnitsOfMeasureFailed = 1739,

        /// <summary>
        /// Database exception occurred. Saving units of measure failed.
        /// </summary>
        DatabaseExceptionOccurredSavingUnitsOfMeasureFailed = 1740,

        /// <summary>
        /// Deleting database 
        /// </summary>
        DeletingDatabase = 1741,

        /// <summary>
        /// Creating database 
        /// </summary>
        CreatingDatabase = 1742,

        /// <summary>
        /// Creating activity table.
        /// </summary>
        CreatingActivityTable = 1743,

        /// <summary>
        /// Creating detail product scheme variant xref table.
        /// </summary>
        CreatingDetailProductSchemeVariantXrefTable = 1744,

        /// <summary>
        /// Creating activity item table.
        /// </summary>
        CreatingActivityItemTable = 1745,

        /// <summary>
        /// Creating activity detail table.
        /// </summary>
        CreatingActivityDetailTable = 1746,

        /// <summary>
        /// Creating detail extra table.
        /// </summary>
        CreatingDetailExtraTable = 1747,

        /// <summary>
        /// Creating count table.
        /// </summary>
        CreatingCountTable = 1748,

        /// <summary>
        /// Creating picking table.
        /// </summary>
        CreatingPickingTable = 1749,

        /// <summary>
        /// Creating returns table.
        /// </summary>
        CreatingReturnsTable = 1750,

        /// <summary>
        /// Creating picking route table.
        /// </summary>
        CreatingPickingRouteTable = 1751,

        /// <summary>
        /// Creating receiving table.
        /// </summary>
        CreatingReceivingTable = 1752,

        /// <summary>
        /// Creating transfer table.
        /// </summary>
        CreatingTransferTable = 1753,

        /// <summary>
        /// Creating count item table.
        /// </summary>
        CreatingCountItemTable = 1754,

        /// <summary>
        /// Creating picking item table.
        /// </summary>
        CreatingPickingItemTable = 1755,

        /// <summary>
        /// Creating returns item table.
        /// </summary>
        CreatingReturnsItemTable = 1756,

        /// <summary>
        /// Creating picking comment item table.
        /// </summary>
        CreatingPickingCommentItemTable = 1757,

        /// <summary>
        /// Creating returns comment item table.
        /// </summary>
        CreatingReturnsCommentItemTable = 1758,

        /// <summary>
        /// Creating receiving item table.
        /// </summary>
        CreatingReceivingItemTable = 1759,

        /// <summary>
        /// Creating purchase order work sheet item table.
        /// </summary>
        CreatingPurchaseOrderWorkSheetItemTable = 1760,

        /// <summary>
        /// Creating receiving comment item table.
        /// </summary>
        CreatingReceivingCommentItemTable = 1761,

        /// <summary>
        /// Creating transfer item table.
        /// </summary>
        CreatingTransferItemTable = 1762,

        /// <summary>
        /// Creating bar code association item table.
        /// </summary>
        CreatingBarCodeAssociationItemTable = 1763,

        /// <summary>
        /// Creating count detail table.
        /// </summary>
        CreatingCountDetailTable = 1764,

        /// <summary>
        /// Creating picking detail table.
        /// </summary>
        CreatingPickingDetailTable = 1765,

        /// <summary>
        /// Creating returns detail table.
        /// </summary>
        CreatingReturnsDetailTable = 1766,

        /// <summary>
        /// Creating receiving detail table.
        /// </summary>
        CreatingReceivingDetailTable = 1767,

        /// <summary>
        /// Creating transfer detail table.
        /// </summary>
        CreatingTransferDetailTable = 1768,

        /// <summary>
        /// Creating unit of measure table.
        /// </summary>
        CreatingUnitOfMeasureTable = 1769,

        /// <summary>
        /// Creating routes table.
        /// </summary>
        CreatingRoutesTable = 1770,

        /// <summary>
        /// Creating location map table.
        /// </summary>
        CreatingLocationMapTable = 1771,

        /// <summary>
        /// Creating location map warehouse table.
        /// </summary>
        CreatingLocationMapWarehouseTable = 1772,

        /// <summary>
        /// Creating location map section table.
        /// </summary>
        CreatingLocationMapSectionTable = 1773,

        /// <summary>
        /// Creating location map slot table.
        /// </summary>
        CreatingLocationMapSlotTable = 1774,

        /// <summary>
        /// Creating location map type table.
        /// </summary>
        CreatingLocationMapTypeTable = 1775,

        /// <summary>
        /// Creating product scheme variants xref table.
        /// </summary>
        CreatingProductSchemeVariantsXrefTable = 1776,

        /// <summary>
        /// Creating product scheme lot numbers xref table.
        /// </summary>
        CreatingProductSchemeLotNumbersXrefTable = 1777,

        /// <summary>
        /// Creating product scheme table.
        /// </summary>
        CreatingProductSchemeTable = 1778,

        /// <summary>
        /// Creating product scheme variants table.
        /// </summary>
        CreatingProductSchemeVariantsTable = 1779,

        /// <summary>
        /// Creating product scheme lots table.
        /// </summary>
        CreatingProductSchemeLotsTable = 1780,

        /// <summary>
        /// Creating products table.
        /// </summary>
        CreatingProductsTable = 1781,

        /// <summary>
        /// Creating product units table.
        /// </summary>
        CreatingProductUnitsTable = 1782,

        /// <summary>
        /// Creating product unit bar codes table.
        /// </summary>
        CreatingProductUnitBarCodesTable = 1783,

        /// <summary>
        /// Creating product unit relations table.
        /// </summary>
        CreatingProductUnitRelationsTable = 1784,

        /// <summary>
        /// Creating warehouse location map table.
        /// </summary>
        CreatingWarehouseLocationMapTable = 1785,

        /// <summary>
        /// Creating product location map table.
        /// </summary>
        CreatingProductLocationMapTable = 1786,

        /// <summary>
        /// Creating contacts table.
        /// </summary>
        CreatingContactsTable = 1787,

        /// <summary>
        /// Creating contact users table.
        /// </summary>
        CreatingContactUsersTable = 1788,

        /// <summary>
        /// Creating contact customers table.
        /// </summary>
        CreatingContactCustomersTable = 1789,

        /// <summary>
        /// Creating contact customer shipto's table.
        /// </summary>
        CreatingContactCustomerShiptosTable = 1790,

        /// <summary>
        /// Creating contact vendors table.
        /// </summary>
        CreatingContactVendorsTable = 1791,

        /// <summary>
        /// Creating contact vendor suppliers table.
        /// </summary>
        CreatingContactVendorSuppliersTable = 1792,

        /// <summary>
        /// Creating reason code table.
        /// </summary>
        CreatingReasonCodeTable = 1793,

        /// <summary>
        /// Creating item reason code xref table.
        /// </summary>
        CreatingItemReasonCodeXrefTable = 1794,

        /// <summary>
        /// Creating item convert units table.
        /// </summary>
        CreatingItemConvertUnitsTable = 1795,

        /// <summary>
        /// Creating detail convert units table.
        /// </summary>
        CreatingDetailConvertUnitsTable = 1796,

        /// <summary>
        /// Creating item move inventory table.
        /// </summary>
        CreatingItemMoveInventoryTable = 1797,

        /// <summary>
        /// Creating detail move inventory table.
        /// </summary>
        CreatingDetailMoveInventoryTable = 1798,

        /// <summary>
        /// Creating adjustment table.
        /// </summary>
        CreatingAdjustmentTable = 1799,

        /// <summary>
        /// Creating item adjustment table.
        /// </summary>
        CreatingItemAdjustmentTable = 1800,

        /// <summary>
        /// Creating detail adjustment table.
        /// </summary>
        CreatingDetailAdjustmentTable = 1801,

        /// <summary>
        /// Creating production table.
        /// </summary>
        CreatingProductionTable = 1802,

        /// <summary>
        /// Creating production finished table.
        /// </summary>
        CreatingProductionFinishedTable = 1803,

        /// <summary>
        /// Creating production material table.
        /// </summary>
        CreatingProductionMaterialTable = 1804,

        /// <summary>
        /// Creating production byproduct table.
        /// </summary>
        CreatingProductionByproductTable = 1805,

        /// <summary>
        /// Creating item production table.
        /// </summary>
        CreatingItemProductionTable = 1806,

        /// <summary>
        /// Creating item production finished table.
        /// </summary>
        CreatingItemProductionFinishedTable = 1807,

        /// <summary>
        /// Creating item production material table.
        /// </summary>
        CreatingItemProductionMaterialTable = 1808,

        /// <summary>
        /// Creating item production byproduct table.
        /// </summary>
        CreatingItemProductionByproductTable = 1809,

        /// <summary>
        /// Creating detail production finished table.
        /// </summary>
        CreatingDetailProductionFinishedTable = 1810,

        /// <summary>
        /// Creating detail production material table.
        /// </summary>
        CreatingDetailProductionMaterialTable = 1811,

        /// <summary>
        /// Creating detail production byproduct table.
        /// </summary>
        CreatingDetailProductionByproductTable = 1812,

        /// <summary>
        /// Creating families table.
        /// </summary>
        CreatingFamiliesTable = 1813,

        /// <summary>
        /// Unable to locate product scheme no {0} in tProductSchemeVariant
        /// </summary>
        UnableToLocateProductSchemeNoInTproductschemevariant = 1814,

        /// <summary>
        /// drLocMapSlot.Read() failed.
        /// </summary>
        DrlocmapslotreadFailed = 1815,

        /// <summary>
        /// drLocMapSection.Read() failed.
        /// </summary>
        DrlocmapsectionreadFailed = 1816,

        /// <summary>
        /// drLocMapWhs.Read() failed.
        /// </summary>
        DrlocmapwhsreadFailed = 1817,

        /// <summary>
        /// Unable to identify the location map reference number. {0}
        /// </summary>
        UnableToIdentifyTheLocationMapRefno = 1818,

        /// <summary>
        /// Whs FK Refno {1}{2}Section FK Refno {3}{4} Slot FK Refno {5}{6}
        /// </summary>
        WhsFkRefnoSectionFkRefnoSlotFkRefno = 1819,

        /// <summary>
        /// Unable to locate the location map reference number.
        /// </summary>
        UnableToLocateTheLocationMapRefno = 1820,

        /// <summary>
        /// Unable to locate the unit refno from the unit code in  tUnit.
        /// </summary>
        UnableToLocateTheUnitRefnoFromTheUnitCodeInTunitzzz = 1821,

        /// <summary>
        /// Unable to locate the product unit refno from the product fk refno, bc and unit code in tProductUnit.
        /// </summary>
        UnableToLocateTheProductUnitRefnoFromTheProductFkRefnoBcAndUnitCodeInTproductunit = 1822,

        /// <summary>
        /// Seeding product scheme lot numbers.
        /// </summary>
        SeedingProductSchemeLotNumbers = 1823,

        /// <summary>
        /// Seeding product scheme lot numbers complete.
        /// </summary>
        SeedingProductSchemeLotNumbersComplete = 1824,

        /// <summary>
        /// Seeding product scheme lot numbers failed.
        /// </summary>
        SeedingProductSchemeLotNumbersFailed = 1825,

        /// <summary>
        /// Purging product scheme lot mumbers.
        /// </summary>
        PurgingProductSchemeLotMumbers = 1826,

        /// <summary>
        /// Purging product scheme lot mumbers complete.
        /// </summary>
        PurgingProductSchemeLotMumbersComplete = 1827,

        /// <summary>
        /// Purging product scheme lot numbers failed.
        /// </summary>
        PurgingProductSchemeLotNumbersFailed = 1828,

        /// <summary>
        /// Database exception occurred. Purging product scheme lot numbers failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingProductSchemeLotNumbersFailed = 1829,

        /// <summary>
        /// {0} product scheme lot numbers.
        /// </summary>
        ProductSchemeLotNumbers = 1830,

        /// <summary>
        /// Product scheme lot numbers status failed.
        /// </summary>
        ProductSchemeLotNumbersStatusFailed = 1831,

        /// <summary>
        /// Database exception occurred. Product scheme lot numbers status failed.
        /// </summary>
        DatabaseExceptionOccurredProductSchemeLotNumbersStatusFailed = 1832,

        /// <summary>
        /// Saving product scheme lot numbers.
        /// </summary>
        SavingProductSchemeLotNumbers = 1833,

        /// <summary>
        /// Saving product scheme lot numbers complete.
        /// </summary>
        SavingProductSchemeLotNumbersComplete = 1834,

        /// <summary>
        /// Saving product scheme lot numbers failed.
        /// </summary>
        SavingProductSchemeLotNumbersFailed = 1835,

        /// <summary>
        /// Seeding product scheme lot number xref.
        /// </summary>
        SeedingProductSchemeLotNumberXref = 1836,

        /// <summary>
        /// Seeding product scheme lot number xref complete.
        /// </summary>
        SeedingProductSchemeLotNumberXrefComplete = 1837,

        /// <summary>
        /// Seeding product scheme lot number xref failed.
        /// </summary>
        SeedingProductSchemeLotNumberXrefFailed = 1838,

        /// <summary>
        /// Purging product scheme lot number xref.
        /// </summary>
        PurgingProductSchemeLotNumberXref = 1839,

        /// <summary>
        /// Purging product scheme lot number xref failed.
        /// </summary>
        PurgingProductSchemeLotNumberXrefFailed = 1840,

        /// <summary>
        /// Database exception occurred. Purging product scheme lot number xref failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingProductSchemeLotNumberXrefFailed = 1841,

        /// <summary>
        /// {0} product scheme lot number xrefs.
        /// </summary>
        ProductSchemeLotNumberXrefs = 1842,

        /// <summary>
        /// Database exception occurred. Product scheme lot number xrefs status failed.
        /// </summary>
        DatabaseExceptionOccurredProductSchemeLotNumberXrefsStatusFailed = 1843,

        /// <summary>
        /// Saving product scheme lot number xref.
        /// </summary>
        SavingProductSchemeLotNumberXref = 1844,

        /// <summary>
        /// drSchemeLot.Read() failed.
        /// </summary>
        DrschemelotreadFailed = 1845,

        /// <summary>
        /// Unable to locate the product scheme lot number code in tProductSchemeLotNumber.
        /// </summary>
        UnableToLocateTheProductSchemeLotNumberCodeInTproductschemelotnumber = 1846,

        /// <summary>
        /// Saving product scheme lot number xref complete.
        /// </summary>
        SavingProductSchemeLotNumberXrefComplete = 1847,

        /// <summary>
        /// Request product scheme lot number xref failed.
        /// </summary>
        RequestProductSchemeLotNumberXrefFailed = 1848,

        /// <summary>
        /// Database exception occurred. Request product scheme lot number xref failed.
        /// </summary>
        DatabaseExceptionOccurredRequestProductSchemeLotNumberXrefFailed = 1849,

        /// <summary>
        /// Seeding warehouse locations.
        /// </summary>
        SeedingWarehouseLocations = 1850,

        /// <summary>
        /// Seeding warehouse locations complete.
        /// </summary>
        SeedingWarehouseLocationsComplete = 1851,

        /// <summary>
        /// Seeding warehouse locations failed.
        /// </summary>
        SeedingWarehouseLocationsFailed = 1852,

        /// <summary>
        /// {0} warehouse locations.
        /// </summary>
        WarehouseLocations = 1853,

        /// <summary>
        /// warehouse locations status failed.
        /// </summary>
        WarehouseLocationsStatusFailed = 1854,

        /// <summary>
        /// Database exception occurred. Warehouse location status failed.
        /// </summary>
        DatabaseExceptionOccurredWarehouseLocationStatusFailed = 1855,

        /// <summary>
        /// Purging warehouse locations.
        /// </summary>
        PurgingWarehouseLocations = 1856,

        /// <summary>
        /// Database exception occurred. Purging warehouse locations.
        /// </summary>
        DatabaseExceptionOccurredPurgingWarehouseLocations = 1857,

        /// <summary>
        /// Saving warehouse locations.
        /// </summary>
        SavingWarehouseLocations = 1858,

        /// <summary>
        /// Saving warehouse locations complete.
        /// </summary>
        SavingWarehouseLocationsComplete = 1859,

        /// <summary>
        /// Saving warehouse locations failed.
        /// </summary>
        SavingWarehouseLocationsFailed = 1860,

        /// <summary>
        /// Database exception occurred. Saving warehouse locations failed.
        /// </summary>
        DatabaseExceptionOccurredSavingWarehouseLocationsFailed = 1861,

        /// <summary>
        /// Purging product scheme variant xref details.
        /// </summary>
        PurgingProductSchemeVariantXrefDetails = 1862,

        /// <summary>
        /// Purging product scheme variant xref details complete.
        /// </summary>
        PurgingProductSchemeVariantXrefDetailsComplete = 1863,

        /// <summary>
        /// Purging product scheme variant xref details failed.
        /// </summary>
        PurgingProductSchemeVariantXrefDetailsFailed = 1864,

        /// <summary>
        /// Database exception occurred. Purging product scheme variant xref details failed.
        /// </summary>
        DatabaseExceptionOccurredPurgingProductSchemeVariantXrefDetailsFailed = 1865,

        /// <summary>
        /// Select To Product
        /// </summary>
        SelectToProduct = 1866,

        /// <summary>
        /// Select From Product
        /// </summary>
        SelectFromProduct = 1867,

        /// <summary>
        /// Convert Product
        /// </summary>
        ConvertProduct = 1868,

        /// <summary>
        /// No convert to product details have be set.
        /// </summary>
        NoConvertToProductDetailsHaveBeSet = 1869,

        /// <summary>
        /// Convert Products
        /// </summary>
        ConvertProducts = 1870,

        /// <summary>
        /// Build
        /// </summary>
        Build = 1871,

        /// <summary>
        /// BX
        /// </summary>
        Bx = 1872,

        /// <summary>
        /// Remove From Skid
        /// </summary>
        RemoveFromSkid = 1873,

        /// <summary>
        /// Do You Wish To Consume This Entire Skid?
        /// </summary>
        DoYouWishToConsumeThisEntireSkid = 1874,

        /// <summary>
        /// 6. Convert Label
        /// </summary>
        ConvertLabel = 1875,

        /// <summary>
        /// TO Product cannot be the same as FROM Product.
        /// </summary>
        ToProductCannotBeTheSameAsFromProduct = 1876,

        /// <summary>
        /// No convert to product has be set.
        /// </summary>
        NoConvertToProductHasBeSet = 1877,

        /// <summary>
        /// Cannot convert a product without the Product Flag: Convert Label Adjustment Allow Product Change.
        /// </summary>
        CannotConvertAProductWithoutTheProductFlagConvertLabelAdjustmentAllowProductChange = 1878,

        /// <summary>
        /// From {0} {1} {2}
        /// </summary>
        Fromz = 1879,

        /// <summary>
        /// To {0} {1} {2}
        /// </summary>
        Toz = 1880,

        /// <summary>
        /// Print Product Labels
        /// </summary>
        PrintProductLabelsz = 1881,

        /// <summary>
        /// Batch printing enabled.
        /// </summary>
        BatchPrintingEnabled = 1882,

        /// <summary>
        /// Print labels on scan.
        /// </summary>
        PrintLabelsOnScan = 1883,

        /// <summary>
        /// Product ID {0}
        /// </summary>
        ProductIdz = 1884,

        /// <summary>
        /// Case statement for ActSubType '
        /// </summary>
        CaseStatementForActsubtype = 1885,

        /// <summary>
        /// ' was not defined in GetCallingControl.
        /// </summary>
        WasNotDefinedInGetcallingcontrol = 1886,

        /// <summary>
        /// Inventory
        /// </summary>
        Inventoryzz = 1887,

        /// <summary>
        /// Unit Converstion
        /// </summary>
        UnitConverstion = 1888,

        /// <summary>
        /// Rework
        /// </summary>
        Reworkz = 1889,

        /// <summary>
        /// Partial Rework
        /// </summary>
        PartialRework = 1890,

        /// <summary>
        /// Partial Production
        /// </summary>
        PartialProduction = 1891,

        /// <summary>
        /// Disposal
        /// </summary>
        Disposalz = 1892,

        /// <summary>
        /// Fresh To Frozen
        /// </summary>
        FreshToFrozen = 1893,

        /// <summary>
        /// Transfer
        /// </summary>
        Transferzz = 1894,

        /// <summary>
        /// Pack Off
        /// </summary>
        PackOff = 1895,

        /// <summary>
        /// Lost & Found
        /// </summary>
        LostFound = 1896,

        /// <summary>
        /// Cycle Count
        /// </summary>
        CycleCountz = 1897,

        /// <summary>
        /// Move To Deli
        /// </summary>
        MoveToDeli = 1898,

        /// <summary>
        /// Periodic Count
        /// </summary>
        PeriodicCount = 1899,

        /// <summary>
        /// Manufacture
        /// </summary>
        Manufacture = 1900,

        /// <summary>
        /// Requisition
        /// </summary>
        Requisition = 1901,

        /// <summary>
        /// Auto Convert To
        /// </summary>
        AutoConvertTo = 1902,

        /// <summary>
        /// Auto Create From
        /// </summary>
        AutoCreateFrom = 1903,

        /// <summary>
        /// Skid Putaway
        /// </summary>
        SkidPutaway = 1904,

        /// <summary>
        /// Adjustment # Ascending
        /// </summary>
        AdjustmentAscending = 1905,

        /// <summary>
        /// Adjustment # Descending
        /// </summary>
        AdjustmentDescending = 1906,

        /// <summary>
        /// Adjustment Type # Ascending
        /// </summary>
        AdjustmentTypeAscending = 1907,

        /// <summary>
        /// Adjustment Type # Descending
        /// </summary>
        AdjustmentTypeDescending = 1908,

        /// <summary>
        /// Adjustment Date Ascending
        /// </summary>
        AdjustmentDateAscending = 1909,

        /// <summary>
        /// Adjustment Date Descending
        /// </summary>
        AdjustmentDateDescending = 1910,

        /// <summary>
        /// Count # Ascending
        /// </summary>
        CountAscending = 1911,

        /// <summary>
        /// Count # Descending
        /// </summary>
        CountDescending = 1912,

        /// <summary>
        /// Count Date Ascending
        /// </summary>
        CountDateAscending = 1913,

        /// <summary>
        /// Count Date Descending
        /// </summary>
        CountDateDescending = 1914,

        /// <summary>
        /// Order # Ascending
        /// </summary>
        OrderAscending = 1915,

        /// <summary>
        /// Order # Descending
        /// </summary>
        OrderDescending = 1916,

        /// <summary>
        /// Route # Ascending
        /// </summary>
        RouteAscending = 1917,

        /// <summary>
        /// Route # Descending
        /// </summary>
        RouteDescending = 1918,

        /// <summary>
        /// Ordered Date Ascending
        /// </summary>
        OrderedDateAscending = 1919,

        /// <summary>
        /// Ordered Date Descending
        /// </summary>
        OrderedDateDescending = 1920,

        /// <summary>
        /// Delivery Ascending
        /// </summary>
        DeliveryAscending = 1921,

        /// <summary>
        /// Delivery Descending
        /// </summary>
        DeliveryDescending = 1922,

        /// <summary>
        /// Production # Ascending
        /// </summary>
        ProductionAscending = 1923,

        /// <summary>
        /// Production # Descending
        /// </summary>
        ProductionDescending = 1924,

        /// <summary>
        /// Production Date Ascending
        /// </summary>
        ProductionDateAscending = 1925,

        /// <summary>
        /// Production Date Descending
        /// </summary>
        ProductionDateDescending = 1926,

        /// <summary>
        /// Purchase Order # Ascending
        /// </summary>
        PurchaseOrderAscending = 1927,

        /// <summary>
        /// Purchase Order # Descending
        /// </summary>
        PurchaseOrderDescending = 1928,

        /// <summary>
        /// Issued Date Ascending
        /// </summary>
        IssuedDateAscending = 1929,

        /// <summary>
        /// Issued Date Descending
        /// </summary>
        IssuedDateDescending = 1930,

        /// <summary>
        /// Arrival Ascending
        /// </summary>
        ArrivalAscending = 1931,

        /// <summary>
        /// Arrival Descending
        /// </summary>
        ArrivalDescending = 1932,

        /// <summary>
        /// Entered Date Ascending
        /// </summary>
        EnteredDateAscending = 1933,

        /// <summary>
        /// Entered Date Descending
        /// </summary>
        EnteredDateDescending = 1934,

        /// <summary>
        /// Returned Ascending
        /// </summary>
        ReturnedAscending = 1935,

        /// <summary>
        /// Returned Descending
        /// </summary>
        ReturnedDescending = 1936,

        /// <summary>
        /// Transfer # Ascending
        /// </summary>
        TransferAscending = 1937,

        /// <summary>
        /// Transfer # Descending
        /// </summary>
        TransferDescending = 1938,

        /// <summary>
        /// Date Ascending
        /// </summary>
        DateAscending = 1939,

        /// <summary>
        /// Date Descending
        /// </summary>
        DateDescending = 1940,

        /// <summary>
        /// Whs # Ascending
        /// </summary>
        WhsAscending = 1941,

        /// <summary>
        /// Whs # Descending
        /// </summary>
        WhsDescending = 1942,

        /// <summary>
        /// Inventory Inquiry Summary is unavailable in disconnected mode.
        /// </summary>
        InventoryInquirySummaryIsUnavailableInDisconnectedMode = 1943,

        /// <summary>
        /// Section {0} Slot {1}
        /// </summary>
        SectionSlot = 1944,

        /// <summary>
        /// Section {0}
        /// </summary>
        Sectionzz = 1945,

        /// <summary>
        /// No product at selected location.
        /// </summary>
        NoProductAtSelectedLocation = 1946,

        /// <summary>
        /// Whs {0} {1}
        /// </summary>
        Whszz = 1947,

        /// <summary>
        /// Cannot remove more than exists on skid!
        /// </summary>
        CannotRemoveMoreThanExistsOnSkid = 1948,

        /// <summary>
        /// Cannot remove negative quantity!
        /// </summary>
        CannotRemoveNegativeQuantity = 1949,

        /// <summary>
        /// Cannot remove partial boxes!
        /// </summary>
        CannotRemovePartialBoxes = 1950,

        /// <summary>
        /// Production Date: {0}
        /// </summary>
        ProductionDatez = 1951,

        /// <summary>
        /// 1 Error
        /// </summary>
        Errorz = 1952,

        /// <summary>
        /// {0} Errors
        /// </summary>
        Errors = 1953,

        /// <summary>
        /// Could not parse '{0}'. {1}
        /// </summary>
        CouldNotParse = 1954,

        /// <summary>
        /// Failed to parse '{0}'.
        /// </summary>
        FailedToParse = 1955,

        /// <summary>
        /// Failed to resize string '{0}' to length {1}.
        /// </summary>
        FailedToResizeStringToLength = 1956,

        /// <summary>
        /// Batch Lot Number
        /// </summary>
        BatchLotNumberz = 1957,

        /// <summary>
        /// Serial Number
        /// </summary>
        SerialNumber = 1958,

        /// <summary>
        /// Cannot convert {0} to a valid YYMMDD DateTime.
        /// </summary>
        CannotConvertToAValidYymmddDatetime = 1959,

        /// <summary>
        /// Production Date
        /// </summary>
        ProductionDatezz = 1960,

        /// <summary>
        /// Due Date
        /// </summary>
        DueDatez = 1961,

        /// <summary>
        /// Parse 'Packaging Date' as 'Production Date' enabled.
        /// </summary>
        ParsePackagingDateAsProductionDateEnabled = 1962,

        /// <summary>
        /// Packaging Date
        /// </summary>
        PackagingDatez = 1963,

        /// <summary>
        /// Best Before Sell By Date
        /// </summary>
        BestBeforeSellByDate = 1964,

        /// <summary>
        /// Expiry Use By Sell By Date
        /// </summary>
        ExpiryUseBySellByDate = 1965,

        /// <summary>
        /// Cannot convert {0} to an integer.
        /// </summary>
        CannotConvertToAnInteger = 1966,

        /// <summary>
        /// Product Variant
        /// </summary>
        ProductVariantz = 1967,

        /// <summary>
        /// Count of Trade Items
        /// </summary>
        CountOfTradeItemsz = 1968,

        /// <summary>
        /// Variable Count
        /// </summary>
        VariableCountz = 1969,

        /// <summary>
        /// Failed to parse '{0}'
        /// </summary>
        FailedToParsez = 1970,

        /// <summary>
        /// Metric Net Weight KG
        /// </summary>
        MetricNetWeightKg = 1971,

        /// <summary>
        /// Failed to parse '{0}'. Could not determine number of decimal places.
        /// </summary>
        FailedToParseCouldNotDetermineNumberOfDecimalPlaces = 1972,

        /// <summary>
        /// Non Metric Net Weight LB
        /// </summary>
        NonMetricNetWeightLb = 1973,

        /// <summary>
        /// Metric Gross Weight KG
        /// </summary>
        MetricGrossWeightKg = 1974,

        /// <summary>
        /// Non Metric Gross Weight LB
        /// </summary>
        NonMetricGrossWeightLb = 1975,

        /// <summary>
        /// Length in Meters
        /// </summary>
        LengthInMeters = 1976,

        /// <summary>
        /// Cannot convert {0} to a valid DDMMYY DateTime.
        /// </summary>
        CannotConvertToAValidDdmmyyDatetime = 1977,

        /// <summary>
        /// Bar Code Parse Exception
        /// </summary>
        BarCodeParseException = 1978,

        /// <summary>
        /// SerialNo
        /// </summary>
        Serialno = 1979,

        /// <summary>
        /// Prod Date
        /// </summary>
        ProdDate = 1980,

        /// <summary>
        /// BBSB Date
        /// </summary>
        BbsbDate = 1981,

        /// <summary>
        /// Lot
        /// </summary>
        Lot = 1982,

        /// <summary>
        /// BB/SB Date
        /// </summary>
        BbsbDatez = 1983,

        /// <summary>
        /// Add Product
        /// </summary>
        AddProduct = 1984,

        /// <summary>
        /// Assign Scan Data To Product
        /// </summary>
        AssignScanDataToProduct = 1985,

        /// <summary>
        /// 4. Skids
        /// </summary>
        Skidsz = 1986,

        /// <summary>
        /// 3. Counts
        /// </summary>
        Countszz = 1987,

        /// <summary>
        /// 2. Convert Units
        /// </summary>
        ConvertUnitszz = 1988,

        /// <summary>
        /// 1. Movement
        /// </summary>
        Movementz = 1989,

        /// <summary>
        /// 6. Adjustments
        /// </summary>
        Adjustmentszzz = 1990,

        /// <summary>
        /// 4. Submit Batch
        /// </summary>
        SubmitBatchz = 1991,

        /// <summary>
        /// 2. Comm, Setup
        /// </summary>
        CommSetup = 1992,

        /// <summary>
        /// 5. Utilities
        /// </summary>
        Utilitiesz = 1993,

        /// <summary>
        /// Remove
        /// </summary>
        Remove = 1994,

        /// <summary>
        /// Invalid Quantity.
        /// </summary>
        InvalidQuantityzzz = 1995,

        /// <summary>
        /// Debox Label Count
        /// </summary>
        DeboxLabelCount = 1996,

        /// <summary>
        /// 3. Move Skid
        /// </summary>
        MoveSkidzz = 1997,

        /// <summary>
        /// 4. Debox
        /// </summary>
        Debox = 1998,


    }
}
#endif