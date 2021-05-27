using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tProductSchemeVariantXRef"
    //+ "("

    //+ "ProductSchemeRefno INTEGER NOT NULL,"
    //+ "ProductSchemeVariantRefno INTEGER NOT NULL, "

    //+ "CONSTRAINT ProductSchemeVariantXRefToProductSchemeOnProductSchemeRefno FOREIGN KEY (ProductSchemeRefno) REFERENCES tProductScheme(ProductSchemeRefno) ON DELETE CASCADE ON UPDATE CASCADE"
    //+ "CONSTRAINT ProductSchemeVariantXRefToProductSchemeVariantOnProductSchemeVariantRefno FOREIGN KEY (ProductSchemeVariantRefno) REFERENCES tProductSchemeVariant(ProductSchemeVariantRefno) ON DELETE CASCADE ON UPDATE CASCADE, "

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating product scheme variants xref table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS PK_ProductSchemeVariantXRefOnProductSchemeRefnoAndProductSchemeVariantRefno ON tProductSchemeVariantXRef(ProductSchemeRefno ASC, ProductSchemeVariantRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductSchemeVariantXRefOnProductSchemeRefno ON tProductSchemeVariantXRef(ProductSchemeRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductSchemeVariantXRefOnProductSchemeVariantRefno ON tProductSchemeVariantXRef(ProductSchemeVariantRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tProductSchemeVariantXRef")]
    public class tProductSchemeVariantXRefModel
    {
        [NotNull, ForeignKey(typeof(tProductSchemeModel)), Indexed(Name = "FK_ProductSchemeVariantXRefOnProductSchemeRefno", Order = 1, Unique = false)]
        public int ProductSchemeRefno { get; set; }

        [NotNull, ForeignKey(typeof(tProductSchemeVariantModel)), Indexed(Name = "FK_ProductSchemeVariantXRefOnProductSchemeVariantRefno", Order = 1, Unique = false)]
        public int ProductSchemeVariantRefno { get; set; }
    }
}
