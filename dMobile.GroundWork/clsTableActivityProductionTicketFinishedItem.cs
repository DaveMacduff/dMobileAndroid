namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableActivityProductionTicketFinishedItem : TableBase
  {
    #region Constructors

    public TableActivityProductionTicketFinishedItem()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketFinishedItem";
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

      //GC.Collect();
      //GC.WaitForPendingFinalizers();

      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingFinishedTicketItems), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket items.", true);
#endif

        if (actRefno == 0)
        {
          using (SQLiteCommand cmndActivity = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndActivity.CommandType = CommandType.Text;
            cmndActivity.CommandText = "SELECT tActivity.ActivityRefno "
                + "FROM tActivity "
                + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketFinished;

            using (SQLiteDataReader drActivity = cmndActivity.ExecuteReader())
            {
              while (drActivity.Read())
              {
                actRefno = Convert.ToInt32(drActivity["ActivityRefno"]);
                reqData = this.Send(actRefno);
                if (0 < reqData.Length)
                {
                  if (reqData.Contains("<message>ERROR</message>"))
                  {
                    XmlDocument reqErr = new XmlDocument();
                    reqErr.LoadXml(reqData);
                    new Exception(reqErr.DocumentElement["detail"].InnerText);
                  }

                  this.Save(reqData, actRefno);
                }
              }

              drActivity.Close();
            }
          }
        }
        else
        {
          reqData = this.Send(actRefno);
          if (0 < reqData.Length)
          {
            if (reqData.Contains("<message>ERROR</message>"))
            {
              XmlDocument reqErr = new XmlDocument();
              reqErr.LoadXml(reqData);
              new Exception(reqErr.DocumentElement["detail"].InnerText);
            }

            this.Save(reqData, actRefno);
          }
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingFinishedTicketItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket items complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingFinishedTicketItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingFinishedTicketItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding finished ticket items failed.", ex);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingFinishedTicketItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingFinishedTicketItemsFailed), ex);
#else
        throw new Exception("Seeding finished ticket items failed.", ex);
#endif
      }
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingFinishedTicketItems), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket items.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef"
              + " WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN"
              + " (SELECT tActivityItem.ActivityItemRefno"
              + " FROM tActivityItem"
              + " WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TicketFinished + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItemProductionFinished"
              + " WHERE tActivityItemProductionFinished.ActivityItemRefno IN"
              + " (SELECT tActivityItem.ActivityItemRefno"
              + " FROM tActivityItem"
              + " WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TicketFinished + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItemProduction"
              + " WHERE tActivityItemProduction.ActivityItemRefno IN"
              + " (SELECT tActivityItem.ActivityItemRefno"
              + " FROM tActivityItem"
              + " WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TicketFinished + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItem "
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TicketFinished;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingFinishedTicketItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket items complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingFinishedTicketItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingFinishedTicketItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging finished ticket items failed.", ex);
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
              + " WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TicketFinished;
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this,
              EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.FinishedTicketItems), rowsAffected),
#else
              string.Format("{0} finished ticket items.", rowsAffected),
#endif
              true);
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredFinishedTicketItemsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Finished ticket items status failed.", ex);
#endif
      }

      return rowsAffected;
    }

    #endregion

    #region Private Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actRefno"></param>
    /// <returns></returns>
    protected override string Send(int actRefno)
    {
      int actProductionFKRefno = 0;

//      using (SQLiteCommand cmndFinished = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//      {
//        cmndFinished.CommandText = "tActivityProductionFinished";
//        cmndFinished.CommandType = CommandType.TableDirect;
//        cmndFinished.IndexName = "PK_ActivityProductionFinishedOnActivityRefno";

//        using (SQLiteDataReader drFinished = cmndFinished.ExecuteReader())
//        {
//          if (drFinished.Seek(DbSeekOptions.FirstEqual, new object[] { actRefno }))
//          {
//            if (drFinished.Read())
//            {
//              actProductionFKRefno = drFinished.GetInt32((int)SysCommon.ActProdFinishedFields.FKRefno);
//            }
//            else
//            {
//#if MULTILANGUAGE
//              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrfinishedreadFailed));
//#else
//              throw new Exception("drFinished.Read() failed.");
//#endif
//            }
//          }
//          else
//          {
//            throw new Exception(
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheActivityProductionTicketFinishedRefnoIn)
//#else
//                string.Format("Unable to locate the activity production ticket finished refno {0} in "
//#endif
//                + "tActivityProductionFinished.", actRefno));
//          }
//        }

