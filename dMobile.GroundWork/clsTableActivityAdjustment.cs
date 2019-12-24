namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using Microsoft.VisualBasic;

  public class TableActivityAdjustment : TableBase
  {
    #region Constructors

    public TableActivityAdjustment()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityAdjustment";
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
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingAdjustments), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustments.", true);
#endif
        
        reqData = this.SendComm(this.Create());
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingAdjustmentsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustments complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingAdjustmentsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustments failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingAdjustmentsFailed), ex);
#else
        throw new Exception("Seeding adjustments failed.", ex);
#endif
      }
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingAdjustments), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustments.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityAdjustment " 
              + "WHERE tActivityAdjustment.ActivitySubType = " + (int)SysCommon.ActSubTypes.Adjustment;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivity " 
              + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.Adjustment;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingAdjustmentsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustments complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingAdjustmentsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustments failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgeAdjustmentsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purge adjustments failed.", ex);
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
          cmndStatus.CommandText = "SELECT COUNT(tActivity.ActivityRefno)" 
              + " FROM tActivity" 
              + " WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.Adjustment;
          cmndStatus.CommandType = CommandType.Text;
          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Adjustmentszz), rowsAffected), 
#else
              string.Format("{0} adjustment(s)", rowsAffected), 
