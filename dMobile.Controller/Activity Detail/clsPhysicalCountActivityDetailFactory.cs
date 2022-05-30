// <copyright file="clsPhysicalCountActivityDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// <see cref="PhysicalCountActDetailFactory" /> Physical count activity detail.
    /// </summary>
    public class PhysicalCountActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// Get the physical count activity detail.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity detail.</param>
        /// <returns><see cref="ActDetail"/> Physical count activity detail.</returns>
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new PhysicalCountActDetail(actItem);
        }
    }
}
