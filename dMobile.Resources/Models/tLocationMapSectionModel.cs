using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //  string sqlTable = "CREATE TABLE IF NOT EXISTS tLocationMapSection"
    //  + "("
    //  + "LocationMapRefno INTEGER NOT NULL CONSTRAINT PK_LocationMapSectionOnLocationMapRefno PRIMARY KEY,"
    //  + "LocationMapSubType INTEGER NOT NULL,"
    //  + "WarehouseLocationMapRefno INTEGER NOT NULL,"
    //  + "LocationMapSectionCode INTEGER NOT NULL,"
    //  + "LocationMapSectionName nvarchar(10),"
    //  + "LocationMapSectionFKRefno INTEGER NOT NULL,"
    //  + "LocationMapSectionBarCode nvarchar(13),"

    //  + "CONSTRAINT LocationMapSectionToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //   this.MsgEventArgs.StatusOutMsg = "Creating location map section table.";
    //   this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //  await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE INDEX IF NOT EXISTS FK_LocationMapSectionOnWarehouseLocationMapRefno ON tLocationMapSection(WarehouseLocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSectionOnLocationMapRefnoAndLocationMapSubType ON tLocationMapSection(LocationMapRefno ASC, LocationMapSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSectionOnLocationMapSectionFKRefno ON tLocationMapSection(LocationMapSectionFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSectionOnWarehouseLocationMapRefnoLocationMapSectionName ON tLocationMapSection(WarehouseLocationMapRefno ASC, LocationMapSectionName ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSectionOnLocationMapSectionCode ON tLocationMapSection(LocationMapSectionCode ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSectionOnLocationMapSectionBarCode ON tLocationMapSection(LocationMapSectionBarCode ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tLocationMapSection")]
    public class tLocationMapSectionModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tLocationMapModel)), OneToOne(CascadeOperations = CascadeOperation.All), Indexed(Name = "UQ_LocationMapSectionOnLocationMapRefnoAndLocationMapSubType", Order = 1, Unique = true)]
        public int LocationMapRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_LocationMapSectionOnLocationMapRefnoAndLocationMapSubType", Order = 2, Unique = true)]
        public int LocationMapSubType { get; set; }

        [NotNull, Indexed(Name = "FK_LocationMapSectionOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_LocationMapSectionOnLocationMapSectionCode", Order = 1, Unique = true)]
        public int LocationMapSectionCode { get; set; }

        [Indexed(Name = "UQ_LocationMapSectionOnWarehouseLocationMapRefnoLocationMapSectionName", Order = 1, Unique = false)]
        public string LocationMapSectionName { get; set; }

        [NotNull, Indexed(Name = "UQ_LocationMapSectionOnLocationMapSectionFKRefno", Order = 1, Unique = true)]
        public int LocationMapSectionFKRefno { get; set; }

        [Indexed(Name = "UQ_LocationMapSectionOnLocationMapSectionBarCode", Order = 1, Unique = true)]
        public string LocationMapSectionBarCode { get; set; }
    }
}
