using System;
using System.ComponentModel;
using System.Reflection;

namespace SolutionInspector.ConfigurationUi.Infrastructure
{
  internal interface IDocumentationRetriever
  {
    string RetrieveRuleDocumentation (Type ruleType);
  }

  internal class DocumentationRetriever : IDocumentationRetriever
  {
    public string RetrieveRuleDocumentation (Type ruleType)
    {
      var description = ruleType.GetCustomAttribute<DescriptionAttribute>();
      return description?.Description;
    }
  }
}