namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Defines the <see cref="TableActivityProductionTicketByProductDetail" />
    /// </summary>
    public class TableActivityProductionTicketByProductDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketByProductDetail"/> class.
        /// </summary>
        public TableActivityProductionTicketByProductDetail()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketByProductDetail";
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

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product details.", true);

                if (actItemRefno == 0)
                {
                    string query = "SELECT ActivityItemRefno FROM tActivityItem WHERE ActivityItemSubType = ?";
                    List<tActivityItemModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemModel>(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                    if (results != null && results.Count > 0)
                    {
                        actItemRefno = Convert.ToInt32(results[0].ActivityItemRefno);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding ticket add-product details failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product details failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding ticket add-product details failed.", ex);
            }
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
                string query = "SELECT COUNT(ActivityDetailRefno) FROM tActivityDetail WHERE ActivityDetailSubType = ?";
                rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} ticket add-product details.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Ticket add-product details status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override async Task Purge()
        {
            // TODO: Resource strings
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product details.", true);

            try
            {
                string query = "DELETE FROM tActivityDetailProductSchemeVariantXRef"
                    + " WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN"
                    + " (SELECT tActivityDetail.ActivityDetailRefno"
                    + " FROM tActivityDetail"
                    + " WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                query = "DELETE FROM tActivityDetailProductionByProduct"
                    + " WHERE tActivityDetailProductionByProduct.ActivityDetailRefno IN "
                    + "(SELECT tActivityDetail.ActivityDetailRefno"
                    + " FROM tActivityDetail"
                    + " WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                query = "DELETE FROM tActivityDetailExtra"
                        + " WHERE tActivityDetailExtra.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno"
                        + " FROM tActivityDetail"
                        + " WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging ticket add-product details failed.", ex);
            }
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actItemRefno"></param>
        /// <returns></returns>
        protected override async Task<string> Send(int actItemRefno)
        {
            try
            {
                int actItemFKRefno = 0;

                string query = "SELECT * FROM tActivityItemProductionByProduct WHERE ActivityItemRefno = ?";
                List<tActivityItemProductionByProductModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemProductionByProductModel>(query, actItemRefno);

                if (results != null && results.Count > 0)
                {
                    actItemRefno = results[0].ActivityItemProductionByProductFKRefno;
                }

                return this.SendComm(this.Create(actItemFKRefno));
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database Exception occurred. tActivityItemProductionByProductModel Send. ", ex);
            }
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actItemProductionRefno"></param>
        protected override void Save(string reqData, int actItemProductionRefno)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            SQLiteCommand cmndExtra = conn.CreateCommand();
            string errMsg = string.Empty;

            using (SQLiteCommand cmndDetByProduct = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product details.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailProduction"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndDetByProduct = conn.CreateCommand())
                            //{
                            //cmndDetByProduct.CommandText = "tActivityDetailProductionByProduct";
                            //cmndDetByProduct.CommandType = CommandType.Text;
                            //cmndDetByProduct.IndexName = "UQ_ActivityDetailProductionByProductOnActivityDetailProductionByProductFKRefno";

                            //using (SqlCeResultSet rsDetailByProduct = cmndDetByProduct.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recDetByProduct = rsDetailByProduct.CreateRecord();

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

                            int actDetailRefno = 0;
                            int progValue = 0;

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
                                //throw new Exception("Unable to Locate the activity item refno in tActivityItemProduction " + Environment.NewLine + errMsg);
                                //}

                                foreach (DataRow drow in ds.Tables["tActivityDetailProduction"].Rows)
                                {
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
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct)));
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@productUnitRefno", drItemProduction.GetInt32((int)SysCommon.ActItemProdFields.ProdUnitRefno)));
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemProductionRefno));

                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.SubType, (int)SysCommon.ActSubTypes.TicketAddProduct);
                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.ProductUnitRefno, drItemProduction.GetInt32((int)SysCommon.ActItemProdFields.ProdUnitRefno));
                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.ActivityItemRefno, actItemProductionRefno);

                                    //this.SaveDetailLocation(drow, drLocMapSlot, drLocMapSect, drLocMapWhs, /*ref recDetail,*/ false);

                                    int locMapRefno = 0;
                                    int locMapWhsFKRefno = 0;
                                    int locMapSectFKRefno = 0;
                                    int locMapSlotFKRefno = 0;
                                    bool useFNo = false;

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

                                        using (SQLiteDataReader drLocMapSection = cmndLocMapSect.ExecuteReader())
                                        {
                                            //if (drLocMapSection.Seek(DbSeekOptions.FirstEqual, new object[] { locMapSectFKRefno }))
                                            //{
                                            if (drLocMapSection.Read())
                                            {
                                                locMapRefno = Convert.ToInt32(drLocMapSection.GetValue((int)SysCommon.LocMapSectFields.LocMapRefno));
                                            }
                                            //    else
                                            //    {
                                            //        // TODO: Resource strings
                                            //        throw new Exception("drLocMapSection.Read() failed.");
                                            //    }
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
                                            //    else
                                            //    {
                                            //        // TODO: Resource strings
                                            //        throw new Exception("drLocMapWhs.Read() failed.");
                                            //    }
                                            //}
                                        }
                                    }

                                    if (locMapRefno == 0)
                                    {
                                        // TODO: Resource strings
                                        throw new Exception(string.Format("Unable to identify the location map refno. {0}Whs FK Refno {1}{2}Section FK Refno {3}{4} Slot FK Refno {5}{6}",
                                            Environment.NewLine,
                                            locMapWhsFKRefno,
                                            Environment.NewLine,
                                            locMapSectFKRefno,
                                            Environment.NewLine,
                                            locMapSlotFKRefno));
                                    }

                                    cmndDetail.Parameters.Add(new SQLiteParameter("@locationMapRefno", locMapRefno));

                                    //this.SaveDetail(drow, /*ref recDetail, */ false);

                                    string emptyDate = "01-01-1900";

                                    cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailStatus", Convert.ToInt32(0)));
                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.Status, 0);

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

                                    int actDetailProductionFKRefno = Convert.ToInt32(drow["ActivityDetailProductionFKRefno"]);
                                    errMsg += Environment.NewLine + "Detail fk refno " + actDetailProductionFKRefno;

                                    cmndDetByProduct.CommandText = "INSERT INTO tActivityDetailProductionByProduct (ActivityDetailRefno, ActivityDetailProductionByProductFKRefno) VALUES (@activityDetailRefno, @activityDetailProductionByProductFKRefno)";
                                    cmndDetByProduct.CommandType = CommandType.Text;
                                    //cmndDetByProduct.IndexName = "UQ_ActivityDetailProductionByProductOnActivityDetailProductionByProductFKRefno";
                                    cmndDetByProduct.Parameters.Add(new SQLiteParameter("@activityDetailRefno", actDetailRefno));
                                    cmndDetByProduct.Parameters.Add(new SQLiteParameter("@activityDetailProductionByProductFKRefno", actDetailProductionFKRefno));

                                    cmndDetByProduct.ExecuteNonQuery();

                                    //recDetByProduct.SetInt32((int)SysCommon.ActDetailProdFinishedFields.ActDetailRefno, actDetailRefno);
                                    //recDetByProduct.SetInt32((int)SysCommon.ActDetailProdFinishedFields.FKRefno, actDetailProductionFKRefno);
                                    //rsDetailByProduct.Insert(recDetByProduct);

                                    //if (this.SaveCommonDetailVariant(drow, drSchemeVar, ref recSchemeVarXRef, actDetailRefno, false))
                                    //{
                                    //    rsSchemeVarXRef.Insert(recSchemeVarXRef);
                                    //}

                                    int schemeVariantNo = 0;
                                    //bool isNeedToSave = false;

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

                                    progValue++;
                                    if (progValue % 100 == 0)
                                    {
                                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                    }
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                        //}
                        //                                                                }

                        //                                                                drSchemeVar.Close();
                        //                                                            }
                        //                                                        }

                        //                                                        drLocMapWhs.Close();
                        //                                                    }
                        //                                                }

                        //                                                drLocMapSect.Close();
                        //                                            }
                        //                                        }

                        //                                        drLocMapSlot.Close();
                        //                                    }
                        //                                }

                        //                                drItemProduction.Close();
                        //                            }
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        transaction.Commit();

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product details complete.", false);
                        //}
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product details failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving ticket add-product details failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="actItemFKRefno">The actItemFKRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override string Create(int actItemFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCTION TICKET BYPRODUCT DETAILS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<itm_refno>" + actItemFKRefno + "</itm_refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
