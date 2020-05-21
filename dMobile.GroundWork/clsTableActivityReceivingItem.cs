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
    /// Defines the <see cref="TableActivityRecevingItem" />
    /// </summary>
    public class TableActivityRecevingItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityRecevingItem"/> class.
        /// </summary>
        public TableActivityRecevingItem()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReceivingItem";
        }

        /// <summary>
        /// The Request
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/></param>
        public override void Request(int actRefno)
        {
            string reqData = string.Empty;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving items.", true);

                if (actRefno == 0)
                {
                    using (SQLiteCommand cmndRecv = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndRecv.CommandText = "SELECT * FROM tActivity WHERE ActivitySubType = @activitySubType";
                        cmndRecv.CommandType = CommandType.Text;
                        cmndRecv.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Receiving)));

                        using (SQLiteDataReader drRecv = cmndRecv.ExecuteReader())
                        {
                            while (drRecv.Read())
                            {
                                actRefno = Convert.ToInt32(drRecv["ActivityRefno"]);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving items failed.", false);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding receiving items failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving items failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding receiving items failed.", ex);
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
                    cmndStatus.CommandType = CommandType.Text;
                    cmndStatus.CommandText = "SELECT COUNT(tActivityItem.ActivityItemRefno) FROM tActivityItem WHERE ActivityItemSubType = @activityItemSubType";
                    cmndStatus.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Receiving)));

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} receiving items.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Receiving items status failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving items.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Receiving)));

                    cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef "
                        + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
                        + "(SELECT tActivityItem.ActivityItemRefno "
                        + "FROM tActivityItem "
                        + "WHERE tActivityItem.ActivityItemSubType = @activityItemSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItemReceiving WHERE tActivityItemReceiving.ActivityItemSubType = @activityItemSubType";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = @activityItemSubType";
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging receiving items failed.", ex);
            }
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override string Send(int actRefno)
        {
            int recvFKRefno = 0;

            using (SQLiteCommand cmndRecv = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndRecv.CommandText = "SELECT * FROM tActivityReceiving WHERE ActivityRefno = @activityRefno";
                cmndRecv.CommandType = CommandType.Text;
                //cmndRecv.IndexName = "PK_ActivityReceivingOnActivityRefno";
                cmndRecv.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));

                using (SQLiteDataReader drRecv = cmndRecv.ExecuteReader())
                {
                    //if (drRecv.Seek(DbSeekOptions.FirstEqual, new object[] { actRefno }))
                    //{
                    if (drRecv.Read())
                    {
                        recvFKRefno = drRecv.GetInt32((int)SysCommon.ActReceivingFields.FKRefno);
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception("drRecv.Read() failed.");
                    }
                    //}
                    //else
                    //{
                    //    // TODO: Resource strings
                    //    throw new Exception(string.Format("Unable to locate the receiving activity refno {0} in tActivityReceiving", actRefno));
                    //}
                }
            }

            return this.SendComm(this.Create(recvFKRefno));
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

            using (SQLiteCommand cmndRecvItem = conn.CreateCommand())
            using (SQLiteCommand cmndRecvItemComment = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving items.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityItemPurchaseOrder"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndRecvItem = conn.CreateCommand())
                            //{
                            //cmndRecvItem.CommandText = "tActivityItemReceiving";
                            //cmndRecvItem.CommandType = CommandType.Text;
                            ////cmndRecvItem.IndexName = "UQ_ActivityItemReceivingOnActivityItemReceivingFKRefno";

                            //using (SQLiteCommand cmndRecvItemComment = conn.CreateCommand())
                            //{
                            //cmndRecvItemComment.CommandText = "tActivityItemReceivingComment";
                            //cmndRecvItemComment.CommandType = CommandType.Text;
                            //cmndRecvItemComment.IndexName = "PK_ActivityItemReceivingCommentOnActivityItemRefno";

                            //using (SQLiteCommand cmndItem = conn.CreateCommand())
                            //{
                            //cmndItem.CommandText = "tActivityItem";
                            //cmndItem.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rsItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recItem = rsItem.CreateRecord();

                            //using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
                            //{
                            //cmndProdUnit.CommandText = "tProductUnit";
                            //cmndProdUnit.CommandType = CommandType.Text;
                            ////cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

                            //using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndProd = conn.CreateCommand())
                            //{
                            //cmndProd.CommandText = "tProduct";
                            ////cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                            //cmndProd.CommandType = CommandType.Text;

                            //using (SQLiteDataReader drProd = cmndProd.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndUnit = conn.CreateCommand())
                            //{
                            //cmndUnit.CommandText = "tUnit";
                            ////cmndUnit.IndexName = "UQ_UnitOnUnitCode";
                            //cmndUnit.CommandType = CommandType.Text;

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
                            //cmndID.CommandText = "SELECT last_insert_rowid()";
                            //cmndID.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rsRecvItemComment = cmndRecvItemComment.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recRecvItemComment = rsRecvItemComment.CreateRecord();

                            //using (SqlCeResultSet rs = cmndRecvItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord rec = rs.CreateRecord();
                            int actItemRefno = 0;
                            int progValue = 0;
                            int prodUnitRefno = 0;
                            int locMapRefno = 0;
                            string recvItemComment = string.Empty;
                            int recvItemFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityItemPurchaseOrder"].Rows)
                            {
                                errMsg = "Arguments:";

                                cmndItem.CommandText = "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (@activityItemSubType, @activityItemStatus, @activityRefno)";
                                cmndItem.CommandType = CommandType.Text;
                                cmndItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Receiving)));
                                cmndItem.Parameters.Add(new SQLiteParameter("@activityItemStatus", Convert.ToInt32(0)));
                                cmndItem.Parameters.Add(new SQLiteParameter("@activityRefno", Convert.ToInt32(actRefno)));

                                cmndItem.ExecuteNonQuery();

                                //recItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.Receiving);
                                //recItem.SetInt32((int)SysCommon.ActItemFields.Status, 0);
                                //recItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, actRefno);
                                //rsItem.Insert(recItem);

                                cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityItem";
                                cmndID.CommandType = CommandType.Text;

                                actItemRefno = Convert.ToInt32(cmndID.ExecuteScalar());
                                prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, ref conn, ref errMsg, false);
                                locMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, cmndLocMapWhs);

                                recvItemFKRefno = Convert.ToInt32(drow["ActivityItemPurchaseOrderFKRefno"]);
                                errMsg += Environment.NewLine + "Item fk refno " + recvItemFKRefno;

                                cmndRecvItem.CommandText = "INSERT INTO tActivityItemReceiving (" +
                                    "ActivityItemRefno, " +
                                    "ActivityItemSubType, " +
                                    "ActivityItemReceivingFKRefno, " +
                                    "ActivityItemReceivingStringFKRefno, " +
                                    "ProductUnitRefno, " +
                                    "WarehouseLocationMapRefno, " +
                                    "ActivityItemReceivingQuantityOrdered, " +
                                    "ActivityItemReceivingBaseQuantityOrdered, " +
                                    "ActivityItemReceivingWeightQuantityOrdered, " +
                                    "ActivityItemReceivingQuantityReceived, " +
                                    "ActivityItemReceivingBaseQuantityReceived, " +
                                    "ActivityItemReceivingWeightQuantityReceived, " +
                                    "ActivityItemReceivingVendorProductUnitCode, " +
                                    "ActivityItemReceivingDescription, " +
                                    "ActivityItemReceivingQuantityReceivedToDate, " +
                                    "ActivityItemReceivingBaseQuantityReceivedToDate, " +
                                    "ActivityItemReceivingWeightQuantityReceivedToDate" +
                                    ") VALUES (" +
                                    "@activityItemRefno, " +
                                    "@activityItemSubType, " +
                                    "@activityItemReceivingFKRefno, " +
                                    "@activityItemReceivingStringFKRefno, " +
                                    "@productUnitRefno, " +
                                    "@warehouseLocationMapRefno, " +
                                    "@activityItemReceivingQuantityOrdered, " +
                                    "@activityItemReceivingBaseQuantityOrdered, " +
                                    "@activityItemReceivingWeightQuantityOrdered, " +
                                    "@activityItemReceivingQuantityReceived, " +
                                    "@activityItemReceivingBaseQuantityReceived, " +
                                    "@activityItemReceivingWeightQuantityReceived, " +
                                    "@activityItemReceivingVendorProductUnitCode, " +
                                    "@activityItemReceivingDescription, " +
                                    "@activityItemReceivingQuantityReceivedToDate, " +
                                    "@activityItemReceivingBaseQuantityReceivedToDate, " +
                                    "@activityItemReceivingWeightQuantityReceivedToDate" +
                                    ")";
                                cmndRecvItem.CommandType = CommandType.Text;
                                //cmndRecvItem.IndexName = "UQ_ActivityItemReceivingOnActivityItemReceivingFKRefno";
                                cmndRecvItem.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(actItemRefno)));
                                cmndRecvItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Receiving)));
                                cmndRecvItem.Parameters.Add(new SQLiteParameter("@activityItemReceivingFKRefno", Convert.ToInt32(recvItemFKRefno)));
                                cmndRecvItem.Parameters.Add(new SQLiteParameter("@activityItemReceivingStringFKRefno", Convert.ToString(recvItemFKRefno)));
                                cmndRecvItem.Parameters.Add(new SQLiteParameter("@productUnitRefno", Convert.ToInt32(prodUnitRefno)));
                                cmndRecvItem.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", Convert.ToInt32(locMapRefno)));
                                cmndRecvItem.Parameters.Add(new SQLiteParameter("@activityItemReceivingQuantityOrdered", Convert.ToDecimal(drow["ActivityItemPurchaseOrderQuantityOrdered"])));
                                cmndRecvItem.Parameters.Add(new SQLiteParameter("@activityItemReceivingBaseQuantityOrdered", Convert.ToDecimal(drow["ActivityItemPurchaseOrderBaseQuantityOrdered"])));
                                cmndRecvItem.Parameters.Add(new SQLiteParameter("@activityItemReceivingWeightQuantityOrdered", Convert.ToDecimal(drow["ActivityItemPurchaseOrderWeightQuantityOrdered"])));
                                cmndRecvItem.Parameters.Add(new SQLiteParameter("@activityItemReceivingQuantityReceived", Convert.ToDecimal(drow["ActivityItemPurchaseOrderQuantityReceived"])));
                                cmndRecvItem.Parameters.Add(new SQLiteParameter("@activityItemReceivingBaseQuantityReceived", Convert.ToDecimal(drow["ActivityItemPurchaseOrderBaseQuantityReceived"])));
                                cmndRecvItem.Parameters.Add(new SQLiteParameter("@activityItemReceivingWeightQuantityReceived", Convert.ToDecimal(drow["ActivityItemPurchaseOrderWeightQuantityReceived"])));
                                cmndRecvItem.Parameters.Add(new SQLiteParameter("@activityItemReceivingVendorProductUnitCode", Convert.ToString(drow["ActivityItemPurchaseOrderVendorProductUnitCode"])));
                                cmndRecvItem.Parameters.Add(new SQLiteParameter("@activityItemReceivingDescription", Convert.ToString(drow["ActivityItemPurchaseOrderDescription"])));
                                cmndRecvItem.Parameters.Add(new SQLiteParameter("@activityItemReceivingQuantityReceivedToDate", Convert.ToDecimal(drow["ActivityItemPurchaseOrderQuantityReceivedToDate"])));
                                cmndRecvItem.Parameters.Add(new SQLiteParameter("@activityItemReceivingBaseQuantityReceivedToDate", Convert.ToDecimal(drow["ActivityItemPurchaseOrderBaseQuantityReceivedToDate"])));
                                cmndRecvItem.Parameters.Add(new SQLiteParameter("@activityItemReceivingWeightQuantityReceivedToDate", Convert.ToDecimal(drow["ActivityItemPurchaseOrderWeightQuantityReceivedToDate"])));

                                cmndRecvItem.ExecuteNonQuery();

                                //rec.SetInt32((int)SysCommon.ActItemReceivingFields.ActItemRefno, actItemRefno);
                                //rec.SetInt32((int)SysCommon.ActItemReceivingFields.ActItemSubType, (int)SysCommon.ActSubTypes.Receiving);
                                //rec.SetInt32((int)SysCommon.ActItemReceivingFields.FKRefno, recvItemFKRefno);
                                //rec.SetString((int)SysCommon.ActItemReceivingFields.StringFKRefno, Convert.ToString(recvItemFKRefno));
                                //rec.SetInt32((int)SysCommon.ActItemReceivingFields.ProdUnitRefno, prodUnitRefno);
                                //rec.SetInt32((int)SysCommon.ActItemReceivingFields.WhsLocMapRefno, locMapRefno);
                                //rec.SetDecimal((int)SysCommon.ActItemReceivingFields.QuantityOrdered, Convert.ToDecimal(drow["ActivityItemPurchaseOrderQuantityOrdered"]));
                                //rec.SetDecimal((int)SysCommon.ActItemReceivingFields.BaseQuantityOrdered, Convert.ToDecimal(drow["ActivityItemPurchaseOrderBaseQuantityOrdered"]));
                                //rec.SetDecimal((int)SysCommon.ActItemReceivingFields.WeightQuantityOrdered, Convert.ToDecimal(drow["ActivityItemPurchaseOrderWeightQuantityOrdered"]));
                                //rec.SetDecimal((int)SysCommon.ActItemReceivingFields.QuantityReceived, Convert.ToDecimal(drow["ActivityItemPurchaseOrderQuantityReceived"]));
                                //rec.SetDecimal((int)SysCommon.ActItemReceivingFields.BaseQuantityReceived, Convert.ToDecimal(drow["ActivityItemPurchaseOrderBaseQuantityReceived"]));
                                //rec.SetDecimal((int)SysCommon.ActItemReceivingFields.WeightQuantityReceived, Convert.ToDecimal(drow["ActivityItemPurchaseOrderWeightQuantityReceived"]));
                                //rec.SetString((int)SysCommon.ActItemReceivingFields.VendorProductUnitCode, drow["ActivityItemPurchaseOrderVendorProductUnitCode"].ToString());
                                //rec.SetString((int)SysCommon.ActItemReceivingFields.Description, drow["ActivityItemPurchaseOrderDescription"].ToString());
                                //rec.SetDecimal((int)SysCommon.ActItemReceivingFields.QuantityReceivedToDate, Convert.ToDecimal(drow["ActivityItemPurchaseOrderQuantityReceivedToDate"]));
                                //rec.SetDecimal((int)SysCommon.ActItemReceivingFields.BaseQuantityReceivedToDate, Convert.ToDecimal(drow["ActivityItemPurchaseOrderBaseQuantityReceivedToDate"]));
                                //rec.SetDecimal((int)SysCommon.ActItemReceivingFields.WeightQuantityReceivedToDate, Convert.ToDecimal(drow["ActivityItemPurchaseOrderWeightQuantityReceivedToDate"]));
                                //rs.Insert(rec);

                                recvItemComment = drow["ActivityItemPurchaseOrderComment"].ToString();

                                if (!string.IsNullOrEmpty(recvItemComment))
                                {
                                    cmndRecvItemComment.CommandText = "INSERT INTO tActivityItemReceivingComment (ActivityItemRefno, ActivityItemReceivingCommentComment) VALUES (@activityItemRefno, @activityItemReceivingCommentComment)";
                                    cmndRecvItemComment.CommandType = CommandType.Text;
                                    cmndRecvItemComment.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemRefno));
                                    cmndRecvItemComment.Parameters.Add(new SQLiteParameter("@activityItemReceivingCommentComment", recvItemComment));

                                    cmndRecvItemComment.ExecuteNonQuery();

                                    //recRecvItemComment.SetInt32((int)SysCommon.ActItemReceivingCommentFields.ActItemRefno, actItemRefno);
                                    //recRecvItemComment.SetString((int)SysCommon.ActItemReceivingCommentFields.Comment, recvItemComment);
                                    //rsRecvItemComment.Insert(recRecvItemComment);
                                }

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                        //}
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving items complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving items failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving receiving items failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="recvFKRefno"></param>
        /// <returns></returns>
        protected override string Create(int recvFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            string deviceID = SysDeviceID.GetDeviceID();
            //string revision = DateAndTime.DateSerial(1900, 1, 1).ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
            string revision = new DateTime(1900, 1, 1).ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
            string loginID = SysEnv.ContactLog.LoginID.ToString();

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PURCHASE ORDER ITEMS</action>"
                + ("<device_id>" + deviceID + "</device_id>")
                + ("<login_id>" + loginID + "</login_id>")
                + ("<revision_datetime>" + revision + "</revision_datetime>")
                + ("<refno>" + recvFKRefno.ToString() + "</refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// The GetProductXRefUnitRefnoFromDataRow
        /// </summary>
        /// <param name="drow"></param>
        /// <param name="errMsg"></param>
        /// <param name="drProduct"></param>
        /// <param name="drUnit"></param>
        /// <param name="drProdUnit"></param>
        /// <returns></returns>
        private int GetProductXRefUnitRefnoFromDataRow(
            DataRow drow,
            ref string errMsg,
            SQLiteDataReader drProduct,
            SQLiteDataReader drUnit,
            SQLiteDataReader drProdUnit)
        {
            //      int prodRefno = 0;
            //      int prodFKRefno = Convert.ToInt32(drow["ProductFKRefno"]);
            //      errMsg += Environment.NewLine + "Prod fk refno " + prodFKRefno;

            //      if (drProduct.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno }))
            //      {
            //        if (drProduct.Read())
            //        {
            //          prodRefno = drProduct.GetInt32((int)SysCommon.ProductFields.Refno);
            //        }
            //        else
            //        {
            //#if MULTILANGUAGE
            //          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrproductreadFailed) + Environment.NewLine + errMsg);
            //#else
            //          throw new Exception("drProduct.Read() failed." + Environment.NewLine + errMsg);
            //#endif
            //        }
            //      }
            //      else
            //      {
            //#if MULTILANGUAGE
            //        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductRefnoFromTheProductFkRefnoInTproduct)
            //#else
            //        throw new Exception("Unable to locate the product refno from the product fk refno in tProduct."
            //#endif
            //            + Environment.NewLine + errMsg);
            //      }

            //      int unitRefno = 0;
            //      string unitCode = drow["XRefUnitCode"].ToString();
            //      errMsg += Environment.NewLine + "Unit \'" + unitCode + "\'";

            //      if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode }))
            //      {
            //        if (drUnit.Read())
            //        {
            //          unitRefno = drUnit.GetInt32((int)SysCommon.UnitFields.UnitRefno);
            //        }
            //        else
            //        {
            //#if MULTILANGUAGE
            //          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrunitreadFailed) + Environment.NewLine + errMsg);
            //#else
            //          throw new Exception("drUnit.Read() failed." + Environment.NewLine + errMsg);
            //#endif
            //        }
            //      }
            //      else
            //      {
            //#if MULTILANGUAGE
            //        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheUnitRefnoFromTheXrefUnitCodeInTunit)
            //#else
            //        throw new Exception("Unable to locate the unit refno from the xref unit code in  tUnit."
            //#endif
            //            + Environment.NewLine + errMsg);
            //      }

            int prodUnitRefno = 0;
            //      bool prodUnitIsBC = Convert.ToBoolean(drow["ProductUnitIsBC"]);
            //      errMsg += Environment.NewLine + "Is bc " + prodUnitIsBC;

            //      if (drProdUnit.Seek(DbSeekOptions.FirstEqual, new object[] { prodRefno, unitRefno, prodUnitIsBC }))
            //      {
            //        if (drProdUnit.Read())
            //        {
            //          prodUnitRefno = drProdUnit.GetInt32((int)SysCommon.ProdUnitFields.Refno);
            //        }
            //        else
            //        {
            //#if MULTILANGUAGE
            //          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrprodunitreadFailed) + Environment.NewLine + errMsg);
            //#else
            //          throw new Exception("drProdUnit.Read() failed." + Environment.NewLine + errMsg);
            //#endif
            //        }
            //      }
            //      else
            //      {
            //        throw new Exception(
            //#if MULTILANGUAGE
            //          Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductUnitRefnoFromTheProductFkRefnoBcAndXrefUnitCodeInTproductunit)
            //#else
            //          "Unable to locate the product unit refno from the product fk refno, bc and xref unit code in tProductUnit."
            //#endif
            //          + Environment.NewLine + errMsg);
            //      }

            return prodUnitRefno;
        }
    }
}
