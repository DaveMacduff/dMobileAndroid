// <copyright file="TableActivityTransferToItem.cs" company="InterSect Business Systems Inc">
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
    using Resources.Models;
    using SQLite;

    public class TableActivityTransferToItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityTransferToItem"/> class.
        /// </summary>
        public TableActivityTransferToItem()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityTransferToItem";
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
            this.DataAccessStatusOutHandler(
                this,
                EventArgs.Empty,
                $"Purging {this.LowerCaseName}.",
                true);
            var conn = DatabaseConnection.GetInstance().SqlConn;
            await conn.RunInTransactionAsync(
                transaction =>
                {
                    try
                    {
                        // Corruption recovery cleanup.
                        var query = @"DELETE FROM tActivityItemBarCodeAssociationXRef
                                WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno <= 0";
                        transaction.Execute(query);

                        query = @"DELETE FROM tActivityItemBarCodeAssociationXRef
                            WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno NOT IN
                                (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                        transaction.Execute(query);

                        query = @"DELETE FROM tActivityItemTransfer
                            WHERE tActivityItemTransfer.ActivityItemRefno <= 0
                                OR tActivityItemTransfer.ActivityItemFKRefno <= 0";
                        transaction.Execute(query);

                        query = @"DELETE FROM tActivityItemTransfer
                            WHERE tActivityItemTransfer.ActivityItemRefno
                                NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                        transaction.Execute(query);

                        query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityRefno <= 0";
                        transaction.Execute(query);

                        // Regular purge.
                        query = "DELETE FROM tActivityItemBarCodeAssociationXRef" +
                                " WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN" +
                                " (SELECT tActivityItem.ActivityItemRefno" +
                                " FROM tActivityItem" +
                                " WHERE tActivityItem.ActivityItemSubType = ?)";
                        transaction.Execute(query, Convert.ToInt32(this.CurrentSubType));

                        query = "DELETE FROM tActivityItemTransfer WHERE ActivityItemSubType = ?";
                        transaction.Execute(query, Convert.ToInt32(this.CurrentSubType));

                        query = "DELETE FROM tActivityItem WHERE ActivityItemSubType = ?";
                        transaction.Execute(query, Convert.ToInt32(this.CurrentSubType));

                        transaction.Commit();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            "Purging transfer items from complete.",
                            true);
                    }
                    catch (SQLiteException sqlEx)
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
                            $"{sqlEx.Message}",
                            true);
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} failed.",
                            true);
                        throw;
                    }
                    catch
                    {
                        transaction.Rollback();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} failed.",
                            true);
                        throw;
                    }
                });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        protected override async Task<string> SendRequest(int activityFkRefno)
        {
            var results = await DatabaseConnection.GetInstance()
                .SqlConn.QueryAsync<ActivityTransferModel>(
                    "SELECT * FROM tActivityTransfer WHERE ActivityFKRefno = ?",
                    Convert.ToInt32(activityFkRefno));

            int transFKRefno;
            if (results?.Count > 0)
            {
                transFKRefno = results[0].ActivityFKRefno;
            }
            else
            {
                throw new Exception(
                    $"Unable to locate the transfer refno {activityFkRefno} in tActivityTransfer.");
            }

            return await this.SendComm(await this.CreateRequest(transFKRefno));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="responseData"></param>
        /// <remarks></remarks>
        /// <inheritdoc/>
        protected override async Task Save(string responseData, int actRefno)
        {
            await DatabaseConnection.GetInstance()
                .SqlConn.RunInTransactionAsync(
                    transaction =>
                    {
                        try
                        {
                            using (var ds = new DataSet())
                            {
                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    "Reading received data.",
                                    false);

                                ds.ReadXml(
                                    SysCommon.GetXmlReader(ref responseData),
                                    XmlReadMode.Auto);

                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    "Saving transfer items.",
                                    false);

                                if (ds.Tables.Count != 0)
                                {
                                    var progMax = Convert.ToInt32(
                                        ds.Tables["tActivityItemTransfer"].Rows.Count);

                                    this.DataAccessStatusOutHandler(
                                        this,
                                        EventArgs.Empty,
                                        $"Records {progMax}",
                                        false);

                                    var actItemRefno = 0;
                                    var progValue = 0;
                                    var prodUnitRefno = 0;
                                    var locMapRefno = 0;
                                    var locMapWhsFKRefno = 0;
                                    var transItemFKRefno = 0;

                                    this.DataAccessProgressOutHandler(
                                        this,
                                        EventArgs.Empty,
                                        0,
                                        0,
                                        progMax);

                                    foreach (DataRow drow in
                                             ds.Tables["tActivityItemTransfer"].Rows)
                                    {
                                        transaction.Execute(
                                            "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus," +
                                            " ActivityRefno, ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                                            Convert.ToInt32(this.CurrentSubType),
                                            Convert.ToInt32(SysCommon.ActStatus.NoChange),
                                            Convert.ToInt32(actRefno),
                                            0);
                                        actItemRefno = Convert.ToInt32(
                                            transaction.ExecuteScalar<int>(
                                                "SELECT last_insert_rowid() FROM tActivityItem"));
                                        prodUnitRefno = this.GetProductUnitRefnoFromDataRow(
                                            drow,
                                            transaction,
                                            false);
                                        locMapRefno = 0;
                                        locMapWhsFKRefno = Convert.ToInt32(
                                            drow["TransferFromLocationMapWarehouseFKRefno"]);

                                        var resultsLocationMapWarehouse =
                                            transaction.Query<LocationMapWarehouseModel>(
                                                "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?",
                                                Convert.ToInt32(locMapWhsFKRefno));

                                        if (resultsLocationMapWarehouse?.Count > 0)
                                        {
                                            locMapRefno = resultsLocationMapWarehouse[0]
                                                .LocationMapRefno;
                                        }

                                        if (locMapRefno == 0)
                                        {
                                            throw new Exception(
                                                "Unable to indentify the location map reference number.");
                                        }

                                        transItemFKRefno =
                                            Convert.ToInt32(drow["ActivityItemFKRefno"]);
                                        var query = "INSERT INTO tActivityItemTransfer (" +
                                                    "ActivityItemRefno, " +
                                                    "ActivityItemSubType, " +
                                                    "ActivityItemFKRefno, " +
                                                    "ActivityItemStringFKRefno, " +
                                                    "ProductUnitRefno, " +
                                                    "TransferFromWarehouseLocationMapRefno, " +
                                                    "ActivityItemTransferRequiredQuantity, " +
                                                    "ActivityItemTransferQuantity, " +
                                                    "ActivityItemTransferBaseQuantity, " +
                                                    "ActivityItemTransferWeightQuantity" +
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
                                                    "?" +
                                                    ")";
                                        var parameters = new object[]
                                        {
                                            Convert.ToInt32(actItemRefno),
                                            Convert.ToInt32(this.CurrentSubType),
                                            Convert.ToInt32(transItemFKRefno),
                                            Convert.ToString(transItemFKRefno),
                                            Convert.ToInt32(prodUnitRefno),
                                            Convert.ToInt32(locMapRefno),
                                            Convert.ToDecimal(
                                                drow["ActivityItemTransferRequiredQuantity"]),
                                            Convert.ToDecimal(drow["ActivityItemTransferQuantity"]),
                                            Convert.ToDecimal(
                                                drow["ActivityItemTransferBaseQuantity"]),
                                            Convert.ToDecimal(
                                                drow["ActivityItemTransferWeightQuantity"])
                                        };

                                        transaction.Execute(query, parameters);

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
                            }

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving transfer items complete.",
                                false);
                        }
                        catch (SQLiteException ex)
                        {
                            transaction.Rollback();

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Database exception occurred.",
                                false);
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving transfer items failed.",
                                false);
                            throw new Exception(
                                "Database exception occurred. Saving transfer items failed.",
                                ex);
                        }
                    });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="activityFkRefno"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override async Task<string> CreateRequest(int activityFkRefno)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                "<imclient>" +
                "<command>" +
                "<action>REQUEST TRANSFER TO ITEMS</action>" +
                await SysCommon.GetRequestDefaults(false) +
                "<refno>" +
                activityFkRefno.ToString() +
                "</refno>" +
                "</command>" +
                "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        protected override Task<string> CreateRequest()
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(XDocument criteriaFilter)
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

        protected override Task<string> CreateRequest(int activityFkRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int prodLocMapType, int whsLocMapFkRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequestProdLoc(int prodLocMapType)
        {
            throw new NotImplementedException();
        }
    }
}