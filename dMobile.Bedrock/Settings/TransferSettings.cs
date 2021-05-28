namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;

    [Serializable]
    public class TransferSettings
    {
        //VARIABLES
        [NonSerialized()]
        private string m_strFullClassName = "dMobile.TransferSettings";
        [NonSerialized()]
        private string m_strFilePath = string.Empty;
        [NonSerialized()]
        private string m_strFileASCIIZ = string.Empty;
        [NonSerialized()]
        private string m_strFileName = "dMobileTransferSettings.xml";
        [NonSerialized]
        private readonly object writeLockObject = new object();
        [NonSerialized]
        private readonly object readLockObject = new object();
        private Int32 m_iSortBy1Index = 0;
        private Sort sortBy1Value = new Sort() { SortValue = "ActivityTransferFKRefno", SortOrder = SortOrder.ASC };
        private Int32 m_iSortBy2Index = -1;
        private Sort sortBy2Value = null;
        private Int32 m_iSortBy3Index = -1;
        private Sort sortBy3Value = null;
        private Int32 m_iSortBy4Index = -1;
        private Sort sortBy4Value = null;
        private bool isApplyTransferDateFilter = false;
        private DateTime transferDateFilter = new System.DateTime(1900, 1, 1);

        //PROPERTIES
        public bool IsApplyTransferDateFilter
        {
            get { return isApplyTransferDateFilter; }
            set { isApplyTransferDateFilter = value; }
        }
        public DateTime TransferDateFilter
        {
            get { return transferDateFilter; }
            set { transferDateFilter = value; }
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
        public Int32 iSortBy3Index
        {
            get { return m_iSortBy3Index; }
            set { m_iSortBy3Index = value; }
        }
        public Sort SortBy3Value
        {
            get { return sortBy3Value; }
            set { sortBy3Value = value; }
        }
        public Int32 iSortBy4Index
        {
            get { return m_iSortBy4Index; }
            set { m_iSortBy4Index = value; }
        }
        public Sort SortBy4Value
        {
            get { return sortBy4Value; }
            set { sortBy4Value = value; }
        }

        //METHODS

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public TransferSettings()
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

            sortBy.Add(SortBy1Value);

            if (SortBy2Value != null && !string.IsNullOrEmpty(SortBy2Value.SortValue))
            {
                sortBy.Add(SortBy2Value);
            }

            if (SortBy3Value != null && !string.IsNullOrEmpty(SortBy3Value.SortValue))
            {
                sortBy.Add(SortBy3Value);
            }

            if (SortBy4Value != null && !string.IsNullOrEmpty(SortBy4Value.SortValue))
            {
                sortBy.Add(SortBy4Value);
            }

            return sortBy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public TransferSettings LoadData()
        {
            lock (readLockObject)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TransferSettings));
                TransferSettings oTransferSettings = new TransferSettings();

                // use relection to get the current assembly path for the database
                string strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string strFullPathToMyFile = Path.Combine(strAppDir, m_strFileName);

                if (!File.Exists(strFullPathToMyFile))
                {
                    SaveData(ref oTransferSettings);
                }

                // Create a FileStream to read with.
                using (FileStream reader = new FileStream(strFullPathToMyFile, FileMode.Open))
                {
                    // Deserialize the stream to the object, and close the TextReader
                    oTransferSettings = (TransferSettings)serializer.Deserialize(reader);
                }

                return oTransferSettings;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oTransferSettings"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool SaveData(ref TransferSettings oTransferSettings)
        {
            lock (writeLockObject)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TransferSettings));
                bool bSaveComplete = false;

                try
                {
                    // use relection to get the current assembly path for the database
                    string strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    string strFullPathToMyFile = Path.Combine(strAppDir, m_strFileName);

                    // Create a FileStream to write with.
                    using (FileStream writer = new FileStream(strFullPathToMyFile, FileMode.Create))
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
