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
            this.CurrentSubType = SysCommon.ActSubTypes.TransferTo;
            this.LowerCaseName = "to transfer";
            this.ProperCaseName = "To transfer";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            var conn = DatabaseConnection.GetInstance().SQLConn;
            var subType = Convert.ToInt32(this.CurrentSubType);
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer details.", true);

            await conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    // Corruption recovery cleanup.
                    var query = "DELETE FROM tActivityDetailProductSchemeVariantXRef" +
                                " WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno <= 0" +
                                " OR tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno <= 0";
                    transaction.Execute(query);

                    // Corruption recovery cleanup.
                    query = "DELETE FROM tActivityDetailExtra WHERE tActivityDetailExtra.ActivityDetailRefno <= 0";
                    transaction.Execute(query);

                    // Corruption recovery cleanup.
                    query = "DELETE FROM tActivityDetailTransfer WHERE tActivityDetailTransfer.ActivityDetailRefno " +
                            "NOT IN (SELECT tActivityDetail.ActivityDetailRefno FROM tActivityDetail)";
                    transaction.Execute(query);

                    // Corruption recovery cleanup.
                    query = "DELETE FROM tActivityDetailTransfer WHERE tActivityDetailTransfer.ActivityDetailRefno <= 0" +
                            " OR tActivityDetailTransfer.ActivityDetailFKRefno <= 0";
                    transaction.Execute(query);

                    // Corruption recovery cleanup.
                    query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?" +
                            " AND tActivityDetail.ActivityDetailRefno <= 0";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                        + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityDetailTransfer "
                        + "WHERE tActivityDetailTransfer.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityDetailExtra "
                        + "WHERE tActivityDetailExtra.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                    transaction.Execute(query, subType);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer details complete.", true);
                    transaction.Commit();
                }
                catch (SQLiteException sqlEx)
                {
                    if (transaction.IsInTransaction)
                    {
                        transaction.Rollback();
                    }

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer details failed.", true);
                    throw new Exception("Database exception occurred. Purging transfer details failed.", sqlEx);
                }
                catch (Exception ex)
                {
                    if (transaction.IsInTransaction)
                    {
                        transaction.Rollback();
                    }

                    throw new Exception("Database exception occurred. Purging transfer details failed.", ex);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        protected override async Task<string> SendRequest(int actItemRefno)
        {
            var results = await DatabaseConnection.GetInstance().SQLConn.
                QueryAsync<tActivityItemTransferModel>("SELECT * FROM tActivityItemTransfer WHERE ActivityItemRefno = ?",
                    Convert.ToInt32(actItemRefno));

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
        /// <param name="responseData"></param>
        /// <param name="actItemTransferRefno"></param>
        /// <inheritdoc/>
        protected override async Task Save(string responseData, int actItemTransferRefno)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (var ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

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
        /// <param name="activityItemFkRefno"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override async Task<string> CreateRequest(int activityItemFkRefno)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST TRANSFER TO DETAILS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<itm_refno>" + activityItemFkRefno.ToString() + "</itm_refno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        protected override Task<string> CreateRequest()
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(string familyCode)
        {
            throw new NotImplementedException();
        }

        public override Task RequestData(int actRefno)
        {
            throw new NotImplementedException();
        }

        public override Task PurgeData(int sectLocMapRefno)
        {
            throw new NotImplementedException();
        }

        public override Task PurgeData(SysCommon.ActSubTypes actSubType)
        {
            throw new NotImplementedException();
        }

        public override Task<int> DataStatus()
        {
            throw new NotImplementedException();
        }

        protected override Task Save(string responseData)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityItemFkRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int prodLocMapType, int whsLocMapFKRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequestProdLoc(int prodLocMapType)
        {
            throw new NotImplementedException();
        }
    }
}