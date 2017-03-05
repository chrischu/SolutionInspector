namespace SolutionInspector.ConfigurationUi.Features.Undo.Actions
{
  internal interface IDoableAction
  {
    IUndoableAction Do ();
  }
}