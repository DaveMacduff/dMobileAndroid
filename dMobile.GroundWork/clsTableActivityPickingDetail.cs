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

  public class TableActivityPickingDetail : TableBase
  {
    #region Constructors

    public TableActivityPickingDetail()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityPickingDetail";
    }

    #endregion

    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Request(int actItemRefno)
    {
      string reqData = string.Empty;

      //GC.Collect();
      //GC.WaitForPendingFinalizers();

      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesOrderDetails), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order details.", true);
#endif

        if (actItemRefno == 0)
        {
          using (SQLiteCommand cmndPickItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndPickItem.CommandType = CommandType.Text;
            cmndPickItem.CommandText = "SELECT tActivityItem.ActivityItemRefno " 
                + "FROM tActivityItem " 
                + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Picking;

            using (SQLiteDataReader drPickItem = cmndPickItem.ExecuteReader())
            {
              while (drPickItem.Read())
              {
                actItemRefno = Convert.ToInt32(drPickItem["ActivityItemRefno"]);
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

              drPickItem.Close();
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesOrderDetailsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order details complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingPickingDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding picking details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingPickingDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding picking details failed.", ex);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesOrderDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesOrderDetailsFailed), ex);
#else
        throw new Exception("Seeding sales order details failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesOrderDetails), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order details.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef " 
              + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN " 
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail "
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Picking + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailPicking " 
              + "WHERE tActivityDetailPicking.ActivityDetailRefno IN " 
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail "
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Picking + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailExtra " 
              + "WHERE tActivityDetailExtra.ActivityDetailRefno IN " 
              + "(SELECT tActivityDetail.ActivityDetailRefno "
              + "FROM tActivityDetail "
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Picking + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetail "
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Picking;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesOrderDetailsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order details complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesOrderDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingSalesOrderDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging sales order details failed.", ex);
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
          cmndStatus.CommandText = "SELECT COUNT(tActivityDetail.ActivityDetailRefno)" 
              + " FROM tActivityDetail" 
              + " WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Picking;
          cmndStatus.CommandType = CommandType.Text;
          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.SalesOrderDetails), rowsAffected), 
#else
              string.Format("{0} Sales order details.", rowsAffected), 
#endif
              true);
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSalesOrderDetailsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Sales order details status failed.", ex);
#endif
      }

      return rowsAffected;
    }

    #endregion

    #region Private Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemPickRefno"></param>
    /// <returns></returns>
    protected override string Send(int actItemPickRefno)
    {
      int actItemPickFKRefno = 0;

//      using (SQLiteCommand cmndItemPick = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//      {
//        cmndItemPick.CommandText = "tActivityItemPicking";
//        cmndItemPick.CommandType = CommandType.TableDirect;
//        cmndItemPick.IndexName = "PK_ActivityItemPickingOnActivityItemRefno";
//        using (SQLiteDataReader drItemPick = cmndItemPick.ExecuteReader())
//        {
//          if (drItemPick.Seek(DbSeekOptions.FirstEqual, new object[] { actItemPickRefno }))
//          {
//            if (drItemPick.Read())
//            {
//              actItemPickFKRefno = Convert.ToInt32(drItemPick.GetValue((int)SysCommon.ActItemPickingFields.FKRefno));
//            }
//            else
//            {
//#if MULTILANGUAGE
//              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DritempickreadFailed));
//#else
//              throw new Exception("drItemPick.Read() failed.");
//#endif
//            }
//          }
//          else
//          {
//            throw new Exception(
//                string.Format(
//#if MULTILANGUAGE
//                Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheItemPickingRefnoInTactivityitempicking),
//#else
//                "Unable to locate the item picking refno {0} in tActivityItemPicking.",
//#endif
//                actItemPickRefno));
//          }
//        }
//      }

      return this.SendComm(this.Create(actItemPickFKRefno));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reqData"></param>
    /// <param name="actItemPickRefno"></param>
    protected override void Save(string reqData, int actItemPickRefno)
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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSalesOrderDetails), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order details.", false);
//#endif
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailPicking"].Rows.Count);

//            this.DataAccessStatusOutHandler(
//                this,
//                EventArgs.Empty,
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax),
//#else
//                string.Format("Records {0}", progMax),
//#endif
//                false);

//            using (SQLiteCommand cmndDetailPick = conn.CreateCommand())
//            {
//              cmndDetailPick.CommandText = "tActivityDetailPicking";
//              cmndDetailPick.CommandType = CommandType.TableDirect;
//              cmndDetailPick.IndexName = "UQ_ActivityDetailPickingOnActivityDetailPickingFKRefno";

