namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.IO;
  using dMobile.Bedrock;

  public class DatabaseNub : IDataNubPublisher
  {
    #region Fields

    private bool errorRaised = false;
    private DatabaseConnection dbConn;
    private DataNubEventArgs msgEventArgs = new DataNubEventArgs();

    #endregion

    #region Constructors

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public DatabaseNub()
    {
      this.DBConn = DatabaseConnection.GetInstance();
    }

    #endregion

    #region Delegates

    private delegate void OnNubStatOutEventHandler(
        IDataNubPublisher databaseNubEventPublisher,
        DataNubEventArgs databaseNubEventArgs);

    #endregion

    #region Events

    private event OnNubStatOutEventHandler OnNubStatOut;

    #endregion

    #region Properties

    public DataNubEventArgs MsgEventArgs
    {
      get
      {
        return this.msgEventArgs;
      }

      set
      {
        this.msgEventArgs = value;
      }
    }

    public DatabaseConnection DBConn
    {
      get
      {
        return this.dbConn;
      }

      set
      {
        this.dbConn = value;
      }
    }

    public bool ErrorRaised
    {
      get
      {
        return this.errorRaised;
      }

      set
      {
        this.errorRaised = value;
      }
    }

    #endregion

    #region Public Members

    /// <summary>
    /// Compact database
    /// -Reorganizes a table's pages to reside in adjacent database pages, which improves performance by reducing 
    /// table fragmentation
    /// -Reorders the rows of the table when there is a unique or primary key constraint present, which increases 
    /// search/insert performance
    /// -Reclaims used space created by object and record deletions
    /// -Resets incrementing IDENTITY columns so that the next values with be one more than the highest value in 
    /// the current table
    /// -Regenerates the table statistic, which may help improve the query optimization process
    /// -Attempts to repair errors in the database
    /// -Might modify database properties
    /// </summary>
    public void CompactDatabase()
    {
      //string srceDBName = this.DBConn.DBName;
      //string destDBName = this.DBConn.DBName + ".tmp";
      //string newPassword = "Password = " + this.DBConn.DBPassword;
      //SqlCeEngine engine = new SqlCeEngine("Data Source = " + srceDBName);

      //try
      //{
      //  this.ErrorRaised = false;
      //  engine.Compact((string)("Data Source = " + destDBName + ";" + srceDBName));
      //  engine.Dispose();
      //  File.Delete(srceDBName);
      //  File.Move(destDBName, srceDBName);
      //}
      //catch (SQLiteException ssce)
      //{
      //  this.ErrorRaised = true;

      //  throw ssce;
      //}
      //finally
      //{
      //  if (engine != null)
      //  {
      //    engine.Dispose();
      //  }
      //}
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public void RepairDatabase()
    {
      //string srceDBName = this.DBConn.DBName;
      //SqlCeEngine engine = new SqlCeEngine("Data Source = " + srceDBName);

      //try
      //{
      //  this.ErrorRaised = false;
      //  engine.Repair((string)("Data Source = " + srceDBName), RepairOption.RecoverAllPossibleRows);
      //  engine.Dispose();
      //}
      //catch (SQLiteException ssce)
      //{
      //  this.ErrorRaised = true;

      //  throw ssce;
      //}
      //finally
      //{
      //  if (engine != null)
      //  {
      //    engine.Dispose();
      //  }
      //}
    }

    /// <summary>
    /// Create a new sql server database called dMobile
    /// Delete the database if it already exists
    /// </summary>
    /// <remarks>Password is 'dMobile'</remarks>
    public void CreateNewDatabase()
    {
            string dbASCIIZ = this.DBConn.DBPath + this.DBConn.DBName;

            DatabaseConnection.CloseDataBaseConnection();
            DatabaseConnection.DisposeDataBaseConnection();

            if (File.Exists(dbASCIIZ))
            {
                if (this.OnNubStatOut != null)
                {
#if MULTILANGUAGE
          this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.DeletingDatabase) + dbASCIIZ;
#else
                    this.MsgEventArgs.StatusOutMsg = "Deleting database " + dbASCIIZ;
#endif
                    this.OnNubStatOut(this, this.MsgEventArgs);
                }

                File.Delete(dbASCIIZ);
            }

            if (this.OnNubStatOut != null)
            {
#if MULTILANGUAGE
          this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingDatabase) + dbASCIIZ;
#else
                this.MsgEventArgs.StatusOutMsg = "Creating database " + dbASCIIZ;
#endif
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            DatabaseConnection.GetInstance();
            DatabaseConnection.OpenDataBaseConnection();
        }

    /// <summary>
    ///
    /// </summary>
    /// <param name="strCmndDDLStr"></param>
    /// <remarks></remarks>
    public void RunDDLCommand(string cmndDDLStr)
    {
      SQLiteCommand cmndDLLCmd = null;
      try
      {
        cmndDLLCmd = new SQLiteCommand(cmndDDLStr, DatabaseConnection.GetInstance().SQLConn);
        cmndDLLCmd.CommandType = CommandType.Text;
        cmndDLLCmd.ExecuteNonQuery();
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
      finally
      {
        if (cmndDLLCmd != null)
        {
          cmndDLLCmd.Dispose();
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public void CreateTables()
    {
      try
      {
        this.CreateTableContact();
        this.CreateTableContactUser();

        this.CreateTableLocationMap();
        this.CreateTableLocationMapType();
        this.CreateTableLocationMapWarehouse();
        this.CreateTableLocationMapSection();
        this.CreateTableLocationMapSlotCoordinate();
        this.CreateTableLocationMapSlot();
        this.CreateTableWarehouseLocationMapType();

        this.CreateTableUnit();
        this.CreateTableRoute();
        this.CreateTableReasonCode();
        this.CreateTableFamily();

        this.CreateTableContactCustomer();
        this.CreateTableContactCustomerShipTo();
        this.CreateTableContactVendor();
        this.CreateTableContactVendorSupplier();

        this.CreateTableProductScheme();
        this.CreateTableProductSchemeVariant();
        this.CreateTableProductSchemeVariantXRef();
        this.CreateTableProductSchemeLotNumber();
        this.CreateTableProductSchemeLotNumberXRef();
        this.CreateTableProduct();
        this.CreateTableProductLocationMapType();
        this.CreateTableProductUnit();
        this.CreateTableProductUnitRelation();
        this.CreateTableProductUnitBarCode();

        this.CreateTableActivity();
        this.CreateTableActivityItem();
        this.CreateTableActivityItemReasonCodeXRef();
        this.CreateTableActivityItemBarCodeAssociationXRef();

        this.CreateTableActivityDetail();
        this.CreateTableActivityDetailExtra();
        this.CreateTableActivityDetailProductSchemeVariantXRef();

        this.CreateTableActivityCount();
        this.CreateTableActivityItemCount();
        this.CreateTableActivityDetailCount();

        this.CreateTableActivityPicking();
        this.CreateTableActivityItemPicking();
        this.CreateTableActivityItemPickingComment();
        this.CreateTableActivityDetailPicking();
        this.CreateTableActivityPickingRoute();

        this.CreateTableActivityReturns();
        this.CreateTableActivityItemReturns();
        this.CreateTableActivityItemReturnsComment();
        this.CreateTableActivityDetailReturns();

        this.CreateTableActivityReceiving();
        this.CreateTableActivityItemReceiving();
        this.CreateTableActivityItemReceivingComment();
        this.CreateTableActivityDetailReceiving();

        this.CreateTableActivityTransfer();
        this.CreateTableActivityItemTransfer();
        this.CreateTableActivityDetailTransfer();

        // this.CreateTableActivityConvertUnit()
        // this.CreateTableActivityItemConvertUnit()
        // this.CreateTableActivityDetailConvertUnit()

        // this.CreateTableActivityMoveInventory()
        // this.CreateTableActivityItemMoveInventory()
        //// this.CreateTableActivityDetailMoveInventory()

        this.CreateTableActivityAdjustment();
        this.CreateTableActivityItemAdjustment();
        this.CreateTableActivityDetailAdjustment();

        this.CreateTableActivityProduction();
        this.CreateTableActivityProductionFinished();
        this.CreateTableActivityProductionMaterial();
        this.CreateTableActivityProductionByProduct();
        this.CreateTableActivityItemProduction();
        this.CreateTableActivityItemProductionFinished();
        this.CreateTableActivityItemProductionMaterial();
        this.CreateTableActivityItemProductionByProduct();
        this.CreateTableActivityDetailProductionFinished();
        this.CreateTableActivityDetailProductionMaterial();
        this.CreateTableActivityDetailProductionByProduct();

        this.CreateTableActivityItemPurchaseOrderWorkSheet();
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

    #region IDatabaseNubEventPublisher Members

    public void RegNubListenerStatOut(IDataNubListener dataNubEventListener)
    {
      this.OnNubStatOut += new OnNubStatOutEventHandler(dataNubEventListener.OnNubStatOut);
    }

    public void UnregNubListenerStatOut(IDataNubListener dataNubEventListener)
    {
      this.OnNubStatOut -= new OnNubStatOutEventHandler(dataNubEventListener.OnNubStatOut);
    }

    #endregion

    #endregion

    #region Private Members

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivity()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivity"
          + "("
          + "ActivityRefno INTEGER NOT NULL CONSTRAINT PK_ActivityOnActivityRefno PRIMARY KEY,"
          + "ActivityStatus INTEGER NOT NULL,"
          + "ActivitySubType INTEGER NOT NULL"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingActivityTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating activity table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityOnActivityRefnoAndActivitySubType "
          + "ON tActivity(ActivityRefno ASC, ActivitySubType ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailProductSchemeVariantXRef()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailProductSchemeVariantXRef"
          + "("
          + "ActivityDetailRefno INTEGER NOT NULL,"
          + "ProductSchemeVariantRefno INTEGER NOT NULL, "
          + "CONSTRAINT ActivityDetailProductSchemeVariantXRefTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityDetailProductSchemeVariantXRefToProductSchemeVariantOnProductSchemeVariantRefno FOREIGN KEY (ProductSchemeVariantRefno) " 
          + "REFERENCES tProductSchemeVariant(ProductSchemeVariantRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingDetailProductSchemeVariantXrefTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating detail product scheme variant xref table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "PK_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefnoAndProductSchemeVariantRefno ON "
          + "tActivityDetailProductSchemeVariantXRef(ActivityDetailRefno ASC, ProductSchemeVariantRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefno ON "
          + "tActivityDetailProductSchemeVariantXRef(ActivityDetailRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityDetailProductSchemeVariantXRefOnProductSchemeVariantRefno "
          + "ON tActivityDetailProductSchemeVariantXRef(ProductSchemeVariantRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityDetailProductSchemeVariantXRef ADD CONSTRAINT "
      //    + "ActivityDetailProductSchemeVariantXRefTactDetailOnActivityDetailRefno  "
      //    + "FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityDetailProductSchemeVariantXRef ADD CONSTRAINT "
      //    + "ActivityDetailProductSchemeVariantXRefToProductSchemeVariantOnProductSchemeVariantRefno "
      //    + "FOREIGN KEY (ProductSchemeVariantRefno) REFERENCES tProductSchemeVariant(ProductSchemeVariantRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItem()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItem"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType INTEGER NOT NULL,"
          + "ActivityItemStatus INTEGER NOT NULL,"
          + "ActivityRefno INTEGER NOT NULL, "
          + "CONSTRAINT ActivityItemToActivityOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingActivityItemTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating activity item table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailOnActivityItemRefnoAndActivityItemSubType "
          + "ON tActivityItem(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemOnActivityRefno ON tActivityItem(ActivityRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityItem ADD CONSTRAINT ActivityItemToActivityOnActivityRefno "
      //    + "FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///  SN:12345678 BB:11/12/2010 BL:001.FREEZER.123456   1TT=100EA@300KG
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetail()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetail"
          + "("
          + "ActivityDetailRefno INTEGER NOT NULL "
          + "CONSTRAINT PK_ActivityDetailOnActivityDetailRefno PRIMARY KEY,"
          + "ActivityDetailSubType INTEGER NOT NULL,"
          + "ActivityDetailStatus INTEGER NOT NULL,"
          + "ActivityItemRefno INTEGER NOT NULL,"
          + "LocationMapRefno INTEGER NOT NULL,"
          + "ProductUnitRefno INTEGER NOT NULL,"
          + "ActivityDetailUniqueSchemeID nvarchar(80),"
          + "ActivityDetailSchemeID nvarchar(80),"
          + "ActivityDetailQuantity numeric(11, 3) NOT NULL,"
          + "ActivityDetailBaseQuantity numeric(11, 3) NOT NULL,"
          + "ActivityDetailWeightQuantity numeric(11, 3) NOT NULL,"
          + "ActivityDetailSingleTradeItemSerialNumber nvarchar(20) NOT NULL,"
          + "ActivityDetailLogisticUnitBatchLotNumber nvarchar(20) NOT NULL,"
          + "ActivityDetailLogisticUnitBestBeforeSellByDate datetime NOT NULL,"
          + "ActivityDetailLogisticUnitExpiryUseByDate datetime NOT NULL,"
          + "ActivityDetailLogisticUnitProductionDate datetime NOT NULL,"
          + "ActivityDetailLogisticUnitDueDate datetime NOT NULL,"
          + "ActivityDetailLogisticUnitPackagingDate datetime NOT NULL,"
          + "ActivityDetailLogisticUnitSkidNumber nvarchar(18) NOT NULL, "
          + "CONSTRAINT ActivityDetailTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityDetailTactItemOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityDetailTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingActivityDetailTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating activity detail table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailOnActivityDetailRefnoAndActivityDetailSubType "
          + "ON tActivityDetail(ActivityDetailRefno ASC, ActivityDetailSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS ActivityDetailOnActivityDetailUniqueSchemeID ON "
          + "tActivityDetail(ActivityDetailUniqueSchemeID ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityDetailOnActivityItemRefno ON "
          + "tActivityDetail(ActivityItemRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityDetailOnLocationMapRefno ON "
          + "tActivityDetail(LocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityDetailOnProductUnitRefno ON "
          + "tActivityDetail(ProductUnitRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityDetail ADD CONSTRAINT ActivityDetailTactItemOnActivityItemRefno "
      //    + "FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityDetail ADD CONSTRAINT ActivityDetailTactItemOnLocationMapRefno "
      //    + "FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityDetail ADD CONSTRAINT ActivityDetailTactItemOnProductUnitRefno "
      //    + "FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailExtra()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailExtra"
          + "("
          + "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailExtraOnActivityDetailRefno PRIMARY KEY,"
          + "ActivityDetailSubType INTEGER NOT NULL,"
          + "ActivityDetailExtra1Text nvarchar(10) NOT NULL,"
          + "ActivityDetailExtra2Text nvarchar(10) NOT NULL,"
          + "ActivityDetailExtra3Text nvarchar(10) NOT NULL,"
          + "ActivityDetailExtra4Text nvarchar(10) NOT NULL,"
          + "ActivityDetailExtra5Text nvarchar(10) NOT NULL, "
          + "CONSTRAINT ActivityDetailExtraToActivityDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingDetailExtraTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating detail extra table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailExtraOnActivityDetailRefnoAndActivityDetailSubType "
          + "ON tActivityDetailExtra(ActivityDetailRefno ASC, ActivityDetailSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityDetailExtra ADD CONSTRAINT "
      //    + "ActivityDetailExtraToActivityDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) "
      //    + "REFERENCES tActivityDetail(ActivityDetailRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityCount()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityCount"
          + "("
          + "ActivityRefno INTEGER NOT NULL CONSTRAINT PK_ActivityCountOnActivityRefno PRIMARY KEY,"
          + "ActivitySubType INTEGER NOT NULL,"
          + "ActivityCountFKRefno INTEGER NOT NULL,"
          + "ActivityCountStringFKRefno nvarchar(8) NOT NULL,"
          + "WarehouseLocationMapRefno INTEGER NOT NULL,"
          + "ActivityCountType INTEGER NOT NULL,"
          + "ActivityCountDateTime datetime NOT NULL,"
          + "ActivityCountTypeDescription nvarchar(60) NOT NULL,"
          + "ActivityCountDescription nvarchar(60) NOT NULL, "
          + "CONSTRAINT ActivityCountToActivityOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityCountToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno)"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingCountTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating count table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityCountOnActivityRefnoAndActivitySubType ON "
          + "tActivityCount(ActivityRefno ASC, ActivitySubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityCountOnActivityCountFKRefno ON "
          + "tActivityCount(ActivityCountFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityCountOnWarehouseLocationMapRefno ON "
          + "tActivityCount(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityCount ADD CONSTRAINT ActivityCountToActivityOnActivityRefno "
      //    + "FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityCount ADD CONSTRAINT "
      //    + "ActivityCountToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno "
      //    + "FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno)";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityPicking()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityPicking"
          + "("
          + "ActivityRefno INTEGER NOT NULL CONSTRAINT PK_ActivityPickingOnActivityRefno PRIMARY KEY,"
          + "ActivitySubType INTEGER NOT NULL,"
          + "ActivityPickingFKRefno INTEGER NOT NULL,"
          + "ActivityPickingStringFKRefno nvarchar(8) NOT NULL,"
          + "WarehouseLocationMapRefno INTEGER NOT NULL,"
          + "ActivityPickingOrderedDateTime datetime NOT NULL,"
          + "ActivityPickingDeliveryDateTime datetime NOT NULL,"
          + "ActivityPickingCustomerContactName nvarchar(40) NOT NULL,"
          + "ActivityPickingCustomerShipToInfo nvarchar(40) NOT NULL,"
          + "ActivityPickingIsReviewed bit NOT NULL, "
          + "CONSTRAINT ActivityPickingToActivityOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityPickingToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingPickingTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating picking table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityPickingOnActivityRefnoAndActivitySubType "
          + "ON tActivityPicking(ActivityRefno ASC, ActivitySubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityPickingOnActivityPickingFKRefno ON "
          + "tActivityPicking(ActivityPickingFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityPickingOnWarehouseLocationMapRefno ON "
          + "tActivityPicking(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityPicking ADD CONSTRAINT ActivityPickingToActivityOnActivityRefno "
      //    + "FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityPicking ADD CONSTRAINT "
      //    + "ActivityPickingToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno "
      //    + "FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityReturns()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityReturns"
          + "("
          + "ActivityRefno INTEGER NOT NULL CONSTRAINT PK_ActivityReturnsOnActivityRefno PRIMARY KEY,"
          + "ActivitySubType INTEGER NOT NULL,"
          + "ActivityReturnsFKRefno INTEGER NOT NULL,"
          + "ActivityReturnsStringFKRefno nvarchar(8) NOT NULL,"
          + "WarehouseLocationMapRefno INTEGER NOT NULL,"
          + "ActivityReturnsOrderedDateTime datetime NOT NULL,"
          + "ActivityReturnsDeliveryDateTime datetime NOT NULL,"
          + "ActivityReturnsCustomerContactName nvarchar(40) NOT NULL,"
          + "ActivityReturnsCustomerShipToInfo nvarchar(40) NOT NULL, "
          + "CONSTRAINT ActivityReturnsToActivityOnActivityRefnoAndActivitySubType FOREIGN KEY (ActivityRefno, ActivitySubType) REFERENCES tActivity(ActivityRefno, ActivitySubType) ON DELETE CASCADE, "
          + "CONSTRAINT ActivityReturnsToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE NO ACTION"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingReturnsTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating returns table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityReturnsOnActivityRefnoAndActivitySubType "
          + "ON tActivityReturns(ActivityRefno ASC, ActivitySubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityReturnsOnActivityReturnsFKRefno ON "
          + "tActivityReturns(ActivityReturnsFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityReturnsOnWarehouseLocationMapRefno ON "
          + "tActivityReturns(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityReturns ADD CONSTRAINT "
      //    + "ActivityReturnsToActivityOnActivityRefnoAndActivitySubType FOREIGN KEY (ActivityRefno, ActivitySubType) "
      //    + "REFERENCES tActivity(ActivityRefno, ActivitySubType) ON DELETE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityReturns ADD CONSTRAINT "
      //    + "ActivityReturnsToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno "
      //    + "FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
      //    + "ON DELETE NO ACTION";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityPickingRoute()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityPickingRoute"
          + "("
          + "PickingActivityRefno INTEGER NOT NULL,"
          + "RouteRefno INTEGER NOT NULL,"
          + "RouteStop INTEGER NOT NULL, "
          + "CONSTRAINT ActivityPickingRouteToActivityPickingOnPickingActivityRefnoAndActivityRefno FOREIGN KEY (PickingActivityRefno) REFERENCES tActivityPicking(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityPickingRouteToRouteOnRouteRefno FOREIGN KEY (RouteRefno) REFERENCES tRoute(RouteRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingPickingRouteTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating picking route table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityPickingRouteOnPickingActivityRefnoAndRouteRefno "
          + "ON tActivityPickingRoute(PickingActivityRefno ASC, RouteRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS FK_ActivityPickingRouteOnPickingActivityRefno ON "
          + "tActivityPickingRoute(PickingActivityRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityPickingRouteOnRouteRefno ON tActivityPickingRoute(RouteRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityPickingRoute ADD CONSTRAINT "
      //    + "ActivityPickingRouteToActivityPickingOnPickingActivityRefnoAndActivityRefno "
      //    + "FOREIGN KEY (PickingActivityRefno) REFERENCES tActivityPicking(ActivityRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityPickingRoute ADD CONSTRAINT ActivityPickingRouteToRouteOnRouteRefno "
      //    + "FOREIGN KEY (RouteRefno) REFERENCES tRoute(RouteRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityReceiving()
    {
      // TODO: tActivityReceiving.ActivityReceivingVendorSupplierGTINCompanyPrefix is not being used as of Apr 4/2014
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityReceiving"
          + "("
          + "ActivityRefno INTEGER NOT NULL CONSTRAINT PK_ActivityReceivingOnActivityRefno PRIMARY KEY,"
          + "ActivitySubType INTEGER NOT NULL,"
          + "ActivityReceivingFKRefno INTEGER NOT NULL,"
          + "ActivityReceivingStringFKRefno nvarchar(8) NOT NULL,"
          + "ActivityReceivingPONumber nvarchar(8) NOT NULL,"
          + "WarehouseLocationMapRefno INTEGER NOT NULL,"
          + "ActivityReceivingIssuedDateTime datetime NOT NULL,"
          + "ActivityReceivingArrivalDateTime datetime NOT NULL,"
          + "ActivityReceivingVendorContactName nvarchar(40) NOT NULL,"
          + "ActivityReceivingVendorSupplierContactName nvarchar(40) NOT NULL,"
          + "ActivityReceivingVendorSupplierGTINCompanyPrefix nvarchar(11) NOT NULL, "
          + "CONSTRAINT ActivityReceivingToActivityOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityReceivingToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingReceivingTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating receiving table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityReceivingOnActivityRefnoAndActivitySubType "
          + "ON tActivityReceiving(ActivityRefno ASC, ActivitySubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityReceivingOnActivityReceivingFKRefno ON "
          + "tActivityReceiving(ActivityReceivingFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityReceivingOnActivityReceivingPONumber ON "
          + "tActivityReceiving(ActivityReceivingPONumber ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityReceivingOnWarehouseLocationMapRefno ON "
          + "tActivityReceiving(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityReceiving ADD CONSTRAINT ActivityReceivingToActivityOnActivityRefno "
      //    + "FOREIGN KEY (ActivityRefno)" + " REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityReceiving ADD CONSTRAINT "
      //    + "ActivityReceivingToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno "
      //    + "FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityTransfer()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityTransfer"
          + "("
          + "ActivityRefno INTEGER NOT NULL CONSTRAINT PK_ActivityTransferOnActivityRefno PRIMARY KEY,"
          + "ActivitySubType INTEGER NOT NULL,"
          + "ActivityTransferFKRefno INTEGER NOT NULL,"
          + "ActivityTransferStringFKRefno nvarchar(8) NOT NULL,"
          + "TransferToWarehouseLocationMapRefno INTEGER NOT NULL,"
          + "ActivityTransferDescription nvarchar(30) NOT NULL,"
          + "ActivityTransferDateTime datetime NOT NULL,"
          + "ActivityTransferArrivalDateTime datetime NOT NULL, "
          + "CONSTRAINT ActivityTransferToActivityOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityTransferToLocationMapWarehouseOnTransferToWarehouseLocationMapRefnoAndLocationMapRefno FOREIGN KEY (TransferToWarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingTransferTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating transfer table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityTransferOnActivityRefnoAndActivitySubType ON "
          + "tActivityTransfer(ActivityRefno ASC, ActivitySubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityTransferOnActivityTransferFKRefno ON "
          + "tActivityTransfer(ActivityTransferFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityTransferOnTransferToWarehouseLocationMapRefno ON "
          + "tActivityTransfer(TransferToWarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityTransfer ADD CONSTRAINT ActivityTransferToActivityOnActivityRefno "
      //    + "FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityTransfer ADD CONSTRAINT "
      //    + "ActivityTransferToLocationMapWarehouseOnTransferToWarehouseLocationMapRefnoAndLocationMapRefno "
      //    + "FOREIGN KEY (TransferToWarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemCount()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemCount"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemCountOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType INTEGER NOT NULL,"
          + "ActivityItemCountFKRefno INTEGER NOT NULL,"
          + "ActivityItemCountStringFKRefno nvarchar(8) NOT NULL,"
          + "ProductUnitRefno INTEGER NOT NULL,"
          + "LocationMapRefno INTEGER NOT NULL,"
          + "ActivityItemCountQuantityOnHand numeric(11, 3) NOT NULL,"
          + "ActivityItemCountBaseQuantityOnHand numeric(11, 3) NOT NULL,"
          + "ActivityItemCountWeightQuantityOnHand numeric(11, 3) NOT NULL,"
          + "ActivityItemCountQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemCountBaseQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemCountWeightQuantity numeric(11, 3) NOT NULL, "
          + "CONSTRAINT ActivityItemCountTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityItemCountToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityItemCountTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingCountItemTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating count item table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemCountOnActivityItemRefnoAndActivityItemSubType "
          + "ON tActivityItemCount(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemCountOnActivityItemCountFKRefno ON "
          + "tActivityItemCount(ActivityItemCountFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //// Note: Cannot enforce uniqueness on LocationMapRefno and ProductUnitRefno without ActivityRefno in the table.

      //string alterTable = "ALTER TABLE tActivityItemCount ADD CONSTRAINT ActivityItemCountTactItemOnActivityItemRefno "
      //    + "FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityItemCount ADD CONSTRAINT ActivityItemCountToLocationMapOnLocationMapRefno "
      //    + "FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityItemCount ADD CONSTRAINT ActivityItemCountTactItemOnProductUnitRefno "
      //    + "FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemPicking()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemPicking"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemPickingOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType INTEGER NOT NULL,"
          + "ActivityItemPickingFKRefno INTEGER NOT NULL,"
          + "ActivityItemPickingStringFKRefno nvarchar(8) NOT NULL,"
          + "ProductUnitRefno INTEGER NOT NULL,"
          + "WarehouseLocationMapRefno INTEGER NOT NULL,"
          + "ActivityItemPickingQuantityOrdered numeric(11, 3) NOT NULL,"
          + "ActivityItemPickingBaseQuantityOrdered numeric(11, 3) NOT NULL,"
          + "ActivityItemPickingWeightQuantityOrdered numeric(11, 3) NOT NULL,"
          + "ActivityItemPickingQuantityShipped numeric(11, 3) NOT NULL,"
          + "ActivityItemPickingBaseQuantityShipped numeric(11, 3) NOT NULL,"
          + "ActivityItemPickingWeightQuantityShipped numeric(11, 3) NOT NULL,"
          + "ActivityItemPickingCustomerProductUnitCode nvarchar(10) NOT NULL,"
          + "ActivityItemPickingDescription nvarchar(40) NOT NULL,"
          + "ActivityItemPickingQuantityAvailable numeric(11, 3) NOT NULL,"
          + "ActivityItemPickingBaseQuantityAvailable numeric(11, 3) NOT NULL,"
          + "ActivityItemPickingWeightQuantityAvailable numeric(11, 3) NOT NULL,"
          + "ActivityItemPickingIsReviewed bit NOT NULL, "
          + "CONSTRAINT ActivityItemPickingTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityItemPickingToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityItemPickingTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingPickingItemTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating picking item table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemPickingOnActivityItemRefnoAndActivityItemSubType"
          + " ON tActivityItemPicking(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemPickingOnActivityItemPickingFKRefno ON "
          + "tActivityItemPicking(ActivityItemPickingFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemPickingOnWarehouseLocationMapRefno ON "
          + "tActivityItemPicking(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityItemPicking ADD CONSTRAINT "
      //    + "ActivityItemPickingTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
      //    + "tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityItemPicking ADD CONSTRAINT "
      //    + "ActivityItemPickingToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY "
      //    + "(WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityItemPicking ADD CONSTRAINT ActivityItemPickingTactItemOnProductUnitRefno "
      //    + "FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemReturns()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemReturns"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemReturnsOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType INTEGER NOT NULL,"
          + "ActivityItemReturnsFKRefno INTEGER NOT NULL,"
          + "ActivityItemReturnsStringFKRefno nvarchar(8) NOT NULL,"
          + "ProductUnitRefno INTEGER NOT NULL,"
          + "WarehouseLocationMapRefno INTEGER NOT NULL,"
          + "ActivityItemReturnsQuantityReturned numeric(11, 3) NOT NULL,"
          + "ActivityItemReturnsBaseQuantityReturned numeric(11, 3) NOT NULL,"
          + "ActivityItemReturnsWeightQuantityReturned numeric(11, 3) NOT NULL,"
          + "ActivityItemReturnsCustomerProductUnitCode nvarchar(10) NOT NULL,"
          + "ActivityItemReturnsDescription nvarchar(40) NOT NULL, "
          + "CONSTRAINT ActivityItemReturnsTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityItemReturnsToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityItemReturnsTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingReturnsItemTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating returns item table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemReturnsOnActivityItemRefnoAndActivityItemSubType"
          + " ON tActivityItemReturns(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemReturnsOnActivityItemReturnsFKRefno ON "
          + "tActivityItemReturns(ActivityItemReturnsFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemReturnsOnWarehouseLocationMapRefno ON "
          + "tActivityItemReturns(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityItemReturns ADD CONSTRAINT "
      //    + "ActivityItemReturnsTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
      //    + "tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityItemReturns ADD CONSTRAINT "
      //    + "ActivityItemReturnsToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY "
      //    + "(WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityItemReturns ADD CONSTRAINT ActivityItemReturnsTactItemOnProductUnitRefno "
      //    + "FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemPickingComment()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemPickingComment"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemPickingCommentOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemPickingCommentComment nvarchar(255) NOT NULL, "
          + "CONSTRAINT ActivityItemPickingCommentTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItemPicking(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingPickingCommentItemTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating picking comment item table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      //string alterTable = "ALTER TABLE tActivityItemPickingComment ADD CONSTRAINT "
      //    + "ActivityItemPickingCommentTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
      //    + "tActivityItemPicking(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemReturnsComment()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemReturnsComment"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemReturnsCommentOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemReturnsCommentComment nvarchar(255) NOT NULL, "
          + "CONSTRAINT ActivityItemReturnsCommentTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItemReturns(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingReturnsCommentItemTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating returns comment item table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      //string alterTable = "ALTER TABLE tActivityItemReturnsComment ADD CONSTRAINT "
      //    + "ActivityItemReturnsCommentTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
      //    + "tActivityItemReturns(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemReceiving()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemReceiving"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemReceivingOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType INTEGER NOT NULL,"
          + "ActivityItemReceivingFKRefno INTEGER NOT NULL,"
          + "ActivityItemReceivingStringFKRefno nvarchar(8) NOT NULL,"
          + "ProductUnitRefno INTEGER NOT NULL,"
          + "WarehouseLocationMapRefno INTEGER NOT NULL,"
          + "ActivityItemReceivingQuantityOrdered numeric(11, 3) NOT NULL,"
          + "ActivityItemReceivingBaseQuantityOrdered numeric(11, 3) NOT NULL,"
          + "ActivityItemReceivingWeightQuantityOrdered numeric(11, 3) NOT NULL,"
          + "ActivityItemReceivingQuantityReceived numeric(11, 3) NOT NULL,"
          + "ActivityItemReceivingBaseQuantityReceived numeric(11, 3) NOT NULL,"
          + "ActivityItemReceivingWeightQuantityReceived numeric(11, 3) NOT NULL,"
          + "ActivityItemReceivingVendorProductUnitCode nvarchar(10) NOT NULL,"
          + "ActivityItemReceivingDescription nvarchar(40) NOT NULL,"
          + "ActivityItemReceivingQuantityReceivedToDate numeric(11, 3) NOT NULL,"
          + "ActivityItemReceivingBaseQuantityReceivedToDate numeric(11, 3) NOT NULL,"
          + "ActivityItemReceivingWeightQuantityReceivedToDate numeric(11, 3) NOT NULL, "
          + "CONSTRAINT ActivityItemReceivingTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityItemReceivingToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityItemReceivingTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingReceivingItemTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating receiving item table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemReceivingOnActivityItemRefnoAndActivityItemSubType"
          + " ON tActivityItemReceiving(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemReceivingOnActivityItemReceivingFKRefno ON "
          + "tActivityItemReceiving(ActivityItemReceivingFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemReceivingOnWarehouseLocationMapRefno ON "
          + "tActivityItemReceiving(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityItemReceiving ADD CONSTRAINT "
      //    + "ActivityItemReceivingTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
      //    + "tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityItemReceiving ADD CONSTRAINT "
      //    + "ActivityItemReceivingToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY "
      //    + "(WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityItemReceiving ADD CONSTRAINT ActivityItemReceivingTactItemOnProductUnitRefno "
      //    + "FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
   }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemPurchaseOrderWorkSheet()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemPurchaseOrderWorkSheet"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemPurchaseOrderWorkSheetOnActivityItemRefno "
          + "PRIMARY KEY,"
          + "ActivityItemSubType INTEGER NOT NULL,"
          + "ActivityItemPurchaseOrderWorkSheetFKRefno INTEGER NOT NULL,"
          + "ActivityItemPurchaseOrderWorkSheetStringFKRefno nvarchar(8) NOT NULL,"
          + "ProductUnitRefno INTEGER NOT NULL,"
          + "WarehouseLocationMapRefno INTEGER NOT NULL,"
          + "ActivityItemPurchaseOrderWorkSheetQuantityOrdered numeric(11, 3) NOT NULL,"
          + "ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered numeric(11, 3) NOT NULL,"
          + "ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered numeric(11, 3) NOT NULL, "
          + "CONSTRAINT ActivityItemPurchaseOrderWorkSheetTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityItemPurchaseOrderWorkSheetToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityItemPurchaseOrderWorkSheetTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingPurchaseOrderWorkSheetItemTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating purchase order work sheet item table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "UQ_ActivityItemPurchaseOrderWorkSheetOnActivityItemRefnoAndActivityItemSubType ON "
          + "tActivityItemPurchaseOrderWorkSheet(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "UQ_ActivityItemPurchaseOrderWorkSheetOnActivityItemPurchaseOrderWorkSheetFKRefno ON "
          + "tActivityItemPurchaseOrderWorkSheet(ActivityItemPurchaseOrderWorkSheetFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemPurchaseOrderWorkSheetOnWarehouseLocationMapRefno ON "
          + "tActivityItemPurchaseOrderWorkSheet(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityItemPurchaseOrderWorkSheet ADD CONSTRAINT "
      //    + "ActivityItemPurchaseOrderWorkSheetTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) "
      //    + "REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityItemPurchaseOrderWorkSheet ADD CONSTRAINT "
      //    + "ActivityItemPurchaseOrderWorkSheetToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY "
      //    + "(WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityItemPurchaseOrderWorkSheet ADD CONSTRAINT "
      //    + "ActivityItemPurchaseOrderWorkSheetTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) "
      //    + "REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemReceivingComment()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemReceivingComment"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemReceivingCommentOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemReceivingCommentComment nvarchar(255) NOT NULL, "
          + "CONSTRAINT ActivityItemReceivingCommentTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItemReceiving(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingReceivingCommentItemTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating receiving comment item table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      //string alterTable = "ALTER TABLE tActivityItemReceivingComment ADD CONSTRAINT "
      //    + "ActivityItemReceivingCommentTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
      //    + "tActivityItemReceiving(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemTransfer()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemTransfer"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemTransferOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType INTEGER NOT NULL,"
          + "ActivityItemTransferFKRefno INTEGER NOT NULL,"
          + "ActivityItemTransferStringFKRefno nvarchar(8) NOT NULL,"
          + "ProductUnitRefno INTEGER NOT NULL,"
          + "TransferFromWarehouseLocationMapRefno INTEGER NOT NULL,"
          + "ActivityItemTransferRequiredQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemTransferQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemTransferBaseQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemTransferWeightQuantity numeric(11, 3) NOT NULL, "
          + "CONSTRAINT ActivityItemTransferTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityItemTransferToLocationMapWarehouseOnTransferFromWarehouseLocationMapRefnoAndLocationMapRefno FOREIGN KEY (TransferFromWarehouseLocationMapRefno) "
          + "REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityItemTransferTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingTransferItemTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating transfer item table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemTransferOnActivityItemRefnoAndActivityItemSubType"
          + " ON tActivityItemTransfer(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemTransferOnActivityItemTransferFKRefno ON "
          + "tActivityItemTransfer(ActivityItemTransferFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemTransferOnTransferFromWarehouseLocationMapRefno ON "
          + "tActivityItemTransfer(TransferFromWarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityItemTransfer ADD CONSTRAINT "
      //    + "ActivityItemTransferTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
      //    + "tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityItemTransfer ADD CONSTRAINT "
      //    + "ActivityItemTransferToLocationMapWarehouseOnTransferFromWarehouseLocationMapRefnoAndLocationMapRefno "
      //    + "FOREIGN KEY (TransferFromWarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityItemTransfer ADD CONSTRAINT ActivityItemTransferTactItemOnProductUnitRefno "
      //    + "FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemBarCodeAssociationXRef()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemBarCodeAssociationXRef"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL,"
          + "ProductUnitBarCodeRefno INTEGER NOT NULL, "
          + "CONSTRAINT ActivityItemBarCodeAssociationXRefTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityItemBarCodeAssociationXRefToProductUnitBarCodeOnProductUnitBarCodeRefno FOREIGN KEY (ProductUnitBarCodeRefno) REFERENCES tProductUnitBarCode(ProductUnitBarCodeRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingBarCodeAssociationItemTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating bar code association item table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "PK_ActivityItemBarCodeAssociationXRefOnActivityItemRefnoAndProductUnitBarCodeRefno ON "
          + "tActivityItemBarCodeAssociationXRef(ActivityItemRefno ASC, ProductUnitBarCodeRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemBarCodeAssociationXRefOnActivityItemRefno ON "
          + "tActivityItemBarCodeAssociationXRef(ActivityItemRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemBarCodeAssociationXRefOnProductUnitBarCodeRefno ON "
          + "tActivityItemBarCodeAssociationXRef(ProductUnitBarCodeRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityItemBarCodeAssociationXRef ADD CONSTRAINT "
      //    + "ActivityItemBarCodeAssociationXRefTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) "
      //    + "REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityItemBarCodeAssociationXRef ADD CONSTRAINT "
      //    + "ActivityItemBarCodeAssociationXRefToProductUnitBarCodeOnProductUnitBarCodeRefno FOREIGN KEY "
      //    + "(ProductUnitBarCodeRefno) REFERENCES tProductUnitBarCode(ProductUnitBarCodeRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailCount()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailCount"
          + "("
          + "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailCountOnActivityDetailRefno PRIMARY KEY,"
          + "ActivityDetailCountFKRefno INTEGER NOT NULL, "
          + "CONSTRAINT ActivityDetailCountTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingCountDetailTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating count detail table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailCountOnActivityDetailCountFKRefno ON "
          + "tActivityDetailCount(ActivityDetailCountFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityDetailCount ADD CONSTRAINT "
      //    + "ActivityDetailCountTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES "
      //    + "tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailPicking()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailPicking"
          + "("
          + "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailPickingOnActivityDetailRefno PRIMARY KEY,"
          + "ActivityDetailPickingFKRefno INTEGER NOT NULL,"
          + "ActivityDetailPickingIsConfirmed bit NOT NULL, "
          + "CONSTRAINT ActivityDetailPickingTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingPickingDetailTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating picking detail table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailPickingOnActivityDetailPickingFKRefno ON "
          + "tActivityDetailPicking(ActivityDetailPickingFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityDetailPicking ADD CONSTRAINT "
      //    + "ActivityDetailPickingTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES "
      //    + "tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailReturns()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailReturns" 
                + "(" 
                + "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailReturnsOnActivityDetailRefno PRIMARY KEY," 
                + "ActivityDetailReturnsFKRefno INTEGER NOT NULL," 
                + "ActivityDetailReturnsIsConfirmed bit NOT NULL, " 
                + "CONSTRAINT ActivityDetailReturnsTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"
                + ")";
      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingReturnsDetailTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating returns detail table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailReturnsOnActivityDetailReturnsFKRefno ON "
          + "tActivityDetailReturns(ActivityDetailReturnsFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityDetailReturns ADD CONSTRAINT "
      //    + "ActivityDetailReturnsTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES "
      //    + "tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailReceiving()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailReceiving"
          + "("
          + "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailReceivingOnActivityDetailRefno PRIMARY KEY,"
          + "ActivityDetailReceivingFKRefno INTEGER NOT NULL, "
          + "CONSTRAINT ActivityDetailReceivingTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingReceivingDetailTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating receiving detail table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailReceivingOnActivityDetailReceivingFKRefno ON "
          + "tActivityDetailReceiving(ActivityDetailReceivingFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityDetailReceiving ADD CONSTRAINT "
      //    + "ActivityDetailReceivingTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES "
      //    + "tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailTransfer()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailTransfer"
          + "("
          + "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailTransferOnActivityDetailRefno PRIMARY KEY,"
          + "ActivityDetailTransferFKRefno INTEGER NOT NULL,"
          + "ActivityDetailTransferIsConfirmed bit NOT NULL, "
          + "CONSTRAINT ActivityDetailTransferTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingTransferDetailTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating transfer detail table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailTransferOnActivityDetailTransferFKRefno ON "
          + "tActivityDetailTransfer(ActivityDetailTransferFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityDetailTransfer ADD CONSTRAINT "
      //    + "ActivityDetailTransferTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES "
      //    + "tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableUnit()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tUnit"
          + "("
          + "UnitRefno INTEGER NOT NULL CONSTRAINT PK_UnitOnUnitRefno PRIMARY KEY,"
          + "UnitCode nvarchar(2) NOT NULL"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingUnitOfMeasureTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating unit of measure table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_UnitOnUnitCode ON tUnit(UnitCode ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableRoute()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tRoute"
          + "("
          + "RouteRefno INTEGER NOT NULL CONSTRAINT PK_RouteOnRouteRefno PRIMARY KEY,"
          + "RouteCode nvarchar(5) NOT NULL,"
          + "RouteDescription nvarchar(35) NOT NULL,"
          + "RouteDayOfWeek nvarchar(3) NOT NULL"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingRoutesTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating routes table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_RouteOnRouteCode ON tRoute(RouteCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS RouteOnRouteDescription ON tRoute(RouteDescription ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableLocationMap()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tLocationMap"
          + "("
          + "LocationMapRefno INTEGER NOT NULL CONSTRAINT PK_LocationMapOnLocationMapRefno PRIMARY KEY,"
          + "LocationMapSubType INTEGER NOT NULL"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingLocationMapTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating location map table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapOnLocationMapRefnoAndLocationMapSubType ON "
          + "tLocationMap(LocationMapRefno ASC, LocationMapSubType ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableLocationMapWarehouse()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tLocationMapWarehouse"
          + "("
          + "LocationMapRefno INTEGER NOT NULL CONSTRAINT PK_LocationMapWarehouseOnLocationMapRefno PRIMARY KEY,"
          + "LocationMapSubType INTEGER NOT NULL,"
          + "LocationMapWarehouseName nvarchar(22) NOT NULL,"
          + "LocationMapWarehouseCode INTEGER NOT NULL,"
          + "LocationMapWarehouseFKRefno INTEGER NOT NULL,"
          + "LocationMapWarehouseBarCode nvarchar(13) NOT NULL,"
          + "CONSTRAINT LocationMapWarehouseToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES "
          + "tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE" 
          + ")";

            if (this.OnNubStatOut != null)
            {
#if MULTILANGUAGE
                    this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingLocationMapWarehouseTable);
#else
                this.MsgEventArgs.StatusOutMsg = "Creating location map warehouse table.";
#endif
                this.OnNubStatOut(this, this.MsgEventArgs);
            }

            this.RunDDLCommand(sqlTable);

            //string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapWarehouseOnLocationMapRefnoAndLocationMapSubType "
            //    + "ON tLocationMapWarehouse(LocationMapRefno ASC, LocationMapSubType ASC)";
            //this.RunDDLCommand(sqlNdx);

            //sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno ON "
            //    + "tLocationMapWarehouse(LocationMapWarehouseFKRefno ASC)";
            //this.RunDDLCommand(sqlNdx);

            //sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapWarehouseOnLocationMapWarehouseName ON "
            //    + "tLocationMapWarehouse(LocationMapWarehouseName ASC)";
            //this.RunDDLCommand(sqlNdx);

            //sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapWarehouseOnLocationMapWarehouseCode ON "
            //    + "tLocationMapWarehouse(LocationMapWarehouseCode ASC)";
            //this.RunDDLCommand(sqlNdx);

            //sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapWarehouseOnLocationMapWarehouseBarCode ON "
            //    + "tLocationMapWarehouse(LocationMapWarehouseBarCode ASC)";
            //this.RunDDLCommand(sqlNdx);

            //string alterTable = "ALTER TABLE tLocationMapWarehouse ADD CONSTRAINT "
            //    + "LocationMapWarehouseToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES "
            //    + "tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE";
            //this.RunDDLCommand(alterTable);


            string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapWarehouseOnLocationMapRefnoAndLocationMapSubType "
                + "ON tLocationMapWarehouse(LocationMapRefno ASC, LocationMapSubType ASC)";
            this.RunDDLCommand(sqlNdx);

            sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno ON "
                + "tLocationMapWarehouse(LocationMapWarehouseFKRefno ASC)";
            this.RunDDLCommand(sqlNdx);

            sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapWarehouseOnLocationMapWarehouseName ON "
                + "tLocationMapWarehouse(LocationMapWarehouseName ASC)";
            this.RunDDLCommand(sqlNdx);

            sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapWarehouseOnLocationMapWarehouseCode ON "
                + "tLocationMapWarehouse(LocationMapWarehouseCode ASC)";
            this.RunDDLCommand(sqlNdx);

            sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapWarehouseOnLocationMapWarehouseBarCode ON "
                + "tLocationMapWarehouse(LocationMapWarehouseBarCode ASC)";
            this.RunDDLCommand(sqlNdx);

            //string alterTable = "ALTER TABLE tLocationMapWarehouse ADD CONSTRAINT "
            //    + "LocationMapWarehouseToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES "
            //    + "tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE";
            //this.RunDDLCommand(alterTable);
        }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableLocationMapSection()
    {
        string sqlTable = "CREATE TABLE IF NOT EXISTS tLocationMapSection"
            + "("
            + "LocationMapRefno INTEGER NOT NULL CONSTRAINT PK_LocationMapSectionOnLocationMapRefno PRIMARY KEY,"
            + "LocationMapSubType INTEGER NOT NULL,"
            + "WarehouseLocationMapRefno INTEGER NOT NULL,"
            + "LocationMapSectionCode INTEGER NOT NULL,"
            + "LocationMapSectionName nvarchar(10),"
            + "LocationMapSectionFKRefno INTEGER NOT NULL,"
            + "LocationMapSectionBarCode nvarchar(13),"
            + "CONSTRAINT LocationMapSectionToLocationMapOnLocationMapRefno "
            + "FOREIGN KEY (LocationMapRefno) "
            + "REFERENCES tLocationMap(LocationMapRefno) "
            + "ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingLocationMapSectionTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating location map section table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE INDEX IF NOT EXISTS FK_LocationMapSectionOnWarehouseLocationMapRefno ON "
          + "tLocationMapSection(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSectionOnLocationMapRefnoAndLocationMapSubType ON "
          + "tLocationMapSection(LocationMapRefno ASC, LocationMapSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSectionOnLocationMapSectionFKRefno ON "
          + "tLocationMapSection(LocationMapSectionFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSectionOnWarehouseLocationMapRefnoLocationMapSectionName"
          + " ON tLocationMapSection(WarehouseLocationMapRefno ASC, LocationMapSectionName ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSectionOnLocationMapSectionCode ON "
          + "tLocationMapSection(LocationMapSectionCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSectionOnLocationMapSectionBarCode ON "
          + "tLocationMapSection(LocationMapSectionBarCode ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tLocationMapSection ADD CONSTRAINT "
      //    + "LocationMapSectionToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES "
      //    + "tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableLocationMapSlotCoordinate()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tLocationMapSlotCoordinate"
          + "("
          + "LocationMapSlotCoordinateRefno INTEGER NOT NULL CONSTRAINT PK_LocationMapSlotCoordinateRefno PRIMARY KEY,"
          + "LocationMapSlotCoordinateEnum INTEGER NOT NULL,"
          + "LocationMapSlotCoordinateCode nvarchar(10) NOT NULL"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingLocationMapSlotTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating location map slot table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndLocationMapSlotCoordCode ON "
          + "tLocationMapSlotCoordinate(LocationMapSlotCoordinateEnum ASC, LocationMapSlotCoordinateCode ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableLocationMapSlot()
    {
      //// LocationMapSlotCode is a de-normalized agregate of the coordinates 1+2+3

      string sqlTable = "CREATE TABLE IF NOT EXISTS tLocationMapSlot"
          + "("
          + "LocationMapRefno INTEGER NOT NULL CONSTRAINT PK_LocationMapSlotOnLocationMapRefno PRIMARY KEY,"
          + "LocationMapSubType INTEGER NOT NULL,"
          + "SectionLocationMapRefno INTEGER NOT NULL,"
          + "LocationMapSlotCode INTEGER NOT NULL,"
          + "LocationMapSlotName nvarchar(10) NOT NULL,"
          + "SlotCoord1LocationMapRefno INTEGER NOT NULL,"
          + "SlotCoord2LocationMapRefno INTEGER NOT NULL,"
          + "SlotCoord3LocationMapRefno INTEGER NOT NULL,"
          + "LocationMapSlotFKRefno INTEGER NOT NULL,"
          + "LocationMapSlotBarCode nvarchar(13) NOT NULL,"
          + "WarehouseLocationMapRefno INTEGER NOT NULL,"
          + "CONSTRAINT LocationMapSlotToLocationMapOnLocationMapRefno "
          + "FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE,"
          + "CONSTRAINT LocationMapSlotToLocationMapSectionOnSectionLocationMapRefnoAndLocationMapRefno "
          + "FOREIGN KEY(SectionLocationMapRefno) REFERENCES tLocationMapSection(LocationMapRefno), "
          + "CONSTRAINT LocationMapSlotToLocationMapSlotCoordOnSlotCoord1LocationMapRefnoAndLocationMapSlotCoordRefno "
          + "FOREIGN KEY (SlotCoord1LocationMapRefno) REFERENCES tLocationMapSlotCoordinate(LocationMapSlotCoordinateRefno), "
          + "CONSTRAINT LocationMapSlotToLocationMapSlotCoordOnSlotCoord2LocationMapRefnoAndLocationMapSlotCoordRefno " 
          + "FOREIGN KEY (SlotCoord2LocationMapRefno) REFERENCES tLocationMapSlotCoordinate(LocationMapSlotCoordinateRefno), "
          + "CONSTRAINT LocationMapSlotToLocationMapSlotCoordOnSlotCoord3LocationMapRefnoAndLocationMapSlotCoordRefno "
          + "FOREIGN KEY (SlotCoord3LocationMapRefno) REFERENCES tLocationMapSlotCoordinate(LocationMapSlotCoordinateRefno)"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingLocationMapSlotTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating location map slot table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE INDEX IF NOT EXISTS FK_LocationMapSlotOnSectionLocationMapRefno ON "
          + "tLocationMapSlot(SectionLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_LocationMapSlotOnWarehouseLocationMapRefno ON "
          + "tLocationMapSlot(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_LocationMapSlotOnSlotCoord1LocationMapRefno ON "
          + "tLocationMapSlot(SlotCoord1LocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_LocationMapSlotOnSlotCoord2LocationMapRefno ON "
          + "tLocationMapSlot(SlotCoord2LocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_LocationMapSlotOnSlotCoord3LocationMapRefno ON "
          + "tLocationMapSlot(SlotCoord3LocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSlotOnLocationMapRefnoAndLocationMapSubType ON "
          + "tLocationMapSlot(LocationMapRefno ASC, LocationMapSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSlotOnLocationMapSlotFKRefno ON "
          + "tLocationMapSlot(LocationMapSlotFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSlotOnLocationMapSlotCode ON "
          + "tLocationMapSlot(LocationMapSlotCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSlotOnSectionLocationMapRefnoAndLocationMapSlotCode ON "
          + "tLocationMapSlot(SectionLocationMapRefno ASC, LocationMapSlotCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSlotOnLocationMapSlotBarCode ON "
          + "tLocationMapSlot(LocationMapSlotBarCode ASC)";
      this.RunDDLCommand(sqlNdx);

    //string alterTable;
    //= "ALTER TABLE tLocationMapSlot ADD CONSTRAINT LocationMapSlotToLocationMapOnLocationMapRefno "
    //    + "FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) "
    //    + "ON DELETE CASCADE ON UPDATE CASCADE";
    //this.RunDDLCommand(alterTable);

    //alterTable = "ALTER TABLE tLocationMapSlot ADD CONSTRAINT "
    //    + "LocationMapSlotToLocationMapSectionOnSectionLocationMapRefnoAndLocationMapRefno FOREIGN KEY "
    //    + "(SectionLocationMapRefno) REFERENCES tLocationMapSection(LocationMapRefno)";
    //this.RunDDLCommand(alterTable);

      //      alterTable = "ALTER TABLE tLocationMapSlot ADD CONSTRAINT "
      //    + "LocationMapSlotToLocationMapSlotCoordOnSlotCoord1LocationMapRefnoAndLocationMapSlotCoordRefno FOREIGN KEY "
      //    + "(SlotCoord1LocationMapRefno) REFERENCES tLocationMapSlotCoordinate(LocationMapSlotCoordinateRefno)";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tLocationMapSlot ADD CONSTRAINT "
      //    + "LocationMapSlotToLocationMapSlotCoordOnSlotCoord2LocationMapRefnoAndLocationMapSlotCoordRefno FOREIGN KEY "
      //    + "(SlotCoord2LocationMapRefno) REFERENCES tLocationMapSlotCoordinate(LocationMapSlotCoordinateRefno)";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tLocationMapSlot ADD CONSTRAINT "
      //    + "LocationMapSlotToLocationMapSlotCoordOnSlotCoord3LocationMapRefnoAndLocationMapSlotCoordRefno FOREIGN KEY "
      //    + "(SlotCoord3LocationMapRefno) REFERENCES tLocationMapSlotCoordinate(LocationMapSlotCoordinateRefno)";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableLocationMapType()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tLocationMapType"
          + "("
          + "LocationMapRefno INTEGER NOT NULL,"
          + "LocationMapTypeEnum INTEGER NOT NULL,"
          + "CONSTRAINT LocationMapTypeToLocationMapOnLocationMapRefno "
          + "FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingLocationMapTypeTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating location map type table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS PK_LocationMapTypeOnLocationMapRefnoAndLocationMapTypeEnum ON "
          + "tLocationMapType(LocationMapRefno ASC, LocationMapTypeEnum ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_LocationMapTypeOnLocationMapRefno ON "
          + "tLocationMapType(LocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tLocationMapType ADD CONSTRAINT LocationMapTypeToLocationMapOnLocationMapRefno "
      //    + "FOREIGN KEY (LocationMapRefno)" + " REFERENCES tLocationMap(LocationMapRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProductSchemeVariantXRef()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tProductSchemeVariantXRef"
          + "("
          + "ProductSchemeRefno INTEGER NOT NULL,"
          + "ProductSchemeVariantRefno INTEGER NOT NULL, "
          + "CONSTRAINT ProductSchemeVariantXRefToProductSchemeVariantOnProductSchemeVariantRefno " 
          + "FOREIGN KEY (ProductSchemeVariantRefno) REFERENCES tProductSchemeVariant(ProductSchemeVariantRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ProductSchemeVariantXRefToProductSchemeOnProductSchemeRefno FOREIGN KEY (ProductSchemeRefno) REFERENCES tProductScheme(ProductSchemeRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingProductSchemeVariantsXrefTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating product scheme variants xref table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "PK_ProductSchemeVariantXRefOnProductSchemeRefnoAndProductSchemeVariantRefno ON "
          + "tProductSchemeVariantXRef(ProductSchemeRefno ASC, ProductSchemeVariantRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductSchemeVariantXRefOnProductSchemeRefno ON "
          + "tProductSchemeVariantXRef(ProductSchemeRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductSchemeVariantXRefOnProductSchemeVariantRefno ON "
          + "tProductSchemeVariantXRef(ProductSchemeVariantRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tProductSchemeVariantXRef ADD CONSTRAINT "
      //    + "ProductSchemeVariantXRefToProductSchemeVariantOnProductSchemeVariantRefno FOREIGN KEY "
      //    + "(ProductSchemeVariantRefno) REFERENCES tProductSchemeVariant(ProductSchemeVariantRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tProductSchemeVariantXRef ADD CONSTRAINT "
      //    + "ProductSchemeVariantXRefToProductSchemeOnProductSchemeRefno FOREIGN KEY (ProductSchemeRefno) REFERENCES "
      //    + "tProductScheme(ProductSchemeRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProductSchemeLotNumberXRef()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tProductSchemeLotNumberXRef"
          + "("
          + "ProductSchemeRefno INTEGER NOT NULL,"
          + "ProductSchemeLotNumberRefno INTEGER NOT NULL, "
          + "CONSTRAINT ProductSchemeLotNumberXRefToProductSchemeLotNumberOnProductSchemeLotNumberRefno " 
          + "FOREIGN KEY (ProductSchemeLotNumberRefno) REFERENCES tProductSchemeLotNumber(ProductSchemeLotNumberRefno) ON DELETE CASCADE ON UPDATE CASCADE "
          + "CONSTRAINT ProductSchemeLotNumberXRefToProductSchemeOnProductSchemeRefno " 
          + "FOREIGN KEY (ProductSchemeRefno) REFERENCES tProductScheme(ProductSchemeRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingProductSchemeLotNumbersXrefTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating product scheme lot numbers xref table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "PK_ProductSchemeLotNumberXRefOnProductSchemeRefnoAndProductSchemeLotNumberRefno ON "
          + "tProductSchemeLotNumberXRef(ProductSchemeRefno ASC, ProductSchemeLotNumberRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductSchemeLotNumberXRefOnProductSchemeRefno ON "
          + "tProductSchemeLotNumberXRef(ProductSchemeRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductSchemeLotNumberXRefOnProductSchemeLotNumberRefno ON "
          + "tProductSchemeLotNumberXRef(ProductSchemeLotNumberRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tProductSchemeLotNumberXRef ADD CONSTRAINT "
      //    + "ProductSchemeLotNumberXRefToProductSchemeLotNumberOnProductSchemeLotNumberRefno FOREIGN KEY "
      //    + "(ProductSchemeLotNumberRefno) REFERENCES tProductSchemeLotNumber(ProductSchemeLotNumberRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tProductSchemeLotNumberXRef ADD CONSTRAINT "
      //    + "ProductSchemeLotNumberXRefToProductSchemeOnProductSchemeRefno FOREIGN KEY (ProductSchemeRefno) REFERENCES "
      //    + "tProductScheme(ProductSchemeRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProductScheme()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tProductScheme"
          + "("
          + "ProductSchemeRefno INTEGER NOT NULL CONSTRAINT PK_ProductSchemeOnProductSchemeRefno PRIMARY KEY,"
          + "ProductSchemeCode nvarchar(10) NOT NULL,"
          + "ProductSchemeIsSerialTracked bit NOT NULL,"
          + "ProductSchemeIsProductionDateTracked bit NOT NULL,"
          + "ProductSchemeIsLotTracked bit NOT NULL,"
          + "ProductSchemeIsBestBeforeDateTracked bit NOT NULL,"
          + "ProductSchemeIsVariantTracked bit NOT NULL,"
          + "ProductSchemeIsLotUniqueComponent bit NOT NULL,"
          + "ProductSchemeIsProductionDateUniqueComponent bit NOT NULL,"
          + "ProductSchemeIsBestBeforeDateUniqueComponent bit NOT NULL,"
          + "ProductSchemeIsVariantUniqueComponent bit NOT NULL,"
          + "ProductSchemeExtra1Type INTEGER NOT NULL,"
          + "ProductSchemeExtra2Type INTEGER NOT NULL,"
          + "ProductSchemeExtra3Type INTEGER NOT NULL,"
          + "ProductSchemeExtra4Type INTEGER NOT NULL,"
          + "ProductSchemeExtra5Type INTEGER NOT NULL,"
          + "ProductSchemeExtra1Caption nvarchar(10) NOT NULL,"
          + "ProductSchemeExtra2Caption nvarchar(10) NOT NULL,"
          + "ProductSchemeExtra3Caption nvarchar(10) NOT NULL,"
          + "ProductSchemeExtra4Caption nvarchar(10) NOT NULL,"
          + "ProductSchemeExtra5Caption nvarchar(10) NOT NULL,"
          + "ProductSchemeExtra1IsRequired bit NOT NULL,"
          + "ProductSchemeExtra2IsRequired bit NOT NULL,"
          + "ProductSchemeExtra3IsRequired bit NOT NULL,"
          + "ProductSchemeExtra4IsRequired bit NOT NULL,"
          + "ProductSchemeExtra5IsRequired bit NOT NULL,"
          + "ProductSchemeIsLotTrackedDefault bit NOT NULL,"
          + "ProductSchemeIsProductionDateTrackedDefault bit NOT NULL,"
          + "ProductSchemeIsBestBeforeDateTrackedDefault bit NOT NULL"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingProductSchemeTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating product scheme table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ProductSchemeOnProductSchemeCode ON "
          + "tProductScheme(ProductSchemeCode ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProductSchemeVariant()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tProductSchemeVariant"
          + "("
          + "ProductSchemeVariantRefno INTEGER NOT NULL CONSTRAINT "
          + "PK_ProductSchemeVariantOnProductSchemeVariantRefno PRIMARY KEY,"
          + "ProductSchemeVariantCode nvarchar(5) NOT NULL,"
          + "ProductSchemeVariantDescription nvarchar(30) NOT NULL,"
          + "ProductSchemeVariantNo INTEGER NOT NULL,"
          + "ProductSchemeVariantIsDefault bit NOT NULL "
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingProductSchemeVariantsTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating product scheme variants table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ProductSchemeVariantOnProductSchemeVariantCode ON "
          + "tProductSchemeVariant(ProductSchemeVariantCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ProductSchemeVariantOnProductSchemeVariantNo ON "
          + "tProductSchemeVariant(ProductSchemeVariantNo ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProductSchemeLotNumber()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tProductSchemeLotNumber"
          + "("
          + "ProductSchemeLotNumberRefno INTEGER NOT NULL CONSTRAINT "
          + "PK_ProductSchemeLotNumberOnProductSchemeLotNumberRefno PRIMARY KEY,"
          + "ProductSchemeLotNumberCode nvarchar(20) NOT NULL,"
          + "ProductSchemeLotNumberCodeDefault nvarchar(20) NOT NULL"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingProductSchemeLotsTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating product scheme lots table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ProductSchemeLotNumberOnProductSchemeLotNumberCode ON "
          + "tProductSchemeLotNumber(ProductSchemeLotNumberCode ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProduct()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tProduct" 
          + "(" 
          + "ProductRefno INTEGER NOT NULL CONSTRAINT PK_ProductOnProductRefno PRIMARY KEY," 
          + "ProductSchemeRefno INTEGER NOT NULL," 
          + "FamilyRefno INTEGER NOT NULL," 
          + "ProductCode nvarchar(10) NOT NULL," 
          + "ProductFKRefno INTEGER NOT NULL," 
          + "ProductDescription nvarchar(25) NOT NULL," 
          + "ProductIsCatchCount bit NOT NULL," 
          + "ProductIsCatchWeight bit NOT NULL," 
          + "ProductIsCatchAsk bit NOT NULL,"
          + "ProductIsCatchAskZero bit NOT NULL,"
          + "ProductBrandDescription nvarchar(10) NOT NULL," 
          + "ProductAllowConvertProduct bit NOT NULL, "
          + "CONSTRAINT ProductToProductSchemeOnProductSchemeRefno "
          + "FOREIGN KEY (ProductSchemeRefno) REFERENCES tProductScheme(ProductSchemeRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingProductsTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating products table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductOnProductSchemeRefno ON tProduct(ProductSchemeRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ProductOnProductCode ON tProduct(ProductCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS ProductOnProductDescription ON tProduct(ProductDescription ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ProductOnProductFKRefno ON tProduct(ProductFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tProduct ADD CONSTRAINT ProductToProductSchemeOnProductSchemeRefno " 
      //    + "FOREIGN KEY (ProductSchemeRefno)" 
      //    + " REFERENCES tProductScheme(ProductSchemeRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProductUnit()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tProductUnit" 
          + "(" 
          + "ProductUnitRefno INTEGER NOT NULL CONSTRAINT PK_ProductUnitOnProductUnitRefno PRIMARY KEY," 
          + "ProductRefno INTEGER NOT NULL," 
          + "UnitRefno INTEGER NOT NULL," 
          + "BaseUnitRefno INTEGER NOT NULL," 
          + "WtUnitRefno INTEGER NOT NULL," 
          + "ProductUnitIsBC bit NOT NULL," 
          + "ProductUnitBC nvarchar(1) NOT NULL," 
          + "ProductUnitWeight numeric(11, 3) NOT NULL," 
          + "ProductUnitSizeDescription nvarchar(15) NOT NULL," 
          + "ProductUnitIsSellUnit bit NOT NULL," 
          + "ProductUnitIsDefault bit NOT NULL," 
          + "ProductUnitBoxCount numeric(10, 4) NOT NULL, "
          + "CONSTRAINT ProductUnitToProductOnProductRefno FOREIGN KEY (ProductRefno) REFERENCES tProduct(ProductRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ProductUnitToUnitOnUnitRefno FOREIGN KEY (UnitRefno) REFERENCES tUnit(UnitRefno), "
          + "CONSTRAINT ProductUnitToUnitOnBaseUnitRefno FOREIGN KEY (BaseUnitRefno) REFERENCES tUnit(UnitRefno), "
          + "CONSTRAINT ProductUnitToUnitOnWtUnitRefno FOREIGN KEY (WtUnitRefno) REFERENCES tUnit(UnitRefno)"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingProductUnitsTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating product units table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductUnitOnProductRefno ON tProductUnit(ProductRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductUnitOnUnitRefno ON tProductUnit(UnitRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductUnitOnBaseUnitRefno ON tProductUnit(BaseUnitRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductUnitOnWtUnitRefno ON tProductUnit(WtUnitRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC " 
          + "ON tProductUnit(ProductRefno ASC, UnitRefno ASC, ProductUnitIsBC ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tProductUnit ADD CONSTRAINT ProductUnitToProductOnProductRefno " 
      //    + "FOREIGN KEY (ProductRefno)" + " REFERENCES tProduct(ProductRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tProductUnit ADD CONSTRAINT ProductUnitToUnitOnUnitRefno FOREIGN KEY (UnitRefno)" 
      //    + " REFERENCES tUnit(UnitRefno)";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tProductUnit ADD CONSTRAINT ProductUnitToUnitOnBaseUnitRefno " 
      //    + "FOREIGN KEY (BaseUnitRefno) REFERENCES tUnit(UnitRefno)";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tProductUnit ADD CONSTRAINT ProductUnitToUnitOnWtUnitRefno " 
      //    + "FOREIGN KEY (WtUnitRefno) REFERENCES tUnit(UnitRefno)";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProductUnitBarCode()
    {
      // 255 max len for a 1D barcode
      string sqlTable = "CREATE TABLE IF NOT EXISTS tProductUnitBarCode"
          + "("
          + "ProductUnitBarCodeRefno INTEGER NOT NULL CONSTRAINT "
          + "PK_ProductUnitBarCodeOnProductUnitBarCodeRefno PRIMARY KEY,"
          + "ProductUnitRefno INTEGER NOT NULL,"
          + "ProductUnitBarCodeCode nvarchar(20) NOT NULL,"
          + "ProductUnitBarCodeRaw nvarchar(255) NOT NULL,"
          + "ProductUnitBarCodeSymbologyEnum INTEGER NOT NULL,"
          + "ProductUnitBarCodeModifier1 nvarchar(1) NOT NULL,"
          + "ProductUnitBarCodeStatus INTEGER NOT NULL, "
          + "CONSTRAINT ProductUnitBarCodeToProductUnitOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingProductUnitBarCodesTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating product unit bar codes table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductUnitBarCodeOnProductUnitRefno ON "
          + "tProductUnitBarCode(ProductUnitRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1 ON "
          + "tProductUnitBarCode(ProductUnitBarCodeCode ASC, ProductUnitBarCodeModifier1 ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tProductUnitBarCode ADD CONSTRAINT "
      //    + "ProductUnitBarCodeToProductUnitOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES "
      //    + "tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProductUnitRelation()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tProductUnitRelation"
          + "("
          + "ProductUnit1Refno INTEGER NOT NULL,"
          + "ProductUnit2Refno INTEGER NOT NULL,"
          + "ProductUnitRelationFactor numeric(12, 6) NOT NULL, "
          + "CONSTRAINT ProductUnitRelationToProductUnitOnProductUnit1RefnoAndProductUnitRefno FOREIGN KEY (ProductUnit1Refno) REFERENCES tProductUnit(ProductUnitRefno), "
          + "CONSTRAINT ProductUnitRelationToProductUnitOnProductUnit2RefnoAndProductUnitRefno FOREIGN KEY (ProductUnit2Refno) REFERENCES tProductUnit(ProductUnitRefno)"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingProductUnitRelationsTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating product unit relations table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS PK_ProductUnitRelationOnProductUnit1RefnoAndProductUnit2Refno "
          + "ON tProductUnitRelation(ProductUnit1Refno, ProductUnit2Refno)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductUnitRelationOnProductUnit1Refno ON "
          + "tProductUnitRelation(ProductUnit1Refno)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductUnitRelationOnProductUnit2Refno ON "
          + "tProductUnitRelation(ProductUnit2Refno)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tProductUnitRelation ADD ;
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tProductUnitRelation ADD CONSTRAINT "
      //    + "ProductUnitRelationToProductUnitOnProductUnit2RefnoAndProductUnitRefno FOREIGN KEY (ProductUnit2Refno) "
      //    + "REFERENCES tProductUnit(ProductUnitRefno)";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    /// 
    /// </summary>
    private void CreateTableWarehouseLocationMapType()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tWarehouseLocationMapType"
          + "("
          + "WarehouseLocationMapRefno INTEGER NOT NULL,"
          + "WarehouseLocationMapTypeEnum INTEGER NOT NULL,"
          + "LocationMapRefno INTEGER NOT NULL, "
          + "CONSTRAINT WarehouseLocationMapTypeToLocationMapOnLocationMapRefno "
          + "FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT WarehouseLocationMapTypeToLocationMapOnWarehouseLocationMapRefno "
          + "FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMap(LocationMapRefno)"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingWarehouseLocationMapTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating warehouse location map table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "PK_WarehouseLocationMapTypeOnWarehouseLocationMapRefnoAndLocationMapRefnoAndWarehouseLocationMapTypeEnum ON "
          + "tWarehouseLocationMapType(WarehouseLocationMapRefno, LocationMapRefno, WarehouseLocationMapTypeEnum)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "UQ_WarehouseLocationMapTypeOnWarehouseLocationMapRefnoAndWarehouseLocationMapTypeEnum ON "
          + "tWarehouseLocationMapType(WarehouseLocationMapRefno, WarehouseLocationMapTypeEnum)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_WarehouseLocationMapTypeOnWarehouseLocationMapRefno ON "
          + "tWarehouseLocationMapType(WarehouseLocationMapRefno)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_WarehouseLocationMapTypeOnLocationMapRefno ON "
          + "tWarehouseLocationMapType(LocationMapRefno)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tWarehouseLocationMapType ADD CONSTRAINT "
      //    + "WarehouseLocationMapTypeToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES "
      //    + "tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tWarehouseLocationMapType ADD CONSTRAINT "
      //    + "WarehouseLocationMapTypeToLocationMapOnWarehouseLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) "
      //    + "REFERENCES tLocationMap(LocationMapRefno)";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProductLocationMapType()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tProductLocationMapType"
          + "("
          + "ProductRefno INTEGER NOT NULL,"
          + "WarehouseLocationMapRefno INTEGER NOT NULL,"
          + "ProductLocationMapTypeEnum INTEGER NOT NULL,"
          + "LocationMapRefno INTEGER NOT NULL, "
          + "CONSTRAINT ProductLocationMapTypeToProductOnProductRefno FOREIGN KEY (ProductRefno) REFERENCES tProduct(ProductRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ProductLocationMapTypeToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ProductLocationMapTypeToLocationMapOnWarehouseLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMap(LocationMapRefno)"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingProductLocationMapTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating product location map table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "PK_ProductLocationMapTypeOnProductRefnoAndLocationMapRefnoAndProductLocationMapTypeEnum ON "
          + "tProductLocationMapType(ProductRefno, LocationMapRefno, ProductLocationMapTypeEnum)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "FK_ProductLocationMapTypeOnWarehouseLocationMapRefnoAndProductRefnoAndProductLocationMapTypeEnum ON "
          + "tProductLocationMapType(WarehouseLocationMapRefno, ProductRefno, ProductLocationMapTypeEnum)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductLocationMapTypeOnProductRefno ON "
          + "tProductLocationMapType(ProductRefno)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductLocationMapTypeOnWarehouseLocationMapRefno ON "
          + "tProductLocationMapType(WarehouseLocationMapRefno)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductLocationMapTypeOnLocationMapRefno ON "
          + "tProductLocationMapType(LocationMapRefno)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tProductLocationMapType ADD CONSTRAINT "
      //    + "ProductLocationMapTypeToProductOnProductRefno FOREIGN KEY (ProductRefno) REFERENCES tProduct(ProductRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tProductLocationMapType ADD CONSTRAINT "
      //    + "ProductLocationMapTypeToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES "
      //    + "tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tProductLocationMapType ADD CONSTRAINT "
      //    + "ProductLocationMapTypeToLocationMapOnWarehouseLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) "
      //    + "REFERENCES tLocationMap(LocationMapRefno)";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableContact()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tContact"
          + "("
          + "ContactRefno INTEGER NOT NULL CONSTRAINT PK_ContactOnContactRefno PRIMARY KEY,"
          + "ContactSubType INTEGER NOT NULL"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingContactsTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating contacts table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactOnContactRefnoAndContactSubType ON "
          + "tContact(ContactRefno ASC, ContactSubType ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableContactUser()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tContactUser"
          + "("
          + "ContactRefno INTEGER NOT NULL CONSTRAINT PK_UserOnContactRefno PRIMARY KEY,"
          + "ContactSubType INTEGER NOT NULL,"
          + "ContactUserCode INTEGER NOT NULL,"
          + "ContactUserName nvarchar(20),"
          + "ContactUserPassword nvarchar(6),"
          + "ContactUserIsAllowedAssociateProductBarCode bit NOT NULL,"
          + "ContactUserIsAllowedPickingModuleOverridePicker bit NOT NULL,"
          + "ContactUserIsAllowedPickingModuleOverrideAddManualDetail bit NOT NULL,"
          + "ContactUserLocalizationLanguage nvarchar(3),"
          + "CONSTRAINT ContactUserToContactOnContactRefno FOREIGN KEY (ContactRefno) REFERENCES tContact(ContactRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingContactUsersTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating contact users table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactUserOnContactRefnoAndContactSubType ON "
          + "tContactUser(ContactRefno ASC, ContactSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactUserOnContactUserCode ON tContactUser(ContactUserCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactUserOnContactUserName ON tContactUser(ContactUserName ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tContactUser ADD CONSTRAINT ContactUserToContactOnContactRefno FOREIGN KEY "
      //    + "(ContactRefno) REFERENCES tContact(ContactRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableContactCustomer()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tContactCustomer"
          + "("
          + "ContactRefno INTEGER NOT NULL CONSTRAINT PK_UserOnContactRefno PRIMARY KEY,"
          + "ContactSubType INTEGER NOT NULL,"
          + "ContactCustomerFKRefno INTEGER NOT NULL,"
          + "ContactCustomerCode nvarchar(8) NOT NULL,"
          + "ContactCustomerName nvarchar(40) NOT NULL,"
          + "CONSTRAINT ContactCustomerToContactOnContactRefno FOREIGN KEY (ContactRefno) REFERENCES tContact(ContactRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingContactCustomersTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating contact customers table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactCustomerOnContactRefnoAndContactSubType ON "
          + "tContactCustomer(ContactRefno ASC, ContactSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactCustomerOnContactCustomerFKRefno ON "
          + "tContactCustomer(ContactCustomerFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactCustomerOnContactCustomerCode ON "
          + "tContactCustomer(ContactCustomerCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactCustomerOnContactCustomerName ON "
          + "tContactCustomer(ContactCustomerName ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tContactCustomer ADD CONSTRAINT ContactCustomerToContactOnContactRefno "
      //    + "FOREIGN KEY (ContactRefno) REFERENCES tContact(ContactRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableContactCustomerShipTo()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tContactCustomerShipTo"
          + "("
          + "ContactRefno INTEGER NOT NULL CONSTRAINT PK_UserOnContactRefno PRIMARY KEY,"
          + "ContactSubType INTEGER NOT NULL,"
          + "CustomerContactRefno INTEGER NOT NULL,"
          + "ContactCustomerShipToFKRefno INTEGER NOT NULL,"
          + "ContactCustomerShipToCode nvarchar(8) NOT NULL,"
          + "ContactCustomerShipToName nvarchar(40) NOT NULL," 
          + "CONSTRAINT ContactCustomerShipToContactOnContactRefno FOREIGN KEY (ContactRefno) REFERENCES tContact(ContactRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ContactCustomerShipToContactCustomerOnCustomerContactRefno FOREIGN KEY (CustomerContactRefno) REFERENCES tContactCustomer(ContactRefno)"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingContactCustomerShiptosTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating contact customer shipto's table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactCustomerShipToOnContactRefnoAndContactSubType ON "
          + "tContactCustomerShipTo(ContactRefno ASC, ContactSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactCustomerShipToOnContactCustomerShipToFKRefno ON "
          + "tContactCustomerShipTo(ContactCustomerShipToFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactCustomerShipToOnContactCustomerShipToCode ON "
          + "tContactCustomerShipTo(ContactCustomerShipToCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactCustomerShipToOnContactCustomerShipToName ON "
          + "tContactCustomerShipTo(ContactCustomerShipToName ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tContactCustomerShipTo ADD CONSTRAINT "
      //    + "ContactCustomerShipToContactOnContactRefno FOREIGN KEY (ContactRefno) REFERENCES tContact(ContactRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tContactCustomerShipTo ADD CONSTRAINT "
      //    + "ContactCustomerShipToContactCustomerOnCustomerContactRefno FOREIGN KEY (CustomerContactRefno) REFERENCES "
      //    + "tContactCustomer(ContactRefno)";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableContactVendor()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tContactVendor"
          + "("
          + "ContactRefno INTEGER NOT NULL CONSTRAINT PK_UserOnContactRefno PRIMARY KEY,"
          + "ContactSubType INTEGER NOT NULL,"
          + "ContactVendorFKRefno INTEGER NOT NULL,"
          + "ContactVendorCode nvarchar(8) NOT NULL,"
          + "ContactVendorName nvarchar(40) NOT NULL, "
          + "CONSTRAINT ContactVendorToContactOnContactRefno FOREIGN KEY (ContactRefno) REFERENCES tContact(ContactRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingContactVendorsTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating contact vendors table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactVendorOnContactRefnoAndContactSubType ON "
          + "tContactVendor(ContactRefno ASC, ContactSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactVendorOnContactVendorFKRefno ON "
          + "tContactVendor(ContactVendorFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactVendorOnContactVendorCode ON "
          + "tContactVendor(ContactVendorCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactVendorOnContactVendorName ON "
          + "tContactVendor(ContactVendorName ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tContactVendor ADD CONSTRAINT ContactVendorToContactOnContactRefno FOREIGN KEY "
      //    + "(ContactRefno) REFERENCES tContact(ContactRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableContactVendorSupplier()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tContactVendorSupplier"
          + "("
          + "ContactRefno INTEGER NOT NULL CONSTRAINT PK_UserOnContactRefno PRIMARY KEY,"
          + "ContactSubType INTEGER NOT NULL,"
          + "VendorContactRefno INTEGER NOT NULL,"
          + "ContactVendorSupplierFKRefno INTEGER NOT NULL,"
          + "ContactVendorSupplierCode nvarchar(8) NOT NULL,"
          + "ContactVendorSupplierName nvarchar(40) NOT NULL, "
          + "CONSTRAINT ContactVendorSupplierToContactOnContactRefno FOREIGN KEY (ContactRefno) REFERENCES tContact(ContactRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ContactVendorSupplierToVendorContactOnVendorContactRefno FOREIGN KEY (VendorContactRefno) REFERENCES tContactVendor(ContactRefno)"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingContactVendorSuppliersTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating contact vendor suppliers table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactVendorSupplierOnContactRefnoAndContactSubType ON "
          + "tContactVendorSupplier(ContactRefno ASC, ContactSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactVendorSupplierOnContactVendorSupplierFKRefno ON "
          + "tContactVendorSupplier(ContactVendorSupplierFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactVendorSupplierOnContactVendorSupplierCode ON "
          + "tContactVendorSupplier(ContactVendorSupplierCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactVendorSupplierOnContactVendorSupplierName ON "
          + "tContactVendorSupplier(ContactVendorSupplierName ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tContactVendorSupplier ADD CONSTRAINT "
      //    + "ContactVendorSupplierToContactOnContactRefno FOREIGN KEY (ContactRefno) REFERENCES tContact(ContactRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tContactVendorSupplier ADD CONSTRAINT "
      //    + "ContactVendorSupplierToVendorContactOnVendorContactRefno FOREIGN KEY (VendorContactRefno) REFERENCES "
      //    + "tContactVendor(ContactRefno)";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableReasonCode()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tReasonCode"
          + "("
          + "ReasonCodeRefno INTEGER NOT NULL CONSTRAINT PK_ReasonCodeOnReasonCodeRefno PRIMARY KEY,"
          + "ReasonCodeFKRefno INTEGER NOT NULL,"
          + "ReasonCodeCode nvarchar(4) NOT NULL,"
          + "ReasonCodeDescription nvarchar(25) NOT NULL"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingReasonCodeTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating reason code table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityOnReasonCodeFKRefno ON "
          + "tReasonCode(ReasonCodeFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityOnReasonCodeCode ON tReasonCode(ReasonCodeCode ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemReasonCodeXRef()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemReasonCodeXRef"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL,"
          + "ReasonCodeRefno INTEGER NOT NULL, "
          + "CONSTRAINT ActivityItemReasonCodeXRefTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityItemReasonCodeXRefToReasonCodeOnReasonCodeRefno FOREIGN KEY (ReasonCodeRefno) REFERENCES tReasonCode(ReasonCodeRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingItemReasonCodeXrefTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating item reason code xref table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "PK_ActivityItemReasonCodeXRefOnActivityItemRefnoAndReasonCodeRefno ON "
          + "tActivityItemReasonCodeXRef(ActivityItemRefno ASC, ReasonCodeRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemReasonCodeXRefOnActivityItemRefno ON "
          + "tActivityItemReasonCodeXRef(ActivityItemRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemReasonCodeXRefOnReasonCodeRefno ON "
          + "tActivityItemReasonCodeXRef(ReasonCodeRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityItemReasonCodeXRef ADD CONSTRAINT "
      //    + "ActivityItemReasonCodeXRefTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
      //    + "tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityItemReasonCodeXRef ADD CONSTRAINT "
      //    + "ActivityItemReasonCodeXRefToReasonCodeOnReasonCodeRefno FOREIGN KEY (ReasonCodeRefno) REFERENCES "
      //    + "tReasonCode(ReasonCodeRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemConvertUnit()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemConvertUnit"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemConvertUnitOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType INTEGER NOT NULL,"
          + "ActivityItemConvertUnitType INTEGER NOT NULL,"
          + "ProductUnitRefno INTEGER NOT NULL"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingItemConvertUnitsTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating item convert units table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "UQ_ActivityItemConvertUnitOnActivityItemRefnoAndActivityItemSubType ON "
          + "tActivityItemConvertUnit(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityItemConvertUnit ADD CONSTRAINT "
          + "ActivityItemConvertUnitTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
          + "tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemConvertUnit ADD CONSTRAINT "
          + "ActivityItemConvertUnitTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES "
          + "tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailConvertUnit()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailConvertUnit"
          + "("
          + "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailConvertUnitOnActivityDetailRefno PRIMARY KEY,"
          + "ActivityDetailConvertUnitFKRefno INTEGER NOT NULL "
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingDetailConvertUnitsTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating detail convert units table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityDetailConvertUnitOnActivityDetailConvertUnitFKRefno "
          + "ON tActivityDetailConvertUnit(ActivityDetailConvertUnitFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityDetailConvertUnit ADD CONSTRAINT "
          + "ActivityDetailConvertUnitTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES "
          + "tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemMoveInventory()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemMoveInventory"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemMoveInventoryOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType INTEGER NOT NULL,"
          + "ActivityItemMoveInventoryType INTEGER NOT NULL,"
          + "ProductUnitRefno INTEGER NOT NULL"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingItemMoveInventoryTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating item move inventory table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "UQ_ActivityItemMoveInventoryOnActivityItemRefnoAndActivityItemSubType ON "
          + "tActivityItemMoveInventory(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityItemMoveInventory ADD CONSTRAINT "
          + "ActivityItemMoveInventoryTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
          + "tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemMoveInventory ADD CONSTRAINT "
          + "ActivityItemMoveInventoryTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES "
          + "tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailMoveInventory()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailMoveInventory"
          + "("
          + "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailMoveInventoryOnActivityDetailRefno PRIMARY KEY,"
          + "ActivityDetailMoveInventoryFKRefno INTEGER NOT NULL "
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingDetailMoveInventoryTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating detail move inventory table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "UQ_ActivityDetailMoveInventoryOnActivityDetailMoveInventoryFKRefno ON "
          + "tActivityDetailMoveInventory(ActivityDetailMoveInventoryFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityDetailMoveInventory ADD CONSTRAINT "
          + "ActivityDetailMoveInventoryTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES "
          + "tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityAdjustment()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityAdjustment"
          + "("
          + "ActivityRefno INTEGER NOT NULL CONSTRAINT PK_ActivityAdjustmentOnActivityRefno PRIMARY KEY,"
          + "ActivitySubType INTEGER NOT NULL,"
          + "ActivityAdjustmentFKRefno INTEGER NOT NULL,"
          + "ActivityAdjustmentStringFKRefno nvarchar(8) NOT NULL,"
          + "ActivityAdjustmentType INTEGER NOT NULL,"
          + "WarehouseLocationMapRefno INTEGER NOT NULL,"
          + "ActivityAdjustmentDateTime datetime NOT NULL,"
          + "ActivityAdjustmentDescription nvarchar(40) NOT NULL,"
          + "ActivityAdjustmentTypeDescription nvarchar(20) NOT NULL, "
          + "CONSTRAINT ActivityAdjustmentToActivityOnActivityRefnoAndActivitySubType FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityAdjustmentToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingAdjustmentTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating adjustment table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityAdjustmentOnActivityRefnoAndActivitySubType ON "
          + "tActivityAdjustment(ActivityRefno ASC, ActivitySubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityAdjustmentOnActivityAdjustmentFKRefno ON "
          + "tActivityAdjustment(ActivityAdjustmentFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityAdjustmentOnWarehouseLocationMapRefno ON "
          + "tActivityAdjustment(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityAdjustment ADD CONSTRAINT "
      //    + "ActivityAdjustmentToActivityOnActivityRefnoAndActivitySubType FOREIGN KEY (ActivityRefno) REFERENCES "
      //    + "tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityAdjustment ADD CONSTRAINT "
      //    + "ActivityAdjustmentToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno FOREIGN KEY "
      //    + "(WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemAdjustment()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemAdjustment"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemAdjustmentOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType INTEGER NOT NULL,"
          + "ActivityItemAdjustmentFKRefno INTEGER NOT NULL,"
          + "ActivityItemAdjustmentStringFKRefno nvarchar(8) NOT NULL,"
          + "ProductUnitRefno INTEGER NOT NULL,"
          + "WarehouseLocationMapRefno INTEGER NOT NULL,"
          + "ActivityItemAdjustmentQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemAdjustmentBaseQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemAdjustmentWeightQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemAdjustmentDescription nvarchar(42) NOT NULL, "
          + "CONSTRAINT ActivityItemAdjustmentTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityItemAdjustmentToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityItemAdjustmentTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingItemAdjustmentTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating item adjustment table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "UQ_ActivityItemAdjustmentOnActivityItemRefnoAndActivityItemSubType ON "
          + "tActivityItemAdjustment(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemAdjustmentOnActivityItemAdjustmentFKRefno ON "
          + "tActivityItemAdjustment(ActivityItemAdjustmentFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemAdjustmentOnWarehouseLocationMapRefno ON "
          + "tActivityItemAdjustment(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityItemAdjustment ADD CONSTRAINT "
      //    + "ActivityItemAdjustmentTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
      //    + "tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityItemAdjustment ADD CONSTRAINT "
      //    + "ActivityItemAdjustmentToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY "
      //    + "(WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityItemAdjustment ADD CONSTRAINT "
      //    + "ActivityItemAdjustmentTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES "
      //    + "tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailAdjustment()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailAdjustment"
          + "("
          + "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailAdjustmentOnActivityDetailRefno PRIMARY KEY,"
          + "ActivityDetailAdjustmentFKRefno INTEGER NOT NULL, "
          + "CONSTRAINT ActivityDetailAdjustmentTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingDetailAdjustmentTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating detail adjustment table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "UQ_ActivityDetailAdjustmentOnActivityDetailAdjustmentFKRefno ON "
          + "tActivityDetailAdjustment(ActivityDetailAdjustmentFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityDetailAdjustment ADD CONSTRAINT "
      //    + "ActivityDetailAdjustmentTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES "
      //    + "tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityProduction()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityProduction"
          + "("
          + "ActivityRefno INTEGER NOT NULL CONSTRAINT PK_ActivityProductionOnActivityRefno PRIMARY KEY,"
          + "ActivitySubType INTEGER NOT NULL,"
          + "WarehouseLocationMapRefno INTEGER NOT NULL,"
          + "ActivityProductionDateTime datetime NOT NULL,"
          + "ActivityProductionDescription nvarchar(25) NOT NULL, "
          + "CONSTRAINT ActivityProductionToActivityOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityProductionToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingProductionTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating production table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityProductionOnActivityRefnoAndActivitySubType ON "
          + "tActivityProduction(ActivityRefno ASC, ActivitySubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityProductionOnWarehouseLocationMapRefno ON "
          + "tActivityProduction(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityProduction ADD CONSTRAINT ActivityProductionToActivityOnActivityRefno "
      //    + "FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityProduction ADD CONSTRAINT "
      //    + "ActivityProductionToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno "
      //    + "FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
      //    + "ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityProductionFinished()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityProductionFinished"
          + "("
          + "ActivityRefno INTEGER NOT NULL,"
          + "ActivityProductionFKRefno INTEGER NOT NULL,"
          + "ActivityProductionStringFKRefno nvarchar(8) NOT NULL,"
          + "ActivityProductionFinishedDefaultLogisticUnitBatchLotNumber nvarchar(20) NOT NULL, "
          + "CONSTRAINT ActivityProductionFinishedToActivityProductionOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES tActivityProduction(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingProductionFinishedTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating production finished table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS PK_ActivityProductionFinishedOnActivityRefno ON "
          + "tActivityProductionFinished(ActivityRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityProductionFinishedOnActivityProductionFKRefno ON "
          + "tActivityProductionFinished(ActivityProductionFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityProductionFinished ADD CONSTRAINT "
      //    + "ActivityProductionFinishedToActivityProductionOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES "
      //    + "tActivityProduction(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityProductionMaterial()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityProductionMaterial"
          + "("
          + "ActivityRefno INTEGER NOT NULL,"
          + "ActivityProductionFKRefno INTEGER NOT NULL,"
          + "ActivityProductionStringFKRefno nvarchar(8) NOT NULL, "
          + "CONSTRAINT ActivityProductionMaterialToActivityProductionOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES tActivityProduction(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingProductionMaterialTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating production material table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS PK_ActivityProductionMaterialOnActivityRefno ON "
          + "tActivityProductionMaterial(ActivityRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityProductionMaterialOnActivityProductionFKRefno ON "
          + "tActivityProductionMaterial(ActivityProductionFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityProductionMaterial ADD CONSTRAINT "
      //    + "ActivityProductionMaterialToActivityProductionOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES "
      //    + "tActivityProduction(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityProductionByProduct()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityProductionByProduct"
          + "("
          + "ActivityRefno INTEGER NOT NULL,"
          + "ActivityProductionFKRefno INTEGER NOT NULL,"
          + "ActivityProductionStringFKRefno nvarchar(8) NOT NULL, "
          + "CONSTRAINT ActivityProductionByProductToActivityProductionOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES tActivityProduction(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingProductionByproductTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating production byproduct table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS PK_ActivityProductionByProductOnActivityRefno ON "
          + "tActivityProductionByProduct(ActivityRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityProductionByProductOnActivityProductionFKRefno ON "
          + "tActivityProductionByProduct(ActivityProductionFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityProductionByProduct ADD CONSTRAINT "
      //    + "ActivityProductionByProductToActivityProductionOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES "
      //    + "tActivityProduction(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemProduction()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemProduction"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemProductionOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType INTEGER NOT NULL,"
          + "ProductUnitRefno INTEGER NOT NULL,"
          + "LocationMapRefno INTEGER NOT NULL,"
          + "ActivityItemProductionQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemProductionBaseQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemProductionWeightQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemProductionDescription nvarchar(42) NOT NULL,"
          + "ActivityItemProductionQuantityExpected numeric(11, 3) NOT NULL,"
          + "ActivityItemProductionBaseQuantityExpected numeric(11, 3) NOT NULL,"
          + "ActivityItemProductionWeightQuantityExpected numeric(11, 3) NOT NULL, "
          + "CONSTRAINT ActivityItemProductionTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityItemProductionToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
          + "CONSTRAINT ActivityItemProductionTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingItemProductionTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating item production table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "UQ_ActivityItemProductionOnActivityItemRefnoAndActivityItemSubType ON "
          + "tActivityItemProduction(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemProductionOnLocationMapRefno ON "
          + "tActivityItemProduction(LocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityItemProduction ADD CONSTRAINT "
      //    + "ActivityItemProductionTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
      //    + "tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityItemProduction ADD CONSTRAINT "
      //    + "ActivityItemProductionToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES "
      //    + "tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);

      //alterTable = "ALTER TABLE tActivityItemProduction ADD CONSTRAINT "
      //    + "ActivityItemProductionTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES "
      //    + "tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemProductionFinished()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemProductionFinished"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemProductionFinishedOnActivityItemRefno "
          + "PRIMARY KEY,"
          + "ActivityItemProductionFinishedFKRefno INTEGER NOT NULL,"
          + "ActivityItemProductionFinishedStringFKRefno nvarchar(8) NOT NULL, "
          + "CONSTRAINT ActivityItemProductionFinishedTactItemProductionOnActivityRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItemProduction(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingItemProductionFinishedTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating item production finished table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "UQ_ActivityItemProductionFinishedOnActivityItemProductionFinishedFKRefno ON "
          + "tActivityItemProductionFinished(ActivityItemProductionFinishedFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityItemProductionFinished ADD CONSTRAINT "
      //    + "ActivityItemProductionFinishedTactItemProductionOnActivityRefno FOREIGN KEY (ActivityItemRefno) "
      //    + "REFERENCES tActivityItemProduction(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemProductionMaterial()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemProductionMaterial"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemProductionMaterialOnActivityItemRefno "
          + "PRIMARY KEY,"
          + "ActivityItemProductionMaterialFKRefno INTEGER NOT NULL,"
          + "ActivityItemProductionMaterialStringFKRefno nvarchar(8) NOT NULL, "
          + "CONSTRAINT ActivityItemProductionMaterialTactItemProductionOnActivityRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItemProduction(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingItemProductionMaterialTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating item production material table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "UQ_ActivityItemProductionMaterialOnActivityItemProductionMaterialFKRefno ON "
          + "tActivityItemProductionMaterial(ActivityItemProductionMaterialFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityItemProductionMaterial ADD CONSTRAINT "
      //    + "ActivityItemProductionMaterialTactItemProductionOnActivityRefno FOREIGN KEY (ActivityItemRefno) "
      //    + "REFERENCES tActivityItemProduction(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemProductionByProduct()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemProductionByProduct"
          + "("
          + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemProductionByProductOnActivityItemRefno "
          + "PRIMARY KEY,"
          + "ActivityItemProductionByProductFKRefno INTEGER NOT NULL,"
          + "ActivityItemProductionByProductStringFKRefno nvarchar(8) NOT NULL, "
          + "CONSTRAINT ActivityItemProductionByProductTactItemProductionOnActivityRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItemProduction(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingItemProductionByproductTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating item production byproduct table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "UQ_ActivityItemProductionByProductOnActivityItemProductionByProductFKRefno ON "
          + "tActivityItemProductionByProduct(ActivityItemProductionByProductFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityItemProductionByProduct ADD CONSTRAINT "
      //    + "ActivityItemProductionByProductTactItemProductionOnActivityRefno FOREIGN KEY (ActivityItemRefno) "
      //    + "REFERENCES tActivityItemProduction(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailProductionFinished()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailProductionFinished"
          + "("
          + "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailProductionFinishedOnActivityDetailRefno "
          + "PRIMARY KEY,"
          + "ActivityDetailProductionFinishedFKRefno INTEGER NOT NULL, "
          + "CONSTRAINT ActivityDetailProductionFinishedTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingDetailProductionFinishedTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating detail production finished table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "UQ_ActivityDetailProductionFinishedOnActivityDetailProductionFinishedFKRefno ON "
          + "tActivityDetailProductionFinished(ActivityDetailProductionFinishedFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityDetailProductionFinished ADD CONSTRAINT "
      //    + "ActivityDetailProductionFinishedTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) "
      //    + "REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailProductionMaterial()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailProductionMaterial"
          + "("
          + "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailProductionMaterialOnActivityDetailRefno "
          + "PRIMARY KEY,"
          + "ActivityDetailProductionMaterialFKRefno INTEGER NOT NULL, "
          + "CONSTRAINT ActivityDetailProductionMaterialTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingDetailProductionMaterialTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating detail production material table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "UQ_ActivityDetailProductionMaterialOnActivityDetailProductionMaterialFKRefno ON "
          + "tActivityDetailProductionMaterial(ActivityDetailProductionMaterialFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityDetailProductionMaterial ADD CONSTRAINT "
      //    + "ActivityDetailProductionMaterialTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) "
      //    + "REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailProductionByProduct()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailProductionByProduct"
          + "("
          + "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailProductionByProductOnActivityDetailRefno "
          + "PRIMARY KEY,"
          + "ActivityDetailProductionByProductFKRefno INTEGER NOT NULL, "
          + "CONSTRAINT ActivityDetailProductionByProductTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingDetailProductionByproductTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating detail production byproduct table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
          + "UQ_ActivityDetailProductionByProductOnActivityDetailProductionByProductFKRefno ON "
          + "tActivityDetailProductionByProduct(ActivityDetailProductionByProductFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //string alterTable = "ALTER TABLE tActivityDetailProductionByProduct ADD CONSTRAINT "
      //    + "ActivityDetailProductionByProductTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) "
      //    + "REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      //this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableFamily()
    {
      string sqlTable = "CREATE TABLE IF NOT EXISTS tFamily"
          + "("
          + "FamilyRefno INTEGER NOT NULL CONSTRAINT PK_FamilyOnFamilyRefno PRIMARY KEY,"
          + "FamilyCode nvarchar(20) NOT NULL"
          + ")";

      if (this.OnNubStatOut != null)
      {
#if MULTILANGUAGE
        this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingFamiliesTable);
#else
        this.MsgEventArgs.StatusOutMsg = "Creating families table.";
#endif
        this.OnNubStatOut(this, this.MsgEventArgs);
      }

      this.RunDDLCommand(sqlTable);

      string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_FamilyOnFamilyCode ON tFamily(FamilyCode ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    #endregion
  }
}
