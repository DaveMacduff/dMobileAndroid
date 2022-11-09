// <copyright file="DatabaseNub.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
    using SQLite;

    public class DatabaseNub : IDataNubPublisher
    {
        private DataNubEventArgs msgEventArgs = new DataNubEventArgs();

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
/// <summary>
/// Initializes a new instance of the <see cref="DatabaseNub"/> class.
/// </summary>
        public DatabaseNub()
        {
            this.DBConn = DatabaseConnection.GetInstance();
        }

        private delegate void OnNubStatOutEventHandler(
            IDataNubPublisher databaseNubEventPublisher,
            DataNubEventArgs databaseNubEventArgs);

        private event OnNubStatOutEventHandler OnNubStatOut;

        public DataNubEventArgs MsgEventArgs
        {
            get => this.msgEventArgs;

            set => this.msgEventArgs = value;
        }

        public DatabaseConnection DBConn { get; set; }

        public bool ErrorRaised { get; set; } = false;

        /// <summary>
        /// Create a new sql server database called dMobile
        /// Delete the database if it already exists.
        /// </summary>
        /// <remarks>Password is 'dMobile'.</remarks>
        public async Task CreateNewDatabase()
        {
            var dbASCIIZ = Path.Combine(this.DBConn.DBPath, this.DBConn.DBName);

            await DatabaseConnection.Close();
            DatabaseConnection.Dispose();

            if (File.Exists(dbASCIIZ))
            {
                if (this.OnNubStatOut != null)
                {
                    this.MsgEventArgs.StatusOutMsg = "Deleting database " + dbASCIIZ;
                    this.OnNubStatOut(this, this.MsgEventArgs);
                }

                File.Delete(dbASCIIZ);
            }

            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating database " + dbASCIIZ;
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            File.Create(dbASCIIZ);

            DatabaseConnection.GetInstance();
            // DatabaseConnection.OpenDataBaseConnection();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
/// <summary>
///
/// </summary>
/// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task CreateTables()
        {
            try
            {
                await this.CreateTableContact();
                await this.CreateTableContactUser();

                await this.CreateTableLocationMap();
                await this.CreateTableLocationMapType();
                await this.CreateTableLocationMapWarehouse();
                await this.CreateTableLocationMapSection();
                await this.CreateTableLocationMapSlotCoordinate();
                await this.CreateTableLocationMapSlot();
                await this.CreateTableWarehouseLocationMapType();

                await this.CreateTableUnit();
                await this.CreateTableRoute();
                await this.CreateTableReasonCode();
                await this.CreateTableFamily();

                await this.CreateTableContactCustomer();
                await this.CreateTableContactCustomerShipTo();
                await this.CreateTableContactVendor();
                await this.CreateTableContactVendorSupplier();

                await this.CreateTableProductScheme();
                await this.CreateTableProductSchemeVariant();
                await this.CreateTableProductSchemeVariantXRef();
                await this.CreateTableProductSchemeLotNumber();
                await this.CreateTableProductSchemeLotNumberXRef();

                await this.CreateTableProduct();

                await this.CreateTableProductUnit();
                await this.CreateTableProductUnitRelation();
                await this.CreateTableProductUnitBarCode();

                await this.CreateTableProductLocationMapType();
                await this.CreateTableLocationMapWarehouseProductScheme();

                await this.CreateTableActivity();
                await this.CreateTableActivityItem();
                await this.CreateTableActivityItemReasonCodeXRef();
                await this.CreateTableActivityItemBarCodeAssociationXRef();

                await this.CreateTableActivityDetail();
                await this.CreateTableActivityDetailExtra();
                await this.CreateTableActivityDetailProductSchemeVariantXRef();

                await this.CreateTableActivityCount();
                await this.CreateTableActivityItemCount();
                await this.CreateTableActivityDetailCount();

                await this.CreateTableActivityPicking();
                await this.CreateTableActivityItemPicking();
                await this.CreateTableActivityItemPickingComment();
                await this.CreateTableActivityDetailPicking();
                await this.CreateTableActivityPickingRoute();

                await this.CreateTableActivityReturns();
                await this.CreateTableActivityItemReturns();
                await this.CreateTableActivityItemReturnsComment();
                await this.CreateTableActivityDetailReturns();

                await this.CreateTableActivityReceiving();
                await this.CreateTableActivityItemReceiving();
                await this.CreateTableActivityItemReceivingComment();
                await this.CreateTableActivityDetailReceiving();

                await this.CreateTableActivityTransfer();
                await this.CreateTableActivityItemTransfer();
                await this.CreateTableActivityDetailTransfer();

                await this.CreateTableActivityAdjustment();
                await this.CreateTableActivityItemAdjustment();
                await this.CreateTableActivityDetailAdjustment();

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

                await this.CreateTableActivityItemPurchaseOrderWorkSheet();
            }
            catch (SQLiteException ex)
            {
                this.ErrorRaised = true;

                throw ex;
            }
            catch (Exception ex)
            {
                this.ErrorRaised = true;

                throw ex;
            }
        }

        public void RegNubListenerStatOut(IDataNubListener dataNubEventListener)
        {
            this.OnNubStatOut += new OnNubStatOutEventHandler(dataNubEventListener.OnNubStatOut);
        }

        public void UnregNubListenerStatOut(IDataNubListener dataNubEventListener)
        {
            this.OnNubStatOut -= new OnNubStatOutEventHandler(dataNubEventListener.OnNubStatOut);
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivity()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating activity table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<ActivityModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityDetailProductSchemeVariantXRef()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating detail product scheme variant xref table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityDetailProductSchemeVariantXRefModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItem()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating activity item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemModel>();
        }

        /// <summary>
        ///  SN:12345678 BB:11/12/2010 BL:001.FREEZER.123456   1TT=100EA@300KG.
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityDetail()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating activity detail table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityDetailModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityDetailExtra()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating detail extra table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityDetailExtraModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityCount()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating count table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<ActivityCountModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityPicking()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating picking table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityPickingModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityReturns()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating returns table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityReturnsModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityPickingRoute()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating picking route table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityPickingRouteModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityReceiving()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating receiving table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityReceivingModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityTransfer()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating transfer table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityTransferModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItemCount()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating count item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemCountModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItemPicking()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating picking item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemPickingModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItemReturns()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating returns item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemReturnsModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItemPickingComment()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating picking comment item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemPickingCommentModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItemReturnsComment()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating returns comment item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemReturnsCommentModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItemReceiving()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating receiving item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemReceivingModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItemPurchaseOrderWorkSheet()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating purchase order work sheet item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemPurchaseOrderWorkSheetModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItemReceivingComment()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating receiving comment item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemReceivingCommentModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItemTransfer()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating transfer item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemTransferModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItemBarCodeAssociationXRef()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating bar code association item table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemBarCodeAssociationXRefModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityDetailCount()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating count detail table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<ActivityDetailCountModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityDetailPicking()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating picking detail table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityDetailPickingModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityDetailReturns()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating returns detail table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityDetailReturnsModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityDetailReceiving()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating receiving detail table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityDetailReceivingModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityDetailTransfer()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating transfer detail table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityDetailTransferModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableUnit()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating unit of measure table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tUnitModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableRoute()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating routes table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tRouteModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableLocationMap()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating location map table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tLocationMapModel>();
        }

        /// <summary>
        ///
        /// </summary>
        private async Task CreateTableLocationMapWarehouseProductScheme()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating warehouse product scheme table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tLocationMapWarehouseProductSchemeModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableLocationMapWarehouse()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating location map warehouse table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tLocationMapWarehouseModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableLocationMapSection()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating location map section table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tLocationMapSectionModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableLocationMapSlotCoordinate()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating location map slot table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tLocationMapSlotCoordinateModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableLocationMapSlot()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating location map slot table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tLocationMapSlotModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableLocationMapType()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating location map type table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tLocationMapTypeModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableProductSchemeVariantXRef()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating product scheme variants xref table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tProductSchemeVariantXRefModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableProductSchemeLotNumberXRef()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating product scheme lot numbers xref table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tProductSchemeLotNumberXRefModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableProductScheme()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating product scheme table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tProductSchemeModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableProductSchemeVariant()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating product scheme variants table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tProductSchemeVariantModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableProductSchemeLotNumber()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating product scheme lots table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tProductSchemeLotNumberModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableProduct()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating products table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tProductModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableProductUnit()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating product units table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tProductUnitModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableProductUnitBarCode()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating product unit bar codes table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tProductUnitBarCodeModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableProductUnitRelation()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating product unit relations table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tProductUnitRelationModel>();
        }

        /// <summary>
        ///
        /// </summary>
        private async Task CreateTableWarehouseLocationMapType()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating warehouse location map table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tWarehouseLocationMapTypeModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableProductLocationMapType()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating product location map table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tProductLocationMapTypeModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableContact()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating contacts table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<ContactModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableContactUser()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating contact users table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tContactUserModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableContactCustomer()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating contact customers table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<ContactCustomerModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableContactCustomerShipTo()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating contact customer shipto's table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tContactCustomerShipToModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableContactVendor()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating contact vendors table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tContactVendorModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableContactVendorSupplier()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating contact vendor suppliers table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tContactVendorSupplierModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableReasonCode()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating reason code table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tReasonCodeModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItemReasonCodeXRef()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating item reason code xref table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemReasonCodeXRefModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItemConvertUnit()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating item convert units table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemConvertUnitModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityDetailConvertUnit()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating detail convert units table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<ActivityDetailConvertUnitModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItemMoveInventory()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating item move inventory table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemMoveInventoryModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityDetailMoveInventory()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating detail move inventory table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityDetailMoveInventoryModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityAdjustment()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating adjustment table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<ActivityAdjustmentModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItemAdjustment()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating item adjustment table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemAdjustmentModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityDetailAdjustment()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating detail adjustment table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<ActivityDetailAdjustmentModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityProduction()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating production table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityProductionModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityProductionFinished()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating production finished table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityProductionFinishedModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityProductionMaterial()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating production material table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityProductionMaterialModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityProductionByProduct()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating production byproduct table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityProductionByProductModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItemProduction()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating item production table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemProductionModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItemProductionFinished()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating item production finished table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemProductionFinishedModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItemProductionMaterial()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating item production material table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemProductionMaterialModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityItemProductionByProduct()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating item production byproduct table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityItemProductionByProductModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityDetailProductionFinished()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating detail production finished table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityDetailProductionFinishedModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityDetailProductionMaterial()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating detail production material table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityDetailProductionMaterialModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableActivityDetailProductionByProduct()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating detail production byproduct table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tActivityDetailProductionByProductModel>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task CreateTableFamily()
        {
            if (this.OnNubStatOut != null)
            {
                this.MsgEventArgs.StatusOutMsg = "Creating families table.";
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            await DatabaseConnection.GetInstance().SQLConn.CreateTableAsync<tFamilyModel>();
        }
    }
}
