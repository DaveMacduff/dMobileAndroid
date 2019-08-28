namespace dMobile.Bedrock
{
  public interface ISysSettingsEventListener
  {
    void OnSysSettingsStatusOut(
        ISysSettingsEventPublisher sysSettingsEventPublisher,
        SysSettingsEventArgs sysSettingsEventArgs);

    void OnSysSettingsProgressOut(
        ISysSettingsEventPublisher sysSettingsEventPublisher,
        SysSettingsEventArgs sysSettingsEventArgs);
  }

  public interface ISysSettingsEventPublisher
  {
    void RegisterSysSettingsListenerStatusOut(ISysSettingsEventListener sysSettingsEventListener);

    void UnregisterSysSettingsListenerStatusOut(ISysSettingsEventListener sysSettingsEventListener);

    void RegisterSysSettingsListenerProgressOut(ISysSettingsEventListener sysSettingsEventListener);
    
    void UnregisterSysSettingsListenerProgressOut(ISysSettingsEventListener sysSettingsEventListener);
  }
}
