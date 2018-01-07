using SolutionInspector.Commons.Attributes;

namespace SolutionInspector.Configuration.Validation
{
  internal interface IConfigurationValidationErrorCollector
  {
    [ForFutureUse]
    void AddDocumentError (string message);
    void AddPropertyError (string propertyPath, string message);
  }
}