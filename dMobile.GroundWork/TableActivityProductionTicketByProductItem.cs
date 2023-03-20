// <copyright file="TableActivityProductionTicketByProductItem.cs" company="InterSect Business Systems Inc">
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
    using dMobile.Resources.Models;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityProductionTicketByProductItem" />.
    /// </summary>
    public class TableActivityProductionTicketByProductItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketByProductItem"/> class.
        /// </summary>
        public TableActivityProductionTicketByProductItem()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketByProductItem";
            this.CurrentSubType = SysCommon.ActSubTypes.TicketAddProduct;
            this.LowerCaseName = "ticket additional product";
            this.ProperCaseName = "Ticket additional product";
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product items.", true);

                var query = "DELETE FROM tActivityItemBarCodeAssociationXRef "
                            + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
                            + "(SELECT tActivityItem.ActivityItemRefno "
                            + "FROM tActivityItem "
                            + "WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                query = "DELETE FROM tActivityItemProductionByProduct "
                    + "WHERE tActivityItemProductionByProduct.ActivityItemRefno IN "
                    + "(SELECT tActivityItem.ActivityItemRefno "
                    + "FROM tActivityItem "
                    + "WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                query = "DELETE FROM tActivityItemProduction "
                    + "WHERE tActivityItemProduction.ActivityItemRefno IN "
                    + "(SELECT tActivityItem.ActivityItemRefno "
                    + "FROM tActivityItem "
                    + "WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product items failed.", true);

                throw new Exception("Database exception occurred. Purging ticket add-product items failed.", ex);
            }
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <param name="actItemRefno"></param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int actItemRefno)
        {
            var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityProductionByProductModel>("SELECT * FROM tActivityProductionByProduct WHERE ActivityRefno = ?", actItemRefno);

            int productionFKRefno;
            if (results?.Count > 0)
            {
                productionFKRefno = results[0].ActivityFKRefno;
            }
            else
            {
                throw new Exception("drByProduct.Read() failed.");
            }

            return await this.SendComm(await this.CreateRequest(productionFKRefno));
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

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product items.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["tActivityItemProduction"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var actItemRefno = 0;
                        var progValue = 0;
                        var locMapRefno = 0;
                        var actItemProductionFKRefno = 0;
                        var prodUnitRefno = 0;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in ds.Tables["tActivityItemProduction"].Rows)
                        {
                            transaction.Execute("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct), Convert.ToInt32(SysCommon.ActStatus.NoChange), actRefno);

                            actItemRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                            actItemProductionFKRefno = Convert.ToInt32(drow["ActivityItemProductionFKRefno"]);
                            prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, transaction, false);
                            locMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, transaction);

                            var queryActivityItemProduction = "INSERT INTO tActivityItemProduction " +
                                                              "(" +
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
                                                              ") " +
                                                              "VALUES " +
                                                              "(" +
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
                                    Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct),
                                    prodUnitRefno,
                                    locMapRefno,
                                    Convert.ToDecimal(drow["ActivityItemProductionQuantity"]),
                                    Convert.ToDecimal(drow["ActivityItemProductionBaseQuantity"]),
                                    Convert.ToDecimal(drow["ActivityItemProductionWeightQuantity"]),
                                    Convert.ToString(drow["ActivityItemProductionDescription"]),
                                    Convert.ToDecimal(drow["ActivityItemProductionQuantityExpected"]),
                                    Convert.ToDecimal(drow["ActivityItemProductionBaseQuantityExpected"]),
                                    Convert.ToDecimal(drow["ActivityItemProductionWeightQuantityExpected"]),
                            };

                            transaction.Execute(queryActivityItemProduction, parametersActivityItemProduction);

                            var queryActivityItemProductionByProduct = "INSERT INTO tActivityItemProductionByProduct " +
                                                                       "(" +
                                                                       "ActivityItemRefno, " +
                                                                       "ActivityItemFKRefno, " +
                                                                       "ActivityItemStringFKRefno" +
                                                                       ") VALUES (" +
                                                                       "?, " +
                                                                       "?, " +
                                                                       "?" +
                                                                       ")";
                            var parametersActivityItemProductionByProduct = new object[]
                            {
                                    actItemRefno,
                                    actItemProductionFKRefno,
                                    Convert.ToString(actItemProductionFKRefno),
                            };

                            transaction.Execute(queryActivityItemProductionByProduct, parametersActivityItemProductionByProduct);

                            progValue++;
                            if (progValue % 100 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                            }
                        }

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                    }

                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product items complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product items failed.", false);

                    throw new Exception("Database exception occurred. Saving ticket add-product items failed.", ex);
                }
            });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="activityItemFkRefno"></param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(int activityItemFkRefno)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCTION TICKET BYPRODUCT ITEMS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<refno>" + activityItemFkRefno.ToString() + "</refno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
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
