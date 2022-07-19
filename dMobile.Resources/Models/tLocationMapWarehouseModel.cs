// <copyright file="tLocationMapWarehouseModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tLocationMapWarehouse")]
    public class tLocationMapWarehouseModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(tLocationMapModel))]
        [Indexed(Name = "UQ_LocationMapWarehouseOnLocationMapRefnoAndLocationMapSubType", Order = 1, Unique = true)]
        public int LocationMapRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_LocationMapWarehouseOnLocationMapRefnoAndLocationMapSubType", Order = 2, Unique = true)]
        public int LocationMapSubType { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_LocationMapWarehouseOnLocationMapWarehouseName", Order = 1, Unique = true)]
        public string LocationMapWarehouseName { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_LocationMapWarehouseOnLocationMapWarehouseCode", Order = 1, Unique = true)]
        public int LocationMapWarehouseCode { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno", Order = 1, Unique = true)]
        public int LocationMapWarehouseFKRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_LocationMapWarehouseOnLocationMapWarehouseBarCode", Order = 1, Unique = true)]
        public string LocationMapWarehouseBarCode { get; set; }

        [NotNull]
        public bool LocationMapWarehouseIsWMS { get; set; }

        [NotNull]
        public string LocationMapWarehouseMask { get; set; }

        [NotNull]
        public string LocationMapWarehouseSectionMask { get; set; }

        [NotNull]
        public string LocationMapWarehouseSlotMask { get; set; }

        [NotNull]
        public string LocationMapWarehouseSlotCoordinate1Mask { get; set; }

        [NotNull]
        public string LocationMapWarehouseSlotCoordinate2Mask { get; set; }

        [NotNull]
        public string LocationMapWarehouseSlotCoordinate3Mask { get; set; }

        [NotNull]
        public string LocationMapWarehouseSlotCoordinate1Name { get; set; }

        [NotNull]
        public string LocationMapWarehouseSlotCoordinate2Name { get; set; }

        [NotNull]
        public string LocationMapWarehouseSlotCoordinate3Name { get; set; }

        [NotNull]
        public bool LocationMapWarehouseIsSectionTracked { get; set; }

        [NotNull]
        public bool LocationMapWarehouseIsSlotTracked { get; set; }

        [NotNull]
        public bool LocationMapWarehouseAdhocEnabled { get; set; }
    }
}
