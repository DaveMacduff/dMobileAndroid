// <copyright file="ReplenishmentSettings.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
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
        /// Contains full replenishment settings class name.
        /// </summary>
        [NonSerialized]
        private string fullClassName = "dMobile.ReplenishmentSettings";

        /// <summary>
        /// Contains replenishment settings path name.
        /// </summary>
        [NonSerialized]
        private string filePath = string.Empty;

        /// <summary>
        /// Contains replenishment settings drive letter, path and file name.
        /// </summary>
        [NonSerialized]
        private string fileASCIIZ = string.Empty;

        /// <summary>
        /// Contains replenishment settings file name.
        /// </summary>
        [NonSerialized]
        private string fileName = "dMobileReplenishmentSettings.xml";

        /// <summary>
        /// Contains currently filtered replenishment type.
        /// </summary>
        private SysCommon.ReplenishmentTypes replenishmentType = SysCommon.ReplenishmentTypes.Manual;

        /// <summary>
        /// Contains if the currently select adjustment type filter applied or enabled.
        /// </summary>
        private bool isApplyReplenishmentType = false;

        /// <summary>
        /// Contains first sort option selected index.
        /// </summary>
        private int sortBy1Index = 0;

        /// <summary>
        /// Contains first sort option selected index value.
        /// </summary>
        private Sort sortBy1Value = new Sort() { SortValue = "ActivityItemFKRefno", SortOrder = SortOrder.ASC };

        /////// <summary>
        /////// Contains second sort option selected index.
        /////// </summary>
        ////private int sortBy2Index = -1;

        /////// <summary>
        /////// Contains second sort option selected index value.
        /////// </summary>
        ////private Sort sortBy2Value = null;

        /////// <summary>
        /////// Contains third sort option selected index.
        /////// </summary>
        ////private int sortBy3Index = -1;

        /////// <summary>
        /////// Contains third sort option selected index value.
        /////// </summary>
        ////private Sort sortBy3Value = null;

        // CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplenishmentSettings"/> class.
        /// </summary>
        public ReplenishmentSettings()
        {
            this.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.FileASCIIZ = Path.Combine(this.FilePath, this.FileName);
        }

        // PROPERTIES

        /// <summary>
        /// Gets or sets a value indicating whether the flag to apply replenishment type filter.
        /// </summary>
        public bool IsApplyReplenishmentType
        {
            get => this.isApplyReplenishmentType;
            set => this.isApplyReplenishmentType = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the replenishment type filter.
        /// </summary>
        public SysCommon.ReplenishmentTypes ReplenishmentType
        {
            get => this.ReplenishmentType;
            set => this.ReplenishmentType = value;
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
        /// <remarks>
        /// Includes the drive letter, path and file name.
        /// </remarks>
        public string FileASCIIZ
        {
            get => this.fileASCIIZ;
            set => this.fileASCIIZ = value;
        }

        /// <summary>
        /// Gets or sets the file name of the xml file.
        /// </summary>
        /// <remarks>
        /// Does not include any path information.
        /// </remarks>
        public string FileName
        {
            get => this.fileName;
            set => this.fileName = value;
        }

        /// <summary>
        /// Gets or sets the first sort by index.
        /// </summary>
        public int SortBy1Index
        {
            get => this.sortBy1Index;
            set => this.sortBy1Index = value;
        }

        /// <summary>
        /// Gets or sets the first sort by value.
        /// </summary>
        public Sort SortBy1Value
        {
            get => this.sortBy1Value;
            set => this.sortBy1Value = value;
        }

        // METHODS

        /// <summary>
        /// Get the data view sort by value.
        /// </summary>
        /// <returns>Sort by data view expression.</returns>
        public List<Sort> GetDataViewSortBy()
        {
            var sortBy = new List<Sort>();

            sortBy.Add(this.SortBy1Value);

            return sortBy;
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
                var fullPathToMyFile = Path.Combine(appDir, this.FileName);

                if (!File.Exists(fullPathToMyFile))
                {
                    this.SaveData(ref settings);
                }

                // Create a FileStream to read with.
                using (var reader = new FileStream(
                    fullPathToMyFile,
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
                    var fullPathToMyFile = Path.Combine(appDir, this.FileName);

                    // Create a FileStream to write with.
                    using (var writer = new FileStream(
                        fullPathToMyFile,
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
