// <copyright file="TableContactUsers.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;
    using System.Xml;
    using System.Xml.Linq;
    using Bedrock;
    using Resources.Models;
    using Resources.Types;
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
            this.LowerCaseName = "contact users";
            this.ProperCaseName = "Contact users";
        }

        /// <summary>
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName}.", true);
                await this.SeedContactUsersWithSystemUsers();
                var reqData = await this.SendRequest();
                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        var reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement!["detail"] !.InnerText);
                    }

                    await this.Save(reqData);
                }

                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} complete.",
                    true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} failed.",
                    true);
                throw new Exception($"Seeding {this.LowerCaseName} failed.", ex);
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
                rowsAffected = Convert.ToInt32(
                    await DatabaseConnection.GetInstance()
                        .SqlConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tContactUser"));

                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{rowsAffected} users.",
                    true);
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
                await DatabaseConnection.GetInstance()
                    .SqlConn.ExecuteAsync("DELETE FROM tContactUser");
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Database exception occurred.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Purging users failed.",
                    true);

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
                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tContact");
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Database exception occurred.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Purging tContact failed.",
                    true);

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

            await DatabaseConnection.GetInstance()
                .SqlConn.RunInTransactionAsync(
                    transaction =>
                    {
                        try
                        {
                            var resultsContactUser = transaction.Query<ContactUserModel>(
                                "SELECT ContactUserCode FROM tContactUser WHERE ContactUserCode = ? OR ContactUserCode = ?",
                                SysCommon.SystemLoginId,
                                SysCommon.SetupLoginId);

                            if (resultsContactUser == null ||
                                resultsContactUser?.Count == 0)
                            {
                                transaction.Execute(
                                    "INSERT INTO tContact (ContactSubType) VALUES (?)",
                                    Convert.ToInt32(SysCommon.ContactSubTypes.User));

                                var contactRefno = Convert.ToInt32(
                                    transaction.ExecuteScalar<int>(
                                        "SELECT last_insert_rowid() FROM tContact"));
                                var contactCode = SysCommon.SystemLoginId;
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
                                    this.SysEnv.SysSettings.DefaultLanguage
                                };

                                transaction.Execute(
                                    queryInsertContactUser,
                                    parametersContactUserSystemLogin);

                                // Create User 99
                                transaction.Execute(
                                    "INSERT INTO tContact (ContactSubType) VALUES (?)",
                                    Convert.ToInt32(SysCommon.ContactSubTypes.User));

                                contactRefno = Convert.ToInt32(
                                    transaction.ExecuteScalar<int>(
                                        "SELECT last_insert_rowid() FROM tContact"));
                                contactCode = SysCommon.SetupLoginId;

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
                                    this.SysEnv.SysSettings.DefaultLanguage
                                };

                                transaction.Execute(
                                    queryInsertContactUser,
                                    parametersContactUserSetupLogin);
                            }

                            transaction.Commit();
                        }
                        catch (SQLiteException ex)
                        {
                            transaction.Rollback();
                            throw new Exception(
                                "Database exception occurred. Seeding system users failed." +
                                Environment.NewLine +
                                errMsg,
                                ex);
                        }
                    });
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData">XML data from the comm server.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        protected override async Task Save(string responseData)
        {
            var conn = DatabaseConnection.GetInstance().SqlConn;
            using var dataSet = new DataSet();
            dataSet.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (dataSet.Tables.Count > 0)
            {
                var users = new List<ContactUserModel>();
                var rowsToSave = dataSet.Tables["tContactUser"].Rows.Count;
                for (var idx = 0; idx < rowsToSave; idx++)
                {
                    var row = dataSet.Tables["tContactUser"].Rows[idx];
                    var query = "INSERT INTO tContact (ContactSubType) VALUES (?)";
                    await conn.ExecuteAsync(
                        query,
                        Convert.ToInt32(SysCommon.ContactSubTypes.User));
                    query = "SELECT tContact.ContactRefno FROM tContact" +
                            " ORDER BY tContact.ContactRefno DESC LIMIT 1";
                    var contactRefno = Convert.ToInt32(await conn.ExecuteScalarAsync<int>(query));

                    // Decided not to check for duplicates. Just insert all.
                    // Assume duplicates have been removed at the comm server, including user 30 and 99.
                    var user = new ContactUserModel
                    {
                        ContactRefno = contactRefno,
                        ContactSubType = Convert.ToInt32(SysCommon.ContactSubTypes.User),
                        ContactUserCode = Convert.ToInt32(row["ContactUserCode"]),
                        ContactUserName = row["ContactUserName"].ToString(),
                        ContactUserPassword = row["ContactUserPassword"].ToString(),
                        ContactUserIsAllowedAssociateProductBarCode = Convert.ToBoolean(
                            row["ContactUserIsAllowedAssociateProductBarCode"]),
                        ContactUserIsAllowedPickingModuleOverridePicker = Convert.ToBoolean(
                            row["ContactUserIsAllowedPickingModuleOverridePicker"]),
                        ContactUserIsAllowedPickingModuleOverrideAddManualDetail =
                            Convert.ToBoolean(
                                row[
                                    "ContactUserIsAllowedPickingModuleOverrideAddManualDetail"]),
                        ContactUserLocalizationLanguage =
                            row["ContactUserLocalizationLanguage"].ToString()
                    };
                    users.Add(user);
                }

                await conn.InsertAllAsync(users);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{users.Count} {this.LowerCaseName}.",
                    true);
            }
            else
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"No {this.LowerCaseName} to save.",
                    true);
            }
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
                Indent = false
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                "<imclient>" +
                "<command>" +
                "<action>REQUEST CONTACT USERS</action>" +
                await SysCommon.GetRequestDefaults(true) +
                "</command>" +
                "</imclient>");

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

        protected override Task<string> CreateRequest(XDocument criteriaFilter)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(string familyCode)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityFkRefno)
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