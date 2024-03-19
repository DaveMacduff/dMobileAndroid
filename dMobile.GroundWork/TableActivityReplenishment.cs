// <copyright file="TableActivityReplenishment.cs" company="InterSect Business Systems Inc">
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
    using Bedrock;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityReplenishment" />.
    /// </summary>
    public class TableActivityReplenishment : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityReplenishment"/> class.
        /// </summary>
        public TableActivityReplenishment()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReplenishment";
            this.CurrentSubType = SysCommon.ActSubTypes.InventoryReplenishment;
            this.LowerCaseName = "replenishment";
            this.ProperCaseName = "Replenishment";
        }

        /// <summary>
        /// Purge the activity header, item and detail data.
        /// </summary>
        /// <returns><see cref="Task"/> Task complete.</returns>
        public override async Task PurgeData()
        {
            var conn = DatabaseConnection.GetInstance().SqlConn;
            var subType = Convert.ToInt32(this.CurrentSubType);
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);

            await conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    // Corruption recovery cleanup.
                    var query = "DELETE FROM tActivityDetailProductSchemeVariantXRef" +
                                " WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno <= 0" +
                                " OR tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno <= 0";
                    transaction.Execute(query);

                    query = "DELETE FROM tActivityDetailExtra WHERE tActivityDetailExtra.ActivityDetailRefno <= 0";
                    transaction.Execute(query);

                    query = "DELETE FROM tActivityDetailReplenishment WHERE tActivityDetailReplenishment.ActivityDetailRefno <= 0" +
                            " OR tActivityDetailReplenishment.ActivityDetailFKRefno <= 0";
                    transaction.Execute(query);

                    query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?" +
                            " AND tActivityDetail.ActivityDetailRefno <= 0";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                        + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityDetailReplenishment "
                        + "WHERE tActivityDetailReplenishment.ActivityDetailRefno IN "
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

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);
                    query = "DELETE FROM tActivityItemBarCodeAssociationXRef WHERE" +
                        " tActivityItemBarCodeAssociationXRef.ActivityItemRefno <= 0";
                    transaction.Execute(query);

                    query = "DELETE FROM tActivityItemReplenishment WHERE tActivityItemReplenishment.ActivityItemRefno <= 0" +
                        " OR tActivityItemReplenishment.ActivityItemFKRefno <= 0";
                    transaction.Execute(query);

                    query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityRefno <= 0";
                    transaction.Execute(query);

                    query = "DELETE FROM tActivityItemBarCodeAssociationXRef "
                        + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
                        + "(SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?)";
                    transaction.Execute(query, Convert.ToInt32(this.CurrentSubType));

                    query = "DELETE FROM tActivityItemReplenishment WHERE tActivityItemReplenishment.ActivityItemSubType = ?";
                    transaction.Execute(query, Convert.ToInt32(this.CurrentSubType));

                    query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                    transaction.Execute(query, Convert.ToInt32(this.CurrentSubType));

                    query = "DELETE FROM tActivityReplenishment WHERE tActivityReplenishment.ActivityRefno <= 0";
                    transaction.Execute(query);

                    query = "DELETE FROM tActivity WHERE tActivity.ActivityRefno <= 0";
                    transaction.Execute(query);

                    query = "DELETE FROM tActivityReplenishment WHERE ActivitySubType = ?";
                    transaction.Execute(query, Convert.ToInt32(this.CurrentSubType));

                    query = "DELETE FROM tActivity WHERE ActivitySubType = ?";
                    transaction.Execute(query, Convert.ToInt32(this.CurrentSubType));

                    transaction.Commit();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} complete.", true);
                }
                catch (SQLiteException sqlEx)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{sqlEx.Message}", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);
                    throw;
                }
                catch
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);
                    throw;
                }
            });
        }

        /// <summary>
        /// Create request for replenishment data, to send to communication server.
        /// </summary>
        /// <param name="actRefno"><see cref="int"/> Activity reference number.</param>
        /// <returns><see cref="string"/> Filtered request.</returns>
        /// <exception cref="NotImplementedException"><see cref="NotImplementedException"/> Not implemented exception.</exception>
        public override Task RequestData(int actRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Purge the activity header, item and detail data.
        /// </summary>
        /// <param name="sectLocMapRefno"><see cref="int"/> Section location map reference number.</param>
        /// <returns><see cref="Task"/> Task complete.</returns>
        /// <exception cref="NotImplementedException"><see cref="NotImplementedException"/> Not implemented exception.</exception>
        public override Task PurgeData(int sectLocMapRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Purge the activity header, item and detail data.
        /// </summary>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes"/> Activity subtype.</param>
        /// <returns><see cref="Task"/> Task complete.</returns>
        /// <exception cref="NotImplementedException"><see cref="NotImplementedException"/> Not implemented exception.</exception>
        public override Task PurgeData(SysCommon.ActSubTypes actSubType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Data status, replenishment data.
        /// </summary>
        /// <remarks> We cannot return the number of records here for the header, items and details.</remarks>
        /// <returns><see cref="int"/> Number of replenishment headers.</returns>
        /// <exception cref="NotImplementedException"><see cref="NotImplementedException"/> Not implemented exception.</exception>
        public override Task<int> DataStatus()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save the activity header, item and detail replenishment data.
        /// </summary>
        /// <param name="responseData"><see cref="string"/> Received data from communication server to save.</param>
        /// <param name="refno"><see cref="int"/> Activity header reference number.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        /// <exception cref="NotImplementedException"><see cref="NotImplementedException"/> Not implemented exception.</exception>
        protected override Task Save(string responseData, int refno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save the activity header, item and detail replenishment data.
        /// </summary>
        /// <param name="responseData"><see cref="string"/> Received data from communication server to save.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        protected override async Task Save(string responseData)
        {
            await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received responseData.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving replenishment.", false);

                    // Receiving schema but no responseData results in no table being created in the dataset.
                    if (ds.Tables.Count != 0)
                    {
                        var progressMax = Convert.ToInt32(ds.Tables["tActivityReplenishment"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progressMax}", false);

                        var progressValue = 0;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progressMax);
                        foreach (DataRow drow in ds.Tables["tActivityReplenishment"].Rows)
                        {
                            var actRefno = InsertHeader(drow);
                            var actItemRefno = InsertItem(drow, actRefno);
                            _ = InsertDetail(actItemRefno, drow);
                            progressValue++;
                            if (progressValue % 10 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progressValue, 0, progressMax);
                            }
                        }

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progressMax, 0, progressMax);
                    }

                    transaction.Commit();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustments complete.", true);
                }
                catch (SQLiteException sqlEx)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{sqlEx.Message}", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustments failed.", true);
                    throw;
                }

                return;

                //////////////////////////////////////////////////////////////////////////
                int InsertHeader(DataRow drow)
                {
                    // Find the warehouse foreign reference number in the location map warehouse table.
                    var whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, transaction);

                    // Insert a new activity record, and read the new activity reference number.
                    _ = transaction.Execute(
                        "INSERT INTO tActivity(ActivityStatus, ActivitySubType) VALUES(?, ?)",
                        0,
                        Convert.ToInt32(SysCommon.ActSubTypes.InventoryReplenishment));
                    var actRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(
                        "SELECT last_insert_rowid() FROM tActivity"));

                    // Insert the activity replenishment header record.
                    const string sql = "INSERT INTO tActivityReplenishment (ActivityRefno, " +
                                       "ActivitySubType, " +
                                       "WarehouseLocationMapRefno) " +
                                       "VALUES " +
                                       "(?, " +
                                       "?, " +
                                       "?)";

                    var parameters = new object[]
                    {
                        actRefno,
                        Convert.ToInt32(SysCommon.ActSubTypes.InventoryReplenishment),
                        whsLocMapRefno,
                    };

                    transaction.Execute(sql, parameters);

                    return actRefno;
                }

                //////////////////////////////////////////////////////////////////////////
                int InsertItem(DataRow drow, int actRefno)
                {
                    transaction.Execute(
                        "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno," +
                        " ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                        Convert.ToInt32(SysCommon.ActSubTypes.InventoryReplenishment),
                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                        actRefno,
                        0);

                    var actItemRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(
                        "SELECT last_insert_rowid() FROM tActivityItem"));

                    var fkRefno = Convert.ToInt32(drow["Refno"]);
                    var fkParentRefno = Convert.ToInt32(drow["ParentRefno"]);
                    var prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, transaction, false);
                    var locMapRefno = this.GetDetailLocation(drow, transaction, false);
                    var moveType = (SysCommon.ReplenishmentTypes)Convert.ToInt32(drow["MoveType"]);
                    var altQty = Convert.ToDecimal(drow["ActivityDetailQuantity"]) * -1;
                    var baseQty = Convert.ToDecimal(drow["ActivityDetailBaseQuantity"]) * -1;
                    var wtQty = Convert.ToDecimal(drow["ActivityDetailWeightQuantity"]) * -1;

                    var query = @"INSERT INTO tActivityItemReplenishment (
                            ActivityItemRefno
                            ,ActivityItemSubType
                            ,MoveType
                            ,ActivityItemFKRefno
                            ,ActivityItemFKParRefno
                            ,ProductUnitRefno
                            ,LocationMapRefno
                            ,ActivityItemReplenishmentQuantity
                            ,ActivityItemReplenishmentBaseQuantity
                            ,ActivityItemReplenishmentWeightQuantity
                        ) VALUES (
                            ?
                            ,?
                            ,?
                            ,?
                            ,?
                            ,?
                            ,?
                            ,?
                            ,?
                            ,?)";

                    var paramsReplenishment = new object[]
                    {
                                actItemRefno,
                                Convert.ToInt32(this.CurrentSubType),
                                Convert.ToInt32(moveType),
                                fkRefno,
                                fkParentRefno,
                                prodUnitRefno,
                                locMapRefno,
                                altQty,
                                baseQty,
                                wtQty,
                    };

                    transaction.Execute(query, paramsReplenishment);

                    return actItemRefno;
                }

                //////////////////////////////////////////////////////////////////////////
                int InsertDetail(int actItemRefno, DataRow drow)
                {
                    const string query = "INSERT INTO tActivityDetail(" +
                                         "ActivityDetailSubType" +
                                         ",ActivityDetailStatus" +
                                         ",ActivityItemRefno" +
                                         ",LocationMapRefno" +
                                         ",ProductUnitRefno" +
                                         ",ActivityDetailUniqueSchemeID" +
                                         ",ActivityDetailSchemeID" +
                                         ",ActivityDetailQuantity" +
                                         ",ActivityDetailBaseQuantity" +
                                         ",ActivityDetailWeightQuantity" +
                                         ",ActivityDetailSingleTradeItemSerialNumber" +
                                         ",ActivityDetailLogisticUnitBatchLotNumber" +
                                         ",ActivityDetailLogisticUnitBestBeforeSellByDate" +
                                         ",ActivityDetailLogisticUnitExpiryUseByDate" +
                                         ",ActivityDetailLogisticUnitProductionDate" +
                                         ",ActivityDetailLogisticUnitDueDate" +
                                         ",ActivityDetailLogisticUnitPackagingDate" +
                                         ",ActivityDetailLogisticUnitSkidNumber)" +
                                         " VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

                    var fkRefno = Convert.ToInt32(drow["Refno"]);
                    var prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, transaction, false);
                    var parametersInsertDetail = new List<object>
                            {
                                Convert.ToInt32(SysCommon.ActSubTypes.InventoryReplenishment),
                                Convert.ToInt32(0),
                                actItemRefno,
                                this.GetDetailLocation(drow, transaction, false),
                                prodUnitRefno,
                            };

                    parametersInsertDetail.AddRange(this.SaveDetail(drow, transaction, false));

                    var rowsAffected = transaction.Execute(query, parametersInsertDetail.ToArray());
                    if (rowsAffected <= 0)
                    {
                        throw new Exception("Insert into ActivityDetail failed. Rows affected <= 0");
                    }

                    var actDetailRefno = Convert.ToInt32(
                        transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityDetail"));

                    var queryInsertDetail = "INSERT INTO tActivityDetailReplenishment (" +
                                            "ActivityDetailRefno, " +
                                            "ActivityDetailFKRefno, " +
                                            "ActivityDetailReplenishmentIsConfirmed" +
                                            ") VALUES (" +
                                            "?, " +
                                            "?, " +
                                            "?" +
                                            ")";

                    transaction.Execute(
                        queryInsertDetail,
                        actDetailRefno,
                        fkRefno,
                        Convert.ToBoolean(drow["ActivityDetailReplenishmentIsConfirmed"]));

                    this.SaveCommonDetailVariant(drow, transaction, actDetailRefno, false);
                    this.SaveDetailExtra(drow, transaction, actDetailRefno, false);

                    return actDetailRefno;
                }
            });
        }

        /// <summary>
        /// Create request to the comm server for the replenishment data.
        /// </summary>
        /// <returns><see cref="string"/> Response data from server.</returns>
        protected override async Task<string> CreateRequest()
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
                + "<action>REQUEST REPLENISHMENT</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// Create request for replenishment data, to send to communication server.
        /// </summary>
        /// <param name="criteria"><see cref="string"/> Request filter criteria.</param>
        /// <returns><see cref="string"/> Filtered request.</returns>
        /// <exception cref="NotImplementedException"><see cref="NotImplementedException"/> Not implemented exception.</exception>
        protected override Task<string> CreateRequest(XDocument criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create request for replenishment data, to send to communication server.
        /// </summary>
        /// <param name="familyCode"><see cref="string"/> Family code to filter with.</param>
        /// <returns><see cref="string"/> Filtered request.</returns>
        /// <exception cref="NotImplementedException"><see cref="NotImplementedException"/> Not implemented exception.</exception>
        protected override Task<string> CreateRequest(string familyCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create request for replenishment data, to send to communication server.
        /// </summary>
        /// <param name="activityItemFkRefno"><see cref="int"/> Activity item foreign reference number.</param>
        /// <returns><see cref="string"/> Filtered request.</returns>
        /// <exception cref="NotImplementedException"><see cref="NotImplementedException"/> Not implemented exception.</exception>
        protected override Task<string> CreateRequest(int activityItemFkRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create request for replenishment data, to send to communication server.
        /// </summary>
        /// <param name="activityItemFkRefno"><see cref="int"/> Activity item foreign reference number.</param>
        /// <param name="criteria"><see cref="XDocument"/> Filter criteria.</param>
        /// <returns><see cref="string"/> Filtered request.</returns>
        /// <exception cref="NotImplementedException"><see cref="NotImplementedException"/> Not implemented exception.</exception>
        protected override Task<string> CreateRequest(int activityItemFkRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create request for replenishment data, to send to communication server.
        /// </summary>
        /// <param name="prodLocMapType"><see cref="int"/> Product location map type.</param>
        /// <param name="whsLocMapFKRefno"><see cref="int"/> Warehouse location map foreign key reference number.</param>
        /// <returns><see cref="string"/> Filtered request.</returns>
        /// <exception cref="NotImplementedException"><see cref="NotImplementedException"/> Not implemented exception.</exception>
        protected override Task<string> CreateRequest(int prodLocMapType, int whsLocMapFKRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create request for replenishment data, to send to communication server.
        /// </summary>
        /// <param name="prodLocMapType"><see cref="int"/> Product location map type.</param>
        /// <returns><see cref="string"/> Filtered request.</returns>
        /// <exception cref="NotImplementedException"><see cref="NotImplementedException"/> Not implemented exception.</exception>
        protected override Task<string> CreateRequestProdLoc(int prodLocMapType)
        {
            throw new NotImplementedException();
        }
    }
}
