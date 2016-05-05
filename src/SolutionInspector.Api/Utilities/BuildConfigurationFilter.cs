using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Api.Utilities
{
  /// <summary>
  ///   Filter for build configurations (e.g. Debug/AnyCPU).
  /// </summary>
  public class BuildConfigurationFilter
  {
    private readonly BuildConfiguration[] _filters;

    /// <summary>
    ///   Creates a <see cref="BuildConfigurationFilter" /> from the given <see cref="BuildConfiguration" />s.
    /// </summary>
    public BuildConfigurationFilter (IEnumerable<BuildConfiguration> filters)
        : this(filters.ToArray())
    {
    }

    /// <summary>
    ///   Creates a <see cref="BuildConfigurationFilter" /> from the given <see cref="BuildConfiguration" />s.
    /// </summary>
    public BuildConfigurationFilter (params BuildConfiguration[] filters)
    {
      _filters = filters.ToArray();
    }

    /// <summary>
    ///   Returns <c>true</c> if the given <paramref name="buildConfiguration" /> matches the filter, <c>false</c> otherwise.
    /// </summary>
    public bool IsMatch (BuildConfiguration buildConfiguration)
    {
      var comparer = new BuildConfigurationFilterEqualityComparer();
      return _filters.Any(f => comparer.Equals(f, buildConfiguration));
    }

    private class BuildConfigurationFilterEqualityComparer : IEqualityComparer<BuildConfiguration>
    {
      public bool Equals ([NotNull] BuildConfiguration x, [NotNull] BuildConfiguration y)
      {
        return (x.ConfigurationName == y.ConfigurationName || x.ConfigurationName == "*" || y.ConfigurationName == "*") &&
               (x.PlatformName == y.PlatformName || x.PlatformName == "*" || y.PlatformName == "*");
      }

      public int GetHashCode ([NotNull] BuildConfiguration obj)
      {
        return obj.GetHashCode();
      }
    }

    /// <inheritdoc />
    public override string ToString ()
    {
      return string.Join(",", _filters.Select(f => f.ToString()));
    }
  }
}