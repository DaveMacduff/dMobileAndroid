namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using Microsoft.VisualBasic;

  public class TableActivityPickingItem : TableBase
  {
    #region Constructors

    public TableActivityPickingItem()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityPickingItem";
    }

    #endregion

    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void RequestData(int actRefno, XDocument xmlCriteria)
    {
      string reqData = string.Empty;

      GC.Collect();
      GC.WaitForPendingFinalizers();

      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesOrderItems), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order items.", true);
#endif

        if (actRefno == 0)
        {
          using (SQLiteCommand cmndPicking = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndPicking.CommandType = CommandType.Text;
            cmndPicking.CommandText = "SELECT tActivity.ActivityRefno " 
                + "FROM tActivity " 
                + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.Picking;

            using (SQLiteDataReader drPicking = cmndPicking.ExecuteReader())
            {
              while (drPicking.Read())
              {
                actRefno = Convert.ToInt32(drPicking["ActivityRefno"]);
                reqData = this.SendRequest(actRefno, xmlCriteria);
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
              }

              drPicking.Close();
            }
          }
        }
        else
        {
          reqData = this.SendRequest(actRefno, xmlCriteria);
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
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesOrderItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order items complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesOrderItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingSalesOrderItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding sales order items failed.", ex);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesOrderItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesOrderItemsFailed), ex);
#else
        throw new Exception("Seeding sales order items failed.", ex);
#endif
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Purge()
    {
      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesOrderItems), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order items.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef "
              + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
              + "(SELECT tActivityItem.ActivityItemRefno "
              + "FROM tActivityItem "
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Picking + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItemPicking "
              + "WHERE tActivityItemPicking.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Picking;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItem "
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Picking;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesOrderItemsCompleted), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order items completed.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesOrderItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingSalesOrderItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging sales order items failed.", ex);
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
          cmndStatus.CommandText = "SELECT COUNT(tActivityItem.ActivityItemRefno) " 
              + "FROM tActivityItem " 
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Picking;
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.SalesOrderItems), rowsAffected),
#else
              string.Format("{0} Sales order items.", rowsAffected),
#endif
              true);
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSalesOrderItemsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Sales order items status failed.", ex);
#endif
      }

      return rowsAffected;
    }

    #endregion

    #region Private Members

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string SendRequest(int actRefno, XDocument xmlCriteria)
    {
      int actPickFKRefno = 0;

//      using (SQLiteCommand cmndPicking = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//      {
//        cmndPicking.CommandText = "tActivityPicking";
//        cmndPicking.CommandType = CommandType.TableDirect;
//        cmndPicking.IndexName = "PK_ActivityPickingOnActivityRefno";

//        using (SQLiteDataReader drPicking = cmndPicking.ExecuteReader())
//        {
//          if (drPicking.Seek(DbSeekOptions.FirstEqual, new object[] { actRefno }))
//          {
//            if (drPicking.Read())
//            {
//              actPickFKRefno = Convert.ToInt32(drPicking.GetValue((int)SysCommon.ActPickingFields.FKRefno));
//            }
//            else
//            {
//#if MULTILANGUAGE
//              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrpickingreadFailed));
//#else
//              throw new Exception("drPicking.Read() failed.");
//#endif
//            }
//          }
//          else
//          {
//            throw new Exception(
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheSalesOrderRefnoInTactivitypicking), actRefno));
//#else
//                string.Format("Unable to locate the sales order refno {0} In tActivityPicking", actRefno));
//#endif
//          }
//        }
//      }

      return this.SendComm(this.CreateRequest(actPickFKRefno, xmlCriteria));
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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSalesOrderItems), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order items.", false);
//#endif
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tActivityItemPicking"].Rows.Count);

//            this.DataAccessStatusOutHandler(
//                this, 
//                EventArgs.Empty, 
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), 
//#else
//                string.Format("Records {0}", progMax), 
//#endif
//                false);

//            using (SQLiteCommand cmndPickItem = conn.CreateCommand())
//            {
//              cmndPickItem.CommandText = "tActivityItemPicking";
//              cmndPickItem.CommandType = CommandType.TableDirect;
//              cmndPickItem.IndexName = "UQ_ActivityItemPickingOnActivityItemPickingFKRefno";

//              using (SQLiteCommand cmndPickItemComment = conn.CreateCommand())
//              {
//                cmndPickItemComment.CommandText = "tActivityItemPickingComment";
//                cmndPickItemComment.CommandType = CommandType.TableDirect;
//                cmndPickItemComment.IndexName = "PK_ActivityItemPickingCommentOnActivityItemRefno";

//                using (SQLiteCommand cmndItem = conn.CreateCommand())
//                {
//                  cmndItem.CommandText = "tActivityItem";
//                  cmndItem.CommandType = CommandType.TableDirect;

//                  using (SqlCeResultSet rsItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
//                  {
//                    SqlCeUpdatableRecord recItem = rsItem.CreateRecord();

