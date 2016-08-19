using System.Xml.Linq;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   A project item representing the project's configuration file (App.config/Web.config).
  /// </summary>
  public interface IConfigurationProjectItem : IProjectItem
  {
    /// <summary>
    ///   The XML contents of the configuration file.
    /// </summary>
    XDocument ConfigurationXml { get; }
  }
}