namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Windows.Forms;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableProductSchemeVariant : TableBase
  {
    #region Constructors

    public TableProductSchemeVariant()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableProductSchemeVariant";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeVariants), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme variants.", true);
#endif
        reqData = this.Send("product scheme variants");
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeVariantsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme variants complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeVariantsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme variants failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeVariantsFailed), ex);
#else
        throw new Exception("Seeding product scheme variants failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemeVariants), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variants.", true);
#endif
        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tProductSchemeVariantXRef";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tProductSchemeVariant";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemeVariantsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variants complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemeVariantsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variants failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingProductSchemeVariantsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging product scheme variants failed.", ex);
#endif
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="schemeVarNo"></param>
    /// <returns></returns>
    public int GetProdSchemeVarRefnoFromProdSchemeVarNo(int schemeVarNo)
    {
      int schemeVarRefno = 0;

      try
      {
        using (SQLiteCommand cmndSchemeVar = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndSchemeVar.CommandText = "tProductSchemeVariant";
          cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";
          cmndSchemeVar.CommandType = CommandType.TableDirect;

          using (SqliteDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
          {
            if (drSchemeVar.Seek(DbSeekOptions.FirstEqual, new object[] { schemeVarNo }))
            {
              if (drSchemeVar.Read())
              {
                schemeVarRefno = Convert.ToInt32(drSchemeVar.GetValue((int)SysCommon.ProdSchemeVariantFields.Refno));
              }
              else
              {
#if MULTILANGUAGE
                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrschemevarreadFailed));
#else
                throw new Exception("drSchemeVar.Read() failed.");
#endif
              }
            }
            else
            {
              throw new Exception(
                  string.Format(
#if MULTILANGUAGE
                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateProductSchemeVariantNumberInTproductschemevariant), 
#else
                  "Unable To Locate Product Scheme Variant Number {0} In tProductSchemeVariant", 
#endif
                  schemeVarNo));
            }

            drSchemeVar.Close();
          }
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetprodschemevarrefnofromprodschemevarnoFailed), ex);
#else
        throw new Exception("Database exception occurred. GetProdSchemeVarRefnoFromProdSchemeVarNo failed.", ex);
#endif
      }

      return schemeVarRefno;
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
          cmndStatus.CommandText = "SELECT COUNT(*) FROM tProductSchemeVariant";
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());
          
          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ProductSchemeVariants), rowsAffected), 
#else
              string.Format("{0} product scheme variants.", rowsAffected), 
#endif
              false);
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredProductSchemeVariantsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Product scheme variants status failed.", ex);
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductSchemeVariants), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variants.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["tProductSchemeVariant"].Rows.Count);

            this.DataAccessStatusOutHandler(
                this, 
                EventArgs.Empty, 
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), 
#else
                string.Format("Records {0}", progMax), 
#endif
                false);

            using (SQLiteCommand cmndSchemeVar = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
              cmndSchemeVar.CommandText = "tProductSchemeVariant";
              cmndSchemeVar.CommandType = CommandType.TableDirect;
              cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";

              using (SqliteDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
              {
                using (SqlCeResultSet rs = cmndSchemeVar.ExecuteResultSet(ResultSetOptions.Updatable))
                {
                  SqlCeUpdatableRecord rec = rs.CreateRecord();
                  int progValue = 0;
                  bool isSchemeVarFound = false;
                  string schemeVarCode = string.Empty;
                  string schemeVarDesc = string.Empty;
                  int schemeVarNo = 0;
                  bool schemeVarIsDefault = false;

                  this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                  foreach (DataRow drow in ds.Tables["tProductSchemeVariant"].Rows)
                  {
                    schemeVarCode = drow["ProductSchemeVariantCode"].ToString();
                    errMsg = "Variant code \'" + schemeVarCode + "\'";
                    schemeVarDesc = drow["ProductSchemeVariantDescription"].ToString();
                    errMsg += Environment.NewLine + "Variant desc \'" + schemeVarDesc + "\'";
                    schemeVarNo = Convert.ToInt32(drow["ProductSchemeVariantNo"]);
                    errMsg += Environment.NewLine + "Variant no " + schemeVarNo;
                    schemeVarIsDefault = Convert.ToBoolean(drow["ProductSchemeVariantIsDefault"]);
                    errMsg += Environment.NewLine + "Variant is default " + schemeVarIsDefault;

                    isSchemeVarFound = false;
                    if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                    {
                      cmndSchemeVar.CommandText = "tProductSchemeVariant";
                      cmndSchemeVar.CommandType = CommandType.TableDirect;
                      cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";

                      if (drSchemeVar.Seek(DbSeekOptions.FirstEqual, new object[] { schemeVarNo }))
                      {
                        isSchemeVarFound = true;
                        
                        cmndSchemeVar.CommandType = CommandType.Text;
                        cmndSchemeVar.IndexName = string.Empty;
                        cmndSchemeVar.CommandText = "UPDATE tProductSchemeVariant " 
                            + "SET ProductSchemeVariantCode = \'" + schemeVarCode + "\', " 
                            + "ProductSchemeVariantDescription = \'" + schemeVarDesc + "\', " 
                            + "ProductSchemeVariantIsDefault = " + Convert.ToString(schemeVarIsDefault ? 1 : 0)
                            + " WHERE ProductSchemeVariantNo = " + schemeVarNo;
                        cmndSchemeVar.ExecuteNonQuery();
                      }
                    }

                    if (!isSchemeVarFound)
                    {
                      cmndSchemeVar.CommandText = "tProductSchemeVariant";
                      cmndSchemeVar.CommandType = CommandType.TableDirect;
                      cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";

                      rec.SetString((int)SysCommon.ProdSchemeVariantFields.Code, schemeVarCode);
                      rec.SetString((int)SysCommon.ProdSchemeVariantFields.Description, schemeVarDesc);
                      rec.SetInt32((int)SysCommon.ProdSchemeVariantFields.No, schemeVarNo);
                      rec.SetBoolean((int)SysCommon.ProdSchemeVariantFields.IsDefault, schemeVarIsDefault);

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
          }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductSchemeVariantsComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variants complete.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductSchemeVariantsComplete), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variants complete.", false);
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
          Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingProductSchemeVariantsFailed) + Environment.NewLine + errMsg, 
#else
          "Database exception occurred. Saving product scheme variants failed." + Environment.NewLine + errMsg, 
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
          + "<action>REQUEST PRODUCT SCHEME VARIANTS</action>"
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
