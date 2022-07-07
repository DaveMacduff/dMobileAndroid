// <copyright file="SkidInfoActDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// <see cref="SkidInfoActDetailFactory" /> Skid information activity detail factory.
    /// </summary>
    public class SkidInfoActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// Get the skid information activity detail factory.
        /// </summary>
        /// <param name="activityItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns><see cref="ActDetail"/> Skid information activity detail.</returns>
        public override ActDetail GetActDetail(ActItem activityItem)
        {
            return new SkidInfoActDetail(activityItem);
        }
    }
}
