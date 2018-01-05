using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SolutionInspector.Configuration.Dynamic;

namespace SolutionInspector.Configuration.Collections
{
  internal class DynamicConfigurationElementCollection<TElement>
      : ConfigurationElementCollectionBase<TElement>, IDynamicConfigurationElementCollection<TElement>
      where TElement : ConfigurationElement
  {
    public DynamicConfigurationElementCollection (XElement collectionElement, IEnumerable<TElement> collectionItems = null)
        : base(collectionElement, collectionItems ?? Enumerable.Empty<TElement>())
    {
    }

    protected override void ValidateNewElement (TElement element)
    {
      try
      {
        ConfigurationBase.DynamicConfigurationElementTypeHelper.ValidateElementCompatibility(ContainingDocument, element);
      }catch(DynamicElementTypeCompatibilityException ex)
      {
        throw new ArgumentException("The given element is not compatible with this collection (see inner exception for details).", nameof(element), ex);
      }
    }
  }
}