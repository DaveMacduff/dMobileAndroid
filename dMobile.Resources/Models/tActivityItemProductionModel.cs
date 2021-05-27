using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemProduction"
    //+ "("
    //+ "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemProductionOnActivityItemRefno PRIMARY KEY,"
    //+ "ActivityItemSubType INTEGER NOT NULL,"
    //+ "ProductUnitRefno INTEGER NOT NULL,"
    //+ "LocationMapRefno INTEGER NOT NULL,"
    //+ "ActivityItemProductionQuantity numeric(11, 3) NOT NULL,"
    //+ "ActivityItemProductionBaseQuantity numeric(11, 3) NOT NULL,"
    //+ "ActivityItemProductionWeightQuantity numeric(11, 3) NOT NULL,"
    //+ "ActivityItemProductionDescription nvarchar(42) NOT NULL,"
    //+ "ActivityItemProductionQuantityExpected numeric(11, 3) NOT NULL,"
    //+ "ActivityItemProductionBaseQuantityExpected numeric(11, 3) NOT NULL,"
    //+ "ActivityItemProductionWeightQuantityExpected numeric(11, 3) NOT NULL, "

    //+ "CONSTRAINT ActivityItemProductionTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityItemProductionToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityItemProductionTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating item production table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemProductionOnActivityItemRefnoAndActivityItemSubType ON tActivityItemProduction(ActivityItemRefno ASC, ActivityItemSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemProductionOnLocationMapRefno ON tActivityItemProduction(LocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityItemProduction")]
    public class tActivityItemProductionModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemModel)), Indexed(Name = "UQ_ActivityItemProductionOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemProductionOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        [NotNull, ForeignKey(typeof(tProductUnitModel))]
        public int ProductUnitRefno { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapModel)), Indexed(Name = "FK_ActivityItemProductionOnLocationMapRefno", Order = 2, Unique = false)]
        public int LocationMapRefno { get; set; }

        [NotNull]
        public decimal ActivityItemProductionQuantity { get; set; }

        [NotNull]
        public decimal ActivityItemProductionBaseQuantity { get; set; }

        [NotNull]
        public decimal ActivityItemProductionWeightQuantity { get; set; }

        [NotNull]
        public string ActivityItemProductionDescription { get; set; }

        [NotNull]
        public decimal ActivityItemProductionQuantityExpected { get; set; }

        [NotNull]
        public decimal ActivityItemProductionBaseQuantityExpected { get; set; }

        [NotNull]
        public decimal ActivityItemProductionWeightQuantityExpected { get; set; }
    }
}
