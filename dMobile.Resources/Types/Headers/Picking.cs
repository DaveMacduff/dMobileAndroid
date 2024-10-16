// <copyright file="Picking.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Headers
{
    using System;

    /// <summary>
    /// Represents a picking activity.
    /// </summary>
    public class Picking
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
        /// Gets or sets the name of the warehouse location map.
        /// </summary>
        public string LocationMapWarehouseName { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the picking was ordered.
        /// </summary>
        public DateTime ActivityPickingOrderedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the picking is to be delivered.
        /// </summary>
        public DateTime ActivityPickingDeliveryDateTime { get; set; }

        /// <summary>
        /// Gets or sets the name of the customer contact for the picking activity.
        /// </summary>
        public string ActivityPickingCustomerContactName { get; set; }

        /// <summary>
        /// Gets or sets the shipping information of the customer for the picking activity.
        /// </summary>
        public string ActivityPickingCustomerShipToInfo { get; set; }

        /// <summary>
        /// Gets or sets the route code for the picking activity.
        /// </summary>
        public string RouteCode { get; set; }

        /// <summary>
        /// Gets or sets the description of the route for the picking activity.
        /// </summary>
        public string RouteDescription { get; set; }

        /// <summary>
        /// Gets or sets the day of the week for the route.
        /// </summary>
        public string RouteDayOfWeek { get; set; }

        /// <summary>
        /// Gets or sets the stop number for the route.
        /// </summary>
        public int RouteStop { get; set; }

        /// <summary>
        /// Gets or sets the status of the activity.
        /// </summary>
        public int ActivityStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the picking activity has been reviewed.
        /// </summary>
        public bool ActivityPickingIsReviewed { get; set; }
    }
}