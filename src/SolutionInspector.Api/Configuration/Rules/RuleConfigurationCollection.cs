using System;
using System.Configuration;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration.Infrastructure;

namespace SolutionInspector.Api.Configuration.Rules
{
  [UsedImplicitly]
  [ConfigurationCollection(typeof(RuleConfigurationElement), AddItemName = "rule")]
  internal class RuleConfigurationCollection : KeyedConfigurationElementCollectionBase<RuleConfigurationElement, string>
  {

  }
}