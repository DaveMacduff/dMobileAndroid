// <copyright file="PrinterSettings.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

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
    /// Printer Settings Class.
    /// </summary>
    [XmlInclude(typeof(PrinterInfo))]
    [Serializable]
    public class PrinterSettings : ISysSocketEventListener, IPrnSettingsPublisher
    {
        /// <summary>
        /// Contains full printer settings class name..
        /// </summary>
        [NonSerialized]
        private string fullClassName = "dMobile.Bedrock.PrinterSettings";

        /// <summary>
        /// Contains printer settings path name..
        /// </summary>
        [NonSerialized]
        private string filePath = string.Empty;

        /// <summary>
        /// Contains printer settings drive letter, path and file name..
        /// </summary>
        [NonSerialized]
        private string fileASCIIZ = string.Empty;

        /// <summary>
        /// Contains printer settings file name..
        /// </summary>
        [NonSerialized]
        private string fileName = "dMobilePrinterSettings.xml";

        /// <summary>
        /// Defines the writeLockObject.
        /// </summary>
        [NonSerialized]
        private readonly object writeLockObject = new object();

        /// <summary>
        /// Defines the readLockObject.
        /// </summary>
        [NonSerialized]
        private readonly object readLockObject = new object();

        /// <summary>
        /// Enables shipping label printer..
        /// </summary>
        private bool isEnabledShippingLabelPrinter = false;

        /// <summary>
        /// Contains a list of shipping label printer names..
        /// </summary>
        private string shippingLabelPrinterName = string.Empty;

        /// <summary>
        /// Enables shelf label printer..
        /// </summary>
        private bool isEnabledShelfLabelPrinter = false;

        /// <summary>
        /// Contains a list of shelf label printer names..
        /// </summary>
        private string shelfLabelPrinterName = string.Empty;

        /// <summary>
        /// Enables product label printer..
        /// </summary>
        private bool isEnabledProductLabelPrinter = false;

        /// <summary>
        /// Contains a list of product label printer names..
        /// </summary>
        private string productLabelPrinterName = string.Empty;

        /// <summary>
        /// arraylist of printer definitions of type printerInfo..
        /// </summary>
        private ArrayList allPrinters = new ArrayList();

        /// <summary>
        /// Initializes a new instance of the <see cref="PrinterSettings"/> class.
        /// </summary>
        public PrinterSettings()
        {
            this.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.FileASCIIZ = Path.Combine(this.FilePath, this.FileName);
        }

        /// <summary>
        /// The OnPrnSettingsStatusOutHandler.
        /// </summary>
        /// <param name="prnSettingsPublisher">The prnSettingsPublisher<see cref="IPrnSettingsPublisher"/>.</param>
        /// <param name="prnSettingsArgs">The prnSettingsArgs<see cref="PrnSettingsArgs"/>.</param>
        private delegate void OnPrnSettingsStatusOutHandler(IPrnSettingsPublisher prnSettingsPublisher, PrnSettingsArgs prnSettingsArgs);

        /// <summary>
        /// The OnPrnSettingsProgressOutHandler.
        /// </summary>
        /// <param name="prnSettingsPublisher">The prnSettingsPublisher<see cref="IPrnSettingsPublisher"/>.</param>
        /// <param name="prnSettingsArgs">The prnSettingsArgs<see cref="PrnSettingsArgs"/>.</param>
        private delegate void OnPrnSettingsProgressOutHandler(IPrnSettingsPublisher prnSettingsPublisher, PrnSettingsArgs prnSettingsArgs);

        /// <summary>
        /// Defines the OnPrnSettingsStatusOut.
        /// </summary>
        private event OnPrnSettingsStatusOutHandler OnPrnSettingsStatusOut;

        /// <summary>
        /// Defines the OnPrnSettingsProgressOut.
        /// </summary>
        private event OnPrnSettingsProgressOutHandler OnPrnSettingsProgressOut;

        /// <summary>
        /// Gets or sets the AllPrinters
        /// Gets or sets an arraylist of printer definitions of type printerInfo..
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

            set => this.allPrinters = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsEnabledShippingLabelPrinter
        /// Enables the shipping label printer..
        /// </summary>
        public bool IsEnabledShippingLabelPrinter
        {
            get => this.isEnabledShippingLabelPrinter;

            set => this.isEnabledShippingLabelPrinter = value;
        }

        /// <summary>
        /// Gets or sets the current shipping label printer name..
        /// </summary>
        public string ShippingLabelPrinterName
        {
            get => this.shippingLabelPrinterName;

            set => this.shippingLabelPrinterName = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsEnabledShelfLabelPrinter
        /// Enables the shelf label printer..
        /// </summary>
        public bool IsEnabledShelfLabelPrinter
        {
            get => this.isEnabledShelfLabelPrinter;

            set => this.isEnabledShelfLabelPrinter = value;
        }

        /// <summary>
        /// Gets or sets the current shelf label printer name..
        /// </summary>
        public string ShelfLabelPrinterName
        {
            get => this.shelfLabelPrinterName;

            set => this.shelfLabelPrinterName = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsEnabledProductLabelPrinter
        /// Enables the product label printer..
        /// </summary>
        public bool IsEnabledProductLabelPrinter
        {
            get => this.isEnabledProductLabelPrinter;

            set => this.isEnabledProductLabelPrinter = value;
        }

        /// <summary>
        /// Gets or sets the current product label printer name..
        /// </summary>
        public string ProductLabelPrinterName
        {
            get => this.productLabelPrinterName;

            set => this.productLabelPrinterName = value;
        }

        /// <summary>
        /// Gets or sets the class name to serialize..
        /// </summary>
        public string FullClassName
        {
            get => this.fullClassName;

            set => this.fullClassName = value;
        }

        /// <summary>
        /// Gets or sets the path name of the xml file..
        /// </summary>
        public string FilePath
        {
            get => this.filePath;

            set => this.filePath = value;
        }

        /// <summary>
        /// Gets or sets the full path and file name of the xml file..
        /// </summary>
        public string FileASCIIZ
        {
            get => this.fileASCIIZ;

            set => this.fileASCIIZ = value;
        }

        /// <summary>
        /// Gets or sets the file name of the xml file..
        /// </summary>
        public string FileName
        {
            get => this.fileName;

            set => this.fileName = value;
        }

        /// <summary>
        /// Request the printer settings data and save the file.
        /// </summary>
        public void RequestPrinterSettings()
        {
            var responseData = string.Empty;
            var msgArgs = new PrnSettingsArgs();

            try
            {
                msgArgs.StatusOutMsg = "Seeding printer settings.";
                this.OnPrnSettingsStatusOut(this, msgArgs);
                responseData = SysCommon.SubmitComm(this.Create());
                this.Save(responseData);

                msgArgs.StatusOutMsg = "Seeding printer settings complete.";
                this.OnPrnSettingsStatusOut(this, msgArgs);
            }
            catch (Exception ex)
            {
                msgArgs.StatusOutMsg = "Seeding printer settings failed.";
                this.OnPrnSettingsStatusOut(this, msgArgs);

                throw new Exception("Seeding printer settings failed.", ex);
            }
        }

        /// <summary>
        /// Load and object of type PrinterSettings from a serialized xml file.
        /// </summary>
        /// <returns>Object of type PrinterSettings.</returns>
        public PrinterSettings LoadData()
        {
            lock (this.readLockObject)
            {
                var serializer = new XmlSerializer(typeof(PrinterSettings));
                var prnSettings = new PrinterSettings();

                // use reflection to get the current assembly path for the database
                var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var fullPathToMyFile = Path.Combine(appDir, this.fileName);

                if (!File.Exists(fullPathToMyFile))
                {
                    this.SaveData(ref prnSettings);
                }

                // Create a FileStream to read with.
                using (var reader = new FileStream(fullPathToMyFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // Deserialize the stream to the object, and close the TextReader
                    prnSettings = (PrinterSettings)serializer.Deserialize(reader);
                }

                return prnSettings;
            }
        }

        /// <summary>
        /// Serialize and save the data into a xml file.
        /// </summary>
        /// <param name="prnSettings"> Instance of PrinterSettings to serialize.</param>
        /// <returns>True if save was successful else false.</returns>
        public bool SaveData(ref PrinterSettings prnSettings)
        {
            lock (this.writeLockObject)
            {
                var serializer = new XmlSerializer(typeof(PrinterSettings));
                var saveComplete = false;

                try
                {
                    // use reflection to get the current assembly path for the database
                    var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    var fullPathToMyFile = Path.Combine(appDir, this.fileName);

                    // Create a FileStream to write with.
                    using (var writer = new FileStream(fullPathToMyFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        serializer.Serialize(writer, prnSettings);
                    }

                    saveComplete = true;
                }
                catch
                {
                    saveComplete = false;
                }

                return saveComplete;
            }
        }

        /// <summary>
        /// The RegPrnSettingsListenerStatusOut.
        /// </summary>
        /// <param name="prnSettingsListener">.</param>
        public void RegPrnSettingsListenerStatusOut(IPrnSettingsListener prnSettingsListener)
        {
            this.OnPrnSettingsStatusOut += new OnPrnSettingsStatusOutHandler(prnSettingsListener.OnPrnSettingsStatusOut);
        }

        /// <summary>
        /// The UnregPrnSettingsListenerStatusOut.
        /// </summary>
        /// <param name="prnSettingsListener">.</param>
        public void UnregPrnSettingsListenerStatusOut(IPrnSettingsListener prnSettingsListener)
        {
            this.OnPrnSettingsStatusOut -= new OnPrnSettingsStatusOutHandler(prnSettingsListener.OnPrnSettingsStatusOut);
        }

        /// <summary>
        /// The RegPrnSettingsListenerProgressOut.
        /// </summary>
        /// <param name="prnSettingsListener">.</param>
        public void RegPrnSettingsListenerProgressOut(IPrnSettingsListener prnSettingsListener)
        {
            this.OnPrnSettingsProgressOut += new OnPrnSettingsProgressOutHandler(
                prnSettingsListener.OnPrnSettingsProgressOut);
        }

        /// <summary>
        /// The UnregPrnSettingsListenerProgressOut.
        /// </summary>
        /// <param name="prnSettingsListener">.</param>
        public void UnregPrnSettingsListenerProgressOut(IPrnSettingsListener prnSettingsListener)
        {
            this.OnPrnSettingsProgressOut -= new OnPrnSettingsProgressOutHandler(
                prnSettingsListener.OnPrnSettingsProgressOut);
        }

        /// <summary>
        /// The OnSysSocketStatusOut.
        /// </summary>
        /// <param name="sockPublisher">.</param>
        /// <param name="sockArgs">.</param>
        public void OnSysSocketStatusOut(ISysSocketEventPublisher sockPublisher, SysSocketEventArgs sockArgs)
        {
            var args = new PrnSettingsArgs(sockArgs.StatusOutMsg);
            this.OnPrnSettingsStatusOut(this, args);
        }

        /// <summary>
        /// Process the xml data from the communication server and store it.
        /// </summary>
        /// <param name="xmlData"> Xml data received from the communication server.</param>
        private void Save(string xmlData)
        {
            var msgArg = new PrnSettingsArgs();

            using var ds = new DataSet();
            if (this.OnPrnSettingsStatusOut != null)
            {
                msgArg.StatusOutMsg = "Reading received data.";
                this.OnPrnSettingsStatusOut(this, msgArg);
            }

            ds.ReadXml(SysCommon.GetXmlReader(ref xmlData), XmlReadMode.Auto);

            if (this.OnPrnSettingsStatusOut != null)
            {
                msgArg.StatusOutMsg = "Saving printer settings.";
                this.OnPrnSettingsStatusOut(this, msgArg);
            }

            if (ds.Tables.Count != 0)
            {
                var progMax = ds.Tables["tPrinterSettings"].Rows.Count;

                if (this.OnPrnSettingsStatusOut != null)
                {
                    msgArg.StatusOutMsg = $"Records {progMax}";
                    this.OnPrnSettingsStatusOut(this, msgArg);
                }

                if (this.OnPrnSettingsProgressOut != null)
                {
                    msgArg.CurrentValue = 0;
                    msgArg.MinValue = 0;
                    msgArg.MaxValue = progMax;
                    this.OnPrnSettingsProgressOut(this, msgArg);
                }

                var progValue = 0;
                var prnInfo = new PrinterInfo();

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
                msgArg.StatusOutMsg = "Saving printer settings complete.";
                this.OnPrnSettingsStatusOut(this, msgArg);
            }
        }

        /// <summary>
        /// Create the xml request string to send to the communication server.
        /// </summary>
        /// <returns>String of the xml request to get the printer settings.</returns>
        private string Create()
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);
            var revision = new DateTime(1900, 1, 1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var sysEnv = SysEnvironment.GetInstance();

            var request = new XDocument(
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

        /// <summary>
        /// Defines the <see cref="PrinterInfo" />.
        /// </summary>
        public struct PrinterInfo
        {
            /// <summary>
            /// Gets or sets the Name.
            /// </summary>
            public string Name { get; set; }
        }
    }
}