//      }

      return this.SendComm(this.Create(actProductionFKRefno));
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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingFinishedTicketItems), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished ticket items.", false);
//#endif
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tActivityItemProduction"].Rows.Count);

//#if MULTILANGUAGE
//            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), false);
//#else
//            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);
//#endif

//            using (SQLiteCommand cmndItemProd = conn.CreateCommand())
//            {
//              cmndItemProd.CommandText = "tActivityItemProduction";
//              cmndItemProd.CommandType = CommandType.TableDirect;
//              cmndItemProd.IndexName = "PK_ActivityItemProductionOnActivityItemRefno";

//              using (SQLiteCommand cmndItemFinished = conn.CreateCommand())
//              {
//                cmndItemFinished.CommandText = "tActivityItemProductionFinished";
//                cmndItemFinished.CommandType = CommandType.TableDirect;
//                cmndItemFinished.IndexName = "UQ_ActivityItemProductionFinishedOnActivityItemProductionFinishedFKRefno";

//                using (SQLiteCommand cmndItem = conn.CreateCommand())
//                {
//                  cmndItem.CommandText = "tActivityItem";
//                  cmndItem.CommandType = CommandType.TableDirect;

//                  using (SqlCeResultSet rsActItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
//                  {
//                    SqlCeUpdatableRecord recActItem = rsActItem.CreateRecord();

//                    using (SqlCeResultSet rsActItemFinished = cmndItemFinished.ExecuteResultSet(ResultSetOptions.Updatable))
//                    {
//                      SqlCeUpdatableRecord recActItemFinished = rsActItemFinished.CreateRecord();

//                      using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
//                      {
//                        cmndProdUnit.CommandText = "tProductUnit";
//                        cmndProdUnit.CommandType = CommandType.TableDirect;
//                        cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

//                        using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
//                        {
//                          using (SQLiteCommand cmndGetProduct = conn.CreateCommand())
//                          {
//                            cmndGetProduct.CommandText = "tProduct";
//                            cmndGetProduct.IndexName = "UQ_ProductOnProductFKRefno";
//                            cmndGetProduct.CommandType = CommandType.TableDirect;

//                            using (SQLiteDataReader drProd = cmndGetProduct.ExecuteReader())
//                            {
//                              using (SQLiteCommand cmndGetUnit = conn.CreateCommand())
//                              {
//                                cmndGetUnit.CommandText = "tUnit";
//                                cmndGetUnit.IndexName = "UQ_UnitOnUnitCode";
//                                cmndGetUnit.CommandType = CommandType.TableDirect;

//                                using (SQLiteDataReader drUnit = cmndGetUnit.ExecuteReader())
//                                {
//                                  using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
//                                  {
//                                    cmndLocMapWhs.CommandText = "tLocationMapWarehouse";
//                                    cmndLocMapWhs.CommandType = CommandType.TableDirect;
//                                    cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

//                                    using (SQLiteDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
//                                    {
//                                      using (SQLiteCommand cmndID = conn.CreateCommand())
//                                      {
//                                        cmndID.CommandText = "SELECT @@IDENTITY";
//                                        cmndID.CommandType = CommandType.Text;

//                                        using (SqlCeResultSet rsActItemProd
//                                            = cmndItemProd.ExecuteResultSet(ResultSetOptions.Updatable))
//                                        {
//                                          SqlCeUpdatableRecord recActItemProd = rsActItemProd.CreateRecord();

//                                          int actItemRefno = 0;
//                                          int progValue = 0;
//                                          int prodUnitRefno = 0;
//                                          int locMapRefno = 0;

//                                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

//                                          foreach (DataRow drow in ds.Tables["tActivityItemProduction"].Rows)
//                                          {
//                                            errMsg = "Arguments:";

//                                            recActItem.SetInt32(
//                                                (int)SysCommon.ActItemFields.SubType,
//                                                (int)SysCommon.ActSubTypes.TicketFinished);
//                                            recActItem.SetInt32(
//                                                (int)SysCommon.ActItemFields.Status,
//                                                (int)SysCommon.ActStatus.NoChange);
//                                            recActItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, actRefno);

//                                            rsActItem.Insert(recActItem);

//                                            actItemRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                                            prodUnitRefno = this.GetProductUnitRefnoFromDataRow(
//                                                drow,
//                                                ref errMsg,
//                                                drProd,
//                                                drUnit,
//                                                drProdUnit,
//                                                false);

