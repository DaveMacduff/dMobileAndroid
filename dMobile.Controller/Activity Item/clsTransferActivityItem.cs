namespace dMobile.Controller
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Windows.Forms;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using dMobile.GroundWork;

  public class TransferActItem : ActItem
  {
    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="activity"></param>
    public TransferActItem(Activity activity)
      : base(activity)
    {
      this.FullClassName = typeof(TransferActItem).FullName;
      this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProd2Trans;
      this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsTransCreateNewItemNoAsk;
    }

    #endregion

    #region Public Members

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool IsCatchAsk()
    {
      bool isCatchAsk = base.IsCatchAsk();
      if (isCatchAsk)
      {
          // modules are for exclusions
          isCatchAsk = this.SysEnv.SysSettings.IsTransCatchAskEnabled;
      }

      return isCatchAsk;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool IsCatchAskZero()
    {
      bool isCatchAskZero = base.IsCatchAskZero();
      if (isCatchAskZero)
      {
        isCatchAskZero = !this.SysEnv.SysSettings.IsTransferNoCatchAskZeroEnabled;
      }

      return isCatchAskZero;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public override void IsReadyToApply(XDocument data)
    {
      base.IsReadyToApply(data);

      // New items are identified with 0 < this.FKRefno, do not validate.
      if (this.SysEnv.SysSettings.TransQtyOverWarn
          != SysCommon.ActTransfersQtyOverValLevels.NoValidationOrWarning
          && 0 < this.FKRefno)
      {
        try
        {
          this.TransferModuleQuantityOverCheck(
              int.Parse(data.Root.Element("itm_refno").Value),
              decimal.Parse(data.Root.Element("alt_qty").Value));
        }
        catch (Exception ex)
        {
          switch (this.SysEnv.SysSettings.TransQtyOverWarn)
          {
            case SysCommon.ActTransfersQtyOverValLevels.ValidateWarnAndContinue:
              if (MessageBox.Show(
#if MULTILANGUAGE
                  ex.Message + Environment.NewLine + Translator.GetInstance().Translate(TranslatorVocabulary.Continue),
                  Translator.GetInstance().Translate(TranslatorVocabulary.QuantityOver),
#else
                  ex.Message + Environment.NewLine + "Continue?",
                  "Quantity Over",
#endif
                  MessageBoxButtons.YesNo,
                  MessageBoxIcon.Question,
                  MessageBoxDefaultButton.Button2) == DialogResult.No)
              {
                throw new IsReadyToApplyException(ex.Message, true, false);
              }
              else
              {
                ////throw new IsReadyToApplyException(ex.Message, true, true);
              }

              break;

            case SysCommon.ActTransfersQtyOverValLevels.ValidateWarnAndStop:
              throw new IsReadyToApplyException(ex.Message, false, false);
          }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemStringFKRefno"></param>
    /// <returns></returns>
    public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
    {
#if MULTILANGUAGE
      return string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToParseTheTransferItem), actItemStringFKRefno);
#else
      return string.Format("Unable to parse the transfer item # {0}", actItemStringFKRefno);
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemStringFKRefno"></param>
    /// <returns></returns>
    public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
    {
#if MULTILANGUAGE
      return string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToFindTheTransferItem), actItemStringFKRefno);
#else
      return string.Format("Unable to find the transfer item # {0}", actItemStringFKRefno);
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetDataViewSortFKRefnoIdx()
    {
      return "ActivityItemTransferStringFKRefno";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="askToInsertAsNewItem"></param>
    public override void Insert(XDocument data, bool askToInsertAsNewItem)
    {
      int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);

      using (SQLiteCommand cmndActTransferItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndActTransferItem.CommandText = "tActivityItemTransfer";
        cmndActTransferItem.CommandType = CommandType.TableDirect;
        cmndActTransferItem.IndexName = "UQ_ActivityItemTransferOnActivityItemTransferFKRefno";

        using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndItem.CommandText = "tActivityItem";
          cmndItem.CommandType = CommandType.TableDirect;

          using (SQLiteCommand cmndID = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndID.CommandText = "SELECT @@IDENTITY";
            cmndID.CommandType = CommandType.Text;

            using (SqlCeResultSet rs = cmndActTransferItem.ExecuteResultSet(ResultSetOptions.Updatable))
            {
              SqlCeUpdatableRecord rec = rs.CreateRecord();

              using (SqlCeResultSet rsActItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
              {
                SqlCeUpdatableRecord recActItem = rsActItem.CreateRecord();

                recActItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.Transfers);
                recActItem.SetInt32((int)SysCommon.ActItemFields.Status, (int)SysCommon.ActStatus.NoChange);
                recActItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, this.ActHdr.Refno);

                rsActItem.Insert(recActItem);

                this.Refno = Convert.ToInt32(cmndID.ExecuteScalar());

                this.FKRefno = this.Refno * -1;
                rec.SetInt32((int)SysCommon.ActItemTransferFields.ActItemRefno, this.Refno);
                rec.SetInt32((int)SysCommon.ActItemTransferFields.ActItemSubType, (int)SysCommon.ActSubTypes.Transfers);
                rec.SetInt32((int)SysCommon.ActItemTransferFields.FKRefno, this.FKRefno);
                rec.SetString((int)SysCommon.ActItemTransferFields.StringFKRefno, this.FKRefno.ToString());
                rec.SetInt32((int)SysCommon.ActItemTransferFields.ProductUnitRefno, prodUnitRefno);
                rec.SetInt32(
                    (int)SysCommon.ActItemTransferFields.TransferFromWhsLocMapRefno, 
                    this.LocMap.CurrLocMap.WhsLocMapRefno);
                rec.SetDecimal((int)SysCommon.ActItemTransferFields.RequiredQuantity, 0);
                rec.SetDecimal((int)SysCommon.ActItemTransferFields.Quantity, 0);
                rec.SetDecimal((int)SysCommon.ActItemTransferFields.BaseQuantity, 0);
                rec.SetDecimal((int)SysCommon.ActItemTransferFields.WeightQuantity, 0);
                
                rs.Insert(rec);

                this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                if (this.DataViewItems != null)
                {
                  DataRowView rowView = this.DataViewItems.AddNew();
                  rowView["ActivityItemRefno"] = this.Refno;
                  rowView["ActivityItemTransferFKRefno"] = this.FKRefno;
                  rowView["ActivityItemTransferStringFKRefno"] = this.FKRefno.ToString();
                  rowView["ProductUnitRefno"] = prodUnitRefno;
                  rowView["ProductRefno"] = int.Parse(data.Root.Element("ProductRefno").Value);
                  rowView["ProductCode"] = data.Root.Element("ProductCode").Value;
                  rowView["ProductDescription"] = data.Root.Element("ProductDescription").Value;
                  rowView["UnitCode"] = data.Root.Element("UnitCode").Value;
                  rowView["ProductUnitBC"] = data.Root.Element("ProductUnitBC").Value;
                  rowView["ProductUnitSizeDescription"] = data.Root.Element("ProductUnitSizeDescription").Value;
                  rowView["LocationMapRefno"] = this.LocMap.CurrLocMap.WhsLocMapRefno;
                  rowView["LocationCode"] = this.LocMap.CurrLocMap.WhsCode;
                  rowView["LocationName"] = this.LocMap.CurrLocMap.WhsName;
                  rowView["ActivityItemStatus"] = SysCommon.ActStatus.Inserted;
                  rowView["ActivityItemTransferRequiredQuantity"] = 0;
                  rowView["ActivityItemTransferQuantity"] = 0;
                  rowView["ActivityItemTransferBaseQuantity"] = 0;
                  rowView["ActivityItemTransferWeightQuantity"] = 0;
                  rowView.EndEdit();
                }
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public override void Refresh(XDocument data)
    {
      if (this.SysEnv.SysSettings.IsDisconnectedMode)
      {
        this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
      }
      else
      {
        this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

        TableActivityTransferItem tableActTransferItem = new TableActivityTransferItem();
        TableActivityTransferDetail tableActTransferDetail = new TableActivityTransferDetail();
        tableActTransferDetail.Purge();
        tableActTransferItem.Purge();

        this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

        tableActTransferItem.RegisterDataAccessListenerProgressOut((TransferActItem)this);
        tableActTransferItem.Request(this.ActHdr.Refno);
        tableActTransferItem.UnregisterDataAccessListenerProgressOut((TransferActItem)this);
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dataGridMain"></param>
    /// <remarks></remarks>
    public override void DesignDataGrid(DataGrid dataGridMain)
    {
      int colWidthFactor = 1;

      if (this.SysEnv.IsWindowsMobile6Device)
      {
        colWidthFactor = 2;
      }

      if (dataGridMain.TableStyles.Count > 0)
      {
        dataGridMain.TableStyles.Clear();
      }

      DataGridTableStyle dataGridStyle = new DataGridTableStyle();
      dataGridStyle.MappingName = this.MainGridMappingName;

      DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ActivityItemRefno";
      dataGridCol.HeaderText = string.Empty;
      dataGridCol.Width = -1;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ProductCode";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.ProductId);
#else
      dataGridCol.HeaderText = "Product ID";
#endif
      dataGridCol.Width = 65 * colWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ProductUnitBC";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Bc);
#else
      dataGridCol.HeaderText = "BC";
#endif
      dataGridCol.Width = 20 * colWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "UnitCode";
      dataGridCol.HeaderText = string.Empty;
      dataGridCol.Width = 22 * colWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ActivityItemTransferRequiredQuantity";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Required);
#else
      dataGridCol.HeaderText = "Required";
#endif
      dataGridCol.Width = 55 * colWidthFactor;
      dataGridCol.Format = "0.000";
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ActivityItemTransferQuantity";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Qtyzzz);
#else
      dataGridCol.HeaderText = "Qty";
#endif
      dataGridCol.Width = 55 * colWidthFactor;
      dataGridCol.Format = "0.000";
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "LocationCode";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Whs);
#else
      dataGridCol.HeaderText = "Whs";
#endif
      dataGridCol.Width = 58 * colWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridMain.TableStyles.Add(dataGridStyle);
      dataGridMain.RowHeadersVisible = false;
      dataGridMain.ColumnHeadersVisible = true;
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
        sql.Append("SELECT tActivityItemTransfer.ActivityItemRefno,");
        sql.Append("tActivityItemTransfer.ActivityItemTransferFKRefno,");
        sql.Append("tActivityItemTransfer.ActivityItemTransferStringFKRefno,");
        sql.Append("tProductUnit.ProductUnitRefno,");
        sql.Append("tProduct.ProductRefno,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProduct.ProductDescription,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitSizeDescription,");
        sql.Append("tActivityItemTransfer.ActivityItemTransferRequiredQuantity,");
        sql.Append("tActivityItemTransfer.ActivityItemTransferQuantity,");
        sql.Append("tActivityItemTransfer.ActivityItemTransferBaseQuantity,");
        sql.Append("tActivityItemTransfer.ActivityItemTransferWeightQuantity,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
        sql.Append("tActivityitem.ActivityItemStatus ");
        sql.Append("FROM tActivityItemTransfer ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
        sql.Append("= tActivityItemTransfer.ActivityItemRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
        sql.Append("= tActivityItemTransfer.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityItemTransfer.TransferFromWarehouseLocationMapRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
        sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
        sql.Append("UNION ");
        sql.Append("SELECT tActivityItemTransfer.ActivityItemRefno,");
        sql.Append("tActivityItemTransfer.ActivityItemTransferFKRefno,");
        sql.Append("tActivityItemTransfer.ActivityItemTransferStringFKRefno,");
        sql.Append("tProductUnit.ProductUnitRefno,");
        sql.Append("tProduct.ProductRefno,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProduct.ProductDescription,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitSizeDescription,");
        sql.Append("tActivityItemTransfer.ActivityItemTransferRequiredQuantity,");
        sql.Append("tActivityItemTransfer.ActivityItemTransferQuantity,");
        sql.Append("tActivityItemTransfer.ActivityItemTransferBaseQuantity,");
        sql.Append("tActivityItemTransfer.ActivityItemTransferWeightQuantity,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
        sql.Append("tActivityitem.ActivityItemStatus ");
        sql.Append("FROM tActivityItemTransfer ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
        sql.Append("= tActivityItemTransfer.ActivityItemRefno");
        sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
        sql.Append("= tActivityItemTransfer.ActivityItemRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
        sql.Append("= tActivityItemTransfer.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityItemTransfer.TransferFromWarehouseLocationMapRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
        sql.AppendFormat(" AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
        sql.Append(" ORDER BY tActivityItemTransfer.ActivityItemRefno");
      }
      else
      {
        sql.Append("SELECT tActivityItemTransfer.ActivityItemRefno, ");
        sql.Append("tActivityItemTransfer.ActivityItemTransferFKRefno, ");
        sql.Append("tActivityItemTransfer.ActivityItemTransferStringFKRefno, ");
        sql.Append("tProductUnit.ProductUnitRefno, ");
        sql.Append("tProduct.ProductRefno,");
        sql.Append("tProduct.ProductCode, ");
        sql.Append("tProduct.ProductDescription, ");
        sql.Append("tUnit.UnitCode, ");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitSizeDescription, ");
        sql.Append("tActivityItemTransfer.ActivityItemTransferRequiredQuantity, ");
        sql.Append("tActivityItemTransfer.ActivityItemTransferQuantity, ");
        sql.Append("tActivityItemTransfer.ActivityItemTransferBaseQuantity, ");
        sql.Append("tActivityItemTransfer.ActivityItemTransferWeightQuantity, ");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
        sql.Append("tActivityitem.ActivityItemStatus ");
        sql.Append("FROM tActivityItemTransfer ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
        sql.Append("= tActivityItemTransfer.ActivityItemRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
        sql.Append("= tActivityItemTransfer.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityItemTransfer.TransferFromWarehouseLocationMapRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
        sql.Append("ORDER BY tActivityItemTransfer.ActivityItemRefno");
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
    public override DataView Search(string searchText)
    {
      if (string.IsNullOrEmpty(this.ActItemFilter))
      {
        this.DataViewItems.RowFilter = "ProductCode LIKE \'" + searchText.Trim() + "%\'";
      }
      else
      {
        this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'" + searchText.Trim() + "%\'";
      }

      return this.DataViewItems;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    public override void SelectItem(int idx)
    {
      base.SelectItem(idx);
      this.FKRefno = (int)this.DataViewItems[idx]["ActivityItemTransferFKRefno"];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prodUnitRefno"></param>
    /// <returns></returns>
    public override int GetRefnoFromProdUnitRefno(int prodUnitRefno)
    {
      int foundItemRefno = 0;

      using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndItem.CommandType = CommandType.Text;
        cmndItem.CommandText = "SELECT tActivityItem.ActivityItemRefno,"
            + "tActivityItemTransfer.ActivityItemTransferFKRefno," 
            + "tActivityItemTransfer.ProductUnitRefno," 
            + "tLocationMapWarehouse.LocationMapRefno " 
            + "FROM tActivityItemTransfer " 
            + "INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = tActivityItemTransfer.ActivityItemRefno " 
            + "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno "
            + "= tActivityItemTransfer.TransferFromWarehouseLocationMapRefno " 
            + "WHERE tActivityItem.ActivityRefno = " + this.ActHdr.Refno
            + " AND tActivityItemTransfer.TransferFromWarehouseLocationMapRefno = " 
            + this.LocMap.CurrLocMap.WhsLocMapRefno
            + " AND tActivityItemTransfer.ProductUnitRefno = " + prodUnitRefno;

        using (SqliteDataReader drItem = cmndItem.ExecuteReader())
        {
          if (drItem.Read())
          {
            ////this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno((int)drItem["LocationMapRefno"]);
            this.Refno = (int)drItem["ActivityItemRefno"];
            this.FKRefno = (int)drItem["ActivityItemTransferFKRefno"];
            this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno((int)drItem["ProductUnitRefno"]);
            this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

            foundItemRefno = this.Refno;
          }

          drItem.Close();
        }
      }

      return foundItemRefno;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public override string CreateSubmitXML(XDocument data)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>" 
          + "<action>SUBMIT TRANSFER ITEM</action>" 
          + "</command>" 
          + SysCommon.GetRequestDefaults(false)
          + ("<prod_id>" + data.Root.Element("ProductCode").Value.PadRight(10) + "</prod_id>")
          + ("<alt_unit>" + data.Root.Element("UnitCode").Value + "</alt_unit>")
          + ("<is_bc>" + bool.Parse(data.Root.Element("ProductUnitIsBC").Value) + "</is_bc>")
          + ("<symbology>" + data.Root.Element("ProductUnitBarCodeSymbologyPrefix") + "</symbology>")
          + ("<gtin>" + data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20) + "</gtin>") 
          + "<whs_id></whs_id>" 
          + "<sect_refno></sect_refno>" 
          + "<slot_refno></slot_refno>" 
          + "<refno></refno>" 
          + "</imclient>");

      request.Save(xml);
      xml.Flush();
      xml.Close();

      return xmlString.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedIndex"></param>
    /// <param name="quantity"></param>
    /// <param name="baseQuantity"></param>
    /// <param name="weightQuantity"></param>
    public override void RefreshQuantity(
        int selectedIndex, 
        decimal quantity, 
        decimal baseQuantity, 
        decimal weightQuantity)
    {
      this.DataViewItems[selectedIndex].BeginEdit();
      this.DataViewItems[selectedIndex]["ActivityItemTransferQuantity"] = quantity;
      this.DataViewItems[selectedIndex]["ActivityItemTransferBaseQuantity"] = baseQuantity;
      this.DataViewItems[selectedIndex]["ActivityItemTransferWeightQuantity"] = weightQuantity;
      this.DataViewItems[selectedIndex].EndEdit();
    }

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
      this.DataViewItems[rowIdx].BeginEdit();
      this.DataViewItems[rowIdx]["ActivityItemTransferQuantity"] 
          = (decimal)DataViewItems[rowIdx]["ActivityItemTransferQuantity"] + quantity;
      this.DataViewItems[rowIdx]["ActivityItemTransferBaseQuantity"] 
          = (decimal)DataViewItems[rowIdx]["ActivityItemTransferBaseQuantity"] + baseQuantity;
      this.DataViewItems[rowIdx]["ActivityItemTransferWeightQuantity"] 
          = (decimal)DataViewItems[rowIdx]["ActivityItemTransferWeightQuantity"] + weightQuantity;
      this.DataViewItems[rowIdx].EndEdit();

      return rowIdx;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedIndex"></param>
    /// <param name="actItemRefno"></param>
    public override void UpdateQuantityFromDataView(int idx, int actItemRefno)
    {
      using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndItem.CommandText = "tActivityItemTransfer";
        cmndItem.CommandType = CommandType.Text;
        cmndItem.IndexName = string.Empty;
        cmndItem.CommandText = "UPDATE tActivityItemTransfer "
            + "SET ActivityItemTransferQuantity = " 
            + this.DataViewItems[idx]["ActivityItemTransferQuantity"] + "," 
            + "ActivityItemTransferBaseQuantity = " 
            + this.DataViewItems[idx]["ActivityItemTransferBaseQuantity"] + "," 
            + "ActivityItemTransferWeightQuantity = " 
            + this.DataViewItems[idx]["ActivityItemTransferWeightQuantity"] +
            " WHERE tActivityItemTransfer.ActivityItemRefno = " + actItemRefno;
        cmndItem.ExecuteNonQuery();
      }
    }

    #endregion

    #region Private Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemTransFKRefno"></param>
    /// <param name="actItemTransQuantity"></param>
    private void TransferModuleQuantityOverCheck(int actItemTransFKRefno, decimal actItemTransQuantity)
    {
      if (this.CurrRowIdx >= 0 && this.CurrRowIdx < this.DataViewItems.Count)
      {
        decimal transferred = (decimal)this.DataViewItems[this.CurrRowIdx]["ActivityItemTransferQuantity"];
        decimal required = (decimal)this.DataViewItems[this.CurrRowIdx]["ActivityItemTransferRequiredQuantity"];
        if (transferred + actItemTransQuantity > required)
        {
          throw new Exception(
              string.Format(
#if MULTILANGUAGE
              Translator.GetInstance().Translate(TranslatorVocabulary.TheResultingQuantityTransferredWillBeGreaterThanTheQuantityRequired),
#else
              "The resulting quantity transferred {0} {1} will be greater than the quantity required {2} {3}.",
#endif
              transferred + actItemTransQuantity,
              this.ProdInfo.UnitCode,
              required,
              this.ProdInfo.UnitCode));
        }
      }
    }

    #endregion
  }
}
