namespace dMobile.Bedrock
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.SQLite;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Printer Settings Class
    /// </summary>
    [XmlInclude(typeof(ProdSchemeLabelInfo))]
    [Serializable]
    public class ProdSchemeLabelSettings : ISysSocketEventListener, IProdSchemeLabelSettingsPublisher
    {
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ProdSchemeLabelSettings"/> class.
        /// </summary>
        public ProdSchemeLabelSettings()
        {
            this.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.FileASCIIZ = Path.Combine(this.FilePath, this.FileName);
        }

        /// <summary>
        /// The OnProdSchemeLabelSettingsStatusOutHandler
        /// </summary>
        /// <param name="prodSchemeLabelSettingsPublisher">The prodSchemeLabelSettingsPublisher<see cref="IProdSchemeLabelSettingsPublisher"/></param>
        /// <param name="prodSchemeLabelSettingsArgs">The prodSchemeLabelSettingsArgs<see cref="ProdSchemeLabelSettingsEventArgs"/></param>
        private delegate void OnProdSchemeLabelSettingsStatusOutHandler(
        IProdSchemeLabelSettingsPublisher prodSchemeLabelSettingsPublisher,
        ProdSchemeLabelSettingsEventArgs prodSchemeLabelSettingsArgs);

        /// <summary>
        /// The OnProdSchemeLabelSettingsProgressOutHandler
        /// </summary>
        /// <param name="prodSchemeLabelSettingsPublisher">The prodSchemeLabelSettingsPublisher<see cref="IProdSchemeLabelSettingsPublisher"/></param>
        /// <param name="prodSchemeLabelSettingsArgs">The prodSchemeLabelSettingsArgs<see cref="ProdSchemeLabelSettingsEventArgs"/></param>
        private delegate void OnProdSchemeLabelSettingsProgressOutHandler(
        IProdSchemeLabelSettingsPublisher prodSchemeLabelSettingsPublisher,
        ProdSchemeLabelSettingsEventArgs prodSchemeLabelSettingsArgs);

        /// <summary>
        /// Defines the OnProdSchemeLabelSettingsStatusOut
        /// </summary>
        private event OnProdSchemeLabelSettingsStatusOutHandler OnProdSchemeLabelSettingsStatusOut;

        /// <summary>
        /// Defines the OnProdSchemeLabelSettingsProgressOut
        /// </summary>
        private event OnProdSchemeLabelSettingsProgressOutHandler OnProdSchemeLabelSettingsProgressOut;

        /// <summary>
        /// Gets or sets the AllProdSchemeLabel
        /// Gets or sets an arraylist of printer definitions of type prodSchemeLabelInfo
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

        /// <summary>
        /// Request the product scheme label settings data and save the file.
        /// </summary>
        public void RequestProductSchemeLabelSettings()
        {
            string reqData = string.Empty;
            ProdSchemeLabelSettingsEventArgs msgArgs = new ProdSchemeLabelSettingsEventArgs();

            try
            {
                // TODO: Resource String
                msgArgs.StatusOutMsg = "Seeding product scheme label settings.";

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

                // TODO: Resource String
                msgArgs.StatusOutMsg = "Seeding product scheme label settings complete.";

                this.OnProdSchemeLabelSettingsStatusOut(this, msgArgs);
            }
            catch (Exception ex)
            {
                // TODO: Resource String
                msgArgs.StatusOutMsg = "Seeding product scheme label settings failed.";

                this.OnProdSchemeLabelSettingsStatusOut(this, msgArgs);

                // TODO: Resource String
                throw new Exception("Seeding product scheme label settings failed.", ex);
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
            string appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
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
                string appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
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
        /// The GetSchemeInfoFromSchemeRefno
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
                // TODO: Resource strings
                throw new Exception(string.Format("GetSchemeInfoFromSchemeRefno failed. Could not find scheme refno {0} in the AllProdSchemeLabel arraylist.", schemeRefno));
            }

            return schemeLblInfo;
        }

        /// <summary>
        /// The RegProdSchemeLabelSettingsListenerStatusOut
        /// </summary>
        /// <param name="prodSchemeLabelSettingsListener"></param>
        public void RegProdSchemeLabelSettingsListenerStatusOut(
        IProdSchemeLabelSettingsListener prodSchemeLabelSettingsListener)
        {
            this.OnProdSchemeLabelSettingsStatusOut += new OnProdSchemeLabelSettingsStatusOutHandler(
                prodSchemeLabelSettingsListener.OnProdSchemeLabelSettingsStatusOut);
        }

        /// <summary>
        /// The UnregProdSchemeLabelSettingsListenerStatusOut
        /// </summary>
        /// <param name="prodSchemeLabelSettingsListener"></param>
        public void UnregProdSchemeLabelSettingsListenerStatusOut(
        IProdSchemeLabelSettingsListener prodSchemeLabelSettingsListener)
        {
            this.OnProdSchemeLabelSettingsStatusOut -= new OnProdSchemeLabelSettingsStatusOutHandler(
                prodSchemeLabelSettingsListener.OnProdSchemeLabelSettingsStatusOut);
        }

        /// <summary>
        /// The RegProdSchemeLabelSettingsListenerProgressOut
        /// </summary>
        /// <param name="prodSchemeLabelSettingsListener"></param>
        public void RegProdSchemeLabelSettingsListenerProgressOut(
        IProdSchemeLabelSettingsListener prodSchemeLabelSettingsListener)
        {
            this.OnProdSchemeLabelSettingsProgressOut += new OnProdSchemeLabelSettingsProgressOutHandler(
                prodSchemeLabelSettingsListener.OnProdSchemeLabelSettingsProgressOut);
        }

        /// <summary>
        /// The UnregProdSchemeLabelSettingsListenerProgressOut
        /// </summary>
        /// <param name="prodSchemeLabelSettingsListener"></param>
        public void UnregProdSchemeLabelSettingsListenerProgressOut(
        IProdSchemeLabelSettingsListener prodSchemeLabelSettingsListener)
        {
            this.OnProdSchemeLabelSettingsProgressOut -= new OnProdSchemeLabelSettingsProgressOutHandler(
                prodSchemeLabelSettingsListener.OnProdSchemeLabelSettingsProgressOut);
        }

        /// <summary>
        /// The OnSysSocketStatusOut
        /// </summary>
        /// <param name="sockPublisher"></param>
        /// <param name="sockArgs"></param>
        public void OnSysSocketStatusOut(ISysSocketEventPublisher sockPublisher, SysSocketEventArgs sockArgs)
        {
            ProdSchemeLabelSettingsEventArgs args = new ProdSchemeLabelSettingsEventArgs(sockArgs.StatusOutMsg);
            this.OnProdSchemeLabelSettingsStatusOut(this, args);
        }

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
                        // TODO: Resource String
                        msgArg.StatusOutMsg = "Reading received data.";

                        this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
                    }

                    ds.ReadXml(SysCommon.GetXmlReader(ref xmlData), XmlReadMode.Auto);

                    if (this.OnProdSchemeLabelSettingsStatusOut != null)
                    {
                        // TODO: Resource String
                        msgArg.StatusOutMsg = "Saving product scheme label settings.";

                        this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
                    }

                    if (ds.Tables.Count != 0)
                    {
                        int progMax = ds.Tables["seg"].Rows.Count;

                        if (this.OnProdSchemeLabelSettingsStatusOut != null)
                        {
                            // TODO: Resource String
                            msgArg.StatusOutMsg = string.Format("Records {0}", progMax);

                            this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
                        }

                        if (this.OnProdSchemeLabelSettingsProgressOut != null)
                        {
                            msgArg.CurrentValue = 0;
                            msgArg.MinValue = 0;
                            msgArg.MaxValue = progMax;
                            this.OnProdSchemeLabelSettingsProgressOut(this, msgArg);
                        }

                        using (SQLiteCommand cmndScheme = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                        {
                            cmndScheme.CommandText = "SELECT * FROM tProductScheme WHERE ProductSchemeCode = @productSchemeCode";
                            cmndScheme.CommandType = CommandType.Text;
                            //cmndScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";

                            this.AllProdSchemeLabel.Clear();
                            int progValue = 0;
                            ProdSchemeLabelInfo schemeLblInfo = new ProdSchemeLabelInfo();

                            foreach (DataRow drow in ds.Tables["seg"].Rows)
                            {
                                schemeCode = Convert.ToString(drow["ProductSchemeCode"]);
                                errMsg = "Scheme code \'" + errMsg + "\'";

                                cmndScheme.Parameters.Add(new SQLiteParameter("@productSchemeCode", schemeCode));

                                using (SQLiteDataReader drScheme = cmndScheme.ExecuteReader())
                                {
                                    //if (!drScheme.Seek(DbSeekOptions.FirstEqual, new object[] { schemeCode }))
                                    //{
                                    //    throw new Exception("Unable to locate the product scheme code in tProductScheme."
                                    //        + Environment.NewLine + errMsg);
                                    //}

                                    if (!drScheme.Read())
                                    {
                                        // TODO: Resource strings
                                        throw new Exception("drProdScheme.Read() failed." + Environment.NewLine + errMsg);
                                    }

                                    schemeLblInfo.SchemeRefno = Convert.ToInt32(drScheme.GetValue(Convert.ToInt32(SysCommon.ProdSchemeFields.Refno)));
                                    schemeLblInfo.IsBestBeforeDateTracked = Convert.ToBoolean(drow["ProductSchemeLabelIsBestBeforeDateTracked"]);
                                    schemeLblInfo.IsLotTracked = Convert.ToBoolean(drow["ProductSchemeLabelIsLotTracked"]);
                                    schemeLblInfo.IsProductionDateTracked = Convert.ToBoolean(drow["ProductSchemeLabelIsProductionDateTracked"]);
                                    schemeLblInfo.IsSerialTracked = Convert.ToBoolean(drow["ProductSchemeLabelIsSerialTracked"]);
                                    schemeLblInfo.IsVariantTracked = Convert.ToBoolean(drow["ProductSchemeLabelIsVariantTracked"]);

                                    this.AllProdSchemeLabel.Add(schemeLblInfo);

                                    progValue += 1;

                                    if (this.OnProdSchemeLabelSettingsProgressOut != null)
                                    {
                                        msgArg.CurrentValue = progValue;
                                        msgArg.MinValue = 0;
                                        msgArg.MaxValue = progMax;
                                        this.OnProdSchemeLabelSettingsProgressOut(this, msgArg);
                                    }

                                    drScheme.Close();
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
                        // TODO: Resource strings
                        msgArg.StatusOutMsg = "Saving product scheme label settings complete.";

                        this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                if (this.OnProdSchemeLabelSettingsStatusOut != null)
                {
                    // TODO: Resource String
                    msgArg.StatusOutMsg = "Database exception occurred.";

                    this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
                }

                if (this.OnProdSchemeLabelSettingsStatusOut != null)
                {
                    // TODO: Resource String
                    msgArg.StatusOutMsg = "Saving product scheme label settings failed.";

                    this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
                }

                // TODO: Resource String
                throw new Exception("Database exception occurred. Saving product scheme label settings failed.", ex);
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

        /// <summary>
        /// Defines the <see cref="ProdSchemeLabelInfo" />
        /// </summary>
        public struct ProdSchemeLabelInfo
        {
            /// <summary>
            /// Defines the schemeRefno
            /// </summary>
            private int schemeRefno;

            /// <summary>
            /// Defines the isSerialTracked
            /// </summary>
            private bool isSerialTracked;

            /// <summary>
            /// Defines the isProductionDateTracked
            /// </summary>
            private bool isProductionDateTracked;

            /// <summary>
            /// Defines the isLotTracked
            /// </summary>
            private bool isLotTracked;

            /// <summary>
            /// Defines the isBestBeforeDateTracked
            /// </summary>
            private bool isBestBeforeDateTracked;

            /// <summary>
            /// Defines the isVariantTracked
            /// </summary>
            private bool isVariantTracked;

            /// <summary>
            /// Gets or sets the SchemeRefno
            /// </summary>
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

            /// <summary>
            /// Gets or sets a value indicating whether IsVariantTracked
            /// </summary>
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

            /// <summary>
            /// Gets or sets a value indicating whether IsBestBeforeDateTracked
            /// </summary>
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

            /// <summary>
            /// Gets or sets a value indicating whether IsSerialTracked
            /// </summary>
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

            /// <summary>
            /// Gets or sets a value indicating whether IsProductionDateTracked
            /// </summary>
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

            /// <summary>
            /// Gets or sets a value indicating whether IsLotTracked
            /// </summary>
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
    }
}
