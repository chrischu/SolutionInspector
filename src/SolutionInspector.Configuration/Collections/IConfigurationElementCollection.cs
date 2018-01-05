using System;

namespace SolutionInspector.Configuration.Collections
{
  /// <summary>
  ///   Represents a collection of <see cref="ConfigurationElement" />s with a fixed type (in contrast to
  ///   <see cref="IDynamicConfigurationElementCollection{TElement}" />).
  /// </summary>
  public interface IConfigurationElementCollection<TElement> : IConfigurationElementCollectionBase<TElement>
      where TElement : ConfigurationElement, new()
  {
    TElement AddNew ();
  }
}