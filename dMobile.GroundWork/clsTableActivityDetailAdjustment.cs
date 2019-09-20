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

  public class TableActivityAdjustmentDetail : TableBase
  {
    #region Constructors

    public TableActivityAdjustmentDetail()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityAdjustmentDetail";
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

#if MULTILANGUAGE
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingAdjustmentDetails), true);
#else
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustment details.", true);
#endif

      if (actItemRefno == 0)
      {
        using (SQLiteCommand cmndAdjItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndAdjItem.CommandType = CommandType.Text;
          cmndAdjItem.CommandText = "SELECT tActivityItem.ActivityItemRefno "
              + "FROM tActivityItem "
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Adjustment;

          using (SQLiteDataReader drAdjItem = cmndAdjItem.ExecuteReader())
          {
            while (drAdjItem.Read())
            {
              actItemRefno = Convert.ToInt32(drAdjItem["ActivityItemRefno"]);
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

            drAdjItem.Close();
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
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingAdjustmentDetailsComplete), true);
#else
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustment details complete.", true);
#endif
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
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Adjustment;
          
          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
                this, 
                EventArgs.Empty, 
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.AdjustmentDetails), rowsAffected), 
#else
                string.Format("{0} adjustment details.", rowsAffected), 
#endif
                true);
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredAdjustmentDetailsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Adjustment details status failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingAdjustmentDetails), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustment details.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef " 
              + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN " 
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Adjustment + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailAdjustment " 
              + "WHERE tActivityDetailAdjustment.ActivityDetailRefno IN " 
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Adjustment + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailExtra "
              + "WHERE tActivityDetailExtra.ActivityDetailRefno IN " 
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Adjustment + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Adjustment;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingAdjustmentDetailsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustment details complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingAdjustmentDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustment details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingAdjustmentDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging adjustment details failed.", ex);
#endif
      }
    }

    #endregion

    #region Private Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemAdjRefno"></param>
    /// <returns></returns>
    protected override string Send(int actItemAdjRefno)
    {
      string reqData = string.Empty;
      int actItemAdjFKRefno = 0;

//      using (SQLiteCommand cmndItemAdjustment = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//      {
//        cmndItemAdjustment.CommandText = "tActivityItemAdjustment";
//        cmndItemAdjustment.CommandType = CommandType.TableDirect;
//        cmndItemAdjustment.IndexName = "PK_ActivityItemAdjustmentOnActivityItemRefno";

//        using (SQLiteDataReader drItemAdj = cmndItemAdjustment.ExecuteReader())
//        {
//          if (drItemAdj.Seek(DbSeekOptions.FirstEqual, new object[] { actItemAdjRefno }))
//          {
//            if (drItemAdj.Read())
//            {
//              actItemAdjFKRefno = Convert.ToInt32(
//                  drItemAdj.GetValue((int)SysCommon.ActItemAdjFields.FKRefno));
//            }
//            else
//            {
//              throw new Exception("drItemAdj.Read()");
//            }
//          }
//          else
//          {
//            throw new Exception(
//#if MULTILANGUAGE
//              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateItemAdjustmentInTactivityitemadjustment), actItemAdjRefno));
//#else
//              string.Format("Unable to locate item adjustment {0} in tActivityItemAdjustment", actItemAdjRefno));
//#endif
//          }
//        }
//      }

      return this.SendComm(this.Create(actItemAdjFKRefno));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reqData"></param>
    /// <param name="actItemAdjRefno"></param>
    protected override void Save(string reqData, int actItemAdjRefno)
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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingAdjustmentDetails), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustment details.", false);
//#endif

//          // Receiving schema but no data results in no table being created in the dataset.
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailAdjustment"].Rows.Count);

//            this.DataAccessStatusOutHandler(
//              this, 
//              EventArgs.Empty, 
//#if MULTILANGUAGE
//              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), 
//#else
//              string.Format("Records {0}", progMax), 
//#endif
//              false);

//            using (SQLiteCommand cmndDetailAdj = conn.CreateCommand())
//            {
//              cmndDetailAdj.CommandText = "tActivityDetailAdjustment";
//              cmndDetailAdj.CommandType = CommandType.TableDirect;
//              cmndDetailAdj.IndexName = "UQ_ActivityDetailAdjustmentOnActivityDetailAdjustmentFKRefno";

//              using (SqlCeResultSet rsDetailAdj = cmndDetailAdj.ExecuteResultSet(ResultSetOptions.Updatable))
//              {
//                SqlCeUpdatableRecord recDetailAdj = rsDetailAdj.CreateRecord();

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

