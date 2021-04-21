using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailProductionFinished"
    //+ "("
    //+ "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailProductionFinishedOnActivityDetailRefno PRIMARY KEY,"
    //+ "ActivityDetailProductionFinishedFKRefno INTEGER NOT NULL, "

    //+ "CONSTRAINT ActivityDetailProductionFinishedTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating detail production finished table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailProductionFinishedOnActivityDetailProductionFinishedFKRefno ON tActivityDetailProductionFinished(ActivityDetailProductionFinishedFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityDetailProductionFinished")]
    public class tActivityDetailProductionFinishedModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityDetailModel)), OneToOne(CascadeOperations = CascadeOperation.All)]
        public int ActivityDetailRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityDetailProductionFinishedOnActivityDetailProductionFinishedFKRefno", Order = 1, Unique = true)]
        public int ActivityDetailProductionFinishedFKRefno { get; set; }
    }
}
