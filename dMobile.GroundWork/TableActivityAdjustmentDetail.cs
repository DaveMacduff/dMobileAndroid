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
        }

        /// <summary>
        /// The Request.
        /// </summary>
        /// <param name="actItemRefno"></param>
        public override async Task RequestData(int actItemRefno)
        {

            // GC.Collect();
            // GC.WaitForPendingFinalizers();
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustment details.", true);

            string reqData;
            if (actItemRefno == 0)
            {
                var results = await DatabaseConnection.GetInstance().SQLConn.
                    QueryAsync<tActivityItemModel>(
                    "SELECT ActivityItemRefno FROM tActivityItem WHERE ActivityItemSubType = ?",
                    Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                if (results != null && results.Count > 0)
                {
                    foreach (var activityItem in results)
                    {
                        actItemRefno = Convert.ToInt32(activityItem.ActivityItemRefno);
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

            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustment details complete.", true);
        }

        /// <summary>
        /// The DataStatus.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                var query = "SELECT COUNT(tActivityDetail.ActivityDetailRefno) FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} adjustment details.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Adjustment details status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Purge the activity detail.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustment details.", true);

                var query = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
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
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustment details complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustment details failed.", true);

                throw new Exception("Database exception occurred. Purging adjustment details failed.", ex);
            }
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <param name="actItemAdjRefno"></param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int actItemAdjRefno)
        {
            var query = "SELECT * FROM tActivityItemAdjustment WHERE ActivityItemRefno = ?";
            var results = await DatabaseConnection.GetInstance().
                SQLConn.QueryAsync<tActivityItemAdjustmentModel>(query, actItemAdjRefno);
            int actItemAdjFKRefno;
            if (results != null && results.Count > 0)
            {
                actItemAdjFKRefno = results[0].ActivityItemFKRefno;
            }
            else
            {
                throw new Exception(string.Format(
                    "Unable to locate item adjustment {0} in tActivityItemAdjustment",
                    actItemAdjRefno));
            }

            return await this.SendComm(await this.CreateRequest(actItemAdjFKRefno));
        }

        /// <summary>
        /// Save the activity detail.
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actItemAdjRefno"></param>
        protected override async Task Save(string reqData, int actItemAdjRefno)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (var ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustment details.", false);
                        if (ds.Tables.Count != 0)
                        {
                            var actDetailRefno = 0;
                            var progValue = 0;
                            var actDetailAdjFKRefno = 0;
                            var progMax = Convert.ToInt32(ds.Tables["tActivityDetailAdjustment"].Rows.Count);
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);
                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            // Find the line item.
                            var adjustmentItemResults =
                                transaction.Query<tActivityItemAdjustmentModel>(
                                    "SELECT * FROM tActivityItemAdjustment WHERE ActivityItemRefno = ?",
                                    actItemAdjRefno);
                            if (adjustmentItemResults == null || adjustmentItemResults.Count == 0)
                            {
                                throw new Exception("Unable to find the activity adjustment line item.");
                            }

                            // Iterate through the detail rows and insert them.
                            foreach (DataRow drow in ds.Tables["tActivityDetailAdjustment"].Rows)
                            {
                                errMsg = "Arguments:";
                                InsertActivityDetail(actItemAdjRefno, transaction, adjustmentItemResults, drow);
                                errMsg = InsertActivityAdjustmentDetail(
                                    transaction,
                                    errMsg,
                                    out actDetailRefno,
                                    out actDetailAdjFKRefno,
                                    drow);
                                InsertActivityDetailProductSchemeXRef(transaction, actDetailRefno, drow);
                                InsertActivityDetailExtraInfo(transaction, actDetailRefno, drow);

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

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustment details complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustment details failed.", true);

                    throw new Exception("Database exception occurred. Saving adjustment details failed." + Environment.NewLine + errMsg, ex);
                }
                catch
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustment details failed.", true);
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
            static void InsertActivityDetailProductSchemeXRef(SQLiteConnection transaction, int actDetailRefno, DataRow drow)
            {
                // Insert the detail product scheme cross reference record.
                var schemeVariantNo = Convert.ToInt32(drow["ProductSchemeVariantNo"]);
                if (schemeVariantNo > 0)
                {
                    var emptyDate = "01-01-1900";
                    var resultsProductSchemeVariant =
                        transaction.Query<tProductSchemeVariantModel>(
                        "SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = ?",
                        DateTime.Parse(emptyDate));

                    if (resultsProductSchemeVariant?.Count > 0)
                    {
                        transaction.Execute(
                            "INSERT INTO tActivityDetailProductSchemeVariantXRef" +
                            " (ActivityDetailRefno, ProductSchemeVariantRefno) VALUES (?, ?)",
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
                actDetailRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(
                    "SELECT last_insert_rowid() FROM tActivityDetail"));
                actDetailAdjFKRefno = Convert.ToInt32(drow["ActivityDetailFKRefno"]);
                errMsg += $"{Environment.NewLine}Detail foreign key reference number {actDetailAdjFKRefno}";

                // Create the activity detail adjustment record.
                transaction.Execute(
                    "INSERT INTO tActivityDetailAdjustment (ActivityDetailRefno, ActivityDetailFKRefno) VALUES (?, ?)",
                    actDetailRefno,
                    actDetailAdjFKRefno);
                return errMsg;
            }

            //////////////////////////////////////////////////////////////////////////
            static void InsertActivityDetail(
                int actItemAdjRefno,
                SQLiteConnection transaction,
                List<tActivityItemAdjustmentModel> adjustmentItemResults,
                DataRow drow)
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

                var locMapRefno = GetLocationMapRefno(transaction, drow);
                var paramList = CreateParameterList(
                    actItemAdjRefno,
                    Convert.ToInt32(adjustmentItemResults[0].ProductUnitRefno),
                    drow,
                    locMapRefno);

                // Insert the detail.
                transaction.Execute(query, paramList.ToArray());
            }

            //////////////////////////////////////////////////////////////////////////
            static int GetLocationMapRefno(SQLiteConnection transaction, DataRow drow)
            {
                var locMapRefno = 0;
                var locMapWhsFKRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);
                var locMapSectFKRefno = Convert.ToInt32(drow["LocationMapSectionFKRefno"]);
                var locMapSlotFKRefno = Convert.ToInt32(drow["LocationMapSlotFKRefno"]);

                var resultSlots = transaction.Query<tLocationMapSlotModel>(
                    "SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?",
                    locMapSlotFKRefno);

                if (resultSlots?.Count > 0)
                {
                    locMapRefno = resultSlots[0].LocationMapRefno;
                }

                if (locMapRefno == 0)
                {
                    var resultSections = transaction.Query<tLocationMapSectionModel>(
                        "SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?",
                        locMapSectFKRefno);
                    if (resultSections?.Count > 0)
                    {
                        locMapRefno = resultSections[0].LocationMapRefno;
                    }
                }

                if (locMapRefno == 0)
                {
                    var resultWarehouses = transaction.Query<tLocationMapWarehouseModel>(
                        "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?",
                        locMapWhsFKRefno);
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
                        locMapWhsFKRefno,
                        Environment.NewLine,
                        locMapSectFKRefno,
                        Environment.NewLine,
                        locMapSlotFKRefno));
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

                var emptyDate = "01-01-1900";
                if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"].ToString()))
                {
                    paramList.Add(DateTime.Parse(emptyDate));
                }
                else
                {
                    paramList.Add(Convert.ToDateTime(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"]));
                }

                if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitExpiryUseByDate"].ToString()))
                {
                    paramList.Add(DateTime.Parse(emptyDate));
                }
                else
                {
                    paramList.Add(Convert.ToDateTime(drow["ActivityDetailLogisticUnitExpiryUseByDate"]));
                }

                if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitProductionDate"].ToString()))
                {
                    paramList.Add(DateTime.Parse(emptyDate));
                }
                else
                {
                    paramList.Add(Convert.ToDateTime(drow["ActivityDetailLogisticUnitProductionDate"]));
                }

                if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitDueDate"].ToString()))
                {
                    paramList.Add(DateTime.Parse(emptyDate));
                }
                else
                {
                    paramList.Add(Convert.ToDateTime(drow["ActivityDetailLogisticUnitDueDate"]));
                }

                if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitPackagingDate"].ToString()))
                {
                    paramList.Add(DateTime.Parse(emptyDate));
                }
                else
                {
                    paramList.Add(Convert.ToDateTime(drow["ActivityDetailLogisticUnitPackagingDate"]));
                }

                paramList.Add(drow["ActivityDetailLogisticUnitSkidCode"].ToString());

                return paramList;
            }
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="actItemAdjFKRefno"><see cref="int"/> Activity item adjustment foreign key reference number.</param>
        /// <returns><see cref="string"/> Request xml string.</returns>
        protected override async Task<string> CreateRequest(int actItemAdjFKRefno)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST ADJUSTMENT DETAILS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<itm_refno>" + actItemAdjFKRefno + "</itm_refno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
