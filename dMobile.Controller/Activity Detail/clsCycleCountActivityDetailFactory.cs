// <copyright file="clsCycleCountActivityDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// Defines the <see cref="CycleCountActDetailFactory" /> Cycle count activity detail factory.
    /// </summary>
    public class CycleCountActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// The GetActDetail.
        /// </summary>
        /// <param name="actItem">The actItem<see cref="ActItem"/>.</param>
        /// <returns>The <see cref="ActDetail"/>.</returns>
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new CycleCountActDetail(actItem);
        }
    }
}
