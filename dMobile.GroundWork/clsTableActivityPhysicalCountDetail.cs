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

  public class TableActivityPhysicalCountDetail : TableBase
  {
    #region Constructors

    public TableActivityPhysicalCountDetail()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityPhysicalCountDetail";
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

      GC.Collect();
      GC.WaitForPendingFinalizers();

      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingPhysicalCountDetails), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical count details.", true);
#endif

        if (actItemRefno == 0)
        {
          using (SQLiteCommand cmndCountItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndCountItem.CommandType = CommandType.Text;
            cmndCountItem.CommandText = "SELECT tActivityItem.ActivityItemRefno" 
                + " FROM tActivityItem" 
                + " WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.PhysicalCounts;

            using (SQLiteDataReader drCountItem = cmndCountItem.ExecuteReader())
            {
              while (drCountItem.Read())
              {
                actItemRefno = Convert.ToInt32(drCountItem["ActivityItemRefno"]);
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

              drCountItem.Close();
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingPhysicalCountDetailsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical count details complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingPhysicalCountDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical count details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingPhysicalCountDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding physical count details failed.", ex);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingPhysicalCountDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical count details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingPhysicalCountDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding physical count details failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingPhysicalCountDetails), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical count details.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef " 
              + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN " 
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail "
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.PhysicalCounts + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailCount " 
              + "WHERE tActivityDetailCount.ActivityDetailRefno IN " 
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.PhysicalCounts + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailExtra " 
              + "WHERE tActivityDetailExtra.ActivityDetailRefno IN " 
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.PhysicalCounts + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.PhysicalCounts;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingPhysicalCountDetailsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical count details complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingPhysicalCountDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical count details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingPhysicalCountDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging physical count details failed.", ex);
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
          cmndStatus.CommandText = "SELECT COUNT(tActivityDetail.ActivityDetailRefno) " 
              + "FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.PhysicalCounts;
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.PhysicalCountDetails), rowsAffected), true);
#else
              string.Format("{0} Physical count details.", rowsAffected), true);
#endif
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPhysicalCountDetailsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Physical count details status failed.", ex);
#endif
      }

      return rowsAffected;
    }

    #endregion

    #region Private Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemCountRefno"></param>
    /// <returns></returns>
    protected override string Send(int actItemCountRefno)
    {
      int actItemCountFKRefno = 0;

//      using (SQLiteCommand cmndItemCount = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//      {
//        cmndItemCount.CommandText = "tActivityItemCount";
//        cmndItemCount.CommandType = CommandType.TableDirect;
//        cmndItemCount.IndexName = "PK_ActivityItemCountOnActivityItemRefno";
//        using (SQLiteDataReader drItemCount = cmndItemCount.ExecuteReader())
//        {
//          if (drItemCount.Seek(DbSeekOptions.FirstEqual, new object[] { actItemCountRefno }))
//          {
//            if (drItemCount.Read())
//            {
//              actItemCountFKRefno = Convert.ToInt32(drItemCount.GetValue((int)SysCommon.ActItemCountFields.FKRefno));
//            }
//            else
//            {
//#if MULTILANGUAGE
//              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DritemcountreadFailed));
//#else
//              throw new Exception("drItemCount.Read() failed.");
//#endif
//            }
//          }
//          else
//          {
//            throw new Exception(
//                string.Format(
//#if MULTILANGUAGE
//                Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheItemPhysicalCountRefnoInTactivityitemcount),
//#else
//                "Unable to locate the item physical count refno {0} in tActivityItemCount.",
//#endif
//                actItemCountRefno));
//          }
//        }
//      }

      return this.SendComm(this.Create(actItemCountFKRefno));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reqData"></param>
    /// <param name="actItemCountRefno"></param>
    protected override void Save(string reqData, int actItemCountRefno)
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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingPhysicalCountDetails), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical count details.", false);
//#endif
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailCount"].Rows.Count);

//            this.DataAccessStatusOutHandler(
//                this,
//                EventArgs.Empty,
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Records), progMax),
//#else
//                string.Format("Records {0}.", progMax),
//#endif
//                false);

