// <copyright file="TransferActDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity_Item;

namespace dMobile.Controller.Activity_Detail
{
    /// <summary>
    /// <see cref="TransferActDetailFactory" /> Warehouse transfer activity detail factory.
    /// </summary>
    public class TransferActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// Get the warehouse transfer activity detail.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns><see cref="ActDetail"/> Warehouse transfer activity detail.</returns>
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new TransferActDetail(actItem);
        }
    }
}
