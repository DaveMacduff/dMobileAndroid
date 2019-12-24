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
    /// Defines the <see cref="TableActivityCycleCountDetail" />
    /// </summary>
    public class TableActivityCycleCountDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityCycleCountDetail"/> class.
        /// </summary>
        public TableActivityCycleCountDetail()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityCycleCountDetail";
        }

        /// <summary>
        /// The Request
        /// </summary>
        /// <param name="actItemRefno">The actItemRefno<see cref="int"/></param>
        public override void Request(int actItemRefno)
        {
            string reqData = string.Empty;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingCycleCountDetails), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding cycle count details.", true);
#endif

                if (actItemRefno == 0)
                {
                    using (SQLiteCommand cmndCountItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndCountItem.CommandType = CommandType.Text;
                        cmndCountItem.CommandText = "SELECT tActivityItem.ActivityItemRefno"
                            + " FROM tActivityItem"
                            + " WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.CycleCounts;

                        using (SQLiteDataReader drCountDetail = cmndCountItem.ExecuteReader())
                        {
                            while (drCountDetail.Read())
                            {
                                actItemRefno = Convert.ToInt32(drCountDetail["ActivityItemRefno"]);

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

                            drCountDetail.Close();
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

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingCycleCountDetailsComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding cycle count details complete.", true);
#endif
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.RequestCycleCountDetailsFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Request cycle count details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredRequestCycleCountDetailsFailed), ex);
#else
                throw new Exception("Database exception occurred. Request cycle count details failed.", ex);
#endif
            }
            catch (Exception ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.RequestCycleCountDetailsFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Request cycle count details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.RequestCycleCountDetailsFailed), ex);
#else
                throw new Exception("Request cycle count details failed.", ex);
#endif
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
                    cmndStatus.CommandText = "SELECT COUNT(tActivityDetail.ActivityDetailRefno) "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.CycleCounts;
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.CycleCountDetails), rowsAffected),
#else
              string.Format("{0} cycle count detail(s)", rowsAffected),
#endif
              true);
                }
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredCycleCountDetailsStatusFailed), ex);
#else
                throw new Exception("Database exception occurred. Cycle count details status failed.", ex);
#endif
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
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingCycleCountDetails), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle count details.", true);
#endif

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                        + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.CycleCounts + ")";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailCount "
                        + "WHERE tActivityDetailCount.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.CycleCounts + ")";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailExtra "
                        + "WHERE tActivityDetailExtra.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.CycleCounts + ")";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.CycleCounts;
                    cmndPurge.ExecuteNonQuery();
                }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingCycleCountDetailsComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle count details complete.", true);
#endif
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingCycleCountDetailsFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle count details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingCycleCountDetailsFailed), ex);
#else
                throw new Exception("Database exception occurred. Purging cycle count details failed.", ex);