#endif
              true);
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredAdjustmentsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Adjustments status failed.", ex);
#endif
      }

      return rowsAffected;
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

            using (SQLiteCommand cmndAdj = conn.CreateCommand())
            using (SQLiteCommand cmndAct = conn.CreateCommand())
            using (SQLiteCommand cmndID = conn.CreateCommand())
            using (SQLiteCommand cmndWhs = conn.CreateCommand())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustments.", false);

                        // Receiving schema but no data results in no table being created in the dataset.
                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityAdjustment"].Rows.Count);

                            // TODO: Resource Strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            cmndAct.CommandText = "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (@activityStatus, @activitySubType)";
                            cmndAct.CommandType = CommandType.Text;

                            cmndAdj.CommandText = "INSERT INTO tActivityAdjustment (ActivityRefno, " +
                                "ActivitySubType, " +
                                "ActivityAdjustmentFKRefno, " +
                                "ActivityAdjustmentStringFKRefno, " +
                                "ActivityAdjustmentType, " +
                                "WarehouseLocationMapRefno, " +
                                "ActivityAdjustmentDateTime, " +
                                "ActivityAdjustmentDescription, " +
                                "ActivityAdjustmentTypeDescription) " +
                                "VALUES " +
                                "(@activityRefno, " +
                                "@activitySubType, " +
                                "@activityAdjustmentFKRefno, " +
                                "@activityAdjustmentStringFKRefno, " +
                                "@activityAdjustmentType, " +
                                "@warehouseLocationMapRefno, " +
                                "@activityAdjustmentDateTime, " +
                                "@activityAdjustmentDescription, " +
                                "@activityAdjustmentTypeDescription)";
                            cmndAdj.CommandType = CommandType.Text;
                            //cmndAdj.IndexName = "UQ_ActivityAdjustmentOnActivityAdjustmentFKRefno";

                            //using (SqlCeResultSet rsAct = cmndAct.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recAct = rsAct.CreateRecord();

                            cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivity";
                            cmndID.CommandType = CommandType.Text;

                            cmndWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                            cmndWhs.CommandType = CommandType.Text;
                            //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

                                //using (SqlCeResultSet rs = cmndAdj.ExecuteResultSet(ResultSetOptions.Updatable))
                                //{
                                //    SqlCeUpdatableRecord rec = rs.CreateRecord();
                                int actRefno = 0;
                                int progValue = 0;
                                int whsLocMapRefno = 0;
                                string actAdjTypeDesc = string.Empty;
                                int adjFKRefno = 0;

                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                                foreach (DataRow drow in ds.Tables["tActivityAdjustment"].Rows)
                                {
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", Convert.ToInt32(drow["LocationMapWarehouseFKRefno"])));

                                    using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                                    {
                                        if (drWhs.Read())
                                        {
                                            whsLocMapRefno = drWhs.GetInt32((int)SysCommon.LocMapWhsFields.LocMapRefno);
                                        }
                                        else
                                        {
                                            // TODO: Resource String
                                            throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
                                        }
                                    }

                                    // TODO: Resource Strings
                                    errMsg = "Arguments:";

                                    cmndAct.Parameters.Add(new SQLiteParameter("@activityStatus", Convert.ToInt32(0)));
                                    cmndAct.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Adjustment)));
                                    cmndAct.ExecuteNonQuery();

                                    //recAct.SetInt32((int)SysCommon.ActFields.SubType, (int)SysCommon.ActSubTypes.Adjustment);
                                    //recAct.SetInt32((int)SysCommon.ActFields.Status, 0);
                                    //rsAct.Insert(recAct);

                                    switch (Convert.ToInt32(drow["ActivityAdjustmentType"]))
                                    {
                                        case (int)SysCommon.AdjTypes.Inventory:
                                            actAdjTypeDesc = "Inventory";
                                            break;

                                        case (int)SysCommon.AdjTypes.UnitConversion:
                                            actAdjTypeDesc = "Unit Converstion";
                                            break;

                                        case (int)SysCommon.AdjTypes.Rework:
                                            actAdjTypeDesc = "Rework";
                                            break;

                                        case (int)SysCommon.AdjTypes.PartialRework:
                                            actAdjTypeDesc = "Partial Rework";
                                            break;

                                        case (int)SysCommon.AdjTypes.Production:
                                            actAdjTypeDesc = "Production";
                                            break;

                                        case (int)SysCommon.AdjTypes.PartialProduction:
                                            actAdjTypeDesc = "Partial Production";
                                            break;

                                        case (int)SysCommon.AdjTypes.Disposal:
                                            actAdjTypeDesc = "Disposal";
                                            break;

                                        case (int)SysCommon.AdjTypes.Fresh2Frozen:
                                            actAdjTypeDesc = "Fresh To Frozen";
                                            break;

                                        case (int)SysCommon.AdjTypes.Transfer:
                                            actAdjTypeDesc = "Transfer";
                                            break;

                                        case (int)SysCommon.AdjTypes.PackOff:
                                            actAdjTypeDesc = "Pack Off";
                                            break;

                                        case (int)SysCommon.AdjTypes.LostFound:
                                            actAdjTypeDesc = "Lost And Found";
                                            break;

                                        case (int)SysCommon.AdjTypes.CycleCount:
                                            actAdjTypeDesc = "Cycle Count";
                                            break;

                                        case (int)SysCommon.AdjTypes.MoveToDeli:
                                            actAdjTypeDesc = "Move To Deli";
                                            break;

                                        case (int)SysCommon.AdjTypes.PeriodicCount:
                                            actAdjTypeDesc = "Periodic Count";
                                            break;

                                        case (int)SysCommon.AdjTypes.Manufacture:
                                            actAdjTypeDesc = "Manufacture";
                                            break;

                                        case (int)SysCommon.AdjTypes.Requisition:
                                            actAdjTypeDesc = "Requisition";
                                            break;

                                        case (int)SysCommon.AdjTypes.AutoConvertTo:
                                            actAdjTypeDesc = "Auto Convert To";
                                            break;

                                        case (int)SysCommon.AdjTypes.AutoCreateFrom:
                                            actAdjTypeDesc = "Auto Create From";
                                            break;

                                        case (int)SysCommon.AdjTypes.Debox:
                                            actAdjTypeDesc = "Debox";
                                            break;

                                        case (int)SysCommon.AdjTypes.ConvertSkid:
                                            actAdjTypeDesc = "Convert Skid";
                                            break;
                                    }

                                    actRefno = Convert.ToInt32(cmndID.ExecuteScalar());
                                    adjFKRefno = Convert.ToInt32(drow["ActivityAdjustmentFKRefno"]);
                                    //whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, drWhs);
                                    DateTime dt = Convert.ToDateTime(drow["ActivityAdjustmentDateTime"]);

                                    cmndAdj.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));
                                    cmndAdj.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Adjustment)));

                                    // TODO: Resource Strings
                                    errMsg += Environment.NewLine + "FK refno " + adjFKRefno;

                                    cmndAdj.Parameters.Add(new SQLiteParameter("@activityAdjustmentFKRefno", adjFKRefno));
                                    cmndAdj.Parameters.Add(new SQLiteParameter("@activityAdjustmentStringFKRefno", adjFKRefno.ToString()));
                                    cmndAdj.Parameters.Add(new SQLiteParameter("@activityAdjustmentType", Convert.ToInt32(drow["ActivityAdjustmentType"])));
                                    
                                    // TODO: Resource Strings
                                    errMsg += Environment.NewLine + "Adj type \'" + actAdjTypeDesc + "\'";

                                    cmndAdj.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", whsLocMapRefno));
                                    cmndAdj.Parameters.Add(new SQLiteParameter("@activityAdjustmentDateTime", DateTime.Parse(dt.ToShortDateString())));
                                    cmndAdj.Parameters.Add(new SQLiteParameter("@activityAdjustmentDescription", actAdjTypeDesc));
                                    cmndAdj.Parameters.Add(new SQLiteParameter("@activityAdjustmentTypeDescription", drow["ActivityAdjustmentDescription"].ToString()));

                                    cmndAdj.ExecuteNonQuery();

                                    //rec.SetInt32((int)SysCommon.ActAdjFields.ActRefno, actRefno);
                                    //rec.SetInt32((int)SysCommon.ActAdjFields.ActSubType, Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));
                                    //rec.SetInt32((int)SysCommon.ActAdjFields.FKRefno, adjFKRefno);
                                    //rec.SetString((int)SysCommon.ActAdjFields.StringFKRefno, adjFKRefno.ToString());

                                    //rec.SetInt32((int)SysCommon.ActAdjFields.WhsLocMapRefno, whsLocMapRefno);


                                    //rec.SetDateTime((int)SysCommon.ActAdjFields.DateTime, DateTime.Parse(dt.ToShortDateString()));
                                    //rec.SetInt32((int)SysCommon.ActAdjFields.Type, Convert.ToInt32(drow["ActivityAdjustmentType"]));

                                    //rec.SetString((int)SysCommon.ActAdjFields.TypeDescription, actAdjTypeDesc);
                                    //rec.SetString((int)SysCommon.ActAdjFields.Description, drow["ActivityAdjustmentDescription"].ToString());
                                    //rs.Insert(rec);

                                    progValue++;
                                    if (progValue % 10 == 0)
                                    {
                                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                    }
                                }

                                transaction.Commit();
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                            }
                        }
                    //}


                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustments complete.", true);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    transaction.Dispose();

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustments failed.", true);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource Strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource Strings
                    throw new Exception("Database exception occurred. Saving adjustments failed." + Environment.NewLine + errMsg, ex);
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
          + "<action>REQUEST ADJUSTMENTS</action>"
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
