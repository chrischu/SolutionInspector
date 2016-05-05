using System;
using System.Collections.Generic;
using SolutionInspector.Api.Extensions;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that a project is configured so that XML documentation is generated.
  /// </summary>
  public class ProjectMustProvideXmlDocumentationRule : ProjectRule
  {
    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate (IProject target)
    {
      foreach (var matchingBuildConfig in target.BuildConfigurations)
      {
        var properties = target.Advanced.ConfigurationDependentProperties[matchingBuildConfig];

        var documentationFile = properties.GetValueOrDefault("DocumentationFile");

        if (documentationFile == null)
          yield return
              new RuleViolation(this, target, $"In the build configuration '{matchingBuildConfig}' the XML documentation configuration is missing.");
        else
        {
          var outputPath = properties["OutputPath"];
          var expectedDocumentationFile = $"{outputPath}{target.Name}.XML";

          if (documentationFile != expectedDocumentationFile)
            yield return
                new RuleViolation(
                    this,
                    target,
                    $"In the build configuration '{matchingBuildConfig}' the XML documentation " +
                    $"configuration is invalid (was: '{documentationFile}', expected: '{expectedDocumentationFile}').");
        }
      }
    }
  }
}