// <copyright file="Return.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Headers
{
    using System;

    /// <summary>
    /// Represents a return activity.
    /// </summary>
    public class Return
    {
        /// <summary>
        /// Gets or sets the reference number of the activity.
        /// </summary>
        public int ActivityRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity.
        /// </summary>
        public int ActivityFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the string foreign key reference number of the activity.
        /// </summary>
        public string ActivityStringFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the return activity was ordered.
        /// </summary>
        public DateTime ActivityReturnsOrderedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the return activity is scheduled for delivery.
        /// </summary>
        public DateTime ActivityReturnsDeliveryDateTime { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the location map.
        /// </summary>
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the warehouse location map.
        /// </summary>
        public int LocationMapWarehouseFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the name of the warehouse location map.
        /// </summary>
        public string LocationMapWarehouseName { get; set; }

        /// <summary>
        /// Gets or sets the name of the customer contact for the return activity.
        /// </summary>
        public string ActivityReturnsCustomerContactName { get; set; }

        /// <summary>
        /// Gets or sets the shipping information for the customer.
        /// </summary>
        public string ActivityReturnsCustomerShipToInfo { get; set; }

        /// <summary>
        /// Gets or sets the status of the return activity.
        /// </summary>
        public int ActivityStatus { get; set; }
    }
}