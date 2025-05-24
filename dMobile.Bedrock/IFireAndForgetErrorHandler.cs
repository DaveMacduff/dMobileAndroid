// <copyright file="IFireAndForgetErrorHandler.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Fire and forget error handler interface.
    /// </summary>
    public interface IFireAndForgetErrorHandler
    {
        /// <summary>
        /// Handle error.
        /// </summary>
        /// <param name="ex"><see cref="Exception"/> Exception.</param>
        void FireAndForgetHandleError(Exception ex);
    }
}
