namespace dMobile.GroundWork
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using Microsoft.VisualBasic;

  public class TableActivityProductionTicketByProductDetail : TableBase
  {
    #region Constructors

    public TableActivityProductionTicketByProductDetail()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketByProductDetail";
    }

    #endregion

    #region Public Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemRefno"></param>
    public override void Request(int actItemRefno)
    {
      string reqData = string.Empty;

      //GC.Collect();
      //GC.WaitForPendingFinalizers();

      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketAddproductDetails), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product details.", true);
#endif

        if (actItemRefno == 0)
        {
          using (SQLiteCommand cmndActItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndActItem.CommandType = CommandType.Text;
            cmndActItem.CommandText = "SELECT tActivityItem.ActivityItemRefno" 
                + " FROM tActivityItem" 
                + " WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TicketAddProduct;

            using (SQLiteDataReader drActItem = cmndActItem.ExecuteReader())
            {
              while (drActItem.Read())
              {
                actItemRefno = Convert.ToInt32(drActItem["ActivityItemRefno"]);
                reqData = this.Send(actItemRefno);
                if (0 < reqData.Length)
                {
                  if (reqData.Contains("<message>ERROR</message>"))
                  {
                    XmlDocument reqErr = new XmlDocument();
                    reqErr.LoadXml(reqData);
                    throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                  }

                  this.Save(reqData, actItemRefno);
                }
              }

              drActItem.Close();
            }
          }
        }
        else
        {
          reqData = this.Send(actItemRefno);
          if (0 < reqData.Length)
          {
            if (reqData.Contains("<message>ERROR</message>"))
            {
              XmlDocument reqErr = new XmlDocument();
              reqErr.LoadXml(reqData);
              throw new Exception(reqErr.DocumentElement["detail"].InnerText);
            }

            this.Save(reqData, actItemRefno);
          }
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketAddproductDetailsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product details complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketAddproductDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingTicketAddproductDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding ticket add-product details failed.", ex);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketAddproductDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketAddproductDetailsFailed), ex);
#else
        throw new Exception("Seeding ticket add-product details failed.", ex);
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
          cmndStatus.CommandType = CommandType.Text;
          cmndStatus.CommandText = "SELECT COUNT(tActivityDetail.ActivityDetailRefno)" 
              + " FROM tActivityDetail" 
              + " WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.TicketAddProduct;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());
          
          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.TicketAddproductDetails), rowsAffected), 
#else
              string.Format("{0} ticket add-product details.", rowsAffected), 
#endif
              true);
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredTicketAddproductDetailsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Ticket add-product details status failed.", ex);
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
#if MULTILANGUAGE
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTicketAddproductDetails), true);
#else
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product details.", true);
#endif

      try
      {
        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef" 
              + " WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN" 
              + " (SELECT tActivityDetail.ActivityDetailRefno" 
              + " FROM tActivityDetail" 
              + " WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.TicketAddProduct + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailProductionByProduct" 
              + " WHERE tActivityDetailProductionByProduct.ActivityDetailRefno IN " 
              + "(SELECT tActivityDetail.ActivityDetailRefno" 
              + " FROM tActivityDetail" 
              + " WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.TicketAddProduct + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailExtra" 
              + " WHERE tActivityDetailExtra.ActivityDetailRefno IN " 
              + "(SELECT tActivityDetail.ActivityDetailRefno" 
              + " FROM tActivityDetail" 
              + " WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.TicketAddProduct + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.TicketAddProduct;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTicketAddproductDetailsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product details complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTicketAddproductDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingTicketAddproductDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging ticket add-product details failed.", ex);
