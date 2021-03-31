namespace dMobile.Bedrock
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

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
        private static readonly object lockObject = new object();
        private Int32 m_iSortBy1Index = 0;
        private string m_strSortBy1Value = "ActivityTransferFKRefno ASC";
        private Int32 m_iSortBy2Index = -1;
        private string m_strSortBy2Value = string.Empty;
        private Int32 m_iSortBy3Index = -1;
        private string m_strSortBy3Value = string.Empty;
        private Int32 m_iSortBy4Index = -1;
        private string m_strSortBy4Value = string.Empty;
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
        public Int32 iSortBy3Index
        {
            get { return m_iSortBy3Index; }
            set { m_iSortBy3Index = value; }
        }
        public string strSortBy3Value
        {
            get { return m_strSortBy3Value; }
            set { m_strSortBy3Value = value; }
        }
        public Int32 iSortBy4Index
        {
            get { return m_iSortBy4Index; }
            set { m_iSortBy4Index = value; }
        }
        public string strSortBy4Value
        {
            get { return m_strSortBy4Value; }
            set { m_strSortBy4Value = value; }
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
        public string GetDataViewSortBy()
        {
            string strSortBy = strSortBy1Value;

            if (!string.IsNullOrEmpty(strSortBy2Value))
            {
                strSortBy = strSortBy + ", " + strSortBy2Value;
            }
            if (!string.IsNullOrEmpty(strSortBy3Value))
            {
                strSortBy = strSortBy + ", " + strSortBy3Value;
            }
            if (!string.IsNullOrEmpty(strSortBy4Value))
            {
                strSortBy = strSortBy + ", " + strSortBy4Value;
            }
            return strSortBy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public TransferSettings LoadData()
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oTransferSettings"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool SaveData(ref TransferSettings oTransferSettings)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TransferSettings));
            bool bSaveComplete = false;

            lock (lockObject)
            {
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
            }

            return bSaveComplete;
        }
    }
}