//                    using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
//                    {
//                      cmndProdUnit.CommandText = "tProductUnit";
//                      cmndProdUnit.CommandType = CommandType.TableDirect;
//                      cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

//                      using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
//                      {
//                        using (SQLiteCommand cmndProd = conn.CreateCommand())
//                        {
//                          cmndProd.CommandText = "tProduct";
//                          cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
//                          cmndProd.CommandType = CommandType.TableDirect;

//                          using (SQLiteDataReader drProd = cmndProd.ExecuteReader())
//                          {
//                            using (SQLiteCommand cmndUnit = conn.CreateCommand())
//                            {
//                              cmndUnit.CommandText = "tUnit";
//                              cmndUnit.IndexName = "UQ_UnitOnUnitCode";
//                              cmndUnit.CommandType = CommandType.TableDirect;

//                              using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
//                              {
//                                using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
//                                {
//                                  cmndLocMapWhs.CommandText = "tLocationMapWarehouse";
//                                  cmndLocMapWhs.CommandType = CommandType.TableDirect;
//                                  cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

//                                  using (SQLiteDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
//                                  {
//                                    using (SQLiteCommand cmndID = conn.CreateCommand())
//                                    {
//                                      cmndID.CommandText = "SELECT @@IDENTITY";
//                                      cmndID.CommandType = CommandType.Text;

//                                      using (SqlCeResultSet rsPickItemComment = 
//                                          cmndPickItemComment.ExecuteResultSet(ResultSetOptions.Updatable))
//                                      {
//                                        SqlCeUpdatableRecord recPickItemComment = rsPickItemComment.CreateRecord();

//                                        using (SqlCeResultSet rsPickItem = 
//                                            cmndPickItem.ExecuteResultSet(ResultSetOptions.Updatable))
//                                        {
//                                          SqlCeUpdatableRecord recPickItem = rsPickItem.CreateRecord();
//                                          int actItemRefno = 0;
//                                          int progValue = 0;
//                                          int prodUnitRefno = 0;
//                                          int locMapRefno = 0;
//                                          string actItemPickComment = string.Empty;
//                                          int actItemPickFKRefno = 0;

//                                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);
                                          
//                                          foreach (DataRow drow in ds.Tables["tActivityItemPicking"].Rows)
//                                          {
//                                            errMsg = "Arguments:";

//                                            recItem.SetInt32(
//                                              (int)SysCommon.ActItemFields.SubType, 
//                                              (int)SysCommon.ActSubTypes.Picking);

//                                            recItem.SetInt32((int)SysCommon.ActItemFields.Status, 0);
//                                            recItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, actRefno);
                                            
//                                            rsItem.Insert(recItem);
                                            
//                                            actItemRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                                            recPickItem.SetInt32(
//                                                (int)SysCommon.ActItemPickingFields.ActItemRefno, 
//                                                actItemRefno);

//                                            recPickItem.SetInt32(
//                                                (int)SysCommon.ActItemPickingFields.ActItemSubType, 
//                                                Convert.ToInt32(SysCommon.ActSubTypes.Picking));

//                                            actItemPickFKRefno = Convert.ToInt32(drow["ActivityItemPickingFKRefno"]);
//                                            errMsg += Environment.NewLine + "Item fk refno " + actItemPickFKRefno;
//                                            recPickItem.SetInt32(
//                                                (int)SysCommon.ActItemPickingFields.FKRefno, 
//                                                actItemPickFKRefno);
//                                            recPickItem.SetString(
//                                                (int)SysCommon.ActItemPickingFields.StringFKRefno, 
//                                                actItemPickFKRefno.ToString());

//                                            prodUnitRefno = this.GetProductUnitRefnoFromDataRow(
//                                                drow,
//                                                ref errMsg,
//                                                drProd,
//                                                drUnit,
//                                                drProdUnit,
//                                                false);
//                                            recPickItem.SetInt32(
//                                                (int)SysCommon.ActItemPickingFields.ProdUnitRefno, 
//                                                prodUnitRefno);

//                                            locMapRefno = this.GetWhsLocMapRefnoFromDataRow(
//                                                drow,
//                                                ref errMsg,
//                                                drLocMapWhs);
//                                            recPickItem.SetInt32(
//                                                (int)SysCommon.ActItemPickingFields.WhsLocMapRefno,
//                                                locMapRefno);