#endif
      }
    }

    #endregion

    #region Private Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemRefno"></param>
    /// <returns></returns>
    protected override string Send(int actItemRefno)
    {
      int actItemFKRefno = 0;

//      using (SQLiteCommand cmndItemByProduct = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//      {
//        cmndItemByProduct.CommandText = "tActivityItemProductionByProduct";
//        cmndItemByProduct.CommandType = CommandType.TableDirect;
//        cmndItemByProduct.IndexName = "PK_ActivityItemProductionByProductOnActivityItemRefno";
//        using (SQLiteDataReader drItemByProduct = cmndItemByProduct.ExecuteReader())
//        {
//          if (drItemByProduct.Seek(DbSeekOptions.FirstEqual, new object[] { actItemRefno }))
//          {
//            if (drItemByProduct.Read())
//            {
//              actItemFKRefno = drItemByProduct.GetInt32((int)SysCommon.ActItemProdByProductFields.FKRefno);
//            }
//            else
//            {
//#if MULTILANGUAGE
//              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DritemfinishedreadFailed));
//#else
//              throw new Exception("drItemFinished.Read() failed.");
//#endif
//            }
//          }
//          else
//          {
//            throw new Exception(
//                string.Format(
//#if MULTILANGUAGE
//                Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheActivityItemTicketFinishedInTactivityitemproductionfinished),
//#else
//                "Unable to locate the activity item ticket finished {0} in tActivityItemProductionFinished.",
//#endif
//                actItemRefno));
//          }
//        }
//      }

      return this.SendComm(this.Create(actItemFKRefno));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reqData"></param>
    /// <param name="actItemProductionRefno"></param>
    protected override void Save(string reqData, int actItemProductionRefno)
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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTicketAddproductDetails), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product details.", false);
//#endif
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailProduction"].Rows.Count);

//#if MULTILANGUAGE
//            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), false);
//#else
//            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);
//#endif

//            using (SQLiteCommand cmndDetByProduct = conn.CreateCommand())
//            {
//              cmndDetByProduct.CommandText = "tActivityDetailProductionByProduct";
//              cmndDetByProduct.CommandType = CommandType.TableDirect;
//              cmndDetByProduct.IndexName = 
//                  "UQ_ActivityDetailProductionByProductOnActivityDetailProductionByProductFKRefno";

//              using (SqlCeResultSet rsDetailByProduct = cmndDetByProduct.ExecuteResultSet(ResultSetOptions.Updatable))
//              {
//                SqlCeUpdatableRecord recDetByProduct = rsDetailByProduct.CreateRecord();

//                using (SQLiteCommand cmndDetail = conn.CreateCommand())
//                {
//                  cmndDetail.CommandText = "tActivityDetail";
//                  cmndDetail.CommandType = CommandType.TableDirect;
//                  cmndDetail.IndexName = "PK_ActivityDetailOnActivityDetailRefno";

//                  using (SqlCeResultSet rsDetail = cmndDetail.ExecuteResultSet(ResultSetOptions.Updatable))
//                  {
//                    SqlCeUpdatableRecord recDetail = rsDetail.CreateRecord();

//                    using (SQLiteCommand cmndExtra = conn.CreateCommand())
//                    {
//                      cmndExtra.CommandText = "tActivityDetailExtra";
//                      cmndExtra.CommandType = CommandType.TableDirect;
//                      cmndExtra.IndexName = "PK_ActivityDetailExtraOnActivityDetailRefno";

//                      using (SqlCeResultSet rsExtra = cmndExtra.ExecuteResultSet(ResultSetOptions.Updatable))
//                      {
//                        SqlCeUpdatableRecord recExtra = rsExtra.CreateRecord();

//                        using (SQLiteCommand cmndItemProduction = conn.CreateCommand())
//                        {
//                          cmndItemProduction.CommandText = "tActivityItemProduction";
//                          cmndItemProduction.CommandType = CommandType.TableDirect;
//                          cmndItemProduction.IndexName = "PK_ActivityItemProductionOnActivityItemRefno";

