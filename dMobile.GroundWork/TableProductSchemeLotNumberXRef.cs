namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableProductSchemeLotNumberXRef : TableBase
  {
    #region Constructors

    public TableProductSchemeLotNumberXRef()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableProductSchemeLotNumberXRef";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeLotNumberXref), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot number xref.", true);
#endif

        reqData = this.Send("product scheme lot number xref");
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

        this.DataAccessStatusOutHandler(
            this, 
            EventArgs.Empty, 
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeLotNumberXrefComplete), 
#else
            "Seeding product scheme lot number xref complete.", 
#endif
            true);
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeLotNumberXrefFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product scheme lot number xref failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeLotNumberXrefFailed), ex);
#else
        throw new Exception("Seeding product scheme lot number xref failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemeLotNumberXref), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot number xref.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tProductSchemeLotNumber";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tProductSchemeLotNumberXRef";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemeLotNumberXrefFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme lot number xref failed.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
#endif
        this.DataAccessStatusOutHandler(
            this, 
            EventArgs.Empty, 
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemeLotNumberXrefFailed), 
#else
            "Purging product scheme lot number xref failed.", 
#endif
            false);

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingProductSchemeLotNumberXrefFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging product scheme lot number xref failed.", ex);
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
          cmndStatus.CommandText = "SELECT COUNT(*) FROM tProductSchemeLotNumberXRef";
          cmndStatus.CommandType = CommandType.Text;
          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ProductSchemeLotNumberXrefs), rowsAffected),
#else
              string.Format("{0} product scheme lot number xrefs.", rowsAffected),
#endif
              true);
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredProductSchemeLotNumberXrefsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Product scheme lot number xrefs status failed.", ex);
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
//      SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
//      string errMsg = string.Empty;

//      try
//      {
//        using (DataSet ds = new DataSet())
//        {
//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.ReadingReceivedData), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
//#endif
//          ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductSchemeLotNumberXref), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme lot number xref.", false);
//#endif
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tProductSchemeLotNumberXRef"].Rows.Count);

//            this.DataAccessStatusOutHandler(
//                this, 
//                EventArgs.Empty, 
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax),
//#else
//                string.Format("Records {0}", progMax),
//#endif
//                false);

//            using (SQLiteCommand cmndSchemeLotXRef = conn.CreateCommand())
//            {
//              cmndSchemeLotXRef.CommandText = "tProductSchemeLotNumberXRef";
//              cmndSchemeLotXRef.CommandType = CommandType.TableDirect;
//              cmndSchemeLotXRef.IndexName = 
//                  "PK_ProductSchemeLotNumberXRefOnProductSchemeRefnoAndProductSchemeLotNumberRefno";

//              using (SQLiteDataReader drSchemeLotXRef = cmndSchemeLotXRef.ExecuteReader())
//              {
//                using (SQLiteCommand cmndGetScheme = conn.CreateCommand())
//                {
//                  cmndGetScheme.CommandText = "tProductScheme";
//                  cmndGetScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";
//                  cmndGetScheme.CommandType = CommandType.TableDirect;

//                  using (SQLiteDataReader drScheme = cmndGetScheme.ExecuteReader())
//                  {
//                    using (SQLiteCommand cmndGetSchemeLot = conn.CreateCommand())
//                    {
//                      cmndGetSchemeLot.CommandText = "tProductSchemeLotNumber";
//                      cmndGetSchemeLot.IndexName = "UQ_ProductSchemeLotNumberOnProductSchemeLotNumberCode";
//                      cmndGetSchemeLot.CommandType = CommandType.TableDirect;

//                      using (SQLiteDataReader drSchemeLot = cmndGetSchemeLot.ExecuteReader())
//                      {
//                        using (SqlCeResultSet rs = cmndSchemeLotXRef.ExecuteResultSet(ResultSetOptions.Updatable))
//                        {
//                          SqlCeUpdatableRecord rec = rs.CreateRecord();
//                          int schemeRefno = 0;
//                          int schemeLotRefno = 0;
//                          int progValue = 0;
//                          bool isSchemeLotXRef = false;
//                          string schemeCode = string.Empty;
//                          string schemeLotCode = string.Empty;

//                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

