namespace dMobile.GroundWork
{
  public interface IDataNubListener
  {
    void OnNubStatOut(IDataNubPublisher dataNubPublisher, DataNubEventArgs dataNubArgs);
  }

  public interface IDataNubPublisher
  {
    void RegNubListenerStatOut(IDataNubListener dataNubListener);

    void UnregNubListenerStatOut(IDataNubListener dataNubListener);
  }
}
