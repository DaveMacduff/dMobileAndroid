// <copyright file="IDMobileModal.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.CustomPages.Modal
{
    using System.Threading.Tasks;
    using Resources.Enums;

    /// <summary>
    /// Defines the <see cref="IDMobileModal" />.
    /// </summary>
    public interface IDMobileModal
    {
        /// <summary>
        /// Gets or sets the DialogResult.
        /// </summary>
        DialogResult DialogResult { get; set; }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public Task<DialogResult> ShowDialog();

        /// <summary>
        /// Close the dialog.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public Task Close();
    }
}
