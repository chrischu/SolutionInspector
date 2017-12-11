using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration.Validation.Dynamic
{
  internal interface IDynamicConfigurationVisitor
  {
    void BeginTypeVisit (string propertyPath, Type configurationElementType, XElement element);

    void VisitValue (string propertyPath, PropertyInfo property, ConfigurationValueAttribute attribute, [CanBeNull] XAttribute xAttribute);
    void VisitSubelement (string propertyPath, PropertyInfo property, ConfigurationSubelementAttribute attribute, [CanBeNull] XElement subelement);

    void VisitCollection (
      string propertyPath,
      PropertyInfo property,
      ConfigurationCollectionAttribute attribute,
      [CanBeNull] XElement collectionElement,
      [CanBeNull] IReadOnlyCollection<XElement> collectionItems);

    void EndTypeVisit (string propertyPath, Type configurationElementType, XElement element);
  }
}