namespace dMobile.Bedrock
{
  using System;
  using System.IO;
  using System.Reflection;
  using System.Xml.Serialization;

  [Serializable]
  public class ReturnsSettings
  {
    // VARIABLES
    [NonSerialized()]
    private string m_strFullClassName = "dMobile.ReturnsSettings";
    [NonSerialized()]
    private string m_strFilePath = string.Empty;
    [NonSerialized()]
    private string m_strFileASCIIZ = string.Empty;
    [NonSerialized()]
    private string m_strFileName = "dMobileReturnsSettings.xml";
    private Int32 m_iSortBy1Index = 0;
    private string m_strSortBy1Value = "ActivityReturnsFKRefno ASC";
    private Int32 m_iSortBy2Index = -1;
    private string m_strSortBy2Value = string.Empty;
    private Int32 m_iSortBy3Index = -1;
    private string m_strSortBy3Value = string.Empty;
    private DateTime enteredDateFilter = new DateTime(1900, 1, 1);
    private DateTime returnedDateFilter = new DateTime(1900, 1, 1);
    private bool isApplyEnteredDateFilter = false;
    private bool isApplyReturnedDateFilter = false;

    // PROPERTIES
    public bool IsApplyEnteredDateFilter
    {
      get { return isApplyEnteredDateFilter; }
      set { isApplyEnteredDateFilter = value; }
    }
    public bool IsApplyReturnedDateFilter
    {
      get { return isApplyReturnedDateFilter; }
      set { isApplyReturnedDateFilter = value; }
    }
    public DateTime EnteredDateFilter
    {
      get { return enteredDateFilter; }
      set { enteredDateFilter = value; }
    }
    public DateTime ReturnedDateFilter
    {
      get { return returnedDateFilter; }
      set { returnedDateFilter = value; }
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

    //METHODS

    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public ReturnsSettings()
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

      if (!string.IsNullOrEmpty(strSortBy2Value))
      {
        strSortBy = strSortBy + ", " + strSortBy2Value;
      }
      if (!string.IsNullOrEmpty(strSortBy3Value))
      {
        strSortBy = strSortBy + ", " + strSortBy3Value;
      }
      return strSortBy;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public ReturnsSettings LoadData()
    {
      XmlSerializer serializer = new XmlSerializer(typeof(ReturnsSettings));
      ReturnsSettings oReturnsSettings = new ReturnsSettings();

      // use relection to get the current assembly path for the database
      string strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
      string strFullPathToMyFile = Path.Combine(strAppDir, m_strFileName);

      if (!File.Exists(strFullPathToMyFile))
      {
        SaveData(ref oReturnsSettings);
      }

      // Create a FileStream to read with.
      FileStream reader = new FileStream(strFullPathToMyFile, FileMode.Open);
      // Deserialize the stream to the object, and close the TextReader
      oReturnsSettings = (ReturnsSettings)serializer.Deserialize(reader);
      reader.Close();

      return oReturnsSettings;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="oReturnsSettings"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public bool SaveData(ref ReturnsSettings oReturnsSettings)
    {
      XmlSerializer serializer = new XmlSerializer(typeof(ReturnsSettings));
      bool bSaveComplete = false;

      try
      {
        // use relection to get the current assembly path for the database
        string strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string strFullPathToMyFile = Path.Combine(strAppDir, m_strFileName);

        // Create a FileStream to write with.
        FileStream writer = new FileStream(strFullPathToMyFile, FileMode.Create);
        // Serialize the object, and close the TextWriter
        serializer.Serialize(writer, oReturnsSettings);
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
