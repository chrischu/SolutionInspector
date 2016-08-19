using System;
using System.Reflection;
using System.Xml;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration.Validation.Static.Validators
{
  internal class XmlNameValidator : StaticConfigurationValidatorBase
  {
    public override void ValidateValue (PropertyInfo property, ConfigurationValueAttribute attribute, ReportValidationError reportValidationError)
    {
      ValidateXmlName(property, attribute.XmlName, reportValidationError);
    }

    public override void ValidateSubelement (
      PropertyInfo property,
      ConfigurationSubelementAttribute attribute,
      ReportValidationError reportValidationError)
    {
      ValidateXmlName(property, attribute.XmlName, reportValidationError);
    }

    public override void ValidateCollection (
      PropertyInfo property,
      ConfigurationCollectionAttribute attribute,
      ReportValidationError reportValidationError)
    {
      ValidateXmlName(property, attribute.XmlName, reportValidationError);
      ValidateXmlName(property, attribute.ElementName, reportValidationError);
    }

    private void ValidateXmlName (PropertyInfo property, [CanBeNull] string xmlName, ReportValidationError reportValidationError)
    {
      if (xmlName != null && !VerifyXmlName(xmlName))
        reportValidationError(property, $"'{xmlName}' is not a valid XML name.");
    }

    private bool VerifyXmlName (string xmlName)
    {
      try
      {
        XmlConvert.VerifyName(xmlName);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
}