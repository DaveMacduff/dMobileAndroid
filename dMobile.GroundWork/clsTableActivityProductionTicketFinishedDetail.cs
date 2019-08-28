namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableActivityProductionTicketFinishedDetail : TableBase
  {
    #region Constructors

    public TableActivityProductionTicketFinishedDetail()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketFinishedDetail";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingFinishedTicketDetails), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket details.", true);
#endif

        if (actItemRefno == 0)
        {
          using (SqlCeCommand cmndActItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndActItem.CommandType = CommandType.Text;
            cmndActItem.CommandText = "SELECT tActivityItem.ActivityItemRefno"
                + " FROM tActivityItem"
                + " WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TicketFinished;

            using (SqlCeDataReader drActItem = cmndActItem.ExecuteReader())
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingFinishedTicketDetailsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket details complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingFinishedTicketDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingFinishedTicketDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding finished ticket details failed.", ex);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingFinishedTicketDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingFinishedTicketDetailsFailed), ex);
#else
        throw new Exception("Seeding finished ticket details failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingFinishedTicketDetails), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket details.", true);
#endif

        using (SqlCeCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {

          cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef"
              + " WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN"
              + " (SELECT tActivityDetail.ActivityDetailRefno"
              + " FROM tActivityDetail"
              + " WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.TicketFinished + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailProductionFinished"
              + " WHERE tActivityDetailProductionFinished.ActivityDetailRefno IN"
              + " (SELECT tActivityDetail.ActivityDetailRefno"
              + " FROM tActivityDetail"
              + " WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.TicketFinished + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailExtra"
              + " WHERE tActivityDetailExtra.ActivityDetailRefno IN"
              + " (SELECT tActivityDetail.ActivityDetailRefno"
              + " FROM tActivityDetail"
              + " WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.TicketFinished + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetail "
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.TicketFinished;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingFinishedTicketDetails), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket details.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingFinishedTicketDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingFinishedTicketDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging finished ticket details failed.", ex);
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
          cmndStatus.CommandText = "SELECT COUNT(tActivityDetail.ActivityDetailRefno)"
              + " FROM tActivityDetail"
              + " WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.TicketFinished;
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this,
              EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.FinishedTicketDetails), rowsAffected),
#else
              string.Format("{0} finished ticket details.", rowsAffected),
#endif
              true);
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredFinishedTicketDetailsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Finished ticket details status failed.", ex);
#endif
      }

      return rowsAffected;
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
      int actItemProductionFKRefno = 0;

      using (SqlCeCommand cmndItemFinished = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndItemFinished.CommandText = "tActivityItemProductionFinished";
        cmndItemFinished.CommandType = CommandType.TableDirect;
        cmndItemFinished.IndexName = "PK_ActivityItemProductionFinishedOnActivityItemRefno";
        using (SqlCeDataReader drItemFinished = cmndItemFinished.ExecuteReader())
        {
          if (drItemFinished.Seek(DbSeekOptions.FirstEqual, new object[] { actItemRefno }))
          {
            if (drItemFinished.Read())
            {
              actItemProductionFKRefno = drItemFinished.GetInt32(
                  (int)SysCommon.ActItemProdFinishedFields.FKRefno);
            }
            else
            {
#if MULTILANGUAGE
              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DritemfinishedreadFailed));
#else
              throw new Exception("drItemFinished.Read() failed.");
#endif
            }
          }
          else
          {
            throw new Exception(
                string.Format(
#if MULTILANGUAGE
                Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheActivityItemTicketFinishedInTactivityitemproductionfinished),
#else
                "Unable to locate the activity item ticket finished {0} in tActivityItemProductionFinished.",
#endif
                actItemRefno));
          }
        }
      }

      return this.SendComm(this.Create(actItemProductionFKRefno));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reqData"></param>
    /// <param name="actItemProductionRefno"></param>
    protected override void Save(string reqData, int actItemProductionRefno)
    {
      string errMsg = string.Empty;
      SqlCeConnection conn = DatabaseConnection.GetInstance().SQLConn;

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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingFinishedTicketDetails), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished ticket details.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailProduction"].Rows.Count);

