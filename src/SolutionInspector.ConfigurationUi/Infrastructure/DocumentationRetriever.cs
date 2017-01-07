using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace SolutionInspector.ConfigurationUi.Infrastructure
{
  internal interface IDocumentationRetriever
  {
    string RetrieveRuleDocumentation (Type ruleType);
    string RetrieveRuleConfigurationDocumentation (Type ruleConfigurationType, string name);
  }

  internal class DocumentationRetriever : IDocumentationRetriever
  {
    private static readonly Dictionary<Assembly, XDocument> s_assemblyDocuments = new Dictionary<Assembly, XDocument>();

    public string RetrieveRuleDocumentation (Type ruleType)
    {
      var description = ruleType.GetCustomAttribute<DescriptionAttribute>();
      return description?.Description;
      //var documentation = GetDocumentation(ruleType.Assembly);
      //if (documentation == null)
      //  return null;

      //var element = documentation.XPathSelectElement($"//member[@name='T:{ruleType}']");
      //var summary = element.Element("summary").AssertNotNull();

      //return summary.Value.Trim();
    }

    public string RetrieveRuleConfigurationDocumentation (Type ruleConfigurationType, string name)
    {
      throw new NotImplementedException();
    }

    [CanBeNull]
    private XDocument GetDocumentation (Assembly assembly)
    {
      XDocument xdoc;
      if (s_assemblyDocuments.TryGetValue(assembly, out xdoc))
        return xdoc;

      var codeBase = new Uri(assembly.CodeBase);
      var documentationFile = Path.ChangeExtension(codeBase.AbsolutePath, "xml");

      var documentation = XDocument.Load(documentationFile);
      return s_assemblyDocuments[assembly] = documentation;
    }
  }
}