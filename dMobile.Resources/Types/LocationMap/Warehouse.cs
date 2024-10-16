// <copyright file="Warehouse.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.LocationMap
{
    /// <summary>
    /// Represents a warehouse in the location map.
    /// </summary>
    public class Warehouse
    {
        /// <summary>
        /// Gets or sets the display name of the warehouse.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the location map.
        /// </summary>
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the sub type of the location map.
        /// </summary>
        public int LocationMapSubType { get; set; }

        /// <summary>
        /// Gets or sets the name of the warehouse in the location map.
        /// </summary>
        public string LocationMapWarehouseName { get; set; }

        /// <summary>
        /// Gets or sets the code of the warehouse in the location map.
        /// </summary>
        public int LocationMapWarehouseCode { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the warehouse in the location map.
        /// </summary>
        public int LocationMapWarehouseFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the warehouse in the location map.
        /// </summary>
        public string LocationMapWarehouseBarCode { get; set; }
    }
}