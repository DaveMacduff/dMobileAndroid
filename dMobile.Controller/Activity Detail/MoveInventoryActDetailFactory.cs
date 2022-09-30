// <copyright file="MoveInventoryActDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity_Item;

namespace dMobile.Controller.Activity_Detail
{
    /// <summary>
    /// <see cref="MoveInventoryActDetailFactory" /> Move inventory activity detail.
    /// </summary>
    public class MoveInventoryActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// The GetActDetail.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns><see cref="ActDetail"/> Move inventory activity detail.</returns>
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new MoveInventoryActDetail(actItem);
        }
    }
}
