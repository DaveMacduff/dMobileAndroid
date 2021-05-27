using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityProductionByProduct"
    //+ "("
    //+ "ActivityRefno INTEGER NOT NULL,"
    //+ "ActivityProductionFKRefno INTEGER NOT NULL,"
    //+ "ActivityProductionStringFKRefno nvarchar(8) NOT NULL, "

    //+ "CONSTRAINT ActivityProductionByProductToActivityProductionOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES tActivityProduction(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //        if (this.OnNubStatOut != null)
    //        {
    //    this.MsgEventArgs.StatusOutMsg = "Creating production byproduct table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS PK_ActivityProductionByProductOnActivityRefno ON tActivityProductionByProduct(ActivityRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityProductionByProductOnActivityProductionFKRefno ON tActivityProductionByProduct(ActivityProductionFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityProductionByProduct")]
    public class tActivityProductionByProductModel
    {
        [NotNull, ForeignKey(typeof(tActivityProductionModel)), Indexed(Name = "PK_ActivityProductionByProductOnActivityRefno", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityProductionByProductOnActivityProductionFKRefno", Order = 1, Unique = true)]
        public int ActivityProductionFKRefno { get; set; }

        [NotNull]
        public string ActivityProductionStringFKRefno { get; set; }
    }
}
