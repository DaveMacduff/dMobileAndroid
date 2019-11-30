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

  public class TableActivityPhysicalCountItem : TableBase
  {
    #region Constructors

    public TableActivityPhysicalCountItem()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityPhysicalCountItem";
    }

    #endregion

    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <param name="actRefno"></param>
    /// <remarks></remarks>
    public override void Request(int actRefno)
    {
      string reqData = string.Empty;

      //GC.Collect();
      //GC.WaitForPendingFinalizers();

      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingPhysicalCountItems), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical count items.", true);
#endif

        if (actRefno == 0)
        {
          using (SQLiteCommand cmndCount = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndCount.CommandType = CommandType.Text;
            cmndCount.CommandText = "SELECT tActivity.ActivityRefno " 
                + "FROM tActivity " 
                + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.PhysicalCounts;

            using (SQLiteDataReader drActivityCount = cmndCount.ExecuteReader())
            {
              while (drActivityCount.Read())
              {
                actRefno = Convert.ToInt32(drActivityCount["ActivityRefno"]);
                reqData = this.Send(actRefno);
                if (0 < reqData.Length)
                {
                  if (reqData.Contains("<message>ERROR</message>"))
                  {
                    XmlDocument reqErr = new XmlDocument();
                    reqErr.LoadXml(reqData);
                    throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                  }

                  this.Save(reqData, actRefno);
                }
              }

              drActivityCount.Close();
            }
          }
        }
        else
        {
          reqData = this.Send(actRefno);
          if (0 < reqData.Length)
          {
            if (reqData.Contains("<message>ERROR</message>"))
            {
              XmlDocument reqErr = new XmlDocument();
              reqErr.LoadXml(reqData);
              throw new Exception(reqErr.DocumentElement["detail"].InnerText);
            }

            this.Save(reqData, actRefno);
          }
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingPhysicalCountItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical count items complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingPhysicalCountItemsFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical count items failed.", false);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingPhysicalCountItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding physical count items failed.", ex);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingPhysicalCountItemsFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical count items failed.", false);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingPhysicalCountItemsFailed), ex);
#else
        throw new Exception("Seeding physical count items failed.", ex);
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
          cmndStatus.CommandText = "SELECT COUNT(tActivityItem.ActivityItemRefno) " 
              + "FROM tActivityItem " 
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.PhysicalCounts;
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.PhysicalCountItems), rowsAffected), 
#else
              string.Format("{0} physical count items.", rowsAffected), 
#endif
              true);
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPhysicalCountItemsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Physical count items status failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingPhysicalCountItems), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical count items.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef "
              + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
              + "(SELECT tActivityItem.ActivityItemRefno "
              + "FROM tActivityItem "
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.PhysicalCounts + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItemCount "
              + "WHERE tActivityItemCount.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.PhysicalCounts;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItem "
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.PhysicalCounts;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingPhysicalCountItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical count items complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingPhysicalCountItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical count items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingPhysicalCountItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging physical count items failed.", ex);
#endif
      }
    }

    #endregion

    #region Private Members

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Send(int actRefno)
    {
      int actCountFKRefno = 0;

//      using (SQLiteCommand cmndCount = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//      {
//        cmndCount.CommandText = "tActivityCount";
//        cmndCount.CommandType = CommandType.TableDirect;
//        cmndCount.IndexName = "PK_ActivityCountOnActivityRefno";
//        using (SQLiteDataReader drCount = cmndCount.ExecuteReader())
//        {
//          if (drCount.Seek(DbSeekOptions.FirstEqual, new object[] { actRefno }))
//          {
//            if (drCount.Read())
//            {
//              actCountFKRefno = Convert.ToInt32(drCount.GetValue((int)SysCommon.ActCountFields.FKRefno));
//            }
//            else
//            {
//#if MULTILANGUAGE
//              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrcountreadFailed));
//#else
//              throw new Exception("drCount.Read() failed.");
//#endif
//            }
//          }
//          else
//          {
//            throw new Exception(
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateThePhysicalCountRefnoInTactivitycount), actRefno));
//#else
//                string.Format("Unable to locate the physical count refno {0} in tActivityCount", actRefno));
//#endif
//          }
//        }
//      }

      return this.SendComm(this.Create(actCountFKRefno));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="reqData"></param>
    /// <remarks></remarks>
    protected override void Save(string reqData, int actRefno)
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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingPhysicalCountItems), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical count items.", false);
//#endif
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tActivityItemCount"].Rows.Count);

