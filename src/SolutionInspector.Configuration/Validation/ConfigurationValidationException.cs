using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration.Validation
{
  /// <summary>
  ///   Represents errors that occur when validating a configuration.
  /// </summary>
  [Serializable]
  public class ConfigurationValidationException : Exception
  {
    /// <summary>
    ///   All validation errors by property path.
    /// </summary>
    [PublicAPI]
    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> ValidationErrors { get; }

    /// <summary>
    ///   Creates a new <see cref="ConfigurationValidationException" />
    /// </summary>
    public ConfigurationValidationException (IReadOnlyDictionary<string, IReadOnlyCollection<string>> validationErrors)
      : base(FormatValidationErrorMessage(validationErrors))
    {
      ValidationErrors = validationErrors;
    }

    private static string FormatValidationErrorMessage (IReadOnlyDictionary<string, IReadOnlyCollection<string>> validationErrors)
    {
      var properties = string.Join(
        Environment.NewLine,
        validationErrors.Select(
          e =>
          {
            var messages = string.Join(Environment.NewLine, e.Value.Select(s => $"    - {s}"));
            return $"  - For property '{e.Key}':{Environment.NewLine}{messages}";
          }));

      return $"Validation failed because of the following errors:{Environment.NewLine}{properties}";
    }

    /// <summary>
    ///   Serialization constructor.
    /// </summary>
    [ExcludeFromCodeCoverage /* Serialization ctor */]
    protected ConfigurationValidationException (SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}