//                          foreach (DataRow drow in ds.Tables["tProductSchemeLotNumberXRef"].Rows)
//                          {
//                            schemeCode = Convert.ToString(drow["ProductSchemeCode"]);
//                            errMsg = "Scheme code \'" + schemeCode + "\'";
//                            if (drScheme.Seek(DbSeekOptions.FirstEqual, new object[] { schemeCode }))
//                            {
//                              if (drScheme.Read())
//                              {
//                                schemeRefno = Convert.ToInt32(
//                                    drScheme.GetValue(
//                                    Convert.ToInt32(SysCommon.ProdSchemeFields.Refno)));
//                              }
//                              else
//                              {
//#if MULTILANGUAGE
//                                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrschemereadFailed) + Environment.NewLine + errMsg);
//#else
//                                throw new Exception("drScheme.Read() failed." + Environment.NewLine + errMsg);
//#endif
//                              }
//                            }
//                            else
//                            {
//                              throw new Exception(
//#if MULTILANGUAGE
//                                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductSchemeCodeInTproductscheme)
//#else
//                                  "Unable to locate the product scheme code in tProductScheme."
//#endif
//                                  + Environment.NewLine + errMsg);
//                            }

//                            schemeLotCode = Convert.ToString(drow["ProductSchemeLotNumberCode"]);
//                            errMsg += Environment.NewLine + "Scheme lot code \'" + schemeLotCode + "\'";
//                            if (drSchemeLot.Seek(DbSeekOptions.FirstEqual, new object[] { schemeLotCode }))
//                            {
//                              if (drSchemeLot.Read())
//                              {
//                                schemeLotRefno = Convert.ToInt32(
//                                    drSchemeLot.GetValue(
//                                    Convert.ToInt32(
//                                    SysCommon.ProdSchemeLotNumberFields.Refno)));
//                              }
//                              else
//                              {
//                                throw new Exception("drSchemeLot.Read() failed." + Environment.NewLine + errMsg);
//                              }
//                            }
//                            else
//                            {
//                              throw new Exception(
//#if MULTILANGUAGE
//                                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductSchemeLotNumberCodeInTproductschemelotnumber)
//#else
//                                  "Unable to locate the product scheme lot number code in tProductSchemeLotNumber."
//#endif
//                                  + Environment.NewLine + errMsg);
//                            }

//                            isSchemeLotXRef = false;
//                            if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
//                            {
//                              cmndSchemeLotXRef.CommandText = "tProductSchemeLotNumberXRef";
//                              cmndSchemeLotXRef.CommandType = CommandType.TableDirect;
//                              cmndSchemeLotXRef.IndexName =
//                                  "PK_ProductSchemeLotNumberXRefOnProductSchemeRefnoAndProductSchemeLotNumberRefno";
//                              if (drSchemeLotXRef.Seek(
//                                  DbSeekOptions.FirstEqual, 
//                                  new object[] { schemeRefno, schemeLotRefno }))
//                              {
//                                isSchemeLotXRef = true;
//                              }
//                            }

//                            if (!isSchemeLotXRef)
//                            {
//                              cmndSchemeLotXRef.CommandText = "tProductSchemeLotNumberXRef";
//                              cmndSchemeLotXRef.CommandType = CommandType.TableDirect;
//                              cmndSchemeLotXRef.IndexName = 
//                                  "PK_ProductSchemeLotNumberXRefOnProductSchemeRefnoAndProductSchemeLotNumberRefno";
//                              rec.SetInt32(
//                                  Convert.ToInt32(
//                                  SysCommon.ProdSchemeLotNumberXRefFields.ProdSchemeRefno), 
//                                  schemeRefno);

//                              rec.SetInt32(
//                                  Convert.ToInt32(
//                                  SysCommon.ProdSchemeLotNumberXRefFields.ProdSchemeLotNumberRefno), 
//                                  schemeLotRefno);

//                              rs.Insert(rec);
//                            }

//                            progValue++;
//                            if (progValue % 100 == 0)
//                            {
//                              this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
//                            }
//                          }

//                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
//                        }

//                        drSchemeLot.Close();
//                      }
//                    }

//                    drScheme.Close();
//                  }
//                }

//                drSchemeLotXRef.Close();
//              }
//            }
//          }

//          this.DataAccessStatusOutHandler(
//              this,
//              EventArgs.Empty,
//#if MULTILANGUAGE
//              Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductSchemeLotNumberXrefComplete),
//#else
//              "Saving product scheme lot number xref complete.",
//#endif
//              false);
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
//#endif
//        this.DataAccessStatusOutHandler(
//            this, 
//            EventArgs.Empty, 
//#if MULTILANGUAGE
//            Translator.GetInstance().Translate(TranslatorVocabulary.RequestProductSchemeLotNumberXrefFailed), 
//#else
//            "Request product scheme lot number xref failed.", 
//#endif
//            false);

//#if MULTILANGUAGE
//        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredRequestProductSchemeLotNumberXrefFailed) + errMsg, ex);
//#else
//        throw new Exception("Database exception occurred. Request product scheme lot number xref failed." + errMsg, ex);
//#endif
//      }
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
          + "<action>REQUEST PRODUCT SCHEME LOT NUMBER XREF</action>"
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
