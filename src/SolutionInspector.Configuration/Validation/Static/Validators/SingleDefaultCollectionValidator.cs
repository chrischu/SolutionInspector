using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SolutionInspector.Configuration.Validation.Static.Validators
{
  internal class SingleDefaultCollectionValidator : StaticConfigurationValidatorBase
  {
    private readonly List<PropertyInfo> _defaultCollectionProperties = new List<PropertyInfo>();

    public override void BeginTypeValidation (Type configurationElementType, ReportValidationError reportValidationError)
    {
      _defaultCollectionProperties.Clear();
    }

    public override void ValidateCollection (
      PropertyInfo property,
      ConfigurationCollectionAttribute attribute,
      ReportValidationError reportValidationError)
    {
      if (attribute.IsDefaultCollection)
        _defaultCollectionProperties.Add(property);
    }

    public override void EndTypeValidation (Type configurationElementType, ReportValidationError reportValidationError)
    {
      if (_defaultCollectionProperties.Count > 1)
        foreach (var collectionProperty in _defaultCollectionProperties)
        {
          var otherDefaultCollectionProperties = string.Join(
            ", ",
            _defaultCollectionProperties.Where(p => p.Name != collectionProperty.Name).Select(p => $"'{p.Name}'"));

          reportValidationError(
            collectionProperty,
            "There can only be one default collection per configuration element and the following properties are already marked as " +
            $"default collection: {otherDefaultCollectionProperties}.");
        }
    }
  }
}