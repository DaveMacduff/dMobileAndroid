namespace dMobile.Bedrock
{
    
    using System;
  using System.IO;
    using System.Reflection;
  using System.Xml.Serialization;



  [Serializable()]
    public class PickingSettings
    {
	    //VARIABLES
	    [NonSerialized()]
	    private string m_strFullClassName = "dMobile.PickingSettings";
	    [NonSerialized()]
	    private string m_strFilePath = string.Empty;
	    [NonSerialized()]
	    private string m_strFileASCIIZ = string.Empty;
	    [NonSerialized()]
	    private string m_strFileName = "dMobilePickingSettings.xml";
	    private Int32 m_iSortBy1Index = 0;
	    private string m_strSortBy1Value = "ActivityPickingFKRefno ASC";
	    private Int32 m_iSortBy2Index = -1;
	    private string m_strSortBy2Value = string.Empty;
	    private Int32 m_iSortBy3Index = -1;
	    private string m_strSortBy3Value = string.Empty;
	    private Int32 m_iSortBy4Index = -1;
	    private string m_strSortBy4Value = string.Empty;
	    private Int32 m_iRouteCodeSelectedIndex = -1;
	    private string m_strRouteCodeSelectedValue = string.Empty;
	    private bool isApplyRouteCodeFilter = false;
	    private System.DateTime m_dtOrderedDateFilter = new System.DateTime(1900, 1, 1);
	    private bool isApplyOrderedDateFilter = false;
	    private System.DateTime m_dtDeliveryDateFilter = new System.DateTime(1900, 1, 1);
	    private bool isApplyDeliveryDateFilter = false;

	    //PROPERTIES
	    public string strRouteCodeSelectedValue 
        {
		    get { return m_strRouteCodeSelectedValue; }
		    set { m_strRouteCodeSelectedValue = value; }
	    }

	    public bool IsApplyDeliveryDateFilter 
        {
		    get { return isApplyDeliveryDateFilter; }
		    set { isApplyDeliveryDateFilter = value; }
	    }

	    public bool IsApplyOrderedDateFilter
        {
		    get { return isApplyOrderedDateFilter; }
		    set { isApplyOrderedDateFilter = value; }
	    }

	    public System.DateTime dtDeliveryDateFilter
        {
		    get { return m_dtDeliveryDateFilter; }
		    set { m_dtDeliveryDateFilter = value; }
	    }

	    public System.DateTime dtOrderedDateFilter
        {
		    get { return m_dtOrderedDateFilter; }
		    set { m_dtOrderedDateFilter = value; }
	    }

	    public bool IsApplyRouteCodeFilter 
        {
		    get { return isApplyRouteCodeFilter; }
            set { isApplyRouteCodeFilter = value; }
	    }

	    public Int32 iRouteCodeSelectedIndex 
        {
		    get { return m_iRouteCodeSelectedIndex; }
		    set { m_iRouteCodeSelectedIndex = value; }
	    }

	    public string strFullClassName 
        {
		    get { return m_strFullClassName; }
		    set { m_strFullClassName = value; }
	    }

	    public string strFilePath
        {
		    get { return m_strFilePath; }
		    set { m_strFilePath = value; }
	    }

	    public string strFileASCIIZ 
        {
		    get { return m_strFileASCIIZ; }
		    set { m_strFileASCIIZ = value; }
	    }

	    public string strFileName
        {
		    get { return m_strFileName; }
		    set { m_strFileName = value; }
	    }

	    public Int32 iSortBy1Index
        {
		    get { return m_iSortBy1Index; }
		    set { m_iSortBy1Index = value; }
	    }

	    public string strSortBy1Value
        {
		    get { return m_strSortBy1Value; }
		    set { m_strSortBy1Value = value; }
	    }

	    public Int32 iSortBy2Index
        {
		    get { return m_iSortBy2Index; }
		    set { m_iSortBy2Index = value; }
	    }

	    public string strSortBy2Value
        {
		    get { return m_strSortBy2Value; }
		    set { m_strSortBy2Value = value; }
	    }

	    public Int32 iSortBy3Index
        {
		    get { return m_iSortBy3Index; }
		    set { m_iSortBy3Index = value; }
	    }

	    public string strSortBy3Value
        {
		    get { return m_strSortBy3Value; }
		    set { m_strSortBy3Value = value; }
	    }

	    public Int32 iSortBy4Index 
        {
		    get { return m_iSortBy4Index; }
		    set { m_iSortBy4Index = value; }
	    }

	    public string strSortBy4Value
        {
		    get { return m_strSortBy4Value; }
		    set { m_strSortBy4Value = value; }
	    }

	    //METHODS

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <remarks></remarks>
	    public PickingSettings()
	    {
		    strFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\";
		    strFileASCIIZ = Path.Combine(strFilePath, strFileName);
	    }

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <returns></returns>
	    /// <remarks></remarks>
	    public string GetDataViewSortBy()
	    {
		    string strSortBy = strSortBy1Value;

		    if (!string.IsNullOrEmpty(strSortBy2Value))
            {
			    strSortBy = strSortBy + ", " + strSortBy2Value;
		    }

		    if (!string.IsNullOrEmpty(strSortBy3Value))
            {
			    strSortBy = strSortBy + ", " + strSortBy3Value;
		    }

		    if (!string.IsNullOrEmpty(strSortBy4Value)) 
            {
			    strSortBy = strSortBy + ", " + strSortBy4Value;
		    }

		    return strSortBy;
	    }

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <returns></returns>
	    /// <remarks></remarks>
	    public PickingSettings LoadData()
	    {
		    XmlSerializer serializer = new XmlSerializer(typeof(PickingSettings));
		    PickingSettings oPickingSettings = new PickingSettings();

		    // use relection to get the current assembly path for the database
		    string strAppDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
		    string strFullPathToMyFile = Path.Combine(strAppDir, m_strFileName);

		    if (!File.Exists(strFullPathToMyFile)) 
            {
			    SaveData(ref oPickingSettings);
		    }

		    // Create a FileStream to read with.
		    FileStream reader = new FileStream(strFullPathToMyFile, FileMode.Open);
		    // Deserialize the stream to the object, and close the TextReader
		    oPickingSettings = (PickingSettings)serializer.Deserialize(reader);
		    reader.Close();

		    return oPickingSettings;
	    }

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="oPickingSettings"></param>
	    /// <returns></returns>
	    /// <remarks></remarks>
	    public bool SaveData(ref PickingSettings oPickingSettings)
	    {
		    XmlSerializer serializer = new XmlSerializer(typeof(PickingSettings));
		    bool bSaveComplete = false;

		    try 
            {
			    // use relection to get the current assembly path for the database
			    string strAppDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
			    string strFullPathToMyFile = Path.Combine(strAppDir, m_strFileName);

			    // Create a FileStream to write with.
			    FileStream writer = new FileStream(strFullPathToMyFile, FileMode.Create);
			    // Serialize the object, and close the TextWriter
			    serializer.Serialize(writer, oPickingSettings);
			    writer.Close();

			    bSaveComplete = true;
		    }
            catch
            {
			    bSaveComplete = false;
		    }

		    return bSaveComplete;
	    }
    }
}
