// <copyright file="PhysicalCountSettings.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock.Settings
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;

    /// <summary>
    /// Defines the <see cref="PhysicalCountSettings" />.
    /// </summary>
    [Serializable]
    public class PhysicalCountSettings
    {
        [NonSerialized]
        private readonly object writeLockObject = new object();

        [NonSerialized]
        private readonly object readLockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicalCountSettings"/> class.
        /// </summary>
        public PhysicalCountSettings()
        {
            this.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.FileASCIIZ = Path.Combine(this.FilePath, this.FileName);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to apply the count date filter.
        /// </summary>
        public bool IsApplyCountDate { get; set; }

        /// <summary>
        /// Gets or sets the count date.
        /// </summary>
        public DateTime CountDate { get; set; } = SysCommon.EmptyDateTime;

        /// <summary>
        /// Gets the full class name.
        /// </summary>
        public string FullClassName { get; } = "dMobile.PhysicalCountSettings";

        /// <summary>
        /// Gets or sets the settings file path.
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file ASCIIZ.
        /// </summary>
        public string FileASCIIZ { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the settings file name.
        /// </summary>
        public string FileName { get; set; } = "dMobilePhysicalCountSettings.xml";

        /// <summary>
        /// Gets or sets the sort by index for the first sort criteria.
        /// </summary>
        public int SortBy1Index { get; set; } = 0;

        /// <summary>
        /// Gets or sets the sort by value for the first sort criteria.
        /// </summary>
        public Sort SortBy1Value { get; set; } = new Sort() { SortValue = "ActivityFKRefno", SortOrder = SortOrder.ASC };

        /// <summary>
        /// Gets or sets the sort by index for the second sort criteria.
        /// </summary>
        public int SortBy2Index { get; set; } = -1;

        /// <summary>
        /// Gets or sets the sort by value for the second sort criteria.
        /// </summary>
        public Sort SortBy2Value { get; set; } = null;

        /// <summary>
        /// Get the data view sort by.
        /// </summary>
        /// <returns>List of data sort by values.</returns>
        public List<Sort> GetDataViewSortBy()
        {
            var sortBy = new List<Sort>
            {
                this.SortBy1Value
            };

            if (this.SortBy2Value != null && !string.IsNullOrEmpty(this.SortBy2Value.SortValue))
            {
                sortBy.Add(this.SortBy2Value);
            }

            return sortBy;
        }

        /// <summary>
        /// Load the settings data from the XML file.
        /// </summary>
        /// <returns>Physical count settings.</returns>
        public PhysicalCountSettings LoadData()
        {
            lock (this.readLockObject)
            {
                var serializer = new XmlSerializer(typeof(PhysicalCountSettings));
                var settings = new PhysicalCountSettings();

                // use reflection to get the current assembly path for the database
                var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var settingsFileName = Path.Combine(appDir, this.FileName);

                if (!File.Exists(settingsFileName))
                {
                    this.SaveData(ref settings);
                }

                // Create a FileStream to read with.
                using (var reader = new FileStream(settingsFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // Deserialize the stream to the object, and close the TextReader
                    settings = (PhysicalCountSettings)serializer.Deserialize(reader);
                }

                return settings;
            }
        }

        /// <summary>
        /// Save the settings data to the XML file.
        /// </summary>
        /// <param name="settings">Physical count settings.</param>
        /// <returns>True if saved, else false.</returns>
        public bool SaveData(ref PhysicalCountSettings settings)
        {
            lock (this.writeLockObject)
            {
                var serializer = new XmlSerializer(typeof(PhysicalCountSettings));
                var saveComplete = false;

                try
                {
                    // use reflection to get the current assembly path for the database
                    var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    var settingsFileName = Path.Combine(appDir, this.FileName);

                    // Create a FileStream to write with.
                    using (var writer = new FileStream(settingsFileName, FileMode.Create, FileAccess.Write, FileShare.None))
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
    }
}
