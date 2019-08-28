namespace dMobile.Bedrock
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Data;
  
  using System.IO;
  using System.Net.Sockets;
  using System.Reflection;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using System.Xml.Serialization;
  using Microsoft.VisualBasic;

  /// <summary>
  /// Printer Settings Class
  /// </summary>
  [XmlInclude(typeof(ProdSchemeLabelInfo))]
  [Serializable]
  public class ProdSchemeLabelSettings : ISysSocketEventListener, IProdSchemeLabelSettingsPublisher
  {
    #region Fields

    /// <summary>
    /// Contains full product scheme label settings class name.
    /// </summary>
    [NonSerialized]
    private string fullClassName = "dMobile.Bedrock.ProductSchemeLabelSettings";

    /// <summary>
    /// Contains product scheme label settings path name.
    /// </summary>
    [NonSerialized]
    private string filePath = string.Empty;

    /// <summary>
    /// Contains product scheme label settings drive letter, path and file name.
    /// </summary>
    [NonSerialized]
    private string fileASCIIZ = string.Empty;

    /// <summary>
    /// Contains product scheme label settings file name.
    /// </summary>
    [NonSerialized]
    private string fileName = "dMobileProductSchemeLabelSettings.xml";

    /// <summary>
    /// arraylist of product scheme label definitions of type SchemeInfo
    /// </summary>
    private ArrayList allProdSchemeLabel = new ArrayList();

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ProdSchemelabelSettings"/> class.
    /// </summary>
    public ProdSchemeLabelSettings()
    {
      this.FilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\";
      this.FileASCIIZ = Path.Combine(this.FilePath, this.FileName);
    }

    #endregion

    #region Delegates

    private delegate void OnProdSchemeLabelSettingsStatusOutHandler(
        IProdSchemeLabelSettingsPublisher prodSchemeLabelSettingsPublisher,
        ProdSchemeLabelSettingsEventArgs prodSchemeLabelSettingsArgs);

    private delegate void OnProdSchemeLabelSettingsProgressOutHandler(
        IProdSchemeLabelSettingsPublisher prodSchemeLabelSettingsPublisher,
        ProdSchemeLabelSettingsEventArgs prodSchemeLabelSettingsArgs);

    #endregion

    #region Events

    private event OnProdSchemeLabelSettingsStatusOutHandler OnProdSchemeLabelSettingsStatusOut;

    private event OnProdSchemeLabelSettingsProgressOutHandler OnProdSchemeLabelSettingsProgressOut;

    #endregion

    #region Properties

    /// <summary>
    ///  Gets or sets an arraylist of printer definitions of type prodSchemeLabelInfo
    /// </summary>
    public ArrayList AllProdSchemeLabel
    {
      get
      {
        if (this.allProdSchemeLabel == null)
        {
          this.allProdSchemeLabel = new ArrayList();
        }

        return this.allProdSchemeLabel;
      }

      set
      {
        this.allProdSchemeLabel = value;
      }
    }

    /// <summary>
    /// Gets or sets the class name to serialize.
    /// </summary>
    public string FullClassName
    {
      get
      {
        return this.fullClassName;
      }

      set
      {
        this.fullClassName = value;
      }
    }

    /// <summary>
    /// Gets or sets the path name of the xml file.
    /// </summary>
    public string FilePath
    {
      get
      {
        return this.filePath;
      }

      set
      {
        this.filePath = value;
      }
    }

    /// <summary>
    /// Gets or sets the full path and file name of the xml file.
    /// </summary>
    /// <remarks>
    /// Includes the drive letter, path and file name.
    /// </remarks>
    public string FileASCIIZ
    {
      get
      {
        return this.fileASCIIZ;
      }

      set
      {
        this.fileASCIIZ = value;
      }
    }

    /// <summary>
    /// Gets or sets the file name of the xml file. 
    /// </summary>
    /// <remarks>
    /// Does not include any path information.
    /// </remarks>
    public string FileName
    {
      get
      {
        return this.fileName;
      }

      set
      {
        this.fileName = value;
      }
    }

    #endregion

    #region Public Members

    /// <summary>
    /// Request the product scheme label settings data and save the file.
    /// </summary>
    public void RequestProductSchemeLabelSettings()
    {
      string reqData = string.Empty;
      ProdSchemeLabelSettingsEventArgs msgArgs = new ProdSchemeLabelSettingsEventArgs();

      try
      {
#if MULTILANGUAGE
        msgArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeLabelSettings);
#else
        msgArgs.StatusOutMsg = "Seeding product scheme label settings.";
#endif
        this.OnProdSchemeLabelSettingsStatusOut(this, msgArgs);

        reqData = SysCommon.SubmitComm(this.Create());
        if (reqData.Length > 0)
        {
          if (reqData.Contains("<message>ERROR</message>"))
          {
            XmlDocument docError = new XmlDocument();
            docError.LoadXml(reqData);
            throw new Exception(docError.DocumentElement["detail"].InnerText);
          }

          this.Save(reqData);
        }

#if MULTILANGUAGE
        msgArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeLabelSettingsComplete);
#else
        msgArgs.StatusOutMsg = "Seeding product scheme label settings complete.";
#endif
        this.OnProdSchemeLabelSettingsStatusOut(this, msgArgs);
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        msgArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeLabelSettingsFailed);
#else
        msgArgs.StatusOutMsg = "Seeding product scheme label settings failed.";
