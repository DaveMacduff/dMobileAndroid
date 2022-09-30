// <copyright file="AdjActDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity_Item;

namespace dMobile.Controller.Activity_Detail
{
    /// <summary>
    /// Defines the <see cref="AdjActDetailFactory" /> adjustment activity detail factory.
    /// </summary>
    public class AdjActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// Get the adjustment activity detail object.
        /// </summary>
        /// <param name="activityItem"><see cref="ActItem"/> Activity item object.</param>
        /// <returns><see cref="ActDetail"/> Adjustment activity detail object.</returns>
        public override ActDetail GetActDetail(ActItem activityItem)
        {
            return new AdjActDetail(activityItem);
        }
    }
}
