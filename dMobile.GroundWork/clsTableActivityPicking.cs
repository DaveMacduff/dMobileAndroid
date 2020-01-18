namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Defines the <see cref="TableActivityPicking" />
    /// </summary>
    public class TableActivityPicking : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityPicking"/> class.
        /// </summary>
        public TableActivityPicking()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityPicking";
        }

        /// <summary>
        /// The RequestData
        /// </summary>
        /// <param name="criteria"></param>
        public override void RequestData(XDocument criteria)
        {
            string reqData = string.Empty;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales orders.", true);

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

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales orders complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales orders failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding sales orders failed.", ex);
            }
        }

        /// <summary>
        /// The DataStatus
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public override int DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                using (SQLiteCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndStatus.CommandText = "SELECT COUNT(tActivity.ActivityRefno) FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                    cmndStatus.CommandType = CommandType.Text;
                    cmndStatus.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} sales orders.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Sales orders status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override void Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales orders.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tActivityPickingRoute WHERE tActivityPickingRoute.PickingActivityRefno IN "
                        + "(SELECT tActivity.ActivityRefno FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType)";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityPicking WHERE tActivityPicking.ActivitySubType = @activitySubType";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales orders complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales orders failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging sales orders failed.", ex);
            }
        }

        /// <summary>
        /// The SendRequest
        /// </summary>
        /// <param name="criteria">The criteria<see cref="XDocument"/></param>
        /// <returns></returns>
        protected override string SendRequest(XDocument criteria)
        {
            return this.SendComm(this.CreateRequest(criteria));
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        protected override void Save(string reqData)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            using (SQLiteCommand cmndPick = conn.CreateCommand())
            using (SQLiteCommand cmndAct = conn.CreateCommand())
            using (SQLiteCommand cmndID = conn.CreateCommand())
            using (SQLiteCommand cmndWhs = conn.CreateCommand())
            using (SQLiteCommand cmndPickRoute = conn.CreateCommand())
            using (SQLiteCommand cmndRoute = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales orders.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityPicking"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndPick = conn.CreateCommand())
                            //{
                            //cmndPick.CommandText = "tActivityPicking";
                            //cmndPick.CommandType = CommandType.Text;
                            //cmndPick.IndexName = "UQ_ActivityPickingOnActivityPickingFKRefno";

                            //using (SQLiteCommand cmndAct = conn.CreateCommand())
                            //{
                            //cmndAct.CommandText = "tActivity";
                            //cmndAct.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rsAct = cmndAct.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recAct = rsAct.CreateRecord();

                            //using (SQLiteCommand cmndID = conn.CreateCommand())
                            //{
                            cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivity";
                            cmndID.CommandType = CommandType.Text;

                            //using (SQLiteCommand cmndWhs = conn.CreateCommand())
                            //{
                            //cmndWhs.CommandText = "tLocationMapWarehouse";
                            //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                            //cmndWhs.CommandType = CommandType.Text;

                            //using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndPickRoute = conn.CreateCommand())
                            //{
                            //cmndPickRoute.CommandType = CommandType.Text;
                            //cmndPickRoute.CommandText = "tActivityPickingRoute";
                            //cmndPickRoute.IndexName = "UQ_ActivityPickingRouteOnPickingActivityRefnoAndRouteRefno";

                            //using (SQLiteCommand cmndRoute = conn.CreateCommand())
                            //{
                            //cmndRoute.CommandText = "tRoute";
                            //cmndRoute.CommandType = CommandType.Text;
                            //cmndRoute.IndexName = "UQ_RouteOnRouteCode";

                            //using (SQLiteDataReader drRoute = cmndRoute.ExecuteReader())
                            //{
                            //using (SqlCeResultSet rsPickRoute = cmndPickRoute.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recPickRoute = rsPickRoute.CreateRecord();

                            //using (SqlCeResultSet rsPick = cmndPick.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recPick = rsPick.CreateRecord();
                            int actRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;
                            int routeRefno = 0;
                            string pickRouteCode = string.Empty;
                            int actPickFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityPicking"].Rows)
                            {
                                errMsg = "Arguments:";

                                cmndAct.CommandText = "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (@activityStatus, @activitySubType)";
                                cmndAct.CommandType = CommandType.Text;
                                cmndAct.Parameters.Add(new SQLiteParameter("@activityStatus", Convert.ToInt32(0)));
                                cmndAct.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));

                                cmndAct.ExecuteNonQuery();

                                //recAct.SetInt32((int)SysCommon.ActFields.Status, 0);
                                //recAct.SetInt32((int)SysCommon.ActFields.SubType, (int)SysCommon.ActSubTypes.Picking);
                                //rsAct.Insert(recAct);

                                actRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                actPickFKRefno = Convert.ToInt32(drow["ActivityPickingFKRefno"]);
                                errMsg += Environment.NewLine + "FK refno " + actPickFKRefno;

                                cmndWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                                cmndWhs.CommandType = CommandType.Text;
                                //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                                cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", Convert.ToInt32(drow["LocationMapWarehouseFKRefno"])));

                                using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                                {
                                    whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, drWhs);
                                }

                                cmndPick.CommandText = "INSERT INTO tActivityPicking (" +
                                    "ActivityRefno, " +
                                    "ActivitySubType, " +
                                    "ActivityPickingFKRefno, " +
                                    "ActivityPickingStringFKRefno, " +
                                    "WarehouseLocationMapRefno, " +
                                    "ActivityPickingOrderedDateTime, " +
                                    "ActivityPickingDeliveryDateTime, " +
                                    "ActivityPickingCustomerContactName, " +
                                    "ActivityPickingCustomerShipToInfo, " +
                                    "ActivityPickingIsReviewed" +
                                    ") VALUES (" +
                                    "@activityRefno, " +
                                    "@activitySubType, " +
                                    "@activityPickingFKRefno, " +
                                    "@activityPickingStringFKRefno, " +
                                    "@warehouseLocationMapRefno, " +
                                    "@activityPickingOrderedDateTime, " +
                                    "@activityPickingDeliveryDateTime, " +
                                    "@activityPickingCustomerContactName, " +
                                    "@activityPickingCustomerShipToInfo, " +
                                    "@activityPickingIsReviewed" +
                                    ")";
                                cmndPick.CommandType = CommandType.Text;
                                //cmndPick.IndexName = "UQ_ActivityPickingOnActivityPickingFKRefno";
                                cmndPick.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));
                                cmndPick.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));
                                cmndPick.Parameters.Add(new SQLiteParameter("@activityPickingFKRefno", actPickFKRefno));
                                cmndPick.Parameters.Add(new SQLiteParameter("@activityPickingStringFKRefno", actPickFKRefno.ToString()));
                                cmndPick.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", whsLocMapRefno));
                                cmndPick.Parameters.Add(new SQLiteParameter("@activityPickingOrderedDateTime", Convert.ToDateTime(drow["ActivityPickingOrderedDateTime"])));
                                cmndPick.Parameters.Add(new SQLiteParameter("@activityPickingDeliveryDateTime", Convert.ToDateTime(drow["ActivityPickingDeliveryDateTime"])));
                                cmndPick.Parameters.Add(new SQLiteParameter("@activityPickingCustomerContactName", drow["ActivityPickingCustomerContactName"].ToString()));
                                cmndPick.Parameters.Add(new SQLiteParameter("@activityPickingCustomerShipToInfo", drow["ActivityPickingCustomerShipToInfo"].ToString()));
                                cmndPick.Parameters.Add(new SQLiteParameter("@activityPickingIsReviewed", false));

                                cmndPick.ExecuteNonQuery();

                                //recPick.SetInt32((int)SysCommon.ActPickingFields.ActRefno, actRefno);
                                //recPick.SetInt32((int)SysCommon.ActPickingFields.ActSubType, (int)SysCommon.ActSubTypes.Picking);
                                //recPick.SetInt32((int)SysCommon.ActPickingFields.FKRefno, actPickFKRefno);
                                //recPick.SetString((int)SysCommon.ActPickingFields.StringFKRefno, actPickFKRefno.ToString());
                                //recPick.SetInt32((int)SysCommon.ActPickingFields.WhsLocMapRefno, whsLocMapRefno);
                                //recPick.SetDateTime((int)SysCommon.ActPickingFields.OrderedDateTime, Convert.ToDateTime(drow["ActivityPickingOrderedDateTime"]));
                                //recPick.SetDateTime((int)SysCommon.ActPickingFields.DeliveryDateTime, Convert.ToDateTime(drow["ActivityPickingDeliveryDateTime"]));
                                //recPick.SetString((int)SysCommon.ActPickingFields.CustomerContactName, drow["ActivityPickingCustomerContactName"].ToString());
                                //recPick.SetString((int)SysCommon.ActPickingFields.CustomerShipToInfo, drow["ActivityPickingCustomerShipToInfo"].ToString());
                                //recPick.SetBoolean((int)SysCommon.ActPickingFields.IsReviewed, false);
                                //rsPick.Insert(recPick);

                                routeRefno = 0;
                                pickRouteCode = drow["ActivityPickingRouteCode"].ToString();
                                errMsg += Environment.NewLine + "Route code \'" + pickRouteCode + "\'";

                                if (!string.IsNullOrEmpty(pickRouteCode))
                                {
                                    cmndRoute.CommandText = "SELECT * FROM tRoute WHERE RouteCode = @routeCode";
                                    cmndRoute.CommandType = CommandType.Text;
                                    //cmndRoute.IndexName = "UQ_RouteOnRouteCode";
                                    cmndRoute.Parameters.Add(new SQLiteParameter("@routeCode", pickRouteCode));

                                    using (SQLiteDataReader drRoute = cmndRoute.ExecuteReader())
                                    {
                                        //if (drRoute.Seek(DbSeekOptions.FirstEqual, new object[] { pickRouteCode }))
                                        //{
                                        if (drRoute.Read())
                                        {
                                            routeRefno = drRoute.GetInt32((int)SysCommon.RouteFields.Refno);
                                        }
                                        else
                                        {
                                            // TODO: Resource strings
                                            throw new Exception("drRoute.Read() failed." + Environment.NewLine + errMsg);
                                        }
                                        //}
                                        //else
                                        //{
                                        //    // TODO: Resource strings
                                        //    throw new Exception("Unable to locate the route code in tRoute." + Environment.NewLine + errMsg);
                                        //}
                                    }
                                }

                                if (routeRefno != 0)
                                {
                                    cmndPickRoute.CommandText = "INSERT INTO tActivityPickingRoute (PickingActivityRefno, RouteRefno, RouteStop) VALUES (@pickingActivityRefno, @routeRefno, @routeStop)";
                                    cmndPickRoute.CommandType = CommandType.Text;
                                    //cmndPickRoute.IndexName = "UQ_ActivityPickingRouteOnPickingActivityRefnoAndRouteRefno";
                                    cmndPickRoute.Parameters.Add(new SQLiteParameter("@pickingActivityRefno", actRefno));
                                    cmndPickRoute.Parameters.Add(new SQLiteParameter("@routeRefno", routeRefno));
                                    cmndPickRoute.Parameters.Add(new SQLiteParameter("@routeStop", Convert.ToInt32(drow["ActivityPickingRouteStop"])));

                                    cmndPickRoute.ExecuteNonQuery();

                                    //recPickRoute.SetInt32((int)SysCommon.ActPickingRouteFields.PickingActRefno, actRefno);
                                    //recPickRoute.SetInt32((int)SysCommon.ActPickingRouteFields.RouteRefno, routeRefno);
                                    //recPickRoute.SetInt32((int)SysCommon.ActPickingRouteFields.RouteStop, Convert.ToInt32(drow["ActivityPickingRouteStop"]));
                                    //rsPickRoute.Insert(recPickRoute);
                                }

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax);
                        }
                        //}

                        //                                        drRoute.Close();
                        //                                    }
                        //                                }
                        //                            }

                        //                            drWhs.Close();
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        transaction.Commit();

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales orders complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales orders failed.", true);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving sales orders failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        /// The CreateRequest
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
    }
}
