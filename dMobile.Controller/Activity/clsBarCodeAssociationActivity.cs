namespace dMobile.Controller
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using dMobile.GroundWork;

  public class BarCodeAssocActivity : Activity
  {
    #region Constructors

    public BarCodeAssocActivity()
      : base()
    {
      this.FullClassName = typeof(BarCodeAssocActivity).FullName;
    }

    #endregion

    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Purge()
    {
      TableActivityItemBarCodeAssociationXRef actBarCodeAssocXRef = new TableActivityItemBarCodeAssociationXRef();

      actBarCodeAssocXRef.Purge();

      try
      {
        using (SQLiteCommand cmndPurgeActItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurgeActItem.CommandText = "DELETE FROM tActivityItem " 
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.BarCodeAssociation;
          cmndPurgeActItem.CommandType = CommandType.Text;
          cmndPurgeActItem.ExecuteNonQuery();
        }

        using (SQLiteCommand cmndPurgeActivity = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurgeActivity.CommandText = "DELETE FROM tActivity " 
              + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.BarCodeAssociation;
          cmndPurgeActivity.CommandType = CommandType.Text;
          cmndPurgeActivity.ExecuteNonQuery();
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingBarCodeAssociationFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging bar code association failed.", ex);
#endif
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override string GetFKRefnoIdx()
    {
      return "ActivityCountStringFKRefno";
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override string GetControlDescription()
    {
#if MULTILANGUAGE
      return Translator.GetInstance().Translate(TranslatorVocabulary.BarCodeAssociation);
#else
      return "Bar Code Association";
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    public override void SelectHdr(int idx)
    {
      this.FKRefno = (int)this.DataViewHdrs[idx]["ActivityRefno"];
      this.Refno = (int)this.DataViewHdrs[idx]["ActivityRefno"];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public override int GetRefnoFromRowIdx(int idx)
    {
      this.FKRefno = (int)this.DataViewHdrs[idx]["ActivityRefno"];
      this.Refno = (int)this.DataViewHdrs[idx]["ActivityRefno"];

      return this.Refno;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="isSubmitBatch"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public override DataView GetDataView(bool isSubmitBatch)
    {
      StringBuilder sql = new StringBuilder();

      if (isSubmitBatch)
      {
        sql.Append("SELECT tActivity.ActivityRefno,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("tProductUnitBarCode.ProductUnitBarCodeRefno,");
        sql.Append("tProductUnitBarCode.ProductUnitBarCodeCode,");
        sql.Append("tProductUnitBarCode.ProductUnitBarCodeModifier1,");
        sql.Append("tActivity.ActivityStatus");
        sql.Append(" FROM tActivity");
        sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno");
        sql.Append(" INNER JOIN tActivityItemBarCodeAssociationXRef ON ");
        sql.Append("tActivityItemBarCodeAssociationXRef.ActivityItemRefno = tActivityItem.ActivityItemRefno");
        sql.Append(" INNER JOIN tProductUnitBarCode ON tProductUnitBarCode.ProductUnitBarCodeRefno = ");
        sql.Append("tActivityItemBarCodeAssociationXRef.ProductUnitBarCodeRefno");
        sql.Append(" INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tProductUnitBarCode.ProductUnitRefno");
        sql.Append(" INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno");
        sql.Append(" INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno");
        sql.AppendFormat(" WHERE tActivityItem.ActivityItemStatus <> {0}", (int)SysCommon.ActStatus.NoChange);
        sql.AppendFormat(" AND tActivityItem.ActivityItemSubType = {0}", (int)SysCommon.ActSubTypes.BarCodeAssociation);
      }
      else
      {
        sql.Append("SELECT tActivity.ActivityRefno,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("tProductUnitBarCode.ProductUnitBarCodeRefno,");
        sql.Append("tProductUnitBarCode.ProductUnitBarCodeCode,");
        sql.Append("tProductUnitBarCode.ProductUnitBarCodeModifier1,");
        sql.Append("tActivity.ActivityStatus");
        sql.Append(" FROM tActivity");
        sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno");
        sql.Append(" INNER JOIN tActivityItemBarCodeAssociationXRef ON ");
        sql.Append("tActivityItemBarCodeAssociationXRef.ActivityItemRefno = tActivityItem.ActivityItemRefno");
        sql.Append(" INNER JOIN tProductUnitBarCode ON tProductUnitBarCode.ProductUnitBarCodeRefno = ");
        sql.Append("tActivityItemBarCodeAssociationXRef.ProductUnitBarCodeRefno");
        sql.Append(" INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tProductUnitBarCode.ProductUnitRefno");
        sql.Append(" INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno");
        sql.Append(" INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno");
        sql.AppendFormat(" AND tActivityItem.ActivityItemSubType = {0}", (int)SysCommon.ActSubTypes.BarCodeAssociation);
      }

      using (SQLiteDataAdapter dataAdapterBarCode = new SQLiteDataAdapter(
          sql.ToString(), 
          DatabaseConnection.GetInstance().SQLConn))
      {
        using (DataSet dataSetBarCode = new DataSet())
        {
          dataAdapterBarCode.Fill(dataSetBarCode);
          this.DataViewHdrs = dataSetBarCode.Tables[0].DefaultView;
          this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
        }
      }

      return this.DataViewHdrs;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="searchText"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public override DataView Search(string searchText)
    {
      // TODO: search needs to handle non alphanumeric characters such as '[' which blows chunks
      this.DataViewHdrs.RowFilter = "ProductUnitBarCodeCode LIKE \'" + searchText.Trim() + "%\'";

      return this.DataViewHdrs;
    }

//    /// <summary>
//    ///
//    /// </summary>
//    /// <remarks></remarks>
//    public override void DesignDataGrid(DataGrid dataGridMain)
//    {
//      if (dataGridMain.TableStyles.Count > 0)
//      {
//        dataGridMain.TableStyles.Clear();
//      }

//      DataGridTableStyle dataGridStyle = new DataGridTableStyle();
//      dataGridStyle.MappingName = this.MainGridMappingName;

//      DataGridTextBoxColumn dataGridCol1 = new DataGridTextBoxColumn();
//      dataGridCol1.MappingName = "ProductCode";
//#if MULTILANGUAGE
//      dataGridCol1.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Productz);
//#else
//      dataGridCol1.HeaderText = "Product";
//#endif
//      dataGridCol1.Width = 65;
//      dataGridStyle.GridColumnStyles.Add(dataGridCol1);

//      DataGridTextBoxColumn dataGridCol2 = new DataGridTextBoxColumn();
//      dataGridCol2.MappingName = "ProductUnitBC";
//      dataGridCol2.HeaderText = string.Empty;
//      dataGridCol2.Width = 12;
//      dataGridStyle.GridColumnStyles.Add(dataGridCol2);

//      DataGridTextBoxColumn dataGridCol3 = new DataGridTextBoxColumn();
//      dataGridCol3.MappingName = "UnitCode";
//      dataGridCol3.HeaderText = string.Empty;
//      dataGridCol3.Width = 22;
//      dataGridStyle.GridColumnStyles.Add(dataGridCol3);

//      DataGridTextBoxColumn dataGridCol4 = new DataGridTextBoxColumn();
//      dataGridCol4.MappingName = "ProductUnitBarCodeCode";
//#if MULTILANGUAGE
//      dataGridCol4.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.BarCodez);
//#else
//      dataGridCol4.HeaderText = "Bar Code";
//#endif
//      dataGridCol4.Width = 130;
//      dataGridStyle.GridColumnStyles.Add(dataGridCol4);

//      DataGridTextBoxColumn dataGridCol5 = new DataGridTextBoxColumn();
//      dataGridCol5.MappingName = "ProductUnitBarCodeModifier1";
//      dataGridCol5.HeaderText = string.Empty;
//      dataGridCol5.Width = 12;
//      dataGridStyle.GridColumnStyles.Add(dataGridCol5);

//      dataGridMain.TableStyles.Add(dataGridStyle);
//      dataGridMain.RowHeadersVisible = false;
//      dataGridMain.ColumnHeadersVisible = true;
//    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Refresh(XDocument data)
    {
      if (!this.SysEnv.SysSettings.IsDisconnectedMode)
      {
        this.Purge();
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="xmlParams"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public override string GetFilter(XDocument data)
    {
      return string.Empty;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override string GetFilter()
    {
      return string.Empty;
    }

    #endregion
  }
}
