using System;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration.Infrastructure;

namespace SolutionInspector.Api.Configuration.MsBuildParsing
{
  [UsedImplicitly]
  internal class ProjectBuildActionsConfigurationElement : KeyedConfigurationElementCollectionBase<ProjectBuildActionConfigurationElement, string>
  {
    protected override string ElementName => "projectBuildAction";
  }
}