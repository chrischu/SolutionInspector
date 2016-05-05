using System;
using System.Linq;

namespace SolutionInspector.Api.Rules
{
  internal static class RuleTypeUtility
  {
    public static Type GetConfigurationType(Type ruleType)
    {
      if (!typeof (IRule).IsAssignableFrom(ruleType))
        throw new ArgumentException($"Given type '{ruleType}' is not a valid rule type.", nameof(ruleType));

      if (!typeof (IConfigurableRule).IsAssignableFrom(ruleType))
        return null;

      return ruleType.GetInterfaces()
          .Single(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof (IConfigurableRule<,>))
          .GenericTypeArguments[1];
    }
  }
}