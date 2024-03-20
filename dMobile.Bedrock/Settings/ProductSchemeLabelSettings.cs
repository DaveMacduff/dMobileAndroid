// <copyright file="ProductSchemeLabelSettings.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using dMobile.Resources.Models;
    using SQLite;
    using System;
    using System.Collections;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Printer Settings Class.
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
        /// arraylist of product scheme label definitions of type SchemeInfo.
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
        /// The OnProdSchemeLabelSettingsStatusOutHandler.
        /// </summary>
        /// <param name="prodSchemeLabelSettingsPublisher">The prodSchemeLabelSettingsPublisher<see cref="IProdSchemeLabelSettingsPublisher"/>.</param>
        /// <param name="prodSchemeLabelSettingsArgs">The prodSchemeLabelSettingsArgs<see cref="ProdSchemeLabelSettingsEventArgs"/>.</param>
        private delegate void OnProdSchemeLabelSettingsStatusOutHandler(
        IProdSchemeLabelSettingsPublisher prodSchemeLabelSettingsPublisher,
        ProdSchemeLabelSettingsEventArgs prodSchemeLabelSettingsArgs);

        /// <summary>
        /// The OnProdSchemeLabelSettingsProgressOutHandler.
        /// </summary>
        /// <param name="prodSchemeLabelSettingsPublisher">The prodSchemeLabelSettingsPublisher<see cref="IProdSchemeLabelSettingsPublisher"/>.</param>
        /// <param name="prodSchemeLabelSettingsArgs">The prodSchemeLabelSettingsArgs<see cref="ProdSchemeLabelSettingsEventArgs"/>.</param>
        private delegate void OnProdSchemeLabelSettingsProgressOutHandler(
        IProdSchemeLabelSettingsPublisher prodSchemeLabelSettingsPublisher,
        ProdSchemeLabelSettingsEventArgs prodSchemeLabelSettingsArgs);

        /// <summary>
        /// Defines the OnProdSchemeLabelSettingsStatusOut.
        /// </summary>
        private event OnProdSchemeLabelSettingsStatusOutHandler OnProdSchemeLabelSettingsStatusOut;

        /// <summary>
        /// Defines the OnProdSchemeLabelSettingsProgressOut.
        /// </summary>
        private event OnProdSchemeLabelSettingsProgressOutHandler OnProdSchemeLabelSettingsProgressOut;

        /// <summary>
        /// Gets or sets the AllProdSchemeLabel
        /// Gets or sets an arraylist of printer definitions of type prodSchemeLabelInfo.
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

            set => this.allProdSchemeLabel = value;
        }

        /// <summary>
        /// Gets or sets the class name to serialize.
        /// </summary>
        public string FullClassName
        {
            get => this.fullClassName;

            set => this.fullClassName = value;
        }

        /// <summary>
        /// Gets or sets the path name of the xml file.
        /// </summary>
        public string FilePath
        {
            get => this.filePath;

            set => this.filePath = value;
        }

        /// <summary>
        /// Gets or sets the full path and file name of the xml file.
        /// </summary>
        public string FileASCIIZ
        {
            get => this.fileASCIIZ;

            set => this.fileASCIIZ = value;
        }

        /// <summary>
        /// Gets or sets the file name of the xml file.
        /// </summary>
        public string FileName
        {
            get => this.fileName;

            set => this.fileName = value;
        }

        /// <summary>
        /// Request the product scheme label settings data and save the file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task RequestProductSchemeLabelSettings()
        {
            var responseData = string.Empty;
            var msgArgs = new ProdSchemeLabelSettingsEventArgs();

            try
            {
                msgArgs.StatusOutMsg = "Seeding product scheme label settings.";

                this.OnProdSchemeLabelSettingsStatusOut(this, msgArgs);

                responseData = SysCommon.SubmitComm(this.Create());
                await this.Save(responseData);

                msgArgs.StatusOutMsg = "Seeding product scheme label settings complete.";

                this.OnProdSchemeLabelSettingsStatusOut(this, msgArgs);
            }
            catch (Exception ex)
            {
                msgArgs.StatusOutMsg = "Seeding product scheme label settings failed.";

                this.OnProdSchemeLabelSettingsStatusOut(this, msgArgs);

                throw new Exception("Seeding product scheme label settings failed.", ex);
            }
        }

        /// <summary>
        /// Load and object of type ProductSchemeLabelSettings from a serialized xml file.
        /// </summary>
        /// <returns>Object of type ProductSchemeLabelSettings.</returns>
        public ProdSchemeLabelSettings LoadData()
        {
            lock (this.readLockObject)
            {
                var serializer = new XmlSerializer(typeof(ProdSchemeLabelSettings));
                var settings = new ProdSchemeLabelSettings();

                // use reflection to get the current assembly path for the database
                var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var fullPathToMyFile = Path.Combine(appDir, this.fileName);

                if (!File.Exists(fullPathToMyFile))
                {
                    this.SaveData(ref settings);
                }

                // Create a FileStream to read with.
                using var reader = new FileStream(fullPathToMyFile, FileMode.Open, FileAccess.Read, FileShare.Read);

                // Deserialize the stream to the object, and close the TextReader
                settings = (ProdSchemeLabelSettings)serializer.Deserialize(reader);

                return settings;
            }
        }

        /// <summary>
        /// Serialize and save the data into a xml file.
        /// </summary>
        /// <param name="settings"> Instance of ProductSchemeLabelSettings to serialize.</param>
        /// <returns>True if save was successful else false.</returns>
        public bool SaveData(ref ProdSchemeLabelSettings settings)
        {
            lock (this.writeLockObject)
            {
                var serializer = new XmlSerializer(typeof(ProdSchemeLabelSettings));
                var saveComplete = false;

                try
                {
                    // use reflection to get the current assembly path for the database
                    var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    var fullPathToMyFile = Path.Combine(appDir, this.fileName);

                    // Create a FileStream to write with.
                    using (var writer = new FileStream(fullPathToMyFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        // Serialize the object, and close the TextWriter
                        serializer.Serialize(writer, settings);
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
        /// The GetSchemeInfoFromSchemeRefno.
        /// </summary>
        /// <param name="schemeRefno">.</param>
        /// <returns>.</returns>
        public ProdSchemeLabelInfo GetSchemeInfoFromSchemeRefno(int schemeRefno)
        {
            var schemeLblInfo = new ProdSchemeLabelInfo();

            if (this.AllProdSchemeLabel.Count > 0)
            {
                for (var idx = 0; idx < this.AllProdSchemeLabel.Count; idx++)
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
                    $"GetSchemeInfoFromSchemeRefno failed. Could not find scheme refno {schemeRefno} in the AllProdSchemeLabel arraylist.");
            }

            return schemeLblInfo;
        }

        /// <summary>
        /// The RegProdSchemeLabelSettingsListenerStatusOut.
        /// </summary>
        /// <param name="prodSchemeLabelSettingsListener">.</param>
        public void RegProdSchemeLabelSettingsListenerStatusOut(
        IProdSchemeLabelSettingsListener prodSchemeLabelSettingsListener)
        {
            this.OnProdSchemeLabelSettingsStatusOut += new OnProdSchemeLabelSettingsStatusOutHandler(
                prodSchemeLabelSettingsListener.OnProdSchemeLabelSettingsStatusOut);
        }

        /// <summary>
        /// The UnregProdSchemeLabelSettingsListenerStatusOut.
        /// </summary>
        /// <param name="prodSchemeLabelSettingsListener">.</param>
        public void UnregProdSchemeLabelSettingsListenerStatusOut(
        IProdSchemeLabelSettingsListener prodSchemeLabelSettingsListener)
        {
            this.OnProdSchemeLabelSettingsStatusOut -= new OnProdSchemeLabelSettingsStatusOutHandler(
                prodSchemeLabelSettingsListener.OnProdSchemeLabelSettingsStatusOut);
        }

        /// <summary>
        /// The RegProdSchemeLabelSettingsListenerProgressOut.
        /// </summary>
        /// <param name="prodSchemeLabelSettingsListener">.</param>
        public void RegProdSchemeLabelSettingsListenerProgressOut(
        IProdSchemeLabelSettingsListener prodSchemeLabelSettingsListener)
        {
            this.OnProdSchemeLabelSettingsProgressOut += new OnProdSchemeLabelSettingsProgressOutHandler(
                prodSchemeLabelSettingsListener.OnProdSchemeLabelSettingsProgressOut);
        }

        /// <summary>
        /// The UnregProdSchemeLabelSettingsListenerProgressOut.
        /// </summary>
        /// <param name="prodSchemeLabelSettingsListener">.</param>
        public void UnregProdSchemeLabelSettingsListenerProgressOut(
        IProdSchemeLabelSettingsListener prodSchemeLabelSettingsListener)
        {
            this.OnProdSchemeLabelSettingsProgressOut -= new OnProdSchemeLabelSettingsProgressOutHandler(
                prodSchemeLabelSettingsListener.OnProdSchemeLabelSettingsProgressOut);
        }

        /// <summary>
        /// The OnSysSocketStatusOut.
        /// </summary>
        /// <param name="sockPublisher">.</param>
        /// <param name="sockArgs">.</param>
        public void OnSysSocketStatusOut(ISysSocketEventPublisher sockPublisher, SysSocketEventArgs sockArgs)
        {
            var args = new ProdSchemeLabelSettingsEventArgs(sockArgs.StatusOutMsg);
            this.OnProdSchemeLabelSettingsStatusOut(this, args);
        }

        /// <summary>
        /// Process the xml data from the communication server and store it.
        /// </summary>
        /// <param name="xmlData"> Xml data received from the communication server.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task Save(string xmlData)
        {
            var msgArg = new ProdSchemeLabelSettingsEventArgs();
            var schemeCode = string.Empty;
            var errMsg = string.Empty;

            try
            {
                using var ds = new DataSet();
                if (this.OnProdSchemeLabelSettingsStatusOut != null)
                {
                    msgArg.StatusOutMsg = "Reading received data.";

                    this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
                }

                ds.ReadXml(SysCommon.GetXmlReader(ref xmlData), XmlReadMode.Auto);

                if (this.OnProdSchemeLabelSettingsStatusOut != null)
                {
                    msgArg.StatusOutMsg = "Saving product scheme label settings.";

                    this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
                }

                if (ds.Tables.Count != 0)
                {
                    var progMax = ds.Tables["seg"].Rows.Count;

                    if (this.OnProdSchemeLabelSettingsStatusOut != null)
                    {
                        msgArg.StatusOutMsg = $"Records {progMax}";

                        this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
                    }

                    if (this.OnProdSchemeLabelSettingsProgressOut != null)
                    {
                        msgArg.CurrentValue = 0;
                        msgArg.MinValue = 0;
                        msgArg.MaxValue = progMax;
                        this.OnProdSchemeLabelSettingsProgressOut(this, msgArg);
                    }

                    this.AllProdSchemeLabel.Clear();
                    var progValue = 0;
                    var schemeLblInfo = new ProdSchemeLabelInfo();

                    foreach (DataRow drow in ds.Tables["seg"].Rows)
                    {
                        schemeCode = Convert.ToString(drow["ProductSchemeCode"]);
                        errMsg = "Scheme code \'" + errMsg + "\'";

                        var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<tProductSchemeModel>("SELECT * FROM tProductScheme WHERE ProductSchemeCode = ?", schemeCode);

                        // if (!drScheme.Seek(DbSeekOptions.FirstEqual, new object[] { schemeCode }))
                        // {
                        //    throw new Exception("Unable to locate the product scheme code in tProductScheme."
                        //        + Environment.NewLine + errMsg);
                        // }

                        // if (!drScheme.Read())
                        if (results == null || results.Count <= 0)
                        {
                            throw new Exception("drProdScheme.Read() failed." + Environment.NewLine + errMsg);
                        }

                        // schemeLblInfo.SchemeRefno = Convert.ToInt32(drScheme.GetValue(Convert.ToInt32(SysCommon.ProdSchemeFields.Refno)));
                        schemeLblInfo.SchemeRefno = Convert.ToInt32(results[0].ProductSchemeRefno);
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
                    }

                    if (this.OnProdSchemeLabelSettingsProgressOut != null)
                    {
                        msgArg.CurrentValue = progMax;
                        msgArg.MinValue = 0;
                        msgArg.MaxValue = progMax;
                        this.OnProdSchemeLabelSettingsProgressOut(this, msgArg);
                    }
                }

                if (this.OnProdSchemeLabelSettingsStatusOut != null)
                {
                    msgArg.StatusOutMsg = "Saving product scheme label settings complete.";

                    this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
                }
            }
            catch (SQLiteException ex)
            {
                if (this.OnProdSchemeLabelSettingsStatusOut != null)
                {
                    msgArg.StatusOutMsg = "Database exception occurred.";

                    this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
                }

                if (this.OnProdSchemeLabelSettingsStatusOut != null)
                {
                    msgArg.StatusOutMsg = "Saving product scheme label settings failed.";

                    this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
                }

                throw new Exception("Database exception occurred. Saving product scheme label settings failed.", ex);
            }
        }

        /// <summary>
        /// Create the xml request string to send to the communication server.
        /// </summary>
        /// <returns>String of the xml request to get the product scheme label settings.</returns>
        private string Create()
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            // Keep the revision date to empty 1/1/1900.
            // I had to do this because on a partial refresh we do not maintain the prior xml data.
            // The structure is empty.
            var revision = new DateTime(1900, 1, 1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var sysEnv = SysEnvironment.GetInstance();

            var request = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                string.Empty,
                new XElement(
                    "imclient",
                    new XElement(
                        "command",
                        new XElement("action", "REQUEST PRODUCT SCHEME LABEL SETTINGS"),
                        new XElement("device_id", sysEnv.DeviceId),
                        new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                        new XElement("revision_datetime", revision))));

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// Defines the <see cref="ProdSchemeLabelInfo" />.
        /// </summary>
        public struct ProdSchemeLabelInfo
        {
            /// <summary>
            /// Gets or sets the SchemeRefno.
            /// </summary>
            public int SchemeRefno { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether IsVariantTracked.
            /// </summary>
            public bool IsVariantTracked { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether IsBestBeforeDateTracked.
            /// </summary>
            public bool IsBestBeforeDateTracked { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether IsSerialTracked.
            /// </summary>
            public bool IsSerialTracked { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether IsProductionDateTracked.
            /// </summary>
            public bool IsProductionDateTracked { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether IsLotTracked.
            /// </summary>
            public bool IsLotTracked { get; set; }
        }
    }
}
