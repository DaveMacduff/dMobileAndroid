// <copyright file="TransferSettings.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TransferSettings" />.
    /// </summary>
    [Serializable]
    public class TransferSettings
    {
        // VARIABLES
        /// <summary>
        /// Defines the m_strFullClassName.
        /// </summary>
        [NonSerialized()]
        private string m_strFullClassName = "dMobile.TransferSettings";

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
        private string m_strFileName = "dMobileTransferSettings.xml";

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
        private Sort sortBy1Value = new Sort() { SortValue = "ActivityFKRefno", SortOrder = SortOrder.ASC };

        /// <summary>
        /// Defines the m_iSortBy2Index.
        /// </summary>
        private int m_iSortBy2Index = -1;

        /// <summary>
        /// Defines the sortBy2Value.
        /// </summary>
        private Sort sortBy2Value = null;

        /// <summary>
        /// Defines the m_iSortBy3Index.
        /// </summary>
        private int m_iSortBy3Index = -1;

        /// <summary>
        /// Defines the sortBy3Value.
        /// </summary>
        private Sort sortBy3Value = null;

        /// <summary>
        /// Defines the m_iSortBy4Index.
        /// </summary>
        private int m_iSortBy4Index = -1;

        /// <summary>
        /// Defines the sortBy4Value.
        /// </summary>
        private Sort sortBy4Value = null;

        /// <summary>
        /// Defines the isApplyTransferDateFilter.
        /// </summary>
        private bool isApplyTransferDateFilter = false;

        /// <summary>
        /// Defines the transferDateFilter.
        /// </summary>
        private DateTime transferDateFilter = new DateTime(1900, 1, 1);

        // PROPERTIES
        /// <summary>
        /// Gets or sets a value indicating whether IsApplyTransferDateFilter.
        /// </summary>
        public bool IsApplyTransferDateFilter
        {
            get { return this.isApplyTransferDateFilter; }
            set { this.isApplyTransferDateFilter = value; }
        }

        /// <summary>
        /// Gets or sets the TransferDateFilter.
        /// </summary>
        public DateTime TransferDateFilter
        {
            get { return this.transferDateFilter; }
            set { this.transferDateFilter = value; }
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
        /// Gets or sets the iSortBy3Index.
        /// </summary>
        public int iSortBy3Index
        {
            get { return this.m_iSortBy3Index; }
            set { this.m_iSortBy3Index = value; }
        }

        /// <summary>
        /// Gets or sets the SortBy3Value.
        /// </summary>
        public Sort SortBy3Value
        {
            get { return this.sortBy3Value; }
            set { this.sortBy3Value = value; }
        }

        /// <summary>
        /// Gets or sets the iSortBy4Index.
        /// </summary>
        public int iSortBy4Index
        {
            get { return this.m_iSortBy4Index; }
            set { this.m_iSortBy4Index = value; }
        }

        /// <summary>
        /// Gets or sets the SortBy4Value.
        /// </summary>
        public Sort SortBy4Value
        {
            get { return this.sortBy4Value; }
            set { this.sortBy4Value = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferSettings"/> class.
        /// </summary>
        public TransferSettings()
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
        /// The LoadData.
        /// </summary>
        /// <returns>.</returns>
        public TransferSettings LoadData()
        {
            lock (this.readLockObject)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TransferSettings));
                TransferSettings oTransferSettings = new TransferSettings();

                // use reflection to get the current assembly path for the database
                string strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string strFullPathToMyFile = Path.Combine(strAppDir, this.m_strFileName);

                if (!File.Exists(strFullPathToMyFile))
                {
                    this.SaveData(ref oTransferSettings);
                }

                // Create a FileStream to read with.
                using (FileStream reader = new FileStream(strFullPathToMyFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // Deserialize the stream to the object, and close the TextReader
                    oTransferSettings = (TransferSettings)serializer.Deserialize(reader);
                }

                return oTransferSettings;
            }
        }

        /// <summary>
        /// The SaveData.
        /// </summary>
        /// <param name="oTransferSettings">.</param>
        /// <returns>.</returns>
        public bool SaveData(ref TransferSettings oTransferSettings)
        {
            lock (this.writeLockObject)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TransferSettings));
                bool bSaveComplete = false;

                try
                {
                    // use reflection to get the current assembly path for the database
                    string strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    string strFullPathToMyFile = Path.Combine(strAppDir, this.m_strFileName);

                    // Create a FileStream to write with.
                    using (FileStream writer = new FileStream(strFullPathToMyFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        serializer.Serialize(writer, oTransferSettings);
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
