namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using Microsoft.VisualBasic;

  public class TableActivityCycleCountDetail : TableBase
  {
    #region Constructors

    public TableActivityCycleCountDetail()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityCycleCountDetail";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingCycleCountDetails), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding cycle count details.", true);
#endif

        if (actItemRefno == 0)
        {
          using (SqlCeCommand cmndCountItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndCountItem.CommandType = CommandType.Text;
            cmndCountItem.CommandText = "SELECT tActivityItem.ActivityItemRefno"
                + " FROM tActivityItem"
                + " WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.CycleCounts;

            using (SqlCeDataReader drCountDetail = cmndCountItem.ExecuteReader())
            {
              while (drCountDetail.Read())
              {
                actItemRefno = Convert.ToInt32(drCountDetail["ActivityItemRefno"]);

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

              drCountDetail.Close();
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingCycleCountDetailsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding cycle count details complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.RequestCycleCountDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Request cycle count details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredRequestCycleCountDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Request cycle count details failed.", ex);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.RequestCycleCountDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Request cycle count details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.RequestCycleCountDetailsFailed), ex);
#else
        throw new Exception("Request cycle count details failed.", ex);
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
          cmndStatus.CommandText = "SELECT COUNT(tActivityDetail.ActivityDetailRefno) " 
              + "FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.CycleCounts;
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.CycleCountDetails), rowsAffected),
#else
              string.Format("{0} cycle count detail(s)", rowsAffected),
#endif
              true);
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredCycleCountDetailsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Cycle count details status failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingCycleCountDetails), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle count details.", true);
#endif

        using (SqlCeCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef " 
              + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail "
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.CycleCounts + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailCount " 
              + "WHERE tActivityDetailCount.ActivityDetailRefno IN " 
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.CycleCounts + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailExtra " 
              + "WHERE tActivityDetailExtra.ActivityDetailRefno IN " 
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.CycleCounts + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.CycleCounts;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingCycleCountDetailsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle count details complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingCycleCountDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle count details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingCycleCountDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging cycle count details failed.", ex);
#endif
      }
    }

    #endregion

    #region Protected Methods

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Send(int actItemCountRefno)
    {
      int actItemCountFKRefno = 0;

      using (SqlCeCommand cmndItemCount = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndItemCount.CommandText = "tActivityItemCount";
        cmndItemCount.CommandType = CommandType.TableDirect;
        cmndItemCount.IndexName = "PK_ActivityItemCountOnActivityItemRefno";

        using (SqlCeDataReader drItemCount = cmndItemCount.ExecuteReader())
        {
          if (drItemCount.Seek(DbSeekOptions.FirstEqual, new object[] { actItemCountRefno }))
          {
            if (drItemCount.Read())
            {
              actItemCountFKRefno = Convert.ToInt32(drItemCount.GetValue((int)SysCommon.ActItemCountFields.FKRefno));
            }
            else
            {
#if MULTILANGUAGE
              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DritemcountreadFailed));
#else
              throw new Exception("drItemCount.Read() failed.");
#endif
            }
          }
          else
          {
            throw new Exception(
                string.Format(
#if MULTILANGUAGE
                Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheItemCycleCountRefnoInTactivityitemcount), 
#else
                "Unable to locate the item cycle count refno {0} in tActivityItemCount.", 
#endif
                actItemCountRefno));
          }
        }
      }

      return this.SendComm(this.Create(actItemCountFKRefno));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reqData"></param>
    /// <param name="actItemCountRefno"></param>
    protected override void Save(string reqData, int actItemCountRefno)
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingCycleCountDetails), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle count details.", false);
#endif

          // Receiving schema but no data results in no table being created in the dataset.
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailCount"].Rows.Count);

            this.DataAccessStatusOutHandler(
                this, 
                EventArgs.Empty, 
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Records), progMax),
#else
                string.Format("Records {0}.", progMax),
