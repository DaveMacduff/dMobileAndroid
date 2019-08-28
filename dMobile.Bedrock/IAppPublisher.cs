namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IAppPublisher
    {
        void RegisterReqClose(IAppListener appEventListener);
        void UnregisterReqClose(IAppListener appEventListener);

        void RegisterReqSaveEnable(IAppListener appEventListener);
        void UnregisterReqSaveEnable(IAppListener appEventListener);

        void RegisterReqSaveDisable(IAppListener appEventListener);
        void UnregisterReqSaveDisable(IAppListener appEventListener);

        void RegisterReqCancelEnable(IAppListener appEventListener);
        void UnregisterReqCancelEnable(IAppListener appEventListener);

        void RegisterReqCancelDisable(IAppListener appEventListener);
        void UnregisterReqCancelDisable(IAppListener appEventListener);
    }
}
