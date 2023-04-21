// <copyright file="TableActivityPurchaseOrderWorkSheetItem.cs" company="InterSect Business Systems Inc">
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
    using dMobile.Bedrock;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityPurchaseOrderWorkSheetItem" />.
    /// </summary>
    public class TableActivityPurchaseOrderWorkSheetItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityPurchaseOrderWorkSheetItem"/> class.
        /// </summary>
        public TableActivityPurchaseOrderWorkSheetItem()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityPurchaseOrderWorkSheetItem";
            this.CurrentSubType = SysCommon.ActSubTypes.PurchaseOrderWorkSheet;
            this.LowerCaseName = "work sheet";
            this.ProperCaseName = "Work sheet";
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging work sheet items.", true);

                var query = "DELETE FROM tActivityItemBarCodeAssociationXRef WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno " +
                            "IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet));

                query = "DELETE FROM tActivityItemPurchaseOrderWorkSheet WHERE tActivityItemPurchaseOrderWorkSheet.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet));

                query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging work sheet items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging work sheet items failed.", true);

                throw new Exception("Database exception occurred. Purging work sheet items failed.", ex);
            }
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <param name="actItemRefno">The actItemRefno<see cref="int"/>.</param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int actItemRefno)
        {
            var actWorkSheetFKRefno = 0;
            return await this.SendComm(await this.CreateRequest(actWorkSheetFKRefno));
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData"></param>
        /// <param name="actRefno">The actItemRefno<see cref="int"/>.</param>
        protected override async Task Save(string responseData, int actRefno)
        {
            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving work sheet items.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["tActivityItemPurchaseOrderWorkSheet"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var actItemRefno = 0;
                        var progValue = 0;
                        var whsLocMapRefno = 0;
                        var actItemWorkSheetFKRefno = 0;
                        var prodUnitRefno = 0;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in ds.Tables["tActivityItemPurchaseOrderWorkSheet"].Rows)
                        {
                            transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet));
                            actRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivity"));
                            transaction.Execute(
                                "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno," +
                                " ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                                Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet),
                                Convert.ToInt32(SysCommon.ActStatus.NoChange),
                                actRefno,
                                0);
                            actItemRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                            prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, transaction, false);
                            whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, transaction);

                            actItemWorkSheetFKRefno = Convert.ToInt32(drow["ActivityItemFKRefno"]);

                            var query = "INSERT INTO tActivityItemPurchaseOrderWorkSheet (" +
                                        "ActivityItemRefno, " +
                                        "ActivityItemSubType, " +
                                        "ActivityItemFKRefno, " +
                                        "ActivityItemStringFKRefno, " +
                                        "ProductUnitRefno, " +
                                        "WarehouseLocationMapRefno, " +
                                        "ActivityItemPurchaseOrderWorkSheetQuantityOrdered, " +
                                        "ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered, " +
                                        "ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered" +
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
                                    Convert.ToInt32(actItemRefno),
                                    Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet),
                                    Convert.ToInt32(actItemWorkSheetFKRefno),
                                    Convert.ToString(actItemWorkSheetFKRefno),
                                    Convert.ToInt32(prodUnitRefno),
                                    Convert.ToInt32(whsLocMapRefno),
                                    Convert.ToDecimal(drow["ActivityItemPurchaseOrderWorkSheetQuantityOrdered"]),
                                    Convert.ToDecimal(drow["ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered"]),
                                    Convert.ToDecimal(drow["ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered"]),
                            };

                            transaction.Execute(query, parameters);

                            progValue++;
                            if (progValue % 100 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                            }
                        }

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                    }

                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving work sheet items complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving work sheet items failed.", false);
                    throw new Exception("Database exception occurred. Saving work sheet items failed.", ex);
                }
            });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="activityItemFkRefno">The activityItemFkRefno<see cref="int"/>.</param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(int activityItemFkRefno)
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
                + "<action>REQUEST PURCHASE ORDER WORK SHEET ITEMS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<refno>" + activityItemFkRefno + "</refno>"
                + "</command>"
                + "<whs_id>" + LocationMap.GetInstance().CurrLocMap.WhsFKRefno + "</whs_id>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        protected override Task<string> CreateRequest()
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(XDocument criteria)
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

        protected override Task<string> CreateRequest(int activityItemFkRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int prodLocMapType, int whsLocMapFKRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequestProdLoc(int prodLocMapType)
        {
            throw new NotImplementedException();
        }
    }
}
