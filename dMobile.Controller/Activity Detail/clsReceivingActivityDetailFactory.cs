// <copyright file="clsReceivingActivityDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// <see cref="RecvActDetailFactory" /> Receiving activity detail factory.
    /// </summary>
    public class RecvActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// Get the receiving activity detail factory.
        /// </summary>
        /// <param name="activityItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns>The <see cref="ActDetail"/> Receiving activity detail.</returns>
        public override ActDetail GetActDetail(ActItem activityItem)
        {
            return new RecvActDetail(activityItem);
        }
    }
}
