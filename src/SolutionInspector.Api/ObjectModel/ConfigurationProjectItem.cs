using System;
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

  internal class ConfigurationProjectItem : ProjectItem, IConfigurationProjectItem
  {
    public ConfigurationProjectItem (IProject project, IProjectItem projectItem)
      // TODO: Create a special constructor to avoid processing the MSBuild ProjectItem twice.
        : base(project, projectItem.OriginalProjectItem)
    {
      string xmlString;
      using (var str = File.OpenText())
      {
        xmlString = str.ReadToEnd();
      }
      ConfigurationXml = XDocument.Parse(xmlString);
    }

    public XDocument ConfigurationXml { get; }
  }
}