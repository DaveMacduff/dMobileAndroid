using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    ////// LocationMapSlotCode is a de-normalized agregate of the coordinates 1+2+3

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tLocationMapSlot"
    //    + "("
    //    + "LocationMapRefno INTEGER NOT NULL CONSTRAINT PK_LocationMapSlotOnLocationMapRefno PRIMARY KEY,"
    //    + "LocationMapSubType INTEGER NOT NULL,"
    //    + "SectionLocationMapRefno INTEGER NOT NULL,"
    //    + "LocationMapSlotCode INTEGER NOT NULL,"
    //    + "LocationMapSlotName nvarchar(10) NOT NULL,"
    //    + "SlotCoord1LocationMapRefno INTEGER NOT NULL,"
    //    + "SlotCoord2LocationMapRefno INTEGER NOT NULL,"
    //    + "SlotCoord3LocationMapRefno INTEGER NOT NULL,"
    //    + "LocationMapSlotFKRefno INTEGER NOT NULL,"
    //    + "LocationMapSlotBarCode nvarchar(13) NOT NULL,"
    //    + "WarehouseLocationMapRefno INTEGER NOT NULL,"

    //    + "CONSTRAINT LocationMapSlotToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE,"
    //    + "CONSTRAINT LocationMapSlotToLocationMapSectionOnSectionLocationMapRefnoAndLocationMapRefno FOREIGN KEY(SectionLocationMapRefno) REFERENCES tLocationMapSection(LocationMapRefno), "
    //    + "CONSTRAINT LocationMapSlotToLocationMapSlotCoordOnSlotCoord1LocationMapRefnoAndLocationMapSlotCoordRefno FOREIGN KEY (SlotCoord1LocationMapRefno) REFERENCES tLocationMapSlotCoordinate(LocationMapSlotCoordinateRefno), "
    //    + "CONSTRAINT LocationMapSlotToLocationMapSlotCoordOnSlotCoord2LocationMapRefnoAndLocationMapSlotCoordRefno FOREIGN KEY (SlotCoord2LocationMapRefno) REFERENCES tLocationMapSlotCoordinate(LocationMapSlotCoordinateRefno), "
    //    + "CONSTRAINT LocationMapSlotToLocationMapSlotCoordOnSlotCoord3LocationMapRefnoAndLocationMapSlotCoordRefno FOREIGN KEY (SlotCoord3LocationMapRefno) REFERENCES tLocationMapSlotCoordinate(LocationMapSlotCoordinateRefno)"

    //    + ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating location map slot table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE INDEX IF NOT EXISTS FK_LocationMapSlotOnSectionLocationMapRefno ON tLocationMapSlot(SectionLocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_LocationMapSlotOnWarehouseLocationMapRefno ON tLocationMapSlot(WarehouseLocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_LocationMapSlotOnSlotCoord1LocationMapRefno ON tLocationMapSlot(SlotCoord1LocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_LocationMapSlotOnSlotCoord2LocationMapRefno ON tLocationMapSlot(SlotCoord2LocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_LocationMapSlotOnSlotCoord3LocationMapRefno ON tLocationMapSlot(SlotCoord3LocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSlotOnLocationMapRefnoAndLocationMapSubType ON tLocationMapSlot(LocationMapRefno ASC, LocationMapSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSlotOnLocationMapSlotFKRefno ON tLocationMapSlot(LocationMapSlotFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSlotOnLocationMapSlotCode ON tLocationMapSlot(LocationMapSlotCode ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSlotOnSectionLocationMapRefnoAndLocationMapSlotCode ON tLocationMapSlot(SectionLocationMapRefno ASC, LocationMapSlotCode ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSlotOnLocationMapSlotBarCode ON tLocationMapSlot(LocationMapSlotBarCode ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tLocationMapSlot")]
    public class tLocationMapSlotModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tLocationMapModel)), Indexed(Name = "UQ_LocationMapSlotOnLocationMapRefnoAndLocationMapSubType", Order = 1, Unique = true)]
        public int LocationMapRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_LocationMapSlotOnLocationMapRefnoAndLocationMapSubType", Order = 1, Unique = true)]
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
