// <copyright file="clsAssignDefaultLocationActivityDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// Defines the <see cref="AssignDefaultLocActDetailFactory" /> Assign default location activity detail factory.
    /// </summary>
    public class AssignDefaultLocActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// Get activity detail for assign default location activity detail factory.
        /// </summary>
        /// <param name="activityItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns>The <see cref="ActDetail"/> Activity detail.</returns>
        public override ActDetail GetActDetail(ActItem activityItem)
        {
            return new AssignDefaultLocActDetail(activityItem);
        }
    }
}
