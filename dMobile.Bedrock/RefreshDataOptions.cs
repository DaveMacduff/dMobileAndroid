// <copyright file="RefreshDataOptions.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the <see cref="RefreshDataOptions" />.
    /// </summary>
    [Serializable]
    public class RefreshDataOptions
    {
        /// <summary>
        /// Defines the fileName.
        /// </summary>
        [NonSerialized]
        private string fileName = "dMobileRefreshDataOptions.xml";

        // <issue="15668" author="RD" timestamp="2018-01-02" comment="refreshDataChangesOnly=false; and systemData=true is now default for when an autoupdate occurs and clears these properties." />
        /// <summary>
        /// Defines the refreshDataChangesOnly.
        /// </summary>
        private bool refreshDataChangesOnly = false;

        /// <summary>
        /// Defines the systemData.
        /// </summary>
        private bool systemData = true;

        /// <summary>
        /// Defines the adjustments.
        /// </summary>
        private bool adjustments = false;

        /// <summary>
        /// Defines the purchaseOrders.
        /// </summary>
        private bool purchaseOrders = false;

        /// <summary>
        /// Defines the salesOrders.
        /// </summary>
        private bool salesOrders = false;

        /// <summary>
        /// Defines the productionTickets.
        /// </summary>
        private bool productionTickets = false;

        /// <summary>
        /// Defines the replenishment.
        /// </summary>
        private bool replenishment = false;

        /// <summary>
        /// Defines the counts.
        /// </summary>
        private bool counts = false;

        /// <summary>
        /// Defines the transfers.
        /// </summary>
        private bool transfers = false;

        /// <summary>
        /// Defines the purchaseOrderWorkSheet.
        /// </summary>
        private bool purchaseOrderWorkSheet = false;

        // private DateTime refreshedDataLast = DateAndTime.DateSerial(1900, 1, 1);
        /// <summary>
        /// Defines the refreshedDataLast.
        /// </summary>
        private DateTime refreshedDataLast = new DateTime(1900, 1, 1);

        /// <summary>
        /// Defines the isRefreshedLastDataChangesOnly.
        /// </summary>
        private bool isRefreshedLastDataChangesOnly = false;

        /// <summary>
        /// Defines the isDataCreatedOnStartUp.
        /// </summary>
        private bool isDataCreatedOnStartUp = false;

        /// <summary>
        /// Defines the isRefreshDataFailed.
        /// </summary>
        private bool isRefreshDataFailed = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsRefreshDataFailed.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsDataCreatedInStartUp.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsRefreshedLastDataChangesOnly.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the RefreshedDataLast.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether PurchaseOrderWorkSheet.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether Transfers.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether RefreshDataChangesOnly.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether SystemData.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether Adjustments.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether PurchaseOrders.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether SalesOrders.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether ProductionTickets.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether Replenishment.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether Counts.
        /// </summary>
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

        /// <summary>
        /// The LoadData.
        /// </summary>
        /// <returns>.</returns>
        public RefreshDataOptions LoadData()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(RefreshDataOptions));
            RefreshDataOptions refreshDataOptions = new RefreshDataOptions();

            // use reflection to get the current assembly path for the database
            string appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
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
        /// The SaveData.
        /// </summary>
        /// <param name="options">.</param>
        /// <returns>.</returns>
        public bool SaveData(ref RefreshDataOptions options)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(RefreshDataOptions));
            bool isSaveComplete;
            try
            {
                // use reflection to get the current assembly path for the database
                string appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
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
    }
}
