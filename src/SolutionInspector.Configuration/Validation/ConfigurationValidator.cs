using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration.Validation.Dynamic;
using SolutionInspector.Configuration.Validation.Static;

namespace SolutionInspector.Configuration.Validation
{
  /// <summary>
  ///   Utility class to validate configurations.
  /// </summary>
  public static class ConfigurationValidator
  {
    private static readonly Lazy<List<IStaticConfigurationValidator>> s_staticConfigurationValidators;
    private static readonly Lazy<List<IDynamicConfigurationValidator>> s_dynamicConfigurationValidators;

    static ConfigurationValidator ()
    {
      s_staticConfigurationValidators = new Lazy<List<IStaticConfigurationValidator>>(GetConfigurationValidators<IStaticConfigurationValidator>);
      s_dynamicConfigurationValidators = new Lazy<List<IDynamicConfigurationValidator>>(GetConfigurationValidators<IDynamicConfigurationValidator>);
    }

    private static List<IStaticConfigurationValidator> StaticConfigurationValidators => s_staticConfigurationValidators.Value;
    private static List<IDynamicConfigurationValidator> DynamicConfigurationValidators => s_dynamicConfigurationValidators.Value;

    public static void Validate (Type configurationType)
    {
      if (!typeof(ConfigurationBase).IsAssignableFrom(configurationType))
        throw new ArgumentException($"The given type '{configurationType}' does not derive from {typeof(ConfigurationBase)}.", nameof(configurationType));

      ValidateInternal(configurationType, null);
    }

    public static void Validate (ConfigurationBase configuration)
    {
      ValidateInternal(configuration.GetType(), configuration);
    }

    private static void ValidateInternal (Type configurationType, [CanBeNull] ConfigurationBase configuration)
    {
      var validationErrorCollector = new ConfigurationValidationErrorCollector();

      var compositeValidator = new ValidatingConfigurationVisitor(
          validationErrorCollector,
          StaticConfigurationValidators,
          DynamicConfigurationValidators);

      var staticWalker = new StaticConfigurationTypeWalker();
      staticWalker.Walk(configurationType, compositeValidator);

      if (configuration != null)
      {
        var dynamicWalker = new DynamicConfigurationTypeWalker();
        dynamicWalker.Walk(configuration.GetType(), configuration.Element, compositeValidator);
      }

      if (validationErrorCollector.ValidationErrors.Count > 0)
        throw new ConfigurationValidationException(validationErrorCollector.ValidationErrors);
    }

    private static List<T> GetConfigurationValidators<T> ()
    {
      var validatorType = typeof(T);
      var collection = typeof(ConfigurationValidator).Assembly.GetTypes()
          .Where(t => validatorType.IsAssignableFrom(t))
          .Where(t => t.IsClass && !t.IsAbstract)
          .Select(Activator.CreateInstance)
          .Cast<T>();
      return new List<T>(collection);
    }

    private class ConfigurationValidationErrorCollector : IConfigurationValidationErrorCollector
    {
      private readonly Dictionary<string, List<string>> _validationErrors = new Dictionary<string, List<string>>();

      public IReadOnlyDictionary<string, IReadOnlyCollection<string>> ValidationErrors
        => _validationErrors.ToDictionary(kvp => kvp.Key, kvp => (IReadOnlyCollection<string>) kvp.Value);

      public void AddError (string propertyPath, string message)
      {
        var propertyErrors = _validationErrors.GetOrAdd(propertyPath, s => new List<string>());
        propertyErrors.Add(message);
      }
    }
  }
}