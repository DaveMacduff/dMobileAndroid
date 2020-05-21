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
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            using (SQLiteCommand cmndReturns = conn.CreateCommand())
            using (SQLiteCommand cmndAct = conn.CreateCommand())
            using (SQLiteCommand cmndID = conn.CreateCommand())
            using (SQLiteCommand cmndWhs = conn.CreateCommand())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales returns.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityReturns"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //{
                            //using (SqlCeResultSet rsAct = cmndAct.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recAct = rsAct.CreateRecord();

                            //{
                            //{   
                            //using (SqlCeResultSet rsReturns = cmndReturns.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recActReturns = rsReturns.CreateRecord();
                            int actRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;
                            int returnsFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityReturns"].Rows)
                            {
                                errMsg = "Arguments:";

                                cmndAct.CommandText = "INSERT INTO tActivity (ActivitySubType, ActivityStatus) VALUES (@activitySubType, @activityStatus)";
                                cmndAct.CommandType = CommandType.Text;
                                cmndAct.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Returns)));
                                cmndAct.Parameters.Add(new SQLiteParameter("@activityStatus", Convert.ToInt32(0)));

                                cmndAct.ExecuteNonQuery();

                                //recAct.SetInt32((int)SysCommon.ActFields.SubType, (int)SysCommon.ActSubTypes.Returns);
                                //recAct.SetInt32((int)SysCommon.ActFields.Status, 0);
                                //rsAct.Insert(recAct);

                                cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivity";
                                cmndID.CommandType = CommandType.Text;

                                actRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                returnsFKRefno = Convert.ToInt32(drow["ActivityReturnsFKRefno"]);
                                errMsg += Environment.NewLine + "FK refno " + returnsFKRefno;

                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, cmndWhs);

                                cmndReturns.CommandText = "INSERT INTO tActivityReturns (" +
                                    "ActivityRefno, " +
                                    "ActivitySubType, " +
                                    "ActivityReturnsFKRefno, " +
                                    "ActivityReturnsStringFKRefno, " +
                                    "WarehouseLocationMapRefno, " +
                                    "ActivityReturnsOrderedDateTime, " +
                                    "ActivityReturnsDeliveryDateTime, " +
                                    "ActivityReturnsCustomerContactName, " +
                                    "ActivityReturnsCustomerShipToInfo" +
                                    ") VALUES (" +
                                    "@activityRefno, " +
                                    "@activitySubType, " +
                                    "@activityReturnsFKRefno, " +
                                    "@activityReturnsStringFKRefno, " +
                                    "@warehouseLocationMapRefno, " +
                                    "@activityReturnsOrderedDateTime, " +
                                    "@activityReturnsDeliveryDateTime, " +
                                    "@activityReturnsCustomerContactName, " +
                                    "@activityReturnsCustomerShipToInfo" +
                                    ")";
                                cmndReturns.CommandType = CommandType.Text;
                                //cmndReturns.IndexName = "UQ_ActivityReturnsOnActivityReturnsFKRefno";
                                cmndReturns.Parameters.Add(new SQLiteParameter("@activityRefno", Convert.ToInt32(actRefno)));
                                cmndReturns.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Returns)));
                                cmndReturns.Parameters.Add(new SQLiteParameter("@activityReturnsFKRefno", Convert.ToInt32(returnsFKRefno)));
                                cmndReturns.Parameters.Add(new SQLiteParameter("@activityReturnsStringFKRefno", Convert.ToString(returnsFKRefno)));
                                cmndReturns.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", Convert.ToInt32(whsLocMapRefno)));
                                cmndReturns.Parameters.Add(new SQLiteParameter("@activityReturnsOrderedDateTime", Convert.ToDateTime(drow["ActivityReturnsOrderedDateTime"])));
                                cmndReturns.Parameters.Add(new SQLiteParameter("@activityReturnsDeliveryDateTime", Convert.ToDateTime(drow["ActivityReturnsDeliveryDateTime"])));
                                cmndReturns.Parameters.Add(new SQLiteParameter("@activityReturnsCustomerContactName", Convert.ToString(drow["ActivityReturnsCustomerContactName"])));
                                cmndReturns.Parameters.Add(new SQLiteParameter("@activityReturnsCustomerShipToInfo", Convert.ToString(drow["ActivityReturnsCustomerShipToInfo"])));

                                cmndReturns.ExecuteNonQuery();

                                //recActReturns.SetInt32((int)SysCommon.ActReturnsFields.ActRefno, actRefno);
                                //recActReturns.SetInt32((int)SysCommon.ActReturnsFields.ActSubType, (int)SysCommon.ActSubTypes.Returns);
                                //recActReturns.SetInt32((int)SysCommon.ActReturnsFields.FKRefno, returnsFKRefno);
                                //recActReturns.SetString((int)SysCommon.ActReturnsFields.StringFKRefno, Convert.ToString(returnsFKRefno));
                                //recActReturns.SetInt32((int)SysCommon.ActReturnsFields.WhsLocMapRefno, whsLocMapRefno);
                                //recActReturns.SetDateTime((int)SysCommon.ActReturnsFields.OrderedDateTime, Convert.ToDateTime(drow["ActivityReturnsOrderedDateTime"]));
                                //recActReturns.SetDateTime((int)SysCommon.ActReturnsFields.DeliveryDateTime, Convert.ToDateTime(drow["ActivityReturnsDeliveryDateTime"]));
                                //recActReturns.SetString((int)SysCommon.ActReturnsFields.CustomerContactName, drow["ActivityReturnsCustomerContactName"].ToString());
                                //recActReturns.SetString((int)SysCommon.ActReturnsFields.CustomerShipToInfo, drow["ActivityReturnsCustomerShipToInfo"].ToString());
                                //rsReturns.Insert(recActReturns);

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                    }
                    //}
                    //}
                    //}
                    //}
                    //}
                    //}

                    transaction.Commit();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales returns complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales returns failed.", true);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving sales returns failed." + Environment.NewLine + errMsg, ex);
                }
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
