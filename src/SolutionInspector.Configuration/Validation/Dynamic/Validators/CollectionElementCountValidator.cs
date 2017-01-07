using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration.Validation.Dynamic.Validators
{
  internal class CollectionElementCountValidator : DynamicConfigurationValidatorBase
  {
    public override void ValidateCollection (
      PropertyInfo property,
      ConfigurationCollectionAttribute attribute,
      [CanBeNull] XElement collectionElement,
      [CanBeNull] IReadOnlyCollection<XElement> collectionItems,
      ReportValidationError reportValidationError)
    {
      if (collectionElement == null)
        return;

      var collectionItemCount = collectionItems?.Count ?? 0;

      if (collectionItemCount < attribute.MinimumElementCount)
        reportValidationError(
          property,
          $"The collection needs to contain at least {attribute.MinimumElementCount} element, but contains {collectionItemCount}.");

      if (collectionItemCount > attribute.MaximumElementCount)
        reportValidationError(
          property,
          $"The collection needs to contain at most {attribute.MaximumElementCount} element, but contains {collectionItemCount}.");
    }
  }
}