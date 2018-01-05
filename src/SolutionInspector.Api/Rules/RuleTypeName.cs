using System;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  ///   Represents the <see cref="Type"/> name of a <see cref="IRule" />.
  /// </summary>
  public class RuleTypeName
  {
    public string RuleName { get; }
    public string AssemblyName { get; }

    private RuleTypeName (string ruleName, string assemblyName)
    {
      RuleName = ruleName;
      AssemblyName = assemblyName;
    }

    public static RuleTypeName FromRuleType (Type ruleType)
    {
      return new RuleTypeName(ruleType.FullName.AssertNotNull(), ruleType.Assembly.GetName().Name);
    }

    public override string ToString ()
    {
      return $"{RuleName}, {AssemblyName}";
    }
  }
}