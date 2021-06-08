using SQLite;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tContact"
    //                + "("
    //                + "ContactRefno INTEGER NOT NULL CONSTRAINT PK_ContactOnContactRefno PRIMARY KEY,"
    //                + "ContactSubType INTEGER NOT NULL"
    //                + ")";

    //        if (this.OnNubStatOut != null)
    //        {
    //    this.MsgEventArgs.StatusOutMsg = "Creating contacts table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //        this.RunDDLCommand(sqlTable);

    //        string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactOnContactRefnoAndContactSubType ON "
    //            + "tContact(ContactRefno ASC, ContactSubType ASC)";
    //        this.RunDDLCommand(sqlNdx);

    [Table("tContact")]
    public class tContactModel
    {
        [PrimaryKey, Indexed(Name = "UQ_ContactOnContactRefnoAndContactSubType", Order = 1, Unique = true), NotNull]
        public int ContactRefno { get; set; }
        
        [Indexed(Name = "UQ_ContactOnContactRefnoAndContactSubType", Order = 2, Unique = true), NotNull]
        public int ContactSubType { get; set; }
    }
}