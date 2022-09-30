// <copyright file="PutAwaySkidActDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity_Item;

namespace dMobile.Controller.Activity_Detail
{
    /// <summary>
    /// <see cref="PutAwaySkidActDetailFactory" /> Put away skid activity detail factory.
    /// </summary>
    public class PutAwaySkidActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// Get the put away skid activity detail factory.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns><see cref="ActDetail"/> Put away skid activity detail.</returns>
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new PutAwaySkidActDetail(actItem);
        }
    }
}
