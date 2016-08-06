using System;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration;

namespace SolutionInspector.Configuration.Rules
{
  [UsedImplicitly]
  internal class RuleConfigurationCollection : KeyedConfigurationElementCollectionBase<RuleConfigurationElement, string>
  {
    protected override string ElementName => "rule";
  }
}