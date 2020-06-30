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
    /// Defines the <see cref="TableActivityProductionTicketFinishedDetail" />
    /// </summary>
    public class TableActivityProductionTicketFinishedDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketFinishedDetail"/> class.
        /// </summary>
        public TableActivityProductionTicketFinishedDetail()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketFinishedDetail";
        }

        /// <summary>
        /// The Request
        /// </summary>
        /// <param name="actItemRefno"></param>
        public override void Request(int actItemRefno)
        {
            string reqData = string.Empty;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket details.", true);

                if (actItemRefno == 0)
                {
                    using (SQLiteCommand cmndActItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndActItem.CommandText = "SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = @activityItemSubType";
                        cmndActItem.CommandType = CommandType.Text;
                        cmndActItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished)));

                        using (SQLiteDataReader drActItem = cmndActItem.ExecuteReader())
                        {
                            while (drActItem.Read())
                            {
                                actItemRefno = Convert.ToInt32(drActItem["ActivityItemRefno"]);
                                reqData = this.Send(actItemRefno);

                                if (0 < reqData.Length)
                                {
                                    if (reqData.Contains("<message>ERROR</message>"))
                                    {
                                        XmlDocument reqErr = new XmlDocument();
                                        reqErr.LoadXml(reqData);
                                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                                    }

                                    this.Save(reqData, actItemRefno);
                                }
                            }

                            drActItem.Close();
                        }
                    }
                }
                else
                {
                    reqData = this.Send(actItemRefno);

                    if (0 < reqData.Length)
                    {
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            XmlDocument reqErr = new XmlDocument();
                            reqErr.LoadXml(reqData);
                            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                        }

                        this.Save(reqData, actItemRefno);
                    }
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding finished ticket details failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket details failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding finished ticket details failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket details.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished)));

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef"
                        + " WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN"
                        + " (SELECT tActivityDetail.ActivityDetailRefno"
                        + " FROM tActivityDetail"
                        + " WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailProductionFinished"
                        + " WHERE tActivityDetailProductionFinished.ActivityDetailRefno IN"
                        + " (SELECT tActivityDetail.ActivityDetailRefno"
                        + " FROM tActivityDetail"
                        + " WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailExtra"
                        + " WHERE tActivityDetailExtra.ActivityDetailRefno IN"
                        + " (SELECT tActivityDetail.ActivityDetailRefno"
                        + " FROM tActivityDetail"
                        + " WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType";
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket details.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging finished ticket details failed.", ex);
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
                    cmndStatus.CommandText = "SELECT COUNT(tActivityDetail.ActivityDetailRefno) FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType";
                    cmndStatus.CommandType = CommandType.Text;
                    cmndStatus.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished)));

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} finished ticket details.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Finished ticket details status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actItemRefno"></param>
        /// <returns></returns>
        protected override string Send(int actItemRefno)
        {
            int actItemProductionFKRefno = 0;

            using (SQLiteCommand cmndItemFinished = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndItemFinished.CommandText = "SELECT * FROM tActivityItemProductionFinished WHERE ActivityItemRefno = @activityItemRefno";
                cmndItemFinished.CommandType = CommandType.Text;
                //cmndItemFinished.IndexName = "PK_ActivityItemProductionFinishedOnActivityItemRefno";
                cmndItemFinished.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemRefno));

                using (SQLiteDataReader drItemFinished = cmndItemFinished.ExecuteReader())
                {
                    //if (drItemFinished.Seek(DbSeekOptions.FirstEqual, new object[] { actItemRefno }))
                    //{
                    if (drItemFinished.Read())
                    {
                        actItemProductionFKRefno = drItemFinished.GetInt32((int)SysCommon.ActItemProdFinishedFields.FKRefno);
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception("drItemFinished.Read() failed.");
                    }
                    //}
                    //else
                    //{
                    //    // TODO: Resource strings
                    //    throw new Exception(string.Format("Unable to locate the activity item ticket finished {0} in tActivityItemProductionFinished.", actItemRefno));
                    //}
                }
            }

            return this.SendComm(this.Create(actItemProductionFKRefno));
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actItemProductionRefno"></param>
        protected override void Save(string reqData, int actItemProductionRefno)
        {
            string errMsg = string.Empty;
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            SQLiteCommand cmndExtra = conn.CreateCommand();

            using (SQLiteCommand cmndDetailFinished = conn.CreateCommand())
            using (SQLiteCommand cmndDetail = conn.CreateCommand())
            using (SQLiteCommand cmndItemProduction = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapSlot = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapSect = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
            using (SQLiteCommand cmndSchemeVar = conn.CreateCommand())
            using (SQLiteCommand cmndSchemeVarXRef = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished ticket details.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailProduction"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndDetailFinished = conn.CreateCommand())
                            //{
                            //cmndDetailFinished.CommandText = "tActivityDetailProductionFinished";
                            //cmndDetailFinished.CommandType = CommandType.Text;
                            //cmndDetailFinished.IndexName = "UQ_ActivityDetailProductionFinishedOnActivityDetailProductionFinishedFKRefno";

                            //using (SqlCeResultSet rsDetailFinished = cmndDetailFinished.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recDetailFinished = rsDetailFinished.CreateRecord();

                            //using (SQLiteCommand cmndDetail = conn.CreateCommand())
                            //{
                            //cmndDetail.CommandText = "tActivityDetail";
                            //cmndDetail.CommandType = CommandType.Text;
                            //cmndDetail.IndexName = "PK_ActivityDetailOnActivityDetailRefno";

                            //using (SqlCeResultSet rsDetail = cmndDetail.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recDetail = rsDetail.CreateRecord();

                            //using (SQLiteCommand cmndExtra = conn.CreateCommand())
                            //{
                            //cmndExtra.CommandText = "tActivityDetailExtra";
                            //cmndExtra.CommandType = CommandType.Text;
                            //cmndExtra.IndexName = "PK_ActivityDetailExtraOnActivityDetailRefno";

                            //using (SqlCeResultSet rsExtra = cmndExtra.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recExtra = rsExtra.CreateRecord();

                            //using (SQLiteCommand cmndItemProduction = conn.CreateCommand())
                            //{
                            //cmndItemProduction.CommandText = "tActivityItemProduction";
                            //cmndItemProduction.CommandType = CommandType.Text;
                            //cmndItemProduction.IndexName = "PK_ActivityItemProductionOnActivityItemRefno";

                            //using (SQLiteDataReader drItemProduction = cmndItemProduction.ExecuteReader())
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
                            //using (SQLiteCommand cmndSchemeVar = conn.CreateCommand())
                            //{
                            //cmndSchemeVar.CommandText = "tProductSchemeVariant";
                            //cmndSchemeVar.CommandType = CommandType.Text;
                            //cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";

                            //using (SQLiteDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndSchemeVarXRef = conn.CreateCommand())
                            //{
                            //cmndSchemeVarXRef.CommandText = "tActivityDetailProductSchemeVariantXRef";
                            //cmndSchemeVarXRef.CommandType = CommandType.Text;
                            //cmndSchemeVarXRef.IndexName = "PK_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefnoAndProductSchemeVariantRefno";

                            //using (SqlCeResultSet rsSchemeVarXRef = cmndSchemeVarXRef.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recSchemeVarXRef = rsSchemeVarXRef.CreateRecord();

                            //using (SQLiteCommand cmndID = conn.CreateCommand())
                            //{
                            //cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityDetail";
                            //cmndID.CommandType = CommandType.Text;

                            int actDetailRefno = 0;
                            int progValue = 0;
                            int actDetailFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            cmndItemProduction.CommandText = "SELECT * FROM tActivityItemProduction WHERE ActivityItemRefno = @activityItemRefno";
                            cmndItemProduction.CommandType = CommandType.Text;
                            //cmndItemProduction.IndexName = "PK_ActivityItemProductionOnActivityItemRefno";
                            cmndItemProduction.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemProductionRefno));

                            using (SQLiteDataReader drItemProduction = cmndItemProduction.ExecuteReader())
                            {
                                //if (drItemProduction.Seek(DbSeekOptions.FirstEqual, new object[] { actItemProductionRefno }))
                                //{
                                if (!drItemProduction.Read())
                                {
                                    // TODO: Resource strings
                                    throw new Exception("drItemProduction.Read() failed." + Environment.NewLine + errMsg);
                                }
                                //}
                                //else
                                //{
                                //    // TODO: Resource strings
                                //    throw new Exception(string.Format("Unable to locate the activity item refno {0} in tActivityItemProduction", actItemProductionRefno));
                                //}

                                foreach (DataRow drow in ds.Tables["tActivityDetailProduction"].Rows)
                                {
                                    // TODO: Resource strings
                                    errMsg = "Arguments:";

                                    int locMapRefno = 0;
                                    int locMapWhsFKRefno = 0;
                                    int locMapSectFKRefno = 0;
                                    int locMapSlotFKRefno = 0;
                                    bool useFNo = false;
                                    string emptyDate = "01-01-1900";

                                    if (useFNo)
                                    {
                                        locMapWhsFKRefno = Convert.ToInt32(drow["f07"]);
                                        locMapSectFKRefno = Convert.ToInt32(drow["f08"]);
                                        locMapSlotFKRefno = Convert.ToInt32(drow["f09"]);
                                    }
                                    else
                                    {
                                        locMapWhsFKRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);
                                        locMapSectFKRefno = Convert.ToInt32(drow["LocationMapSectionFKRefno"]);
                                        locMapSlotFKRefno = Convert.ToInt32(drow["LocationMapSlotFKRefno"]);
                                    }

                                    if (locMapRefno == 0)
                                    {
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
                                            //else
                                            //{
                                            //    // TODO: Resource strings
                                            //    throw new Exception("drLocMapSlot.Read() failed.");
                                            //}
                                            //}
                                        }
                                    }

                                    if (locMapRefno == 0)
                                    {
                                        cmndLocMapSect.CommandText = "SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = @locationMapSectionFKRefno";
                                        cmndLocMapSect.CommandType = CommandType.Text;
                                        //cmndLocMapSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";
                                        cmndLocMapSect.Parameters.Add(new SQLiteParameter("@locationMapSectionFKRefno", locMapSectFKRefno));

                                        using (SQLiteDataReader drLocMapSect = cmndLocMapSect.ExecuteReader())
                                        {
                                            //if (drLocMapSection.Seek(DbSeekOptions.FirstEqual, new object[] { locMapSectFKRefno }))
                                            //{
                                            if (drLocMapSect.Read())
                                            {
                                                locMapRefno = Convert.ToInt32(drLocMapSect.GetValue((int)SysCommon.LocMapSectFields.LocMapRefno));
                                            }
                                            //else
                                            //{
                                            //    // TODO: Resource strings
                                            //    throw new Exception("drLocMapSection.Read() failed.");
                                            //}
                                            //}
                                        }
                                    }

                                    if (locMapRefno == 0)
                                    {
                                        cmndLocMapWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapFKRefno = @locationMapFKRefno";
                                        cmndLocMapWhs.CommandType = CommandType.Text;
                                        //cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                                        cmndLocMapWhs.Parameters.Add(new SQLiteParameter("@locationMapFKRefno", locMapWhsFKRefno));

                                        using (SQLiteDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
                                        {
                                            //if (drLocMapWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapWhsFKRefno }))
                                            //{
                                            if (drLocMapWhs.Read())
                                            {
                                                locMapRefno = Convert.ToInt32(drLocMapWhs.GetValue((int)SysCommon.LocMapWhsFields.LocMapRefno));
                                            }
                                            //else
                                            //{
                                            //// TODO: Resource strings
                                            //throw new Exception("drLocMapWhs.Read() failed.");
                                            //}
                                            //}
                                        }
                                    }

                                    if (locMapRefno == 0)
                                    {
                                        throw new Exception(string.Format("Unable to identify the location map refno. {0}Whs FK Refno {1}{2}Section FK Refno {3}{4} Slot FK Refno {5}{6}",
                                            Environment.NewLine,
                                            locMapWhsFKRefno,
                                            Environment.NewLine,
                                            locMapSectFKRefno,
                                            Environment.NewLine,
                                            locMapSlotFKRefno));
                                    }

                                    cmndDetail.CommandText = "INSERT INTO tActivityDetail " +
                                    "(" +
                                    "ActivityDetailSubType, " +
                                    "ActivityDetailStatus, " +
                                    "ActivityItemRefno, " +
                                    "LocationMapRefno, " +
                                    "ProductUnitRefno, " +
                                    "ActivityDetailUniqueSchemeID, " +
                                    "ActivityDetailSchemeID, " +
                                    "ActivityDetailQuantity, " +
                                    "ActivityDetailBaseQuantity, " +
                                    "ActivityDetailWeightQuantity, " +
                                    "ActivityDetailSingleTradeItemSerialNumber, " +
                                    "ActivityDetailLogisticUnitBatchLotNumber, " +
                                    "ActivityDetailLogisticUnitBestBeforeSellByDate, " +
                                    "ActivityDetailLogisticUnitExpiryUseByDate, " +
                                    "ActivityDetailLogisticUnitProductionDate, " +
                                    "ActivityDetailLogisticUnitDueDate, " +
                                    "ActivityDetailLogisticUnitPackagingDate, " +
                                    "ActivityDetailLogisticUnitSkidNumber" +
                                    ") " +
                                    "VALUES" +
                                    "(" +
                                    "@activityDetailSubType, " +
                                    "@activityDetailStatus, " +
                                    "@activityItemRefno, " +
                                    "@locationMapRefno, " +
                                    "@productUnitRefno, " +
                                    "@activityDetailUniqueSchemeID, " +
                                    "@activityDetailSchemeID, " +
                                    "@activityDetailQuantity, " +
                                    "@activityDetailBaseQuantity, " +
                                    "@activityDetailWeightQuantity, " +
                                    "@activityDetailSingleTradeItemSerialNumber, " +
                                    "@activityDetailLogisticUnitBatchLotNumber, " +
                                    "@activityDetailLogisticUnitBestBeforeSellByDate, " +
                                    "@activityDetailLogisticUnitExpiryUseByDate, " +
                                    "@activityDetailLogisticUnitProductionDate, " +
                                    "@activityDetailLogisticUnitDueDate, " +
                                    "@activityDetailLogisticUnitPackagingDate, " +
                                    "@activityDetailLogisticUnitSkidNumber" +
                                    ")";
                                    cmndDetail.CommandType = CommandType.Text;
                                    //cmndDetail.IndexName = "PK_ActivityDetailOnActivityDetailRefno";
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished)));
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailStatus", Convert.ToInt32(0)));
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemProductionRefno));
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@locationMapRefno", locMapRefno));
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@productUnitRefno", drItemProduction.GetInt32((int)SysCommon.ActItemProdFields.ProdUnitRefno)));

                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.Status, 0);
                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.SubType, (int)SysCommon.ActSubTypes.TicketFinished);
                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.ActivityItemRefno, actItemProductionRefno);
                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.ProductUnitRefno, drItemProduction.GetInt32((int)SysCommon.ActItemProdFields.ProdUnitRefno));

                                    //this.SaveDetailLocation(drow, drLocMapSlot, drLocMapSect, drLocMapWhs, /*ref recDetail, */ false);
                                    //this.SaveDetail(drow, ref recDetail, false);

                                    if (useFNo)
                                    {
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailUniqueSchemeID", drow["f05"].ToString()));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailSchemeID", drow["f06"].ToString()));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailQuantity", Convert.ToDecimal(drow["f02"])));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailBaseQuantity", Convert.ToDecimal(drow["f03"])));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailWeightQuantity", Convert.ToDecimal(drow["f04"])));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailSingleTradeItemSerialNumber", drow["f10"].ToString()));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitBatchLotNumber", drow["f11"].ToString()));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitSkidNumber", drow["f18"].ToString()));

                                        //recDetail.SetString((int)SysCommon.ActDetailFields.UniqueSchemeID, drow["f05"].ToString());
                                        //recDetail.SetString((int)SysCommon.ActDetailFields.SchemeID, drow["f06"].ToString());
                                        //recDetail.SetDecimal((int)SysCommon.ActDetailFields.Quantity, Convert.ToDecimal(drow["f02"]));
                                        //recDetail.SetDecimal((int)SysCommon.ActDetailFields.BaseQuantity, Convert.ToDecimal(drow["f03"]));
                                        //recDetail.SetDecimal((int)SysCommon.ActDetailFields.WeightQuantity, Convert.ToDecimal(drow["f04"]));
                                        //recDetail.SetString((int)SysCommon.ActDetailFields.SingleTradeItemSerialNumber, drow["f10"].ToString());
                                        //recDetail.SetString((int)SysCommon.ActDetailFields.LogisticUnitBatchLotNumber, drow["f11"].ToString());
                                        //recDetail.SetString((int)SysCommon.ActDetailFields.LogisticUnitSkidNumber, drow["f18"].ToString());

                                        if (string.IsNullOrEmpty(drow["f13"].ToString()))
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitBestBeforeSellByDate", DateTime.Parse(emptyDate)));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitBestBeforeSellByDate, DateTime.Parse(emptyDate));
                                        }
                                        else
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitBestBeforeSellByDate", Convert.ToDateTime(drow["f13"])));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitBestBeforeSellByDate, Convert.ToDateTime(drow["f13"]));
                                        }

                                        if (string.IsNullOrEmpty(drow["f14"].ToString()))
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitExpiryUseByDate", DateTime.Parse(emptyDate)));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitExpiryUseByDate, DateTime.Parse(emptyDate));
                                        }
                                        else
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitExpiryUseByDate", Convert.ToDateTime(drow["f14"])));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitExpiryUseByDate, Convert.ToDateTime(drow["f14"]));
                                        }

                                        if (string.IsNullOrEmpty(drow["f12"].ToString()))
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitProductionDate", DateTime.Parse(emptyDate)));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitProductionDate, DateTime.Parse(emptyDate));
                                        }
                                        else
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitProductionDate", Convert.ToDateTime(drow["f12"])));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitProductionDate, Convert.ToDateTime(drow["f12"]));
                                        }

                                        if (string.IsNullOrEmpty(drow["f15"].ToString()))
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitDueDate", DateTime.Parse(emptyDate)));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitDueDate, DateTime.Parse(emptyDate));
                                        }
                                        else
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitDueDate", Convert.ToDateTime(drow["f15"])));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitDueDate, Convert.ToDateTime(drow["f15"]));
                                        }

                                        if (string.IsNullOrEmpty(drow["f16"].ToString()))
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitPackagingDate", DateTime.Parse(emptyDate)));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitPackagingDate, DateTime.Parse(emptyDate));
                                        }
                                        else
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitPackagingDate", Convert.ToDateTime(drow["f16"])));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitPackagingDate, Convert.ToDateTime(drow["f16"]));
                                        }
                                    }
                                    else
                                    {
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailUniqueSchemeID", drow["ActivityDetailUniqueSchemeID"].ToString()));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailSchemeID", drow["ActivityDetailSchemeID"].ToString()));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailQuantity", Convert.ToDecimal(drow["ActivityDetailQuantity"])));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailBaseQuantity", Convert.ToDecimal(drow["ActivityDetailBaseQuantity"])));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailWeightQuantity", Convert.ToDecimal(drow["ActivityDetailWeightQuantity"])));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailSingleTradeItemSerialNumber", drow["ActivityDetailSingleTradeItemSerialNumber"].ToString()));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitBatchLotNumber", drow["ActivityDetailLogisticUnitBatchLotNumber"].ToString()));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitSkidNumber", drow["ActivityDetailLogisticUnitSkidCode"].ToString()));

                                        //recDetail.SetString((int)SysCommon.ActDetailFields.UniqueSchemeID, drow["ActivityDetailUniqueSchemeID"].ToString());
                                        //recDetail.SetString((int)SysCommon.ActDetailFields.SchemeID, drow["ActivityDetailSchemeID"].ToString());
                                        //recDetail.SetDecimal((int)SysCommon.ActDetailFields.Quantity, Convert.ToDecimal(drow["ActivityDetailQuantity"]));
                                        //recDetail.SetDecimal((int)SysCommon.ActDetailFields.BaseQuantity, Convert.ToDecimal(drow["ActivityDetailBaseQuantity"]));
                                        //recDetail.SetDecimal((int)SysCommon.ActDetailFields.WeightQuantity, Convert.ToDecimal(drow["ActivityDetailWeightQuantity"]));
                                        //recDetail.SetString((int)SysCommon.ActDetailFields.SingleTradeItemSerialNumber, drow["ActivityDetailSingleTradeItemSerialNumber"].ToString());
                                        //recDetail.SetString((int)SysCommon.ActDetailFields.LogisticUnitBatchLotNumber, drow["ActivityDetailLogisticUnitBatchLotNumber"].ToString());
                                        //recDetail.SetString((int)SysCommon.ActDetailFields.LogisticUnitSkidNumber, drow["ActivityDetailLogisticUnitSkidCode"].ToString());

                                        if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"].ToString()))
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitBestBeforeSellByDate", DateTime.Parse(emptyDate)));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitBestBeforeSellByDate, DateTime.Parse(emptyDate));
                                        }
                                        else
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitBestBeforeSellByDate", Convert.ToDateTime(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"])));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitBestBeforeSellByDate, Convert.ToDateTime(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"]));
                                        }

                                        if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitExpiryUseByDate"].ToString()))
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitExpiryUseByDate", DateTime.Parse(emptyDate)));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitExpiryUseByDate, DateTime.Parse(emptyDate));
                                        }
                                        else
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitExpiryUseByDate", Convert.ToDateTime(drow["ActivityDetailLogisticUnitExpiryUseByDate"])));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitExpiryUseByDate, Convert.ToDateTime(drow["ActivityDetailLogisticUnitExpiryUseByDate"]));
                                        }

                                        if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitProductionDate"].ToString()))
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitProductionDate", DateTime.Parse(emptyDate)));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitProductionDate, DateTime.Parse(emptyDate));
                                        }
                                        else
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitProductionDate", Convert.ToDateTime(drow["ActivityDetailLogisticUnitProductionDate"])));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitProductionDate, Convert.ToDateTime(drow["ActivityDetailLogisticUnitProductionDate"]));
                                        }

                                        if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitDueDate"].ToString()))
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitDueDate", DateTime.Parse(emptyDate)));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitDueDate, DateTime.Parse(emptyDate));
                                        }
                                        else
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitDueDate", Convert.ToDateTime(drow["ActivityDetailLogisticUnitDueDate"])));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitDueDate, Convert.ToDateTime(drow["ActivityDetailLogisticUnitDueDate"]));
                                        }

                                        if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitPackagingDate"].ToString()))
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitPackagingDate", DateTime.Parse(emptyDate)));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitPackagingDate, DateTime.Parse(emptyDate));
                                        }
                                        else
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitPackagingDate", Convert.ToDateTime(drow["ActivityDetailLogisticUnitPackagingDate"])));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitPackagingDate, Convert.ToDateTime(drow["ActivityDetailLogisticUnitPackagingDate"]));
                                        }
                                    }

                                    cmndDetail.ExecuteNonQuery();
                                    //rsDetail.Insert(recDetail);

                                    cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityDetail";
                                    cmndID.CommandType = CommandType.Text;

                                    actDetailRefno = Convert.ToInt32(cmndID.ExecuteScalar());
                                    actDetailFKRefno = Convert.ToInt32(drow["ActivityDetailProductionFKRefno"]);

                                    // TODO: Resource strings
                                    errMsg = "FK detail refno " + actDetailFKRefno;

                                    cmndDetailFinished.CommandText = "INSERT INTO tActivityDetailProductionFinished (ActivityDetailRefno, ActivityDetailProductionFinishedFKRefno) VALUES (@activityDetailRefno, @activityDetailProductionFinishedFKRefno)";
                                    cmndDetailFinished.CommandType = CommandType.Text;
                                    //cmndDetailFinished.IndexName = "UQ_ActivityDetailProductionFinishedOnActivityDetailProductionFinishedFKRefno";
                                    cmndDetailFinished.Parameters.Add(new SQLiteParameter("@activityDetailRefno", actDetailRefno));
                                    cmndDetailFinished.Parameters.Add(new SQLiteParameter("@activityDetailProductionFinishedFKRefno", actDetailFKRefno));

                                    cmndDetailFinished.ExecuteNonQuery();

                                    //recDetailFinished.SetInt32((int)SysCommon.ActDetailProdFinishedFields.ActDetailRefno, actDetailRefno);
                                    //recDetailFinished.SetInt32((int)SysCommon.ActDetailProdFinishedFields.FKRefno, actDetailFKRefno);
                                    //rsDetailFinished.Insert(recDetailFinished);

                                    //if (this.SaveCommonDetailVariant(drow, drSchemeVar, ref recSchemeVarXRef, actDetailRefno, false))
                                    //{
                                    //    rsSchemeVarXRef.Insert(recSchemeVarXRef);
                                    //}

                                    int schemeVariantNo = 0;

                                    if (useFNo)
                                    {
                                        schemeVariantNo = Convert.ToInt32(drow["f17"]);
                                    }
                                    else
                                    {
                                        schemeVariantNo = Convert.ToInt32(drow["ProductSchemeVariantNo"]);
                                    }

                                    if (0 < schemeVariantNo)
                                    {
                                        cmndSchemeVar.CommandText = "SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = @productSchemeVariantNo";
                                        cmndSchemeVar.CommandType = CommandType.Text;
                                        //cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";
                                        cmndSchemeVar.Parameters.Add(new SQLiteParameter("@productSchemeVariantNo", schemeVariantNo));

                                        using (SQLiteDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
                                        {
                                            //if (drSchemeVar.Seek(DbSeekOptions.FirstEqual, new object[] { schemeVariantNo }))
                                            //{
                                            if (drSchemeVar.Read())
                                            {
                                                cmndSchemeVarXRef.CommandText = "INSERT INTO tActivityDetailProductSchemeVariantXRef (ActivityDetailRefno, ProductSchemeVariantRefno) VALUES (@activityDetailRefno, @productSchemeVariantRefno)";
                                                cmndSchemeVarXRef.CommandType = CommandType.Text;
                                                //cmndSchemeVarXRef.IndexName = "PK_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefnoAndProductSchemeVariantRefno";
                                                cmndSchemeVarXRef.Parameters.Add(new SQLiteParameter("@activityDetailRefno", actDetailRefno));
                                                cmndSchemeVarXRef.Parameters.Add(new SQLiteParameter("@productSchemeVariantRefno", drSchemeVar.GetInt32((int)SysCommon.ProdSchemeVariantFields.Refno)));

                                                cmndSchemeVarXRef.ExecuteNonQuery();

                                                //recSchemeVarXRef.SetInt32((int)SysCommon.ActDetailSchemeVariantXRefFields.ActDetailRefno, actDetailRefno);
                                                //recSchemeVarXRef.SetInt32((int)SysCommon.ActDetailSchemeVariantXRefFields.ProdSchemeVariantRefno, drSchemeVar.GetInt32((int)SysCommon.ProdSchemeVariantFields.Refno));

                                                //isNeedToSave = true;
                                            }
                                            else
                                            {
                                                // TODO: Resource strings
                                                throw new Exception("drSchemeVar.Read() failed.");
                                            }
                                            //}
                                            //else
                                            //{
                                            //    // TODO: Resource strings
                                            //    throw new Exception(string.Format("Unable to locate product scheme no {0} in tProductSchemeVariant", schemeVariantNo));
                                            //}
                                        }
                                    }

                                    this.SaveDetailExtra(drow, ref cmndExtra, actDetailRefno, false);
                                    //rsExtra.Insert(recExtra);

                                    progValue++;
                                    if (progValue % 100 == 0)
                                    {
                                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                    }
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                        //                                                                        }
                        //                                                                    }

                        //                                                                    drSchemeVar.Close();
                        //                                                                }
                        //                                                            }

                        //                                                            drLocMapWhs.Close();
                        //                                                        }
                        //                                                    }

                        //                                                    drLocMapSect.Close();
                        //                                                }
                        //                                            }

                        //                                            drLocMapSlot.Close();
                        //                                        }
                        //                                    }

                        //                                    drItemProduction.Close();
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished ticket details complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished ticket details failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving finished ticket details failed." + Environment.NewLine + errMsg, ex);
                }
                finally
                {
                    cmndExtra.Dispose();
                }
            }
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="actItemProductionFKRefno">The actItemProductionFKRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override string Create(int actItemProductionFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCTION TICKET FINISHED DETAILS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<itm_refno>" + actItemProductionFKRefno.ToString() + "</itm_refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
