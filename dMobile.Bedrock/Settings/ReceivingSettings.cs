// <copyright file="ReceivingSettings.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="ReceivingSettings" />.
    /// </summary>
    [Serializable]
    public class ReceivingSettings
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
        /// Initializes a new instance of the <see cref="ReceivingSettings"/> class.
        /// </summary>
        public ReceivingSettings()
        {
            this.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.FileASCIIZ = Path.Combine(this.FilePath, this.FileName);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to apply issued date filter.
        /// </summary>
        public bool IsApplyIssuedDateFilter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to apply arrival date filter.
        /// </summary>
        public bool IsApplyArrivalDateFilter { get; set; }

        /// <summary>
        /// Gets or sets the issued date filter.
        /// </summary>
        public DateTime IssuedDateFilter { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        /// Gets or sets the arrival date filter.
        /// </summary>
        public DateTime ArrivalDateFilter { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        /// Gets or sets the full class name.
        /// </summary>
        public string FullClassName { get; set; } = "dMobile.ReceivingSettings";

        /// <summary>
        /// Gets or sets the settings file path.
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the settings file ASCIIZ.
        /// </summary>
        public string FileASCIIZ { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the settings file name.
        /// </summary>
        public string FileName { get; set; } = "dMobileReceivingSettings.xml";

        /// <summary>
        /// Gets or sets the sort by 1 index.
        /// </summary>
        public int SortBy1Index { get; set; } = 0;

        /// <summary>
        /// Gets or sets sort by 1 value.
        /// </summary>
        public Sort SortBy1Value { get; set; } = new Sort() { SortValue = "ActivityFKRefno", SortOrder = SortOrder.ASC };

        /// <summary>
        /// Gets or sets the sort by 2 index.
        /// </summary>
        public int SortBy2Index { get; set; } = -1;

        /// <summary>
        /// Gets or sets the sort by 2 value.
        /// </summary>
        public Sort SortBy2Value { get; set; }

        /// <summary>
        /// Gets or sets the sort by 3 index.
        /// </summary>
        public int SortBy3Index { get; set; } = -1;

        /// <summary>
        /// Gets or sets the sort by 3 value.
        /// </summary>
        public Sort SortBy3Value { get; set; }

        /// <summary>
        /// Gets or sets the sort by 4 index.
        /// </summary>
        public int SortBy4Index { get; set; } = -1;

        /// <summary>
        /// Gets or sets the sort by 4 value.
        /// </summary>
        public Sort SortBy4Value { get; set; }

        /// <summary>
        /// Gets the data view sort by.
        /// </summary>
        /// <returns>The <see cref="List{Sort}"/>.</returns>
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

            if (this.SortBy3Value != null && !string.IsNullOrEmpty(this.SortBy3Value.SortValue))
            {
                sortBy.Add(this.SortBy3Value);
            }

            if (this.SortBy4Value != null && !string.IsNullOrEmpty(this.SortBy4Value.SortValue))
            {
                sortBy.Add(this.SortBy4Value);
            }

            return sortBy;
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <returns>The <see cref="ReceivingSettings"/>Receive settings.</returns>
        public ReceivingSettings LoadData()
        {
            lock (this.readLockObject)
            {
                var serializer = new XmlSerializer(typeof(ReceivingSettings));
                var settings = new ReceivingSettings();

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
                    settings = (ReceivingSettings)serializer.Deserialize(reader);
                }

                return settings;
            }
        }

        /// <summary>
        /// Save the settings data.
        /// </summary>
        /// <param name="settings">Receive settings.</param>
        /// <returns>True if the settings saved, else false.</returns>
        public bool SaveData(ref ReceivingSettings settings)
        {
            lock (this.writeLockObject)
            {
                var serializer = new XmlSerializer(typeof(ReceivingSettings));
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
