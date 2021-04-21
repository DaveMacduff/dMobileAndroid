using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemConvertUnit"
    //+ "("
    //+ "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemConvertUnitOnActivityItemRefno PRIMARY KEY,"
    //+ "ActivityItemSubType INTEGER NOT NULL,"
    //+ "ActivityItemConvertUnitType INTEGER NOT NULL,"
    //+ "ProductUnitRefno INTEGER NOT NULL"
    //+ ")";

    //        if (this.OnNubStatOut != null)
    //        { 
    //    this.MsgEventArgs.StatusOutMsg = "Creating item convert units table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //        string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX "
    //            + "UQ_ActivityItemConvertUnitOnActivityItemRefnoAndActivityItemSubType ON "
    //            + "tActivityItemConvertUnit(ActivityItemRefno ASC, ActivityItemSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //        string alterTable = "ALTER TABLE tActivityItemConvertUnit ADD CONSTRAINT "
    //            + "ActivityItemConvertUnitTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES "
    //            + "tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
    //await this.RunDDLCommand(alterTable);

    //alterTable = "ALTER TABLE tActivityItemConvertUnit ADD CONSTRAINT "
    //            + "ActivityItemConvertUnitTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES "
    //            + "tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE";
    //await this.RunDDLCommand(alterTable);

    [Table("tActivityItemConvertUnit")]
    public class tActivityItemConvertUnitModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemModel)), OneToMany(CascadeOperations = CascadeOperation.All), Indexed(Name = "UQ_ActivityItemConvertUnitOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemConvertUnitOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemSubType { get; set; }

        [NotNull]
        public int ActivityItemConvertUnitType { get; set; }

        [NotNull, ForeignKey(typeof(tProductUnitModel)), OneToMany(CascadeOperations = CascadeOperation.All)]
        public int ProductUnitRefno { get; set; }
    }
}