//            using (SQLiteCommand cmndDetailCount = conn.CreateCommand())
//            {
//              cmndDetailCount.CommandText = "tActivityDetailCount";
//              cmndDetailCount.CommandType = CommandType.TableDirect;
//              cmndDetailCount.IndexName = "UQ_ActivityDetailCountOnActivityDetailCountFKRefno";

//              using (SqlCeResultSet rsDetailCount =
//                  cmndDetailCount.ExecuteResultSet(ResultSetOptions.Updatable))
//              {
//                SqlCeUpdatableRecord recDetailCount = rsDetailCount.CreateRecord();

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

//                        using (SQLiteCommand cmndItemCount = conn.CreateCommand())
//                        {
//                          cmndItemCount.CommandText = "tActivityItemCount";
//                          cmndItemCount.CommandType = CommandType.TableDirect;
//                          cmndItemCount.IndexName = "PK_ActivityItemCountOnActivityItemRefno";

//                          using (SQLiteDataReader drItemCount = cmndItemCount.ExecuteReader())
//                          {
//                            using (SQLiteCommand cmndLocMapSlot = conn.CreateCommand())
//                            {
//                              cmndLocMapSlot.CommandText = "tLocationMapSlot";
//                              cmndLocMapSlot.CommandType = CommandType.TableDirect;
//                              cmndLocMapSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";

//                              using (SQLiteDataReader drLocMapSlot = cmndLocMapSlot.ExecuteReader())
//                              {
//                                using (SQLiteCommand cmndLocMapSection = conn.CreateCommand())
//                                {
//                                  cmndLocMapSection.CommandText = "tLocationMapSection";
//                                  cmndLocMapSection.CommandType = CommandType.TableDirect;
//                                  cmndLocMapSection.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";

//                                  using (SQLiteDataReader drLocMapSection = cmndLocMapSection.ExecuteReader())
//                                  {
//                                    using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
//                                    {
//                                      cmndLocMapWhs.CommandText = "tLocationMapWarehouse";
//                                      cmndLocMapWhs.CommandType = CommandType.TableDirect;
//                                      cmndLocMapWhs.IndexName =
//                                          "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

//                                      using (SQLiteDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
//                                      {
//                                        using (SQLiteCommand cmndSchemeVar = conn.CreateCommand())
//                                        {
//                                          cmndSchemeVar.CommandText = "tProductSchemeVariant";
//                                          cmndSchemeVar.CommandType = CommandType.TableDirect;
//                                          cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";

//                                          using (SQLiteDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
//                                          {
//                                            using (SQLiteCommand cmndDetailSchemeVarXRef = conn.CreateCommand())
//                                            {
//                                              cmndDetailSchemeVarXRef.CommandText =
//                                                  "tActivityDetailProductSchemeVariantXRef";
//                                              cmndDetailSchemeVarXRef.CommandType = CommandType.TableDirect;
//                                              cmndDetailSchemeVarXRef.IndexName =
//                                                  "PK_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefnoAndProductSchemeVariantRefno";

//                                              using (SqlCeResultSet rsDetailSchemeVarXRef =
//                                                  cmndDetailSchemeVarXRef.ExecuteResultSet(ResultSetOptions.Updatable))
//                                              {
//                                                SqlCeUpdatableRecord recDetailSchemeVarXRef =
//                                                    rsDetailSchemeVarXRef.CreateRecord();

//                                                using (SQLiteCommand cmndID = conn.CreateCommand())
//                                                {
//                                                  cmndID.CommandText = "SELECT @@IDENTITY FROM tActivityDetail";
//                                                  cmndID.CommandType = CommandType.Text;

//                                                  int actDetailRefno = 0;
//                                                  int progValue = 0;
//                                                  int actDetailPhysicalFKRefno = 0;

//                                                  this.DataAccessProgressOutHandler(
//                                                      this,
//                                                      EventArgs.Empty,
//                                                      0,
//                                                      0,
//                                                      progMax);

