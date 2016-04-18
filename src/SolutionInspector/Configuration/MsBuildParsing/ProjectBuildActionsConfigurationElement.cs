using System.Configuration;
using JetBrains.Annotations;
using SolutionInspector.Configuration.Infrastructure;

namespace SolutionInspector.Configuration.MsBuildParsing
{
  [UsedImplicitly]
  [ConfigurationCollection(typeof(ProjectBuildActionConfigurationElement), AddItemName = "projectBuildAction",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
  internal class ProjectBuildActionsConfigurationElement : KeyedConfigurationElementCollectionBase<ProjectBuildActionConfigurationElement, string>
  {

  }
}