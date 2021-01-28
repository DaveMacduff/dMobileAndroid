namespace dMobile.Controller
{
    public interface IDataLayerEventListener
    {
        void OnLayerStatOut(IDataLayerEventPublisher dataLayerEventPublisher, DataLayerEventArgs dataLayerEventArgs);

        void OnLayerProgOut(IDataLayerEventPublisher dataLayerEventPublisher, DataLayerEventArgs dataLayerEventArgs);
    }

    public interface IDataLayerEventPublisher
    {
        void RegLayerListenerStatOut(IDataLayerEventListener dataLayerEventListener);
        void UnregLayerListenerStatOut(IDataLayerEventListener dataLayerEventListener);

        void RegLayerListenerProgOut(IDataLayerEventListener dataLayerEventListener);
        void UnregLayerListenerProgOut(IDataLayerEventListener dataLayerEventListener);
    }
}
