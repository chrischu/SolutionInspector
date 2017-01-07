namespace SolutionInspector.ConfigurationUi.Messages
{
  internal class LoadConfigurationFileMessage
  {
    public LoadConfigurationFileMessage (string configurationFilePath)
    {
      ConfigurationFilePath = configurationFilePath;
    }

    public string ConfigurationFilePath { get; }
  }
}