//                          using (SQLiteDataReader drItemProduction = cmndItemProduction.ExecuteReader())
//                          {
//                            using (SQLiteCommand cmndLocMapSlot = conn.CreateCommand())
//                            {
//                              cmndLocMapSlot.CommandText = "tLocationMapSlot";
//                              cmndLocMapSlot.CommandType = CommandType.TableDirect;
//                              cmndLocMapSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";

//                              using (SQLiteDataReader drLocMapSlot = cmndLocMapSlot.ExecuteReader())
//                              {
//                                using (SQLiteCommand cmndLocMapSect = conn.CreateCommand())
//                                {
//                                  cmndLocMapSect.CommandText = "tLocationMapSection";
//                                  cmndLocMapSect.CommandType = CommandType.TableDirect;
//                                  cmndLocMapSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";

//                                  using (SQLiteDataReader drLocMapSect = cmndLocMapSect.ExecuteReader())
//                                  {
//                                    using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
//                                    {
//                                      cmndLocMapWhs.CommandText = "tLocationMapWarehouse";
//                                      cmndLocMapWhs.CommandType = CommandType.TableDirect;
//                                      cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

//                                      using (SQLiteDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
//                                      {
//                                        using (SQLiteCommand cmndSchemeVar = conn.CreateCommand())
//                                        {
//                                          cmndSchemeVar.CommandText = "tProductSchemeVariant";
//                                          cmndSchemeVar.CommandType = CommandType.TableDirect;
//                                          cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";

//                                          using (SQLiteDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
//                                          {
//                                            using (SQLiteCommand cmndSchemeVarXRef = conn.CreateCommand())
//                                            {
//                                              cmndSchemeVarXRef.CommandText = "tActivityDetailProductSchemeVariantXRef";
//                                              cmndSchemeVarXRef.CommandType = CommandType.TableDirect;
//                                              cmndSchemeVarXRef.IndexName =
//                                                  "PK_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefnoAndProductSchemeVariantRefno";

//                                              using (SqlCeResultSet rsSchemeVarXRef = 
//                                                  cmndSchemeVarXRef.ExecuteResultSet(ResultSetOptions.Updatable))
//                                              {
//                                                SqlCeUpdatableRecord recSchemeVarXRef = rsSchemeVarXRef.CreateRecord();

//                                                using (SQLiteCommand cmndID = conn.CreateCommand())
//                                                {
//                                                  cmndID.CommandText = "SELECT @@IDENTITY FROM tActivityDetail";
//                                                  cmndID.CommandType = CommandType.Text;

//                                                  int actDetailRefno = 0;
//                                                  int progValue = 0;

//                                                  this.DataAccessProgressOutHandler(
//                                                      this, 
//                                                      EventArgs.Empty, 
//                                                      0, 
//                                                      0, 
//                                                      progMax);

//                                                  if (drItemProduction.Seek(
//                                                      DbSeekOptions.FirstEqual,
//                                                      new object[] { actItemProductionRefno }))
//                                                  {
//                                                    if (!drItemProduction.Read())
//                                                    {
//                                                      throw new Exception(
//#if MULTILANGUAGE
//                                                          Translator.GetInstance().Translate(TranslatorVocabulary.DritemproductionreadFailed) + Environment.NewLine
//#else
//                                                          "drItemProduction.Read() failed." + Environment.NewLine
//#endif
//                                                          + errMsg);
//                                                    }
//                                                  }
//                                                  else
//                                                  {
//                                                    throw new Exception(
//#if MULTILANGUAGE
//                                                        Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheActivityItemRefnoInz) +
//#else
//                                                        "Unable to Locate the activity item refno in " +
//#endif
//                                                        "tActivityItemProduction " + Environment.NewLine + errMsg);
//                                                  }

//                                                  foreach (DataRow drow in ds.Tables["tActivityDetailProduction"].Rows)
//                                                  {
//                                                    errMsg = "Arguments:";

