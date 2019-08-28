namespace dMobile.Bedrock
{
  public interface IProdSchemeLabelSettingsListener
  {
    void OnProdSchemeLabelSettingsStatusOut(
        IProdSchemeLabelSettingsPublisher prodSchemeLabelSettingsPublisher,
        ProdSchemeLabelSettingsEventArgs prodSchemeLabelSettingsArgs);

    void OnProdSchemeLabelSettingsProgressOut(
        IProdSchemeLabelSettingsPublisher prodSchemeLabelSettingsPublisher,
        ProdSchemeLabelSettingsEventArgs prodSchemeLabelSettingsArgs);
  }

  public interface IProdSchemeLabelSettingsPublisher
  {
    void RegProdSchemeLabelSettingsListenerStatusOut(IProdSchemeLabelSettingsListener rodSchemeLabelSettingsListener);

    void UnregProdSchemeLabelSettingsListenerStatusOut(IProdSchemeLabelSettingsListener prodSchemeLabelSettingListener);

    void RegProdSchemeLabelSettingsListenerProgressOut(IProdSchemeLabelSettingsListener prodSchemeLabelSettingsListener);

    void UnregProdSchemeLabelSettingsListenerProgressOut(IProdSchemeLabelSettingsListener prodSchemeLabelSettingsListener);
  }
}
