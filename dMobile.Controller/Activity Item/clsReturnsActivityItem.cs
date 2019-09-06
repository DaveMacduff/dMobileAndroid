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

  public class ReturnsActItem : ActItem
  {
    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="activity"></param>
    public ReturnsActItem(Activity activity)
      : base(activity)
    {
      this.FullClassName = typeof(ReturnsActItem).FullName;
      this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProd2Ret;
      this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsRetCreateNewItemNoAsk;
    }

    #endregion

    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override bool IsCatchAsk()
    {
      bool isCatchAsk = base.IsCatchAsk();
      if (isCatchAsk)
      {
          // modules are for exclusions
          isCatchAsk = this.SysEnv.SysSettings.IsRetCatchAskEnabled;
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
        isCatchAskZero = !this.SysEnv.SysSettings.IsReturnsNoCatchAskZeroEnabled;
      }

      return isCatchAskZero;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedIndex"></param>
    /// <returns></returns>
    public override string GetComment(int selectedIndex)
    {
      string comment = base.GetComment(selectedIndex);
      using (SQLiteCommand cmndItemReturnsComment = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndItemReturnsComment.CommandType = CommandType.TableDirect;
        cmndItemReturnsComment.CommandText = "SELECT ActivityItemRefno FROM tActivityItemReturnsComment WHERE ActivityItemRefno = @selectedIndex";
        cmndItemReturnsComment.Parameters.Add(new SQLiteParameter("@selectedIndex", selectedIndex));

        //cmndItemReturnsComment.IndexName = "PK_ActivityItemReturnsCommentOnActivityItemRefno";

        using (SQLiteDataReader drItemReturnsComment = cmndItemReturnsComment.ExecuteReader())
        {
          //if (drItemReturnsComment.Seek(DbSeekOptions.FirstEqual, new object[] { selectedIndex }))
          //{
            if (drItemReturnsComment.Read())
            {
              comment = drItemReturnsComment["ActivityItemReturnsCommentComment"].ToString();
            }
          //}
        }
      }

      return comment;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="whsLocMapRefno"></param>
    /// <param name="prodRefno"></param>
    /// <returns></returns>
    public override int GetDefaultLocMapRefno(int whsLocMapRefno, int prodRefno)
    {
      int locMapRefno = LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(
          whsLocMapRefno,
          prodRefno,
          SysCommon.ProdLocMapTypes.Receiving);

      return locMapRefno;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemStringFKRefno"></param>
    /// <returns></returns>
    public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
    {
#if MULTILANGUAGE
      return string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToParseReturnsItem), actItemStringFKRefno);
#else
      return string.Format("Unable to parse returns item # {0}", actItemStringFKRefno);
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
      return string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToFindReturnsItem), actItemStringFKRefno);
#else
      return string.Format("Unable to find returns item # {0}", actItemStringFKRefno);
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetDataViewSortFKRefnoIdx()
    {
      return "ActivityItemReturnsStringFKRefno";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="askToInsertAsNewItem"></param>
    public override void Insert(XDocument data, bool askToInsertAsNewItem)
    {
#if MULTILANGUAGE
      throw new NotImplementedException(Translator.GetInstance().Translate(TranslatorVocabulary.AddNewReturnItemsIsNotSupported));
#else
      throw new NotImplementedException("Add new return items is not supported.");
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xmlParam"></param>
    public override void Refresh(XDocument xmlParam)
    {
      if (this.SysEnv.SysSettings.IsDisconnectedMode)
      {
        this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
      }
      else
      {
        this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

        TableActivityReturnsItem tableActReturnsItem = new TableActivityReturnsItem();
        TableActivityReturnsDetail tableActReturnsDetail = new TableActivityReturnsDetail();
        tableActReturnsDetail.Purge();
        tableActReturnsItem.Purge();

        this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

        tableActReturnsItem.RegisterDataAccessListenerProgressOut((ReturnsActItem)this);
        tableActReturnsItem.Request(ActHdr.Refno);
        tableActReturnsItem.UnregisterDataAccessListenerProgressOut((ReturnsActItem)this);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataGridMain"></param>
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
      dataGridCol.MappingName = "IsActivityItemReturnsComment";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.C);
#else
      dataGridCol.HeaderText = "C";
#endif
      dataGridCol.Width = 12 * columnWidthFactor;
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
      dataGridCol.HeaderText = string.Empty;
      dataGridCol.Width = 20 * columnWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "UnitCode";
      dataGridCol.HeaderText = string.Empty;
      dataGridCol.Width = 22 * columnWidthFactor;
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      dataGridCol = new DataGridTextBoxColumn();
      dataGridCol.MappingName = "ActivityItemReturnsQuantityReturned";
#if MULTILANGUAGE
      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Returned);
#else
      dataGridCol.HeaderText = "Returned";
#endif
      dataGridCol.Width = 52 * columnWidthFactor;
      dataGridCol.Format = "0.000";
      dataGridStyle.GridColumnStyles.Add(dataGridCol);

      ////dataGridCol.MappingName = string.Empty
      ////dataGridCol.HeaderText = "R"
      ////dataGridCol.Width = 12 * colWidthFactor
      ////dataGridStyle.GridColumnStyles.Add(dataGridCol)

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
      SQLiteCommand cmd = DatabaseConnection.GetInstance().SQLConn.CreateCommand();
      cmd.CommandType = CommandType.Text;

      if (isSubmitBatch)
      {
        cmd.CommandText = 
        "SELECT tActivityItemReturns.ActivityItemRefno, " +
        "tActivityItemReturns.ActivityItemReturnsFKRefno, " +
        "tActivityItemReturns.ActivityItemReturnsStringFKRefno, " +
        "tProductUnit.ProductUnitRefno, " +
        "tProduct.ProductRefno, " +
        "tProduct.ProductCode, " +
        "tProduct.ProductDescription, " +
        "tProduct.ProductBrandDescription, " +
        "tUnit.UnitCode, " +
        "tProductUnit.ProductUnitBC, " +
        "tProductUnit.ProductUnitSizeDescription, " +
        "tActivityItemReturns.ActivityItemReturnsQuantityReturned, " +
        "tActivityItemReturns.ActivityItemReturnsBaseQuantityReturned, " +
        "tActivityItemReturns.ActivityItemReturnsWeightQuantityReturned, " +
        "tActivityItemReturns.ActivityItemReturnsCustomerProductUnitCode, " +
        "tLocationMapWarehouse.LocationMapRefno, " +
        "tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode, " +
        "tLocationMapWarehouse.LocationMapWarehouseName AS LocationName, " +
        "CASE WHEN tActivityItemReturnsComment.ActivityItemReturnsCommentComment IS NULL " +
        "THEN \' \' ELSE \'*\' END AS IsActivityItemReturnsComment, " +
        "tActivityItem.ActivityItemStatus " +
        "FROM tActivityItemReturns " +
        "INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = " +
        "tActivityItemReturns.ActivityItemRefno " +
        "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemReturns.ProductUnitRefno " +
        "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno " +
        "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno " +
        "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = " +
        "tActivityItemReturns.WarehouseLocationMapRefno " +
        "LEFT JOIN tActivityItemReturnsComment ON tActivityItemReturnsComment.ActivityItemRefno = " +
        "tActivityItemReturns.ActivityItemRefno " +
        "WHERE tActivityItem.ActivityRefno = @Refno " +
        "AND tActivityItem.ActivityItemStatus <> @ActStatus " +
        "UNION " +
        "SELECT tActivityItemReturns.ActivityItemRefno, " +
        "tActivityItemReturns.ActivityItemReturnsFKRefno, " +
        "tActivityItemReturns.ActivityItemReturnsStringFKRefno, " +
        "tProductUnit.ProductUnitRefno, " +
        "tProduct.ProductRefno, " +
        "tProduct.ProductCode, " +
        "tProduct.ProductDescription, " +
        "tProduct.ProductBrandDescription, " +
        "tUnit.UnitCode, " +
        "tProductUnit.ProductUnitBC, " +
        "tProductUnit.ProductUnitSizeDescription, " +
        "tActivityItemReturns.ActivityItemReturnsQuantityReturned, " +
        "tActivityItemReturns.ActivityItemReturnsBaseQuantityReturned, " +
        "tActivityItemReturns.ActivityItemReturnsWeightQuantityReturned, " +
        "tActivityItemReturns.ActivityItemReturnsCustomerProductUnitCode, " +
        "tLocationMapWarehouse.LocationMapRefno, " +
        "tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode, " +
        "tLocationMapWarehouse.LocationMapWarehouseName AS LocationName, " +
        "CASE WHEN tActivityItemReturnsComment.ActivityItemReturnsCommentComment IS NULL " +
        "THEN \' \' ELSE \'*\' END AS IsActivityItemReturnsComment, " +
        "tActivityItem.ActivityItemStatus " +
        "FROM tActivityItemReturns " +
        "INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = " +
        "tActivityItemReturns.ActivityItemRefno " +
        "INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = " +
        "tActivityItemReturns.ActivityItemRefno " +
        "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemReturns.ProductUnitRefno " +
        "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno " +
        "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno " +
        "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = " +
        "tActivityItemReturns.WarehouseLocationMapRefno " +
        "LEFT JOIN tActivityItemReturnsComment ON tActivityItemReturnsComment.ActivityItemRefno = " +
        "tActivityItemReturns.ActivityItemRefno " +
        "WHERE tActivityItem.ActivityRefno = @Refno " +
        "AND tActivityDetail.ActivityDetailStatus <> @ActStatus " +
        "ORDER BY tActivityItemReturns.ActivityItemRefno";
         cmd.Parameters.Add(new SQLiteParameter("@Refno", this.ActHdr.Refno));
         cmd.Parameters.Add(new SQLiteParameter("@ActStatus", (int)SysCommon.ActStatus.NoChange));
      }
      else
      {
        cmd.CommandText = 
        "SELECT tActivityItemReturns.ActivityItemRefno, " +
        "tActivityItemReturns.ActivityItemReturnsFKRefno, " +
        "tActivityItemReturns.ActivityItemReturnsStringFKRefno, " +
        "tProductUnit.ProductUnitRefno, " +
        "tProduct.ProductRefno, " +
        "tProduct.ProductCode, " +
        "tProduct.ProductDescription, " +
        "tProduct.ProductBrandDescription, " +
        "tUnit.UnitCode, " +
        "tProductUnit.ProductUnitBC, " +
        "tProductUnit.ProductUnitSizeDescription, " +
        "tActivityItemReturns.ActivityItemReturnsQuantityReturned, " +
        "tActivityItemReturns.ActivityItemReturnsBaseQuantityReturned, " +
        "tActivityItemReturns.ActivityItemReturnsWeightQuantityReturned, " +
        "tActivityItemReturns.ActivityItemReturnsCustomerProductUnitCode, " +
        "tLocationMapWarehouse.LocationMapRefno, " +
        "tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode, " +
        "tLocationMapWarehouse.LocationMapWarehouseName AS LocationName, " +
        "CASE WHEN tActivityItemReturnsComment.ActivityItemReturnsCommentComment IS NULL " +
        "THEN \' \' ELSE \'*\' END AS IsActivityItemReturnsComment, " +
        "tActivityItem.ActivityItemStatus " +
        "FROM tActivityItemReturns " +
        "INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = " +
        "tActivityItemReturns.ActivityItemRefno " +
        "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemReturns.ProductUnitRefno " +
        "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno " +
        "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno " +
        "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = " +
        "tActivityItemReturns.WarehouseLocationMapRefno " +
        "LEFT JOIN tActivityItemReturnsComment ON tActivityItemReturnsComment.ActivityItemRefno = " +
        "tActivityItemReturns.ActivityItemRefno " +
        "WHERE tActivityItem.ActivityRefno = @Refno " +
        "ORDER BY tActivityItemReturns.ActivityItemRefno";
         cmd.Parameters.Add(new SQLiteParameter("@Refno", this.ActHdr.Refno));
      }

      using (SQLiteDataAdapter dataAdapterReturnItems = new SQLiteDataAdapter(cmd))
      {
        using (DataSet dataSetReturnItems = new DataSet())
        {
          dataAdapterReturnItems.Fill(dataSetReturnItems);
          this.DataViewItems = dataSetReturnItems.Tables[0].DefaultView;
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
      this.FKRefno = Convert.ToInt32(this.DataViewItems[idx]["ActivityItemReturnsFKRefno"]);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prodUnitRefno"></param>
    /// <returns></returns>
    public override int GetRefnoFromProdUnitRefno(int prodUnitRefno)
    {
      int foundActItemRefno = 0;
      int whsLocMapRefno = this.LocMap.CurrLocMap.WhsLocMapRefno;

      using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndItem.CommandType = CommandType.Text;
        cmndItem.CommandText = "SELECT tActivityItem.ActivityItemRefno, " +
        "tActivityItemReturns.ActivityItemReturnsFKRefno, " +
        "tActivityItemReturns.ProductUnitRefno, " +
        "tLocationMapWarehouse.LocationMapRefno " +
        "FROM tActivityItemReturns " +
        "INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = " +
        "tActivityItemReturns.ActivityItemRefno " +
        "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = " +
        "tActivityItemReturns.WarehouseLocationMapRefno " +
        "WHERE tActivityItem.ActivityRefno = @Refno " +
        "AND tActivityItemReturns.WarehouseLocationMapRefno = @whsLocMapRefno " + 
        "AND tActivityItemReturns.ProductUnitRefno = @prodUnitRefno";
        cmndItem.Parameters.Add(new SQLiteParameter("@Refno", this.ActHdr.Refno));
        cmndItem.Parameters.Add(new SQLiteParameter("@whsLocMapRefno", whsLocMapRefno));
        cmndItem.Parameters.Add(new SQLiteParameter("@prodUnitRefno", prodUnitRefno));

        using (SQLiteDataReader drItem = cmndItem.ExecuteReader())
        {
          if (drItem.Read())
          {
            // <WO ID="14777" comment="The last of the uncommented locmap code was removed here. We do not wish to set location map when we just want refnos." />
            this.Refno = Convert.ToInt32(drItem["ActivityItemRefno"]);
            this.FKRefno = Convert.ToInt32(drItem["ActivityItemReturnsFKRefno"]);
            this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(drItem["ProductUnitRefno"]));
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
    /// <param name="data"></param>
    /// <returns></returns>
    public override string CreateSubmitXML(XDocument data)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);
      string deviceID = this.SysEnv.DeviceID;
      string revision = this.SysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
      string loginID = this.SysEnv.ContactLog.LoginID.ToString();
      string prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
      string prodUnitBarCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
      bool prodUnitIsBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
      string unitCode = data.Root.Element("UnitCode").Value;
      string barCodeSymbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;
      string current = DateTime.Now.ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>" 
          + "<action>SUBMIT SALES RETURN ITEM</action>" 
          + "</command>" 
          + ("<device_id>" + deviceID + "</device_id>") 
          + ("<login_id>" + loginID + "</login_id>") 
          + ("<revision_datetime>" + revision + "</revision_datetime>") 
          + ("<stamp>" + current + "</stamp>") 
          + ("<prod_id>" + prodCode + "</prod_id>") 
          + ("<is_bc>" + prodUnitIsBC.ToString() + "</is_bc>") 
          + ("<symbology>" + barCodeSymbologyPrefix + "</symbology>") 
          + ("<gtin>" + prodUnitBarCodeCode + "</gtin>") 
          + ("<whs_id>" + LocMap.CurrLocMap.WhsFKRefno.ToString() + "</whs_id>") 
          + ("<sect_refno>" + LocMap.CurrLocMap.SectFKRefno.ToString() + "</sect_refno>") 
          + ("<slot_refno>" + LocMap.CurrLocMap.SlotFKRefno.ToString() + "</slot_refno>") 
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
      this.DataViewItems[idx]["ActivityItemReturnsQuantityReturned"] = quantity;
      this.DataViewItems[idx]["ActivityItemReturnsBaseQuantityReturned"] = baseQuantity;
      this.DataViewItems[idx]["ActivityItemReturnsWeightQuantityReturned"] = weightQuantity;
      this.DataViewItems[idx].EndEdit();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="quantity"></param>
    /// <param name="baseQuantity"></param>
    /// <param name="weightQuantity"></param>
    /// <returns></returns>
    public override int UpdateDataViewQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
    {
      this.DataViewItems[idx].BeginEdit();
      this.DataViewItems[idx]["ActivityItemReturnsQuantityReturned"] 
          = Convert.ToDecimal(this.DataViewItems[idx]["ActivityItemReturnsQuantityReturned"]) + quantity;
      this.DataViewItems[idx]["ActivityItemReturnsBaseQuantityReturned"]
          = Convert.ToDecimal(this.DataViewItems[idx]["ActivityItemReturnsBaseQuantityReturned"]) + baseQuantity;
      this.DataViewItems[idx]["ActivityItemReturnsWeightQuantityReturned"]
          = Convert.ToDecimal(this.DataViewItems[idx]["ActivityItemReturnsWeightQuantityReturned"]) + weightQuantity;
      this.DataViewItems[idx].EndEdit();

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
        cmndItem.CommandType = CommandType.Text;
        cmndItem.CommandText = 
            "UPDATE tActivityItemReturns " + 
            "SET ActivityItemReturnsQuantityReturned = @ActivityItemReturnsQuantityReturned, " +
            "ActivityItemReturnsBaseQuantityReturned = @ActivityItemReturnsBaseQuantityReturned, " +
            "ActivityItemReturnsWeightQuantityReturned = @ActivityItemReturnsWeightQuantityReturned " +
            "WHERE tActivityItemReturns.ActivityItemRefno = @actItemRefno";
        cmndItem.Parameters.Add(new SQLiteParameter("@ActivityItemReturnsQuantityReturned", this.DataViewItems[idx]["ActivityItemReturnsQuantityReturned"]));
        cmndItem.Parameters.Add(new SQLiteParameter("@ActivityItemReturnsBaseQuantityReturned", this.DataViewItems[idx]["ActivityItemReturnsBaseQuantityReturned"]));
        cmndItem.Parameters.Add(new SQLiteParameter("@ActivityItemReturnsWeightQuantityReturned", this.DataViewItems[idx]["ActivityItemReturnsWeightQuantityReturned"]));
        cmndItem.Parameters.Add(new SQLiteParameter("@actItemRefno", actItemRefno));

        cmndItem.ExecuteNonQuery();
      }
    }

    #endregion
  }
}