#if MULTILANGUAGE
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), false);
#else
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);
#endif

            using (SqlCeCommand cmndDetailFinished = conn.CreateCommand())
            {
              cmndDetailFinished.CommandText = "tActivityDetailProductionFinished";
              cmndDetailFinished.CommandType = CommandType.TableDirect;
              cmndDetailFinished.IndexName
                  = "UQ_ActivityDetailProductionFinishedOnActivityDetailProductionFinishedFKRefno";

              using (SqlCeResultSet rsDetailFinished = cmndDetailFinished.ExecuteResultSet(ResultSetOptions.Updatable))
              {
                SqlCeUpdatableRecord recDetailFinished = rsDetailFinished.CreateRecord();

                using (SqlCeCommand cmndDetail = conn.CreateCommand())
                {
                  cmndDetail.CommandText = "tActivityDetail";
                  cmndDetail.CommandType = CommandType.TableDirect;
                  cmndDetail.IndexName = "PK_ActivityDetailOnActivityDetailRefno";

                  using (SqlCeResultSet rsDetail = cmndDetail.ExecuteResultSet(ResultSetOptions.Updatable))
                  {
                    SqlCeUpdatableRecord recDetail = rsDetail.CreateRecord();

                    using (SqlCeCommand cmndExtra = conn.CreateCommand())
                    {
                      cmndExtra.CommandText = "tActivityDetailExtra";
                      cmndExtra.CommandType = CommandType.TableDirect;
                      cmndExtra.IndexName = "PK_ActivityDetailExtraOnActivityDetailRefno";

                      using (SqlCeResultSet rsExtra = cmndExtra.ExecuteResultSet(ResultSetOptions.Updatable))
                      {
                        SqlCeUpdatableRecord recExtra = rsExtra.CreateRecord();

                        using (SqlCeCommand cmndItemProduction = conn.CreateCommand())
                        {
                          cmndItemProduction.CommandText = "tActivityItemProduction";
                          cmndItemProduction.CommandType = CommandType.TableDirect;
                          cmndItemProduction.IndexName = "PK_ActivityItemProductionOnActivityItemRefno";

                          using (SqlCeDataReader drItemProduction = cmndItemProduction.ExecuteReader())
                          {
                            using (SqlCeCommand cmndLocMapSlot = conn.CreateCommand())
                            {
                              cmndLocMapSlot.CommandText = "tLocationMapSlot";
                              cmndLocMapSlot.CommandType = CommandType.TableDirect;
                              cmndLocMapSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";

                              using (SqlCeDataReader drLocMapSlot = cmndLocMapSlot.ExecuteReader())
                              {
                                using (SqlCeCommand cmndLocMapSect = conn.CreateCommand())
                                {
                                  cmndLocMapSect.CommandText = "tLocationMapSection";
                                  cmndLocMapSect.CommandType = CommandType.TableDirect;
                                  cmndLocMapSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";

                                  using (SqlCeDataReader drLocMapSect = cmndLocMapSect.ExecuteReader())
                                  {
                                    using (SqlCeCommand cmndLocMapWhs = conn.CreateCommand())
                                    {
                                      cmndLocMapWhs.CommandText = "tLocationMapWarehouse";
                                      cmndLocMapWhs.CommandType = CommandType.TableDirect;
                                      cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

                                      using (SqlCeDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
                                      {
                                        using (SqlCeCommand cmndSchemeVar = conn.CreateCommand())
                                        {
                                          cmndSchemeVar.CommandText = "tProductSchemeVariant";
                                          cmndSchemeVar.CommandType = CommandType.TableDirect;
                                          cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";

                                          using (SqlCeDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
                                          {
                                            using (SqlCeCommand cmndSchemeVarXRef = conn.CreateCommand())
                                            {
                                              cmndSchemeVarXRef.CommandText = "tActivityDetailProductSchemeVariantXRef";
                                              cmndSchemeVarXRef.CommandType = CommandType.TableDirect;
                                              cmndSchemeVarXRef.IndexName
                                                = "PK_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefnoAndProductSchemeVariantRefno";

                                              using (SqlCeResultSet rsSchemeVarXRef
                                                  = cmndSchemeVarXRef.ExecuteResultSet(ResultSetOptions.Updatable))
                                              {
                                                SqlCeUpdatableRecord recSchemeVarXRef = rsSchemeVarXRef.CreateRecord();

                                                using (SqlCeCommand cmndID = conn.CreateCommand())
                                                {
                                                  cmndID.CommandText = "SELECT @@IDENTITY FROM tActivityDetail";
                                                  cmndID.CommandType = CommandType.Text;

                                                  int actDetailRefno = 0;
                                                  int progValue = 0;
                                                  int actDetailFKRefno = 0;

                                                  this.DataAccessProgressOutHandler(
                                                      this,
                                                      EventArgs.Empty,
                                                      0,
                                                      0,
                                                      progMax);

                                                  if (drItemProduction.Seek(
                                                      DbSeekOptions.FirstEqual,
                                                      new object[] { actItemProductionRefno }))
                                                  {
                                                    if (!drItemProduction.Read())
                                                    {
                                                      throw new Exception(
#if MULTILANGUAGE
                                                          Translator.GetInstance().Translate(TranslatorVocabulary.DritemproductionreadFailed)
#else
                                                          "drItemProduction.Read() failed."
#endif
                                                          + Environment.NewLine + errMsg);
                                                    }
                                                  }
                                                  else
                                                  {
                                                    throw new Exception(
                                                        string.Format(
#if MULTILANGUAGE
                                                        Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheActivityItemRefnoIn)
#else
                                                        "Unable to locate the activity item refno {0} in "
#endif
                                                        + "tActivityItemProduction",
                                                        actItemProductionRefno));
                                                  }

                                                  foreach (DataRow drow in ds.Tables["tActivityDetailProduction"].Rows)
                                                  {
                                                    errMsg = "Arguments:";

                                                    recDetail.SetInt32(
                                                        (int)SysCommon.ActDetailFields.SubType,
                                                        (int)SysCommon.ActSubTypes.TicketFinished);
                                                    recDetail.SetInt32(
                                                        (int)SysCommon.ActDetailFields.ActivityItemRefno,
                                                        actItemProductionRefno);
                                                    recDetail.SetInt32(
                                                        (int)SysCommon.ActDetailFields.ProductUnitRefno,
                                                        drItemProduction.GetInt32(
                                                        (int)SysCommon.ActItemProdFields.ProdUnitRefno));

                                                    this.SaveDetailLocation(
                                                        drow,
                                                        drLocMapSlot,
                                                        drLocMapSect,
                                                        drLocMapWhs,
                                                        ref recDetail,
                                                        false);

                                                    this.SaveDetail(drow, ref recDetail, false);

                                                    rsDetail.Insert(recDetail);

                                                    actDetailRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                                    recDetailFinished.SetInt32(
                                                        (int)SysCommon.ActDetailProdFinishedFields.ActDetailRefno,
                                                        actDetailRefno);

                                                    actDetailFKRefno = Convert.ToInt32(
                                                        drow["ActivityDetailProductionFKRefno"]);
                                                    errMsg = "FK detail refno " + actDetailFKRefno;
                                                    recDetailFinished.SetInt32(
                                                        (int)SysCommon.ActDetailProdFinishedFields.FKRefno,
                                                        actDetailFKRefno);

                                                    rsDetailFinished.Insert(recDetailFinished);

                                                    if (this.SaveCommonDetailVariant(
                                                        drow,
                                                        drSchemeVar,
                                                        ref recSchemeVarXRef,
                                                        actDetailRefno,
                                                        false))
                                                    {
                                                      rsSchemeVarXRef.Insert(recSchemeVarXRef);
                                                    }

                                                    this.SaveDetailExtra(
                                                      drow,
                                                      ref recExtra,
                                                      actDetailRefno,
                                                      false);

                                                    rsExtra.Insert(recExtra);

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

                                                  this.DataAccessProgressOutHandler(
                                                      this,
                                                      EventArgs.Empty,
                                                      progMax,
                                                      0,
                                                      progMax);
                                                }
                                              }
                                            }

                                            drSchemeVar.Close();
                                          }
                                        }

                                        drLocMapWhs.Close();
                                      }
                                    }

                                    drLocMapSect.Close();
                                  }
                                }

                                drLocMapSlot.Close();
                              }
                            }

                            drItemProduction.Close();
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
          }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingFinishedTicketDetailsComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished ticket details complete.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingFinishedTicketDetailsFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished ticket details failed.", false);
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
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingFinishedTicketDetailsFailed) + Environment.NewLine + errMsg,
#else
            "Database exception occurred. Saving finished ticket details failed." + Environment.NewLine + errMsg,
#endif
            ex);
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Create(int actItemProductionFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>"
          + "<action>REQUEST PRODUCTION TICKET FINISHED DETAILS</action>" 
          + SysCommon.GetRequestDefaults(false)
          + ("<itm_refno>" + actItemProductionFKRefno.ToString() + "</itm_refno>") 
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