//            this.DataAccessStatusOutHandler(
//                this,
//                EventArgs.Empty,
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax),
//#else
//                string.Format("Records {0}", progMax),
//#endif
//                false);

//            using (SQLiteCommand cmndCountItem = conn.CreateCommand())
//            {
//              cmndCountItem.CommandText = "tActivityItemCount";
//              cmndCountItem.CommandType = CommandType.TableDirect;
//              cmndCountItem.IndexName = "UQ_ActivityItemCountOnActivityItemCountFKRefno";

//              using (SQLiteCommand cmndActItem = conn.CreateCommand())
//              {
//                cmndActItem.CommandText = "tActivityItem";
//                cmndActItem.CommandType = CommandType.TableDirect;

//                using (SqlCeResultSet rsActItem = cmndActItem.ExecuteResultSet(ResultSetOptions.Updatable))
//                {
//                  SqlCeUpdatableRecord recActItem = rsActItem.CreateRecord();

//                  using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
//                  {
//                    cmndProdUnit.CommandText = "tProductUnit";
//                    cmndProdUnit.CommandType = CommandType.TableDirect;
//                    cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

//                    using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
//                    {
//                      using (SQLiteCommand cmndProd = conn.CreateCommand())
//                      {
//                        cmndProd.CommandText = "tProduct";
//                        cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
//                        cmndProd.CommandType = CommandType.TableDirect;

//                        using (SQLiteDataReader drProd = cmndProd.ExecuteReader())
//                        {
//                          using (SQLiteCommand cmndUnit = conn.CreateCommand())
//                          {
//                            cmndUnit.CommandText = "tUnit";
//                            cmndUnit.IndexName = "UQ_UnitOnUnitCode";
//                            cmndUnit.CommandType = CommandType.TableDirect;

//                            using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
//                            {
//                              using (SQLiteCommand cmndLocMapSlot = conn.CreateCommand())
//                              {
//                                cmndLocMapSlot.CommandText = "tLocationMapSlot";
//                                cmndLocMapSlot.CommandType = CommandType.TableDirect;
//                                cmndLocMapSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";

//                                using (SQLiteDataReader drLocMapSlot = cmndLocMapSlot.ExecuteReader())
//                                {
//                                  using (SQLiteCommand cmndLocMapSect = conn.CreateCommand())
//                                  {
//                                    cmndLocMapSect.CommandText = "tLocationMapSection";
//                                    cmndLocMapSect.CommandType = CommandType.TableDirect;
//                                    cmndLocMapSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";

//                                    using (SQLiteDataReader drLocMapSect = cmndLocMapSect.ExecuteReader())
//                                    {
//                                      using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
//                                      {
//                                        cmndLocMapWhs.CommandText = "tLocationMapWarehouse";
//                                        cmndLocMapWhs.CommandType = CommandType.TableDirect;
//                                        cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

//                                        using (SQLiteDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
//                                        {
//                                          using (SQLiteCommand cmndID = conn.CreateCommand())
//                                          {
//                                            cmndID.CommandText = "SELECT @@IDENTITY";
//                                            cmndID.CommandType = CommandType.Text;

//                                            using (SqlCeResultSet rsCountItem =
//                                                cmndCountItem.ExecuteResultSet(ResultSetOptions.Updatable))
//                                            {
//                                              SqlCeUpdatableRecord recCountItem = rsCountItem.CreateRecord();
//                                              int actItemRefno = 0;
//                                              int progValue = 0;
//                                              int prodUnitRefno = 0;
//                                              int locMapRefno = 0;
//                                              int locMapWhsFKRefno = 0;
//                                              int locMapSectFKRefno = 0;
//                                              int locMapSlotFKRefno = 0;
//                                              int countItemFKRefno = 0;

