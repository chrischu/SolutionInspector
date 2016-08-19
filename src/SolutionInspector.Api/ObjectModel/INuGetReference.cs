namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a DLL reference via NuGet.
  /// </summary>
  public interface INuGetReference : IDllReference
  {
    /// <summary>
    ///   The NuGet package that created the reference.
    /// </summary>
    INuGetPackage Package { get; }

    /// <summary>
    ///   <c>True</c> if the reference is private, <c>false</c> otherwise.
    /// </summary>
    bool IsPrivate { get; }
  }
}