// <copyright file="IContactLogin.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    /// <summary>
    /// Defines the <see cref="IContactLoginEventListener" />.
    /// </summary>
    public interface IContactLoginEventListener
    {
        /// <summary>
        /// The OnContactLoginStatusOut.
        /// </summary>
        /// <param name="contactLoginEventPublisher">The contactLoginEventPublisher<see cref="IContactLoginEventPublisher"/>.</param>
        /// <param name="contactLoginEventArgs">The contactLoginEventArgs<see cref="ContactLoginEventArgs"/>.</param>
        void OnContactLoginStatusOut(IContactLoginEventPublisher contactLoginEventPublisher,
            ContactLoginEventArgs contactLoginEventArgs);
    }

    /// <summary>
    /// Defines the <see cref="IContactLoginEventPublisher" />.
    /// </summary>
    public interface IContactLoginEventPublisher
    {
        /// <summary>
        /// The RegisterContactLoginListenerStatusOut.
        /// </summary>
        /// <param name="contactLoginEventListener">The contactLoginEventListener<see cref="IContactLoginEventListener"/>.</param>
        void RegisterContactLoginListenerStatusOut(
            IContactLoginEventListener contactLoginEventListener);

        /// <summary>
        /// The UnregisterContactLoginListenerStatusOut.
        /// </summary>
        /// <param name="contactLoginEventListener">The contactLoginEventListener<see cref="IContactLoginEventListener"/>.</param>
        void UnregisterContactLoginListenerStatusOut(
            IContactLoginEventListener contactLoginEventListener);
    }
}
