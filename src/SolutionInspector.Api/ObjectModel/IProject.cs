using System;
using System.Collections.Generic;
using System.Xml.Linq;
using JetBrains.Annotations;
using SolutionInspector.Api.Rules;
using Wrapperator.Interfaces.IO;

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
    ///   A collection of referenced <see cref="INuGetPackage" />s.
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
}