//                                                    recDetail.SetInt32(
//                                                        (int)SysCommon.ActDetailFields.SubType, 
//                                                        (int)SysCommon.ActSubTypes.TicketAddProduct);
//                                                    recDetail.SetInt32((int)SysCommon.ActDetailFields.ProductUnitRefno, 
//                                                        drItemProduction.GetInt32(
//                                                        (int)SysCommon.ActItemProdFields.ProdUnitRefno));
//                                                    recDetail.SetInt32(
//                                                        (int)SysCommon.ActDetailFields.ActivityItemRefno,
//                                                        actItemProductionRefno);

//                                                    this.SaveDetailLocation(
//                                                        drow,
//                                                        drLocMapSlot,
//                                                        drLocMapSect,
//                                                        drLocMapWhs,
//                                                        ref recDetail,
//                                                        false);

//                                                    this.SaveDetail(drow, ref recDetail, false);

//                                                    rsDetail.Insert(recDetail);

//                                                    actDetailRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                                                    recDetByProduct.SetInt32(
//                                                        (int)SysCommon.ActDetailProdFinishedFields.ActDetailRefno, 
//                                                        actDetailRefno);

//                                                    int actDetailProductionFKRefno = 
//                                                        Convert.ToInt32(drow["ActivityDetailProductionFKRefno"]);
//                                                    errMsg += Environment.NewLine 
//                                                        + "Detail fk refno " + actDetailProductionFKRefno;

//                                                    recDetByProduct.SetInt32(
//                                                        (int)SysCommon.ActDetailProdFinishedFields.FKRefno,
//                                                        actDetailProductionFKRefno);

//                                                    rsDetailByProduct.Insert(recDetByProduct);

//                                                    if (this.SaveCommonDetailVariant(
//                                                        drow,
//                                                        drSchemeVar,
//                                                        ref recSchemeVarXRef,
//                                                        actDetailRefno,
//                                                        false))
//                                                    {
//                                                      rsSchemeVarXRef.Insert(recSchemeVarXRef);
//                                                    }

//                                                    this.SaveDetailExtra(
//                                                      drow,
//                                                      ref recExtra,
//                                                      actDetailRefno,
//                                                      false);

//                                                    rsExtra.Insert(recExtra);

//                                                    progValue++;
//                                                    if (progValue % 100 == 0)
//                                                    {
//                                                      this.DataAccessProgressOutHandler(
//                                                          this, 
//                                                          EventArgs.Empty, 
//                                                          progValue, 
//                                                          0, 
//                                                          progMax);
//                                                    }
//                                                  }

//                                                  this.DataAccessProgressOutHandler(
//                                                      this, 
//                                                      EventArgs.Empty, 
//                                                      progMax, 
//                                                      0, 
//                                                      progMax);
//                                                }
//                                              }
//                                            }

//                                            drSchemeVar.Close();
//                                          }
//                                        }

//                                        drLocMapWhs.Close();
//                                      }
//                                    }

//                                    drLocMapSect.Close();
//                                  }
//                                }

//                                drLocMapSlot.Close();
//                              }
//                            }

//                            drItemProduction.Close();
//                          }
//                        }
//                      }
//                    }
//                  }
//                }
//              }
//            }

//            this.DataAccessStatusOutHandler(
//                this, 
//                EventArgs.Empty, 
//#if MULTILANGUAGE
//                Translator.GetInstance().Translate(TranslatorVocabulary.SavingTicketAddproductDetailsComplete), 
//#else
//                "Saving ticket add-product details complete.", 
//#endif
//                false);
//          }
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTicketAddproductDetailsFailed), false);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product details failed.", false);
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
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingTicketAddproductDetailsFailed) + Environment.NewLine + errMsg,
//#else
//            "Database exception occurred. Saving ticket add-product details failed." + Environment.NewLine + errMsg,
//#endif
//            ex);
//      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Create(int actItemFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>"
          + "<action>REQUEST PRODUCTION TICKET BYPRODUCT DETAILS</action>" 
          + SysCommon.GetRequestDefaults(false)
          + ("<itm_refno>" + actItemFKRefno + "</itm_refno>") 
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
