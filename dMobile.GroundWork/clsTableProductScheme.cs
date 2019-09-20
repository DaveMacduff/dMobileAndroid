namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableProductScheme : TableBase
  {
    #region Constructors

    public TableProductScheme()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableProductScheme";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemes), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product schemes.", true);
#endif

        reqData = this.Send("product scheme");
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemesComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product schemes complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemesFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product schemes failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemesFailed), ex);
#else
        throw new Exception("Seeding product schemes failed.", ex);
#endif
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="schemeCode"></param>
    /// <returns></returns>
    public int GetProdSchemeRefnoFromProdSchemeCode(string schemeCode)
    {
      int schemeRefno = 0;

//      try
//      {
//        using (SQLiteCommand cmndScheme = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//        {
//          cmndScheme.CommandText = "tProductScheme";
//          cmndScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";
//          cmndScheme.CommandType = CommandType.TableDirect;

//          using (SQLiteDataReader drScheme = cmndScheme.ExecuteReader())
//          {
//            if (drScheme.Seek(DbSeekOptions.FirstEqual, new object[] { schemeCode }))
//            {
//              if (drScheme.Read())
//              {
//                schemeRefno = Convert.ToInt32(drScheme.GetValue((int)SysCommon.ProdSchemeFields.Refno));
//              }
//              else
//              {
//                throw new Exception(
//#if MULTILANGUAGE
//                    Translator.GetInstance().Translate(TranslatorVocabulary.DrschemereadFailed) + Environment.NewLine + Translator.GetInstance().Translate(TranslatorVocabulary.SchemeCode) + schemeCode + "\'");
//#else
//                    "drScheme.Read() failed." + Environment.NewLine + "Scheme code \'" + schemeCode + "\'");
//#endif
//              }
//            }
//            else
//            {
//              throw new Exception(
//#if MULTILANGUAGE
//                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductSchemeCodeInTproductscheme) 
//                  + Environment.NewLine + Translator.GetInstance().Translate(TranslatorVocabulary.SchemeCode) + schemeCode + "\'");
//#else
//                  "Unable to locate the product scheme code in tProductScheme." 
//                  + Environment.NewLine + "Scheme code \'" + schemeCode + "\'");
//#endif
//            }

//            drScheme.Close();
//          }
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetprodschemerefnofromprodschemecodeFailed), ex);
//#else
//        throw new Exception("Database exception occurred. GetProdSchemeRefnoFromProdSchemeCode failed.", ex);
//#endif
//      }

      return schemeRefno;
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override int DataStatus()
    {
      int rowsAffected = 0;

//      try
//      {
//        using (SQLiteCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//        {
//          cmndStatus.CommandText = "SELECT COUNT(*) FROM tProductScheme";
//          cmndStatus.CommandType = CommandType.Text;
//          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

//          this.DataAccessStatusOutHandler(
//              this,
//              EventArgs.Empty,
//#if MULTILANGUAGE
//              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ProductSchemes), rowsAffected),
//#else
//              string.Format("{0} product schemes.", rowsAffected),
//#endif
//              true);
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredProductSchemesStatusFailed), ex);
//#else
//        throw new Exception("Database exception occurred. Product schemes status failed.", ex);
//#endif
//      }

      return rowsAffected;
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Purge()
    {
//      try
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemes), true);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product schemes.", true);
//#endif

//        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//        {
//          cmndPurge.CommandText = "DELETE FROM tProductScheme";
//          cmndPurge.CommandType = CommandType.Text;
//          cmndPurge.ExecuteNonQuery();
//        }

//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemesFailed), true);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product schemes failed.", true);
//#endif
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemesFailed), true);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product schemes failed.", true);
//#endif

//#if MULTILANGUAGE
//        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgeProductSchemesFailed), ex);
//#else
//        throw new Exception("Database exception occurred. Purge product schemes failed.", ex);
//#endif
//      }
//    }

//    #endregion

//    #region Protected Methods

//    /// <summary>
//    ///
//    /// </summary>
//    /// <param name="reqData"></param>
//    /// <remarks></remarks>
//    protected override void Save(string reqData)
//    {
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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductSchemes), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product schemes.", false);
//#endif
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tProductScheme"].Rows.Count);

//            this.DataAccessStatusOutHandler(
//                this, 
//                EventArgs.Empty, 
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), ds.Tables["tProductScheme"].Rows.Count), 
//#else
//                string.Format("Records {0}", ds.Tables["tProductScheme"].Rows.Count), 
//#endif
//                false);

