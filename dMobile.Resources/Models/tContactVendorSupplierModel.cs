using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tContactVendorSupplier"
    //+ "("
    //+ "ContactRefno INTEGER NOT NULL CONSTRAINT PK_UserOnContactRefno PRIMARY KEY,"
    //+ "ContactSubType INTEGER NOT NULL,"
    //+ "VendorContactRefno INTEGER NOT NULL,"
    //+ "ContactVendorSupplierFKRefno INTEGER NOT NULL,"
    //+ "ContactVendorSupplierCode nvarchar(8) NOT NULL,"
    //+ "ContactVendorSupplierName nvarchar(40) NOT NULL, "

    //+ "CONSTRAINT ContactVendorSupplierToContactOnContactRefno FOREIGN KEY (ContactRefno) REFERENCES tContact(ContactRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ContactVendorSupplierToVendorContactOnVendorContactRefno FOREIGN KEY (VendorContactRefno) REFERENCES tContactVendor(ContactRefno)"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating contact vendor suppliers table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactVendorSupplierOnContactRefnoAndContactSubType ON tContactVendorSupplier(ContactRefno ASC, ContactSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactVendorSupplierOnContactVendorSupplierFKRefno ON tContactVendorSupplier(ContactVendorSupplierFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactVendorSupplierOnContactVendorSupplierCode ON tContactVendorSupplier(ContactVendorSupplierCode ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactVendorSupplierOnContactVendorSupplierName ON tContactVendorSupplier(ContactVendorSupplierName ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tContactVendorSupplier")]
    public class tContactVendorSupplierModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tContactModel)), Indexed(Name = "UQ_ContactVendorSupplierOnContactRefnoAndContactSubType", Order = 1, Unique = true)]
        public int ContactRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactVendorSupplierOnContactRefnoAndContactSubType", Order = 2, Unique = true)]
        public int ContactSubType { get; set; }

        [NotNull, ForeignKey(typeof(tContactVendorModel))]
        public int VendorContactRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactVendorSupplierOnContactVendorSupplierFKRefno", Order = 1, Unique = true)]
        public int ContactVendorSupplierFKRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactVendorSupplierOnContactVendorSupplierCode", Order = 1, Unique = true)]
        public string ContactVendorSupplierCode { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactVendorSupplierOnContactVendorSupplierName", Order = 1, Unique = true)]
        public string ContactVendorSupplierName { get; set; }
    }
}
