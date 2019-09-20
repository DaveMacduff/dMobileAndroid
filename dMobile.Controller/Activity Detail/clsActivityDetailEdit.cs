namespace dMobile.Controller
{
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class ActDetailEdit
  {
    #region Fields

    private SProductSchemeData schemeData = new SProductSchemeData();
    private decimal quantity = 0;
    private decimal baseQuantity = 0;
    private decimal weightQuantity = 0;
    private ActDetailExtra actExtra = new ActDetailExtra();

    #endregion

    #region Properties

    public ActDetailExtra ActExtra
    {
      get
      {
        return this.actExtra;
      }

      set
      {
        this.actExtra = value;
      }
    }

    public decimal Quantity
    {
      get
      {
        return this.quantity;
      }

      set
      {
        this.quantity = value;
      }
    }

    public decimal BaseQuantity
    {
      get
      {
        return this.baseQuantity;
      }

      set
      {
        this.baseQuantity = value;
      }
    }

    public decimal WeightQuantity
    {
      get
      {
        return this.weightQuantity;
      }

      set
      {
        this.weightQuantity = value;
      }
    }

    public SProductSchemeData SchemeData
    {
      get
      {
        return this.schemeData;
      }

      set
      {
        this.schemeData = value;
      }
    }

    #endregion

    #region Public Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prodRefno"></param>
    /// <returns></returns>
    public DataView GetVariantDataView(int prodRefno)
    {
      DataView dataViewVariant;

      StringBuilder s = new StringBuilder();
      s.Append("SELECT tProductSchemeVariant.ProductSchemeVariantCode,");
      s.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
      s.Append("tProductSchemeVariant.ProductSchemeVariantCode + \'  \' ");
      s.Append("+ tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDisplay ");
      s.Append("FROM tProduct ");
      s.Append("LEFT JOIN tProductSchemeVariantXRef ON tProductSchemeVariantXRef.ProductSchemeRefno = ");
      s.Append("tProduct.ProductSchemeRefno ");
      s.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno = ");
      s.Append("tProductSchemeVariantXRef.ProductSchemeVariantRefno ");
      s.AppendFormat("WHERE tProduct.ProductRefno = {0}", prodRefno);
      s.Append(" ORDER BY tProductSchemeVariant.ProductSchemeVariantCode");

      using (SQLiteDataAdapter dataAdapterVariant = new SQLiteDataAdapter(
          s.ToString(),
          DatabaseConnection.GetInstance().SQLConn))
      {
        using (DataSet dataSetVariant = new DataSet())
        {
          dataAdapterVariant.Fill(dataSetVariant);
          dataViewVariant = dataSetVariant.Tables[0].DefaultView;
        }
      }

      return dataViewVariant;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prodRefno"></param>
    /// <returns></returns>
    public DataView GetLotDataView(int prodRefno)
    {
      DataView dataViewLot;

      // TODO Put a number of lot max check, if tProductSchemeLotNumber is > 1000 only load the Detail lots not the
      // tProductSchemeLotNumber the exception would be receiving. The supplier Lot needs to exist in the list for
      // a lookup match.
      StringBuilder s = new StringBuilder();
      s.Append("SELECT DISTINCT tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber ");
      s.Append("FROM tActivityDetail ");
      s.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
      s.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
      s.AppendFormat("WHERE tProduct.ProductRefno = {0} ", prodRefno);
      s.Append("UNION ");
      s.Append("SELECT tProductSchemeLotNumber.ProductSchemeLotNumberCode ");
      s.Append("AS ActivityDetailLogisticUnitBatchLotNumber ");
      s.Append("FROM tProductSchemeLotNumber ");
      s.Append("ORDER BY ActivityDetailLogisticUnitBatchLotNumber");

      using (SQLiteDataAdapter dataAdapterLot = new SQLiteDataAdapter(
          s.ToString(),
          DatabaseConnection.GetInstance().SQLConn))
      {
        using (DataSet dataSetLot = new DataSet())
        {
          dataAdapterLot.Fill(dataSetLot);
          dataViewLot = dataSetLot.Tables[0].DefaultView;
        }
      }

      return dataViewLot;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prodRefno"></param>
    /// <returns></returns>
    public DataView GetSerialNumberDataView(int prodRefno)
    {
      DataView dataViewSerial;

      StringBuilder s = new StringBuilder();
      s.Append("SELECT DISTINCT tActivityDetail.ActivityDetailSingleTradeItemSerialNumber ");
      s.Append("FROM tActivityDetail ");
      s.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
      s.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
      s.AppendFormat("WHERE tProduct.ProductRefno = {0} ", prodRefno);
      s.Append("ORDER BY tActivityDetail.ActivityDetailSingleTradeItemSerialNumber");

      using (SQLiteDataAdapter dataAdapterSerial = new SQLiteDataAdapter(
          s.ToString(),
          DatabaseConnection.GetInstance().SQLConn))
      {
        using (DataSet dataSetSerial = new DataSet())
        {
          dataAdapterSerial.Fill(dataSetSerial);
          dataViewSerial = dataSetSerial.Tables[0].DefaultView;
        }
      }

      return dataViewSerial;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="detailEdit"></param>
    /// <returns></returns>
    public XDocument GetXMLFromActivityDetailEdit(ActDetailEdit detailEdit)
    {
      bool isBatchPrintProductLabel = SysEnvironment.GetInstance().SysSettings.IsPrintProductLabelsAsBatch;

      XDocument xmlDoc = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
          + "<params>"
          + "<ser_refno>0</ser_refno>"
          + "<refno>0</refno>"
          + ("<serial>" + detailEdit.SchemeData.SingleTradeItemSerialNumber + "</serial>")
          + ("<pdate>" + detailEdit.SchemeData.LogisticUnitProductionDate + "</pdate>")
          + ("<lot_no>" + detailEdit.SchemeData.LogisticUnitBatchLotNumber + "</lot_no>")
          + ("<bdate>" + detailEdit.SchemeData.LogisticUnitBestBeforeSellByDate + "</bdate>")
          + ("<variant_code>" + detailEdit.SchemeData.VariantCode + "</variant_code>")
          + ("<variant_no>" + detailEdit.SchemeData.VariantNumber + "</variant_no>")
          + ("<alt_qty>" + detailEdit.Quantity + "</alt_qty>")
          + ("<wt_qty>" + detailEdit.WeightQuantity + "</wt_qty>")
          + ("<base_qty>" + detailEdit.BaseQuantity + "</base_qty>")
          + ("<is_batch_print_product_label>" + isBatchPrintProductLabel + "</is_batch_print_product_label>")
          + ("<extra1>" + detailEdit.ActExtra.Text1 + "</extra1>")
          + ("<extra2>" + detailEdit.ActExtra.Text2 + "</extra2>")
          + ("<extra3>" + detailEdit.ActExtra.Text3 + "</extra3>")
          + ("<extra4>" + detailEdit.ActExtra.Text4 + "</extra4>")
          + ("<extra5>" + detailEdit.ActExtra.Text5 + "</extra5>")
          + "</params>");

      return xmlDoc;
    }

    #endregion
  }
}
