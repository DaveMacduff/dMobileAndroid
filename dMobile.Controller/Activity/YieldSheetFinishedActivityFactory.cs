// <copyright file="YieldSheetFinishedActivityFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity
{
    /// <summary>
    /// Defines the <see cref="YieldSheetFinishedActivityFactory" />.
    /// </summary>
    public class YieldSheetFinishedActivityFactory : ActivityFactory
    {
        /// <summary>
        /// Get the activity.
        /// </summary>
        /// <returns>The <see cref="Activity"/>.</returns>
        public override Activity GetActivity()
        {
            return new YieldSheetFinishedActivity();
        }
    }
}
