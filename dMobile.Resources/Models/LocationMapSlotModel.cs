// <copyright file="LocationMapSlotModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a slot in a location map.
    /// </summary>
    [Table("tLocationMapSlot")]
    public class LocationMapSlotModel
    {
        /// <summary>
        /// Gets or sets the reference number of the location map.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(LocationMapModel))]
        [Indexed(Name = "UQ_LocationMapSlotOnLocationMapRefnoAndLocationMapSubType", Order = 1, Unique = true)]
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the location map.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_LocationMapSlotOnLocationMapRefnoAndLocationMapSubType", Order = 2, Unique = true)]
        public int LocationMapSubType { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the section location map.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(LocationMapSectionModel))]
        [Indexed(Name = "FK_LocationMapSlotOnSectionLocationMapRefno", Order = 1, Unique = false)]
        public int SectionLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the code of the location map slot.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_LocationMapSlotOnLocationMapSlotCode", Order = 1, Unique = true)]
        public int LocationMapSlotCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the location map slot.
        /// </summary>
        [NotNull]
        public string LocationMapSlotName { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the first slot coordinate.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(LocationMapSlotCoordinateModel))]
        [Indexed(Name = "FK_LocationMapSlotOnSlotCoord1LocationMapRefno", Order = 1, Unique = false)]
        public int SlotCoord1LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the second slot coordinate.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(LocationMapSlotCoordinateModel))]
        [Indexed(Name = "FK_LocationMapSlotOnSlotCoord2LocationMapRefno", Order = 1, Unique = false)]
        public int SlotCoord2LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the third slot coordinate.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(LocationMapSlotCoordinateModel))]
        [Indexed(Name = "FK_LocationMapSlotOnSlotCoord3LocationMapRefno", Order = 1, Unique = false)]
        public int SlotCoord3LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the location map slot.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_LocationMapSlotOnLocationMapSlotFKRefno", Order = 1, Unique = true)]
        public int LocationMapSlotFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the location map slot.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_LocationMapSlotOnLocationMapSlotBarCode", Order = 1, Unique = true)]
        public string LocationMapSlotBarCode { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        [NotNull]
        [Indexed(Name = "FK_LocationMapSlotOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }
    }
}
