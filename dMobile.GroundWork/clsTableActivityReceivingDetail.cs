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
    /// Defines the <see cref="TableActivityReceivingDetail" />
    /// </summary>
    public class TableActivityReceivingDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityReceivingDetail"/> class.
        /// </summary>
        public TableActivityReceivingDetail()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReceivingDetail";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving details.", true);

                if (actItemRefno == 0)
                {
                    using (SQLiteCommand cmndRecvItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        int test = Convert.ToInt32(SysCommon.ActSubTypes.Receiving);

                        cmndRecvItem.CommandText = "SELECT ActivityItemRefno FROM tActivityItem WHERE ActivityItemSubType = @activityItemSubType";
                        cmndRecvItem.CommandType = CommandType.Text;
                        cmndRecvItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Receiving)));

                        using (SQLiteDataReader drRecvItem = cmndRecvItem.ExecuteReader())
                        {
                            while (drRecvItem.Read())
                            {
                                actItemRefno = Convert.ToInt32(drRecvItem["ActivityItemRefno"]);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding receiving details failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving details failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding receiving details failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving details.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.Receiving)));

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                        + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailReceiving "
                        + "WHERE tActivityDetailReceiving.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailExtra "
                        + "WHERE tActivityDetailExtra.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType";
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging receiving details failed.", ex);
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
                    cmndStatus.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.Receiving)));

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} receiving details.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Receiving details status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actItemRecvRefno"></param>
        /// <returns></returns>
        protected override string Send(int actItemRecvRefno)
        {
            string reqData = string.Empty;
            int actItemRecvFKRefno = 0;

            using (SQLiteCommand cmndItemRecv = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndItemRecv.CommandText = "SELECT * FROM tActivityItemReceiving WHERE ActivityItemRefno = @activityItemRefno";
                cmndItemRecv.CommandType = CommandType.Text;
                //cmndItemRecv.IndexName = "PK_ActivityItemReceivingOnActivityItemRefno";
                cmndItemRecv.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemRecvRefno));

                using (SQLiteDataReader drItemRecv = cmndItemRecv.ExecuteReader())
                {
                    //if (drItemRecv.Seek(DbSeekOptions.FirstEqual, new object[] { actItemRecvRefno }))
                    //{
                    if (drItemRecv.Read())
                    {
                        actItemRecvFKRefno = drItemRecv.GetInt32((int)SysCommon.ActItemReceivingFields.FKRefno);
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception("drItemRecv.Read() failed.");
                    }
                    //}
                    //else
                    //{
                    //    // TODO: Resource strings
                    //    throw new Exception(string.Format("Unable to locate the item receiving refno {0} in tActivityItemReceiving", actItemRecvRefno));
                    //}
                }
            }

            reqData = this.SendComm(this.Create(actItemRecvFKRefno));

            return reqData;
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actItemRecvRefno"></param>
        protected override void Save(string reqData, int actItemRecvRefno)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;
            SQLiteCommand cmndDetail = conn.CreateCommand();
            SQLiteCommand cmndExtra = conn.CreateCommand();

            using (SQLiteCommand cmndDetailRecv = conn.CreateCommand())
            //using (SQLiteCommand cmndDetail = conn.CreateCommand())
            //using (SQLiteCommand cmndExtra = conn.CreateCommand())
            using (SQLiteCommand cmndItemRecv = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapSlot = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapSection = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
            using (SQLiteCommand cmndSchemeVar = conn.CreateCommand())
            using (SQLiteCommand cmndDetailSchemeVarXRef = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving details.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailPurchaseOrder"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndDetailRecv = conn.CreateCommand())
                            //{
                            //cmndDetailRecv.CommandText = "tActivityDetailReceiving";
                            //cmndDetailRecv.CommandType = CommandType.Text;
                            //cmndDetailRecv.IndexName = "UQ_ActivityDetailReceivingOnActivityDetailReceivingFKRefno";

                            //using (SqlCeResultSet rsDetailRecv = cmndDetailRecv.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recDetailRecv = rsDetailRecv.CreateRecord();

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

                            //using (SQLiteCommand cmndItemRecv = conn.CreateCommand())
                            //{
                            //cmndItemRecv.CommandText = "tActivityItemReceiving";
                            //cmndItemRecv.CommandType = CommandType.Text;
                            //cmndItemRecv.IndexName = "PK_ActivityItemReceivingOnActivityItemRefno";

                            //using (SQLiteDataReader drItemRecv = cmndItemRecv.ExecuteReader())
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
                            cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityDetail";
                            cmndID.CommandType = CommandType.Text;

                            int actDetailRefno = 0;
                            int productUnitRefno = 0;
                            int progValue = 0;
                            int recvDetailFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            cmndItemRecv.CommandText = "SELECT * FROM tActivityItemReceiving WHERE ActivityItemRefno = @activityItemRefno";
                            cmndItemRecv.CommandType = CommandType.Text;
                            //cmndItemRecv.IndexName = "PK_ActivityItemReceivingOnActivityItemRefno";
                            cmndItemRecv.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemRecvRefno));

                            using (SQLiteDataReader drItemRecv = cmndItemRecv.ExecuteReader())
                            {
                                //if (drItemRecv.Seek(DbSeekOptions.FirstEqual, new object[] { actItemRecvRefno }))
                                //{
                                if (!drItemRecv.Read())
                                {
                                    // TODO: Resource strings
                                    throw new Exception("!drItemRecv.Read() failed.");
                                }
                                //}
                                //else
                                //{
                                //    throw new Exception(string.Format("Unable to locate the item refno {0} in tActivityItemReceiving", actItemRecvRefno));
                                //}

                                productUnitRefno = Convert.ToInt32(drItemRecv.GetValue((int)SysCommon.ActItemReceivingFields.ProdUnitRefno));
                            }


                            foreach (DataRow drow in ds.Tables["tActivityDetailPurchaseOrder"].Rows)
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
                                cmndDetail.CommandType = CommandType.Text;
                                //cmndDetail.IndexName = "PK_ActivityDetailOnActivityDetailRefno";
                                cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.Receiving)));
                                cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailStatus", Convert.ToInt32(0)));
                                cmndDetail.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemRecvRefno));
                                cmndDetail.Parameters.Add(new SQLiteParameter("@productUnitRefno", productUnitRefno));

                                //recDetail.SetInt32((int)SysCommon.ActDetailFields.SubType, (int)SysCommon.ActSubTypes.Receiving);
                                //recDetail.SetInt32((int)SysCommon.ActDetailFields.Status, 0);
                                //recDetail.SetInt32((int)SysCommon.ActDetailFields.ActivityItemRefno, actItemRecvRefno);
                                //recDetail.SetInt32((int)SysCommon.ActDetailFields.ProductUnitRefno, productUnitRefno);

                                bool useFNo = false;

                                this.SaveDetailLocation(drow, ref conn, ref cmndDetail, useFNo);
                                this.SaveDetail(drow, ref cmndDetail, useFNo);

                                cmndDetail.ExecuteNonQuery();

                                actDetailRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                recvDetailFKRefno = Convert.ToInt32(drow["ActivityDetailPurchaseOrderFKRefno"]);
                                errMsg += Environment.NewLine + "Detail fk refno " + recvDetailFKRefno;

                                cmndDetailRecv.CommandText = "INSERT INTO tActivityDetailReceiving (ActivityDetailRefno, ActivityDetailReceivingFKRefno) VALUES (@activityDetailRefno, @activityDetailReceivingFKRefno)";
                                cmndDetailRecv.CommandType = CommandType.Text;
                                //cmndDetailRecv.IndexName = "UQ_ActivityDetailReceivingOnActivityDetailReceivingFKRefno";
                                cmndDetailRecv.Parameters.Add(new SQLiteParameter("@activityDetailRefno", actDetailRefno));
                                cmndDetailRecv.Parameters.Add(new SQLiteParameter("@activityDetailReceivingFKRefno", recvDetailFKRefno));

                                cmndDetailRecv.ExecuteNonQuery();

                                //recDetailRecv.SetInt32((int)SysCommon.ActDetailReceivingFields.ActDetailRefno, actDetailRefno);
                                //recDetailRecv.SetInt32((int)SysCommon.ActDetailReceivingFields.FKRefno, recvDetailFKRefno);
                                //rsDetailRecv.Insert(recDetailRecv);

                                //if (this.SaveCommonDetailVariant(drow, drSchemeVar, ref recDetailSchemeVarXRef, actDetailRefno, false))
                                //{
                                //    rsDetailSchemeVarXRef.Insert(recDetailSchemeVarXRef);
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
                                            cmndDetailSchemeVarXRef.CommandText = "INSERT INTO tActivityDetailProductSchemeVariantXRef (ActivityDetailRefno, ProductSchemeVariantRefno) VALUES (@activityDetailRefno, @productSchemeVariantRefno)";
                                            cmndDetailSchemeVarXRef.CommandType = CommandType.Text;
                                            //cmndDetailSchemeVarXRef.IndexName = "PK_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefnoAndProductSchemeVariantRefno";
                                            cmndDetailSchemeVarXRef.Parameters.Add(new SQLiteParameter("@activityDetailRefno", actDetailRefno));
                                            cmndDetailSchemeVarXRef.Parameters.Add(new SQLiteParameter("@productSchemeVariantRefno", drSchemeVar.GetInt32((int)SysCommon.ProdSchemeVariantFields.Refno)));

                                            cmndDetailSchemeVarXRef.ExecuteNonQuery();

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

                                this.SaveDetailExtra(drow, ref cmndExtra, actDetailRefno, useFNo);

                                //rsExtra.Insert(recExtra);

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
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

                        //                                                drLocMapSection.Close();
                        //                                            }
                        //                                        }

                        //                                        drLocMapSlot.Close();
                        //                                    }
                        //                                }

                        //                                drItemRecv.Close();
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving details complete.", false);
                        //}
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving details failed.", true);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving receiving details failed." + Environment.NewLine + errMsg, ex);
                }
                finally
                {
                    cmndDetail.Dispose();
                    cmndExtra.Dispose();
                }
            }
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="iActivityItemReceivingFKRefno">The iActivityItemReceivingFKRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override string Create(int iActivityItemReceivingFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PURCHASE ORDER DETAILS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<itm_refno>" + iActivityItemReceivingFKRefno.ToString() + "</itm_refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
