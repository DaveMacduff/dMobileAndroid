// <copyright file="TableActivityProductionTicketByProduct.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityProductionTicketByProduct" />.
    /// </summary>
    public class TableActivityProductionTicketByProduct : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketByProduct"/> class.
        /// </summary>
        public TableActivityProductionTicketByProduct()
            : base()
        {
            this.CurrentSubType = SysCommon.ActSubTypes.TicketAddProduct;
            this.LowerCaseName = "ticket additional products";
            this.ProperCaseName = "Ticket additional products";
        }

        /// <inheritdoc cref="TableBase.PurgeData()"/>
        public override async Task PurgeData()
        {
            var subType = Convert.ToInt32(this.CurrentSubType);
            this.DataAccessStatusOutHandler(
                this,
                EventArgs.Empty,
                $"Purging {this.LowerCaseName} details.",
                true);
            await Conn.RunInTransactionAsync(
                transaction =>
                {
                    try
                    {
                        var query = @"
                        DELETE FROM tActivityProductionByProduct
                            WHERE tActivityProductionByProduct.ActivityRefno <= ?
                                OR tActivityProductionByProduct.ActivityFKRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityProductionByProduct
                            WHERE tActivityProductionByProduct.ActivityRefno
                                NOT IN (SELECT tActivityProduction.ActivityRefno FROM tActivityProduction)";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityProductionByProduct
                            WHERE tActivityProductionByProduct.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityProduction
                            WHERE tActivityProduction.ActivityRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityProduction
                            WHERE tActivityProduction.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                        transaction.Execute(query);

                        query = "DELETE FROM tActivity WHERE tActivity.ActivityRefno <= 0";
                        transaction.Execute(query);

                        // Regular purge.
                        query = "DELETE FROM tActivityProductionByProduct";
                        transaction.Execute(query, subType);

                        query =
                            "DELETE FROM tActivityProduction WHERE tActivityProduction.ActivitySubType = ?";
                        transaction.Execute(query, subType);

                        query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                        transaction.Execute(query, subType);

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
                                "Saving ticket add-products.",
                                false);

                            if (ds.Tables.Count != 0)
                            {
                                var progMax = Convert.ToInt32(
                                    ds.Tables["tActivityProduction"].Rows.Count);

                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    $"Records {progMax}",
                                    false);

                                var actRefno = 0;
                                var progValue = 0;
                                var whsLocMapRefno = 0;

                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    0,
                                    0,
                                    progMax);

                                foreach (DataRow drow in ds.Tables["tActivityProduction"].Rows)
                                {
                                    transaction.Execute(
                                        "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)",
                                        0,
                                        Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                                    actRefno = Convert.ToInt32(
                                        transaction.ExecuteScalar<int>(
                                            "SELECT last_insert_rowid() FROM tActivity"));
                                    whsLocMapRefno =
                                        GetWhsLocMapRefnoFromDataRow(drow, transaction);

                                    // To remove time - convert twice
                                    var productionDateTime = Convert
                                        .ToDateTime(drow["ActivityProductionDateTime"])
                                        .ToShortDateString();
                                    var productionDesc =
                                        drow["ActivityProductionDescription"].ToString();
                                    const string queryActivityProduction =
                                        "INSERT INTO tActivityProduction (" +
                                        "ActivityRefno, " +
                                        "ActivitySubType, " +
                                        "WarehouseLocationMapRefno, " +
                                        "ActivityProductionDateTime, " +
                                        "ActivityProductionDescription" +
                                        ") VALUES (" +
                                        "?, " +
                                        "?, " +
                                        "?, " +
                                        "?, " +
                                        "?" +
                                        ")";
                                    var parametersActivityProduction = new object[]
                                    {
                                        actRefno,
                                        Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct),
                                        whsLocMapRefno,
                                        DateTime.Parse(productionDateTime),
                                        productionDesc
                                    };

                                    transaction.Execute(
                                        queryActivityProduction,
                                        parametersActivityProduction);

                                    var productionFKRefno =
                                        Convert.ToInt32(drow["ActivityFKRefno"]);
                                    const string queryActivityProductionByProduct =
                                        "INSERT INTO tActivityProductionByProduct (" +
                                        "ActivityRefno, " +
                                        "ActivityFKRefno, " +
                                        "ActivityStringFKRefno" +
                                        ") VALUES (" +
                                        "?, " +
                                        "?, " +
                                        "?" +
                                        ")";
                                    var parametersActivityProductionByProduct = new object[]
                                    {
                                        actRefno,
                                        productionFKRefno,
                                        Convert.ToString(productionFKRefno)
                                    };

                                    transaction.Execute(
                                        queryActivityProductionByProduct,
                                        parametersActivityProductionByProduct);

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
                                "Saving ticket add-products complete.",
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
                                "Saving ticket add-products failed.",
                                false);
                            throw new BaseException(
                                "Database exception occurred. Saving ticket add-products failed.",
                                ex);
                        }
                    });
        }

        /// <inheritdoc cref="TableBase.CreateRequest()"/>
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
                "<action>REQUEST PRODUCTION TICKET BYPRODUCT</action>" +
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