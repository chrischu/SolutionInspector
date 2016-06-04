using System;
using System.Linq;
using System.Reflection;

namespace SolutionInspector.Api.Rules
{
  internal interface IRuleTypeResolver
  {
    RuleTypeInfo Resolve (string ruleTypeName);
  }

  internal class RuleTypeResolver : IRuleTypeResolver
  {
    public RuleTypeInfo Resolve (string ruleTypeName)
    {
      var ruleType = ResolveRuleType(ruleTypeName);
      var configurationType = ResolveConfigurationType(ruleType);

      Func<ConstructorInfo, bool> constructorFilter = c => c.GetParameters().Length == 0;
      if (configurationType != null)
        constructorFilter = c => c.GetParameters().Length == 1 && c.GetParameters().Any(p => p.ParameterType == configurationType);
      var constructor = ResolveConstructor(
          ruleType,
          constructorFilter,
          configurationType == null ? "" : $" only taking a parameter of type '{configurationType.Name}' as a parameter");

      return new RuleTypeInfo(ruleType, configurationType, constructor);
    }

    private ConstructorInfo ResolveConstructor (Type ruleType, Func<ConstructorInfo, bool> constructorFilter, string taking)
    {
      var validConstructors = ruleType.GetConstructors().Where(constructorFilter).ToArray();

      //taking a parameter of type '{configurationType.Name}' as a parameter.
      if (validConstructors.Length == 0)
        throw new RuleTypeResolvingException(
            $"The rule type '{ruleType.Name}' does not provide a public constructor{taking}.");

      if (validConstructors.Length > 1)
        throw new RuleTypeResolvingException(
            $"The rule type '{ruleType.Name}' has multiple constructors{taking}, but only one is allowed.");

      return validConstructors[0];
    }

    private static Type ResolveRuleType (string ruleTypeName)
    {
      var ruleType = Type.GetType(ruleTypeName);

      if (ruleType == null)
        throw new RuleTypeResolvingException($"Could not resolve rule type '{ruleTypeName}'.");

      if (!typeof(IRule).IsAssignableFrom(ruleType))
        throw new RuleTypeResolvingException($"The type '{ruleType.Name}' is not a valid rule type.");

      return ruleType;
    }

    private Type ResolveConfigurationType (Type ruleType)
    {
      if (!typeof(IConfigurableRule).IsAssignableFrom(ruleType))
        return null;

      return ruleType.GetInterfaces()
          .Single(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(IConfigurableRule<,>))
          .GenericTypeArguments[1];
    }
  }
}