using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.Build.Construction;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Commons.Utilities;

namespace SolutionInspector.Internals.ObjectModel
{
  /// <inheritdoc cref="IProjectPropertyCondition" />
  [DebuggerDisplay ("{" + nameof(DebuggerDisplay) + ",nq}")]
  public sealed class ProjectPropertyCondition : IProjectPropertyCondition, IEquatable<ProjectPropertyCondition>
  {
    internal ProjectPropertyCondition ([CanBeNull] string self, [CanBeNull] string parent)
    {
      Self = ConvertCondition(self);
      Parent = ConvertCondition(parent);
    }

    internal ProjectPropertyCondition (ProjectPropertyElement element)
      : this(element.Condition, element.Parent?.Condition)
    {
    }

    [ExcludeFromCodeCoverage]
    private string DebuggerDisplay
    {
      get
      {
        if (Self == null)
          return Parent;

        if (Parent == null)
          return Self;

        return $"{Parent} AND {Self}";
      }
    }

    /// <inheritdoc />
    public bool Equals([CanBeNull] ProjectPropertyCondition other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return string.Equals(Self, other.Self) && string.Equals(Parent, other.Parent);
    }

    /// <inheritdoc />
    [CanBeNull]
    public string Self { get; }

    /// <inheritdoc />
    [CanBeNull]
    public string Parent { get; }

    private string ConvertCondition ([CanBeNull] string condition)
    {
      return string.IsNullOrWhiteSpace(condition) ? null : condition;
    }

    /// <inheritdoc />
    public override bool Equals ([CanBeNull] object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      return obj is ProjectPropertyCondition && Equals((ProjectPropertyCondition) obj);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override int GetHashCode ()
    {
      return HashCodeHelper.GetHashCode(Self, Parent);
    }

    /// <summary>
    ///   Compares two <see cref="ProjectPropertyCondition" />s by using <see cref="Equals(object)" /> and returns <see langword="true" /> if
    ///   they are equal, <see langword="false" /> otherwise.
    /// </summary>
    public static bool operator == (ProjectPropertyCondition left, ProjectPropertyCondition right)
    {
      return Equals(left, right);
    }

    /// <summary>
    ///   Compares two <see cref="ProjectPropertyCondition" />s by using <see cref="Equals(object)" /> and returns <see langword="false" /> if
    ///   they are equal, <see langword="true" /> otherwise.
    /// </summary>
    public static bool operator != (ProjectPropertyCondition left, ProjectPropertyCondition right)
    {
      return !Equals(left, right);
    }
  }
}