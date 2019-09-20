namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableActivityPurchaseOrderWorkSheetItem : TableBase
  {
    #region Constructors

    public TableActivityPurchaseOrderWorkSheetItem()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityPurchaseOrderWorkSheetItem";
    }

    #endregion

    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Request(int actRefno)
    {
      string reqData = string.Empty;

      GC.Collect();
      GC.WaitForPendingFinalizers();

      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWorkSheetItems), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding work sheet items.", true);
#endif

        reqData = this.Send(actRefno);
        if (0 < reqData.Length)
        {
          if (reqData.Contains("<message>ERROR</message>"))
          {
            XmlDocument reqErr = new XmlDocument();
            reqErr.LoadXml(reqData);
            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
          }

          this.Save(reqData, actRefno);
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWorkSheetItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding work sheet items complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWorkSheetItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding work sheet items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWorkSheetItemsFailed), ex);
#else
        throw new Exception("Seeding work sheet items failed.", ex);
#endif
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override int DataStatus()
    {
      int rowsAffected = 0;

      try
      {
        using (SQLiteCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndStatus.CommandText = "SELECT COUNT(tActivityItem.ActivityItemRefno)" 
              + " FROM tActivityItem" 
              + " WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.PurchaseOrderWorkSheet;
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.WorkSheetItems), rowsAffected), 
#else
              string.Format("{0} work sheet items.", rowsAffected), 
#endif
              true);
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredWorkSheetStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Work sheet status failed.", ex);
#endif
      }

      return rowsAffected;
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Purge()
    {
      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingWorkSheetItems), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging work sheet items.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef " 
              + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN " 
              + "(SELECT tActivityItem.ActivityItemRefno " 
              + "FROM tActivityItem " 
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.PurchaseOrderWorkSheet + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItemPurchaseOrderWorkSheet " 
              + "WHERE tActivityItemPurchaseOrderWorkSheet.ActivityItemSubType = " 
              + (int)SysCommon.ActSubTypes.PurchaseOrderWorkSheet;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItem " 
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.PurchaseOrderWorkSheet;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingWorkSheetItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging work sheet items complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingWorkSheetItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging work sheet items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingWorkSheetItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging work sheet items failed.", ex);
#endif
      }
    }

    #endregion

    #region Private Members

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Send(int actRefno)
    {
      int actWorkSheetFKRefno = 0;
      return this.SendComm(this.Create(actWorkSheetFKRefno));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="reqData"></param>
    /// <remarks></remarks>
    protected override void Save(string reqData, int actRefno)
    {
//      SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
//      string errMsg = string.Empty;

//      try
//      {
//        using (DataSet ds = new DataSet())
//        {
//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.ReadingReceivedData), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
//#endif
//          ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingWorkSheetItems), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving work sheet items.", false);
//#endif
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tActivityItemPurchaseOrderWorkSheet"].Rows.Count);

//#if MULTILANGUAGE
//            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), false);
//#else
//            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);
//#endif

//            using (SQLiteCommand cmndWorkSheetItem = conn.CreateCommand())
//            {
//              cmndWorkSheetItem.CommandText = "tActivityItemPurchaseOrderWorkSheet";
//              cmndWorkSheetItem.CommandType = CommandType.TableDirect;
//              cmndWorkSheetItem.IndexName 
//                  = "UQ_ActivityItemPurchaseOrderWorkSheetOnActivityItemPurchaseOrderWorkSheetFKRefno";

//              using (SQLiteCommand cmndItem = conn.CreateCommand())
//              {
//                cmndItem.CommandText = "tActivityItem";
//                cmndItem.CommandType = CommandType.TableDirect;

//                using (SqlCeResultSet rsItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
//                {
//                  SqlCeUpdatableRecord recItem = rsItem.CreateRecord();

//                  using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
//                  {
//                    cmndProdUnit.CommandText = "tProductUnit";
//                    cmndProdUnit.CommandType = CommandType.TableDirect;
//                    cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

//                    using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
//                    {
//                      using (SQLiteCommand cmndProd = conn.CreateCommand())
//                      {
//                        cmndProd.CommandText = "tProduct";
//                        cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
//                        cmndProd.CommandType = CommandType.TableDirect;

//                        using (SQLiteDataReader drProd = cmndProd.ExecuteReader())
//                        {
//                          using (SQLiteCommand cmndUnit = conn.CreateCommand())
//                          {
//                            cmndUnit.CommandText = "tUnit";
//                            cmndUnit.IndexName = "UQ_UnitOnUnitCode";
//                            cmndUnit.CommandType = CommandType.TableDirect;

//                            using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
//                            {
//                              using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
//                              {
//                                cmndLocMapWhs.CommandText = "tLocationMapWarehouse";
//                                cmndLocMapWhs.CommandType = CommandType.TableDirect;
//                                cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

//                                using (SQLiteDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
//                                {
//                                  using (SQLiteCommand cmndActivity = conn.CreateCommand())
//                                  {
//                                    cmndActivity.CommandText = "tActivity";
//                                    cmndActivity.CommandType = CommandType.TableDirect;

//                                    using (SqlCeResultSet rsActivity 
//                                        = cmndActivity.ExecuteResultSet(ResultSetOptions.Updatable))
//                                    {
//                                      SqlCeUpdatableRecord recActivity = rsActivity.CreateRecord();

//                                      using (SQLiteCommand cmndID = conn.CreateCommand())
//                                      {
//                                        cmndID.CommandText = "SELECT @@IDENTITY";
//                                        cmndID.CommandType = CommandType.Text;

//                                        using (SqlCeResultSet rs 
//                                            = cmndWorkSheetItem.ExecuteResultSet(ResultSetOptions.Updatable))
//                                        {
//                                          SqlCeUpdatableRecord rec = rs.CreateRecord();
//                                          int actItemRefno = 0;
//                                          int progValue = 0;
//                                          int prodUnitRefno = 0;
//                                          int whsLocMapRefno = 0;
//                                          int actItemWorkSheetFKRefno = 0;

//                                          DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);
//                                          foreach (DataRow drow in ds.Tables["tActivityItemPurchaseOrderWorkSheet"].Rows)
//                                          {
//                                            errMsg = "Arguments:";

//                                            recActivity.SetInt32(
//                                                (int)SysCommon.ActFields.SubType, 
//                                                (int)SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
//                                            recActivity.SetInt32((int)SysCommon.ActFields.Status, 0);

//                                            rsActivity.Insert(recActivity);
                                            
//                                            actRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                                            recItem.SetInt32(
//                                                (int)SysCommon.ActItemFields.SubType,
//                                                (int)SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
//                                            recItem.SetInt32((int)SysCommon.ActItemFields.Status, 0);
//                                            recItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, actRefno);

//                                            rsItem.Insert(recItem);

//                                            actItemRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                                            prodUnitRefno = this.GetProductUnitRefnoFromDataRow(
//                                                drow, 
//                                                ref errMsg, 
//                                                drProd, 
//                                                drUnit, 
//                                                drProdUnit, 
//                                                false);
//                                            rec.SetInt32(
//                                                (int)SysCommon.ActItemWorkSheetFields.ProdUnitRefno, 
//                                                prodUnitRefno);

//                                            whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(
//                                                drow, 
//                                                ref errMsg, 
//                                                drLocMapWhs);
//                                            rec.SetInt32(
//                                                (int)SysCommon.ActItemWorkSheetFields.WhsLocMapRefno, 
//                                                whsLocMapRefno);

//                                            rec.SetInt32(
//                                                (int)SysCommon.ActItemWorkSheetFields.ActItemRefno, 
//                                                actItemRefno);
//                                            rec.SetInt32(
//                                                (int)SysCommon.ActItemWorkSheetFields.ActItemSubType, 
//                                                (int)SysCommon.ActSubTypes.PurchaseOrderWorkSheet);

//                                            actItemWorkSheetFKRefno
//                                                = Convert.ToInt32(drow["ActivityItemPurchaseOrderWorkSheetFKRefno"]);
//                                            rec.SetInt32(
//                                                (int)SysCommon.ActItemWorkSheetFields.FKRefno,
//                                                actItemWorkSheetFKRefno);
//                                            rec.SetString(
//                                                (int)SysCommon.ActItemWorkSheetFields.StringFKRefno,
//                                                actItemWorkSheetFKRefno.ToString());

//                                            rec.SetDecimal(
//                                                (int)SysCommon.ActItemWorkSheetFields.QuantityOrdered,
//                                                Convert.ToDecimal(
//                                                drow["ActivityItemPurchaseOrderWorkSheetQuantityOrdered"]));
//                                            rec.SetDecimal(
//                                                (int)SysCommon.ActItemWorkSheetFields.BaseQuantityOrdered,
//                                                Convert.ToDecimal(
//                                                drow["ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered"]));
//                                            rec.SetDecimal(
//                                                (int)SysCommon.ActItemWorkSheetFields.WeightQuantityOrdered,
//                                                Convert.ToDecimal(
//                                                drow["ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered"]));
//                                            rs.Insert(rec);

//                                            progValue++;
//                                            if (progValue % 100 == 0)
//                                            {
//                                              this.DataAccessProgressOutHandler(
//                                                  this, 
//                                                  EventArgs.Empty, 
//                                                  progValue, 
//                                                  0, 
//                                                  progMax);
//                                            }
//                                          }

//                                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
//                                        }
//                                      }
//                                    }
//                                  }

//                                  drLocMapWhs.Close();
//                                }
//                              }

//                              drUnit.Close();
//                            }
//                          }

//                          drProd.Close();
//                        }
//                      }

//                      drProdUnit.Close();
//                    }
//                  }
//                }
//              }
//            }
//          }

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingWorkSheetItemsComplete), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving work sheet items complete.", false);
//#endif
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingWorkSheetItemsFailed), false);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving work sheet items failed.", false);
//#endif

//        // Duplicate key violations
//        if (ex.NativeError == 25016)
//        {
//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation), false);
//          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation) + Environment.NewLine + errMsg, ex);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
//          throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
//#endif
//        }

//        throw new Exception(
//#if MULTILANGUAGE
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingWorkSheetItemsFailed) + Environment.NewLine + errMsg,
//#else
//            "Database exception occurred. Saving work sheet items failed." + Environment.NewLine + errMsg,
//#endif
//            ex);
//      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Create(int actWorkSheetFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>"
          + "<action>REQUEST PURCHASE ORDER WORK SHEET ITEMS</action>" 
          + SysCommon.GetRequestDefaults(false)
          + ("<refno>" + actWorkSheetFKRefno + "</refno>") 
          + "</command>"
          + ("<whs_id>" + LocationMap.GetInstance().CurrLocMap.WhsFKRefno + "</whs_id>") 
          + "</imclient>");

      request.Save(xml);
      xml.Flush();
      xml.Close();

      return xmlString.ToString();
    }

    #endregion
  }
}
