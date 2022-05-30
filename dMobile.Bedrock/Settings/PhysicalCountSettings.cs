// <copyright file="PhysicalCountSettings.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the <see cref="PhysicalCountSettings" />.
    /// </summary>
    [Serializable()]
    public class PhysicalCountSettings
    {
        // VARIABLES
        /// <summary>
        /// Defines the m_strFullClassName.
        /// </summary>
        [NonSerialized()]
        private string m_strFullClassName = "dMobile.PhysicalCountSettings";

        /// <summary>
        /// Defines the m_strFilePath.
        /// </summary>
        [NonSerialized()]
        private string m_strFilePath = string.Empty;

        /// <summary>
        /// Defines the m_strFileASCIIZ.
        /// </summary>
        [NonSerialized()]
        private string m_strFileASCIIZ = string.Empty;

        /// <summary>
        /// Defines the m_strFileName.
        /// </summary>
        [NonSerialized()]
        private string m_strFileName = "dMobilePhysicalCountSettings.xml";

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
        /// Defines the m_iSortBy1Index.
        /// </summary>
        private int m_iSortBy1Index = 0;

        /// <summary>
        /// Defines the sortBy1Value.
        /// </summary>
        private Sort sortBy1Value = new Sort() { SortValue = "ActivityCountFKRefno", SortOrder = SortOrder.ASC };

        /// <summary>
        /// Defines the m_iSortBy2Index.
        /// </summary>
        private int m_iSortBy2Index = -1;

        /// <summary>
        /// Defines the sortBy2Value.
        /// </summary>
        private Sort sortBy2Value = null;

        /// <summary>
        /// Defines the m_dtCountDate.
        /// </summary>
        private DateTime m_dtCountDate = new DateTime(1900, 1, 1);

        /// <summary>
        /// Defines the isApplyCountDate.
        /// </summary>
        private bool isApplyCountDate = false;

        // PROPERTIES
        /// <summary>
        /// Gets or sets a value indicating whether IsApplyCountDate.
        /// </summary>
        public bool IsApplyCountDate
        {
            get { return this.isApplyCountDate; }
            set { this.isApplyCountDate = value; }
        }

        /// <summary>
        /// Gets or sets the dtCountDate.
        /// </summary>
        public DateTime dtCountDate
        {
            get { return this.m_dtCountDate; }
            set { this.m_dtCountDate = value; }
        }

        /// <summary>
        /// Gets or sets the strFullClassName.
        /// </summary>
        public string strFullClassName
        {
            get { return this.m_strFullClassName; }
            set { this.m_strFullClassName = value; }
        }

        /// <summary>
        /// Gets or sets the strFilePath.
        /// </summary>
        public string strFilePath
        {
            get { return this.m_strFilePath; }
            set { this.m_strFilePath = value; }
        }

        /// <summary>
        /// Gets or sets the strFileASCIIZ.
        /// </summary>
        public string strFileASCIIZ
        {
            get { return this.m_strFileASCIIZ; }
            set { this.m_strFileASCIIZ = value; }
        }

        /// <summary>
        /// Gets or sets the strFileName.
        /// </summary>
        public string strFileName
        {
            get { return this.m_strFileName; }
            set { this.m_strFileName = value; }
        }

        /// <summary>
        /// Gets or sets the iSortBy1Index.
        /// </summary>
        public int iSortBy1Index
        {
            get { return this.m_iSortBy1Index; }
            set { this.m_iSortBy1Index = value; }
        }

        /// <summary>
        /// Gets or sets the SortBy1Value.
        /// </summary>
        public Sort SortBy1Value
        {
            get { return this.sortBy1Value; }
            set { this.sortBy1Value = value; }
        }

        /// <summary>
        /// Gets or sets the iSortBy2Index.
        /// </summary>
        public int iSortBy2Index
        {
            get { return this.m_iSortBy2Index; }
            set { this.m_iSortBy2Index = value; }
        }

        /// <summary>
        /// Gets or sets the SortBy2Value.
        /// </summary>
        public Sort SortBy2Value
        {
            get { return this.sortBy2Value; }
            set { this.sortBy2Value = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicalCountSettings"/> class.
        /// </summary>
        public PhysicalCountSettings()
        {
            this.strFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.strFileASCIIZ = Path.Combine(this.strFilePath, this.strFileName);
        }

        /// <summary>
        /// The GetDataViewSortBy.
        /// </summary>
        /// <returns>.</returns>
        public List<Sort> GetDataViewSortBy()
        {
            List<Sort> sortBy = new List<Sort>();

            sortBy.Add(this.SortBy1Value);

            if (this.SortBy2Value != null && !string.IsNullOrEmpty(this.SortBy2Value.SortValue))
            {
                sortBy.Add(this.SortBy2Value);
            }

            return sortBy;
        }

        /// <summary>
        /// The LoadData.
        /// </summary>
        /// <returns>.</returns>
        public PhysicalCountSettings LoadData()
        {
            lock (this.readLockObject)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PhysicalCountSettings));
                PhysicalCountSettings oPhysicalCountSettings = new PhysicalCountSettings();

                // use reflection to get the current assembly path for the database
                string strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string strFullPathToMyFile = Path.Combine(strAppDir, this.m_strFileName);

                if (!File.Exists(strFullPathToMyFile))
                {
                    this.SaveData(ref oPhysicalCountSettings);
                }

                // Create a FileStream to read with.
                using (FileStream reader = new FileStream(strFullPathToMyFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // Deserialize the stream to the object, and close the TextReader
                    oPhysicalCountSettings = (PhysicalCountSettings)serializer.Deserialize(reader);
                }

                return oPhysicalCountSettings;
            }
        }

        /// <summary>
        /// The SaveData.
        /// </summary>
        /// <param name="oPhysicalCountSettings">.</param>
        /// <returns>.</returns>
        public bool SaveData(ref PhysicalCountSettings oPhysicalCountSettings)
        {
            lock (this.writeLockObject)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PhysicalCountSettings));
                bool bSaveComplete = false;

                try
                {
                    // use reflection to get the current assembly path for the database
                    string strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    string strFullPathToMyFile = Path.Combine(strAppDir, this.m_strFileName);

                    // Create a FileStream to write with.
                    using (FileStream writer = new FileStream(strFullPathToMyFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        // Serialize the object, and close the TextWriter
                        serializer.Serialize(writer, oPhysicalCountSettings);
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
