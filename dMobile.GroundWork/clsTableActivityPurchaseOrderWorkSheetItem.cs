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

    /// <summary>
    /// Defines the <see cref="TableActivityPurchaseOrderWorkSheetItem" />
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding work sheet items.", true);

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

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding work sheet items complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding work sheet items failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding work sheet items failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityItemRefno) FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet)));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} work sheet items.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Work sheet status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override async Task Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging work sheet items.", true);

                string query = "DELETE FROM tActivityItemBarCodeAssociationXRef WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno " +
                    "IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet));

                query = "DELETE FROM tActivityItemPurchaseOrderWorkSheet WHERE tActivityItemPurchaseOrderWorkSheet.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet));

                query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging work sheet items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging work sheet items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging work sheet items failed.", ex);
            }
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override async Task<string> Send(int actRefno)
        {
            int actWorkSheetFKRefno = 0;
            return await this.SendComm(await this.Create(actWorkSheetFKRefno));
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving work sheet items.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityItemPurchaseOrderWorkSheet"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actItemRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;
                            int actItemWorkSheetFKRefno = 0;
                            int prodUnitRefno = 0;

                            DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityItemPurchaseOrderWorkSheet"].Rows)
                            {
                                errMsg = "Arguments:";

                                transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet));

                                actRefno = Convert.ToInt32(transaction.QueryScalars<int>("SELECT last_insert_rowid() FROM tActivity"));

                                transaction.Execute("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)", Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet), 0, actRefno);

                                actItemRefno = Convert.ToInt32(transaction.QueryScalars<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                                prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, transaction, ref errMsg, false);
                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, transaction);

                                actItemWorkSheetFKRefno = Convert.ToInt32(drow["ActivityItemPurchaseOrderWorkSheetFKRefno"]);

                                string query = "INSERT INTO tActivityItemPurchaseOrderWorkSheet (" +
                                    "ActivityItemRefno, " +
                                    "ActivityItemSubType, " +
                                    "ActivityItemPurchaseOrderWorkSheetFKRefno, " +
                                    "ActivityItemPurchaseOrderWorkSheetStringFKRefno, " +
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
                                List<object> parameters = new List<object>()
                                {
                                    Convert.ToInt32(actItemRefno),
                                    Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet),
                                    Convert.ToInt32(actItemWorkSheetFKRefno),
                                    Convert.ToString(actItemWorkSheetFKRefno),
                                    Convert.ToInt32(prodUnitRefno),
                                    Convert.ToInt32(whsLocMapRefno),
                                    Convert.ToDecimal(drow["ActivityItemPurchaseOrderWorkSheetQuantityOrdered"]),
                                    Convert.ToDecimal(drow["ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered"]),
                                    Convert.ToDecimal(drow["ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered"])
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

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving work sheet items complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving work sheet items failed.", false);

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving work sheet items failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="actWorkSheetFKRefno">The actWorkSheetFKRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override async Task<string> Create(int actWorkSheetFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PURCHASE ORDER WORK SHEET ITEMS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<refno>" + actWorkSheetFKRefno + "</refno>")
                + "</command>"
                + ("<whs_id>" + LocationMap.GetInstance().CurrLocMap.WhsFKRefno + "</whs_id>")
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
