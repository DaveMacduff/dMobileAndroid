using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailConvertUnit"
    //+ "("
    //+ "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailConvertUnitOnActivityDetailRefno PRIMARY KEY,"
    //+ "ActivityDetailConvertUnitFKRefno INTEGER NOT NULL "
    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating detail convert units table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityDetailConvertUnitOnActivityDetailConvertUnitFKRefno ON tActivityDetailConvertUnit(ActivityDetailConvertUnitFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //string alterTable = "ALTER TABLE tActivityDetailConvertUnit ADD CONSTRAINT ActivityDetailConvertUnitTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
    //await this.RunDDLCommand(alterTable);

    [Table("tActivityDetailConvertUnit")]
    public class tActivityDetailConvertUnitModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityDetailConvertUnitOnActivityDetailConvertUnitFKRefno", Order = 1, Unique = true)]
        public int ActivityDetailConvertUnitFKRefno { get; set; }
    }
}
