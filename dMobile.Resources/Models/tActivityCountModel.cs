using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityCount"
    //+ "("
    //+ "ActivityRefno INTEGER NOT NULL CONSTRAINT PK_ActivityCountOnActivityRefno PRIMARY KEY,"
    //+ "ActivitySubType INTEGER NOT NULL,"
    //+ "ActivityCountFKRefno INTEGER NOT NULL,"
    //+ "ActivityCountStringFKRefno nvarchar(8) NOT NULL,"
    //+ "WarehouseLocationMapRefno INTEGER NOT NULL,"
    //+ "ActivityCountType INTEGER NOT NULL,"
    //+ "ActivityCountDateTime datetime NOT NULL,"
    //+ "ActivityCountTypeDescription nvarchar(60) NOT NULL,"
    //+ "ActivityCountDescription nvarchar(60) NOT NULL, "

    //+ "CONSTRAINT ActivityCountToActivityOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityCountToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno)"
    //+ ")";

    //        if (this.OnNubStatOut != null)
    //        {
    //    // TODO: Resource strings
    //    this.MsgEventArgs.StatusOutMsg = "Creating count table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //        string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityCountOnActivityRefnoAndActivitySubType ON "
    //            + "tActivityCount(ActivityRefno ASC, ActivitySubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityCountOnActivityCountFKRefno ON "
    //            + "tActivityCount(ActivityCountFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityCountOnWarehouseLocationMapRefno ON "
    //            + "tActivityCount(WarehouseLocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityCount")]
    public class tActivityCountModel
    {
        [PrimaryKey, NotNull, Indexed(Name = "UQ_ActivityCountOnActivityRefnoAndActivitySubType", Order = 1, Unique = true), ForeignKey(typeof(tActivityModel))]
        public int ActivityRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityCountOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        public int ActivitySubType { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityCountOnActivityCountFKRefno", Order = 1, Unique = true)] 
        public int ActivityCountFKRefno { get; set; }

        [NotNull]
        public string ActivityCountStringFKRefno { get; set; }

        [NotNull, Indexed(Name = "FK_ActivityCountOnWarehouseLocationMapRefno", Order = 1, Unique = false), ForeignKey(typeof(tLocationMapWarehouseModel))]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public int ActivityCountType { get; set; }

        [NotNull]
        public DateTime ActivityCountDateTime { get; set; }

        [NotNull]
        public string ActivityCountTypeDescription { get; set; }

        [NotNull]
        public string ActivityCountDescription { get; set; }
    }
}
