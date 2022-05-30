// <copyright file="clsTableLocationMapSlotCoordinate.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableLocationMapSlotCoordinate" />.
    /// </summary>
    public class TableLocationMapSlotCoordinate : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableLocationMapSlotCoordinate"/> class.
        /// </summary>
        public TableLocationMapSlotCoordinate()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableLocationMapSlotCoordinate";
        }

        /// <summary>
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slot coordinates.", true);

                string reqData = string.Empty;
                reqData = await this.SendRequest();

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

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slot coordinates complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slot coordinates failed.", true);

                throw new Exception("Seeding slot coordinates failed.", ex);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging slot coordinates.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tLocationMapSlotCoordinate");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging slot coordinates complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging slot coordinates failed.", true);

                throw new Exception("Database exception occurred. Purging slot coordinates failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tLocationMapSlotCoordinate"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} slot coordinates.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Slot coordinates status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="reqData"></param>
        protected override async Task Save(string reqData)
        {
            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slot coordinates.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tLocationMapSlotCoordinate"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int progValue = 0;
                            bool isSlotCoordFound = false;
                            int slotCoordType = Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord1);
                            string slotCoordCode = string.Empty;
                            int whsLocMapRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tLocationMapSlotCoordinate"].Rows)
                            {
                                slotCoordCode = drow["LocationMapSlotCoordinateCode"].ToString();
                                slotCoordType = Convert.ToInt32(drow["LocationMapSlotCoordinateEnum"]);

                                switch (slotCoordType)
                                {
                                    case (int)SysCommon.LocMapSlotCoordTypes.Coord1:
                                        break;

                                    case (int)SysCommon.LocMapSlotCoordTypes.Coord2:
                                        break;

                                    case (int)SysCommon.LocMapSlotCoordTypes.Coord3:
                                        break;

                                    default:

                                        throw new Exception("Unidentified slot coordinate enum type " + slotCoordType);
                                }

                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, transaction);
                                isSlotCoordFound = false;

                                if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    List<tLocationMapSlotCoordinateModel> results = transaction.Query<tLocationMapSlotCoordinateModel>("SELECT * FROM tLocationMapSlotCoordinate WHERE LocationMapSlotCoordinateEnum = ? AND LocationMapSlotCoordinateCode = ? AND WarehouseLocationMapRefno = ?",
                                        slotCoordType,
                                        slotCoordCode,
                                        whsLocMapRefno);

                                    if (results?.Count > 0)
                                    {
                                        isSlotCoordFound = true;
                                    }
                                }

                                if (!isSlotCoordFound)
                                {
                                    transaction.Execute("INSERT INTO tLocationMapSlotCoordinate (LocationMapSlotCoordinateEnum, WarehouseLocationMapRefno, LocationMapSlotCoordinateCode) VALUES (?, ?, ?)",
                                        Convert.ToInt32(drow["LocationMapSlotCoordinateEnum"]),
                                        whsLocMapRefno,
                                        Convert.ToString(drow["LocationMapSlotCoordinateCode"]));
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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slot coordinates complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slot coordinates failed.", false);
                    throw new Exception("Database exception occurred. Saving slot coordinates failed.", ex);
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
                + "<action>REQUEST SLOT COORDINATES</action>"
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
