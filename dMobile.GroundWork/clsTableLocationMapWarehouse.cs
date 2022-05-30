// <copyright file="clsTableLocationMapWarehouse.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableLocationMapWarehouse" />.
    /// </summary>
    public class TableLocationMapWarehouse : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableLocationMapWarehouse"/> class.
        /// </summary>
        public TableLocationMapWarehouse()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableLocationMapWarehouse";
        }

        /// <summary>
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouses.", true);

                string reqData = await this.SendRequest();

                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    await this.Save(reqData);
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouses complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouses failed.", true);

                throw new Exception("Seeding warehouses failed.", ex);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouses.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tLocationMapWarehouse WHERE LocationMapSubType = ?", Convert.ToInt32(SysCommon.LocMapSubTypes.Warehouse));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouses complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouses failed.", true);

                throw new Exception("Database exception occurred. Purging warehouses failed.", ex);
            }
        }

        /// <summary>
        /// The GetLocMapWhsRefnoFromLocWhsFKRefno.
        /// </summary>
        /// <param name="locMapWhsFKRefno"></param>
        /// <returns></returns>
        public async Task<int> GetLocMapWhsRefnoFromLocWhsFKRefno(int locMapWhsFKRefno)
        {
            int whsLocMapRefno;
            try
            {
                List<tLocationMapWarehouseModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", Convert.ToInt32(locMapWhsFKRefno));

                if (results?.Count > 0)
                {
                    whsLocMapRefno = results[0].LocationMapRefno;
                }
                else
                {
                    throw new Exception(string.Format("Unable to locate warehouse refno using the warehouse fk refno {0} in tLocationMapWarehouse.", locMapWhsFKRefno));
                }
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. GetLocMapWhsRefnoFromLocWhsFKRefno failed.", ex);
            }

            return whsLocMapRefno;
        }

        /// <summary>
        /// The GetLocMapWhsFKRefnoFromLocMapWhsRefno.
        /// </summary>
        /// <param name="locMapWhsRefno"></param>
        /// <returns></returns>
        public async Task<int> GetLocMapWhsFKRefnoFromLocMapWhsRefno(int locMapWhsRefno)
        {
            int locMapWhsFKRefno;
            try
            {
                List<tLocationMapWarehouseModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapRefno = ?", locMapWhsRefno);

                if (results?.Count > 0)
                {
                    locMapWhsFKRefno = results[0].LocationMapWarehouseFKRefno;
                }
                else
                {
                    throw new Exception(string.Format("Unable to locate warehouse FKRefno using the warehouse refno {0} in tLocationMapWarehouse.", locMapWhsRefno));
                }
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. GetLocMapWhsFKRefnoFromLocMapWhsRefno failed.", ex);
            }

            return locMapWhsFKRefno;
        }

        /// <summary>
        /// The DataStatus.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tLocationMapWarehouse"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} warehouses.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Warehouses status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> SendRequest()
        {
            return await this.SendComm(await this.CreateRequest());
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="reqData"></param>
        protected override async Task Save(string reqData)
        {
            string errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouses.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tLocationMapWarehouse"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int locMapRefno = 0;
                            int progValue = 0;
                            bool isLocMapWhsFound = false;
                            int locMapWhsFKRefno = 0;
                            int locMapWhsCode = 0;
                            string locMapWhsName = string.Empty;
                            string locMapWhsBarCode = string.Empty;
                            bool isLocMapWhsIsWMS = false;
                            string locMapWhsMask = string.Empty;
                            string locMapSectionMask = string.Empty;
                            string locMapSlotMask = string.Empty;
                            string locMapSlotCoord1Mask = string.Empty;
                            string locMapSlotCoord1Name = string.Empty;
                            string locMapSlotCoord2Mask = string.Empty;
                            string locMapSlotCoord2Name = string.Empty;
                            string locMapSlotCoord3Mask = string.Empty;
                            string locMapSlotCoord3Name = string.Empty;
                            bool isLocMapSectionTracked = false;
                            bool isLocMapSlotTracked = false;
                            bool isLocMapAdhocEnabled = false;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tLocationMapWarehouse"].Rows)
                            {
                                locMapWhsFKRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);

                                errMsg = "Whs fk refno " + locMapWhsFKRefno;
                                locMapWhsCode = Convert.ToInt32(drow["LocationMapWarehouseCode"]);
                                errMsg += Environment.NewLine + "Whs code " + locMapWhsCode;
                                locMapWhsName = drow["LocationMapWarehouseName"].ToString();
                                errMsg += Environment.NewLine + "Whs name \'" + locMapWhsCode + "\'";
                                locMapWhsBarCode = drow["LocationMapWarehouseBarCode"].ToString();
                                errMsg += Environment.NewLine + "Whs bar code \'" + locMapWhsBarCode + "\'";
                                isLocMapWhsIsWMS = Convert.ToBoolean(drow["LocationMapWarehouseIsWMS"]);
                                errMsg += Environment.NewLine + "Whs is wms " + isLocMapWhsIsWMS.ToString();
                                locMapWhsMask = drow["LocationMapWarehouseMask"].ToString();
                                errMsg += Environment.NewLine + "Whs mask \'" + locMapWhsMask + "\'";
                                locMapSectionMask = drow["LocationMapSectionMask"].ToString();
                                errMsg += Environment.NewLine + "Whs section mask \'" + locMapSectionMask + "\'";
                                locMapSlotMask = drow["LocationMapSlotMask"].ToString();
                                errMsg += Environment.NewLine + "Whs slot mask \'" + locMapSlotMask + "\'";
                                locMapSlotCoord1Mask = drow["LocationMapSlotCoord1Mask"].ToString();
                                errMsg += Environment.NewLine + "Whs slot coord1 mask \'" + locMapSlotCoord1Mask + "\'";
                                locMapSlotCoord1Name = drow["LocationMapSlotCoord1Name"].ToString();
                                errMsg += Environment.NewLine + "Whs slot coord1 name \'" + locMapSlotCoord1Name + "\'";
                                locMapSlotCoord2Mask = drow["LocationMapSlotCoord2Mask"].ToString();
                                errMsg += Environment.NewLine + "Whs slot coord2 mask \'" + locMapSlotCoord2Mask + "\'";
                                locMapSlotCoord2Name = drow["LocationMapSlotCoord2Name"].ToString();
                                errMsg += Environment.NewLine + "Whs slot coord2 name \'" + locMapSlotCoord2Name + "\'";
                                locMapSlotCoord3Name = drow["LocationMapSlotCoord3Mask"].ToString();
                                errMsg += Environment.NewLine + "Whs slot coord3 mask \'" + locMapSlotCoord3Mask + "\'";
                                locMapSlotCoord3Mask = drow["LocationMapSlotCoord3Mask"].ToString();
                                errMsg += Environment.NewLine + "Whs slot coord3 name \'" + locMapSlotCoord3Name + "\'";
                                isLocMapSectionTracked = Convert.ToBoolean(drow["LocationMapIsSectionTracked"]);
                                errMsg += Environment.NewLine + "Whs is section tracked " + isLocMapSectionTracked.ToString();
                                isLocMapSlotTracked = Convert.ToBoolean(drow["LocationMapIsSlotTracked"]);
                                errMsg += Environment.NewLine + "Whs is slot tracked " + isLocMapSlotTracked.ToString();
                                isLocMapAdhocEnabled = Convert.ToBoolean(drow["LocationMapIsAdhocEnabled"]);
                                errMsg += Environment.NewLine + "Whs is adhoc enabled " + isLocMapAdhocEnabled.ToString();

                                isLocMapWhsFound = false;

                                List<tLocationMapWarehouseModel> results = transaction.Query<tLocationMapWarehouseModel>("SELECT LocationMapWarehouseFKRefno FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", locMapWhsFKRefno);

                                if (results?.Count > 0)
                                {
                                    isLocMapWhsFound = true;

                                    if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                    {
                                        string queryUpdate = "UPDATE tLocationMapWarehouse " +
                                        "SET LocationMapWarehouseCode = " + locMapWhsCode.ToString() + ", " +
                                        "LocationMapWarehouseName = \'" + locMapWhsName + "\', " +
                                        "LocationMapWarehouseBarCode = \'" + locMapWhsBarCode + "\', " +
                                        "LocationMapWarehouseIsWMS = " + (isLocMapWhsIsWMS ? 1 : 0).ToString() +
                                        "LocationMapWarehouseMask = \'" + locMapWhsMask + "\', " +
                                        "LocationMapWarehouseSectionMask = \'" + locMapSectionMask + "\', " +
                                        "LocationMapWarehouseSlotMask = \'" + locMapSlotMask + "\', " +
                                        "LocationMapWarehouseSlotCoordinate1Mask = \'" + locMapSlotCoord1Mask + "\', " +
                                        "LocationMapWarehouseSlotCoordinate2Mask = \'" + locMapSlotCoord2Mask + "\', " +
                                        "LocationMapWarehouseSlotCoordinate3Mask = \'" + locMapSlotCoord3Mask + "\', " +
                                        "LocationMapWarehouseSlotCoordinate1Name = \'" + locMapSlotCoord1Name + "\', " +
                                        "LocationMapWarehouseSlotCoordinate2Name = \'" + locMapSlotCoord2Name + "\', " +
                                        "LocationMapWarehouseSlotCoordinate3Name = \'" + locMapSlotCoord3Name + "\', " +
                                        "LocationMapWarehouseIsSectionTracked = " + (isLocMapSectionTracked ? 1 : 0).ToString() +
                                        "LocationMapWarehouseIsSlotTracked = " + (isLocMapSlotTracked ? 1 : 0).ToString() +
                                        "LocationMapWarehouseAdhocEnabled = " + (isLocMapAdhocEnabled ? 1 : 0).ToString() +
                                        "WHERE LocationMapWarehouseFKRefno = " + locMapWhsFKRefno.ToString();

                                        transaction.Execute(queryUpdate);
                                    }
                                }

                                if (!isLocMapWhsFound)
                                {
                                    transaction.Execute("INSERT INTO tLocationMap (LocationMapSubType) VALUES (?)", Convert.ToInt32(SysCommon.LocMapSubTypes.Warehouse));

                                    locMapRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tLocationMap"));

                                    string queryInsert = "INSERT INTO tLocationMapWarehouse (" +
                                    "LocationMapRefno, " +
                                    "LocationMapSubType, " +
                                    "LocationMapWarehouseName, " +
                                    "LocationMapWarehouseCode, " +
                                    "LocationMapWarehouseFKRefno, " +
                                    "LocationMapWarehouseBarCode, " +
                                    "LocationMapWarehouseIsWMS, " +
                                    "LocationMapWarehouseMask, " +
                                    "LocationMapWarehouseSectionMask, " +
                                    "LocationMapWarehouseSlotMask, " +
                                    "LocationMapWarehouseSlotCoordinate1Mask, " +
                                    "LocationMapWarehouseSlotCoordinate2Mask, " +
                                    "LocationMapWarehouseSlotCoordinate3Mask, " +
                                    "LocationMapWarehouseSlotCoordinate1Name, " +
                                    "LocationMapWarehouseSlotCoordinate2Name, " +
                                    "LocationMapWarehouseSlotCoordinate3Name, " +
                                    "LocationMapWarehouseIsSectionTracked, " +
                                    "LocationMapWarehouseIsSlotTracked, " +
                                    "LocationMapWarehouseAdhocEnabled" +
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
                                    object[] parametersInsert = new object[]
                                    {
                                        locMapRefno,
                                        Convert.ToInt32(SysCommon.LocMapSubTypes.Warehouse),
                                        locMapWhsName,
                                        locMapWhsCode,
                                        locMapWhsFKRefno,
                                        locMapWhsBarCode,
                                        Convert.ToInt32(isLocMapWhsIsWMS),
                                        locMapWhsMask,
                                        locMapSectionMask,
                                        locMapSlotMask,
                                        locMapSlotCoord1Mask,
                                        locMapSlotCoord2Mask,
                                        locMapSlotCoord3Mask,
                                        locMapSlotCoord1Name,
                                        locMapSlotCoord2Name,
                                        locMapSlotCoord3Name,
                                        Convert.ToInt32(isLocMapSectionTracked),
                                        Convert.ToInt32(isLocMapSlotTracked),
                                        Convert.ToInt32(isLocMapAdhocEnabled),
                                    };

                                    transaction.Execute(queryInsert, parametersInsert);
                                }

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        transaction.Commit();

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouses complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouses failed.", false);

                    throw new Exception("Database exception occurred. Saving warehouses failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> CreateRequest()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST WAREHOUSES</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
