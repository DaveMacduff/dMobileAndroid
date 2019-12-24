using System;
using dMobile.Bedrock;
using dMobile.View.Forms;
using Xamarin.Forms;
using Resource = dMobile.Resources.Properties.Resources;

namespace dMobile.View.Exceptions
{
    class MessageOnlyException : Exception
    {
        private SysCommon.ShowErrorMessageType showMessageType = SysCommon.ShowErrorMessageType.MessageBox;

        public SysCommon.ShowErrorMessageType ShowMessageType
        {
            get
            {
                return this.showMessageType;
            }

            set
            {
                this.showMessageType = value;
            }
        }

        public MessageOnlyException()
          : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public MessageOnlyException(string message)
          : base(ModifyMessage(message))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public MessageOnlyException(string message, Exception inner)
          : base(ModifyMessage(message), inner)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public MessageOnlyException(string message, SysCommon.ShowErrorMessageType showMessageType)
          : base(ModifyMessage(message))
        {
            this.ShowMessageType = showMessageType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public MessageOnlyException(string message, Exception inner, SysCommon.ShowErrorMessageType showMessageType)
          : base(ModifyMessage(message), inner)
        {
            this.ShowMessageType = showMessageType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public void Show(/*UserControl control, */ SysCommon.ActSubTypes actSubType)
        {
            switch (this.ShowMessageType)
            {
                case SysCommon.ShowErrorMessageType.MessageBox:
                    Device.BeginInvokeOnMainThread(() => App.Current.MainPage.DisplayAlert(Resource.Error, this.Message, Resource.OK));

                    break;

                case SysCommon.ShowErrorMessageType.ErrorReport:
                    Device.BeginInvokeOnMainThread(() => {
                        FormErrorReport errRpt = new FormErrorReport(actSubType, this);
                        errRpt.ShowDialog().ContinueWith(result => { });
                    });

                    break;

                default:
                    throw new Exception(Resource.UnsupportedSysCommonShowErrorMessageTypeCalled);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string ModifyMessage(string message)
        {
            string msgToken = "MSGBOX-";

            if (message.Contains(msgToken))
            {
                message = message.Substring(message.IndexOf(msgToken) + msgToken.Length);
            }

            return message;
        }
    }
}