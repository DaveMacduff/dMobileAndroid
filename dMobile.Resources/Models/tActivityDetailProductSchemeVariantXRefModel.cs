using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailProductSchemeVariantXRef"
    //        + "("
    //        + "ActivityDetailRefno INTEGER NOT NULL,"
    //        + "ProductSchemeVariantRefno INTEGER NOT NULL, "
    //        + "CONSTRAINT ActivityDetailProductSchemeVariantXRefTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //        + "CONSTRAINT ActivityDetailProductSchemeVariantXRefToProductSchemeVariantOnProductSchemeVariantRefno FOREIGN KEY (ProductSchemeVariantRefno) "
    //        + "REFERENCES tProductSchemeVariant(ProductSchemeVariantRefno) ON DELETE CASCADE ON UPDATE CASCADE"
    //        + ")";

    //    if (this.OnNubStatOut != null)
    //    {
    //    // TODO: Resource strings
    //    this.MsgEventArgs.StatusOutMsg = "Creating detail product scheme variant xref table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //    string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
    //        + "PK_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefnoAndProductSchemeVariantRefno ON "
    //        + "tActivityDetailProductSchemeVariantXRef(ActivityDetailRefno ASC, ProductSchemeVariantRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);


    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefno ON "
    //    + "tActivityDetailProductSchemeVariantXRef(ActivityDetailRefno ASC)";

    //await this.RunDDLCommand(sqlNdx);


    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityDetailProductSchemeVariantXRefOnProductSchemeVariantRefno "
    //    + "ON tActivityDetailProductSchemeVariantXRef(ProductSchemeVariantRefno ASC)";

    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityDetailProductSchemeVariantXRef")]
    public class tActivityDetailProductSchemeVariantXRefModel
    {
        // TODO: Can only support one index right now
        [Indexed(Name = "UQ_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefno", Order = 1, Unique = true), NotNull, ForeignKey(typeof(tActivityDetailModel)), OneToOne(CascadeOperations = CascadeOperation.All)]
        public int ActivityDetailRefno { get; set; }
        [Indexed(Name = "FK_ActivityDetailProductSchemeVariantXRefOnProductSchemeVariantRefno", Order = 1, Unique = true), NotNull, ForeignKey(typeof(tProductSchemeVariantModel)), OneToOne(CascadeOperations = CascadeOperation.All)]
        public int ProductSchemeVariantRefno { get; set; }
    }
}
