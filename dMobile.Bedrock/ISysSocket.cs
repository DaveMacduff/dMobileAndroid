namespace dMobile.Bedrock
{
    public interface ISysSocketEventListener
    {
        void OnSysSocketStatusOut(ISysSocketEventPublisher sockPublisher,
            SysSocketEventArgs sockArgs);
    }

    public interface ISysSocketEventPublisher
    {
        void RegSockListenerStatusOut(ISysSocketEventListener sockListener);

        void UnregSockListenerStatusOut(ISysSocketEventListener sockListener);
    }
}
