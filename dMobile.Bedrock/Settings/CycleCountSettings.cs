// ----------------------------------------------------------------------
// <copyright file="CycleCountSettings.cs" company="InterSect Business Systems Inc.">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------
namespace dMobile.Bedrock
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Xml.Serialization;

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
        private string m_strSortBy1Value = "ActivityCountFKRefno ASC";
        private Int32 m_iSortBy2Index = -1;
        private string m_strSortBy2Value = string.Empty;
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
        public string strSortBy1Value
        {
            get { return m_strSortBy1Value; }
            set { m_strSortBy1Value = value; }
        }
        public Int32 iSortBy2Index
        {
            get { return m_iSortBy2Index; }
            set { m_iSortBy2Index = value; }
        }
        public string strSortBy2Value
        {
            get { return m_strSortBy2Value; }
            set { m_strSortBy2Value = value; }
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
        public string GetDataViewSortBy()
        {
            string strSortBy = strSortBy1Value;

            if (!string.IsNullOrEmpty(strSortBy2Value))
            {
                strSortBy = strSortBy + ", " + strSortBy2Value;
            }
            return strSortBy;
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
                using (FileStream reader = new FileStream(strFullPathToMyFile, FileMode.Open))
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
                    using (FileStream writer = new FileStream(strFullPathToMyFile, FileMode.Create))
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
