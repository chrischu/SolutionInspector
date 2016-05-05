using System;
using System.Configuration;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration.Infrastructure;

namespace SolutionInspector.Api.Configuration.MsBuildParsing
{
  [UsedImplicitly]
  [ConfigurationCollection(typeof(ProjectBuildActionConfigurationElement), AddItemName = "projectBuildAction",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
  internal class ProjectBuildActionsConfigurationElement : KeyedConfigurationElementCollectionBase<ProjectBuildActionConfigurationElement, string>
  {

  }
}