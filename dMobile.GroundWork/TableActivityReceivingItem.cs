// <copyright file="TableActivityReceivingItem.cs" company="InterSect Business Systems Inc">
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
    using dMobile.Resources.Models.Headers;
    using Resources.Models;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityReceivingItem" />.
    /// </summary>
    public class TableActivityReceivingItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityReceivingItem"/> class.
        /// </summary>
        public TableActivityReceivingItem()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReceivingItem";
            this.CurrentSubType = SysCommon.ActSubTypes.Receiving;
            this.LowerCaseName = "receiving";
            this.ProperCaseName = "Receiving";
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(
                this,
                EventArgs.Empty,
                $"Purging existing {this.LowerCaseName} items.",
                true);
            var conn = DatabaseConnection.GetInstance().SqlConn;
            await conn.RunInTransactionAsync(
                transaction =>
                {
                    try
                    {
                        // Corruption recovery cleanup.
                        var query = @"
                        DELETE FROM tActivityItemBarCodeAssociationXRef
                            WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItemBarCodeAssociationXRef
                            WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno
                                NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItemReceivingComment
                            WHERE tActivityItemReceivingComment.ActivityItemRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItemReceivingComment
                            WHERE tActivityItemReceivingComment.ActivityItemRefno
                                NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItemReceiving
                            WHERE tActivityItemReceiving.ActivityItemRefno <= 0
                                OR tActivityItemReceiving.ActivityItemFKRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItemReceiving
                            WHERE tActivityItemReceiving.ActivityItemRefno
                                NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItem
                            WHERE tActivityItem.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                        transaction.Execute(query);

                        query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityRefno <= 0";
                        transaction.Execute(query);

                        // Regular purge.
                        query = @"
                        DELETE FROM tActivityItemBarCodeAssociationXRef
                            WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN
                                (SELECT tActivityItem.ActivityItemRefno
                                    FROM tActivityItem
                                    WHERE tActivityItem.ActivityItemSubType = ?)";
                        transaction.Execute(query, this.CurrentSubType);

                        query = @"
                        DELETE FROM tActivityItemReceivingComment
                            WHERE tActivityItemReceivingComment.ActivityItemRefno IN
                                (SELECT tActivityItem.ActivityItemRefno
                                    FROM tActivityItem
                                    WHERE tActivityItem.ActivityItemSubType = ?)";
                        transaction.Execute(query, this.CurrentSubType);

                        query = "DELETE FROM tActivityItemReceiving WHERE ActivityItemSubType = ?";
                        transaction.Execute(query, this.CurrentSubType);

                        query = "DELETE FROM tActivityItem WHERE ActivityItemSubType = ?";
                        transaction.Execute(query, this.CurrentSubType);

                        transaction.Commit();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} items complete.",
                            true);
                    }
                    catch (SQLiteException ex)
                    {
                        transaction.Rollback();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            "Database exception occurred.",
                            true);
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} items failed.",
                            true);
                        throw new Exception(
                            $"Database exception occurred. Purging {this.LowerCaseName} items failed.",
                            ex);
                    }
                    catch
                    {
                        transaction.Rollback();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} items failed.",
                            true);
                        throw;
                    }
                });
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <param name="activityFkRefnoThe activityFkRefno<see cref="int"/>.</param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int activityFkRefno)
        {
            var results = await DatabaseConnection.GetInstance()
                .SqlConn.QueryAsync<ActivityReceivingModel>(
                    "SELECT * FROM tActivityReceiving WHERE ActivityFKRefno = ?",
                    activityFkRefno);

            int recvFKRefno;
            if (results?.Count > 0)
            {
                recvFKRefno = results[0].ActivityFKRefno;
            }
            else
            {
                throw new Exception("drRecv.Read() failed.");
            }

            return await SendComm(await this.CreateRequest(recvFKRefno));
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData"></param>
        /// <param name="actRefno">The actItemRefno<see cref="int"/>.</param>
        protected override async Task Save(string responseData, int actRefno)
        {
            await DatabaseConnection.GetInstance()
                .SqlConn.RunInTransactionAsync(
                    transaction =>
                    {
                        try
                        {
                            using var ds = new DataSet();
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Reading received data.",
                                false);

                            ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving receiving items.",
                                false);

                            if (ds.Tables.Count != 0)
                            {
                                var progMax = Convert.ToInt32(
                                    ds.Tables["tActivityItemPurchaseOrder"].Rows.Count);

                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    $"Records {progMax}",
                                    false);

                                var actItemRefno = 0;
                                var progValue = 0;
                                var prodUnitRefno = 0;
                                var locMapRefno = 0;
                                var recvItemComment = string.Empty;
                                var recvItemFKRefno = 0;

                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    0,
                                    0,
                                    progMax);

                                foreach (DataRow drow in ds.Tables["tActivityItemPurchaseOrder"]
                                             .Rows)
                                {
                                    transaction.Execute(
                                        "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno," +
                                        " ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                                        Convert.ToInt32(SysCommon.ActSubTypes.Receiving),
                                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                                        Convert.ToInt32(actRefno),
                                        0);

                                    actItemRefno = Convert.ToInt32(
                                        transaction.ExecuteScalar<int>(
                                            "SELECT last_insert_rowid() FROM tActivityItem"));
                                    prodUnitRefno = GetProductUnitRefnoFromDataRow(
                                        drow,
                                        transaction,
                                        false);
                                    locMapRefno = GetWhsLocMapRefnoFromDataRow(
                                        drow,
                                        transaction);
                                    recvItemFKRefno = Convert.ToInt32(drow["ActivityItemFKRefno"]);

                                    var queryInsertActivityItemReceiving =
                                        "INSERT INTO tActivityItemReceiving (" +
                                        "ActivityItemRefno, " +
                                        "ActivityItemSubType, " +
                                        "ActivityItemFKRefno, " +
                                        "ActivityItemStringFKRefno, " +
                                        "ProductUnitRefno, " +
                                        "WarehouseLocationMapRefno, " +
                                        "ActivityItemReceivingQuantityOrdered, " +
                                        "ActivityItemReceivingBaseQuantityOrdered, " +
                                        "ActivityItemReceivingWeightQuantityOrdered, " +
                                        "ActivityItemReceivingQuantityReceived, " +
                                        "ActivityItemReceivingBaseQuantityReceived, " +
                                        "ActivityItemReceivingWeightQuantityReceived, " +
                                        "ActivityItemReceivingVendorProductUnitCode, " +
                                        "ActivityItemReceivingDescription, " +
                                        "ActivityItemReceivingQuantityReceivedToDate, " +
                                        "ActivityItemReceivingBaseQuantityReceivedToDate, " +
                                        "ActivityItemReceivingWeightQuantityReceivedToDate" +
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
                                        "?" +
                                        ")";
                                    var parametersActivityItemReceiving = new object[]
                                    {
                                        Convert.ToInt32(actItemRefno),
                                        Convert.ToInt32(SysCommon.ActSubTypes.Receiving),
                                        Convert.ToInt32(recvItemFKRefno),
                                        Convert.ToString(recvItemFKRefno),
                                        Convert.ToInt32(prodUnitRefno),
                                        Convert.ToInt32(locMapRefno),
                                        Convert.ToDecimal(
                                            drow["ActivityItemPurchaseOrderQuantityOrdered"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemPurchaseOrderBaseQuantityOrdered"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemPurchaseOrderWeightQuantityOrdered"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemPurchaseOrderQuantityReceived"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemPurchaseOrderBaseQuantityReceived"]),
                                        Convert.ToDecimal(
                                            drow[
                                                "ActivityItemPurchaseOrderWeightQuantityReceived"]),
                                        Convert.ToString(
                                            drow["ActivityItemPurchaseOrderVendorProductUnitCode"]),
                                        Convert.ToString(
                                            drow["ActivityItemPurchaseOrderDescription"]),
                                        Convert.ToDecimal(
                                            drow[
                                                "ActivityItemPurchaseOrderQuantityReceivedToDate"]),
                                        Convert.ToDecimal(
                                            drow[
                                                "ActivityItemPurchaseOrderBaseQuantityReceivedToDate"]),
                                        Convert.ToDecimal(
                                            drow[
                                                "ActivityItemPurchaseOrderWeightQuantityReceivedToDate"])
                                    };

                                    transaction.Execute(
                                        queryInsertActivityItemReceiving,
                                        parametersActivityItemReceiving);

                                    recvItemComment = Convert.ToString(
                                        drow["ActivityItemPurchaseOrderComment"]);

                                    if (!string.IsNullOrEmpty(recvItemComment))
                                    {
                                        transaction.Execute(
                                            "INSERT INTO tActivityItemReceivingComment (ActivityItemRefno, ActivityItemReceivingCommentComment) VALUES (?, ?)",
                                            actItemRefno,
                                            recvItemComment);
                                    }

                                    progValue++;
                                    if (progValue % 100 == 0)
                                    {
                                        this.DataAccessProgressOutHandler(
                                            this,
                                            EventArgs.Empty,
                                            progValue,
                                            0,
                                            progMax);
                                    }
                                }

                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    progMax,
                                    0,
                                    progMax);
                            }

                            transaction.Commit();

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving receiving items complete.",
                                false);
                        }
                        catch (SQLiteException ex)
                        {
                            transaction.Rollback();

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Database exception occurred.",
                                true);
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving receiving items failed.",
                                false);
                            throw new Exception(
                                "Database exception occurred. Saving receiving items failed.",
                                ex);
                        }
                    });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="activityFkRefno"></param>
        /// <returns></returns>
        protected override Task<string> CreateRequest(int activityFkRefno)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var deviceID = SysDeviceId.GetDeviceId();
            var revision =
                new DateTime(1900, 1, 1).ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
            var loginID = this.SysEnv.ContactLog.LoginID.ToString();

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                "<imclient>" +
                "<command>" +
                "<action>REQUEST PURCHASE ORDER ITEMS</action>" +
                "<device_id>" +
                deviceID +
                "</device_id>" +
                "<login_id>" +
                loginID +
                "</login_id>" +
                "<revision_datetime>" +
                revision +
                "</revision_datetime>" +
                "<refno>" +
                activityFkRefno.ToString() +
                "</refno>" +
                "</command>" +
                "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return Task.FromResult(xmlString.ToString());
        }
    }
}