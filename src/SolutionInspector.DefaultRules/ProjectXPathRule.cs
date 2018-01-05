using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;
using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration.Attributes;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Evaluates an XPath expression (configured via <see cref="XPath" />) against the project XML file
  ///   and returns a violation if it does not evaluate to <c>true</c>.
  /// </summary>
  /// <remarks>
  ///   Please note that in order to make writing XPath expressions easier all namespaces are ignored. To change this configure the rule like this:
  ///   <code>&lt;rule type="..." xPath="..." ignoreNamespaces="false" /&gt;</code>
  /// </remarks>
  [Description (
    "Evaluates an XPath expression (configured via 'xPath') against the project XML file and returns a violation if it does not " +
    "evaluate to true. By default all XML namespaces are ignored, if that is not desirable, change the 'ignoreNamespaces' property to false")]
  public class ProjectXPathRule : ProjectRule
  {
    /// <summary>
    ///   XPath expression that should evaluate to true.
    /// </summary>
    [ConfigurationValue]
    [Description ("XPath expression that should evaluate to true.")]
    public string XPath
    {
      get => GetConfigurationValue<string>();
      set => SetConfigurationValue(value);
    }

    /// <summary>
    ///   Controls if XML namespaces should be ignored during XPath evaluation.
    /// </summary>
    [ConfigurationValue(IsOptional = true, DefaultValue = "true")]
    [Description ("Controls whether XML namespaces should be ignored or not during XPath evaluation.")]
    public bool IgnoreNamespaces
    {
      get => GetConfigurationValue<bool>();
      set => SetConfigurationValue(value);
    }

    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate ([NotNull] IProject target)
    {
      var xdoc = GetXDocumentForXPathEvaluation(target.ProjectXml, IgnoreNamespaces);
      var result = xdoc.XPathEvaluate(XPath);

      if (result.GetType() != typeof(bool))
        throw new InvalidXPathExpressionException(XPath);

      var boolResult = (bool) result;

      if (!boolResult)
        yield return new RuleViolation(this, target, $"The XPath expression '{XPath}' did not evaluate to 'true', but to 'false'.");
    }

    private XDocument GetXDocumentForXPathEvaluation (XDocument xdoc, bool ignoreNamespaces)
    {
      return ignoreNamespaces ? StripNamespaces(xdoc) : xdoc;
    }

    [ExcludeFromCodeCoverage]
    private XDocument StripNamespaces (XDocument xdoc)
    {
      var copy = new XDocument(xdoc);

      foreach (var element in copy.Root.AssertNotNull().DescendantsAndSelf())
      {
        if (element.Name.Namespace != XNamespace.None)
          element.Name = XNamespace.None.GetName(element.Name.LocalName);

        if (element.Attributes().Any(a => a.IsNamespaceDeclaration || a.Name.Namespace != XNamespace.None))
        {
          var newAttributes =
              element.Attributes()
                  .Where(a => !a.IsNamespaceDeclaration)
                  .Select(a => new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value));

          element.ReplaceAttributes(newAttributes);
        }
      }

      return copy;
    }

    /// <summary>
    ///   Occurs when the XPath expression used in <see cref="XPath" /> does not evaluate to a boolean value.
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
}