//              using (SqlCeResultSet rsDetailPick = cmndDetailPick.ExecuteResultSet(ResultSetOptions.Updatable))
//              {
//                SqlCeUpdatableRecord recDetailPick = rsDetailPick.CreateRecord();

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

//                        using (SQLiteCommand cmndItemPick = conn.CreateCommand())
//                        {
//                          cmndItemPick.CommandText = "tActivityItemPicking";
//                          cmndItemPick.CommandType = CommandType.TableDirect;
//                          cmndItemPick.IndexName = "PK_ActivityItemPickingOnActivityItemRefno";

//                          using (SQLiteDataReader drItemPick = cmndItemPick.ExecuteReader())
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
//                                                  "PK_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefno" 
//                                                  + "AndProductSchemeVariantRefno";

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
//                                                  int actDetailPickFKRefno = 0;

//                                                  this.DataAccessProgressOutHandler(
//                                                      this,
//                                                      EventArgs.Empty,
//                                                      0,
//                                                      0,
//                                                      progMax);

//                                                  if (drItemPick.Seek(
//                                                      DbSeekOptions.FirstEqual,
//                                                      new object[] { actItemPickRefno }))
//                                                  {
//                                                    if (!drItemPick.Read())
//                                                    {
//#if MULTILANGUAGE
//                                                      throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DritempickreadFailed));
//#else
//                                                      throw new Exception("drItemPick.Read() failed.");
//#endif
//                                                    }
//                                                  }
//                                                  else
//                                                  {
//                                                    throw new Exception(
//                                                        string.Format(
//#if MULTILANGUAGE
//                                                        Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheActivityItemRefnoIn)
//                                                        + Translator.GetInstance().Translate(TranslatorVocabulary.Tactivityitempicking),
//#else
//                                                        "Unable to locate the activity item refno {0} in "
//                                                        + "tActivityItemPicking",
//#endif
//                                                        actItemPickRefno));
//                                                  }

//                                                  foreach (DataRow drow in ds.Tables["tActivityDetailPicking"].Rows)
//                                                  {
//                                                    errMsg = "Arguments:";

//                                                    recDetail.SetInt32(
//                                                        (int)SysCommon.ActDetailFields.SubType,
//                                                        (int)SysCommon.ActSubTypes.Picking);

//                                                    recDetail.SetInt32(
//                                                        (int)SysCommon.ActDetailFields.ActivityItemRefno,
//                                                        actItemPickRefno);

//                                                    recDetail.SetInt32(
//                                                        (int)SysCommon.ActDetailFields.ProductUnitRefno,
//                                                        drItemPick.GetInt32(
//                                                        (int)SysCommon.ActItemPickingFields.ProdUnitRefno));

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

//                                                    recDetailPick.SetInt32(
//                                                        (int)SysCommon.ActDetailPickingFields.ActDetailRefno,
//                                                        actDetailRefno);

//                                                    actDetailPickFKRefno = Convert.ToInt32(
//                                                        drow["ActivityDetailPickingFKRefno"]);
//                                                    errMsg += Environment.NewLine 
//                                                        + "Detail fk refno " + actDetailPickFKRefno;
//                                                    recDetailPick.SetInt32(
//                                                        (int)SysCommon.ActDetailPickingFields.FKRefno,
//                                                        actDetailPickFKRefno);

//                                                    recDetailPick.SetBoolean(
//                                                        (int)SysCommon.ActDetailPickingFields.IsConfirmed,
//                                                        Convert.ToBoolean(
//                                                        drow["ActivityDetailPickingIsConfirmed"]));

//                                                    rsDetailPick.Insert(recDetailPick);

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

//                            drItemPick.Close();
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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSalesOrderDetailsCompleted), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order details completed.", false);
//#endif
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSalesOrderDetailsFailed), false);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order details failed.", false);
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
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingSalesOrderDetailsFailed) + Environment.NewLine + errMsg,
//#else
//            "Database exception occurred. Saving sales order details failed." + Environment.NewLine + errMsg,
//#endif
//            ex);
//      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemPickFKRefno"></param>
    /// <returns></returns>
    protected override string Create(int actItemPickFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>" 
          + "<action>REQUEST SALES ORDER DETAILS</action>" 
          + SysCommon.GetRequestDefaults(false)
          + ("<itm_refno>" + actItemPickFKRefno.ToString() + "</itm_refno>") 
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
