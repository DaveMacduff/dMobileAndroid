namespace dMobile.Bedrock
{
  public interface IBarCodeSettingsEventListener
  {
    void OnBarCodeSettingsStatusOut(
        IBarCodeSettingsEventPublisher barCodeSettingsEventPublisher,
        BarCodeSettingsEventArgs barCodeSettingsEventArgs);

    void OnBarCodeSettingsProgressOut(
        IBarCodeSettingsEventPublisher barCodeSettingsEventPublisher,
        BarCodeSettingsEventArgs barCodeSettingsEventArgs);
  }

  public interface IBarCodeSettingsEventPublisher
  {
    void RegisterBarCodeSettingsListenerStatusOut(IBarCodeSettingsEventListener barCodeSettingsEventListener);

    void UnregisterBarCodeSettingsListenerStatusOut(IBarCodeSettingsEventListener barCodeSettingsEventListener);

    void RegisterBarCodeSettingsListenerProgressOut(IBarCodeSettingsEventListener barCodeSettingsEventListener);

    void UnregisterBarCodeSettingsListenerProgressOut(IBarCodeSettingsEventListener barCodeSettingsEventListener);
  }
}
