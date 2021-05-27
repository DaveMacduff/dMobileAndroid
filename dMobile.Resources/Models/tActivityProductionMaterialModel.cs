using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityProductionMaterial"
    //+ "("

    //+ "ActivityRefno INTEGER NOT NULL,"
    //+ "ActivityProductionFKRefno INTEGER NOT NULL,"
    //+ "ActivityProductionStringFKRefno nvarchar(8) NOT NULL, "

    //+ "CONSTRAINT ActivityProductionMaterialToActivityProductionOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES tActivityProduction(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating production material table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS PK_ActivityProductionMaterialOnActivityRefno ON tActivityProductionMaterial(ActivityRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityProductionMaterialOnActivityProductionFKRefno ON tActivityProductionMaterial(ActivityProductionFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityProductionMaterial")]
    public class tActivityProductionMaterialModel
    {
        [NotNull, ForeignKey(typeof(tActivityProductionModel)), Indexed(Name = "PK_ActivityProductionMaterialOnActivityRefno", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityProductionMaterialOnActivityProductionFKRefno", Order = 1, Unique = true)]
        public int ActivityProductionFKRefno { get; set; }

        [NotNull]
        public string ActivityProductionStringFKRefno { get; set; }
    }
}
