namespace dMobile.Bedrock
{
  public interface IPrnSettingsListener
  {
    void OnPrnSettingsStatusOut(
        IPrnSettingsPublisher prnSettingsPublisher, 
        PrnSettingsArgs prnSettingsArgs);

    void OnPrnSettingsProgressOut(
        IPrnSettingsPublisher prnSettingsPublisher,
        PrnSettingsArgs prnSettingsArgs);
  }

  public interface IPrnSettingsPublisher
  {
    void RegPrnSettingsListenerStatusOut(IPrnSettingsListener prnSettingsListener);
    void UnregPrnSettingsListenerStatusOut(IPrnSettingsListener prnSettingListener);
    void RegPrnSettingsListenerProgressOut(IPrnSettingsListener prnSettingsListener);
    void UnregPrnSettingsListenerProgressOut(IPrnSettingsListener prnSettingsListener);
  }
}
