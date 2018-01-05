using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration.Collections
{
  /// <summary>
  ///   Represents a collection of <see cref="ConfigurationElement" />s.
  /// </summary>
  public interface IConfigurationElementCollectionBase<TElement> : IList<TElement>, IReadOnlyList<TElement>
      where TElement : ConfigurationElement
  {
    new int Count { get; }

    [PublicAPI]
    new TElement this [int index] { get; set; }
  }
}