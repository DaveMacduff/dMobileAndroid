// <copyright file="clsInventoryInquiryActivityDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// Defines the <see cref="InventoryInquiryActDetailFactory" /> Inventory inquiry activity detail factory.
    /// </summary>
    public class InventoryInquiryActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// Get inventory inquiry activity detail.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns><see cref="ActDetail"/> Inventory inquiry activity detail.</returns>
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new InventoryInquiryActDetail(actItem);
        }
    }
}
