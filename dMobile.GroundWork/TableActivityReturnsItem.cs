// <copyright file="TableActivityReturnsItem.cs" company="InterSect Business Systems Inc">
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

    public class TableActivityReturnsItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityReturnsItem"/> class.
        /// </summary>
        public TableActivityReturnsItem()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReturnsItem";
            this.CurrentSubType = SysCommon.ActSubTypes.Returns;
            this.LowerCaseName = "sales order return";
            this.ProperCaseName = "Sales order return";
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
                        DELETE FROM tActivityItemReasonCodeXRef
                            WHERE tActivityItemReasonCodeXRef.ActivityItemRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItemReasonCodeXRef
                            WHERE tActivityItemReasonCodeXRef.ActivityItemRefno
                                NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItemReturnsComment
                            WHERE tActivityItemReturnsComment.ActivityItemRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItemReturnsComment
                            WHERE tActivityItemReturnsComment.ActivityItemRefno
                                NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItemReturns
                            WHERE tActivityItemReturns.ActivityItemRefno <= 0
                                OR tActivityItemReturns.ActivityItemFKRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItemReturns
                            WHERE tActivityItemReturns.ActivityItemRefno
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
                        query = "DELETE FROM tActivityItemBarCodeAssociationXRef " +
                                "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN " +
                                "(SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?)";
                        transaction.Execute(query, this.CurrentSubType);

                        query = @"DELETE FROM tActivityItemReasonCodeXRef
                                WHERE tActivityItemReasonCodeXRef.ActivityItemRefno IN
                                (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?)";
                        transaction.Execute(query, this.CurrentSubType);

                        query = @"DELETE FROM tActivityItemReturnsComment
                                WHERE tActivityItemReturnsComment.ActivityItemRefno IN
                                (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?)";
                        transaction.Execute(query, this.CurrentSubType);

                        query =
                            "DELETE FROM tActivityItemReturns WHERE tActivityItemReturns.ActivityItemSubType = ?";
                        transaction.Execute(query, this.CurrentSubType);

                        query =
                            "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                        transaction.Execute(query, this.CurrentSubType);

                        transaction.Commit();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} completed.",
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
                .SqlConn.QueryAsync<ActivityReturnsModel>(
                    "SELECT * FROM tActivityReturns WHERE ActivityFKRefno = ?",
                    Convert.ToInt32(activityFkRefno));

            int returnsFKRefno;
            if (results?.Count > 0)
            {
                returnsFKRefno = results[0].ActivityFKRefno;
            }
            else
            {
                throw new Exception("drReturns.Read() failed.");
            }

            return await this.SendComm(await this.CreateRequest(returnsFKRefno));
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
                                    "Saving sales return items.",
                                    false);

                                if (ds.Tables.Count != 0)
                                {
                                    var progMax = Convert.ToInt32(
                                        ds.Tables["tActivityItemReturns"].Rows.Count);

                                    this.DataAccessStatusOutHandler(
                                        this,
                                        EventArgs.Empty,
                                        $"Records {progMax}",
                                        false);

                                    var actItemRefno = 0;
                                    var progValue = 0;
                                    var prodUnitRefno = 0;
                                    var locMapRefno = 0;
                                    var actItemReturnsFKRefno = 0;

                                    var actItemRetComment = string.Empty;

                                    this.DataAccessProgressOutHandler(
                                        this,
                                        EventArgs.Empty,
                                        0,
                                        0,
                                        progMax);

                                    foreach (DataRow drow in ds.Tables["tActivityItemReturns"].Rows)
                                    {
                                        transaction.Execute(
                                            "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus," +
                                            " ActivityRefno, ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                                            Convert.ToInt32(this.CurrentSubType),
                                            Convert.ToInt32(SysCommon.ActStatus.NoChange),
                                            actRefno,
                                            0);

                                        actItemRefno = Convert.ToInt32(
                                            transaction.ExecuteScalar<int>(
                                                "SELECT last_insert_rowid() FROM tActivityItem"));
                                        prodUnitRefno = this.GetProductUnitRefnoFromDataRow(
                                            drow,
                                            transaction,
                                            false);
                                        locMapRefno = this.GetWhsLocMapRefnoFromDataRow(
                                            drow,
                                            transaction);
                                        actItemReturnsFKRefno =
                                            Convert.ToInt32(drow["ActivityItemFKRefno"]);
                                        var queryInsertItem = "INSERT INTO tActivityItemReturns (" +
                                                              "ActivityItemRefno, " +
                                                              "ActivityItemSubType, " +
                                                              "ActivityItemFKRefno, " +
                                                              "ActivityItemStringFKRefno, " +
                                                              "ProductUnitRefno, " +
                                                              "WarehouseLocationMapRefno, " +
                                                              "ActivityItemReturnsQuantityReturned, " +
                                                              "ActivityItemReturnsBaseQuantityReturned, " +
                                                              "ActivityItemReturnsWeightQuantityReturned, " +
                                                              "ActivityItemReturnsCustomerProductUnitCode, " +
                                                              "ActivityItemReturnsDescription" +
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
                                        var parametersInsertItem = new object[]
                                        {
                                            Convert.ToInt32(actItemRefno),
                                            Convert.ToInt32(this.CurrentSubType),
                                            Convert.ToInt32(actItemReturnsFKRefno),
                                            Convert.ToString(actItemReturnsFKRefno),
                                            Convert.ToInt32(prodUnitRefno),
                                            Convert.ToInt32(locMapRefno),
                                            Convert.ToDecimal(
                                                drow["ActivityItemReturnsQuantityReturned"]),
                                            Convert.ToDecimal(
                                                drow["ActivityItemReturnsBaseQuantityReturned"]),
                                            Convert.ToDecimal(
                                                drow["ActivityItemReturnsWeightQuantityReturned"]),
                                            Convert.ToString(
                                                drow["ActivityItemReturnsCustomerProductUnitCode"]),
                                            Convert.ToString(drow["ActivityItemReturnsDescription"])
                                        };

                                        transaction.Execute(queryInsertItem, parametersInsertItem);

                                        actItemRetComment = Convert.ToString(
                                            drow["ActivityItemReturnsComment"]);

                                        if (!string.IsNullOrEmpty(actItemRetComment))
                                        {
                                            transaction.Execute(
                                                "INSERT INTO tActivityItemReturnsComment (ActivityItemRefno, ActivityItemReturnsCommentComment) VALUES (?, ?)",
                                                Convert.ToInt32(actItemRefno),
                                                Convert.ToString(actItemRetComment));
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
                            }

                            transaction.Commit();

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving sales return items complete.",
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
                                "Saving sales return items failed.",
                                false);

                            throw new Exception(
                                "Database exception occurred. Saving sales return items failed.",
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
                "<action>REQUEST SALES RETURN ITEMS</action>" +
                await SysCommon.GetRequestDefaults(false) +
                "<refno>" +
                activityFkRefno +
                "</refno>" +
                "</command>" +
                "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
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