using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailExtra"
    //            + "("
    //            + "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailExtraOnActivityDetailRefno PRIMARY KEY,"
    //            + "ActivityDetailSubType INTEGER NOT NULL,"
    //            + "ActivityDetailExtra1Text nvarchar(10) NOT NULL,"
    //            + "ActivityDetailExtra2Text nvarchar(10) NOT NULL,"
    //            + "ActivityDetailExtra3Text nvarchar(10) NOT NULL,"
    //            + "ActivityDetailExtra4Text nvarchar(10) NOT NULL,"
    //            + "ActivityDetailExtra5Date datetime NOT NULL, "

    //            + "CONSTRAINT ActivityDetailExtraToActivityDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"
    //            + ")";

    //        if (this.OnNubStatOut != null)
    //        {
    //    // TODO: Resource strings
    //    this.MsgEventArgs.StatusOutMsg = "Creating detail extra table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //        string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailExtraOnActivityDetailRefnoAndActivityDetailSubType "
    //            + "ON tActivityDetailExtra(ActivityDetailRefno ASC, ActivityDetailSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityDetailExtra")]
    public class tActivityDetailExtraModel
    {
        [PrimaryKey, Indexed(Name = "UQ_ActivityDetailExtraOnActivityDetailRefnoAndActivityDetailSubType", Order = 1, Unique = true), NotNull, ForeignKey(typeof(tActivityDetailModel)), OneToOne(CascadeOperations = CascadeOperation.All)]
        public int ActivityDetailRefno { get; set; }

        [Indexed(Name = "UQ_ActivityDetailExtraOnActivityDetailRefnoAndActivityDetailSubType", Order = 2, Unique = true), NotNull]
        public int ActivityDetailSubType { get; set; }

        [NotNull]
        public string ActivityDetailExtra1Text { get; set; }

        [NotNull]
        public string ActivityDetailExtra2Text { get; set; }

        [NotNull]
        public string ActivityDetailExtra3Text { get; set; }

        [NotNull]
        public string ActivityDetailExtra4Text { get; set; }

        [NotNull]
        public DateTime ActivityDetailExtra5Date { get; set; }
    }
}
