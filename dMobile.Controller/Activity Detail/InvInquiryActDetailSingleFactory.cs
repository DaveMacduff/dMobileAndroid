// <copyright file="InvInquiryActDetailSingleFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity_Detail
{
    using dMobile.Controller.Activity_Item;

    /// <summary>
    /// Defines the <see cref="InvInquiryActDetailSingleFactory" /> Inventory inquiry activity single detail factory.
    /// </summary>
    public class InvInquiryActDetailSingleFactory : ActDetailFactory
    {
        /// <summary>
        /// Get inventory inquiry activity single detail.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns><see cref="ActDetail"/> Inventory inquiry activity single detail.</returns>
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new InvInquiryActDetailSingle(actItem);
        }
    }
}
