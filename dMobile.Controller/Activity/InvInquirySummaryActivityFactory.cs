// <copyright file="InvInquirySummaryActivityFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity
{
    /// <summary>
    /// Defines the <see cref="InvInquirySummaryActivityFactory" />.
    /// </summary>
    public class InvInquirySummaryActivityFactory : ActivityFactory
    {
        /// <summary>
        /// Constructs and returns a new InvInquirySummaryActivity.
        /// </summary>
        /// <returns>New InvInquirySummaryActivity.</returns>
        public override Activity GetActivity()
        {
            return new InvInquirySummaryActivity();
        }
    }
}
