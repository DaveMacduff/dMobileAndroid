// <copyright file="TableActivityTransferToDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
    using SQLite;

    public class TableActivityTransferToDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityTransferToDetail"/> class.
        /// </summary>
        public TableActivityTransferToDetail()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityTransferToDetail";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task RequestData(int actItemRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer from details.", true);

                string reqData;
                if (actItemRefno == 0)
                {
                    var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemModel>("SELECT ActivityItemRefnoFROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));

                    if (results?.Count > 0)
                    {
                        foreach (var activityItemModel in results)
                        {
                            actItemRefno = Convert.ToInt32(activityItemModel.ActivityItemRefno);
                            reqData = await this.SendRequest(actItemRefno);

                            if (reqData.Length > 0)
                            {
                                if (reqData.Contains("<message>ERROR</message>"))
                                {
                                    var reqErr = new XmlDocument();
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
                    reqData = await this.SendRequest(actItemRefno);

                    if (reqData.Length > 0)
                    {
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            var reqErr = new XmlDocument();
                            reqErr.LoadXml(reqData);
                            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                        }

                        await this.Save(reqData, actItemRefno);
                    }
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer from details complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer from details failed.", true);

                throw new Exception("Database exception occurred. Seeding transfer details failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer details failed.", true);

                throw new Exception("Seeding transfer details failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityDetailRefno) FROM tActivityDetail WHERE ActivityDetailSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TransferTo)));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} transfer details.", true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Transfer details status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer details.", true);

                var query = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
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

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer details complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer details failed.", true);

                throw new Exception("Database exception occurred. Purging transfer details failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        protected override async Task<string> SendRequest(int actItemRefno)
        {
            var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemTransferModel>("SELECT * FROM tActivityItemTransfer WHERE ActivityItemRefno = ?", Convert.ToInt32(actItemRefno));

            int actItemFKRefno;
            if (results?.Count > 0)
            {
                actItemFKRefno = results[0].ActivityItemFKRefno;
            }
            else
            {
                throw new Exception("drItem.Read() failed. " +
                                    $"Unable to locate to activity item transfer refno {actItemRefno} in tActivityItemTransfer.");
            }

            return await this.SendComm(await this.CreateRequest(actItemFKRefno));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="saveData"></param>
        /// <param name="actItemTransferRefno"></param>
        /// <inheritdoc/>
        protected override async Task Save(string saveData, int actItemTransferRefno)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (var ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref saveData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer details.", false);

                        if (ds.Tables.Count != 0)
                        {
                            var progMax = Convert.ToInt32(ds.Tables["tActivityDetailTransfer"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                            var actDetailRefno = 0;
                            var progValue = 0;
                            var actDetailTransferFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            var resultsActivityItemTransfer = transaction.Query<tActivityItemTransferModel>("SELECT * FROM tActivityItemTransfer WHERE ActivityItemRefno = ?", Convert.ToInt32(actItemTransferRefno));

                            if (resultsActivityItemTransfer == null || resultsActivityItemTransfer?.Count == 0)
                            {
                                throw new Exception(
                                    $"Unable to locate the activity item transfer refno {actItemTransferRefno} in tActivityItemTransfer." + Environment.NewLine + errMsg);
                            }

                            foreach (DataRow drow in ds.Tables["tActivityDetailTransfer"].Rows)
                            {
                                errMsg = "Arguments:";

                                var queryInsertDetail = "INSERT INTO tActivityDetail (" +
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
                                var parametersInsertDetail = new List<object>();
                                parametersInsertDetail.Add(Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));
                                parametersInsertDetail.Add(Convert.ToInt32(SysCommon.ActStatus.NoChange));
                                parametersInsertDetail.Add(Convert.ToInt32(actItemTransferRefno));
                                parametersInsertDetail.Add(this.GetDetailLocation(drow, transaction, false));
                                parametersInsertDetail.Add(resultsActivityItemTransfer[0].ProductUnitRefno);
                                parametersInsertDetail.AddRange(this.SaveDetail(drow, transaction, false));

                                transaction.Execute(queryInsertDetail, parametersInsertDetail.ToArray());

                                actDetailRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityDetail"));
                                actDetailTransferFKRefno = Convert.ToInt32(drow["ActivityDetailFKRefno"]);
                                errMsg += Environment.NewLine + "Detail fk refno " + actDetailTransferFKRefno;

                                var queryInsertDetailTransfer = "INSERT INTO tActivityDetailTransfer (" +
                                                                "ActivityDetailRefno, " +
                                                                "ActivityDetailFKRefno, " +
                                                                "ActivityDetailTransferIsConfirmed, " +
                                                                "ActivityDetailTransferTransModeEnum" +
                                                                ") VALUES (" +
                                                                "?, " +
                                                                "?, " +
                                                                "?, " +
                                                                "?" +
                                                                ")";
                                var parametersInsertDetailTransfer = new object[]
                                {
                                    Convert.ToInt32(actDetailRefno),
                                    Convert.ToInt32(actDetailTransferFKRefno),
                                    Convert.ToBoolean(drow["ActivityDetailTransferIsConfirmed"]),
                                    Convert.ToInt32(drow["ActivityDetailTransferTransModeEnum"]),
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

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        transaction.Commit();
                    }

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer details failed.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer details failed.", false);

                    throw new Exception("Database exception occurred. Saving transfer details failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="actItemTransferFKRefno"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override async Task<string> CreateRequest(int actItemTransferFKRefno)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST TRANSFER TO DETAILS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<itm_refno>" + actItemTransferFKRefno.ToString() + "</itm_refno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}