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
    /// Defines the <see cref="TableActivityReceiving" />
    /// </summary>
    public class TableActivityReceiving : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityReceiving"/> class.
        /// </summary>
        public TableActivityReceiving()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReceiving";
        }

        /// <summary>
        /// The Request
        /// </summary>
        public override void Request()
        {
            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receivings.", true);

                string reqData = this.Send("receiving");

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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receivings complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receivings failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding receivings failed.", ex);
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
                    cmndStatus.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Receiving)));

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} receivings.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Receivings status failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receivings.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Receiving)));

                    cmndPurge.CommandText = "DELETE FROM tActivityReceiving";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receivings completed.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receivings failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging receivings failed.", ex);
            }
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        protected override void Save(string reqData)
        {
            string errMsg = string.Empty;
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;

            using (SQLiteCommand cmndRecv = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receivings.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityPurchaseOrder"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndRecv = conn.CreateCommand())
                            //{
                            //cmndRecv.CommandText = "tActivityReceiving";
                            //cmndRecv.CommandType = CommandType.Text;
                            //cmndRecv.IndexName = "UQ_ActivityReceivingOnActivityReceivingFKRefno";

                            //using (SQLiteCommand cmndAct = conn.CreateCommand())
                            //{
                            //cmndAct.CommandText = "tActivity";
                            //cmndAct.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rsAct = cmndAct.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recAct = rsAct.CreateRecord();

                            //using (SQLiteCommand cmndID = conn.CreateCommand())
                            //{
                            //cmndID.CommandText = "SELECT last_insert_rowid()";
                            //cmndID.CommandType = CommandType.Text;

                            //using (SQLiteCommand cmndWhs = conn.CreateCommand())
                            //{
                            //cmndWhs.CommandText = "tLocationMapWarehouse";
                            //cmndWhs.CommandType = CommandType.Text;
                            //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

                            //using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                            //{
                            //using (SqlCeResultSet rs = cmndRecv.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord rec = rs.CreateRecord();
                            int actRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;
                            int recvFKRefno = 0;
                            string recvPONumber = string.Empty;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityPurchaseOrder"].Rows)
                            {
                                errMsg = "Arguments:";

                                cmndAct.CommandText = "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (@activityStatus, @activitySubType)";
                                cmndAct.CommandType = CommandType.Text;
                                cmndAct.Parameters.Add(new SQLiteParameter("@activityStatus", Convert.ToInt32(0)));
                                cmndAct.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Receiving)));

                                cmndAct.ExecuteNonQuery();

                                //recAct.SetInt32((int)SysCommon.ActFields.Status, 0);
                                //recAct.SetInt32((int)SysCommon.ActFields.SubType, (int)SysCommon.ActSubTypes.Receiving);
                                //rsAct.Insert(recAct);

                                cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivity";
                                cmndID.CommandType = CommandType.Text;

                                actRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                recvFKRefno = Convert.ToInt32(drow["ActivityPurchaseOrderFKRefno"]);
                                errMsg += Environment.NewLine + "Recv fk refno " + recvFKRefno;

                                recvPONumber = drow["ActivityPurchaseOrderPONumber"].ToString();
                                errMsg += Environment.NewLine + "PO number " + recvPONumber;

                                cmndWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                                cmndWhs.CommandType = CommandType.Text;
                                //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                                cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", Convert.ToInt32(drow["LocationMapWarehouseFKRefno"])));

                                using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                                {
                                    whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, drWhs);
                                }

                                cmndRecv.CommandText = "INSERT INTO tActivityReceiving (" +
                                    "ActivityRefno, " +
                                    "ActivitySubType, " +
                                    "ActivityReceivingFKRefno, " +
                                    "ActivityReceivingStringFKRefno, " +
                                    "ActivityReceivingPONumber, " +
                                    "WarehouseLocationMapRefno, " +
                                    "ActivityReceivingIssuedDateTime, " +
                                    "ActivityReceivingArrivalDateTime, " +
                                    "ActivityReceivingVendorContactName, " +
                                    "ActivityReceivingVendorSupplierContactName, " +
                                    "ActivityReceivingVendorSupplierGTINCompanyPrefix" +
                                    ") VALUES (" +
                                    "@activityRefno, " +
                                    "@activitySubType, " +
                                    "@activityReceivingFKRefno, " +
                                    "@activityReceivingStringFKRefno, " +
                                    "@activityReceivingPONumber, " +
                                    "@warehouseLocationMapRefno, " +
                                    "@activityReceivingIssuedDateTime, " +
                                    "@activityReceivingArrivalDateTime, " +
                                    "@activityReceivingVendorContactName, " +
                                    "@activityReceivingVendorSupplierContactName, " +
                                    "@activityReceivingVendorSupplierGTINCompanyPrefix" +
                                    ")";
                                cmndRecv.CommandType = CommandType.Text;
                                //cmndRecv.IndexName = "UQ_ActivityReceivingOnActivityReceivingFKRefno";
                                cmndRecv.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));
                                cmndRecv.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Receiving)));
                                cmndRecv.Parameters.Add(new SQLiteParameter("@activityReceivingFKRefno", recvFKRefno));
                                cmndRecv.Parameters.Add(new SQLiteParameter("@activityReceivingStringFKRefno", Convert.ToString(recvFKRefno)));
                                cmndRecv.Parameters.Add(new SQLiteParameter("@activityReceivingPONumber", recvPONumber));
                                cmndRecv.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", whsLocMapRefno));
                                cmndRecv.Parameters.Add(new SQLiteParameter("@activityReceivingIssuedDateTime", Convert.ToDateTime(drow["ActivityPurchaseOrderIssuedDateTime"])));
                                cmndRecv.Parameters.Add(new SQLiteParameter("@activityReceivingArrivalDateTime", Convert.ToDateTime(drow["ActivityPurchaseOrderArrivalDateTime"])));
                                cmndRecv.Parameters.Add(new SQLiteParameter("@activityReceivingVendorContactName", drow["ActivityPurchaseOrderVendorContactName"].ToString()));
                                cmndRecv.Parameters.Add(new SQLiteParameter("@activityReceivingVendorSupplierContactName", drow["ActivityPurchaseOrderVendorSupplierContactName"].ToString()));
                                cmndRecv.Parameters.Add(new SQLiteParameter("@activityReceivingVendorSupplierGTINCompanyPrefix", drow["ActivityPurchaseOrderVendorSupplierGTINCompanyPrefix"].ToString()));

                                cmndRecv.ExecuteNonQuery();

                                //rec.SetInt32(Convert.ToInt32(SysCommon.ActReceivingFields.ActRefno), actRefno);
                                //rec.SetInt32(Convert.ToInt32(SysCommon.ActReceivingFields.ActSubType), Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                                //rec.SetInt32((int)SysCommon.ActReceivingFields.FKRefno, recvFKRefno);
                                //rec.SetString((int)SysCommon.ActReceivingFields.StringFKRefno, Convert.ToString(recvFKRefno));
                                //rec.SetInt32((int)SysCommon.ActReceivingFields.WhsLocMapRefno, whsLocMapRefno);
                                //rec.SetDateTime((int)SysCommon.ActReceivingFields.IssuedDateTime, Convert.ToDateTime(drow["ActivityPurchaseOrderIssuedDateTime"]));
                                //rec.SetDateTime((int)SysCommon.ActReceivingFields.ArrivalDateTime, Convert.ToDateTime(drow["ActivityPurchaseOrderArrivalDateTime"]));
                                //rec.SetString((int)SysCommon.ActReceivingFields.PONumber, recvPONumber);
                                //rec.SetString((int)SysCommon.ActReceivingFields.VendorContactName, drow["ActivityPurchaseOrderVendorContactName"].ToString());
                                //rec.SetString((int)SysCommon.ActReceivingFields.VendorSupplierContactName, drow["ActivityPurchaseOrderVendorSupplierContactName"].ToString());
                                //rec.SetString((int)SysCommon.ActReceivingFields.VendorSupplierGTINCompanyPrefix, drow["ActivityPurchaseOrderVendorSupplierGTINCompanyPrefix"].ToString());

                                //rs.Insert(rec);

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receivings complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receivings failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving receivings failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <returns></returns>
        protected override string Create()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                  "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                  + "<imclient>"
                  + "<command>"
                  + "<action>REQUEST PURCHASE ORDERS</action>"
                  + SysCommon.GetRequestDefaults(false)
                  + "</command>"
                  + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
