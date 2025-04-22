// <copyright file="InvInquiryActivitySingleFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines the <see cref="InvInquirySummaryActivityFactory" />.
    /// </summary>
    public class InvInquiryActivitySingleFactory : ActivityFactory
    {
        /// <summary>
        /// Constructs and returns a new InvInquirySummaryActivity.
        /// </summary>
        /// <returns>New InvInquirySummaryActivity.</returns>
        public override Activity GetActivity()
        {
            return new InvInquiryActivitySingle();
        }
    }
}
