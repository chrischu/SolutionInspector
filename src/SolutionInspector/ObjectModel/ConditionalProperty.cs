using JetBrains.Annotations;

namespace SolutionInspector.ObjectModel
{
  /// <summary>
  /// Represents a MSBuild project property that is dependent on some <see cref="Condition"/>.
  /// </summary>
  [PublicAPI]
  public class ConditionalProperty
  {
    /// <summary>
    /// The value of the property.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// The condition that controlls if the property is active.
    /// </summary>
    public string Condition { get; }

    /// <summary>
    /// Creates new <see cref="ConditionalProperty"/>.
    /// </summary>
    public ConditionalProperty(string value, string condition)
    {
      Value = value;
      Condition = condition;
    }
  }
}