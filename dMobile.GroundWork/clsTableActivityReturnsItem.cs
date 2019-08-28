namespace dMobile.GroundWork
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  
  using System.Text;
  using System.Windows.Forms;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using Microsoft.VisualBasic;

  public class TableActivityReturnsItem : TableBase
  {
    public TableActivityReturnsItem()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityReturnsItem";
    }

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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesReturnItems), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return items.", true);
#endif

        if (actRefno == 0)
        {
          using (SqlCeCommand cmndReturns = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndReturns.CommandType = CommandType.Text;
            cmndReturns.CommandText = "SELECT tActivity.ActivityRefno" 
                + " FROM tActivity" 
                + " WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.Returns;

            using (SqlCeDataReader drReturns = cmndReturns.ExecuteReader())
            {
              while (drReturns.Read())
              {
                actRefno = Convert.ToInt32(drReturns["ActivityRefno"]);

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
              }

              drReturns.Close();
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
              throw new Exception(reqErr.DocumentElement["detail"].InnerText);
            }

            this.Save(reqData, actRefno);
          }
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesReturnItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return items complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesReturnItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingSalesReturnItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding sales return items failed.", ex);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesReturnItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesReturnItemsFailed), ex);
#else
        throw new Exception("Seeding sales return items failed.", ex);
#endif
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Purge()
    {
#if MULTILANGUAGE
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesReturnItems), true);
#else
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return items.", true);
#endif

      try
      {
        using (SqlCeCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef "
              + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
              + "(SELECT tActivityItem.ActivityItemRefno "
              + "FROM tActivityItem "
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Returns + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItemReturnsComment";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItemReturns " 
                + "WHERE tActivityItemReturns.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Returns;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItem " 
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Returns;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesReturnItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return items complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesReturnItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingSalesReturnItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging sales return items failed.", ex);
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
        using (SqlCeCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndStatus.CommandType = CommandType.Text;
          cmndStatus.CommandText = "SELECT COUNT(tActivityItem.ActivityItemRefno) "
              + "FROM tActivityItem " 
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Returns;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.SalesReturnItems), rowsAffected), 
#else
              string.Format("{0} Sales return items.", rowsAffected), 
#endif
              true);
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSalesReturnItemsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Sales return items status failed.", ex);
#endif
      }

      return rowsAffected;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Send(int actRefno)
    {
      int returnsFKRefno = 0;

      using (SqlCeCommand cmndReturns = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndReturns.CommandText = "tActivityReturns";
        cmndReturns.CommandType = CommandType.TableDirect;
        cmndReturns.IndexName = "PK_ActivityReturnsOnActivityRefno";

        using (SqlCeDataReader drReturns = cmndReturns.ExecuteReader())
        {
          if (drReturns.Seek(DbSeekOptions.FirstEqual, new object[] { actRefno }))
          {
            if (drReturns.Read())
            {
              returnsFKRefno = drReturns.GetInt32((int)SysCommon.ActReturnsFields.FKRefno);
            }
            else
            {
#if MULTILANGUAGE
              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrreturnsreadFailed));
#else
              throw new Exception("drReturns.Read() failed.");
#endif
            }
          }
          else
          {
            throw new Exception(
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheSalesReturnRefnoInTactivityreturns), actRefno));
#else
                string.Format("Unable to locate the sales return refno {0} in tActivityReturns", actRefno));
#endif
          }
        }
      }

      return this.SendComm(this.Create(returnsFKRefno));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="reqData"></param>
    /// <remarks></remarks>
    protected override void Save(string reqData, int actRefno)
    {
      SqlCeConnection conn = DatabaseConnection.GetInstance().SQLConn;
      string errMsg = string.Empty;

      try
      {
        using (DataSet ds = new DataSet())
        {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.ReadingReceivedData), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
#endif
          ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSalesReturnItems), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return items.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["tActivityItemReturns"].Rows.Count);

            this.DataAccessStatusOutHandler(
                this, 
                EventArgs.Empty, 
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), 
#else
                string.Format("Records {0}", progMax), 
