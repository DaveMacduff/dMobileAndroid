// <copyright file="TableActivityProductionTicketFinishedItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Bedrock;
    using Resources.Models.Headers;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityProductionTicketFinishedItem" />.
    /// </summary>
    public class TableActivityProductionTicketFinishedItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketFinishedItem"/> class.
        /// </summary>
        public TableActivityProductionTicketFinishedItem()
            : base()
        {
            this.CurrentSubType = SysCommon.ActSubTypes.TicketFinished;
            this.LowerCaseName = "ticket finished product";
            this.ProperCaseName = "Ticket finished product";
        }

        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(
                this,
                EventArgs.Empty,
                $"Purging {this.LowerCaseName} items.",
                true);
            await Conn.RunInTransactionAsync(
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

                        query =
                            "DELETE FROM tActivityItemProductionFinished WHERE tActivityItemProductionFinished.ActivityItemRefno <= 0" +
                            " OR tActivityItemProductionFinished.ActivityItemFKRefno <= 0";
                        transaction.Execute(query);

                        query =
                            "DELETE FROM tActivityItemProductionFinished WHERE tActivityItemProductionFinished.ActivityItemRefno " +
                            "NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                        transaction.Execute(query);

                        query =
                            "DELETE FROM tActivityItemProduction WHERE tActivityItemProduction.ActivityItemRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItemProduction
                            WHERE tActivityItemProduction.ActivityItemRefno
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

                        query = "DELETE FROM tActivityItemProductionFinished";
                        transaction.Execute(query, this.CurrentSubType);

                        query = "DELETE FROM tActivityItemProduction WHERE ActivityItemSubType = ?";
                        transaction.Execute(query, this.CurrentSubType);

                        query = "DELETE FROM tActivityItem WHERE ActivityItemSubType = ?";
                        transaction.Execute(query, this.CurrentSubType);

                        transaction.Commit();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} details complete.",
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
                            $"Purging {this.LowerCaseName} details failed.",
                            true);
                        throw new BaseException(
                            $"Database exception occurred. Purging {this.LowerCaseName} details failed.",
                            sqlEx);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new BaseException(
                            $"Database exception occurred. Purging {this.LowerCaseName} details failed.",
                            ex);
                    }
                });
        }

        /// <inheritdoc cref="TableBase.SendRequest(int)"/>
        protected override async Task<string> SendRequest(int activityFkRefno)
        {
            var results = await DatabaseConnection.GetInstance()
                .SqlConn.QueryAsync<ActivityProductionFinishedModel>(
                    "SELECT * FROM tActivityProductionFinished WHERE ActivityFKRefno = ?",
                    Convert.ToInt32(activityFkRefno));

            int actProductionFKRefno;
            if (results?.Count > 0)
            {
                actProductionFKRefno = results[0].ActivityFKRefno;
            }
            else
            {
                throw new BaseException("drFinished.Read() failed.");
            }

            return await SendComm(await this.CreateRequest(actProductionFKRefno));
        }

        /// <inheritdoc cref="TableBase.Save(string, int)"/>
        protected override async Task Save(string responseData, int refno)
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
                                "Saving finished ticket items.",
                                false);

                            if (ds.Tables.Count != 0)
                            {
                                var progMax = Convert.ToInt32(
                                    ds.Tables["tActivityItemProduction"].Rows.Count);

                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    $"Records {progMax}",
                                    false);

                                var progValue = 0;

                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    0,
                                    0,
                                    progMax);

                                foreach (DataRow drow in ds.Tables["tActivityItemProduction"].Rows)
                                {
                                    transaction.Execute(
                                        "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno," +
                                        " ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                                        Convert.ToInt32(this.CurrentSubType),
                                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                                        Convert.ToInt32(refno),
                                        0);
                                    var actItemRefno = Convert.ToInt32(
                                        transaction.ExecuteScalar<int>(
                                            "SELECT last_insert_rowid() FROM tActivityItem"));
                                    var prodUnitRefno = GetProductUnitRefnoFromDataRow(
                                        drow,
                                        transaction,
                                        false);

                                    const string queryActivityItemProduction =
                                        "INSERT INTO tActivityItemProduction (" +
                                        "ActivityItemRefno, " +
                                        "ActivityItemSubType, " +
                                        "ProductUnitRefno, " +
                                        "LocationMapRefno, " +
                                        "ActivityItemProductionQuantity, " +
                                        "ActivityItemProductionBaseQuantity, " +
                                        "ActivityItemProductionWeightQuantity, " +
                                        "ActivityItemProductionDescription, " +
                                        "ActivityItemProductionQuantityExpected, " +
                                        "ActivityItemProductionBaseQuantityExpected, " +
                                        "ActivityItemProductionWeightQuantityExpected" +
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
                                        "?" +
                                        ")";
                                    var parametersActivityItemProduction = new object[]
                                    {
                                        actItemRefno,
                                        Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished),
                                        prodUnitRefno,
                                        GetWhsLocMapRefnoFromDataRow(drow, transaction),
                                        Convert.ToDecimal(drow["ActivityItemProductionQuantity"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemProductionBaseQuantity"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemProductionWeightQuantity"]),
                                        drow["ActivityItemProductionDescription"].ToString(),
                                        Convert.ToDecimal(
                                            drow["ActivityItemProductionQuantityExpected"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemProductionBaseQuantityExpected"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemProductionWeightQuantityExpected"])
                                    };

                                    transaction.Execute(
                                        queryActivityItemProduction,
                                        parametersActivityItemProduction);

                                    var actItemProductionFKRefno =
                                        Convert.ToInt32(drow["ActivityItemProductionFKRefno"]);
                                    transaction.Execute(
                                        "INSERT INTO tActivityItemProductionFinished (ActivityItemRefno, ActivityItemFKRefno, ActivityItemStringFKRefno) VALUES (?, ?, ?)",
                                        actItemRefno,
                                        actItemProductionFKRefno,
                                        Convert.ToString(actItemProductionFKRefno));

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
                                "Saving finished ticket items complete.",
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
                                "Saving finished ticket items failed.",
                                false);

                            throw new BaseException(
                                "Database exception occurred. Saving finished ticket items failed.",
                                ex);
                        }
                    });
        }

        /// <inheritdoc cref="TableBase.CreateRequest(int)"/>
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
                "<action>REQUEST PRODUCTION TICKET FINISHED ITEMS</action>" +
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
    }
}