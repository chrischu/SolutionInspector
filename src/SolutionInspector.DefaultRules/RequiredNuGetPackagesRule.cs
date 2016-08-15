using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that the project references all the NuGet packages configured via <see cref="RequiredNuGetPackagesRuleConfiguration" />.
  /// </summary>
  [Description("Verifies that the project references all the configured NuGet packages.")]
  public class RequiredNuGetPackagesRule : ConfigurableProjectRule<RequiredNuGetPackagesRuleConfiguration>
  {
    /// <inheritdoc />
    public RequiredNuGetPackagesRule ([NotNull] RequiredNuGetPackagesRuleConfiguration configuration)
        : base(configuration)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate (IProject target)
    {
      return from requiredNuGetPackage in Configuration
        where target.NuGetPackages.All(p => p.Id != requiredNuGetPackage.Id)
        select new RuleViolation(this, target, $"Required NuGet package '{requiredNuGetPackage.Id}' is missing.");
    }
  }

  /// <summary>
  ///   Configuration for the <see cref="RequiredNuGetPackagesRule" />.
  /// </summary>
  public class RequiredNuGetPackagesRuleConfiguration : KeyedConfigurationElementCollectionBase<RequiredNuGetPackageConfigurationElement, string>
  {
    /// <inheritdoc />
    protected override string ElementName => "nuGetPackage"; // TODO
  }

  /// <summary>
  ///   Configuration element that represents a required NuGet package.
  /// </summary>
  public class RequiredNuGetPackageConfigurationElement : KeyedConfigurationElement<string>
  {
    /// <inheritdoc />
    public override string KeyName => "id";

    /// <summary>
    ///   The id of the required NuGet package.
    /// </summary>
    [ConfigurationProperty ("id", DefaultValue = "", IsRequired = true, IsKey = true)]
    [Description("The id of the required NuGet package.")]
    public string Id
    {
      get { return (string) this["id"]; }
      set { this["id"] = value; }
    }
  }
}