// <copyright file="TableLocationMapSection.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableLocationMapSection" />.
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
        /// The Request.
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        public override async Task RequestData(int whsLocMapRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sections.", true);

                var responseData = string.Empty;

                if (whsLocMapRefno > 0)
                {
                    var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapRefno = ?", whsLocMapRefno);

                    if (results?.Count > 0)
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty,
                            $"For whs \'{results[0].LocationMapWarehouseName}\'.", true);
                    }
                    else
                    {
                        throw new Exception(
                            $"Unable to locate the warehouse name using the warehouse refno {whsLocMapRefno} in tLocationMapWarehouse.");
                    }
                }

                responseData = await this.SendRequest(whsLocMapRefno);
                await this.Save(responseData);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sections complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sections failed.", true);

                throw new Exception("Database exception occurred. Seeding sections failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sections failed.", true);

                throw new Exception("Seeding sections failed.", ex);
            }
        }

        /// <summary>
        /// The PurgeData.
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        public override async Task PurgeData(int whsLocMapRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sections.", true);

                if (whsLocMapRefno == 0)
                {
                    await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tLocationMapSection");
                }
                else
                {
                    await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tLocationMapSection WHERE WarehouseLocationMapRefno = ?", Convert.ToString(whsLocMapRefno));
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sections complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sections failed.", true);

                throw new Exception("Database exception occurred. Purging sections failed.", ex);
            }
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM tLocationMapSection"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} sections.", true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Sections status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <param name="activityFkRefno</param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int activityFkRefno)
        {
            return await this.SendComm(await this.CreateRequest(activityFkRefno));
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData"></param>
        protected override async Task Save(string responseData)
        {
            await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received responseData.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sections.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["tLocationMapSection"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var locMapRefno = 0;
                        var whsLocMapRefno = 0;
                        var progValue = 0;
                        var isSectFound = false;
                        var sectFKRefno = 0;
                        var sectCode = 0;
                        var sectName = string.Empty;
                        var sectBarCode = string.Empty;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drowSection in ds.Tables["tLocationMapSection"].Rows)
                        {
                            sectFKRefno = Convert.ToInt32(drowSection["LocationMapSectionFKRefno"]);
                            sectCode = Convert.ToInt32(drowSection["LocationMapSectionCode"]);
                            sectName = Convert.ToString(drowSection["LocationMapSectionName"]);
                            sectBarCode = Convert.ToString(drowSection["LocationMapSectionBarCode"]);
                            whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drowSection, transaction);
                            isSectFound = false;

                            var resultsLocationMapSection = transaction.Query<tLocationMapSectionModel>("SELECT LocationMapSectionFKRefno FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?", Convert.ToString(sectFKRefno));

                            if (resultsLocationMapSection?.Count > 0)
                            {
                                isSectFound = true;

                                if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    var queryUpdate = "UPDATE tLocationMapSection "
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

                                var queryInsert = "INSERT INTO tLocationMapSection " +
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
                                var parametersInsert = new object[]
                                {
                                        locMapRefno,
                                        Convert.ToInt32(SysCommon.LocMapSubTypes.Section),
                                        whsLocMapRefno,
                                        sectCode,
                                        sectName,
                                        sectFKRefno,
                                        sectBarCode,
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

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sections complete.", false);
                }
                catch (SQLiteException ex)
                {
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sections failed.", false);
                    throw new Exception("Database exception occurred. Saving sections failed.", ex);
                }
            });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="activityFkRefno"></param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(int activityFkRefno)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var locMapWhs = new TableLocationMapWarehouse();
            var locMapWhsFKRefno = 0;

            // lookup and verify warehouse
            if (activityFkRefno > 0)
            {
                locMapWhsFKRefno = await locMapWhs.GetLocMapWhsFKRefnoFromLocMapWhsRefno(activityFkRefno);
            }

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST SECTIONS</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "<LocationMapWarehouseFKRefno>" + locMapWhsFKRefno + "</LocationMapWarehouseFKRefno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        public override Task PurgeData()
        {
            throw new NotImplementedException();
        }

        public override Task PurgeData(SysCommon.ActSubTypes actSubType)
        {
            throw new NotImplementedException();
        }

        protected override Task Save(string responseData, int refno)
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
