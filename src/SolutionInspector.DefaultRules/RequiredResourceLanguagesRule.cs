using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Checks that all given resource files are localized in all given languages in the project.
  /// </summary>
  public class RequiredResourceLanguagesRule : ConfigurableProjectRule<RequiredResourceLanguagesRuleConfiguration>
  {
    /// <inheritdoc />
    public RequiredResourceLanguagesRule (RequiredResourceLanguagesRuleConfiguration configuration)
        : base(configuration)
    {
    }

    public override IEnumerable<IRuleViolation> Evaluate (IProject target)
    {
      foreach (var resourceName in Configuration.RequiredResources)
      {
        var resourceDefaultLanguageFileName = $"{resourceName}.resx";
        if (target.ProjectItems.All(i => i.Name != resourceDefaultLanguageFileName))
          yield return
              new RuleViolation(
                  this,
                  target,
                  $"For the required resource '{resourceName}' no default resource file ('{resourceDefaultLanguageFileName}') could be found.");

        foreach (var languageName in Configuration.RequiredLanguages)
        {
          var resourceLanguageFileName = $"{resourceName}.{languageName}.resx";
          if (target.ProjectItems.All(i => i.Name != resourceLanguageFileName))
            yield return
                new RuleViolation(
                    this,
                    target,
                    $"For the required resource '{resourceName}' no resource file for language '{languageName}' " +
                    $"('{resourceLanguageFileName}') could be found.")
                ;
        }
      }
    }
  }

  /// <summary>
  ///   Configuration for the <see cref="RequiredResourceLanguagesRule" />.
  /// </summary>
  public class RequiredResourceLanguagesRuleConfiguration : ConfigurationElement
  {
    /// <summary>
    ///   Comma-separated list of required resources (e.g. "Resources,OtherResources").
    /// </summary>
    [TypeConverter (typeof(CommaDelimitedStringCollectionConverter))]
    [ConfigurationProperty ("requiredResources", DefaultValue = "", IsRequired = true)]
    public CommaDelimitedStringCollection RequiredResources
    {
      get { return (CommaDelimitedStringCollection) this["requiredResources"]; }
      set { this["requiredResources"] = value; }
    }

    /// <summary>
    ///   Comma-separated list of required languages (e.g. "de,pl,hr").
    /// </summary>
    [TypeConverter (typeof(CommaDelimitedStringCollectionConverter))]
    [ConfigurationProperty ("requiredLanguages", DefaultValue = "", IsRequired = true)]
    public CommaDelimitedStringCollection RequiredLanguages
    {
      get { return (CommaDelimitedStringCollection) this["requiredLanguages"]; }
      set { this["requiredLanguages"] = value; }
    }
  }
}