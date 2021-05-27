using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailAdjustment"
    //+ "("
    //+ "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailAdjustmentOnActivityDetailRefno PRIMARY KEY,"
    //+ "ActivityDetailAdjustmentFKRefno INTEGER NOT NULL, "

    //+ "CONSTRAINT ActivityDetailAdjustmentTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating detail adjustment table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailAdjustmentOnActivityDetailAdjustmentFKRefno ON tActivityDetailAdjustment(ActivityDetailAdjustmentFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityDetailAdjustment")]
    public class tActivityDetailAdjustmentModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityDetailAdjustmentOnActivityDetailAdjustmentFKRefno", Order = 1, Unique = true)]
        public int ActivityDetailAdjustmentFKRefno { get; set; }
    }
}
