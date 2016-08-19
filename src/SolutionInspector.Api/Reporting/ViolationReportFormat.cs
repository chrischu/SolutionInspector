namespace SolutionInspector.Api.Reporting
{
  /// <summary>
  ///   The format used for the violation report.
  /// </summary>
  public enum ViolationReportFormat
  {
    /// <summary>
    ///   Output violations as XML.
    /// </summary>
    Xml = 0,

    /// <summary>
    ///   Output violations as a formatted table.
    /// </summary>
    Table,

    /// <summary>
    ///   Output violations in such a format that violations will show up in the VisualStudio error list.
    /// </summary>
    VisualStudio
  }
}