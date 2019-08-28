namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using Microsoft.VisualBasic;

  public class TableActivityReceivingDetail : TableBase
  {
    #region Constructors

    public TableActivityReceivingDetail()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityReceivingDetail";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingReceivingDetails), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving details.", true);
#endif

        if (actItemRefno == 0)
        {
          using (SqlCeCommand cmndRecvItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndRecvItem.CommandType = CommandType.Text;
            cmndRecvItem.CommandText = "SELECT tActivityItem.ActivityItemRefno" 
                + " FROM tActivityItem" 
                + " WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Receiving;

            using (SqlCeDataReader drRecvItem = cmndRecvItem.ExecuteReader())
            {
              while (drRecvItem.Read())
              {
                actItemRefno = Convert.ToInt32(drRecvItem["ActivityItemRefno"]);
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

              drRecvItem.Close();
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingReceivingDetailsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving details complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingReceivingDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingReceivingDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding receiving details failed.", ex);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingReceivingDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingReceivingDetailsFailed), ex);
#else
        throw new Exception("Seeding receiving details failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingReceivingDetails), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving details.", true);
#endif

        using (SqlCeCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef " 
              + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN " 
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail "
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Receiving + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailReceiving " 
              + "WHERE tActivityDetailReceiving.ActivityDetailRefno IN " 
              + "(SELECT tActivityDetail.ActivityDetailRefno "
              + "FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Receiving + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailExtra " 
              + "WHERE tActivityDetailExtra.ActivityDetailRefno IN " 
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Receiving + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetail " + 
              "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Receiving;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingReceivingDetailsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving details complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingReceivingDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingReceivingDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging receiving details failed.", ex);
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
              + " WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Receiving;
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());
          
          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ReceivingDetails), rowsAffected), 
#else
              string.Format("{0} receiving details.", rowsAffected), 
#endif
              true);
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredReceivingDetailsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Receiving details status failed.", ex);
#endif
      }

      return rowsAffected;
    }

    #endregion

    #region Private Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemRecvRefno"></param>
    /// <returns></returns>
    protected override string Send(int actItemRecvRefno)
    {
      string reqData = string.Empty;
      int actItemRecvFKRefno = 0;

      using (SqlCeCommand cmndItemRecv = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndItemRecv.CommandText = "tActivityItemReceiving";
        cmndItemRecv.CommandType = CommandType.TableDirect;
        cmndItemRecv.IndexName = "PK_ActivityItemReceivingOnActivityItemRefno";

        using (SqlCeDataReader drItemRecv = cmndItemRecv.ExecuteReader())
        {
          if (drItemRecv.Seek(DbSeekOptions.FirstEqual, new object[] { actItemRecvRefno }))
          {
            if (drItemRecv.Read())
            {
              actItemRecvFKRefno = drItemRecv.GetInt32((int)SysCommon.ActItemReceivingFields.FKRefno);
            }
            else
            {
#if MULTILANGUAGE
              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DritemrecvreadFailed));
#else
              throw new Exception("drItemRecv.Read() failed.");
#endif
            }
          }
          else
          {
            throw new Exception(
                string.Format(
#if MULTILANGUAGE
                Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheItemReceivingRefnoInTactivityitemreceiving),
#else
                "Unable to locate the item receiving refno {0} in tActivityItemReceiving",
#endif
                actItemRecvRefno));
          }
        }
      }

      reqData = this.SendComm(this.Create(actItemRecvFKRefno));

      return reqData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reqData"></param>
    /// <param name="actItemRecvRefno"></param>
    protected override void Save(string reqData, int actItemRecvRefno)
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingReceivingDetails), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving details.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailPurchaseOrder"].Rows.Count);

            this.DataAccessStatusOutHandler(
                this, 
                EventArgs.Empty, 
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax),
#else
                string.Format("Records {0}", progMax),
