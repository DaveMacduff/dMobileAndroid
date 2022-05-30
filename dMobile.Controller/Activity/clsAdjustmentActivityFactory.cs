// <copyright file="clsAdjustmentActivityFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// Defines the <see cref="AdjActivityFactory" />.
    /// </summary>
    public class AdjActivityFactory : ActivityFactory
    {
        /// <summary>
        /// Get the adjustment header activity.
        /// </summary>
        /// <returns><see cref="Activity"/> Adjustment activity header.</returns>
        public override Activity GetActivity()
        {
            return new AdjActivity();
        }
    }
}
