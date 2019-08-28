namespace dMobile.Bedrock
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;

    public class MessageOnlyException : Exception
    {
        #region Constructors

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
        /// <returns></returns>
        private static string ModifyMessage(string message)
        {
            string msgToken = "MSGBOX-";
            string errMsg = string.Empty;

            if (message.Contains(msgToken))
            {
                errMsg = message.Substring(message.IndexOf(msgToken) + msgToken.Length);
            }
            else
            {
                errMsg = message;
            }

            return errMsg;
        }

        #endregion
    }
}
