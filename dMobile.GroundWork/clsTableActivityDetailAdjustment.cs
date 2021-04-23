namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Defines the <see cref="TableActivityAdjustmentDetail" />
    /// </summary>
    public class TableActivityAdjustmentDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityAdjustmentDetail"/> class.
        /// </summary>
        public TableActivityAdjustmentDetail()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityAdjustmentDetail";
        }

        /// <summary>
        /// The Request
        /// </summary>
        /// <param name="actItemRefno"></param>
        public override async Task Request(int actItemRefno)
        {
            string reqData = string.Empty;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            // TODO: Resource strings
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustment details.", true);

            if (actItemRefno == 0)
            {
                string query = "SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                List<tActivityItemModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemModel>(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                if (results != null && results.Count > 0)
                {
                    actItemRefno = results[0].ActivityItemRefno;
                    reqData = await this.Send(actItemRefno);

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
            }
            else
            {
                reqData = await this.Send(actItemRefno);

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
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustment details complete.", true);
        }

        /// <summary>
        /// The DataStatus
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public override async Task<int> DataStatus()
        {
            int rowsAffected = 0;
            try
            {
                string query = "SELECT COUNT(tActivityDetail.ActivityDetailRefno) FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} adjustment details.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Adjustment details status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override async Task Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustment details.", true);

                string query = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                        + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                query = "DELETE FROM tActivityDetailAdjustment "
                        + "WHERE tActivityDetailAdjustment.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment)); 

                query = "DELETE FROM tActivityDetailExtra "
                        + "WHERE tActivityDetailExtra.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = @?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustment details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustment details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging adjustment details failed.", ex);
            }
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actItemAdjRefno"></param>
        /// <returns></returns>
        protected override async Task<string> Send(int actItemAdjRefno)
        {
            string reqData = string.Empty;
            int actItemAdjFKRefno = 0;

            string query = "SELECT * FROM tActivityItemAdjustment WHERE ActivityItemRefno = @activityItemRefno";
            List<tActivityItemAdjustmentModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemAdjustmentModel>(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

            if (results != null && results.Count > 0)
            {
                actItemAdjFKRefno = results[0].ActivityItemAdjustmentFKRefno;
            }
            else
            {
                throw new Exception(string.Format("Unable to locate item adjustment {0} in tActivityItemAdjustment", actItemAdjRefno));
            }

            return this.SendComm(this.Create(actItemAdjFKRefno));
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actItemAdjRefno"></param>
        protected override void Save(string reqData, int actItemAdjRefno)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            using (SQLiteCommand cmndDetailAdj = conn.CreateCommand())
            using (SQLiteCommand cmndDetail = conn.CreateCommand())
            using (SQLiteCommand cmndExtra = conn.CreateCommand())
            using (SQLiteCommand cmndItemAdj = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapSection = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
            using (SQLiteCommand cmndSchemeVar = conn.CreateCommand())
            using (SQLiteCommand cmndDetailSchemeVarXRef = conn.CreateCommand())
            using (SQLiteCommand cmndID = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapSlot = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustment details.", false);

                        // Receiving schema but no data results in no table being created in the dataset.
                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailAdjustment"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndDetailAdj = conn.CreateCommand())
                            //{
                            //cmndDetailAdj.CommandText = "tActivityDetailAdjustment";
                            //cmndDetailAdj.CommandType = CommandType.Text;
                            //cmndDetailAdj.IndexName = "UQ_ActivityDetailAdjustmentOnActivityDetailAdjustmentFKRefno";

                            //using (SqlCeResultSet rsDetailAdj = cmndDetailAdj.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //    SqlCeUpdatableRecord recDetailAdj = rsDetailAdj.CreateRecord();

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

                            //using (SQLiteCommand cmndItemAdj = conn.CreateCommand())
                            //{
                            //cmndItemAdj.CommandText = "tActivityItemAdjustment";
                            //cmndItemAdj.CommandType = CommandType.Text;
                            //cmndItemAdj.IndexName = "PK_ActivityItemAdjustmentOnActivityItemRefno";

                            //using (SQLiteDataReader drItemAdj = cmndItemAdj.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndLocMapSlot = conn.CreateCommand())
                            //{
                            //cmndLocMapSlot.CommandText = "tLocationMapSlot";
                            //cmndLocMapSlot.CommandType = CommandType.Text;
                            //cmndLocMapSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";

                            //using (SQLiteDataReader drLocMapSlot = cmndLocMapSlot.ExecuteReader())
                            //{
                            //using (SQLiteCommand cmndLocMapSection = conn.CreateCommand())
                            //{
                            //cmndLocMapSection.CommandText = "tLocationMapSection";
                            //cmndLocMapSection.CommandType = CommandType.Text;
                            //cmndLocMapSection.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";

                            //using (SQLiteDataReader drLocMapSection = cmndLocMapSection.ExecuteReader())
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
                            //using (SQLiteCommand cmndDetailSchemeVarXRef = conn.CreateCommand())
                            //{
                            //cmndDetailSchemeVarXRef.CommandText = "tActivityDetailProductSchemeVariantXRef";
                            //cmndDetailSchemeVarXRef.CommandType = CommandType.Text;
                            //cmndDetailSchemeVarXRef.IndexName = "PK_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefnoAndProductSchemeVariantRefno";

                            //using (SqlCeResultSet rsDetailSchemeVarXRef = cmndDetailSchemeVarXRef.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recDetailSchemeVarXRef = rsDetailSchemeVarXRef.CreateRecord();

                            //using (SQLiteCommand cmndID = conn.CreateCommand())
                            //{
                            int actDetailRefno = 0;
                            int progValue = 0;
                            int actDetailAdjFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            cmndItemAdj.CommandText = "SELECT * FROM tActivityItemAdjustment WHERE ActivityItemRefno = @activityItemRefno";
                            cmndItemAdj.CommandType = CommandType.Text;
                            //cmndItemAdj.IndexName = "PK_ActivityItemAdjustmentOnActivityItemRefno";
                            cmndItemAdj.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemAdjRefno));

                            using (SQLiteDataReader drItemAdj = cmndItemAdj.ExecuteReader())
                            {
                                //if (drItemAdj.Seek(DbSeekOptions.FirstEqual, new object[] { actItemAdjRefno }))
                                //{
                                if (!drItemAdj.Read())
                                {
                                    throw new Exception("drItemAdj.Read() failed.");
                                }
                                //}
                                //else
                                //{
                                //    // TODO: Resource strings
                                //    throw new Exception(string.Format("Unable to locate the item adjustment refno {0} in tActivityItemAdjustment", actItemAdjRefno));
                                //}

                                foreach (DataRow drow in ds.Tables["tActivityDetailAdjustment"].Rows)
                                {
                                    // TODO: Resource strings
                                    errMsg = "Arguments:";

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

                                    bool useFNo = false;
                                    int locMapRefno = 0;
                                    int locMapWhsFKRefno = 0;
                                    int locMapSectFKRefno = 0;
                                    int locMapSlotFKRefno = 0;

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
                                        cmndLocMapSection.CommandText = "SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = @locationMapSectionFKRefno";
                                        cmndLocMapSection.CommandType = CommandType.Text;
                                        //cmndLocMapSection.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";
                                        cmndLocMapSection.Parameters.Add(new SQLiteParameter("@locationMapSectionFKRefno", locMapSectFKRefno));

                                        using (SQLiteDataReader drLocMapSection = cmndLocMapSection.ExecuteReader())
                                        {
                                            //if (drLocMapSection.Seek(DbSeekOptions.FirstEqual, new object[] { locMapSectFKRefno }))
                                            //{
                                            if (drLocMapSection.Read())
                                            {
                                                locMapRefno = Convert.ToInt32(drLocMapSection.GetValue((int)SysCommon.LocMapSectFields.LocMapRefno));
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
                                            //}
                                            //else
                                            //{
                                            //    // TODO: Resource strings
                                            //    throw new Exception("drLocMapWhs.Read() failed.");
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

                                    string emptyDate = "01-01-1900";

                                    cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.Adjustment)));
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailStatus", Convert.ToInt32(0)));
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemAdjRefno));
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@locationMapRefno", locMapRefno));
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@productUnitRefno", Convert.ToInt32(drItemAdj.GetValue((int)SysCommon.ActItemAdjFields.ProdUnitRefno))));

                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.Status, 0);
                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.LocationMapRefno, locMapRefno);
                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.SubType, (int)SysCommon.ActSubTypes.Adjustment);
                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.ActivityItemRefno, actItemAdjRefno);
                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.ProductUnitRefno, Convert.ToInt32(drItemAdj.GetValue((int)SysCommon.ActItemAdjFields.ProdUnitRefno)));

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
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activitDetailLogisticUnitPackagingDate", DateTime.Parse(emptyDate)));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitPackagingDate, DateTime.Parse(emptyDate));
                                        }
                                        else
                                        {
                                            cmndDetail.Parameters.Add(new SQLiteParameter("@activitDetailLogisticUnitPackagingDate", Convert.ToDateTime(drow["f16"])));
                                            //recDetail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitPackagingDate, Convert.ToDateTime(drow["f16"]));
                                        }
                                    }
                                    else
                                    {
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailStatus", Convert.ToInt32(0)));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailUniqueSchemeID", drow["ActivityDetailUniqueSchemeID"].ToString()));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailSchemeID", drow["ActivityDetailSchemeID"].ToString()));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailQuantity", drow["ActivityDetailQuantity"].ToString()));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailBaseQuantity", Convert.ToDecimal(drow["ActivityDetailBaseQuantity"])));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailWeightQuantity", Convert.ToDecimal(drow["ActivityDetailWeightQuantity"])));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailSingleTradeItemSerialNumber", drow["ActivityDetailSingleTradeItemSerialNumber"].ToString()));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitBatchLotNumber", drow["ActivityDetailLogisticUnitBatchLotNumber"].ToString()));
                                        cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitSkidNumber", drow["ActivityDetailLogisticUnitSkidCode"].ToString()));

                                        //recDetail.SetInt32((int)SysCommon.ActDetailFields.Status, 0);
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

                                    //rsDetail.Insert(recDetail);

                                    cmndDetail.ExecuteNonQuery();

                                    cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityDetail";
                                    cmndID.CommandType = CommandType.Text;

                                    actDetailRefno = Convert.ToInt32(cmndID.ExecuteScalar());
                                    actDetailAdjFKRefno = Convert.ToInt32(drow["ActivityDetailAdjustmentFKRefno"]);

                                    // TODO: Resource strings
                                    errMsg += Environment.NewLine + "Detail fk refno " + actDetailAdjFKRefno;

                                    cmndDetailAdj.CommandText = "INSERT INTO tActivityDetailAdjustment (ActivityDetailRefno, ActivityDetailAdjustmentFKRefno) VALUES (@activityDetailRefno, @activityDetailAdjustmentFKRefno)";
                                    cmndDetailAdj.CommandType = CommandType.Text;
                                    //cmndDetailAdj.IndexName = "UQ_ActivityDetailAdjustmentOnActivityDetailAdjustmentFKRefno";

                                    cmndDetailAdj.Parameters.Add(new SQLiteParameter("@activityDetailRefno", actDetailRefno));
                                    cmndDetailAdj.Parameters.Add(new SQLiteParameter("@activityDetailAdjustmentFKRefno", actDetailAdjFKRefno));

                                    cmndDetailAdj.ExecuteNonQuery();

                                    //recDetailAdj.SetInt32((int)SysCommon.ActDetailAdjFields.ActDetailRefno, actDetailRefno);
                                    //recDetailAdj.SetInt32((int)SysCommon.ActDetailAdjFields.FKRefno, actDetailAdjFKRefno);
                                    //rsDetailAdj.Insert(recDetailAdj);

                                    int schemeVariantNo = 0;
                                    bool isNeedToSave = false;

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
                                        cmndSchemeVar.Parameters.Add(new SQLiteParameter("@productSchemeVariantNo", DateTime.Parse(emptyDate)));

                                        using (SQLiteDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
                                        {
                                            //if (drSchemeVar.Seek(DbSeekOptions.FirstEqual, new object[] { schemeVariantNo }))
                                            //{
                                            if (drSchemeVar.Read())
                                            {
                                                isNeedToSave = true;

                                                cmndDetailSchemeVarXRef.CommandText = "INSERT INTO tActivityDetailProductSchemeVariantXRef (ActivityDetailRefno, ProductSchemeVariantRefno) VALUES (@activityDetailRefno, @productSchemeVariantRefno)";
                                                cmndDetailSchemeVarXRef.CommandType = CommandType.Text;
                                                //cmndDetailSchemeVarXRef.IndexName = "PK_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefnoAndProductSchemeVariantRefno";
                                                cmndDetailSchemeVarXRef.Parameters.Add(new SQLiteParameter("@activityDetailRefno", actDetailRefno));
                                                cmndDetailSchemeVarXRef.Parameters.Add(new SQLiteParameter("@productSchemeVariantRefno", drSchemeVar.GetInt32((int)SysCommon.ProdSchemeVariantFields.Refno)));

                                                //recSchemeVarXRef.SetInt32((int)SysCommon.ActDetailSchemeVariantXRefFields.ActDetailRefno, actDetailRefno);
                                                //recSchemeVarXRef.SetInt32((int)SysCommon.ActDetailSchemeVariantXRefFields.ProdSchemeVariantRefno, drSchemeVar.GetInt32((int)SysCommon.ProdSchemeVariantFields.Refno));

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

                                    if (isNeedToSave)
                                    {
                                        cmndDetailSchemeVarXRef.ExecuteNonQuery();
                                        //rsDetailSchemeVarXRef.Insert(recDetailSchemeVarXRef);
                                    }

                                    //this.SaveDetailExtra(drow, ref recExtra, actDetailRefno, false);

                                    cmndExtra.CommandText = "INSERT INTO tActivityDetailExtra (" +
                                                            "ActivityDetailRefno, " +
                                                            "ActivityDetailSubType, " +
                                                            "ActivityDetailExtra1Text, " +
                                                            "ActivityDetailExtra2Text, " +
                                                            "ActivityDetailExtra3Text, " +
                                                            "ActivityDetailExtra4Text, " +
                                                            "ActivityDetailExtra5Date" +
                                                            ") VALUES (" +
                                                            "@activityDetailRefno, " +
                                                            "@activityDetailSubType, " +
                                                            "@activityDetailExtra1Text, " +
                                                            "@activityDetailExtra2Text, " +
                                                            "@activityDetailExtra3Text, " +
                                                            "@activityDetailExtra4Text, " +
                                                            "@activityDetailExtra5Date)";
                                    cmndExtra.CommandType = CommandType.Text;
                                    //cmndExtra.IndexName = "PK_ActivityDetailExtraOnActivityDetailRefno";
                                    cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailRefno", actDetailRefno));
                                    cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts)));

                                    //recExtra.SetInt32((int)SysCommon.ActDetailExtraFields.ActDetailRefno, actDetailRefno);
                                    //recExtra.SetInt32((int)SysCommon.ActDetailExtraFields.ActDetailSubType, (int)SysCommon.ActSubTypes.CycleCounts);

                                    if (useFNo)
                                    {
                                        cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra1Text", drow["f19"].ToString()));
                                        cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra2Text", drow["f20"].ToString()));
                                        cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra3Text", drow["f21"].ToString()));
                                        cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra4Text", drow["f22"].ToString()));
                                        cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra5Date", drow["f23"].ToString()));

                                        //recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text1, drow["f19"].ToString());
                                        //recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text2, drow["f20"].ToString());
                                        //recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text3, drow["f21"].ToString());
                                        //recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text4, drow["f22"].ToString());
                                        //recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text5, drow["f23"].ToString());
                                    }
                                    else
                                    {
                                        cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra1Text", drow["ActivityDetailExtra1"].ToString()));
                                        cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra2Text", drow["ActivityDetailExtra2"].ToString()));
                                        cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra3Text", drow["ActivityDetailExtra3"].ToString()));
                                        cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra4Text", drow["ActivityDetailExtra4"].ToString()));
                                        cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra5Date", drow["ActivityDetailExtra5"].ToString()));

                                        //recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text1, drow["ActivityDetailExtra1"].ToString());
                                        //recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text2, drow["ActivityDetailExtra2"].ToString());
                                        //recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text3, drow["ActivityDetailExtra3"].ToString());
                                        //recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text4, drow["ActivityDetailExtra4"].ToString());
                                        //recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text5, drow["ActivityDetailExtra5"].ToString());
                                    }

                                    cmndExtra.ExecuteNonQuery();
                                    //rsExtra.Insert(recExtra);

                                    progValue++;
                                    if (progValue % 10 == 0)
                                    {
                                        this.DataAccessProgressOutHandler(
                                            this,
                                            EventArgs.Empty,
                                            progValue,
                                            0,
                                            progMax);
                                    }
                                }

                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                            }
                        }
                        //}

                        //drSchemeVar.Close();
                        //}
                        //}

                        //drLocMapWhs.Close();
                        //}
                        //}

                        //drLocMapSection.Close();
                        //}
                        //}

                        //drLocMapSlot.Close();
                        //    }
                        //}

                        //drItemAdj.Close();
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustment details complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustment details failed.", true);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving adjustment details failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="actItemAdjFKRefno"></param>
        /// <returns></returns>
        protected override string Create(int actItemAdjFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST ADJUSTMENT DETAILS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<itm_refno>" + actItemAdjFKRefno + "</itm_refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
