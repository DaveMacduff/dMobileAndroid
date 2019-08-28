namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IAppListener
    {
        /// <summary>
        /// Used to indicate a close request to the containing form. Containing form need to 
        /// remove handlers.
        /// </summary>
        /// <param name="appEventPub"></param>
        /// <param name="appEventArgs"></param>
        void ReqClose(IAppPublisher appEventPub, AppEventArgs appEventArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventPub"></param>
        /// <param name="appEventArgs"></param>
        void ReqSaveEnable(IAppPublisher appEventPub, AppEventArgs appEventArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventPub"></param>
        /// <param name="appEventArgs"></param>
        void ReqSaveDisable(IAppPublisher appEventPub, AppEventArgs appEventArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventPub"></param>
        /// <param name="appEventArgs"></param>
        void ReqCancelEnable(IAppPublisher appEventPub, AppEventArgs appEventArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventPub"></param>
        /// <param name="appEventArgs"></param>
        void ReqCancelDisable(IAppPublisher appEventPub, AppEventArgs appEventArgs);
    }
}
