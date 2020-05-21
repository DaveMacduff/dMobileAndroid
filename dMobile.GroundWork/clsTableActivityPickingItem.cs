namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Defines the <see cref="TableActivityPickingItem" />
    /// </summary>
    public class TableActivityPickingItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityPickingItem"/> class.
        /// </summary>
        public TableActivityPickingItem()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityPickingItem";
        }

        /// <summary>
        /// The RequestData
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/></param>
        /// <param name="xmlCriteria">The xmlCriteria<see cref="XDocument"/></param>
        public override void RequestData(int actRefno, XDocument xmlCriteria)
        {
            string reqData = string.Empty;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order items.", true);

                if (actRefno == 0)
                {
                    using (SQLiteCommand cmndPicking = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndPicking.CommandText = "SELECT tActivity.ActivityRefno FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                        cmndPicking.CommandType = CommandType.Text;
                        cmndPicking.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));

                        using (SQLiteDataReader drPicking = cmndPicking.ExecuteReader())
                        {
                            while (drPicking.Read())
                            {
                                actRefno = Convert.ToInt32(drPicking["ActivityRefno"]);
                                reqData = this.SendRequest(actRefno, xmlCriteria);

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
                        }
                    }
                }
                else
                {
                    reqData = this.SendRequest(actRefno, xmlCriteria);

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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding sales order items failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order items failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding sales order items failed.", ex);
            }
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override void Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order items.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));

                    cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef "
                        + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
                        + "(SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = @activityItemSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItemPicking WHERE tActivityItemPicking.ActivityItemSubType = @activityItemSubType";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = @activityItemSubType";
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order items completed.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging sales order items failed.", ex);
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
                    cmndStatus.CommandText = "SELECT COUNT(tActivityItem.ActivityItemRefno) FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = @activityItemSubType";
                    cmndStatus.CommandType = CommandType.Text;
                    cmndStatus.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} Sales order items.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Sales order items status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The SendRequest
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/></param>
        /// <param name="xmlCriteria">The xmlCriteria<see cref="XDocument"/></param>
        /// <returns></returns>
        protected override string SendRequest(int actRefno, XDocument xmlCriteria)
        {
            int actPickFKRefno = 0;

            using (SQLiteCommand cmndPicking = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndPicking.CommandText = "SELECT * FROM tActivityPicking WHERE ActivityRefno = @activityRefno";
                cmndPicking.CommandType = CommandType.Text;
                //cmndPicking.IndexName = "PK_ActivityPickingOnActivityRefno";
                cmndPicking.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));

                using (SQLiteDataReader drPicking = cmndPicking.ExecuteReader())
                {
                    //if (drPicking.Seek(DbSeekOptions.FirstEqual, new object[] { actRefno }))
                    //{
                    if (drPicking.Read())
                    {
                        actPickFKRefno = Convert.ToInt32(drPicking.GetValue((int)SysCommon.ActPickingFields.FKRefno));
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception("drPicking.Read() failed.");
                    }
                    //}
                    //else
                    //{
                    //    // TODO: Resource strings
                    //    throw new Exception(string.Format("Unable to locate the sales order refno {0} In tActivityPicking", actRefno));
                    //}
                }
            }

            return this.SendComm(this.CreateRequest(actPickFKRefno, xmlCriteria));
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actRefno">The actRefno<see cref="int"/></param>
        protected override void Save(string reqData, int actRefno)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            using (SQLiteCommand cmndPickItem = conn.CreateCommand())
            using (SQLiteCommand cmndPickItemComment = conn.CreateCommand())
            using (SQLiteCommand cmndItem = conn.CreateCommand())
            using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
            using (SQLiteCommand cmndProd = conn.CreateCommand())
            using (SQLiteCommand cmndUnit = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
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

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order items.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityItemPicking"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndPickItem = conn.CreateCommand())
                            //{
                            //cmndPickItem.CommandText = "tActivityItemPicking";
                            //cmndPickItem.CommandType = CommandType.Text;
                            //cmndPickItem.IndexName = "UQ_ActivityItemPickingOnActivityItemPickingFKRefno";

                            //using (SQLiteCommand cmndPickItemComment = conn.CreateCommand())
                            //{
                            //cmndPickItemComment.CommandText = "tActivityItemPickingComment";
                            //cmndPickItemComment.CommandType = CommandType.Text;
                            //cmndPickItemComment.IndexName = "PK_ActivityItemPickingCommentOnActivityItemRefno";

                            //using (SQLiteCommand cmndItem = conn.CreateCommand())
                            //{
                            //cmndItem.CommandText = "tActivityItem";
                            //cmndItem.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rsItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recItem = rsItem.CreateRecord();

                            //using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
                            //{
                            //    cmndProdUnit.CommandText = "tProductUnit";
                            //    cmndProdUnit.CommandType = CommandType.Text;
                            //    cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

                            //using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndProd = conn.CreateCommand())
                            //{
                            //cmndProd.CommandText = "tProduct";
                            //cmndProd.CommandType = CommandType.Text;
                            //cmndProd.IndexName = "UQ_ProductOnProductFKRefno";

                            //using (SQLiteDataReader drProd = cmndProd.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndUnit = conn.CreateCommand())
                            //{
                            //cmndUnit.CommandText = "tUnit";
                            //cmndUnit.CommandType = CommandType.Text;
                            //cmndUnit.IndexName = "UQ_UnitOnUnitCode";

                            //using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
                            //{
                            //cmndLocMapWhs.CommandText = "tLocationMapWarehouse";
                            //cmndLocMapWhs.CommandType = CommandType.Text;
                            //cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

                            //using (SQLiteDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndID = conn.CreateCommand())
                            //{
                            cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityItem";
                            cmndID.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rsPickItemComment = cmndPickItemComment.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recPickItemComment = rsPickItemComment.CreateRecord();

                            //using (SqlCeResultSet rsPickItem = cmndPickItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recPickItem = rsPickItem.CreateRecord();
                            int actItemRefno = 0;
                            int prodUnitRefno = 0;
                            int progValue = 0;
                            int locMapRefno = 0;
                            string actItemPickComment = string.Empty;
                            int actItemPickFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityItemPicking"].Rows)
                            {
                                errMsg = "Arguments:";

                                cmndItem.CommandText = "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (@activityItemSubType, @activityItemStatus, @activityRefno)";
                                cmndItem.CommandType = CommandType.Text;
                                cmndItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));
                                cmndItem.Parameters.Add(new SQLiteParameter("@activityItemStatus", Convert.ToInt32(0)));
                                cmndItem.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));

                                cmndItem.ExecuteNonQuery();

                                //recItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.Picking);
                                //recItem.SetInt32((int)SysCommon.ActItemFields.Status, 0);
                                //recItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, actRefno);
                                //rsItem.Insert(recItem);

                                actItemRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                actItemPickFKRefno = Convert.ToInt32(drow["ActivityItemPickingFKRefno"]);
                                errMsg += Environment.NewLine + "Item fk refno " + actItemPickFKRefno;

                                prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, ref conn, ref errMsg, false);
                                locMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, cmndLocMapWhs);

                                cmndPickItem.CommandText = "INSERT INTO tActivityItemPicking (" +
                                    "ActivityItemRefno, " +
                                    "ActivityItemSubType, " +
                                    "ActivityItemPickingFKRefno, " +
                                    "ActivityItemPickingStringFKRefno, " +
                                    "ProductUnitRefno, " +
                                    "WarehouseLocationMapRefno, " +
                                    "ActivityItemPickingQuantityOrdered, " +
                                    "ActivityItemPickingBaseQuantityOrdered, " +
                                    "ActivityItemPickingWeightQuantityOrdered, " +
                                    "ActivityItemPickingQuantityShipped, " +
                                    "ActivityItemPickingBaseQuantityShipped, " +
                                    "ActivityItemPickingWeightQuantityShipped, " +
                                    "ActivityItemPickingCustomerProductUnitCode, " +
                                    "ActivityItemPickingDescription, " +
                                    "ActivityItemPickingQuantityAvailable, " +
                                    "ActivityItemPickingBaseQuantityAvailable, " +
                                    "ActivityItemPickingWeightQuantityAvailable," +
                                    "ActivityItemPickingIsReviewed" +
                                    ") VALUES (" +
                                    "@activityItemRefno, " +
                                    "@activityItemSubType, " +
                                    "@activityItemPickingFKRefno, " +
                                    "@activityItemPickingStringFKRefno, " +
                                    "@productUnitRefno, " +
                                    "@warehouseLocationMapRefno, " +
                                    "@activityItemPickingQuantityOrdered, " +
                                    "@activityItemPickingBaseQuantityOrdered, " +
                                    "@activityItemPickingWeightQuantityOrdered, " +
                                    "@activityItemPickingQuantityShipped, " +
                                    "@activityItemPickingBaseQuantityShipped, " +
                                    "@activityItemPickingWeightQuantityShipped, " +
                                    "@activityItemPickingCustomerProductUnitCode, " +
                                    "@activityItemPickingDescription, " +
                                    "@activityItemPickingQuantityAvailable, " +
                                    "@activityItemPickingBaseQuantityAvailable, " +
                                    "@activityItemPickingWeightQuantityAvailable," +
                                    "@activityItemPickingIsReviewed" +
                                    ")";
                                cmndPickItem.CommandType = CommandType.Text;
                                //cmndPickItem.IndexName = "UQ_ActivityItemPickingOnActivityItemPickingFKRefno";
                                cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemRefno));
                                cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));
                                cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingFKRefno", actItemPickFKRefno));
                                cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingStringFKRefno", actItemPickFKRefno.ToString()));
                                cmndPickItem.Parameters.Add(new SQLiteParameter("@productUnitRefno", prodUnitRefno));
                                cmndPickItem.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", locMapRefno));
                                cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingCustomerProductUnitCode", drow["ActivityItemPickingCustomerProductUnitCode"].ToString()));
                                cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingDescription", drow["ActivityItemPickingDescription"].ToString()));
                                cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingQuantityOrdered", Convert.ToDecimal(drow["ActivityItemPickingQuantityOrdered"])));
                                cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingBaseQuantityOrdered", Convert.ToDecimal(drow["ActivityItemPickingBaseQuantityOrdered"])));
                                cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingWeightQuantityOrdered", Convert.ToDecimal(drow["ActivityItemPickingWeightQuantityOrdered"])));
                                cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingQuantityShipped", Convert.ToDecimal(drow["ActivityItemPickingQuantityPicked"])));
                                cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingBaseQuantityShipped", Convert.ToDecimal(drow["ActivityItemPickingBaseQuantityPicked"])));
                                cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingWeightQuantityShipped", Convert.ToDecimal(drow["ActivityItemPickingWeightQuantityPicked"])));
                                cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingQuantityAvailable", Convert.ToDecimal(drow["ActivityItemPickingQuantityAvailable"])));
                                cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingBaseQuantityAvailable", Convert.ToDecimal(drow["ActivityItemPickingBaseQuantityAvailable"])));
                                cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingWeightQuantityAvailable", Convert.ToDecimal(drow["ActivityItemPickingWeightQuantityAvailable"])));
                                cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingIsReviewed", false));

                                cmndPickItem.ExecuteNonQuery();

                                //recPickItem.SetInt32((int)SysCommon.ActItemPickingFields.ActItemRefno, actItemRefno);
                                //recPickItem.SetInt32((int)SysCommon.ActItemPickingFields.ActItemSubType, Convert.ToInt32(SysCommon.ActSubTypes.Picking));
                                //recPickItem.SetInt32((int)SysCommon.ActItemPickingFields.FKRefno, actItemPickFKRefno);
                                //recPickItem.SetString((int)SysCommon.ActItemPickingFields.StringFKRefno, actItemPickFKRefno.ToString());
                                //recPickItem.SetInt32((int)SysCommon.ActItemPickingFields.ProdUnitRefno, prodUnitRefno);
                                //recPickItem.SetInt32((int)SysCommon.ActItemPickingFields.WhsLocMapRefno, locMapRefno);
                                //recPickItem.SetString((int)SysCommon.ActItemPickingFields.CustomerProductUnitCode, drow["ActivityItemPickingCustomerProductUnitCode"].ToString());
                                //recPickItem.SetString((int)SysCommon.ActItemPickingFields.Description, drow["ActivityItemPickingDescription"].ToString());
                                //recPickItem.SetDecimal((int)SysCommon.ActItemPickingFields.QuantityOrdered, Convert.ToDecimal(drow["ActivityItemPickingQuantityOrdered"]));
                                //recPickItem.SetDecimal((int)SysCommon.ActItemPickingFields.BaseQuantityOrdered, Convert.ToDecimal(drow["ActivityItemPickingBaseQuantityOrdered"]));
                                //recPickItem.SetDecimal((int)SysCommon.ActItemPickingFields.WeightQuantityOrdered, Convert.ToDecimal(drow["ActivityItemPickingWeightQuantityOrdered"]));
                                //recPickItem.SetDecimal((int)SysCommon.ActItemPickingFields.QuantityShipped, Convert.ToDecimal(drow["ActivityItemPickingQuantityPicked"]));
                                //recPickItem.SetDecimal((int)SysCommon.ActItemPickingFields.BaseQuantityShipped, Convert.ToDecimal(drow["ActivityItemPickingBaseQuantityPicked"]));
                                //recPickItem.SetDecimal((int)SysCommon.ActItemPickingFields.WeightQuantityShipped, Convert.ToDecimal(drow["ActivityItemPickingWeightQuantityPicked"]));
                                //recPickItem.SetDecimal((int)SysCommon.ActItemPickingFields.QuantityAvailable, Convert.ToDecimal(drow["ActivityItemPickingQuantityAvailable"]));
                                //recPickItem.SetDecimal((int)SysCommon.ActItemPickingFields.BaseQuantityAvailable, Convert.ToDecimal(drow["ActivityItemPickingBaseQuantityAvailable"]));
                                //recPickItem.SetDecimal((int)SysCommon.ActItemPickingFields.WeightQuantityAvailable, Convert.ToDecimal(drow["ActivityItemPickingWeightQuantityAvailable"]));
                                //recPickItem.SetBoolean((int)SysCommon.ActItemPickingFields.IsReviewed, false);
                                //rsPickItem.Insert(recPickItem);

                                actItemPickComment = drow["ActivityItemPickingComment"].ToString();

                                if (!string.IsNullOrEmpty(actItemPickComment))
                                {

                                    cmndPickItemComment.CommandText = "INSERT INTO tActivityItemPickingComment (ActivityItemRefno, ActivityItemPickingCommentComment) VALUES (@activityItemRefno, @activityItemPickingCommentComment)";
                                    cmndPickItemComment.CommandType = CommandType.Text;
                                    //cmndPickItemComment.IndexName = "PK_ActivityItemPickingCommentOnActivityItemRefno";
                                    cmndPickItemComment.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemRefno));
                                    cmndPickItemComment.Parameters.Add(new SQLiteParameter("@activityItemPickingCommentComment", actItemPickComment));

                                    cmndPickItemComment.ExecuteNonQuery();

                                    //recPickItemComment.SetInt32((int)SysCommon.ActItemPickingCommentFields.ActItemRefno, actItemRefno);
                                    //recPickItemComment.SetString((int)SysCommon.ActItemPickingCommentFields.Comment, actItemPickComment);
                                    //rsPickItemComment.Insert(recPickItemComment);
                                }

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                        //                                                        }
                        //                                                    }

                        //                                                    drLocMapWhs.Close();
                        //                                                }
                        //                                            }

                        //                                            drUnit.Close();
                        //                                        }
                        //                                    }

                        //                                    drProd.Close();
                        //                                }
                        //                            }

                        //                            drProdUnit.Close();
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        transaction.Commit();

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order items complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order items failed.", true);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving sales order items failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        /// The CreateRequest
        /// </summary>
        /// <param name="actPickFKRefno"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected override string CreateRequest(int actPickFKRefno, XDocument criteria)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            XDocument request = null;

            if (criteria == null)
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>REQUEST SALES ORDER ITEMS</action>"
                    + SysCommon.GetRequestDefaults(false)
                    + ("<refno>" + actPickFKRefno + "</refno>")
                    + "<whs_id></whs_id>"
                    + "<sect_refno></sect_refno>"
                    + "</command>" +
                    ("<is_disconnected>" + this.SysEnv.SysSettings.IsDisconnectedMode + "</is_disconnected>")
                    + "</imclient>");
            }
            else
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>REQUEST SALES ORDER ITEMS</action>"
                    + SysCommon.GetRequestDefaults(false)
                    + ("<refno>" + actPickFKRefno + "</refno>")
                    + ("<whs_id>" + criteria.Root.Element("whs_id").Value + "</whs_id>")
                    + ("<sect_refno>" + criteria.Root.Element("sect_refno").Value + "</sect_refno>")
                    + "</command>"
                    + ("<is_disconnected>" + this.SysEnv.SysSettings.IsDisconnectedMode + "</is_disconnected>")
                    + "</imclient>");
            }

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
