// <copyright file="ProdSchemeLabelSettings.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock.Settings
{
    using System;
    using System.Collections;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using dMobile.Bedrock;
    using dMobile.Resources.Models.Products;
    using SQLite;

    /// <summary>
    /// Printer Settings Class.
    /// </summary>
    [XmlInclude(typeof(ProdSchemeLabelInfo))]
    [Serializable]
    public class ProdSchemeLabelSettings : ISysSocketEventListener, IProdSchemeLabelSettingsPublisher
    {
        /// <summary>
        /// Defines the write lock object.
        /// </summary>
        [NonSerialized]
        private readonly object writeLockObject = new object();

        /// <summary>
        /// Defines the read lock object.
        /// </summary>
        [NonSerialized]
        private readonly object readLockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProdSchemeLabelSettings"/> class.
        /// </summary>
        public ProdSchemeLabelSettings()
        {
            this.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.FileASCIIZ = Path.Combine(this.FilePath, this.FileName);
        }

        /// <summary>
        /// The on product scheme label settings status out handler.
        /// </summary>
        /// <param name="publisher">The prodSchemeLabelSettingsPublisher<see cref="IProdSchemeLabelSettingsPublisher"/>.</param>
        /// <param name="args">The prodSchemeLabelSettingsArgs<see cref="ProdSchemeLabelSettingsEventArgs"/>.</param>
        private delegate void OnProdSchemeLabelSettingsStatusOutHandler(
        IProdSchemeLabelSettingsPublisher publisher,
        ProdSchemeLabelSettingsEventArgs args);

        /// <summary>
        /// The on product scheme label settings progress out handler.
        /// </summary>
        /// <param name="publisher">The prodSchemeLabelSettingsPublisher<see cref="IProdSchemeLabelSettingsPublisher"/>.</param>
        /// <param name="args'">The prodSchemeLabelSettingsArgs<see cref="ProdSchemeLabelSettingsEventArgs"/>.</param>
        private delegate void OnProdSchemeLabelSettingsProgressOutHandler(
        IProdSchemeLabelSettingsPublisher publisher,
        ProdSchemeLabelSettingsEventArgs args);

        /// <summary>
        /// Defines the on product scheme label settings status out.
        /// </summary>
        private event OnProdSchemeLabelSettingsStatusOutHandler OnProdSchemeLabelSettingsStatusOut;

        /// <summary>
        /// Defines the on product scheme label settings progress out.
        /// </summary>
        private event OnProdSchemeLabelSettingsProgressOutHandler OnProdSchemeLabelSettingsProgressOut;

        /// <summary>
        /// Gets or sets an arraylist of printer definitions of type prod scheme label info.
        /// </summary>
        public ArrayList AllProdSchemeLabel { get; set; } = new ArrayList();

        /// <summary>
        /// Gets or sets the class name to serialize.
        /// </summary>
        public string FullClassName { get; set; } = "dMobile.Bedrock.ProdSchemeLabelSettings";

        /// <summary>
        /// Gets or sets the path name of the xml file.
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the full path and file name of the xml file.
        /// </summary>
        public string FileASCIIZ { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file name of the xml file.
        /// </summary>
        public string FileName { get; set; } = "dMobileProdSchemeLabelSettings.xml";

        /// <summary>
        /// Request the product scheme label settings data and save the file.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task RequestProductSchemeLabelSettings()
        {
            var args = new ProdSchemeLabelSettingsEventArgs();

            try
            {
                args.StatusOutMsg = "Seeding product scheme label settings.";
                this.OnProdSchemeLabelSettingsStatusOut(this, args);

                var responseData = SysCommon.SubmitComm(this.Create());
                await this.Save(responseData);

                args.StatusOutMsg = "Seeding product scheme label settings complete.";
                this.OnProdSchemeLabelSettingsStatusOut(this, args);
            }
            catch (Exception ex)
            {
                args.StatusOutMsg = "Seeding product scheme label settings failed.";
                this.OnProdSchemeLabelSettingsStatusOut(this, args);

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
                var fullPathToMyFile = Path.Combine(appDir, this.FileName);

                if (!File.Exists(fullPathToMyFile))
                {
                    this.SaveData(ref settings);
                }

                // Create a FileStream to read with.
                using var reader = new FileStream(fullPathToMyFile, FileMode.Open, FileAccess.Read, FileShare.Read);

                // Deserialize the stream to the object, and close the TextReader
                return (ProdSchemeLabelSettings)serializer.Deserialize(reader);
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
                    var fullPathToMyFile = Path.Combine(appDir, this.FileName);

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
        /// Get scheme info from the scheme reference number.
        /// </summary>
        /// <param name="schemeRefno">Scheme reference number.</param>
        /// <returns>Product scheme label information.</returns>
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
                    $"GetSchemeInfoFromSchemeRefno failed. Could not find scheme refno {schemeRefno} in the " +
                    "AllProdSchemeLabel arraylist.");
            }

            return schemeLblInfo;
        }

        /// <summary>
        /// Register a listener for product scheme label settings status out events.
        /// </summary>
        /// <param name="listener">Product scheme label settings listener.</param>
        public void RegProdSchemeLabelSettingsListenerStatusOut(IProdSchemeLabelSettingsListener listener)
        {
            this.OnProdSchemeLabelSettingsStatusOut += new OnProdSchemeLabelSettingsStatusOutHandler(
                listener.OnProdSchemeLabelSettingsStatusOut);
        }

        /// <summary>
        /// Unregister a listener for product scheme label settings status out events.
        /// </summary>
        /// <param name="listener">Product scheme label settings listener.</param>
        public void UnregProdSchemeLabelSettingsListenerStatusOut(IProdSchemeLabelSettingsListener listener)
        {
            this.OnProdSchemeLabelSettingsStatusOut -= new OnProdSchemeLabelSettingsStatusOutHandler(
                listener.OnProdSchemeLabelSettingsStatusOut);
        }

        /// <summary>
        /// Register a listener for product scheme label settings progress out events.
        /// </summary>
        /// <param name="listener">Product scheme label settings listener.</param>
        public void RegProdSchemeLabelSettingsListenerProgressOut(IProdSchemeLabelSettingsListener listener)
        {
            this.OnProdSchemeLabelSettingsProgressOut += new OnProdSchemeLabelSettingsProgressOutHandler(
                listener.OnProdSchemeLabelSettingsProgressOut);
        }

        /// <summary>
        /// Unregister a listener for product scheme label settings progress out events.
        /// </summary>
        /// <param name="listener">Product scheme label settings listener.</param>
        public void UnregProdSchemeLabelSettingsListenerProgressOut(IProdSchemeLabelSettingsListener listener)
        {
            this.OnProdSchemeLabelSettingsProgressOut -= new OnProdSchemeLabelSettingsProgressOutHandler(
                listener.OnProdSchemeLabelSettingsProgressOut);
        }

        /// <summary>
        /// On system socket status out event.
        /// </summary>
        /// <param name="sockPublisher">Socket event publisher.</param>
        /// <param name="sockArgs">Socket event arguments.</param>
        public void OnSysSocketStatusOut(ISysSocketEventPublisher sockPublisher, SysSocketEventArgs sockArgs)
        {
            var args = new ProdSchemeLabelSettingsEventArgs(sockArgs.StatusOutMsg);
            this.OnProdSchemeLabelSettingsStatusOut(this, args);
        }

        /// <summary>
        /// Process the xml data from the communication server and store it.
        /// </summary>
        /// <param name="xmlData"> Xml data received from the communication server.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task Save(string xmlData)
        {
            var msgArg = new ProdSchemeLabelSettingsEventArgs();
            var schemeCode = string.Empty;
            var errMsg = string.Empty;

            try
            {
                if (this.OnProdSchemeLabelSettingsStatusOut != null)
                {
                    msgArg.StatusOutMsg = "Reading received data.";
                    this.OnProdSchemeLabelSettingsStatusOut(this, msgArg);
                }

                using var ds = new DataSet();
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

                        var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ProductSchemeModel>(
                            "SELECT * FROM tProductScheme WHERE ProductSchemeCode = ?", schemeCode);

                        // if (!drScheme.Seek(DbSeekOptions.FirstEqual, new object[] { schemeCode }))
                        // {
                        //    throw new Exception("Unable to locate the product scheme code in tProductScheme."
                        //        + Environment.NewLine + errMsg);
                        // }

                        // if (!drScheme.Read())
                        if (results == null || results.Count == 0)
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

                        progValue++;

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
        /// Defines the product scheme label information structure.
        /// </summary>
        public struct ProdSchemeLabelInfo
        {
            /// <summary>
            /// Gets or sets the scheme reference number.
            /// </summary>
            public int SchemeRefno { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the scheme is variant tracked.
            /// </summary>
            public bool IsVariantTracked { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the scheme is best before date tracked.
            /// </summary>
            public bool IsBestBeforeDateTracked { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the scheme is serial tracked.
            /// </summary>
            public bool IsSerialTracked { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the scheme is production date tracked.
            /// </summary>
            public bool IsProductionDateTracked { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the scheme is lot tracked.
            /// </summary>
            public bool IsLotTracked { get; set; }
        }
    }
}
