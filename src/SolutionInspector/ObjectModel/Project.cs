using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using SystemInterface.IO;
using SystemWrapper.IO;
using JetBrains.Annotations;
using Microsoft.Build.Construction;
using SolutionInspector.Configuration.MsBuildParsing;
using SolutionInspector.Extensions;
using SolutionInspector.Rules;

namespace SolutionInspector.ObjectModel
{
  /// <summary>
  /// Represents a MSBuild project.
  /// </summary>
  [PublicAPI]
  public interface IProject : IRuleTarget
  {
    /// <summary>
    /// Contains advanced/raw properties of the underlying MSBuild file.
    /// </summary>
    IAdvancedProject Advanced { get; }

    /// <summary>
    /// The project's name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The name in which the project is stored, relative to the solution file.
    /// </summary>
    string FolderName { get; }

    /// <summary>
    /// A <see cref="IFileInfo"/> that represents the project file.
    /// </summary>
    IFileInfo ProjectFile { get; }

    /// <summary>
    /// All <see cref="BuildConfiguration"/>s in the project.
    /// </summary>
    IReadOnlyCollection<BuildConfiguration> BuildConfigurations { get; }

    /// <summary>
    /// The project's default namespace.
    /// </summary>
    string DefaultNamespace { get; }

    /// <summary>
    /// The project's assembly name.
    /// </summary>
    string AssemblyName { get; }

    /// <summary>
    /// The project's target framework version.
    /// </summary>
    Version TargetFrameworkVersion { get; }

    /// <summary>
    /// The project's output type.
    /// </summary>
    ProjectOutputType OutputType { get; }

    /// <summary>
    /// A <see cref="IFileInfo"/> that represents the project's NuGet packages file.
    /// </summary>
    IFileInfo NuGetPackagesFile { get; }

    /// <summary>
    /// A collection of referenced <see cref="NuGetPackage"/>s.
    /// </summary>
    IReadOnlyCollection<NuGetPackage> NuGetPackages { get; }

    /// <summary>
    /// A collection of DLLs referenced from the GAC.
    /// </summary>
    IReadOnlyCollection<GacReference> GacReferences { get; }

    /// <summary>
    /// A collection of DLLs referenced from NuGet.
    /// </summary>
    IReadOnlyCollection<NuGetReference> NuGetReferences { get; }

    /// <summary>
    /// A collection of DLLs referenced from the file system.
    /// </summary>
    IReadOnlyCollection<FileReference> FileReferences { get; }

    /// <summary>
    /// A collection of referenced projects.
    /// </summary>
    IReadOnlyCollection<ProjectReference> ProjectReferences { get; }

    /// <summary>
    /// The solution the project is contained in.
    /// </summary>
    ISolution Solution { get; }

    /// <summary>
    /// A collection of all project items contained in the project.
    /// </summary>
    IReadOnlyCollection<IProjectItem> ProjectItems { get; }

    /// <summary>
    /// The project configuration file (App.config/Web.config).
    /// </summary>
    IConfigurationProjectItem ConfigurationProjectItem { get; }
  }

  internal sealed class Project : IProject
  {
    private readonly IMsBuildParsingConfiguration _msBuildParsingConfiguration;
    private Lazy<ClassifiedReferences> _classifiedReferences; 

    private Project(ISolution solution, ProjectInSolution projectInSolution, Microsoft.Build.Evaluation.Project project, IMsBuildParsingConfiguration msBuildParsingConfiguration)
    {
      _msBuildParsingConfiguration = msBuildParsingConfiguration;
      Name = projectInSolution.ProjectName;

      BuildConfigurations =
          projectInSolution.ProjectConfigurations.Values.Select(c => new BuildConfiguration(c.ConfigurationName, c.PlatformName)).Distinct().ToArray();

      Advanced = new AdvancedProject(this, project, projectInSolution);

      NuGetPackages = BuildNuGetPackages(NuGetPackagesFile).ToArray();

      _classifiedReferences = new Lazy<ClassifiedReferences>(() => ClassifyReferences(project, NuGetPackages, solution));

      ProjectItems = BuildProjectItems(project.ItemsIgnoringCondition).ToArray();

      ConfigurationProjectItem = BuildConfigurationProjectItem();
    }

    private IConfigurationProjectItem BuildConfigurationProjectItem()
    {
      var configurationItem = ProjectItems.SingleOrDefault(
          i =>
              string.Equals(i.Include, "App.config", StringComparison.InvariantCultureIgnoreCase)
              || string.Equals(i.Include, "Web.config", StringComparison.InvariantCultureIgnoreCase));

      if (configurationItem == null)
        return null;

      return new ConfigurationProjectItem(this, configurationItem);
    }

    private IEnumerable<ProjectItem> BuildProjectItems(ICollection<Microsoft.Build.Evaluation.ProjectItem> msBuildProjectItems)
    {
      var projectItems =
          msBuildProjectItems.Where(i => !i.IsImported && _msBuildParsingConfiguration.IsValidProjectItem(i))
              .Select(p => ProjectItem.FromMsBuildProjectItem(this, p))
              .ToDictionary(i => i.Include);

      foreach(var projectItem in projectItems.Values)
      {
        var dependentUpon = projectItem.Metadata.GetValueOrDefault("DependentUpon");

        if(dependentUpon != null)
        {
          var dependentUponInclude = Path.Combine(Path.GetDirectoryName(projectItem.Include).AssertNotNull(), dependentUpon);

          var parent = projectItems[dependentUponInclude];
          projectItem.SetParent(parent);
        }
      }

      return projectItems.Values;
    }

    public IAdvancedProject Advanced { get; }

    public ISolution Solution { get; }

