namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableUnit : TableBase
  {
    #region Constructors

    public TableUnit()
    {
      this.FullClassName = "dMobile.GroundWork.TableUnit";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingUnitsOfMeasure), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding units of measure.", true);
#endif

        reqData = this.Send("units of measure");
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

        // Require to have units.
        if (0 >= this.DataStatus())
        {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.NoUnitsOfMeasureWhereReceived), true);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "No units of measure where received.", true);
#endif

#if MULTILANGUAGE
          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.NoUnitsOfMeasureWhereReceived));
#else
          throw new Exception("No units of measure where received.");
#endif
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingUnitsOfMeasureComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding units of measure complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingUnitsOfMeasureFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding units of measure failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingUnitsOfMeasureFailed), ex);
#else
        throw new Exception("Seeding units of measure failed.", ex);
#endif
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unitCode"></param>
    /// <returns></returns>
    public int GetiUnitRefnoFromUnitCode(string unitCode)
    {
      int unitRefno = 0;

      try
      {
        using (SQLiteCommand cmndUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndUnit.CommandText = "tUnit";
          //cmndUnit.IndexName = "UQ_UnitOnUnitCode";
          cmndUnit.CommandType = CommandType.TableDirect;

          using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
          {
            if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode }))
            {
              if (drUnit.Read())
              {
                unitRefno = Convert.ToInt32(drUnit.GetValue((int)SysCommon.UnitFields.UnitRefno));
              }
              else
              {
#if MULTILANGUAGE
                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrunitreadFailed));
#else
                throw new Exception("drUnit.Read() failed.");
#endif
              }
            }
            else
            {
              throw new Exception(
#if MULTILANGUAGE
                  string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheUnitRefnoFromUnitCodeInTunit), unitCode));
#else
                  string.Format("Unable to locate the unit refno from unit code {0} in tUnit", unitCode));
#endif
            }

            drUnit.Close();
          }
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetiunitrefnofromunitcodeFailed), ex);
#else
        throw new Exception("Database exception occurred. GetiUnitRefnoFromUnitCode failed.", ex);
#endif
      }

      return unitRefno;
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
          cmndStatus.CommandText = "SELECT COUNT(*) FROM tUnit";
          cmndStatus.CommandType = CommandType.Text;
          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());
      
          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnitsOfMeasure), rowsAffected),
#else
              string.Format("{0} units of measure.", rowsAffected),
#endif
              true);
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredUnitsOfMeasureStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Units of measure status failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingUnitsOfMeasure), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging units of measure.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tUnit";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingUnitsOfMeasureComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging units of measure complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingUnitsOfMeasureFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging units of measure failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingUnitsOfMeasureFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging units of measure failed.", ex);
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingUnitsOfMeasure), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving units of measure.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = ds.Tables["tUnit"].Rows.Count;
#if MULTILANGUAGE
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), false);
#else
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);
#endif
           
            using (SQLiteCommand cmndUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
              cmndUnit.CommandText = "tUnit";
              cmndUnit.CommandType = CommandType.TableDirect;
              //cmndUnit.IndexName = "UQ_UnitOnUnitCode";

              using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
              {
                using (SqlCeResultSet rs = cmndUnit.ExecuteResultSet(ResultSetOptions.Updatable))
                {
                  SqlCeUpdatableRecord rec = rs.CreateRecord();
                  int progValue = 0;
                  bool isUnitFound = false;
                  string unitCode = string.Empty;

                  this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                  foreach (DataRow drow in ds.Tables["tUnit"].Rows)
                  {
                    unitCode = drow["UnitCode"].ToString();
                    errMsg = "Unit code \'" + unitCode + "\'";
                    isUnitFound = false;

                    if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                    {
                      cmndUnit.CommandText = "tUnit";
                      cmndUnit.CommandType = CommandType.TableDirect;
                      //cmndUnit.IndexName = "UQ_UnitOnUnitCode";

                      if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode }))
                      {
                        isUnitFound = true;
                      }
                    }

                    if (!isUnitFound)
                    {
                      cmndUnit.CommandText = "tUnit";
                      cmndUnit.CommandType = CommandType.TableDirect;
                      //cmndUnit.IndexName = "UQ_UnitOnUnitCode";

                      rec.SetString((int)SysCommon.UnitFields.UnitCode, unitCode);

                      rs.Insert(rec);
                    }

                    progValue++;
                    if (progValue % 100 == 0)
                    {
                      this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                    }
                  }

                  this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                }

                drUnit.Close();
              }
            }
          }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingUnitsOfMeasureComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving units of measure complete.", false);
#endif
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingUnitsOfMeasureFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving units of measure failed.", false);
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
          Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingUnitsOfMeasureFailed) + Environment.NewLine + errMsg, 
#else
          "Database exception occurred. Saving units of measure failed." + Environment.NewLine + errMsg, 
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
          + "<action>REQUEST UNITS</action>"
          + SysCommon.GetRequestDefaults(true)
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
