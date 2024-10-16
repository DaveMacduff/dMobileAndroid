// <copyright file="IDMobileModal.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.CustomPages.Modal
{
    using System.Threading.Tasks;
    using Resources.Enums;

    public interface IDMobileModal
    {
        DialogResult DialogResult { get; set; }
        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public Task<DialogResult> ShowDialog();
        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public Task Close();
    }
}
