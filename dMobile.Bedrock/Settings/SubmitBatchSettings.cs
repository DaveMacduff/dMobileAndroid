// <copyright file="SubmitBatchSettings.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the <see cref="SubmitBatchSettings" />.
    /// </summary>
    [Serializable()]
    public class SubmitBatchSettings
    {
        // VARIABLES
        /// <summary>
        /// Defines the m_strFullClassName.
        /// </summary>
        [NonSerialized()]
        private string m_strFullClassName = "dMobile.SubmitBatchSettings";

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
        private string m_strFileName = "dMobileSubmitBatchSettings.xml";

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
        /// Defines the m_iActivityStatusIdx.
        /// </summary>
        private int m_iActivityStatusIdx = 0;

        /// <summary>
        /// Defines the m_eActivityStatus.
        /// </summary>
        private SysCommon.ActStatus m_eActivityStatus = SysCommon.ActStatus.NoChange;

        /// <summary>
        /// Defines the m_iActivitySubTypeIdx.
        /// </summary>
        private int m_iActivitySubTypeIdx = 0;

        /// <summary>
        /// Defines the m_eActivitySubType.
        /// </summary>
        private SysCommon.ActSubTypes m_eActivitySubType = SysCommon.ActSubTypes.Picking;

        // PROPERTIES
        /// <summary>
        /// Gets or sets the eActivitySubType.
        /// </summary>
        public SysCommon.ActSubTypes eActivitySubType
        {
            get => this.m_eActivitySubType;
            set => this.m_eActivitySubType = value;
        }

        /// <summary>
        /// Gets or sets the eActivityStatus.
        /// </summary>
        public SysCommon.ActStatus eActivityStatus
        {
            get => this.m_eActivityStatus;
            set => this.m_eActivityStatus = value;
        }

        /// <summary>
        /// Gets or sets the iActivityStatusIdx.
        /// </summary>
        public int iActivityStatusIdx
        {
            get => this.m_iActivityStatusIdx;
            set => this.m_iActivityStatusIdx = value;
        }

        /// <summary>
        /// Gets or sets the iActivitySubTypeIdx.
        /// </summary>
        public int iActivitySubTypeIdx
        {
            get => this.m_iActivitySubTypeIdx;
            set => this.m_iActivitySubTypeIdx = value;
        }

        /// <summary>
        /// Gets or sets the strFullClassName.
        /// </summary>
        public string strFullClassName
        {
            get => this.m_strFullClassName;
            set => this.m_strFullClassName = value;
        }

        /// <summary>
        /// Gets or sets the strFilePath.
        /// </summary>
        public string strFilePath
        {
            get => this.m_strFilePath;
            set => this.m_strFilePath = value;
        }

        /// <summary>
        /// Gets or sets the strFileASCIIZ.
        /// </summary>
        public string strFileASCIIZ
        {
            get => this.m_strFileASCIIZ;
            set => this.m_strFileASCIIZ = value;
        }

        /// <summary>
        /// Gets or sets the strFileName.
        /// </summary>
        public string strFileName
        {
            get => this.m_strFileName;
            set => this.m_strFileName = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitBatchSettings"/> class.
        /// </summary>
        public SubmitBatchSettings()
        {
            this.strFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.strFileASCIIZ = Path.Combine(this.strFilePath, this.strFileName);
        }

        /// <summary>
        /// The LoadData.
        /// </summary>
        /// <returns>.</returns>
        public SubmitBatchSettings LoadData()
        {
            lock (this.readLockObject)
            {
                var serializer = new XmlSerializer(typeof(SubmitBatchSettings));
                var oSubmitBatchSettings = new SubmitBatchSettings();

                // use reflection to get the current assembly path for the database
                var strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var strFullPathToMyFile = Path.Combine(strAppDir, this.m_strFileName);

                if (!File.Exists(strFullPathToMyFile))
                {
                    this.SaveData(ref oSubmitBatchSettings);
                }

                // Create a FileStream to read with.
                using (var reader = new FileStream(strFullPathToMyFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // Deserialize the stream to the object, and close the TextReader
                    oSubmitBatchSettings = (SubmitBatchSettings)serializer.Deserialize(reader);
                }

                return oSubmitBatchSettings;
            }
        }

        /// <summary>
        /// The SaveData.
        /// </summary>
        /// <param name="oSubmitBatchSettings">.</param>
        /// <returns>.</returns>
        public bool SaveData(ref SubmitBatchSettings oSubmitBatchSettings)
        {
            lock (this.writeLockObject)
            {
                var serializer = new XmlSerializer(typeof(SubmitBatchSettings));
                var bSaveComplete = false;

                try
                {
                    // use reflection to get the current assembly path for the database
                    var strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    var strFullPathToMyFile = Path.Combine(strAppDir, this.m_strFileName);

                    // Create a FileStream to write with.
                    using (var writer = new FileStream(strFullPathToMyFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        serializer.Serialize(writer, oSubmitBatchSettings);
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
