// <copyright file="PickingSettings.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="PickingSettings" />.
    /// </summary>
    [Serializable]
    public class PickingSettings
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
        /// Initializes a new instance of the <see cref="PickingSettings"/> class.
        /// </summary>
        public PickingSettings()
        {
            this.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.FileASCIIZ = Path.Combine(this.FilePath, this.FileName);
        }

        /// <summary>
        /// Gets or sets the route code selected value.
        /// </summary>
        public string RouteCodeSelectedValue { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to apply delivery date filter.
        /// </summary>
        public bool IsApplyDeliveryDateFilter { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the flag to apply ordered date filter.
        /// </summary>
        public bool IsApplyOrderedDateFilter { get; set; } = false;

        /// <summary>
        /// Gets or sets the delivery date filter.
        /// </summary>
        public DateTime DeliveryDateFilter { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        /// Gets or sets the ordered date filter.
        /// </summary>
        public DateTime OrderedDateFilter { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        /// Gets or sets a value indicating whether the flag to apply route code filter.
        /// </summary>
        public bool IsApplyRouteCodeFilter { get; set; } = false;

        /// <summary>
        /// Gets or sets the route code selected index.
        /// </summary>
        public int RouteCodeSelectedIndex { get; set; } = -1;

        /// <summary>
        /// Gets or sets the full class name.
        /// </summary>
        public string FullClassName { get; set; } = "dMobile.PickingSettings";

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
        public string FileName { get; set; } = "dMobilePickingSettings.xml";

        /// <summary>
        /// Gets or sets the sort by 1 index.
        /// </summary>
        public int SortBy1Index { get; set; } = 0;

        /// <summary>
        /// Gets or sets the sort by 1 value.
        /// </summary>
        public Sort SortBy1Value { get; set; } = new Sort() { SortValue = "ActivityFKRefno", SortOrder = SortOrder.ASC };

        /// <summary>
        /// Gets or sets the sort by 2 index.
        /// </summary>
        public int SortBy2Index { get; set; } = -1;

        /// <summary>
        /// Gets or sets the sort by 2 value.
        /// </summary>
        public Sort SortBy2Value { get; set; } = null;

        /// <summary>
        /// Gets or sets the sort by 3 index.
        /// </summary>
        public int SortBy3Index { get; set; } = -1;

        /// <summary>
        /// Gets or sets the sort by 3 value.
        /// </summary>
        public Sort SortBy3Value { get; set; } = null;

        /// <summary>
        /// Gets or sets the sort by 4 index.
        /// </summary>
        public int SortBy4Index { get; set; } = -1;

        /// <summary>
        /// Gets or sets the sort by 4 value.
        /// </summary>
        public Sort SortBy4Value { get; set; } = null;

        /// <summary>
        /// Get the data view sort by.
        /// </summary>
        /// <returns>.</returns>
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
        /// Load the picking settings data from the XML file.
        /// </summary>
        /// <returns>Picking settings.</returns>
        public PickingSettings LoadData()
        {
            lock (this.readLockObject)
            {
                var serializer = new XmlSerializer(typeof(PickingSettings));
                var settings = new PickingSettings();

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
                    settings = (PickingSettings)serializer.Deserialize(reader);
                }

                return settings;
            }
        }

        /// <summary>
        /// Save the picking settings data to the XML file.
        /// </summary>
        /// <param name="settings">Picking settings to save.</param>
        /// <returns>True if save successful, else false.</returns>
        public bool SaveData(ref PickingSettings settings)
        {
            lock (this.writeLockObject)
            {
                var serializer = new XmlSerializer(typeof(PickingSettings));
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
