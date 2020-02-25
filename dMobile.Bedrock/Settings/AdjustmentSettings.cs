// ----------------------------------------------------------------------
// <copyright file="AdjustmentSettings.cs" company="InterSect Business Systems Inc.">
//     Copyright (2013) InterSect Business Systems Inc.
// </copyright>
//
// ------------------------------------------------------------------------
namespace dMobile.Bedrock
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Adjustment Settings Class
    /// </summary>
    [Serializable]
    public class AdjustmentSettings
    {
        // VARIABLES

        /// <summary>
        /// Contains full adjustment settings class name.
        /// </summary>
        [NonSerialized]
        private string fullClassName = "dMobile.AdjustmentSettings";

        /// <summary>
        /// Contains adjustment settings path name.
        /// </summary>
        [NonSerialized]
        private string filePath = string.Empty;

        /// <summary>
        /// Contains adjustment settings drive letter, path and file name.
        /// </summary>
        [NonSerialized]
        private string fileASCIIZ = string.Empty;

        /// <summary>
        /// Contains adjustment settings file name.
        /// </summary>
        [NonSerialized]
        private string fileName = "dMobileAdjustmentSettings.xml";

        /// <summary>
        /// Contains currently filtered adjustment type.
        /// </summary>
        private SysCommon.AdjTypes adjustmentType = SysCommon.AdjTypes.Inventory;

        /// <summary>
        /// Contains if the currently select adjustment type filter applied or enabled.
        /// </summary>
        private bool isApplyAdjustmentType = false;

        /// <summary>
        /// Contains first sort option selected index.
        /// </summary>
        private int sortBy1Index = 0;

        /// <summary>
        /// Contains first sort option selected index value.
        /// </summary>
        private string sortBy1Value = "ActivityAdjustmentFKRefno ASC";

        /// <summary>
        /// Contains second sort option selected index.
        /// </summary>
        private int sortBy2Index = -1;

        /// <summary>
        /// Contains second sort option selected index value.
        /// </summary>
        private string sortBy2Value = string.Empty;

        /// <summary>
        /// Contains third sort option selected index.
        /// </summary>
        private int sortBy3Index = -1;

        /// <summary>
        /// Contains third sort option selected index value.
        /// </summary>
        private string sortBy3Value = string.Empty;

        /// <summary>
        /// Contains setting if scan quantity is negated for the adjustment type.
        /// </summary>
        private bool negateScanQuantity = false;

        /// <summary>
        /// Contains the date for adjustment date filtering.
        /// </summary>
        private System.DateTime adjustmentDate = new System.DateTime(1900, 1, 1);

        /// <summary>
        /// Contains the enabler for adjustment date filter.
        /// </summary>
        private bool isApplyAdjustmentDate = false;

        // CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the <see cref="AdjustmentSettings"/> class.
        /// </summary>
        public AdjustmentSettings()
        {
            this.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.FileASCIIZ = Path.Combine(this.FilePath, this.FileName);
        }

        // PROPERTIES

        /// <summary>
        /// Gets or sets a value indicating whether the adjustment date filter is enabled.
        /// </summary>
        public bool IsApplyAdjustmentDate
        {
            get { return this.isApplyAdjustmentDate; }
            set { this.isApplyAdjustmentDate = value; }
        }

        /// <summary>
        /// Gets or sets as value indicating whether the adjustment date to filter.
        /// </summary>
        public DateTime AdjustmentDate
        {
            get { return this.adjustmentDate; }
            set { this.adjustmentDate = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the flag to negate the scanned quantity.
        /// </summary>
        public bool NegateScanQuantity
        {
            get { return this.negateScanQuantity; }
            set { this.negateScanQuantity = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the flag to apply adjustment type filter.
        /// </summary>
        public bool IsApplyAdjustmentType
        {
            get { return this.isApplyAdjustmentType; }
            set { this.isApplyAdjustmentType = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the adjustment type filter.
        /// </summary>
        public SysCommon.AdjTypes AdjustmentType
        {
            get { return this.adjustmentType; }
            set { this.adjustmentType = value; }
        }

        /// <summary>
        /// Gets or sets the class name to serialize.
        /// </summary>
        public string FullClassName
        {
            get { return this.fullClassName; }
            set { this.fullClassName = value; }
        }

        /// <summary>
        /// Gets or sets the path name of the xml file.
        /// </summary>
        public string FilePath
        {
            get { return this.filePath; }
            set { this.filePath = value; }
        }

        /// <summary>
        /// Gets or sets the full path and file name of the xml file.
        /// </summary>
        /// <remarks>
        /// Includes the drive letter, path and file name.
        /// </remarks>
        public string FileASCIIZ
        {
            get { return this.fileASCIIZ; }
            set { this.fileASCIIZ = value; }
        }

        /// <summary>
        /// Gets or sets the file name of the xml file. 
        /// </summary>
        /// <remarks>
        /// Does not include any path information.
        /// </remarks>
        public string FileName
        {
            get { return this.fileName; }
            set { this.fileName = value; }
        }

        /// <summary>
        /// Gets or sets the first sort by index.
        /// </summary>
        public int SortBy1Index
        {
            get { return this.sortBy1Index; }
            set { this.sortBy1Index = value; }
        }

        /// <summary>
        /// Gets or sets the first sort by value.
        /// </summary>
        public string SortBy1Value
        {
            get { return this.sortBy1Value; }
            set { this.sortBy1Value = value; }
        }

        /// <summary>
        /// Gets or sets the second sort by index.
        /// </summary>
        public int SortBy2Index
        {
            get { return this.sortBy2Index; }
            set { this.sortBy2Index = value; }
        }

        /// <summary>
        /// Gets or sets the second sort by value.
        /// </summary>
        public string SortBy2Value
        {
            get { return this.sortBy2Value; }
            set { this.sortBy2Value = value; }
        }

        /// <summary>
        /// Gets or sets the third sort by index.
        /// </summary>
        public int SortBy3Index
        {
            get { return this.sortBy3Index; }
            set { this.sortBy3Index = value; }
        }

        /// <summary>
        /// Gets or sets the third sort by value.
        /// </summary>
        public string SortBy3Value
        {
            get { return this.sortBy3Value; }
            set { this.sortBy3Value = value; }
        }

        // METHODS

        /// <summary>
        /// Get the data view sort by value.
        /// </summary>
        /// <returns>Sort by data view expression.</returns>
        public string GetDataViewSortBy()
        {
            string sortBy = this.SortBy1Value;

            if (!string.IsNullOrEmpty(this.SortBy2Value))
            {
                sortBy = sortBy + ", " + this.SortBy2Value;
            }

            if (!string.IsNullOrEmpty(this.SortBy3Value))
            {
                sortBy = sortBy + ", " + this.SortBy3Value;
            }

            return sortBy;
        }

        /// <summary>
        /// Load and object of type AdjustmentsSettings from a serialized xml file.
        /// </summary>
        /// <returns>Object of AdjustmentSettings</returns>
        public AdjustmentSettings LoadData()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AdjustmentSettings));
            AdjustmentSettings adjustmentSettings = new AdjustmentSettings();

            // use relection to get the current assembly path for the database
            string appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string fullPathToMyFile = Path.Combine(appDir, this.FileName);

            if (!File.Exists(fullPathToMyFile))
            {
                this.SaveData(ref adjustmentSettings);
            }

            // Create a FileStream to read with.
            FileStream reader = new FileStream(fullPathToMyFile, FileMode.Open);

            // Deserialize the stream to the object, and close the TextReader
            adjustmentSettings = (AdjustmentSettings)serializer.Deserialize(reader);
            reader.Close();

            return adjustmentSettings;
        }

        /// <summary>
        /// Serialize and save the data into a xml file.
        /// </summary>
        /// <param name="adjustmentSettings"> Instance of PrinterSettings to serialize.</param>
        /// <returns>True if save was successful else false</returns>
        public bool SaveData(ref AdjustmentSettings adjustmentSettings)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AdjustmentSettings));
            bool saveComplete = false;

            try
            {
                // use relection to get the current assembly path for the database
                string appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string fullPathToMyFile = Path.Combine(appDir, this.FileName);

                // Create a FileStream to write with.
                FileStream writer = new FileStream(fullPathToMyFile, FileMode.Create);

                // Serialize the object, and close the TextWriter
                serializer.Serialize(writer, adjustmentSettings);
                writer.Close();

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
