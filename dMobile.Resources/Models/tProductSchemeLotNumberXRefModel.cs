using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tProductSchemeLotNumberXRef"
    //+ "("
    //+ "ProductSchemeRefno INTEGER NOT NULL,"
    //+ "ProductSchemeLotNumberRefno INTEGER NOT NULL, "

    //+ "CONSTRAINT ProductSchemeLotNumberXRefToProductSchemeOnProductSchemeRefno FOREIGN KEY (ProductSchemeRefno) REFERENCES tProductScheme(ProductSchemeRefno) ON DELETE CASCADE ON UPDATE CASCADE"
    //+ "CONSTRAINT ProductSchemeLotNumberXRefToProductSchemeLotNumberOnProductSchemeLotNumberRefno FOREIGN KEY (ProductSchemeLotNumberRefno) REFERENCES tProductSchemeLotNumber(ProductSchemeLotNumberRefno) ON DELETE CASCADE ON UPDATE CASCADE "

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating product scheme lot numbers xref table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS PK_ProductSchemeLotNumberXRefOnProductSchemeRefnoAndProductSchemeLotNumberRefno ON tProductSchemeLotNumberXRef(ProductSchemeRefno ASC, ProductSchemeLotNumberRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductSchemeLotNumberXRefOnProductSchemeRefno ON tProductSchemeLotNumberXRef(ProductSchemeRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductSchemeLotNumberXRefOnProductSchemeLotNumberRefno ON tProductSchemeLotNumberXRef(ProductSchemeLotNumberRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tProductSchemeLotNumberXRef")]
    public class tProductSchemeLotNumberXRefModel
    {
        [NotNull, ForeignKey(typeof(tProductSchemeModel)), OneToMany(CascadeOperations = CascadeOperation.All), Indexed(Name = "FK_ProductSchemeLotNumberXRefOnProductSchemeRefno", Order = 1, Unique = false)]
        public int ProductSchemeRefno { get; set; }

        [NotNull, ForeignKey(typeof(tProductSchemeLotNumberModel)), OneToMany(CascadeOperations = CascadeOperation.All), Indexed(Name = "FK_ProductSchemeLotNumberXRefOnProductSchemeLotNumberRefno", Order = 1, Unique = false)]
        public int ProductSchemeLotNumberRefno { get; set; }
    }
}
