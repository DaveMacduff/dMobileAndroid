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

  public class TableActivityReturnsDetail : TableBase
  {
    #region Constructors

    public TableActivityReturnsDetail()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityReturnsDetail";
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

      GC.Collect();
      GC.WaitForPendingFinalizers();

      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesReturnDetails), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return details.", true);
#endif

        if (actItemRefno == 0)
        {
          using (SQLiteCommand cmndRetItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndRetItem.CommandType = CommandType.Text;
            cmndRetItem.CommandText = "SELECT tActivityItem.ActivityItemRefno " 
                + "FROM tActivityItem " 
                + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Returns;

            using (SQLiteDataReader drReturnsItem = cmndRetItem.ExecuteReader())
            {
              while (drReturnsItem.Read())
              {
                actItemRefno = Convert.ToInt32(drReturnsItem["ActivityItemRefno"]);
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

              drReturnsItem.Close();
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesReturnDetailsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return details complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesReturnDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingSalesReturnDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding sales return details failed.", ex);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesReturnDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesReturnDetailsFailed), ex);
#else
        throw new Exception("Seeding sales return details failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesReturnDetails), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return details.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef " 
              + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail "
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Returns + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailReturns " 
              + "WHERE tActivityDetailReturns.ActivityDetailRefno IN " 
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Returns + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailExtra " 
              + "WHERE tActivityDetailExtra.ActivityDetailRefno IN " 
              + "(SELECT tActivityDetail.ActivityDetailRefno "
              + "FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Returns + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Returns;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesReturnDetailsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return details complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesReturnDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingSalesReturnDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging sales return details failed.", ex);
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
          cmndStatus.CommandText = "SELECT COUNT(tActivityDetail.ActivityDetailRefno) " 
              + "FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Returns;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.SalesReturnDetails), rowsAffected),
#else
              string.Format("{0} sales return details.", rowsAffected),
#endif
              true);
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSalesReturnDetailsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Sales return details status failed.", ex);
#endif
      }

      return rowsAffected;
    }

    #endregion

    #region Private Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemRetRefno"></param>
    /// <returns></returns>
    protected override string Send(int actItemRetRefno)
    {
            int actItemRetFKRefno = 0;

            //      using (SQLiteCommand cmndItemReturns = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            //      {
            //        cmndItemReturns.CommandText = "tActivityItemReturns";
            //        cmndItemReturns.CommandType = CommandType.TableDirect;
            //        cmndItemReturns.IndexName = "PK_ActivityItemReturnsOnActivityItemRefno";

            //        using (SQLiteDataReader drItemReturns = cmndItemReturns.ExecuteReader())
            //        {
            //          if (drItemReturns.Seek(DbSeekOptions.FirstEqual, new object[] { actItemRetRefno }))
            //          {
            //            if (drItemReturns.Read())
            //            {
            //              actItemRetFKRefno = Convert.ToInt32(
            //                  drItemReturns.GetValue((int)SysCommon.ActItemReturnsFields.FKRefno));
            //            }
            //            else
            //            {
            //#if MULTILANGUAGE
            //              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DritemreturnsreadFailed));
            //#else
            //              throw new Exception("drItemReturns.Read() failed.");
            //#endif
            //            }
            //          }
            //          else
            //          {
            //            throw new Exception(
            //#if MULTILANGUAGE
            //                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheItemReturnsRefnoInTactivityitemreturns), actItemRetRefno));
            //#else
            //                string.Format("Unable to locate the item returns refno {0} in tActivityItemReturns.", actItemRetRefno));
            //#endif
            //          }
            //        }
            //      }

            return this.SendComm(this.Create(actItemRetFKRefno));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reqData"></param>
    /// <param name="actItemReturnsRefno"></param>
    protected override void Save(string reqData, int actItemReturnsRefno)
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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSalesReturnDetails), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return details.", false);
//#endif
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailReturns"].Rows.Count);

//            this.DataAccessStatusOutHandler(
//                this, 
//                EventArgs.Empty, 
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), 
//#else
//                string.Format("Records {0}", progMax), 
//#endif
//                false);
              
//            using (SQLiteCommand cmndDetailRet = conn.CreateCommand())
//            {
//              cmndDetailRet.CommandText = "tActivityDetailReturns";
//              cmndDetailRet.CommandType = CommandType.TableDirect;
//              cmndDetailRet.IndexName = "UQ_ActivityDetailReturnsOnActivityDetailReturnsFKRefno";

//              using (SqlCeResultSet rsDetailRet = cmndDetailRet.ExecuteResultSet(ResultSetOptions.Updatable))
//              {
//                SqlCeUpdatableRecord recDetailRet = rsDetailRet.CreateRecord();

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

