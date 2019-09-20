namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableRoute : TableBase
  {
    #region Constructors

    public TableRoute()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableRoute";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingRoutes), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding routes.", true);
#endif

        reqData = this.Send("routes");
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingRoutesComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding routes complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingRoutesFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding routes failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingRoutesFailed), ex);
#else
        throw new Exception("Seeding routes failed.", ex);
#endif
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="routeCode"></param>
    /// <returns></returns>
    public int GetiRouteRefnoFromRouteCode(string routeCode)
    {
      int routeRefno = 0;

//      try
//      {
//        using (SQLiteCommand cmndRoute = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//        {
//          cmndRoute.CommandText = "tRoute";
//          cmndRoute.IndexName = "UQ_RouteOnRouteCode";
//          cmndRoute.CommandType = CommandType.TableDirect;

//          using (SQLiteDataReader drRoute = cmndRoute.ExecuteReader())
//          {
//            if (drRoute.Seek(DbSeekOptions.FirstEqual, new object[] { routeCode }))
//            {
//              if (drRoute.Read())
//              {
//                routeRefno = Convert.ToInt32(drRoute.GetValue((int)SysCommon.RouteFields.Refno));
//              }
//              else
//              {
//#if MULTILANGUAGE
//                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrroutereadFailed));
//#else
//                throw new Exception("drRoute.Read() failed.");
//#endif
//              }
//            }
//            else
//            {
//              throw new Exception(
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateRouteRefnoFromRouteCodeInTroute), routeCode));
//#else
//                string.Format("Unable To Locate Route Refno From Route Code {0} In tRoute", routeCode));
//#endif
//            }

//            drRoute.Close();
//          }
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetirouterefnofromroutecodeFailed), ex);
//#else
//        throw new Exception("Database exception occurred. GetiRouteRefnoFromRouteCode failed.", ex);
//#endif
//      }

      return routeRefno;
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
          cmndStatus.CommandText = "SELECT COUNT(*) FROM tRoute";
          cmndStatus.CommandType = CommandType.Text;
          
          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Routes), rowsAffected),
#else
              string.Format("{0} routes.", rowsAffected),
#endif
              true);
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredRoutesStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Routes status failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingRoutes), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging routes.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tRoute";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingRoutesComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging routes complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingRoutesFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging routes failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingRoutesFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging routes failed.", ex);
#endif
      }
    }

    #endregion

    #region Protected Methods

    /// <summary>
    ///
    /// </summary>
    /// <param name="reqData"></param>
    /// <remarks></remarks>
    protected override void Save(string reqData)
    {
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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingRoutes), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving routes.", false);
//#endif
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tRoute"].Rows.Count);

//            this.DataAccessStatusOutHandler(
//                this, 
//                EventArgs.Empty, 
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), 
//#else
//                string.Format("Records {0}", progMax), 
//#endif
//                false);

//            using (SQLiteCommand cmndRoute = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//            {
//              cmndRoute.CommandText = "tRoute";
//              cmndRoute.CommandType = CommandType.TableDirect;
//              cmndRoute.IndexName = "UQ_RouteOnRouteCode";

//              using (SQLiteDataReader drRoute = cmndRoute.ExecuteReader())
//              {
//                using (SqlCeResultSet rs = cmndRoute.ExecuteResultSet(ResultSetOptions.Updatable))
//                {
//                  SqlCeUpdatableRecord rec = rs.CreateRecord();
//                  int progValue = 0;
//                  bool isRouteFound = false;
//                  string routeDesc = string.Empty;
//                  string routeDayOfWeek = string.Empty;
//                  string routeCode = string.Empty;

//                  this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

//                  foreach (DataRow drow in ds.Tables["tRoute"].Rows)
//                  {
//                    routeCode = Convert.ToString(drow["RouteCode"]);
//                    errMsg = "Route code \'" + routeCode + "\'";
//                    routeDayOfWeek = Convert.ToString(drow["RouteDayOfWeek"]);
//                    errMsg += Environment.NewLine + "Route dow \'" + routeDayOfWeek + "\'";
//                    routeDesc = Convert.ToString(drow["RouteDescription"]);
//                    errMsg += Environment.NewLine + "Route desc \'" + routeDesc + "\'";

//                    isRouteFound = false;
//                    if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
//                    {
//                      cmndRoute.CommandText = "tRoute";
//                      cmndRoute.CommandType = CommandType.TableDirect;
//                      cmndRoute.IndexName = "UQ_RouteOnRouteCode";

//                      if (drRoute.Seek(DbSeekOptions.FirstEqual, new object[] { routeCode }))
//                      {
//                        isRouteFound = true;

//                        cmndRoute.CommandType = CommandType.Text;
//                        cmndRoute.IndexName = string.Empty;
//                        cmndRoute.CommandText = "UPDATE tRoute "
//                            + "SET RouteDescription = \'" + routeDesc + "\', " 
//                            + "RouteDayOfWeek = \'" + routeDayOfWeek + "\' " 
//                            + "WHERE RouteCode = \'" + routeCode + "\'";
//                        cmndRoute.ExecuteNonQuery();
//                      }
//                    }

//                    if (!isRouteFound)
//                    {
//                      cmndRoute.CommandText = "tRoute";
//                      cmndRoute.CommandType = CommandType.TableDirect;
//                      cmndRoute.IndexName = "UQ_RouteOnRouteCode";
                      
//                      rec.SetString((int)SysCommon.RouteFields.Code, routeCode);
//                      rec.SetString((int)SysCommon.RouteFields.Description, routeDesc);
//                      rec.SetString((int)SysCommon.RouteFields.DayOfWeek, routeDayOfWeek);

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

//                drRoute.Close();
//              }
//            }
//          }

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingRoutesComplete), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving routes complete.", false);
//#endif
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingRoutesFailed), false);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving routes failed.", false);
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

//#if MULTILANGUAGE
//        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingRoutesFailed) + Environment.NewLine + errMsg, ex);
//#else
//        throw new Exception("Database exception occurred. Saving routes failed." + Environment.NewLine + errMsg, ex);
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
          + "<action>REQUEST ROUTES</action>"
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
