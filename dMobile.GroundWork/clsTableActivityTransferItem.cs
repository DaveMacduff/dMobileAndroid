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
    /// Defines the <see cref="TableActivityTransferItem" />
    /// </summary>
    public class TableActivityTransferItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityTransferItem"/> class.
        /// </summary>
        public TableActivityTransferItem()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityTransferItem";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer items.", true);

                if (actRefno == 0)
                {
                    using (SQLiteCommand cmndTrans = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndTrans.CommandText = "SELECT tActivity.ActivityRefno FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                        cmndTrans.CommandType = CommandType.Text;
                        cmndTrans.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Transfers)));

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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding transfer items failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer items failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding transfer items failed.", ex);
            }
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override void Purge()
        {
            // TODO: Resource strings
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer items.", true);

            try
            {
                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Transfers)));

                    cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef"
                        + " WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN"
                        + " (SELECT tActivityItem.ActivityItemRefno"
                        + " FROM tActivityItem"
                        + " WHERE tActivityItem.ActivityItemSubType = @activityItemSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItemTransfer WHERE tActivityItemTransfer.ActivityItemSubType = @activityItemSubType";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = @activityItemSubType";
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging transfer items failed.", ex);
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
                    cmndStatus.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Transfers)));

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} transfer items.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Transfer items status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override string Send(int actRefno)
        {
            int transFKRefno = 0;

            using (SQLiteCommand cmndTrans = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndTrans.CommandText = "SELECT * FROM tActivityTransfer WHERE ActivityRefno = @activityRefno";
                cmndTrans.CommandType = CommandType.Text;
                //cmndTrans.IndexName = "PK_ActivityTransferOnActivityRefno";
                cmndTrans.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));

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
                        throw new Exception("drTrans.Read() failed.");
                    }
                    //}
                    //else
                    //{
                    //    // TODO: Resource strings
                    //    throw new Exception(string.Format("Unable to locate the transfer refno {0} in tActivityTransfer.", actRefno));
                    //}
                }
            }

            return this.SendComm(this.Create(transFKRefno));
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actRefno">The actRefno<see cref="int"/></param>
        protected override void Save(string reqData, int actRefno)
        {
            string errMsg = string.Empty;
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;

            using (SQLiteCommand cmndTransItem = conn.CreateCommand())
            using (SQLiteCommand cmndActItem = conn.CreateCommand())
            using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
            using (SQLiteCommand cmndProd = conn.CreateCommand())
            using (SQLiteCommand cmndUnit = conn.CreateCommand())
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

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer items.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityItemTransfer"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndTransItem = conn.CreateCommand())
                            //{
                            //cmndTransItem.CommandText = "tActivityItemTransfer";
                            //cmndTransItem.CommandType = CommandType.Text;
                            //cmndTransItem.IndexName = "UQ_ActivityItemTransferOnActivityItemTransferFKRefno";

                            //using (SQLiteCommand cmndActItem = conn.CreateCommand())
                            //{
                            //cmndActItem.CommandText = "tActivityItem";
                            //cmndActItem.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rsActItem = cmndActItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recActItem = rsActItem.CreateRecord();

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
                            //using (SQLiteCommand cmndWhs = conn.CreateCommand())
                            //{
                            //cmndWhs.CommandText = "tLocationMapWarehouse";
                            //cmndWhs.CommandType = CommandType.Text;
                            //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

                            //using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndID = conn.CreateCommand())
                            //{
                            cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityItem";
                            cmndID.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rs = cmndTransItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord rec = rs.CreateRecord();
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

                                cmndActItem.CommandText = "INSERT INTO tActivityItem (" +
                                    "ActivityItemSubType, " +
                                    "ActivityItemStatus, " +
                                    "ActivityRefno" +
                                    ") VALUES (" +
                                    "@activityItemSubType, " +
                                    "@activityItemStatus, " +
                                    "@activityRefno)";
                                cmndActItem.CommandType = CommandType.Text;
                                cmndActItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Transfers)));
                                cmndActItem.Parameters.Add(new SQLiteParameter("@activityItemStatus", Convert.ToInt32(0)));
                                cmndActItem.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));

                                cmndActItem.ExecuteNonQuery();

                                //recActItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.Transfers);
                                //recActItem.SetInt32((int)SysCommon.ActItemFields.Status, 0);
                                //recActItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, actRefno);
                                //rsActItem.Insert(recActItem);

                                actItemRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                //prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, ref errMsg, drProd, drUnit, drProdUnit, false);

                                int prodRefno = 0;
                                int prodFKRefno = 0;
                                bool useFNo = false;

                                if (useFNo)
                                {
                                    prodFKRefno = Convert.ToInt32(drow["f03"]);
                                }
                                else
                                {
                                    prodFKRefno = Convert.ToInt32(drow["ProductFKRefno"]);
                                }

                                errMsg += Environment.NewLine + "Prod fk refno " + prodFKRefno;

                                cmndProd.CommandText = "SELECT * FROM tProduct WHERE ProductFKRefno = @productFKRefno";
                                cmndProd.CommandType = CommandType.Text;
                                //cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                                cmndProd.Parameters.Add(new SQLiteParameter("@productFKRefno", prodFKRefno));

                                using (SQLiteDataReader drProd = cmndProd.ExecuteReader())
                                {
                                    //if (drProduct.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno }))
                                    //{
                                    if (drProd.Read())
                                    {
                                        prodRefno = drProd.GetInt32((int)SysCommon.ProductFields.Refno);
                                    }
                                    else
                                    {
                                        // TODO: Resource strings
                                        throw new Exception("drProduct.Read() failed." + Environment.NewLine + errMsg);
                                    }
                                    //}
                                    //else
                                    //{
                                    //    // TODO: Resource strings
                                    //    throw new Exception("Unable to locate the product refno from the product fk refno in tProduct." + Environment.NewLine + errMsg);
                                    //}
                                }

                                string unitCode = string.Empty;
                                int unitRefno = 0;

                                if (useFNo)
                                {
                                    unitCode = drow["f05"].ToString();
                                }
                                else
                                {
                                    unitCode = drow["UnitCode"].ToString();
                                }

                                errMsg += Environment.NewLine + "Unit \'" + unitCode + "\'";

                                cmndUnit.CommandText = "SELECT * FROM tUnit WHERE UnitCode = @unitCode";
                                cmndUnit.CommandType = CommandType.Text;
                                //cmndUnit.IndexName = "UQ_UnitOnUnitCode";
                                cmndUnit.Parameters.Add(new SQLiteParameter("@unitCode", unitCode));

                                using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
                                {
                                    //if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode }))
                                    //{
                                    if (drUnit.Read())
                                    {
                                        unitRefno = drUnit.GetInt32((int)SysCommon.UnitFields.UnitRefno);
                                    }
                                    else
                                    {
                                        // TODO: Resource strings
                                        throw new Exception("drUnit.Read() failed." + Environment.NewLine + errMsg);
                                    }
                                    //}
                                    //else
                                    //{
                                    //    // TODO: Resource strings
                                    //    throw new Exception("Unable to locate the unit refno from the unit code in  tUnit." + Environment.NewLine + errMsg);
                                    //}
                                }

                                bool prodUnitIsBC = false;

                                if (useFNo)
                                {
                                    prodUnitIsBC = Convert.ToBoolean(drow["f04"]);
                                }
                                else
                                {
                                    prodUnitIsBC = Convert.ToBoolean(drow["ProductUnitIsBC"]);
                                }

                                errMsg += Environment.NewLine + "Is bc " + prodUnitIsBC;

                                cmndProdUnit.CommandText = "SELECT * FROM tProductUnit WHERE ProductRefno = @productRefno AND UnitRefno = @unitRefno AND ProductUnitIsBC = @productUnitIsBC";
                                cmndProdUnit.CommandType = CommandType.Text;
                                //cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";
                                cmndProdUnit.Parameters.Add(new SQLiteParameter("@productRefno", prodRefno));
                                cmndProdUnit.Parameters.Add(new SQLiteParameter("@unitRefno", unitRefno));
                                cmndProdUnit.Parameters.Add(new SQLiteParameter("@productUnitIsBC", prodUnitIsBC));

                                using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                                {
                                    //if (drProdUnit.Seek(DbSeekOptions.FirstEqual, new object[] { prodRefno, unitRefno, prodUnitIsBC }))
                                    //{
                                    if (drProdUnit.Read())
                                    {
                                        prodUnitRefno = drProdUnit.GetInt32((int)SysCommon.ProdUnitFields.Refno);
                                    }
                                    else
                                    {
                                        // TODO: Resource strings
                                        throw new Exception("drProdUnit.Read() failed." + Environment.NewLine + errMsg);
                                    }
                                    //}
                                    //else
                                    //{
                                    //    // TODO: Resource strings
                                    //    throw new Exception("Unable to locate the product unit refno from the product fk refno, bc and unit code in tProductUnit." + Environment.NewLine + errMsg);
                                    //}
                                }

                                locMapRefno = 0;

                                if (locMapRefno == 0)
                                {
                                    locMapWhsFKRefno = Convert.ToInt32(drow["TransferFromLocationMapWarehouseFKRefno"]);
                                    errMsg += Environment.NewLine + "whs fk refno " + locMapWhsFKRefno;

                                    cmndWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                                    cmndWhs.CommandType = CommandType.Text;
                                    //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", locMapWhsFKRefno));

                                    using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                                    {
                                        //if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapWhsFKRefno }))
                                        //{
                                        if (drWhs.Read())
                                        {
                                            locMapRefno = drWhs.GetInt32((int)SysCommon.LocMapWhsFields.LocMapRefno);
                                        }
                                        //else
                                        //{
                                        //    // TODO: Resource strings
                                        //    throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
                                        //}
                                        //}
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
                                //cmndTransItem.IndexName = "UQ_ActivityItemTransferOnActivityItemTransferFKRefno";
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemRefno));
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Transfers)));
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@activityItemTransferFKRefno", Convert.ToInt32(drow["ActivityItemTransferFKRefno"])));
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@activityItemTransferStringFKRefno", Convert.ToString(transItemFKRefno)));
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@productUnitRefno", prodUnitRefno));
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@transferFromWarehouseLocationMapRefno", locMapRefno));
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@activityItemTransferRequiredQuantity", Convert.ToDecimal(drow["ActivityItemTransferRequiredQuantity"])));
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@activityItemTransferQuantity", Convert.ToDecimal(drow["ActivityItemTransferQuantity"])));
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@activityItemTransferBaseQuantity", Convert.ToDecimal(drow["ActivityItemTransferBaseQuantity"])));
                                cmndTransItem.Parameters.Add(new SQLiteParameter("@activityItemTransferWeightQuantity", Convert.ToDecimal(drow["ActivityItemTransferWeightQuantity"])));

                                cmndTransItem.ExecuteNonQuery();

                                //rec.SetInt32((int)SysCommon.ActItemTransferFields.ActItemRefno, actItemRefno);
                                //rec.SetInt32((int)SysCommon.ActItemTransferFields.ActItemSubType, (int)SysCommon.ActSubTypes.Transfers);
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

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                        //}
                        //drWhs.Close();
                        //}
                        //}
                        //drUnit.Close();
                        //}
                        //}
                        //drProd.Close();
                        //}
                        //}
                        //drProdUnit.Close();
                        //}
                        //}
                        //}
                        //}
                        //}
                        //}

                        transaction.Commit();

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer items complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer items failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving transfer items failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        /// The Create
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
                + "<action>REQUEST TRANSFER ITEMS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<refno>" + transFKRefno.ToString() + "</refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
