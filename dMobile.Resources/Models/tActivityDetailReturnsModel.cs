using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailReturns"
    //      + "("

    //      + "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailReturnsOnActivityDetailRefno PRIMARY KEY,"
    //      + "ActivityDetailReturnsFKRefno INTEGER NOT NULL,"
    //      + "ActivityDetailReturnsIsConfirmed bit NOT NULL, "

    //      + "CONSTRAINT ActivityDetailReturnsTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //      + ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating returns detail table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailReturnsOnActivityDetailReturnsFKRefno ON tActivityDetailReturns(ActivityDetailReturnsFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityDetailReturns")]
    public class tActivityDetailReturnsModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityDetailModel)), OneToOne(CascadeOperations = CascadeOperation.All)]
        public int ActivityDetailRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityDetailReturnsOnActivityDetailReturnsFKRefno", Order = 1, Unique = true)]
        public int ActivityDetailReturnsFKRefno { get; set; }

        [NotNull]
        public bool ActivityDetailReturnsIsConfirmed { get; set; }
    }
}
