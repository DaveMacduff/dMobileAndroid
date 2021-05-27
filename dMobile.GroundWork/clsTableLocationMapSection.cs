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
    using dMobile.Resources.Models;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="TableLocationMapSection" />
    /// </summary>
    public class TableLocationMapSection : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableLocationMapSection"/> class.
        /// </summary>
        public TableLocationMapSection()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableLocationMapSection";
        }

        /// <summary>
        /// The Request
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        public override async Task Request(int whsLocMapRefno)
        {
            try
            {
                // TODO: Resource String
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sections.", true);

                string reqData = string.Empty;

                if (0 < whsLocMapRefno)
                {
                    List<tLocationMapWarehouseModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapRefno = ?", whsLocMapRefno);

                    if (results?.Count > 0)
                    {
                        // TODO: Resource String
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("For whs \'{0}\'.", results[0].LocationMapWarehouseName), true);
                    }
                    else
                    {
                        // TODO: Resource String
                        throw new Exception(string.Format("Unable to locate the warehouse name using the warehouse refno {0} in tLocationMapWarehouse.", whsLocMapRefno));
                    }

                }

                reqData = await this.Send(whsLocMapRefno);

                if (0 < reqData.Length)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    await this.Save(reqData);
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sections complete.", true);
            }
            catch (SQLiteException ex)
            {

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sections failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding sections failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sections failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding sections failed.", ex);
            }
        }

        /// <summary>
        /// The PurgeData
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        public override async Task PurgeData(int whsLocMapRefno)
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sections.", true);

                if (whsLocMapRefno == 0)
                {
                    await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tLocationMapSection");
                }
                else
                {
                    await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tLocationMapSection WHERE WarehouseLocationMapRefno = ?", Convert.ToString(whsLocMapRefno));
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sections complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sections failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging sections failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tLocationMapSection"));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} sections.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Sections status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        /// <returns></returns>
        protected override async Task<string> Send(int whsLocMapRefno)
        {
            return await this.SendComm(await this.Create(whsLocMapRefno));
        }

        /// <summary>
        /// The Save
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
                        // TODO: Resource String
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource String
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sections.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tLocationMapSection"].Rows.Count);

                            // TODO: Resource String
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int locMapRefno = 0;
                            int whsLocMapRefno = 0;
                            int progValue = 0;
                            bool isSectFound = false;
                            int sectFKRefno = 0;
                            int sectCode = 0;
                            string sectName = string.Empty;
                            string sectBarCode = string.Empty;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drowSection in ds.Tables["tLocationMapSection"].Rows)
                            {
                                sectFKRefno = Convert.ToInt32(drowSection["LocationMapSectionFKRefno"]);
                                errMsg = "Section fk refno " + sectFKRefno;
                                sectCode = Convert.ToInt32(drowSection["LocationMapSectionCode"]);
                                errMsg += Environment.NewLine + "Section code " + sectCode;
                                sectName = Convert.ToString(drowSection["LocationMapSectionName"]);
                                errMsg += Environment.NewLine + "Section name \'" + sectName + "\'";
                                sectBarCode = Convert.ToString(drowSection["LocationMapSectionBarCode"]);
                                errMsg += Environment.NewLine + "Section bar code \'" + sectBarCode + "\'";
                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drowSection, ref errMsg, transaction);
                                isSectFound = false;

                                List<tLocationMapSectionModel> resultsLocationMapSection = transaction.Query<tLocationMapSectionModel>("SELECT LocationMapSectionFKRefno FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?", Convert.ToString(sectFKRefno));

                                if (resultsLocationMapSection?.Count > 0)
                                {
                                    isSectFound = true;

                                    if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                    {
                                        string queryUpdate = "UPDATE tLocationMapSection "
                                        + "SET WarehouseLocationMapRefno = " + whsLocMapRefno + ","
                                        + "LocationMapSectionCode = " + sectCode + ","
                                        + "LocationMapSectionName = \'" + sectName + "\',"
                                        + "LocationMapSectionBarCode = \'" + sectBarCode + "\' "
                                        + "WHERE LocationMapSectionFKRefno = " + sectFKRefno;

                                        transaction.Execute(queryUpdate);
                                    }
                                }

                                if (!isSectFound)
                                {
                                    transaction.Execute("INSERT INTO tLocationMap (LocationMapSubType) VALUES (?)", Convert.ToInt32(SysCommon.LocMapSubTypes.Section));

                                    locMapRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tLocationMap"));

                                    string queryInsert = "INSERT INTO tLocationMapSection " +
                                    "(LocationMapRefno, " +
                                    "LocationMapSubType, " +
                                    "WarehouseLocationMapRefno, " +
                                    "LocationMapSectionCode, " +
                                    "LocationMapSectionName, " +
                                    "LocationMapSectionFKRefno, " +
                                    "LocationMapSectionBarCode " +
                                    ") " +
                                    "VALUES " +
                                    "(?, " +
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
                                        Convert.ToInt32(SysCommon.LocMapSubTypes.Section),
                                        whsLocMapRefno,
                                        sectCode,
                                        sectName,
                                        sectFKRefno,
                                        sectBarCode
                                    };

                                    transaction.Execute(queryInsert, parametersInsert);
                                }

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }

                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                            }
                        }

                        transaction.Commit();

                        // TODO: Resource String
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sections complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    // TODO: Resource String
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sections failed.", false);

                    // TODO: Resource String
                    throw new Exception("Database exception occurred. Saving sections failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        /// <returns></returns>
        protected override async Task<string> Create(int whsLocMapRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            TableLocationMapWarehouse locMapWhs = new TableLocationMapWarehouse();
            int locMapWhsFKRefno = 0;

            // lookup and verify warehouse
            if (0 < whsLocMapRefno)
            {
                locMapWhsFKRefno = await locMapWhs.GetLocMapWhsFKRefnoFromLocMapWhsRefno(whsLocMapRefno);
            }

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST SECTIONS</action>"
                + SysCommon.GetRequestDefaults(true)
                + ("<LocationMapWarehouseFKRefno>" + locMapWhsFKRefno + "</LocationMapWarehouseFKRefno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
