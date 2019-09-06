namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableFamily : TableBase
  {
    #region Constructors

    public TableFamily()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableFamily";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingFamilies), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding families.", true);
#endif

        reqData = this.Send("families");
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingFamiliesComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding families complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingFamiliesFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding families failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingFamiliesFailed), ex);
#else
        throw new Exception("Seeding families failed.", ex);
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
          cmndStatus.CommandText = "SELECT COUNT(*) FROM tFamily";
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Families), rowsAffected), true);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} families.", rowsAffected), true);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.FamiliesStatusFailed), ex);
#else
        throw new Exception("Families status failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingFamilies), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging families.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tFamily";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingFamiliesComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging families complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingFamiliesFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging families failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingFamiliesFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging families failed.", ex);
#endif
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="familyCode"></param>
    /// <returns></returns>
    public int GetiFamilyRefnoFromFamilyCode(string familyCode)
    {
      int familyRefno = 0;

      try
      {
        using (SQLiteCommand cmndFamily = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndFamily.CommandText = "tFamily";
          cmndFamily.IndexName = "UQ_FamilyOnFamilyCode";
          cmndFamily.CommandType = CommandType.TableDirect;

          using (SqliteDataReader drFamily = cmndFamily.ExecuteReader())
          {
            if (drFamily.Seek(DbSeekOptions.FirstEqual, new object[] { familyCode }))
            {
              if (drFamily.Read())
              {
                familyRefno = Convert.ToInt32(drFamily.GetValue((int)SysCommon.FamilyFields.Refno));
              }
              else
              {
#if MULTILANGUAGE
                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrfamilyreadFailed));
#else
                throw new Exception("drFamily.Read() failed.");
#endif
              }
            }
            else
            {
              throw new Exception(
                  string.Format(
#if MULTILANGUAGE
                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheFamilyRefnoFromFamilyCodeInTfamily),
#else
                  "Unable to locate the family refno from family code {0} in tFamily.",
#endif
                  familyCode));
            }

            drFamily.Close();
          }
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetifamilyrefnofromfamilycodeFailed), ex);
#else
        throw new Exception("Database exception occurred. GetiFamilyRefnoFromFamilyCode failed.", ex);
#endif
      }

      return familyRefno;
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingFamilies), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving families.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = ds.Tables["tFamily"].Rows.Count;
#if MULTILANGUAGE
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), false);
#else
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);
#endif

            using (SQLiteCommand cmndFamily = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
              cmndFamily.CommandText = "tFamily";
              cmndFamily.CommandType = CommandType.TableDirect;
              cmndFamily.IndexName = "UQ_FamilyOnFamilyCode";

              using (SqliteDataReader drFamily = cmndFamily.ExecuteReader())
              {
                using (SqlCeResultSet rs = cmndFamily.ExecuteResultSet(ResultSetOptions.Updatable))
                {
                  SqlCeUpdatableRecord rec = rs.CreateRecord();
                  int progValue = 0;
                  bool isFamilyFound = false;
                  string familyCode = string.Empty;

                  this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                  foreach (DataRow drow in ds.Tables["tFamily"].Rows)
                  {
                    familyCode = drow["FamilyCode"].ToString();
                    errMsg = "Family \'" + familyCode + "\'";

                    cmndFamily.CommandText = "tFamily";
                    cmndFamily.CommandType = CommandType.TableDirect;
                    cmndFamily.IndexName = "UQ_FamilyOnFamilyCode";

                    isFamilyFound = false;
                    if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                    {
                      if (drFamily.Seek(DbSeekOptions.FirstEqual, new object[] { familyCode }))
                      {
                        isFamilyFound = true;
                      }
                    }

                    if (!isFamilyFound)
                    {
                      rec.SetString((int)SysCommon.FamilyFields.Code, familyCode);

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

                drFamily.Close();
              }
            }
          }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingFamiliesComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving families complete.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingFamiliesFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving families failed.", false);
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
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingFamiliesFailed) + Environment.NewLine + errMsg,
#else
            "Database exception occurred. Saving families failed." + Environment.NewLine + errMsg,
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
          + "<action>REQUEST FAMILIES</action>"
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