//                                                  if (drItemCount.Seek(
//                                                      DbSeekOptions.FirstEqual,
//                                                      new object[] { actItemCountRefno }))
//                                                  {
//                                                    if (!drItemCount.Read())
//                                                    {
//#if MULTILANGUAGE
//                                                      throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DritemcountreadFailed));
//#else
//                                                      throw new Exception("drItemCount.Read() failed.");
//#endif
//                                                    }
//                                                  }
//                                                  else
//                                                  {
//                                                    throw new Exception(
//                                                      string.Format(
//                                                      "Unable to locate the activity item refno {0} in tActivityItemCount",
//                                                      actItemCountRefno));
//                                                  }

//                                                  foreach (DataRow drow in ds.Tables["tActivityDetailCount"].Rows)
//                                                  {
//                                                    errMsg = "Arguments:";

//                                                    recDetail.SetInt32(
//                                                        (int)SysCommon.ActDetailFields.SubType,
//                                                        (int)SysCommon.ActSubTypes.PhysicalCounts);
//                                                    recDetail.SetInt32(
//                                                        (int)SysCommon.ActDetailFields.ActivityItemRefno,
//                                                        actItemCountRefno);
//                                                    recDetail.SetInt32(
//                                                        (int)SysCommon.ActDetailFields.ProductUnitRefno,
//                                                        Convert.ToInt32(drItemCount.GetValue(
//                                                        (int)SysCommon.ActItemCountFields.ProdUnitRefno)));

//                                                    this.SaveDetailLocation(
//                                                        drow,
//                                                        drLocMapSlot,
//                                                        drLocMapSection,
//                                                        drLocMapWhs,
//                                                        ref recDetail,
//                                                        true);

//                                                    this.SaveDetail(drow, ref recDetail, true);

//                                                    rsDetail.Insert(recDetail);

//                                                    actDetailRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                                                    recDetailCount.SetInt32(
//                                                        (int)SysCommon.ActDetailCountFields.ActDetailRefno,
//                                                        actDetailRefno);

//                                                    actDetailPhysicalFKRefno = Convert.ToInt32(drow["f01"]);
//                                                    errMsg += Environment.NewLine 
//                                                        + "Detail fk refno " + actDetailPhysicalFKRefno;
//                                                    recDetailCount.SetInt32(
//                                                        (int)SysCommon.ActDetailCountFields.FKRefno,
//                                                        actDetailPhysicalFKRefno);

//                                                    rsDetailCount.Insert(recDetailCount);

//                                                    if (this.SaveCommonDetailVariant(
//                                                        drow,
//                                                        drSchemeVar,
//                                                        ref recDetailSchemeVarXRef,
//                                                        actDetailRefno,
//                                                        true))
//                                                    {
//                                                      rsDetailSchemeVarXRef.Insert(recDetailSchemeVarXRef);
//                                                    }

//                                                    this.SaveDetailExtra(
//                                                        drow,
//                                                        ref recExtra,
//                                                        actDetailRefno,
//                                                        true);

//                                                    rsExtra.Insert(recExtra);

//                                                    progValue++;
//                                                    if (progValue % 10 == 0)
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

//                                    drLocMapSection.Close();
//                                  }
//                                }

//                                drLocMapSlot.Close();
//                              }
//                            }

//                            drItemCount.Close();
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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingPhysicalCountDetailsComplete), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical count details complete.", false);
//#endif
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingPhysicalCountDetailsFailed), true);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical count details failed.", true);
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
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingPhysicalCountDetailsFailed) + Environment.NewLine + errMsg,
//#else
//            "Database exception occurred. Saving physical count details failed." + Environment.NewLine + errMsg,
//#endif
//            ex);
//      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemCountFKRefno"></param>
    /// <returns></returns>
    protected override string Create(int actItemCountFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);
 
      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>"
          + "<command>" +
          "<action>REQUEST PHYSICAL COUNT DETAILS</action>" 
          + SysCommon.GetRequestDefaults(false)
          + ("<itm_refno>" + actItemCountFKRefno.ToString() + "</itm_refno>")
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
