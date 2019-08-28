namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using Microsoft.VisualBasic;

  public class TableActivityPhysicalCount : TableBase
  {
    #region Constructors

    public TableActivityPhysicalCount()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityPhysicalCount";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingPhysicalCounts), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical counts.", true);
#endif

#if MULTILANGUAGE
        reqData = this.Send(Translator.GetInstance().Translate(TranslatorVocabulary.PhysicalCountsz));
#else
        reqData = this.Send("physical counts");
#endif
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingPhysicalCountsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical counts complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingPhysicalCountsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical counts failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingPhysicalCountsFailed), ex);
#else
        throw new Exception("Seeding physical counts failed.", ex);
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
        using (SqlCeCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndStatus.CommandText = "SELECT COUNT(tActivity.ActivityRefno) " 
              + "FROM tActivity " 
              + "WHERE tActivity.ActivitySubType = " 
              + (int)SysCommon.ActSubTypes.PhysicalCounts;

          cmndStatus.CommandType = CommandType.Text;
          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.PhysicalCountszz), rowsAffected), 
#else
              string.Format("{0} physical counts.", rowsAffected), 
#endif
              true);
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPhysicalCountsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Physical counts status failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingPhysicalCounts), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical counts.", true);
#endif

        using (SqlCeCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityCount " 
              + "WHERE tActivityCount.ActivitySubType = " + (int)SysCommon.ActSubTypes.PhysicalCounts;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivity " 
              + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.PhysicalCounts;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingPhysicalCountsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical counts complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingPhysicalCountsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical counts failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingPhysicalCountsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging physical counts failed.", ex);
#endif
      }
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
      SqlCeConnection conn = DatabaseConnection.GetInstance().SQLConn;
      string errMsg = string.Empty;

      try
      {
        using (DataSet ds = new DataSet())
        {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.ReadingReceivedData), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
#endif
          ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingPhysicalCounts), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical counts.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["tActivityCount"].Rows.Count);

            this.DataAccessStatusOutHandler(
                this, 
                EventArgs.Empty, 
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax),
#else
                string.Format("Records {0}", progMax),
