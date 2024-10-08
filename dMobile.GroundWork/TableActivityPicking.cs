// <copyright file="TableActivityPicking.cs" company="InterSect Business Systems Inc">
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
    using Resources.Models;
    using SQLite;

    /// <summary>
    ///     Defines the <see cref="TableActivityPicking" />.
    /// </summary>
    public class TableActivityPicking : TableBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TableActivityPicking" /> class.
        /// </summary>
        public TableActivityPicking()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityPicking";
            this.CurrentSubType = SysCommon.ActSubTypes.Picking;
            this.LowerCaseName = "sales orders";
            this.ProperCaseName = "Sales orders";
        }

        /// <summary>
        ///     Purge the activity header data.
        /// </summary>
        /// <returns><see cref="Task" /> Task complete.</returns>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(
                this,
                EventArgs.Empty,
                $"Purging {this.LowerCaseName}.",
                true);
            var conn = DatabaseConnection.GetInstance().SqlConn;
            var subType = Convert.ToInt32(this.CurrentSubType);
            await conn.RunInTransactionAsync(
                transaction =>
                {
                    try
                    {
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName}.",
                            true);

                        // Corruption recovery cleanup.
                        var query = @"
                            DELETE FROM tActivityPickingRoute
                                WHERE tActivityPickingRoute.PickingActivityRefno <= 0
                                    OR tActivityPickingRoute.RouteRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                            DELETE FROM tActivityPickingRoute
                                WHERE tActivityPickingRoute.PickingActivityRefno
                                    NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                        transaction.Execute(query);

                        query = @"
                            DELETE FROM tActivityPickingRoute
                                WHERE tActivityPickingRoute.RouteRefno
                                    NOT IN (SELECT tRoute.RouteRefno FROM tRoute)";
                        transaction.Execute(query);

                        query = @"
                            DELETE FROM tActivityPicking
                                WHERE tActivityPicking.ActivityRefno <= 0
                                    OR tActivityPicking.ActivityFKRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                            DELETE FROM tActivityPicking
                                WHERE tActivityPicking.ActivityRefno
                                    NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                        transaction.Execute(query);

                        query = "DELETE FROM tActivity WHERE tActivity.ActivityRefno <= 0";
                        transaction.Execute(query);

                        // Regular purge.
                        query = @"
                            DELETE FROM tActivityPickingRoute
                                WHERE PickingActivityRefno IN
                                    (SELECT ActivityRefno FROM tActivity WHERE ActivitySubType = ?)";
                        transaction.Execute(query, subType);

                        query = "DELETE FROM tActivityPicking WHERE ActivitySubType = ?";
                        transaction.Execute(query, subType);

                        query = "DELETE FROM tActivity WHERE ActivitySubType = ?";
                        transaction.Execute(query, subType);

                        transaction.Commit();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} complete.",
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
                            $"Purging {this.LowerCaseName} failed.",
                            true);
                        throw new Exception(
                            $"Database exception occurred. Purging {this.LowerCaseName} failed.",
                            ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            "Exception occurred.",
                            true);
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} failed.",
                            true);
                        throw new Exception(
                            $"Exception occurred. Purging {this.LowerCaseName} failed.",
                            ex);
                    }
                });
        }

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
                                "Saving sales orders.",
                                false);

                            if (ds.Tables.Count != 0)
                            {
                                var progMax = Convert.ToInt32(
                                    ds.Tables["tActivityPicking"].Rows.Count);

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

                                foreach (DataRow drow in ds.Tables["tActivityPicking"].Rows)
                                {
                                    transaction.Execute(
                                        "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)",
                                        0,
                                        Convert.ToInt32(SysCommon.ActSubTypes.Picking));

                                    var actRefno = Convert.ToInt32(
                                        transaction.ExecuteScalar<int>(
                                            "SELECT last_insert_rowid() FROM tActivity"));
                                    var actPickFkRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                                    var whsLocMapRefno =
                                        this.GetWhsLocMapRefnoFromDataRow(drow, transaction);
                                    const string queryInsertPicking =
                                        "INSERT INTO tActivityPicking (" +
                                        "ActivityRefno, " +
                                        "ActivitySubType, " +
                                        "ActivityFKRefno, " +
                                        "ActivityStringFKRefno, " +
                                        "WarehouseLocationMapRefno, " +
                                        "ActivityPickingOrderedDateTime, " +
                                        "ActivityPickingDeliveryDateTime, " +
                                        "ActivityPickingCustomerContactName, " +
                                        "ActivityPickingCustomerShipToInfo, " +
                                        "ActivityPickingIsReviewed" +
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
                                    var parametersInsertPicking = new object[]
                                    {
                                        actRefno,
                                        Convert.ToInt32(SysCommon.ActSubTypes.Picking),
                                        actPickFkRefno,
                                        Convert.ToString(actPickFkRefno),
                                        whsLocMapRefno,
                                        Convert.ToDateTime(drow["ActivityPickingOrderedDateTime"]),
                                        Convert.ToDateTime(drow["ActivityPickingDeliveryDateTime"]),
                                        Convert.ToString(
                                            drow["ActivityPickingCustomerContactName"]),
                                        Convert.ToString(drow["ActivityPickingCustomerShipToInfo"]),
                                        false
                                    };

                                    transaction.Execute(
                                        queryInsertPicking,
                                        parametersInsertPicking);

                                    var routeRefno = 0;
                                    var pickRouteCode =
                                        Convert.ToString(drow["ActivityPickingRouteCode"]);
                                    if (!string.IsNullOrEmpty(pickRouteCode))
                                    {
                                        var resultRoutes = transaction.Query<RouteModel>(
                                            "SELECT * FROM tRoute WHERE RouteCode = ?",
                                            pickRouteCode);

                                        if (resultRoutes?.Count > 0)
                                        {
                                            routeRefno = resultRoutes[0].RouteRefno;
                                        }
                                        else
                                        {
                                            throw new Exception("drRoute.Read() failed.");
                                        }
                                    }

                                    if (routeRefno != 0)
                                    {
                                        transaction.Execute(
                                            "INSERT INTO tActivityPickingRoute (PickingActivityRefno, RouteRefno, RouteStop) VALUES (?, ?, ?)",
                                            actRefno,
                                            routeRefno,
                                            Convert.ToInt32(drow["ActivityPickingRouteStop"]));
                                    }

                                    progValue++;
                                    if (progValue % 10 == 0)
                                    {
                                        this.DataAccessProgressOutHandler(
                                            this,
                                            EventArgs.Empty,
                                            progValue);
                                    }
                                }

                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax);
                            }

                            transaction.Commit();

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving sales orders complete.",
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
                                "Saving sales orders failed.",
                                true);

                            throw new Exception(
                                "Database exception occurred. Saving sales orders failed.",
                                ex);
                        }
                    });
        }

        /// <summary>
        ///     The CreateRequest.
        /// </summary>
        /// <param name="criteriaFilter"></param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(XDocument criteriaFilter)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false
            };
            var xml = XmlWriter.Create(xmlString, settings);

            XDocument request;
            if (criteriaFilter == null)
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<imclient>" +
                    "<command>" +
                    "<action>REQUEST SALES ORDERS</action>" +
                    await SysCommon.GetRequestDefaults(false) +
                    "<whs_id></whs_id>" +
                    "<sect_refno></sect_refno>" +
                    "</command>" +
                    "</imclient>");
            }
            else
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<imclient>" +
                    "<command>" +
                    "<action>REQUEST SALES ORDERS</action>" +
                    await SysCommon.GetRequestDefaults(false) +
                    "<whs_id>" +
                    criteriaFilter.Root?.Element("whs_id")?.Value +
                    "</whs_id>" +
                    "<sect_refno>" +
                    criteriaFilter.Root?.Element("sect_refno")?.Value +
                    "</sect_refno>" +
                    "</command>" +
                    "</imclient>");
            }

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

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
                "<action>REQUEST SALES ORDERS</action>" +
                await SysCommon.GetRequestDefaults(false) +
                "<whs_id></whs_id>" +
                "<sect_refno></sect_refno>" +
                "</command>" +
                "</imclient>");
            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        protected override Task<string> CreateRequest(string familyCode)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityFkRefno)
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

        protected override Task Save(string responseData, int refno)
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