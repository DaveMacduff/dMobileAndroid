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

  public class TableActivityProductionTicketByProductItem : TableBase
  {
    #region Constructors

    public TableActivityProductionTicketByProductItem()
      : base()
    {
      FullClassName = "dMobile.GroundWork.TableActivityProductionTicketByProductItem";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketAddproductItems), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product items.", true);
#endif

        if (actRefno == 0)
        {
          using (SQLiteCommand cmndActivity = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndActivity.CommandType = CommandType.Text;
            cmndActivity.CommandText = "SELECT tActivity.ActivityRefno"
                + " FROM tActivity"
                + " WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketAddProduct;

            using (SqliteDataReader drActivity = cmndActivity.ExecuteReader())
            {
              actRefno = Convert.ToInt32(drActivity["ActivityRefno"]);
              while (drActivity.Read())
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
              throw new Exception(reqErr.DocumentElement["detail"].InnerText);
            }

            this.Save(reqData, actRefno);
          }
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketAddproductItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product items complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketAddproductItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingTicketAddproductItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding ticket add-product items failed.", ex);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketAddproductItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketAddproductItemsFailed), ex);
#else
        throw new Exception("Seeding ticket add-product items failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTicketAddproductItems), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product items.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef "
              + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
              + "(SELECT tActivityItem.ActivityItemRefno "
              + "FROM tActivityItem "
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TicketAddProduct + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItemProductionByProduct "
              + "WHERE tActivityItemProductionByProduct.ActivityItemRefno IN "
              + "(SELECT tActivityItem.ActivityItemRefno "
              + "FROM tActivityItem "
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TicketAddProduct + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItemProduction "
              + "WHERE tActivityItemProduction.ActivityItemRefno IN "
              + "(SELECT tActivityItem.ActivityItemRefno "
              + "FROM tActivityItem "
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TicketAddProduct + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItem "
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TicketAddProduct;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTicketAddproductItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product items complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTicketAddproductItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingTicketAddproductItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging ticket add-product items failed.", ex);
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
              + " WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TicketAddProduct;
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this,
              EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.TicketAddproductItems), rowsAffected),
#else
              string.Format("{0} ticket add-product items.", rowsAffected),
#endif
              true);
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredTicketAddproductItemsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Ticket add-product items status failed.", ex);
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
      int productionFKRefno = 0;

      using (SQLiteCommand cmndByProduct = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndByProduct.CommandText = "tActivityProductionByProduct";
        cmndByProduct.CommandType = CommandType.TableDirect;
        cmndByProduct.IndexName = "PK_ActivityProductionByProductOnActivityRefno";
        using (SqliteDataReader drByProduct = cmndByProduct.ExecuteReader())
        {
          if (drByProduct.Seek(DbSeekOptions.FirstEqual, new object[] { actRefno }))
          {
            if (drByProduct.Read())
            {
              productionFKRefno = drByProduct.GetInt32((int)SysCommon.ActProdByProductFields.FKRefno);
            }
            else
            {
#if MULTILANGUAGE
              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrbyproductreadFailed));
#else
              throw new Exception("drByProduct.Read() failed.");
#endif
            }
          }
          else
          {
            throw new Exception(
                string.Format(
#if MULTILANGUAGE
                Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheTicketAddproductRefnoInTactivityproductionbyproduct),
#else
                "Unable to locate the ticket add-product refno {0} in tActivityProductionByProduct.",
#endif
                actRefno));
          }
        }

      }

      return this.SendComm(Create(productionFKRefno));
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTicketAddproductItems), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product items.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["tActivityItemProduction"].Rows.Count);

