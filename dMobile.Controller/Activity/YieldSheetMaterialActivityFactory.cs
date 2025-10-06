// <copyright file="YieldSheetMaterialActivityFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity
{
    /// <summary>
    /// Defines the <see cref="YieldSheetMaterialActivityFactory" />.
    /// </summary>
    public class YieldSheetMaterialActivityFactory : ActivityFactory
    {
        /// <summary>
        /// Get the activity.
        /// </summary>
        /// <returns>The <see cref="Activity"/>.</returns>
        public override Activity GetActivity()
        {
            return new YieldSheetMaterialActivity();
        }
    }
}
