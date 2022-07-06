// <copyright file="tLocationMapSlotModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tLocationMapSlot")]
    public class tLocationMapSlotModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tLocationMapModel)), Indexed(Name = "UQ_LocationMapSlotOnLocationMapRefnoAndLocationMapSubType", Order = 1, Unique = true)]
        public int LocationMapRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_LocationMapSlotOnLocationMapRefnoAndLocationMapSubType", Order = 2, Unique = true)]
        public int LocationMapSubType { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapSectionModel)), Indexed(Name = "FK_LocationMapSlotOnSectionLocationMapRefno", Order = 1, Unique = false)]
        public int SectionLocationMapRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_LocationMapSlotOnLocationMapSlotCode", Order = 1, Unique = true)]
        public int LocationMapSlotCode { get; set; }

        [NotNull]
        public string LocationMapSlotName { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapSlotCoordinateModel)), Indexed(Name = "FK_LocationMapSlotOnSlotCoord1LocationMapRefno", Order = 1, Unique = false)]
        public int SlotCoord1LocationMapRefno { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapSlotCoordinateModel)), Indexed(Name = "FK_LocationMapSlotOnSlotCoord2LocationMapRefno", Order = 1, Unique = false)]
        public int SlotCoord2LocationMapRefno { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapSlotCoordinateModel)), Indexed(Name = "FK_LocationMapSlotOnSlotCoord3LocationMapRefno", Order = 1, Unique = false)]
        public int SlotCoord3LocationMapRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_LocationMapSlotOnLocationMapSlotFKRefno", Order = 1, Unique = true)]
        public int LocationMapSlotFKRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_LocationMapSlotOnLocationMapSlotBarCode", Order = 1, Unique = true)]
        public string LocationMapSlotBarCode { get; set; }

        [NotNull, Indexed(Name = "FK_LocationMapSlotOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }
    }
}