//            using (SQLiteCommand cmndScheme = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//            {
//              cmndScheme.CommandText = "tProductScheme";
//              cmndScheme.CommandType = CommandType.TableDirect;
//              cmndScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";

//              using (SQLiteDataReader drScheme = cmndScheme.ExecuteReader())
//              {
//                using (SqlCeResultSet rs = cmndScheme.ExecuteResultSet(ResultSetOptions.Updatable))
//                {
//                  SqlCeUpdatableRecord rec = rs.CreateRecord();
//                  int progValue = 0;
//                  string schemeCode = string.Empty;
//                  bool isSchemeFound = false;

//                  this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

//                  foreach (DataRow drow in ds.Tables["tProductScheme"].Rows)
//                  {
//                    schemeCode = drow["ProductSchemeCode"].ToString();
//                    errMsg = "Scheme code \'" + schemeCode + "\'";

//                    isSchemeFound = false;
//                    if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
//                    {
//                      cmndScheme.CommandText = "tProductScheme";
//                      cmndScheme.CommandType = CommandType.TableDirect;
//                      cmndScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";

//                      if (drScheme.Seek(DbSeekOptions.FirstEqual, new object[] { schemeCode }))
//                      {
//                        isSchemeFound = true;

//                        StringBuilder s = new StringBuilder();
//                        s.Append("UPDATE tProductScheme ");
//                        s.AppendFormat(
//                            "SET ProductSchemeIsSerialTracked = {0},", 
//                            Convert.ToBoolean(drow["ProductSchemeIsSerialTracked"]) ? 1 : 0);
//                        s.AppendFormat(
//                            "ProductSchemeIsProductionDateTracked = {0},", 
//                            Convert.ToBoolean(drow["ProductSchemeIsProductionDateTracked"]) ? 1 : 0);
//                        s.AppendFormat(
//                            "ProductSchemeIsLotTracked = {0},", 
//                            Convert.ToBoolean(drow["ProductSchemeIsLotTracked"]) ? 1 : 0);
//                        s.AppendFormat(
//                            "ProductSchemeIsBestBeforeDateTracked = {0},", 
//                            Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateTracked"]) ? 1 : 0);
//                        s.AppendFormat(
//                            "ProductSchemeIsVariantTracked = {0},", 
//                            Convert.ToBoolean(drow["ProductSchemeIsVariantTracked"]) ? 1 : 0);
//                        s.AppendFormat(
//                            "ProductSchemeIsLotUniqueComponent = {0},", 
//                            Convert.ToBoolean(drow["ProductSchemeIsLotUniqueComponent"]) ? 1 : 0);
//                        s.AppendFormat(
//                            "ProductSchemeIsProductionDateUniqueComponent = {0},", 
//                            Convert.ToBoolean(drow["ProductSchemeIsProductionDateUniqueComponent"]) ? 1 : 0);
//                        s.AppendFormat(
//                            "ProductSchemeIsBestBeforeDateUniqueComponent = {0},", 
//                            Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateUniqueComponent"]) ? 1 : 0);
//                        s.AppendFormat(
//                            "ProductSchemeIsVariantUniqueComponent = {0},", 
//                            Convert.ToBoolean(drow["ProductSchemeIsVariantUniqueComponent"]) ? 1 : 0);
//                        s.AppendFormat(
//                            "ProductSchemeExtra1Type = {0},", 
//                            Convert.ToInt32(drow["ProductSchemeExtra1Type"]));
//                        s.AppendFormat(
//                            "ProductSchemeExtra2Type = {0},", 
//                            Convert.ToInt32(drow["ProductSchemeExtra2Type"]));
//                        s.AppendFormat(
//                            "ProductSchemeExtra3Type = {0},", 
//                            Convert.ToInt32(drow["ProductSchemeExtra3Type"]));
//                        s.AppendFormat(
//                            "ProductSchemeExtra4Type = {0},", 
//                            Convert.ToInt32(drow["ProductSchemeExtra4Type"]));
//                        s.AppendFormat(
//                            "ProductSchemeExtra5Type = {0},", 
//                            Convert.ToInt32(drow["ProductSchemeExtra5Type"]));
//                        s.AppendFormat(
//                            "ProductSchemeExtra1Caption = \'{0}\',", 
//                            Convert.ToString(drow["ProductSchemeExtra1Caption"]));
//                        s.AppendFormat(
//                            "ProductSchemeExtra2Caption = \'{0}\',", 
//                            Convert.ToString(drow["ProductSchemeExtra2Caption"]));
//                        s.AppendFormat(
//                            "ProductSchemeExtra3Caption = \'{0}\',", 
//                            Convert.ToString(drow["ProductSchemeExtra3Caption"]));
//                        s.AppendFormat(
//                            "ProductSchemeExtra4Caption = \'{0}\',", 
//                            Convert.ToString(drow["ProductSchemeExtra4Caption"]));
//                        s.AppendFormat(
//                            "ProductSchemeExtra5Caption = \'{0}\',", 
//                            Convert.ToString(drow["ProductSchemeExtra5Caption"]));
//                        s.AppendFormat(
//                            "ProductSchemeExtra1IsRequired = {0},", 
//                            Convert.ToBoolean(drow["ProductSchemeExtra1IsRequired"]) ? 1 : 0);
//                        s.AppendFormat(
//                            "ProductSchemeExtra2IsRequired = {0},", 
//                            Convert.ToBoolean(drow["ProductSchemeExtra2IsRequired"]) ? 1 : 0);
//                        s.AppendFormat(
//                            "ProductSchemeExtra3IsRequired = {0},", 
//                            Convert.ToBoolean(drow["ProductSchemeExtra3IsRequired"]) ? 1 : 0);
//                        s.AppendFormat(
//                            "ProductSchemeExtra4IsRequired = {0},", 
//                            Convert.ToBoolean(drow["ProductSchemeExtra4IsRequired"]) ? 1 : 0);
//                        s.AppendFormat(
//                            "ProductSchemeExtra5IsRequired = {0},", 
//                            Convert.ToBoolean(drow["ProductSchemeExtra5IsRequired"]) ? 1 : 0);
//                        s.AppendFormat(
//                            "ProductSchemeIsLotTrackedDefault = {0},", 
//                            Convert.ToBoolean(drow["ProductSchemeIsLotTrackedDefault"]) ? 1 : 0);
//                        s.AppendFormat(
//                            "ProductSchemeIsProductionDateTrackedDefault = {0},", 
//                            Convert.ToBoolean(drow["ProductSchemeIsProductionDateTrackedDefault"]) ? 1 : 0);
//                        s.AppendFormat(
//                            "ProductSchemeIsBestBeforeDateTrackedDefault = {0} ", 
//                            Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateTrackedDefault"]) ? 1 : 0);
//                        s.AppendFormat("WHERE ProductSchemeCode = \'{0}\'", schemeCode);

