namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using dMobile.Resources.Models;

    /// <summary>
    /// Defines the <see cref="TableActivityProductionTicketByProductItem" />
    /// </summary>
    public class TableActivityProductionTicketByProductItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketByProductItem"/> class.
        /// </summary>
        public TableActivityProductionTicketByProductItem()
      : base()
        {
            FullClassName = "dMobile.GroundWork.TableActivityProductionTicketByProductItem";
        }

        /// <summary>
        /// The Request
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/></param>
        public override async Task Request(int actRefno)
        {
            string reqData = string.Empty;

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product items.", true);

                if (actRefno == 0)
                {
                    List<tActivityModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityModel>("SELECT ActivityRefno FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                    if (results?.Count > 0)
                    {
                        foreach (tActivityModel activity in results)
                        {
                            actRefno = Convert.ToInt32(activity.ActivityRefno);
                            reqData = await this.Send(actRefno);

                            if (0 < reqData.Length)
                            {
                                if (reqData.Contains("<message>ERROR</message>"))
                                {
                                    XmlDocument reqErr = new XmlDocument();
                                    reqErr.LoadXml(reqData);
                                    throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                                }

                                await this.Save(reqData, actRefno);
                            }
                        }
                    }
                }
                else
                {
                    reqData = await this.Send(actRefno);

                    if (0 < reqData.Length)
                    {
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            XmlDocument reqErr = new XmlDocument();
                            reqErr.LoadXml(reqData);
                            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                        }

                        await this.Save(reqData, actRefno);
                    }
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding ticket add-product items failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-product items failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding ticket add-product items failed.", ex);
            }
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override async Task Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product items.", true);

                string query = "DELETE FROM tActivityItemBarCodeAssociationXRef "
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

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging ticket add-product items failed.", ex);
            }
        }

        /// <summary>
        /// The DataStatus
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public override async Task<int> DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.QueryScalarsAsync<int>("SELECT COUNT(ActivityItemRefno) FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct)));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} ticket add-product items.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Ticket add-product items status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actRefno"></param>
        /// <returns></returns>
        protected override async Task<string> Send(int actRefno)
        {
            int productionFKRefno = 0;
            List<tActivityProductionByProductModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityProductionByProductModel>("SELECT * FROM tActivityProductionByProduct WHERE ActivityRefno = ?", actRefno);

            if (results?.Count > 0)
            {
                productionFKRefno = results[0].ActivityProductionFKRefno;
            }
            else
            {
                // TODO: Resource strings
                throw new Exception("drByProduct.Read() failed.");
            }

            return await this.SendComm(await Create(productionFKRefno));
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actRefno">The actRefno<see cref="int"/></param>
        protected override async Task Save(string reqData, int actRefno)
        {
            string errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product items.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityItemProduction"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actItemRefno = 0;
                            int progValue = 0;
                            int locMapRefno = 0;
                            int actItemProductionFKRefno = 0;
                            int prodUnitRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityItemProduction"].Rows)
                            {
                                errMsg = "Arguments:";

                                transaction.Execute("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct), Convert.ToInt32(SysCommon.ActStatus.NoChange), actRefno);

                                actItemRefno = Convert.ToInt32(transaction.QueryScalars<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                                actItemProductionFKRefno = Convert.ToInt32(drow["ActivityItemProductionFKRefno"]);
                                errMsg += Environment.NewLine + "Item fk refno " + actItemProductionFKRefno;

                                prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, transaction, ref errMsg, false);
                                locMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, transaction);

                                string queryActivityItemProduction = "INSERT INTO tActivityItemProduction " +
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

                                List<object> parametersActivityItemProduction = new List<object>()
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

                                string queryActivityItemProductionByProduct = "INSERT INTO tActivityItemProductionByProduct " +
                                    "(" +
                                    "ActivityItemRefno, " +
                                    "ActivityItemProductionByProductFKRefno, " +
                                    "ActivityItemProductionByProductStringFKRefno" +
                                    ") VALUES (" +
                                    "?, " +
                                    "?, " +
                                    "?" +
                                    ")";
                                List<object> parametersActivityItemProductionByProduct = new List<object>()
                                {
                                    actItemRefno,
                                    actItemProductionFKRefno,
                                    Convert.ToString(actItemProductionFKRefno)
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

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product items complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product items failed.", false);

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving ticket add-product items failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="actProductionFKRefno"></param>
        /// <returns></returns>
        protected override async Task<string> Create(int actProductionFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCTION TICKET BYPRODUCT ITEMS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<refno>" + actProductionFKRefno.ToString() + "</refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
