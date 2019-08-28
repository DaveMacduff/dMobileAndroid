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

  public class TableActivityTransferDetail : TableBase
  {
    #region Constructors

    public TableActivityTransferDetail()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityTransferDetail";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTransferDetails), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer details.", true);
#endif

        if (actItemRefno == 0)
        {
          using (SqlCeCommand cmndActItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndActItem.CommandType = CommandType.Text;
            cmndActItem.CommandText = "SELECT tActivityItem.ActivityItemRefno " 
                + "FROM tActivityItem "
                + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Transfers;

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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTransferDetailsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer details complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingCoproductMaterialsDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding co-product materials details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingTransferDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding transfer details failed.", ex);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTransferDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTransferDetailsFailed), ex);
#else
        throw new Exception("Seeding transfer details failed.", ex);
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
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Transfers;
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());
          DataAccessStatusOutHandler(
              this,
              EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.TransferDetails),
#else
              string.Format("{0} transfer details.",
#endif
              rowsAffected),
              true);
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredTransferDetailsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Transfer details status failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTransferDetails), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer details.", true);
#endif

        using (SqlCeCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {

          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
              + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail "
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Transfers + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailTransfer "
              + "WHERE tActivityDetailTransfer.ActivityDetailRefno IN "
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail "
              + " WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Transfers + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailExtra "
              + "WHERE tActivityDetailExtra.ActivityDetailRefno IN "
              + "(SELECT tActivityDetail.ActivityDetailRefno "
              + "FROM tActivityDetail "
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Transfers + ")";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = (string)("DELETE FROM tActivityDetail" +
              " WHERE tActivityDetail.ActivityDetailSubType = " +
              Convert.ToInt32(SysCommon.ActSubTypes.Transfers));
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTransferDetailsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer details complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTransferDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingTransferDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging transfer details failed.", ex);
#endif
      }
    }

    #endregion

    #region Private Members

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Send(int actItemRefno)
    {
      int actItemFKRefno = 0;

      using (SqlCeCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndItem.CommandText = "tActivityItemTransfer";
        cmndItem.CommandType = CommandType.TableDirect;
        cmndItem.IndexName = "PK_ActivityItemTransferOnActivityItemRefno";

        using (SqlCeDataReader drItem = cmndItem.ExecuteReader())
        {
          if (drItem.Seek(DbSeekOptions.FirstEqual, new object[] { actItemRefno }))
          {
            if (drItem.Read())
            {
              actItemFKRefno = drItem.GetInt32((int)SysCommon.ActItemTransferFields.FKRefno);
            }
            else
            {
#if MULTILANGUAGE
              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DritemreadFailed));
#else
              throw new Exception("drItem.Read() failed.");
#endif
            }
          }
          else
          {
            throw new Exception(
                string.Format(
#if MULTILANGUAGE
                Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateToActivityItemTransferRefnoInTactivityitemtransfer),
#else
                "Unable to locate to activity item transfer refno {0} in tActivityItemTransfer.",
#endif
                actItemRefno));
          }
        }
      }

      return this.SendComm(this.Create(actItemFKRefno));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="saveData"></param>
    /// <param name="actItemTransferRefno"></param>
    protected override void Save(string saveData, int actItemTransferRefno)
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
          ds.ReadXml(SysCommon.GetXmlReader(ref saveData), XmlReadMode.Auto);

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTransferDetails), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer details.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailTransfer"].Rows.Count);

