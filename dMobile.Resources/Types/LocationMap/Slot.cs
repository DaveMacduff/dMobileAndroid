// <copyright file="Slot.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.LocationMap
{
    /// <summary>
    /// Represents a slot in the location map.
    /// </summary>
    public class Slot
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
        /// Gets or sets the reference number of the section location map.
        /// </summary>
        public int SectionLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the slot code of the location map.
        /// </summary>
        public int LocationMapSlotCode { get; set; }

        /// <summary>
        /// Gets or sets the slot name of the location map.
        /// </summary>
        public string LocationMapSlotName { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the first slot coordinate location map.
        /// </summary>
        public int SlotCoord1LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the second slot coordinate location map.
        /// </summary>
        public int SlotCoord2LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the third slot coordinate location map.
        /// </summary>
        public int SlotCoord3LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the location map slot.
        /// </summary>
        public int LocationMapSlotFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the location map slot.
        /// </summary>
        public string LocationMapSlotBarCode { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the location map slot coordinate.
        /// </summary>
        public int LocationMapSlotCoordinateRefno { get; set; }

        /// <summary>
        /// Gets or sets the enum value of the location map slot coordinate.
        /// </summary>
        public int LocationMapSlotCoordinateEnum { get; set; }

        /// <summary>
        /// Gets or sets the code of the location map slot coordinate.
        /// </summary>
        public string LocationMapSlotCoordinateCode { get; set; }
    }
}