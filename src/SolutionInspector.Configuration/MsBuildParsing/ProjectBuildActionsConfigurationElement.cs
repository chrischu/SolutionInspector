using System;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration;

namespace SolutionInspector.Configuration.MsBuildParsing
{
  [UsedImplicitly]
  internal class ProjectBuildActionsConfigurationElement : KeyedConfigurationElementCollectionBase<ProjectBuildActionConfigurationElement, string>
  {
    protected override string ElementName => "projectBuildAction";
  }
}