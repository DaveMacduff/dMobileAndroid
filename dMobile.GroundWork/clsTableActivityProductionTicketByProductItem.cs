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
    /// Defines the <see cref="TableActivityProductionTicketByProductItem" />
    /// </summary>
    public class TableActivityProductionTicketByProductItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketByProductItem"/> class.
        /// </summary>
        public TableActivityProductionTicketByProductItem()
      : base()
        {
            FullClassName = "dMobile.GroundWork.TableActivityProductionTicketByProductItem";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product items.", true);

                if (actRefno == 0)
                {
                    using (SQLiteCommand cmndActivity = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndActivity.CommandType = CommandType.Text;
                        cmndActivity.CommandText = "SELECT tActivity.ActivityRefno FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                        cmndActivity.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct)));

                        using (SQLiteDataReader drActivity = cmndActivity.ExecuteReader())
                        {
                            while (drActivity.Read())
                            {
                                actRefno = Convert.ToInt32(drActivity["ActivityRefno"]);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding ticket add-product items failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product items failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding ticket add-product items failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product items.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct)));

                    cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef "
                        + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
                        + "(SELECT tActivityItem.ActivityItemRefno "
                        + "FROM tActivityItem "
                        + "WHERE tActivityItem.ActivityItemSubType = @activityItemSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItemProductionByProduct "
                        + "WHERE tActivityItemProductionByProduct.ActivityItemRefno IN "
                        + "(SELECT tActivityItem.ActivityItemRefno "
                        + "FROM tActivityItem "
                        + "WHERE tActivityItem.ActivityItemSubType = @activityItemSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItemProduction "
                        + "WHERE tActivityItemProduction.ActivityItemRefno IN "
                        + "(SELECT tActivityItem.ActivityItemRefno "
                        + "FROM tActivityItem "
                        + "WHERE tActivityItem.ActivityItemSubType = @activityItemSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = @activityItemSubType";
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging ticket add-product items failed.", ex);
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
                    cmndStatus.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct)));

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} ticket add-product items.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Ticket add-product items status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actRefno"></param>
        /// <returns></returns>
        protected override string Send(int actRefno)
        {
            int productionFKRefno = 0;

            using (SQLiteCommand cmndByProduct = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndByProduct.CommandText = "SELECT * FROM tActivityProductionByProduct WHERE ActivityRefno = @activityRefno";
                cmndByProduct.CommandType = CommandType.Text;
                //cmndByProduct.IndexName = "PK_ActivityProductionByProductOnActivityRefno";
                cmndByProduct.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));

                using (SQLiteDataReader drByProduct = cmndByProduct.ExecuteReader())
                {
                    //if (drByProduct.Seek(DbSeekOptions.FirstEqual, new object[] { actRefno }))
                    //{
                    if (drByProduct.Read())
                    {
                        productionFKRefno = drByProduct.GetInt32((int)SysCommon.ActProdByProductFields.FKRefno);
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception("drByProduct.Read() failed.");
                    }
                    //}
                    //else
                    //{
                    //    // TODO: Resource strings
                    //    throw new Exception(string.Format("Unable to locate the ticket add-product refno {0} in tActivityProductionByProduct.", actRefno));
                    //}
                }
            }

            return this.SendComm(Create(productionFKRefno));
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

            using (SQLiteCommand cmndProduction = conn.CreateCommand())
            using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
            using (SQLiteCommand cmndProd = conn.CreateCommand())
            using (SQLiteCommand cmndUnit = conn.CreateCommand())
            using (SQLiteCommand cmndWhs = conn.CreateCommand())
            using (SQLiteCommand cmndID = conn.CreateCommand())
            using (SQLiteCommand cmndItem = conn.CreateCommand())
            using (SQLiteCommand cmndActItemByProduct = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product items.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityItemProduction"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndProduction = conn.CreateCommand())
                            //{
                            //cmndProduction.CommandText = "tActivityItemProduction";
                            //cmndProduction.CommandType = CommandType.Text;
                            //cmndProduction.IndexName = "PK_ActivityItemProductionOnActivityItemRefno";

                            //using (SQLiteCommand cmndItem = conn.CreateCommand())
                            //{
                            //cmndItem.CommandText = "tActivityItem";
                            //cmndItem.CommandType = CommandType.Text;

                            //using (SQLiteCommand cmndActItemByProduct = conn.CreateCommand())
                            //{
                            //cmndActItemByProduct.CommandText = "tActivityItemProductionByProduct";
                            //cmndActItemByProduct.CommandType = CommandType.Text;
                            //cmndActItemByProduct.IndexName = "UQ_ActivityItemProductionByProductOnActivityItemProductionByProductFKRefno";

                            //using (SqlCeResultSet rsItemByProduct = cmndActItemByProduct.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recItemByProduct = rsItemByProduct.CreateRecord();

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
                            //cmndProd.CommandType = CommandType.Text;
                            //cmndProd.IndexName = "UQ_ProductOnProductFKRefno";

                            //using (SQLiteDataReader drProduct = cmndProd.ExecuteReader())
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
                            //cmndID.CommandText = "SELECT last_insert_rowid()";
                            //cmndID.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rsItemProduction = cmndProduction.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recItemProduction = rsItemProduction.CreateRecord();
                            int actItemRefno = 0;
                            int progValue = 0;
                            int locMapRefno = 0;
                            int actItemProductionFKRefno = 0;
                            int prodUnitRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityItemProduction"].Rows)
                            {
                                errMsg = "Arguments:";

                                cmndItem.CommandText = "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (@activityItemSubType, @activityItemStatus, @activityRefno)";
                                cmndItem.CommandType = CommandType.Text;
                                cmndItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct)));
                                cmndItem.Parameters.Add(new SQLiteParameter("@activityItemStatus", Convert.ToInt32(SysCommon.ActStatus.NoChange)));
                                cmndItem.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));

                                cmndItem.ExecuteNonQuery();

                                //recItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.TicketAddProduct);
                                //recItem.SetInt32((int)SysCommon.ActItemFields.Status, (int)SysCommon.ActStatus.NoChange);
                                //recItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, actRefno);
                                //rsItem.Insert(recItem);

                                cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityItem";
                                cmndID.CommandType = CommandType.Text;

                                actItemRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                actItemProductionFKRefno = Convert.ToInt32(drow["ActivityItemProductionFKRefno"]);
                                errMsg += Environment.NewLine + "Item fk refno " + actItemProductionFKRefno;

                                //prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, ref errMsg, drProduct, drUnit, drProdUnit, false);

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

                                using (SQLiteDataReader drProduct = cmndProd.ExecuteReader())
                                {
                                    //if (drProduct.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno }))
                                    //{
                                    if (drProduct.Read())
                                    {
                                        prodRefno = drProduct.GetInt32((int)SysCommon.ProductFields.Refno);
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

                                cmndWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                                cmndWhs.CommandType = CommandType.Text;
                                //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                                cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", Convert.ToInt32(drow["LocationMapWarehouseFKRefno"])));

                                using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                                {
                                    locMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, drWhs);
                                }

                                cmndProduction.CommandText = "INSERT INTO tActivityItemProduction " +
                                    "(" +
                                    "ActivityItemRefno, " +
                                    "ActivityItemSubType, " +
                                    "ProductUnitRefno, " +
                                    "LocationMapRefno, " +
                                    "ActivityItemProductionQuantity, " +
                                    "ActivityItemProductionBaseQuantity, " +
                                    "ActivityItemProductionWeightQuantity, " +
                                    "ActivityItemProductionDescription, " +
                                    "ActivityItemProductionQuantityExpected, " +
                                    "ActivityItemProductionBaseQuantityExpected, " +
                                    "ActivityItemProductionWeightQuantityExpected" +
                                    ") " +
                                    "VALUES " +
                                    "(" +
                                    "@activityItemRefno, " +
                                    "@activityItemSubType, " +
                                    "@productUnitRefno, " +
                                    "@locationMapRefno, " +
                                    "@activityItemProductionQuantity, " +
                                    "@activityItemProductionBaseQuantity, " +
                                    "@activityItemProductionWeightQuantity, " +
                                    "@activityItemProductionDescription, " +
                                    "@activityItemProductionQuantityExpected, " +
                                    "@activityItemProductionBaseQuantityExpected, " +
                                    "@activityItemProductionWeightQuantityExpected" +
                                    ")";
                                cmndProduction.CommandType = CommandType.Text;
                                //cmndProduction.IndexName = "PK_ActivityItemProductionOnActivityItemRefno";
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemRefno));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct)));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@productUnitRefno", prodUnitRefno));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@locationMapRefno", locMapRefno));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activityItemProductionQuantity", Convert.ToDecimal(drow["ActivityItemProductionQuantity"])));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activityItemProductionBaseQuantity", Convert.ToDecimal(drow["ActivityItemProductionBaseQuantity"])));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activityItemProductionWeightQuantity", Convert.ToDecimal(drow["ActivityItemProductionWeightQuantity"])));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activityItemProductionDescription", drow["ActivityItemProductionDescription"].ToString()));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activityItemProductionQuantityExpected", Convert.ToDecimal(drow["ActivityItemProductionQuantityExpected"])));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activityItemProductionBaseQuantityExpected", Convert.ToDecimal(drow["ActivityItemProductionBaseQuantityExpected"])));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activityItemProductionWeightQuantityExpected", Convert.ToDecimal(drow["ActivityItemProductionWeightQuantityExpected"])));

                                cmndProduction.ExecuteNonQuery();

                                //recItemProduction.SetInt32((int)SysCommon.ActItemProdFields.ActItemRefno, actItemRefno);
                                //recItemProduction.SetInt32((int)SysCommon.ActItemProdFields.ActivityItemSubType, (int)SysCommon.ActSubTypes.TicketAddProduct);
                                //recItemProduction.SetInt32((int)SysCommon.ActItemProdFields.ProdUnitRefno, prodUnitRefno);
                                //recItemProduction.SetInt32((int)SysCommon.ActItemProdFields.LocMapRefno, locMapRefno);
                                //recItemProduction.SetDecimal((int)SysCommon.ActItemProdFields.Quantity, Convert.ToDecimal(drow["ActivityItemProductionQuantity"]));
                                //recItemProduction.SetDecimal((int)SysCommon.ActItemProdFields.BaseQuantity, Convert.ToDecimal(drow["ActivityItemProductionBaseQuantity"]));
                                //recItemProduction.SetDecimal((int)SysCommon.ActItemProdFields.WeightQuantity, Convert.ToDecimal(drow["ActivityItemProductionWeightQuantity"]));
                                //recItemProduction.SetString((int)SysCommon.ActItemProdFields.Description, drow["ActivityItemProductionDescription"].ToString());
                                //recItemProduction.SetDecimal((int)SysCommon.ActItemProdFields.QuantityExpected, Convert.ToDecimal(drow["ActivityItemProductionQuantityExpected"]));
                                //recItemProduction.SetDecimal((int)SysCommon.ActItemProdFields.BaseQuantityExpected, Convert.ToDecimal(drow["ActivityItemProductionBaseQuantityExpected"]));
                                //recItemProduction.SetDecimal((int)SysCommon.ActItemProdFields.WeightQuantityExpected, Convert.ToDecimal(drow["ActivityItemProductionWeightQuantityExpected"]));
                                //rsItemProduction.Insert(recItemProduction);

                                cmndActItemByProduct.CommandText = "INSERT INTO tActivityItemProductionByProduct " +
                                    "(" +
                                    "ActivityItemRefno, " +
                                    "ActivityItemProductionByProductFKRefno, " +
                                    "ActivityItemProductionByProductStringFKRefno" +
                                    ") VALUES (" +
                                    "@activityItemRefno, " +
                                    "@activityItemProductionByProductFKRefno, " +
                                    "@activityItemProductionByProductStringFKRefno" +
                                    ")";
                                cmndActItemByProduct.CommandType = CommandType.Text;
                                //cmndActItemByProduct.IndexName = "UQ_ActivityItemProductionByProductOnActivityItemProductionByProductFKRefno";
                                cmndActItemByProduct.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemRefno));
                                cmndActItemByProduct.Parameters.Add(new SQLiteParameter("@activityItemProductionByProductFKRefno", actItemProductionFKRefno));
                                cmndActItemByProduct.Parameters.Add(new SQLiteParameter("@activityItemProductionByProductStringFKRefno", actItemProductionFKRefno.ToString()));

                                cmndActItemByProduct.ExecuteNonQuery();

                                //recItemByProduct.SetInt32((int)SysCommon.ActItemProdByProductFields.ActItemRefno, actItemRefno);
                                //recItemByProduct.SetInt32((int)SysCommon.ActItemProdByProductFields.FKRefno, actItemProductionFKRefno);
                                //recItemByProduct.SetString((int)SysCommon.ActItemProdByProductFields.StringFKRefno, actItemProductionFKRefno.ToString());
                                //rsItemByProduct.Insert(recItemByProduct);

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                        //}

                        //                                                        drWhs.Close();
                        //                                                    }
                        //                                                }

                        //                                                drUnit.Close();
                        //                                            }
                        //                                        }

                        //                                        drProduct.Close();
                        //                                    }
                        //                                }

                        //                                drProdUnit.Close();
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product items complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product items failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving ticket add-product items failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="actProductionFKRefno"></param>
        /// <returns></returns>
        protected override string Create(int actProductionFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCTION TICKET BYPRODUCT ITEMS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<refno>" + actProductionFKRefno.ToString() + "</refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