#if MULTILANGUAGE
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), false);
#else
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);
#endif

            using (SQLiteCommand cmndProduction = conn.CreateCommand())
            {
              cmndProduction.CommandText = "tActivityItemProduction";
              cmndProduction.CommandType = CommandType.TableDirect;
              cmndProduction.IndexName = "PK_ActivityItemProductionOnActivityItemRefno";

              using (SQLiteCommand cmndItem = conn.CreateCommand())
              {
                cmndItem.CommandText = "tActivityItem";
                cmndItem.CommandType = CommandType.TableDirect;

                using (SQLiteCommand cmndActItemByProduct = conn.CreateCommand())
                {
                  cmndActItemByProduct.CommandText = "tActivityItemProductionByProduct";
                  cmndActItemByProduct.CommandType = CommandType.TableDirect;
                  cmndActItemByProduct.IndexName
                      = "UQ_ActivityItemProductionByProductOnActivityItemProductionByProductFKRefno";

                  using (SqlCeResultSet rsItemByProduct
                      = cmndActItemByProduct.ExecuteResultSet(ResultSetOptions.Updatable))
                  {
                    SqlCeUpdatableRecord recItemByProduct = rsItemByProduct.CreateRecord();

                    using (SqlCeResultSet rsItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
                    {
                      SqlCeUpdatableRecord recItem = rsItem.CreateRecord();

                      using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
                      {
                        cmndProdUnit.CommandText = "tProductUnit";
                        cmndProdUnit.CommandType = CommandType.TableDirect;
                        cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

                        using (SqliteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                        {
                          using (SQLiteCommand cmndProd = conn.CreateCommand())
                          {
                            cmndProd.CommandText = "tProduct";
                            cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                            cmndProd.CommandType = CommandType.TableDirect;

                            using (SqliteDataReader drProduct = cmndProd.ExecuteReader())
                            {
                              using (SQLiteCommand cmndUnit = conn.CreateCommand())
                              {
                                cmndUnit.CommandText = "tUnit";
                                cmndUnit.IndexName = "UQ_UnitOnUnitCode";
                                cmndUnit.CommandType = CommandType.TableDirect;

                                using (SqliteDataReader drUnit = cmndUnit.ExecuteReader())
                                {
                                  using (SQLiteCommand cmndWhs = conn.CreateCommand())
                                  {
                                    cmndWhs.CommandText = "tLocationMapWarehouse";
                                    cmndWhs.CommandType = CommandType.TableDirect;
                                    cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

                                    using (SqliteDataReader drWhs = cmndWhs.ExecuteReader())
                                    {
                                      using (SQLiteCommand cmndID = conn.CreateCommand())
                                      {
                                        cmndID.CommandText = "SELECT @@IDENTITY";
                                        cmndID.CommandType = CommandType.Text;

                                        using (SqlCeResultSet rsItemProduction
                                            = cmndProduction.ExecuteResultSet(ResultSetOptions.Updatable))
                                        {
                                          SqlCeUpdatableRecord recItemProduction = rsItemProduction.CreateRecord();
                                          int actItemRefno = 0;
                                          int progValue = 0;
                                          int locMapRefno = 0;
                                          int actItemProductionFKRefno = 0;
                                          int prodUnitRefno = 0;

                                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                                          foreach (DataRow drow in ds.Tables["tActivityItemProduction"].Rows)
                                          {
                                            errMsg = "Arguments:";

                                            recItem.SetInt32(
                                                (int)SysCommon.ActItemFields.SubType,
                                                (int)SysCommon.ActSubTypes.TicketAddProduct);
                                            recItem.SetInt32(
                                                (int)SysCommon.ActItemFields.Status,
                                                (int)SysCommon.ActStatus.NoChange);
                                            recItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, actRefno);

                                            rsItem.Insert(recItem);

                                            actItemRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                            actItemProductionFKRefno
                                                = Convert.ToInt32(drow["ActivityItemProductionFKRefno"]);
                                            errMsg += Environment.NewLine + "Item fk refno " + actItemProductionFKRefno;

                                            prodUnitRefno = this.GetProductUnitRefnoFromDataRow(
                                                drow,
                                                ref errMsg,
                                                drProduct,
                                                drUnit,
                                                drProdUnit,
                                                false);

                                            recItemProduction.SetInt32(
                                                (int)SysCommon.ActItemProdFields.ActItemRefno, actItemRefno);
                                            recItemProduction.SetInt32(
                                                (int)SysCommon.ActItemProdFields.ActivityItemSubType,
                                                (int)SysCommon.ActSubTypes.TicketAddProduct);
                                            recItemProduction.SetInt32(
                                                (int)SysCommon.ActItemProdFields.ProdUnitRefno,
                                                prodUnitRefno);

                                            locMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, drWhs);
                                            recItemProduction.SetInt32(
                                                (int)SysCommon.ActItemProdFields.LocMapRefno,
                                                locMapRefno);

                                            recItemProduction.SetDecimal(
                                                (int)SysCommon.ActItemProdFields.Quantity,
                                                Convert.ToDecimal(drow["ActivityItemProductionQuantity"]));
                                            recItemProduction.SetDecimal(
                                                (int)SysCommon.ActItemProdFields.BaseQuantity,
                                                Convert.ToDecimal(drow["ActivityItemProductionBaseQuantity"]));
                                            recItemProduction.SetDecimal(
                                                (int)SysCommon.ActItemProdFields.WeightQuantity,
                                                Convert.ToDecimal(drow["ActivityItemProductionWeightQuantity"]));
                                            recItemProduction.SetString(
                                                (int)SysCommon.ActItemProdFields.Description,
                                                drow["ActivityItemProductionDescription"].ToString());
                                            recItemProduction.SetDecimal(
                                                (int)SysCommon.ActItemProdFields.QuantityExpected,
                                                Convert.ToDecimal(drow["ActivityItemProductionQuantityExpected"]));
                                            recItemProduction.SetDecimal(
                                                (int)SysCommon.ActItemProdFields.BaseQuantityExpected,
                                                Convert.ToDecimal(drow["ActivityItemProductionBaseQuantityExpected"]));
                                            recItemProduction.SetDecimal(
                                                (int)SysCommon.ActItemProdFields.WeightQuantityExpected,
                                                Convert.ToDecimal(drow["ActivityItemProductionWeightQuantityExpected"]));

                                            rsItemProduction.Insert(recItemProduction);

                                            recItemByProduct.SetInt32(
                                                (int)SysCommon.ActItemProdByProductFields.ActItemRefno,
                                                actItemRefno);
                                            recItemByProduct.SetInt32(
                                                (int)SysCommon.ActItemProdByProductFields.FKRefno,
                                                actItemProductionFKRefno);
                                            recItemByProduct.SetString(
                                                (int)SysCommon.ActItemProdByProductFields.StringFKRefno,
                                                actItemProductionFKRefno.ToString());

                                            rsItemByProduct.Insert(recItemByProduct);

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

                                      drWhs.Close();
                                    }
                                  }

                                  drUnit.Close();
                                }
                              }

                              drProduct.Close();
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
          }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTicketAddproductItemsComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product items complete.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTicketAddproductItemsFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product items failed.", false);
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

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingTicketAddproductItemsFailed)
#else
        throw new Exception("Database exception occurred. Saving ticket add-product items failed."
#endif
          + Environment.NewLine + errMsg, ex);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actProductionFKRefno"></param>
    /// <returns></returns>
    protected override string Create(int actProductionFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>"
          + "<command>"
          + "<action>REQUEST PRODUCTION TICKET BYPRODUCT ITEMS</action>"
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
