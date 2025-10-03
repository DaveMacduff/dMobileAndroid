// <copyright file="TableActivityReceiving.cs" company="InterSect Business Systems Inc">
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
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityReceiving" />.
    /// </summary>
    public class TableActivityReceiving : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityReceiving"/> class.
        /// </summary>
        public TableActivityReceiving()
            : base()
        {
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
                $"Purging {this.LowerCaseName}.",
                true);
            var subType = Convert.ToInt32(this.CurrentSubType);
            await Conn.RunInTransactionAsync(
                transaction =>
                {
                    try
                    {
                        // Corruption recovery cleanup.
                        var query = @"
                        DELETE FROM tActivityReceiving
                            WHERE tActivityReceiving.ActivityRefno <= 0
                                OR tActivityReceiving.ActivityFKRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityReceiving
                            WHERE tActivityReceiving.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                        transaction.Execute(query);

                        query = "DELETE FROM tActivity WHERE tActivity.ActivityRefno <= 0";
                        transaction.Execute(query);

                        // REGULAR PURGE
                        query =
                            "DELETE FROM tActivityReceiving WHERE tActivityReceiving.ActivitySubType = ?";
                        transaction.Execute(query, subType);

                        query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                        transaction.Execute(query, subType);

                        transaction.Commit();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} complete.",
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

        /// <inheritdoc cref="TableBase.Save(string)"/>
        protected override async Task Save(string responseData)
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
                                "Reading received responseData.",
                                false);

                            ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving receiving.",
                                false);

                            if (ds.Tables.Count != 0)
                            {
                                var progMax = Convert.ToInt32(
                                    ds.Tables["tActivityPurchaseOrder"].Rows.Count);

                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    $"Records {progMax}",
                                    false);

                                var actRefno = 0;
                                var progValue = 0;
                                var whsLocMapRefno = 0;
                                var recvFKRefno = 0;
                                var recvPONumber = string.Empty;

                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    0,
                                    0,
                                    progMax);

                                foreach (DataRow drow in ds.Tables["tActivityPurchaseOrder"].Rows)
                                {
                                    transaction.Execute(
                                        "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)",
                                        0,
                                        Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                                    actRefno = Convert.ToInt32(
                                        transaction.ExecuteScalar<int>(
                                            "SELECT last_insert_rowid() FROM tActivity"));
                                    recvFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                                    recvPONumber = drow["ActivityPurchaseOrderPONumber"].ToString();
                                    whsLocMapRefno =
                                        GetWhsLocMapRefnoFromDataRow(drow, transaction);

                                    const string query = "INSERT INTO tActivityReceiving (" +
                                                "ActivityRefno, " +
                                                "ActivitySubType, " +
                                                "ActivityFKRefno, " +
                                                "ActivityStringFKRefno, " +
                                                "ActivityReceivingPONumber, " +
                                                "WarehouseLocationMapRefno, " +
                                                "ActivityReceivingIssuedDateTime, " +
                                                "ActivityReceivingArrivalDateTime, " +
                                                "ActivityReceivingVendorContactName, " +
                                                "ActivityReceivingVendorSupplierContactName, " +
                                                "ActivityReceivingVendorSupplierGTINCompanyPrefix" +
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
                                    var parameters = new object[]
                                    {
                                        actRefno,
                                        Convert.ToInt32(SysCommon.ActSubTypes.Receiving),
                                        recvFKRefno,
                                        Convert.ToString(recvFKRefno),
                                        recvPONumber,
                                        whsLocMapRefno,
                                        Convert.ToDateTime(
                                            drow["ActivityPurchaseOrderIssuedDateTime"]),
                                        Convert.ToDateTime(
                                            drow["ActivityPurchaseOrderArrivalDateTime"]),
                                        drow["ActivityPurchaseOrderVendorContactName"].ToString(),
                                        drow["ActivityPurchaseOrderVendorSupplierContactName"]
                                            .ToString(),
                                        drow["ActivityPurchaseOrderVendorSupplierGTINCompanyPrefix"]
                                            .ToString()
                                    };

                                    transaction.Execute(query, parameters);

                                    progValue++;
                                    if (progValue % 10 == 0)
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
                                "Saving receiving complete.",
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
                                "Saving receiving failed.",
                                false);
                            throw new BaseException(
                                "Database exception occurred. Saving receiving failed.",
                                ex);
                        }
                    });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> CreateRequest()
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
                "<action>REQUEST PURCHASE ORDERS</action>" +
                await SysCommon.GetRequestDefaults(false) +
                "</command>" +
                "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }
    }
}