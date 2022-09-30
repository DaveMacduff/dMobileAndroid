// <copyright file="PhysicalCountActDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity_Item;

namespace dMobile.Controller.Activity_Detail
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
