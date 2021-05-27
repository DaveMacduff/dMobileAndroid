namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using System.Collections.Generic;
    using dMobile.Resources.Models;

    /// <summary>
    /// Defines the <see cref="TableContactUsers" />
    /// </summary>
    public class TableContactUsers : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableContactUsers"/> class.
        /// </summary>
        public TableContactUsers()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableContactUsers";
        }

        /// <summary>
        /// The Request
        /// </summary>
        public override async Task Request()
        {
            string reqData = string.Empty;

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding users.", true);

                await this.SeedContactUsersWithSystemUsers();

                reqData = await this.Send("contact users");

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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding users complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding users failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding users failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tContactUser"));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} users.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Users status failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging users.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tContactUser");

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging users complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging users failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging users failed.", ex);
            }
        }

        public async Task PurgeTContact()
        {
            try
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging tContact.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tContact");

                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging tContact complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging tContact failed.", true);

                throw new Exception("Database exception occurred. Purging tContact failed.", ex);
            }
        }

        /// <summary>
        /// The SeedContactUsersWithSystemUsers
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public async Task SeedContactUsersWithSystemUsers()
        {
            string errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    List<tContactUserModel> resultsContactUser = transaction.Query<tContactUserModel>("SELECT ContactUserCode FROM tContactUser WHERE ContactUserCode = ? OR ContactUserCode = ?", SysCommon.SystemLoginID, SysCommon.SetupLoginID);

                    if (resultsContactUser == null || resultsContactUser?.Count == 0)
                    {
                        transaction.Execute("INSERT INTO tContact (ContactSubType) VALUES (?)", Convert.ToInt32(SysCommon.ContactSubTypes.User));

                        int contactRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tContact"));
                        int contactCode = SysCommon.SystemLoginID;
                        string contactName = "InterSect";
                        errMsg = "Code " + contactCode;
                        errMsg += Environment.NewLine + "Name \'" + contactName + "\'";

                        string queryInsertContactUser = "INSERT INTO tContactUser " +
                            "(ContactRefno, " +
                            "ContactSubType, " +
                            "ContactUserCode, " +
                            "ContactUserName, " +
                            "ContactUserPassword, " +
                            "ContactUserIsAllowedAssociateProductBarCode, " +
                            "ContactUserIsAllowedPickingModuleOverridePicker, " +
                            "ContactUserIsAllowedPickingModuleOverrideAddManualDetail, " +
                            "ContactUserLocalizationLanguage) " +
                            "VALUES " +
                            "(?, " +
                            "?, " +
                            "?, " +
                            "?, " +
                            "?, " +
                            "?, " +
                            "?, " +
                            "?, " +
                            "?)";
                        object[] parametersContactUserSystemLogin = new object[]
                        {
                            contactRefno,
                            Convert.ToInt32(SysCommon.ContactSubTypes.User),
                            contactCode,
                            contactName,
                            SysCommon.SystemLoginPassword,
                            true,
                            true,
                            true,
                            SysEnv.SysSettings.DefaultLanguage
                        };

                        transaction.Execute(queryInsertContactUser, parametersContactUserSystemLogin);

                        // Create User 99
                        transaction.Execute("INSERT INTO tContact (ContactSubType) VALUES (?)", Convert.ToInt32(SysCommon.ContactSubTypes.User));

                        contactRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tContact"));
                        contactCode = SysCommon.SetupLoginID;

                        errMsg = "Code " + contactCode;
                        contactName = "Setup User";
                        errMsg += Environment.NewLine + "Name \'" + contactName + "\'";

                        object[] parametersContactUserSetupLogin = new object[]
                        {
                            contactRefno,
                            Convert.ToInt32(SysCommon.ContactSubTypes.User),
                            contactCode,
                            contactName,
                            SysCommon.SetupLoginPassword,
                            true,
                            false,
                            false,
                            SysEnv.SysSettings.DefaultLanguage
                        };

                        transaction.Execute(queryInsertContactUser, parametersContactUserSetupLogin);
                    }

                    transaction.Commit();
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    throw new Exception("Database exception occurred. Seeding system users failed." + Environment.NewLine + errMsg, ex);
                }
            });
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
                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving users.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tContactUser"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            string queryInsertContactUser = "INSERT INTO tContactUser " +
                            "(ContactRefno, " +
                            "ContactSubType, " +
                            "ContactUserCode, " +
                            "ContactUserName, " +
                            "ContactUserPassword, " +
                            "ContactUserIsAllowedAssociateProductBarCode, " +
                            "ContactUserIsAllowedPickingModuleOverridePicker, " +
                            "ContactUserIsAllowedPickingModuleOverrideAddManualDetail, " +
                            "ContactUserLocalizationLanguage) " +
                            "VALUES " +
                            "(?, " +
                            "?, " +
                            "?, " +
                            "?, " +
                            "?, " +
                            "?, " +
                            "?, " +
                            "?, " +
                            "?)";

                            int progValue = 0;
                            int contactRefno = 0;
                            int contactCode = 0;
                            string contactName = string.Empty;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tContactUser"].Rows)
                            {
                                transaction.Execute("INSERT INTO tContact (ContactSubType) VALUES (?)", Convert.ToInt32(SysCommon.ContactSubTypes.User));

                                contactRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tContact"));

                                contactCode = Convert.ToInt32(drow["ContactUserCode"]);
                                errMsg = "Code " + contactCode;
                                contactName = drow["ContactUserName"].ToString();
                                errMsg += Environment.NewLine + "Name \'" + contactName + "\'";

                                object[] parametersInsertContactUser = new object[]
                                {
                                    contactRefno,
                                    Convert.ToInt32(SysCommon.ContactSubTypes.User),
                                    contactCode,
                                    contactName,
                                    Convert.ToString(drow["ContactUserPassword"]),
                                    Convert.ToBoolean(drow["ContactUserIsAllowedAssociateProductBarCode"]),
                                    Convert.ToBoolean(drow["ContactUserIsAllowedPickingModuleOverridePicker"]),
                                    Convert.ToBoolean(drow["ContactUserIsAllowedPickingModuleOverrideAddManualDetail"]),
                                    Convert.ToString(drow["ContactUserLocalizationLanguage"])
                                };

                                transaction.Execute(queryInsertContactUser, parametersInsertContactUser);

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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving users complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving users failed.", false);
                    throw new Exception("Database exception occurred. Saving users failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> Create()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST CONTACT USERS</action>"
                + SysCommon.GetRequestDefaults(true)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
