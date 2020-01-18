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
    /// Defines the <see cref="TableActivityCycleCountItem" />
    /// </summary>
    public class TableActivityCycleCountItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityCycleCountItem"/> class.
        /// </summary>
        public TableActivityCycleCountItem()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityCycleCountItem";
        }

        /// <summary>
        /// The Request
        /// </summary>
        /// <param name="actRefno"></param>
        public override void Request(int actRefno)
        {
            string reqData = string.Empty;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding cycle count items.", true);

                if (actRefno == 0)
                {
                    using (SQLiteCommand cmndCount = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndCount.CommandText = "SELECT tActivity.ActivityRefno FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                        cmndCount.CommandType = CommandType.Text;
                        cmndCount.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts)));

                        using (SQLiteDataReader drCount = cmndCount.ExecuteReader())
                        {
                            while (drCount.Read())
                            {
                                actRefno = Convert.ToInt32(drCount["ActivityRefno"]);
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

                            drCount.Close();
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding cycle count items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding cycle count items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding cycle count items failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding cycle count items failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding cycle count items failed.", ex);
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
                    cmndStatus.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts)));

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} cycle count item(s)", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Cycle count items status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override void Purge()
        {
            // TODO: Resource strings
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle count items.", true);

            try
            {
                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts)));
                    cmndPurge.CommandType = CommandType.Text;

                    cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef "
                        + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
                        + "(SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = @activityItemSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItemCount WHERE tActivityItemCount.ActivityItemSubType = @activityItemSubType";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = @activityItemSubType";
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle count items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle count items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred.Purging cycle count items failed.", ex);
            }
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override string Send(int actRefno)
        {
            string reqData = string.Empty;
            int actCountFKRefno = 0;

            using (SQLiteCommand cmndCount = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndCount.CommandText = "SELECT * FROM tActivityCount WHERE ActivityRefno = @activityRefno";
                cmndCount.CommandType = CommandType.Text;
                //cmndCount.IndexName = "PK_ActivityCountOnActivityRefno";
                cmndCount.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));

                using (SQLiteDataReader drCount = cmndCount.ExecuteReader())
                {
                    //if (drCount.Seek(DbSeekOptions.FirstEqual, new object[] { actRefno }))
                    //{
                    if (drCount.Read())
                    {
                        actCountFKRefno = Convert.ToInt32(drCount.GetValue((int)SysCommon.ActCountFields.FKRefno));
                    }
                    else
                    {
                        throw new Exception("drCount.Read() failed.");
                    }
                    //}
                    //else
                    //{
                    //    // TODO: Resource strings
                    //    throw new Exception(string.Format("Unable to locate the cycle count refno {0} in tActivityCount", actRefno));
                    //}
                }
            }

            reqData = this.SendComm(this.Create(actCountFKRefno));

            return reqData;
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

            using (SQLiteCommand cmndCountItem = conn.CreateCommand())
            using (SQLiteCommand cmndActItem = conn.CreateCommand())
            using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
            using (SQLiteCommand cmndProd = conn.CreateCommand())
            using (SQLiteCommand cmndUnit = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapSlot = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapSect = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle count items.", false);

                        // Receiving schema but no data results in no table being created in the dataset.
                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityItemCount"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndCountItem = conn.CreateCommand())
                            //{
                            //cmndCountItem.CommandText = "tActivityItemCount";
                            //cmndCountItem.CommandType = CommandType.Text;
                            //cmndCountItem.IndexName = "UQ_ActivityItemCountOnActivityItemCountFKRefno";

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
                            //using (SQLiteCommand cmndLocMapSlot = conn.CreateCommand())
                            //{
                            //cmndLocMapSlot.CommandText = "tLocationMapSlot";
                            //cmndLocMapSlot.CommandType = CommandType.Text;
                            //cmndLocMapSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";

                            //using (SQLiteDataReader drLocMapSlot = cmndLocMapSlot.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndLocMapSect = conn.CreateCommand())
                            //{
                            //cmndLocMapSect.CommandText = "tLocationMapSection";
                            //cmndLocMapSect.CommandType = CommandType.Text;
                            //cmndLocMapSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";

                            //using (SQLiteDataReader drLocMapSect = cmndLocMapSect.ExecuteReader())
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
                            //cmndID.CommandText = "SELECT @@IDENTITY";
                            //cmndID.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rsCountItem = cmndCountItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recCountItem = rsCountItem.CreateRecord();

                            int actItemRefno = 0;
                            int progValue = 0;
                            int prodUnitRefno = 0;
                            int locMapRefno = 0;
                            int locMapWhsFKRefno = 0;
                            int locMapSectFKRefno = 0;
                            int locMapSlotFKRefno = 0;
                            int countItemFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityItemCount"].Rows)
                            {
                                // TODO: Resource strings
                                errMsg = "Arguments:";

                                cmndActItem.CommandText = "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (@activityItemSubType, @activityItemStatus, @activityRefno)";
                                cmndActItem.CommandType = CommandType.Text;
                                cmndActItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts)));
                                cmndActItem.Parameters.Add(new SQLiteParameter("@activityItemStatus", Convert.ToInt32(0)));
                                cmndActItem.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));

                                cmndActItem.ExecuteNonQuery();

                                //recActItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.CycleCounts);
                                //recActItem.SetInt32((int)SysCommon.ActItemFields.Status, 0);
                                //recActItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, actRefno);
                                //rsActItem.Insert(recActItem);

                                cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityItem";
                                cmndID.CommandType = CommandType.Text;

                                actItemRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                //prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, ref errMsg, drProd, drUnit, drProdUnit, true);

                                int prodRefno = 0;
                                int prodFKRefno = 0;
                                bool useFNo = true;

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
                                    //// TODO: Resource strings
                                    //throw new Exception("Unable to locate the unit refno from the unit code in  tUnit." + Environment.NewLine + errMsg);
                                    //}
                                }

                                //int prodUnitRefno = 0;
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
                                    //     TODO: Resource strings
                                    //    throw new Exception("Unable to locate the product unit refno from the product fk refno, bc and unit code in tProductUnit." + Environment.NewLine + errMsg);
                                    //}
                                }

                                // Cannot use this.SaveDetailLocation() f00 tags are not the same.
                                locMapRefno = 0;

                                if (locMapRefno == 0)
                                {
                                    locMapSlotFKRefno = Convert.ToInt32(drow["f08"]);

                                    cmndLocMapSlot.CommandText = "SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = @locationMapSlotFKRefno";
                                    cmndLocMapSlot.CommandType = CommandType.Text;
                                    //cmndLocMapSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";
                                    cmndLocMapSlot.Parameters.Add(new SQLiteParameter("@locationMapSlotFKRefno", locMapSlotFKRefno));

                                    using (SQLiteDataReader drLocMapSlot = cmndLocMapSlot.ExecuteReader())
                                    {
                                        //if (drLocMapSlot.Seek(DbSeekOptions.FirstEqual, new object[] { locMapSlotFKRefno }))
                                        //{
                                        if (drLocMapSlot.Read())
                                        {
                                            locMapRefno = Convert.ToInt32(drLocMapSlot.GetValue((int)SysCommon.LocMapSlotFields.LocMapRefno));
                                        }
                                        //    else
                                        //    {
                                        //        throw new Exception("drLocMapSlot.Read() failed.");
                                        //    }
                                        //}
                                    }
                                }

                                if (locMapRefno == 0)
                                {
                                    locMapSectFKRefno = Convert.ToInt32(drow["f07"]);

                                    cmndLocMapSect.CommandText = "SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = @locationMapSectionFKRefno";
                                    cmndLocMapSect.CommandType = CommandType.Text;
                                    //cmndLocMapSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";
                                    cmndLocMapSect.Parameters.Add(new SQLiteParameter("@locationMapSectionFKRefno", locMapSectFKRefno));

                                    using (SQLiteDataReader drLocMapSect = cmndLocMapSect.ExecuteReader())
                                    {
                                        //if (drLocMapSect.Seek(DbSeekOptions.FirstEqual, new object[] { locMapSectFKRefno }))
                                        //{
                                        if (drLocMapSect.Read())
                                        {
                                            locMapRefno = Convert.ToInt32(drLocMapSect.GetValue((int)SysCommon.LocMapSectFields.LocMapRefno));
                                        }
                                        //else
                                        //{
                                        //    throw new Exception("drLocMapSection.Read() failed.");
                                        //}
                                        //}
                                    }
                                }

                                if (locMapRefno == 0)
                                {
                                    locMapWhsFKRefno = Convert.ToInt32(drow["f06"]);

                                    cmndLocMapWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                                    cmndLocMapWhs.CommandType = CommandType.Text;
                                    //cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                                    cmndLocMapWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", locMapWhsFKRefno));

                                    using (SQLiteDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
                                    {
                                        //if (drLocMapWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapWhsFKRefno }))
                                        //{
                                        if (drLocMapWhs.Read())
                                        {
                                            locMapRefno = Convert.ToInt32(drLocMapWhs.GetValue((int)SysCommon.LocMapWhsFields.LocMapRefno));
                                        }
                                        //    else
                                        //    {
                                        //        throw new Exception("drLocMapWhs.Read() failed.");
                                        //    }
                                        //}
                                    }
                                }

                                if (locMapRefno == 0)
                                {
                                    throw new Exception(string.Format("Unable to indentify the location map refno.{0}Whs FK Refno {1}{2} Section FK Refno {3}{4} Slot FK Refno {5}",
                                        Environment.NewLine,
                                        locMapWhsFKRefno,
                                        Environment.NewLine,
                                        locMapSectFKRefno,
                                        Environment.NewLine,
                                        locMapSlotFKRefno));
                                }

                                countItemFKRefno = Convert.ToInt32(drow["f02"]);
                                errMsg += Environment.NewLine + "Item fk refno " + countItemFKRefno;

                                cmndCountItem.CommandText = "INSERT INTO tActivityItemCount (" +
                                "ActivityItemRefno, " +
                                "ActivityItemSubType, " +
                                "ActivityItemCountFKRefno, " +
                                "ActivityItemCountStringFKRefno, " +
                                "ProductUnitRefno, " +
                                "LocationMapRefno, " +
                                "ActivityItemCountQuantityOnHand, " +
                                "ActivityItemCountBaseQuantityOnHand, " +
                                "ActivityItemCountWeightQuantityOnHand, " +
                                "ActivityItemCountQuantity, " +
                                "ActivityItemCountBaseQuantity, " +
                                "ActivityItemCountWeightQuantity" +
                                ") VALUES (" +
                                "@activityItemRefno, " +
                                "@activityItemSubType, " +
                                "@activityItemCountFKRefno, " +
                                "@activityItemCountStringFKRefno, " +
                                "@productUnitRefno, " +
                                "@locationMapRefno, " +
                                "@activityItemCountQuantityOnHand, " +
                                "@activityItemCountBaseQuantityOnHand, " +
                                "@activityItemCountWeightQuantityOnHand, " +
                                "@activityItemCountQuantity, " +
                                "@activityItemCountBaseQuantity, " +
                                "@activityItemCountWeightQuantity" +
                                ")";
                                cmndCountItem.CommandType = CommandType.Text;
                                //cmndCountItem.IndexName = "UQ_ActivityItemCountOnActivityItemCountFKRefno";
                                cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemRefno));
                                cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts)));
                                cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountFKRefno", countItemFKRefno));
                                cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountStringFKRefno", Convert.ToString(countItemFKRefno)));
                                cmndCountItem.Parameters.Add(new SQLiteParameter("@productUnitRefno", prodUnitRefno));
                                cmndCountItem.Parameters.Add(new SQLiteParameter("@locationMapRefno", locMapRefno));
                                cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountQuantityOnHand", Convert.ToDecimal(drow["f09"])));
                                cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountBaseQuantityOnHand", Convert.ToDecimal(drow["f10"])));
                                cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountWeightQuantityOnHand", Convert.ToDecimal(drow["f11"])));
                                cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountQuantity", Convert.ToDecimal(drow["f12"])));
                                cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountBaseQuantity", Convert.ToDecimal(drow["f13"])));
                                cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountWeightQuantity", Convert.ToDecimal(drow["f14"])));

                                cmndCountItem.ExecuteNonQuery();

                                //recCountItem.SetInt32((int)SysCommon.ActItemCountFields.ActItemRefno, actItemRefno);
                                //recCountItem.SetInt32((int)SysCommon.ActItemCountFields.ActItemSubType, (int)SysCommon.ActSubTypes.CycleCounts);
                                //recCountItem.SetInt32((int)SysCommon.ActItemCountFields.FKRefno, countItemFKRefno);
                                //recCountItem.SetString((int)SysCommon.ActItemCountFields.StringFKRefno, Convert.ToString(countItemFKRefno));
                                //recCountItem.SetInt32((int)SysCommon.ActItemCountFields.LocMapRefno, locMapRefno);
                                //recCountItem.SetInt32((int)SysCommon.ActItemCountFields.ProdUnitRefno, prodUnitRefno);
                                //recCountItem.SetDecimal((int)SysCommon.ActItemCountFields.QuantityOnHand, Convert.ToDecimal(drow["f09"]));
                                //recCountItem.SetDecimal((int)SysCommon.ActItemCountFields.BaseQuantityOnHand, Convert.ToDecimal(drow["f10"]));
                                //recCountItem.SetDecimal((int)SysCommon.ActItemCountFields.WeightQuantityOnHand, Convert.ToDecimal(drow["f11"]));
                                //recCountItem.SetDecimal((int)SysCommon.ActItemCountFields.Quantity, Convert.ToDecimal(drow["f12"]));
                                //recCountItem.SetDecimal((int)SysCommon.ActItemCountFields.BaseQuantity, Convert.ToDecimal(drow["f13"]));
                                //recCountItem.SetDecimal((int)SysCommon.ActItemCountFields.WeightQuantity, Convert.ToDecimal(drow["f14"]));

                                //rsCountItem.Insert(recCountItem);

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                        //}

                        //drLocMapWhs.Close();
                        //}
                        //}

                        //drLocMapSect.Close();
                        //}
                        //}

                        //drLocMapSlot.Close();
                        //}
                        //}

                        //drUnit.Close();
                        //}
                        //}

                        //drProd.Close();
                        //}
                        //}

                        //drProdUnit.Close();
                        //                    }
                        //                }
                        //            }
                        //        }
                        //}
                        //}

                        transaction.Commit();

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle count items complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle count items failed.", true);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving cycle count items failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="actCountFKRefno"></param>
        /// <returns></returns>
        protected override string Create(int actCountFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST CYCLE COUNT ITEMS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<refno>" + actCountFKRefno.ToString() + "</refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
