namespace SolutionInspector.Configuration.Validation
{
  internal interface IConfigurationValidationErrorCollector
  {
    bool HasErrors { get; }

    void AddDocumentError (string message);
    void AddPropertyError (string propertyPath, string message);
  }
}