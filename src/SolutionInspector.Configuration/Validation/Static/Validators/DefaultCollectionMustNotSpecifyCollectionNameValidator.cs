using System.Reflection;
using SolutionInspector.Configuration.Attributes;

namespace SolutionInspector.Configuration.Validation.Static.Validators
{
  internal class DefaultCollectionMustNotSpecifyCollectionNameValidator : StaticConfigurationValidatorBase
  {
    public override void ValidateCollection (
      PropertyInfo property,
      ConfigurationCollectionAttribute attribute,
      ReportValidationError reportValidationError)
    {
      if (attribute.IsDefaultCollection && attribute.CollectionName != null)
        reportValidationError(property, "A configuration collection marked as the default collection must not specify a collection name.");
    }
  }
}