#endif
        this.OnProdSchemeLabelSettingsStatusOut(this, msgArgs);

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductSchemeLabelSettingsFailed), ex);
#else
        throw new Exception("Seeding product scheme label settings failed.", ex);
#endif
      }
    }

    /// <summary>
    /// Load and object of type ProductSchemeLabelSettings from a serialized xml file.
    /// </summary>
    /// <returns>Object of type ProductSchemeLabelSettings</returns>
    public ProdSchemeLabelSettings LoadData()
    {
      XmlSerializer serializer = new XmlSerializer(typeof(ProdSchemeLabelSettings));
      ProdSchemeLabelSettings settings = new ProdSchemeLabelSettings();

      // use relection to get the current assembly path for the database
      string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
      string fullPathToMyFile = Path.Combine(appDir, this.fileName);

      if (!File.Exists(fullPathToMyFile))
      {
        this.SaveData(ref settings);
      }

      // Create a FileStream to read with.
      FileStream reader = new FileStream(fullPathToMyFile, FileMode.Open);

      // Deserialize the stream to the object, and close the TextReader
      settings = (ProdSchemeLabelSettings)serializer.Deserialize(reader);
      reader.Close();

      return settings;
    }

    /// <summary>
    /// Serialize and save the data into a xml file.
    /// </summary>
    /// <param name="settings"> Instance of ProductSchemeLabelSettings to serialize.</param>
    /// <returns>True if save was successful else false</returns>
    public bool SaveData(ref ProdSchemeLabelSettings settings)
    {
      XmlSerializer serializer = new XmlSerializer(typeof(ProdSchemeLabelSettings));
      bool saveComplete = false;

      try
      {
        // use relection to get the current assembly path for the database
        string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
        string fullPathToMyFile = Path.Combine(appDir, this.fileName);

        // Create a FileStream to write with.
        FileStream writer = new FileStream(fullPathToMyFile, FileMode.Create);

        // Serialize the object, and close the TextWriter
        serializer.Serialize(writer, settings);
        writer.Close();

        saveComplete = true;
      }
      catch
      {
        saveComplete = false;
      }

      return saveComplete;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="schemeRefno"></param>
    /// <returns></returns>
    public ProdSchemeLabelInfo GetSchemeInfoFromSchemeRefno(int schemeRefno)
    {
      ProdSchemeLabelInfo schemeLblInfo = new ProdSchemeLabelInfo();

      if (0 < this.AllProdSchemeLabel.Count)
      {
        for (int idx = 0; idx < this.AllProdSchemeLabel.Count; idx++)
        {
          if (((ProdSchemeLabelInfo)this.AllProdSchemeLabel[idx]).SchemeRefno == schemeRefno)
          {
            schemeLblInfo = (ProdSchemeLabelInfo)this.AllProdSchemeLabel[idx];
          }
        }
      }

      if (schemeLblInfo.SchemeRefno == 0)
      {
        throw new Exception(
            string.Format(
            "GetSchemeInfoFromSchemeRefno failed. Could not find scheme refno {0} in the AllProdSchemeLabel arraylist.", 
            schemeRefno));
      }

      return schemeLblInfo;
    }

    #region IProdSchemeLabelSettingsEventPublisher Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prodSchemeLabelSettingsListener"></param>
    public void RegProdSchemeLabelSettingsListenerStatusOut(
        IProdSchemeLabelSettingsListener prodSchemeLabelSettingsListener)
    {
      this.OnProdSchemeLabelSettingsStatusOut += new OnProdSchemeLabelSettingsStatusOutHandler(
          prodSchemeLabelSettingsListener.OnProdSchemeLabelSettingsStatusOut);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prodSchemeLabelSettingsListener"></param>
    public void UnregProdSchemeLabelSettingsListenerStatusOut(
        IProdSchemeLabelSettingsListener prodSchemeLabelSettingsListener)
    {
      this.OnProdSchemeLabelSettingsStatusOut -= new OnProdSchemeLabelSettingsStatusOutHandler(
          prodSchemeLabelSettingsListener.OnProdSchemeLabelSettingsStatusOut);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prodSchemeLabelSettingsListener"></param>
    public void RegProdSchemeLabelSettingsListenerProgressOut(
        IProdSchemeLabelSettingsListener prodSchemeLabelSettingsListener)
    {
      this.OnProdSchemeLabelSettingsProgressOut += new OnProdSchemeLabelSettingsProgressOutHandler(
          prodSchemeLabelSettingsListener.OnProdSchemeLabelSettingsProgressOut);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prodSchemeLabelSettingsListener"></param>
    public void UnregProdSchemeLabelSettingsListenerProgressOut(
        IProdSchemeLabelSettingsListener prodSchemeLabelSettingsListener)
    {
      this.OnProdSchemeLabelSettingsProgressOut -= new OnProdSchemeLabelSettingsProgressOutHandler(
          prodSchemeLabelSettingsListener.OnProdSchemeLabelSettingsProgressOut);
    }

    #endregion

    #region ISysSocketEventListener Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sockPublisher"></param>
    /// <param name="sockArgs"></param>
    public void OnSysSocketStatusOut(ISysSocketEventPublisher sockPublisher, SysSocketEventArgs sockArgs)
    {
      ProdSchemeLabelSettingsEventArgs args = new ProdSchemeLabelSettingsEventArgs(sockArgs.StatusOutMsg);
      this.OnProdSchemeLabelSettingsStatusOut(this, args);
    }

    #endregion

    #endregion

    #region Private Members

    /// <summary>
    /// Process the xml data from the communication server and store it.
    /// </summary>
    /// <param name="xmlData"> Xml data received from the communication server</param>
    private void Save(string xmlData)
    {
      ProdSchemeLabelSettingsEventArgs msgArg = new ProdSchemeLabelSettingsEventArgs();
      string schemeCode = string.Empty;
      string errMsg = string.Empty;

      try
      {
        using (DataSet ds = new DataSet())
        {
          if (this.OnProdSchemeLabelSettingsStatusOut != null)
          {
#if MULTILANGUAGE
            msgArg.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.ReadingReceivedData);
#else
            msgArg.StatusOutMsg = "Reading received data.";
#endif
            this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
          }

          ds.ReadXml(SysCommon.GetXmlReader(ref xmlData), XmlReadMode.Auto);

          if (this.OnProdSchemeLabelSettingsStatusOut != null)
          {
#if MULTILANGUAGE
            msgArg.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductSchemeLabelSettings);
#else
            msgArg.StatusOutMsg = "Saving product scheme label settings.";
#endif
            this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
          }

          if (ds.Tables.Count != 0)
          {
            int progMax = ds.Tables["seg"].Rows.Count;
            if (this.OnProdSchemeLabelSettingsStatusOut != null)
            {
#if MULTILANGUAGE
              msgArg.StatusOutMsg = string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax);
#else
              msgArg.StatusOutMsg = string.Format("Records {0}", progMax);
#endif
              this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
            }

            if (this.OnProdSchemeLabelSettingsProgressOut != null)
            {
              msgArg.CurrentValue = 0;
              msgArg.MinValue = 0;
              msgArg.MaxValue = progMax;
              this.OnProdSchemeLabelSettingsProgressOut(this, msgArg);
            }
             
            using (SqlCeCommand cmndScheme = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
              cmndScheme.CommandText = "tProductScheme";
              cmndScheme.CommandType = CommandType.TableDirect;
              cmndScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";

              using (SqlCeDataReader drScheme = cmndScheme.ExecuteReader())
              {
                this.AllProdSchemeLabel.Clear();
                int progValue = 0;
                ProdSchemeLabelInfo schemeLblInfo = new ProdSchemeLabelInfo();
                foreach (DataRow drow in ds.Tables["seg"].Rows)
                {
                  schemeCode = Convert.ToString(drow["ProductSchemeCode"]);
                  errMsg = "Scheme code \'" + errMsg + "\'";
                  if (!drScheme.Seek(DbSeekOptions.FirstEqual, new object[] { schemeCode }))
                  {
                    throw new Exception("Unable to locate the product scheme code in tProductScheme." 
                        + Environment.NewLine + errMsg);
                  }

                  if (!drScheme.Read())
                  {
                    throw new Exception("drProdScheme.Read() failed." + Environment.NewLine + errMsg);
                  }

                  schemeLblInfo.SchemeRefno = Convert.ToInt32(drScheme.GetValue((int)SysCommon.ProdSchemeFields.Refno));
                  schemeLblInfo.IsBestBeforeDateTracked = Convert.ToBoolean(
                      drow["ProductSchemeLabelIsBestBeforeDateTracked"]);
                  schemeLblInfo.IsLotTracked = Convert.ToBoolean(drow["ProductSchemeLabelIsLotTracked"]);
                  schemeLblInfo.IsProductionDateTracked = Convert.ToBoolean(
                      drow["ProductSchemeLabelIsProductionDateTracked"]);
                  schemeLblInfo.IsSerialTracked = Convert.ToBoolean(drow["ProductSchemeLabelIsSerialTracked"]);
                  schemeLblInfo.IsVariantTracked = Convert.ToBoolean(drow["ProductSchemeLabelIsVariantTracked"]);

                  this.AllProdSchemeLabel.Add(schemeLblInfo);

                  progValue = progValue + 1;
                  if (this.OnProdSchemeLabelSettingsProgressOut != null)
                  {
                    msgArg.CurrentValue = progValue;
                    msgArg.MinValue = 0;
                    msgArg.MaxValue = progMax;
                    this.OnProdSchemeLabelSettingsProgressOut(this, msgArg);
                  }
                }

                if (this.OnProdSchemeLabelSettingsProgressOut != null)
                {
                  msgArg.CurrentValue = progMax;
                  msgArg.MinValue = 0;
                  msgArg.MaxValue = progMax;
                  this.OnProdSchemeLabelSettingsProgressOut(this, msgArg);
                }
              }
            }
          }

          if (this.OnProdSchemeLabelSettingsStatusOut != null)
          {
#if MULTILANGUAGE
            msgArg.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductSchemeLabelSettingsComplete);
#else
            msgArg.StatusOutMsg = "Saving product scheme label settings complete.";
#endif
            this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
          }
        }
      }
      catch (SqlCeException ex)
      {
        if (this.OnProdSchemeLabelSettingsStatusOut != null)
        {
#if MULTILANGUAGE
          msgArg.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred);
#else
          msgArg.StatusOutMsg = "Database exception occurred.";
#endif
          this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
        }

        if (this.OnProdSchemeLabelSettingsStatusOut != null)
        {
#if MULTILANGUAGE
          msgArg.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductSchemeLabelSettingsFailed);
#else
          msgArg.StatusOutMsg = "Saving product scheme label settings failed.";
#endif
          this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
        }

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingProductSchemeLabelSettingsFailed), ex);
#else
        throw new Exception("Database exception occurred. Saving product scheme label settings failed.", ex);
