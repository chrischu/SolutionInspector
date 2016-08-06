using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration.MsBuildParsing;

namespace SolutionInspector.Api.Configuration
{
  /// <summary>
  ///   Configuration for the SolutionInspector.
  /// </summary>
  public interface ISolutionInspectorConfiguration
  {
    /// <summary>
    ///   Controls how SolutionInspector parses MSBuild files.
    /// </summary>
    IMsBuildParsingConfiguration MsBuildParsing { get; }
  }

  [UsedImplicitly]
  [ExcludeFromCodeCoverage]
  internal class SolutionInspectorConfiguration : ConfigurationSectionGroup, ISolutionInspectorConfiguration
  {
    public IMsBuildParsingConfiguration MsBuildParsing => (MsBuildParsingConfigurationSection) Sections["msBuildParsing"];
  }
}