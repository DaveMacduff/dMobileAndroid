// <copyright file="ProductSchemeData.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Define the product scheme data.
    /// </summary>
    public class ProductSchemeData
    {
        /// <summary>
        /// Gets or sets the single trade item serial number.
        /// </summary>
        public string SingleTradeItemSerialNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the logistic unit batch lot number.
        /// </summary>
        public string LogisticUnitBatchLotNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the logistic unit best before sell by date.
        /// </summary>
        public DateTime LogisticUnitBestBeforeSellByDate { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        /// Gets or sets the logistic unit expiry use by date.
        /// </summary>
        public DateTime LogisticUnitExpiryUseByDate { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        /// Gets or sets the logistic unit production date.
        /// </summary>
        public DateTime LogisticUnitProductionDate { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        /// Gets or sets the logistic unit due date.
        /// </summary>
        public DateTime LogisticUnitDueDate { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        /// Gets or sets the logistic unit packaging date.
        /// </summary>
        public DateTime LogisticUnitPackagingDate { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        /// Gets or sets the logistic unit skid number.
        /// </summary>
        public string LogisticUnitSkidNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the variant code.
        /// </summary>
        public string VariantCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the variant number.
        /// </summary>
        public int VariantNumber { get; set; } = 0;

        /// <summary>
        /// Gets or sets variant description.
        /// </summary>
        public string VariantDescription { get; set; } = string.Empty;
    }
}
