// <copyright file="tContactUserModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //    string sqlTable = "CREATE TABLE IF NOT EXISTS tContactUser"
    //+ "("
    //+ "ContactRefno INTEGER NOT NULL CONSTRAINT PK_UserOnContactRefno PRIMARY KEY,"
    //+ "ContactSubType INTEGER NOT NULL,"
    //+ "ContactUserCode INTEGER NOT NULL,"
    //+ "ContactUserName nvarchar(20),"
    //+ "ContactUserPassword nvarchar(6),"
    //+ "ContactUserIsAllowedAssociateProductBarCode bit NOT NULL,"
    //+ "ContactUserIsAllowedPickingModuleOverridePicker bit NOT NULL,"
    //+ "ContactUserIsAllowedPickingModuleOverrideAddManualDetail bit NOT NULL,"
    //+ "ContactUserLocalizationLanguage nvarchar(3),"
    //+ "CONSTRAINT ContactUserToContactOnContactRefno FOREIGN KEY (ContactRefno) REFERENCES tContact(ContactRefno) ON DELETE CASCADE ON UPDATE CASCADE"
    //+ ")";

    //        if (this.OnNubStatOut != null)
    //        {
    //        this.MsgEventArgs.StatusOutMsg = "Creating contact users table.";
    //        this.OnNubStatOut(this, this.MsgEventArgs);
    //    }

    //        this.RunDDLCommand(sqlTable);

    //        string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactUserOnContactRefnoAndContactSubType ON "
    //            + "tContactUser(ContactRefno ASC, ContactSubType ASC)";
    //        this.RunDDLCommand(sqlNdx);

    //    sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactUserOnContactUserCode ON tContactUser(ContactUserCode ASC)";
    //        this.RunDDLCommand(sqlNdx);

    //    sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactUserOnContactUserName ON tContactUser(ContactUserName ASC)";
    //        this.RunDDLCommand(sqlNdx);

    //[Indexed(Name = "UQ_ActivityOnActivityRefnoAndActivitySubType", Order = 1, Unique = true), NotNull]
    [Table("tContactUser")]
    public class tContactUserModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tContactModel)), Indexed(Name = "UQ_ContactUserOnContactRefnoAndContactSubType", Order = 1, Unique = true)]
        public int ContactRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactUserOnContactRefnoAndContactSubType", Order = 2, Unique = true)]
        public int ContactSubType { get; set; }

        [Indexed(Name = "UQ_ContactUserOnContactUserCode", Order = 1, Unique = true), NotNull]
        public int ContactUserCode { get; set; }

        [Indexed(Name = "UQ_ContactUserOnContactUserName", Order = 1, Unique = true), NotNull]
        public string ContactUserName { get; set; }

        [NotNull]
        public string ContactUserPassword { get; set; }

        [NotNull]
        public bool ContactUserIsAllowedAssociateProductBarCode { get; set; }

        [NotNull]
        public bool ContactUserIsAllowedPickingModuleOverridePicker { get; set; }

        [NotNull]
        public bool ContactUserIsAllowedPickingModuleOverrideAddManualDetail { get; set; }

        public string ContactUserLocalizationLanguage { get; set; }
    }
}