//                                              this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

//                                              foreach (DataRow drow in ds.Tables["tActivityItemCount"].Rows)
//                                              {
//                                                errMsg = "Arguments:";

//                                                recActItem.SetInt32(
//                                                    (int)SysCommon.ActItemFields.SubType,
//                                                    (int)SysCommon.ActSubTypes.PhysicalCounts);
//                                                recActItem.SetInt32(
//                                                    (int)SysCommon.ActItemFields.Status, 0);
//                                                recActItem.SetInt32(
//                                                    (int)SysCommon.ActItemFields.ActRefno, actRefno);

//                                                rsActItem.Insert(recActItem);

//                                                actItemRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                                                prodUnitRefno = this.GetProductUnitRefnoFromDataRow(
//                                                    drow,
//                                                    ref errMsg,
//                                                    drProd,
//                                                    drUnit,
//                                                    drProdUnit,
//                                                    true);

//                                                // Cannot use this.SaveDetailLocation() f00 tags are not the same.
//                                                locMapRefno = 0;

//                                                if (locMapRefno == 0)
//                                                {
//                                                  locMapSlotFKRefno = Convert.ToInt32(drow["f08"]);
//                                                  if (drLocMapSlot.Seek(
//                                                      DbSeekOptions.FirstEqual,
//                                                      new object[] { locMapSlotFKRefno }))
//                                                  {
//                                                    if (drLocMapSlot.Read())
//                                                    {
//                                                      locMapRefno = Convert.ToInt32(
//                                                          drLocMapSlot.GetValue(
//                                                          (int)SysCommon.LocMapSlotFields.LocMapRefno));
//                                                    }
//                                                    else
//                                                    {
//                                                      throw new Exception("drLocMapSlot.Read() failed.");
//                                                    }
//                                                  }
//                                                }

//                                                if (locMapRefno == 0)
//                                                {
//                                                  locMapSectFKRefno = Convert.ToInt32(drow["f07"]);
//                                                  if (drLocMapSect.Seek(
//                                                      DbSeekOptions.FirstEqual, new object[] { locMapSectFKRefno }))
//                                                  {
//                                                    if (drLocMapSect.Read())
//                                                    {
//                                                      locMapRefno = Convert.ToInt32(
//                                                          drLocMapSect.GetValue(
//                                                          (int)SysCommon.LocMapSectFields.LocMapRefno));
//                                                    }
//                                                    else
//                                                    {
//                                                      throw new Exception("drLocMapSection.Read() failed.");
//                                                    }
//                                                  }
//                                                }

//                                                if (locMapRefno == 0)
//                                                {
//                                                  locMapWhsFKRefno = Convert.ToInt32(drow["f06"]);
//                                                  if (drLocMapWhs.Seek(
//                                                      DbSeekOptions.FirstEqual, new object[] { locMapWhsFKRefno }))
//                                                  {
//                                                    if (drLocMapWhs.Read())
//                                                    {
//                                                      locMapRefno = Convert.ToInt32(
//                                                          drLocMapWhs.GetValue(
//                                                          (int)SysCommon.LocMapWhsFields.LocMapRefno));
//                                                    }
//                                                    else
//                                                    {
//                                                      throw new Exception("drLocMapWhs.Read() failed.");
//                                                    }
//                                                  }
//                                                }

//                                                if (locMapRefno == 0)
//                                                {
//                                                  throw new Exception(
//#if MULTILANGUAGE
//                                                      string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToIndentifyTheLocationMapRefno)
//#else
//                                                      string.Format("Unable to indentify the location map refno.{0}"
//#endif
//                                                      + "Whs FK Refno {1}{2} Section FK Refno {3}{4} "
//                                                      + "Slot FK Refno {5}",
//                                                      Environment.NewLine,
//                                                      locMapWhsFKRefno,
//                                                      Environment.NewLine,
//                                                      locMapSectFKRefno,
//                                                      Environment.NewLine,
//                                                      locMapSlotFKRefno));
//                                                }