#endif
      }
    }

    /// <summary>
    /// Create the xml request string to send to the communication server.
    /// </summary>
    /// <returns>String of the xml request to get the product scheme label settings</returns>
    private string Create()
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      // Keep the revision date to empty 1/1/1900.
      // I had to do this because on a partial refresh we do not maintain the prior xml data.
      // The structure is empty.
      string revision = new DateTime(1900, 1, 1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
      SysEnvironment sysEnv = SysEnvironment.GetInstance();

      XDocument request = new XDocument(
          new XDeclaration("1.0", "utf-8", "yes"),
          string.Empty,
          new XElement(
              "imclient",
              new XElement(
                  "command",
                  new XElement("action", "REQUEST PRODUCT SCHEME LABEL SETTINGS"),
                  new XElement("device_id", sysEnv.DeviceID),
                  new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                  new XElement("revision_datetime", revision))));

      request.Save(xml);
      xml.Flush();
      xml.Close();

      return xmlString.ToString();
    }

    #endregion

    #region Structures

    /// <summary>
    /// 
    /// </summary>
    public struct ProdSchemeLabelInfo
    {
      private int schemeRefno;
      private bool isSerialTracked;
      private bool isProductionDateTracked;
      private bool isLotTracked;
      private bool isBestBeforeDateTracked;
      private bool isVariantTracked;

      public int SchemeRefno
      {
        get
        {
          return this.schemeRefno;
        }

        set
        {
          this.schemeRefno = value;
        }
      }

      public bool IsVariantTracked
      {
        get
        {
          return this.isVariantTracked;
        }

        set
        {
          this.isVariantTracked = value;
        }
      }

      public bool IsBestBeforeDateTracked
      {
        get
        {
          return this.isBestBeforeDateTracked;
        }

        set
        {
          this.isBestBeforeDateTracked = value;
        }
      }

      public bool IsSerialTracked
      {
        get
        {
          return this.isSerialTracked;
        }

        set
        {
          this.isSerialTracked = value;
        }
      }

      public bool IsProductionDateTracked
      {
        get
        {
          return this.isProductionDateTracked;
        }

        set
        {
          this.isProductionDateTracked = value;
        }
      }

      public bool IsLotTracked
      {
        get
        {
          return this.isLotTracked;
        }

        set
        {
          this.isLotTracked = value;
        }
      }
    }

    #endregion
  }
}
