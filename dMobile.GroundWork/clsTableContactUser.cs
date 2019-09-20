namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableContactUsers : TableBase
  {
    #region Constructor

    public TableContactUsers()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableContactUsers";
    }

    #endregion

    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
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
    ///
    /// </summary>
    /// <remarks></remarks>
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
    ///
    /// </summary>
    /// <remarks></remarks>
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
    ///
    /// </summary>
    /// <remarks></remarks>
    public void SeedContactUsersWithSystemUsers()
    {
//      string errMsg = string.Empty;

//      try
//      {
//        using (SQLiteCommand cmndUser = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//        {
//          cmndUser.CommandText = "tContactUser";
//          cmndUser.CommandType = CommandType.TableDirect;
//          cmndUser.IndexName = "UQ_ContactUserOnContactUserCode";

//          using (SQLiteCommand cmndContact = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//          {
//            cmndContact.CommandText = "tContact";
//            cmndContact.CommandType = CommandType.TableDirect;

//            using (SqlCeResultSet rsContact = cmndContact.ExecuteResultSet(ResultSetOptions.Updatable))
//            {
//              SqlCeUpdatableRecord recContact = rsContact.CreateRecord();

//              using (SQLiteCommand cmndID = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//              {
//                cmndID.CommandText = "SELECT @@IDENTITY";
//                cmndID.CommandType = CommandType.Text;

//                using (SqlCeResultSet rs = cmndUser.ExecuteResultSet(ResultSetOptions.Updatable))
//                {
//                  SqlCeUpdatableRecord rec = rs.CreateRecord();

//                  using (SQLiteDataReader drUser = cmndUser.ExecuteReader())
//                  {
//                    int contactRefno = 0;
//                    int contactCode = 0;
//                    string contactName = string.Empty;
                    
//                    // Create User 30
//                    contactCode = SysCommon.SystemLoginID;
//                    errMsg = "Code " + contactCode;
//                    contactName = "InterSect";
//                    errMsg += Environment.NewLine + "Name \'" + contactName + "\'";

//                    if (!drUser.Seek(DbSeekOptions.FirstEqual, new object[] { contactCode }))
//                    {
//                      recContact.SetInt32(
//                          (int)SysCommon.ContactFields.SubType, 
//                          (int)SysCommon.ContactSubTypes.User);
//                      rsContact.Insert(recContact);
//                      contactRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                      rec.SetInt32((int)SysCommon.ContactUserFields.ContactRefno, contactRefno);
//                      rec.SetInt32(
//                          (int)SysCommon.ContactFields.SubType, 
//                          (int)SysCommon.ContactSubTypes.User);
//                      rec.SetInt32((int)SysCommon.ContactUserFields.Code, contactCode);
//                      rec.SetString((int)SysCommon.ContactUserFields.Name, contactName);

//                      // ibsi 4274
//                      rec.SetString(
//                          (int)SysCommon.ContactUserFields.Password, 
//                          SysCommon.SystemLoginPassword); 

//                      rec.SetBoolean((int)SysCommon.ContactUserFields.IsAllowedAssocProdBarCode, true);
//                      rec.SetBoolean((int)SysCommon.ContactUserFields.IsAllowedPickingOverridePicker, true);
//                      rec.SetBoolean(
//                          (int)SysCommon.ContactUserFields.IsAllowedPickingModuleOverrideAddManualDetail, true);
//                      rec.SetString(
//                          (int)SysCommon.ContactUserFields.UserLocalizationLanguage,
//                          SysEnv.SysSettings.DefaultLanguage);
//                      rs.Insert(rec);
//                    }

//                    // Create User 99
//                    contactCode = SysCommon.SetupLoginID;
//                    errMsg = "Code " + contactCode;
//                    contactName = "Setup User";
//                    errMsg += Environment.NewLine + "Name \'" + contactName + "\'";

//                    if (!drUser.Seek(DbSeekOptions.FirstEqual, new object[] { contactCode }))
//                    {
//                      recContact.SetInt32((int)SysCommon.ContactFields.SubType, (int)SysCommon.ContactSubTypes.User);
                      
//                      rsContact.Insert(recContact);

//                      contactRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                      rec.SetInt32((int)SysCommon.ContactUserFields.ContactRefno, contactRefno);
//                      rec.SetInt32((int)SysCommon.ContactFields.SubType, (int)SysCommon.ContactSubTypes.User);
//                      rec.SetInt32((int)SysCommon.ContactUserFields.Code, contactCode);
//                      rec.SetString((int)SysCommon.ContactUserFields.Name, contactName);
//                      rec.SetString((int)SysCommon.ContactUserFields.Password, SysCommon.SetupLoginPassword);
//                      rec.SetBoolean((int)SysCommon.ContactUserFields.IsAllowedAssocProdBarCode, true);
//                      rec.SetBoolean((int)SysCommon.ContactUserFields.IsAllowedPickingOverridePicker, false);
//                      rec.SetBoolean(
//                          (int)SysCommon.ContactUserFields.IsAllowedPickingModuleOverrideAddManualDetail, false);
//                      rec.SetString(
//                        (int)SysCommon.ContactUserFields.UserLocalizationLanguage,
//                        SysEnv.SysSettings.DefaultLanguage);
//                      rs.Insert(rec);
//                    }

//                    drUser.Close();
//                  }
//                }
//              }
//            }
//          }
//        }
//      }
//      catch (SQLiteException ex)
//      {
//        // Duplicate key violations
//        if (ex.NativeError == 25016)
//        {
//#if MULTILANGUAGE
//          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation) + Environment.NewLine + errMsg, ex);
//#else
//          throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
//#endif
//        }

//        throw new Exception(
//#if MULTILANGUAGE
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingSystemUsersFailed) + Environment.NewLine + errMsg,
//#else
//            "Database exception occurred. Seeding system users failed." + Environment.NewLine + errMsg,
//#endif
//            ex);
//      }
    }

    #endregion

    #region Protected Methods

    /// <summary>
    ///
    /// </summary>
    /// <param name="reqData"></param>
    /// <remarks></remarks>
    protected override void Save(string reqData)
    {
//      SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
//      string errMsg = string.Empty;

//      try
//      {
//        using (DataSet ds = new DataSet())
//        {
//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.ReadingReceivedData), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
//#endif
//          ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingUsers), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving users.", false);
//#endif
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tContactUser"].Rows.Count);

//            this.DataAccessStatusOutHandler(
//                this, 
//                EventArgs.Empty,
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), 
//#else
//                string.Format("Records {0}", progMax), 
//#endif
//                false);

