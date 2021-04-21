using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tContactCustomer"
    //+ "("

    //+ "ContactRefno INTEGER NOT NULL CONSTRAINT PK_UserOnContactRefno PRIMARY KEY,"
    //+ "ContactSubType INTEGER NOT NULL,"
    //+ "ContactCustomerFKRefno INTEGER NOT NULL,"
    //+ "ContactCustomerCode nvarchar(8) NOT NULL,"
    //+ "ContactCustomerName nvarchar(40) NOT NULL,"

    //+ "CONSTRAINT ContactCustomerToContactOnContactRefno FOREIGN KEY (ContactRefno) REFERENCES tContact(ContactRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating contact customers table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactCustomerOnContactRefnoAndContactSubType ON tContactCustomer(ContactRefno ASC, ContactSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactCustomerOnContactCustomerFKRefno ON tContactCustomer(ContactCustomerFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactCustomerOnContactCustomerCode ON tContactCustomer(ContactCustomerCode ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactCustomerOnContactCustomerName ON tContactCustomer(ContactCustomerName ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tContactCustomer")]
    public class tContactCustomerModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tContactModel)), OneToOne(CascadeOperations = CascadeOperation.All), Indexed(Name = "UQ_ContactCustomerOnContactRefnoAndContactSubType", Order = 1, Unique = true)]
        public int ContactRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactCustomerOnContactRefnoAndContactSubType", Order = 2, Unique = true)]
        public int ContactSubType { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactCustomerOnContactCustomerFKRefno", Order = 1, Unique = true)]
        public int ContactCustomerFKRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactCustomerOnContactCustomerCode", Order = 1, Unique = true)]
        public string ContactCustomerCode { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactCustomerOnContactCustomerName", Order = 1, Unique = true)]
        public string ContactCustomerName { get; set; }
    }
}
