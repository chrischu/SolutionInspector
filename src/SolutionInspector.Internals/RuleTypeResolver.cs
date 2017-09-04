using System;
using System.Linq;
using System.Reflection;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Attributes;

namespace SolutionInspector.Internals
{
  /// <summary>
  ///   Utility to resolve rule types from their name.
  /// </summary>
  [PublicApi]
  public interface IRuleTypeResolver
  {
    /// <summary>
    ///   Retrieves information about a rule type from its name.
    /// </summary>
    RuleTypeInfo Resolve (string ruleTypeName);

    /// <summary>
    ///   Retrieves information about a rule type from its type.
    /// </summary>
    RuleTypeInfo Resolve (Type ruleType);
  }

  internal class RuleTypeResolver : IRuleTypeResolver
  {
    public RuleTypeInfo Resolve (string ruleTypeName)
    {
      var ruleType = ResolveRuleType(ruleTypeName);
      return Resolve(ruleType);
    }
    public RuleTypeInfo Resolve(Type ruleType)
    {
      var configurationType = ResolveConfigurationType(ruleType);

      Func<ConstructorInfo, bool> constructorFilter = c => c.GetParameters().Length == 0;
      if (configurationType != null)
        constructorFilter = c => c.GetParameters().Length == 1 && c.GetParameters().Single().ParameterType == configurationType;
      var constructor = ResolveConstructor(
        ruleType,
        constructorFilter,
        configurationType == null ? "" : $" only taking a parameter of type '{configurationType.Name}' as a parameter");

      return new RuleTypeInfo(GetRuleTypeName(ruleType), ruleType, configurationType, constructor);
    }

    private string GetRuleTypeName(Type ruleType)
    {
      return $"{ruleType.FullName}, {ruleType.Assembly.GetName().Name}";
    }

    private ConstructorInfo ResolveConstructor (Type ruleType, Func<ConstructorInfo, bool> constructorFilter, string taking)
    {
      var validConstructors = ruleType.GetConstructors().Where(constructorFilter).ToArray();

      if (validConstructors.Length == 0)
        throw new RuleTypeResolvingException(
          $"The rule type '{ruleType.Name}' does not provide a public constructor{taking}.");

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