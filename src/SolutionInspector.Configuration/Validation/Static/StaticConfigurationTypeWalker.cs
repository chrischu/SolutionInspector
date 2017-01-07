using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SolutionInspector.Configuration.Validation.Static
{
  internal class StaticConfigurationTypeWalker
  {
    public void Walk (Type configurationElementType, IStaticConfigurationVisitor visitor)
    {
      WalkInternal(configurationElementType, visitor, "", new HashSet<Type> { configurationElementType });
    }

    private void WalkInternal (Type configurationElementType, IStaticConfigurationVisitor visitor, string propertyPath, HashSet<Type> typeHierarchy)
    {
      visitor.BeginTypeVisit(propertyPath, configurationElementType);

      var subTypesToWalk = new List<Tuple<string, Type>>();

      foreach (var property in configurationElementType.GetProperties())
      {
        var newPropertyPath = BuildPropertyPath(propertyPath, property);

        var configurationPropertyAttribute = property.GetCustomAttributes<ConfigurationPropertyAttribute>().ToArray();

        if (configurationPropertyAttribute.Length == 0)
          continue;

        var configurationValueAttribute = configurationPropertyAttribute[0] as ConfigurationValueAttribute;
        if (configurationValueAttribute != null)
        {
          visitor.VisitValue(newPropertyPath, property, configurationValueAttribute);
          continue;
        }

        var configurationSubelementAttribute = configurationPropertyAttribute[0] as ConfigurationSubelementAttribute;
        if (configurationSubelementAttribute != null)
        {
          subTypesToWalk.Add(Tuple.Create(newPropertyPath, configurationSubelementAttribute.GetSubelementType(property)));
          visitor.VisitSubelement(newPropertyPath, property, configurationSubelementAttribute);
          continue;
        }

        var configurationCollectionAttribute = configurationPropertyAttribute[0] as ConfigurationCollectionAttribute;
        if (configurationCollectionAttribute != null)
        {
          var collectionElementType = configurationCollectionAttribute.GetCollectionElementType(property);
          if (collectionElementType != null)
            subTypesToWalk.Add(Tuple.Create(newPropertyPath, collectionElementType));

          visitor.VisitCollection(newPropertyPath, property, configurationCollectionAttribute);
        }
      }

      visitor.EndTypeVisit(propertyPath, configurationElementType);

      foreach (var subTypeToWalk in subTypesToWalk.Where(t => !typeHierarchy.Contains(t.Item2)))
        WalkInternal(subTypeToWalk.Item2, visitor, subTypeToWalk.Item1, new HashSet<Type>(typeHierarchy) { configurationElementType });
    }

    private string BuildPropertyPath (string previousPropertyPath, PropertyInfo property)
    {
      if (string.IsNullOrEmpty(previousPropertyPath))
        return property.Name;
      return previousPropertyPath + "." + property.Name;
    }
  }
}