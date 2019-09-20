namespace dMobile.Controller
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Windows.Forms;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using dMobile.GroundWork;

  public class AdjActItem : ActItem
  {
    #region Constructors

    /// <summary>
    ///
    /// </summary>
    /// <param name="activity"></param>
    /// <remarks></remarks>
    public AdjActItem(Activity activity)
      : base(activity)
    {
      this.FullClassName = typeof(AdjActItem).FullName;
      this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProduct2Adjustments;
      this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsAdjustmentCreateNewItemNoAsk;
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
          isCatchAsk = this.SysEnv.SysSettings.IsAdjustmentCatchAskEnabled;
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
        isCatchAskZero = !this.SysEnv.SysSettings.IsAdjustmentNoCatchAskZeroEnabled;
      }

      return isCatchAskZero;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemStringFKRefno"></param>
    /// <returns></returns>
    public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
    {
#if MULTILANGUAGE
      return string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToParseAdjustmentItem), actItemStringFKRefno);
#else
      return string.Format("Unable to parse adjustment item # {0}", actItemStringFKRefno);
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
      return string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToFindAdjustmentItem), actItemStringFKRefno);
#else
      return string.Format("Unable to find adjustment item # {0}", actItemStringFKRefno);
#endif
    }

    /// <summary>
    ///
    /// </summary>
    public override string GetDataViewSortFKRefnoIdx()
    {
      return "ActivityItemAdjustmentStringFKRefno";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="askToInsertAsNewItem"></param>
    public override void Insert(XDocument data, bool askToInsertAsNewItem)
    {
//      int foundActItemRefno = 0;
//      int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);
//      DialogResult result = DialogResult.None;

//      if (prodUnitRefno != 0)
//      {
//        foundActItemRefno = this.GetRefnoFromProdUnitRefno(prodUnitRefno);

//        if (foundActItemRefno > 0 && askToInsertAsNewItem)
//        {
//          result = MessageBox.Show(
//#if MULTILANGUAGE
//              Translator.GetInstance().Translate(TranslatorVocabulary.ALineItemAlreadyExistsForThisLocationAndProduct) + Environment.NewLine +
//              Translator.GetInstance().Translate(TranslatorVocabulary.DoYouWishToCreateOne), 
//              Translator.GetInstance().Translate(TranslatorVocabulary.NewItem), 
//#else
//              "A line item already exists for this location and product." + Environment.NewLine +
//              "Do you wish to create one?", 
//              "New Item", 
//#endif
//              MessageBoxButtons.YesNoCancel, 
//              MessageBoxIcon.Question,
//              MessageBoxDefaultButton.Button3);
//        }
//        else
//        {
//          // If the selected product to insert was not found, force the dialog result to insert it.
//          result = DialogResult.Yes;
//        }

//        if (result == DialogResult.Yes)
//        {
//          using (SQLiteCommand cmndActAdjItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//          {
//            cmndActAdjItem.CommandText = "tActivityItemAdjustment";
//            cmndActAdjItem.CommandType = CommandType.TableDirect;
//            cmndActAdjItem.IndexName = "UQ_ActivityItemAdjustmentOnActivityItemAdjustmentFKRefno";

//            using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//            {
//              cmndItem.CommandText = "tActivityItem";
//              cmndItem.CommandType = CommandType.TableDirect;

//              using (SQLiteCommand cmndID = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//              {
//                cmndID.CommandText = "SELECT @@IDENTITY";
//                cmndID.CommandType = CommandType.Text;

//                using (SqlCeResultSet rs = cmndActAdjItem.ExecuteResultSet(ResultSetOptions.Updatable))
//                {
//                  SqlCeUpdatableRecord rec = rs.CreateRecord();

//                  using (SqlCeResultSet rsActItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
//                  {
//                    SqlCeUpdatableRecord recActItem = rsActItem.CreateRecord();

//                    recActItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.Adjustment);
//                    recActItem.SetInt32((int)SysCommon.ActItemFields.Status, (int)SysCommon.ActStatus.NoChange);
//                    recActItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, this.ActHdr.Refno);

//                    rsActItem.Insert(recActItem);

//                    this.Refno = Convert.ToInt32(cmndID.ExecuteScalar());
//                    this.FKRefno = this.Refno * -1;

//                    rec.SetInt32((int)SysCommon.ActItemAdjFields.ActItemRefno, this.Refno);
//                    rec.SetInt32((int)SysCommon.ActItemAdjFields.ActItemSubType, (int)SysCommon.ActSubTypes.Adjustment);
//                    rec.SetInt32((int)SysCommon.ActItemAdjFields.FKRefno, this.FKRefno);
//                    rec.SetString((int)SysCommon.ActItemAdjFields.StringFKRefno, this.FKRefno.ToString());
//                    rec.SetInt32((int)SysCommon.ActItemAdjFields.ProdUnitRefno, prodUnitRefno);

//                    rec.SetInt32(
//                        (int)SysCommon.ActItemAdjFields.WhsLocMapRefno, 
//                        this.LocMap.CurrLocMap.WhsLocMapRefno);

//                    rec.SetDecimal((int)SysCommon.ActItemAdjFields.Quantity, 0);
//                    rec.SetDecimal((int)SysCommon.ActItemAdjFields.BaseQuantity, 0);
//                    rec.SetDecimal((int)SysCommon.ActItemAdjFields.WeightQuantity, 0);
//                    rec.SetString(
//                        (int)SysCommon.ActItemAdjFields.Description, 
//                        this.ProdInfo.Desc.Trim() + " " + this.ProdInfo.UnitSizeDesc);

//                    rs.Insert(rec);

//                    this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
//                    this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

//                    if (this.DataViewItems != null)
//                    {
//                      DataRowView rowView = this.DataViewItems.AddNew();
//                      rowView["ActivityItemRefno"] = this.Refno;
//                      rowView["ActivityItemAdjustmentFKRefno"] = this.FKRefno;
//                      rowView["ActivityItemAdjustmentStringFKRefno"] = this.FKRefno.ToString();
//                      rowView["ProductUnitRefno"] = prodUnitRefno;
//                      rowView["ProductRefno"] = this.ProdInfo.Refno;
//                      rowView["ProductCode"] = this.ProdInfo.Code;
//                      rowView["ProductDescription"] = this.ProdInfo.Desc;
//                      rowView["UnitCode"] = this.ProdInfo.UnitCode;
//                      rowView["ProductUnitBC"] = this.ProdInfo.UnitBC;
//                      rowView["ProductUnitSizeDescription"] = this.ProdInfo.UnitSizeDesc;
//                      rowView["LocationMapRefno"] = this.LocMap.CurrLocMap.WhsLocMapRefno;
//                      rowView["LocationCode"] = this.LocMap.CurrLocMap.WhsCode;
//                      rowView["LocationName"] = this.LocMap.CurrLocMap.WhsName;
//                      rowView["ActivityItemStatus"] = SysCommon.ActStatus.Inserted;
//                      rowView["ActivityItemAdjustmentQuantity"] = 0;
//                      rowView["ActivityItemAdjustmentBaseQuantity"] = 0;
//                      rowView["ActivityItemAdjustmentWeightQuantity"] = 0;
//                      rowView.EndEdit();
//                    }
//                  }
//                }
//              }
//            }
//          }
//        }
//        else if (result == DialogResult.No)
//        {
//        }
//        else if (result == DialogResult.Cancel)
//        {
//        }
//      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public override void Refresh(XDocument data)
    {
      if (SysEnv.SysSettings.IsDisconnectedMode)
      {
        this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
      }
      else
      {
        this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

        TableActivityAdjustmentItem adjItem = new TableActivityAdjustmentItem();
        TableActivityAdjustmentDetail adjDetail = new TableActivityAdjustmentDetail();
        adjDetail.Purge();
        adjItem.Purge();

        this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

        adjItem.RegisterDataAccessListenerProgressOut(this);
        adjItem.Request(ActHdr.Refno);
        adjItem.UnregisterDataAccessListenerProgressOut(this);
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dataGridMain"></param>
    /// <remarks></remarks>
    public override void DesignDataGrid(DataGrid dataGridMain)
    {
      int columnWidthFactor = 1;

      if (this.SysEnv.IsWindowsMobile6Device)
      {
        columnWidthFactor = 2;
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
      dataGridCol.Width = 65 * columnWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ProductUnitBC";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Bc);
#else
      dataGridCol.HeaderText = "BC";
#endif
      dataGridCol.Width = 20 * columnWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "UnitCode";
      dataGridCol.HeaderText = string.Empty;
      dataGridCol.Width = 22 * columnWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ActivityItemAdjustmentQuantity";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Qtyzzz);
#else
      dataGridCol.HeaderText = "Qty";
#endif
      dataGridCol.Width = 55 * columnWidthFactor;
      dataGridCol.Format = "0.000";
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "LocationCode";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Whs);
#else
      dataGridCol.HeaderText = "Whs";
#endif
      dataGridCol.Width = 30 * columnWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridMain.TableStyles.Add(dataGridStyle);
      dataGridMain.RowHeadersVisible = false;
      dataGridMain.ColumnHeadersVisible = true;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override DataView GetDataView(bool isSubmitBatch)
    {
      StringBuilder sql = new StringBuilder();

      if (isSubmitBatch)
      {
        // Gather any newly added line items.
        // Also perform union into tActivityDetail to check of detail exists.
        sql.Append("SELECT tActivityItemAdjustment.ActivityItemRefno, ");
        sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentFKRefno,");
        sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentStringFKRefno,");
        sql.Append("tProductUnit.ProductUnitRefno,");
        sql.Append("tProduct.ProductRefno,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProduct.ProductDescription,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitSizeDescription,");
        sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentQuantity,");
        sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentBaseQuantity,");
        sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentWeightQuantity,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
        sql.Append("tActivityitem.ActivityItemStatus ");
        sql.Append("FROM tActivityItemAdjustment ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
        sql.Append("= tActivityItemAdjustment.ActivityItemRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
        sql.Append("= tActivityItemAdjustment.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityItemAdjustment.WarehouseLocationMapRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
        sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
        sql.Append("UNION ");
        sql.Append("SELECT tActivityItemAdjustment.ActivityItemRefno,");
        sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentFKRefno,");
        sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentStringFKRefno,");
        sql.Append("tProductUnit.ProductUnitRefno,");
        sql.Append("tProduct.ProductRefno,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProduct.ProductDescription,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitSizeDescription,");
        sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentQuantity,");
        sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentBaseQuantity,");
        sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentWeightQuantity,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
        sql.Append("tActivityitem.ActivityItemStatus ");
        sql.Append("FROM tActivityItemAdjustment ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
        sql.Append("= tActivityItemAdjustment.ActivityItemRefno ");
        sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
        sql.Append("= tActivityItemAdjustment.ActivityItemRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
        sql.Append("= tActivityItemAdjustment.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityItemAdjustment.WarehouseLocationMapRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
        sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
        sql.Append("ORDER BY tActivityItemAdjustment.ActivityItemRefno");
      }
      else
      {
        sql.Append("SELECT tActivityItemAdjustment.ActivityItemRefno,");
        sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentFKRefno,");
        sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentStringFKRefno,");
        sql.Append("tProductUnit.ProductUnitRefno,");
        sql.Append("tProduct.ProductRefno,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProduct.ProductDescription,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitSizeDescription,");
        sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentQuantity,");
        sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentBaseQuantity,");
        sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentWeightQuantity,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
        sql.Append("tActivityitem.ActivityItemStatus ");
        sql.Append("FROM tActivityItemAdjustment ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
        sql.Append("= tActivityItemAdjustment.ActivityItemRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
        sql.Append("= tActivityItemAdjustment.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityItemAdjustment.WarehouseLocationMapRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
        sql.Append("ORDER BY tActivityItemAdjustment.ActivityItemRefno");
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
    /// <remarks></remarks>
    public override void SelectItem(int idx)
    {
      // Did not call base.SelectItem() because of the default location rules for adjustments.

      this.CurrRowIdx = idx;
      this.Refno = (int)DataViewItems[idx]["ActivityItemRefno"];
      this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno((int)DataViewItems[idx]["ProductUnitRefno"]);
      this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

      this.FKRefno = (int)this.DataViewItems[idx]["ActivityItemAdjustmentFKRefno"];

      int actAdjType = (int)this.ActHdr.DataViewHdrs[this.ActHdr.SelectedIndex]["ActivityAdjustmentType"];
      if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc
          && !this.SysEnv.SysSettings.IsVerifyProductLocation
          && (SysCommon.AdjTypes)actAdjType == SysCommon.AdjTypes.Requisition)
      {
        // LocationMapRefno is assumed to be the adjustment WarehouseLocationMapRefno
        this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(
            this.GetDefaultLocMapRefno(
            (int)this.DataViewItems[idx]["LocationMapRefno"],
            (int)this.DataViewItems[idx]["ProductRefno"]));
      }

      // If a skid is selected clear it if the select product does not match the selected skid product.
      if (0 < this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno)
      {
        if (this.ActHdr.SkidInfo.CurrSkid.ProdInfo.Refno == this.ProdInfo.Refno)
        {
          this.LocMap.CurrLocMap = this.ActHdr.SkidInfo.CurrSkid.LocMap;
        }
        else if (SysEnv.SysSettings.DoesSelectingADifferentProductClearSkid)
        {
          this.ActHdr.SkidInfo = new SysSkidInfo();
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prodUnitRefno"></param>
    /// <returns></returns>
    public override int GetRefnoFromProdUnitRefno(int prodUnitRefno)
    {
      int foundActItemRefno = 0;

      using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        StringBuilder sql = new StringBuilder();
        sql.Append("SELECT tActivityItem.ActivityItemRefno,");
        sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentFKRefno,");
        sql.Append("tActivityItemAdjustment.ProductUnitRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("FROM tActivityItemAdjustment ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
        sql.Append("= tActivityItemAdjustment.ActivityItemRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityItemAdjustment.WarehouseLocationMapRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0}", this.ActHdr.Refno);
        sql.AppendFormat(
            " AND tActivityItemAdjustment.WarehouseLocationMapRefno = {0}",
            this.LocMap.CurrLocMap.WhsLocMapRefno);
        sql.AppendFormat(" AND tActivityItemAdjustment.ProductUnitRefno = {0}", prodUnitRefno);

        cmndItem.CommandType = CommandType.Text;
        cmndItem.CommandText = sql.ToString();
        using (SQLiteDataReader drItem = cmndItem.ExecuteReader())
        {
          if (drItem.Read())
          {
            this.Refno = (int)drItem["ActivityItemRefno"];
            this.FKRefno = (int)drItem["ActivityItemAdjustmentFKRefno"];
            this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno((int)drItem["ProductUnitRefno"]);
            this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

            foundActItemRefno = this.Refno;
          }

          drItem.Close();
        }
      }

      return foundActItemRefno;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override string CreateSubmitXML(XDocument data)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);
      string prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
      string barCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
      bool prodUnitIsBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
      string unitCode = data.Root.Element("UnitCode").Value;
      string symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>" 
          + "<action>SUBMIT ADJUSTMENT ITEM</action>" 
          + "</command>" 
          + SysCommon.GetRequestDefaults(false)
          + ("<prod_id>" + prodCode + "</prod_id>")
          + ("<is_bc>" + prodUnitIsBC + "</is_bc>") 
          + ("<symbology>" + symbologyPrefix + "</symbology>") 
          + ("<gtin>" + barCode + "</gtin>") 
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
    /// <param name="idx"></param>
    /// <param name="quantity"></param>
    /// <param name="baseQuantity"></param>
    /// <param name="weightQuantity"></param>
    public override void RefreshQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
    {
      this.DataViewItems[idx].BeginEdit();
      this.DataViewItems[idx]["ActivityItemAdjustmentQuantity"] = quantity;
      this.DataViewItems[idx]["ActivityItemAdjustmentBaseQuantity"] = baseQuantity;
      this.DataViewItems[idx]["ActivityItemAdjustmentWeightQuantity"] = weightQuantity;
      this.DataViewItems[idx].EndEdit();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rowIdx"></param>
    /// <param name="quantity"></param>
    /// <param name="baseQuantity"></param>
    /// <param name="weightQuantity"></param>
    /// <returns></returns>
    public override int UpdateDataViewQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
    {
      DataRowView rowView = this.DataViewItems[idx];

      rowView.BeginEdit();
      rowView["ActivityItemAdjustmentQuantity"] 
          = (decimal)this.DataViewItems[idx]["ActivityItemAdjustmentQuantity"] + quantity;
      rowView["ActivityItemAdjustmentBaseQuantity"]
          = (decimal)this.DataViewItems[idx]["ActivityItemAdjustmentBaseQuantity"] + baseQuantity;
      rowView["ActivityItemAdjustmentWeightQuantity"]
          = (decimal)this.DataViewItems[idx]["ActivityItemAdjustmentWeightQuantity"] + weightQuantity;
      rowView.EndEdit();

      return idx;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="actItemRefno"></param>
    public override void UpdateQuantityFromDataView(int idx, int actItemRefno)
    {
      using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        //cmndItem.IndexName = string.Empty;
        cmndItem.CommandType = CommandType.Text;
        cmndItem.CommandText = "UPDATE tActivityItemAdjustment" 
            + " SET ActivityItemAdjustmentQuantity = "
            + this.DataViewItems[idx]["ActivityItemAdjustmentQuantity"] + "," 
            + " ActivityItemAdjustmentBaseQuantity = "
            + this.DataViewItems[idx]["ActivityItemAdjustmentBaseQuantity"] + "," +
            " ActivityItemAdjustmentWeightQuantity = "
            + this.DataViewItems[idx]["ActivityItemAdjustmentWeightQuantity"] 
            + " WHERE tActivityItemAdjustment.ActivityItemRefno = " + actItemRefno;

        cmndItem.ExecuteNonQuery();
      }
    }

    #endregion
  }
}
