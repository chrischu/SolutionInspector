using System;
using System.Reflection;
using SolutionInspector.Configuration.Attributes;

namespace SolutionInspector.Configuration.Validation.Static
{
  internal interface IStaticConfigurationVisitor
  {
    void BeginTypeVisit (string propertyPath, Type configurationElementType);

    void VisitValue (string propertyPath, PropertyInfo property, ConfigurationValueAttribute attribute);
    void VisitSubelement (string propertyPath, PropertyInfo property, ConfigurationSubelementAttribute attribute);
    void VisitCollection (string propertyPath, PropertyInfo property, ConfigurationCollectionAttribute attribute);

    void EndTypeVisit (string propertyPath, Type configurationElementType);
  }
}