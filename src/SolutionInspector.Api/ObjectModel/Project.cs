using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Api.Extensions;
using SolutionInspector.Api.Rules;
using Wrapperator.Interfaces.IO;
using Wrapperator.Wrappers;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a MSBuild project.
  /// </summary>
  [PublicAPI]
  public interface IProject : IRuleTarget, IDisposable
  {
    /// <summary>
    ///   Contains advanced/raw properties of the underlying MSBuild file.
    /// </summary>
    IAdvancedProject Advanced { get; }

    /// <summary>
    ///   The project's <see cref="Guid" />.
    /// </summary>
    Guid Guid { get; }

    /// <summary>
    ///   The project's name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///   The name of the folder in which the project is stored, relative to the solution file.
    /// </summary>
    string FolderName { get; }

    /// <summary>
    ///   A <see cref="IFileInfo" /> that represents the project file.
    /// </summary>
    IFileInfo ProjectFile { get; }

    /// <summary>
    /// A <see cref="IDirectoryInfo"/> that represents the project directory.
    /// </summary>
    IDirectoryInfo ProjectDirectory { get; }

    /// <summary>
    ///   A <see cref="XDocument" /> that represents the project file.
    /// </summary>
    XDocument ProjectXml { get; }

    /// <summary>
    ///   All <see cref="BuildConfiguration" />s in the project.
    /// </summary>
    IReadOnlyCollection<BuildConfiguration> BuildConfigurations { get; }

    /// <summary>
    ///   The project's default namespace.
    /// </summary>
    string DefaultNamespace { get; }

    /// <summary>
    ///   The project's assembly name.
    /// </summary>
    string AssemblyName { get; }

    /// <summary>
    ///   The project's target framework version.
    /// </summary>
    Version TargetFrameworkVersion { get; }

    /// <summary>
    ///   The project's output type.
    /// </summary>
    ProjectOutputType OutputType { get; }

    /// <summary>
    ///   A <see cref="IFileInfo" /> that represents the project's NuGet packages file.
    /// </summary>
    IFileInfo NuGetPackagesFile { get; }

    /// <summary>
    ///   A collection of referenced <see cref="NuGetPackage" />s.
    /// </summary>
    IReadOnlyCollection<INuGetPackage> NuGetPackages { get; }

    /// <summary>
    ///   A collection of DLLs referenced from the GAC.
    /// </summary>
    IReadOnlyCollection<IGacReference> GacReferences { get; }

    /// <summary>
    ///   A collection of DLLs referenced from NuGet.
    /// </summary>
    IReadOnlyCollection<INuGetReference> NuGetReferences { get; }

    /// <summary>
    ///   A collection of DLLs referenced from the file system.
    /// </summary>
    IReadOnlyCollection<IFileReference> FileReferences { get; }

    /// <summary>
    ///   A collection of referenced projects.
    /// </summary>
    IReadOnlyCollection<IProjectReference> ProjectReferences { get; }

    /// <summary>
    ///   The solution the project is contained in.
    /// </summary>
    ISolution Solution { get; }

    /// <summary>
    ///   A collection of all project items contained in the project.
    /// </summary>
    IReadOnlyCollection<IProjectItem> ProjectItems { get; }

    /// <summary>
    ///   The project configuration file (App.config/Web.config).
    /// </summary>
    IConfigurationProjectItem ConfigurationProjectItem { get; }

    /// <summary>
    ///   Get the include path for the given <paramref name="projectToInclude" /> (relative to the current project file).
    /// </summary>
    string GetIncludePathFor (IProject projectToInclude);
  }

  internal sealed class Project : IProject
  {
    private readonly ProjectCollection _projectCollection;
    private readonly IMsBuildParsingConfiguration _msBuildParsingConfiguration;
    private Lazy<ClassifiedReferences> _classifiedReferences;
    private Lazy<XDocument> _projectXml;

    private Project (
        ProjectCollection projectCollection,
        ISolution solution,
        ProjectInSolution projectInSolution,
        Microsoft.Build.Evaluation.Project project,
        IMsBuildParsingConfiguration msBuildParsingConfiguration)
    {
      _projectCollection = projectCollection;

      Solution = solution;

      _msBuildParsingConfiguration = msBuildParsingConfiguration;
      Name = projectInSolution.ProjectName;

      BuildConfigurations =
          projectInSolution.ProjectConfigurations.Values.Select(c => new BuildConfiguration(c.ConfigurationName, c.PlatformName)).Distinct().ToArray();

      Advanced = new AdvancedProject(this, project, projectInSolution);

      Guid = Guid.Parse(projectInSolution.ProjectGuid);

      NuGetPackages = BuildNuGetPackages(NuGetPackagesFile).ToArray();

      _classifiedReferences = new Lazy<ClassifiedReferences>(() => ClassifyReferences(project, NuGetPackages, solution));

      ProjectItems = BuildProjectItems(project.ItemsIgnoringCondition).ToArray();

      ConfigurationProjectItem = BuildConfigurationProjectItem();

      _projectXml = new Lazy<XDocument>(() => XDocument.Load(ProjectFile.FullName));
    }

    private IConfigurationProjectItem BuildConfigurationProjectItem ()
    {
      var configurationItem = ProjectItems.SingleOrDefault(
          i =>
              string.Equals(i.Include.Unevaluated, "App.config", StringComparison.InvariantCultureIgnoreCase)
              || string.Equals(i.Include.Unevaluated, "Web.config", StringComparison.InvariantCultureIgnoreCase));

      if (configurationItem == null)
        return null;

      return new ConfigurationProjectItem(this, configurationItem);
    }

    private IEnumerable<ProjectItem> BuildProjectItems (ICollection<Microsoft.Build.Evaluation.ProjectItem> msBuildProjectItems)
    {
      var projectItems =
          msBuildProjectItems.Where(i => !i.IsImported && _msBuildParsingConfiguration.IsValidProjectItemType(i.ItemType))
              .Select(p => ProjectItem.FromMsBuildProjectItem(this, p))
              .ToLookup(i => i.Include.Evaluated);

      foreach (var projectItem in projectItems.SelectMany(g => g))
      {
        var dependentUpon = projectItem.Metadata.GetValueOrDefault("DependentUpon");

        if (dependentUpon != null)
        {
          var dependentUponInclude = Path.Combine(Path.GetDirectoryName(projectItem.Include.Evaluated).AssertNotNull(), dependentUpon);

          var parents = projectItems[dependentUponInclude];
          foreach (var parent in parents)
            projectItem.SetParent(parent);
        }
      }

      return projectItems.SelectMany(g => g);
    }

    public IAdvancedProject Advanced { get; }

    public Guid Guid { get; }

    public ISolution Solution { get; }

    public string Name { get; }
    public string FolderName => Path.GetFileName(Advanced.MsBuildProject.DirectoryPath);
    public IFileInfo ProjectFile => Wrapper.Wrap(new FileInfo(Advanced.MsBuildProject.FullPath));
    public IDirectoryInfo ProjectDirectory => Wrapper.Wrap(new DirectoryInfo(Advanced.MsBuildProject.DirectoryPath));
    public XDocument ProjectXml => _projectXml.Value;
    public IFileInfo NuGetPackagesFile => Wrapper.Wrap(new FileInfo(Path.Combine(Advanced.MsBuildProject.DirectoryPath, "packages.config")));

    public IReadOnlyCollection<BuildConfiguration> BuildConfigurations { get; }

    public string DefaultNamespace => Advanced.Properties.GetValueOrDefault("RootNamespace")?.DefaultValue;
    public string AssemblyName => Advanced.Properties.GetValueOrDefault("AssemblyName")?.DefaultValue;

    public Version TargetFrameworkVersion
    {
      get
      {
        var targetFrameworkVersion = Advanced.Properties.GetValueOrDefault("TargetFrameworkVersion")?.DefaultValue;
        return targetFrameworkVersion != null ? Version.Parse(targetFrameworkVersion.TrimStart('v')) : null;
      }
    }

    public ProjectOutputType OutputType
      => Advanced.Properties.GetValueOrDefault("OutputType")?.DefaultValue == "Exe" ? ProjectOutputType.Executable : ProjectOutputType.Library;

    public IReadOnlyCollection<INuGetPackage> NuGetPackages { get; }

    public IReadOnlyCollection<IGacReference> GacReferences => _classifiedReferences.Value.GacReferences;
    public IReadOnlyCollection<INuGetReference> NuGetReferences => _classifiedReferences.Value.NuGetReferences;
    public IReadOnlyCollection<IFileReference> FileReferences => _classifiedReferences.Value.FileReferences;
    public IReadOnlyCollection<IProjectReference> ProjectReferences => _classifiedReferences.Value.ProjectReferences;

    public IReadOnlyCollection<IProjectItem> ProjectItems { get; }

    public IConfigurationProjectItem ConfigurationProjectItem { get; }

    public string GetIncludePathFor (IProject projectToInclude)
    {
      var includeUri = new Uri(projectToInclude.ProjectFile.FullName);
      var selfUri = new Uri(ProjectFile.FullName);
      var relativeUri = selfUri.MakeRelativeUri(includeUri);
      return relativeUri.OriginalString.Replace('/', '\\');
    }

    string IRuleTarget.Identifier => Path.GetFileName(Advanced.MsBuildProject.FullPath);
    string IRuleTarget.FullPath => Advanced.MsBuildProject.FullPath;

    private IEnumerable<INuGetPackage> BuildNuGetPackages (IFileInfo nuGetPackagesFile)
    {
      if (!nuGetPackagesFile.Exists)
        yield break;

      var doc = new XmlDocument();
      doc.Load(nuGetPackagesFile.FullName);

      foreach (var packageElement in doc.SelectNodes("//package").AssertNotNull().Cast<XmlElement>())
        yield return NuGetPackage.FromXmlElement(packageElement);
    }

    private ClassifiedReferences ClassifyReferences (
        Microsoft.Build.Evaluation.Project project,
        IReadOnlyCollection<INuGetPackage> nuGetPackages,
        ISolution solution)
    {
      var projectReferences = project.GetItemsIgnoringCondition("ProjectReference").Select(r => new ProjectReference(solution, r));

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

        var matchingNuGetPackage =
            nuGetPackages.SingleOrDefault(p => reference.HintPath.StartsWith($@"..\packages\{p.PackageDirectoryName}", StringComparison.Ordinal));

        if (matchingNuGetPackage != null)
          nuGetReferences.Add(
              new NuGetReference(
                  matchingNuGetPackage,
                  reference.AssemblyName,
                  reference.Metadata.GetValueOrDefault("Private") == "True",
                  reference.HintPath,
                  project.DirectoryPath));
        else
          fileReferences.Add(new FileReference(reference.AssemblyName, reference.HintPath, project.DirectoryPath));
      }

      return new ClassifiedReferences(gacReferences, fileReferences, nuGetReferences, projectReferences);
    }

    [SuppressMessage ("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposal happens in Dispose().")]
    public static Project FromSolution (
        ISolution solution,
        ProjectInSolution projectInSolution,
        IMsBuildParsingConfiguration msBuildParsingConfiguration)
    {
      var projectCollection = new ProjectCollection();
      var msBuildProject = new Microsoft.Build.Evaluation.Project(
          projectInSolution.AbsolutePath,
          null,
          null,
          projectCollection,
          ProjectLoadSettings.IgnoreMissingImports);

      var project = new Project(
          projectCollection,
          solution,
          projectInSolution,
          msBuildProject,
          msBuildParsingConfiguration);

      return project;
    }

    private class ReferenceItem
    {
      public AssemblyName AssemblyName { get; }

      public string HintPath => Metadata.GetValueOrDefault("HintPath");

      public IReadOnlyDictionary<string, string> Metadata { get; }

      public ReferenceItem (string include, IReadOnlyDictionary<string, string> metadata)
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

      public ClassifiedReferences (
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

    public void Dispose ()
    {
      _projectCollection.UnloadAllProjects();
      _projectCollection.Dispose();
    }
  }
}