//            using (SQLiteCommand cmndUser = conn.CreateCommand())
//            {
//              cmndUser.CommandText = "tContactUser";
//              cmndUser.CommandType = CommandType.TableDirect;

//              using (SQLiteCommand cmndContact = conn.CreateCommand())
//              {
//                cmndContact.CommandText = "tContact";
//                cmndContact.CommandType = CommandType.TableDirect;

//                using (SqlCeResultSet rsContact = cmndContact.ExecuteResultSet(ResultSetOptions.Updatable))
//                {
//                  SqlCeUpdatableRecord recContact = rsContact.CreateRecord();

//                  using (SQLiteCommand cmndID = conn.CreateCommand())
//                  {
//                    cmndID.CommandText = "SELECT @@IDENTITY FROM tContact";
//                    cmndID.CommandType = CommandType.Text;

//                    using (SqlCeResultSet rs = cmndUser.ExecuteResultSet(ResultSetOptions.Updatable))
//                    {
//                      SqlCeUpdatableRecord rec = rs.CreateRecord();
//                      int progValue = 0;
//                      int contactRefno = 0;
//                      int contactCode = 0;
//                      string contactName = string.Empty;

//                      this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);
//                      foreach (DataRow drow in ds.Tables["tContactUser"].Rows)
//                      {
//                        recContact.SetInt32(
//                            (int)SysCommon.ContactFields.SubType, 
//                            (int)SysCommon.ContactSubTypes.User);

//                        rsContact.Insert(recContact);

//                        contactRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                        contactCode = Convert.ToInt32(drow["ContactUserCode"]);
//                        errMsg = "Code " + contactCode;
//                        contactName = drow["ContactUserName"].ToString();
//                        errMsg += Environment.NewLine + "Name \'" + contactName + "\'";

//                        rec.SetInt32((int)SysCommon.ContactUserFields.ContactRefno, contactRefno);

//                        rec.SetInt32(
//                            (int)SysCommon.ContactUserFields.ContactSubType, 
//                            (int)SysCommon.ContactSubTypes.User);

//                        rec.SetInt32((int)SysCommon.ContactUserFields.Code, contactCode);
//                        rec.SetString((int)SysCommon.ContactUserFields.Name, contactName);

//                        rec.SetString(
//                            (int)SysCommon.ContactUserFields.Password, 
//                            drow["ContactUserPassword"].ToString());

//                        rec.SetBoolean(
//                            (int)SysCommon.ContactUserFields.IsAllowedAssocProdBarCode, 
//                            Convert.ToBoolean(drow["ContactUserIsAllowedAssociateProductBarCode"]));

//                        rec.SetBoolean(
//                            (int)SysCommon.ContactUserFields.IsAllowedPickingOverridePicker, 
//                            Convert.ToBoolean(drow["ContactUserIsAllowedPickingModuleOverridePicker"]));

//                        rec.SetBoolean(
//                            (int)SysCommon.ContactUserFields.IsAllowedPickingModuleOverrideAddManualDetail,
//                            Convert.ToBoolean(drow["ContactUserIsAllowedPickingModuleOverrideAddManualDetail"]));

//                        rec.SetString(
//                            (int)SysCommon.ContactUserFields.UserLocalizationLanguage,
//                            drow["ContactUserLocalizationLanguage"].ToString()
//                            );

//                        rs.Insert(rec);

//                        progValue++;
//                        if (progValue % 100 == 0)
//                        {
//                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
//                        }
//                      }

//                      this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
//                    }
//                  }
//                }
//              }
//            }
//          }

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingUsersComplete), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving users complete.", false);
//#endif
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingUsersFailed), false);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving users failed.", false);
//#endif

//        // Duplicate key violations
//        if (ex.NativeError == 25016)
//        {
//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation), false);
//          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation) + Environment.NewLine + errMsg, ex);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
//          throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
//#endif
//        }

//        throw new Exception(
//#if MULTILANGUAGE
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingUsersFailed) + Environment.NewLine + errMsg, 
//#else
//            "Database exception occurred. Saving users failed." + Environment.NewLine + errMsg, 
//#endif
//            ex);
//      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
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

    #endregion
  }
}
