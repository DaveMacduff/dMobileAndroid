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

            // TODO: Uncomment GC calls
            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingUsers), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding users.", true);
#endif

                this.SeedContactUsersWithSystemUsers();

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
                SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingUsers), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging users.", true);
#endif

                using (SQLiteCommand cmndPurge = conn.CreateCommand())
                using (SQLiteTransaction transaction = conn.BeginTransaction())
                {
                    cmndPurge.CommandText = "DELETE FROM tContactUser";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();

                    transaction.Commit();
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

        public void PurgeTContact()
        {
            try
            {
                SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;

                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging tContact.", true);

                using (SQLiteCommand cmndPurge = conn.CreateCommand())
                using (SQLiteTransaction transaction = conn.BeginTransaction())
                {
                    cmndPurge.CommandText = "DELETE FROM tContact";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();

                    transaction.Commit();
                }

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
        public void SeedContactUsersWithSystemUsers()
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
                                    cmndID.CommandText = "SELECT last_insert_rowid() FROM tContact";
                                    cmndID.CommandType = CommandType.Text;

                                    // Create User 30
                                    cmndContact.ExecuteNonQuery();
                                    int contactRefno = Convert.ToInt32(cmndID.ExecuteScalar());
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

                                    cmndContactUser.ExecuteNonQuery();
                                    cmndContactUser.Parameters.Clear();

                                    // Create User 99
                                    cmndContact.ExecuteNonQuery();
                                    contactRefno = Convert.ToInt32(cmndID.ExecuteScalar());
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

                                    cmndContactUser.ExecuteNonQuery();
                                }
                            }

                            reader.Close();
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
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            try
            {
                using (DataSet ds = new DataSet())
                {
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
                    ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving users.", false);

                    if (ds.Tables.Count != 0)
                    {
                        int progMax = Convert.ToInt32(ds.Tables["tContactUser"].Rows.Count);
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                        using (SQLiteCommand cmndUser = conn.CreateCommand())
                        using (SQLiteTransaction transaction = conn.BeginTransaction())
                        {
                            cmndUser.CommandText = "INSERT INTO tContactUser " +
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
                            cmndUser.CommandType = CommandType.Text;

                            using (SQLiteCommand cmndContact = conn.CreateCommand())
                            {
                                cmndContact.CommandText = "INSERT INTO tContact (ContactSubType) VALUES (@contactSubType)";
                                cmndContact.CommandType = CommandType.Text;
                                cmndContact.Parameters.Add(new SQLiteParameter("@contactSubType", (int)SysCommon.ContactSubTypes.User));

                                using (SQLiteCommand cmndID = conn.CreateCommand())
                                {
                                    cmndID.CommandText = "SELECT last_insert_rowid() FROM tContact";
                                    cmndID.CommandType = CommandType.Text;

                                    int progValue = 0;
                                    int contactRefno = 0;
                                    int contactCode = 0;
                                    string contactName = string.Empty;

                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                                    foreach (DataRow drow in ds.Tables["tContactUser"].Rows)
                                    {
                                        cmndContact.ExecuteNonQuery();

                                        contactRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                        contactCode = Convert.ToInt32(drow["ContactUserCode"]);
                                        errMsg = "Code " + contactCode;
                                        contactName = drow["ContactUserName"].ToString();
                                        errMsg += Environment.NewLine + "Name \'" + contactName + "\'";

                                        cmndUser.Parameters.Clear();
                                        cmndUser.Parameters.Add(new SQLiteParameter("@contactRefno", contactRefno));
                                        cmndUser.Parameters.Add(new SQLiteParameter("@contactSubType", (int)SysCommon.ContactSubTypes.User));
                                        cmndUser.Parameters.Add(new SQLiteParameter("@contactUserCode", contactCode));
                                        cmndUser.Parameters.Add(new SQLiteParameter("@contactUserName", contactName));
                                        cmndUser.Parameters.Add(new SQLiteParameter("@contactUserPassword", drow["ContactUserPassword"].ToString()));
                                        cmndUser.Parameters.Add(new SQLiteParameter("@contactUserIsAllowedAssociateProductBarCode", Convert.ToBoolean(drow["ContactUserIsAllowedAssociateProductBarCode"])));
                                        cmndUser.Parameters.Add(new SQLiteParameter("@contactUserIsAllowedPickingModuleOverridePicker", Convert.ToBoolean(drow["ContactUserIsAllowedPickingModuleOverridePicker"])));
                                        cmndUser.Parameters.Add(new SQLiteParameter("@contactUserIsAllowedPickingModuleOverrideAddManualDetail", Convert.ToBoolean(drow["ContactUserIsAllowedPickingModuleOverrideAddManualDetail"])));
                                        cmndUser.Parameters.Add(new SQLiteParameter("@contactUserLocalizationLanguage", drow["ContactUserLocalizationLanguage"].ToString()));

                                        cmndUser.ExecuteNonQuery();

                                        progValue++;
                                        if (progValue % 100 == 0)
                                        {
                                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                        }
                                    }

                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                                }
                            }

                            transaction.Commit();
                        }
                    }

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving users complete.", false);
                }
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving users failed.", false);
                throw new Exception("Database exception occurred. Saving users failed." + Environment.NewLine + errMsg, ex);
            }
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
