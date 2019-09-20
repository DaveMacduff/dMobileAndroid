namespace dMobile.Bedrock
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Xml.Serialization;

    [Serializable]
    public class RefreshDataOptions
    {
        #region Fields

        [NonSerialized]
        private string fileName = "dMobileRefreshDataOptions.xml";

        //<issue="15668" author="RD" timestamp="2018-01-02" comment="refreshDataChangesOnly=false; and systemData=true is now default for when an autoupdate occurs and clears these properties." />
        private bool refreshDataChangesOnly = false;
        private bool systemData = true;
        private bool adjustments = false;
        private bool purchaseOrders = false;
        private bool salesOrders = false;
        private bool productionTickets = false;
        private bool replenishment = false;
        private bool counts = false;
        private bool transfers = false;
        private bool purchaseOrderWorkSheet = false;
        //private DateTime refreshedDataLast = DateAndTime.DateSerial(1900, 1, 1);
        private DateTime refreshedDataLast = DateTime.Now;
        private bool isRefreshedLastDataChangesOnly = false;
        private bool isDataCreatedOnStartUp = false;
        private bool isRefreshDataFailed = false;

        #endregion

        #region Properties

        public bool IsRefreshDataFailed
        {
            get
            {
                return this.isRefreshDataFailed;
            }

            set
            {
                this.isRefreshDataFailed = value;
            }
        }

        public bool IsDataCreatedInStartUp
        {
            get
            {
                return this.isDataCreatedOnStartUp;
            }

            set
            {
                this.isDataCreatedOnStartUp = value;
            }
        }

        public bool IsRefreshedLastDataChangesOnly
        {
            get
            {
                return this.isRefreshedLastDataChangesOnly;
            }

            set
            {
                this.isRefreshedLastDataChangesOnly = value;
            }
        }

        public DateTime RefreshedDataLast
        {
            get
            {
                return this.refreshedDataLast;
            }

            set
            {
                this.refreshedDataLast = value;
            }
        }

        public bool PurchaseOrderWorkSheet
        {
            get
            {
                return this.purchaseOrderWorkSheet;
            }

            set
            {
                this.purchaseOrderWorkSheet = value;
            }
        }

        public bool Transfers
        {
            get
            {
                return this.transfers;
            }

            set
            {
                this.transfers = value;
            }
        }

        public bool RefreshDataChangesOnly
        {
            get
            {
                return this.refreshDataChangesOnly;
            }

            set
            {
                this.refreshDataChangesOnly = value;
            }
        }

        public bool SystemData
        {
            get
            {
                return this.systemData;
            }

            set
            {
                this.systemData = value;
            }
        }

        public bool Adjustments
        {
            get
            {
                return this.adjustments;
            }

            set
            {
                this.adjustments = value;
            }
        }

        public bool PurchaseOrders
        {
            get
            {
                return this.purchaseOrders;
            }

            set
            {
                this.purchaseOrders = value;
            }
        }

        public bool SalesOrders
        {
            get
            {
                return this.salesOrders;
            }

            set
            {
                this.salesOrders = value;
            }
        }

        public bool ProductionTickets
        {
            get
            {
                return this.productionTickets;
            }

            set
            {
                this.productionTickets = value;
            }
        }

        public bool Replenishment
        {
            get
            {
                return this.replenishment;
            }

            set
            {
                this.replenishment = value;
            }
        }

        public bool Counts
        {
            get
            {
                return this.counts;
            }

            set
            {
                this.counts = value;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public RefreshDataOptions LoadData()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(RefreshDataOptions));
            RefreshDataOptions refreshDataOptions = new RefreshDataOptions();

            // use reflection to get the current assembly path for the database
            string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            string fullPathToMyFile = Path.Combine(appDir, this.fileName);

            if (!File.Exists(fullPathToMyFile))
            {
                this.SaveData(ref refreshDataOptions);
            }

            // Create a FileStream to read with.
            using (FileStream reader = new FileStream(fullPathToMyFile, FileMode.Open))
            {
                // Deserialize the stream to the object, and close the TextReader
                refreshDataOptions = (RefreshDataOptions)serializer.Deserialize(reader);
                reader.Close();
            }

            return refreshDataOptions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public bool SaveData(ref RefreshDataOptions options)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(RefreshDataOptions));
            bool isSaveComplete = false;

            try
            {
                // use reflection to get the current assembly path for the database
                string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
                string fullPathToMyFile = Path.Combine(appDir, this.fileName);

                // Create a FileStream to write with.
                using (FileStream writer = new FileStream(fullPathToMyFile, FileMode.Create))
                {
                    // Serialize the object, and close the TextWriter
                    serializer.Serialize(writer, options);
                    writer.Close();
                }

                isSaveComplete = true;
            }
            catch
            {
                isSaveComplete = false;
            }

            return isSaveComplete;
        }

        #endregion
    }
}
