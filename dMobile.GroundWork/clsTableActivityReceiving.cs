namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Windows.Forms;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using Microsoft.VisualBasic;

  public class TableActivityReceiving : TableBase
  {
    #region Constructors

    public TableActivityReceiving()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityReceiving";
    }

    #endregion

    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Request()
    {
      GC.Collect();
      GC.WaitForPendingFinalizers();

      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingReceivings), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receivings.", true);
#endif

        string reqData = this.Send("receiving");
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingReceivingsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receivings complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingReceivingsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receivings failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingReceivingsFailed), ex);
#else
        throw new Exception("Seeding receivings failed.", ex);
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
          cmndStatus.CommandType = CommandType.Text;
          cmndStatus.CommandText = "SELECT COUNT(tActivity.ActivityRefno) "
              + "FROM tActivity "
              + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.Receiving;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this,
              EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Receivings), rowsAffected),
#else
              string.Format("{0} receivings.", rowsAffected),
#endif
              true);
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredReceivingsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Receivings status failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingReceivings), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receivings.", true);
#endif

        using (SqlCeCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.CommandText = "DELETE FROM tActivityReceiving";
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.CommandText = "DELETE FROM tActivity "
              + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.Receiving;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingReceivingsCompleted), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receivings completed.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingReceivingsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receivings failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingReceivingsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging receivings failed.", ex);
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
      string errMsg = string.Empty;
      SqlCeConnection conn = DatabaseConnection.GetInstance().SQLConn;

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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingReceivings), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receivings.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["tActivityPurchaseOrder"].Rows.Count);

            this.DataAccessStatusOutHandler(
                this,
                EventArgs.Empty,
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax),
#else
                string.Format("Records {0}", progMax),
#endif
                false);

            using (SqlCeCommand cmndRecv = conn.CreateCommand())
            {
              cmndRecv.CommandText = "tActivityReceiving";
              cmndRecv.CommandType = CommandType.TableDirect;
              cmndRecv.IndexName = "UQ_ActivityReceivingOnActivityReceivingFKRefno";

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
                        using (SqlCeResultSet rs = cmndRecv.ExecuteResultSet(ResultSetOptions.Updatable))
                        {
                          SqlCeUpdatableRecord rec = rs.CreateRecord();
                          int actRefno = 0;
                          int progValue = 0;
                          int whsLocMapRefno = 0;
                          int recvFKRefno = 0;
                          string recvPONumber = string.Empty;

                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                          foreach (DataRow drow in ds.Tables["tActivityPurchaseOrder"].Rows)
                          {
                            errMsg = "Arguments:";

                            recAct.SetInt32((int)SysCommon.ActFields.SubType, (int)SysCommon.ActSubTypes.Receiving);
                            recAct.SetInt32((int)SysCommon.ActFields.Status, 0);

                            rsAct.Insert(recAct);

                            actRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                            rec.SetInt32(
                                Convert.ToInt32(SysCommon.ActReceivingFields.ActRefno),
                                actRefno);
                            rec.SetInt32(
                                Convert.ToInt32(SysCommon.ActReceivingFields.ActSubType),
                                Convert.ToInt32(SysCommon.ActSubTypes.Receiving));

                            recvFKRefno = Convert.ToInt32(drow["ActivityPurchaseOrderFKRefno"]);
                            errMsg += Environment.NewLine + "Recv fk refno " + recvFKRefno;
                            rec.SetInt32((int)SysCommon.ActReceivingFields.FKRefno, recvFKRefno);
                            rec.SetString(
                                (int)SysCommon.ActReceivingFields.StringFKRefno, 
                                Convert.ToString(recvFKRefno));

                            whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, drWhs);
                            rec.SetInt32((int)SysCommon.ActReceivingFields.WhsLocMapRefno, whsLocMapRefno);

                            rec.SetDateTime(
                                (int)SysCommon.ActReceivingFields.IssuedDateTime,
                                Convert.ToDateTime(drow["ActivityPurchaseOrderIssuedDateTime"]));
                            rec.SetDateTime(
                                (int)SysCommon.ActReceivingFields.ArrivalDateTime,
                                Convert.ToDateTime(drow["ActivityPurchaseOrderArrivalDateTime"]));

                            recvPONumber = drow["ActivityPurchaseOrderPONumber"].ToString();
                            errMsg += Environment.NewLine + "PO number " + recvPONumber;
                            rec.SetString((int)SysCommon.ActReceivingFields.PONumber, recvPONumber);
                            rec.SetString(
                                (int)SysCommon.ActReceivingFields.VendorContactName,
                                drow["ActivityPurchaseOrderVendorContactName"].ToString());
                            rec.SetString(
                                (int)SysCommon.ActReceivingFields.VendorSupplierContactName,
                                drow["ActivityPurchaseOrderVendorSupplierContactName"].ToString());
                            rec.SetString(
                                (int)SysCommon.ActReceivingFields.VendorSupplierGTINCompanyPrefix,
                                drow["ActivityPurchaseOrderVendorSupplierGTINCompanyPrefix"].ToString());

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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingReceivingsComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receivings complete.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingReceivingsFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receivings failed.", false);
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
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingReceivingsFailed) + Environment.NewLine + errMsg,
#else
            "Database exception occurred. Saving receivings failed." + Environment.NewLine + errMsg,
#endif
            ex);
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
            + "<action>REQUEST PURCHASE ORDERS</action>"
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
