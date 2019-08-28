namespace dMobile.Bedrock
{
    public interface IContactLoginEventListener
    {
        void OnContactLoginStatusOut(IContactLoginEventPublisher contactLoginEventPublisher,
            ContactLoginEventArgs contactLoginEventArgs);
    }

    public interface IContactLoginEventPublisher
    {
        void RegisterContactLoginListenerStatusOut(
            IContactLoginEventListener contactLoginEventListener);

        void UnregisterContactLoginListenerStatusOut(
            IContactLoginEventListener contactLoginEventListener);
    }
}
