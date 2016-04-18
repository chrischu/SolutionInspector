namespace SolutionInspector.Rules
{
  /// <summary>
  /// The target of a <see cref="IRule"/>.
  /// </summary>
  public interface IRuleTarget
  {
    /// <summary>
    /// Identifies the <see cref="IRuleTarget"/>.
    /// </summary>
    string Identifier { get; }
  }
}