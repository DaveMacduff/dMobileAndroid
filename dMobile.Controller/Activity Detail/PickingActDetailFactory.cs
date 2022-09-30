// <copyright file="PickingActDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity_Item;

namespace dMobile.Controller.Activity_Detail
{
    /// <summary>
    /// <see cref="PickingActDetailFactory" /> Picking activity detail factory.
    /// </summary>
    public class PickingActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// Get the picking activity detail.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns><see cref="ActDetail"/> Picking activity detail.</returns>
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new PickingActDetail(actItem);
        }
    }
}