#endif
                false);

            using (SqlCeCommand cmndRetItem = conn.CreateCommand())
            {
              cmndRetItem.CommandText = "tActivityItemReturns";
              cmndRetItem.CommandType = CommandType.TableDirect;
              cmndRetItem.IndexName = "UQ_ActivityItemReturnsOnActivityItemReturnsFKRefno";

              using (SqlCeCommand cmndRetItemComment = conn.CreateCommand())
              {
                cmndRetItemComment.CommandText = "tActivityItemReturnsComment";
                cmndRetItemComment.CommandType = CommandType.TableDirect;
                cmndRetItemComment.IndexName = "PK_ActivityItemReturnsCommentOnActivityItemRefno";

                using (SqlCeCommand cmndItem = conn.CreateCommand())
                {
                  cmndItem.CommandText = "tActivityItem";
                  cmndItem.CommandType = CommandType.TableDirect;

                  using (SqlCeResultSet rsItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
                  {
                    SqlCeUpdatableRecord recItem = rsItem.CreateRecord();

                    using (SqlCeCommand cmndProdUnit = conn.CreateCommand())
                    {
                      cmndProdUnit.CommandText = "tProductUnit";
                      cmndProdUnit.CommandType = CommandType.TableDirect;
                      cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

                      using (SqlCeDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                      {
                        using (SqlCeCommand cmndProd = conn.CreateCommand())
                        {
                          cmndProd.CommandText = "tProduct";
                          cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                          cmndProd.CommandType = CommandType.TableDirect;

                          using (SqlCeDataReader drProd = cmndProd.ExecuteReader())
                          {
                            using (SqlCeCommand cmndUnit = conn.CreateCommand())
                            {
                              cmndUnit.CommandText = "tUnit";
                              cmndUnit.IndexName = "UQ_UnitOnUnitCode";
                              cmndUnit.CommandType = CommandType.TableDirect;

                              using (SqlCeDataReader drUnit = cmndUnit.ExecuteReader())
                              {
                                using (SqlCeCommand cmndLocMapWhs = conn.CreateCommand())
                                {
                                  cmndLocMapWhs.CommandText = "tLocationMapWarehouse";
                                  cmndLocMapWhs.CommandType = CommandType.TableDirect;
                                  cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

                                  using (SqlCeDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
                                  {
                                    using (SqlCeCommand cmndID = conn.CreateCommand())
                                    {
                                      cmndID.CommandText = "SELECT @@IDENTITY";
                                      cmndID.CommandType = CommandType.Text;

                                      using (SqlCeResultSet rsRetItemComment 
                                          = cmndRetItemComment.ExecuteResultSet(ResultSetOptions.Updatable))
                                      {
                                        SqlCeUpdatableRecord recRetItemComment = rsRetItemComment.CreateRecord();

                                        using (SqlCeResultSet rsRetItem 
                                            = cmndRetItem.ExecuteResultSet(ResultSetOptions.Updatable))
                                        {
                                          SqlCeUpdatableRecord recRetItem = rsRetItem.CreateRecord();
                                          int actItemRefno = 0;
                                          int progValue = 0;
                                          int prodUnitRefno = 0;
                                          int locMapRefno = 0;
                                          int actItemReturnsFKRefno = 0;

                                          string actItemRetComment = string.Empty;

                                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                                          foreach (DataRow drow in ds.Tables["tActivityItemReturns"].Rows)
                                          {
                                            errMsg = "Arguments:";

                                            recItem.SetInt32(
                                                (int)SysCommon.ActItemFields.SubType, 
                                                (int)SysCommon.ActSubTypes.Returns);
                                            recItem.SetInt32((int)SysCommon.ActItemFields.Status, 0);
                                            recItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, actRefno);

                                            rsItem.Insert(recItem);
                                            
                                            actItemRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                            prodUnitRefno = this.GetProductUnitRefnoFromDataRow(
                                                drow, 
                                                ref errMsg, 
                                                drProd, 
                                                drUnit, 
                                                drProdUnit,
                                                false);

                                            locMapRefno = this.GetWhsLocMapRefnoFromDataRow(
                                                drow, 
                                                ref errMsg, 
                                                drLocMapWhs);

                                            recRetItem.SetInt32(
                                                (int)SysCommon.ActItemReturnsFields.ActItemRefno, 
                                                actItemRefno);
                                            recRetItem.SetInt32(
                                                (int)SysCommon.ActItemReturnsFields.ActItemSubType, 
                                                (int)SysCommon.ActSubTypes.Returns);

                                            actItemReturnsFKRefno = Convert.ToInt32(drow["ActivityItemReturnsFKRefno"]);
                                            errMsg += Environment.NewLine + "Item fk refno " + actItemReturnsFKRefno;
                                            recRetItem.SetInt32(
                                                (int)SysCommon.ActItemReturnsFields.FKRefno, 
                                                actItemReturnsFKRefno);
                                            recRetItem.SetString(
                                                (int)SysCommon.ActItemReturnsFields.StringFKRefno, 
                                                Convert.ToString(actItemReturnsFKRefno));
                                            recRetItem.SetInt32(
                                                (int)SysCommon.ActItemReturnsFields.ProdUnitRefno, 
                                                prodUnitRefno);
                                            recRetItem.SetInt32(
                                                (int)SysCommon.ActItemReturnsFields.WhsLocMapRefno, 
                                                locMapRefno);
                                            recRetItem.SetString(
                                                (int)SysCommon.ActItemReturnsFields.CustomerProdUnitCode, 
                                                drow["ActivityItemReturnsCustomerProductUnitCode"].ToString());
                                            recRetItem.SetString(
                                                (int)SysCommon.ActItemReturnsFields.Description, 
                                                drow["ActivityItemReturnsDescription"].ToString());
                                            recRetItem.SetDecimal(
                                                (int)SysCommon.ActItemReturnsFields.QuantityReturned, 
                                                Convert.ToDecimal(drow["ActivityItemReturnsQuantityReturned"]));
                                            recRetItem.SetDecimal(
                                                (int)SysCommon.ActItemReturnsFields.BaseQuantityReturned, 
                                                Convert.ToDecimal(drow["ActivityItemReturnsBaseQuantityReturned"]));
                                            recRetItem.SetDecimal(
                                                (int)SysCommon.ActItemReturnsFields.WeightQuantityReturned, 
                                                Convert.ToDecimal(drow["ActivityItemReturnsWeightQuantityReturned"]));

                                            rsRetItem.Insert(recRetItem);

                                            actItemRetComment = drow["ActivityItemReturnsComment"].ToString();
                                            if (!string.IsNullOrEmpty(actItemRetComment))
                                            {
                                              recRetItemComment.SetInt32(
                                                  (int)SysCommon.ActItemReturnsCommentFields.ActItemRefno, 
                                                  actItemRefno);
                                              
                                              recRetItemComment.SetString(
                                                  (int)SysCommon.ActItemReturnsCommentFields.Comment, 
                                                  actItemRetComment);

                                              rsRetItemComment.Insert(recRetItemComment);
                                            }

                                            progValue++;
                                            if (progValue % 100 == 0)
                                            {
                                              this.DataAccessProgressOutHandler(
                                                  this, 
                                                  EventArgs.Empty, 
                                                  progValue, 
                                                  0, 
                                                  progMax);
                                            }
                                          }

                                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                                        }
                                      }
                                    }

                                    drLocMapWhs.Close();
                                  }
                                }

                                drUnit.Close();
                              }
                            }

                            drProd.Close();
                          }
                        }

                        drProdUnit.Close();
                      }
                    }
                  }
                }
              }
            }
          }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSalesReturnItemsComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return items complete.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSalesReturnItemsFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return items failed.", false);
#endif

        // Duplicate key violations
        if (ex.NativeError == 25016)
        {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation), false);
          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation) + Environment.NewLine + errMsg, ex);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
          throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
#endif
        }

        throw new Exception(
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingSalesReturnItemsFailed) + Environment.NewLine + errMsg,
#else
            "Database exception occurred. Saving sales return items failed." + Environment.NewLine + errMsg,
#endif
            ex);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="returnsFKRefno"></param>
    /// <returns></returns>
    protected override string Create(int returnsFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>" 
          + "<action>REQUEST SALES RETURN ITEMS</action>" 
          + SysCommon.GetRequestDefaults(false)
          + ("<refno>" + returnsFKRefno + "</refno>") 
          + "</command>"
          + "</imclient>");

      request.Save(xml);
      xml.Flush();
      xml.Close();

      return xmlString.ToString();
    }
  }
}
