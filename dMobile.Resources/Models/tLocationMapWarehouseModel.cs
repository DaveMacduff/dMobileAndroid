using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tLocationMapWarehouse"
    //+ "("
    //+ "LocationMapRefno INTEGER NOT NULL CONSTRAINT PK_LocationMapWarehouseOnLocationMapRefno PRIMARY KEY,"
    //+ "LocationMapSubType INTEGER NOT NULL,"
    //+ "LocationMapWarehouseName nvarchar(22) NOT NULL,"
    //+ "LocationMapWarehouseCode INTEGER NOT NULL,"
    //+ "LocationMapWarehouseFKRefno INTEGER NOT NULL,"
    //+ "LocationMapWarehouseBarCode nvarchar(13) NOT NULL,"
    //+ "LocationMapWarehouseIsWMS bit NOT NULL,"
    //+ "LocationMapWarehouseMask nvarchar(10) NOT NULL,"
    //+ "LocationMapWarehouseSectionMask nvarchar(10) NOT NULL,"
    //+ "LocationMapWarehouseSlotMask nvarchar(10) NOT NULL,"
    //+ "LocationMapWarehouseSlotCoordinate1Mask nvarchar(10) NOT NULL,"
    //+ "LocationMapWarehouseSlotCoordinate2Mask nvarchar(10) NOT NULL,"
    //+ "LocationMapWarehouseSlotCoordinate3Mask nvarchar(10) NOT NULL,"
    //+ "LocationMapWarehouseSlotCoordinate1Name nvarchar(10) NOT NULL,"
    //+ "LocationMapWarehouseSlotCoordinate2Name nvarchar(10) NOT NULL,"
    //+ "LocationMapWarehouseSlotCoordinate3Name nvarchar(10) NOT NULL,"
    //+ "LocationMapWarehouseIsSectionTracked bit NOT NULL,"
    //+ "LocationMapWarehouseIsSlotTracked bit NOT NULL,"
    //+ "LocationMapWarehouseAdhocEnabled bit NOT NULL, "

    //+ "CONSTRAINT LocationMapWarehouseToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating location map warehouse table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapWarehouseOnLocationMapRefnoAndLocationMapSubType ON tLocationMapWarehouse(LocationMapRefno ASC, LocationMapSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno ON tLocationMapWarehouse(LocationMapWarehouseFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapWarehouseOnLocationMapWarehouseName ON tLocationMapWarehouse(LocationMapWarehouseName ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapWarehouseOnLocationMapWarehouseCode ON tLocationMapWarehouse(LocationMapWarehouseCode ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapWarehouseOnLocationMapWarehouseBarCode ON tLocationMapWarehouse(LocationMapWarehouseBarCode ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tLocationMapWarehouse")]
    public class tLocationMapWarehouseModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tLocationMapModel)), OneToOne(CascadeOperations = CascadeOperation.All), Indexed(Name = "UQ_LocationMapWarehouseOnLocationMapRefnoAndLocationMapSubType", Order = 1, Unique = true)]
        public int LocationMapRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_LocationMapWarehouseOnLocationMapRefnoAndLocationMapSubType", Order = 2, Unique = true)]
        public int LocationMapSubType { get; set; }

        [NotNull, Indexed(Name = "UQ_LocationMapWarehouseOnLocationMapWarehouseName", Order = 1, Unique = true)]
        public string LocationMapWarehouseName { get; set; }

        [NotNull, Indexed(Name = "UQ_LocationMapWarehouseOnLocationMapWarehouseCode", Order = 1, Unique = true)]
        public int LocationMapWarehouseCode { get; set; }

        [NotNull, Indexed(Name = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno", Order = 1, Unique = true)]
        public int LocationMapWarehouseFKRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_LocationMapWarehouseOnLocationMapWarehouseBarCode", Order = 1, Unique = true)]
        public string LocationMapWarehouseBarCode { get; set; }

        [NotNull]
        public bool LocationMapWarehouseIsWMS { get; set; }

        [NotNull]
        public string LocationMapWarehouseMask { get; set; }

        [NotNull]
        public string LocationMapWarehouseSectionMask { get; set; }

        [NotNull]
        public string LocationMapWarehouseSlotMask { get; set; }

        [NotNull]
        public string LocationMapWarehouseSlotCoordinate1Mask { get; set; }

        [NotNull]
        public string LocationMapWarehouseSlotCoordinate2Mask { get; set; }

        [NotNull]
        public string LocationMapWarehouseSlotCoordinate3Mask { get; set; }

        [NotNull]
        public string LocationMapWarehouseSlotCoordinate1Name { get; set; }

        [NotNull]
        public string LocationMapWarehouseSlotCoordinate2Name { get; set; }

        [NotNull]
        public string LocationMapWarehouseSlotCoordinate3Name { get; set; }

        [NotNull]
        public bool LocationMapWarehouseIsSectionTracked { get; set; }

        [NotNull]
        public bool LocationMapWarehouseIsSlotTracked { get; set; }

        [NotNull]
        public bool LocationMapWarehouseAdhocEnabled { get; set; }
    }
}
