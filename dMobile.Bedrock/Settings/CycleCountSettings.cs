// ----------------------------------------------------------------------
// <copyright file="CycleCountSettings.cs" company="InterSect Business Systems Inc.">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------
namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml.Serialization;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;

    [Serializable()]
    public class CycleCountSettings
    {
        //VARIABLES
        [NonSerialized()]
        private string m_strFullClassName = "dMobile.CycleCountSettings";

        [NonSerialized()]
        private string m_strFilePath = string.Empty;

        [NonSerialized()]
        private string m_strFileASCIIZ = string.Empty;

        [NonSerialized()]
        private string m_strFileName = "dMobileCycleCountSettings.xml";

        [NonSerialized]
        private readonly object writeLockObject = new object();

        [NonSerialized]
        private readonly object readLockObject = new object();

        private Int32 m_iSortBy1Index = 0;
        private Sort sortBy1Value = new Sort() { SortValue = "ActivityCountFKRefno", SortOrder = SortOrder.ASC };
        private Int32 m_iSortBy2Index = -1;
        private Sort sortBy2Value = null;
        private System.DateTime m_dtCountDate = new System.DateTime(1900, 1, 1);
        private bool isApplyCountDate = false;

        //PROPERTIES
        public bool IsApplyCountDate
        {
            get { return isApplyCountDate; }
            set { isApplyCountDate = value; }
        }
        public System.DateTime dtCountDate
        {
            get { return m_dtCountDate; }
            set { m_dtCountDate = value; }
        }
        public string strFullClassName
        {
            get { return m_strFullClassName; }
            set { m_strFullClassName = value; }
        }
        public string strFilePath
        {
            get { return m_strFilePath; }
            set { m_strFilePath = value; }
        }
        public string strFileASCIIZ
        {
            get { return m_strFileASCIIZ; }
            set { m_strFileASCIIZ = value; }
        }
        public string strFileName
        {
            get { return m_strFileName; }
            set { m_strFileName = value; }
        }
        public Int32 iSortBy1Index
        {
            get { return m_iSortBy1Index; }
            set { m_iSortBy1Index = value; }
        }
        public Sort SortBy1Value
        {
            get { return sortBy1Value; }
            set { sortBy1Value = value; }
        }
        public Int32 iSortBy2Index
        {
            get { return m_iSortBy2Index; }
            set { m_iSortBy2Index = value; }
        }
        public Sort SortBy2Value
        {
            get { return sortBy2Value; }
            set { sortBy2Value = value; }
        }

        //METHODS

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public CycleCountSettings()
        {
            strFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            strFileASCIIZ = Path.Combine(strFilePath, strFileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<Sort> GetDataViewSortBy()
        {
            List<Sort> sortBy = new List<Sort>();

            sortBy.Add(this.SortBy1Value);

            if (this.SortBy2Value != null && !string.IsNullOrEmpty(SortBy2Value.SortValue))
            {
                sortBy.Add(this.SortBy2Value);
            }

            return sortBy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public CycleCountSettings LoadData()
        {
            lock (readLockObject)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CycleCountSettings));
                CycleCountSettings oCycleCountSettings = new CycleCountSettings();

                // use relection to get the current assembly path for the database
                string strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string strFullPathToMyFile = Path.Combine(strAppDir, m_strFileName);

                if (!File.Exists(strFullPathToMyFile))
                {
                    SaveData(ref oCycleCountSettings);
                }

                // Create a FileStream to read with.
                using (FileStream reader = new FileStream(strFullPathToMyFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    oCycleCountSettings = (CycleCountSettings)serializer.Deserialize(reader);
                }

                return oCycleCountSettings;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oCycleCountSettings"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool SaveData(ref CycleCountSettings oCycleCountSettings)
        {
            lock (writeLockObject)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CycleCountSettings));
                bool bSaveComplete = false;

                try
                {
                    // use relection to get the current assembly path for the database
                    string strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    string strFullPathToMyFile = Path.Combine(strAppDir, m_strFileName);

                    // Create a FileStream to write with.
                    using (FileStream writer = new FileStream(strFullPathToMyFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        serializer.Serialize(writer, oCycleCountSettings);
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
