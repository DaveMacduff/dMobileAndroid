namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableActivityProductionTicketFinished : TableBase
  {
    #region Constructors

    public TableActivityProductionTicketFinished()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketFinished";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingFinishedTickets), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished tickets.", true);
#endif

        reqData = this.Send("finished ticket");
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingFinishedTicketsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished tickets complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingFinishedTicketsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished tickets failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingFinishedTicketsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding finished tickets failed.", ex);
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
          cmndStatus.CommandText = "SELECT COUNT(tActivity.ActivityRefno)"
              + " FROM tActivity"
              + " WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketFinished;
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this,
              EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.FinishedTickets), rowsAffected),
#else
              string.Format("{0} finished tickets.", rowsAffected),
#endif
              true);
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredFinishedTicketsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Finished tickets status failed.", ex);
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
        using (SqlCeCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingFinishedTicket), true);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket.", true);
#endif

          cmndPurge.CommandText = "DELETE FROM tActivityProductionFinished"
              + " WHERE tActivityProductionFinished.ActivityRefno IN"
              + " (SELECT tActivityProduction.ActivityRefno"
              + " FROM tActivityProduction"
              + " WHERE tActivityProduction.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketFinished + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityProduction "
              + "WHERE tActivityProduction.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketFinished;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivity "
              + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketFinished;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingFinishedTicketComplete), true);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket complete.", true);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingFinishedTicketComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket complete.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingFinishedTicketComplete), ex);
#else
        throw new Exception("Database exception occurred. Purging finished ticket complete.", ex);
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingFinishedTickets), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished tickets.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["tActivityProduction"].Rows.Count);

            this.DataAccessStatusOutHandler(
                this,
                EventArgs.Empty,
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax),
#else
                string.Format("Records {0}", progMax),
#endif
                false);

            using (SqlCeCommand cmndProduction = conn.CreateCommand())
            {
              cmndProduction.CommandText = "tActivityProduction";
              cmndProduction.CommandType = CommandType.TableDirect;
              cmndProduction.IndexName = "PK_ActivityProductionOnActivityRefno";

              using (SqlCeCommand cmndFinished = conn.CreateCommand())
              {
                cmndFinished.CommandText = "tActivityProductionFinished";
                cmndFinished.CommandType = CommandType.TableDirect;
                cmndFinished.IndexName = "UQ_ActivityProductionFinishedOnActivityProductionFKRefno";

                using (SqlCeResultSet rsFinished = cmndFinished.ExecuteResultSet(ResultSetOptions.Updatable))
                {
                  SqlCeUpdatableRecord recFinished = rsFinished.CreateRecord();

                  using (SqlCeCommand cmndActivity = conn.CreateCommand())
                  {
                    cmndActivity.CommandText = "tActivity";
                    cmndActivity.CommandType = CommandType.TableDirect;

                    using (SqlCeResultSet rsActivity = cmndActivity.ExecuteResultSet(ResultSetOptions.Updatable))
                    {
                      SqlCeUpdatableRecord recActivity = rsActivity.CreateRecord();

                      using (SqlCeCommand cmndID = conn.CreateCommand())
                      {
                        cmndID.CommandText = "SELECT @@IDENTITY";
                        cmndID.CommandType = CommandType.Text;

                        using (SqlCeCommand cmndWhs = conn.CreateCommand())
                        {
                          cmndWhs.CommandText = "tLocationMapWarehouse";
                          cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                          cmndWhs.CommandType = CommandType.TableDirect;

                          using (SqlCeDataReader drWhs = cmndWhs.ExecuteReader())
                          {
                            using (SqlCeResultSet rsProduction = cmndProduction.ExecuteResultSet(
                                ResultSetOptions.Updatable))
                            {
                              SqlCeUpdatableRecord recProduction = rsProduction.CreateRecord();

                              int actRefno = 0;
                              int progValue = 0;
                              int whsLocMapRefno = 0;

                              this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                              foreach (DataRow drow in ds.Tables["tActivityProduction"].Rows)
                              {
                                errMsg = "Arguments:";

                                recActivity.SetInt32(
                                    (int)SysCommon.ActFields.SubType,
                                    (int)SysCommon.ActSubTypes.TicketFinished);
                                recActivity.SetInt32((int)SysCommon.ActFields.Status, 0);

                                rsActivity.Insert(recActivity);

                                actRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                recProduction.SetInt32((int)SysCommon.ActProdFields.ActRefno, actRefno);
                                recProduction.SetInt32(
                                    (int)SysCommon.ActProdFields.ActSubType,
                                    (int)SysCommon.ActSubTypes.TicketFinished);

                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, drWhs);
                                recProduction.SetInt32((int)SysCommon.ActProdFields.WhsLocMapRefno, whsLocMapRefno);

                                DateTime dt = DateTime.Parse(
                                    Convert.ToDateTime(drow["ActivityProductionDateTime"]).ToShortDateString());
                                errMsg += Environment.NewLine + "Date " + dt.ToShortDateString();
                                recProduction.SetDateTime((int)SysCommon.ActProdFields.DateTime, dt);
                                recProduction.SetString(
                                    (int)SysCommon.ActProdFields.Description,
                                    drow["ActivityProductionDescription"].ToString());

                                rsProduction.Insert(recProduction);

                                recFinished.SetInt32(
                                    (int)SysCommon.ActProdFinishedFields.ActivityRefno, actRefno);

                                int actProductionFKRefno = Convert.ToInt32(drow["ActivityProductionFKRefno"]);
                                errMsg += Environment.NewLine + "FK refno " + actProductionFKRefno;
                                recFinished.SetInt32(
                                    (int)SysCommon.ActProdFinishedFields.FKRefno,
                                    actProductionFKRefno);
                                recFinished.SetString(
                                    (int)SysCommon.ActProdFinishedFields.StringFKRefno,
                                    actProductionFKRefno.ToString());
                                recFinished.SetString(
                                    (int)SysCommon.ActProdFinishedFields.DefaultLogisticUnitBatchLotNumber,
                                    string.Empty);

                                rsFinished.Insert(recFinished);

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
            }
          }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingFinishedTicketsComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished tickets complete.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingFinishedTicketsFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished tickets failed.", false);
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
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingFinishedTicketsFailed) + Environment.NewLine + errMsg,
#else
            "Database exception occurred. Saving finished tickets failed." + Environment.NewLine + errMsg,
#endif
            ex);
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
          + "<action>REQUEST PRODUCTION TICKET FINISHED</action>" 
          + SysCommon.GetRequestDefaults(false)
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
