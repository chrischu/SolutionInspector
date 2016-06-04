using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Xml.XPath;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Evaluates an XPath expression (configured via <see cref="ProjectXPathRuleConfiguration" />) against the project XML file
  ///   and returns a violation if it does not evaluate to <c>true</c>.
  /// </summary>
  public class ProjectXPathRule : ConfigurableProjectRule<ProjectXPathRuleConfiguration>
  {
    /// <inheritdoc />
    public ProjectXPathRule (ProjectXPathRuleConfiguration configuration)
        : base(configuration)
    {
    }

    public override IEnumerable<IRuleViolation> Evaluate (IProject target)
    {
      var result = target.ProjectXml.XPathEvaluate(Configuration.XPath);

      if (result.GetType() != typeof(bool))
        throw new InvalidXPathExpressionException(Configuration.XPath);

      var boolResult = (bool) result;

      if (!boolResult)
        yield return new RuleViolation(this, target, $"The XPath expression '{Configuration.XPath}' did not evaluate to 'true', but to 'false'.");
    }

    /// <summary>
    ///   Occurs when the XPath expression used in <see cref="ProjectXPathRuleConfiguration" /> does not evaluate to a boolean value.
    /// </summary>
    [Serializable]
    public class InvalidXPathExpressionException : Exception
    {
      /// <summary>
      ///   Creates a new <see cref="InvalidXPathExpressionException" />
      /// </summary>
      public InvalidXPathExpressionException (string xPathExpression, Exception innerException = null)
          : base($"The configured XPath expression '{xPathExpression}' does not evaluate to a boolean value.", innerException)
      {
      }

      /// <summary>
      ///   Serialization constructor.
      /// </summary>
      [ExcludeFromCodeCoverage /* Serialization ctor */]
      protected InvalidXPathExpressionException (SerializationInfo info, StreamingContext context)
          : base(info, context)
      {
      }
    }
  }

  /// <summary>
  ///   Configuration for the <see cref="ProjectXPathRule" />.
  /// </summary>
  public class ProjectXPathRuleConfiguration : ConfigurationElement
  {
    /// <summary>
    ///   XPath expression that should evaluate to true.
    /// </summary>
    [ConfigurationProperty ("xPath", DefaultValue = "", IsRequired = true)]
    public string XPath
    {
      get { return (string) this["xPath"]; }
      set { this["xPath"] = value; }
    }
  }
}