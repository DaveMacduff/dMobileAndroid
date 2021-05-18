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
        public override async Task Request(int actItemRefno)
        {
            string reqData = string.Empty;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding cycle count details.", true);

                if (actItemRefno == 0)
                {
                    List<tActivityItemModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemModel>("SELECT ActivityItemRefno FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));

                    if (results != null && results.Count > 0)
                    {
                        foreach (tActivityItemModel activityItem in results)
                        {
                            actItemRefno = Convert.ToInt32(activityItem.ActivityItemRefno);
                            reqData = await this.Send(actItemRefno);

                            if (0 < reqData.Length)
                            {
                                if (reqData.Contains("<message>ERROR</message>"))
                                {
                                    XmlDocument reqErr = new XmlDocument();
                                    reqErr.LoadXml(reqData);
                                    throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                                }

                                await this.Save(reqData, actItemRefno);
                            }
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

                        await this.Save(reqData, actItemRefno);
                    }
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding cycle count details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Request cycle count details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Request cycle count details failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Request cycle count details failed.", true);

                // TODO: Resource strings
                throw new Exception("Request cycle count details failed.", ex);
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
                string query = "SELECT COUNT(tActivityDetail.ActivityDetailRefno) FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>(query, Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} cycle count detail(s)", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Cycle count details status failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle count details.", true);

                string query = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                        + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));

                query = "DELETE FROM tActivityDetailCount "
                        + "WHERE tActivityDetailCount.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));

                query = "DELETE FROM tActivityDetailExtra "
                        + "WHERE tActivityDetailExtra.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));

                query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle count details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle count details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging cycle count details failed.", ex);
            }
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actItemCountRefno">The actItemCountRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override async Task<string> Send(int actItemCountRefno)
        {
            int actItemCountFKRefno = 0;

            try
            {
                string query = "SELECT * FROM tActivityItemCount WHERE ActivityItemRefno = ?";
                List<tActivityItemCountModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemCountModel>(query, actItemCountRefno);

                if (results != null && results.Count > 0)
                {
                    actItemCountFKRefno = results[0].ActivityItemCountFKRefno;
                }
                else
                {
                    throw new Exception("No results in tActivityItemCount for ActivityItemRefno " + actItemCountRefno);
                }

                return await this.SendComm(await this.Create(actItemCountFKRefno));
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Send cycle count details failed. ", ex);
            }
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actItemCountRefno"></param>
        protected override async Task Save(string reqData, int actItemCountRefno)
        {
            string errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
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

                            int actDetailRefno = 0;
                            int progValue = 0;
                            int actDetailCycleFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            List<tActivityItemCountModel> countItems = transaction.Query<tActivityItemCountModel>("SELECT * FROM tActivityItemCount WHERE ActivityItemRefno = ?", actItemCountRefno);

                            if (countItems == null || (countItems != null && countItems.Count == 0))
                            {
                                // TODO: Resource strings
                                throw new Exception("drItemCount.Read() failed.");
                            }

                            foreach (DataRow drow in ds.Tables["tActivityDetailCount"].Rows)
                            {
                                // TODO: Resource strings
                                errMsg = "Arguments:";

                                List<object> detailParameterList = new List<object>();
                                string queryInsertDetail = "INSERT INTO tActivityDetail " +
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
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?" +
                                ")";

                                detailParameterList.Add(Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                                detailParameterList.Add(actItemCountRefno);
                                detailParameterList.Add(countItems[0].ProductUnitRefno);
                                detailParameterList.Add(this.SaveDetailLocation(drow, transaction, true));
                                detailParameterList.AddRange(this.SaveDetail(drow, transaction, true));

                                transaction.Execute(queryInsertDetail, detailParameterList);

                                actDetailRefno = Convert.ToInt32(transaction.QueryScalars<int>("SELECT last_insert_rowid() FROM tActivityDetail"));
                                actDetailCycleFKRefno = Convert.ToInt32(drow["f01"]);

                                // TODO: Resource strings
                                errMsg += Environment.NewLine + "Detail fk refno" + actDetailCycleFKRefno;

                                transaction.Execute("INSERT INTO tActivityDetailCount (ActivityDetailRefno, ActivityDetailCountFKRefno) VALUES (?, ?)", actDetailRefno, actDetailCycleFKRefno);

                                this.SaveCommonDetailVariant(drow, transaction, actDetailRefno, true);
                                this.SaveDetailExtra(drow, transaction, actDetailRefno, true);

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                    }

                    transaction.Commit();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle count details complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle count details failed.", true);

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving cycle count details failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="actItemCountFKRefno">The actItemCountFKRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override async Task<string> Create(int actItemCountFKRefno)
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
