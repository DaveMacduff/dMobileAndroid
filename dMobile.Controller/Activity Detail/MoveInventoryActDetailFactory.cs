// <copyright file="MoveInventoryActDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// <see cref="clsMoveInventoryActDetailFactory" /> Move inventory activity detail.
    /// </summary>
    public class clsMoveInventoryActDetailFactory : ActDetailFactory
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
