using SQLite;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivity"
    //+ "("
    //+ "ActivityRefno INTEGER NOT NULL CONSTRAINT PK_ActivityOnActivityRefno PRIMARY KEY,"
    //+ "ActivityStatus INTEGER NOT NULL,"
    //+ "ActivitySubType INTEGER NOT NULL"
    //+ ")";

    //        string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityOnActivityRefnoAndActivitySubType "
    //            + "ON tActivity(ActivityRefno ASC, ActivitySubType ASC)";
    //        this.RunDDLCommand(sqlNdx);

    [Table("tActivity")]
    public class tActivityModel
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int ActivityRefno { get; set; }
        [Indexed(Name = "UQ_ActivityOnActivityRefnoAndActivitySubType", Order = 1, Unique = true), NotNull]
        public int ActivityStatus { get; set; }
        [Indexed(Name = "UQ_ActivityOnActivityRefnoAndActivitySubType", Order = 2, Unique = true), NotNull]
        public int ActivitySubType { get; set; }
    }
}