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

        /// <summary>
        /// Defines the refreshedDataLast.
        /// </summary>
        private DateTime refreshedDataLast = new DateTime(1900, 1, 1);

        /// <summary>
        /// Gets or sets a value indicating whether IsRefreshDataFailed.
        /// </summary>
        public bool IsRefreshDataFailed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDataCreatedInStartUp.
        /// </summary>
        public bool IsDataCreatedInStartUp { get; set; }

        /// <summary>
        /// Gets or sets the RefreshedDataLast.
        /// </summary>
        public DateTime RefreshedDataLast
        {
            get => this.refreshedDataLast;

            set => this.refreshedDataLast = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether PurchaseOrderWorkSheet.
        /// </summary>
        public bool PurchaseOrderWorkSheet { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Transfers.
        /// </summary>
        public bool Transfers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SystemData.
        /// </summary>
        public bool SystemData { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether Adjustments.
        /// </summary>
        public bool Adjustments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether PurchaseOrders.
        /// </summary>
        public bool PurchaseOrders { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SalesOrders.
        /// </summary>
        public bool SalesOrders { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ProductionTickets.
        /// </summary>
        public bool ProductionTickets { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Replenishment.
        /// </summary>
        public bool Replenishment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Counts.
        /// </summary>
        public bool Counts { get; set; }

        /// <summary>
        /// The LoadData.
        /// </summary>
        /// <returns>.</returns>
        public RefreshDataOptions LoadData()
        {
            var serializer = new XmlSerializer(typeof(RefreshDataOptions));
            var refreshDataOptions = new RefreshDataOptions();

            // use reflection to get the current assembly path for the database
            var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var fullPathToMyFile = Path.Combine(appDir, this.fileName);

            if (!File.Exists(fullPathToMyFile))
            {
                this.SaveData(ref refreshDataOptions);
            }

            // Create a FileStream to read with.
            using var reader = new FileStream(fullPathToMyFile, FileMode.Open);

            // Deserialize the stream to the object, and close the TextReader
            refreshDataOptions = (RefreshDataOptions)serializer.Deserialize(reader);
            reader.Close();

            return refreshDataOptions;
        }

        /// <summary>
        /// The SaveData.
        /// </summary>
        /// <param name="options">.</param>
        /// <returns>True if save successful else false.</returns>
        public bool SaveData(ref RefreshDataOptions options)
        {
            var serializer = new XmlSerializer(typeof(RefreshDataOptions));
            bool isSaveComplete;
            try
            {
                // use reflection to get the current assembly path for the database
                var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var fullPathToMyFile = Path.Combine(appDir, this.fileName);

                // Create a FileStream to write with.
                using (var writer = new FileStream(fullPathToMyFile, FileMode.Create))
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
