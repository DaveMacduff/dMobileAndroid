// <copyright file="ProductionTicketSettings.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="ProductionTicketSettings" />.
    /// </summary>
    [Serializable]
    public class ProductionTicketSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionTicketSettings"/> class.
        /// </summary>
        public ProductionTicketSettings()
        {
            this.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.FileASCIIZ = Path.Combine(this.FilePath, this.FileName);
        }

        /// <summary>
        /// Gets or sets the production date filter.
        /// </summary>
        public DateTime ProductionDateFilter { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        /// Gets or sets a value indicating whether the production date filter is applied.
        /// </summary>
        public bool IsApplyProductionDateFilter { get; set; } = false;

        /// <summary>
        /// Gets the full class name.
        /// </summary>
        public string FullClassName { get; } = "dMobile.ProductionTicketSettings";

        /// <summary>
        /// Gets or sets the serialzed file path.
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file ASCIIZ.
        /// </summary>
        public string FileASCIIZ { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; } = "dMobileProductionTicketSettings.xml";

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
        /// Gets or sets the sort by index for the third sort criteria.
        /// </summary>
        public int SortBy3Index { get; set; } = -1;

        /// <summary>
        /// Gets or sets the sort by value for the third sort criteria.
        /// </summary>
        public Sort SortBy3Value { get; set; } = null;

        /// <summary>
        /// Get the data view sort criteria as a list of Sort objects.
        /// </summary>
        /// <returns>List of sorted data.</returns>
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
        /// Loads the production ticket settings data from the XML file.
        /// </summary>
        /// <returns>Production ticket settings.</returns>
        public ProductionTicketSettings LoadData()
        {
            var serializer = new XmlSerializer(typeof(ProductionTicketSettings));
            var settings = new ProductionTicketSettings();

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
                settings = (ProductionTicketSettings)serializer.Deserialize(reader);
            }

            return settings;
        }

        /// <summary>
        /// Save the production ticket settings data to the XML file.
        /// </summary>
        /// <param name="settings">Production ticket settings to save.</param>
        /// <returns>True if the save was successful, else false.</returns>
        public bool SaveData(ref ProductionTicketSettings settings)
        {
            var serializer = new XmlSerializer(typeof(ProductionTicketSettings));
            bool isSaveComplete;
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

                isSaveComplete = true;
            }
            catch
            {
                isSaveComplete = false;
            }

            return isSaveComplete;
        }
    }
}
