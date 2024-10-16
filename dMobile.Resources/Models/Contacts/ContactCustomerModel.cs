// <copyright file="ContactCustomerModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models.Contacts
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a contact customer.
    /// </summary>
    [Table("tContactCustomer")]
    public class ContactCustomerModel
    {
        /// <summary>
        /// Gets or sets the reference number of the contact.
        /// </summary>
        [PrimaryKey, NotNull]
        [ForeignKey(typeof(ContactModel))]
        [Indexed(Name = "UQ_ContactCustomerOnContactRefnoAndContactSubType", Order = 1, Unique = true)]
        public int ContactRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the contact.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ContactCustomerOnContactRefnoAndContactSubType", Order = 2, Unique = true)]
        public int ContactSubType { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the contact customer.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ContactCustomerOnContactCustomerFKRefno", Order = 1, Unique = true)]
        public int ContactCustomerFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the code of the contact customer.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ContactCustomerOnContactCustomerCode", Order = 1, Unique = true)]
        public string ContactCustomerCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the contact customer.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ContactCustomerOnContactCustomerName", Order = 1, Unique = true)]
        public string ContactCustomerName { get; set; }
    }
}