//                                                recCountItem.SetInt32(
//                                                    (int)SysCommon.ActItemCountFields.LocMapRefno,
//                                                    locMapRefno);

//                                                recCountItem.SetInt32(
//                                                    (int)SysCommon.ActItemCountFields.ActItemRefno,
//                                                    actItemRefno);
//                                                recCountItem.SetInt32(
//                                                    (int)SysCommon.ActItemCountFields.ActItemSubType,
//                                                    (int)SysCommon.ActSubTypes.PhysicalCounts);

//                                                countItemFKRefno = Convert.ToInt32(drow["f02"]);
//                                                errMsg += Environment.NewLine + "Item fk refno " + countItemFKRefno;
//                                                recCountItem.SetInt32(
//                                                    (int)SysCommon.ActItemCountFields.FKRefno,
//                                                    countItemFKRefno);
//                                                recCountItem.SetString(
//                                                    (int)SysCommon.ActItemCountFields.StringFKRefno,
//                                                    Convert.ToString(countItemFKRefno));

//                                                recCountItem.SetInt32(
//                                                    (int)SysCommon.ActItemCountFields.ProdUnitRefno,
//                                                    prodUnitRefno);
//                                                recCountItem.SetDecimal(
//                                                    (int)SysCommon.ActItemCountFields.QuantityOnHand,
//                                                    Convert.ToDecimal(drow["f09"]));
//                                                recCountItem.SetDecimal(
//                                                    (int)SysCommon.ActItemCountFields.BaseQuantityOnHand,
//                                                    Convert.ToDecimal(drow["f10"]));
//                                                recCountItem.SetDecimal(
//                                                    (int)SysCommon.ActItemCountFields.WeightQuantityOnHand,
//                                                    Convert.ToDecimal(drow["f11"]));
//                                                recCountItem.SetDecimal(
//                                                    (int)SysCommon.ActItemCountFields.Quantity,
//                                                    Convert.ToDecimal(drow["f12"]));
//                                                recCountItem.SetDecimal(
//                                                    (int)SysCommon.ActItemCountFields.BaseQuantity,
//                                                    Convert.ToDecimal(drow["f13"]));
//                                                recCountItem.SetDecimal(
//                                                    (int)SysCommon.ActItemCountFields.WeightQuantity,
//                                                    Convert.ToDecimal(drow["f14"]));

//                                                rsCountItem.Insert(recCountItem);

//                                                progValue++;
//                                                if (progValue % 10 == 0)
//                                                {
//                                                  this.DataAccessProgressOutHandler(
//                                                      this,
//                                                      EventArgs.Empty,
//                                                      progValue,
//                                                      0,
//                                                      progMax);
//                                                }
//                                              }

//                                              this.DataAccessProgressOutHandler(
//                                                  this,
//                                                  EventArgs.Empty,
//                                                  progMax,
//                                                  0,
//                                                  progMax);
//                                            }
//                                          }

//                                          drLocMapWhs.Close();
//                                        }
//                                      }

//                                      drLocMapSect.Close();
//                                    }
//                                  }

//                                  drLocMapSlot.Close();
//                                }
//                              }

//                              drUnit.Close();
//                            }
//                          }

//                          drProd.Close();
//                        }
//                      }

//                      drProdUnit.Close();
//                    }
//                  }
//                }
//              }
//            }
//          }

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.RequestPhysicalCountItemsComplete), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Request physical count items complete.", false);
//#endif
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.RequestPhysicalCountItemsFailed), true);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Request physical count items failed.", true);
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
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingPhysicalCountItemsFailed) + Environment.NewLine + errMsg,
//#else
//            "Database exception occurred. Saving physical count items failed." + Environment.NewLine + errMsg,
//#endif
//            ex);
//      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actCountFKRefno"></param>
    /// <returns></returns>
    protected override string Create(int actCountFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>"
          + "<command>" 
          + "<action>REQUEST PHYSICAL COUNT ITEMS</action>"
          + SysCommon.GetRequestDefaults(false)
          + ("<refno>" + actCountFKRefno.ToString() + "</refno>")
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
