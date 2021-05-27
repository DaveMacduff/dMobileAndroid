using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItem"
    //    + "("
    //    + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemOnActivityItemRefno PRIMARY KEY,"
    //    + "ActivityItemSubType INTEGER NOT NULL,"
    //    + "ActivityItemStatus INTEGER NOT NULL,"
    //    + "ActivityRefno INTEGER NOT NULL, "
    //    + "CONSTRAINT ActivityItemToActivityOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE"
    //    + ")";

    //        if (this.OnNubStatOut != null)
    //        {
    //    // TODO: Resource strings
    //    this.MsgEventArgs.StatusOutMsg = "Creating activity item table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //        string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailOnActivityItemRefnoAndActivityItemSubType "
    //            + "ON tActivityItem(ActivityItemRefno ASC, ActivityItemSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemOnActivityRefno ON tActivityItem(ActivityRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //[Indexed(Name = "UQ_ContactUserOnContactUserCode", Order = 1, Unique = true), NotNull]

    [Table("tActivityItem")]
    public class tActivityItemModel
    {
        [Indexed(Name = "UQ_ActivityDetailOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true), PrimaryKey, AutoIncrement, NotNull]
        public int ActivityItemRefno { get; set; }

        [Indexed(Name = "UQ_ActivityDetailOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true), NotNull]
        public int ActivityItemSubType { get; set; }

        [NotNull]
        public int ActivityItemStatus { get; set; }

        [Indexed(Name = "FK_ActivityItemOnActivityRefno", Order = 1, Unique = false), NotNull, ForeignKey(typeof(tActivityModel))]
        public int ActivityRefno { get; set; }
    }
}
