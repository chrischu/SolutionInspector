namespace SolutionInspector.Reporting
{
  internal class RuleViolationViewModel
  {
    public RuleViolationViewModel (int index, string rule, string target, string message)
    {
      Index = index;
      Rule = rule;
      Target = target;
      Message = message;
    }

    public int Index { get; }
    public string Rule { get; }
    public string Target { get; }
    public string Message { get; }
  }
}