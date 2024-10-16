// <copyright file="Transfer.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Headers
{
    using System;

    /// <summary>
    /// Represents a transfer activity.
    /// </summary>
    public class Transfer
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
        /// Gets or sets the reference number of the location map.
        /// </summary>
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the warehouse location map.
        /// </summary>
        public int LocationMapWarehouseFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the description of the warehouse location map.
        /// </summary>
        public string LocationMapWarehouseDescription { get; set; }

        /// <summary>
        /// Gets or sets the name of the warehouse location map.
        /// </summary>
        public string LocationMapWarehouseName { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the transfer is expected to arrive.
        /// </summary>
        public DateTime ActivityTransferArrivalDateTime { get; set; }

        /// <summary>
        /// Gets or sets the description of the transfer activity.
        /// </summary>
        public string ActivityTransferDescription { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the transfer activity.
        /// </summary>
        public DateTime ActivityTransferDateTime { get; set; }

        /// <summary>
        /// Gets or sets the status of the activity.
        /// </summary>
        public int ActivityStatus { get; set; }
    }
}