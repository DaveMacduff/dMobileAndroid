// <copyright file="DatabaseNub.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Bedrock;
    using Resources.Models;
    using Resources.Models.Contacts;
    using Resources.Models.Details;
    using Resources.Models.Headers;
    using Resources.Models.Items;
    using Resources.Models.LocationMap;
    using Resources.Models.Products;
    using SQLite;

    /// <summary>
    /// Represents the database operations and event handling for the application.
    /// </summary>
    public class DatabaseNub : IDataNubPublisher
    {
        // Event arguments for data nub events.

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseNub"/> class.
        /// </summary>
        public DatabaseNub()
        {
            // Initialize the database connection.
            this.DbConn = DatabaseConnection.GetInstance();
        }

        // Delegate for handling nub status output events.
        private delegate void NubStatOutEventHandler(
            IDataNubPublisher databaseNubEventPublisher,
            DataNubEventArgs databaseNubEventArgs);

        // Event for nub status output.
        private event NubStatOutEventHandler OnNubStatOut;

        /// <summary>
        /// Gets a value indicating whether an error has been raised.
        /// </summary>
        public bool ErrorRaised { get; private set; }

        /// <summary>
        /// Gets or sets the event arguments for data nub events.
        /// </summary>
        private DataNubEventArgs MsgEventArgs { get; set; } = new DataNubEventArgs();

        /// <summary>
        /// Gets or sets the database connection.
        /// </summary>
        private DatabaseConnection DbConn { get; set; }

        /// <summary>
        /// Creates a new SQL Server database called dMobile.
        /// Deletes the database if it already exists.
        /// </summary>
        /// <returns>Task completed.</returns>
        /// <remarks>Password is 'dMobile'.</remarks>
        public async Task CreateNewDatabase()
        {
            var fileName = Path.Combine(this.DbConn.DatabasePath, this.DbConn.DatabaseName);

            // Close and dispose the existing database connection.
            await DatabaseConnection.Close();
            DatabaseConnection.Dispose();

            // Check if the database file exists.
            if (File.Exists(fileName))
            {
                // Notify listeners about the database deletion.
                if (this.OnNubStatOut != null)
                {
                    this.MsgEventArgs.StatusOutMsg = "Deleting database " + fileName;
                    this.OnNubStatOut(this, this.MsgEventArgs);
                }

                // Delete the existing database file.
                File.Delete(fileName);
            }

            // Notify listeners about the database creation.
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating database " + fileName;
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create a new database file.
            File.Create(fileName);

            // Reinitialize the database connection.
            DatabaseConnection.GetInstance();
        }

        /// <summary>
        /// Creates all necessary tables in the database.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task CreateTables()
        {
            try
            {
                // Create contact-related tables.
                await this.CreateTableContact();
                await this.CreateTableContactUser();

                // Create location map-related tables.
                await this.CreateTableLocationMap();
                await this.CreateTableLocationMapType();
                await this.CreateTableLocationMapWarehouse();
                await this.CreateTableLocationMapSection();
                await this.CreateTableLocationMapSlotCoordinate();
                await this.CreateTableLocationMapSlot();
                await this.CreateTableWarehouseLocationMapType();

                // Create unit and route tables.
                await this.CreateTableUnit();
                await this.CreateTableRoute();
                await this.CreateTableReasonCode();
                await this.CreateTableFamily();

                // Create customer and vendor-related tables.
                await this.CreateTableContactCustomer();
                await this.CreateTableContactCustomerShipTo();
                await this.CreateTableContactVendor();
                await this.CreateTableContactVendorSupplier();

                // Create product scheme-related tables.
                await this.CreateTableProductScheme();
                await this.CreateTableProductSchemeVariant();
                await this.CreateTableProductSchemeVariantXRef();
                await this.CreateTableProductSchemeLotNumber();
                await this.CreateTableProductSchemeLotNumberXRef();

                // Create product-related tables.
                await this.CreateTableProduct();
                await this.CreateTableProductUnit();
                await this.CreateTableProductUnitRelation();
                await this.CreateTableProductUnitBarCode();

                // Create product location map type table.
                await this.CreateTableProductLocationMapType();
                await this.CreateTableLocationMapWarehouseProductScheme();

                // Create activity-related tables.
                await this.CreateTableActivity();
                await this.CreateTableActivityItem();
                await this.CreateTableActivityItemReasonCodeXRef();
                await this.CreateTableActivityItemBarCodeAssociationXRef();

                // Create activity detail-related tables.
                await this.CreateTableActivityDetail();
                await this.CreateTableActivityDetailExtra();
                await this.CreateTableActivityDetailProductSchemeVariantXRef();

                // Create activity count-related tables.
                await this.CreateTableActivityCount();
                await this.CreateTableActivityItemCount();
                await this.CreateTableActivityDetailCount();

                // Create activity picking-related tables.
                await this.CreateTableActivityPicking();
                await this.CreateTableActivityItemPicking();
                await this.CreateTableActivityItemPickingComment();
                await this.CreateTableActivityDetailPicking();
                await this.CreateTableActivityPickingRoute();

                // Create activity returns-related tables.
                await this.CreateTableActivityReturns();
                await this.CreateTableActivityItemReturns();
                await this.CreateTableActivityItemReturnsComment();
                await this.CreateTableActivityDetailReturns();

                // Create activity receiving-related tables.
                await this.CreateTableActivityReceiving();
                await this.CreateTableActivityItemReceiving();
                await this.CreateTableActivityItemReceivingComment();
                await this.CreateTableActivityDetailReceiving();

                // Create activity transfer-related tables.
                await this.CreateTableActivityTransfer();
                await this.CreateTableActivityItemTransfer();
                await this.CreateTableActivityDetailTransfer();

                // Create activity adjustment-related tables.
                await this.CreateTableActivityAdjustment();
                await this.CreateTableActivityItemAdjustment();
                await this.CreateTableActivityDetailAdjustment();

                // Create activity production-related tables.
                await this.CreateTableActivityProduction();
                await this.CreateTableActivityProductionFinished();
                await this.CreateTableActivityProductionMaterial();
                await this.CreateTableActivityProductionByProduct();
                await this.CreateTableActivityItemProduction();
                await this.CreateTableActivityItemProductionFinished();
                await this.CreateTableActivityItemProductionMaterial();
                await this.CreateTableActivityItemProductionByProduct();
                await this.CreateTableActivityDetailProductionFinished();
                await this.CreateTableActivityDetailProductionMaterial();
                await this.CreateTableActivityDetailProductionByProduct();

                // Create activity cut sheet-related tables.
                await this.CreateTableActivityCutSheet();
                await this.CreateTableActivityCutSheetFinished();
                await this.CreateTableActivityCutSheetMaterial();
                await this.CreateTableActivityItemCutSheet();
                await this.CreateTableActivityItemCutSheetFinished();
                await this.CreateTableActivityItemCutSheetMaterial();
                await this.CreateTableActivityDetailCutSheetFinished();
                await this.CreateTableActivityDetailCutSheetMaterial();

                // Create activity yield sheet-related tables.
                await this.CreateTableActivityYieldSheet();
                await this.CreateTableActivityYieldSheetFinished();
                await this.CreateTableActivityYieldSheetMaterial();
                await this.CreateTableActivityItemYieldSheet();
                await this.CreateTableActivityItemYieldSheetFinished();
                await this.CreateTableActivityItemYieldSheetMaterial();
                await this.CreateTableActivityDetailYieldSheetFinished();
                await this.CreateTableActivityDetailYieldSheetMaterial();

                // Create activity purchase order worksheet table.
                await this.CreateTableActivityItemPurchaseOrderWorkSheet();

                // Create activity replenishment-related tables.
                await this.CreateTableActivityReplenishment();
                await this.CreateTableActivityItemReplenishment();
                await this.CreateTableActivityDetailReplenishment();
            }
            catch (SQLiteException)
            {
                // Set error flag and rethrow the exception.
                this.ErrorRaised = true;
                throw;
            }
            catch (Exception)
            {
                // Set error flag and rethrow the exception.
                this.ErrorRaised = true;
                throw;
            }
        }

        /// <inheritdoc/>
        public void RegNubListenerStatOut(IDatabaseNub databaseNubEvent)
        {
            // Register a listener for nub status output events.
            this.OnNubStatOut += databaseNubEvent.OnNubStatOut;
        }

        /// <inheritdoc/>
        public void UnregNubListenerStatOut(IDatabaseNub databaseNubEvent)
        {
            // Unregister a listener for nub status output events.
            this.OnNubStatOut -= databaseNubEvent.OnNubStatOut;
        }

        /// <summary>
        /// Creates the activity table.
        /// </summary>
        private async Task CreateTableActivity()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating activity table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityModel>();
        }

        /// <summary>
        /// Creates the detail product scheme variant xref table.
        /// </summary>
        private async Task CreateTableActivityDetailProductSchemeVariantXRef()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the detail product scheme variant xref table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating detail product scheme variant xref table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityDetailProductSchemeVariantXRefModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailProductSchemeVariantXRefModel>();
        }

        /// <summary>
        /// Creates the activity item table.
        /// </summary>
        private async Task CreateTableActivityItem()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity item table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating activity item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityItemModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemModel>();
        }

        /// <summary>
        /// Creates the activity detail table.
        /// </summary>
        /// <remarks>SN:12345678 BB:11/12/2010 BL:001.FREEZER.123456 1TT=100EA@300KG.</remarks>
        private async Task CreateTableActivityDetail()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity detail table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating activity detail table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityDetailModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailModel>();
        }

        /// <summary>
        /// Creates the detail extra table.
        /// </summary>
        private async Task CreateTableActivityDetailExtra()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the detail extra table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating detail extra table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityDetailExtraModel table asynchronously.
            var result = await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailExtraModel>();
            if (result != CreateTableResult.Created)
            {
                // Throw an exception if the table creation failed.
                throw new Exception("Failed to create table ActivityDetailExtraModel");
            }
        }

        /// <summary>
        /// Creates the count table.
        /// </summary>
        private async Task CreateTableActivityCount()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the count table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating count table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityCountModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityCountModel>();
        }

        /// <summary>
        /// Creates the picking table.
        /// </summary>
        private async Task CreateTableActivityPicking()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the picking table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating picking table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityPickingModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityPickingModel>();
        }

        /// <summary>
        /// Creates the returns table.
        /// </summary>
        private async Task CreateTableActivityReturns()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the returns table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating returns table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityReturnsModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityReturnsModel>();
        }

        /// <summary>
        /// Creates the picking route table.
        /// </summary>
        private async Task CreateTableActivityPickingRoute()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the picking route table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating picking route table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityPickingRouteModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityPickingRouteModel>();
        }

        /// <summary>
        /// Creates the receiving table.
        /// </summary>
        private async Task CreateTableActivityReceiving()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the receiving table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating receiving table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityReceivingModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityReceivingModel>();
        }

        /// <summary>
        /// Creates the replenishment table.
        /// </summary>
        private async Task CreateTableActivityReplenishment()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the replenishment table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating replenishment table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityReplenishmentModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityReplenishmentModel>();
        }

        /// <summary>
        /// Creates the transfer table.
        /// </summary>
        private async Task CreateTableActivityTransfer()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the transfer table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating transfer table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityTransferModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityTransferModel>();
        }

        /// <summary>
        /// Creates the count item table.
        /// </summary>
        private async Task CreateTableActivityItemCount()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the count item table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating count item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityItemCountModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemCountModel>();
        }

        /// <summary>
        /// Creates the picking item table.
        /// </summary>
        private async Task CreateTableActivityItemPicking()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the picking item table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating picking item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityItemPickingModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemPickingModel>();
        }

        /// <summary>
        /// Creates the returns item table.
        /// </summary>
        private async Task CreateTableActivityItemReturns()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the returns item table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating returns item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityItemReturnsModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemReturnsModel>();
        }

        /// <summary>
        /// Creates the picking comment item table.
        /// </summary>
        private async Task CreateTableActivityItemPickingComment()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the picking comment item table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating picking comment item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityItemPickingCommentModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemPickingCommentModel>();
        }

        /// <summary>
        /// Creates the returns comment item table.
        /// </summary>
        private async Task CreateTableActivityItemReturnsComment()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the returns comment item table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating returns comment item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityItemReturnsCommentModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemReturnsCommentModel>();
        }

        /// <summary>
        /// Creates the receiving item table.
        /// </summary>
        private async Task CreateTableActivityItemReceiving()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the receiving item table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating receiving item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityItemReceivingModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemReceivingModel>();
        }

        /// <summary>
        /// Creates the replenishment item table.
        /// </summary>
        private async Task CreateTableActivityItemReplenishment()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the replenishment item table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating replenishment item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityItemReplenishmentModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemReplenishmentModel>();
        }

        /// <summary>
        /// Creates the purchase order work sheet item table.
        /// </summary>
        private async Task CreateTableActivityItemPurchaseOrderWorkSheet()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the purchase order work sheet item table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating purchase order work sheet item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityItemPurchaseOrderWorkSheetModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemPurchaseOrderWorkSheetModel>();
        }

        /// <summary>
        /// Creates the receiving comment item table.
        /// </summary>
        private async Task CreateTableActivityItemReceivingComment()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the receiving comment item table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating receiving comment item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityItemReceivingCommentModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemReceivingCommentModel>();
        }

        /// <summary>
        /// Creates the transfer item table.
        /// </summary>
        private async Task CreateTableActivityItemTransfer()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the transfer item table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating transfer item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityItemTransferModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemTransferModel>();
        }

        /// <summary>
        /// Creates the bar code association item table.
        /// </summary>
        private async Task CreateTableActivityItemBarCodeAssociationXRef()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the bar code association item table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating bar code association item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityItemBarCodeAssociationXRefModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemBarCodeAssociationXRefModel>();
        }

        /// <summary>
        /// Creates the count detail table.
        /// </summary>
        private async Task CreateTableActivityDetailCount()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the count detail table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating count detail table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityDetailCountModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailCountModel>();
        }

        /// <summary>
        /// Creates the picking detail table.
        /// </summary>
        private async Task CreateTableActivityDetailPicking()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the picking detail table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating picking detail table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityDetailPickingModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailPickingModel>();
        }

        /// <summary>
        /// Creates the returns detail table.
        /// </summary>
        private async Task CreateTableActivityDetailReturns()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the returns detail table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating returns detail table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityDetailReturnsModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailReturnsModel>();
        }

        /// <summary>
        /// Creates the receiving detail table.
        /// </summary>
        private async Task CreateTableActivityDetailReceiving()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the receiving detail table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating receiving detail table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityDetailReceivingModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailReceivingModel>();
        }

        /// <summary>
        /// Creates the replenishment detail table.
        /// </summary>
        private async Task CreateTableActivityDetailReplenishment()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the replenishment detail table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating replenishment detail table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityDetailReplenishmentModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailReplenishmentModel>();
        }

        /// <summary>
        /// Creates the transfer detail table.
        /// </summary>
        private async Task CreateTableActivityDetailTransfer()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the transfer detail table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating transfer detail table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityDetailTransferModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailTransferModel>();
        }

        /// <summary>
        /// Creates the unit of measure table.
        /// </summary>
        private async Task CreateTableUnit()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the unit of measure table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating unit of measure table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the UnitModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<UnitModel>();
        }

        /// <summary>
        /// Creates the routes table.
        /// </summary>
        private async Task CreateTableRoute()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the routes table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating routes table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the RouteModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<RouteModel>();
        }

        /// <summary>
        /// Creates the location map table.
        /// </summary>
        private async Task CreateTableLocationMap()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the location map table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating location map table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the LocationMapModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<LocationMapModel>();
        }

        /// <summary>
        /// Creates the warehouse product scheme table.
        /// </summary>
        private async Task CreateTableLocationMapWarehouseProductScheme()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the warehouse product scheme table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating warehouse product scheme table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the LocationMapWarehouseProductSchemeModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<LocationMapWarehouseProductSchemeModel>();
        }

        /// <summary>
        /// Creates the location map warehouse table.
        /// </summary>
        private async Task CreateTableLocationMapWarehouse()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the location map warehouse table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating location map warehouse table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the LocationMapWarehouseModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<LocationMapWarehouseModel>();
        }

        /// <summary>
        /// Creates the location map section table.
        /// </summary>
        private async Task CreateTableLocationMapSection()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the location map section table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating location map section table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the LocationMapSectionModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<LocationMapSectionModel>();
        }

        /// <summary>
        /// Creates the location map slot coordinate table.
        /// </summary>
        private async Task CreateTableLocationMapSlotCoordinate()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the location map slot coordinate table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating location map slot table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the LocationMapSlotCoordinateModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<LocationMapSlotCoordinateModel>();
        }

        /// <summary>
        /// Creates the location map slot table.
        /// </summary>
        private async Task CreateTableLocationMapSlot()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the location map slot table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating location map slot table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the LocationMapSlotModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<LocationMapSlotModel>();
        }

        /// <summary>
        /// Creates the location map type table.
        /// </summary>
        private async Task CreateTableLocationMapType()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the location map type table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating location map type table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the LocationMapTypeModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<LocationMapTypeModel>();
        }

        /// <summary>
        /// Creates the product scheme variants cross-reference table.
        /// </summary>
        private async Task CreateTableProductSchemeVariantXRef()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the product scheme variants cross-reference table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating product scheme variants xref table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ProductSchemeVariantXRefModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ProductSchemeVariantXRefModel>();
        }

        /// <summary>
        /// Creates the product scheme lot numbers cross-reference table.
        /// </summary>
        private async Task CreateTableProductSchemeLotNumberXRef()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the product scheme lot numbers cross-reference table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating product scheme lot numbers xref table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ProductSchemeLotNumberXRefModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ProductSchemeLotNumberXRefModel>();
        }

        /// <summary>
        /// Creates the product scheme table.
        /// </summary>
        private async Task CreateTableProductScheme()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the product scheme table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating product scheme table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ProductSchemeModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ProductSchemeModel>();
        }

        /// <summary>
        /// Creates the product scheme variants table.
        /// </summary>
        private async Task CreateTableProductSchemeVariant()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the product scheme variants table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating product scheme variants table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ProductSchemeVariantModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ProductSchemeVariantModel>();
        }

        /// <summary>
        /// Creates the product scheme lot number table.
        /// </summary>
        private async Task CreateTableProductSchemeLotNumber()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the product scheme lot number table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating product scheme lots table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ProductSchemeLotNumberModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ProductSchemeLotNumberModel>();
        }

        /// <summary>
        /// Creates the product table.
        /// </summary>
        private async Task CreateTableProduct()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the product table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating products table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ProductModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ProductModel>();
        }

        /// <summary>
        /// Creates the product unit table.
        /// </summary>
        private async Task CreateTableProductUnit()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the product unit table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating product units table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ProductUnitModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ProductUnitModel>();
        }

        /// <summary>
        /// Creates the product unit bar code table.
        /// </summary>
        private async Task CreateTableProductUnitBarCode()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the product unit bar code table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating product unit bar codes table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ProductUnitBarCodeModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ProductUnitBarCodeModel>();
        }

        /// <summary>
        /// Creates the product unit relation table.
        /// </summary>
        private async Task CreateTableProductUnitRelation()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the product unit relation table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating product unit relations table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ProductUnitRelationModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ProductUnitRelationModel>();
        }

        /// <summary>
        /// Creates the warehouse location map type table.
        /// </summary>
        private async Task CreateTableWarehouseLocationMapType()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the warehouse location map type table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating warehouse location map table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the WarehouseLocationMapTypeModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<WarehouseLocationMapTypeModel>();
        }

        /// <summary>
        /// Creates the product location map type table.
        /// </summary>
        private async Task CreateTableProductLocationMapType()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the product location map type table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating product location map table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ProductLocationMapTypeModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ProductLocationMapTypeModel>();
        }

        /// <summary>
        /// Creates the contact table.
        /// </summary>
        private async Task CreateTableContact()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the contact table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating contacts table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ContactModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ContactModel>();
        }

        /// <summary>
        /// Creates the contact user table.
        /// </summary>
        private async Task CreateTableContactUser()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the contact user table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating contact users table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ContactUserModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ContactUserModel>();
        }

        /// <summary>
        /// Creates the contact customer table.
        /// </summary>
        private async Task CreateTableContactCustomer()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the contact customer table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating contact customers table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ContactCustomerModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ContactCustomerModel>();
        }

        /// <summary>
        /// Creates the contact customer ship-to table.
        /// </summary>
        private async Task CreateTableContactCustomerShipTo()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the contact customer ship-to table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating contact customer shipto's table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ContactCustomerShipToModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ContactCustomerShipToModel>();
        }

        /// <summary>
        /// Creates the contact vendor table.
        /// </summary>
        private async Task CreateTableContactVendor()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the contact vendor table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating contact vendors table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ContactVendorModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ContactVendorModel>();
        }

        /// <summary>
        /// Creates the contact vendor supplier table.
        /// </summary>
        private async Task CreateTableContactVendorSupplier()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the contact vendor supplier table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating contact vendor suppliers table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ContactVendorSupplierModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ContactVendorSupplierModel>();
        }

        /// <summary>
        /// Creates the reason code table.
        /// </summary>
        private async Task CreateTableReasonCode()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the reason code table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating reason code table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ReasonCodeModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ReasonCodeModel>();
        }

        /// <summary>
        /// Creates the activity item reason code cross-reference table.
        /// </summary>
        private async Task CreateTableActivityItemReasonCodeXRef()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity item reason code cross-reference table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating item reason code xref table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityItemReasonCodeXRefModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemReasonCodeXRefModel>();
        }

        /////// <summary>
        /////// Creates the activity item convert unit table.
        /////// </summary>
        ////private async Task CreateTableActivityItemConvertUnit()
        ////{
        ////    if (this.OnNubStatOut != null)
        ////    {
        ////        // Notify listeners that the activity item convert unit table is being created.
        ////        this.MsgEventArgs.StatusOutMsg = "Creating item convert units table.";
        ////        this.OnNubStatOut(this, this.MsgEventArgs);
        ////    }

        ////    // Create the ActivityItemConvertUnitModel table asynchronously.
        ////    await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemConvertUnitModel>();
        ////}

        /////// <summary>
        /////// Creates the activity detail convert unit table.
        /////// </summary>
        ////private async Task CreateTableActivityDetailConvertUnit()
        ////{
        ////    if (this.OnNubStatOut != null)
        ////    {
        ////        // Notify listeners that the activity detail convert unit table is being created.
        ////        this.MsgEventArgs.StatusOutMsg = "Creating detail convert units table.";
        ////        this.OnNubStatOut(this, this.MsgEventArgs);
        ////    }

        ////    // Create the ActivityDetailConvertUnitModel table asynchronously.
        ////    await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailConvertUnitModel>();
        ////}

        /////// <summary>
        /////// Creates the activity item move inventory table.
        /////// </summary>
        ////private async Task CreateTableActivityItemMoveInventory()
        ////{
        ////    if (this.OnNubStatOut != null)
        ////    {
        ////        // Notify listeners that the activity item move inventory table is being created.
        ////        this.MsgEventArgs.StatusOutMsg = "Creating item move inventory table.";
        ////        this.OnNubStatOut(this, this.MsgEventArgs);
        ////    }

        ////    // Create the ActivityItemMoveInventoryModel table asynchronously.
        ////    await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemMoveInventoryModel>();
        ////}

        /////// <summary>
        /////// Creates the activity detail move inventory table.
        /////// </summary>
        ////private async Task CreateTableActivityDetailMoveInventory()
        ////{
        ////    if (this.OnNubStatOut != null)
        ////    {
        ////        // Notify listeners that the activity detail move inventory table is being created.
        ////        this.MsgEventArgs.StatusOutMsg = "Creating detail move inventory table.";
        ////        this.OnNubStatOut(this, this.MsgEventArgs);
        ////    }

        ////    // Create the ActivityDetailMoveInventoryModel table asynchronously.
        ////    await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailMoveInventoryModel>();
        ////}

        /// <summary>
        /// Creates the activity adjustment table.
        /// </summary>
        private async Task CreateTableActivityAdjustment()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity adjustment table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating adjustment table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityAdjustmentModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityAdjustmentModel>();
        }

        /// <summary>
        /// Creates the activity item adjustment table.
        /// </summary>
        private async Task CreateTableActivityItemAdjustment()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity item adjustment table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating item adjustment table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityItemAdjustmentModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemAdjustmentModel>();
        }

        /// <summary>
        /// Creates the activity detail adjustment table.
        /// </summary>
        private async Task CreateTableActivityDetailAdjustment()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity detail adjustment table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating detail adjustment table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityDetailAdjustmentModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailAdjustmentModel>();
        }

        /// <summary>
        /// Creates the activity cut sheet table.
        /// </summary>
        private async Task CreateTableActivityCutSheet()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity cut sheet table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating cut sheet table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityCutSheetModel>();
        }

        /// <summary>
        /// Creates the activity cut sheet finished table.
        /// </summary>
        private async Task CreateTableActivityCutSheetFinished()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity cut sheet finished table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating cut sheet finished table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityCutSheetFinishedModel>();
        }

        /// <summary>
        /// Creates the activity cut sheet material table.
        /// </summary>
        private async Task CreateTableActivityCutSheetMaterial()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity cut sheet material table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating cut sheet material table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityCutSheetMaterialModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityCutSheetMaterialModel>();
        }

        /// <summary>
        /// Creates the activity item cut sheet table.
        /// </summary>
        private async Task CreateTableActivityItemCutSheet()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity item cut sheet table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating item cut sheet table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemCutSheetModel>();
        }

        /// <summary>
        /// Creates the activity item cut sheet finished table.
        /// </summary>
        private async Task CreateTableActivityItemCutSheetFinished()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity item cut sheet finished table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating item cut sheet finished table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemCutSheetFinishedModel>();
        }

        /// <summary>
        /// Creates the activity item cut sheet material table.
        /// </summary>
        private async Task CreateTableActivityItemCutSheetMaterial()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity item cut sheet material table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating item cut sheet material table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemCutSheetMaterialModel>();
        }

        /// <summary>
        /// Creates the activity detail cut sheet finished table.
        /// </summary>
        private async Task CreateTableActivityDetailCutSheetFinished()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity detail cut sheet finished table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating detail cut sheet finished table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailCutSheetFinishedModel>();
        }

        /// <summary>
        /// Creates the activity detail cut sheet material table.
        /// </summary>
        private async Task CreateTableActivityDetailCutSheetMaterial()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity detail cut sheet material table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating detail cut sheet material table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityDetailProductionMaterialModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailCutSheetMaterialModel>();
        }

        /// <summary>
        /// Creates the activity yield sheet table.
        /// </summary>
        private async Task CreateTableActivityYieldSheet()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity yield sheet table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating yield sheet table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityYieldSheetModel>();
        }

        /// <summary>
        /// Creates the activity yield sheet finished table.
        /// </summary>
        private async Task CreateTableActivityYieldSheetFinished()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity yield sheet finished table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating yield sheet finished table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityYieldSheetFinishedModel>();
        }

        /// <summary>
        /// Creates the activity yield sheet material table.
        /// </summary>
        private async Task CreateTableActivityYieldSheetMaterial()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity yield sheet material table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating yield sheet material table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityCutSheetMaterialModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityYieldSheetMaterialModel>();
        }

        /// <summary>
        /// Creates the activity item yield sheet table.
        /// </summary>
        private async Task CreateTableActivityItemYieldSheet()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity item yield sheet table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating item yield sheet table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemYieldSheetModel>();
        }

        /// <summary>
        /// Creates the activity item yield sheet finished table.
        /// </summary>
        private async Task CreateTableActivityItemYieldSheetFinished()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity item yield sheet finished table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating item yield sheet finished table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemYieldSheetFinishedModel>();
        }

        /// <summary>
        /// Creates the activity item yield sheet material table.
        /// </summary>
        private async Task CreateTableActivityItemYieldSheetMaterial()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity item yield sheet material table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating item yield sheet material table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemYieldSheetMaterialModel>();
        }

        /// <summary>
        /// Creates the activity detail yield sheet finished table.
        /// </summary>
        private async Task CreateTableActivityDetailYieldSheetFinished()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity detail yield sheet finished table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating detail yield sheet finished table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailYieldSheetFinishedModel>();
        }

        /// <summary>
        /// Creates the activity detail yield sheet material table.
        /// </summary>
        private async Task CreateTableActivityDetailYieldSheetMaterial()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity detail yield sheet material table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating detail yield sheet material table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityDetailProductionMaterialModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailYieldSheetMaterialModel>();
        }

        /// <summary>
        /// Creates the activity production table.
        /// </summary>
        private async Task CreateTableActivityProduction()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity production table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating production table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityProductionModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityProductionModel>();
        }

        /// <summary>
        /// Creates the activity production finished table.
        /// </summary>
        private async Task CreateTableActivityProductionFinished()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity production finished table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating production finished table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityProductionFinishedModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityProductionFinishedModel>();
        }

        /// <summary>
        /// Creates the activity production material table.
        /// </summary>
        private async Task CreateTableActivityProductionMaterial()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity production material table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating production material table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityProductionMaterialModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityProductionMaterialModel>();
        }

        /// <summary>
        /// Creates the activity production by-product table.
        /// </summary>
        private async Task CreateTableActivityProductionByProduct()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity production by-product table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating production byproduct table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityProductionByProductModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityProductionByProductModel>();
        }

        /// <summary>
        /// Creates the activity item production table.
        /// </summary>
        private async Task CreateTableActivityItemProduction()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity item production table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating item production table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityItemProductionModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemProductionModel>();
        }

        /// <summary>
        /// Creates the activity item production finished table.
        /// </summary>
        private async Task CreateTableActivityItemProductionFinished()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity item production finished table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating item production finished table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityItemProductionFinishedModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemProductionFinishedModel>();
        }

        /// <summary>
        /// Creates the activity item production material table.
        /// </summary>
        private async Task CreateTableActivityItemProductionMaterial()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity item production material table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating item production material table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityItemProductionMaterialModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemProductionMaterialModel>();
        }

        /// <summary>
        /// Creates the activity item production by-product table.
        /// </summary>
        private async Task CreateTableActivityItemProductionByProduct()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity item production by-product table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating item production byproduct table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityItemProductionByProductModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityItemProductionByProductModel>();
        }

        /// <summary>
        /// Creates the activity detail production finished table.
        /// </summary>
        private async Task CreateTableActivityDetailProductionFinished()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity detail production finished table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating detail production finished table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityDetailProductionFinishedModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailProductionFinishedModel>();
        }

        /// <summary>
        /// Creates the activity detail production material table.
        /// </summary>
        private async Task CreateTableActivityDetailProductionMaterial()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity detail production material table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating detail production material table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityDetailProductionMaterialModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailProductionMaterialModel>();
        }

        /// <summary>
        /// Creates the activity detail production by-product table.
        /// </summary>
        private async Task CreateTableActivityDetailProductionByProduct()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the activity detail production by-product table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating detail production byproduct table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the ActivityDetailProductionByProductModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<ActivityDetailProductionByProductModel>();
        }

        /// <summary>
        /// Creates the family table.
        /// </summary>
        private async Task CreateTableFamily()
        {
            if (this.OnNubStatOut != null)
            {
                // Notify listeners that the family table is being created.
                this.MsgEventArgs.StatusOutMsg = "Creating families table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            // Create the FamilyModel table asynchronously.
            await DatabaseConnection.GetInstance().SqlConn.CreateTableAsync<FamilyModel>();
        }
    }
}
