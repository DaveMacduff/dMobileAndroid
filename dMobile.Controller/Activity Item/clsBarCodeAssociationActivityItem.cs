namespace dMobile.Controller
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Windows.Forms;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class BarCodeActItem : ActItem
  {
    #region Constructors

    public BarCodeActItem(Activity activity)
      : base(activity)
    {
      this.FullClassName = typeof(BarCodeActItem).FullName;
    }

    #endregion

    #region Public Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rowIdx"></param>
    /// <param name="quantity"></param>
    /// <param name="baseQuantity"></param>
    /// <param name="weightQuantity"></param>
    /// <returns></returns>
    public override int UpdateDataViewQuantity(
        int rowIdx, 
        decimal quantity, 
        decimal baseQuantity, 
        decimal weightQuantity)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemStringFKRefno"></param>
    /// <returns></returns>
    public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
    {
      return string.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemStringFKRefno"></param>
    /// <returns></returns>
    public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
    {
      return string.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetDataViewSortFKRefnoIdx()
    {
      return string.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="askToInsertAsNewItem"></param>
    public override void Insert(XDocument data, bool askToInsertAsNewItem)
    {
      int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);
      int prodUnitBarCodeRefno = 0;

      // Look up product bc unit
      string unitCode = string.Empty;
      string prodCode = string.Empty;
      bool prodUnitIsBC = false;
      string prodUnitBC = string.Empty;

      using (SQLiteCommand cmndProdUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndProdUnit.CommandType = CommandType.Text;
        cmndProdUnit.CommandText = "SELECT tProduct.ProductCode,"
            + "tProductUnit.ProductUnitIsBC," 
            + "tProductUnit.ProductUnitBC,"
            + "tUnit.UnitCode " 
            + "FROM tProductUnit "
            + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
            + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
            + "WHERE tProductUnit.ProductUnitRefno = " + prodUnitRefno;

        using (SqliteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
        {
          if (drProdUnit.Read())
          {
            unitCode = drProdUnit["UnitCode"].ToString();
            prodCode = drProdUnit["ProductCode"].ToString();
            prodUnitIsBC = (bool)drProdUnit["ProductUnitIsBC"];
            prodUnitBC = drProdUnit["ProductUnitBC"].ToString();
          }
          else
          {
            throw new Exception(
                string.Format("Unable to location the product unit refno {0} in tProductUnit.", prodUnitRefno));
          }
        }
      }

      // Check if the bar code is already associated to this product or another product
      bool isProdUnitBarCodeFound = false;
      string prodIdentifier = data.Root.Element("ProductIdentifier").Value;
      using (SQLiteCommand cmndProdUnitBarCode = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndProdUnitBarCode.CommandType = CommandType.Text;
        cmndProdUnitBarCode.CommandText = "SELECT tProductUnitBarCode.ProductUnitRefno," 
            + "tProductUnitBarCode.ProductUnitBarCodeRefno " 
            + "FROM tProductUnitBarCode "
            + "WHERE tProductUnitBarCode.ProductUnitBarCodeCode = \'" + prodIdentifier.PadRight(20) + "\' " 
            + "AND tProductUnitBarCode.ProductUnitBarCodeModifier1 = \'" 
            + data.Root.Element("ProductUnitBarCodeModifier1").Value + "\'";

        using (SqliteDataReader drProdUnitBarCode = cmndProdUnitBarCode.ExecuteReader())
        {
          if (drProdUnitBarCode.Read())
          {
            if (prodUnitRefno == (int)drProdUnitBarCode["ProductUnitRefno"])
            {
              // bar code is already associated to this product
              isProdUnitBarCodeFound = true;
            }
            else
            {
              // bar code is associated to a different product
              isProdUnitBarCodeFound = true;
              MessageBox.Show(
                  string.Format(
#if MULTILANGUAGE
                  Translator.GetInstance().Translate(TranslatorVocabulary.ScannedBarCodeIsAlreadyAssociatedToADifferentProduct), 
#else
                  "Scanned bar code is already associated to a different product {0} {1} {2}.", 
#endif
                  prodCode, 
                  prodUnitBC, 
                  unitCode), 
#if MULTILANGUAGE
                  Translator.GetInstance().Translate(TranslatorVocabulary.BarCodez), 
#else
                  "Bar Code", 
#endif
                  MessageBoxButtons.OK, 
                  MessageBoxIcon.Hand, 
                  MessageBoxDefaultButton.Button1);
            }
          }
        }
      }

      if (!isProdUnitBarCodeFound)
      {
        using (SqlCeTransaction barCodeTrx = DatabaseConnection.GetInstance().SQLConn.BeginTransaction(
            IsolationLevel.ReadCommitted))
        {
          try
          {
            this.ActHdr.Create(SysCommon.ActSubTypes.BarCodeAssociation);
            this.Create(SysCommon.ActSubTypes.BarCodeAssociation);

            // Add the new Bar Code to Product Unit Bar Codes
            using (SQLiteCommand cmndProdUnitBarCode = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
              cmndProdUnitBarCode.Transaction = barCodeTrx;
              cmndProdUnitBarCode.CommandType = CommandType.Text;
              cmndProdUnitBarCode.CommandText = "INSERT INTO tProductUnitBarCode (" 
                  + "ProductUnitRefno," 
                  + "ProductUnitBarCodeCode," 
                  + "ProductUnitBarCodeRaw," 
                  + "ProductUnitBarCodeSymbologyEnum," 
                  + "ProductUnitBarCodeModifier1," 
                  + "ProductUnitBarCodeStatus" +
                  ") VALUES (" 
                  + prodUnitRefno + "," 
                  + "\'" + data.Root.Element("ProductIdentifier").Value + "\'," 
                  + "\'" + data.Root.Element("RawData").Value + "\'," 
                  + data.Root.Element("enumBarCodeSymbology").Value + "," 
                  + "\'" + SysEnv.CurrentBarCodeModifier1 + "\'," 
                  + (int)SysCommon.ActStatus.Inserted + ")";

              cmndProdUnitBarCode.ExecuteNonQuery();

              using (SQLiteCommand cmndProdUnitBarCodeID = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
              {
                cmndProdUnitBarCodeID.Transaction = barCodeTrx;
                cmndProdUnitBarCodeID.CommandText = "SELECT @@IDENTITY FROM tProductUnitBarCode";
                cmndProdUnitBarCodeID.IndexName = "PK_ProductUnitBarCodeOnProductUnitBarCodeRefno";

                prodUnitBarCodeRefno = Convert.ToInt32(cmndProdUnitBarCodeID.ExecuteScalar());
              }
            }

            // Create Activity to Bar Code XRef
            using (SQLiteCommand cmndItemBarCode = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
              cmndItemBarCode.Transaction = barCodeTrx;
              cmndItemBarCode.CommandType = CommandType.Text;
              cmndItemBarCode.CommandText = "INSERT INTO tActivityItemBarCodeAssociationXRef ("
                  + "ActivityItemRefno," 
                  + "ProductUnitBarCodeRefno" 
                  + ") VALUES (" 
                  + this.Refno + "," 
                  + prodUnitBarCodeRefno + ")";

              cmndItemBarCode.ExecuteNonQuery();

              data = XDocument.Parse(
                  "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" 
                  + "<params>" 
                  + "<ProductUnitBarCodeRefno>" + prodUnitBarCodeRefno + "</ProductUnitBarCodeRefno>"
                  + "<ProductUnitRefno>" + prodUnitRefno + "</ProductUnitRefno>"
                  + "<ProductUnitBarCodeSymbologyPrefix>" + data.Root.Element("BarCodeSymbologyPrefix").Value 
                  + "</ProductUnitBarCodeSymbologyPrefix>" 
                  + "<ProductUnitBarCodeCode>" + data.Root.Element("ProductIdentifier").Value
                  + "</ProductUnitBarCodeCode>"
                  + "<ProductUnitIsBC>" + prodUnitIsBC + "</ProductUnitIsBC>" 
                  + "<ProductCode>" + prodCode + "</ProductCode>" 
                  + "<UnitCode>" + unitCode + "</UnitCode>" 
                  + "<ProductUnitBarCodeModifier1>" + this.SysEnv.CurrentBarCodeModifier1 
                  + "</ProductUnitBarCodeModifier1>" 
                  + "</params>");

              if (!this.SysEnv.SysSettings.IsDisconnectedMode)
              {
                string reqData = SysCommon.SubmitComm(this.CreateSubmitXML(data));
                if (reqData.Length > 0)
                {
                  // Check if result is an error message
                  if (reqData.Contains("<message>ERROR</message>"))
                  {
                    XmlDocument docError = new XmlDocument();
                    docError.LoadXml(reqData);
                    string errorDetail = docError.DocumentElement["detail"].InnerText;
                    if (errorDetail.Contains("MSGBOX-"))
                    {
                      throw new MessageOnlyException(errorDetail);
                    }
                    else
                    {
                      throw new Exception(errorDetail);
                    }
                  }
                }
              }
            }

            barCodeTrx.Commit();
          }
          catch (SqlCeException ex)
          {
            barCodeTrx.Rollback();
            throw ex;
          }
          catch (Exception ex)
          {
            barCodeTrx.Rollback();
            throw ex;
          }
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dataGridMain"></param>
    /// <remarks></remarks>
    public override void DesignDataGrid(DataGrid dataGridMain)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isSubmitBatch"></param>
    /// <returns></returns>
    public override DataView GetDataView(bool isSubmitBatch)
    {
      StringBuilder sql = new StringBuilder();

      if (isSubmitBatch)
      {
        sql.Append("SELECT tActivityItem.ActivityItemRefno,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProductUnit.ProductUnitRefno,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitIsBC,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("tProductUnitBarCode.ProductUnitBarCodeRefno,");
        sql.Append("tProductUnitBarCode.ProductUnitBarCodeCode,");
        sql.Append("tProductUnitBarCode.ProductUnitBarCodeModifier1,");
        sql.Append("tProductUnitBarCode.ProductUnitBarCodeSymbologyEnum,");
        sql.Append("tActivityItem.ActivityItemStatus AS ActivityStatus ");
        sql.Append("FROM tActivityItem ");
        sql.Append("INNER JOIN tActivityItemBarCodeAssociationXRef ON ");
        sql.Append("tActivityItemBarCodeAssociationXRef.ActivityItemRefno = tActivityItem.ActivityItemRefno ");
        sql.Append("INNER JOIN tProductUnitBarCode ON tProductUnitBarCode.ProductUnitBarCodeRefno ");
        sql.Append("= tActivityItemBarCodeAssociationXRef.ProductUnitBarCodeRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tProductUnitBarCode.ProductUnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
        sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
        sql.Append("ORDER BY tActivityItem.ActivityItemRefno");
      }
      else
      {
        sql.Append("SELECT tActivityItem.ActivityItemRefno,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProductUnit.ProductUnitRefno,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitIsBC,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("tProductUnitBarCode.ProductUnitBarCodeRefno,");
        sql.Append("tProductUnitBarCode.ProductUnitBarCodeCode,");
        sql.Append("tProductUnitBarCode.ProductUnitBarCodeModifier1,");
        sql.Append("tProductUnitBarCode.ProductUnitBarCodeSymbologyEnum,");
        sql.Append("tActivityItem.ActivityItemStatus AS ActivityStatus ");
        sql.Append("FROM tActivityItem ");
        sql.Append("INNER JOIN tActivityItemBarCodeAssociationXRef ");
        sql.Append("ON tActivityItemBarCodeAssociationXRef.ActivityItemRefno = tActivityItem.ActivityItemRefno ");
        sql.Append("INNER JOIN tProductUnitBarCode ON tProductUnitBarCode.ProductUnitBarCodeRefno ");
        sql.Append("= tActivityItemBarCodeAssociationXRef.ProductUnitBarCodeRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tProductUnitBarCode.ProductUnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
        sql.Append("ORDER BY tActivityItem.ActivityItemRefno");
      }

      using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
      {
        using (DataSet ds = new DataSet())
        {
          da.Fill(ds);
          this.DataViewItems = ds.Tables[0].DefaultView;
          this.DataViewItems.Table.TableName = this.MainGridMappingName;
          this.DataViewItems.AllowNew = true;
          this.DataViewItems.AllowEdit = true;
          this.DataViewItems.AllowDelete = true;
          this.DataViewItems.Sort = "ActivityItemRefno";
        }
      }

      return this.DataViewItems;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="searchText"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public override DataView Search(string searchText)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="prodUnitRefno"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public override int GetRefnoFromProdUnitRefno(int prodUnitRefno)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    public override void SelectItem(int idx)
    {
      ////base.SelectItem(idx);
      this.CurrRowIdx = idx;
      this.Refno = (int)DataViewItems[idx]["ActivityItemRefno"];
      this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno((int)DataViewItems[idx]["ProductUnitRefno"]);
      this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
      this.FKRefno = (int)DataViewItems[idx]["ActivityItemRefno"];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public override void Refresh(XDocument data)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="idx"></param>
    public override void Submit(XDocument data, int idx)
    {
      XmlDocument xmlDoc = new XmlDocument();
      string send = this.CreateSubmitXML(data);
      string receivedData = SysCommon.SubmitComm(send);
      if (receivedData.Length > 0)
      {
        xmlDoc.LoadXml(receivedData);

        // check if result is an error message
        if (receivedData.Contains("<message>ERROR</message>"))
        {
          string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;
          if (errorDetail.Contains("MSGBOX-"))
          {
            throw new MessageOnlyException(errorDetail);
          }
          else
          {
            throw new Exception(errorDetail);
          }
        }
        else
        {
          using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndItem.CommandType = CommandType.Text;
            cmndItem.CommandText = "UPDATE tActivityItem " 
                + "SET tActivityItem.ActivityItemStatus = " + (int)SysCommon.ActStatus.NoChange 
                + " WHERE tActivityItem.ActivityItemRefno = " + this.Refno;
            cmndItem.ExecuteNonQuery();
          }

          if (this.DataViewItems != null && idx >= 0)
          {
            this.DataViewItems[idx].BeginEdit();
            this.DataViewItems[idx]["ActivityStatus"] = SysCommon.ActStatus.NoChange;
            this.DataViewItems[idx].EndEdit();
          }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public override string CreateSubmitXML(XDocument data)
    {
      StringBuilder strXML = new StringBuilder();
      XmlWriter writer = XmlWriter.Create(strXML);
      string prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
      string prodUnitBarCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
      bool prodUnitIsBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
      string unitCode = data.Root.Element("UnitCode").Value;
      string barCodeSymbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;
      string barCodeModifier1 = data.Root.Element("ProductUnitBarCodeModifier1").Value;

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>" 
          + "<action>SUBMIT BARCODE</action>" 
          + "</command>" 
          + SysCommon.GetRequestDefaults(false) 
          + ("<prod_id>" + prodCode + "</prod_id>") 
          + ("<is_bc>" + prodUnitIsBC.ToString() + "</is_bc>") 
          + ("<symbology>" + barCodeSymbologyPrefix + "</symbology>") 
          + ("<alt_unit>" + unitCode + "</alt_unit>") 
          + ("<gtin>" + prodUnitBarCodeCode + "</gtin>") 
          + ("<modifier1>" + barCodeModifier1 + "</modifier1>") 
          + "</imclient>");

      request.Save(writer);
      writer.Flush();
      writer.Close();

      return strXML.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="quantity"></param>
    /// <param name="baseQuantity"></param>
    /// <param name="weightQuantity"></param>
    public override void RefreshQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="actItemRefno"></param>
    public override void UpdateQuantityFromDataView(int idx, int actItemRefno)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