#endif
                false);

            using (SqlCeCommand cmndDetailRecv = conn.CreateCommand())
            {
              cmndDetailRecv.CommandText = "tActivityDetailReceiving";
              cmndDetailRecv.CommandType = CommandType.TableDirect;
              cmndDetailRecv.IndexName = "UQ_ActivityDetailReceivingOnActivityDetailReceivingFKRefno";

              using (SqlCeResultSet rsDetailRecv = cmndDetailRecv.ExecuteResultSet(ResultSetOptions.Updatable))
              {
                SqlCeUpdatableRecord recDetailRecv = rsDetailRecv.CreateRecord();

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

                        using (SqlCeCommand cmndItemRecv = conn.CreateCommand())
                        {
                          cmndItemRecv.CommandText = "tActivityItemReceiving";
                          cmndItemRecv.CommandType = CommandType.TableDirect;
                          cmndItemRecv.IndexName = "PK_ActivityItemReceivingOnActivityItemRefno";

                          using (SqlCeDataReader drItemRecv = cmndItemRecv.ExecuteReader())
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
                                                  int recvDetailFKRefno = 0;

                                                  this.DataAccessProgressOutHandler(
                                                      this, 
                                                      EventArgs.Empty, 
                                                      0, 
                                                      0, 
                                                      progMax);

                                                  if (drItemRecv.Seek(
                                                      DbSeekOptions.FirstEqual,
                                                      new object[] { actItemRecvRefno }))
                                                  {
                                                    if (!drItemRecv.Read())
                                                    {
#if MULTILANGUAGE
                                                      throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DritemrecvreadFailedz));
#else
                                                      throw new Exception("!drItemRecv.Read() failed.");
#endif
                                                    }
                                                  }
                                                  else
                                                  {
                                                    throw new Exception(
                                                        string.Format(
#if MULTILANGUAGE
                                                        Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheItemRefnoInTactivityitemreceiving),
#else
                                                        "Unable to locate the item refno {0} in tActivityItemReceiving",
#endif
                                                        actItemRecvRefno));
                                                  }

                                                  foreach (DataRow drow in ds.Tables["tActivityDetailPurchaseOrder"].Rows)
                                                  {
                                                    errMsg = "Arguments:";

                                                    recDetail.SetInt32(
                                                        (int)SysCommon.ActDetailFields.SubType, 
                                                        (int)SysCommon.ActSubTypes.Receiving);
                                                    recDetail.SetInt32(
                                                        (int)SysCommon.ActDetailFields.ActivityItemRefno, 
                                                        actItemRecvRefno);
                                                    recDetail.SetInt32(
                                                        (int)SysCommon.ActDetailFields.ProductUnitRefno, 
                                                        Convert.ToInt32(
                                                        drItemRecv.GetValue(
                                                        (int)SysCommon.ActItemReceivingFields.ProdUnitRefno)));

                                                    this.SaveDetailLocation(
                                                        drow,
                                                        drLocMapSlot,
                                                        drLocMapSection,
                                                        drLocMapWhs,
                                                        ref recDetail,
                                                        false);

                                                    this.SaveDetail(drow, ref recDetail, false);

                                                    rsDetail.Insert(recDetail);

                                                    actDetailRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                                    recDetailRecv.SetInt32(
                                                        (int)SysCommon.ActDetailReceivingFields.ActDetailRefno, 
                                                        actDetailRefno);

                                                    recvDetailFKRefno = Convert.ToInt32(
                                                        drow["ActivityDetailPurchaseOrderFKRefno"]);
                                                    errMsg += Environment.NewLine + "Detail fk refno " 
                                                        + recvDetailFKRefno;
                                                    recDetailRecv.SetInt32(
                                                        (int)SysCommon.ActDetailReceivingFields.FKRefno,
                                                        recvDetailFKRefno);

                                                    rsDetailRecv.Insert(recDetailRecv);

                                                    if (this.SaveCommonDetailVariant(
                                                        drow,
                                                        drSchemeVar,
                                                        ref recDetailSchemeVarXRef,
                                                        actDetailRefno,
                                                        false))
                                                    {
                                                      rsDetailSchemeVarXRef.Insert(recDetailSchemeVarXRef);
                                                    }

                                                    this.SaveDetailExtra(
                                                        drow,
                                                        ref recExtra,
                                                        actDetailRefno,
                                                        false);

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

                            drItemRecv.Close();
                          }
                        }
                      }
                    }
                  }
                }
              }
            }

#if MULTILANGUAGE
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingReceivingDetailsComplete), false);
#else
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving details complete.", false);
#endif
          }
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingReceivingDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving details failed.", true);
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
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingReceivingDetailsFailed) + Environment.NewLine + errMsg,
#else
            "Database exception occurred. Saving receiving details failed." + Environment.NewLine + errMsg,
#endif
            ex);
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Create(int iActivityItemReceivingFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>"
          + "<action>REQUEST PURCHASE ORDER DETAILS</action>"
          + SysCommon.GetRequestDefaults(false)
          + ("<itm_refno>" + iActivityItemReceivingFKRefno.ToString() + "</itm_refno>") 
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