//                        cmndScheme.CommandType = CommandType.Text;
//                        cmndScheme.IndexName = string.Empty;
//                        cmndScheme.CommandText = s.ToString();
//                        cmndScheme.ExecuteNonQuery();
//                      }
//                    }

//                    if (!isSchemeFound)
//                    {
//                      cmndScheme.CommandText = "tProductScheme";
//                      cmndScheme.CommandType = CommandType.TableDirect;
//                      cmndScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";
//                      rec.SetString((int)SysCommon.ProdSchemeFields.Code, schemeCode);
//                      rec.SetBoolean(
//                          (int)SysCommon.ProdSchemeFields.IsSerialTracked, 
//                          Convert.ToBoolean(drow["ProductSchemeIsSerialTracked"]));
//                      rec.SetBoolean(
//                          (int)SysCommon.ProdSchemeFields.IsProductionDateTracked, 
//                          Convert.ToBoolean(drow["ProductSchemeIsProductionDateTracked"]));
//                      rec.SetBoolean(
//                          (int)SysCommon.ProdSchemeFields.IsBestBeforeDateTracked, 
//                          Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateTracked"]));
//                      rec.SetBoolean(
//                          (int)SysCommon.ProdSchemeFields.IsLotTracked, 
//                          Convert.ToBoolean(drow["ProductSchemeIsLotTracked"]));
//                      rec.SetBoolean(
//                          (int)SysCommon.ProdSchemeFields.IsVariantTracked, 
//                          Convert.ToBoolean(drow["ProductSchemeIsVariantTracked"]));
//                      rec.SetBoolean(
//                          (int)SysCommon.ProdSchemeFields.IsLotUniqueComponent, 
//                          Convert.ToBoolean(drow["ProductSchemeIsLotUniqueComponent"]));
//                      rec.SetBoolean(
//                          (int)SysCommon.ProdSchemeFields.IsProductionDateUniqueComponent, 
//                          Convert.ToBoolean(drow["ProductSchemeIsProductionDateUniqueComponent"]));
//                      rec.SetBoolean(
//                          (int)SysCommon.ProdSchemeFields.IsBestBeforeDateUniqueComponent, 
//                          Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateUniqueComponent"]));
//                      rec.SetBoolean(
//                          (int)SysCommon.ProdSchemeFields.IsVariantUniqueComponent, 
//                          Convert.ToBoolean(drow["ProductSchemeIsVariantUniqueComponent"]));
//                      rec.SetInt32(
//                          (int)SysCommon.ProdSchemeFields.Extra1Type, 
//                          Convert.ToInt32(drow["ProductSchemeExtra1Type"]));
//                      rec.SetInt32(
//                          (int)SysCommon.ProdSchemeFields.Extra2Type, 
//                          Convert.ToInt32(drow["ProductSchemeExtra2Type"]));
//                      rec.SetInt32(
//                          (int)SysCommon.ProdSchemeFields.Extra3Type, 
//                          Convert.ToInt32(drow["ProductSchemeExtra3Type"]));
//                      rec.SetInt32(
//                          (int)SysCommon.ProdSchemeFields.Extra4Type, 
//                          Convert.ToInt32(drow["ProductSchemeExtra4Type"]));
//                      rec.SetInt32(
//                          (int)SysCommon.ProdSchemeFields.Extra5Type, 
//                          Convert.ToInt32(drow["ProductSchemeExtra5Type"]));
//                      rec.SetString(
//                          (int)SysCommon.ProdSchemeFields.Extra1Caption, 
//                          drow["ProductSchemeExtra1Caption"].ToString());
//                      rec.SetString(
//                          (int)SysCommon.ProdSchemeFields.Extra2Caption, 
//                          drow["ProductSchemeExtra2Caption"].ToString());
//                      rec.SetString(
//                          (int)SysCommon.ProdSchemeFields.Extra3Caption, 
//                          drow["ProductSchemeExtra3Caption"].ToString());
//                      rec.SetString(
//                          (int)SysCommon.ProdSchemeFields.Extra4Caption, 
//                          drow["ProductSchemeExtra4Caption"].ToString());
//                      rec.SetString(
//                          (int)SysCommon.ProdSchemeFields.Extra5Caption, 
//                          drow["ProductSchemeExtra5Caption"].ToString());
//                      rec.SetBoolean(
//                          (int)SysCommon.ProdSchemeFields.Extra1IsRequired,
//                          Convert.ToBoolean(drow["ProductSchemeExtra1IsRequired"]));
//                      rec.SetBoolean(
//                          (int)SysCommon.ProdSchemeFields.Extra2IsRequired, 
//                          Convert.ToBoolean(drow["ProductSchemeExtra2IsRequired"]));
//                      rec.SetBoolean(
//                          (int)SysCommon.ProdSchemeFields.Extra3IsRequired, 
//                          Convert.ToBoolean(drow["ProductSchemeExtra3IsRequired"]));
//                      rec.SetBoolean(
//                          (int)SysCommon.ProdSchemeFields.Extra4IsRequired, 
//                          Convert.ToBoolean(drow["ProductSchemeExtra4IsRequired"]));
//                      rec.SetBoolean(
//                          (int)SysCommon.ProdSchemeFields.Extra5IsRequired, 
//                          Convert.ToBoolean(drow["ProductSchemeExtra5IsRequired"]));
//                      rec.SetBoolean(
//                          (int)SysCommon.ProdSchemeFields.IsLotTrackedDefault, 
//                          Convert.ToBoolean(drow["ProductSchemeIsLotTrackedDefault"]));
//                      rec.SetBoolean(
//                          (int)SysCommon.ProdSchemeFields.IsProductionDateTrackedDefault,
//                          Convert.ToBoolean(drow["ProductSchemeIsProductionDateTrackedDefault"]));
//                      rec.SetBoolean(
//                          (int)SysCommon.ProdSchemeFields.IsBestBeforeDateTrackedDefault,
//                          Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateTrackedDefault"]));

//                      rs.Insert(rec);
//                    }

//                    progValue++;
//                    if (progValue % 100 == 0)
//                    {
//                      this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
//                    }
//                  }

//                  this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
//                }

//                drScheme.Close();
//              }
//            }
//          }

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductSchemesComplete), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product schemes complete.", false);
//#endif
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductSchemesFailed), false);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product schemes failed.", false);
//#endif

//        // Duplicate key violations
//        if (ex.NativeError == 25016)
//        {
//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation), false);
//          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation) + Environment.NewLine + errMsg, ex);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
//          throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
//#endif
//        }

//        throw new Exception(
//#if MULTILANGUAGE
//          Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingProductSchemesFailed) + Environment.NewLine + errMsg,
//#else
//          "Database exception occurred. Saving product schemes failed." + Environment.NewLine + errMsg,
//#endif
//          ex);
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
          + "<action>REQUEST PRODUCT SCHEMES</action>"
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
