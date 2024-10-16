// <copyright file="ContactVendorModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models.Contacts
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a contact vendor.
    /// </summary>
    [Table("tContactVendor")]
    public class ContactVendorModel
    {
        /// <summary>
        /// Gets or sets the reference number of the contact.
        /// </summary>
        [PrimaryKey, NotNull]
        [ForeignKey(typeof(ContactModel))]
        [Indexed(Name = "UQ_ContactVendorOnContactRefnoAndContactSubType", Order = 1, Unique = true)]
        public int ContactRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the contact.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ContactVendorOnContactRefnoAndContactSubType", Order = 2, Unique = true)]
        public int ContactSubType { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the contact vendor.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ContactVendorOnContactVendorFKRefno", Order = 1, Unique = true)]
        public int ContactVendorFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the code of the contact vendor.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ContactVendorOnContactVendorCode", Order = 1, Unique = true)]
        public string ContactVendorCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the contact vendor.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ContactVendorOnContactVendorName", Order = 1, Unique = true)]
        public string ContactVendorName { get; set; }
    }
}