//                        using (SQLiteCommand cmndItemAdj = conn.CreateCommand())
//                        {
//                          cmndItemAdj.CommandText = "tActivityItemAdjustment";
//                          cmndItemAdj.CommandType = CommandType.TableDirect;
//                          cmndItemAdj.IndexName = "PK_ActivityItemAdjustmentOnActivityItemRefno";

//                          using (SQLiteDataReader drItemAdj = cmndItemAdj.ExecuteReader())
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
//                                                  int actDetailAdjFKRefno = 0;

//                                                  this.DataAccessProgressOutHandler(
//                                                      this, 
//                                                      EventArgs.Empty, 
//                                                      0, 
//                                                      0, 
//                                                      progMax);

//                                                  if (drItemAdj.Seek(
//                                                      DbSeekOptions.FirstEqual,
//                                                      new object[] { actItemAdjRefno }))
//                                                  {
//                                                    if (!drItemAdj.Read())
//                                                    {
//                                                      throw new Exception("drItemAdj.Read() failed.");
//                                                    }
//                                                  }
//                                                  else
//                                                  {
//                                                    throw new Exception(
//                                                        string.Format(
//#if MULTILANGUAGE
//                                                        Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheItemAdjustmentRefnoInTactivityitemadjustment),
//#else
//                                                        "Unable to locate the item adjustment refno {0} in tActivityItemAdjustment",
//#endif
//                                                        actItemAdjRefno));
//                                                  }

//                                                  foreach (DataRow drow in ds.Tables["tActivityDetailAdjustment"].Rows)
//                                                  {
//#if MULTILANGUAGE
//                                                    errMsg = Translator.GetInstance().Translate(TranslatorVocabulary.Arguments);
//#else
//                                                    errMsg = "Arguments:";
//#endif

//                                                    recDetail.SetInt32(
//                                                        (int)SysCommon.ActDetailFields.SubType, 
//                                                        (int)SysCommon.ActSubTypes.Adjustment);
//                                                    recDetail.SetInt32(
//                                                        (int)SysCommon.ActDetailFields.ActivityItemRefno, 
//                                                        actItemAdjRefno);
//                                                    recDetail.SetInt32(
//                                                        (int)SysCommon.ActDetailFields.ProductUnitRefno, 
//                                                        Convert.ToInt32(
//                                                        drItemAdj.GetValue(
//                                                        (int)SysCommon.ActItemAdjFields.ProdUnitRefno)));

//                                                    this.SaveDetailLocation(
//                                                        drow,
//                                                        drLocMapSlot,
//                                                        drLocMapSection,
//                                                        drLocMapWhs,
//                                                        ref recDetail,
//                                                        false);

//                                                    this.SaveDetail(drow, ref recDetail, false);

//                                                    rsDetail.Insert(recDetail);

//                                                    actDetailRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                                                    recDetailAdj.SetInt32(
//                                                        (int)SysCommon.ActDetailAdjFields.ActDetailRefno, 
//                                                        actDetailRefno);

//                                                    actDetailAdjFKRefno = Convert.ToInt32(
//                                                        drow["ActivityDetailAdjustmentFKRefno"]);
//                                                    errMsg += Environment.NewLine 
//                                                        + "Detail fk refno " + actDetailAdjFKRefno;
//                                                    recDetailAdj.SetInt32(
//                                                        (int)SysCommon.ActDetailAdjFields.FKRefno, 
//                                                        actDetailAdjFKRefno);

//                                                    rsDetailAdj.Insert(recDetailAdj);

//                                                    if (this.SaveCommonDetailVariant(
//                                                        drow,
//                                                        drSchemeVar,
//                                                        ref recDetailSchemeVarXRef,
//                                                        actDetailRefno,
//                                                        false))
//                                                    {
//                                                      rsDetailSchemeVarXRef.Insert(recDetailSchemeVarXRef);
//                                                    }

//                                                    this.SaveDetailExtra(
//                                                        drow,
//                                                        ref recExtra,
//                                                        actDetailRefno,
//                                                        false);

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

//                            drItemAdj.Close();
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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingAdjustmentDetailsComplete), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustment details complete.", false);
//#endif
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingAdjustmentDetailsFailed), true);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustment details failed.", true);
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
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingAdjustmentDetailsFailed) + Environment.NewLine + errMsg,
//#else
//            "Database exception occurred. Saving adjustment details failed." + Environment.NewLine + errMsg,
//#endif
//            ex);
//      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemAdjFKRefno"></param>
    /// <returns></returns>
    protected override string Create(int actItemAdjFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>" 
          + "<action>REQUEST ADJUSTMENT DETAILS</action>" 
          + SysCommon.GetRequestDefaults(false)
          + ("<itm_refno>" + actItemAdjFKRefno + "</itm_refno>") 
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
