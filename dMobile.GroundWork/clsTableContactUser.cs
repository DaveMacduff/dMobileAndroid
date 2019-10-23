namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

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
        public override void Request()
        {
            string reqData = string.Empty;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            try
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingUsers), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding users.", true);
#endif

                this.SeedContactUsersWithSystemUsers().ContinueWith(result => {

                    reqData = this.Send("contact users");

                    if (0 < reqData.Length)
                    {
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            XmlDocument reqErr = new XmlDocument();
                            reqErr.LoadXml(reqData);
                            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                        }

                        this.Save(reqData);
                    }

    #if MULTILANGUAGE
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingUsersComplete), true);
    #else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding users complete.", true);
    #endif
                });

            }
            catch (Exception ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingUsersFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding users failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingUsersFailed), ex);
#else
                throw new Exception("Seeding users failed.", ex);
#endif
            }
        }

        /// <summary>
        /// The DataStatus
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public override int DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                using (SQLiteCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tContactUser";
                    cmndStatus.CommandType = CommandType.Text;
                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Users), rowsAffected),
#else
              string.Format("{0} users.", rowsAffected),
#endif
              true);
                }
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredUsersStatusFailed), ex);
#else
                throw new Exception("Database exception occurred. Users status failed.", ex);
#endif
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override void Purge()
        {
            try
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingUsers), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging users.", true);
#endif

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tContactUser";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingUsersComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging users complete.", true);
#endif
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingUsersFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging users failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingUsersFailed), ex);
#else
                throw new Exception("Database exception occurred. Purging users failed.", ex);
