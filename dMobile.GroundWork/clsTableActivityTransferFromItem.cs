namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;

    public class TableActivityTransferFromItem : TableBase
    {
        #region Constructors

        public TableActivityTransferFromItem()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityTransferFromItem";
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Request(int actRefno)
        {
            string reqData = string.Empty;

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer from items.", true);

                if (actRefno == 0)
                {
                    using (SQLiteCommand cmndTrans = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndTrans.CommandType = CommandType.Text;
                        cmndTrans.CommandText = "SELECT tActivity.ActivityRefno"
                            + " FROM tActivity"
                            + " WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.TransferFrom;

                        using (SQLiteDataReader drTrans = cmndTrans.ExecuteReader())
                        {
                            while (drTrans.Read())
                            {
                                actRefno = Convert.ToInt32(drTrans["ActivityRefno"]);
                                reqData = this.Send(actRefno);

                                if (0 < reqData.Length)
                                {
                                    if (reqData.Contains("<message>ERROR</message>"))
                                    {
                                        XmlDocument reqErr = new XmlDocument();
                                        reqErr.LoadXml(reqData);
                                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                                    }

                                    this.Save(reqData, actRefno);
                                }
                            }

                            drTrans.Close();
                        }
                    }
                }
                else
                {
                    reqData = this.Send(actRefno);

                    if (0 < reqData.Length)
                    {
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            XmlDocument reqErr = new XmlDocument();
                            reqErr.LoadXml(reqData);
                            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                        }

                        this.Save(reqData, actRefno);
                    }
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer items from complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer items from failed.", true);

                throw new Exception("Database exception occurred. Seeding transfer items from failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer items from failed.", true);
                throw new Exception("Seeding transfer items from failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            // TODO: Resource strings
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer items from.", true);

            try
            {
                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef"
                        + " WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN"
                        + " (SELECT tActivityItem.ActivityItemRefno"
                        + " FROM tActivityItem"
                        + " WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TransferFrom + ")";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItemTransfer "
                        + "WHERE tActivityItemTransfer.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TransferFrom;
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItem "
                        + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TransferFrom;
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer items from complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer items from failed.", true);
                throw new Exception("Database exception occurred. Purging transfer items from failed.", ex);
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
                    cmndStatus.CommandText = "SELECT COUNT(tActivityItem.ActivityItemRefno)"
                        + " FROM tActivityItem"
                        + " WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TransferFrom;
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} transfer items from.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Transfer items from status failed.", ex);
            }

            return rowsAffected;
        }

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override string Send(int actRefno)
        {
            int transFKRefno = 0;

            using (SQLiteCommand cmndTrans = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndTrans.CommandText = "SELECT * FROM tActivityTransfer WHERE ActivityRefno = @activityRefno";
                cmndTrans.CommandType = CommandType.Text;
                //cmndTrans.IndexName = "PK_ActivityTransferOnActivityRefno";
                cmndTrans.Parameters.Add(new SQLiteParameter("@activityRefno", Convert.ToInt32(actRefno)));

                using (SQLiteDataReader drTrans = cmndTrans.ExecuteReader())
                {
                    //if (drTrans.Seek(DbSeekOptions.FirstEqual, new object[] { actRefno }))
                    //{
                    if (drTrans.Read())
                    {
                        transFKRefno = drTrans.GetInt32((int)SysCommon.ActTransferFields.FKRefno);
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception("drTrans.Read() failed. " + string.Format("Unable to locate the transfer from refno {0} in tActivityTransfer.", actRefno));
                    }
                    //}
                    //else
                    //{
                    //    // TODO: Resource strings
                    //    throw new Exception(string.Format("Unable to locate the transfer from refno {0} in tActivityTransfer.", actRefno));
                    //}
                }
            }

            return this.SendComm(this.Create(transFKRefno));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reqData"></param>
        /// <remarks></remarks>
        protected override void Save(string reqData, int actRefno)
        {
            string errMsg = string.Empty;
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;

            using (SQLiteCommand cmndTransItem = conn.CreateCommand())
            using (SQLiteCommand cmndActItem = conn.CreateCommand())
            using (SQLiteCommand cmndWhs = conn.CreateCommand())
            using (SQLiteCommand cmndID = conn.CreateCommand())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer items from.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityItemTransfer"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteResultSet rsActItem = cmndActItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SQLiteUpdatableRecord recActItem = rsActItem.CreateRecord();

                            //using (SQLiteResultSet rs = cmndTransItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityItem";
                            cmndID.CommandType = CommandType.Text;

                            //SQLiteUpdatableRecord rec = rs.CreateRecord();
                            int actItemRefno = 0;
                            int progValue = 0;
                            int prodUnitRefno = 0;
                            int locMapRefno = 0;
                            int locMapWhsFKRefno = 0;
                            int transItemFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityItemTransfer"].Rows)
                            {
                                errMsg = "Arguments:";

                                cmndActItem.CommandText = "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (@activityItemSubType, @activityItemStatus, @activityRefno)";
                                cmndActItem.CommandType = CommandType.Text;
                                cmndActItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom)));
                                cmndActItem.Parameters.Add(new SQLiteParameter("@activityItemStatus", Convert.ToInt32(0)));
                                cmndActItem.Parameters.Add(new SQLiteParameter("@activityRefno", Convert.ToInt32(actRefno)));

                                cmndActItem.ExecuteNonQuery();

                                //recActItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.TransferFrom);
                                //recActItem.SetInt32((int)SysCommon.ActItemFields.Status, 0);
                                //recActItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, actRefno);
                                //rsActItem.Insert(recActItem);

                                actItemRefno = Convert.ToInt32(cmndID.ExecuteScalar());
                                prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, ref conn, ref errMsg, false);
                                locMapRefno = 0;

                                if (locMapRefno == 0)
                                {
                                    locMapWhsFKRefno = Convert.ToInt32(drow["TransferFromLocationMapWarehouseFKRefno"]);
                                    errMsg += Environment.NewLine + "whs fk refno " + locMapWhsFKRefno;

                                    cmndWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                                    cmndWhs.CommandType = CommandType.Text;
                                    //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", Convert.ToInt32(locMapWhsFKRefno)));

                                    using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                                    {
                                        if (drWhs.Read())
                                        {
                                            locMapRefno = drWhs.GetInt32((int)SysCommon.LocMapWhsFields.LocMapRefno);
                                        }
                                    }
                                }

                                if (locMapRefno == 0)
                                {
                                    // TODO: Resource strings
                                    throw new Exception("Unable to indentify the location map refno." + Environment.NewLine + errMsg);
                                }

                                transItemFKRefno = Convert.ToInt32(drow["ActivityItemTransferFKRefno"]);
                                errMsg += Environment.NewLine + "Item fk refno " + transItemFKRefno;

                                cmndTransItem.CommandText = "INSERT INTO tActivityItemTransfer (" +
                                    "ActivityItemRefno, " +
                                    "ActivityItemSubType, " +
                                    "ActivityItemTransferFKRefno, " +
                                    "ActivityItemTransferStringFKRefno, " +
                                    "ProductUnitRefno, " +
                                    "TransferFromWarehouseLocationMapRefno, " +
                                    "ActivityItemTransferRequiredQuantity, " +
                                    "ActivityItemTransferQuantity, " +
                                    "ActivityItemTransferBaseQuantity, " +
                                    "ActivityItemTransferWeightQuantity" +
                                    ") VALUES (" +
                                    "@activityItemRefno, " +
                                    "@activityItemSubType, " +
                                    "@activityItemTransferFKRefno, " +
                                    "@activityItemTransferStringFKRefno, " +
                                    "@productUnitRefno, " +
                                    "@transferFromWarehouseLocationMapRefno, " +
                                    "@activityItemTransferRequiredQuantity, " +
                                    "@activityItemTransferQuantity, " +
                                    "@activityItemTransferBaseQuantity, " +
                                    "@activityItemTransferWeightQuantity" +
                                    ")";
                                cmndTransItem.CommandType = CommandType.Text;
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(actItemRefno)));
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom)));
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@activityItemTransferFKRefno", Convert.ToInt32(drow["ActivityItemTransferFKRefno"])));
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@activityItemTransferStringFKRefno", Convert.ToString(transItemFKRefno)));
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@productUnitRefno", Convert.ToInt32(prodUnitRefno)));
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@transferFromWarehouseLocationMapRefno", Convert.ToInt32(locMapRefno)));
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@activityItemTransferRequiredQuantity", Convert.ToDecimal(drow["ActivityItemTransferRequiredQuantity"])));
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@activityItemTransferQuantity", Convert.ToDecimal(drow["ActivityItemTransferQuantity"])));
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@activityItemTransferBaseQuantity", Convert.ToDecimal(drow["ActivityItemTransferBaseQuantity"])));
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@activityItemTransferWeightQuantity", Convert.ToDecimal(drow["ActivityItemTransferWeightQuantity"])));

                                cmndTransItem.ExecuteNonQuery();

                                //rec.SetInt32((int)SysCommon.ActItemTransferFields.ActItemRefno, actItemRefno);
                                //rec.SetInt32((int)SysCommon.ActItemTransferFields.ActItemSubType, (int)SysCommon.ActSubTypes.TransferFrom);
                                //rec.SetInt32((int)SysCommon.ActItemTransferFields.FKRefno, Convert.ToInt32(drow["ActivityItemTransferFKRefno"]));
                                //rec.SetString((int)SysCommon.ActItemTransferFields.StringFKRefno, Convert.ToString(transItemFKRefno));
                                //rec.SetInt32((int)SysCommon.ActItemTransferFields.ProductUnitRefno, prodUnitRefno);
                                //rec.SetInt32((int)SysCommon.ActItemTransferFields.TransferFromWhsLocMapRefno, locMapRefno);
                                //rec.SetDecimal((int)SysCommon.ActItemTransferFields.RequiredQuantity, Convert.ToDecimal(drow["ActivityItemTransferRequiredQuantity"]));
                                //rec.SetDecimal((int)SysCommon.ActItemTransferFields.Quantity, Convert.ToDecimal(drow["ActivityItemTransferQuantity"]));
                                //rec.SetDecimal((int)SysCommon.ActItemTransferFields.BaseQuantity, Convert.ToDecimal(drow["ActivityItemTransferBaseQuantity"]));
                                //rec.SetDecimal((int)SysCommon.ActItemTransferFields.WeightQuantity, Convert.ToDecimal(drow["ActivityItemTransferWeightQuantity"]));
                                //rs.Insert(rec);

                                progValue++;

                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            transaction.Commit();

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                    }

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer items from complete.", false);
                    //}
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer items found failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving transfer items from failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transFKRefno"></param>
        /// <returns></returns>
        protected override string Create(int transFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST TRANSFER FROM ITEMS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<refno>" + transFKRefno.ToString() + "</refno>")
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