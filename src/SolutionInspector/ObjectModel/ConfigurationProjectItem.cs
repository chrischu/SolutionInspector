using System.Xml.Linq;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.ObjectModel
{
  internal class ConfigurationProjectItem : ProjectItem, IConfigurationProjectItem
  {
    public ConfigurationProjectItem (IProject project, IProjectItem projectItem)
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