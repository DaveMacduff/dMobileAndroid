// <copyright file="TableContactUsers.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableContactUsers" />.
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
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding users.", true);

                await this.SeedContactUsersWithSystemUsers();

                var reqData = await this.SendRequest();
                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        var reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    await this.Save(reqData);
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding users complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding users failed.", true);

                throw new Exception("Seeding users failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tContactUser"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} users.", true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Users status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging users.", true);

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tContactUser");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging users complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging users failed.", true);

                throw new Exception("Database exception occurred. Purging users failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task PurgeTContact()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging tContact.", true);

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tContact");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging tContact complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging tContact failed.", true);

                throw new Exception("Database exception occurred. Purging tContact failed.", ex);
            }
        }

        /// <summary>
        /// The SeedContactUsersWithSystemUsers.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task SeedContactUsersWithSystemUsers()
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    var resultsContactUser = transaction.Query<tContactUserModel>("SELECT ContactUserCode FROM tContactUser WHERE ContactUserCode = ? OR ContactUserCode = ?", SysCommon.SystemLoginID, SysCommon.SetupLoginID);

                    if (resultsContactUser == null || resultsContactUser?.Count == 0)
                    {
                        transaction.Execute("INSERT INTO tContact (ContactSubType) VALUES (?)", Convert.ToInt32(SysCommon.ContactSubTypes.User));

                        var contactRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tContact"));
                        var contactCode = SysCommon.SystemLoginID;
                        var contactName = "InterSect";
                        errMsg = "Code " + contactCode;
                        errMsg += Environment.NewLine + "Name \'" + contactName + "\'";

                        var queryInsertContactUser = "INSERT INTO tContactUser " +
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
                        var parametersContactUserSystemLogin = new object[]
                        {
                            contactRefno,
                            Convert.ToInt32(SysCommon.ContactSubTypes.User),
                            contactCode,
                            contactName,
                            SysCommon.SystemLoginPassword,
                            true,
                            true,
                            true,
                            this.SysEnv.SysSettings.DefaultLanguage,
                        };

                        transaction.Execute(queryInsertContactUser, parametersContactUserSystemLogin);

                        // Create User 99
                        transaction.Execute("INSERT INTO tContact (ContactSubType) VALUES (?)", Convert.ToInt32(SysCommon.ContactSubTypes.User));

                        contactRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tContact"));
                        contactCode = SysCommon.SetupLoginID;

                        errMsg = "Code " + contactCode;
                        contactName = "Setup User";
                        errMsg += Environment.NewLine + "Name \'" + contactName + "\'";

                        var parametersContactUserSetupLogin = new object[]
                        {
                            contactRefno,
                            Convert.ToInt32(SysCommon.ContactSubTypes.User),
                            contactCode,
                            contactName,
                            SysCommon.SetupLoginPassword,
                            true,
                            false,
                            false,
                            this.SysEnv.SysSettings.DefaultLanguage,
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
        /// The Save.
        /// </summary>
        /// <param name="responseData"></param>
        protected override async Task Save(string responseData)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received responseData.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving users.", false);

                    // Check to see what users already exist
                    var resultsContactUser = transaction.Query<tContactUserModel>("SELECT ContactUserCode FROM tContactUser");

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["tContactUser"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var queryInsertContactUser = "INSERT INTO tContactUser " +
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

                        var progValue = 0;
                        var contactRefno = 0;
                        var contactCode = 0;
                        var contactName = string.Empty;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in ds.Tables["tContactUser"].Rows)
                        {
                            contactCode = Convert.ToInt32(drow["ContactUserCode"]);

                            /// Check to see if user already exists in table
                            if (resultsContactUser.FirstOrDefault(x => x.ContactUserCode == contactCode) != null)
                            {
                                continue;
                            }

                            transaction.Execute("INSERT INTO tContact (ContactSubType) VALUES (?)", Convert.ToInt32(SysCommon.ContactSubTypes.User));
                            contactRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tContact"));

                            errMsg = "Code " + contactCode;
                            contactName = drow["ContactUserName"].ToString();
                            errMsg += Environment.NewLine + "Name \'" + contactName + "\'";

                            var parametersInsertContactUser = new object[]
                            {
                                    contactRefno,
                                    Convert.ToInt32(SysCommon.ContactSubTypes.User),
                                    contactCode,
                                    contactName,
                                    Convert.ToString(drow["ContactUserPassword"]),
                                    Convert.ToBoolean(drow["ContactUserIsAllowedAssociateProductBarCode"]),
                                    Convert.ToBoolean(drow["ContactUserIsAllowedPickingModuleOverridePicker"]),
                                    Convert.ToBoolean(drow["ContactUserIsAllowedPickingModuleOverrideAddManualDetail"]),
                                    Convert.ToString(drow["ContactUserLocalizationLanguage"]),
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

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving users complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving users failed.", false);
                    throw new Exception("Database exception occurred. Saving users failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> CreateRequest()
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
                + "<action>REQUEST CONTACT USERS</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
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

        protected override Task Save(string responseData, int refno)
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

        protected override Task<string> CreateRequest(int activityItemFkRefno)
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
