using System;
using System.Configuration;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration.Infrastructure;

namespace SolutionInspector.Api.Configuration.RuleAssemblyImports
{
  /// <summary>
  /// Represents a set of rule assemblies that should be imported.
  /// </summary>
  [PublicAPI]
  public interface IRuleAssemblyImport
  {
    /// <summary>
    ///  The path (file/directory) where the rule assembly/assemblies are located.
    /// </summary>
    string Path { get; }
  }

  internal class RuleAssemblyImportConfigurationElement : KeyedConfigurationElement<string>, IRuleAssemblyImport
  {
    [ConfigurationProperty("path")]
    public string Path => Key;

    public override string KeyName => "path";
  }
}