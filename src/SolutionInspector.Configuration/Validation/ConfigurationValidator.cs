using System;
using System.Collections.Generic;
using System.Linq;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration.Validation.Dynamic;
using SolutionInspector.Configuration.Validation.Static;

namespace SolutionInspector.Configuration.Validation
{
  internal static class ConfigurationValidator
  {
    private static readonly Lazy<List<IStaticConfigurationValidator>> s_staticConfigurationValidators;
    private static readonly Lazy<List<IDynamicConfigurationValidator>> s_dynamicConfigurationValidators;

    static ConfigurationValidator ()
    {
      s_staticConfigurationValidators = new Lazy<List<IStaticConfigurationValidator>>(GetConfigurationValidators<IStaticConfigurationValidator>);
      s_dynamicConfigurationValidators = new Lazy<List<IDynamicConfigurationValidator>>(GetConfigurationValidators<IDynamicConfigurationValidator>);
    }

    public static List<IStaticConfigurationValidator> StaticConfigurationValidators => s_staticConfigurationValidators.Value;
    public static List<IDynamicConfigurationValidator> DynamicConfigurationValidators => s_dynamicConfigurationValidators.Value;

    public static void Validate (ConfigurationBase configuration)
    {
      var validationErrorCollector = new ConfigurationValidationErrorCollector();

      var compositeValidator = new ValidatingConfigurationVisitor(
                                 validationErrorCollector,
                                 StaticConfigurationValidators,
                                 DynamicConfigurationValidators);

      var staticWalker = new StaticConfigurationTypeWalker();
      staticWalker.Walk(configuration.GetType(), compositeValidator);

      var dynamicWalker = new DynamicConfigurationTypeWalker();
      dynamicWalker.Walk(configuration.GetType(), configuration.Element, compositeValidator);

      if (validationErrorCollector.ValidationErrors.Count > 0)
        throw new ConfigurationValidationException(validationErrorCollector.ValidationErrors);
    }

    private static List<T> GetConfigurationValidators<T>()
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