using JetBrains.Annotations;

namespace SolutionInspector.Api.Configuration.MsBuildParsing
{
  [UsedImplicitly]
  internal class ProjectBuildActionsConfigurationElement : KeyedConfigurationElementCollectionBase<ProjectBuildActionConfigurationElement, string>
  {
    protected override string ElementName => "projectBuildAction";
  }
}