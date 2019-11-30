namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
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

      // TODO: Uncomment GC calls
      //GC.Collect();
      //GC.WaitForPendingFinalizers();

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

            try
            {
                using (SQLiteCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tProductScheme";
                    cmndStatus.CommandType = CommandType.Text;
                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ProductSchemes), rowsAffected),
#else
              string.Format("{0} product schemes.", rowsAffected),
#endif
              true);
                }
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredProductSchemesStatusFailed), ex);
#else
                throw new Exception("Database exception occurred. Product schemes status failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemes), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product schemes.", true);
#endif

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tProductScheme";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

#if MULTILANGUAGE
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemesFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product schemes failed.", true);
#endif
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemesFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product schemes failed.", true);
#endif

#if MULTILANGUAGE
                    throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgeProductSchemesFailed), ex);
#else
                throw new Exception("Database exception occurred. Purge product schemes failed.", ex);
#endif
            }
        }

        //    /// <summary>
        //    ///
        //    /// </summary>
        //    /// <param name="reqData"></param>
        //    /// <remarks></remarks>

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
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product schemes.", false);

                    if (ds.Tables.Count != 0)
                    {
                        int progMax = Convert.ToInt32(ds.Tables["tProductScheme"].Rows.Count);

                        // TODO: Resource String
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", ds.Tables["tProductScheme"].Rows.Count), false);

                        using (SQLiteCommand cmndScheme = conn.CreateCommand())
                        using (SQLiteTransaction transaction = conn.BeginTransaction())
                        {
                            cmndScheme.CommandText = "SELECT ProductSchemeCode FROM tProductScheme WHERE ProductSchemeCode = @productSchemeCode";
                            cmndScheme.CommandType = CommandType.Text;
                            //cmndScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";

                            //using (SqlCeResultSet rs = cmndScheme.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //    SqlCeUpdatableRecord rec = rs.CreateRecord();
                            int progValue = 0;
                            string schemeCode = string.Empty;
                            bool isSchemeFound = false;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tProductScheme"].Rows)
                            {
                                schemeCode = drow["ProductSchemeCode"].ToString();
                                errMsg = "Scheme code \'" + schemeCode + "\'";
                                isSchemeFound = false;

                                cmndScheme.Parameters.Add(new SQLiteParameter("@productSchemeCode", schemeCode));

                                using (SQLiteDataReader drScheme = cmndScheme.ExecuteReader())
                                {
                                    //cmndScheme.CommandText = "tProductScheme";
                                    //cmndScheme.CommandType = CommandType.TableDirect;
                                    //cmndScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";

                                    //if (drScheme.Seek(DbSeekOptions.FirstEqual, new object[] { schemeCode }))
                                    if (drScheme.Read())
                                    {
                                        isSchemeFound = true;

                                        if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                        {
                                            StringBuilder s = new StringBuilder();
                                            s.Append("UPDATE tProductScheme ");
                                            s.AppendFormat(
                                                "SET ProductSchemeIsSerialTracked = {0},",
                                                Convert.ToBoolean(drow["ProductSchemeIsSerialTracked"]) ? 1 : 0);
                                            s.AppendFormat(
                                                "ProductSchemeIsProductionDateTracked = {0},",
                                                Convert.ToBoolean(drow["ProductSchemeIsProductionDateTracked"]) ? 1 : 0);
                                            s.AppendFormat(
                                                "ProductSchemeIsLotTracked = {0},",
                                                Convert.ToBoolean(drow["ProductSchemeIsLotTracked"]) ? 1 : 0);
                                            s.AppendFormat(
                                                "ProductSchemeIsBestBeforeDateTracked = {0},",
                                                Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateTracked"]) ? 1 : 0);
                                            s.AppendFormat(
                                                "ProductSchemeIsVariantTracked = {0},",
                                                Convert.ToBoolean(drow["ProductSchemeIsVariantTracked"]) ? 1 : 0);
                                            s.AppendFormat(
                                                "ProductSchemeIsLotUniqueComponent = {0},",
                                                Convert.ToBoolean(drow["ProductSchemeIsLotUniqueComponent"]) ? 1 : 0);
                                            s.AppendFormat(
                                                "ProductSchemeIsProductionDateUniqueComponent = {0},",
                                                Convert.ToBoolean(drow["ProductSchemeIsProductionDateUniqueComponent"]) ? 1 : 0);
                                            s.AppendFormat(
                                                "ProductSchemeIsBestBeforeDateUniqueComponent = {0},",
                                                Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateUniqueComponent"]) ? 1 : 0);
                                            s.AppendFormat(
                                                "ProductSchemeIsVariantUniqueComponent = {0},",
                                                Convert.ToBoolean(drow["ProductSchemeIsVariantUniqueComponent"]) ? 1 : 0);
                                            s.AppendFormat(
                                                "ProductSchemeExtra1Type = {0},",
                                                Convert.ToInt32(drow["ProductSchemeExtra1Type"]));
                                            s.AppendFormat(
                                                "ProductSchemeExtra2Type = {0},",
                                                Convert.ToInt32(drow["ProductSchemeExtra2Type"]));
                                            s.AppendFormat(
                                                "ProductSchemeExtra3Type = {0},",
                                                Convert.ToInt32(drow["ProductSchemeExtra3Type"]));
                                            s.AppendFormat(
                                                "ProductSchemeExtra4Type = {0},",
                                                Convert.ToInt32(drow["ProductSchemeExtra4Type"]));
                                            s.AppendFormat(
                                                "ProductSchemeExtra5Type = {0},",
                                                Convert.ToInt32(drow["ProductSchemeExtra5Type"]));
                                            s.AppendFormat(
                                                "ProductSchemeExtra1Caption = \'{0}\',",
                                                Convert.ToString(drow["ProductSchemeExtra1Caption"]));
                                            s.AppendFormat(
                                                "ProductSchemeExtra2Caption = \'{0}\',",
                                                Convert.ToString(drow["ProductSchemeExtra2Caption"]));
                                            s.AppendFormat(
                                                "ProductSchemeExtra3Caption = \'{0}\',",
                                                Convert.ToString(drow["ProductSchemeExtra3Caption"]));
                                            s.AppendFormat(
                                                "ProductSchemeExtra4Caption = \'{0}\',",
                                                Convert.ToString(drow["ProductSchemeExtra4Caption"]));
                                            s.AppendFormat(
                                                "ProductSchemeExtra5Caption = \'{0}\',",
                                                Convert.ToString(drow["ProductSchemeExtra5Caption"]));
                                            s.AppendFormat(
                                                "ProductSchemeExtra1IsRequired = {0},",
                                                Convert.ToBoolean(drow["ProductSchemeExtra1IsRequired"]) ? 1 : 0);
                                            s.AppendFormat(
                                                "ProductSchemeExtra2IsRequired = {0},",
                                                Convert.ToBoolean(drow["ProductSchemeExtra2IsRequired"]) ? 1 : 0);
                                            s.AppendFormat(
                                                "ProductSchemeExtra3IsRequired = {0},",
                                                Convert.ToBoolean(drow["ProductSchemeExtra3IsRequired"]) ? 1 : 0);
                                            s.AppendFormat(
                                                "ProductSchemeExtra4IsRequired = {0},",
                                                Convert.ToBoolean(drow["ProductSchemeExtra4IsRequired"]) ? 1 : 0);
                                            s.AppendFormat(
                                                "ProductSchemeExtra5IsRequired = {0},",
                                                Convert.ToBoolean(drow["ProductSchemeExtra5IsRequired"]) ? 1 : 0);
                                            s.AppendFormat(
                                                "ProductSchemeIsLotTrackedDefault = {0},",
                                                Convert.ToBoolean(drow["ProductSchemeIsLotTrackedDefault"]) ? 1 : 0);
                                            s.AppendFormat(
                                                "ProductSchemeIsProductionDateTrackedDefault = {0},",
                                                Convert.ToBoolean(drow["ProductSchemeIsProductionDateTrackedDefault"]) ? 1 : 0);
                                            s.AppendFormat(
                                                "ProductSchemeIsBestBeforeDateTrackedDefault = {0} ",
                                                Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateTrackedDefault"]) ? 1 : 0);
                                            s.AppendFormat("WHERE ProductSchemeCode = \'{0}\'", schemeCode);

                                            using (SQLiteCommand cmndSchemeUpdate = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                                            {
                                                cmndSchemeUpdate.CommandText = s.ToString();
                                                cmndSchemeUpdate.CommandType = CommandType.Text;
                                                cmndSchemeUpdate.ExecuteNonQuery();
                                            }
                                        }
                                    }

                                    if (!isSchemeFound)
                                    {
                                        using (SQLiteCommand cmndSchemeInsert = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                                        {
                                            cmndSchemeInsert.CommandText = "INSERT INTO tProductScheme " +
                                                "(ProductSchemeCode, " +
                                                "ProductSchemeIsSerialTracked, " +
                                                "ProductSchemeIsProductionDateTracked, " +
                                                "ProductSchemeIsLotTracked, " +
                                                "ProductSchemeIsBestBeforeDateTracked, " +
                                                "ProductSchemeIsVariantTracked, " +
                                                "ProductSchemeIsLotUniqueComponent, " +
                                                "ProductSchemeIsProductionDateUniqueComponent, " +
                                                "ProductSchemeIsBestBeforeDateUniqueComponent, " +
                                                "ProductSchemeIsVariantUniqueComponent, " +
                                                "ProductSchemeExtra1Type, " +
                                                "ProductSchemeExtra2Type, " +
                                                "ProductSchemeExtra3Type, " +
                                                "ProductSchemeExtra4Type, " +
                                                "ProductSchemeExtra5Type, " +
                                                "ProductSchemeExtra1Caption, " +
                                                "ProductSchemeExtra2Caption, " +
                                                "ProductSchemeExtra3Caption, " +
                                                "ProductSchemeExtra4Caption, " +
                                                "ProductSchemeExtra5Caption, " +
                                                "ProductSchemeExtra1IsRequired, " +
                                                "ProductSchemeExtra2IsRequired, " +
                                                "ProductSchemeExtra3IsRequired, " +
                                                "ProductSchemeExtra4IsRequired, " +
                                                "ProductSchemeExtra5IsRequired," +
                                                "ProductSchemeIsLotTrackedDefault, " +
                                                "ProductSchemeIsProductionDateTrackedDefault, " +
                                                "ProductSchemeIsBestBeforeDateTrackedDefault) " +
                                                "VALUES " +
                                                "(@productSchemeCode, " +
                                                "@productSchemeIsSerialTracked, " +
                                                "@productSchemeIsProductionDateTracked, " +
                                                "@productSchemeIsLotTracked, " +
                                                "@productSchemeIsBestBeforeDateTracked, " +
                                                "@productSchemeIsVariantTracked, " +
                                                "@productSchemeIsLotUniqueComponent, " +
                                                "@productSchemeIsProductionDateUniqueComponent, " +
                                                "@productSchemeIsBestBeforeDateUniqueComponent, " +
                                                "@productSchemeIsVariantUniqueComponent, " +
                                                "@productSchemeExtra1Type, " +
                                                "@productSchemeExtra2Type, " +
                                                "@productSchemeExtra3Type, " +
                                                "@productSchemeExtra4Type, " +
                                                "@productSchemeExtra5Type, " +
                                                "@productSchemeExtra1Caption, " +
                                                "@productSchemeExtra2Caption, " +
                                                "@productSchemeExtra3Caption, " +
                                                "@productSchemeExtra4Caption, " +
                                                "@productSchemeExtra5Caption, " +
                                                "@productSchemeExtra1IsRequired, " +
                                                "@productSchemeExtra2IsRequired, " +
                                                "@productSchemeExtra3IsRequired, " +
                                                "@productSchemeExtra4IsRequired, " +
                                                "@productSchemeExtra5IsRequired, " +
                                                "@productSchemeIsLotTrackedDefault, " +
                                                "@productSchemeIsProductionDateTrackedDefault, " +
                                                "@productSchemeIsBestBeforeDateTrackedDefault)";
                                            cmndSchemeInsert.CommandType = CommandType.Text;
                                            //cmndScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeCode", schemeCode));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeIsSerialTracked", Convert.ToBoolean(drow["ProductSchemeIsSerialTracked"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeIsProductionDateTracked", Convert.ToBoolean(drow["ProductSchemeIsProductionDateTracked"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeIsLotTracked", Convert.ToBoolean(drow["ProductSchemeIsLotTracked"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeIsBestBeforeDateTracked", Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateTracked"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeIsVariantTracked", Convert.ToBoolean(drow["ProductSchemeIsVariantTracked"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeIsLotUniqueComponent", Convert.ToBoolean(drow["ProductSchemeIsLotUniqueComponent"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeIsProductionDateUniqueComponent", Convert.ToBoolean(drow["ProductSchemeIsProductionDateUniqueComponent"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeIsBestBeforeDateUniqueComponent", Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateUniqueComponent"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeIsVariantUniqueComponent", Convert.ToBoolean(drow["ProductSchemeIsVariantUniqueComponent"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeExtra1Type", Convert.ToInt32(drow["ProductSchemeExtra1Type"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeExtra2Type", Convert.ToInt32(drow["ProductSchemeExtra2Type"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeExtra3Type", Convert.ToInt32(drow["ProductSchemeExtra3Type"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeExtra4Type", Convert.ToInt32(drow["ProductSchemeExtra4Type"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeExtra5Type", Convert.ToInt32(drow["ProductSchemeExtra5Type"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeExtra1Caption", drow["ProductSchemeExtra1Caption"].ToString()));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeExtra2Caption", drow["ProductSchemeExtra2Caption"].ToString()));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeExtra3Caption", drow["ProductSchemeExtra3Caption"].ToString()));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeExtra4Caption", drow["ProductSchemeExtra4Caption"].ToString()));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeExtra5Caption", drow["ProductSchemeExtra5Caption"].ToString()));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeExtra1IsRequired", Convert.ToBoolean(drow["ProductSchemeExtra1IsRequired"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeExtra2IsRequired", Convert.ToBoolean(drow["ProductSchemeExtra2IsRequired"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeExtra3IsRequired", Convert.ToBoolean(drow["ProductSchemeExtra3IsRequired"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeExtra4IsRequired", Convert.ToBoolean(drow["ProductSchemeExtra4IsRequired"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeExtra5IsRequired", Convert.ToBoolean(drow["ProductSchemeExtra5IsRequired"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeIsLotTrackedDefault", Convert.ToBoolean(drow["ProductSchemeIsLotTrackedDefault"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeIsProductionDateTrackedDefault", Convert.ToBoolean(drow["ProductSchemeIsProductionDateTrackedDefault"])));
                                            cmndSchemeInsert.Parameters.Add(new SQLiteParameter("@productSchemeIsBestBeforeDateTrackedDefault", Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateTrackedDefault"])));

                                            cmndSchemeInsert.ExecuteNonQuery();

                                            //rec.SetString((int)SysCommon.ProdSchemeFields.Code, schemeCode);
                                            //rec.SetBoolean((int)SysCommon.ProdSchemeFields.IsSerialTracked, Convert.ToBoolean(drow["ProductSchemeIsSerialTracked"]));
                                            //rec.SetBoolean((int)SysCommon.ProdSchemeFields.IsProductionDateTracked, Convert.ToBoolean(drow["ProductSchemeIsProductionDateTracked"]));
                                            //rec.SetBoolean((int)SysCommon.ProdSchemeFields.IsBestBeforeDateTracked, Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateTracked"]));
                                            //rec.SetBoolean((int)SysCommon.ProdSchemeFields.IsLotTracked, Convert.ToBoolean(drow["ProductSchemeIsLotTracked"]));
                                            //rec.SetBoolean((int)SysCommon.ProdSchemeFields.IsVariantTracked, Convert.ToBoolean(drow["ProductSchemeIsVariantTracked"]));
                                            //rec.SetBoolean((int)SysCommon.ProdSchemeFields.IsLotUniqueComponent, Convert.ToBoolean(drow["ProductSchemeIsLotUniqueComponent"]));
                                            //rec.SetBoolean((int)SysCommon.ProdSchemeFields.IsProductionDateUniqueComponent, Convert.ToBoolean(drow["ProductSchemeIsProductionDateUniqueComponent"]));
                                            //rec.SetBoolean((int)SysCommon.ProdSchemeFields.IsBestBeforeDateUniqueComponent, Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateUniqueComponent"]));
                                            //rec.SetBoolean((int)SysCommon.ProdSchemeFields.IsVariantUniqueComponent, Convert.ToBoolean(drow["ProductSchemeIsVariantUniqueComponent"]));

                                            //rec.SetInt32((int)SysCommon.ProdSchemeFields.Extra1Type, Convert.ToInt32(drow["ProductSchemeExtra1Type"]));
                                            //rec.SetInt32((int)SysCommon.ProdSchemeFields.Extra2Type, Convert.ToInt32(drow["ProductSchemeExtra2Type"]));
                                            //rec.SetInt32((int)SysCommon.ProdSchemeFields.Extra3Type, Convert.ToInt32(drow["ProductSchemeExtra3Type"]));
                                            //rec.SetInt32((int)SysCommon.ProdSchemeFields.Extra4Type, Convert.ToInt32(drow["ProductSchemeExtra4Type"]));
                                            //rec.SetInt32((int)SysCommon.ProdSchemeFields.Extra5Type, Convert.ToInt32(drow["ProductSchemeExtra5Type"]));

                                            //rec.SetString((int)SysCommon.ProdSchemeFields.Extra1Caption, drow["ProductSchemeExtra1Caption"].ToString());
                                            //rec.SetString((int)SysCommon.ProdSchemeFields.Extra2Caption, drow["ProductSchemeExtra2Caption"].ToString());
                                            //rec.SetString((int)SysCommon.ProdSchemeFields.Extra3Caption, drow["ProductSchemeExtra3Caption"].ToString());
                                            //rec.SetString((int)SysCommon.ProdSchemeFields.Extra4Caption, drow["ProductSchemeExtra4Caption"].ToString());
                                            //rec.SetString((int)SysCommon.ProdSchemeFields.Extra5Caption, drow["ProductSchemeExtra5Caption"].ToString());

                                            //rec.SetBoolean((int)SysCommon.ProdSchemeFields.Extra1IsRequired, Convert.ToBoolean(drow["ProductSchemeExtra1IsRequired"]));
                                            //rec.SetBoolean((int)SysCommon.ProdSchemeFields.Extra2IsRequired, Convert.ToBoolean(drow["ProductSchemeExtra2IsRequired"]));
                                            //rec.SetBoolean((int)SysCommon.ProdSchemeFields.Extra3IsRequired, Convert.ToBoolean(drow["ProductSchemeExtra3IsRequired"]));
                                            //rec.SetBoolean((int)SysCommon.ProdSchemeFields.Extra4IsRequired, Convert.ToBoolean(drow["ProductSchemeExtra4IsRequired"]));
                                            //rec.SetBoolean((int)SysCommon.ProdSchemeFields.Extra5IsRequired, Convert.ToBoolean(drow["ProductSchemeExtra5IsRequired"]));
                                            //rec.SetBoolean((int)SysCommon.ProdSchemeFields.IsLotTrackedDefault, Convert.ToBoolean(drow["ProductSchemeIsLotTrackedDefault"]));
                                            //rec.SetBoolean((int)SysCommon.ProdSchemeFields.IsProductionDateTrackedDefault, Convert.ToBoolean(drow["ProductSchemeIsProductionDateTrackedDefault"]));
                                            //rec.SetBoolean((int)SysCommon.ProdSchemeFields.IsBestBeforeDateTrackedDefault, Convert.ToBoolean(drow["ProductSchemeIsBestBeforeDateTrackedDefault"]));

                                            //rs.Insert(rec);
                                        }
                                    }

                                    progValue++;
                                    if (progValue % 100 == 0)
                                    {
                                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                    }

                                    drScheme.Close();
                                }

                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);

                                //}
                                //drScheme.Close();
                            }

                            transaction.Commit();
                        }
                    }

                    // TODO: Resource String
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product schemes complete.", false);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource String
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product schemes failed.", false);

                //// Duplicate key violations
                //if (ex.NativeError == 25016)
                //{
                //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                //}

                // TODO: Resource String
                throw new Exception("Database exception occurred. Saving product schemes failed." + Environment.NewLine + errMsg, ex);
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
