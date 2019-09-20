namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableActivityProductionTicketByProduct : TableBase
  {
    #region Constructors

    public TableActivityProductionTicketByProduct()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketByProduct";
    }

    #endregion

    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Request()
    {
      string reqData = string.Empty;

      GC.Collect();
      GC.WaitForPendingFinalizers();

      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketAddproducts), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-products.", true);
#endif

        reqData = this.Send("production ticket add-product(s)");
        if (0 < reqData.Length)
        {
          if (reqData.Contains("<message>ERROR</message>"))
          {
            XmlDocument reqErr = new XmlDocument();
            reqErr.LoadXml(reqData);
            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
          }

          this.Save(reqData);
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketAddproductsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-products complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketAddproductsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-products failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingTicketAddproductsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding ticket add-products failed.", ex);
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
          cmndStatus.CommandText = "SELECT COUNT(tActivity.ActivityRefno)"
              + " FROM tActivity"
              + " WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketAddProduct;
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this,
              EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.TicketAddproducts), rowsAffected),
#else
              string.Format("{0} ticket add-products.", rowsAffected),
#endif
              true);
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredTicketAddproductStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Ticket add-product status failed.", ex);
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
        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTicketAddproducts), true);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-products.", true);
#endif

          cmndPurge.CommandText = "DELETE FROM tActivityProductionByProduct"
              + " WHERE tActivityProductionByProduct.ActivityRefno IN"
              + " (SELECT tActivityProduction.ActivityRefno"
              + " FROM tActivityProduction"
              + " WHERE tActivityProduction.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketAddProduct + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityProduction "
              + "WHERE tActivityProduction.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketAddProduct;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivity "
              + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketAddProduct;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTicketAddproductsComplete), true);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-products complete.", true);
#endif
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTicketAddproductsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-products complete.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingTicketAddproductsComplete), ex);
#else
        throw new Exception("Database exception occurred. Purging ticket add-products complete.", ex);
#endif
      }
    }

    #endregion

    #region Private Members

    /// <summary>
    ///
    /// </summary>
    /// <param name="reqData"></param>
    /// <remarks></remarks>
    protected override void Save(string reqData)
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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTicketAddproducts), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-products.", false);
//#endif
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tActivityProduction"].Rows.Count);

//#if MULTILANGUAGE
//            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), false);
//#else
//            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);
//#endif

//            using (SQLiteCommand cmndProduction = conn.CreateCommand())
//            {
//              cmndProduction.CommandText = "tActivityProduction";
//              cmndProduction.CommandType = CommandType.TableDirect;
//              cmndProduction.IndexName = "PK_ActivityProductionOnActivityRefno";

//              using (SQLiteCommand cmndByProduct = conn.CreateCommand())
//              {
//                cmndByProduct.CommandText = "tActivityProductionByProduct";
//                cmndByProduct.CommandType = CommandType.TableDirect;
//                cmndByProduct.IndexName = "UQ_ActivityProductionByProductOnActivityProductionFKRefno";

//                using (SqlCeResultSet rsByProduct = cmndByProduct.ExecuteResultSet(ResultSetOptions.Updatable))
//                {
//                  SqlCeUpdatableRecord recByProduct = rsByProduct.CreateRecord();

//                  using (SQLiteCommand cmndAct = conn.CreateCommand())
//                  {
//                    cmndAct.CommandText = "tActivity";
//                    cmndAct.CommandType = CommandType.TableDirect;

//                    using (SqlCeResultSet rsAct = cmndAct.ExecuteResultSet(ResultSetOptions.Updatable))
//                    {
//                      SqlCeUpdatableRecord recAct = rsAct.CreateRecord();

//                      using (SQLiteCommand cmndID = conn.CreateCommand())
//                      {
//                        cmndID.CommandText = "SELECT @@IDENTITY";
//                        cmndID.CommandType = CommandType.Text;

//                        using (SQLiteCommand cmndWhs = conn.CreateCommand())
//                        {
//                          cmndWhs.CommandText = "tLocationMapWarehouse";
//                          cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
//                          cmndWhs.CommandType = CommandType.TableDirect;

//                          using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
//                          {
//                            using (SqlCeResultSet rsProduction
//                                = cmndProduction.ExecuteResultSet(ResultSetOptions.Updatable))
//                            {
//                              SqlCeUpdatableRecord recProduction = rsProduction.CreateRecord();
//                              int actRefno = 0;
//                              int progValue = 0;
//                              int whsLocMapRefno = 0;

//                              this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

//                              foreach (DataRow drow in ds.Tables["tActivityProduction"].Rows)
//                              {
//                                errMsg = "Arguments:";

//                                recAct.SetInt32(
//                                    (int)SysCommon.ActFields.SubType,
//                                    (int)SysCommon.ActSubTypes.TicketAddProduct);
//                                recAct.SetInt32((int)SysCommon.ActFields.Status, 0);

//                                rsAct.Insert(recAct);

//                                actRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                                recProduction.SetInt32((int)SysCommon.ActProdFields.ActRefno, actRefno);
//                                recProduction.SetInt32(
//                                    (int)SysCommon.ActProdFields.ActSubType,
//                                    (int)SysCommon.ActSubTypes.TicketAddProduct);

//                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, drWhs);
//                                recProduction.SetInt32((int)SysCommon.ActProdFields.WhsLocMapRefno, whsLocMapRefno);

//                                // To remove time - convert twice
//                                string productionDateTime = Convert.ToDateTime(
//                                    drow["ActivityProductionDateTime"]).ToShortDateString();
//                                errMsg += Environment.NewLine + "Date time " + productionDateTime;
//                                recProduction.SetDateTime(
//                                    (int)SysCommon.ActProdFields.DateTime,
//                                    DateTime.Parse(productionDateTime));

//                                string productionDesc = drow["ActivityProductionDescription"].ToString();
//                                errMsg += Environment.NewLine + "Desc \'" + productionDesc + "\'";
//                                recProduction.SetString((int)SysCommon.ActProdFields.Description, productionDesc);

//                                rsProduction.Insert(recProduction);

//                                recByProduct.SetInt32((int)SysCommon.ActProdByProductFields.ActRefno, actRefno);

//                                int productionFKRefno = Convert.ToInt32(drow["ActivityProductionFKRefno"]);
//                                errMsg += Environment.NewLine + "FK refno " + productionFKRefno;
//                                recByProduct.SetInt32(
//                                    (int)SysCommon.ActProdByProductFields.FKRefno,
//                                    productionFKRefno);

//                                recByProduct.SetString(
//                                    (int)SysCommon.ActProdByProductFields.StringFKRefno,
//                                    productionFKRefno.ToString());

//                                rsByProduct.Insert(recByProduct);

//                                progValue++;
//                                if (progValue % 10 == 0)
//                                {
//                                  this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
//                                }
//                              }

//                              this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
//                            }

//                            drWhs.Close();
//                          }
//                        }
//                      }
//                    }
//                  }
//                }
//              }
//            }
//          }

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTicketAddproductsComplete), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-products complete.", false);
//#endif
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTicketAddproductsFailed), false);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-products failed.", false);
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
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingTicketAddproductsFailed) + Environment.NewLine + errMsg,
//#else
//            "Database exception occurred. Saving ticket add-products failed." + Environment.NewLine + errMsg,
//#endif
//            ex);
//      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Create()
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>" 
          + "<action>REQUEST PRODUCTION TICKET BYPRODUCT</action>" 
          + SysCommon.GetRequestDefaults(false)
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