#if MULTILANGUAGE
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), false);
#else
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);
#endif

            using (SqlCeCommand cmndDetailTransfer = conn.CreateCommand())
            {
              cmndDetailTransfer.CommandText = "tActivityDetailTransfer";
              cmndDetailTransfer.CommandType = CommandType.TableDirect;
              cmndDetailTransfer.IndexName = "UQ_ActivityDetailTransferOnActivityDetailTransferFKRefno";

              using (SqlCeResultSet rsDetailTransfer = cmndDetailTransfer.ExecuteResultSet(ResultSetOptions.Updatable))
              {
                SqlCeUpdatableRecord recDetailTransfer = rsDetailTransfer.CreateRecord();

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

                        using (SqlCeCommand cmndItemTransfer = conn.CreateCommand())
                        {
                          cmndItemTransfer.CommandText = "tActivityItemTransfer";
                          cmndItemTransfer.CommandType = CommandType.TableDirect;
                          cmndItemTransfer.IndexName = "PK_ActivityItemTransferOnActivityItemRefno";

                          using (SqlCeDataReader drItemTransfer = cmndItemTransfer.ExecuteReader())
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
                                                  int actDetailTransferFKRefno = 0;

                                                  this.DataAccessProgressOutHandler(
                                                      this, 
                                                      EventArgs.Empty, 
                                                      0, 
                                                      0, 
                                                      progMax);

                                                  if (drItemTransfer.Seek(
                                                      DbSeekOptions.FirstEqual,
                                                      new object[] { actItemTransferRefno }))
                                                  {
                                                    if (!drItemTransfer.Read())
                                                    {
#if MULTILANGUAGE
                                                      throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DritemtransferreadFailed)
#else
                                                      throw new Exception("drItemTransfer.Read() failed."
#endif
                                                          + Environment.NewLine + errMsg);
                                                    }
                                                  }
                                                  else
                                                  {
                                                    throw new Exception(
                                                        string.Format(
#if MULTILANGUAGE
                                                        Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheActivityItemTransferRefno)
                                                        + Translator.GetInstance().Translate(TranslatorVocabulary.InTactivityitemtransfer), actItemTransferRefno));
#else
                                                        "Unable to locate the activity item transfer refno {0} "
                                                        + "in tActivityItemTransfer.", actItemTransferRefno));
#endif
                                                  }

                                                  foreach (DataRow drow in ds.Tables["tActivityDetailTransfer"].Rows)
                                                  {
                                                    errMsg = "Arguments:";

                                                    recDetail.SetInt32(
                                                        (int)SysCommon.ActDetailFields.SubType, 
                                                        (int)SysCommon.ActSubTypes.Transfers);
                                                    recDetail.SetInt32(
                                                        (int)SysCommon.ActDetailFields.ActivityItemRefno, 
                                                        actItemTransferRefno);
                                                    recDetail.SetInt32(
                                                        (int)SysCommon.ActDetailFields.ProductUnitRefno,
                                                        drItemTransfer.GetInt32(
                                                        (int)SysCommon.ActItemTransferFields.ProductUnitRefno));

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

                                                    recDetailTransfer.SetInt32(
                                                        (int)SysCommon.ActDetailTransferFields.ActDetailRefno, 
                                                        actDetailRefno);

                                                    actDetailTransferFKRefno = Convert.ToInt32(
                                                        drow["ActivityDetailTransferFKRefno"]);
                                                    errMsg += Environment.NewLine + "Detail fk refno " 
                                                        + actDetailTransferFKRefno;
                                                    recDetailTransfer.SetInt32(
                                                        (int)SysCommon.ActDetailTransferFields.FKRefno, 
                                                        actDetailTransferFKRefno);
                                                    recDetailTransfer.SetBoolean(
                                                        (int)SysCommon.ActDetailTransferFields.IsConfirmed,
                                                        Convert.ToBoolean(
                                                        drow["ActivityDetailTransferIsConfirmed"]));
                                                    
                                                    rsDetailTransfer.Insert(recDetailTransfer);

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

                            drItemTransfer.Close();
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTransferDetailsFailed), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer details failed.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTransferDetailsFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer details failed.", false);
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
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingTransferDetailsFailed) + Environment.NewLine + errMsg,
#else
            "Database exception occurred. Saving transfer details failed." + Environment.NewLine + errMsg,
#endif
            ex);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemTransferFKRefno"></param>
    /// <returns></returns>
    protected override string Create(int actItemTransferFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>"
          + "<command>"
          + "<action>REQUEST TRANSFER DETAILS</action>"
          + SysCommon.GetRequestDefaults(false)
          + ("<itm_refno>" + actItemTransferFKRefno.ToString() + "</itm_refno>")
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
