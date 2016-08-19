namespace SolutionInspector.Configuration.Validation
{
  internal interface IConfigurationValidationErrorCollector
  {
    void AddError (string propertyPath, string message);
  }
}