namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using System.Threading.Tasks;
    using dMobile.Resources.Models;

    public class TableActivityTransferToDetail : TableBase
    {
        #region Constructors

        public TableActivityTransferToDetail()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityTransferToDetail";
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Request(int actItemRefno)
        {
            string reqData = string.Empty;

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer from details.", true);

                if (actItemRefno == 0)
                {
                    List<tActivityItemModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemModel>("SELECT ActivityItemRefnoFROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));

                    if (results?.Count > 0)
                    {
                        foreach (tActivityItemModel activityItemModel in results)
                        {
                            actItemRefno = Convert.ToInt32(activityItemModel.ActivityItemRefno);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer from details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer from details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding transfer details failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer details failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding transfer details failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task<int> DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityDetailRefno) FROM tActivityDetail WHERE ActivityDetailSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TransferTo)));

                DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} transfer details.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Transfer details status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer details.", true);

                string query = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                    + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                    + "(SELECT tActivityDetail.ActivityDetailRefno "
                    + "FROM tActivityDetail "
                    + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));

                query = "DELETE FROM tActivityDetailTransfer "
                    + "WHERE tActivityDetailTransfer.ActivityDetailRefno IN "
                    + "(SELECT tActivityDetail.ActivityDetailRefno "
                    + "FROM tActivityDetail "
                    + " WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));

                query = "DELETE FROM tActivityDetailExtra "
                    + "WHERE tActivityDetailExtra.ActivityDetailRefno IN "
                    + "(SELECT tActivityDetail.ActivityDetailRefno "
                    + "FROM tActivityDetail "
                    + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));

                query = "DELETE FROM tActivityDetail WHERE ActivityDetailSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging transfer details failed.", ex);
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override async Task<string> Send(int actItemRefno)
        {
            int actItemFKRefno = 0;
            List<tActivityItemTransferModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemTransferModel>("SELECT * FROM tActivityItemTransfer WHERE ActivityItemRefno = ?", Convert.ToInt32(actItemRefno));

            if (results?.Count > 0)
            {
                actItemFKRefno = results[0].ActivityItemTransferFKRefno;
            }
            else
            {
                // TODO: Resource strings
                throw new Exception("drItem.Read() failed. " + string.Format("Unable to locate to activity item transfer refno {0} in tActivityItemTransfer.", actItemRefno));
            }

            return await this.SendComm(await this.Create(actItemFKRefno));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveData"></param>
        /// <param name="actItemTransferRefno"></param>
        protected override async Task Save(string saveData, int actItemTransferRefno)
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

                        ds.ReadXml(SysCommon.GetXmlReader(ref saveData), XmlReadMode.Auto);

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer details.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailTransfer"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actDetailRefno = 0;
                            int progValue = 0;
                            int actDetailTransferFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            List<tActivityItemTransferModel> resultsActivityItemTransfer = transaction.Query<tActivityItemTransferModel>("SELECT * FROM tActivityItemTransfer WHERE ActivityItemRefno = ?", Convert.ToInt32(actItemTransferRefno));

                            if (resultsActivityItemTransfer == null || resultsActivityItemTransfer?.Count == 0)
                            {
                                // TODO: Resource strings
                                throw new Exception(string.Format("Unable to locate the activity item transfer refno {0} in tActivityItemTransfer.", actItemTransferRefno) + Environment.NewLine + errMsg);
                            }

                            foreach (DataRow drow in ds.Tables["tActivityDetailTransfer"].Rows)
                            {
                                errMsg = "Arguments:";

                                string queryInsertDetail = "INSERT INTO tActivityDetail (" +
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
                                List<object> parametersInsertDetail = new List<object>();
                                parametersInsertDetail.Add(Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));
                                parametersInsertDetail.Add(Convert.ToInt32(SysCommon.ActStatus.NoChange));
                                parametersInsertDetail.Add(Convert.ToInt32(actItemTransferRefno));
                                parametersInsertDetail.Add(this.SaveDetailLocation(drow, transaction, false));
                                parametersInsertDetail.Add(resultsActivityItemTransfer[0].ProductUnitRefno);
                                parametersInsertDetail.AddRange(this.SaveDetail(drow, transaction, false));

                                transaction.Execute(queryInsertDetail, parametersInsertDetail.ToArray());

                                actDetailRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityDetail"));
                                actDetailTransferFKRefno = Convert.ToInt32(drow["ActivityDetailTransferFKRefno"]);
                                errMsg += Environment.NewLine + "Detail fk refno " + actDetailTransferFKRefno;

                                string queryInsertDetailTransfer = "INSERT INTO tActivityDetailTransfer (" +
                                    "ActivityDetailRefno, " +
                                    "ActivityDetailTransferFKRefno, " +
                                    "ActivityDetailTransferIsConfirmed, " +
                                    "ActivityDetailTransferTransModeEnum" +
                                    ") VALUES (" +
                                    "?, " +
                                    "?, " +
                                    "?, " +
                                    "?" +
                                    ")";
                                object[] parametersInsertDetailTransfer = new object[]
                                {
                                    Convert.ToInt32(actDetailRefno),
                                    Convert.ToInt32(actDetailTransferFKRefno),
                                    Convert.ToBoolean(drow["ActivityDetailTransferIsConfirmed"]),
                                    Convert.ToInt32(drow["ActivityDetailTransferTransModeEnum"])
                                };

                                transaction.Execute(queryInsertDetailTransfer, parametersInsertDetailTransfer);

                                this.SaveCommonDetailVariant(drow, transaction, actDetailRefno, false);
                                this.SaveDetailExtra(drow, transaction, actDetailRefno, false);

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            // TODO: Resource strings
                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        transaction.Commit();
                    }

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer details failed.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer details failed.", false);

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving transfer details failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemTransferFKRefno"></param>
        /// <returns></returns>
        protected override async Task<string> Create(int actItemTransferFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST TRANSFER TO DETAILS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + ("<itm_refno>" + actItemTransferFKRefno.ToString() + "</itm_refno>")
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