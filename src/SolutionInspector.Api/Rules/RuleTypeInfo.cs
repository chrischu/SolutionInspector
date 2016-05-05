using System;
using System.Reflection;
using JetBrains.Annotations;

namespace SolutionInspector.Api.Rules
{
  internal class RuleTypeInfo
  {
    public Type RuleType { get; }

    [CanBeNull]
    public Type ConfigurationType { get; }

    public ConstructorInfo Constructor { get; }

    public bool IsConfigurable => ConfigurationType != null;

    public RuleTypeInfo(Type ruleType, [CanBeNull] Type configurationType, ConstructorInfo constructor)
    {
      RuleType = ruleType;
      Constructor = constructor;
      ConfigurationType = configurationType;
    }
  }
}