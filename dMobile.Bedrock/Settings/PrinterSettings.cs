namespace dMobile.Bedrock
{
    using System;
    using System.Collections;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Printer Settings Class
    /// </summary>
    [XmlInclude(typeof(PrinterInfo))]
    [Serializable]
    public class PrinterSettings : ISysSocketEventListener, IPrnSettingsPublisher
    {
        #region Fields

        /// <summary>
        /// Contains full printer settings class name.
        /// </summary>
        [NonSerialized]
        private string fullClassName = "dMobile.Bedrock.PrinterSettings";

        /// <summary>
        /// Contains printer settings path name.
        /// </summary>
        [NonSerialized]
        private string filePath = string.Empty;

        /// <summary>
        /// Contains printer settings drive letter, path and file name.
        /// </summary>
        [NonSerialized]
        private string fileASCIIZ = string.Empty;

        /// <summary>
        /// Contains printer settings file name.
        /// </summary>
        [NonSerialized]
        private string fileName = "dMobilePrinterSettings.xml";

        [NonSerialized]
        private static readonly object lockObject = new object();

        /// <summary>
        /// Enables shipping label printer.
        /// </summary>
        private bool isEnabledShippingLabelPrinter = false;

        /// <summary>
        /// Contains a list of shipping label printer names.
        /// </summary>
        private string shippingLabelPrinterName = string.Empty;

        /// <summary>
        /// Enables shelf label printer.
        /// </summary>
        private bool isEnabledShelfLabelPrinter = false;

        /// <summary>
        /// Contains a list of shelf label printer names.
        /// </summary>
        private string shelfLabelPrinterName = string.Empty;

        /// <summary>
        /// Enables product label printer.
        /// </summary>
        private bool isEnabledProductLabelPrinter = false;

        /// <summary>
        /// Contains a list of product label printer names.
        /// </summary>
        private string productLabelPrinterName = string.Empty;

        /// <summary>
        /// arraylist of printer definitions of type printerInfo
        /// </summary>
        private ArrayList allPrinters = new ArrayList();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrinterSettings"/> class.
        /// </summary>
        public PrinterSettings()
        {
            this.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.FileASCIIZ = Path.Combine(this.FilePath, this.FileName);
        }

        #endregion

        #region Delegates

        private delegate void OnPrnSettingsStatusOutHandler(IPrnSettingsPublisher prnSettingsPublisher, PrnSettingsArgs prnSettingsArgs);
        private delegate void OnPrnSettingsProgressOutHandler(IPrnSettingsPublisher prnSettingsPublisher, PrnSettingsArgs prnSettingsArgs);

        #endregion

        #region Events

        private event OnPrnSettingsStatusOutHandler OnPrnSettingsStatusOut;
        private event OnPrnSettingsProgressOutHandler OnPrnSettingsProgressOut;

        #endregion

        #region Properties

        /// <summary>
        ///  Gets or sets an arraylist of printer definitions of type printerInfo
        /// </summary>
        public ArrayList AllPrinters
        {
            get
            {
                if (this.allPrinters == null)
                {
                    this.allPrinters = new ArrayList();
                }

                return this.allPrinters;
            }

            set
            {
                this.allPrinters = value;
            }
        }

        /// <summary>
        /// Enables the shipping label printer
        /// </summary>
        public bool IsEnabledShippingLabelPrinter
        {
            get
            {
                return this.isEnabledShippingLabelPrinter;
            }

            set
            {
                this.isEnabledShippingLabelPrinter = value;
            }
        }

        /// <summary>
        /// Gets or sets the current shipping label printer name.
        /// </summary>
        public string ShippingLabelPrinterName
        {
            get
            {
                return this.shippingLabelPrinterName;
            }

            set
            {
                this.shippingLabelPrinterName = value;
            }
        }

        /// <summary>
        /// Enables the shelf label printer
        /// </summary>
        public bool IsEnabledShelfLabelPrinter
        {
            get
            {
                return this.isEnabledShelfLabelPrinter;
            }

            set
            {
                this.isEnabledShelfLabelPrinter = value;
            }
        }

        /// <summary>
        /// Gets or sets the current shelf label printer name.
        /// </summary>
        public string ShelfLabelPrinterName
        {
            get
            {
                return this.shelfLabelPrinterName;
            }

            set
            {
                this.shelfLabelPrinterName = value;
            }
        }

        /// <summary>
        /// Enables the product label printer
        /// </summary>
        public bool IsEnabledProductLabelPrinter
        {
            get
            {
                return this.isEnabledProductLabelPrinter;
            }

            set
            {
                this.isEnabledProductLabelPrinter = value;
            }
        }

        /// <summary>
        /// Gets or sets the current product label printer name.
        /// </summary>
        public string ProductLabelPrinterName
        {
            get
            {
                return this.productLabelPrinterName;
            }

            set
            {
                this.productLabelPrinterName = value;
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
        /// Request the printer settings data and save the file.
        /// </summary>
        public void RequestPrinterSettings()
        {
            string reqData = string.Empty;
            PrnSettingsArgs msgArgs = new PrnSettingsArgs();

            try
            {
                // TODO: Resource strings
                msgArgs.StatusOutMsg = "Seeding printer settings.";
                this.OnPrnSettingsStatusOut(this, msgArgs);

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

                // TODO: Resource strings
                msgArgs.StatusOutMsg = "Seeding printer settings complete.";
                this.OnPrnSettingsStatusOut(this, msgArgs);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                msgArgs.StatusOutMsg = "Seeding printer settings failed.";
                this.OnPrnSettingsStatusOut(this, msgArgs);

                // TODO: Resource strings
                throw new Exception("Seeding printer settings failed.", ex);
            }
        }

        /// <summary>
        /// Load and object of type PrinterSettings from a serialized xml file.
        /// </summary>
        /// <returns>Object of type PrinterSettings</returns>
        public PrinterSettings LoadData()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PrinterSettings));
            PrinterSettings prnSettings = new PrinterSettings();

            // use relection to get the current assembly path for the database
            string appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string fullPathToMyFile = Path.Combine(appDir, this.fileName);

            if (!File.Exists(fullPathToMyFile))
            {
                this.SaveData(ref prnSettings);
            }

            // Create a FileStream to read with.
            using (FileStream reader = new FileStream(fullPathToMyFile, FileMode.Open))
            {
                // Deserialize the stream to the object, and close the TextReader
                prnSettings = (PrinterSettings)serializer.Deserialize(reader);
            }

            return prnSettings;
        }

        /// <summary>
        /// Serialize and save the data into a xml file.
        /// </summary>
        /// <param name="prnSettings"> Instance of PrinterSettings to serialize.</param>
        /// <returns>True if save was successful else false</returns>
        public bool SaveData(ref PrinterSettings prnSettings)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PrinterSettings));
            bool saveComplete = false;

            lock (lockObject)
            {
                try
                {
                    // use relection to get the current assembly path for the database
                    string appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    string fullPathToMyFile = Path.Combine(appDir, this.fileName);

                    // Create a FileStream to write with.
                    using (FileStream writer = new FileStream(fullPathToMyFile, FileMode.Create))
                    {
                        serializer.Serialize(writer, prnSettings);
                    }

                    saveComplete = true;
                }
                catch
                {
                    saveComplete = false;
                }
            }

            return saveComplete;
        }

        #region IPrinterSettingsEventPublisher Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prnSettingsListener"></param>
        public void RegPrnSettingsListenerStatusOut(IPrnSettingsListener prnSettingsListener)
        {
            this.OnPrnSettingsStatusOut += new OnPrnSettingsStatusOutHandler(prnSettingsListener.OnPrnSettingsStatusOut);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prnSettingsListener"></param>
        public void UnregPrnSettingsListenerStatusOut(IPrnSettingsListener prnSettingsListener)
        {
            this.OnPrnSettingsStatusOut -= new OnPrnSettingsStatusOutHandler(prnSettingsListener.OnPrnSettingsStatusOut);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prnSettingsListener"></param>
        public void RegPrnSettingsListenerProgressOut(IPrnSettingsListener prnSettingsListener)
        {
            this.OnPrnSettingsProgressOut += new OnPrnSettingsProgressOutHandler(
                prnSettingsListener.OnPrnSettingsProgressOut);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prnSettingsListener"></param>
        public void UnregPrnSettingsListenerProgressOut(IPrnSettingsListener prnSettingsListener)
        {
            this.OnPrnSettingsProgressOut -= new OnPrnSettingsProgressOutHandler(
                prnSettingsListener.OnPrnSettingsProgressOut);
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
            PrnSettingsArgs args = new PrnSettingsArgs(sockArgs.StatusOutMsg);
            this.OnPrnSettingsStatusOut(this, args);
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
            PrnSettingsArgs msgArg = new PrnSettingsArgs();

            using (DataSet ds = new DataSet())
            {
                if (this.OnPrnSettingsStatusOut != null)
                {
                    // TODO: Resource strings
                    msgArg.StatusOutMsg = "Reading received data.";
                    this.OnPrnSettingsStatusOut(this, msgArg);
                }

                ds.ReadXml(SysCommon.GetXmlReader(ref xmlData), XmlReadMode.Auto);

                if (this.OnPrnSettingsStatusOut != null)
                {
                    // TODO: Resource strings
                    msgArg.StatusOutMsg = "Saving printer settings.";
                    this.OnPrnSettingsStatusOut(this, msgArg);
                }

                if (ds.Tables.Count != 0)
                {
                    int progMax = ds.Tables["tPrinterSettings"].Rows.Count;

                    if (this.OnPrnSettingsStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgArg.StatusOutMsg = string.Format("Records {0}", progMax);
                        this.OnPrnSettingsStatusOut(this, msgArg);
                    }

                    if (this.OnPrnSettingsProgressOut != null)
                    {
                        msgArg.CurrentValue = 0;
                        msgArg.MinValue = 0;
                        msgArg.MaxValue = progMax;
                        this.OnPrnSettingsProgressOut(this, msgArg);
                    }

                    int progValue = 0;
                    PrinterInfo prnInfo = new PrinterInfo();

                    foreach (DataRow drow in ds.Tables["tPrinterSettings"].Rows)
                    {
                        prnInfo.Name = Convert.ToString(drow["PrinterSettingsPrinterName"]);
                        this.AllPrinters.Add(prnInfo);

                        progValue += 1;

                        if (this.OnPrnSettingsProgressOut != null)
                        {
                            msgArg.CurrentValue = progValue;
                            msgArg.MinValue = 0;
                            msgArg.MaxValue = progMax;
                            this.OnPrnSettingsProgressOut(this, msgArg);
                        }
                    }
                }

                if (this.OnPrnSettingsStatusOut != null)
                {
                    // TODO: Resource strings
                    msgArg.StatusOutMsg = "Saving printer settings complete.";
                    this.OnPrnSettingsStatusOut(this, msgArg);
                }
            }
        }

        /// <summary>
        /// Create the xml request string to send to the communication server.
        /// </summary>
        /// <returns>String of the xml request to get the printer settings</returns>
        private string Create()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            string revision = new DateTime(1900, 1, 1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            SysEnvironment sysEnv = SysEnvironment.GetInstance();

            XDocument request = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                string.Empty,
                new XElement(
                    "imclient",
                    new XElement(
                        "command",
                        new XElement("action", "REQUEST PRINTER SETTINGS"),
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
        public struct PrinterInfo
        {
            /// <summary>
            /// 
            /// </summary>
            private string name;

            /// <summary>
            /// 
            /// </summary>
            public string Name
            {
                get
                {
                    return this.name;
                }

                set
                {
                    this.name = value;
                }
            }
        }

        #endregion
    }
}
