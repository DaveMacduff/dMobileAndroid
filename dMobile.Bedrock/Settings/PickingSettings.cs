// <copyright file="PickingSettings.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="PickingSettings" />.
    /// </summary>
    [Serializable()]
    public class PickingSettings
    {
        // VARIABLES
        /// <summary>
        /// Defines the m_strFullClassName.
        /// </summary>
        [NonSerialized()]
        private string m_strFullClassName = "dMobile.PickingSettings";

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
        private string m_strFileName = "dMobilePickingSettings.xml";

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
        /// Defines the m_iRouteCodeSelectedIndex.
        /// </summary>
        private int m_iRouteCodeSelectedIndex = -1;

        /// <summary>
        /// Defines the m_strRouteCodeSelectedValue.
        /// </summary>
        private string m_strRouteCodeSelectedValue = string.Empty;

        /// <summary>
        /// Defines the isApplyRouteCodeFilter.
        /// </summary>
        private bool isApplyRouteCodeFilter = false;

        /// <summary>
        /// Defines the m_dtOrderedDateFilter.
        /// </summary>
        private DateTime m_dtOrderedDateFilter = new DateTime(1900, 1, 1);

        /// <summary>
        /// Defines the isApplyOrderedDateFilter.
        /// </summary>
        private bool isApplyOrderedDateFilter = false;

        /// <summary>
        /// Defines the m_dtDeliveryDateFilter.
        /// </summary>
        private DateTime m_dtDeliveryDateFilter = new DateTime(1900, 1, 1);

        /// <summary>
        /// Defines the isApplyDeliveryDateFilter.
        /// </summary>
        private bool isApplyDeliveryDateFilter = false;

        // PROPERTIES
        /// <summary>
        /// Gets or sets the strRouteCodeSelectedValue.
        /// </summary>
        public string strRouteCodeSelectedValue
        {
            get { return this.m_strRouteCodeSelectedValue; }
            set { this.m_strRouteCodeSelectedValue = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsApplyDeliveryDateFilter.
        /// </summary>
        public bool IsApplyDeliveryDateFilter
        {
            get { return this.isApplyDeliveryDateFilter; }
            set { this.isApplyDeliveryDateFilter = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsApplyOrderedDateFilter.
        /// </summary>
        public bool IsApplyOrderedDateFilter
        {
            get { return this.isApplyOrderedDateFilter; }
            set { this.isApplyOrderedDateFilter = value; }
        }

        /// <summary>
        /// Gets or sets the dtDeliveryDateFilter.
        /// </summary>
        public DateTime dtDeliveryDateFilter
        {
            get { return this.m_dtDeliveryDateFilter; }
            set { this.m_dtDeliveryDateFilter = value; }
        }

        /// <summary>
        /// Gets or sets the dtOrderedDateFilter.
        /// </summary>
        public DateTime dtOrderedDateFilter
        {
            get { return this.m_dtOrderedDateFilter; }
            set { this.m_dtOrderedDateFilter = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsApplyRouteCodeFilter.
        /// </summary>
        public bool IsApplyRouteCodeFilter
        {
            get { return this.isApplyRouteCodeFilter; }
            set { this.isApplyRouteCodeFilter = value; }
        }

        /// <summary>
        /// Gets or sets the iRouteCodeSelectedIndex.
        /// </summary>
        public int iRouteCodeSelectedIndex
        {
            get { return this.m_iRouteCodeSelectedIndex; }
            set { this.m_iRouteCodeSelectedIndex = value; }
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
        /// Initializes a new instance of the <see cref="PickingSettings"/> class.
        /// </summary>
        public PickingSettings()
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
            var sortBy = new List<Sort>();
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
        public PickingSettings LoadData()
        {
            lock (this.readLockObject)
            {
                var serializer = new XmlSerializer(typeof(PickingSettings));
                var oPickingSettings = new PickingSettings();

                // use reflection to get the current assembly path for the database
                var strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var strFullPathToMyFile = Path.Combine(strAppDir, this.m_strFileName);

                if (!File.Exists(strFullPathToMyFile))
                {
                    this.SaveData(ref oPickingSettings);
                }

                // Create a FileStream to read with.
                using (var reader = new FileStream(strFullPathToMyFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // Deserialize the stream to the object, and close the TextReader
                    oPickingSettings = (PickingSettings)serializer.Deserialize(reader);
                }

                return oPickingSettings;
            }
        }

        /// <summary>
        /// The SaveData.
        /// </summary>
        /// <param name="oPickingSettings">.</param>
        /// <returns>.</returns>
        public bool SaveData(ref PickingSettings oPickingSettings)
        {
            lock (this.writeLockObject)
            {
                var serializer = new XmlSerializer(typeof(PickingSettings));
                var bSaveComplete = false;

                try
                {
                    // use reflection to get the current assembly path for the database
                    var strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    var strFullPathToMyFile = Path.Combine(strAppDir, this.m_strFileName);

                    // Create a FileStream to write with.
                    using (var writer = new FileStream(strFullPathToMyFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        // Serialize the object, and close the TextWriter
                        serializer.Serialize(writer, oPickingSettings);
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
