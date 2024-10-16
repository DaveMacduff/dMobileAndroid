// <copyright file="Section.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.LocationMap
{
    /// <summary>
    /// Represents a section in the location map.
    /// </summary>
    public class Section
    {
        /// <summary>
        /// Gets or sets the reference number of the location map.
        /// </summary>
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the location map.
        /// </summary>
        public int LocationMapSubType { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the section code of the location map.
        /// </summary>
        public int LocationMapSectionCode { get; set; }

        /// <summary>
        /// Gets or sets the section name of the location map.
        /// </summary>
        public string LocationMapSectionName { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the location map section.
        /// </summary>
        public int LocationMapSectionFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the location map section.
        /// </summary>
        public string LocationMapSectionBarCode { get; set; }
    }
}