//                                            recActItemProd.SetInt32(
//                                                (int)SysCommon.ActItemProdFields.ActItemRefno,
//                                                actItemRefno);
//                                            recActItemProd.SetInt32(
//                                                (int)SysCommon.ActItemProdFields.ActivityItemSubType,
//                                                (int)SysCommon.ActSubTypes.TicketFinished);
//                                            recActItemProd.SetInt32(
//                                                (int)SysCommon.ActItemProdFields.ProdUnitRefno,
//                                                prodUnitRefno);

//                                            locMapRefno = this.GetWhsLocMapRefnoFromDataRow(
//                                                drow,
//                                                ref errMsg,
//                                                drLocMapWhs);
//                                            recActItemProd.SetInt32(
//                                                (int)SysCommon.ActItemProdFields.LocMapRefno,
//                                                locMapRefno);

//                                            recActItemProd.SetDecimal(
//                                                (int)SysCommon.ActItemProdFields.Quantity,
//                                                Convert.ToDecimal(drow["ActivityItemProductionQuantity"]));
//                                            recActItemProd.SetDecimal(
//                                                (int)SysCommon.ActItemProdFields.BaseQuantity,
//                                                Convert.ToDecimal(drow["ActivityItemProductionBaseQuantity"]));
//                                            recActItemProd.SetDecimal(
//                                                (int)SysCommon.ActItemProdFields.WeightQuantity,
//                                                Convert.ToDecimal(drow["ActivityItemProductionWeightQuantity"]));
//                                            recActItemProd.SetString(
//                                                (int)SysCommon.ActItemProdFields.Description,
//                                                drow["ActivityItemProductionDescription"].ToString());
//                                            recActItemProd.SetDecimal(
//                                                (int)SysCommon.ActItemProdFields.QuantityExpected,
//                                                Convert.ToDecimal(drow["ActivityItemProductionQuantityExpected"]));
//                                            recActItemProd.SetDecimal(
//                                                (int)SysCommon.ActItemProdFields.BaseQuantityExpected,
//                                                Convert.ToDecimal(drow["ActivityItemProductionBaseQuantityExpected"]));
//                                            recActItemProd.SetDecimal(
//                                                (int)SysCommon.ActItemProdFields.WeightQuantityExpected,
//                                                Convert.ToDecimal(drow["ActivityItemProductionWeightQuantityExpected"]));

//                                            rsActItemProd.Insert(recActItemProd);

//                                            recActItemFinished.SetInt32(
//                                                (int)SysCommon.ActItemProdFinishedFields.ActItemRefno,
//                                                actItemRefno);

//                                            int actItemProductionFKRefno =
//                                                Convert.ToInt32(drow["ActivityItemProductionFKRefno"]);
//                                            errMsg += Environment.NewLine + "Item fk refno " + actItemProductionFKRefno;
//                                            recActItemFinished.SetInt32(
//                                              (int)SysCommon.ActItemProdFinishedFields.FKRefno,
//                                              actItemProductionFKRefno);
//                                            recActItemFinished.SetString(
//                                              (int)SysCommon.ActItemProdFinishedFields.StringFKRefno,
//                                              actItemProductionFKRefno.ToString());

//                                            rsActItemFinished.Insert(recActItemFinished);

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

//                                      drLocMapWhs.Close();
//                                    }
//                                  }

//                                  drUnit.Close();
//                                }
//                              }

//                              drProd.Close();
//                            }
//                          }

//                          drProdUnit.Close();
//                        }
//                      }
//                    }
//                  }
//                }
//              }
//            }
//          }

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingFinishedTicketItemsComplete), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished ticket items complete.", false);
//#endif
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingFinishedTicketItemsFailed), false);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished ticket items failed.", false);
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
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingFinishedTicketItemsFailed) + Environment.NewLine + errMsg,
//#else
//            "Database exception occurred. Saving finished ticket items failed." + Environment.NewLine + errMsg,
//#endif
//            ex);
//      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Create(int actProductionFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
          + "<imclient>" 
          + "<command>"
          + "<action>REQUEST PRODUCTION TICKET FINISHED ITEMS</action>" 
          + SysCommon.GetRequestDefaults(false)
          + ("<refno>" + actProductionFKRefno.ToString() + "</refno>")
          + "</command>"
          + "</imclient>");

      request.Save(xml);
      xml.Flush();
      xml.Close();

      return xmlString.ToString();
    }

    #endregion
  }
}