#endif
                false);

            using (SqlCeCommand cmndDetailCount = conn.CreateCommand())
            {
              cmndDetailCount.CommandText = "tActivityDetailCount";
              cmndDetailCount.CommandType = CommandType.TableDirect;
              cmndDetailCount.IndexName = "UQ_ActivityDetailCountOnActivityDetailCountFKRefno";

              using (SqlCeResultSet rsDetailCount = 
                  cmndDetailCount.ExecuteResultSet(ResultSetOptions.Updatable))
              {
                SqlCeUpdatableRecord recDetailCount = rsDetailCount.CreateRecord();

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

                        using (SqlCeCommand cmndItemCount = conn.CreateCommand())
                        {
                          cmndItemCount.CommandText = "tActivityItemCount";
                          cmndItemCount.CommandType = CommandType.TableDirect;
                          cmndItemCount.IndexName = "PK_ActivityItemCountOnActivityItemRefno";

                          using (SqlCeDataReader drItemCount = cmndItemCount.ExecuteReader())
                          {
                            using (SqlCeCommand cmndLocMapSlot = conn.CreateCommand())
                            {
                              cmndLocMapSlot.CommandText = "tLocationMapSlot";
                              cmndLocMapSlot.CommandType = CommandType.TableDirect;
                              cmndLocMapSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";

                              using (SqlCeDataReader drLocMapSlot = cmndLocMapSlot.ExecuteReader())
                              {
                                using (SqlCeCommand cmndLocMapSection = conn.CreateCommand())
                                {
                                  cmndLocMapSection.CommandText = "tLocationMapSection";
                                  cmndLocMapSection.CommandType = CommandType.TableDirect;
                                  cmndLocMapSection.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";

                                  using (SqlCeDataReader drLocMapSection = cmndLocMapSection.ExecuteReader())
                                  {
                                    using (SqlCeCommand cmndLocMapWhs = conn.CreateCommand())
                                    {
                                      cmndLocMapWhs.CommandText = "tLocationMapWarehouse";
                                      cmndLocMapWhs.CommandType = CommandType.TableDirect;
                                      cmndLocMapWhs.IndexName = 
                                          "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

                                      using (SqlCeDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
                                      {
                                        using (SqlCeCommand cmndSchemeVar = conn.CreateCommand())
                                        {
                                          cmndSchemeVar.CommandText = "tProductSchemeVariant";
                                          cmndSchemeVar.CommandType = CommandType.TableDirect;
                                          cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";

                                          using (SqlCeDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
                                          {
                                            using (SqlCeCommand cmndDetailSchemeVarXRef = conn.CreateCommand())
                                            {
                                              cmndDetailSchemeVarXRef.CommandText = 
                                                  "tActivityDetailProductSchemeVariantXRef";
                                              cmndDetailSchemeVarXRef.CommandType = CommandType.TableDirect;
                                              cmndDetailSchemeVarXRef.IndexName = 
                                                  "PK_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefnoAndProductSchemeVariantRefno";

                                              using (SqlCeResultSet rsDetailSchemeVarXRef =
                                                  cmndDetailSchemeVarXRef.ExecuteResultSet(ResultSetOptions.Updatable))
                                              {
                                                SqlCeUpdatableRecord recDetailSchemeVarXRef = 
                                                    rsDetailSchemeVarXRef.CreateRecord();

                                                using (SqlCeCommand cmndID = conn.CreateCommand())
                                                {
                                                  cmndID.CommandText = "SELECT @@IDENTITY FROM tActivityDetail";
                                                  cmndID.CommandType = CommandType.Text;

                                                  int actDetailRefno = 0;
                                                  int progValue = 0;
                                                  int actDetailCycleFKRefno = 0;

                                                  this.DataAccessProgressOutHandler(
                                                      this, 
                                                      EventArgs.Empty, 
                                                      0, 
                                                      0,
                                                      progMax);

                                                  if (drItemCount.Seek(
                                                      DbSeekOptions.FirstEqual,
                                                      new object[] { actItemCountRefno }))
                                                  {
                                                    if (!drItemCount.Read())
                                                    {
#if MULTILANGUAGE
                                                      throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DritemcountreadFailed));
#else
                                                      throw new Exception("drItemCount.Read() failed.");
#endif
                                                    }
                                                  }
                                                  else
                                                  {
                                                    throw new Exception(
                                                      string.Format(
#if MULTILANGUAGE
                                                      Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheActivityItemRefnoInTactivityitemcount),
#else
                                                      "Unable to locate the activity item refno {0} in tActivityItemCount",
#endif
                                                      actItemCountRefno));
                                                  }

                                                  foreach (DataRow drow in ds.Tables["tActivityDetailCount"].Rows)
                                                  {
#if MULTILANGUAGE
                                                    errMsg = Translator.GetInstance().Translate(TranslatorVocabulary.Arguments);
#else
                                                    errMsg = "Arguments:";
#endif

                                                    recDetail.SetInt32(
                                                        (int)SysCommon.ActDetailFields.SubType, 
                                                        (int)SysCommon.ActSubTypes.CycleCounts);
                                                    recDetail.SetInt32(
                                                        (int)SysCommon.ActDetailFields.ActivityItemRefno, 
                                                        actItemCountRefno);
                                                    recDetail.SetInt32(
                                                        (int)SysCommon.ActDetailFields.ProductUnitRefno, 
                                                        drItemCount.GetInt32(
                                                        (int)SysCommon.ActItemCountFields.ProdUnitRefno));

                                                    this.SaveDetailLocation(
                                                        drow,
                                                        drLocMapSlot,
                                                        drLocMapSection,
                                                        drLocMapWhs,
                                                        ref recDetail,
                                                        true);

                                                    this.SaveDetail(drow, ref recDetail, true);

                                                    rsDetail.Insert(recDetail);

                                                    actDetailRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                                    recDetailCount.SetInt32(
                                                        (int)SysCommon.ActDetailCountFields.ActDetailRefno, 
                                                        actDetailRefno);

                                                    actDetailCycleFKRefno = Convert.ToInt32(drow["f01"]);
                                                    errMsg += Environment.NewLine 
#if MULTILANGUAGE
                                                        + Translator.GetInstance().Translate(TranslatorVocabulary.DetailFkRefno) + actDetailCycleFKRefno;
#else
                                                        + "Detail fk refno" + actDetailCycleFKRefno;
#endif
                                                    recDetailCount.SetInt32(
                                                        (int)SysCommon.ActDetailCountFields.FKRefno, 
                                                        actDetailCycleFKRefno);

                                                    rsDetailCount.Insert(recDetailCount);

                                                    if (this.SaveCommonDetailVariant(
                                                        drow,
                                                        drSchemeVar,
                                                        ref recDetailSchemeVarXRef,
                                                        actDetailRefno,
                                                        true))
                                                    {
                                                      rsDetailSchemeVarXRef.Insert(recDetailSchemeVarXRef);
                                                    }

                                                    this.SaveDetailExtra(
                                                        drow,
                                                        ref recExtra,
                                                        actDetailRefno,
                                                        true);

                                                    rsExtra.Insert(recExtra);

                                                    progValue++;
                                                    if (progValue % 10 == 0)
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

                                    drLocMapSection.Close();
                                  }
                                }

                                drLocMapSlot.Close();
                              }
                            }

                            drItemCount.Close();
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingCycleCountDetailsComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle count details complete.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingCycleCountDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle count details failed.", true);
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
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingCycleCountDetailsFailed) + Environment.NewLine + errMsg,
#else
            "Database exception occurred. Saving cycle count details failed." + Environment.NewLine + errMsg,
#endif
            ex);
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Create(int actItemCountFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>" 
          + "<action>REQUEST CYCLE COUNT DETAILS</action>" 
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
