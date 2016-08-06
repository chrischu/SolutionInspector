using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.Build.Construction;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Utilities;

namespace SolutionInspector.ObjectModel
{
  [DebuggerDisplay ("{DebuggerDisplay,nq}")]
  internal sealed class ProjectPropertyCondition : IProjectPropertyCondition, IEquatable<ProjectPropertyCondition>
  {
    [CanBeNull]
    public string Self { get; }

    [CanBeNull]
    public string Parent { get; }

    public ProjectPropertyCondition ([CanBeNull] string self, [CanBeNull] string parent)
    {
      Self = ConvertCondition(self);
      Parent = ConvertCondition(parent);
    }

    public ProjectPropertyCondition (ProjectPropertyElement element)
        : this(element.Condition, element.Parent?.Condition)
    {
    }

    private string ConvertCondition ([CanBeNull] string condition)
    {
      return string.IsNullOrWhiteSpace(condition) ? null : condition;
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

    public static bool operator == (ProjectPropertyCondition left, ProjectPropertyCondition right)
    {
      return Equals(left, right);
    }

    public static bool operator != (ProjectPropertyCondition left, ProjectPropertyCondition right)
    {
      return !Equals(left, right);
    }
  }
}