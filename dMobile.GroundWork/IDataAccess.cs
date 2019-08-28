namespace dMobile.GroundWork
{
  public interface IDataAccessEventListener
  {
    void OnDataAccessStatusOut(
        IDataAccessEventPublisher dataAccessEventPublisher, 
        DataAccessEventArgs dataAccessEventArgs);

    void OnDataAccessProgressOut(
        IDataAccessEventPublisher dataAccessEventPublisher, 
        DataAccessEventArgs dataAccessEventArgs);
  }

  public interface IDataAccessEventPublisher
  {
    void RegisterDataAccessListenerStatusOut(IDataAccessEventListener dataAccessEventListener);

    void UnregisterDataAccessListenerStatusOut(IDataAccessEventListener dataAccessEventListener);

    void RegisterDataAccessListenerProgressOut(IDataAccessEventListener dataAccessEventListener);

    void UnregisterDataAccessListenerProgressOut(IDataAccessEventListener dataAccessEventListener);
  }
}
