namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using Microsoft.VisualBasic;

  public class TableActivityPicking : TableBase
  {
    #region Constructors

    public TableActivityPicking()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityPicking";
    }

    #endregion

    #region Public Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="criteria"></param>
    public override void RequestData(XDocument criteria)
    {
      string reqData = string.Empty;

      //GC.Collect();
      //GC.WaitForPendingFinalizers();

      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesOrders), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales orders.", true);
#endif

        reqData = this.SendRequest(criteria);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesOrdersComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales orders complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesOrdersFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales orders failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSalesOrdersFailed), ex);
#else
        throw new Exception("Seeding sales orders failed.", ex);
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
          cmndStatus.CommandType = CommandType.Text;
          cmndStatus.CommandText = "SELECT COUNT(tActivity.ActivityRefno) "
              + "FROM tActivity "
              + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.Picking;
          
          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());
          
          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.SalesOrderszz), rowsAffected),
#else
              string.Format("{0} sales orders.", rowsAffected),
#endif
              true);
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSalesOrdersStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Sales orders status failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesOrders), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales orders.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityPickingRoute "
              + "WHERE tActivityPickingRoute.PickingActivityRefno IN "
              + "(SELECT tActivity.ActivityRefno FROM tActivity "
              + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.Picking + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityPicking " 
              + "WHERE tActivityPicking.ActivitySubType = " + (int)SysCommon.ActSubTypes.Picking;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivity " 
              + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.Picking;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesOrdersComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales orders complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesOrdersFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales orders failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingSalesOrdersFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging sales orders failed.", ex);
#endif
      }
    }

    #endregion

    #region Private Members

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string SendRequest(XDocument criteria)
    {
      return this.SendComm(this.CreateRequest(criteria));
    }

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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSalesOrders), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales orders.", false);
//#endif
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tActivityPicking"].Rows.Count);

//            this.DataAccessStatusOutHandler(
//                this, 
//                EventArgs.Empty, 
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), 
//#else
//                string.Format("Records {0}", progMax), 
//#endif
//                false);

//            using (SQLiteCommand cmndPick = conn.CreateCommand())
//            {
//              cmndPick.CommandText = "tActivityPicking";
//              cmndPick.CommandType = CommandType.TableDirect;
//              cmndPick.IndexName = "UQ_ActivityPickingOnActivityPickingFKRefno";

//              using (SQLiteCommand cmndAct = conn.CreateCommand())
//              {
//                cmndAct.CommandText = "tActivity";
//                cmndAct.CommandType = CommandType.TableDirect;

//                using (SqlCeResultSet rsAct = cmndAct.ExecuteResultSet(ResultSetOptions.Updatable))
//                {
//                  SqlCeUpdatableRecord recAct = rsAct.CreateRecord();

//                  using (SQLiteCommand cmndID = conn.CreateCommand())
//                  {
//                    cmndID.CommandText = "SELECT @@IDENTITY";
//                    cmndID.CommandType = CommandType.Text;

//                    using (SQLiteCommand cmndWhs = conn.CreateCommand())
//                    {
//                      cmndWhs.CommandText = "tLocationMapWarehouse";
//                      cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
//                      cmndWhs.CommandType = CommandType.TableDirect;

//                      using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
//                      {
//                        using (SQLiteCommand cmndPickRoute = conn.CreateCommand())
//                        {
//                          cmndPickRoute.CommandType = CommandType.TableDirect;
//                          cmndPickRoute.CommandText = "tActivityPickingRoute";
//                          cmndPickRoute.IndexName = "UQ_ActivityPickingRouteOnPickingActivityRefnoAndRouteRefno";

//                          using (SQLiteCommand cmndRoute = conn.CreateCommand())
//                          {
//                            cmndRoute.CommandText = "tRoute";
//                            cmndRoute.CommandType = CommandType.TableDirect;
//                            cmndRoute.IndexName = "UQ_RouteOnRouteCode";

//                            using (SQLiteDataReader drRoute = cmndRoute.ExecuteReader())
//                            {
//                              using (SqlCeResultSet rsPickRoute = 
//                                  cmndPickRoute.ExecuteResultSet(ResultSetOptions.Updatable))
//                              {
//                                SqlCeUpdatableRecord recPickRoute = rsPickRoute.CreateRecord();

//                                using (SqlCeResultSet rsPick = cmndPick.ExecuteResultSet(ResultSetOptions.Updatable))
//                                {
//                                  SqlCeUpdatableRecord recPick = rsPick.CreateRecord();
//                                  int actRefno = 0;
//                                  int progValue = 0;
//                                  int whsLocMapRefno = 0;
//                                  int routeRefno = 0;
//                                  string pickRouteCode = string.Empty;
//                                  int actPickFKRefno = 0;

//                                  this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

//                                  foreach (DataRow drow in ds.Tables["tActivityPicking"].Rows)
//                                  {
//                                    errMsg = "Arguments:";

//                                    recAct.SetInt32(
//                                        (int)SysCommon.ActFields.SubType, 
//                                        (int)SysCommon.ActSubTypes.Picking);

//                                    recAct.SetInt32((int)SysCommon.ActFields.Status, 0);

//                                    rsAct.Insert(recAct);

//                                    actRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                                    recPick.SetInt32(
//                                        (int)SysCommon.ActPickingFields.ActRefno,
//                                        actRefno);

