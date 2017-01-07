using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.Build.Construction;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Utilities;

namespace SolutionInspector.ObjectModel
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

    public bool Equals ([CanBeNull] ProjectPropertyCondition other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return string.Equals(Self, other.Self) && string.Equals(Parent, other.Parent);
    }

    [CanBeNull]
    public string Self { get; }

    [CanBeNull]
    public string Parent { get; }

    private string ConvertCondition ([CanBeNull] string condition)
    {
      return string.IsNullOrWhiteSpace(condition) ? null : condition;
    }

    public override bool Equals ([CanBeNull] object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      return obj is ProjectPropertyCondition && Equals((ProjectPropertyCondition) obj);
    }

    [ExcludeFromCodeCoverage]
    public override int GetHashCode ()
    {
      return HashCodeHelper.GetHashCode(Self, Parent);
    }

    /// <inheritdoc />
    public static bool operator == (ProjectPropertyCondition left, ProjectPropertyCondition right)
    {
      return Equals(left, right);
    }

    /// <inheritdoc />
    public static bool operator != (ProjectPropertyCondition left, ProjectPropertyCondition right)
    {
      return !Equals(left, right);
    }
  }
}