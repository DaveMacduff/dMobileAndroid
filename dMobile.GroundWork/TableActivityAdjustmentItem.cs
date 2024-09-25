// <copyright file="TableActivityAdjustmentItem.cs" company="InterSect Business Systems Inc">
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
    ///     Defines the <see cref="TableActivityAdjustmentItem" />.
    /// </summary>
    public class TableActivityAdjustmentItem : TableBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TableActivityAdjustmentItem" /> class.
        /// </summary>
        public TableActivityAdjustmentItem()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityAdjustmentItem";
            this.CurrentSubType = SysCommon.ActSubTypes.Adjustment;
            this.LowerCaseName = "adjustment";
            this.ProperCaseName = "Adjustment";
        }

        /// <summary>
        ///     The Purge.
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
                        DELETE FROM tActivityItemAdjustment
                            WHERE tActivityItemAdjustment.ActivityItemRefno <= 0
                                OR tActivityItemAdjustment.ActivityItemFKRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItemAdjustment
                            WHERE tActivityItemAdjustment.ActivityItemRefno
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

                        query = "DELETE FROM tActivityItemAdjustment WHERE ActivityItemSubType = ?";
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

        protected override async Task<string> SendRequest(int activityFkRefno)
        {
            var results = await DatabaseConnection.GetInstance()
                .SqlConn.QueryAsync<ActivityAdjustmentModel>(
                    "SELECT * FROM tActivityAdjustment WHERE ActivityFKRefno = ?",
                    activityFkRefno);
            int actAdjFkRefno;
            if (results?.Count > 0)
            {
                actAdjFkRefno = results[0].ActivityFKRefno;
            }
            else
            {
                throw new Exception(
                    "No results in tActivityAdjustment for ActivityRefno " + activityFkRefno);
            }

            return await this.SendComm(await this.CreateRequest(actAdjFkRefno));
        }

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
                                "Saving adjustment items.",
                                false);

                            if (ds.Tables.Count != 0)
                            {
                                var progMax = Convert.ToInt32(
                                    ds.Tables["tActivityItemAdjustment"].Rows.Count);

                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    $"Records {progMax}",
                                    false);

                                var actItemRefno = 0;
                                var progValue = 0;
                                var actItemAdjFKRefno = 0;
                                var prodUnitRefno = 0;
                                var locMapRefno = 0;

                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    0,
                                    0,
                                    progMax);

                                foreach (DataRow drow in ds.Tables["tActivityItemAdjustment"].Rows)
                                {
                                    transaction.Execute(
                                        "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus," +
                                        " ActivityRefno, ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                                        Convert.ToInt32(SysCommon.ActSubTypes.Adjustment),
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
                                    actItemAdjFKRefno =
                                        Convert.ToInt32(drow["ActivityItemFKRefno"]);

                                    var query = "INSERT INTO tActivityItemAdjustment " +
                                                "(ActivityItemRefno, " +
                                                "ActivityItemSubType, " +
                                                "ActivityItemFKRefno, " +
                                                "ActivityItemStringFKRefno, " +
                                                "ProductUnitRefno, " +
                                                "WarehouseLocationMapRefno, " +
                                                "ActivityItemAdjustmentQuantity, " +
                                                "ActivityItemAdjustmentBaseQuantity, " +
                                                "ActivityItemAdjustmentWeightQuantity, " +
                                                "ActivityItemAdjustmentDescription) " +
                                                "VALUES " +
                                                "(?, " +
                                                "?, " +
                                                "?, " +
                                                "?, " +
                                                "?, " +
                                                "?, " +
                                                "?, " +
                                                "?, " +
                                                "?, " +
                                                "?)";
                                    var parameters = new object[]
                                    {
                                        actItemRefno,
                                        Convert.ToInt32(SysCommon.ActSubTypes.Adjustment),
                                        actItemAdjFKRefno,
                                        Convert.ToString(actItemAdjFKRefno),
                                        prodUnitRefno,
                                        locMapRefno,
                                        Convert.ToDecimal(drow["ActivityItemAdjustmentQuantity"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemAdjustmentBaseQuantity"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemAdjustmentWeightQuantity"]),
                                        Convert.ToString(drow["ActivityItemAdjustmentDescription"])
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
                                "Saving adjustment items complete.",
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
                                "saving adjustment items failed.",
                                true);

                            throw new Exception(
                                "Database exception occurred. Saving adjustment items failed.",
                                ex);
                            throw new Exception(
                                "Database exception occurred. Saving adjustment items failed.",
                                ex);
                        }
                    });
        }

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
                "<action>REQUEST ADJUSTMENT ITEMS</action>" +
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