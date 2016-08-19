using JetBrains.Annotations;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.RuleAssemblyImports
{
  /// <summary>
  ///   Represents a set of rule assemblies that should be imported.
  /// </summary>
  [PublicAPI]
  public interface IRuleAssemblyImport
  {
    /// <summary>
    ///   The path (file/directory) where the rule assembly/assemblies are located.
    /// </summary>
    string Path { get; }
  }

  internal class RuleAssemblyImportConfigurationElement : ConfigurationElement, IRuleAssemblyImport
  {
    [ConfigurationValue]
    public string Path
    {
      get { return GetConfigurationProperty<string>(); }
      set { SetConfigurationProperty(value); }
    }
  }
}