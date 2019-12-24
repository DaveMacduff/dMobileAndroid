namespace dMobile.Bedrock
{
  
    using System;
  using System.IO;
    using System.Reflection;
  using System.Xml.Serialization;



  /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class PhysicalCountSettings
    {
	    //VARIABLES
	    [NonSerialized()]
	    private string m_strFullClassName = "dMobile.PhysicalCountSettings";
	    [NonSerialized()]
	    private string m_strFilePath = string.Empty;
	    [NonSerialized()]
	    private string m_strFileASCIIZ = string.Empty;
	    [NonSerialized()]
	    private string m_strFileName = "dMobilePhysicalCountSettings.xml";
	    private Int32 m_iSortBy1Index = 0;
	    private string m_strSortBy1Value = "ActivityCountFKRefno ASC";
	    private Int32 m_iSortBy2Index = -1;
	    private string m_strSortBy2Value = string.Empty;
	    private System.DateTime m_dtCountDate = new System.DateTime(1900, 1, 1);
	    private bool isApplyCountDate = false;

	    //PROPERTIES
	    public bool IsApplyCountDate {
		    get { return isApplyCountDate; }
		    set { isApplyCountDate = value; }
	    }

	    public System.DateTime dtCountDate {
		    get { return m_dtCountDate; }
		    set { m_dtCountDate = value; }
	    }

	    public string strFullClassName {
		    get { return m_strFullClassName; }
		    set { m_strFullClassName = value; }
	    }

	    public string strFilePath {
		    get { return m_strFilePath; }
		    set { m_strFilePath = value; }
	    }

	    public string strFileASCIIZ {
		    get { return m_strFileASCIIZ; }
		    set { m_strFileASCIIZ = value; }
	    }

	    public string strFileName {
		    get { return m_strFileName; }
		    set { m_strFileName = value; }
	    }

	    public Int32 iSortBy1Index {
		    get { return m_iSortBy1Index; }
		    set { m_iSortBy1Index = value; }
	    }

	    public string strSortBy1Value {
		    get { return m_strSortBy1Value; }
		    set { m_strSortBy1Value = value; }
	    }

	    public Int32 iSortBy2Index {
		    get { return m_iSortBy2Index; }
		    set { m_iSortBy2Index = value; }
	    }

	    public string strSortBy2Value {
		    get { return m_strSortBy2Value; }
		    set { m_strSortBy2Value = value; }
	    }

	    //METHODS

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <remarks></remarks>
	    public PhysicalCountSettings()
	    {
		    strFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
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

		    if (!string.IsNullOrEmpty(strSortBy2Value)) {
			    strSortBy = strSortBy + ", " + strSortBy2Value;
		    }
		    return strSortBy;
	    }

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <returns></returns>
	    /// <remarks></remarks>
	    public PhysicalCountSettings LoadData()
	    {
		    XmlSerializer serializer = new XmlSerializer(typeof(PhysicalCountSettings));
		    PhysicalCountSettings oPhysicalCountSettings = new PhysicalCountSettings();

		    // use relection to get the current assembly path for the database
		    string strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		    string strFullPathToMyFile = Path.Combine(strAppDir, m_strFileName);

		    if (!File.Exists(strFullPathToMyFile))
            {
			    SaveData(ref oPhysicalCountSettings);
		    }

		    // Create a FileStream to read with.
		    FileStream reader = new FileStream(strFullPathToMyFile, FileMode.Open);
		    // Deserialize the stream to the object, and close the TextReader
		    oPhysicalCountSettings = (PhysicalCountSettings)serializer.Deserialize(reader);
		    reader.Close();

		    return oPhysicalCountSettings;
	    }

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="oPhysicalCountSettings"></param>
	    /// <returns></returns>
	    /// <remarks></remarks>
	    public bool SaveData(ref PhysicalCountSettings oPhysicalCountSettings)
	    {
		    XmlSerializer serializer = new XmlSerializer(typeof(PhysicalCountSettings));
		    bool bSaveComplete = false;

		    try {
			    // use relection to get the current assembly path for the database
			    string strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			    string strFullPathToMyFile = Path.Combine(strAppDir, m_strFileName);

			    // Create a FileStream to write with.
			    FileStream writer = new FileStream(strFullPathToMyFile, FileMode.Create);
			    // Serialize the object, and close the TextWriter
			    serializer.Serialize(writer, oPhysicalCountSettings);
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
