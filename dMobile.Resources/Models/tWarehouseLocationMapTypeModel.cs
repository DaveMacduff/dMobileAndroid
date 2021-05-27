using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tWarehouseLocationMapType"
    //+ "("

    //+ "WarehouseLocationMapRefno INTEGER NOT NULL,"
    //+ "WarehouseLocationMapTypeEnum INTEGER NOT NULL,"
    //+ "LocationMapRefno INTEGER NOT NULL, "

    //+ "CONSTRAINT WarehouseLocationMapTypeToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT WarehouseLocationMapTypeToLocationMapOnWarehouseLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMap(LocationMapRefno)"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating warehouse location map table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS PK_WarehouseLocationMapTypeOnWarehouseLocationMapRefnoAndLocationMapRefnoAndWarehouseLocationMapTypeEnum ON tWarehouseLocationMapType(WarehouseLocationMapRefno, LocationMapRefno, WarehouseLocationMapTypeEnum)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_WarehouseLocationMapTypeOnWarehouseLocationMapRefnoAndWarehouseLocationMapTypeEnum ON tWarehouseLocationMapType(WarehouseLocationMapRefno, WarehouseLocationMapTypeEnum)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_WarehouseLocationMapTypeOnWarehouseLocationMapRefno ON tWarehouseLocationMapType(WarehouseLocationMapRefno)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_WarehouseLocationMapTypeOnLocationMapRefno ON tWarehouseLocationMapType(LocationMapRefno)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tWarehouseLocationMapType")]
    public class tWarehouseLocationMapTypeModel
    {
        [NotNull, ForeignKey(typeof(tLocationMapModel)), Indexed(Name = "FK_WarehouseLocationMapTypeOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull, Indexed(Name = "WarehouseLocationMapTypeOnWarehouseLocationMapTypeEnum", Order = 1, Unique = false)]
        public int WarehouseLocationMapTypeEnum { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapModel)), Indexed(Name = "FK_WarehouseLocationMapTypeOnLocationMapRefno", Order = 1, Unique = false)]
        public int LocationMapRefno { get; set; }
    }
}
