namespace dMobile.GroundWork
{
  using System;
  using System.Data;
    using System.Data.SQLite;
    using System.Text;
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

      // TODO: Uncomment GC calls
      //GC.Collect();
      //GC.WaitForPendingFinalizers();

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
            catch (SQLiteException ex)
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

//      try
//      {
//        using (SQLiteCommand cmndSchemeVar = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//        {
//          cmndSchemeVar.CommandText = "tProductSchemeVariant";
//          cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";
//          cmndSchemeVar.CommandType = CommandType.TableDirect;

//          using (SQLiteDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
//          {
//            if (drSchemeVar.Seek(DbSeekOptions.FirstEqual, new object[] { schemeVarNo }))
//            {
//              if (drSchemeVar.Read())
//              {
//                schemeVarRefno = Convert.ToInt32(drSchemeVar.GetValue((int)SysCommon.ProdSchemeVariantFields.Refno));
//              }
//              else
//              {
//#if MULTILANGUAGE
//                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrschemevarreadFailed));
//#else
//                throw new Exception("drSchemeVar.Read() failed.");
//#endif
//              }
//            }
//            else
//            {
//              throw new Exception(
//                  string.Format(
//#if MULTILANGUAGE
//                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateProductSchemeVariantNumberInTproductschemevariant), 
//#else
//                  "Unable To Locate Product Scheme Variant Number {0} In tProductSchemeVariant", 
//#endif
//                  schemeVarNo));
//            }

//            drSchemeVar.Close();
//          }
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetprodschemevarrefnofromprodschemevarnoFailed), ex);
//#else
//        throw new Exception("Database exception occurred. GetProdSchemeVarRefnoFromProdSchemeVarNo failed.", ex);
//#endif
//      }

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
            catch (SQLiteException ex)
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
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            try
            {
                using (DataSet ds = new DataSet())
                {
                    // TODO: Resource String
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                    // TODO: Resource String
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variants.", false);

                    if (ds.Tables.Count != 0)
                    {
                        int progMax = Convert.ToInt32(ds.Tables["tProductSchemeVariant"].Rows.Count);

                        // TODO: Resource String
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                        using (SQLiteCommand cmndSchemeVar = conn.CreateCommand())
                        using (SQLiteCommand cmndSchemeVarUpdate = conn.CreateCommand())
                        using (SQLiteTransaction transaction = conn.BeginTransaction())
                        {
                            cmndSchemeVar.CommandText = "SELECT ProductSchemeVariantNo FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = @productSchemeVariantNo";
                            cmndSchemeVar.CommandType = CommandType.Text;
                            //cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";

                            //using (SqlCeResultSet rs = cmndSchemeVar.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //    SqlCeUpdatableRecord rec = rs.CreateRecord();
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

                                cmndSchemeVar.Parameters.Add(new SQLiteParameter("@productSchemeVariantNo", schemeVarNo));

                                using (SQLiteDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
                                {
                                    //cmndSchemeVar.CommandText = "tProductSchemeVariant";
                                    //cmndSchemeVar.CommandType = CommandType.Text;
                                    //cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";

                                    //if (drSchemeVar.Seek(DbSeekOptions.FirstEqual, new object[] { schemeVarNo }))
                                    if (drSchemeVar.Read())
                                    {
                                        isSchemeVarFound = true;

                                        if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                        {
                                            cmndSchemeVarUpdate.Parameters.Clear();
                                            cmndSchemeVarUpdate.CommandType = CommandType.Text;
                                            cmndSchemeVarUpdate.CommandText = "UPDATE tProductSchemeVariant "
                                                + "SET ProductSchemeVariantCode = \'" + schemeVarCode + "\', "
                                                + "ProductSchemeVariantDescription = \'" + schemeVarDesc + "\', "
                                                + "ProductSchemeVariantIsDefault = " + Convert.ToString(schemeVarIsDefault ? 1 : 0)
                                                + " WHERE ProductSchemeVariantNo = " + schemeVarNo;
                                            cmndSchemeVarUpdate.ExecuteNonQuery();
                                        }
                                    }

                                    if (!isSchemeVarFound)
                                    {
                                        using (SQLiteCommand cmndSchemeVarInsert = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                                        {
                                            cmndSchemeVarInsert.CommandText = "INSERT INTO tProductSchemeVariant (ProductSchemeVariantCode, ProductSchemeVariantDescription, ProductSchemeVariantNo, ProductSchemeVariantIsDefault) " +
                                            "VALUES (@productSchemeVariantCode, @productSchemeVariantDescription, @productSchemeVariantNo, @productSchemeVariantIsDefault)";
                                            cmndSchemeVar.CommandType = CommandType.Text;
                                            //cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";

                                            cmndSchemeVarInsert.Parameters.Add(new SQLiteParameter("@productSchemeVariantCode", schemeVarCode));
                                            cmndSchemeVarInsert.Parameters.Add(new SQLiteParameter("@productSchemeVariantDescription", schemeVarDesc));
                                            cmndSchemeVarInsert.Parameters.Add(new SQLiteParameter("@productSchemeVariantNo", schemeVarNo));
                                            cmndSchemeVarInsert.Parameters.Add(new SQLiteParameter("@productSchemeVariantIsDefault", schemeVarIsDefault));

                                            cmndSchemeVarInsert.ExecuteNonQuery();
                                        }
                                        //rec.SetString((int)SysCommon.ProdSchemeVariantFields.Code, schemeVarCode);
                                        //rec.SetString((int)SysCommon.ProdSchemeVariantFields.Description, schemeVarDesc);
                                        //rec.SetInt32((int)SysCommon.ProdSchemeVariantFields.No, schemeVarNo);
                                        //rec.SetBoolean((int)SysCommon.ProdSchemeVariantFields.IsDefault, schemeVarIsDefault);

                                        //rs.Insert(rec);
                                    }

                                    drSchemeVar.Close();
                                }

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                                //}

                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);

                            }

                            //drSchemeVar.Close();
                            //}

                            transaction.Commit();
                        }
                    }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductSchemeVariantsComplete), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variants complete.", false);
#endif
                }
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductSchemeVariantsComplete), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product scheme variants complete.", false);
#endif

//                // Duplicate key violations
//                if (ex.NativeError == 25016)
//                {
//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation), false);
//          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation) + Environment.NewLine + errMsg, ex);
//#else
//                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
//                    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
//#endif
//                }

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
