using System;

namespace SolutionInspector.Configuration.Collections
{
  /// <summary>
  ///   Represents a collection of <see cref="ConfigurationElement" />s with a dynamic type (in contrast to
  ///   <see cref="IConfigurationElementCollection{TElement}" />).
  /// </summary>
  public interface IDynamicConfigurationElementCollection<TElement> : IConfigurationElementCollectionBase<TElement>
      where TElement : ConfigurationElement
  {
  }
}