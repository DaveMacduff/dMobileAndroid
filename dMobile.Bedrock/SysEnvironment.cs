// <copyright file="SysEnvironment.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using dMobile.Bedrock.Settings;
    using System;

    /// <summary>
    /// System Environment Class.
    /// </summary>
    public class SysEnvironment
    {
        /// <summary>
        /// Contains system environment settings unique instance. For singleton.
        /// </summary>
        private static SysEnvironment sysEnv = new SysEnvironment();

        /// <summary>
        /// Contains current bar code modifier.
        /// </summary>
        private static string currentBarCodeModifier1 = string.Empty;

        /// <summary>
        /// Contains device id.
        /// </summary>
        private static string deviceId = SysDeviceId.GetDeviceId();

        /// <summary>
        /// Contains system settings instance.
        /// </summary>
        /// <remarks>IMPORTANT to set to null, LoadData is called in get() in SystemSettings.</remarks>
        private static SystemSettings sysSettings;

        /// <summary>
        /// Contains refresh data settings instance.
        /// </summary>
        private static RefreshDataOptions refreshOpt = new RefreshDataOptions().LoadData();

        /// <summary>
        /// Contain current login information.
        /// </summary>
        private static ContactLogin contactLog = new ContactLogin().LoadData();

        /// <summary>
        /// Contains communication settings.
        /// </summary>
        private static CommunicationSetup commSetup = new CommunicationSetup().LoadData();

        /// <summary>
        /// Contains current socket instance.
        /// </summary>
        private static SysSocket sysSock = new SysSocket();

        /// <summary>
        /// Contains current bar code settings and rules.
        /// </summary>
        private static BarCodeSettings barCodeSettingsInstance = new BarCodeSettings().LoadData();

        /// <summary>
        /// Test if last communication test was successful.
        /// </summary>
        private static bool isCommunicationTestSuccessful;

        /// <summary>
        /// Allow login double click logo to override device mode licenses.
        /// </summary>
        private static bool isEnableAllModuleBypassLicense;

        /// <summary>
        /// Contains the product scheme data for keeping last used data entered.
        /// </summary>
        private static ProductSchemeData lastEnteredSchemeData = new ProductSchemeData();

        /// <summary>
        /// Prevents a default instance of the <see cref="SysEnvironment"/>
        /// class from being created.
        /// </summary>
        /// <remarks>
        /// Visibility is private for implementing singleton.
        /// </remarks>
        private SysEnvironment()
        {
        }

        /// <summary>
        /// Gets or sets the last entered scheme data.
        /// </summary>
        public ProductSchemeData LastEnteredSchemeData
        {
            get => lastEnteredSchemeData;

            set => lastEnteredSchemeData = value;
        }

        /// <summary>
        /// Gets or sets the current bar code modifier.
        /// </summary>
        public string CurrentBarCodeModifier1
        {
            get => currentBarCodeModifier1;

            set => currentBarCodeModifier1 = value;
        }

        /// <summary>
        /// Gets or sets the current device id.
        /// </summary>
        public string DeviceId
        {
            get => deviceId;

            set => deviceId = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to bypass the device module license,
        /// double-clicking the login logo.
        /// </summary>
        public bool IsEnableAllModuleBypassLicense
        {
            get => isEnableAllModuleBypassLicense;

            set => isEnableAllModuleBypassLicense = value;
        }

        /// <summary>
        /// Gets or sets the system settings object.
        /// </summary>
        /// <remarks>A new instance will be created if one does not exist.</remarks>
        public SystemSettings SysSettings
        {
            get
            {
                if (sysSettings == null)
                {
                    sysSettings = new SystemSettings();
                    SystemSettingsTasks.LoadData(ref sysSettings);
                }

                return sysSettings;
            }

            set => sysSettings = value;
        }

        /// <summary>
        /// Gets or sets the refresh data options object.
        /// </summary>
        /// <remarks>A new instance will be created if one does not exist.</remarks>
        public RefreshDataOptions RefreshOpt
        {
            get
            {
                if (refreshOpt == null)
                {
                    refreshOpt = new RefreshDataOptions();
                    refreshOpt = refreshOpt.LoadData();
                }

                return refreshOpt;
            }

            set => refreshOpt = value;
        }

        /// <summary>
        /// Gets or sets the contact login object.
        /// </summary>
        public ContactLogin ContactLog
        {
            get => contactLog;
            set => contactLog = value;
        }

        /// <summary>
        /// Gets or sets the communication setup object.
        /// </summary>
        /// <remarks>A new instance will be created if one does not exist.</remarks>
        public CommunicationSetup CommSetup
        {
            get
            {
                if (commSetup == null)
                {
                    commSetup = new CommunicationSetup();
                    commSetup = commSetup.LoadData();
                }

                return commSetup;
            }

            set => commSetup = value;
        }

        /// <summary>
        /// Gets or sets the communication socket object.
        /// </summary>
        /// <remarks>Loads the socket with the current settings from the communication settings
        /// object.</remarks>
        public SysSocket SysSock
        {
            get
            {
                if (string.IsNullOrEmpty(sysSock.ServerIp))
                {
                    sysSock.BasePort = this.CommSetup.BaseSocketPort;
                    sysSock.DefSocketPort = this.CommSetup.DefaultSocketPort;
                    sysSock.MaxPorts = this.CommSetup.MaxSocketPorts;
                    sysSock.ConnTotalRetries = this.CommSetup.ConnectionAttempts;
                    sysSock.SendTotalRetries = this.CommSetup.SendAttempts;
                    sysSock.ReceiveTotalRetries = this.CommSetup.ReceiveAttempts;
                    sysSock.SendTimeOutMilliseconds = this.CommSetup.SendTimeOutSeconds * 1000;
                    sysSock.ReceiveTimeOutMilliseconds = this.CommSetup.ReceiveTimeOutSeconds * 1000;
                    sysSock.ReceiveBuffSize = this.CommSetup.ReceiveBufferSize;
                    sysSock.ServerIp = this.CommSetup.HostIpAddressComponent1 +
                        "." + this.CommSetup.HostIpAddressComponent2 + "." +
                        this.CommSetup.HostIpAddressComponent3 + "." +
                        this.CommSetup.HostIpAddressComponent4;
                }

                return sysSock;
            }

            set => sysSock = value;
        }

        /// <summary>
        /// Gets or sets the bar code settings object.
        /// </summary>
        /// <remarks>A new instance will be created if one does not exist.</remarks>
        public BarCodeSettings BarCodeSettings
        {
            get
            {
                if (barCodeSettingsInstance == null)
                {
                    barCodeSettingsInstance = new BarCodeSettings();
                    barCodeSettingsInstance = barCodeSettingsInstance.LoadData();
                }

                return barCodeSettingsInstance;
            }

            set => barCodeSettingsInstance = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the last communication test was successful.
        /// </summary>
        public bool IsCommunicationTestSuccessful
        {
            get => isCommunicationTestSuccessful;

            set => isCommunicationTestSuccessful = value;
        }

        /// <summary>
        /// Gets or sets the inserted detail reference number.
        /// </summary>
        public int InsertedDetailRefno { get; set; }

        /// <summary>
        /// Gets or sets the inserted detail foreign key reference number.
        /// </summary>
        public int InsertedDetailFkRefno { get; set; }

        /// <summary>
        /// Gets a value indicating whether the device is a Zebra scanner.
        /// </summary>
        public bool IsZebraScanner =>
            (Android.OS.Build.Manufacturer != null &&
             Android.OS.Build.Manufacturer.Equals("Zebra Technologies", StringComparison.OrdinalIgnoreCase));

        // METHODS

        /// <summary>
        /// Return an instance of self. Create a new instance if self does not
        /// exist yet or return the current one.
        /// </summary>
        /// <returns>A new or current singleton instance.</returns>
        public static SysEnvironment GetInstance()
        {
            return sysEnv ??= new SysEnvironment();
        }
    }
}
