// <copyright file="LocationMapSlotQueryResult.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.QueryResults
{
    /// <summary>
    /// Represents the result of a query for location map slots.
    /// </summary>
    public class LocationMapSlotQueryResult
    {
        /// <summary>
        /// Gets or sets the reference number of the location map.
        /// </summary>
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the code of the location map slot.
        /// </summary>
        public int LocationMapSlotCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the location map slot.
        /// </summary>
        public string LocationMapSlotName { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the location map slot.
        /// </summary>
        public int LocationMapSlotFKRefno { get; set; }

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
        /// Gets or sets the reference number of the section location map.
        /// </summary>
        public int SectionLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the code of the location map section.
        /// </summary>
        public int LocationMapSectionCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the location map section.
        /// </summary>
        public string LocationMapSectionName { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the location map section.
        /// </summary>
        public int LocationMapSectionFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the name of the warehouse.
        /// </summary>
        public string LocationMapWarehouseName { get; set; }

        /// <summary>
        /// Gets or sets the code of the warehouse.
        /// </summary>
        public int LocationMapWarehouseCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the warehouse is WMS.
        /// </summary>
        public bool LocationMapWarehouseIsWMS { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the warehouse.
        /// </summary>
        public int LocationMapWarehouseFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the mask of the warehouse.
        /// </summary>
        public string LocationMapWarehouseMask { get; set; }

        /// <summary>
        /// Gets or sets the section mask of the warehouse.
        /// </summary>
        public string LocationMapWarehouseSectionMask { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the warehouse is section tracked.
        /// </summary>
        public bool LocationMapWarehouseIsSectionTracked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the warehouse is slot tracked.
        /// </summary>
        public bool LocationMapWarehouseIsSlotTracked { get; set; }

        /// <summary>
        /// Gets or sets the name of the first slot coordinate of the warehouse.
        /// </summary>
        public string LocationMapWarehouseSlotCoordinate1Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the second slot coordinate of the warehouse.
        /// </summary>
        public string LocationMapWarehouseSlotCoordinate2Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the third slot coordinate of the warehouse.
        /// </summary>
        public string LocationMapWarehouseSlotCoordinate3Name { get; set; }

        /// <summary>
        /// Gets or sets the slot mask of the warehouse.
        /// </summary>
        public string LocationMapWarehouseSlotMask { get; set; }

        /// <summary>
        /// Gets or sets the mask of the first slot coordinate of the warehouse.
        /// </summary>
        public string LocationMapWarehouseSlotCoordinate1Mask { get; set; }

        /// <summary>
        /// Gets or sets the mask of the second slot coordinate of the warehouse.
        /// </summary>
        public string LocationMapWarehouseSlotCoordinate2Mask { get; set; }

        /// <summary>
        /// Gets or sets the mask of the third slot coordinate of the warehouse.
        /// </summary>
        public string LocationMapWarehouseSlotCoordinate3Mask { get; set; }
    }
}