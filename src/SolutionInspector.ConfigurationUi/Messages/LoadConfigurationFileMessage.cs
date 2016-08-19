namespace SolutionInspector.ConfigurationUi.Messages
{
  internal class LoadConfigurationFileMessage
  {
    public string ConfigurationFilePath { get; }

    public LoadConfigurationFileMessage (string configurationFilePath)
    {
      ConfigurationFilePath = configurationFilePath;
    }
  }
}