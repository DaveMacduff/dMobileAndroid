// <copyright file="clsInventoryInquirySummaryActivityItemFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity_Item
{
    public class InvInquirySummaryActItemFactory : ActItemFactory
    {
        /// <summary>
        /// Constructs and returns a new InvInquirySummaryActItem.
        /// </summary>
        /// <param name="activity">Activity to use.</param>
        /// <returns>New InvInquirySummaryActItem.</returns>
        public override ActItem GetActItem(Activity.Activity activity)
        {
            return new InvInquirySummaryActItem(activity);
        }
    }
}