//                                            recPickItem.SetString(
//                                                (int)SysCommon.ActItemPickingFields.CustomerProductUnitCode, 
//                                                drow["ActivityItemPickingCustomerProductUnitCode"].ToString());
//                                            recPickItem.SetString(
//                                                (int)SysCommon.ActItemPickingFields.Description, 
//                                                drow["ActivityItemPickingDescription"].ToString());
//                                            recPickItem.SetDecimal(
//                                                (int)SysCommon.ActItemPickingFields.QuantityOrdered, 
//                                                Convert.ToDecimal(drow["ActivityItemPickingQuantityOrdered"]));
//                                            recPickItem.SetDecimal(
//                                                (int)SysCommon.ActItemPickingFields.BaseQuantityOrdered,
//                                              Convert.ToDecimal(drow["ActivityItemPickingBaseQuantityOrdered"]));
//                                            recPickItem.SetDecimal(
//                                                (int)SysCommon.ActItemPickingFields.WeightQuantityOrdered,
//                                                Convert.ToDecimal(drow["ActivityItemPickingWeightQuantityOrdered"]));
//                                            recPickItem.SetDecimal(
//                                                (int)SysCommon.ActItemPickingFields.QuantityShipped, 
//                                                Convert.ToDecimal(drow["ActivityItemPickingQuantityPicked"]));
//                                            recPickItem.SetDecimal(
//                                                (int)SysCommon.ActItemPickingFields.BaseQuantityShipped,
//                                                Convert.ToDecimal(drow["ActivityItemPickingBaseQuantityPicked"]));
//                                            recPickItem.SetDecimal(
//                                                (int)SysCommon.ActItemPickingFields.WeightQuantityShipped, 
//                                                Convert.ToDecimal(drow["ActivityItemPickingWeightQuantityPicked"]));
//                                            recPickItem.SetDecimal(
//                                                (int)SysCommon.ActItemPickingFields.QuantityAvailable,
//                                                Convert.ToDecimal(drow["ActivityItemPickingQuantityAvailable"]));
//                                            recPickItem.SetDecimal(
//                                                (int)SysCommon.ActItemPickingFields.BaseQuantityAvailable,
//                                                Convert.ToDecimal(drow["ActivityItemPickingBaseQuantityAvailable"]));
//                                            recPickItem.SetDecimal(
//                                                (int)SysCommon.ActItemPickingFields.WeightQuantityAvailable,
//                                                Convert.ToDecimal(drow["ActivityItemPickingWeightQuantityAvailable"]));
//                                            recPickItem.SetBoolean(
//                                                (int)SysCommon.ActItemPickingFields.IsReviewed, false);

//                                            rsPickItem.Insert(recPickItem);

//                                            actItemPickComment = drow["ActivityItemPickingComment"].ToString();
//                                            if (!string.IsNullOrEmpty(actItemPickComment))
//                                            {
//                                              recPickItemComment.SetInt32(
//                                                  (int)SysCommon.ActItemPickingCommentFields.ActItemRefno, 
//                                                  actItemRefno);
//                                              recPickItemComment.SetString(
//                                                  (int)SysCommon.ActItemPickingCommentFields.Comment, 
//                                                  actItemPickComment);

//                                              rsPickItemComment.Insert(recPickItemComment);
//                                            }

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

//                                          this.DataAccessProgressOutHandler(
//                                              this, 
//                                              EventArgs.Empty, 
//                                              progMax, 
//                                              0, 
//                                              progMax);
//                                        }
//                                      }
//                                    }

//                                    drLocMapWhs.Close();
//                                  }
//                                }

//                                drUnit.Close();
//                              }
//                            }

//                            drProd.Close();
//                          }
//                        }

//                        drProdUnit.Close();
//                      }
//                    }
//                  }
//                }
//              }
//            }
//          }
          
//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSalesOrderItemsComplete), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order items complete.", false);
//#endif
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSalesOrderItemsFailed), true);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order items failed.", true);
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
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingSalesOrderItemsFailed) + Environment.NewLine + errMsg,
//#else
//            "Database exception occurred. Saving sales order items failed." + Environment.NewLine + errMsg,
//#endif
//            ex);
//      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actPickFKRefno"></param>
    /// <param name="criteria"></param>
    /// <returns></returns>
    protected override string CreateRequest(int actPickFKRefno, XDocument criteria)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);
      XDocument request = null;

      if (criteria == null)
      {
        request = XDocument.Parse(
            "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
            + "<imclient>" 
            + "<command>" 
            + "<action>REQUEST SALES ORDER ITEMS</action>" 
            + SysCommon.GetRequestDefaults(false)
            + ("<refno>" + actPickFKRefno + "</refno>") 
            + "<whs_id></whs_id>" 
            + "<sect_refno></sect_refno>"
            + "</command>" +
            ("<is_disconnected>" + this.SysEnv.SysSettings.IsDisconnectedMode + "</is_disconnected>") 
            + "</imclient>");
      }
      else
      {
        request = XDocument.Parse(
            "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
            + "<imclient>" 
            + "<command>"
            + "<action>REQUEST SALES ORDER ITEMS</action>"
            + SysCommon.GetRequestDefaults(false)
            + ("<refno>" + actPickFKRefno + "</refno>") 
            + ("<whs_id>" + criteria.Root.Element("whs_id").Value + "</whs_id>") 
            + ("<sect_refno>" + criteria.Root.Element("sect_refno").Value + "</sect_refno>") 
            + "</command>" 
            + ("<is_disconnected>" + this.SysEnv.SysSettings.IsDisconnectedMode + "</is_disconnected>")
            + "</imclient>");
      }

      request.Save(xml);
      xml.Flush();
      xml.Close();

      return xmlString.ToString();
    }

    #endregion
  }
}