#endif
                false);

            using (SqlCeCommand cmndCount = conn.CreateCommand())
            {
              cmndCount.CommandText = "tActivityCount";
              cmndCount.CommandType = CommandType.TableDirect;
              cmndCount.IndexName = "UQ_ActivityCountOnActivityCountFKRefno";

              using (SqlCeCommand cmndAct = conn.CreateCommand())
              {
                cmndAct.CommandText = "tActivity";
                cmndAct.CommandType = CommandType.TableDirect;

                using (SqlCeResultSet rsAct = cmndAct.ExecuteResultSet(ResultSetOptions.Updatable))
                {
                  SqlCeUpdatableRecord recAct = rsAct.CreateRecord();

                  using (SqlCeCommand cmndID = conn.CreateCommand())
                  {
                    cmndID.CommandText = "SELECT @@IDENTITY";
                    cmndID.CommandType = CommandType.Text;

                    using (SqlCeCommand cmndWhs = conn.CreateCommand())
                    {
                      cmndWhs.CommandText = "tLocationMapWarehouse";
                      cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                      cmndWhs.CommandType = CommandType.TableDirect;

                      using (SqlCeDataReader drWhs = cmndWhs.ExecuteReader())
                      {
                        using (SqlCeResultSet rs = cmndCount.ExecuteResultSet(ResultSetOptions.Updatable))
                        {
                          SqlCeUpdatableRecord rec = rs.CreateRecord();
                          int actRefno = 0;
                          int progValue = 0;
                          int whsLocMapRefno = 0;
                          string actCountTypeDesc = string.Empty;
                          int countFKRefno = 0;

                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                          foreach (DataRow drow in ds.Tables["tActivityCount"].Rows)
                          {
                            errMsg = "Arguments:";

                            recAct.SetInt32(
                                (int)SysCommon.ActFields.SubType, 
                                (int)SysCommon.ActSubTypes.PhysicalCounts);

                            recAct.SetInt32((int)SysCommon.ActFields.Status, 0);

                            rsAct.Insert(recAct);

                            actRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                            rec.SetInt32((int)SysCommon.ActCountFields.ActRefno, actRefno);
                            rec.SetInt32(
                                (int)SysCommon.ActCountFields.ActSubType, 
                                (int)SysCommon.ActSubTypes.PhysicalCounts);

                            countFKRefno = Convert.ToInt32(drow["ActivityCountFKRefno"]);
                            errMsg += Environment.NewLine + "FK refno " + countFKRefno;
                            rec.SetInt32((int)SysCommon.ActCountFields.FKRefno, countFKRefno);
                            rec.SetString(
                                (int)SysCommon.ActCountFields.StringFKRefno,
                                countFKRefno.ToString());

                            whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, drWhs);
                            rec.SetInt32((int)SysCommon.ActCountFields.WhsLocMapRefno, whsLocMapRefno);

                            rec.SetInt32(
                                (int)SysCommon.ActCountFields.Type, 
                                Convert.ToInt32(drow["ActivityCountType"]));

                            // To remove time - convert twice
                            DateTime dt = Convert.ToDateTime(drow["ActivityCountDateTime"]);
                            rec.SetDateTime(
                                (int)SysCommon.ActCountFields.DateTime,
                                DateTime.Parse(dt.ToShortDateString()));

                            switch (Convert.ToInt32(drow["ActivityCountType"]))
                            {
                              case (int)SysCommon.ActCountTypeFields.PhysicalCountByWarehouse:
#if MULTILANGUAGE
                                actCountTypeDesc = Translator.GetInstance().Translate(TranslatorVocabulary.StandardPhysicalCountByWarehouse);
#else
                                actCountTypeDesc = "Standard Physical Count By Warehouse";
#endif
                                break;

                              case (int)SysCommon.ActCountTypeFields.CycleCountByWarehouse:
#if MULTILANGUAGE
                                actCountTypeDesc = Translator.GetInstance().Translate(TranslatorVocabulary.CycleCountByWarehouse);
#else
                                actCountTypeDesc = "Cycle Count By Warehouse";
#endif
                                break;

                              case (int)SysCommon.ActCountTypeFields.CycleCountBySection:
#if MULTILANGUAGE
                                actCountTypeDesc = Translator.GetInstance().Translate(TranslatorVocabulary.CycleCountBySection);
#else
                                actCountTypeDesc = "Cycle Count By Section";
#endif
                                break;

                              case (int)SysCommon.ActCountTypeFields.CycleCountBySlot:
#if MULTILANGUAGE
                                actCountTypeDesc = Translator.GetInstance().Translate(TranslatorVocabulary.CycleCountBySlot);
#else
                                actCountTypeDesc = "Cycle Count By Slot";
#endif
                                break;

                              case (int)SysCommon.ActCountTypeFields.BaseCycleCountByWarehouse:
#if MULTILANGUAGE
                                actCountTypeDesc = Translator.GetInstance().Translate(TranslatorVocabulary.BaseCycleCountByWarehouse);
#else
                                actCountTypeDesc = "Base Cycle Count By Warehouse";
#endif
                                break;
                            }

                            errMsg += Environment.NewLine + "Type \'" + actCountTypeDesc + "\'";
                            rec.SetString((int)SysCommon.ActCountFields.TypeDescription, actCountTypeDesc);

                            // <wo="14563" comment="receiving strings up to 75 in length when database wants max 60. This is the viewable description when selecting line item." />
                            string incomingDescription = ReduceWhitespace(drow["ActivityCountDescription"].ToString());
                            if (incomingDescription.Length > 60)
                                incomingDescription = incomingDescription.Replace(" ", string.Empty); // remove all spaces
                            if (incomingDescription.Length > 60)
                                incomingDescription = incomingDescription.Substring(0, 60); // truncate the end if we need to.

                            rec.SetString(
                                (int)SysCommon.ActCountFields.Description, 
                                incomingDescription);

                            rs.Insert(rec);

                            progValue++;
                            if (progValue % 10 == 0)
                            {
                              this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                            }
                          }

                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        drWhs.Close();
                      }
                    }
                  }
                }
              }
            }
          }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingPhysicalCountsComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical counts complete.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingPhysicalCountsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical counts failed.", true);
#endif

        // Duplicate key violations
        if (ex.NativeError == 25016)
        {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation), false);
          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation) + Environment.NewLine + errMsg, ex);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
          throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
#endif
        }

        throw new Exception(
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingPhysicalCountsFailed) + Environment.NewLine + errMsg,
#else
            "Database exception occurred. Saving physical counts failed." + Environment.NewLine + errMsg,
#endif
            ex);
      }
    }


    /// <summary>
    /// Reduces excess whitespace in a string.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private string ReduceWhitespace(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        StringBuilder sb = new StringBuilder();
        bool prevIsWhitespace = false;
        char[] charArray = input.ToCharArray();
        for (int i = 0; i < input.Length; ++i)
        {
            if (Char.IsWhiteSpace(charArray[i]))
            {
                if (prevIsWhitespace)
                    continue;
                prevIsWhitespace = true;
            }
            else
            {
                prevIsWhitespace = false;
            }

            sb.Append(charArray[i]);
        }
        return sb.ToString();
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
          + "<action>REQUEST PHYSICAL COUNTS</action>" 
          + SysCommon.GetRequestDefaults(false)
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
