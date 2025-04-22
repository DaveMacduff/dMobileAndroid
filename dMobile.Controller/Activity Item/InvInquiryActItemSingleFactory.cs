// <copyright file="InvInquiryActItemSingleFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity_Item
{
    /// <summary>
    /// Inventory Inquiry Activity Item Single Factory.
    /// </summary>
    public class InvInquiryActItemSingleFactory : ActItemFactory
    {
        /// <summary>
        /// Constructs and returns a new InvInquirySummaryActItem.
        /// </summary>
        /// <param name="activity">Activity to use.</param>
        /// <returns>New InvInquirySummaryActItem.</returns>
        public override ActItem GetActItem(Activity.Activity activity)
        {
            return new InvInquiryActItemSingle(activity);
        }
    }
}