    public string Name { get; }
    public string FolderName => Path.GetFileName(Advanced.MsBuildProject.DirectoryPath);
    public IFileInfo ProjectFile => new FileInfoWrap(Advanced.MsBuildProject.FullPath);
    public IFileInfo NuGetPackagesFile => new FileInfoWrap(Path.Combine(Advanced.MsBuildProject.DirectoryPath, "packages.config"));

    public IReadOnlyCollection<BuildConfiguration> BuildConfigurations { get; }

    public string DefaultNamespace => Advanced.Properties["RootNamespace"];
    public string AssemblyName => Advanced.Properties["AssemblyName"];
    public Version TargetFrameworkVersion => Version.Parse(Advanced.Properties["TargetFrameworkVersion"].TrimStart('v'));
    public ProjectOutputType OutputType => Advanced.Properties["OutputType"] == "Exe" ? ProjectOutputType.Executable : ProjectOutputType.Library;

    public IReadOnlyCollection<NuGetPackage> NuGetPackages { get; }

    public IReadOnlyCollection<GacReference> GacReferences => _classifiedReferences.Value.GacReferences;
    public IReadOnlyCollection<NuGetReference> NuGetReferences => _classifiedReferences.Value.NuGetReferences;
    public IReadOnlyCollection<FileReference> FileReferences => _classifiedReferences.Value.FileReferences;
    public IReadOnlyCollection<ProjectReference> ProjectReferences => _classifiedReferences.Value.ProjectReferences;

    public IReadOnlyCollection<IProjectItem> ProjectItems { get; } 

    public IConfigurationProjectItem ConfigurationProjectItem { get; }

    string IRuleTarget.Identifier => Path.GetFileName(Advanced.MsBuildProject.FullPath);
    string IRuleTarget.FullPath => Advanced.MsBuildProject.FullPath;

    private IEnumerable<NuGetPackage> BuildNuGetPackages(IFileInfo nuGetPackagesFile)
    {
      if (!nuGetPackagesFile.Exists)
        yield break;

      var doc = new XmlDocument();
      doc.Load(nuGetPackagesFile.FullName);

      foreach (var packageElement in doc.SelectNodes("//package").AssertNotNull().Cast<XmlElement>())
        yield return NuGetPackage.FromXmlElement(packageElement);
    }

    private ClassifiedReferences ClassifyReferences(
        Microsoft.Build.Evaluation.Project project,
        IReadOnlyCollection<NuGetPackage> nuGetPackages,
        ISolution solution)
    {
      var projectReferences =
          project.GetItemsIgnoringCondition("ProjectReference")
              .Select(
                  r =>
                      new ProjectReference(
                          solution.Projects.Single(
                              p => p.ProjectFile.FullName == Path.GetFullPath(Path.Combine(project.DirectoryPath, r.EvaluatedInclude)))));

      var dllReferences =
          project.GetItemsIgnoringCondition("Reference")
              .Select(r => new ReferenceItem(r.EvaluatedInclude, r.Metadata.ToDictionary(m => m.Name, m => m.EvaluatedValue)));

      var gacReferences = new List<GacReference>();
      var fileReferences = new List<FileReference>();
      var nuGetReferences = new List<NuGetReference>();

      foreach (var reference in dllReferences)
      {
        if (reference.HintPath == null)
        {
          gacReferences.Add(new GacReference(reference.AssemblyName));
          continue;
        }

        var matchingNuGetPackage = nuGetPackages.SingleOrDefault(p => reference.HintPath.StartsWith($@"..\packages\{p.PackageDirectoryName}", StringComparison.Ordinal));

        if (matchingNuGetPackage != null)
          nuGetReferences.Add(
              new NuGetReference(
                  matchingNuGetPackage,
                  reference.AssemblyName,
                  reference.Metadata.ContainsKey("IsPrivate") && reference.Metadata["IsPrivate"] == "True",
                  reference.HintPath));
        else
          fileReferences.Add(new FileReference(reference.AssemblyName, reference.HintPath));
      }

      return new ClassifiedReferences(gacReferences, fileReferences, nuGetReferences, projectReferences);
    }

    public static Project FromSolution(ISolution solution, ProjectInSolution projectInSolution, IMsBuildParsingConfiguration msBuildParsingConfiguration)
    {
      return new Project(solution, projectInSolution, new Microsoft.Build.Evaluation.Project(projectInSolution.AbsolutePath), msBuildParsingConfiguration);
    }

    private class ReferenceItem
    {
      public AssemblyName AssemblyName { get; }

      public string HintPath => Metadata.GetValueOrDefault("HostName");

      public IReadOnlyDictionary<string, string> Metadata { get; }

      public ReferenceItem(string include, IReadOnlyDictionary<string, string> metadata)
      {
        AssemblyName = new AssemblyName(include);
        Metadata = metadata;
      }
    }

    private class ClassifiedReferences
    {
      public IReadOnlyCollection<GacReference> GacReferences { get; }
      public IReadOnlyCollection<FileReference> FileReferences { get; }
      public IReadOnlyCollection<NuGetReference> NuGetReferences { get; }
      public IReadOnlyCollection<ProjectReference> ProjectReferences { get; }

      public ClassifiedReferences(
          IEnumerable<GacReference> gacReferences,
          IEnumerable<FileReference> fileReferences,
          IEnumerable<NuGetReference> nuGetReferences,
          IEnumerable<ProjectReference> projectReferences)
      {
        GacReferences = gacReferences.ToArray();
        FileReferences = fileReferences.ToArray();
        NuGetReferences = nuGetReferences.ToArray();
        ProjectReferences = projectReferences.ToArray();
      }
    }
  }
}