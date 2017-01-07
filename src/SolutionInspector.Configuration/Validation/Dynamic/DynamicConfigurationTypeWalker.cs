using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration.Validation.Dynamic
{
  internal class DynamicConfigurationTypeWalker
  {
    public void Walk (Type configurationElementType, XElement element, IDynamicConfigurationVisitor visitor)
    {
      WalkInternal(configurationElementType, element, visitor, "");
    }

    private void WalkInternal (
      Type configurationElementType,
      XElement element,
      IDynamicConfigurationVisitor visitor,
      string propertyPath)
    {
      visitor.BeginTypeVisit(propertyPath, configurationElementType, element);

      var subTypesToWalk = new List<Tuple<string, Type, XElement>>();

      foreach (var property in configurationElementType.GetProperties())
      {
        var newPropertyPath = BuildPropertyPath(propertyPath, property);

        var configurationPropertyAttribute = property.GetCustomAttributes<ConfigurationPropertyAttribute>().ToArray();

        if (configurationPropertyAttribute.Length == 0)
          continue;

        var configurationValueAttribute = configurationPropertyAttribute[0] as ConfigurationValueAttribute;
        if (configurationValueAttribute != null)
        {
          XAttribute attribute;
          if (TryGetAttribute(element, configurationValueAttribute.GetXmlName(property.Name), out attribute))
            visitor.VisitValue(newPropertyPath, property, configurationValueAttribute, attribute);

          continue;
        }

        var configurationSubelementAttribute = configurationPropertyAttribute[0] as ConfigurationSubelementAttribute;
        if (configurationSubelementAttribute != null)
        {
          XElement subelement;
          if (TryGetSubelement(element, configurationSubelementAttribute.GetXmlName(property.Name), out subelement))
          {
            if (subelement != null)
              subTypesToWalk.Add(Tuple.Create(newPropertyPath, configurationSubelementAttribute.GetSubelementType(property), subelement));

            visitor.VisitSubelement(newPropertyPath, property, configurationSubelementAttribute, subelement);
          }

          continue;
        }

        var configurationCollectionAttribute = configurationPropertyAttribute[0] as ConfigurationCollectionAttribute;
        if (configurationCollectionAttribute != null)
        {
          XElement collectionElement;

          if (TryGetSubelement(element, configurationCollectionAttribute.GetXmlName(property.Name), out collectionElement))
          {
            ReadOnlyCollection<XElement> collectionItems;
            if (TryGetSubelements(collectionElement, configurationCollectionAttribute.ElementName, out collectionItems))
            {
              var collectionElementType = configurationCollectionAttribute.GetCollectionElementType(property);

              if (collectionElementType != null && collectionElement != null)
                for (var i = 0; i < collectionItems.Count; i++)
                  subTypesToWalk.Add(Tuple.Create($"{newPropertyPath}[{i}]", collectionElementType, collectionItems[i]));

              visitor.VisitCollection(newPropertyPath, property, configurationCollectionAttribute, collectionElement, collectionItems);
            }
          }
        }
      }

      visitor.EndTypeVisit(propertyPath, configurationElementType, element);

      foreach (var subTypeToWalk in subTypesToWalk)
        WalkInternal(subTypeToWalk.Item2, subTypeToWalk.Item3, visitor, subTypeToWalk.Item1);
    }

    private bool TryGetAttribute (XElement element, string attributeName, out XAttribute attribute)
    {
      try
      {
        attribute = element.Attribute(attributeName);
        return true;
      }
      catch (XmlException)
      {
        // Invalid name => no need to walk further
        attribute = null;
        return false;
      }
    }

    private bool TryGetSubelement (XElement element, string subelementName, out XElement subelement)
    {
      try
      {
        subelement = element.Element(subelementName);
        return true;
      }
      catch (XmlException)
      {
        // Invalid name => no need to walk further
        subelement = null;
        return false;
      }
    }

    private bool TryGetSubelements (
      [CanBeNull] XElement element,
      string subelementName,
      out ReadOnlyCollection<XElement> subelements)
    {
      try
      {
        subelements = element?.Elements(subelementName).ToList().AsReadOnly();
        return true;
      }
      catch (XmlException)
      {
        // Invalid name => no need to walk further
        subelements = null;
        return false;
      }
    }

    private string BuildPropertyPath (string previousPropertyPath, PropertyInfo property)
    {
      if (string.IsNullOrEmpty(previousPropertyPath))
        return property.Name;
      return previousPropertyPath + "." + property.Name;
    }
  }
}