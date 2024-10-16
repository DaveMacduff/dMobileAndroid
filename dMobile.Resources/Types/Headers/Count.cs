// <copyright file="Count.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Headers
{
    using System;

    /// <summary>
    /// Represents a count activity.
    /// </summary>
    public class Count
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
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the type of the count activity.
        /// </summary>
        public int ActivityCountType { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the count activity.
        /// </summary>
        public DateTime ActivityCountDateTime { get; set; }

        /// <summary>
        /// Gets or sets the description of the count type.
        /// </summary>
        public string ActivityCountTypeDescription { get; set; }

        /// <summary>
        /// Gets or sets the description of the count activity.
        /// </summary>
        public string ActivityCountDescription { get; set; }
    }
}