using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.Configuration.Validation.Static.Validators
{
  internal class XmlNameUniquenessValidator : StaticConfigurationValidatorBase
  {
    private readonly Dictionary<string, List<PropertyInfo>> _xmlElementNamesWithProperties = new Dictionary<string, List<PropertyInfo>>();
    private readonly Dictionary<string, List<PropertyInfo>> _xmlAttributeNamesWithProperties = new Dictionary<string, List<PropertyInfo>>();

    public override void BeginTypeValidation (Type configurationElementType, ReportValidationError reportValidationError)
    {
      _xmlElementNamesWithProperties.Clear();
      _xmlAttributeNamesWithProperties.Clear();
    }

    public override void ValidateValue (PropertyInfo property, ConfigurationValueAttribute attribute, ReportValidationError reportValidationError)
    {
      if (attribute.XmlName != null)
      {
        var list = _xmlAttributeNamesWithProperties.GetOrAdd(attribute.XmlName, key => new List<PropertyInfo>());
        list.Add(property);
      }
    }

    public override void ValidateSubelement (
      PropertyInfo property,
      ConfigurationSubelementAttribute attribute,
      ReportValidationError reportValidationError)
    {
      if (attribute.XmlName != null)
      {
        var list = _xmlElementNamesWithProperties.GetOrAdd(attribute.XmlName, key => new List<PropertyInfo>());
        list.Add(property);
      }
    }

    public override void ValidateCollection (
      PropertyInfo property,
      ConfigurationCollectionAttribute attribute,
      ReportValidationError reportValidationError)
    {
      if (attribute.XmlName != null)
      {
        var list = _xmlElementNamesWithProperties.GetOrAdd(attribute.XmlName, key => new List<PropertyInfo>());
        list.Add(property);
      }
    }

    public override void EndTypeValidation (Type configurationElementType, ReportValidationError reportValidationError)
    {
      var xmlElementNamesWithMultipleProperties = _xmlElementNamesWithProperties.Where(kvp => kvp.Value.Count > 1);

      foreach (var xmlElementNameWithMultipleProperties in xmlElementNamesWithMultipleProperties)
      {
        foreach (var property in xmlElementNameWithMultipleProperties.Value)
        {
          var properties = string.Join(
            ", ",
            xmlElementNameWithMultipleProperties.Value.Where(p => p.Name != property.Name).Select(p => $"'{p.Name}'"));
          reportValidationError(
            property,
            $"The XML element name '{xmlElementNameWithMultipleProperties.Key}' is not unique (it is duplicated in {properties}).");
        }
      }

      var xmlAttributeNamesWithMultipleProperties = _xmlAttributeNamesWithProperties.Where(kvp => kvp.Value.Count > 1);

      foreach (var xmlAttributeNameWithMultipleProperties in xmlAttributeNamesWithMultipleProperties)
      {
        foreach (var property in xmlAttributeNameWithMultipleProperties.Value)
        {
          var properties = string.Join(
            ", ",
            xmlAttributeNameWithMultipleProperties.Value.Where(p => p.Name != property.Name).Select(p => $"'{p.Name}'"));
          reportValidationError(
            property,
            $"The XML attribute name '{xmlAttributeNameWithMultipleProperties.Key}' is not unique (it is duplicated in {properties}).");
        }
      }
    }
  }
}