//                        using (SQLiteCommand cmndItemRet = conn.CreateCommand())
//                        {
//                          cmndItemRet.CommandText = "tActivityItemReturns";
//                          cmndItemRet.CommandType = CommandType.TableDirect;
//                          cmndItemRet.IndexName = "PK_ActivityItemReturnsOnActivityItemRefno";

//                          using (SQLiteDataReader drItemRet = cmndItemRet.ExecuteReader())
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
//                                              cmndSchemeVarXRef.CommandText 
//                                                  = "tActivityDetailProductSchemeVariantXRef";
//                                              cmndSchemeVarXRef.CommandType = CommandType.TableDirect;
//                                              cmndSchemeVarXRef.IndexName = 
//                                                  "PK_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefnoAndProductSchemeVariantRefno";

//                                              using (SqlCeResultSet rsSchemeVarXRef 
//                                                  = cmndSchemeVarXRef.ExecuteResultSet(ResultSetOptions.Updatable))
//                                              {
//                                                SqlCeUpdatableRecord recSchemeVarXRef = rsSchemeVarXRef.CreateRecord();

//                                                using (SQLiteCommand cmndID = conn.CreateCommand())
//                                                {
//                                                  cmndID.CommandText = "SELECT @@IDENTITY FROM tActivityDetail";
//                                                  cmndID.CommandType = CommandType.Text;

//                                                  int actDetailRefno = 0;
//                                                  int detailRetFKRefno = 0;
//                                                  int progValue = 0;

//                                                  this.DataAccessProgressOutHandler(
//                                                      this, 
//                                                      EventArgs.Empty, 
//                                                      0, 
//                                                      0, 
//                                                      progMax);

//                                                  if (drItemRet.Seek(
//                                                      DbSeekOptions.FirstEqual,
//                                                      new object[] { actItemReturnsRefno }))
//                                                  {
//                                                    if (!drItemRet.Read())
//                                                    {
//#if MULTILANGUAGE
//                                                      throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DritemretreadFailed));
//#else
//                                                      throw new Exception("drItemRet.Read() failed.");
//#endif
//                                                    }
//                                                  }
//                                                  else
//                                                  {
//                                                    throw new Exception(
//                                                        string.Format(
//#if MULTILANGUAGE
//                                                        Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheReturnsItemRefnoInTactivityitemreturns),
//#else
//                                                        "Unable to locate the returns item refno {0} in tActivityItemReturns",
//#endif
//                                                        actItemReturnsRefno));
//                                                  }

//                                                  foreach (DataRow drow in ds.Tables["tActivityDetailReturns"].Rows)
//                                                  {
//                                                    errMsg = "Arguments:";

//                                                    recDetail.SetInt32(
//                                                        (int)SysCommon.ActDetailFields.SubType, 
//                                                        (int)SysCommon.ActSubTypes.Returns);
//                                                    recDetail.SetInt32(
//                                                        (int)SysCommon.ActDetailFields.ActivityItemRefno, 
//                                                        actItemReturnsRefno);
//                                                    recDetail.SetInt32(
//                                                        (int)SysCommon.ActDetailFields.ProductUnitRefno, 
//                                                        drItemRet.GetInt32((int)SysCommon.ActItemReturnsFields.ProdUnitRefno));

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

//                                                    recDetailRet.SetInt32(
//                                                        (int)SysCommon.ActDetailReturnsFields.ActDetailRefno, 
//                                                        actDetailRefno);

//                                                    detailRetFKRefno = 
//                                                        Convert.ToInt32(drow["ActivityDetailReturnsFKRefno"]);
//                                                    errMsg += Environment.NewLine + "Detail fk refno " 
//                                                        + detailRetFKRefno;
//                                                    recDetailRet.SetInt32(
//                                                        (int)SysCommon.ActDetailReturnsFields.FKRefno, 
//                                                        detailRetFKRefno);

//                                                    recDetailRet.SetBoolean(
//                                                        (int)SysCommon.ActDetailReturnsFields.IsConfirmed, 
//                                                        Convert.ToBoolean(drow["ActivityDetailReturnsIsConfirmed"]));

//                                                    rsDetailRet.Insert(recDetailRet);

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

//                            drItemRet.Close();
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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSalesReturnDetailsComplete), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return details complete.", false);
//#endif
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSalesReturnDetailsFailed), false);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return details failed.", false);
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
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingSalesReturnDetailsFailed) + Environment.NewLine + errMsg,
//#else
//            "Database exception occurred. Saving sales return details failed." + Environment.NewLine + errMsg,
//#endif
//            ex);
//      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Create(int iActivityItemReturnsFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>"
          + "<action>REQUEST SALES RETURN DETAILS</action>"
          + SysCommon.GetRequestDefaults(false)
          + ("<itm_refno>" + iActivityItemReturnsFKRefno.ToString() + "</itm_refno>")
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
