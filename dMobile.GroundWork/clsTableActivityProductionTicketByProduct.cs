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
    /// Defines the <see cref="TableActivityProductionTicketByProduct" />
    /// </summary>
    public class TableActivityProductionTicketByProduct : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketByProduct"/> class.
        /// </summary>
        public TableActivityProductionTicketByProduct()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketByProduct";
        }

        /// <summary>
        /// The Request
        /// </summary>
        public override void Request()
        {
            string reqData = string.Empty;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-products.", true);

                reqData = this.Send("production ticket add-product(s)");

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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-products complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-products failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding ticket add-products failed.", ex);
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
                    cmndStatus.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct)));

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} ticket add-products.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Ticket add-product status failed.", ex);
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
                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-products.", true);

                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct)));

                    cmndPurge.CommandText = "DELETE FROM tActivityProductionByProduct"
                        + " WHERE tActivityProductionByProduct.ActivityRefno IN"
                        + " (SELECT tActivityProduction.ActivityRefno FROM tActivityProduction WHERE tActivityProduction.ActivitySubType = @activitySubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityProduction WHERE tActivityProduction.ActivitySubType = @activitySubType";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                    cmndPurge.ExecuteNonQuery();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-products complete.", true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-products complete.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging ticket add-products complete.", ex);
            }
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        protected override void Save(string reqData)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            using (SQLiteCommand cmndProduction = conn.CreateCommand())
            using (SQLiteCommand cmndByProduct = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-products.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityProduction"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndProduction = conn.CreateCommand())
                            //{
                            //cmndProduction.CommandText = "tActivityProduction";
                            //cmndProduction.CommandType = CommandType.Text;
                            //cmndProduction.IndexName = "PK_ActivityProductionOnActivityRefno";

                            //using (SQLiteCommand cmndByProduct = conn.CreateCommand())
                            //{
                            //cmndByProduct.CommandText = "tActivityProductionByProduct";
                            //cmndByProduct.CommandType = CommandType.Text;
                            //cmndByProduct.IndexName = "UQ_ActivityProductionByProductOnActivityProductionFKRefno";

                            //using (SqlCeResultSet rsByProduct = cmndByProduct.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recByProduct = rsByProduct.CreateRecord();

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
                            //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                            //cmndWhs.CommandType = CommandType.Text;

                            //using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                            //{
                            //using (SqlCeResultSet rsProduction = cmndProduction.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recProduction = rsProduction.CreateRecord();

                            int actRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityProduction"].Rows)
                            {
                                errMsg = "Arguments:";

                                cmndAct.CommandText = "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (@activityStatus, @activitySubType)";
                                cmndAct.CommandType = CommandType.Text;
                                cmndAct.Parameters.Add(new SQLiteParameter("@activityStatus", Convert.ToInt32(0)));
                                cmndAct.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct)));

                                cmndAct.ExecuteNonQuery();

                                //recAct.SetInt32((int)SysCommon.ActFields.SubType, (int)SysCommon.ActSubTypes.TicketAddProduct);
                                //recAct.SetInt32((int)SysCommon.ActFields.Status, 0);

                                //rsAct.Insert(recAct);

                                cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivity";
                                cmndID.CommandType = CommandType.Text;

                                actRefno = Convert.ToInt32(cmndID.ExecuteScalar());
                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, cmndWhs);

                                // To remove time - convert twice
                                string productionDateTime = Convert.ToDateTime(drow["ActivityProductionDateTime"]).ToShortDateString();
                                errMsg += Environment.NewLine + "Date time " + productionDateTime;

                                string productionDesc = drow["ActivityProductionDescription"].ToString();
                                errMsg += Environment.NewLine + "Desc \'" + productionDesc + "\'";

                                cmndProduction.CommandText = "INSERT INTO tActivityProduction (" +
                                    "ActivityRefno, " +
                                    "ActivitySubType, " +
                                    "WarehouseLocationMapRefno, " +
                                    "ActivityProductionDateTime, " +
                                    "ActivityProductionDescription" +
                                    ") VALUES (" +
                                    "@activityRefno, " +
                                    "@activitySubType, " +
                                    "@warehouseLocationMapRefno, " +
                                    "@activityProductionDateTime, " +
                                    "@activityProductionDescription" +
                                    ")";
                                cmndProduction.CommandType = CommandType.Text;
                                //cmndProduction.IndexName = "PK_ActivityProductionOnActivityRefno";
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct)));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", whsLocMapRefno));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activityProductionDateTime", DateTime.Parse(productionDateTime)));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activityProductionDescription", productionDesc));

                                cmndProduction.ExecuteNonQuery();

                                //recProduction.SetInt32((int)SysCommon.ActProdFields.ActRefno, actRefno);
                                //recProduction.SetInt32((int)SysCommon.ActProdFields.ActSubType, (int)SysCommon.ActSubTypes.TicketAddProduct);
                                //recProduction.SetInt32((int)SysCommon.ActProdFields.WhsLocMapRefno, whsLocMapRefno);
                                //recProduction.SetDateTime((int)SysCommon.ActProdFields.DateTime, DateTime.Parse(productionDateTime));
                                //recProduction.SetString((int)SysCommon.ActProdFields.Description, productionDesc);

                                //rsProduction.Insert(recProduction);

                                int productionFKRefno = Convert.ToInt32(drow["ActivityProductionFKRefno"]);
                                errMsg += Environment.NewLine + "FK refno " + productionFKRefno;

                                cmndByProduct.CommandText = "INSERT INTO tActivityProductionByProduct (" +
                                    "ActivityRefno, " +
                                    "ActivityProductionFKRefno, " +
                                    "ActivityProductionStringFKRefno" +
                                    ") VALUES (" +
                                    "@activityRefno, " +
                                    "@activityProductionFKRefno, " +
                                    "@activityProductionStringFKRefno" +
                                    ")";
                                cmndByProduct.CommandType = CommandType.Text;
                                //cmndByProduct.IndexName = "UQ_ActivityProductionByProductOnActivityProductionFKRefno";
                                cmndByProduct.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));
                                cmndByProduct.Parameters.Add(new SQLiteParameter("@activityProductionFKRefno", productionFKRefno));
                                cmndByProduct.Parameters.Add(new SQLiteParameter("@activityProductionStringFKRefno", productionFKRefno.ToString()));

                                cmndByProduct.ExecuteNonQuery();

                                //recByProduct.SetInt32((int)SysCommon.ActProdByProductFields.ActRefno, actRefno);
                                //recByProduct.SetInt32((int)SysCommon.ActProdByProductFields.FKRefno, productionFKRefno);
                                //recByProduct.SetString((int)SysCommon.ActProdByProductFields.StringFKRefno, productionFKRefno.ToString());
                                //rsByProduct.Insert(recByProduct);

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        //drWhs.Close();
                        //                                }
                        //                            }
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        transaction.Commit();

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-products complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-products failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving ticket add-products failed." + Environment.NewLine + errMsg, ex);
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
                + "<action>REQUEST PRODUCTION TICKET BYPRODUCT</action>"
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
