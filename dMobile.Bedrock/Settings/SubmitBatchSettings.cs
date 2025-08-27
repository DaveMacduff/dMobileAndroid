// <copyright file="SubmitBatchSettings.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock.Settings
{
    using System;
    using System.IO;
    using System.Xml.Serialization;
    using dMobile.Bedrock;

    /// <summary>
    /// Defines the <see cref="SubmitBatchSettings" />.
    /// </summary>
    [Serializable]
    public class SubmitBatchSettings
    {
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
        /// Initializes a new instance of the <see cref="SubmitBatchSettings"/> class.
        /// </summary>
        public SubmitBatchSettings()
        {
            this.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.FileASCIIZ = Path.Combine(this.FilePath, this.FileName);
        }

        /// <summary>
        /// Gets or sets the eActivitySubType.
        /// </summary>
        public SysCommon.ActSubTypes ActivitySubType { get; set; } = SysCommon.ActSubTypes.Picking;

        /// <summary>
        /// Gets or sets the eActivityStatus.
        /// </summary>
        public SysCommon.ActStatus ActivityStatus { get; set; } = SysCommon.ActStatus.NoChange;

        /// <summary>
        /// Gets or sets the iActivityStatusIdx.
        /// </summary>
        public int ActivityStatusIdx { get; set; }

        /// <summary>
        /// Gets or sets the iActivitySubTypeIdx.
        /// </summary>
        public int ActivitySubTypeIdx { get; set; }

        /// <summary>
        /// Gets or sets the strFullClassName.
        /// </summary>
        public string FullClassName { get; set; } = "dMobile.SubmitBatchSettings";

        /// <summary>
        /// Gets or sets the strFilePath.
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the strFileASCIIZ.
        /// </summary>
        public string FileASCIIZ { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the strFileName.
        /// </summary>
        public string FileName { get; set; } = "dMobileSubmitBatchSettings.xml";

        /// <summary>
        /// Load the data from the xml file.
        /// </summary>
        /// <returns>Submit batch settings.</returns>
        public SubmitBatchSettings LoadData()
        {
            lock (this.readLockObject)
            {
                var serializer = new XmlSerializer(typeof(SubmitBatchSettings));
                var settings = new SubmitBatchSettings();

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
                    settings = (SubmitBatchSettings)serializer.Deserialize(reader);
                }

                return settings;
            }
        }

        /// <summary>
        /// Save the data to the xml file.
        /// </summary>
        /// <param name="settings">Submit batch settings.</param>
        /// <returns>True if the settings were saved, else false.</returns>
        public bool SaveData(ref SubmitBatchSettings settings)
        {
            lock (this.writeLockObject)
            {
                var serializer = new XmlSerializer(typeof(SubmitBatchSettings));
                var saveComplete = false;

                try
                {
                    // use reflection to get the current assembly path for the database
                    var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    var settingsFileName = Path.Combine(appDir, this.FileName);

                    // Create a FileStream to write with.
                    using (var writer = new FileStream(settingsFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
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