#endif
            }
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actItemCountRefno">The actItemCountRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override string Send(int actItemCountRefno)
        {
            int actItemCountFKRefno = 0;

            using (SQLiteCommand cmndItemCount = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndItemCount.CommandText = "SELECT * FROM tActivityItemCount WHERE ActivityItemRefno = @activityItemRefno";
                cmndItemCount.CommandType = CommandType.Text;
                //cmndItemCount.IndexName = "PK_ActivityItemCountOnActivityItemRefno";
                cmndItemCount.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemCountRefno));

                using (SQLiteDataReader drItemCount = cmndItemCount.ExecuteReader())
                {
                    //if (drItemCount.Seek(DbSeekOptions.FirstEqual, new object[] { actItemCountRefno }))
                    //{
                    if (drItemCount.Read())
                    {
                        actItemCountFKRefno = Convert.ToInt32(drItemCount.GetValue((int)SysCommon.ActItemCountFields.FKRefno));
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception("drItemCount.Read() failed.");
                    }
                    //}
                    //else
                    //{
                    //    // TODO: Resource strings
                    //    throw new Exception(string.Format("Unable to locate the item cycle count refno {0} in tActivityItemCount.", actItemCountRefno));
                    //}
                }
            }

            return this.SendComm(this.Create(actItemCountFKRefno));
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actItemCountRefno"></param>
        protected override void Save(string reqData, int actItemCountRefno)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            using (SQLiteCommand cmndDetailCount = conn.CreateCommand())
            using (SQLiteCommand cmndDetail = conn.CreateCommand())
            using (SQLiteCommand cmndExtra = conn.CreateCommand())
            using (SQLiteCommand cmndItemCount = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle count details.", false);

                        // Receiving schema but no data results in no table being created in the dataset.
                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailCount"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}.", progMax), false);

                            //using (SQLiteCommand cmndDetailCount = conn.CreateCommand())
                            //{
                            //cmndDetailCount.CommandText = "tActivityDetailCount";
                            //cmndDetailCount.CommandType = CommandType.Text;
                            //cmndDetailCount.IndexName = "UQ_ActivityDetailCountOnActivityDetailCountFKRefno";

                            //using (SqlCeResultSet rsDetailCount = cmndDetailCount.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recDetailCount = rsDetailCount.CreateRecord();

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

                            //using (SQLiteCommand cmndItemCount = conn.CreateCommand())
                            //{
                            //cmndItemCount.CommandText = "tActivityItemCount";
                            //cmndItemCount.CommandType = CommandType.Text;
                            //cmndItemCount.IndexName = "PK_ActivityItemCountOnActivityItemRefno";

                            //using (SQLiteDataReader drItemCount = cmndItemCount.ExecuteReader())
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
                            //cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityDetail";
                            //cmndID.CommandType = CommandType.Text;

                            int actDetailRefno = 0;
                            int progValue = 0;
                            int actDetailCycleFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            cmndItemCount.CommandText = "SELECT * FROM tActivityItemCount WHERE ActivityItemRefno = @activityItemRefno";
                            cmndItemCount.CommandType = CommandType.Text;
                            //cmndItemCount.IndexName = "PK_ActivityItemCountOnActivityItemRefno";
                            cmndItemCount.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemCountRefno));

                            using (SQLiteDataReader drItemCount = cmndItemCount.ExecuteReader())
                            {
                                //if (drItemCount.Seek(DbSeekOptions.FirstEqual, new object[] { actItemCountRefno }))
                                //{
                                if (!drItemCount.Read())
                                {
                                    // TODO: Resource strings
                                    throw new Exception("drItemCount.Read() failed.");
                                }
                                //}
                                //else
                                //{
                                //    // TODO: Resource strings
                                //    throw new Exception(string.Format("Unable to locate the activity item refno {0} in tActivityItemCount", actItemCountRefno));
                                //}

                                foreach (DataRow drow in ds.Tables["tActivityDetailCount"].Rows)
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

                                    cmndDetail.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts)));
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@activityItemRefno", actItemCountRefno));
                                    cmndDetail.Parameters.Add(new SQLiteParameter("@productUnitRefno", drItemCount.GetInt32((int)SysCommon.ActItemCountFields.ProdUnitRefno)));

                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.SubType, (int)SysCommon.ActSubTypes.CycleCounts);
                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.ActivityItemRefno, actItemCountRefno);
                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.ProductUnitRefno, drItemCount.GetInt32((int)SysCommon.ActItemCountFields.ProdUnitRefno));
                                    //this.SaveDetailLocation(drow, drLocMapSlot, drLocMapSection, drLocMapWhs, ref recDetail, true);

                                    int locMapRefno = 0;
                                    int locMapWhsFKRefno = 0;
                                    int locMapSectFKRefno = 0;
                                    int locMapSlotFKRefno = 0;
                                    bool useFNo = true;

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
                                        throw new Exception(string.Format("Unable to identify the location map refno. {0}Whs FK Refno {1}{2}Section FK Refno {3}{4} Slot FK Refno {5}{6}",
                                            Environment.NewLine,
                                            locMapWhsFKRefno,
                                            Environment.NewLine,
                                            locMapSectFKRefno,
                                            Environment.NewLine,
                                            locMapSlotFKRefno));
                                    }

                                    cmndDetail.Parameters.Add(new SQLiteParameter("@locationMapRefno", locMapRefno));
                                    //recDetail.SetInt32((int)SysCommon.ActDetailFields.LocationMapRefno, locMapRefno);
                                    //this.SaveDetail(drow, ref recDetail, true);

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

                                    actDetailCycleFKRefno = Convert.ToInt32(drow["f01"]);

                                    // TODO: Resource strings
                                    errMsg += Environment.NewLine + "Detail fk refno" + actDetailCycleFKRefno;

                                    cmndDetailCount.CommandText = "INSERT INTO tActivityDetailCount (ActivityDetailRefno, ActivityDetailCountFKRefno) VALUES (@activityDetailRefno, @activityDetailCountFKRefno)";
                                    cmndDetailCount.CommandType = CommandType.Text;
                                    //cmndDetailCount.IndexName = "UQ_ActivityDetailCountOnActivityDetailCountFKRefno";
                                    cmndDetailCount.Parameters.Add(new SQLiteParameter("@activityDetailRefno", actDetailRefno));
                                    cmndDetailCount.Parameters.Add(new SQLiteParameter("@activityDetailCountFKRefno", actDetailCycleFKRefno));

                                    cmndDetailCount.ExecuteNonQuery();

                                    //recDetailCount.SetInt32((int)SysCommon.ActDetailCountFields.ActDetailRefno, actDetailRefno);
                                    //recDetailCount.SetInt32((int)SysCommon.ActDetailCountFields.FKRefno, actDetailCycleFKRefno);
                                    //rsDetailCount.Insert(recDetailCount);

                                    //if (this.SaveCommonDetailVariant(drow, drSchemeVar, ref recDetailSchemeVarXRef, actDetailRefno, true))
                                    //{
                                    //rsDetailSchemeVarXRef.Insert(recDetailSchemeVarXRef);
                                    //}

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
                                        cmndSchemeVar.Parameters.Add(new SQLiteParameter("@productSchemeVariantNo", schemeVariantNo));

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

                                    //this.SaveDetailExtra(drow, ref recExtra, actDetailRefno, true);

                                    cmndExtra.CommandText = "INSERT INTO tActivityDetailExtra (" +
                                                            "ActivityDetailRefno, " +
                                                            "ActivityDetailSubType, " +
                                                            "ActivityDetailExtra1Text, " +
                                                            "ActivityDetailExtra2Text, " +
                                                            "ActivityDetailExtra3Text, " +
                                                            "ActivityDetailExtra4Text, " +
                                                            "ActivityDetailExtra5Text" +
                                                            ") VALUES (" +
                                                            "@activityDetailRefno, " +
                                                            "@activityDetailSubType, " +
                                                            "@activityDetailExtra1Text, " +
                                                            "@activityDetailExtra2Text, " +
                                                            "@activityDetailExtra3Text, " +
                                                            "@activityDetailExtra4Text, " +
                                                            "@activityDetailExtra5Text)";
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
                                        cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra5Text", drow["f23"].ToString()));

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
                                        cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra5Text", drow["ActivityDetailExtra5"].ToString()));

                                        //recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text1, drow["ActivityDetailExtra1"].ToString());
                                        //recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text2, drow["ActivityDetailExtra2"].ToString());
                                        //recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text3, drow["ActivityDetailExtra3"].ToString());
                                        //recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text4, drow["ActivityDetailExtra4"].ToString());
                                        //recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text5, drow["ActivityDetailExtra5"].ToString());
                                    }

                                    cmndExtra.ExecuteNonQuery();

                                    progValue++;
                                    if (progValue % 10 == 0)
                                    {
                                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                    }
                                }
                            }

                            transaction.Commit();

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                        //}
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

                        //drItemCount.Close();
                        //                                }
                        //                            }
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle count details complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    transaction.Dispose();

                    cmndDetailCount.Dispose();
                    cmndDetail.Dispose();
                    cmndExtra.Dispose();
                    cmndItemCount.Dispose();
                    cmndLocMapSlot.Dispose();
                    cmndLocMapSection.Dispose();
                    cmndLocMapWhs.Dispose();
                    cmndSchemeVar.Dispose();
                    cmndDetailSchemeVarXRef.Dispose();
                    cmndID.Dispose();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle count details failed.", true);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving cycle count details failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="actItemCountFKRefno">The actItemCountFKRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override string Create(int actItemCountFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST CYCLE COUNT DETAILS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<itm_refno>" + actItemCountFKRefno.ToString() + "</itm_refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
