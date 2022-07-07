// <copyright file="ConvertUnitActDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// Convert unit activity detail factory.
    /// </summary>
    public class ConvertUnitActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// Get a convert unit activity detail object.
        /// </summary>
        /// <param name="activityItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns><see cref="ActDetail"/> Activity detail.</returns>
        public override ActDetail GetActDetail(ActItem activityItem)
        {
            return new ConvertUnitActDetail(activityItem);
        }
    }
}
