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
    /// Defines the <see cref="TableActivityPurchaseOrderWorkSheetItem" />
    /// </summary>
    public class TableActivityPurchaseOrderWorkSheetItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityPurchaseOrderWorkSheetItem"/> class.
        /// </summary>
        public TableActivityPurchaseOrderWorkSheetItem()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityPurchaseOrderWorkSheetItem";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding work sheet items.", true);

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

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding work sheet items complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding work sheet items failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding work sheet items failed.", ex);
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
                    cmndStatus.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet)));

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} work sheet items.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Work sheet status failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging work sheet items.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet)));

                    cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno " +
                        "IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = @activityItemSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItemPurchaseOrderWorkSheet WHERE tActivityItemPurchaseOrderWorkSheet.ActivityItemSubType = @activityItemSubType";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = @activityItemSubType";
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging work sheet items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging work sheet items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging work sheet items failed.", ex);
            }
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override string Send(int actRefno)
        {
            int actWorkSheetFKRefno = 0;
            return this.SendComm(this.Create(actWorkSheetFKRefno));
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

            using (SQLiteCommand cmndWorkSheetItem = conn.CreateCommand())
            using (SQLiteCommand cmndItem = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
            using (SQLiteCommand cmndActivity = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving work sheet items.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityItemPurchaseOrderWorkSheet"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndWorkSheetItem = conn.CreateCommand())
                            //{
                            //cmndWorkSheetItem.CommandText = "tActivityItemPurchaseOrderWorkSheet";
                            //cmndWorkSheetItem.CommandType = CommandType.Text;
                            //cmndWorkSheetItem.IndexName = "UQ_ActivityItemPurchaseOrderWorkSheetOnActivityItemPurchaseOrderWorkSheetFKRefno";

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
                            //cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

                            //using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndProd = conn.CreateCommand())
                            //{
                            //cmndProd.CommandText = "tProduct";
                            //cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                            //cmndProd.CommandType = CommandType.Text;

                            //using (SQLiteDataReader drProd = cmndProd.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndUnit = conn.CreateCommand())
                            //{
                            //cmndUnit.CommandText = "tUnit";
                            //cmndUnit.IndexName = "UQ_UnitOnUnitCode";
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
                            //using (SQLiteCommand cmndActivity = conn.CreateCommand())
                            //{
                            //cmndActivity.CommandText = "tActivity";
                            //cmndActivity.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rsActivity = cmndActivity.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recActivity = rsActivity.CreateRecord();

                            //using (SQLiteCommand cmndID = conn.CreateCommand())
                            //{
                            cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivity";
                            cmndID.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rs = cmndWorkSheetItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord rec = rs.CreateRecord();
                            int actItemRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;
                            int actItemWorkSheetFKRefno = 0;
                            int prodUnitRefno = 0;

                            DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityItemPurchaseOrderWorkSheet"].Rows)
                            {
                                errMsg = "Arguments:";

                                cmndActivity.CommandText = "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (@activityStatus, @activitySubType)";
                                cmndActivity.CommandType = CommandType.Text;
                                cmndActivity.Parameters.Add(new SQLiteParameter("@activityStatus", Convert.ToInt32(0)));
                                cmndActivity.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet)));

                                cmndActivity.ExecuteNonQuery();

                                //recActivity.SetInt32((int)SysCommon.ActFields.Status, 0);
                                //recActivity.SetInt32((int)SysCommon.ActFields.SubType, (int)SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                                //rsActivity.Insert(recActivity);

                                actRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                cmndItem.CommandText = "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (@activityItemSubType, @activityItemStatus, @activityRefno)";
                                cmndItem.CommandType = CommandType.Text;
                                cmndItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet)));
                                cmndItem.Parameters.Add(new SQLiteParameter("@activityItemStatus", Convert.ToInt32(0)));
                                cmndItem.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));

                                cmndItem.ExecuteNonQuery();

                                //recItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                                //recItem.SetInt32((int)SysCommon.ActItemFields.Status, 0);
                                //recItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, actRefno);
                                //rsItem.Insert(recItem);

                                prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, ref conn, ref errMsg, false);
                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, cmndLocMapWhs);

                                actItemWorkSheetFKRefno = Convert.ToInt32(drow["ActivityItemPurchaseOrderWorkSheetFKRefno"]);

                                cmndWorkSheetItem.CommandText = "INSERT INTO tActivityItemPurchaseOrderWorkSheet (" +
                                    "ActivityItemRefno, " +
                                    "ActivityItemSubType, " +
                                    "ActivityItemPurchaseOrderWorkSheetFKRefno, " +
                                    "ActivityItemPurchaseOrderWorkSheetStringFKRefno, " +
                                    "ProductUnitRefno, " +
                                    "WarehouseLocationMapRefno, " +
                                    "ActivityItemPurchaseOrderWorkSheetQuantityOrdered, " +
                                    "ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered, " +
                                    "ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered" +
                                    ") VALUES (" +
                                    "@activityItemRefno, " +
                                    "@activityItemSubType, " +
                                    "@activityItemPurchaseOrderWorkSheetFKRefno, " +
                                    "@activityItemPurchaseOrderWorkSheetStringFKRefno, " +
                                    "@productUnitRefno, " +
                                    "@warehouseLocationMapRefno, " +
                                    "@activityItemPurchaseOrderWorkSheetQuantityOrdered, " +
                                    "@activityItemPurchaseOrderWorkSheetBaseQuantityOrdered, " +
                                    "@activityItemPurchaseOrderWorkSheetWeightQuantityOrdered" +
                                    ")";
                                cmndWorkSheetItem.CommandType = CommandType.Text;
                                //cmndWorkSheetItem.IndexName = "UQ_ActivityItemPurchaseOrderWorkSheetOnActivityItemPurchaseOrderWorkSheetFKRefno";
                                cmndWorkSheetItem.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemRefno));
                                cmndWorkSheetItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet)));
                                cmndWorkSheetItem.Parameters.Add(new SQLiteParameter("@activityItemPurchaseOrderWorkSheetFKRefno", actItemWorkSheetFKRefno));
                                cmndWorkSheetItem.Parameters.Add(new SQLiteParameter("@activityItemPurchaseOrderWorkSheetStringFKRefno", actItemWorkSheetFKRefno.ToString()));
                                cmndWorkSheetItem.Parameters.Add(new SQLiteParameter("@productUnitRefno", prodUnitRefno));
                                cmndWorkSheetItem.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", whsLocMapRefno));
                                cmndWorkSheetItem.Parameters.Add(new SQLiteParameter("@activityItemPurchaseOrderWorkSheetQuantityOrdered", Convert.ToDecimal(drow["ActivityItemPurchaseOrderWorkSheetQuantityOrdered"])));
                                cmndWorkSheetItem.Parameters.Add(new SQLiteParameter("@activityItemPurchaseOrderWorkSheetBaseQuantityOrdered", Convert.ToDecimal(drow["ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered"])));
                                cmndWorkSheetItem.Parameters.Add(new SQLiteParameter("@activityItemPurchaseOrderWorkSheetWeightQuantityOrdered", Convert.ToDecimal(drow["ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered"])));

                                cmndWorkSheetItem.ExecuteNonQuery();

                                //rec.SetInt32((int)SysCommon.ActItemWorkSheetFields.ActItemRefno, actItemRefno);
                                //rec.SetInt32((int)SysCommon.ActItemWorkSheetFields.ActItemSubType, (int)SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                                //rec.SetInt32((int)SysCommon.ActItemWorkSheetFields.FKRefno, actItemWorkSheetFKRefno);
                                //rec.SetString((int)SysCommon.ActItemWorkSheetFields.StringFKRefno, actItemWorkSheetFKRefno.ToString());
                                //rec.SetInt32((int)SysCommon.ActItemWorkSheetFields.ProdUnitRefno, prodUnitRefno);
                                //rec.SetInt32((int)SysCommon.ActItemWorkSheetFields.WhsLocMapRefno, whsLocMapRefno);
                                //rec.SetDecimal((int)SysCommon.ActItemWorkSheetFields.QuantityOrdered, Convert.ToDecimal(drow["ActivityItemPurchaseOrderWorkSheetQuantityOrdered"]));
                                //rec.SetDecimal((int)SysCommon.ActItemWorkSheetFields.BaseQuantityOrdered, Convert.ToDecimal(drow["ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered"]));
                                //rec.SetDecimal((int)SysCommon.ActItemWorkSheetFields.WeightQuantityOrdered, Convert.ToDecimal(drow["ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered"]));
                                //rs.Insert(rec);

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
                        //                                                }

                        //                                                drLocMapWhs.Close();
                        //                                            }
                        //                                        }

                        //                                        drUnit.Close();
                        //                                    }
                        //                                }

                        //                                drProd.Close();
                        //                            }
                        //                        }

                        //                        drProdUnit.Close();
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        transaction.Commit();

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving work sheet items complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving work sheet items failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving work sheet items failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="actWorkSheetFKRefno">The actWorkSheetFKRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override string Create(int actWorkSheetFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PURCHASE ORDER WORK SHEET ITEMS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<refno>" + actWorkSheetFKRefno + "</refno>")
                + "</command>"
                + ("<whs_id>" + LocationMap.GetInstance().CurrLocMap.WhsFKRefno + "</whs_id>")
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
