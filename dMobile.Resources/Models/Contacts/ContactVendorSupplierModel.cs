// <copyright file="ContactVendorSupplierModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models.Contacts
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a contact vendor supplier.
    /// </summary>
    [Table("tContactVendorSupplier")]
    public class ContactVendorSupplierModel
    {
        /// <summary>
        /// Gets or sets the reference number of the contact.
        /// </summary>
        [PrimaryKey, NotNull]
        [ForeignKey(typeof(ContactModel))]
        [Indexed(Name = "UQ_ContactVendorSupplierOnContactRefnoAndContactSubType", Order = 1, Unique = true)]
        public int ContactRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the contact.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ContactVendorSupplierOnContactRefnoAndContactSubType", Order = 2, Unique = true)]
        public int ContactSubType { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the vendor contact.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(ContactVendorModel))]
        public int VendorContactRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the contact vendor supplier.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ContactVendorSupplierOnContactVendorSupplierFKRefno", Order = 1, Unique = true)]
        public int ContactVendorSupplierFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the code of the contact vendor supplier.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ContactVendorSupplierOnContactVendorSupplierCode", Order = 1, Unique = true)]
        public string ContactVendorSupplierCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the contact vendor supplier.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ContactVendorSupplierOnContactVendorSupplierName", Order = 1, Unique = true)]
        public string ContactVendorSupplierName { get; set; }
    }
}