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

  public class TableActivityReturns : TableBase
  {
    #region Constructors

    public TableActivityReturns()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityReturns";
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

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales returns.", true);

                reqData = this.Send("activity returns");

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

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales returns complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales returns failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding sales returns failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            // TODO: Resource strings
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales returns.", true);

            try
            {
                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Returns)));

                    cmndPurge.CommandText = "DELETE FROM tActivityReturns WHERE tActivityReturns.ActivitySubType = @activitySubType";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales returns failed.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales returns failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging sales returns failed.", ex);
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
                    cmndStatus.CommandText = "SELECT COUNT(tActivity.ActivityRefno) FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                    cmndStatus.CommandType = CommandType.Text;
                    cmndStatus.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Returns)));

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} sales returns.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Sales returns status failed.", ex);
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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSalesReturns), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales returns.", false);
//#endif
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tActivityReturns"].Rows.Count);

//            this.DataAccessStatusOutHandler(
//                this, 
//                EventArgs.Empty, 
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), 
//#else
//                string.Format("Records {0}", progMax), 
//#endif
//                false);

//            using (SQLiteCommand cmndReturns = conn.CreateCommand())
//            {
//              cmndReturns.CommandText = "tActivityReturns";
//              cmndReturns.CommandType = CommandType.TableDirect;
//              cmndReturns.IndexName = "UQ_ActivityReturnsOnActivityReturnsFKRefno";

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
//                        using (SqlCeResultSet rsReturns = cmndReturns.ExecuteResultSet(ResultSetOptions.Updatable))
//                        {
//                          SqlCeUpdatableRecord recActReturns = rsReturns.CreateRecord();
//                          int actRefno = 0;
//                          int progValue = 0;
//                          int whsLocMapRefno = 0;
//                          int returnsFKRefno = 0;

//                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

//                          foreach (DataRow drow in ds.Tables["tActivityReturns"].Rows)
//                          {
//                            errMsg = "Arguments:";

//                            recAct.SetInt32(
//                                (int)SysCommon.ActFields.SubType,
//                                (int)SysCommon.ActSubTypes.Returns);
//                            recAct.SetInt32((int)SysCommon.ActFields.Status, 0);

//                            rsAct.Insert(recAct);

//                            actRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                            recActReturns.SetInt32((int)SysCommon.ActReturnsFields.ActRefno, actRefno);
//                            recActReturns.SetInt32(
//                                (int)SysCommon.ActReturnsFields.ActSubType, 
//                                (int)SysCommon.ActSubTypes.Returns);

//                            returnsFKRefno = Convert.ToInt32(drow["ActivityReturnsFKRefno"]);
//                            errMsg += Environment.NewLine + "FK refno " + returnsFKRefno;
//                            recActReturns.SetInt32((int)SysCommon.ActReturnsFields.FKRefno, returnsFKRefno);
//                            recActReturns.SetString(
//                                (int)SysCommon.ActReturnsFields.StringFKRefno, 
//                                Convert.ToString(returnsFKRefno));

//                            whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, drWhs);
//                            recActReturns.SetInt32((int)SysCommon.ActReturnsFields.WhsLocMapRefno, whsLocMapRefno);

//                            recActReturns.SetDateTime(
//                                (int)SysCommon.ActReturnsFields.OrderedDateTime, 
//                                Convert.ToDateTime(drow["ActivityReturnsOrderedDateTime"]));
//                            recActReturns.SetDateTime(
//                                (int)SysCommon.ActReturnsFields.DeliveryDateTime, 
//                                Convert.ToDateTime(drow["ActivityReturnsDeliveryDateTime"]));
//                            recActReturns.SetString(
//                                (int)SysCommon.ActReturnsFields.CustomerContactName, 
//                                drow["ActivityReturnsCustomerContactName"].ToString());
//                            recActReturns.SetString(
//                                (int)SysCommon.ActReturnsFields.CustomerShipToInfo, 
//                                drow["ActivityReturnsCustomerShipToInfo"].ToString());

//                            rsReturns.Insert(recActReturns);

//                            progValue++;
//                            if (progValue % 10 == 0)
//                            {
//                              this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
//                            }
//                          }

//                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSalesReturnsComplete), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales returns complete.", false);
//#endif
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSalesReturnsFailed), true);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales returns failed.", true);
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
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingSalesReturnsFailed) + Environment.NewLine + errMsg,
//#else
//            "Database exception occurred. Saving sales returns failed." + Environment.NewLine + errMsg,
//#endif
//            ex);
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
          + "<action>REQUEST SALES RETURNS</action>" 
          + SysCommon.GetRequestDefaults(false)
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
