// <copyright file="ReplenishmentSettings.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock.Settings
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;

    /// <summary>
    /// Replenishment Settings Class.
    /// </summary>
    [Serializable]
    public class ReplenishmentSettings
    {
        /// <summary>
        /// Mutual-exclusion lock used for file writing.
        /// </summary>
        [NonSerialized]
        private readonly object writeLockObject = new object();

        /// <summary>
        /// Mutual-exclusion lock used for file reading.
        /// </summary>
        [NonSerialized]
        private readonly object readLockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplenishmentSettings"/> class.
        /// </summary>
        public ReplenishmentSettings()
        {
            this.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.FileASCIIZ = Path.Combine(this.FilePath, this.FileName);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the flag to apply replenishment type filter.
        /// </summary>
        public bool IsApplyReplenishmentType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the replenishment type filter.
        /// </summary>
        public SysCommon.ReplenishmentTypes ReplenishmentType { get; set; } = SysCommon.ReplenishmentTypes.Manual;

        /// <summary>
        /// Gets or sets the class name to serialize.
        /// </summary>
        public string FullClassName { get; set; } = "dMobile.ReplenishmentSettings";

        /// <summary>
        /// Gets or sets the path name of the xml file.
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the full path and file name of the xml file.
        /// </summary>
        /// <remarks>
        /// Includes the drive letter, path and file name.
        /// </remarks>
        public string FileASCIIZ { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file name of the xml file.
        /// </summary>
        /// <remarks>
        /// Does not include any path information.
        /// </remarks>
        public string FileName { get; set; } = "dMobileReplenishmentSettings.xml";

        /// <summary>
        /// Gets or sets the first sort by index.
        /// </summary>
        public int SortBy1Index { get; set; } = 0;

        /// <summary>
        /// Gets or sets the first sort by value.
        /// </summary>
        public Sort SortBy1Value { get; set; } = new Sort() { SortValue = "ActivityItemFKRefno", SortOrder = SortOrder.ASC };

        // METHODS

        /// <summary>
        /// Get the data view sort by value.
        /// </summary>
        /// <returns>Sort by data view expression.</returns>
        public List<Sort> GetDataViewSortBy()
        {
            return new List<Sort>
            {
                this.SortBy1Value
            };
        }

        /// <summary>
        /// Load and object of type ReplenishmentsSettings from a serialized xml file.
        /// </summary>
        /// <returns>Object of ReplenishmentSettings.</returns>
        public ReplenishmentSettings LoadData()
        {
            lock (this.readLockObject)
            {
                var serializer = new XmlSerializer(typeof(ReplenishmentSettings));
                var settings = new ReplenishmentSettings();

                // use reflection to get the current assembly path for the database
                var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var settingsFileName = Path.Combine(appDir, this.FileName);

                if (!File.Exists(settingsFileName))
                {
                    this.SaveData(ref settings);
                }

                // Create a FileStream to read with.
                using (var reader = new FileStream(
                    settingsFileName,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read))
                {
                    // Deserialize the stream to the object, and close the TextReader
                    settings = (ReplenishmentSettings)serializer.Deserialize(reader);
                }

                return settings;
            }
        }

        /// <summary>
        /// Serialize and save the data into a xml file.
        /// </summary>
        /// <param name="replenishmentSettings"> Instance of replenishment settings to serialize.</param>
        /// <returns>True if save was successful else false.</returns>
        public bool SaveData(ref ReplenishmentSettings replenishmentSettings)
        {
            lock (this.writeLockObject)
            {
                var serializer = new XmlSerializer(typeof(ReplenishmentSettings));
                var saveComplete = false;

                try
                {
                    // use reflection to get the current assembly path for the database
                    var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    var settingsFileName = Path.Combine(appDir, this.FileName);

                    // Create a FileStream to write with.
                    using (var writer = new FileStream(
                        settingsFileName,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None))
                    {
                        // Serialize the object, and close the TextWriter
                        serializer.Serialize(writer, replenishmentSettings);
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
    }
}
