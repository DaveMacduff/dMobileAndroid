namespace dMobile.Bedrock
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    [Serializable]
    public class ProductionTicketSettings
    {
        //VARIABLES
        [NonSerialized()]
        private string m_strFullClassName = "dMobile.ProductionTicketSettings";
        [NonSerialized()]
        private string m_strFilePath = string.Empty;
        [NonSerialized()]
        private string m_strFileASCIIZ = string.Empty;
        [NonSerialized()]
        private string m_strFileName = "dMobileProductionTicketSettings.xml";
        [NonSerialized]
        private static readonly object lockObject = new object();
        private Int32 m_iSortBy1Index = 0;
        private string m_strSortBy1Value = "ActivityProductionFKRefno ASC";
        private Int32 m_iSortBy2Index = -1;
        private string m_strSortBy2Value = string.Empty;
        private Int32 m_iSortBy3Index = -1;
        private string m_strSortBy3Value = string.Empty;
        private DateTime productionDateFilter = new System.DateTime(1900, 1, 1);
        private bool isApplyProductionDateFilter = false;

        //PROPERTIES
        public DateTime ProductionDateFilter
        {
            get { return productionDateFilter; }
            set { productionDateFilter = value; }
        }
        public bool IsApplyProductionDateFilter
        {
            get { return isApplyProductionDateFilter; }
            set { isApplyProductionDateFilter = value; }
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

        //METHODS

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public ProductionTicketSettings()
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
        public ProductionTicketSettings LoadData()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ProductionTicketSettings));
            ProductionTicketSettings oProductionTicketSettings = new ProductionTicketSettings();

            // use relection to get the current assembly path for the database
            string strAppDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string strFullPathToMyFile = Path.Combine(strAppDir, m_strFileName);

            if (!File.Exists(strFullPathToMyFile))
            {
                SaveData(ref oProductionTicketSettings);
            }

            // Create a FileStream to read with.
            using (FileStream reader = new FileStream(strFullPathToMyFile, FileMode.Open))
            {
                // Deserialize the stream to the object, and close the TextReader
                oProductionTicketSettings = (ProductionTicketSettings)serializer.Deserialize(reader);
            }

            return oProductionTicketSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oProductionTicketSettings"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool SaveData(ref ProductionTicketSettings oProductionTicketSettings)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ProductionTicketSettings));
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
                        // Serialize the object, and close the TextWriter
                        serializer.Serialize(writer, oProductionTicketSettings);
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
