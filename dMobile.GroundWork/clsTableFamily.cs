namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableFamily : TableBase
  {
    #region Constructors

    public TableFamily()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableFamily";
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

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding families.", true);

                reqData = this.Send("families");

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

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding families complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding families failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding families failed.", ex);
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
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tFamily";
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} families.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Families status failed.", ex);
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
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging families.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tFamily";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging families complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging families failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging families failed.", ex);
            }
        }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="familyCode"></param>
    /// <returns></returns>
    public int GetiFamilyRefnoFromFamilyCode(string familyCode)
    {
      int familyRefno = 0;

//      try
//      {
//        using (SQLiteCommand cmndFamily = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//        {
//          cmndFamily.CommandText = "tFamily";
//          cmndFamily.IndexName = "UQ_FamilyOnFamilyCode";
//          cmndFamily.CommandType = CommandType.TableDirect;

//          using (SQLiteDataReader drFamily = cmndFamily.ExecuteReader())
//          {
//            if (drFamily.Seek(DbSeekOptions.FirstEqual, new object[] { familyCode }))
//            {
//              if (drFamily.Read())
//              {
//                familyRefno = Convert.ToInt32(drFamily.GetValue((int)SysCommon.FamilyFields.Refno));
//              }
//              else
//              {
//#if MULTILANGUAGE
//                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrfamilyreadFailed));
//#else
//                throw new Exception("drFamily.Read() failed.");
//#endif
//              }
//            }
//            else
//            {
//              throw new Exception(
//                  string.Format(
//#if MULTILANGUAGE
//                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheFamilyRefnoFromFamilyCodeInTfamily),
//#else
//                  "Unable to locate the family refno from family code {0} in tFamily.",
//#endif
//                  familyCode));
//            }

//            drFamily.Close();
//          }
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetifamilyrefnofromfamilycodeFailed), ex);
//#else
//        throw new Exception("Database exception occurred. GetiFamilyRefnoFromFamilyCode failed.", ex);
//#endif
//      }

      return familyRefno;
    }

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="reqData"></param>
        /// <remarks></remarks>
        protected override void Save(string reqData)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource String
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource String
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving families.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = ds.Tables["tFamily"].Rows.Count;

                            // TODO: Resource String
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            using (SQLiteCommand cmndFamily = conn.CreateCommand())
                            {
                                cmndFamily.CommandText = "SELECT FamilyCode FROM tFamily WHERE FamilyCode = @familyCode";
                                cmndFamily.CommandType = CommandType.Text;
                                //cmndFamily.IndexName = "UQ_FamilyOnFamilyCode";

                                int progValue = 0;
                                bool isFamilyFound = false;
                                string familyCode = string.Empty;

                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                                foreach (DataRow drow in ds.Tables["tFamily"].Rows)
                                {
                                    familyCode = drow["FamilyCode"].ToString();
                                    errMsg = "Family \'" + familyCode + "\'";
                                    isFamilyFound = false;

                                    cmndFamily.Parameters.Add(new SQLiteParameter("@familyCode", familyCode));

                                    using (SQLiteDataReader drFamily = cmndFamily.ExecuteReader())
                                    {
                                        if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                        {
                                            if (drFamily.Read())
                                            {
                                                isFamilyFound = true;
                                            }
                                        }

                                        if (!isFamilyFound)
                                        {
                                            using (SQLiteCommand cmndFamilyInsert = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                                            {
                                                cmndFamilyInsert.Parameters.Clear();
                                                cmndFamilyInsert.CommandText = "INSERT INTO tFamily (FamilyCode) VALUES (@familyCode)";
                                                cmndFamilyInsert.CommandType = CommandType.Text;
                                                cmndFamilyInsert.Parameters.Add(new SQLiteParameter("@familyCode", familyCode));
                                                cmndFamilyInsert.ExecuteNonQuery();
                                            }
                                        }

                                        drFamily.Close();
                                    }

                                    progValue++;

                                    if (progValue % 100 == 0)
                                    {
                                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                    }
                                }

                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                                //}

                                //drFamily.Close();
                                //}

                            }
                        }

                        transaction.Commit();

                        // TODO: Resource String
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving families complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource String
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving families failed.", false);

                    //                // Duplicate key violations
                    //                if (ex.NativeError == 25016)
                    //                {
                    //                    // TODO: Resource String
                    //                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //                    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //                }

                    // TODO: Resource String
                    throw new Exception("Database exception occurred. Saving families failed." + Environment.NewLine + errMsg, ex);
                }
            }
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
          + "<action>REQUEST FAMILIES</action>"
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
