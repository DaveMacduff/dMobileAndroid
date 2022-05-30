// <copyright file="ComboBoxMembers.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using dMobile.Resources.Types;

    /// <summary>
    /// Defines the <see cref="ComboBoxMembers" />.
    /// </summary>
    public class ComboBoxMembers
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComboBoxMembers"/> class.
        /// </summary>
        /// <param name="displayMember">.</param>
        /// <param name="sort">The sort<see cref="Sort"/>.</param>
        public ComboBoxMembers(string displayMember, Sort sort)
        {
            this.DisplayMember = displayMember;
            this.Sort = sort;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComboBoxMembers"/> class.
        /// </summary>
        /// <param name="displayMember">The displayMember<see cref="string"/>.</param>
        /// <param name="valueMember">The valueMember<see cref="string"/>.</param>
        public ComboBoxMembers(string displayMember, string valueMember)
        {
            this.DisplayMember = displayMember;
            this.ValueMember = valueMember;
        }

        /// <summary>
        /// Gets or sets the display member.
        /// </summary>
        public string DisplayMember { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the value member.
        /// </summary>
        public string ValueMember { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sort information.
        /// </summary>
        public Sort Sort { get; set; } = new Sort();
    }
}
