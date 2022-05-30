// <copyright file="IDataLayer.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// Defines the <see cref="IDataLayerEventListener" /> data layer event listener.
    /// </summary>
    public interface IDataLayerEventListener
    {
        /// <summary>
        /// On data layer status out.
        /// </summary>
        /// <param name="publisher"><see cref="IDataLayerEventPublisher"/> Data layer event publisher.</param>
        /// <param name="eventArgs"><see cref="DataLayerEventArgs"/> Data layer event arguments.</param>
        void OnLayerStatOut(IDataLayerEventPublisher publisher, DataLayerEventArgs eventArgs);

        /// <summary>
        /// On data layer progress out.
        /// </summary>
        /// <param name="publisher"><see cref="IDataLayerEventPublisher"/> Data layer event publisher.</param>
        /// <param name="eventArgs"><see cref="DataLayerEventArgs"/> Data layer event arguments.</param>
        void OnLayerProgOut(IDataLayerEventPublisher publisher, DataLayerEventArgs eventArgs);
    }

    /// <summary>
    /// Defines the <see cref="IDataLayerEventPublisher" /> data layer event publisher.
    /// </summary>
    public interface IDataLayerEventPublisher
    {
        /// <summary>
        /// Register the data layer status out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataLayerEventListener"/> Data layer event listener.</param>
        void RegLayerListenerStatOut(IDataLayerEventListener listener);

        /// <summary>
        /// Unregister the data layer status out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataLayerEventListener"/> Data layer event listener.</param>
        void UnregLayerListenerStatOut(IDataLayerEventListener listener);

        /// <summary>
        /// Register the data layer progress out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataLayerEventListener"/> Data layer event listener.</param>
        void RegLayerListenerProgOut(IDataLayerEventListener listener);

        /// <summary>
        /// Unregister the data layer progress out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataLayerEventListener"/> Data layer event listener.</param>
        void UnregLayerListenerProgOut(IDataLayerEventListener listener);
    }
}
