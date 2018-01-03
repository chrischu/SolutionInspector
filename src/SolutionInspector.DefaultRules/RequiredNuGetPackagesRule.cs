using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Configuration;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that the project references all the NuGet packages configured via <see cref="RequiredNuGetPackages" />.
  /// </summary>
  [Description("Verifies that the project references all the configured NuGet packages.")]
  public class RequiredNuGetPackagesRule : ProjectRule
  {
    /// <summary>
    ///   All the required NuGet package ids (e.g. 'Autofac').
    /// </summary>
    [ConfigurationValue]
    [Description("All the required NuGet package ids (e.g. 'Autofac').")]
    public CommaSeparatedStringCollection RequiredNuGetPackages => GetConfigurationValue<CommaSeparatedStringCollection>();

    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate ([NotNull] IProject target)
    {
      foreach (var requiredNuGetPackageId in RequiredNuGetPackages)
        if (target.NuGetPackages.All(p => p.Id != requiredNuGetPackageId))
          yield return new RuleViolation(this, target, $"Required NuGet package '{requiredNuGetPackageId}' is missing.");
    }
  }
}