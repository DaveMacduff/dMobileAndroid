// <copyright file="TableActivityReturns.cs" company="InterSect Business Systems Inc">
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

    public class TableActivityReturns : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityReturns"/> class.
        /// </summary>
        public TableActivityReturns()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReturns";
            this.CurrentSubType = SysCommon.ActSubTypes.Returns;
            this.LowerCaseName = "sales order returns";
            this.ProperCaseName = "Sales order returns";
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
                        DELETE FROM tActivityReturns
                            WHERE tActivityReturns.ActivityRefno <= 0
                                OR tActivityReturns.ActivityFKRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityReturns
                            WHERE tActivityReturns.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                        transaction.Execute(query);

                        query = "DELETE FROM tActivity WHERE tActivity.ActivityRefno <= 0";
                        transaction.Execute(query);

                        // Regular purge.
                        query = "DELETE FROM tActivityReturns WHERE ActivitySubType = ?";
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="responseData"></param>
        /// <remarks></remarks>
        /// <inheritdoc/>
        protected override async Task Save(string responseData)
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
                                    "Reading received responseData.",
                                    false);

                                ds.ReadXml(
                                    SysCommon.GetXmlReader(ref responseData),
                                    XmlReadMode.Auto);

                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    "Saving sales returns.",
                                    false);

                                if (ds.Tables.Count != 0)
                                {
                                    var progMax = Convert.ToInt32(
                                        ds.Tables["tActivityReturns"].Rows.Count);

                                    this.DataAccessStatusOutHandler(
                                        this,
                                        EventArgs.Empty,
                                        $"Records {progMax}",
                                        false);

                                    var actRefno = 0;
                                    var progValue = 0;
                                    var whsLocMapRefno = 0;
                                    var returnsFKRefno = 0;

                                    this.DataAccessProgressOutHandler(
                                        this,
                                        EventArgs.Empty,
                                        0,
                                        0,
                                        progMax);

                                    foreach (DataRow drow in ds.Tables["tActivityReturns"].Rows)
                                    {
                                        transaction.Execute(
                                            "INSERT INTO tActivity (ActivitySubType, ActivityStatus) VALUES (?, ?)",
                                            Convert.ToInt32(SysCommon.ActSubTypes.Returns),
                                            0);

                                        actRefno = Convert.ToInt32(
                                            transaction.ExecuteScalar<int>(
                                                "SELECT last_insert_rowid() FROM tActivity"));
                                        returnsFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                                        whsLocMapRefno =
                                            this.GetWhsLocMapRefnoFromDataRow(drow, transaction);

                                        var query = "INSERT INTO tActivityReturns (" +
                                                    "ActivityRefno, " +
                                                    "ActivitySubType, " +
                                                    "ActivityFKRefno, " +
                                                    "ActivityStringFKRefno, " +
                                                    "WarehouseLocationMapRefno, " +
                                                    "ActivityReturnsOrderedDateTime, " +
                                                    "ActivityReturnsDeliveryDateTime, " +
                                                    "ActivityReturnsCustomerContactName, " +
                                                    "ActivityReturnsCustomerShipToInfo" +
                                                    ") VALUES (" +
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
                                            Convert.ToInt32(actRefno),
                                            Convert.ToInt32(SysCommon.ActSubTypes.Returns),
                                            Convert.ToInt32(returnsFKRefno),
                                            Convert.ToString(returnsFKRefno),
                                            Convert.ToInt32(whsLocMapRefno),
                                            Convert.ToDateTime(
                                                drow["ActivityReturnsOrderedDateTime"]),
                                            Convert.ToDateTime(
                                                drow["ActivityReturnsDeliveryDateTime"]),
                                            Convert.ToString(
                                                drow["ActivityReturnsCustomerContactName"]),
                                            Convert.ToString(
                                                drow["ActivityReturnsCustomerShipToInfo"])
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
                            }

                            transaction.Commit();

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving sales returns complete.",
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
                                "Saving sales returns failed.",
                                true);

                            throw new Exception(
                                "Database exception occurred. Saving sales returns failed.",
                                ex);
                        }
                    });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
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
                "<action>REQUEST SALES RETURNS</action>" +
                await SysCommon.GetRequestDefaults(false) +
                "</command>" +
                "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        protected override Task<string> CreateRequest(XDocument criteriaFilter)
        {
            throw new NotImplementedException();
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