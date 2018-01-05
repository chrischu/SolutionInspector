using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using SolutionInspector.Commons.Attributes;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.Configuration.Validation
{
  /// <summary>
  ///   Represents errors that occur when validating a configuration.
  /// </summary>
  [SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly")]
  [Serializable]
  public class ConfigurationValidationException : Exception
  {
    /// <summary>
    ///   Creates a new <see cref="ConfigurationValidationException" />
    /// </summary>
    public ConfigurationValidationException (IReadOnlyDictionary<string, IReadOnlyCollection<string>> validationErrors)
      : base(FormatValidationErrorMessage(validationErrors))
    {
      ValidationErrors = validationErrors;
    }

    /// <summary>
    ///   Serialization constructor.
    /// </summary>
    [ExcludeFromCodeCoverage /* Serialization ctor */]
    // ReSharper disable once NotNullMemberIsNotInitialized
    protected ConfigurationValidationException (SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    /// <summary>
    ///   All validation errors by property path.
    /// </summary>
    [PublicApi]
    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> ValidationErrors { get; }

    private static string FormatValidationErrorMessage (IReadOnlyDictionary<string, IReadOnlyCollection<string>> validationErrors)
    {
      var properties = validationErrors.ConvertAndJoin(
          e =>
          {
            var messages = EnumerableExtensions.ConvertAndJoin(e.Value, s => $"    - {s}", Environment.NewLine);
            return $"  - For property '{e.Key}':{Environment.NewLine}{messages}";
          },
          Environment.NewLine);

      return $"Validation failed because of the following errors:{Environment.NewLine}{properties}";
    }
  }
}