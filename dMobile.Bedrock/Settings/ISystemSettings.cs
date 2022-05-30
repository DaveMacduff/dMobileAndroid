// <copyright file="ISystemSettings.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    /// <summary>
    /// Defines the <see cref="ISysSettingsEventListener" />.
    /// </summary>
    public interface ISysSettingsEventListener
    {
        /// <summary>
        /// The OnSysSettingsStatusOut.
        /// </summary>
        /// <param name="sysSettingsEventPublisher">The sysSettingsEventPublisher<see cref="ISysSettingsEventPublisher"/>.</param>
        /// <param name="sysSettingsEventArgs">The sysSettingsEventArgs<see cref="SysSettingsEventArgs"/>.</param>
        void OnSysSettingsStatusOut(
            ISysSettingsEventPublisher sysSettingsEventPublisher,
            SysSettingsEventArgs sysSettingsEventArgs);

        /// <summary>
        /// The OnSysSettingsProgressOut.
        /// </summary>
        /// <param name="sysSettingsEventPublisher">The sysSettingsEventPublisher<see cref="ISysSettingsEventPublisher"/>.</param>
        /// <param name="sysSettingsEventArgs">The sysSettingsEventArgs<see cref="SysSettingsEventArgs"/>.</param>
        void OnSysSettingsProgressOut(
            ISysSettingsEventPublisher sysSettingsEventPublisher,
            SysSettingsEventArgs sysSettingsEventArgs);
    }

    /// <summary>
    /// Defines the <see cref="ISysSettingsEventPublisher" />.
    /// </summary>
    public interface ISysSettingsEventPublisher
    {
        /// <summary>
        /// The RegisterSysSettingsListenerStatusOut.
        /// </summary>
        /// <param name="sysSettingsEventListener">The sysSettingsEventListener<see cref="ISysSettingsEventListener"/>.</param>
        void RegisterSysSettingsListenerStatusOut(ISysSettingsEventListener sysSettingsEventListener);

        /// <summary>
        /// The UnregisterSysSettingsListenerStatusOut.
        /// </summary>
        /// <param name="sysSettingsEventListener">The sysSettingsEventListener<see cref="ISysSettingsEventListener"/>.</param>
        void UnregisterSysSettingsListenerStatusOut(ISysSettingsEventListener sysSettingsEventListener);

        /// <summary>
        /// The RegisterSysSettingsListenerProgressOut.
        /// </summary>
        /// <param name="sysSettingsEventListener">The sysSettingsEventListener<see cref="ISysSettingsEventListener"/>.</param>
        void RegisterSysSettingsListenerProgressOut(ISysSettingsEventListener sysSettingsEventListener);

        /// <summary>
        /// The UnregisterSysSettingsListenerProgressOut.
        /// </summary>
        /// <param name="sysSettingsEventListener">The sysSettingsEventListener<see cref="ISysSettingsEventListener"/>.</param>
        void UnregisterSysSettingsListenerProgressOut(ISysSettingsEventListener sysSettingsEventListener);
    }
}
