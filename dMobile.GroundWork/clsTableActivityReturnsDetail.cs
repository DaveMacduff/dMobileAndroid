namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using Microsoft.VisualBasic;

    public class TableActivityReturnsDetail : TableBase
    {
        #region Constructors

        public TableActivityReturnsDetail()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReturnsDetail";
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return details.", true);

                if (actItemRefno == 0)
                {
                    using (SQLiteCommand cmndRetItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndRetItem.CommandType = CommandType.Text;
                        cmndRetItem.CommandText = "SELECT tActivityItem.ActivityItemRefno "
                            + "FROM tActivityItem "
                            + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Returns;

                        using (SQLiteDataReader drReturnsItem = cmndRetItem.ExecuteReader())
                        {
                            while (drReturnsItem.Read())
                            {
                                actItemRefno = Convert.ToInt32(drReturnsItem["ActivityItemRefno"]);
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

                            drReturnsItem.Close();
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding sales return details failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return details failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding sales return details failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return details.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                        + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Returns + ")";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailReturns "
                        + "WHERE tActivityDetailReturns.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Returns + ")";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailExtra "
                        + "WHERE tActivityDetailExtra.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Returns + ")";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Returns;
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging sales return details failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override int DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                using (SQLiteCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndStatus.CommandType = CommandType.Text;
                    cmndStatus.CommandText = "SELECT COUNT(tActivityDetail.ActivityDetailRefno) "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.Returns;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} sales return details.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Sales return details status failed.", ex);
            }

            return rowsAffected;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemRetRefno"></param>
        /// <returns></returns>
        protected override string Send(int actItemRetRefno)
        {
            int actItemRetFKRefno = 0;

            using (SQLiteCommand cmndItemReturns = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndItemReturns.CommandText = "SELECT * FROM tActivityItemReturns WHERE ActivityItemRefno = @activityItemRefno";
                cmndItemReturns.CommandType = CommandType.Text;
                //cmndItemReturns.IndexName = "PK_ActivityItemReturnsOnActivityItemRefno";
                cmndItemReturns.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(actItemRetRefno)));

                using (SQLiteDataReader drItemReturns = cmndItemReturns.ExecuteReader())
                {
                    //if (drItemReturns.Seek(DbSeekOptions.FirstEqual, new object[] { actItemRetRefno }))
                    //{
                        if (drItemReturns.Read())
                        {
                            actItemRetFKRefno = Convert.ToInt32(drItemReturns.GetValue((int)SysCommon.ActItemReturnsFields.FKRefno));
                        }
                        else
                        {
                            // TODO: Resource strings
                            throw new Exception("drItemReturns.Read() failed.");
                        }
                    //}
                    //else
                    //{
                    //    // TODO: Resource strings
                    //    throw new Exception(string.Format("Unable to locate the item returns refno {0} in tActivityItemReturns.", actItemRetRefno));
                    //}
                }
            }

            return this.SendComm(this.Create(actItemRetFKRefno));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actItemReturnsRefno"></param>
        protected override void Save(string reqData, int actItemReturnsRefno)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            SQLiteCommand cmndDetail = conn.CreateCommand();
            SQLiteCommand cmndSchemeVarXRef = conn.CreateCommand();
            SQLiteCommand cmndExtra = conn.CreateCommand();
            string errMsg = string.Empty;

            using (SQLiteCommand cmndDetailRet = conn.CreateCommand())
            using (SQLiteCommand cmndItemRet = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapSlot = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapSect = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
            using (SQLiteCommand cmndSchemeVar = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return details.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailReturns"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndDetailRet = conn.CreateCommand())
                            //{
                            //using (SqlCeResultSet rsDetailRet = cmndDetailRet.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recDetailRet = rsDetailRet.CreateRecord();

                            //using (SQLiteCommand cmndDetail = conn.CreateCommand())
                            //{
                            //using (SqlCeResultSet rsDetail = cmndDetail.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recDetail = rsDetail.CreateRecord();

                            //using (SQLiteCommand cmndExtra = conn.CreateCommand())
                            //{

                            //using (SqlCeResultSet rsExtra = cmndExtra.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recExtra = rsExtra.CreateRecord();

                            //using (SQLiteCommand cmndItemRet = conn.CreateCommand())
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

                            cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityDetail";
                            cmndID.CommandType = CommandType.Text;

                            int actDetailRefno = 0;
                            int detailRetFKRefno = 0;
                            int progValue = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            cmndItemRet.CommandText = "SELECT * FROM tActivityItemReturns WHERE ActivityItemRefno = @activityItemRefno";
                            cmndItemRet.CommandType = CommandType.Text;
                            //cmndItemRet.IndexName = "PK_ActivityItemReturnsOnActivityItemRefno";
                            cmndItemRet.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(actItemReturnsRefno)));

                            using (SQLiteDataReader drItemRet = cmndItemRet.ExecuteReader())
                            {
                                //if (drItemRet.Seek(DbSeekOptions.FirstEqual, new object[] { actItemReturnsRefno }))
                                //{
                                if (!drItemRet.Read())
                                {
                                    // TODO: Resource strings
                                    throw new Exception("drItemRet.Read() failed.");
                                }
                                //}
                                //else
                                //{
                                //    // TODO: Resource strings
                                //    throw new Exception(string.Format("Unable to locate the returns item refno {0} in tActivityItemReturns", actItemReturnsRefno));
                                //}

                                foreach (DataRow drow in ds.Tables["tActivityDetailReturns"].Rows)
                                {
                                    errMsg = "Arguments:";

                                    cmndDetail.CommandText = "INSERT INTO tActivityDetail (" +
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
                                    ") VALUES (" +
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
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.Returns)));
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(actItemReturnsRefno)));
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@productUnitRefno", drItemRet.GetInt32((int)SysCommon.ActItemReturnsFields.ProdUnitRefno)));

                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.SubType, (int)SysCommon.ActSubTypes.Returns);
                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.ActivityItemRefno, actItemReturnsRefno);
                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.ProductUnitRefno, drItemRet.GetInt32((int)SysCommon.ActItemReturnsFields.ProdUnitRefno));

                                    this.SaveDetailLocation(drow, ref conn, ref cmndDetail, false);
                                    this.SaveDetail(drow, ref cmndDetail, false);

                                    cmndDetail.ExecuteNonQuery();
                                    //rsDetail.Insert(recDetail);

                                    actDetailRefno = Convert.ToInt32(cmndID.ExecuteScalar());
                                    detailRetFKRefno = Convert.ToInt32(drow["ActivityDetailReturnsFKRefno"]);
                                    errMsg += Environment.NewLine + "Detail fk refno " + detailRetFKRefno;

                                    cmndDetailRet.CommandText = "INSERT INTO tActivityDetailReturns (ActivityDetailRefno, ActivityDetailReturnsFKRefno, ActivityDetailReturnsIsConfirmed) VALUES (@activityDetailRefno, @activityDetailReturnsFKRefno, @activityDetailReturnsIsConfirmed)";
                                    cmndDetailRet.CommandType = CommandType.Text;
                                    //cmndDetailRet.IndexName = "UQ_ActivityDetailReturnsOnActivityDetailReturnsFKRefno";
                                    cmndDetailRet.Parameters.Add(new SQLiteParameter("@activityDetailRefno", Convert.ToInt32(actDetailRefno)));
                                    cmndDetailRet.Parameters.Add(new SQLiteParameter("@activityDetailReturnsFKRefno", Convert.ToInt32(detailRetFKRefno)));
                                    cmndDetailRet.Parameters.Add(new SQLiteParameter("@activityDetailReturnsIsConfirmed", Convert.ToBoolean(drow["ActivityDetailReturnsIsConfirmed"])));

                                    cmndDetailRet.ExecuteNonQuery();

                                    //recDetailRet.SetInt32((int)SysCommon.ActDetailReturnsFields.ActDetailRefno, actDetailRefno);
                                    //recDetailRet.SetInt32((int)SysCommon.ActDetailReturnsFields.FKRefno, detailRetFKRefno);
                                    //recDetailRet.SetBoolean((int)SysCommon.ActDetailReturnsFields.IsConfirmed, Convert.ToBoolean(drow["ActivityDetailReturnsIsConfirmed"]));
                                    //rsDetailRet.Insert(recDetailRet);

                                    if (this.SaveCommonDetailVariant(drow, ref conn, ref cmndSchemeVarXRef, actDetailRefno, false))
                                    {
                                        cmndSchemeVarXRef.ExecuteNonQuery();
                                        //rsSchemeVarXRef.Insert(recSchemeVarXRef);
                                    }

                                    // this.SaveDetail executes query
                                    this.SaveDetailExtra(drow, ref cmndExtra, actDetailRefno, false);

                                    progValue++;
                                    if (progValue % 100 == 0)
                                    {
                                        // TODO: Resource strings
                                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                    }
                                }

                                transaction.Commit();

                                // TODO: Resource strings
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                            }
                        }

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return details complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return details failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving sales return details failed." + Environment.NewLine + errMsg, ex);
                }
                finally
                {
                    cmndDetail.Dispose();
                    cmndSchemeVarXRef.Dispose();
                    cmndExtra.Dispose();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override string Create(int iActivityItemReturnsFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST SALES RETURN DETAILS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<itm_refno>" + iActivityItemReturnsFKRefno.ToString() + "</itm_refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        #endregion
    }
}
