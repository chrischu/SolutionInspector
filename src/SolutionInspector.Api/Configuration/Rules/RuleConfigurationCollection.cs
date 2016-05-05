using System;
using System.Configuration;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration.Infrastructure;

namespace SolutionInspector.Api.Configuration.Rules
{
  [UsedImplicitly]
  internal class RuleConfigurationCollection : KeyedConfigurationElementCollectionBase<RuleConfigurationElement, string>
  {
    protected override string ElementName => "rule";
  }
}