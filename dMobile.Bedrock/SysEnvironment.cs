// <copyright file="SysEnvironment.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// System Environment Class.
    /// </summary>
    public class SysEnvironment
    {
        // VARIABLES

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
        private static string deviceID = SysDeviceID.GetDeviceID();

        /// <summary>
        /// Contains system settings instance.
        /// </summary>
        /// <remarks>IMPORTANT to set to null, LoadData is called in get() in SystemSettings.</remarks>
        private static SystemSettings sysSettings = null;

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
        private static SysBarCodeSettings barCodeSettingsInstance = new SysBarCodeSettings().LoadData();

        /// <summary>
        /// Test if last communication test was successful.
        /// </summary>
        private static bool isCommunicationTestSuccessful = false;

        /// <summary>
        /// Are we a Windows Mobile 6 device.
        /// </summary>
        private static bool isWindowsMobile6Device = false;

        /// <summary>
        /// Allow login double click logo to override device mode licenses.
        /// </summary>
        private static bool isEnableAllModuleBypassLicense = false;

        // CONSTRUCTOR

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
        public string DeviceID
        {
            get => deviceID;

            set => deviceID = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to bypass the device module license,
        /// double clicking the login logo.
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
                if (string.IsNullOrEmpty(sysSock.ServerIP))
                {
                    sysSock.BasePort = this.CommSetup.BaseSocketPort;
                    sysSock.DefSocketPort = this.CommSetup.DefaultSocketPort;
                    sysSock.MaxPorts = this.CommSetup.MaxSocketPorts;
                    sysSock.ConnTotalRetries = this.CommSetup.ConnectionAttempts;
                    sysSock.SendTotalRetries = this.CommSetup.SendAttempts;
                    sysSock.RecvTotalRetries = this.CommSetup.ReceiveAttempts;
                    sysSock.SendTimeOutMilliseconds = this.CommSetup.SendTimeOutSeconds * 1000;
                    sysSock.ReceiveTimeOutMilliseconds = this.CommSetup.ReceiveTimeOutSeconds * 1000;
                    sysSock.RecvBuffSize = this.CommSetup.ReceiveBufferSize;
                    sysSock.ServerIP = this.CommSetup.HostIpAddressComponent1.ToString() +
                        "." + this.CommSetup.HostIpAddressComponent2.ToString() + "." +
                        this.CommSetup.HostIpAddressComponent3.ToString() + "." +
                        this.CommSetup.HostIpAddressComponent4.ToString();
                }

                return sysSock;
            }

            set => sysSock = value;
        }

        /// <summary>
        /// Gets or sets the bar code settings object.
        /// </summary>
        /// <remarks>A new instance will be created if one does not exist.</remarks>
        public SysBarCodeSettings BarCodeSettings
        {
            get
            {
                if (barCodeSettingsInstance == null)
                {
                    barCodeSettingsInstance = new SysBarCodeSettings();
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
        /// Gets or sets a value indicating whether the OS is a windows mobile 6 variant.
        /// </summary>
        public bool IsWindowsMobile6Device
        {
            get
            {
                // Windows Mobile 6 = Version 5.2
                // Windows Mobile 5 = Version 5.1
                // Windows Mobile 2003 SE = Version 4.21

                // If the OS is Windows Mobile Professional 6.0
                var platform = SysPlatformDetector.GetPlatform();
                if (platform == Platform.PocketPC & Environment.OSVersion.Version.Major == 5 &
                    Environment.OSVersion.Version.Minor == 2)
                {
                    isWindowsMobile6Device = true;
                }
                else
                {
                    isWindowsMobile6Device = false;
                }

                return isWindowsMobile6Device;
            }

            set => isWindowsMobile6Device = value;
        }

        /// <summary>
        /// Gets or sets the inserted detail reference number.
        /// </summary>
        public int InsertedDetailRefno { get; set; }

        /// <summary>
        /// Gets or sets the inserted detail foreign key reference number.
        /// </summary>
        public int InsertedDetailFkRefno { get; set; }

        // METHODS

        /// <summary>
        /// Return an instance of self. Create a new instance if self does not
        /// exist yet or return the current one.
        /// </summary>
        /// <returns>A new or current singleton instance.</returns>
        public static SysEnvironment GetInstance()
        {
            if (sysEnv == null)
            {
                sysEnv = new SysEnvironment();
            }

            return sysEnv;
        }

        /// <summary>
        /// Reload or reload properties with any new values.
        /// </summary>
        public static void RefreshEnv()
        {
            sysEnv.CurrentBarCodeModifier1 = string.Empty;
            sysEnv.DeviceID = SysDeviceID.GetDeviceID();

            var sysSettings = sysEnv.SysSettings;
            SystemSettingsTasks.LoadData(ref sysSettings);
            sysEnv.SysSettings = sysSettings;

            sysEnv.RefreshOpt = refreshOpt.LoadData();
            sysEnv.BarCodeSettings = barCodeSettingsInstance.LoadData();
            sysEnv.CommSetup = commSetup.LoadData();
            sysEnv.ContactLog = contactLog.LoadData();

            sysEnv.SysSock.BasePort = commSetup.BaseSocketPort;
            sysEnv.SysSock.DefSocketPort = commSetup.DefaultSocketPort;
            sysEnv.SysSock.MaxPorts = commSetup.MaxSocketPorts;
            sysEnv.SysSock.ConnTotalRetries = commSetup.ConnectionAttempts;
            sysEnv.SysSock.SendTotalRetries = commSetup.SendAttempts;
            sysEnv.SysSock.RecvTotalRetries = commSetup.ReceiveAttempts;
            sysEnv.SysSock.SendTimeOutMilliseconds = commSetup.SendTimeOutSeconds * 1000;
            sysEnv.SysSock.ReceiveTimeOutMilliseconds = commSetup.ReceiveTimeOutSeconds * 1000;
            sysEnv.SysSock.RecvBuffSize = commSetup.ReceiveBufferSize;
            sysEnv.SysSock.ServerIP = commSetup.HostIpAddressComponent1.ToString() +
                "." + commSetup.HostIpAddressComponent2.ToString() + "." +
                commSetup.HostIpAddressComponent3.ToString() + "." +
                commSetup.HostIpAddressComponent4.ToString();
        }
    }
}
