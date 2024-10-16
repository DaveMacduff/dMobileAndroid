// <copyright file="Receive.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Headers
{
    using System;

    /// <summary>
    /// Represents a receiving activity.
    /// </summary>
    public class Receive
    {
        /// <summary>
        /// Gets or sets the reference number of the activity.
        /// </summary>
        public int ActivityRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity.
        /// </summary>
        public int ActivitySubType { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity.
        /// </summary>
        public int ActivityFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the string foreign key reference number of the activity.
        /// </summary>
        public string ActivityStringFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the purchase order number for the receiving activity.
        /// </summary>
        public string ActivityReceivingPONumber { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the receiving activity was issued.
        /// </summary>
        public DateTime ActivityReceivingIssuedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the receiving activity is expected to arrive.
        /// </summary>
        public DateTime ActivityReceivingArrivalDateTime { get; set; }

        /// <summary>
        /// Gets or sets the name of the vendor contact for the receiving activity.
        /// </summary>
        public string ActivityReceivingVendorContactName { get; set; }

        /// <summary>
        /// Gets or sets the name of the supplier contact for the receiving activity.
        /// </summary>
        public string ActivityReceivingVendorSupplierContactName { get; set; }

        /// <summary>
        /// Gets or sets the GTIN company prefix of the vendor supplier for the receiving activity.
        /// </summary>
        public string ActivityReceivingVendorSupplierGTINCompanyPrefix { get; set; }
    }
}