#endif
            }
        }

        /// <summary>
        /// The SeedContactUsersWithSystemUsers
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public async Task SeedContactUsersWithSystemUsers()
        {
            string errMsg = string.Empty;

            try
            {
                using (SQLiteCommand cmndContactUser = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    //cmndContactUser.IndexName = "UQ_ContactUserOnContactUserCode";
                    using (SQLiteCommand cmndContact = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {

                        cmndContactUser.CommandText = "SELECT ContactUserCode FROM tContactUser WHERE ContactUserCode = @contactUserCodeAdmin OR ContactUserCode = @contactUserCodeSetup";
                        cmndContactUser.CommandType = CommandType.Text;
                        cmndContactUser.Parameters.Add(new SQLiteParameter("@contactUserCodeAdmin", SysCommon.SystemLoginID));
                        cmndContactUser.Parameters.Add(new SQLiteParameter("@contactUserCodeSetup", SysCommon.SetupLoginID));

                        using (SQLiteDataReader reader = cmndContactUser.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                reader.Close();
                                cmndContact.CommandText = "INSERT INTO tContact (ContactSubType) VALUES (@subType)";
                                cmndContact.CommandType = CommandType.Text;
                                cmndContact.Parameters.Add(new SQLiteParameter("@subType", (int)SysCommon.ContactSubTypes.User));

                                using (SQLiteCommand cmndID = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                                {
                                    cmndID.CommandText = "SELECT last_insert_rowid()";
                                    cmndID.CommandType = CommandType.Text;

                                    // Create User 30
                                    await cmndContact.ExecuteNonQueryAsync();
                                    int contactRefno = Convert.ToInt32(await cmndID.ExecuteScalarAsync());
                                    int contactCode = SysCommon.SystemLoginID;
                                    string contactName = "InterSect";
                                    errMsg = "Code " + contactCode;
                                    errMsg += Environment.NewLine + "Name \'" + contactName + "\'";

                                    cmndContactUser.CommandText = "INSERT INTO tContactUser " +
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
                                        "(@contactRefno, " +
                                        "@contactSubType, " +
                                        "@contactUserCode, " +
                                        "@contactUserName, " +
                                        "@contactUserPassword, " +
                                        "@contactUserIsAllowedAssociateProductBarCode, " +
                                        "@contactUserIsAllowedPickingModuleOverridePicker, " +
                                        "@contactUserIsAllowedPickingModuleOverrideAddManualDetail, " +
                                        "@contactUserLocalizationLanguage)";

                                    cmndContactUser.CommandType = CommandType.Text;
                                    cmndContactUser.Parameters.Add(new SQLiteParameter("@contactRefno", contactRefno));
                                    cmndContactUser.Parameters.Add(new SQLiteParameter("@contactSubType", (int)SysCommon.ContactSubTypes.User));
                                    cmndContactUser.Parameters.Add(new SQLiteParameter("@contactUserCode", contactCode));
                                    cmndContactUser.Parameters.Add(new SQLiteParameter("@contactUserName", contactName));
                                    cmndContactUser.Parameters.Add(new SQLiteParameter("@contactUserPassword", SysCommon.SystemLoginPassword));
                                    cmndContactUser.Parameters.Add(new SQLiteParameter("@contactUserIsAllowedAssociateProductBarCode", true));
                                    cmndContactUser.Parameters.Add(new SQLiteParameter("@contactUserIsAllowedPickingModuleOverridePicker", true));
                                    cmndContactUser.Parameters.Add(new SQLiteParameter("@contactUserIsAllowedPickingModuleOverrideAddManualDetail", true));
                                    cmndContactUser.Parameters.Add(new SQLiteParameter("@contactUserLocalizationLanguage", SysEnv.SysSettings.DefaultLanguage));

                                    await cmndContactUser.ExecuteNonQueryAsync();
                                    cmndContactUser.Parameters.Clear();

                                    // Create User 99
                                    await cmndContact.ExecuteNonQueryAsync();
                                    contactRefno = Convert.ToInt32(await cmndID.ExecuteScalarAsync());
                                    contactCode = SysCommon.SetupLoginID;
                                    errMsg = "Code " + contactCode;
                                    contactName = "Setup User";
                                    errMsg += Environment.NewLine + "Name \'" + contactName + "\'";

                                    cmndContactUser.CommandText = "INSERT INTO tContactUser " +
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
                                        "(@contactRefno, " +
                                        "@contactSubType, " +
                                        "@contactUserCode, " +
                                        "@contactUserName, " +
                                        "@contactUserPassword, " +
                                        "@contactUserIsAllowedAssociateProductBarCode, " +
                                        "@contactUserIsAllowedPickingModuleOverridePicker, " +
                                        "@contactUserIsAllowedPickingModuleOverrideAddManualDetail, " +
                                        "@contactUserLocalizationLanguage)";

                                    cmndContactUser.CommandType = CommandType.Text;
                                    cmndContactUser.Parameters.Add(new SQLiteParameter("@contactRefno", contactRefno));
                                    cmndContactUser.Parameters.Add(new SQLiteParameter("@contactSubType", (int)SysCommon.ContactSubTypes.User));
                                    cmndContactUser.Parameters.Add(new SQLiteParameter("@contactUserCode", contactCode));
                                    cmndContactUser.Parameters.Add(new SQLiteParameter("@contactUserName", contactName));
                                    cmndContactUser.Parameters.Add(new SQLiteParameter("@contactUserPassword", SysCommon.SetupLoginPassword));
                                    cmndContactUser.Parameters.Add(new SQLiteParameter("@contactUserIsAllowedAssociateProductBarCode", true));
                                    cmndContactUser.Parameters.Add(new SQLiteParameter("@contactUserIsAllowedPickingModuleOverridePicker", false));
                                    cmndContactUser.Parameters.Add(new SQLiteParameter("@contactUserIsAllowedPickingModuleOverrideAddManualDetail", false));
                                    cmndContactUser.Parameters.Add(new SQLiteParameter("@contactUserLocalizationLanguage", SysEnv.SysSettings.DefaultLanguage));

                                    await cmndContactUser.ExecuteNonQueryAsync();
                                }
                            }
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                //        // Duplicate key violations
                //        if (ex.NativeError == 25016)
                //        {
                //          throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                //} else {}
                throw new Exception("Database exception occurred. Seeding system users failed." + Environment.NewLine + errMsg, ex);
            }
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        protected override void Save(string reqData)
        {
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <returns></returns>
        protected override string Create()
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
