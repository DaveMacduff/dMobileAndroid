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
    /// Defines the <see cref="TableActivityProductionTicketMaterialItem" />
    /// </summary>
    public class TableActivityProductionTicketMaterialItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketMaterialItem"/> class.
        /// </summary>
        public TableActivityProductionTicketMaterialItem()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketMaterialItem";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket material items.", true);

                if (actRefno == 0)
                {
                    using (SQLiteCommand cmndActivity = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndActivity.CommandText = "SELECT tActivity.ActivityRefno FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                        cmndActivity.CommandType = CommandType.Text;
                        cmndActivity.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial)));

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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket material items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket material items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding ticket material items failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket material items failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding ticket material items failed.", ex);
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
                    cmndStatus.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial)));

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} ticket material items.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Material ticket items status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override void Purge()
        {
            // TODO: Resource strings
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket material items.", true);

            try
            {
                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial)));

                    cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef "
                        + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
                        + "(SELECT tActivityItem.ActivityItemRefno "
                        + "FROM tActivityItem "
                        + "WHERE tActivityItem.ActivityItemSubType = @activityItemSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItemProductionMaterial "
                        + "WHERE tActivityItemProductionMaterial.ActivityItemRefno IN "
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket material items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket material items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging ticket material items failed.", ex);
            }
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override string Send(int actRefno)
        {
            int actProductionFKRefno = 0;

            using (SQLiteCommand cmndActMaterial = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndActMaterial.CommandText = "SELECT * FROM tActivityProductionMaterial WHERE ActivityRefno = @activityRefno";
                cmndActMaterial.CommandType = CommandType.Text;
                //cmndActMaterial.IndexName = "PK_ActivityProductionMaterialOnActivityRefno";
                cmndActMaterial.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));

                using (SQLiteDataReader drActMaterial = cmndActMaterial.ExecuteReader())
                {
                    //if (drActMaterial.Seek(DbSeekOptions.FirstEqual, new object[] { actRefno }))
                    //{
                    if (drActMaterial.Read())
                    {
                        actProductionFKRefno = drActMaterial.GetInt32((int)SysCommon.ActProdMaterialFields.FKRefno);
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception("drActMaterial.Read() failed.");
                    }
                    //}
                    //else
                    //{
                    //    // TODO: Resource strings
                    //    throw new Exception(string.Format("Unable to locate the activity production ticket material refno {0} in tActivityProductionMaterial", actRefno));
                    //}
                }
            }

            return this.SendComm(this.Create(actProductionFKRefno));
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

            using (SQLiteCommand cmndItemProduction = conn.CreateCommand())
            using (SQLiteCommand cmndItem = conn.CreateCommand())
            using (SQLiteCommand cmndItemMaterial = conn.CreateCommand())
            using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
            using (SQLiteCommand cmndProduct = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket material items.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityItemProduction"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndItemProduction = conn.CreateCommand())
                            //{
                            //cmndItemProduction.CommandText = "tActivityItemProduction";
                            //cmndItemProduction.CommandType = CommandType.Text;
                            //cmndItemProduction.IndexName = "PK_ActivityItemProductionOnActivityItemRefno";

                            //using (SQLiteCommand cmndItem = conn.CreateCommand())
                            //{
                            //cmndItem.CommandText = "tActivityItem";
                            //cmndItem.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rsActItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recActItem = rsActItem.CreateRecord();

                            //using (SQLiteCommand cmndItemMaterial = conn.CreateCommand())
                            //{
                            //cmndItemMaterial.CommandText = "tActivityItemProductionMaterial";
                            //cmndItemMaterial.CommandType = CommandType.Text;
                            //cmndItemMaterial.IndexName = "UQ_ActivityItemProductionMaterialOnActivityItemProductionMaterialFKRefno";

                            //using (SqlCeResultSet rsActItemMaterial = cmndItemMaterial.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recActItemMaterial = rsActItemMaterial.CreateRecord();

                            //using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
                            //{
                            //cmndProdUnit.CommandText = "tProductUnit";
                            //cmndProdUnit.CommandType = CommandType.Text;
                            //cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

                            //using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndProduct = conn.CreateCommand())
                            //{
                            //cmndProduct.CommandText = "tProduct";
                            //cmndProduct.CommandType = CommandType.Text;
                            //cmndProduct.IndexName = "UQ_ProductOnProductFKRefno";

                            //using (SQLiteDataReader drProd = cmndProduct.ExecuteReader())
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
                            //using (SQLiteCommand cmndID = conn.CreateCommand())
                            //{
                            //cmndID.CommandText = "SELECT last_insert_rowid()";
                            //cmndID.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rsActItemProduction = cmndItemProduction.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recActItemProduction = rsActItemProduction.CreateRecord();
                            int actItemRefno = 0;
                            int progValue = 0;
                            int prodUnitRefno = 0;
                            int locMapRefno = 0;
                            int actItemProductionFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityItemProduction"].Rows)
                            {
                                errMsg = "Arguments:";

                                cmndItem.CommandText = "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityItemRefno) VALUES (@activityItemSubType, @activityItemStatus, @activityItemRefno)";
                                cmndItem.CommandType = CommandType.Text;
                                cmndItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial)));
                                cmndItem.Parameters.Add(new SQLiteParameter("@activityItemStatus", Convert.ToInt32(SysCommon.ActStatus.NoChange)));
                                cmndItem.Parameters.Add(new SQLiteParameter("@activityItemRefno", actRefno));

                                cmndItem.ExecuteNonQuery();

                                //recActItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.TicketMaterial);
                                //recActItem.SetInt32((int)SysCommon.ActItemFields.Status, (int)SysCommon.ActStatus.NoChange);
                                //recActItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, actRefno);
                                //rsActItem.Insert(recActItem);

                                cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityItem";
                                cmndID.CommandType = CommandType.Text;

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

                                cmndProduct.CommandText = "SELECT * FROM tProduct WHERE ProductFKRefno = @productFKRefno";
                                cmndProduct.CommandType = CommandType.Text;
                                //cmndProduct.IndexName = "UQ_ProductOnProductFKRefno";
                                cmndProduct.Parameters.Add(new SQLiteParameter("@productFKRefno", prodFKRefno));

                                using (SQLiteDataReader drProd = cmndProduct.ExecuteReader())
                                {
                                    //if (drProd.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno }))
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

                                cmndLocMapWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                                cmndLocMapWhs.CommandType = CommandType.Text;
                                //cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                                cmndLocMapWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", Convert.ToInt32(drow["LocationMapWarehouseFKRefno"])));

                                using (SQLiteDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
                                {
                                    locMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, drLocMapWhs);
                                }

                                cmndItemProduction.CommandText = "INSERT INTO tActivityItemProduction (" +
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
                                "ActivityItemProductionWeigthtQuantityExpected" +
                                ") VALUES (" +
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
                                "@activityItemProductionWeigthtQuantityExpected" +
                                ")";
                                cmndItemProduction.CommandType = CommandType.Text;
                                //cmndItemProduction.IndexName = "PK_ActivityItemProductionOnActivityItemRefno";
                                cmndItemProduction.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemRefno));
                                cmndItemProduction.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial)));
                                cmndItemProduction.Parameters.Add(new SQLiteParameter("@productUnitRefno", prodUnitRefno));
                                cmndItemProduction.Parameters.Add(new SQLiteParameter("@locationMapRefno", locMapRefno));
                                cmndItemProduction.Parameters.Add(new SQLiteParameter("@activityItemProductionQuantity", Convert.ToDecimal(drow["ActivityItemProductionQuantity"])));
                                cmndItemProduction.Parameters.Add(new SQLiteParameter("@activityItemProductionBaseQuantity", Convert.ToDecimal(drow["ActivityItemProductionBaseQuantity"])));
                                cmndItemProduction.Parameters.Add(new SQLiteParameter("@activityItemProductionWeightQuantity", Convert.ToDecimal(drow["ActivityItemProductionWeightQuantity"])));
                                cmndItemProduction.Parameters.Add(new SQLiteParameter("@activityItemProductionDescription", drow["ActivityItemProductionDescription"].ToString()));
                                cmndItemProduction.Parameters.Add(new SQLiteParameter("@activityItemProductionQuantityExpected", Convert.ToDecimal(drow["ActivityItemProductionQuantityExpected"])));
                                cmndItemProduction.Parameters.Add(new SQLiteParameter("@activityItemProductionBaseQuantityExpected", Convert.ToDecimal(drow["ActivityItemProductionBaseQuantityExpected"])));
                                cmndItemProduction.Parameters.Add(new SQLiteParameter("@activityItemProductionWeigthtQuantityExpected", Convert.ToDecimal(drow["ActivityItemProductionWeightQuantityExpected"])));

                                cmndItemProduction.ExecuteNonQuery();

                                //recActItemProduction.SetInt32((int)SysCommon.ActItemProdFields.ActItemRefno, actItemRefno);
                                //recActItemProduction.SetInt32((int)SysCommon.ActItemProdFields.ActivityItemSubType, (int)SysCommon.ActSubTypes.TicketMaterial);
                                //recActItemProduction.SetInt32((int)SysCommon.ActItemProdFields.ProdUnitRefno, prodUnitRefno);
                                //recActItemProduction.SetInt32((int)SysCommon.ActItemProdFields.LocMapRefno, locMapRefno);
                                //recActItemProduction.SetDecimal((int)SysCommon.ActItemProdFields.Quantity, Convert.ToDecimal(drow["ActivityItemProductionQuantity"]));
                                //recActItemProduction.SetDecimal((int)SysCommon.ActItemProdFields.BaseQuantity, Convert.ToDecimal(drow["ActivityItemProductionBaseQuantity"]));
                                //recActItemProduction.SetDecimal((int)SysCommon.ActItemProdFields.WeightQuantity, Convert.ToDecimal(drow["ActivityItemProductionWeightQuantity"]));
                                //recActItemProduction.SetString((int)SysCommon.ActItemProdFields.Description, drow["ActivityItemProductionDescription"].ToString());
                                //recActItemProduction.SetDecimal((int)SysCommon.ActItemProdFields.QuantityExpected, Convert.ToDecimal(drow["ActivityItemProductionQuantityExpected"]));
                                //recActItemProduction.SetDecimal((int)SysCommon.ActItemProdFields.BaseQuantityExpected, Convert.ToDecimal(drow["ActivityItemProductionBaseQuantityExpected"]));
                                //recActItemProduction.SetDecimal((int)SysCommon.ActItemProdFields.WeightQuantityExpected, Convert.ToDecimal(drow["ActivityItemProductionWeightQuantityExpected"]));
                                //rsActItemProduction.Insert(recActItemProduction);

                                actItemProductionFKRefno = Convert.ToInt32(drow["ActivityItemProductionFKRefno"]);
                                errMsg += Environment.NewLine + "Item fk refno" + actItemProductionFKRefno;

                                cmndItemMaterial.CommandText = "INSERT INTO tActivityItemProductionMaterial (" +
                                    "ActivityItemRefno, " +
                                    "ActivityItemProductionMaterialFKRefno, " +
                                    "ActivityItemProductionMaterialStringFKRefno" +
                                    ") VALUES (" +
                                    "@activityItemRefno, " +
                                    "@activityItemProductionMaterialFKRefno, " +
                                    "@activityItemProductionMaterialStringFKRefno" +
                                    ") ";
                                cmndItemMaterial.CommandType = CommandType.Text;
                                //cmndItemMaterial.IndexName = "UQ_ActivityItemProductionMaterialOnActivityItemProductionMaterialFKRefno";
                                cmndItemMaterial.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemRefno));
                                cmndItemMaterial.Parameters.Add(new SQLiteParameter("@activityItemProductionMaterialFKRefno", actItemProductionFKRefno));
                                cmndItemMaterial.Parameters.Add(new SQLiteParameter("@activityItemProductionMaterialStringFKRefno", actItemProductionFKRefno.ToString()));

                                cmndItemMaterial.ExecuteNonQuery();

                                //recActItemMaterial.SetInt32((int)SysCommon.ActItemProdMaterialFields.ActItemRefno, actItemRefno);
                                //recActItemMaterial.SetInt32((int)SysCommon.ActItemProdMaterialFields.FKRefno, actItemProductionFKRefno);
                                //recActItemMaterial.SetString((int)SysCommon.ActItemProdMaterialFields.StringFKRefno, actItemProductionFKRefno.ToString());
                                //rsActItemMaterial.Insert(recActItemMaterial);

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                        //}

                        //                                                        drLocMapWhs.Close();
                        //                                                    }
                        //                                                }

                        //                                                drUnit.Close();
                        //                                            }
                        //                                        }

                        //                                        drProd.Close();
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket material items complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket material items failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving ticket material items failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="actProductionFKRefno">The actProductionFKRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override string Create(int actProductionFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCTION TICKET MATERIAL ITEMS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<refno>" + actProductionFKRefno + "</refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
