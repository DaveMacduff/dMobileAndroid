namespace dMobile.Controller
{
  using System;
  using System.Data;
  using System.Windows.Forms;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class POWorkSheetActDetail : ActDetail
  {
    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="activityItem"></param>
    public POWorkSheetActDetail(ActItem activityItem)
      : base(activityItem)
    {
      this.FullClassName = typeof(POWorkSheetActDetail).FullName;
    }

    #endregion

    #region Public Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="skidInfo"></param>
    public override void InsertSkid(SysSkidInfo skidInfo)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="skidInfo"></param>
    public override void DeleteSkid(SysSkidInfo skidInfo)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="detailData"></param>
    public override void Insert(XDocument detailData)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    ///
    /// </summary>
    public override void Delete()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedRowIndex"></param>
    /// <param name="detailData"></param>
    public override void Submit(int selectedRowIndex, XDocument detailData)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Refresh()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedIndex"></param>
    /// <returns></returns>
    public override int GetRefno(int selectedIndex)
    {
      return Convert.ToInt32(DataViewDetails[selectedIndex]["ActivityDetailRefno"].ToString());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedIndex"></param>
    /// <returns></returns>
    public override int GetFKRefno(int selectedIndex)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isSubmitBatch"></param>
    /// <returns></returns>
    public override System.Data.DataView GetDataView(bool isSubmitBatch)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dataGridMain"></param>
    public override void DesignDataGrid(DataGrid dataGridMain)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="searchText"></param>
    /// <returns></returns>
    public override DataView Search(string searchText)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedIndex"></param>
    public override void SelectDetail(int selectedIndex)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public override string CreateDeleteXML(XDocument data)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public override string CreateSubmitXML(XDocument data)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
