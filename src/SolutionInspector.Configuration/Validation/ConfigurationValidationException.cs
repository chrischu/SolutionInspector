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
    public ConfigurationValidationException (
        IReadOnlyCollection<string> documentValidationErrors,
        IReadOnlyDictionary<string, IReadOnlyCollection<string>> propertyValidationErrors)
        : base(FormatValidationErrorMessage(documentValidationErrors, propertyValidationErrors))
    {
      DocumentValidationErrors = documentValidationErrors;
      PropertyValidationErrors = propertyValidationErrors;
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
    ///   All document validation errors.
    /// </summary>
    public IReadOnlyCollection<string> DocumentValidationErrors { get; }

    /// <summary>
    ///   All property validation errors by property path.
    /// </summary>
    [PublicApi]
    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> PropertyValidationErrors { get; }

    private static string FormatValidationErrorMessage (
        IReadOnlyCollection<string> documentValidationErrors,
        IReadOnlyDictionary<string, IReadOnlyCollection<string>> validationErrors)
    {
      var documentMessages = EnumerableExtensions.ConvertAndJoin(documentValidationErrors, s => $"    - {s}", Environment.NewLine);
      var documentErrors = $"  - For the document:{Environment.NewLine}{documentMessages}";

      var propertyErrors = validationErrors.ConvertAndJoin(
          e =>
          {
            var messages = EnumerableExtensions.ConvertAndJoin(e.Value, s => $"    - {s}", Environment.NewLine);
            return $"  - For property '{e.Key}':{Environment.NewLine}{messages}";
          },
          Environment.NewLine);

      return $"Validation failed because of the following errors:{Environment.NewLine}{documentErrors}{Environment.NewLine}{propertyErrors}";
    }
  }
}