// <copyright file="LocationMapWarehouseModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models.LocationMap
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a warehouse location map.
    /// </summary>
    [Table("tLocationMapWarehouse")]
    public class LocationMapWarehouseModel
    {
        /// <summary>
        /// Gets or sets the reference number of the location map.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(LocationMapModel))]
        [Indexed(Name = "UQ_LocationMapWarehouseOnLocationMapRefnoAndLocationMapSubType", Order = 1, Unique = true)]
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the location map.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_LocationMapWarehouseOnLocationMapRefnoAndLocationMapSubType", Order = 2, Unique = true)]
        public int LocationMapSubType { get; set; }

        /// <summary>
        /// Gets or sets the name of the warehouse location map.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_LocationMapWarehouseOnLocationMapWarehouseName", Order = 1, Unique = true)]
        public string LocationMapWarehouseName { get; set; }

        /// <summary>
        /// Gets or sets the code of the warehouse location map.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_LocationMapWarehouseOnLocationMapWarehouseCode", Order = 1, Unique = true)]
        public int LocationMapWarehouseCode { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the warehouse location map.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno", Order = 1, Unique = true)]
        public int LocationMapWarehouseFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the warehouse location map.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_LocationMapWarehouseOnLocationMapWarehouseBarCode", Order = 1, Unique = true)]
        public string LocationMapWarehouseBarCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the warehouse is WMS (Warehouse Management System) enabled.
        /// </summary>
        [NotNull]
        public bool LocationMapWarehouseIsWMS { get; set; }

        /// <summary>
        /// Gets or sets the mask for the warehouse location map.
        /// </summary>
        [NotNull]
        public string LocationMapWarehouseMask { get; set; }

        /// <summary>
        /// Gets or sets the section mask for the warehouse location map.
        /// </summary>
        [NotNull]
        public string LocationMapWarehouseSectionMask { get; set; }

        /// <summary>
        /// Gets or sets the slot mask for the warehouse location map.
        /// </summary>
        [NotNull]
        public string LocationMapWarehouseSlotMask { get; set; }

        /// <summary>
        /// Gets or sets the first coordinate mask for the warehouse slot.
        /// </summary>
        [NotNull]
        public string LocationMapWarehouseSlotCoordinate1Mask { get; set; }

        /// <summary>
        /// Gets or sets the second coordinate mask for the warehouse slot.
        /// </summary>
        [NotNull]
        public string LocationMapWarehouseSlotCoordinate2Mask { get; set; }

        /// <summary>
        /// Gets or sets the third coordinate mask for the warehouse slot.
        /// </summary>
        [NotNull]
        public string LocationMapWarehouseSlotCoordinate3Mask { get; set; }

        /// <summary>
        /// Gets or sets the name of the first coordinate for the warehouse slot.
        /// </summary>
        [NotNull]
        public string LocationMapWarehouseSlotCoordinate1Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the second coordinate for the warehouse slot.
        /// </summary>
        [NotNull]
        public string LocationMapWarehouseSlotCoordinate2Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the third coordinate for the warehouse slot.
        /// </summary>
        [NotNull]
        public string LocationMapWarehouseSlotCoordinate3Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the warehouse is section tracked.
        /// </summary>
        [NotNull]
        public bool LocationMapWarehouseIsSectionTracked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the warehouse is slot tracked.
        /// </summary>
        [NotNull]
        public bool LocationMapWarehouseIsSlotTracked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the warehouse allows ad-hoc operations.
        /// </summary>
        [NotNull]
        public bool LocationMapWarehouseAdhocEnabled { get; set; }
    }
}