//                                    recPick.SetInt32(
//                                        (int)SysCommon.ActPickingFields.ActSubType,
//                                        (int)SysCommon.ActSubTypes.Picking);

//                                    actPickFKRefno = Convert.ToInt32(drow["ActivityPickingFKRefno"]);
//                                    errMsg += Environment.NewLine + "FK refno " + actPickFKRefno;
//                                    recPick.SetInt32(
//                                        (int)SysCommon.ActPickingFields.FKRefno,
//                                        actPickFKRefno);
//                                    recPick.SetString(
//                                        (int)SysCommon.ActPickingFields.StringFKRefno,
//                                        actPickFKRefno.ToString());

//                                    whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, drWhs);
//                                    recPick.SetInt32(
//                                        (int)SysCommon.ActPickingFields.WhsLocMapRefno, 
//                                        whsLocMapRefno);

//                                    recPick.SetDateTime(
//                                        (int)SysCommon.ActPickingFields.OrderedDateTime,
//                                        Convert.ToDateTime(drow["ActivityPickingOrderedDateTime"]));

//                                    recPick.SetDateTime(
//                                        (int)SysCommon.ActPickingFields.DeliveryDateTime,
//                                        Convert.ToDateTime(drow["ActivityPickingDeliveryDateTime"]));
                                    
//                                    recPick.SetString(
//                                        (int)SysCommon.ActPickingFields.CustomerContactName, 
//                                        drow["ActivityPickingCustomerContactName"].ToString());

//                                    recPick.SetString(
//                                        (int)SysCommon.ActPickingFields.CustomerShipToInfo,
//                                        drow["ActivityPickingCustomerShipToInfo"].ToString());

//                                    recPick.SetBoolean(
//                                        (int)SysCommon.ActPickingFields.IsReviewed,
//                                        false);
                                    
//                                    rsPick.Insert(recPick);

//                                    routeRefno = 0;
//                                    pickRouteCode = drow["ActivityPickingRouteCode"].ToString();
//                                    errMsg += Environment.NewLine + "Route code \'" + pickRouteCode + "\'";
//                                    if (!string.IsNullOrEmpty(pickRouteCode))
//                                    {
//                                      if (drRoute.Seek(DbSeekOptions.FirstEqual, new object[] { pickRouteCode }))
//                                      {
//                                        if (drRoute.Read())
//                                        {
//                                          routeRefno = drRoute.GetInt32((int)SysCommon.RouteFields.Refno);
//                                        }
//                                        else
//                                        {
//                                          throw new Exception("drRoute.Read() failed." + Environment.NewLine + errMsg);
//                                        }
//                                      }
//                                      else
//                                      {
//                                        throw new Exception(
//#if MULTILANGUAGE
//                                            Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheRouteCodeInTroute)
//#else
//                                            "Unable to locate the route code in tRoute."
//#endif
//                                            + Environment.NewLine + errMsg);
//                                      }
//                                    }

//                                    if (routeRefno != 0)
//                                    {
//                                      recPickRoute.SetInt32(
//                                          (int)SysCommon.ActPickingRouteFields.PickingActRefno,
//                                          actRefno);

//                                      recPickRoute.SetInt32(
//                                          (int)SysCommon.ActPickingRouteFields.RouteRefno,
//                                          routeRefno);

//                                      recPickRoute.SetInt32(
//                                          (int)SysCommon.ActPickingRouteFields.RouteStop,
//                                          Convert.ToInt32(drow["ActivityPickingRouteStop"]));

//                                      rsPickRoute.Insert(recPickRoute);
//                                    }

//                                    progValue++;
//                                    if (progValue % 10 == 0)
//                                    {
//                                      this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue);
//                                    }
//                                  }

//                                  this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax);
//                                }
//                              }

//                              drRoute.Close();
//                            }
//                          }
//                        }

//                        drWhs.Close();
//                      }
//                    }
//                  }
//                }
//              }
//            }
//          }

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSalesOrdersComplete), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales orders complete.", false);
//#endif
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSalesOrdersFailed), true);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales orders failed.", true);
//#endif

//       // Duplicate key violations
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
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingSalesOrdersFailed) + Environment.NewLine + errMsg,
//#else
//            "Database exception occurred. Saving sales orders failed." + Environment.NewLine + errMsg,
//#endif
//            ex);
//      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="critFilter"></param>
    /// <returns></returns>
    protected override string CreateRequest(XDocument critFilter)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);
      XDocument request = null;

      if (critFilter == null)
      {
        request = XDocument.Parse(
            "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
            + "<imclient>" 
            + "<command>" 
            + "<action>REQUEST SALES ORDERS</action>" 
            + SysCommon.GetRequestDefaults(false)
            + "<whs_id></whs_id>" 
            + "<sect_refno></sect_refno>" 
            + "</command>" 
            + "</imclient>");
      }
      else
      {
        request = XDocument.Parse(
            "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
            + "<imclient>"
            + "<command>"
            + "<action>REQUEST SALES ORDERS</action>"
            + SysCommon.GetRequestDefaults(false)
            + ("<whs_id>" + critFilter.Root.Element("whs_id").Value + "</whs_id>") 
            + ("<sect_refno>" + critFilter.Root.Element("sect_refno").Value + "</sect_refno>") 
            + "</command>"
            + "</imclient>");
      }

      request.Save(xml);
      xml.Flush();
      xml.Close();

      return xmlString.ToString();
    }

    #endregion
  }
}
