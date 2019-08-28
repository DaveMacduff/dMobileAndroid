namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using System.IO;
  using System.Windows.Forms;
  using dMobile.Bedrock;
  using Microsoft.VisualBasic;

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
      string srceDBName = this.DBConn.DBName;
      string destDBName = this.DBConn.DBName + ".tmp";
      string newPassword = "Password = " + this.DBConn.DBPassword;
      SqlCeEngine engine = new SqlCeEngine("Data Source = " + srceDBName);

      try
      {
        this.ErrorRaised = false;
        engine.Compact((string)("Data Source = " + destDBName + ";" + srceDBName));
        engine.Dispose();
        File.Delete(srceDBName);
        File.Move(destDBName, srceDBName);
      }
      catch (SqlCeException ssce)
      {
        this.ErrorRaised = true;

        throw ssce;
      }
      finally
      {
        if (engine != null)
        {
          engine.Dispose();
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public void RepairDatabase()
    {
      string srceDBName = this.DBConn.DBName;
      SqlCeEngine engine = new SqlCeEngine("Data Source = " + srceDBName);

      try
      {
        this.ErrorRaised = false;
        engine.Repair((string)("Data Source = " + srceDBName), RepairOption.RecoverAllPossibleRows);
        engine.Dispose();
      }
      catch (SqlCeException ssce)
      {
        this.ErrorRaised = true;

        throw ssce;
      }
      finally
      {
        if (engine != null)
        {
          engine.Dispose();
        }
      }
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

      using (SqlCeEngine dbEngine = new SqlCeEngine(this.DBConn.ConnString))
      {
        if (this.OnNubStatOut != null)
        {
#if MULTILANGUAGE
          this.MsgEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.CreatingDatabase) + dbASCIIZ;
#else
          this.MsgEventArgs.StatusOutMsg = "Creating database " + dbASCIIZ;
#endif
          this.OnNubStatOut(this, this.MsgEventArgs);
        }

        dbEngine.CreateDatabase();
        dbEngine.Dispose();
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
      SqlCeCommand cmndDLLCmd = null;
      try
      {
        cmndDLLCmd = new SqlCeCommand(cmndDDLStr, DatabaseConnection.GetInstance().SQLConn);
        cmndDLLCmd.CommandType = CommandType.Text;
        cmndDLLCmd.ExecuteNonQuery();
      }
      catch (SqlCeException ex)
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
      catch (SqlCeException ex)
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
      string sqlTable = "CREATE TABLE tActivity"
          + "("
          + "ActivityRefno int NOT NULL IDENTITY(1, 1) CONSTRAINT PK_ActivityOnActivityRefno PRIMARY KEY,"
          + "ActivityStatus int NOT NULL,"
          + "ActivitySubType int NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityOnActivityRefnoAndActivitySubType "
          + "ON tActivity(ActivityRefno ASC, ActivitySubType ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailProductSchemeVariantXRef()
    {
      string sqlTable = "CREATE TABLE tActivityDetailProductSchemeVariantXRef"
          + "("
          + "ActivityDetailRefno int NOT NULL,"
          + "ProductSchemeVariantRefno int NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "PK_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefnoAndProductSchemeVariantRefno ON "
          + "tActivityDetailProductSchemeVariantXRef(ActivityDetailRefno ASC, ProductSchemeVariantRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefno ON "
          + "tActivityDetailProductSchemeVariantXRef(ActivityDetailRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityDetailProductSchemeVariantXRefOnProductSchemeVariantRefno "
          + "ON tActivityDetailProductSchemeVariantXRef(ProductSchemeVariantRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityDetailProductSchemeVariantXRef ADD CONSTRAINT "
          + "ActivityDetailProductSchemeVariantXRefTactDetailOnActivityDetailRefno  "
          + "FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityDetailProductSchemeVariantXRef ADD CONSTRAINT "
          + "ActivityDetailProductSchemeVariantXRefToProductSchemeVariantOnProductSchemeVariantRefno "
          + "FOREIGN KEY (ProductSchemeVariantRefno) REFERENCES tProductSchemeVariant(ProductSchemeVariantRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItem()
    {
      string sqlTable = "CREATE TABLE tActivityItem"
          + "("
          + "ActivityItemRefno int NOT NULL IDENTITY(1, 1) CONSTRAINT PK_ActivityItemOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType int NOT NULL,"
          + "ActivityItemStatus int NOT NULL,"
          + "ActivityRefno int NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityDetailOnActivityItemRefnoAndActivityItemSubType "
          + "ON tActivityItem(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityItemOnActivityRefno ON tActivityItem(ActivityRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityItem ADD CONSTRAINT ActivityItemToActivityOnActivityRefno "
          + "FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///  SN:12345678 BB:11/12/2010 BL:001.FREEZER.123456   1TT=100EA@300KG
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetail()
    {
      string sqlTable = "CREATE TABLE tActivityDetail"
          + "("
          + "ActivityDetailRefno int NOT NULL IDENTITY(1, 1) "
          + "CONSTRAINT PK_ActivityDetailOnActivityDetailRefno PRIMARY KEY,"
          + "ActivityDetailSubType int NOT NULL,"
          + "ActivityDetailStatus int NOT NULL,"
          + "ActivityItemRefno int NOT NULL,"
          + "LocationMapRefno int NOT NULL,"
          + "ProductUnitRefno int NOT NULL,"
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
          + "ActivityDetailLogisticUnitSkidNumber nvarchar(18) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityDetailOnActivityDetailRefnoAndActivityDetailSubType "
          + "ON tActivityDetail(ActivityDetailRefno ASC, ActivityDetailSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX ActivityDetailOnActivityDetailUniqueSchemeID ON "
          + "tActivityDetail(ActivityDetailUniqueSchemeID ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityDetailOnActivityItemRefno ON "
          + "tActivityDetail(ActivityItemRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityDetailOnLocationMapRefno ON "
          + "tActivityDetail(LocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityDetailOnProductUnitRefno ON "
          + "tActivityDetail(ProductUnitRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityDetail ADD CONSTRAINT ActivityDetailTactItemOnActivityItemRefno "
          + "FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityDetail ADD CONSTRAINT ActivityDetailTactItemOnLocationMapRefno "
          + "FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityDetail ADD CONSTRAINT ActivityDetailTactItemOnProductUnitRefno "
          + "FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailExtra()
    {
      string sqlTable = "CREATE TABLE tActivityDetailExtra"
          + "("
          + "ActivityDetailRefno int NOT NULL CONSTRAINT PK_ActivityDetailExtraOnActivityDetailRefno PRIMARY KEY,"
          + "ActivityDetailSubType int NOT NULL,"
          + "ActivityDetailExtra1Text nvarchar(10) NOT NULL,"
          + "ActivityDetailExtra2Text nvarchar(10) NOT NULL,"
          + "ActivityDetailExtra3Text nvarchar(10) NOT NULL,"
          + "ActivityDetailExtra4Text nvarchar(10) NOT NULL,"
          + "ActivityDetailExtra5Text nvarchar(10) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityDetailExtraOnActivityDetailRefnoAndActivityDetailSubType "
          + "ON tActivityDetailExtra(ActivityDetailRefno ASC, ActivityDetailSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityDetailExtra ADD CONSTRAINT "
          + "ActivityDetailExtraToActivityDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) "
          + "REFERENCES tActivityDetail(ActivityDetailRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityCount()
    {
      string sqlTable = "CREATE TABLE tActivityCount"
          + "("
          + "ActivityRefno int NOT NULL CONSTRAINT PK_ActivityCountOnActivityRefno PRIMARY KEY,"
          + "ActivitySubType int NOT NULL,"
          + "ActivityCountFKRefno int NOT NULL,"
          + "ActivityCountStringFKRefno nvarchar(8) NOT NULL,"
          + "WarehouseLocationMapRefno int NOT NULL,"
          + "ActivityCountType int NOT NULL,"
          + "ActivityCountDateTime datetime NOT NULL,"
          + "ActivityCountTypeDescription nvarchar(60) NOT NULL,"
          + "ActivityCountDescription nvarchar(60) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityCountOnActivityRefnoAndActivitySubType ON "
          + "tActivityCount(ActivityRefno ASC, ActivitySubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityCountOnActivityCountFKRefno ON "
          + "tActivityCount(ActivityCountFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityCountOnWarehouseLocationMapRefno ON "
          + "tActivityCount(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityCount ADD CONSTRAINT ActivityCountToActivityOnActivityRefno "
          + "FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityCount ADD CONSTRAINT "
          + "ActivityCountToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno "
          + "FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno)";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityPicking()
    {
      string sqlTable = "CREATE TABLE tActivityPicking"
          + "("
          + "ActivityRefno int NOT NULL CONSTRAINT PK_ActivityPickingOnActivityRefno PRIMARY KEY,"
          + "ActivitySubType int NOT NULL,"
          + "ActivityPickingFKRefno int NOT NULL,"
          + "ActivityPickingStringFKRefno nvarchar(8) NOT NULL,"
          + "WarehouseLocationMapRefno int NOT NULL,"
          + "ActivityPickingOrderedDateTime datetime NOT NULL,"
          + "ActivityPickingDeliveryDateTime datetime NOT NULL,"
          + "ActivityPickingCustomerContactName nvarchar(40) NOT NULL,"
          + "ActivityPickingCustomerShipToInfo nvarchar(40) NOT NULL,"
          + "ActivityPickingIsReviewed bit NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityPickingOnActivityRefnoAndActivitySubType "
          + "ON tActivityPicking(ActivityRefno ASC, ActivitySubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityPickingOnActivityPickingFKRefno ON "
          + "tActivityPicking(ActivityPickingFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityPickingOnWarehouseLocationMapRefno ON "
          + "tActivityPicking(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityPicking ADD CONSTRAINT ActivityPickingToActivityOnActivityRefno "
          + "FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityPicking ADD CONSTRAINT "
          + "ActivityPickingToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno "
          + "FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityReturns()
    {
      string sqlTable = "CREATE TABLE tActivityReturns"
          + "("
          + "ActivityRefno int NOT NULL CONSTRAINT PK_ActivityReturnsOnActivityRefno PRIMARY KEY,"
          + "ActivitySubType int NOT NULL,"
          + "ActivityReturnsFKRefno int NOT NULL,"
          + "ActivityReturnsStringFKRefno nvarchar(8) NOT NULL,"
          + "WarehouseLocationMapRefno int NOT NULL,"
          + "ActivityReturnsOrderedDateTime datetime NOT NULL,"
          + "ActivityReturnsDeliveryDateTime datetime NOT NULL,"
          + "ActivityReturnsCustomerContactName nvarchar(40) NOT NULL,"
          + "ActivityReturnsCustomerShipToInfo nvarchar(40) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityReturnsOnActivityRefnoAndActivitySubType "
          + "ON tActivityReturns(ActivityRefno ASC, ActivitySubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityReturnsOnActivityReturnsFKRefno ON "
          + "tActivityReturns(ActivityReturnsFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityReturnsOnWarehouseLocationMapRefno ON "
          + "tActivityReturns(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityReturns ADD CONSTRAINT "
          + "ActivityReturnsToActivityOnActivityRefnoAndActivitySubType FOREIGN KEY (ActivityRefno, ActivitySubType) "
          + "REFERENCES tActivity(ActivityRefno, ActivitySubType) ON DELETE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityReturns ADD CONSTRAINT "
          + "ActivityReturnsToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno "
          + "FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
          + "ON DELETE NO ACTION";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityPickingRoute()
    {
      string sqlTable = "CREATE TABLE tActivityPickingRoute"
          + "("
          + "PickingActivityRefno int NOT NULL,"
          + "RouteRefno int NOT NULL,"
          + "RouteStop int NOT NULL "
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityPickingRouteOnPickingActivityRefnoAndRouteRefno "
          + "ON tActivityPickingRoute(PickingActivityRefno ASC, RouteRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX FK_ActivityPickingRouteOnPickingActivityRefno ON "
          + "tActivityPickingRoute(PickingActivityRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityPickingRouteOnRouteRefno ON tActivityPickingRoute(RouteRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityPickingRoute ADD CONSTRAINT "
          + "ActivityPickingRouteToActivityPickingOnPickingActivityRefnoAndActivityRefno "
          + "FOREIGN KEY (PickingActivityRefno) REFERENCES tActivityPicking(ActivityRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityPickingRoute ADD CONSTRAINT ActivityPickingRouteToRouteOnRouteRefno "
          + "FOREIGN KEY (RouteRefno) REFERENCES tRoute(RouteRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityReceiving()
    {
      // TODO: tActivityReceiving.ActivityReceivingVendorSupplierGTINCompanyPrefix is not being used as of Apr 4/2014
      string sqlTable = "CREATE TABLE tActivityReceiving"
          + "("
          + "ActivityRefno int NOT NULL CONSTRAINT PK_ActivityReceivingOnActivityRefno PRIMARY KEY,"
          + "ActivitySubType int NOT NULL,"
          + "ActivityReceivingFKRefno int NOT NULL,"
          + "ActivityReceivingStringFKRefno nvarchar(8) NOT NULL,"
          + "ActivityReceivingPONumber nvarchar(8) NOT NULL,"
          + "WarehouseLocationMapRefno int NOT NULL,"
          + "ActivityReceivingIssuedDateTime datetime NOT NULL,"
          + "ActivityReceivingArrivalDateTime datetime NOT NULL,"
          + "ActivityReceivingVendorContactName nvarchar(40) NOT NULL,"
          + "ActivityReceivingVendorSupplierContactName nvarchar(40) NOT NULL,"
          + "ActivityReceivingVendorSupplierGTINCompanyPrefix nvarchar(11) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityReceivingOnActivityRefnoAndActivitySubType "
          + "ON tActivityReceiving(ActivityRefno ASC, ActivitySubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityReceivingOnActivityReceivingFKRefno ON "
          + "tActivityReceiving(ActivityReceivingFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityReceivingOnActivityReceivingPONumber ON "
          + "tActivityReceiving(ActivityReceivingPONumber ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityReceivingOnWarehouseLocationMapRefno ON "
          + "tActivityReceiving(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityReceiving ADD CONSTRAINT ActivityReceivingToActivityOnActivityRefno "
          + "FOREIGN KEY (ActivityRefno)" + " REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityReceiving ADD CONSTRAINT "
          + "ActivityReceivingToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno "
          + "FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityTransfer()
    {
      string sqlTable = "CREATE TABLE tActivityTransfer"
          + "("
          + "ActivityRefno int NOT NULL CONSTRAINT PK_ActivityTransferOnActivityRefno PRIMARY KEY,"
          + "ActivitySubType int NOT NULL,"
          + "ActivityTransferFKRefno int NOT NULL,"
          + "ActivityTransferStringFKRefno nvarchar(8) NOT NULL,"
          + "TransferToWarehouseLocationMapRefno int NOT NULL,"
          + "ActivityTransferDescription nvarchar(30) NOT NULL,"
          + "ActivityTransferDateTime datetime NOT NULL,"
          + "ActivityTransferArrivalDateTime datetime NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityTransferOnActivityRefnoAndActivitySubType ON "
          + "tActivityTransfer(ActivityRefno ASC, ActivitySubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityTransferOnActivityTransferFKRefno ON "
          + "tActivityTransfer(ActivityTransferFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityTransferOnTransferToWarehouseLocationMapRefno ON "
          + "tActivityTransfer(TransferToWarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityTransfer ADD CONSTRAINT ActivityTransferToActivityOnActivityRefno "
          + "FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityTransfer ADD CONSTRAINT "
          + "ActivityTransferToLocationMapWarehouseOnTransferToWarehouseLocationMapRefnoAndLocationMapRefno "
          + "FOREIGN KEY (TransferToWarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemCount()
    {
      string sqlTable = "CREATE TABLE tActivityItemCount"
          + "("
          + "ActivityItemRefno int NOT NULL CONSTRAINT PK_ActivityItemCountOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType int NOT NULL,"
          + "ActivityItemCountFKRefno int NOT NULL,"
          + "ActivityItemCountStringFKRefno nvarchar(8) NOT NULL,"
          + "ProductUnitRefno int NOT NULL,"
          + "LocationMapRefno int NOT NULL,"
          + "ActivityItemCountQuantityOnHand numeric(11, 3) NOT NULL,"
          + "ActivityItemCountBaseQuantityOnHand numeric(11, 3) NOT NULL,"
          + "ActivityItemCountWeightQuantityOnHand numeric(11, 3) NOT NULL,"
          + "ActivityItemCountQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemCountBaseQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemCountWeightQuantity numeric(11, 3) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityItemCountOnActivityItemRefnoAndActivityItemSubType "
          + "ON tActivityItemCount(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityItemCountOnActivityItemCountFKRefno ON "
          + "tActivityItemCount(ActivityItemCountFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      //// Note: Cannot enforce uniqueness on LocationMapRefno and ProductUnitRefno without ActivityRefno in the table.

      string alterTable = "ALTER TABLE tActivityItemCount ADD CONSTRAINT ActivityItemCountTactItemOnActivityItemRefno "
          + "FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemCount ADD CONSTRAINT ActivityItemCountToLocationMapOnLocationMapRefno "
          + "FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemCount ADD CONSTRAINT ActivityItemCountTactItemOnProductUnitRefno "
          + "FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemPicking()
    {
      string sqlTable = "CREATE TABLE tActivityItemPicking"
          + "("
          + "ActivityItemRefno int NOT NULL CONSTRAINT PK_ActivityItemPickingOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType int NOT NULL,"
          + "ActivityItemPickingFKRefno int NOT NULL,"
          + "ActivityItemPickingStringFKRefno nvarchar(8) NOT NULL,"
          + "ProductUnitRefno int NOT NULL,"
          + "WarehouseLocationMapRefno int NOT NULL,"
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
          + "ActivityItemPickingIsReviewed bit NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityItemPickingOnActivityItemRefnoAndActivityItemSubType"
          + " ON tActivityItemPicking(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityItemPickingOnActivityItemPickingFKRefno ON "
          + "tActivityItemPicking(ActivityItemPickingFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityItemPickingOnWarehouseLocationMapRefno ON "
          + "tActivityItemPicking(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityItemPicking ADD CONSTRAINT "
          + "ActivityItemPickingTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
          + "tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemPicking ADD CONSTRAINT "
          + "ActivityItemPickingToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY "
          + "(WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemPicking ADD CONSTRAINT ActivityItemPickingTactItemOnProductUnitRefno "
          + "FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemReturns()
    {
      string sqlTable = "CREATE TABLE tActivityItemReturns"
          + "("
          + "ActivityItemRefno int NOT NULL CONSTRAINT PK_ActivityItemReturnsOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType int NOT NULL,"
          + "ActivityItemReturnsFKRefno int NOT NULL,"
          + "ActivityItemReturnsStringFKRefno nvarchar(8) NOT NULL,"
          + "ProductUnitRefno int NOT NULL,"
          + "WarehouseLocationMapRefno int NOT NULL,"
          + "ActivityItemReturnsQuantityReturned numeric(11, 3) NOT NULL,"
          + "ActivityItemReturnsBaseQuantityReturned numeric(11, 3) NOT NULL,"
          + "ActivityItemReturnsWeightQuantityReturned numeric(11, 3) NOT NULL,"
          + "ActivityItemReturnsCustomerProductUnitCode nvarchar(10) NOT NULL,"
          + "ActivityItemReturnsDescription nvarchar(40) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityItemReturnsOnActivityItemRefnoAndActivityItemSubType"
          + " ON tActivityItemReturns(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityItemReturnsOnActivityItemReturnsFKRefno ON "
          + "tActivityItemReturns(ActivityItemReturnsFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityItemReturnsOnWarehouseLocationMapRefno ON "
          + "tActivityItemReturns(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityItemReturns ADD CONSTRAINT "
          + "ActivityItemReturnsTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
          + "tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemReturns ADD CONSTRAINT "
          + "ActivityItemReturnsToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY "
          + "(WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemReturns ADD CONSTRAINT ActivityItemReturnsTactItemOnProductUnitRefno "
          + "FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemPickingComment()
    {
      string sqlTable = "CREATE TABLE tActivityItemPickingComment"
          + "("
          + "ActivityItemRefno int NOT NULL CONSTRAINT PK_ActivityItemPickingCommentOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemPickingCommentComment nvarchar(255) NOT NULL"
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

      string alterTable = "ALTER TABLE tActivityItemPickingComment ADD CONSTRAINT "
          + "ActivityItemPickingCommentTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
          + "tActivityItemPicking(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemReturnsComment()
    {
      string sqlTable = "CREATE TABLE tActivityItemReturnsComment"
          + "("
          + "ActivityItemRefno int NOT NULL CONSTRAINT PK_ActivityItemReturnsCommentOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemReturnsCommentComment nvarchar(255) NOT NULL"
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

      string alterTable = "ALTER TABLE tActivityItemReturnsComment ADD CONSTRAINT "
          + "ActivityItemReturnsCommentTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
          + "tActivityItemReturns(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemReceiving()
    {
      string sqlTable = "CREATE TABLE tActivityItemReceiving"
          + "("
          + "ActivityItemRefno int NOT NULL CONSTRAINT PK_ActivityItemReceivingOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType int NOT NULL,"
          + "ActivityItemReceivingFKRefno int NOT NULL,"
          + "ActivityItemReceivingStringFKRefno nvarchar(8) NOT NULL,"
          + "ProductUnitRefno int NOT NULL,"
          + "WarehouseLocationMapRefno int NOT NULL,"
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
          + "ActivityItemReceivingWeightQuantityReceivedToDate numeric(11, 3) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityItemReceivingOnActivityItemRefnoAndActivityItemSubType"
          + " ON tActivityItemReceiving(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityItemReceivingOnActivityItemReceivingFKRefno ON "
          + "tActivityItemReceiving(ActivityItemReceivingFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityItemReceivingOnWarehouseLocationMapRefno ON "
          + "tActivityItemReceiving(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityItemReceiving ADD CONSTRAINT "
          + "ActivityItemReceivingTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
          + "tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemReceiving ADD CONSTRAINT "
          + "ActivityItemReceivingToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY "
          + "(WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemReceiving ADD CONSTRAINT ActivityItemReceivingTactItemOnProductUnitRefno "
          + "FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
   }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemPurchaseOrderWorkSheet()
    {
      string sqlTable = "CREATE TABLE tActivityItemPurchaseOrderWorkSheet"
          + "("
          + "ActivityItemRefno int NOT NULL CONSTRAINT PK_ActivityItemPurchaseOrderWorkSheetOnActivityItemRefno "
          + "PRIMARY KEY,"
          + "ActivityItemSubType int NOT NULL,"
          + "ActivityItemPurchaseOrderWorkSheetFKRefno int NOT NULL,"
          + "ActivityItemPurchaseOrderWorkSheetStringFKRefno nvarchar(8) NOT NULL,"
          + "ProductUnitRefno int NOT NULL,"
          + "WarehouseLocationMapRefno int NOT NULL,"
          + "ActivityItemPurchaseOrderWorkSheetQuantityOrdered numeric(11, 3) NOT NULL,"
          + "ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered numeric(11, 3) NOT NULL,"
          + "ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered numeric(11, 3) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "UQ_ActivityItemPurchaseOrderWorkSheetOnActivityItemRefnoAndActivityItemSubType ON "
          + "tActivityItemPurchaseOrderWorkSheet(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "UQ_ActivityItemPurchaseOrderWorkSheetOnActivityItemPurchaseOrderWorkSheetFKRefno ON "
          + "tActivityItemPurchaseOrderWorkSheet(ActivityItemPurchaseOrderWorkSheetFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityItemPurchaseOrderWorkSheetOnWarehouseLocationMapRefno ON "
          + "tActivityItemPurchaseOrderWorkSheet(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityItemPurchaseOrderWorkSheet ADD CONSTRAINT "
          + "ActivityItemPurchaseOrderWorkSheetTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) "
          + "REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemPurchaseOrderWorkSheet ADD CONSTRAINT "
          + "ActivityItemPurchaseOrderWorkSheetToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY "
          + "(WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemPurchaseOrderWorkSheet ADD CONSTRAINT "
          + "ActivityItemPurchaseOrderWorkSheetTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) "
          + "REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemReceivingComment()
    {
      string sqlTable = "CREATE TABLE tActivityItemReceivingComment"
          + "("
          + "ActivityItemRefno int NOT NULL CONSTRAINT PK_ActivityItemReceivingCommentOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemReceivingCommentComment nvarchar(255) NOT NULL"
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

      string alterTable = "ALTER TABLE tActivityItemReceivingComment ADD CONSTRAINT "
          + "ActivityItemReceivingCommentTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
          + "tActivityItemReceiving(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemTransfer()
    {
      string sqlTable = "CREATE TABLE tActivityItemTransfer"
          + "("
          + "ActivityItemRefno int NOT NULL CONSTRAINT PK_ActivityItemTransferOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType int NOT NULL,"
          + "ActivityItemTransferFKRefno int NOT NULL,"
          + "ActivityItemTransferStringFKRefno nvarchar(8) NOT NULL,"
          + "ProductUnitRefno int NOT NULL,"
          + "TransferFromWarehouseLocationMapRefno int NOT NULL,"
          + "ActivityItemTransferRequiredQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemTransferQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemTransferBaseQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemTransferWeightQuantity numeric(11, 3) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityItemTransferOnActivityItemRefnoAndActivityItemSubType"
          + " ON tActivityItemTransfer(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityItemTransferOnActivityItemTransferFKRefno ON "
          + "tActivityItemTransfer(ActivityItemTransferFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityItemTransferOnTransferFromWarehouseLocationMapRefno ON "
          + "tActivityItemTransfer(TransferFromWarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityItemTransfer ADD CONSTRAINT "
          + "ActivityItemTransferTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
          + "tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemTransfer ADD CONSTRAINT "
          + "ActivityItemTransferToLocationMapWarehouseOnTransferFromWarehouseLocationMapRefnoAndLocationMapRefno "
          + "FOREIGN KEY (TransferFromWarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemTransfer ADD CONSTRAINT ActivityItemTransferTactItemOnProductUnitRefno "
          + "FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemBarCodeAssociationXRef()
    {
      string sqlTable = "CREATE TABLE tActivityItemBarCodeAssociationXRef"
          + "("
          + "ActivityItemRefno int NOT NULL,"
          + "ProductUnitBarCodeRefno int NOT NULL "
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "PK_ActivityItemBarCodeAssociationXRefOnActivityItemRefnoAndProductUnitBarCodeRefno ON "
          + "tActivityItemBarCodeAssociationXRef(ActivityItemRefno ASC, ProductUnitBarCodeRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityItemBarCodeAssociationXRefOnActivityItemRefno ON "
          + "tActivityItemBarCodeAssociationXRef(ActivityItemRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityItemBarCodeAssociationXRefOnProductUnitBarCodeRefno ON "
          + "tActivityItemBarCodeAssociationXRef(ProductUnitBarCodeRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityItemBarCodeAssociationXRef ADD CONSTRAINT "
          + "ActivityItemBarCodeAssociationXRefTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) "
          + "REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemBarCodeAssociationXRef ADD CONSTRAINT "
          + "ActivityItemBarCodeAssociationXRefToProductUnitBarCodeOnProductUnitBarCodeRefno FOREIGN KEY "
          + "(ProductUnitBarCodeRefno) REFERENCES tProductUnitBarCode(ProductUnitBarCodeRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailCount()
    {
      string sqlTable = "CREATE TABLE tActivityDetailCount"
          + "("
          + "ActivityDetailRefno int NOT NULL CONSTRAINT PK_ActivityDetailCountOnActivityDetailRefno PRIMARY KEY,"
          + "ActivityDetailCountFKRefno int NOT NULL "
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityDetailCountOnActivityDetailCountFKRefno ON "
          + "tActivityDetailCount(ActivityDetailCountFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityDetailCount ADD CONSTRAINT "
          + "ActivityDetailCountTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES "
          + "tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailPicking()
    {
      string sqlTable = "CREATE TABLE tActivityDetailPicking"
          + "("
          + "ActivityDetailRefno int NOT NULL CONSTRAINT PK_ActivityDetailPickingOnActivityDetailRefno PRIMARY KEY,"
          + "ActivityDetailPickingFKRefno int NOT NULL,"
          + "ActivityDetailPickingIsConfirmed bit NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityDetailPickingOnActivityDetailPickingFKRefno ON "
          + "tActivityDetailPicking(ActivityDetailPickingFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityDetailPicking ADD CONSTRAINT "
          + "ActivityDetailPickingTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES "
          + "tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailReturns()
    {
      string sqlTable = "CREATE TABLE tActivityDetailReturns" +
                "(" +
                "ActivityDetailRefno int NOT NULL CONSTRAINT PK_ActivityDetailReturnsOnActivityDetailRefno PRIMARY KEY," +
                "ActivityDetailReturnsFKRefno int NOT NULL," +
                "ActivityDetailReturnsIsConfirmed bit NOT NULL" +
                ")";
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityDetailReturnsOnActivityDetailReturnsFKRefno ON "
          + "tActivityDetailReturns(ActivityDetailReturnsFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityDetailReturns ADD CONSTRAINT "
          + "ActivityDetailReturnsTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES "
          + "tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailReceiving()
    {
      string sqlTable = "CREATE TABLE tActivityDetailReceiving"
          + "("
          + "ActivityDetailRefno int NOT NULL CONSTRAINT PK_ActivityDetailReceivingOnActivityDetailRefno PRIMARY KEY,"
          + "ActivityDetailReceivingFKRefno int NOT NULL "
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityDetailReceivingOnActivityDetailReceivingFKRefno ON "
          + "tActivityDetailReceiving(ActivityDetailReceivingFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityDetailReceiving ADD CONSTRAINT "
          + "ActivityDetailReceivingTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES "
          + "tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailTransfer()
    {
      string sqlTable = "CREATE TABLE tActivityDetailTransfer"
          + "("
          + "ActivityDetailRefno int NOT NULL CONSTRAINT PK_ActivityDetailTransferOnActivityDetailRefno PRIMARY KEY,"
          + "ActivityDetailTransferFKRefno int NOT NULL,"
          + "ActivityDetailTransferIsConfirmed bit NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityDetailTransferOnActivityDetailTransferFKRefno ON "
          + "tActivityDetailTransfer(ActivityDetailTransferFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityDetailTransfer ADD CONSTRAINT "
          + "ActivityDetailTransferTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES "
          + "tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableUnit()
    {
      string sqlTable = "CREATE TABLE tUnit"
          + "("
          + "UnitRefno int NOT NULL IDENTITY(1, 1) CONSTRAINT PK_UnitOnUnitRefno PRIMARY KEY,"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_UnitOnUnitCode ON tUnit(UnitCode ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableRoute()
    {
      string sqlTable = "CREATE TABLE tRoute"
          + "("
          + "RouteRefno int NOT NULL IDENTITY(1, 1) CONSTRAINT PK_RouteOnRouteRefno PRIMARY KEY,"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_RouteOnRouteCode ON tRoute(RouteCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX RouteOnRouteDescription ON tRoute(RouteDescription ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableLocationMap()
    {
      string sqlTable = "CREATE TABLE tLocationMap"
          + "("
          + "LocationMapRefno int NOT NULL IDENTITY(1, 1) CONSTRAINT PK_LocationMapOnLocationMapRefno PRIMARY KEY,"
          + "LocationMapSubType int NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapOnLocationMapRefnoAndLocationMapSubType ON "
          + "tLocationMap(LocationMapRefno ASC, LocationMapSubType ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableLocationMapWarehouse()
    {
      string sqlTable = "CREATE TABLE tLocationMapWarehouse"
          + "("
          + "LocationMapRefno int NOT NULL CONSTRAINT PK_LocationMapWarehouseOnLocationMapRefno PRIMARY KEY,"
          + "LocationMapSubType int NOT NULL,"
          + "LocationMapWarehouseName nvarchar(22) NOT NULL,"
          + "LocationMapWarehouseCode int NOT NULL,"
          + "LocationMapWarehouseFKRefno int NOT NULL,"
          + "LocationMapWarehouseBarCode nvarchar(13) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapWarehouseOnLocationMapRefnoAndLocationMapSubType "
          + "ON tLocationMapWarehouse(LocationMapRefno ASC, LocationMapSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno ON "
          + "tLocationMapWarehouse(LocationMapWarehouseFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapWarehouseOnLocationMapWarehouseName ON "
          + "tLocationMapWarehouse(LocationMapWarehouseName ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapWarehouseOnLocationMapWarehouseCode ON "
          + "tLocationMapWarehouse(LocationMapWarehouseCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapWarehouseOnLocationMapWarehouseBarCode ON "
          + "tLocationMapWarehouse(LocationMapWarehouseBarCode ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tLocationMapWarehouse ADD CONSTRAINT "
          + "LocationMapWarehouseToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES "
          + "tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableLocationMapSection()
    {
      string sqlTable = "CREATE TABLE tLocationMapSection"
          + "("
          + "LocationMapRefno int NOT NULL CONSTRAINT PK_LocationMapSectionOnLocationMapRefno PRIMARY KEY,"
          + "LocationMapSubType int NOT NULL,"
          + "WarehouseLocationMapRefno int NOT NULL,"
          + "LocationMapSectionCode int NOT NULL,"
          + "LocationMapSectionName nvarchar(10),"
          + "LocationMapSectionFKRefno int NOT NULL,"
          + "LocationMapSectionBarCode nvarchar(13) "
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

      string sqlNdx = "CREATE NONCLUSTERED INDEX FK_LocationMapSectionOnWarehouseLocationMapRefno ON "
          + "tLocationMapSection(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapSectionOnLocationMapRefnoAndLocationMapSubType ON "
          + "tLocationMapSection(LocationMapRefno ASC, LocationMapSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapSectionOnLocationMapSectionFKRefno ON "
          + "tLocationMapSection(LocationMapSectionFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapSectionOnWarehouseLocationMapRefnoLocationMapSectionName"
          + " ON tLocationMapSection(WarehouseLocationMapRefno ASC, LocationMapSectionName ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapSectionOnLocationMapSectionCode ON "
          + "tLocationMapSection(LocationMapSectionCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapSectionOnLocationMapSectionBarCode ON "
          + "tLocationMapSection(LocationMapSectionBarCode ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tLocationMapSection ADD CONSTRAINT "
          + "LocationMapSectionToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES "
          + "tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableLocationMapSlotCoordinate()
    {
      string sqlTable = "CREATE TABLE tLocationMapSlotCoordinate"
          + "("
          + "LocationMapSlotCoordinateRefno int IDENTITY(1, 1) NOT NULL CONSTRAINT PK_LocationMapSlotCoordinateRefno "
          + "PRIMARY KEY,"
          + "LocationMapSlotCoordinateEnum int NOT NULL,"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
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

      string sqlTable = "CREATE TABLE tLocationMapSlot"
          + "("
          + "LocationMapRefno int NOT NULL CONSTRAINT PK_LocationMapSlotOnLocationMapRefno PRIMARY KEY,"
          + "LocationMapSubType int NOT NULL,"
          + "SectionLocationMapRefno int NOT NULL,"
          + "LocationMapSlotCode int NOT NULL,"
          + "LocationMapSlotName nvarchar(10) NOT NULL,"
          + "SlotCoord1LocationMapRefno int NOT NULL,"
          + "SlotCoord2LocationMapRefno int NOT NULL,"
          + "SlotCoord3LocationMapRefno int NOT NULL,"
          + "LocationMapSlotFKRefno int NOT NULL,"
          + "LocationMapSlotBarCode nvarchar(13) NOT NULL,"
          + "WarehouseLocationMapRefno int NOT NULL"
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

      string sqlNdx = "CREATE NONCLUSTERED INDEX FK_LocationMapSlotOnSectionLocationMapRefno ON "
          + "tLocationMapSlot(SectionLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_LocationMapSlotOnWarehouseLocationMapRefno ON "
          + "tLocationMapSlot(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_LocationMapSlotOnSlotCoord1LocationMapRefno ON "
          + "tLocationMapSlot(SlotCoord1LocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_LocationMapSlotOnSlotCoord2LocationMapRefno ON "
          + "tLocationMapSlot(SlotCoord2LocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_LocationMapSlotOnSlotCoord3LocationMapRefno ON "
          + "tLocationMapSlot(SlotCoord3LocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapSlotOnLocationMapRefnoAndLocationMapSubType ON "
          + "tLocationMapSlot(LocationMapRefno ASC, LocationMapSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapSlotOnLocationMapSlotFKRefno ON "
          + "tLocationMapSlot(LocationMapSlotFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapSlotOnLocationMapSlotCode ON "
          + "tLocationMapSlot(LocationMapSlotCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapSlotOnSectionLocationMapRefnoAndLocationMapSlotCode ON "
          + "tLocationMapSlot(SectionLocationMapRefno ASC, LocationMapSlotCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_LocationMapSlotOnLocationMapSlotBarCode ON "
          + "tLocationMapSlot(LocationMapSlotBarCode ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tLocationMapSlot ADD CONSTRAINT LocationMapSlotToLocationMapOnLocationMapRefno "
          + "FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tLocationMapSlot ADD CONSTRAINT "
          + "LocationMapSlotToLocationMapSectionOnSectionLocationMapRefnoAndLocationMapRefno FOREIGN KEY "
          + "(SectionLocationMapRefno) REFERENCES tLocationMapSection(LocationMapRefno)";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tLocationMapSlot ADD CONSTRAINT "
          + "LocationMapSlotToLocationMapSlotCoordOnSlotCoord1LocationMapRefnoAndLocationMapSlotCoordRefno FOREIGN KEY "
          + "(SlotCoord1LocationMapRefno) REFERENCES tLocationMapSlotCoordinate(LocationMapSlotCoordinateRefno)";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tLocationMapSlot ADD CONSTRAINT "
          + "LocationMapSlotToLocationMapSlotCoordOnSlotCoord2LocationMapRefnoAndLocationMapSlotCoordRefno FOREIGN KEY "
          + "(SlotCoord2LocationMapRefno) REFERENCES tLocationMapSlotCoordinate(LocationMapSlotCoordinateRefno)";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tLocationMapSlot ADD CONSTRAINT "
          + "LocationMapSlotToLocationMapSlotCoordOnSlotCoord3LocationMapRefnoAndLocationMapSlotCoordRefno FOREIGN KEY "
          + "(SlotCoord3LocationMapRefno) REFERENCES tLocationMapSlotCoordinate(LocationMapSlotCoordinateRefno)";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableLocationMapType()
    {
      string sqlTable = "CREATE TABLE tLocationMapType"
          + "("
          + "LocationMapRefno int NOT NULL,"
          + "LocationMapTypeEnum int NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX PK_LocationMapTypeOnLocationMapRefnoAndLocationMapTypeEnum ON "
          + "tLocationMapType(LocationMapRefno ASC, LocationMapTypeEnum ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_LocationMapTypeOnLocationMapRefno ON "
          + "tLocationMapType(LocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tLocationMapType ADD CONSTRAINT LocationMapTypeToLocationMapOnLocationMapRefno "
          + "FOREIGN KEY (LocationMapRefno)" + " REFERENCES tLocationMap(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProductSchemeVariantXRef()
    {
      string sqlTable = "CREATE TABLE tProductSchemeVariantXRef"
          + "("
          + "ProductSchemeRefno int NOT NULL,"
          + "ProductSchemeVariantRefno int NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "PK_ProductSchemeVariantXRefOnProductSchemeRefnoAndProductSchemeVariantRefno ON "
          + "tProductSchemeVariantXRef(ProductSchemeRefno ASC, ProductSchemeVariantRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ProductSchemeVariantXRefOnProductSchemeRefno ON "
          + "tProductSchemeVariantXRef(ProductSchemeRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ProductSchemeVariantXRefOnProductSchemeVariantRefno ON "
          + "tProductSchemeVariantXRef(ProductSchemeVariantRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tProductSchemeVariantXRef ADD CONSTRAINT "
          + "ProductSchemeVariantXRefToProductSchemeVariantOnProductSchemeVariantRefno FOREIGN KEY "
          + "(ProductSchemeVariantRefno) REFERENCES tProductSchemeVariant(ProductSchemeVariantRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tProductSchemeVariantXRef ADD CONSTRAINT "
          + "ProductSchemeVariantXRefToProductSchemeOnProductSchemeRefno FOREIGN KEY (ProductSchemeRefno) REFERENCES "
          + "tProductScheme(ProductSchemeRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProductSchemeLotNumberXRef()
    {
      string sqlTable = "CREATE TABLE tProductSchemeLotNumberXRef"
          + "("
          + "ProductSchemeRefno int NOT NULL,"
          + "ProductSchemeLotNumberRefno int NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "PK_ProductSchemeLotNumberXRefOnProductSchemeRefnoAndProductSchemeLotNumberRefno ON "
          + "tProductSchemeLotNumberXRef(ProductSchemeRefno ASC, ProductSchemeLotNumberRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ProductSchemeLotNumberXRefOnProductSchemeRefno ON "
          + "tProductSchemeLotNumberXRef(ProductSchemeRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ProductSchemeLotNumberXRefOnProductSchemeLotNumberRefno ON "
          + "tProductSchemeLotNumberXRef(ProductSchemeLotNumberRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tProductSchemeLotNumberXRef ADD CONSTRAINT "
          + "ProductSchemeLotNumberXRefToProductSchemeLotNumberOnProductSchemeLotNumberRefno FOREIGN KEY "
          + "(ProductSchemeLotNumberRefno) REFERENCES tProductSchemeLotNumber(ProductSchemeLotNumberRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tProductSchemeLotNumberXRef ADD CONSTRAINT "
          + "ProductSchemeLotNumberXRefToProductSchemeOnProductSchemeRefno FOREIGN KEY (ProductSchemeRefno) REFERENCES "
          + "tProductScheme(ProductSchemeRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProductScheme()
    {
      string sqlTable = "CREATE TABLE tProductScheme"
          + "("
          + "ProductSchemeRefno int IDENTITY(1, 1) NOT NULL CONSTRAINT PK_ProductSchemeOnProductSchemeRefno PRIMARY KEY,"
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
          + "ProductSchemeExtra1Type int NOT NULL,"
          + "ProductSchemeExtra2Type int NOT NULL,"
          + "ProductSchemeExtra3Type int NOT NULL,"
          + "ProductSchemeExtra4Type int NOT NULL,"
          + "ProductSchemeExtra5Type int NOT NULL,"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ProductSchemeOnProductSchemeCode ON "
          + "tProductScheme(ProductSchemeCode ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProductSchemeVariant()
    {
      string sqlTable = "CREATE TABLE tProductSchemeVariant"
          + "("
          + "ProductSchemeVariantRefno int IDENTITY(1, 1) NOT NULL CONSTRAINT "
          + "PK_ProductSchemeVariantOnProductSchemeVariantRefno PRIMARY KEY,"
          + "ProductSchemeVariantCode nvarchar(5) NOT NULL,"
          + "ProductSchemeVariantDescription nvarchar(30) NOT NULL,"
          + "ProductSchemeVariantNo int NOT NULL,"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ProductSchemeVariantOnProductSchemeVariantCode ON "
          + "tProductSchemeVariant(ProductSchemeVariantCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ProductSchemeVariantOnProductSchemeVariantNo ON "
          + "tProductSchemeVariant(ProductSchemeVariantNo ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProductSchemeLotNumber()
    {
      string sqlTable = "CREATE TABLE tProductSchemeLotNumber"
          + "("
          + "ProductSchemeLotNumberRefno int IDENTITY(1, 1) NOT NULL CONSTRAINT "
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ProductSchemeLotNumberOnProductSchemeLotNumberCode ON "
          + "tProductSchemeLotNumber(ProductSchemeLotNumberCode ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProduct()
    {
      string sqlTable = "CREATE TABLE tProduct" 
          + "(" 
          + "ProductRefno int IDENTITY(1, 1) NOT NULL CONSTRAINT PK_ProductOnProductRefno PRIMARY KEY," 
          + "ProductSchemeRefno int NOT NULL," 
          + "FamilyRefno int NOT NULL," 
          + "ProductCode nvarchar(10) NOT NULL," 
          + "ProductFKRefno int NOT NULL," 
          + "ProductDescription nvarchar(25) NOT NULL," 
          + "ProductIsCatchCount bit NOT NULL," 
          + "ProductIsCatchWeight bit NOT NULL," 
          + "ProductIsCatchAsk bit NOT NULL,"
          + "ProductIsCatchAskZero bit NOT NULL,"
          + "ProductBrandDescription nvarchar(10) NOT NULL," 
          + "ProductAllowConvertProduct bit NOT NULL"
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

      string sqlNdx = "CREATE NONCLUSTERED INDEX FK_ProductOnProductSchemeRefno ON tProduct(ProductSchemeRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ProductOnProductCode ON tProduct(ProductCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX ProductOnProductDescription ON tProduct(ProductDescription ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ProductOnProductFKRefno ON tProduct(ProductFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tProduct ADD CONSTRAINT ProductToProductSchemeOnProductSchemeRefno " 
          + "FOREIGN KEY (ProductSchemeRefno)" 
          + " REFERENCES tProductScheme(ProductSchemeRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProductUnit()
    {
      string sqlTable = "CREATE TABLE tProductUnit" 
          + "(" + "ProductUnitRefno int IDENTITY(1, 1) NOT NULL CONSTRAINT PK_ProductUnitOnProductUnitRefno PRIMARY KEY," 
          + "ProductRefno int NOT NULL," 
          + "UnitRefno int NOT NULL," 
          + "BaseUnitRefno int NOT NULL," 
          + "WtUnitRefno int NOT NULL," 
          + "ProductUnitIsBC bit NOT NULL," 
          + "ProductUnitBC nvarchar(1) NOT NULL," 
          + "ProductUnitWeight numeric(11, 3) NOT NULL," 
          + "ProductUnitSizeDescription nvarchar(15) NOT NULL," 
          + "ProductUnitIsSellUnit bit NOT NULL," 
          + "ProductUnitIsDefault bit NOT NULL," 
          + "ProductUnitBoxCount numeric(10, 4) NOT NULL"
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

      string sqlNdx = "CREATE NONCLUSTERED INDEX FK_ProductUnitOnProductRefno ON tProductUnit(ProductRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ProductUnitOnUnitRefno ON tProductUnit(UnitRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ProductUnitOnBaseUnitRefno ON tProductUnit(BaseUnitRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ProductUnitOnWtUnitRefno ON tProductUnit(WtUnitRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC " 
          + "ON tProductUnit(ProductRefno ASC, UnitRefno ASC, ProductUnitIsBC ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tProductUnit ADD CONSTRAINT ProductUnitToProductOnProductRefno " 
          + "FOREIGN KEY (ProductRefno)" + " REFERENCES tProduct(ProductRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tProductUnit ADD CONSTRAINT ProductUnitToUnitOnUnitRefno FOREIGN KEY (UnitRefno)" 
          + " REFERENCES tUnit(UnitRefno)";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tProductUnit ADD CONSTRAINT ProductUnitToUnitOnBaseUnitRefno " 
          + "FOREIGN KEY (BaseUnitRefno) REFERENCES tUnit(UnitRefno)";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tProductUnit ADD CONSTRAINT ProductUnitToUnitOnWtUnitRefno " 
          + "FOREIGN KEY (WtUnitRefno) REFERENCES tUnit(UnitRefno)";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProductUnitBarCode()
    {
      // 255 max len for a 1D barcode
      string sqlTable = "CREATE TABLE tProductUnitBarCode"
          + "("
          + "ProductUnitBarCodeRefno int IDENTITY(1, 1) NOT NULL CONSTRAINT "
          + "PK_ProductUnitBarCodeOnProductUnitBarCodeRefno PRIMARY KEY,"
          + "ProductUnitRefno int NOT NULL,"
          + "ProductUnitBarCodeCode nvarchar(20) NOT NULL,"
          + "ProductUnitBarCodeRaw nvarchar(255) NOT NULL,"
          + "ProductUnitBarCodeSymbologyEnum int NOT NULL,"
          + "ProductUnitBarCodeModifier1 nvarchar(1) NOT NULL,"
          + "ProductUnitBarCodeStatus int NOT NULL"
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

      string sqlNdx = "CREATE NONCLUSTERED INDEX FK_ProductUnitBarCodeOnProductUnitRefno ON "
          + "tProductUnitBarCode(ProductUnitRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1 ON "
          + "tProductUnitBarCode(ProductUnitBarCodeCode ASC, ProductUnitBarCodeModifier1 ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tProductUnitBarCode ADD CONSTRAINT "
          + "ProductUnitBarCodeToProductUnitOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES "
          + "tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProductUnitRelation()
    {
      string sqlTable = "CREATE TABLE tProductUnitRelation"
          + "("
          + "ProductUnit1Refno int NOT NULL,"
          + "ProductUnit2Refno int NOT NULL,"
          + "ProductUnitRelationFactor numeric(12, 6) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX PK_ProductUnitRelationOnProductUnit1RefnoAndProductUnit2Refno "
          + "ON tProductUnitRelation(ProductUnit1Refno, ProductUnit2Refno)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ProductUnitRelationOnProductUnit1Refno ON "
          + "tProductUnitRelation(ProductUnit1Refno)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ProductUnitRelationOnProductUnit2Refno ON "
          + "tProductUnitRelation(ProductUnit2Refno)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tProductUnitRelation ADD CONSTRAINT "
          + "ProductUnitRelationToProductUnitOnProductUnit1RefnoAndProductUnitRefno FOREIGN KEY (ProductUnit1Refno) "
          + "REFERENCES tProductUnit(ProductUnitRefno)";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tProductUnitRelation ADD CONSTRAINT "
          + "ProductUnitRelationToProductUnitOnProductUnit2RefnoAndProductUnitRefno FOREIGN KEY (ProductUnit2Refno) "
          + "REFERENCES tProductUnit(ProductUnitRefno)";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    /// 
    /// </summary>
    private void CreateTableWarehouseLocationMapType()
    {
      string sqlTable = "CREATE TABLE tWarehouseLocationMapType"
          + "("
          + "WarehouseLocationMapRefno int NOT NULL,"
          + "WarehouseLocationMapTypeEnum int NOT NULL,"
          + "LocationMapRefno int NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "PK_WarehouseLocationMapTypeOnWarehouseLocationMapRefnoAndLocationMapRefnoAndWarehouseLocationMapTypeEnum ON "
          + "tWarehouseLocationMapType(WarehouseLocationMapRefno, LocationMapRefno, WarehouseLocationMapTypeEnum)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "UQ_WarehouseLocationMapTypeOnWarehouseLocationMapRefnoAndWarehouseLocationMapTypeEnum ON "
          + "tWarehouseLocationMapType(WarehouseLocationMapRefno, WarehouseLocationMapTypeEnum)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_WarehouseLocationMapTypeOnWarehouseLocationMapRefno ON "
          + "tWarehouseLocationMapType(WarehouseLocationMapRefno)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_WarehouseLocationMapTypeOnLocationMapRefno ON "
          + "tWarehouseLocationMapType(LocationMapRefno)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tWarehouseLocationMapType ADD CONSTRAINT "
          + "WarehouseLocationMapTypeToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES "
          + "tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tWarehouseLocationMapType ADD CONSTRAINT "
          + "WarehouseLocationMapTypeToLocationMapOnWarehouseLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) "
          + "REFERENCES tLocationMap(LocationMapRefno)";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableProductLocationMapType()
    {
      string sqlTable = "CREATE TABLE tProductLocationMapType"
          + "("
          + "ProductRefno int NOT NULL,"
          + "WarehouseLocationMapRefno int NOT NULL,"
          + "ProductLocationMapTypeEnum int NOT NULL,"
          + "LocationMapRefno int NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "PK_ProductLocationMapTypeOnProductRefnoAndLocationMapRefnoAndProductLocationMapTypeEnum ON "
          + "tProductLocationMapType(ProductRefno, LocationMapRefno, ProductLocationMapTypeEnum)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "FK_ProductLocationMapTypeOnWarehouseLocationMapRefnoAndProductRefnoAndProductLocationMapTypeEnum ON "
          + "tProductLocationMapType(WarehouseLocationMapRefno, ProductRefno, ProductLocationMapTypeEnum)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ProductLocationMapTypeOnProductRefno ON "
          + "tProductLocationMapType(ProductRefno)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ProductLocationMapTypeOnWarehouseLocationMapRefno ON "
          + "tProductLocationMapType(WarehouseLocationMapRefno)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ProductLocationMapTypeOnLocationMapRefno ON "
          + "tProductLocationMapType(LocationMapRefno)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tProductLocationMapType ADD CONSTRAINT "
          + "ProductLocationMapTypeToProductOnProductRefno FOREIGN KEY (ProductRefno) REFERENCES tProduct(ProductRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tProductLocationMapType ADD CONSTRAINT "
          + "ProductLocationMapTypeToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES "
          + "tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tProductLocationMapType ADD CONSTRAINT "
          + "ProductLocationMapTypeToLocationMapOnWarehouseLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) "
          + "REFERENCES tLocationMap(LocationMapRefno)";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableContact()
    {
      string sqlTable = "CREATE TABLE tContact"
          + "("
          + "ContactRefno int NOT NULL IDENTITY(1, 1) CONSTRAINT PK_ContactOnContactRefno PRIMARY KEY,"
          + "ContactSubType int NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactOnContactRefnoAndContactSubType ON "
          + "tContact(ContactRefno ASC, ContactSubType ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableContactUser()
    {
      string sqlTable = "CREATE TABLE tContactUser"
          + "("
          + "ContactRefno int NOT NULL CONSTRAINT PK_UserOnContactRefno PRIMARY KEY,"
          + "ContactSubType int NOT NULL,"
          + "ContactUserCode int NOT NULL,"
          + "ContactUserName nvarchar(20),"
          + "ContactUserPassword nvarchar(6),"
          + "ContactUserIsAllowedAssociateProductBarCode bit NOT NULL,"
          + "ContactUserIsAllowedPickingModuleOverridePicker bit NOT NULL,"
          + "ContactUserIsAllowedPickingModuleOverrideAddManualDetail bit NOT NULL,"
          + "ContactUserLocalizationLanguage nvarchar(3)"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactUserOnContactRefnoAndContactSubType ON "
          + "tContactUser(ContactRefno ASC, ContactSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactUserOnContactUserCode ON tContactUser(ContactUserCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactUserOnContactUserName ON tContactUser(ContactUserName ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tContactUser ADD CONSTRAINT ContactUserToContactOnContactRefno FOREIGN KEY "
          + "(ContactRefno) REFERENCES tContact(ContactRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableContactCustomer()
    {
      string sqlTable = "CREATE TABLE tContactCustomer"
          + "("
          + "ContactRefno int NOT NULL CONSTRAINT PK_UserOnContactRefno PRIMARY KEY,"
          + "ContactSubType int NOT NULL,"
          + "ContactCustomerFKRefno int NOT NULL,"
          + "ContactCustomerCode nvarchar(8) NOT NULL,"
          + "ContactCustomerName nvarchar(40) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactCustomerOnContactRefnoAndContactSubType ON "
          + "tContactCustomer(ContactRefno ASC, ContactSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactCustomerOnContactCustomerFKRefno ON "
          + "tContactCustomer(ContactCustomerFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactCustomerOnContactCustomerCode ON "
          + "tContactCustomer(ContactCustomerCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactCustomerOnContactCustomerName ON "
          + "tContactCustomer(ContactCustomerName ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tContactCustomer ADD CONSTRAINT ContactCustomerToContactOnContactRefno "
          + "FOREIGN KEY (ContactRefno) REFERENCES tContact(ContactRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableContactCustomerShipTo()
    {
      string sqlTable = "CREATE TABLE tContactCustomerShipTo"
          + "("
          + "ContactRefno int NOT NULL CONSTRAINT PK_UserOnContactRefno PRIMARY KEY,"
          + "ContactSubType int NOT NULL,"
          + "CustomerContactRefno int NOT NULL,"
          + "ContactCustomerShipToFKRefno int NOT NULL,"
          + "ContactCustomerShipToCode nvarchar(8) NOT NULL,"
          + "ContactCustomerShipToName nvarchar(40) NOT NULL" + ")";

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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactCustomerShipToOnContactRefnoAndContactSubType ON "
          + "tContactCustomerShipTo(ContactRefno ASC, ContactSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactCustomerShipToOnContactCustomerShipToFKRefno ON "
          + "tContactCustomerShipTo(ContactCustomerShipToFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactCustomerShipToOnContactCustomerShipToCode ON "
          + "tContactCustomerShipTo(ContactCustomerShipToCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactCustomerShipToOnContactCustomerShipToName ON "
          + "tContactCustomerShipTo(ContactCustomerShipToName ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tContactCustomerShipTo ADD CONSTRAINT "
          + "ContactCustomerShipToContactOnContactRefno FOREIGN KEY (ContactRefno) REFERENCES tContact(ContactRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tContactCustomerShipTo ADD CONSTRAINT "
          + "ContactCustomerShipToContactCustomerOnCustomerContactRefno FOREIGN KEY (CustomerContactRefno) REFERENCES "
          + "tContactCustomer(ContactRefno)";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableContactVendor()
    {
      string sqlTable = "CREATE TABLE tContactVendor"
          + "("
          + "ContactRefno int NOT NULL CONSTRAINT PK_UserOnContactRefno PRIMARY KEY,"
          + "ContactSubType int NOT NULL,"
          + "ContactVendorFKRefno int NOT NULL,"
          + "ContactVendorCode nvarchar(8) NOT NULL,"
          + "ContactVendorName nvarchar(40) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactVendorOnContactRefnoAndContactSubType ON "
          + "tContactVendor(ContactRefno ASC, ContactSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactVendorOnContactVendorFKRefno ON "
          + "tContactVendor(ContactVendorFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactVendorOnContactVendorCode ON "
          + "tContactVendor(ContactVendorCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactVendorOnContactVendorName ON "
          + "tContactVendor(ContactVendorName ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tContactVendor ADD CONSTRAINT ContactVendorToContactOnContactRefno FOREIGN KEY "
          + "(ContactRefno) REFERENCES tContact(ContactRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableContactVendorSupplier()
    {
      string sqlTable = "CREATE TABLE tContactVendorSupplier"
          + "("
          + "ContactRefno int NOT NULL CONSTRAINT PK_UserOnContactRefno PRIMARY KEY,"
          + "ContactSubType int NOT NULL,"
          + "VendorContactRefno int NOT NULL,"
          + "ContactVendorSupplierFKRefno int NOT NULL,"
          + "ContactVendorSupplierCode nvarchar(8) NOT NULL,"
          + "ContactVendorSupplierName nvarchar(40) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactVendorSupplierOnContactRefnoAndContactSubType ON "
          + "tContactVendorSupplier(ContactRefno ASC, ContactSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactVendorSupplierOnContactVendorSupplierFKRefno ON "
          + "tContactVendorSupplier(ContactVendorSupplierFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactVendorSupplierOnContactVendorSupplierCode ON "
          + "tContactVendorSupplier(ContactVendorSupplierCode ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ContactVendorSupplierOnContactVendorSupplierName ON "
          + "tContactVendorSupplier(ContactVendorSupplierName ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tContactVendorSupplier ADD CONSTRAINT "
          + "ContactVendorSupplierToContactOnContactRefno FOREIGN KEY (ContactRefno) REFERENCES tContact(ContactRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tContactVendorSupplier ADD CONSTRAINT "
          + "ContactVendorSupplierToVendorContactOnVendorContactRefno FOREIGN KEY (VendorContactRefno) REFERENCES "
          + "tContactVendor(ContactRefno)";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableReasonCode()
    {
      string sqlTable = "CREATE TABLE tReasonCode"
          + "("
          + "ReasonCodeRefno int NOT NULL IDENTITY(1, 1) CONSTRAINT PK_ReasonCodeOnReasonCodeRefno PRIMARY KEY,"
          + "ReasonCodeFKRefno int NOT NULL,"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityOnReasonCodeFKRefno ON "
          + "tReasonCode(ReasonCodeFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityOnReasonCodeCode ON tReasonCode(ReasonCodeCode ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemReasonCodeXRef()
    {
      string sqlTable = "CREATE TABLE tActivityItemReasonCodeXRef"
          + "("
          + "ActivityItemRefno int NOT NULL,"
          + "ReasonCodeRefno int NOT NULL "
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "PK_ActivityItemReasonCodeXRefOnActivityItemRefnoAndReasonCodeRefno ON "
          + "tActivityItemReasonCodeXRef(ActivityItemRefno ASC, ReasonCodeRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityItemReasonCodeXRefOnActivityItemRefno ON "
          + "tActivityItemReasonCodeXRef(ActivityItemRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityItemReasonCodeXRefOnReasonCodeRefno ON "
          + "tActivityItemReasonCodeXRef(ReasonCodeRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityItemReasonCodeXRef ADD CONSTRAINT "
          + "ActivityItemReasonCodeXRefTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
          + "tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemReasonCodeXRef ADD CONSTRAINT "
          + "ActivityItemReasonCodeXRefToReasonCodeOnReasonCodeRefno FOREIGN KEY (ReasonCodeRefno) REFERENCES "
          + "tReasonCode(ReasonCodeRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemConvertUnit()
    {
      string sqlTable = "CREATE TABLE tActivityItemConvertUnit"
          + "("
          + "ActivityItemRefno int NOT NULL CONSTRAINT PK_ActivityItemConvertUnitOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType int NOT NULL,"
          + "ActivityItemConvertUnitType int NOT NULL,"
          + "ProductUnitRefno int NOT NULL"
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
      string sqlTable = "CREATE TABLE tActivityDetailConvertUnit"
          + "("
          + "ActivityDetailRefno int NOT NULL CONSTRAINT PK_ActivityDetailConvertUnitOnActivityDetailRefno PRIMARY KEY,"
          + "ActivityDetailConvertUnitFKRefno int NOT NULL "
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
      string sqlTable = "CREATE TABLE tActivityItemMoveInventory"
          + "("
          + "ActivityItemRefno int NOT NULL CONSTRAINT PK_ActivityItemMoveInventoryOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType int NOT NULL,"
          + "ActivityItemMoveInventoryType int NOT NULL,"
          + "ProductUnitRefno int NOT NULL"
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
      string sqlTable = "CREATE TABLE tActivityDetailMoveInventory"
          + "("
          + "ActivityDetailRefno int NOT NULL CONSTRAINT PK_ActivityDetailMoveInventoryOnActivityDetailRefno PRIMARY KEY,"
          + "ActivityDetailMoveInventoryFKRefno int NOT NULL "
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
      string sqlTable = "CREATE TABLE tActivityAdjustment"
          + "("
          + "ActivityRefno int NOT NULL CONSTRAINT PK_ActivityAdjustmentOnActivityRefno PRIMARY KEY,"
          + "ActivitySubType int NOT NULL,"
          + "ActivityAdjustmentFKRefno int NOT NULL,"
          + "ActivityAdjustmentStringFKRefno nvarchar(8) NOT NULL,"
          + "ActivityAdjustmentType int NOT NULL,"
          + "WarehouseLocationMapRefno int NOT NULL,"
          + "ActivityAdjustmentDateTime datetime NOT NULL,"
          + "ActivityAdjustmentDescription nvarchar(40) NOT NULL,"
          + "ActivityAdjustmentTypeDescription nvarchar(20) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityAdjustmentOnActivityRefnoAndActivitySubType ON "
          + "tActivityAdjustment(ActivityRefno ASC, ActivitySubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityAdjustmentOnActivityAdjustmentFKRefno ON "
          + "tActivityAdjustment(ActivityAdjustmentFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityAdjustmentOnWarehouseLocationMapRefno ON "
          + "tActivityAdjustment(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityAdjustment ADD CONSTRAINT "
          + "ActivityAdjustmentToActivityOnActivityRefnoAndActivitySubType FOREIGN KEY (ActivityRefno) REFERENCES "
          + "tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityAdjustment ADD CONSTRAINT "
          + "ActivityAdjustmentToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno FOREIGN KEY "
          + "(WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemAdjustment()
    {
      string sqlTable = "CREATE TABLE tActivityItemAdjustment"
          + "("
          + "ActivityItemRefno int NOT NULL CONSTRAINT PK_ActivityItemAdjustmentOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType int NOT NULL,"
          + "ActivityItemAdjustmentFKRefno int NOT NULL,"
          + "ActivityItemAdjustmentStringFKRefno nvarchar(8) NOT NULL,"
          + "ProductUnitRefno int NOT NULL,"
          + "WarehouseLocationMapRefno int NOT NULL,"
          + "ActivityItemAdjustmentQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemAdjustmentBaseQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemAdjustmentWeightQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemAdjustmentDescription nvarchar(42) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "UQ_ActivityItemAdjustmentOnActivityItemRefnoAndActivityItemSubType ON "
          + "tActivityItemAdjustment(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityItemAdjustmentOnActivityItemAdjustmentFKRefno ON "
          + "tActivityItemAdjustment(ActivityItemAdjustmentFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityItemAdjustmentOnWarehouseLocationMapRefno ON "
          + "tActivityItemAdjustment(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityItemAdjustment ADD CONSTRAINT "
          + "ActivityItemAdjustmentTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
          + "tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemAdjustment ADD CONSTRAINT "
          + "ActivityItemAdjustmentToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY "
          + "(WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemAdjustment ADD CONSTRAINT "
          + "ActivityItemAdjustmentTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES "
          + "tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailAdjustment()
    {
      string sqlTable = "CREATE TABLE tActivityDetailAdjustment"
          + "("
          + "ActivityDetailRefno int NOT NULL CONSTRAINT PK_ActivityDetailAdjustmentOnActivityDetailRefno PRIMARY KEY,"
          + "ActivityDetailAdjustmentFKRefno int NOT NULL "
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "UQ_ActivityDetailAdjustmentOnActivityDetailAdjustmentFKRefno ON "
          + "tActivityDetailAdjustment(ActivityDetailAdjustmentFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityDetailAdjustment ADD CONSTRAINT "
          + "ActivityDetailAdjustmentTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES "
          + "tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityProduction()
    {
      string sqlTable = "CREATE TABLE tActivityProduction"
          + "("
          + "ActivityRefno int NOT NULL CONSTRAINT PK_ActivityProductionOnActivityRefno PRIMARY KEY,"
          + "ActivitySubType int NOT NULL,"
          + "WarehouseLocationMapRefno int NOT NULL,"
          + "ActivityProductionDateTime datetime NOT NULL,"
          + "ActivityProductionDescription nvarchar(25) NOT NULL "
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityProductionOnActivityRefnoAndActivitySubType ON "
          + "tActivityProduction(ActivityRefno ASC, ActivitySubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityProductionOnWarehouseLocationMapRefno ON "
          + "tActivityProduction(WarehouseLocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityProduction ADD CONSTRAINT ActivityProductionToActivityOnActivityRefno "
          + "FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityProduction ADD CONSTRAINT "
          + "ActivityProductionToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno "
          + "FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) "
          + "ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityProductionFinished()
    {
      string sqlTable = "CREATE TABLE tActivityProductionFinished"
          + "("
          + "ActivityRefno int NOT NULL,"
          + "ActivityProductionFKRefno int NOT NULL,"
          + "ActivityProductionStringFKRefno nvarchar(8) NOT NULL,"
          + "ActivityProductionFinishedDefaultLogisticUnitBatchLotNumber nvarchar(20) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX PK_ActivityProductionFinishedOnActivityRefno ON "
          + "tActivityProductionFinished(ActivityRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityProductionFinishedOnActivityProductionFKRefno ON "
          + "tActivityProductionFinished(ActivityProductionFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityProductionFinished ADD CONSTRAINT "
          + "ActivityProductionFinishedToActivityProductionOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES "
          + "tActivityProduction(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityProductionMaterial()
    {
      string sqlTable = "CREATE TABLE tActivityProductionMaterial"
          + "("
          + "ActivityRefno int NOT NULL,"
          + "ActivityProductionFKRefno int NOT NULL,"
          + "ActivityProductionStringFKRefno nvarchar(8) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX PK_ActivityProductionMaterialOnActivityRefno ON "
          + "tActivityProductionMaterial(ActivityRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityProductionMaterialOnActivityProductionFKRefno ON "
          + "tActivityProductionMaterial(ActivityProductionFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityProductionMaterial ADD CONSTRAINT "
          + "ActivityProductionMaterialToActivityProductionOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES "
          + "tActivityProduction(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityProductionByProduct()
    {
      string sqlTable = "CREATE TABLE tActivityProductionByProduct"
          + "("
          + "ActivityRefno int NOT NULL,"
          + "ActivityProductionFKRefno int NOT NULL,"
          + "ActivityProductionStringFKRefno nvarchar(8) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX PK_ActivityProductionByProductOnActivityRefno ON "
          + "tActivityProductionByProduct(ActivityRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityProductionByProductOnActivityProductionFKRefno ON "
          + "tActivityProductionByProduct(ActivityProductionFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityProductionByProduct ADD CONSTRAINT "
          + "ActivityProductionByProductToActivityProductionOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES "
          + "tActivityProduction(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemProduction()
    {
      string sqlTable = "CREATE TABLE tActivityItemProduction"
          + "("
          + "ActivityItemRefno int NOT NULL CONSTRAINT PK_ActivityItemProductionOnActivityItemRefno PRIMARY KEY,"
          + "ActivityItemSubType int NOT NULL,"
          + "ProductUnitRefno int NOT NULL,"
          + "LocationMapRefno int NOT NULL,"
          + "ActivityItemProductionQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemProductionBaseQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemProductionWeightQuantity numeric(11, 3) NOT NULL,"
          + "ActivityItemProductionDescription nvarchar(42) NOT NULL,"
          + "ActivityItemProductionQuantityExpected numeric(11, 3) NOT NULL,"
          + "ActivityItemProductionBaseQuantityExpected numeric(11, 3) NOT NULL,"
          + "ActivityItemProductionWeightQuantityExpected numeric(11, 3) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "UQ_ActivityItemProductionOnActivityItemRefnoAndActivityItemSubType ON "
          + "tActivityItemProduction(ActivityItemRefno ASC, ActivityItemSubType ASC)";
      this.RunDDLCommand(sqlNdx);

      sqlNdx = "CREATE NONCLUSTERED INDEX FK_ActivityItemProductionOnLocationMapRefno ON "
          + "tActivityItemProduction(LocationMapRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityItemProduction ADD CONSTRAINT "
          + "ActivityItemProductionTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
          + "tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemProduction ADD CONSTRAINT "
          + "ActivityItemProductionToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES "
          + "tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);

      alterTable = "ALTER TABLE tActivityItemProduction ADD CONSTRAINT "
          + "ActivityItemProductionTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES "
          + "tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemProductionFinished()
    {
      string sqlTable = "CREATE TABLE tActivityItemProductionFinished"
          + "("
          + "ActivityItemRefno int NOT NULL CONSTRAINT PK_ActivityItemProductionFinishedOnActivityItemRefno "
          + "PRIMARY KEY,"
          + "ActivityItemProductionFinishedFKRefno int NOT NULL,"
          + "ActivityItemProductionFinishedStringFKRefno nvarchar(8) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "UQ_ActivityItemProductionFinishedOnActivityItemProductionFinishedFKRefno ON "
          + "tActivityItemProductionFinished(ActivityItemProductionFinishedFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityItemProductionFinished ADD CONSTRAINT "
          + "ActivityItemProductionFinishedTactItemProductionOnActivityRefno FOREIGN KEY (ActivityItemRefno) "
          + "REFERENCES tActivityItemProduction(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemProductionMaterial()
    {
      string sqlTable = "CREATE TABLE tActivityItemProductionMaterial"
          + "("
          + "ActivityItemRefno int NOT NULL CONSTRAINT PK_ActivityItemProductionMaterialOnActivityItemRefno "
          + "PRIMARY KEY,"
          + "ActivityItemProductionMaterialFKRefno int NOT NULL,"
          + "ActivityItemProductionMaterialStringFKRefno nvarchar(8) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "UQ_ActivityItemProductionMaterialOnActivityItemProductionMaterialFKRefno ON "
          + "tActivityItemProductionMaterial(ActivityItemProductionMaterialFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityItemProductionMaterial ADD CONSTRAINT "
          + "ActivityItemProductionMaterialTactItemProductionOnActivityRefno FOREIGN KEY (ActivityItemRefno) "
          + "REFERENCES tActivityItemProduction(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityItemProductionByProduct()
    {
      string sqlTable = "CREATE TABLE tActivityItemProductionByProduct"
          + "("
          + "ActivityItemRefno int NOT NULL CONSTRAINT PK_ActivityItemProductionByProductOnActivityItemRefno "
          + "PRIMARY KEY,"
          + "ActivityItemProductionByProductFKRefno int NOT NULL,"
          + "ActivityItemProductionByProductStringFKRefno nvarchar(8) NOT NULL"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "UQ_ActivityItemProductionByProductOnActivityItemProductionByProductFKRefno ON "
          + "tActivityItemProductionByProduct(ActivityItemProductionByProductFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityItemProductionByProduct ADD CONSTRAINT "
          + "ActivityItemProductionByProductTactItemProductionOnActivityRefno FOREIGN KEY (ActivityItemRefno) "
          + "REFERENCES tActivityItemProduction(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailProductionFinished()
    {
      string sqlTable = "CREATE TABLE tActivityDetailProductionFinished"
          + "("
          + "ActivityDetailRefno int NOT NULL CONSTRAINT PK_ActivityDetailProductionFinishedOnActivityDetailRefno "
          + "PRIMARY KEY,"
          + "ActivityDetailProductionFinishedFKRefno int NOT NULL "
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "UQ_ActivityDetailProductionFinishedOnActivityDetailProductionFinishedFKRefno ON "
          + "tActivityDetailProductionFinished(ActivityDetailProductionFinishedFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityDetailProductionFinished ADD CONSTRAINT "
          + "ActivityDetailProductionFinishedTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) "
          + "REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailProductionMaterial()
    {
      string sqlTable = "CREATE TABLE tActivityDetailProductionMaterial"
          + "("
          + "ActivityDetailRefno int NOT NULL CONSTRAINT PK_ActivityDetailProductionMaterialOnActivityDetailRefno "
          + "PRIMARY KEY,"
          + "ActivityDetailProductionMaterialFKRefno int NOT NULL "
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "UQ_ActivityDetailProductionMaterialOnActivityDetailProductionMaterialFKRefno ON "
          + "tActivityDetailProductionMaterial(ActivityDetailProductionMaterialFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityDetailProductionMaterial ADD CONSTRAINT "
          + "ActivityDetailProductionMaterialTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) "
          + "REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableActivityDetailProductionByProduct()
    {
      string sqlTable = "CREATE TABLE tActivityDetailProductionByProduct"
          + "("
          + "ActivityDetailRefno int NOT NULL CONSTRAINT PK_ActivityDetailProductionByProductOnActivityDetailRefno "
          + "PRIMARY KEY,"
          + "ActivityDetailProductionByProductFKRefno int NOT NULL "
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
          + "UQ_ActivityDetailProductionByProductOnActivityDetailProductionByProductFKRefno ON "
          + "tActivityDetailProductionByProduct(ActivityDetailProductionByProductFKRefno ASC)";
      this.RunDDLCommand(sqlNdx);

      string alterTable = "ALTER TABLE tActivityDetailProductionByProduct ADD CONSTRAINT "
          + "ActivityDetailProductionByProductTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) "
          + "REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
      this.RunDDLCommand(alterTable);
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    private void CreateTableFamily()
    {
      string sqlTable = "CREATE TABLE tFamily"
          + "("
          + "FamilyRefno int NOT NULL IDENTITY(1, 1) CONSTRAINT PK_FamilyOnFamilyRefno PRIMARY KEY,"
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

      string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_FamilyOnFamilyCode ON tFamily(FamilyCode ASC)";
      this.RunDDLCommand(sqlNdx);
    }

    #endregion
  }
}
