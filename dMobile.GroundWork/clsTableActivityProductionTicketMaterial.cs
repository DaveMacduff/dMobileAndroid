namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Windows.Forms;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using Microsoft.VisualBasic;

  public class TableActivityProductionTicketMaterial : TableBase
  {
    #region Constructors

    public TableActivityProductionTicketMaterial()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketMaterial";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketMaterials), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket materials.", true);
#endif

        reqData = this.Send("ticket materials");
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketMaterialsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket materials complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketMaterialsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket materials failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketMaterialsFailed), ex);
#else
        throw new Exception("Seeding ticket materials failed.", ex);
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
              + " WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketMaterial;
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this,
              EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.TicketMaterialsz), rowsAffected),
#else
              string.Format("{0} ticket materials.", rowsAffected),
#endif
              true);
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredMaterialTicketsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Material tickets status failed.", ex);
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
        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTicketMaterials), true);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket materials.", true);
#endif

          cmndPurge.CommandText = "DELETE FROM tActivityProductionMaterial"
              + " WHERE tActivityProductionMaterial.ActivityRefno IN"
              + " (SELECT tActivityProduction.ActivityRefno"
              + " FROM tActivityProduction"
              + " WHERE tActivityProduction.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketMaterial + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityProduction "
              + "WHERE tActivityProduction.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketMaterial;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivity "
              + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketMaterial;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTicketMaterialsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket materials complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTicketMaterialsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket materials failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingTicketMaterialsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging ticket materials failed.", ex);
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTicketMaterials), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket materials.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["tActivityProduction"].Rows.Count);

#if MULTILANGUAGE
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), false);
#else
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);
#endif

            using (SQLiteCommand cmndProduction = conn.CreateCommand())
            {
              cmndProduction.CommandText = "tActivityProduction";
              cmndProduction.CommandType = CommandType.TableDirect;
              cmndProduction.IndexName = "PK_ActivityProductionOnActivityRefno";

              using (SQLiteCommand cmndMaterial = conn.CreateCommand())
              {
                cmndMaterial.CommandText = "tActivityProductionMaterial";
                cmndMaterial.CommandType = CommandType.TableDirect;
                cmndMaterial.IndexName = "UQ_ActivityProductionMaterialOnActivityProductionFKRefno";

                using (SqlCeResultSet rsMaterial = cmndMaterial.ExecuteResultSet(ResultSetOptions.Updatable))
                {
                  SqlCeUpdatableRecord recMaterial = rsMaterial.CreateRecord();

                  using (SQLiteCommand cmndActivity = conn.CreateCommand())
                  {
                    cmndActivity.CommandText = "tActivity";
                    cmndActivity.CommandType = CommandType.TableDirect;

                    using (SqlCeResultSet rsActivity = cmndActivity.ExecuteResultSet(ResultSetOptions.Updatable))
                    {
                      SqlCeUpdatableRecord recActivity = rsActivity.CreateRecord();

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
                            using (SqlCeResultSet rsProduction
                                = cmndProduction.ExecuteResultSet(ResultSetOptions.Updatable))
                            {
                              SqlCeUpdatableRecord recProduction = rsProduction.CreateRecord();
                              int actRefno = 0;
                              int progValue = 0;
                              int whsLocMapRefno = 0;
                              int actProductionFKRefno = 0;

                              this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                              foreach (DataRow drow in ds.Tables["tActivityProduction"].Rows)
                              {
                                errMsg = "Arguments:";

                                recActivity.SetInt32(
                                    (int)SysCommon.ActFields.SubType,
                                    (int)SysCommon.ActSubTypes.TicketMaterial);
                                recActivity.SetInt32((int)SysCommon.ActFields.Status, 0);

                                rsActivity.Insert(recActivity);

                                actRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                recProduction.SetInt32((int)SysCommon.ActProdFields.ActRefno, actRefno);
                                recProduction.SetInt32(
                                    (int)SysCommon.ActProdFields.ActSubType,
                                    (int)SysCommon.ActSubTypes.TicketMaterial);

                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, drWhs);
                                recProduction.SetInt32((int)SysCommon.ActProdFields.WhsLocMapRefno, whsLocMapRefno);

                                // To remove time - convert twice
                                DateTime dt = Convert.ToDateTime(drow["ActivityProductionDateTime"]);
                                recProduction.SetDateTime(
                                    (int)SysCommon.ActProdFields.DateTime,
                                    DateTime.Parse(dt.ToShortDateString()));

                                recProduction.SetString(
                                    (int)SysCommon.ActProdFields.Description,
                                    drow["ActivityProductionDescription"].ToString());

                                rsProduction.Insert(recProduction);

                                recMaterial.SetInt32(
                                    (int)SysCommon.ActProdMaterialFields.ActRefno,
                                    actRefno);

                                actProductionFKRefno = Convert.ToInt32(drow["ActivityProductionFKRefno"]);
                                errMsg += Environment.NewLine + "FK refno " + actProductionFKRefno;
                                recMaterial.SetInt32(
                                    (int)SysCommon.ActProdMaterialFields.FKRefno,
                                    actProductionFKRefno);
                                recMaterial.SetString(
                                    (int)SysCommon.ActProdMaterialFields.StringFKRefno,
                                    actProductionFKRefno.ToString());

                                rsMaterial.Insert(recMaterial);

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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTicketMaterialsComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket materials complete.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTicketMaterialsFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket materials failed.", false);
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
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingTicketMaterialsFailed) + Environment.NewLine + errMsg,
#else
            "Database exception occurred. Saving ticket materials failed." + Environment.NewLine + errMsg,
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
          + "<action>REQUEST PRODUCTION TICKET MATERIAL</action>" 
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
