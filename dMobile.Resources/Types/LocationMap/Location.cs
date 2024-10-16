// <copyright file="Location.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.LocationMap
{
    /// <summary>
    /// Represents a location in the location map.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Gets or sets the warehouse code of the location map.
        /// </summary>
        public int LocationMapWarehouseCode { get; set; }

        /// <summary>
        /// Gets or sets the section name of the location map.
        /// </summary>
        public string LocationMapSectionName { get; set; }

        /// <summary>
        /// Gets or sets the slot name of the location map.
        /// </summary>
        public string LocationMapSlotName { get; set; }

        /// <summary>
        /// Gets or sets the type of the location map.
        /// </summary>
        public string LocationMapType { get; set; }
    }
}