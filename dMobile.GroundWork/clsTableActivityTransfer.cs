namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using Microsoft.VisualBasic;

  public class TableActivityTransfer : TableBase
  {
    #region Constructors

    public TableActivityTransfer()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityTransfer";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTransfers), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfers.", true);
#endif

        reqData = this.Send("activity transfers");
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTransfersComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfers complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTransfersFailed), ex);
#else
        throw new Exception("Seeding transfers failed.", ex);
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
              + " WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.Transfers;
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());
          
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Transferszz), rowsAffected), true);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} transfers.", rowsAffected), true);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredTransfersStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Transfers status failed.", ex);
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
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTransfers), true);
#else
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers.", true);
#endif

      try
      {
        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityTransfer " 
              + "WHERE tActivityTransfer.ActivitySubType = " + (int)SysCommon.ActSubTypes.Transfers;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivity " 
              + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.Transfers;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTransfersComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTransfersFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingTransfersFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging transfers failed.", ex);
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTransfers), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfers.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["tActivityTransfer"].Rows.Count);

            this.DataAccessStatusOutHandler(
                this, 
                EventArgs.Empty, 
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax),
#else
                string.Format("Records {0}", progMax),
#endif
                false);

            using (SQLiteCommand cmndTrans = conn.CreateCommand())
            {
              cmndTrans.CommandText = "tActivityTransfer";
              cmndTrans.CommandType = CommandType.TableDirect;
              cmndTrans.IndexName = "UQ_ActivityTransferOnActivityTransferFKRefno";

              using (SQLiteCommand cmndAct = conn.CreateCommand())
              {
                cmndAct.CommandText = "tActivity";
                cmndAct.CommandType = CommandType.TableDirect;

                using (SqlCeResultSet rsAct = cmndAct.ExecuteResultSet(ResultSetOptions.Updatable))
                {
                  SqlCeUpdatableRecord recAct = rsAct.CreateRecord();

                  using (SQLiteCommand cmndID = conn.CreateCommand())
                  {
                    cmndID.CommandText = "SELECT @@IDENTITY";
                    cmndID.CommandType = CommandType.Text;

                    using (SQLiteCommand cmndWhs = conn.CreateCommand())
                    {
                      cmndWhs.CommandText = "tLocationMapWarehouse";
                      cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                      cmndWhs.CommandType = CommandType.TableDirect;

                      using (SqliteDataReader drWhs = cmndWhs.ExecuteReader())
                      {
                        using (SqlCeResultSet rs = cmndTrans.ExecuteResultSet(ResultSetOptions.Updatable))
                        {
                          SqlCeUpdatableRecord rec = rs.CreateRecord();
                          int actRefno = 0;
                          int progValue = 0;
                          int whsLocMapRefno = 0;
                          int locMapWhsFKRefno = 0;
                          int transFKRefno = 0;
                          string transDesc = string.Empty;
                          DateTime transDateTime = DateAndTime.DateSerial(1900, 1, 1);
                          DateTime arrivalDateTime = DateAndTime.DateSerial(1900, 1, 1);

                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                          foreach (DataRow drow in ds.Tables["tActivityTransfer"].Rows)
                          {
                            errMsg = "Arguments:";

                            transFKRefno = Convert.ToInt32(drow["ActivityTransferFKRefno"]);
                            errMsg = "Trans fk refno " + transFKRefno;
                            transDesc = drow["ActivityTransferDescription"].ToString();
                            errMsg += Environment.NewLine + "Trans desc \'" + transDesc + "\'";
                            transDateTime = Convert.ToDateTime(drow["ActivityTransferDateTime"]);
                            errMsg += Environment.NewLine + "Trans date " + transDateTime.ToShortDateString();
                            arrivalDateTime = Convert.ToDateTime(drow["ActivityTransferArrivalDateTime"]);
                            errMsg += Environment.NewLine + "Trans arrival date " + arrivalDateTime.ToShortDateString();

                            locMapWhsFKRefno = Convert.ToInt32(drow["TransferToLocationMapWarehouseFKRefno"]);
                            errMsg += Environment.NewLine + "Whs fk refno " + locMapWhsFKRefno;
                            if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapWhsFKRefno }))
                            {
                              if (drWhs.Read())
                              {
                                whsLocMapRefno = drWhs.GetInt32((int)SysCommon.LocMapWhsFields.LocMapRefno);
                              }
                              else
                              {
#if MULTILANGUAGE
                                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrwhsreadFailed) + Environment.NewLine + errMsg);
#else
                                throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
#endif
                              }
                            }
                            else
                            {
                              throw new Exception(
#if MULTILANGUAGE
                                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheWarehouseFkRefnoInTlocationmapwarehouse)
#else
                                  "Unable to locate the warehouse fk refno in tLocationMapWarehouse."
#endif
                                  + Environment.NewLine + errMsg);
                            }

                            recAct.SetInt32((int)SysCommon.ActFields.SubType, (int)SysCommon.ActSubTypes.Transfers);
                            recAct.SetInt32((int)SysCommon.ActFields.Status, 0);
                            
                            rsAct.Insert(recAct);
                            
                            actRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                            rec.SetInt32((int)SysCommon.ActTransferFields.ActRefno, actRefno);
                            rec.SetInt32(
                                (int)SysCommon.ActTransferFields.ActSubType, 
                                (int)SysCommon.ActSubTypes.Transfers);
                            rec.SetInt32((int)SysCommon.ActTransferFields.FKRefno, transFKRefno);
                            rec.SetString(
                                (int)SysCommon.ActTransferFields.StringFKRefno,
                                Convert.ToString(transFKRefno));
                            rec.SetString((int)SysCommon.ActTransferFields.Description, transDesc);
                            rec.SetInt32((int)SysCommon.ActTransferFields.TransferToWhsLocMapRefno, whsLocMapRefno);
                            rec.SetDateTime((int)SysCommon.ActTransferFields.DateTime, transDateTime);
                            rec.SetDateTime((int)SysCommon.ActTransferFields.ArrivalDateTime, arrivalDateTime);

                            rs.Insert(rec);

                            progValue++;
                            if (progValue % 10 == 0)
                            {
                              this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                            }
                          }

                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        drWhs.Close();
                      }
                    }
                  }
                }
              }
            }
          }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTransfersComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfers complete.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTransfersFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfers failed.", false);
#endif

        // Duplicate key violations
        if (ex.NativeError == 25016)
        {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation), false);
          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolationz) + Environment.NewLine + errMsg, ex);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
          throw new Exception("Uniqueness Violation." + Environment.NewLine + errMsg, ex);
#endif
        }

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingTransfersFailed), ex);
#else
        throw new Exception("Database exception occurred. Saving transfers failed.", ex);
#endif
      }
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
          + "<action>REQUEST TRANSFERS</action>"
          + SysCommon.GetRequestDefaults(false)
          + "</command>" + "</imclient>");

      request.Save(xml);
      xml.Flush();
      xml.Close();

      return xmlString.ToString();
    }

    #endregion
  }
}
