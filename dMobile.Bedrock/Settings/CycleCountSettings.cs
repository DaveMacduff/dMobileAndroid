// ----------------------------------------------------------------------
// <copyright file="CycleCountSettings.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>
//
// ------------------------------------------------------------------------

namespace dMobile.Bedrock.Settings
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;

    /// <summary>
    /// Class for Cycle Count Settings.
    /// </summary>
    [Serializable]
    public class CycleCountSettings
    {
        [NonSerialized]
        private readonly object writeLockObject = new object();

        [NonSerialized]
        private readonly object readLockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="CycleCountSettings"/> class.
        /// </summary>
        public CycleCountSettings()
        {
            this.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.FileASCIIZ = Path.Combine(this.FilePath, this.FileName);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to apply the count date filter.
        /// </summary>
        public bool IsApplyCountDate { get; set; }

        /// <summary>
        /// Gets or sets the count date filter.
        /// </summary>
        public DateTime CountDate { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        /// Gets or sets the full class name.
        /// </summary>
        public string FullClassName { get; set; } = "dMobile.CycleCountSettings";

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
        public string FileName { get; set; } = "dMobileCycleCountSettings.xml";

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
        /// Get a list of sort criteria for data views.
        /// </summary>
        /// <returns>List of sort by items.</returns>
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
        /// Loads the cycle count settings data from the XML file.
        /// </summary>
        /// <returns>Cycle count settings.</returns>
        public CycleCountSettings LoadData()
        {
            lock (this.readLockObject)
            {
                var serializer = new XmlSerializer(typeof(CycleCountSettings));
                var oCycleCountSettings = new CycleCountSettings();

                // use reflection to get the current assembly path for the database
                var strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var strFullPathToMyFile = Path.Combine(strAppDir, this.FileName);

                if (!File.Exists(strFullPathToMyFile))
                {
                    this.SaveData(ref oCycleCountSettings);
                }

                // Create a FileStream to read with.
                using (var reader = new FileStream(strFullPathToMyFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    oCycleCountSettings = (CycleCountSettings)serializer.Deserialize(reader);
                }

                return oCycleCountSettings;
            }
        }

        /// <summary>
        /// Save the cycle count settings data to the XML file.
        /// </summary>
        /// <param name="settings">Cycle count settings to save.</param>
        /// <returns>True if the save was successful, else false.</returns>
        public bool SaveData(ref CycleCountSettings settings)
        {
            lock (this.writeLockObject)
            {
                var serializer = new XmlSerializer(typeof(CycleCountSettings));
                var bSaveComplete = false;

                try
                {
                    // use reflection to get the current assembly path for the database
                    var strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    var strFullPathToMyFile = Path.Combine(strAppDir, this.FileName);

                    // Create a FileStream to write with.
                    using (var writer = new FileStream(strFullPathToMyFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        serializer.Serialize(writer, settings);
                    }

                    bSaveComplete = true;
                }
                catch
                {
                    bSaveComplete = false;
                }

                return bSaveComplete;
            }
        }
    }
}
