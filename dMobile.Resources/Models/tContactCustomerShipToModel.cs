using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tContactCustomerShipTo"
    //+ "("

    //+ "ContactRefno INTEGER NOT NULL CONSTRAINT PK_UserOnContactRefno PRIMARY KEY,"
    //+ "ContactSubType INTEGER NOT NULL,"
    //+ "CustomerContactRefno INTEGER NOT NULL,"
    //+ "ContactCustomerShipToFKRefno INTEGER NOT NULL,"
    //+ "ContactCustomerShipToCode nvarchar(8) NOT NULL,"
    //+ "ContactCustomerShipToName nvarchar(40) NOT NULL,"

    //+ "CONSTRAINT ContactCustomerShipToContactOnContactRefno FOREIGN KEY (ContactRefno) REFERENCES tContact(ContactRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ContactCustomerShipToContactCustomerOnCustomerContactRefno FOREIGN KEY (CustomerContactRefno) REFERENCES tContactCustomer(ContactRefno)"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating contact customer shipto's table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactCustomerShipToOnContactRefnoAndContactSubType ON tContactCustomerShipTo(ContactRefno ASC, ContactSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactCustomerShipToOnContactCustomerShipToFKRefno ON tContactCustomerShipTo(ContactCustomerShipToFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactCustomerShipToOnContactCustomerShipToCode ON tContactCustomerShipTo(ContactCustomerShipToCode ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactCustomerShipToOnContactCustomerShipToName ON tContactCustomerShipTo(ContactCustomerShipToName ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tContactCustomerShipTo")]
    public class tContactCustomerShipToModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tContactModel)), OneToOne(CascadeOperations = CascadeOperation.All), Indexed(Name = "UQ_ContactCustomerShipToOnContactRefnoAndContactSubType", Order = 1, Unique = true)]
        public int ContactRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactCustomerShipToOnContactRefnoAndContactSubType", Order = 2, Unique = true)]
        public int ContactSubType { get; set; }

        [NotNull, ForeignKey(typeof(tContactCustomerModel))]
        public int CustomerContactRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactCustomerShipToOnContactCustomerShipToFKRefno", Order = 1, Unique = true)]
        public int ContactCustomerShipToFKRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactCustomerShipToOnContactCustomerShipToCode", Order = 1, Unique = true)]
        public string ContactCustomerShipToCode { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactCustomerShipToOnContactCustomerShipToName", Order = 1, Unique = true)]
        public string ContactCustomerShipToName { get; set; }
    }
}
