namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Windows.Forms;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableProductSchemeVariantXRef : TableBase
  {
    #region Constructors

    public TableProductSchemeVariantXRef()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableProductSchemeVariantXRef";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeVariantXref), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme variant xref.", true);
#endif

        reqData = this.Send("product scheme variant xref");
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeVariantXrefComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme variant xref complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeVariantXrefFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme variant xref failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeVariantXrefFailed), ex);
#else
        throw new Exception("Seeding product scheme variant xref failed.", ex);
#endif
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks>Purging of a XRef table should not be needed and is redundant, because of the the relationship 
    /// constraints of the parent tables.
    /// I am keeping this routine just to be consistent.</remarks>
    public override void Purge()
    {
      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemeVariantXref), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variant xref.", true);
#endif
        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          // Note should tProductSchemeVariants be purged too?

          cmndPurge.CommandText = "DELETE FROM tProductSchemeVariantXRef";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemeVariantXrefFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variant xref failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingProductSchemeVariantXrefFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging product scheme variant xref failed.", ex);
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
          cmndStatus.CommandText = "SELECT COUNT(*) FROM tProductSchemeVariantXRef";
          cmndStatus.CommandType = CommandType.Text;
      
          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());
          
          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ProductSchemeVariantXrefs), rowsAffected), 
#else
              string.Format("{0} product scheme variant xrefs.", rowsAffected), 
#endif
              true);
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredProductSchemeVariantXrefStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Product scheme variant xref status failed.", ex);
#endif
      }

      return rowsAffected;
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductSchemeVariantXrefs), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variant xrefs.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["tProductSchemeVariantXRef"].Rows.Count);

            this.DataAccessStatusOutHandler(
                this, 
                EventArgs.Empty, 
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), 
#else
                string.Format("Records {0}", progMax), 
#endif
                false);

            using (SQLiteCommand cmndSchemeVarXRef = conn.CreateCommand())
            {
              cmndSchemeVarXRef.CommandText = "tProductSchemeVariantXRef";
              cmndSchemeVarXRef.CommandType = CommandType.TableDirect;
              cmndSchemeVarXRef.IndexName =
                  "PK_ProductSchemeVariantXRefOnProductSchemeRefnoAndProductSchemeVariantRefno";

              using (SqliteDataReader drSchemeVarXRef = cmndSchemeVarXRef.ExecuteReader())
              {
                using (SQLiteCommand cmndScheme = conn.CreateCommand())
                {
                  cmndScheme.CommandText = "tProductScheme";
                  cmndScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";
                  cmndScheme.CommandType = CommandType.TableDirect;

                  using (SqliteDataReader drScheme = cmndScheme.ExecuteReader())
                  {
                    using (SQLiteCommand cmndSchemeVar = conn.CreateCommand())
                    {
                      cmndSchemeVar.CommandText = "tProductSchemeVariant";
                      cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";
                      cmndSchemeVar.CommandType = CommandType.TableDirect;

                      using (SqliteDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
                      {
                        using (SqlCeResultSet rs = cmndSchemeVarXRef.ExecuteResultSet(ResultSetOptions.Updatable))
                        {
                          SqlCeUpdatableRecord rec = rs.CreateRecord();
                          int schemeRefno = 0;
                          int schemeVarRefno = 0;
                          int progValue = 0;
                          bool isSchemeVarXRef = false;
                          string schemeCode = string.Empty;
                          int schemeVariantNo = 0;

                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                          foreach (DataRow drow in ds.Tables["tProductSchemeVariantXRef"].Rows)
                          {
                            schemeCode = drow["ProductSchemeCode"].ToString();
                            errMsg = "Code \'" + schemeCode + "\'";
                            if (drScheme.Seek(DbSeekOptions.FirstEqual, new object[] { schemeCode }))
                            {
                              if (drScheme.Read())
                              {
                                schemeRefno = Convert.ToInt32(drScheme.GetValue((int)SysCommon.ProdSchemeFields.Refno));
                              }
                              else
                              {
#if MULTILANGUAGE
                                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrschemereadFailed) + Environment.NewLine + errMsg);
#else
                                throw new Exception("drScheme.Read() failed." + Environment.NewLine + errMsg);
#endif
                              }
                            }
                            else
                            {
                              throw new Exception(
#if MULTILANGUAGE
                                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductSchemeCodeInTproductscheme) 
#else
                                  "Unable to locate the product scheme code in tProductScheme." 
#endif
                                  + Environment.NewLine + errMsg);
                            }

                            schemeVariantNo = Convert.ToInt32(drow["ProductSchemeVariantNo"]);
                            errMsg += Environment.NewLine + "Variant no " + schemeVariantNo;
                            if (drSchemeVar.Seek(DbSeekOptions.FirstEqual, new object[] { schemeVariantNo }))
                            {
                              if (drSchemeVar.Read())
                              {
                                schemeVarRefno = Convert.ToInt32(
                                    drSchemeVar.GetValue(
                                    (int)SysCommon.ProdSchemeVariantFields.Refno));
                              }
                              else
                              {
#if MULTILANGUAGE
                                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrschemevarreadFailed) + Environment.NewLine + errMsg);
#else
                                throw new Exception("drSchemeVar.Read() failed." + Environment.NewLine + errMsg);
#endif
                              }
                            }
                            else
                            {
                              throw new Exception(
#if MULTILANGUAGE
                                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductSchemeVariantNumberInTproductschemevariant)
#else
                                  "Unable to locate the product scheme variant number in tProductSchemeVariant."
#endif
                                  + Environment.NewLine + errMsg);
                            }

                            isSchemeVarXRef = false;
                            if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                            {
                              cmndSchemeVarXRef.CommandText = "tProductSchemeVariantXRef";
                              cmndSchemeVarXRef.CommandType = CommandType.TableDirect;
                              cmndSchemeVarXRef.IndexName = 
                                  "PK_ProductSchemeVariantXRefOnProductSchemeRefnoAndProductSchemeVariantRefno";
                              if (drSchemeVarXRef.Seek(
                                  DbSeekOptions.FirstEqual, 
                                  new object[] { schemeRefno, schemeVarRefno }))
                              {
                                isSchemeVarXRef = true;
                              }
                            }

                            if (!isSchemeVarXRef)
                            {
                              cmndSchemeVarXRef.CommandText = "tProductSchemeVariantXRef";
                              cmndSchemeVarXRef.CommandType = CommandType.TableDirect;
                              cmndSchemeVarXRef.IndexName = 
                                  "PK_ProductSchemeVariantXRefOnProductSchemeRefnoAndProductSchemeVariantRefno";
                              
                              rec.SetInt32((int)SysCommon.ProdSchemeVariantXRefFields.ProdSchemeRefno, schemeRefno);
                              rec.SetInt32(
                                  (int)SysCommon.ProdSchemeVariantXRefFields.ProdSchemeVariantRefno, 
                                  schemeVarRefno);

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

                        drSchemeVar.Close();
                      }
                    }

                    drScheme.Close();
                  }
                }

                drSchemeVarXRef.Close();
              }
            }
          }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductSchemeVariantXrefComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variant xref complete.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductSchemeVariantXrefFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variant xref failed.", false);
#endif

        throw new Exception(
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingProductSchemeVariantXrefFailed) + Environment.NewLine + errMsg,
#else
            "Database exception occurred. Saving product scheme variant xref failed." + Environment.NewLine + errMsg,
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
          + "<action>REQUEST PRODUCT SCHEME VARIANT XREF</action>"
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
