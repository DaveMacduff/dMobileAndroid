// <copyright file="TableActivityAdjustmentDetail.cs" company="InterSect Business Systems Inc">
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

    /// <summary>
    /// Defines the <see cref="TableActivityAdjustmentDetail" />.
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
            this.CurrentSubType = SysCommon.ActSubTypes.Adjustment;
            this.LowerCaseName = "adjusment";
            this.ProperCaseName = "Adjustment";
        }

        /// <summary>
        /// Purge the activity detail.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task PurgeData()
        {
            var conn = DatabaseConnection.GetInstance().SQLConn;
            var subType = Convert.ToInt32(this.CurrentSubType);
            this.DataAccessStatusOutHandler(
                sender: this,
                EventArgs.Empty,
                "Purging adjustments details.",
                overrideDisconnectedMode: true);
            await conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    this.DataAccessStatusOutHandler(
                        sender: this,
                        EventArgs.Empty,
                        "Purging adjustment details.",
                        overrideDisconnectedMode: true);

                    var query = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                                + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                                + "(SELECT tActivityDetail.ActivityDetailRefno "
                                + "FROM tActivityDetail "
                                + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityDetailAdjustment "
                            + "WHERE tActivityDetailAdjustment.ActivityDetailRefno IN "
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

                    transaction.Commit();
                    this.DataAccessStatusOutHandler(
                        sender: this,
                        EventArgs.Empty,
                        "Purging adjustment details complete.",
                        overrideDisconnectedMode: true);
                }
                catch (SQLiteException sqlEx)
                {
                    if (transaction.IsInTransaction)
                    {
                        transaction.Rollback();
                    }

                    this.DataAccessStatusOutHandler(
                        sender: this,
                        EventArgs.Empty,
                        "Database exception occurred.",
                        overrideDisconnectedMode: true);
                    this.DataAccessStatusOutHandler(
                        sender: this,
                        EventArgs.Empty,
                        "Purging adjustment details failed.",
                        overrideDisconnectedMode: true);
                    throw new Exception("Database exception occurred. Purging adjustment details failed.", sqlEx);
                }
                catch (Exception ex)
                {
                    if (transaction.IsInTransaction)
                    {
                        transaction.Rollback();
                    }

                    throw new Exception("Exception occurred. Purging adjustment details failed.", ex);
                }
            });
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <param name="actItemRefno"> Activity item reference number.</param>
        /// <returns><see cref="string"/> Send request response.</returns>
        protected override async Task<string> SendRequest(int actItemRefno)
        {
            var query = "SELECT * FROM tActivityItemAdjustment WHERE ActivityItemRefno = ?";
            var conn = DatabaseConnection.GetInstance().SQLConn;
            var results = await conn.QueryAsync<tActivityItemAdjustmentModel>(
                query,
                actItemRefno);
            if (results.Count <= 0)
            {
                throw new Exception($"Unable to find {actItemRefno} in tActivityItemAdjustment.");
            }

            var request = await this.CreateRequest(results[0].ActivityItemFKRefno);
            return await this.SendComm(request);
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData"><see cref="string"/> Response data received.</param>
        /// <param name="actItemRefno"><see cref="int"/> Activity item reference number.</param>
        /// <returns><see cref="Task"/> Task completed.</returns>
        protected override async Task Save(string responseData, int actItemRefno)
        {
            var errMsg = string.Empty;
            var conn = DatabaseConnection.GetInstance().SQLConn;
            await conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(
                        sender: this,
                        EventArgs.Empty,
                        "Reading received data.",
                        overrideDisconnectedMode: false);
                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
                    this.DataAccessStatusOutHandler(
                        sender: this,
                        EventArgs.Empty,
                        "Saving adjustment details.",
                        overrideDisconnectedMode: false);
                    if (ds.Tables.Count != 0)
                    {
                        var maxRowCount = Convert.ToInt32(ds.Tables["tActivityDetailAdjustment"].Rows.Count);
                        this.DataAccessStatusOutHandler(
                            sender: this,
                            EventArgs.Empty,
                            $"Records {maxRowCount}",
                            overrideDisconnectedMode: false);
                        this.DataAccessProgressOutHandler(
                            sender: this,
                            EventArgs.Empty,
                            currentValue: 0,
                            minValue: 0,
                            maxValue: maxRowCount);

                        // Find the line item.
                        var query = "SELECT * FROM tActivityItemAdjustment WHERE ActivityItemRefno = ?";
                        var actItemModel = transaction.Query<tActivityItemAdjustmentModel>(query, actItemRefno);
                        if (actItemModel == null || actItemModel.Count == 0)
                        {
                            errMsg = "No results in tActivityItemAdjustment for ActivityItemRefno " + actItemRefno +
                                " " + this.FullClassName;
                            throw new Exception(errMsg);
                        }

                        // Iterate through the detail rows and insert them.
                        var rowIdx = 0;
                        foreach (DataRow row in ds.Tables["tActivityDetailAdjustment"].Rows)
                        {
                            errMsg = "Arguments:";
                            InsertActivityDetail(actItemRefno, transaction, actItemModel, row);
                            errMsg = InsertActivityAdjustmentDetail(
                                transaction,
                                errMsg,
                                out var actDetailRefno,
                                out _,
                                row);
                            InsertActivityDetailProductSchemeXRef(transaction, actDetailRefno, row);
                            InsertActivityDetailExtraInfo(transaction, actDetailRefno, row);

                            rowIdx++;
                            if (rowIdx % 10 == 0)
                            {
                                this.DataAccessProgressOutHandler(
                                    sender: this,
                                    EventArgs.Empty,
                                    currentValue: rowIdx,
                                    minValue: 0,
                                    maxValue: maxRowCount);
                            }
                        }

                        this.DataAccessProgressOutHandler(
                            sender: this,
                            EventArgs.Empty,
                            currentValue: maxRowCount,
                            minValue: 0,
                            maxValue: maxRowCount);
                    }

                    transaction.Commit();
                    this.DataAccessStatusOutHandler(
                        sender: this,
                        EventArgs.Empty,
                        "Saving adjustment details complete.",
                        overrideDisconnectedMode: false);
                }
                catch (SQLiteException ex)
                {
                    if (transaction.IsInTransaction)
                    {
                        transaction?.Rollback();
                    }

                    this.DataAccessStatusOutHandler(
                        sender: this,
                        EventArgs.Empty,
                        "Database exception occurred.",
                        overrideDisconnectedMode: true);
                    this.DataAccessStatusOutHandler(
                        sender: this,
                        EventArgs.Empty,
                        "Saving adjustment details failed.",
                        overrideDisconnectedMode: true);

                    throw new Exception(
                        "Database exception occurred. Saving adjustment details failed." +
                            Environment.NewLine + errMsg,
                        ex);
                }
                catch
                {
                    if (transaction.IsInTransaction)
                    {
                        transaction.Rollback();
                    }

                    this.DataAccessStatusOutHandler(
                        sender: this,
                        EventArgs.Empty,
                        "Exception occurred.",
                        overrideDisconnectedMode: true);
                    this.DataAccessStatusOutHandler(
                        sender: this,
                        EventArgs.Empty,
                        "Saving adjustment details failed.",
                        overrideDisconnectedMode: true);
                    throw;
                }
            });

            //////////////////////////////////////////////////////////////////////////
            static void InsertActivityDetailExtraInfo(SQLiteConnection transaction, int actDetailRefno, DataRow drow)
            {
                // Insert the activity detail extra information.
                var parametersExtra = new List<object>();
                var queryExtra = "INSERT INTO tActivityDetailExtra (" +
                                 "ActivityDetailRefno, " +
                                 "ActivityDetailSubType, " +
                                 "ActivityDetailExtra1Text, " +
                                 "ActivityDetailExtra2Text, " +
                                 "ActivityDetailExtra3Text, " +
                                 "ActivityDetailExtra4Text, " +
                                 "ActivityDetailExtra5Date" +
                                 ") VALUES (" +
                                 "?, " +
                                 "?, " +
                                 "?, " +
                                 "?, " +
                                 "?, " +
                                 "?, " +
                                 "?)";
                parametersExtra.Add(actDetailRefno);
                parametersExtra.Add(Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                parametersExtra.Add(drow["ActivityDetailExtra1"].ToString());
                parametersExtra.Add(drow["ActivityDetailExtra2"].ToString());
                parametersExtra.Add(drow["ActivityDetailExtra3"].ToString());
                parametersExtra.Add(drow["ActivityDetailExtra4"].ToString());
                parametersExtra.Add(drow["ActivityDetailExtra5"].ToString());
                transaction.Execute(queryExtra, parametersExtra.ToArray());
            }

            //////////////////////////////////////////////////////////////////////////
            static void InsertActivityDetailProductSchemeXRef(
                SQLiteConnection transaction,
                int actDetailRefno,
                DataRow drow)
            {
                // Insert the detail product scheme cross reference record.
                var schemeVariantNo = Convert.ToInt32(drow["ProductSchemeVariantNo"]);
                if (schemeVariantNo > 0)
                {
                    var emptyDate = "01-01-1900";
                    var query = "SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = ?";
                    var resultsProductSchemeVariant =
                        transaction.Query<tProductSchemeVariantModel>(
                        query,
                        DateTime.Parse(emptyDate));

                    if (resultsProductSchemeVariant?.Count > 0)
                    {
                        query = "INSERT INTO tActivityDetailProductSchemeVariantXRef" +
                            " (ActivityDetailRefno, ProductSchemeVariantRefno) VALUES (?, ?)";
                        transaction.Execute(
                            query,
                            actDetailRefno,
                            resultsProductSchemeVariant[0].ProductSchemeVariantRefno);
                    }
                    else
                    {
                        throw new Exception("drSchemeVar.Read() failed.");
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            static string InsertActivityAdjustmentDetail(
                SQLiteConnection transaction,
                string errMsg,
                out int actDetailRefno,
                out int actDetailAdjFKRefno,
                DataRow drow)
            {
                // Read the new activity detail reference number.
                var query = "SELECT last_insert_rowid() FROM tActivityDetail";
                actDetailRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(query));
                actDetailAdjFKRefno = Convert.ToInt32(drow["ActivityDetailFKRefno"]);
                errMsg += $"{Environment.NewLine}Detail foreign key reference number {actDetailAdjFKRefno}";

                // Create the activity detail adjustment record.
                query = "INSERT INTO tActivityDetailAdjustment (ActivityDetailRefno, ActivityDetailFKRefno) VALUES" +
                    " (?, ?)";
                transaction.Execute(
                    query,
                    actDetailRefno,
                    actDetailAdjFKRefno);
                return errMsg;
            }

            //////////////////////////////////////////////////////////////////////////
            static void InsertActivityDetail(
                int actItemAdjRefno,
                SQLiteConnection transaction,
                List<tActivityItemAdjustmentModel> adjustmentItemResults,
                DataRow row)
            {
                var query = "INSERT INTO tActivityDetail " +
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

                var locMapRefno = GetLocationMapRefno(transaction, row);
                var paramList = CreateParameterList(
                    actItemAdjRefno,
                    Convert.ToInt32(adjustmentItemResults[0].ProductUnitRefno),
                    row,
                    locMapRefno);

                // Insert the detail.
                transaction.Execute(query, paramList.ToArray());
            }

            //////////////////////////////////////////////////////////////////////////
            static int GetLocationMapRefno(SQLiteConnection transaction, DataRow drow)
            {
                var locMapRefno = 0;
                var locMapWhsFkRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);
                var locMapSectFkRefno = Convert.ToInt32(drow["LocationMapSectionFKRefno"]);
                var locMapSlotFkRefno = Convert.ToInt32(drow["LocationMapSlotFKRefno"]);
                var resultSlots = transaction.Query<tLocationMapSlotModel>(
                    "SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?",
                    locMapSlotFkRefno);
                if (resultSlots?.Count > 0)
                {
                    locMapRefno = resultSlots[0].LocationMapRefno;
                }

                if (locMapRefno == 0)
                {
                    var resultSections = transaction.Query<tLocationMapSectionModel>(
                        "SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?",
                        locMapSectFkRefno);
                    if (resultSections?.Count > 0)
                    {
                        locMapRefno = resultSections[0].LocationMapRefno;
                    }
                }

                if (locMapRefno == 0)
                {
                    var resultWarehouses = transaction.Query<tLocationMapWarehouseModel>(
                        "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?",
                        locMapWhsFkRefno);
                    if (resultWarehouses?.Count > 0)
                    {
                        locMapRefno = resultWarehouses[0].LocationMapRefno;
                    }
                }

                if (locMapRefno == 0)
                {
                    throw new Exception(string.Format(
                        "Unable to identify the location map reference number. {0}Whs FK Refno" +
                        " {1}{2}Section FK Refno {3}{4} Slot FK Refno {5}{6}",
                        Environment.NewLine,
                        locMapWhsFkRefno,
                        Environment.NewLine,
                        locMapSectFkRefno,
                        Environment.NewLine,
                        locMapSlotFkRefno));
                }

                return locMapRefno;
            }

            //////////////////////////////////////////////////////////////////////////
            static List<object> CreateParameterList(
                int actItemAdjRefno,
                int productUnitRefno,
                DataRow drow,
                int locMapRefno)
            {
                var paramList = new List<object>
                {
                    Convert.ToInt32(SysCommon.ActSubTypes.Adjustment),
                    Convert.ToInt32(0),
                    actItemAdjRefno,
                    locMapRefno,
                    productUnitRefno,
                    drow["ActivityDetailUniqueSchemeID"].ToString(),
                    drow["ActivityDetailSchemeID"].ToString(),
                    drow["ActivityDetailQuantity"].ToString(),
                    Convert.ToDecimal(drow["ActivityDetailBaseQuantity"]),
                    Convert.ToDecimal(drow["ActivityDetailWeightQuantity"]),
                    drow["ActivityDetailSingleTradeItemSerialNumber"].ToString(),
                    drow["ActivityDetailLogisticUnitBatchLotNumber"].ToString(),
                };

                const string emptyDate = "01-01-1900";
                paramList.Add(string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"].ToString())
                    ? DateTime.Parse(emptyDate)
                    : Convert.ToDateTime(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"]));
                paramList.Add(string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitExpiryUseByDate"].ToString())
                    ? DateTime.Parse(emptyDate)
                    : Convert.ToDateTime(drow["ActivityDetailLogisticUnitExpiryUseByDate"]));
                paramList.Add(string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitProductionDate"].ToString())
                    ? DateTime.Parse(emptyDate)
                    : Convert.ToDateTime(drow["ActivityDetailLogisticUnitProductionDate"]));
                paramList.Add(string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitDueDate"].ToString())
                    ? DateTime.Parse(emptyDate)
                    : Convert.ToDateTime(drow["ActivityDetailLogisticUnitDueDate"]));
                paramList.Add(string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitPackagingDate"].ToString())
                    ? DateTime.Parse(emptyDate)
                    : Convert.ToDateTime(drow["ActivityDetailLogisticUnitPackagingDate"]));
                paramList.Add(drow["ActivityDetailLogisticUnitSkidCode"].ToString());
                return paramList;
            }
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="activityItemFkRefno"><see cref="int"/> Activity item adjustment foreign key reference number.</param>
        /// <returns><see cref="string"/> Request xml string.</returns>
        protected override async Task<string> CreateRequest(int activityItemFkRefno)
        {
            var request = new StringBuilder();
            using var writer = XmlWriter.Create(request, new XmlWriterSettings() { Async = true });
            try
            {
                var xmlRequest = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>REQUEST ADJUSTMENT DETAILS</action>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<itm_refno>" + activityItemFkRefno + "</itm_refno>"
                    + "</command>"
                    + "</imclient>");

                xmlRequest.Save(writer);
                await writer.FlushAsync();
            }
            catch
            {
                throw;
            }
            finally
            {
                writer.Close();
            }

            const string errMsg = "The created request activity detail xml string length has a zero length.";
            request = (request.Length <= 0) ? throw new Exception(errMsg) : request;

            return request.ToString();
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
