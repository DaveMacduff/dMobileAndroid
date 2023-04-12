// <copyright file="MessageOnlyException.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Exceptions
{
    using System;
    using dMobile.Bedrock;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="MessageOnlyException" />.
    /// </summary>
    public class MessageOnlyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageOnlyException"/> class.
        /// </summary>
        public MessageOnlyException()
          : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageOnlyException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        public MessageOnlyException(string message)
          : base(ModifyMessage(message))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageOnlyException"/> class.
        /// </summary>
        /// <param name="message"><see cref="string"/> Message to attach with exception.</param>
        /// <param name="inner"><see cref="Exception"/> Inner exception.</param>
        public MessageOnlyException(string message, Exception inner)
          : base(ModifyMessage(message), inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageOnlyException"/> class.
        /// </summary>
        /// <param name="message"><see cref="string"/> Message to attach with exception.</param>
        /// <param name="showMessageType"><see cref="SysCommon.ShowErrorMessageType"/> Message type to show.</param>
        public MessageOnlyException(string message, SysCommon.ShowErrorMessageType showMessageType)
          : base(ModifyMessage(message))
        {
            this.ShowMessageType = showMessageType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageOnlyException"/> class.
        /// </summary>
        /// <param name="message"><see cref="string"/> Message to attach with exception.</param>
        /// <param name="inner"><see cref="Exception"/> Inner exception.</param>
        /// <param name="showMessageType"><see cref="SysCommon.ShowErrorMessageType"/> Message type to show.</param>
        public MessageOnlyException(string message, Exception inner, SysCommon.ShowErrorMessageType showMessageType)
          : base(ModifyMessage(message), inner)
        {
            this.ShowMessageType = showMessageType;
        }

        /// <summary>
        /// Gets or sets the show error message type.
        /// </summary>
        public SysCommon.ShowErrorMessageType ShowMessageType { get; set; } = SysCommon.ShowErrorMessageType.MessageBox;

        /// <summary>
        /// The Show.
        /// </summary>
        /// <param name="actSubType">The actSubType<see cref="SysCommon.ActSubTypes"/>.</param>
        public void Show(SysCommon.ActSubTypes actSubType)
        {
            switch (this.ShowMessageType)
            {
                case SysCommon.ShowErrorMessageType.MessageBox:
                    Device.BeginInvokeOnMainThread(async () => await Application.Current.MainPage.DisplayAlert(
                        Resource.Error,
                        this.Message,
                        Resource.OK));
                    break;

                case SysCommon.ShowErrorMessageType.ErrorReport:
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var errRpt = new FormErrorReport(actSubType, this);
                        _ = await errRpt.ShowDialog();
                    });

                    break;

                default:
                    throw new Exception(Resource.UnsupportedSysCommonShowErrorMessageTypeCalled);
            }
        }

        /// <summary>
        /// Modify message text. Removes the expression "MSGBOX-".
        /// </summary>
        /// <param name="message"><see cref="string"/> Message text to alter.</param>
        /// <returns><see cref="string"/> Original or altered text.</returns>
        private static string ModifyMessage(string message)
        {
            var msgToken = "MSGBOX-";

            if (message.Contains(msgToken))
            {
                message = message[(message.IndexOf(msgToken, StringComparison.Ordinal) + msgToken.Length)..];
            }

            return message;
        }
    }
}
